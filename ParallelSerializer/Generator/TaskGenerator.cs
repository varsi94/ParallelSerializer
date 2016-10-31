using DemoModel;
using DynamicSerializer.Core;
using DynamicSerializer.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParallelSerializer.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelSerializer.Generator
{
    public static class TaskGenerator
    {
        internal static object TaskGenerationSyncRoot { get; } = new object();

        private static ExpressionSyntax GetArgumentSyntaxFromMember(SerializableMember member)
        {
            return SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName("Object"),
                SyntaxFactory.IdentifierName(member.Name));
        }

        private static StatementSyntax SerializeAtomicType(ExpressionSyntax expr, Type type)
        {
            var writeExpression = SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName(ConstantsForGeneration.BinaryWriterName),
                            SyntaxFactory.IdentifierName("Write")))
                        .WithArgumentList(
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                    SyntaxFactory.Argument(expr)))));
            if (type == typeof(string))
            {
                return
                    SyntaxFactory.IfStatement(
                        SyntaxFactory.BinaryExpression(
                            SyntaxKind.EqualsExpression,
                            expr,
                            SyntaxFactory.LiteralExpression(
                                SyntaxKind.NullLiteralExpression)),
                        SyntaxFactory.Block(
                            SyntaxFactory.SingletonList<StatementSyntax>(
                                SyntaxFactory.ExpressionStatement(
                                    SyntaxFactory.InvocationExpression(
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.IdentifierName(ConstantsForGeneration.BinaryWriterName),
                                            SyntaxFactory.IdentifierName("Write")))
                                        .WithArgumentList(
                                            SyntaxFactory.ArgumentList(
                                                SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                                    SyntaxFactory.Argument(
                                                        SyntaxFactory.PrefixUnaryExpression(
                                                            SyntaxKind.UnaryMinusExpression,
                                                            SyntaxFactory.LiteralExpression(
                                                                SyntaxKind.NumericLiteralExpression,
                                                                SyntaxFactory.Literal(1)))))))))))
                        .WithElse(
                            SyntaxFactory.ElseClause(SyntaxFactory.Block(
                                        SyntaxFactory.ExpressionStatement(
                                            SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.IdentifierName(ConstantsForGeneration.BinaryWriterName),
                                            SyntaxFactory.IdentifierName("Write")))
                                            .WithArgumentList(
                                                SyntaxFactory.ArgumentList(
                                                    SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                                        SyntaxFactory.Argument(
                                                            SyntaxFactory.LiteralExpression(
                                                                SyntaxKind.NumericLiteralExpression,
                                                                SyntaxFactory.Literal(0))))))), writeExpression)));
            }

            return writeExpression;
        }

        private static StatementSyntax SerializeAtomicType(SerializableMember member)
        {
            return SerializeAtomicType(GetArgumentSyntaxFromMember(member), member.Type);
        }

        internal static StatementSyntax AddTaskCreation(string taskName, ArgumentSyntax argument)
        {
            return SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("SubTasks"),
                        SyntaxFactory.IdentifierName("Add")))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.ObjectCreationExpression(
                                        SyntaxFactory.ParseTypeName(taskName))
                                        .WithArgumentList(
                                            SyntaxFactory.ArgumentList(
                                                SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                    new SyntaxNodeOrToken[]
                                                    {
                                                        argument,
                                                        SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                        SyntaxFactory.Argument(
                                                            SyntaxFactory.IdentifierName(nameof(SerializationContext))),
                                                        SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                        SyntaxFactory.Argument(
                                                            SyntaxFactory.IdentifierName("Scheduler"))
                                                    })))
                                        .WithInitializer(
                                            SyntaxFactory.InitializerExpression(
                                                SyntaxKind.ObjectInitializerExpression,
                                                SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                                                    SyntaxFactory.AssignmentExpression(
                                                        SyntaxKind.SimpleAssignmentExpression,
                                                        SyntaxFactory.IdentifierName("Id"),
                                                        SyntaxFactory.InvocationExpression(
                                                            SyntaxFactory.MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                SyntaxFactory.IdentifierName("Id"),
                                                                SyntaxFactory.IdentifierName("CreateChild")))
                                                            .WithArgumentList(
                                                                SyntaxFactory.ArgumentList(
                                                                    SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>
                                                                        (SyntaxFactory.Argument(SyntaxFactory
                                                                            .PrefixUnaryExpression(
                                                                                SyntaxKind.PreIncrementExpression,
                                                                                SyntaxFactory.IdentifierName(
                                                                                    "SubTaskCount")))))))))))))));
        }

        internal static void GenerateTasksForClass(Type type)
        {
            SerializerState.KnownTypesSerialize.Add(type);
            int typeId = SerializerState.KnownTypesSerialize.IndexOf(type);
            List<ClassDeclarationSyntax> classes = null;
            if (type.IsGenericCollection())
            {
                classes = SerializeCollection(type, typeId, type.IsGenericDictionary());
            }
            else
            {
                classes = SerializeNonCollection(type, typeId);
            }

            CompilationUnitSyntax compUnit = CreateCompilationUnitFromClasses(classes);
            var dispatcher =
                SerializerState.Compilation.SyntaxTrees.SingleOrDefault(
                    x =>
                        x.GetRoot()
                            .DescendantNodes()
                            .OfType<ClassDeclarationSyntax>()
                            .Any(c => c.Identifier.ToString() == ConstantsForGeneration.DispatcherClassName));

            if (dispatcher != null)
            {
                SerializerState.Compilation = SerializerState.Compilation.RemoveSyntaxTrees(dispatcher);
            }

            SerializerState.Compilation =
                SerializerState.Compilation.AddSyntaxTrees(DispatcherGenerator.GenerateDispatcher().SyntaxTree);
            SerializerState.Compilation = SerializerState.Compilation.AddSyntaxTrees(compUnit.SyntaxTree);
            RefreshReferences();
            Emit();
        }

        internal static void RefreshReferences()
        {
            foreach (var assemblyLocation in SerializerState.KnownTypesSerialize.SelectMany(x => x.GenericTypeArguments.Select(y => y.Assembly.Location).Union(new [] {x.Assembly.Location}))
                .Union(new[] { typeof(object).Assembly.Location, typeof(TaskGenerator).Assembly.Location, typeof(SmartBinaryWriter).Assembly.Location }).Distinct())
            {
                if (!SerializerState.References.Contains(assemblyLocation))
                {
                    SerializerState.References.Add(assemblyLocation);
                    SerializerState.Compilation =
                        SerializerState.Compilation.AddReferences(MetadataReference.CreateFromFile(assemblyLocation));
                }
            }
        }

        private static List<ClassDeclarationSyntax> SerializeCollection(Type type, int typeId, bool isDictionary)
        {
            var classes = new List<ClassDeclarationSyntax>();
            var mainClass = type.GetSerializerTask(type.GetSerializerTaskName());
            StatementSyntax statement = null;
            var serializeMethod = mainClass.GetSerializerMethod();
            var setupMethod = mainClass.GetSetupChildTasksMethod();
            ExpressionSyntax identifier = (isDictionary)
                ? (ExpressionSyntax) SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("Object"),
                    SyntaxFactory.IdentifierName("Keys"))
                : (ExpressionSyntax) SyntaxFactory.IdentifierName("Object");
            var foreachStatement = SyntaxFactory.ForEachStatement(
                SyntaxFactory.ParseTypeName(type.GenericTypeArguments[0].GetFullCorrectTypeName()),
                SyntaxFactory.Identifier("item"),
                identifier,
                SyntaxFactory.Block());
            var countStatement = SerializeAtomicType(new SerializableMember { Name = "Count", Type = typeof(int) });
            serializeMethod = serializeMethod.AddBodyStatements(GetTypeIdSerialization(typeId));
            serializeMethod = serializeMethod.AddBodyStatements(countStatement);
            if (!isDictionary)
            {
                if (type.GenericTypeArguments[0].IsAtomic())
                {
                    statement = AddTaskCreation(type.GenericTypeArguments[0].GetSerializerTaskName(),
                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName("item")));
                }
                else
                {
                    statement = AddTaskCreation(ConstantsForGeneration.LazyDispatcherClassName,
                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName("item")));
                }
                foreachStatement = foreachStatement.WithStatement(statement);
                setupMethod = setupMethod.AddBodyStatements(foreachStatement);
            }
            else
            {
                string keyTaskName = type.GenericTypeArguments[0].IsAtomic()
                    ? type.GenericTypeArguments[0].GetSerializerTaskName()
                    : ConstantsForGeneration.LazyDispatcherClassName;
                string valueTaskName = type.GenericTypeArguments[1].IsAtomic()
                    ? type.GenericTypeArguments[1].GetSerializerTaskName()
                    : ConstantsForGeneration.LazyDispatcherClassName;
                statement =
                    SyntaxFactory.Block(AddTaskCreation(keyTaskName,
                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName("item"))),
                        AddTaskCreation(valueTaskName, SyntaxFactory.Argument(SyntaxFactory.ElementAccessExpression(
                            SyntaxFactory.IdentifierName("Object"))
                            .WithArgumentList(
                                SyntaxFactory.BracketedArgumentList(
                                    SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.IdentifierName("item")))))
                            )));
                foreachStatement = foreachStatement.WithStatement(statement);
                setupMethod = setupMethod.AddBodyStatements(foreachStatement);
            }
            mainClass = mainClass.ReplaceNode(mainClass.GetSetupChildTasksMethod(), setupMethod);
            mainClass = mainClass.ReplaceNode(mainClass.GetSerializerMethod(), serializeMethod);
            classes.Add(mainClass);
            return classes;
        }

        private static StatementSyntax GetTypeIdSerialization(int typeId)
        {
            return SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(ConstantsForGeneration.BinaryWriterName),
                        SyntaxFactory.IdentifierName("Write")))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(
                                        SyntaxKind.NumericLiteralExpression,
                                        SyntaxFactory.Literal(typeId)))))));
        }

        private static List<ClassDeclarationSyntax> SerializeNonCollection(Type type, int typeId)
        {
            var classes = new List<ClassDeclarationSyntax>();
            var classCount = 2;
            var mainClass = type.GetSerializerTask(type.GetSerializerTaskName());
            var serializerMethod = mainClass.GetSerializerMethod();
            serializerMethod = serializerMethod.AddBodyStatements(GetTypeIdSerialization(typeId));
            mainClass = mainClass.ReplaceNode(mainClass.GetSerializerMethod(), serializerMethod);
            classes.Add(mainClass);

            var members = type.GetSerializableMembers();
            var lastAtomic = members.IndexOf(members.LastOrDefault(x => x.Type.IsAtomic()));
            for (int i = 0; i < members.Count; i++)
            {
                var serializableMember = members[i];
                var serializableType = serializableMember.Type;
                var method = classes[classCount - 2].GetSerializerMethod();
                var setupMethod = mainClass.GetSetupChildTasksMethod();
                int curr = classCount - 2;
                if (serializableType.IsAtomic())
                {
                    method = method.AddBodyStatements(SerializeAtomicType(serializableMember));
                }
                else
                {
                    var argument = SyntaxFactory.Argument(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("Object"),
                            SyntaxFactory.IdentifierName(serializableMember.Name)));
                    setupMethod = setupMethod.AddBodyStatements(AddTaskCreation(ConstantsForGeneration.LazyDispatcherClassName, argument));
                    if (lastAtomic > i)
                    {
                        var nextClass = type.GetSerializerTask(type.GetSerializerTaskName(classCount));
                        var nextArgument = SyntaxFactory.Argument(SyntaxFactory.IdentifierName("Object"));
                        setupMethod = setupMethod.AddBodyStatements(AddTaskCreation(type.GetSerializerTaskName(classCount), nextArgument));
                        classCount++;
                        classes.Add(nextClass);
                    }
                }
                var currClass = classes[curr].ReplaceNode(classes[curr].GetSerializerMethod(), method);
                currClass = currClass.ReplaceNode(currClass.GetSetupChildTasksMethod(), setupMethod);
                classes[curr] = currClass;
            }
            return classes;
        }

        internal static void Emit()
        {
            using (var ms = new MemoryStream())
            {
                var result = SerializerState.Compilation.Emit(ms);
                if (!result.Success)
                {
                    throw new InvalidOperationException("A fordítás nem sikerült!");
                }
                ms.Position = 0;
                SerializerState.GeneratedAssembly = Assembly.Load(ms.ToArray());
                SerializerState.GeneratedAssembly.GetType(ConstantsForGeneration.TaskNamespace + "." +
                                                          ConstantsForGeneration.DispatcherClassName)
                    .GetMethod(ConstantsForGeneration.SetupFactoryMethodName)
                    .Invoke(null, new object[0]);
            }
        }

        public static void GenerateAssembly()
        {
            using (var fs = new FileStream("output.dll", FileMode.Create))
            {
                var result = SerializerState.Compilation.Emit(fs);
                if (!result.Success)
                {
                    throw new InvalidOperationException("A fordítás nem sikerült!");
                }
            }
        }

        internal static CompilationUnitSyntax CreateCompilationUnitFromClasses(IEnumerable<ClassDeclarationSyntax> classes)
        {
            var nameSpace =
                SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(ConstantsForGeneration.TaskNamespace));
            nameSpace = nameSpace.AddMembers(classes.ToArray());
            var compUnit = SyntaxFactory.CompilationUnit().AddMembers(nameSpace);
            foreach (var ns in SerializerState.KnownTypesSerialize
                .SelectMany(x => x.GenericTypeArguments.Select(y => y.Namespace).Union(new[] {x.Namespace})).Union(new[]
            {
                typeof(object).Namespace, typeof(ISerializationTask).Namespace, typeof(SmartBinaryWriter).Namespace,
                typeof(TaskGenerator).Namespace, typeof(KeyValuePair<,>).Namespace
            }).Distinct())
            {
                compUnit = compUnit.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(ns)));
            }
            return compUnit;
        }
    }
}
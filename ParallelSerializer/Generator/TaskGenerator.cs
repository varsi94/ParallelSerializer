﻿using DemoModel;
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
        private static StatementSyntax SerializeAtomicType(SerializableMember member)
        {
            if (member.Type == typeof(string))
            {
                return
                    SyntaxFactory.IfStatement(
                        SyntaxFactory.BinaryExpression(
                            SyntaxKind.EqualsExpression,
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("Object"),
                                SyntaxFactory.IdentifierName(member.Name)),
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
                                                                SyntaxFactory.Literal(0))))))),
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
                                                            SyntaxFactory.MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                SyntaxFactory.IdentifierName("Object"),
                                                                SyntaxFactory.IdentifierName(member.Name))))))))));
            }
            else
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
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.IdentifierName("Object"),
                                            SyntaxFactory.IdentifierName(member.Name)))))));
            }
        }

        internal static ClassDeclarationSyntax AddNullChecking(ClassDeclarationSyntax taskClass)
        {
            var serializeMethod = taskClass.GetSerializerMethod();
            var newSerializerMethod = serializeMethod.AddBodyStatements(SyntaxFactory.IfStatement(
                SyntaxFactory.BinaryExpression(
                    SyntaxKind.EqualsExpression,
                    SyntaxFactory.IdentifierName("Object"),
                    SyntaxFactory.LiteralExpression(
                        SyntaxKind.NullLiteralExpression)),
                SyntaxFactory.Block(
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
                                                    SyntaxFactory.Literal(1)))))))),
                    SyntaxFactory.ReturnStatement()))
                );
            return taskClass.ReplaceNode(serializeMethod, newSerializerMethod);
        }

        internal static StatementSyntax AddTaskCreation(string taskName, ArgumentSyntax argument)
        {
            return SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("ChildTasks"),
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
                                                            SyntaxFactory.IdentifierName("SerializationContext")),
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

        public static void GenerateTasksForClass(Type type)
        {
            TaskGenerationResult currResult = null;
            if (!SerializerState.TaskDictionary.ContainsKey(type))
            {
                currResult = new TaskGenerationResult { TypeId = SerializerState.TypeCount };
                Interlocked.Increment(ref SerializerState.TypeCount);
                SerializerState.TaskDictionary.TryAdd(type, currResult);
            }
            else
            {
                SerializerState.TaskDictionary.TryGetValue(type, out currResult);
                currResult.AutoResetEvent.WaitOne();
                return;
            }
            List<ClassDeclarationSyntax> classes = new List<ClassDeclarationSyntax>();
            var classCount = 2;
            var mainClass = type.GetSerializerTask(type.GetSerializerTaskName());
            var serializerMethod = mainClass.GetSerializerMethod();
            serializerMethod = serializerMethod.AddBodyStatements(SyntaxFactory.ExpressionStatement(
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
                                                                SyntaxFactory.Literal(currResult.TypeId))))))));
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
                    setupMethod = setupMethod.AddBodyStatements(AddTaskCreation(ConstantsForGeneration.DispatcherClassName, argument));
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

            foreach (var assemblyLocation in SerializerState.TaskDictionary.GetAssemblyLocations()
                .Union(new[] { typeof(object).Assembly.Location, typeof(TaskGenerator).Assembly.Location, typeof(SmartBinaryWriter).Assembly.Location }).Distinct())
            {
                if (!SerializerState.References.Contains(assemblyLocation))
                {
                    SerializerState.References.Add(assemblyLocation);
                    SerializerState.Compilation =
                        SerializerState.Compilation.AddReferences(MetadataReference.CreateFromFile(assemblyLocation));
                }
            }

            Emit();
            currResult.AutoResetEvent.Set();
        }

        public static void Emit()
        {
            using (var ms = new MemoryStream())
            using (var fs = new FileStream("output.dll", FileMode.Create))
            {
                var result = SerializerState.Compilation.Emit(ms);
                ms.Position = 0;
                ms.CopyTo(fs);
                SerializerState.GeneratedAssembly = Assembly.Load(ms.ToArray());
                SerializerState.GeneratedAssembly.GetType(ConstantsForGeneration.TaskNamespace + "." + ConstantsForGeneration.DispatcherClassName)
                    .GetMethod(ConstantsForGeneration.SetupFactoryMethodName)
                    .Invoke(null, new object[0]);
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
            foreach (var ns in SerializerState.TaskDictionary.GetNamespaces().Union(new[]
            {
                typeof(object).Namespace, typeof(ISerializationTask).Namespace, typeof(SmartBinaryWriter).Namespace,
                typeof(TaskGenerator).Namespace
            }))
            {
                compUnit = compUnit.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(ns)));
            }
            return compUnit;
        }
    }
}
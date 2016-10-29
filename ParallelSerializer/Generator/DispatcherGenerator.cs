using DynamicSerializer.Core;
using DynamicSerializer.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParallelSerializer.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer.Generator
{
    public static class DispatcherGenerator
    {
        public static CompilationUnitSyntax GenerateDispatcher()
        {
            var dispatcher = typeof(object).GetSerializerTask(ConstantsForGeneration.DispatcherClassName);
            dispatcher = TaskGenerator.AddNullChecking(dispatcher);
            var serializeMethod = dispatcher.GetSerializerMethod();
            var setupMethod = dispatcher.GetSetupChildTasksMethod();

            foreach (var type in SerializerState.TaskDictionary.Where(x => x.Key != typeof(object)).Select(x => x.Key))
            {
                var argument = SyntaxFactory.Argument(SyntaxFactory.CastExpression(
                    SyntaxFactory.IdentifierName(type.GetFullCorrectTypeName()),
                    SyntaxFactory.IdentifierName("Object")));
                var block = TaskGenerator.AddTaskCreation(type.GetSerializerTaskName(), argument);
                var ifStatement = SyntaxFactory.IfStatement(
                    SyntaxFactory.BinaryExpression(
                        SyntaxKind.EqualsExpression,
                        SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("Object"),
                                SyntaxFactory.IdentifierName("GetType"))),
                        SyntaxFactory.TypeOfExpression(
                            SyntaxFactory.IdentifierName(type.GetFullCorrectTypeName()))),
                    SyntaxFactory.Block(new[] {block, SyntaxFactory.ReturnStatement()}));
                setupMethod = setupMethod.AddBodyStatements(ifStatement);
            }
            
            dispatcher = dispatcher.ReplaceNode(dispatcher.GetSerializerMethod(), serializeMethod);
            dispatcher = dispatcher.ReplaceNode(dispatcher.GetSetupChildTasksMethod(), setupMethod);

            dispatcher = dispatcher.AddMembers(SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(
                    SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                SyntaxFactory.Identifier(ConstantsForGeneration.SetupFactoryMethodName))
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        new[]
                        {
                            SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                            SyntaxFactory.Token(SyntaxKind.StaticKeyword)
                        }))
                .WithBody(
                    SyntaxFactory.Block(
                        SyntaxFactory.SingletonList<StatementSyntax>(
                            SyntaxFactory.ExpressionStatement(
                                SyntaxFactory.AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName(nameof(SerializerState)),
                                        SyntaxFactory.IdentifierName(nameof(SerializerState.DispatcherFactory))),
                                    SyntaxFactory.ParenthesizedLambdaExpression(
                                        SyntaxFactory.ObjectCreationExpression(
                                            SyntaxFactory.IdentifierName(ConstantsForGeneration.DispatcherClassName))
                                            .WithArgumentList(
                                                SyntaxFactory.ArgumentList(
                                                    SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                        new SyntaxNodeOrToken[]
                                                        {
                                                            SyntaxFactory.Argument(
                                                                SyntaxFactory.IdentifierName("o")),
                                                            SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                            SyntaxFactory.Argument(
                                                                SyntaxFactory.IdentifierName("c")),
                                                            SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                            SyntaxFactory.Argument(
                                                                SyntaxFactory.IdentifierName("s"))
                                                        }))))
                                        .WithParameterList(
                                            SyntaxFactory.ParameterList(
                                                SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                    new SyntaxNodeOrToken[]
                                                    {
                                                        SyntaxFactory.Parameter(
                                                            SyntaxFactory.Identifier("o")),
                                                        SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                        SyntaxFactory.Parameter(
                                                            SyntaxFactory.Identifier("c")),
                                                        SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                        SyntaxFactory.Parameter(
                                                            SyntaxFactory.Identifier("s"))
                                                    })))))))));
            return TaskGenerator.CreateCompilationUnitFromClasses(new[] { dispatcher });
        }

        public static void Initialize()
        {
            SerializerState.Compilation = CSharpCompilation.Create("SerializerAssembly",
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddSyntaxTrees(GenerateDispatcher().SyntaxTree);
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

            TaskGenerator.Emit();
        }
    }
}

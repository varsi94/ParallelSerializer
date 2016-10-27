using DemoModel;
using DynamicSerializer.Core;
using DynamicSerializer.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer.Generator
{
    public static class TaskGenerator
    {
        private static ClassDeclarationSyntax GetSerializerTask(string name, Type type)
        {
            return SyntaxFactory.ClassDeclaration(name)
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithBaseList(
                    SyntaxFactory.BaseList(
                        SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(
                            SyntaxFactory.SimpleBaseType(
                                SyntaxFactory.GenericName(
                                    SyntaxFactory.Identifier("SerializationTask"))
                                    .WithTypeArgumentList(
                                        SyntaxFactory.TypeArgumentList(
                                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                SyntaxFactory.ParseTypeName(type.GetFullCorrectTypeName()))))))))
                .WithMembers(
                    SyntaxFactory.List<MemberDeclarationSyntax>(
                        new MemberDeclarationSyntax[]
                        {
                            SyntaxFactory.ConstructorDeclaration(
                                SyntaxFactory.Identifier(name))
                                .WithModifiers(
                                    SyntaxFactory.TokenList(
                                        SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                                .WithParameterList(
                                    SyntaxFactory.ParameterList(
                                        SyntaxFactory.SeparatedList<ParameterSyntax>(
                                            new SyntaxNodeOrToken[]
                                            {
                                                SyntaxFactory.Parameter(
                                                    SyntaxFactory.Identifier("context"))
                                                    .WithType(
                                                        SyntaxFactory.IdentifierName(typeof(SerializationContext).Name)),
                                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                SyntaxFactory.Parameter(
                                                    SyntaxFactory.Identifier("scheduler"))
                                                    .WithType(
                                                        SyntaxFactory.IdentifierName(typeof(IScheduler).Name))
                                            })))
                                .WithInitializer(
                                    SyntaxFactory.ConstructorInitializer(
                                        SyntaxKind.BaseConstructorInitializer,
                                        SyntaxFactory.ArgumentList(
                                            SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                new SyntaxNodeOrToken[]
                                                {
                                                    SyntaxFactory.Argument(
                                                        SyntaxFactory.IdentifierName("context")),
                                                    SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                    SyntaxFactory.Argument(
                                                        SyntaxFactory.IdentifierName("scheduler"))
                                                }))))
                                .WithBody(
                                    SyntaxFactory.Block()),
                            SyntaxFactory.MethodDeclaration(
                                SyntaxFactory.PredefinedType(
                                    SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                                SyntaxFactory.Identifier(ConstantsForGeneration.SerializeMethodName))
                                .WithModifiers(
                                    SyntaxFactory.TokenList(
                                        new[]
                                        {
                                            SyntaxFactory.Token(SyntaxKind.ProtectedKeyword),
                                            SyntaxFactory.Token(SyntaxKind.OverrideKeyword)
                                        }))
                                .WithParameterList(
                                    SyntaxFactory.ParameterList(
                                        SyntaxFactory.SingletonSeparatedList<ParameterSyntax>(
                                            SyntaxFactory.Parameter(
                                                SyntaxFactory.Identifier(ConstantsForGeneration.BinaryWriterName))
                                                .WithType(
                                                    SyntaxFactory.IdentifierName(typeof(SmartBinaryWriter).Name)))))
                                .WithBody(
                                    SyntaxFactory.Block())
                        }));
        }
        
        public static string GetClass()
        {
            return GetSerializerTask("ProductSerializerTask", typeof(Product)).NormalizeWhitespace().ToFullString();
        }
    }
}

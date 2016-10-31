using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicSerializer.Core;
using DynamicSerializer.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParallelSerializer.Generator;

namespace ParallelSerializer.Extensions
{
    public static class TypeExtensions
    {
        internal static ClassDeclarationSyntax GetSerializerTask(this Type type, string name)
        {
            TypeSyntax typeSyntax = SyntaxFactory.ParseTypeName(type.GetFullCorrectTypeName());
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
                                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(typeSyntax)))))))
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
                                                    SyntaxFactory.Identifier("obj"))
                                                    .WithType(
                                                        SyntaxFactory.ParseTypeName(type.GetFullCorrectTypeName())),
                                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                SyntaxFactory.Parameter(
                                                    SyntaxFactory.Identifier("context"))
                                                    .WithType(
                                                        SyntaxFactory.IdentifierName(nameof(SerializationContext))),
                                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                SyntaxFactory.Parameter(
                                                    SyntaxFactory.Identifier("scheduler"))
                                                    .WithType(
                                                        SyntaxFactory.IdentifierName(nameof(IScheduler)))
                                            })))
                                .WithInitializer(
                                    SyntaxFactory.ConstructorInitializer(
                                        SyntaxKind.BaseConstructorInitializer,
                                        SyntaxFactory.ArgumentList(
                                            SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                new SyntaxNodeOrToken[]
                                                {
                                                    SyntaxFactory.Argument(
                                                        SyntaxFactory.IdentifierName("obj")),
                                                    SyntaxFactory.Token(SyntaxKind.CommaToken),
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
                                                SyntaxFactory.Identifier("bw"))
                                                .WithType(
                                                    SyntaxFactory.IdentifierName(nameof(SmartBinaryWriter))))))
                                .WithBody(
                                    SyntaxFactory.Block()),
                            SyntaxFactory.MethodDeclaration(
                                SyntaxFactory.PredefinedType(
                                    SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                                SyntaxFactory.Identifier("SetupChildTasks"))
                                .WithModifiers(
                                    SyntaxFactory.TokenList(
                                        new[]
                                        {
                                            SyntaxFactory.Token(SyntaxKind.ProtectedKeyword),
                                            SyntaxFactory.Token(SyntaxKind.OverrideKeyword)
                                        }))
                                .WithBody(
                                    SyntaxFactory.Block())
                        }));
        }

        public static string GetSerializerTaskName(this Type t, int? id = null)
        {
            if (t.IsAtomic())
            {
                return $"{t.Name}SerializationTask";
            }

            string typeName = t.Namespace.Replace(".", "") + t.GetNameFriendlyTypeName();
            if (id.HasValue)
            {
                return $"{typeName}{id.Value}SerializationTask";
            }
            return $"{typeName}SerializationTask";
        }

        private static string GetNameFriendlyTypeName(this Type t)
        {
            if (t.IsGenericType)
            {
                return t.GetCorrectTypeName() + "_" +
                       string.Join("_", t.GenericTypeArguments.Select(x => x.GetNameFriendlyTypeName()));
            }
            else if (t.IsArray)
            {
                //TODO: lehetséges névkonfliktus.
                return t.GetElementType().GetCorrectTypeName() + "Array";
            }
            else
            {
                return t.GetCorrectTypeName();
            }
        }

        public static string GetPrimitiveSerializationTaskName(this string typeName)
        {
            return $"{char.ToUpper(typeName.First())}{typeName.Substring(1)}SerializationTask";
        }
    }
}

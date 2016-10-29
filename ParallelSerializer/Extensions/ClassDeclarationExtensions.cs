using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer.Extensions
{
    public static class ClassDeclarationExtensions
    {
        internal static MethodDeclarationSyntax GetSerializerMethod(this ClassDeclarationSyntax taskClass)
        {
            return taskClass.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Single(x => x.Identifier.ToString() == "Serialize");
        }

        internal static MethodDeclarationSyntax GetSetupChildTasksMethod(this ClassDeclarationSyntax taskClass)
        {
            return taskClass.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Single(x => x.Identifier.ToString() == "SetupChildTasks");
        }
    }
}

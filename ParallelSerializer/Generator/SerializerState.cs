using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer.Generator
{
    public static class SerializerState
    {
        internal static List<Type> KnownTypesSerialize { get; } = new List<Type> { typeof(string), typeof(int), typeof(long), typeof(short),
            typeof(bool), typeof(double), typeof(byte), typeof(char), typeof(byte), typeof(sbyte), typeof(float),
            typeof(uint), typeof(ulong), typeof(ushort), typeof(DateTime) };

        internal static List<string> References { get; } = new List<string>();

        public static Func<object, SerializationContext, IScheduler, SerializationTask<object>> DispatcherFactory { get; set; }

        internal static Assembly GeneratedAssembly { get; set; }

        internal static CSharpCompilation Compilation { get; set; }
    }
}

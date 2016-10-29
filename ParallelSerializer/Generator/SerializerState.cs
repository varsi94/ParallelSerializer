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
        public static List<Type> KnownTypesSerialize { get; } = new List<Type> { typeof(string), typeof(int), typeof(long), typeof(short),
            typeof(bool), typeof(double), typeof(byte), typeof(char), typeof(byte), typeof(sbyte), typeof(float),
            typeof(uint), typeof(ulong), typeof(ushort), typeof(DateTime) };

        public static TaskDictionary TaskDictionary { get; } = new TaskDictionary();

        public static List<string> References { get; } = new List<string>();

        public static Func<object, SerializationContext, IScheduler, SerializationTask<object>> DispatcherFactory { get; set; }

        internal static Assembly GeneratedAssembly { get; set; }

        public static int TypeCount = 15;

        public static CSharpCompilation Compilation { get; set; }
    }
}

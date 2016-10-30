using ParallelSerializer.SerializerTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer.Generator
{
    internal static class ConstantsForGeneration
    {
        public const string TaskNamespace = "ParallelSerializer.SerializerTasks";
        public const string SerializeMethodName = "Serialize";
        public const string BinaryWriterName = "bw";
        public const string DispatcherClassName = "CustomDispatcherTask";
        public const string LazyDispatcherClassName = nameof(LazyDispatcherTask);
        public const string SetupFactoryMethodName = "SetupFactory";
    }
}

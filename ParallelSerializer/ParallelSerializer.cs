using DynamicSerializer.Core;
using DynamicSerializer.Roslyn;
using ParallelSerializer.Generator;
using ParallelSerializer.SerializerTasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelSerializer
{
    public class ParallelSerializer
    {
        private readonly IScheduler scheduler;
        
        public ParallelSerializer(IScheduler scheduler)
        {
            this.scheduler = scheduler;
        }

        public void Serialize(object obj, Stream output)
        {
            using (var ms = new MemoryStream())
            using (var outputWriter = new SmartBinaryWriter(ms))
            using (var context = new SerializationContext())
            {
                var task = new DispatcherTask(obj, context, scheduler);
                context.TaskTreeRoot = new TaskTreeNode()
                {
                    Task = task
                };
                task.TaskTreeNode = context.TaskTreeRoot;
                scheduler.QueueWorkItem(task);
                context.WaitForAllTasks();

                outputWriter.Write(SerializerState.KnownTypesSerialize.Count - 15);
                foreach (var newType in SerializerState.KnownTypesSerialize.OrderBy(x => x.Key).Where(x => x.Key >= 15).Select(x => x.Value))
                {
                    outputWriter.Write(newType.AssemblyQualifiedName);
                }

                if (obj != null && (obj.GetType().IsValueType || obj is string))
                {
                    outputWriter.Write(SerializerState.KnownTypesSerialize.SingleOrDefault(x => x.Value == obj.GetType()).Key);
                }

                var joinedResult = context.GetJoinedResult();
                outputWriter.Write(joinedResult);
                ms.Position = 0;
                ms.CopyTo(output);
            }
        }

        public object Deserialize(Stream input)
        {
            return RoslynDynamicSerializerEngine.Deserialize(input, false);
        }
    }
}

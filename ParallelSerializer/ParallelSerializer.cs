using DemoModel;
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
            using (var barrier = new Barrier())
            {
                var context = new SerializationContext()
                {
                    Barrier = barrier
                };

                var task = new DispatcherTask(obj, context, scheduler)
                {
                    Object = obj,
                    Id = TaskId.CreateDefault()
                };
                scheduler.QueueWorkItem(task);
                context.Barrier.WaitForAll();

                outputWriter.Write(SerializerState.TaskDictionary.Count);
                foreach (var newType in SerializerState.TaskDictionary.OrderBy(x => x.Value.TypeId).Select(x => x.Key))
                {
                    outputWriter.Write(newType.AssemblyQualifiedName);
                }

                if (obj != null && (obj.GetType().IsValueType || obj is string))
                {
                    outputWriter.Write(SerializerState.TaskDictionary[obj.GetType()].TypeId);
                }
                outputWriter.Write(context.Results.GetJoinedResult());
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

using DynamicSerializer.Core;
using DynamicSerializer.Roslyn;
using ParallelSerializer.Generator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelSerializer
{
    public abstract class SerializationTask<T> : ISerializationTask
    {
        protected int SubTaskCount { get; set; } = 0;

        public List<ISerializationTask> SubTasks { get; } = new List<ISerializationTask>();

        protected IScheduler Scheduler { get; }

        public T Object { get; set; }

        public TaskId Id { get; set; }

        protected SerializationContext SerializationContext { get; }

        public SerializationTask(T obj, SerializationContext context, IScheduler scheduler)
        {
            Object = obj;
            SerializationContext = context;
            Scheduler = scheduler;
            SerializationContext.Barrier.Start();
        }

        public virtual void SerializeObject(object state)
        {
            try
            {
                if (Object != null)
                {
                    SetupChildTasks();
                    foreach (var task in SubTasks)
                    {
                        Scheduler.QueueWorkItem(task);
                    }
                }

                using (var ms = new MemoryStream())
                using (var bw = new SmartBinaryWriter(ms))
                {
                    Serialize(bw);
                    SerializationContext.Results.TryAdd(Id, ms.ToArray());
                }
            }
            finally
            {
                SerializationContext.Barrier.Stop();
            }
        }

        protected abstract void Serialize(SmartBinaryWriter bw);

        protected abstract void SetupChildTasks();
    }
}

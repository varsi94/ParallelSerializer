using DynamicSerializer.Core;
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
        protected IScheduler Scheduler { get; }

        public T Object { get; set; }

        public TaskId Id { get; set; }

        protected SerializationContext SerializationContext { get; }

        public SerializationTask(SerializationContext context, IScheduler scheduler)
        {
            SerializationContext = context;
            Scheduler = scheduler;
        }

        public void SerializeObject(object state)
        {
            using (var ms = new MemoryStream())
            using (var bw = new SmartBinaryWriter(ms))
            {
                SerializationContext.Barrier.Start();
                Serialize(bw);
                SerializationContext.Results.TryAdd(Id, ms.ToArray());
                SerializationContext.Barrier.Stop();
            }
        }

        protected abstract void Serialize(SmartBinaryWriter bw);
    }
}

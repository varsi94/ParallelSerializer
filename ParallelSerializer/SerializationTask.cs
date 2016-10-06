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

        public AutoResetEvent Handle { get; } = new AutoResetEvent(false);

        public WaitCallback Callback { get; }

        public T Object { get; set; }

        public string Id { get; set; }

        public SerializationContext SerializationContext { get; }

        public SerializationTask(SerializationContext context, IScheduler scheduler)
        {
            Callback = SerializeCallback;
            SerializationContext = context;
            Scheduler = scheduler;
        }

        private void SerializeCallback(object state)
        {
            using (var ms = new MemoryStream())
            using (var bw = new SmartBinaryWriter(ms))
            {
                Scheduler.Handles.Add(Handle);
                Handle.WaitOne();
                Serialize(bw);
                SerializationContext.Results.TryAdd(Id, ms.ToArray());
                Handle.Set();
            }
        }

        protected abstract void Serialize(SmartBinaryWriter bw);
    }
}

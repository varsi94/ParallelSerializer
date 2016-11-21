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
    public abstract class SerializationTask<T> : TaskBase<T>, ISerializationTask
    {
        private object syncRoot = new object();

        public SerializationTask(T obj, SerializationContext context, IScheduler scheduler) : base(obj, context, scheduler)
        {

        }

        public override void SerializeObject(object state)
        {
            try
            {
                if (Object != null)
                {
                    SetupChildTasks();
                    foreach (var task in TaskTreeNode.Children.Select(x => x.Task))
                    {
                        Scheduler.QueueWorkItem(task);
                    }
                }

                using (var ms = new MemoryStream())
                using (var bw = new SmartBinaryWriter(ms))
                {
                    Serialize(bw);
                    SerializationResult = ms.ToArray();
                }
            }
            finally
            {
                SerializationContext.StopTask(this);
            }
        }

        protected abstract void Serialize(SmartBinaryWriter bw);

        protected abstract void SetupChildTasks();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicSerializer.Core;

namespace ParallelSerializer
{
    public class DummyTask : SerializationTask<int>
    {
        public DummyTask(int obj, SerializationContext context, IScheduler scheduler) : base(obj, context, scheduler)
        {
            SetupChildTasks();
        }

        protected override void Serialize(SmartBinaryWriter bw)
        {

        }

        protected override void SetupChildTasks()
        {

        }
    }
}

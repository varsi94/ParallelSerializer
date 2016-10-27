using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicSerializer.Core;

namespace ParallelSerializer
{
    public class DummyTask : SerializationTask<List<int>>
    {
        public DummyTask(SerializationContext context, IScheduler scheduler) : base(context, scheduler)
        {
        }

        protected override void Serialize(SmartBinaryWriter bw)
        {
        }
    }
}

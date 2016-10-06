using DemoModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DynamicSerializer.Core;

namespace ParallelSerializer
{
    //TO BE GENERATED
    public class ProductSerializationTask : SerializationTask<Product>
    {
        private class ProductPropSerailizationTask : SerializationTask<Product>
        {
            public ProductPropSerailizationTask(SerializationContext context, IScheduler scheduler) : base(context, scheduler)
            {
            }

            protected override void Serialize(SmartBinaryWriter bw)
            {
                bw.Write(Object.Count);
                bw.Write(Object.ID);
                bw.Write(Object.Name == null ? -1 : 0);
                if (Object.Name != null)
                {
                    bw.Write(Object.Name);
                }
            }
        }
        
        public ProductSerializationTask(SerializationContext context, IScheduler scheduler) : base(context, scheduler)
        {
        }

        protected override void Serialize(SmartBinaryWriter bw)
        {
            bw.Write(Object == null ? -1 : 15);
            if (Object != null)
            {
                var categoryST = new CategorySerializationTask(SerializationContext, Scheduler)
                {
                    Object = Object.Category,
                    Id = Id + "_1"
                };
                Scheduler.QueueWorkItem(categoryST);

                var otherST = new ProductPropSerailizationTask(SerializationContext, Scheduler)
                {
                    Object = Object,
                    Id = Id + "_2"
                };
                Scheduler.QueueWorkItem(otherST);
            }
        }
    }
}

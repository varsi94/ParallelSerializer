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
    public class CategorySerializationTask : SerializationTask<Category>
    {
        public CategorySerializationTask(SerializationContext context, IScheduler scheduler) : base(context, scheduler)
        {
        }

        protected override void Serialize(SmartBinaryWriter bw)
        {
            bw.Write(Object == null ? -1 : 16);
            if (Object != null)
            {
                bw.Write(Object.ID);
                bw.Write(Object.Name == null ? -1 : 0);
                if (Object.Name != null)
                {
                    bw.Write(Object.Name);
                }
                bw.Write(Object.Products == null ? -1 : 17);
                if (Object.Products != null)
                {
                    bw.Write(Object.Products.Count);
                    int i = 1;
                    foreach (var product in Object.Products)
                    {
                        var productST = new ProductSerializationTask(SerializationContext, Scheduler);
                        productST.Object = product;
                        productST.Id = Id + "_" + i.ToString();
                        i++;
                        Scheduler.QueueWorkItem(productST);
                    }
                }
            }
        }
    }
}

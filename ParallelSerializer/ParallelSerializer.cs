using DemoModel;
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
    public class ParallelSerializer
    {
        private readonly IScheduler scheduler;
        
        public ParallelSerializer(IScheduler scheduler)
        {
            this.scheduler = scheduler;
        }

        public void Serialize(Product obj, Stream output)
        {
            using (var ms = new MemoryStream())
            using (var bw = new SmartBinaryWriter(ms))
            using (var barrier = new Barrier())
            {
                var context = new SerializationContext()
                {
                    Barrier = barrier
                };
                var task = new ProductSerializationTask(context, scheduler)
                {
                    Object = obj,
                    Id = TaskId.CreateDefault()
                };

                scheduler.QueueWorkItem(task);
                bw.Write(3);
                bw.Write(typeof(Product).AssemblyQualifiedName);
                bw.Write(typeof(Category).AssemblyQualifiedName);
                bw.Write(typeof(List<Product>).AssemblyQualifiedName);

                barrier.WaitForAll();
                bw.Write(context.Results.GetJoinedResult());
                ms.Position = 0;
                ms.CopyTo(output);
            }
        }

        public Product Deserialize(Stream input)
        {
            using (var br = new BinaryReader(input))
            {
                int id = br.ReadInt32();
                if (id != 15)
                {
                    throw new InvalidOperationException();
                }

                return null;
            }
        }
    }
}

using DemoModel;
using DynamicSerializer.Roslyn;
using ParallelSerializer.App.Wrappers;
using ParallelSerializer.Generator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var obj1 = new Category
            {
                Products =
                    Enumerable.Range(1, 100).Select(x => new Product {Name = "asd" + x, Count = 5, ID = x}).ToList(),
                Name = "dummyCategory",
                ID = 5
            };

            List<ISerializerWrapper> wrappers = new List<ISerializerWrapper>
            {
                new RoslynDynamicSerializerWrapper(),
                new ParallelSerializerWrapper()
            };

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Serialization: " + i);
                foreach (var serializerWrapper in wrappers)
                {
                    Console.WriteLine("Serializer: " + serializerWrapper.GetType().Name);
                    using (var ms = new MemoryStream())
                    {
                        serializerWrapper.Serialize(obj1, ms);
                        ms.Position = 0;
                        serializerWrapper.Deserialize(ms);
                    }
                }
                TaskGenerator.GenerateAssembly();
            }

            foreach (var serializerWrapper in wrappers)
            {
                Console.WriteLine("Serializer: " + serializerWrapper.GetType().Name);
                Console.WriteLine("Min: " + serializerWrapper.MeasurementResults.Min(x => x.TotalMilliseconds) + " ms");
                Console.WriteLine("Average: " + serializerWrapper.MeasurementResults.Average(x => x.TotalMilliseconds) + " ms");
                Console.WriteLine("Max: " + serializerWrapper.MeasurementResults.Max(x => x.TotalMilliseconds) + " ms");
            }
            Console.ReadLine();
        }
    }
}

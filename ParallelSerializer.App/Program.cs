using DemoModel;
using DynamicSerializer.Roslyn;
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
            var parallelSerializer = new ParallelSerializer(new TplScheduler());
            var product = new Product
            {
                Name = "aasd",
                Count = 2,
                Category = new Category() {  ID = 5, Name = "DummyCategory"}
            };
            using (var ms = new MemoryStream())
            {
                parallelSerializer.Serialize(product, ms);
                ms.Position = 0;
                var result = RoslynDynamicSerializerEngine.Deserialize(ms);
            }
        }
    }
}

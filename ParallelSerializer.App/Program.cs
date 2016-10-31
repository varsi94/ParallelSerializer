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
            var category = new Category
            {
                Name = "asd",
                Products = Enumerable.Range(1, 5).Select(x => new Product {ID = x}).ToList()
            };
            using (var ms = new MemoryStream())
            {
                parallelSerializer.Serialize(category, ms);
                TaskGenerator.GenerateAssembly();
                ms.Position = 0;
                var result = parallelSerializer.Deserialize(ms);
            }

            using (var ms = new MemoryStream())
            {
                parallelSerializer.Serialize(category, ms);
                TaskGenerator.GenerateAssembly();
                ms.Position = 0;
                var result = parallelSerializer.Deserialize(ms);
            }
        }
    }
}

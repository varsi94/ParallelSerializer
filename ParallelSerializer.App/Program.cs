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
                Name = "ital",
                ID = 1,
                Products =
                    Enumerable.Range(1, 100).Select(x => new Product {Name = "Staropramen" + x, ID = x, Count = 5}).ToList()
            };
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

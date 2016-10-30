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
            var list = Enumerable.Range(1, 100).Select(x => new Product {Name = "Staropramen", ID = 1, Count = x}).ToList();
            var category = new Category {Name = "Ital", Products = list, ID = 1};
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

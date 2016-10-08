using DemoModel;
using DynamicSerializer.Roslyn;
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
            using (var ms = new MemoryStream())
            {
                var serializer = new ParallelSerializer(new TplScheduler());
                var category = new Category
                {
                    Name = "asdasd",
                    Products = Enumerable.Range(1, 100).Select(x => new Product { Name = "asd" + x, ID = x + 1, Count = 5 }).ToList()
                };
                var product = new Product
                {
                    Name = "asd",
                    ID = 1,
                    Category = category,
                    Count = 5
                };

                RoslynDynamicSerializerEngine.SerializeAssembly = true;
                serializer.Serialize(product, ms);
                ms.Position = 0;
                var result = (Product)RoslynDynamicSerializerEngine.Deserialize(ms, false);
                űConsole.ReadLine();
            }
        }
    }
}

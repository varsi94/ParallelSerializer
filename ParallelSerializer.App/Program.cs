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
            RoslynDynamicSerializerEngine.SerializeAssembly = false;
            var obj1 = new Category
            {
                Products =
                    Enumerable.Range(1, 100).Select(x => new Product { Name = "asd" + x, Count = 5, ID = x }).ToList(),
                Name = "dummyCategory",
                ID = 5
            };

            using (var ms = new MemoryStream())
            {
                var serializer = new ParallelSerializer(new TplScheduler());
                serializer.Serialize(obj1, ms);
                ms.Position = 0;
                var result = (Category)serializer.Deserialize(ms);
            }

            //var test = new SerializerTest(obj1, "Lapos objektum");
            //test.Run();
            //test.SaveResultsToFile();

            //var obj2 = GetDeepObject(100);
            //var test2 = new SerializerTest(obj2, "Mély objektum");
            //test2.Run();
            //test2.SaveResultsToFile();
        }

        static Product GetDeepObject(int size)
        {
            if (size == 0)
            {
                return new Product { Name = "asd" + size, ID = 5, Count = 5 };
            }
            var product = new Product();
            var category = new Category()
            {
                Name = "asd" + size,
                ID = 5,
                Products = new List<Product> { GetDeepObject(size - 1) }
            };
            product.Category = category;
            return product;
        }
    }
}

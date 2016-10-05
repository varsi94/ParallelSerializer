using DemoModel;
using System;
using System.Collections.Generic;
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
            ParallelSerializer serializer = new ParallelSerializer();
            using (var ms = new MemoryStream())
            {
                var product = new Product
                {
                    Name = "asd",
                    ID = 1,
                    Category = new Category {Name = "asdasd"},
                    Count = 5
                };
                serializer.Serialize(product, ms);
            }
        }
    }
}

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
            var dict = Enumerable.Range(1, 100).ToList();
            using (var ms = new MemoryStream())
            {
                parallelSerializer.Serialize(dict, ms);
                TaskGenerator.GenerateAssembly();
                ms.Position = 0;
                var result = parallelSerializer.Deserialize(ms);
            }
        }
    }
}

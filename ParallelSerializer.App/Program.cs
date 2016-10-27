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
            using (var ms = new MemoryStream())
            {
                parallelSerializer.Serialize(15, ms);
                ms.Position = 0;
                var result = (int) RoslynDynamicSerializerEngine.Deserialize(ms);
            }
        }
    }
}

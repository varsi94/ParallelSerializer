using ParallelSerializer.App.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParallelSerializer.Generator;

namespace ParallelSerializer.App
{
    public class SerializerTest
    {
        private readonly List<ISerializerWrapper> wrappers = new List<ISerializerWrapper>
        {
            new RoslynDynamicSerializerWrapper(),
            new ParallelSerializerWrapper()
        };

        private readonly object input;
        private readonly string name;

        public SerializerTest(object input, string name)
        {
            this.input = input;
            this.name = name;
        }

        public void Run()
        {
            for (int i = 0; i < 10; i++)
            {
                foreach (var serializerWrapper in wrappers)
                {
                    using (var ms = new MemoryStream())
                    {
                        serializerWrapper.Serialize(input, ms);
                        ms.Position = 0;
                        serializerWrapper.Deserialize(ms);
                    }
                }
                TaskGenerator.GenerateAssembly();
            }
        }

        public void SaveResultsToFile()
        {
            using (var fs = new FileStream(name + ".txt", FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                foreach (var wrapper in wrappers)
                {
                    var typeName = wrapper.GetType().Name;
                    sw.WriteLine("Serializer name\t" + typeName.Substring(0, typeName.IndexOf("Wrapper")));
                    int i = 1;
                    foreach (var result in wrapper.MeasurementResults)
                    {
                        sw.WriteLine($"\tSerialization #{i}\t{result.TotalMilliseconds}");
                        i++;
                    }
                    sw.WriteLine($"\tMinimum:\t{wrapper.MeasurementResults.Min(x => x.TotalMilliseconds)}");
                    sw.WriteLine($"\tAverage:\t{wrapper.MeasurementResults.Average(x => x.TotalMilliseconds)}");
                    sw.WriteLine($"\tMaximum:\t{wrapper.MeasurementResults.Max(x => x.TotalMilliseconds)}");
                }
            }
        }
    }
}

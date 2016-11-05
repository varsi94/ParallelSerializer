using DynamicSerializer.Roslyn;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer.App.Wrappers
{
    public class RoslynDynamicSerializerWrapper : ISerializerWrapper
    {
        public List<TimeSpan> MeasurementResults { get; } = new List<TimeSpan>();

        public object Deserialize(Stream input)
        {
            return RoslynDynamicSerializerEngine.Deserialize(input, false);
        }

        public void Serialize(object input, Stream output)
        {
            Stopwatch sw = Stopwatch.StartNew();
            RoslynDynamicSerializerEngine.Serialize(input, output, false);
            sw.Stop();
            MeasurementResults.Add(sw.Elapsed);
        }
    }
}

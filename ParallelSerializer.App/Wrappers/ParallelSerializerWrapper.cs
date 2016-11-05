using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer.App.Wrappers
{
    public class ParallelSerializerWrapper : ISerializerWrapper
    {
        private readonly ParallelSerializer serializer;
        public ParallelSerializerWrapper()
        {
            serializer = new ParallelSerializer(new TplScheduler());
        }

        public List<TimeSpan> MeasurementResults { get; } = new List<TimeSpan>();

        public object Deserialize(Stream input)
        {
            return serializer.Deserialize(input);
        }

        public void Serialize(object input, Stream output)
        {
            Stopwatch sw = Stopwatch.StartNew();
            serializer.Serialize(input, output);
            sw.Stop();
            MeasurementResults.Add(sw.Elapsed);
        }
    }
}

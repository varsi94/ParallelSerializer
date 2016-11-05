using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer.App
{
    public interface ISerializerWrapper
    {
        void Serialize(object input, Stream output);

        object Deserialize(Stream input);

        List<TimeSpan> MeasurementResults { get; } 
    }
}

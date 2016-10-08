using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer
{
    public class SerializationContext
    {
        public SerializationResults Results { get; } = new SerializationResults();

        public Barrier Barrier { get; set; }
    }
}

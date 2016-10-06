using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer
{
    public class SerializationResults : ConcurrentDictionary<string, byte[]>
    {
        public byte[] GetJoinedResult()
        {
            return this.OrderBy(x => x.Key).SelectMany(x => x.Value).ToArray();
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer
{
    public class SerializationResults : ConcurrentDictionary<TaskId, byte[]>
    {
        /// <summary>
        /// Joins the byte outputs in the order of the keys. NOTE: This method is not thread-safe!
        /// </summary>
        /// <returns>The joined byte array.</returns>
        public byte[] GetJoinedResult()
        {
            return this.AsParallel().OrderBy(x => x.Key).SelectMany(x => x.Value).ToArray();
        }
    }
}

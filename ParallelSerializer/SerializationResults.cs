using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer
{
    public class SerializationResults : Dictionary<TaskId, byte[]>
    {
        public void AddAtomic(TaskId id, byte[] value)
        {
            lock (((ICollection)this).SyncRoot)
            {
                Add(id, value);
            }
        }

        /// <summary>
        /// Joins the byte outputs in the order of the keys. NOTE: This method is not thread-safe!
        /// </summary>
        /// <returns>The joined byte array.</returns>
        public byte[] GetJoinedResult()
        {
            return this.OrderBy(x => x.Key).SelectMany(x => x.Value).ToArray();
        }
    }
}

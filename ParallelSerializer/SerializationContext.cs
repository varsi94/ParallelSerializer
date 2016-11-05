using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer
{
    public class SerializationContext
    {
        private readonly object syncRoot = new object();

        private bool finished = false;

        private SortedDictionary<TaskId, byte[]> Results { get; } = new SortedDictionary<TaskId, byte[]>();

        private Barrier Barrier { get; } = new Barrier();

        public void StartTask(ISerializationTask task)
        {
            Barrier.Start();
        }

        public void StopTask(ISerializationTask task)
        {
            Barrier.Stop();
        }

        public void WaitForAllTasks()
        {
            Barrier.WaitForAll();
            finished = true;
        }

        public void AddSerializationResult(ISerializationTask task, byte[] result)
        {
            if (finished)
            {
                throw new InvalidOperationException();
            }

            if (result.Length > 0)
            {
                lock (syncRoot)
                {
                    Results.Add(task.Id, result);
                }
            }
        }

        /// <summary>
        /// Joins the byte outputs in the order of the keys. NOTE: This method is not thread-safe!
        /// </summary>
        /// <returns>The joined byte array.</returns>
        public byte[] GetJoinedResult()
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                foreach (var source in Results)
                {
                    bw.Write(source.Value);
                }
                return ms.ToArray();
            }
        }
    }
}

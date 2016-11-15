using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer
{
    public class SerializationContext : IDisposable
    {
        private readonly object syncRoot = new object();
        private bool finished = false;

        public TaskTreeNode TaskTreeRoot { get; set; }

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

        /// <summary>
        /// Joins the byte outputs in the order of the keys. NOTE: This method is not thread-safe!
        /// </summary>
        /// <returns>The joined byte array.</returns>
        public byte[] GetJoinedResult()
        {
            if (!finished)
            {
                throw new InvalidOperationException();
            }

            var result = new byte[TaskTreeRoot.GetLength()];
            AddToBuffer(TaskTreeRoot, result, 0);
            return result;
        }

        private int AddToBuffer(TaskTreeNode treeNode, byte[] buffer, int offset)
        {
            int len = 0;
            if (treeNode.Task.SerializationResult != null && treeNode.Task.SerializationResult.Length > 0)
            {
                treeNode.Task.SerializationResult.CopyTo(buffer, offset);
                len += treeNode.Task.SerializationResult.Length;
            }
            
            foreach (var subTask in treeNode.Children)
            {
                len += AddToBuffer(subTask, buffer, offset + len);
            }
            return len;
        }

        public void Dispose()
        {
            ((IDisposable)Barrier).Dispose();
        }
    }
}

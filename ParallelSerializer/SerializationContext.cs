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

            using (var ms = new MemoryStream())
            {
                TaskTreeNode currentNode = TaskTreeRoot;
                while (currentNode != null)
                {
                    if (currentNode.Task.SerializationResult != null && currentNode.Task.SerializationResult.Length != 0)
                    {
                        ms.Write(currentNode.Task.SerializationResult, 0, currentNode.Task.SerializationResult.Length);
                    }

                    var firstChild = currentNode.Children.FirstOrDefault();
                    if (firstChild != null)
                    {
                        currentNode = firstChild;
                        continue;
                    }

                    if (currentNode.NextSibling != null)
                    {
                        currentNode = currentNode.NextSibling;
                        continue;
                    }

                    while (currentNode != TaskTreeRoot && currentNode.NextSibling == null)
                    {
                        currentNode = currentNode.Parent;
                    }

                    if (currentNode == TaskTreeRoot)
                    {
                        break;
                    }
                    currentNode = currentNode.NextSibling;
                }
                return ms.ToArray();
            }
        }

        public void Dispose()
        {
            ((IDisposable)Barrier).Dispose();
        }
    }
}
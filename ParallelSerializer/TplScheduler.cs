using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelSerializer
{
    public class TplScheduler : IScheduler
    {
        public List<WaitHandle> Handles { get; } = new List<WaitHandle>();

        public void QueueWorkItem(ISerializationTask task)
        {
            Console.WriteLine(task.Id + ": " + Thread.CurrentThread.ManagedThreadId);
            ThreadPool.QueueUserWorkItem(task.Callback);
        }
    }
}

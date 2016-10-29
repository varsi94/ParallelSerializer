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
        public void QueueWorkItem(ISerializationTask task)
        {
            ThreadPool.QueueUserWorkItem(task.SerializeObject);
        }

        public void WaitAll(WaitHandle[] handles)
        {
            WaitHandle.WaitAll(handles);
        }
    }
}

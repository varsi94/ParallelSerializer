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

        public void WaitAllSubTasks(ISerializationTask task)
        {
            WaitHandle.WaitAll(task.SubTasks.Select(x => x.WaitHandle).ToArray());
            task.DisposeHandles();
        }
    }
}

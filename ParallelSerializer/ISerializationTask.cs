using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelSerializer
{
    public interface ISerializationTask
    {
        TaskId Id { get; }
        WaitCallback Callback { get; }
    }
}

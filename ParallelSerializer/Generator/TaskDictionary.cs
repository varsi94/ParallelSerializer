using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer.Generator
{
    public class TaskDictionary : ConcurrentDictionary<Type, TaskFactory>
    {
    }
}

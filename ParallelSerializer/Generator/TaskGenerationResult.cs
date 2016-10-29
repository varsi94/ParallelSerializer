using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelSerializer.Generator
{
    public class TaskGenerationResult
    {
        public int TypeId { get; set; }
        public bool IsFinished { get; set; }
        public AutoResetEvent AutoResetEvent { get; set; }

        public TaskGenerationResult()
        {
            AutoResetEvent = new AutoResetEvent(false);
        }
    }
}

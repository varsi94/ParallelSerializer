using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer.Generator
{
    public class TaskFactory
    {
        private Func<object, SerializationContext, IScheduler, object> Factory { get; set; }

        public TaskFactory(Func<object, SerializationContext, IScheduler, object> taskFactory)
        {
            Factory = taskFactory;
        }

        public SerializationTask<T> GetSerializationTask<T>(T input, SerializationContext context, IScheduler scheduler)
        {
            return (SerializationTask<T>) Factory(input, context, scheduler);
        } 
    }
}

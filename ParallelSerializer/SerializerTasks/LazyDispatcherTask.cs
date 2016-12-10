using ParallelSerializer.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer.SerializerTasks
{
    public class LazyDispatcherTask : TaskBase<object>, ISerializationTask
    {
        private object syncRoot = new object();

        public LazyDispatcherTask(object obj, SerializationContext context, IScheduler scheduler) : base(obj, context, scheduler)
        {
        }

        private void GenerateNewClassTasks()
        {
            var type = Object.GetType();
            if (SerializerState.KnownTypesSerialize.Any(x => x.Value == type))
            {
                return;
            }

            lock (TaskGenerator.TaskGenerationSyncRoot)
            {
                if (SerializerState.KnownTypesSerialize.Any(x => x.Value == type))
                {
                    return;
                }
                TaskGenerator.GenerateTasksForClass(type);
            }
        }

        public override void SerializeObject(object state)
        {
            try
            {
                if (Object != null)
                {
                    GenerateNewClassTasks();
                }
                var dispatcher = SerializerState.DispatcherFactory(Object, SerializationContext, Scheduler);
                AddSubTask(dispatcher);
                Scheduler.QueueWorkItem(dispatcher);
            }
            finally
            {
                SerializationContext.StopTask(this);
            }
        }
    }
}

using ParallelSerializer.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer
{
    public class LazyDispatcherTask : ISerializationTask
    {
        protected object Object { get; }

        protected SerializationContext SerializationContext { get; }

        protected IScheduler Scheduler { get; }

        public TaskId Id { get; set; }

        private void GenerateNewClassTasks()
        {
            lock (TaskGenerator.TaskGenerationSyncRoot)
            {
                if (SerializerState.KnownTypesSerialize.Contains(Object.GetType()))
                {
                    return;
                }
                TaskGenerator.GenerateTasksForClass(Object.GetType());
            }
        }

        public void SerializeObject(object state)
        {
            try
            {
                if (Object != null)
                {
                    GenerateNewClassTasks();
                }
                var dispatcher = SerializerState.DispatcherFactory(Object, SerializationContext, Scheduler);
                dispatcher.Id = Id;
                Scheduler.QueueWorkItem(dispatcher);
            }
            finally
            {
                SerializationContext.Barrier.Stop();
            }
        }

        public LazyDispatcherTask(object obj, SerializationContext context, IScheduler scheduler)
        {
            Object = obj;
            SerializationContext = context;
            Scheduler = scheduler;

            SerializationContext.Barrier.Start();
        }
    }
}

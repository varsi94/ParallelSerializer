using ParallelSerializer.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer.SerializerTasks
{
    public class LazyDispatcherTask : ISerializationTask
    {
        private object syncRoot = new object();

        protected object Object { get; }

        protected SerializationContext SerializationContext { get; }

        protected IScheduler Scheduler { get; }

        public byte[] SerializationResult { get; set; }

        public TaskTreeNode TaskTreeNode { get; set; }

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
                dispatcher.TaskTreeNode = new TaskTreeNode() {Task = dispatcher};
                TaskTreeNode.Children.Add(dispatcher.TaskTreeNode);
                Scheduler.QueueWorkItem(dispatcher);
            }
            finally
            {
                SerializationContext.StopTask(this);
            }
        }

        public LazyDispatcherTask(object obj, SerializationContext context, IScheduler scheduler)
        {
            Object = obj;
            SerializationContext = context;
            Scheduler = scheduler;

            SerializationContext.StartTask(this);
        }
    }
}

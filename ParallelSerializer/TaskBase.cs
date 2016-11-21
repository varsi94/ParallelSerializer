using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer
{
    public abstract class TaskBase<T> : ISerializationTask
    {
        public byte[] SerializationResult { get; protected set; }

        public TaskTreeNode TaskTreeNode { get; set; }
        
        protected IScheduler Scheduler { get; }

        protected T Object { get; }

        protected SerializationContext SerializationContext { get; }

        public TaskBase(T obj, SerializationContext context, IScheduler scheduler)
        {
            Object = obj;
            SerializationContext = context;
            Scheduler = scheduler;

            SerializationContext.StartTask(this);
        }

        public abstract void SerializeObject(object state);

        protected virtual void AddSubTask(ISerializationTask task)
        {
            var child = new TaskTreeNode { Task = task, Parent = TaskTreeNode };
            task.TaskTreeNode = child;
            var last = TaskTreeNode.Children.LastOrDefault();
            if (last != null)
            {
                last.NextSibling = child;
            }
            TaskTreeNode.Children.Add(child);
        }
    }
}

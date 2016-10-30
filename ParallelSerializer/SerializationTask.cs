using DynamicSerializer.Core;
using DynamicSerializer.Roslyn;
using ParallelSerializer.Generator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelSerializer
{
    public abstract class SerializationTask<T> : ISerializationTask
    {
        protected int SubTaskCount { get; set; } = 0;

        public List<ISerializationTask> SubTasks { get; } = new List<ISerializationTask>();

        protected IScheduler Scheduler { get; }

        public T Object { get; set; }

        public TaskId Id { get; set; }

        protected SerializationContext SerializationContext { get; }

        public SerializationTask(T obj, SerializationContext context, IScheduler scheduler)
        {
            Object = obj;
            SerializationContext = context;
            Scheduler = scheduler;
            SerializationContext.Barrier.Start(Id + " " + GetType().Name);
        }

        private bool GenerateNewClassTasks()
        {
            TaskGenerationResult result;
            SerializerState.TaskDictionary.TryGetValue(Object.GetType(), out result);
            if (result == null)
            {
                TaskGenerator.GenerateTasksForClass(Object.GetType());
                var task = SerializerState.DispatcherFactory(Object, SerializationContext, Scheduler);
                task.Id = Id.CreateChild(++SubTaskCount);
                SubTasks.Add(task);
                return true;
            }
            return false;
        }

        public void SerializeObject(object state)
        {
            try
            {
                if (Object != null)
                {
                    bool generated = GenerateNewClassTasks();
                    if (!generated)
                    {
                        SetupChildTasks();
                    }

                    foreach (var task in SubTasks)
                    {
                        Scheduler.QueueWorkItem(task);
                    }
                }

                using (var ms = new MemoryStream())
                using (var bw = new SmartBinaryWriter(ms))
                {
                    Serialize(bw);
                    SerializationContext.Results.TryAdd(Id, ms.ToArray());
                }
            }
            finally
            {
                SerializationContext.Barrier.Stop(Id + " " + GetType().Name);
            }
        }

        protected abstract void Serialize(SmartBinaryWriter bw);

        protected abstract void SetupChildTasks();
    }
}

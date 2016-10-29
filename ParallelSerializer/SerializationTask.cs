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

        public AutoResetEvent WaitHandle { get; set; }

        public SerializationTask(T obj, SerializationContext context, IScheduler scheduler)
        {
            Object = obj;
            SerializationContext = context;
            Scheduler = scheduler;
            WaitHandle = new AutoResetEvent(false);
        }

        private bool GenerateNewClassTasks()
        {
            if (!SerializerState.TaskDictionary.ContainsKey(Object.GetType()))
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
            if (Object != null)
            {
                bool generated = GenerateNewClassTasks();
                if (!generated)
                {
                    SetupChildTasks();
                }
            }
            try
            {
                using (var ms = new MemoryStream())
                using (var bw = new SmartBinaryWriter(ms))
                {
                    SerializationContext.Barrier.Start(Id + " " + GetType().Name);
                    if (Object != null)
                    {
                        foreach (var task in SubTasks)
                        {
                            Scheduler.QueueWorkItem(task);
                        }
                    }
                    Serialize(bw);
                    if (SubTasks.Count > 0 && Object != null)
                    {
                        Scheduler.WaitAllSubTasks(this);
                    }
                    SerializationContext.Results.TryAdd(Id, ms.ToArray());
                    SerializationContext.Barrier.Stop(Id + " " + GetType().Name);
                }
            }
            finally
            {
                WaitHandle.Set();
            }
        }

        protected abstract void Serialize(SmartBinaryWriter bw);

        protected abstract void SetupChildTasks();

        public void DisposeHandles()
        {
            foreach (var task in SubTasks)
            {
                task.WaitHandle.Dispose();
            }
        }
    }
}

﻿using DynamicSerializer.Core;
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
        private object syncRoot = new object();

        protected int SubTaskCount { get; set; } = 0;

        protected List<ISerializationTask> SubTasks { get; } = new List<ISerializationTask>();

        protected IScheduler Scheduler { get; }

        protected T Object { get; set; }

        protected SerializationContext SerializationContext { get; }

        public byte[] SerializationResult { get; set; }

        public TaskTreeNode TaskTreeNode { get; set; }

        public SerializationTask(T obj, SerializationContext context, IScheduler scheduler)
        {
            Object = obj;
            SerializationContext = context;
            Scheduler = scheduler;
            SerializationContext.StartTask(this);
        }

        public virtual void SerializeObject(object state)
        {
            try
            {
                if (Object != null)
                {
                    SetupChildTasks();
                    foreach (var task in SubTasks)
                    {
                        Scheduler.QueueWorkItem(task);
                    }
                }
                
                using (var ms = new MemoryStream())
                using (var bw = new SmartBinaryWriter(ms))
                {
                    Serialize(bw);
                    SerializationResult = ms.ToArray();
                }
            }
            finally
            {
                SerializationContext.StopTask(this);
            }
        }

        protected abstract void Serialize(SmartBinaryWriter bw);

        protected abstract void SetupChildTasks();

        protected virtual void AddSubTask(ISerializationTask task)
        {
            lock (TaskTreeNode.SyncRoot)
            {
                var child = new TaskTreeNode { Task = task };
                TaskTreeNode.Children.Add(child);
                task.TaskTreeNode = child;
                SubTasks.Add(task);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicSerializer.Core;
using ParallelSerializer.Generator;

namespace ParallelSerializer.SerializerTasks
{
    public class DispatcherTask : SerializationTask<object>
    {
        static DispatcherTask()
        {
            DispatcherGenerator.Initialize();
        }

        public DispatcherTask(object obj, SerializationContext context, IScheduler scheduler) : base(obj, context, scheduler)
        {
        }

        protected override void Serialize(SmartBinaryWriter binaryWriter)
        {
            if (Object == null)
            {
                binaryWriter.Write(-1);
            }
			var type = Object.GetType();

			if (type == typeof(string))
			{
				binaryWriter.Write((string)Object);
				return;
			}

			if (type == typeof(int))
			{
				binaryWriter.Write((int)Object);
				return;
			}

			if (type == typeof(long))
			{
				binaryWriter.Write((long)Object);
				return;
			}

			if (type == typeof(short))
			{
				binaryWriter.Write((short)Object);
				return;
			}

			if (type == typeof(bool))
			{
				binaryWriter.Write((bool)Object);
				return;
			}

			if (type == typeof(double))
			{
				binaryWriter.Write((double)Object);
				return;
			}

			if (type == typeof(byte))
			{
				binaryWriter.Write((byte)Object);
				return;
			}

			if (type == typeof(char))
			{
				binaryWriter.Write((char)Object);
				return;
			}

			if (type == typeof(byte))
			{
				binaryWriter.Write((byte)Object);
				return;
			}

			if (type == typeof(sbyte))
			{
				binaryWriter.Write((sbyte)Object);
				return;
			}

			if (type == typeof(float))
			{
				binaryWriter.Write((float)Object);
				return;
			}

			if (type == typeof(uint))
			{
				binaryWriter.Write((uint)Object);
				return;
			}

			if (type == typeof(ulong))
			{
				binaryWriter.Write((ulong)Object);
				return;
			}

			if (type == typeof(ushort))
			{
				binaryWriter.Write((ushort)Object);
				return;
			}

			if (type == typeof(DateTime))
			{
				binaryWriter.Write((DateTime)Object);
				return;
			}
        }

        protected override void SetupChildTasks()
        {
            if (!SerializerState.KnownTypesSerialize.Contains(Object.GetType()))
            {
                var customDispatcher = new LazyDispatcherTask(Object, SerializationContext, Scheduler);
				customDispatcher.Id = Id.CreateChild(++SubTaskCount);
				SubTasks.Add(customDispatcher);
            }
        }
    }
}
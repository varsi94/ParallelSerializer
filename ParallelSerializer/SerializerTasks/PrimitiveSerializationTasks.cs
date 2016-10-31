using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicSerializer.Core;
using ParallelSerializer.Generator;

namespace ParallelSerializer.SerializerTasks
{
	public class StringSerializationTask : SerializationTask<String>
    {
        public StringSerializationTask(String obj, SerializationContext context, IScheduler scheduler) : base(obj, context, scheduler)
        {
        }

        protected override void Serialize(SmartBinaryWriter binaryWriter)
        {
			if (Object == null)
			{
				binaryWriter.Write(-1);
			}
			else
			{
				binaryWriter.Write(0);
				binaryWriter.Write(Object);
			}
        }

        protected override void SetupChildTasks()
        {
        }
    }

	public class Int32SerializationTask : SerializationTask<Int32>
    {
        public Int32SerializationTask(Int32 obj, SerializationContext context, IScheduler scheduler) : base(obj, context, scheduler)
        {
        }

        protected override void Serialize(SmartBinaryWriter binaryWriter)
        {
			binaryWriter.Write(Object);
        }

        protected override void SetupChildTasks()
        {
        }
    }

	public class Int64SerializationTask : SerializationTask<Int64>
    {
        public Int64SerializationTask(Int64 obj, SerializationContext context, IScheduler scheduler) : base(obj, context, scheduler)
        {
        }

        protected override void Serialize(SmartBinaryWriter binaryWriter)
        {
			binaryWriter.Write(Object);
        }

        protected override void SetupChildTasks()
        {
        }
    }

	public class Int16SerializationTask : SerializationTask<Int16>
    {
        public Int16SerializationTask(Int16 obj, SerializationContext context, IScheduler scheduler) : base(obj, context, scheduler)
        {
        }

        protected override void Serialize(SmartBinaryWriter binaryWriter)
        {
			binaryWriter.Write(Object);
        }

        protected override void SetupChildTasks()
        {
        }
    }

	public class BooleanSerializationTask : SerializationTask<Boolean>
    {
        public BooleanSerializationTask(Boolean obj, SerializationContext context, IScheduler scheduler) : base(obj, context, scheduler)
        {
        }

        protected override void Serialize(SmartBinaryWriter binaryWriter)
        {
			binaryWriter.Write(Object);
        }

        protected override void SetupChildTasks()
        {
        }
    }

	public class DoubleSerializationTask : SerializationTask<Double>
    {
        public DoubleSerializationTask(Double obj, SerializationContext context, IScheduler scheduler) : base(obj, context, scheduler)
        {
        }

        protected override void Serialize(SmartBinaryWriter binaryWriter)
        {
			binaryWriter.Write(Object);
        }

        protected override void SetupChildTasks()
        {
        }
    }

	public class ByteSerializationTask : SerializationTask<Byte>
    {
        public ByteSerializationTask(Byte obj, SerializationContext context, IScheduler scheduler) : base(obj, context, scheduler)
        {
        }

        protected override void Serialize(SmartBinaryWriter binaryWriter)
        {
			binaryWriter.Write(Object);
        }

        protected override void SetupChildTasks()
        {
        }
    }

	public class CharSerializationTask : SerializationTask<Char>
    {
        public CharSerializationTask(Char obj, SerializationContext context, IScheduler scheduler) : base(obj, context, scheduler)
        {
        }

        protected override void Serialize(SmartBinaryWriter binaryWriter)
        {
			binaryWriter.Write(Object);
        }

        protected override void SetupChildTasks()
        {
        }
    }

	public class DecimalSerializationTask : SerializationTask<Decimal>
    {
        public DecimalSerializationTask(Decimal obj, SerializationContext context, IScheduler scheduler) : base(obj, context, scheduler)
        {
        }

        protected override void Serialize(SmartBinaryWriter binaryWriter)
        {
			binaryWriter.Write(Object);
        }

        protected override void SetupChildTasks()
        {
        }
    }

	public class SByteSerializationTask : SerializationTask<SByte>
    {
        public SByteSerializationTask(SByte obj, SerializationContext context, IScheduler scheduler) : base(obj, context, scheduler)
        {
        }

        protected override void Serialize(SmartBinaryWriter binaryWriter)
        {
			binaryWriter.Write(Object);
        }

        protected override void SetupChildTasks()
        {
        }
    }

	public class SingleSerializationTask : SerializationTask<Single>
    {
        public SingleSerializationTask(Single obj, SerializationContext context, IScheduler scheduler) : base(obj, context, scheduler)
        {
        }

        protected override void Serialize(SmartBinaryWriter binaryWriter)
        {
			binaryWriter.Write(Object);
        }

        protected override void SetupChildTasks()
        {
        }
    }

	public class UInt32SerializationTask : SerializationTask<UInt32>
    {
        public UInt32SerializationTask(UInt32 obj, SerializationContext context, IScheduler scheduler) : base(obj, context, scheduler)
        {
        }

        protected override void Serialize(SmartBinaryWriter binaryWriter)
        {
			binaryWriter.Write(Object);
        }

        protected override void SetupChildTasks()
        {
        }
    }

	public class UInt64SerializationTask : SerializationTask<UInt64>
    {
        public UInt64SerializationTask(UInt64 obj, SerializationContext context, IScheduler scheduler) : base(obj, context, scheduler)
        {
        }

        protected override void Serialize(SmartBinaryWriter binaryWriter)
        {
			binaryWriter.Write(Object);
        }

        protected override void SetupChildTasks()
        {
        }
    }

	public class UInt16SerializationTask : SerializationTask<UInt16>
    {
        public UInt16SerializationTask(UInt16 obj, SerializationContext context, IScheduler scheduler) : base(obj, context, scheduler)
        {
        }

        protected override void Serialize(SmartBinaryWriter binaryWriter)
        {
			binaryWriter.Write(Object);
        }

        protected override void SetupChildTasks()
        {
        }
    }

	public class DateTimeSerializationTask : SerializationTask<DateTime>
    {
        public DateTimeSerializationTask(DateTime obj, SerializationContext context, IScheduler scheduler) : base(obj, context, scheduler)
        {
        }

        protected override void Serialize(SmartBinaryWriter binaryWriter)
        {
			binaryWriter.Write(Object);
        }

        protected override void SetupChildTasks()
        {
        }
    }

}
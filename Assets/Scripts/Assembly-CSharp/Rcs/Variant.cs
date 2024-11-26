using System;

namespace Rcs
{
	public class Variant : IDisposable
	{
		public enum VariantType
		{
			TypeNull = 0,
			TypeString = 1,
			TypeBoolean = 2,
			TypeInt = 3,
			TypeDouble = 4
		}

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		internal Variant(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		public Variant()
			: this(RCSSDKPINVOKE.new_Variant_0(), true)
		{
		}

		public Variant(string value)
			: this(RCSSDKPINVOKE.new_Variant_1(value), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public Variant(bool value)
			: this(RCSSDKPINVOKE.new_Variant_2(value), true)
		{
		}

		public Variant(int value)
			: this(RCSSDKPINVOKE.new_Variant_3(value), true)
		{
		}

		public Variant(long value)
			: this(RCSSDKPINVOKE.new_Variant_4(value), true)
		{
		}

		public Variant(double value)
			: this(RCSSDKPINVOKE.new_Variant_5(value), true)
		{
		}

		public Variant(Variant other)
			: this(RCSSDKPINVOKE.new_Variant_6(getCPtr(other)), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		internal static IntPtr getCPtr(Variant obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				_DisposeUnmanaged();
				disposed = true;
			}
		}

		~Variant()
		{
			Dispose(false);
		}

		private void _DisposeUnmanaged()
		{
			lock (this)
			{
				if (swigCPtr != IntPtr.Zero)
				{
					if (swigCMemOwn)
					{
						swigCMemOwn = false;
						RCSSDKPINVOKE.delete_Variant(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public VariantType GetVariantType()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return (VariantType)RCSSDKPINVOKE.Variant_GetVariantType(swigCPtr);
		}

		public string StringValue()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Variant_StringValue(swigCPtr);
		}

		public long IntValue()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Variant_IntValue(swigCPtr);
		}

		public double DoubleValue()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Variant_DoubleValue(swigCPtr);
		}

		public bool BoolValue()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Variant_BoolValue(swigCPtr);
		}
	}
}

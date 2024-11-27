using System;

namespace Rcs
{
	public class IdentitySessionBaseSharedPtr : IDisposable
	{
		private IntPtr swigCPtr;

		private bool disposed;

		internal IntPtr CPtr
		{
			get
			{
				return swigCPtr;
			}
		}

		internal IdentitySessionBaseSharedPtr(IntPtr cPtr)
		{
			swigCPtr = cPtr;
		}

		protected IdentitySessionBaseSharedPtr()
		{
			swigCPtr = IntPtr.Zero;
		}

		internal static IntPtr getCPtr(IdentitySessionBaseSharedPtr obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~IdentitySessionBaseSharedPtr()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}
			lock (this)
			{
				if (swigCPtr != IntPtr.Zero)
				{
					RCSSDKPINVOKE.SwigTools_FreeIdentitySharedPtr(swigCPtr);
					swigCPtr = IntPtr.Zero;
				}
			}
			disposed = true;
		}
	}
}

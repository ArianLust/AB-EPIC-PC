using System;

namespace Rcs
{
	internal class SessionSharedPtr
	{
		private IntPtr swigCPtr;

		private bool disposed;

		private bool isOwner;

		internal IntPtr CPtr
		{
			get
			{
				return swigCPtr;
			}
		}

		public SessionSharedPtr(IntPtr cPtr, bool futureUse)
		{
			swigCPtr = cPtr;
			isOwner = futureUse;
		}

		public SessionSharedPtr(SessionSharedPtr otherSession)
		{
		}

		protected SessionSharedPtr()
		{
			swigCPtr = IntPtr.Zero;
		}

		internal static IntPtr getCPtr(SessionSharedPtr obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~SessionSharedPtr()
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
				if (swigCPtr != IntPtr.Zero && isOwner)
				{
					RCSSDKPINVOKE.SwigTools_FreeSessionSharedPtr(swigCPtr);
					swigCPtr = IntPtr.Zero;
				}
			}
			disposed = true;
		}
	}
}

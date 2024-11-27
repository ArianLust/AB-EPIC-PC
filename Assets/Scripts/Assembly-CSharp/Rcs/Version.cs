using System;

namespace Rcs
{
	public class Version : IDisposable
	{
		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		public int Major
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Version_Major_get(swigCPtr);
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				RCSSDKPINVOKE.Version_Major_set(swigCPtr, value);
			}
		}

		public int Minor
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Version_Minor_get(swigCPtr);
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				RCSSDKPINVOKE.Version_Minor_set(swigCPtr, value);
			}
		}

		public int Revision
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Version_Revision_get(swigCPtr);
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				RCSSDKPINVOKE.Version_Revision_set(swigCPtr, value);
			}
		}

		public int Hotfix
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Version_Hotfix_get(swigCPtr);
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				RCSSDKPINVOKE.Version_Hotfix_set(swigCPtr, value);
			}
		}

		public string String
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				string result = RCSSDKPINVOKE.Version_String_get(swigCPtr);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
				return result;
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				RCSSDKPINVOKE.Version_String_set(swigCPtr, value);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}
		}

		internal Version(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		public Version()
			: this(RCSSDKPINVOKE.new_Version(), true)
		{
		}

		internal static IntPtr getCPtr(Version obj)
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

		~Version()
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
						RCSSDKPINVOKE.delete_Version(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}
	}
}

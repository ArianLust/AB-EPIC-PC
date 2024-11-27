using System;

namespace Rcs
{
	public class NetworkCredentials : IDisposable
	{
		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		internal NetworkCredentials(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		public NetworkCredentials(NetworkProvider provider, string credentials)
			: this(RCSSDKPINVOKE.new_NetworkCredentials_0((int)provider, credentials), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public NetworkCredentials(NetworkCredentials arg0)
			: this(RCSSDKPINVOKE.new_NetworkCredentials_1(getCPtr(arg0)), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		internal static IntPtr getCPtr(NetworkCredentials obj)
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

		~NetworkCredentials()
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
						RCSSDKPINVOKE.delete_NetworkCredentials(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public NetworkProvider GetNetworkProvider()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return (NetworkProvider)RCSSDKPINVOKE.NetworkCredentials_GetNetworkProvider(swigCPtr);
		}

		public string GetNetworkName()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.NetworkCredentials_GetNetworkName(swigCPtr);
		}

		public string GetCredentials()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.NetworkCredentials_GetCredentials(swigCPtr);
		}
	}
}

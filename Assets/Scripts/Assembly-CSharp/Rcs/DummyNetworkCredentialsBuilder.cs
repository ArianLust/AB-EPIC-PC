using System;

namespace Rcs
{
	public class DummyNetworkCredentialsBuilder : IDisposable
	{
		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		internal DummyNetworkCredentialsBuilder(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		internal static IntPtr getCPtr(DummyNetworkCredentialsBuilder obj)
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

		~DummyNetworkCredentialsBuilder()
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
						RCSSDKPINVOKE.delete_DummyNetworkCredentialsBuilder(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public static NetworkCredentials Create(string id)
		{
			NetworkCredentials result = new NetworkCredentials(RCSSDKPINVOKE.DummyNetworkCredentialsBuilder_Create(id), true);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}
	}
}

using System;

namespace Rcs
{
	public class RovioAccountNetworkCredentialsBuilder : IDisposable
	{
		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		internal RovioAccountNetworkCredentialsBuilder(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		internal static IntPtr getCPtr(RovioAccountNetworkCredentialsBuilder obj)
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

		~RovioAccountNetworkCredentialsBuilder()
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
						RCSSDKPINVOKE.delete_RovioAccountNetworkCredentialsBuilder(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public static NetworkCredentials Create(string email, string password)
		{
			NetworkCredentials result = new NetworkCredentials(RCSSDKPINVOKE.RovioAccountNetworkCredentialsBuilder_Create(email, password), true);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}
	}
}

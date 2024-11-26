using System;

namespace Rcs
{
	public class FacebookNetworkCredentialsBuilder : IDisposable
	{
		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		internal FacebookNetworkCredentialsBuilder(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		internal static IntPtr getCPtr(FacebookNetworkCredentialsBuilder obj)
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

		~FacebookNetworkCredentialsBuilder()
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
						RCSSDKPINVOKE.delete_FacebookNetworkCredentialsBuilder(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public static NetworkCredentials Create(string facebookAccessToken)
		{
			NetworkCredentials result = new NetworkCredentials(RCSSDKPINVOKE.FacebookNetworkCredentialsBuilder_Create(facebookAccessToken), true);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}
	}
}

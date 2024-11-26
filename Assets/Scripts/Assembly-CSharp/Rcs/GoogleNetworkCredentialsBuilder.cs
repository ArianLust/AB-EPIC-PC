using System;

namespace Rcs
{
	public class GoogleNetworkCredentialsBuilder : IDisposable
	{
		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		internal GoogleNetworkCredentialsBuilder(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		internal static IntPtr getCPtr(GoogleNetworkCredentialsBuilder obj)
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

		~GoogleNetworkCredentialsBuilder()
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
						RCSSDKPINVOKE.delete_GoogleNetworkCredentialsBuilder(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public static NetworkCredentials Create(string googleAppClientId, string googleAppClientSecret, string googleServerAuthorizationCode, string googleRedirectUri)
		{
			NetworkCredentials result = new NetworkCredentials(RCSSDKPINVOKE.GoogleNetworkCredentialsBuilder_Create_0(googleAppClientId, googleAppClientSecret, googleServerAuthorizationCode, googleRedirectUri), true);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public static NetworkCredentials Create(string googleAppClientId, string googleAccessToken)
		{
			NetworkCredentials result = new NetworkCredentials(RCSSDKPINVOKE.GoogleNetworkCredentialsBuilder_Create_1(googleAppClientId, googleAccessToken), true);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}
	}
}

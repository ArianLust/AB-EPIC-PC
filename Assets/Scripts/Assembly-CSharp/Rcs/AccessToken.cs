using System;

namespace Rcs
{
	public class AccessToken : IDisposable
	{
		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		internal AccessToken(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		public AccessToken(string accessToken, long expiresAtMSecs)
			: this(RCSSDKPINVOKE.new_AccessToken_0(accessToken, expiresAtMSecs), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public AccessToken(AccessToken accessToken)
			: this(RCSSDKPINVOKE.new_AccessToken_1(getCPtr(accessToken)), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		internal static IntPtr getCPtr(AccessToken obj)
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

		~AccessToken()
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
						RCSSDKPINVOKE.delete_AccessToken(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public string GetToken()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.AccessToken_GetToken(swigCPtr);
		}

		public bool IsExpired()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.AccessToken_IsExpired(swigCPtr);
		}

		public long ExpiresInMillis()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.AccessToken_ExpiresInMillis(swigCPtr);
		}
	}
}

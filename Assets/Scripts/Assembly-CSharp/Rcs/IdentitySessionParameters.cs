using System;

namespace Rcs
{
	public class IdentitySessionParameters : IDisposable
	{
		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		public string ServerUrl
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				string result = RCSSDKPINVOKE.IdentitySessionParameters_ServerUrl_get(swigCPtr);
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
				RCSSDKPINVOKE.IdentitySessionParameters_ServerUrl_set(swigCPtr, value);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}
		}

		public string ClientId
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				string result = RCSSDKPINVOKE.IdentitySessionParameters_ClientId_get(swigCPtr);
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
				RCSSDKPINVOKE.IdentitySessionParameters_ClientId_set(swigCPtr, value);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}
		}

		public string ClientVersion
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				string result = RCSSDKPINVOKE.IdentitySessionParameters_ClientVersion_get(swigCPtr);
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
				RCSSDKPINVOKE.IdentitySessionParameters_ClientVersion_set(swigCPtr, value);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}
		}

		public string ClientSecret
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				string result = RCSSDKPINVOKE.IdentitySessionParameters_ClientSecret_get(swigCPtr);
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
				RCSSDKPINVOKE.IdentitySessionParameters_ClientSecret_set(swigCPtr, value);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}
		}

		public string Locale
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				string result = RCSSDKPINVOKE.IdentitySessionParameters_Locale_get(swigCPtr);
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
				RCSSDKPINVOKE.IdentitySessionParameters_Locale_set(swigCPtr, value);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}
		}

		public string DistributionChannel
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				string result = RCSSDKPINVOKE.IdentitySessionParameters_DistributionChannel_get(swigCPtr);
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
				RCSSDKPINVOKE.IdentitySessionParameters_DistributionChannel_set(swigCPtr, value);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}
		}

		public string Definition
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				string result = RCSSDKPINVOKE.IdentitySessionParameters_Definition_get(swigCPtr);
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
				RCSSDKPINVOKE.IdentitySessionParameters_Definition_set(swigCPtr, value);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}
		}

		public string BuildId
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				string result = RCSSDKPINVOKE.IdentitySessionParameters_BuildId_get(swigCPtr);
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
				RCSSDKPINVOKE.IdentitySessionParameters_BuildId_set(swigCPtr, value);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}
		}

		internal IdentitySessionParameters(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		public IdentitySessionParameters()
			: this(RCSSDKPINVOKE.new_IdentitySessionParameters(), true)
		{
		}

		public IdentitySessionParameters(IdentitySessionParameters idSessionParams)
			: this(RCSSDKPINVOKE.new_IdentitySessionParameters(), true)
		{
			ServerUrl = idSessionParams.ServerUrl;
			ClientId = idSessionParams.ClientId;
			ClientVersion = idSessionParams.ClientVersion;
			ClientSecret = idSessionParams.ClientSecret;
			Locale = idSessionParams.Locale;
			DistributionChannel = idSessionParams.DistributionChannel;
			Definition = idSessionParams.Definition;
			BuildId = idSessionParams.BuildId;
		}

		internal static IntPtr getCPtr(IdentitySessionParameters obj)
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

		~IdentitySessionParameters()
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
						RCSSDKPINVOKE.delete_IdentitySessionParameters(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}
	}
}

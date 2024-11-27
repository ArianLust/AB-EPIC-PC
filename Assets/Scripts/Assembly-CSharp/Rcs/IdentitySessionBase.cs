using System;
using System.Collections.Generic;

namespace Rcs
{
	public class IdentitySessionBase : IDisposable
	{
		private IntPtr swigCPtr;

		private bool disposed;

		private IdentitySessionBaseSharedPtr sharedPtr;

		internal IdentitySessionBaseSharedPtr SharedPtr
		{
			get
			{
				return sharedPtr;
			}
		}

		internal IdentitySessionBase(IntPtr cPtr)
		{
			swigCPtr = cPtr;
			sharedPtr = SwigTools.MakeIdentitySharedPtr(this);
		}

		internal IdentitySessionBase(IdentitySessionBaseSharedPtr identitySessionPtr)
		{
			swigCPtr = SwigTools.GetIdentitySessionBasePtr(identitySessionPtr);
			sharedPtr = identitySessionPtr;
		}

		internal static IntPtr getCPtr(IdentitySessionBase obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~IdentitySessionBase()
		{
			Dispose(false);
		}

		public virtual void Dispose()
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
					sharedPtr.Dispose();
					sharedPtr = null;
					swigCPtr = IntPtr.Zero;
					GC.SuppressFinalize(this);
				}
			}
			disposed = true;
		}

		public virtual string GetAccountId()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.IdentitySessionBase_GetAccountId(swigCPtr);
		}

		public virtual string GetSharedAccountId()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.IdentitySessionBase_GetSharedAccountId(swigCPtr);
		}

		public virtual string GetAccessTokenString()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.IdentitySessionBase_GetAccessTokenString(swigCPtr);
		}

		public virtual IdentitySessionParameters GetParams()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new IdentitySessionParameters(RCSSDKPINVOKE.IdentitySessionBase_GetParams(swigCPtr), false);
		}

		public void SetProfileField(string key, Variant data)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.IdentitySessionBase_SetProfileField(swigCPtr, key, Variant.getCPtr(data));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void SetProfileFields(Dictionary<string, Variant> data)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.IdentitySessionBase_SetProfileFields(swigCPtr, VariantDict.getCPtr(data.ToSwigDict()));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void ClearProfileFields()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.IdentitySessionBase_ClearProfileFields(swigCPtr);
		}

		public string GetProfileFieldsAsJson()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.IdentitySessionBase_GetProfileFieldsAsJson(swigCPtr);
		}
	}
}

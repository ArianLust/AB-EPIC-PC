using System;
using System.Collections.Generic;

namespace Rcs
{
	public class OtherPlayerData : IDisposable
	{
		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		internal OtherPlayerData(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		public OtherPlayerData(PlayerData arg0)
			: this(RCSSDKPINVOKE.new_OtherPlayerData_0(PlayerData.getCPtr(arg0)), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public OtherPlayerData(OtherPlayerData arg0)
			: this(RCSSDKPINVOKE.new_OtherPlayerData_1(getCPtr(arg0)), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		internal static IntPtr getCPtr(OtherPlayerData obj)
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

		~OtherPlayerData()
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
						RCSSDKPINVOKE.delete_OtherPlayerData(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public Dictionary<string, string> GetPublic()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			StringDict srcDict = new StringDict(RCSSDKPINVOKE.OtherPlayerData_GetPublic(swigCPtr), false);
			return srcDict.ToDictionary();
		}
	}
}

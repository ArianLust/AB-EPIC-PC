using System;

namespace Rcs
{
	public class OtherPlayer : IDisposable
	{
		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		internal OtherPlayer(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		public OtherPlayer(OtherPlayer arg0)
			: this(RCSSDKPINVOKE.new_OtherPlayer_1(getCPtr(arg0)), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		internal static IntPtr getCPtr(OtherPlayer obj)
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

		~OtherPlayer()
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
						RCSSDKPINVOKE.delete_OtherPlayer(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public string GetPlayerId()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.OtherPlayer_GetPlayerId(swigCPtr);
		}

		public OtherPlayerData GetData()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new OtherPlayerData(RCSSDKPINVOKE.OtherPlayer_GetData(swigCPtr), true);
		}
	}
}

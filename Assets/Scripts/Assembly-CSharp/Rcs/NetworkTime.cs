using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class NetworkTime : IDisposable
	{
		public delegate void SyncSuccessCallback(ulong time);

		public delegate void SyncErrorCallback(int errorCode, string message);

		private delegate void SwigDelegateNetworkTime_0(IntPtr cb, ulong time);

		private delegate void SwigDelegateNetworkTime_1(IntPtr cb, int errorCode, string message);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SwigDelegateNetworkTime_0 swigDelegate0;

		private SwigDelegateNetworkTime_1 swigDelegate1;

		internal NetworkTime(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public NetworkTime(IdentitySessionBase identity)
			: this(RCSSDKPINVOKE.new_NetworkTime(identity.SharedPtr.CPtr), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<NetworkTime> callInfo)
		{
			lock (this)
			{
				IntPtr intPtr = callInfo.Pin();
				pendingCallbacks.Add(intPtr);
				return intPtr;
			}
		}

		private void RemovePendingCallback(IntPtr callbackInfoId)
		{
			lock (this)
			{
				pendingCallbacks.Remove(callbackInfoId);
			}
		}

		public void Dispose()
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
			_DisposeUnmanaged();
			lock (this)
			{
				foreach (IntPtr pendingCallback in pendingCallbacks)
				{
					GCHandle.FromIntPtr(pendingCallback).Free();
				}
				pendingCallbacks.Clear();
			}
			disposed = true;
		}

		internal static IntPtr getCPtr(NetworkTime obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~NetworkTime()
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
						RCSSDKPINVOKE.delete_NetworkTime(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public void Sync(SyncSuccessCallback onSuccess, SyncErrorCallback onFailure)
		{
			AsyncCallInfo<NetworkTime> asyncCallInfo = new AsyncCallInfo<NetworkTime>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onFailure);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.NetworkTime_Sync(swigCPtr, intPtr, intPtr);
		}

		public ulong GetTime()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.NetworkTime_GetTime(swigCPtr);
		}

		public bool IsSync()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.NetworkTime_IsSync(swigCPtr);
		}

		private static void OnSyncSuccessCallback(SyncSuccessCallback cb, ulong time)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(time);
		}

		private static void OnSyncErrorCallback(SyncErrorCallback cb, int errorCode, string message)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(errorCode, message);
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnSyncSuccessCallback;
			swigDelegate1 = SwigDirectorOnSyncErrorCallback;
			RCSSDKPINVOKE.NetworkTime_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(NetworkTime));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateNetworkTime_0))]
		private static void SwigDirectorOnSyncSuccessCallback(IntPtr cb, ulong time)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.NetworkTime] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<NetworkTime> asyncCallInfo = (AsyncCallInfo<NetworkTime>)gCHandle.Target;
			SyncSuccessCallback handler = asyncCallInfo.GetHandler<SyncSuccessCallback>();
			try
			{
				OnSyncSuccessCallback(handler, time);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateNetworkTime_1))]
		private static void SwigDirectorOnSyncErrorCallback(IntPtr cb, int errorCode, string message)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.NetworkTime] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<NetworkTime> asyncCallInfo = (AsyncCallInfo<NetworkTime>)gCHandle.Target;
			SyncErrorCallback handler = asyncCallInfo.GetHandler<SyncErrorCallback>();
			try
			{
				OnSyncErrorCallback(handler, errorCode, message);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class Player : IDisposable
	{
		public enum ErrorCode
		{
			ErrorInvalidParameters = 0,
			ErrorConflict = 1,
			ErrorDuplicateNetwork = 2,
			ErrorNetworkFailure = 3,
			ErrorInvalidAccessToken = 4,
			ErrorOtherReason = 5
		}

		public delegate void FailureCallback(ErrorCode errorCode);

		public delegate void SuccessCallback();

		private delegate void SwigDelegatePlayer_0(IntPtr cb, int errorCode);

		private delegate void SwigDelegatePlayer_1(IntPtr cb);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private IdentitySessionBase identitySession;

		private SwigDelegatePlayer_0 swigDelegate0;

		private SwigDelegatePlayer_1 swigDelegate1;

		internal Player(IdentitySessionBase identity, IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
			identitySession = identity;
		}

		public Player(IdentitySessionBase identity)
			: this(identity, RCSSDKPINVOKE.new_Player_0(identity.SharedPtr.CPtr), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Player(Player arg0)
			: this(arg0.identitySession, RCSSDKPINVOKE.new_Player_0(getCPtr(arg0)), true)
		{
			identitySession = arg0.identitySession;
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<Player> callInfo)
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

		internal static IntPtr getCPtr(Player obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~Player()
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
						RCSSDKPINVOKE.delete_Player(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public void AddNetwork(NetworkCredentials credentials, SuccessCallback onSuccess, FailureCallback onFailure)
		{
			AsyncCallInfo<Player> asyncCallInfo = new AsyncCallInfo<Player>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onFailure);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Player_AddNetwork(swigCPtr, NetworkCredentials.getCPtr(credentials), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void RemoveNetwork(NetworkProvider network, SuccessCallback onSuccess, FailureCallback onFailure)
		{
			AsyncCallInfo<Player> asyncCallInfo = new AsyncCallInfo<Player>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onFailure);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Player_RemoveNetwork(swigCPtr, (int)network, intPtr, intPtr);
		}

		public string GetPlayerId()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Player_GetPlayerId(swigCPtr);
		}

		public string GetCustomerId()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Player_GetCustomerId(swigCPtr);
		}

		public Dictionary<NetworkProvider, string> GetNetworks()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			NetworkProviderDict srcDict = new NetworkProviderDict(RCSSDKPINVOKE.Player_GetNetworks(swigCPtr), false);
			return srcDict.ToDictionary();
		}

		public PlayerData GetData()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new PlayerData(RCSSDKPINVOKE.Player_GetData(swigCPtr), true);
		}

		public void SetData(PlayerData data, SuccessCallback onSuccess, FailureCallback onFailure)
		{
			AsyncCallInfo<Player> asyncCallInfo = new AsyncCallInfo<Player>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onFailure);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Player_SetData(swigCPtr, PlayerData.getCPtr(data), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public bool IsMigrated()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Player_IsMigrated(swigCPtr);
		}

		private static void OnFailureCallback(FailureCallback cb, ErrorCode errorCode)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(errorCode);
		}

		private static void OnSuccessCallback(SuccessCallback cb)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb();
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnFailureCallback;
			swigDelegate1 = SwigDirectorOnSuccessCallback;
			RCSSDKPINVOKE.Player_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(Player));
		}

		[MonoPInvokeCallback(typeof(SwigDelegatePlayer_0))]
		private static void SwigDirectorOnFailureCallback(IntPtr cb, int errorCode)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Player] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Player> asyncCallInfo = (AsyncCallInfo<Player>)gCHandle.Target;
			FailureCallback handler = asyncCallInfo.GetHandler<FailureCallback>();
			try
			{
				OnFailureCallback(handler, (ErrorCode)errorCode);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegatePlayer_1))]
		private static void SwigDirectorOnSuccessCallback(IntPtr cb)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Player] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Player> asyncCallInfo = (AsyncCallInfo<Player>)gCHandle.Target;
			SuccessCallback handler = asyncCallInfo.GetHandler<SuccessCallback>();
			try
			{
				OnSuccessCallback(handler);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}
	}
}

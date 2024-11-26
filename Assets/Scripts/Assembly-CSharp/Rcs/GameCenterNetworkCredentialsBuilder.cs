using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class GameCenterNetworkCredentialsBuilder : IDisposable
	{
		public delegate void AuthenticateSuccessCallback(NetworkCredentials credentials);

		public delegate void AuthenticateFailureCallback(string message);

		private delegate void SwigDelegateGameCenterNetworkCredentialsBuilder_0(IntPtr cb, IntPtr credentials);

		private delegate void SwigDelegateGameCenterNetworkCredentialsBuilder_1(IntPtr cb, string message);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SwigDelegateGameCenterNetworkCredentialsBuilder_0 swigDelegate0;

		private SwigDelegateGameCenterNetworkCredentialsBuilder_1 swigDelegate1;

		internal GameCenterNetworkCredentialsBuilder(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public GameCenterNetworkCredentialsBuilder()
			: this(RCSSDKPINVOKE.new_GameCenterNetworkCredentialsBuilder(), true)
		{
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<GameCenterNetworkCredentialsBuilder> callInfo)
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

		internal static IntPtr getCPtr(GameCenterNetworkCredentialsBuilder obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~GameCenterNetworkCredentialsBuilder()
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
						RCSSDKPINVOKE.delete_GameCenterNetworkCredentialsBuilder(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public static NetworkCredentials Create(string playerId, string bundleId, string salt, string publicKeyUrl, string signature, ulong timestamp)
		{
			NetworkCredentials result = new NetworkCredentials(RCSSDKPINVOKE.GameCenterNetworkCredentialsBuilder_Create(playerId, bundleId, salt, publicKeyUrl, signature, timestamp), true);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public void Authenticate(AuthenticateSuccessCallback successCallback, AuthenticateFailureCallback failureCallback)
		{
			AsyncCallInfo<GameCenterNetworkCredentialsBuilder> asyncCallInfo = new AsyncCallInfo<GameCenterNetworkCredentialsBuilder>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(successCallback);
			asyncCallInfo.AddHandler(failureCallback);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.GameCenterNetworkCredentialsBuilder_Authenticate(swigCPtr, intPtr, intPtr);
		}

		private static void OnAuthenticateSuccessCallback(AuthenticateSuccessCallback cb, NetworkCredentials credentials)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(new NetworkCredentials(credentials));
		}

		private static void OnAuthenticateFailureCallback(AuthenticateFailureCallback cb, string message)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(message);
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnAuthenticateSuccessCallback;
			swigDelegate1 = SwigDirectorOnAuthenticateFailureCallback;
			RCSSDKPINVOKE.GameCenterNetworkCredentialsBuilder_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(GameCenterNetworkCredentialsBuilder));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateGameCenterNetworkCredentialsBuilder_0))]
		private static void SwigDirectorOnAuthenticateSuccessCallback(IntPtr cb, IntPtr credentials)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.GameCenterNetworkCredentialsBuilder] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<GameCenterNetworkCredentialsBuilder> asyncCallInfo = (AsyncCallInfo<GameCenterNetworkCredentialsBuilder>)gCHandle.Target;
			AuthenticateSuccessCallback handler = asyncCallInfo.GetHandler<AuthenticateSuccessCallback>();
			try
			{
				OnAuthenticateSuccessCallback(handler, new NetworkCredentials(credentials, false));
			}
			finally
			{
				if (!"AuthenticateSuccessCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateGameCenterNetworkCredentialsBuilder_1))]
		private static void SwigDirectorOnAuthenticateFailureCallback(IntPtr cb, string message)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.GameCenterNetworkCredentialsBuilder] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<GameCenterNetworkCredentialsBuilder> asyncCallInfo = (AsyncCallInfo<GameCenterNetworkCredentialsBuilder>)gCHandle.Target;
			AuthenticateFailureCallback handler = asyncCallInfo.GetHandler<AuthenticateFailureCallback>();
			try
			{
				OnAuthenticateFailureCallback(handler, message);
			}
			finally
			{
				if (!"AuthenticateFailureCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class AppConfiguration : IDisposable
	{
		public enum ErrorCode
		{
			ErrorSignatureMismatch = 0,
			ErrorOther = 1
		}

		public delegate void SuccessCallback(string json);

		public delegate void ErrorCallback(ErrorCode status, string message);

		private delegate void SwigDelegateAppConfiguration_0(IntPtr cb, string json);

		private delegate void SwigDelegateAppConfiguration_1(IntPtr cb, int status, string message);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SwigDelegateAppConfiguration_0 swigDelegate0;

		private SwigDelegateAppConfiguration_1 swigDelegate1;

		internal AppConfiguration(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public AppConfiguration(IdentitySessionBase identity)
			: this(RCSSDKPINVOKE.new_AppConfiguration(identity.SharedPtr.CPtr), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<AppConfiguration> callInfo)
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

		internal static IntPtr getCPtr(AppConfiguration obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~AppConfiguration()
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
						RCSSDKPINVOKE.delete_AppConfiguration(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public void Fetch(SuccessCallback onSuccess, ErrorCallback onError)
		{
			AsyncCallInfo<AppConfiguration> asyncCallInfo = new AsyncCallInfo<AppConfiguration>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.AppConfiguration_Fetch(swigCPtr, intPtr, intPtr);
		}

		private static void OnSuccessCallback(SuccessCallback cb, string json)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(json);
		}

		private static void OnErrorCallback(ErrorCallback cb, ErrorCode status, string message)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(status, message);
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnSuccessCallback;
			swigDelegate1 = SwigDirectorOnErrorCallback;
			RCSSDKPINVOKE.AppConfiguration_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(AppConfiguration));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateAppConfiguration_0))]
		private static void SwigDirectorOnSuccessCallback(IntPtr cb, string json)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.AppConfiguration] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<AppConfiguration> asyncCallInfo = (AsyncCallInfo<AppConfiguration>)gCHandle.Target;
			SuccessCallback handler = asyncCallInfo.GetHandler<SuccessCallback>();
			try
			{
				OnSuccessCallback(handler, json);
			}
			finally
			{
				if (!"SuccessCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateAppConfiguration_1))]
		private static void SwigDirectorOnErrorCallback(IntPtr cb, int status, string message)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.AppConfiguration] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<AppConfiguration> asyncCallInfo = (AsyncCallInfo<AppConfiguration>)gCHandle.Target;
			ErrorCallback handler = asyncCallInfo.GetHandler<ErrorCallback>();
			try
			{
				OnErrorCallback(handler, (ErrorCode)status, message);
			}
			finally
			{
				if (!"ErrorCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}
	}
}

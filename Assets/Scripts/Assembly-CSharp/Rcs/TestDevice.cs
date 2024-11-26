using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class TestDevice : IDisposable
	{
		public enum LogLevel
		{
			LoglevelError = 1,
			LoglevelWarning = 2,
			LoglevelInfo = 3,
			LoglevelDebug = 4
		}

		public delegate void ResultCallback(bool result);

		private delegate void SwigDelegateTestDevice_0(IntPtr cb, bool result);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SwigDelegateTestDevice_0 swigDelegate0;

		internal TestDevice(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public TestDevice(IdentitySessionBase identity)
			: this(RCSSDKPINVOKE.new_TestDevice(identity.SharedPtr.CPtr), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<TestDevice> callInfo)
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

		internal static IntPtr getCPtr(TestDevice obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~TestDevice()
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
						RCSSDKPINVOKE.delete_TestDevice(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public void RegisterDevice(string name, ResultCallback resultCallback)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<TestDevice> callInfo = new AsyncCallInfo<TestDevice>(this, resultCallback);
			IntPtr jarg = AddPendingCallback(callInfo);
			RCSSDKPINVOKE.TestDevice_RegisterDevice(swigCPtr, name, jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void UnregisterDevice(string name, ResultCallback resultCallback)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<TestDevice> callInfo = new AsyncCallInfo<TestDevice>(this, resultCallback);
			IntPtr jarg = AddPendingCallback(callInfo);
			RCSSDKPINVOKE.TestDevice_UnregisterDevice(swigCPtr, name, jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public static string GetDeviceName()
		{
			return RCSSDKPINVOKE.TestDevice_GetDeviceName();
		}

		public static void ServerLog(string tag, LogLevel level, string message)
		{
			RCSSDKPINVOKE.TestDevice_ServerLog(tag, (int)level, message);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		private static void OnResultCallback(ResultCallback cb, bool result)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(result);
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnResultCallback;
			RCSSDKPINVOKE.TestDevice_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(TestDevice));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateTestDevice_0))]
		private static void SwigDirectorOnResultCallback(IntPtr cb, bool result)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.TestDevice] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<TestDevice> asyncCallInfo = (AsyncCallInfo<TestDevice>)gCHandle.Target;
			ResultCallback handler = asyncCallInfo.GetHandler<ResultCallback>();
			try
			{
				OnResultCallback(handler, result);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}
	}
}

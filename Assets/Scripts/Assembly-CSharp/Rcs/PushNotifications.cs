using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class PushNotifications : IDisposable
	{
		public class Info : IDisposable
		{
			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			public string ServiceId
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.PushNotifications_Info_ServiceId_get(swigCPtr);
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
					RCSSDKPINVOKE.PushNotifications_Info_ServiceId_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string Content
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.PushNotifications_Info_Content_get(swigCPtr);
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
					RCSSDKPINVOKE.PushNotifications_Info_Content_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			internal Info(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public Info()
				: this(RCSSDKPINVOKE.new_PushNotifications_Info(), true)
			{
			}

			internal static IntPtr getCPtr(Info obj)
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

			~Info()
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
							RCSSDKPINVOKE.delete_PushNotifications_Info(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}
		}

		public delegate void SuccessCallback();

		public delegate void ErrorCallback(int errorCode, string message);

		private delegate void SwigDelegatePushNotifications_0(IntPtr cb);

		private delegate void SwigDelegatePushNotifications_1(IntPtr cb, int errorCode, string message);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SwigDelegatePushNotifications_0 swigDelegate0;

		private SwigDelegatePushNotifications_1 swigDelegate1;

		internal PushNotifications(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public PushNotifications(IdentitySessionBase identity, string deviceToken)
			: this(RCSSDKPINVOKE.new_PushNotifications(identity.SharedPtr.CPtr, deviceToken), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<PushNotifications> callInfo)
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

		internal static IntPtr getCPtr(PushNotifications obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~PushNotifications()
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
						RCSSDKPINVOKE.delete_PushNotifications(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public void RegisterDevice(SuccessCallback onSuccess, ErrorCallback onError)
		{
			AsyncCallInfo<PushNotifications> asyncCallInfo = new AsyncCallInfo<PushNotifications>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.PushNotifications_RegisterDevice(swigCPtr, intPtr, intPtr);
		}

		public void UnregisterDevice(SuccessCallback onSuccess, ErrorCallback onError)
		{
			AsyncCallInfo<PushNotifications> asyncCallInfo = new AsyncCallInfo<PushNotifications>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.PushNotifications_UnregisterDevice(swigCPtr, intPtr, intPtr);
		}

		public string GetDeviceToken()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.PushNotifications_GetDeviceToken(swigCPtr);
		}

		public static string ServiceIdFromRemoteNotification(string payloadAsJSON)
		{
			string result = RCSSDKPINVOKE.PushNotifications_ServiceIdFromRemoteNotification(payloadAsJSON);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public static Info ServiceInfoFromRemoteNotification(string payloadAsJSON)
		{
			Info result = new Info(RCSSDKPINVOKE.PushNotifications_ServiceInfoFromRemoteNotification(payloadAsJSON), true);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		private static void OnSuccessCallback(SuccessCallback cb)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb();
		}

		private static void OnErrorCallback(ErrorCallback cb, int errorCode, string message)
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
			swigDelegate0 = SwigDirectorOnSuccessCallback;
			swigDelegate1 = SwigDirectorOnErrorCallback;
			RCSSDKPINVOKE.PushNotifications_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(PushNotifications));
		}

		[MonoPInvokeCallback(typeof(SwigDelegatePushNotifications_0))]
		private static void SwigDirectorOnSuccessCallback(IntPtr cb)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.PushNotifications] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<PushNotifications> asyncCallInfo = (AsyncCallInfo<PushNotifications>)gCHandle.Target;
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

		[MonoPInvokeCallback(typeof(SwigDelegatePushNotifications_1))]
		private static void SwigDirectorOnErrorCallback(IntPtr cb, int errorCode, string message)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.PushNotifications] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<PushNotifications> asyncCallInfo = (AsyncCallInfo<PushNotifications>)gCHandle.Target;
			ErrorCallback handler = asyncCallInfo.GetHandler<ErrorCallback>();
			try
			{
				OnErrorCallback(handler, errorCode, message);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class Storage : IDisposable
	{
		public enum ErrorCode
		{
			ErrorMalformedRequest = 0,
			ErrorNoSuchKey = 1,
			ErrorConflict = 2,
			ErrorServiceNotAvailable = 3,
			ErrorNetworkFailure = 4,
			ErrorUndecodableCompressedData = 5
		}

		public enum UploadMode
		{
			ModeRaw = 1,
			ModeCompressed = 2
		}

		public enum Scope
		{
			ScopeRawKeys = 0,
			ScopeClientWide = 1
		}

		public delegate void DataGetErrorCallback(string key, ErrorCode errorCode);

		public delegate void DataByAccountIdGetCallback(string key, Dictionary<string, string> accountToValueMap);

		public delegate string DataSetConflictCallback(string key, string localValue, string remoteValue);

		public delegate void DataGetCallback(string key, string value);

		public delegate void DataSetFailedCallback(string key, ErrorCode errorCode);

		public delegate void DataSetCallback(string key);

		private delegate void SwigDelegateStorage_0(IntPtr cb, string key, int errorCode);

		private delegate void SwigDelegateStorage_1(IntPtr cb, string key, IntPtr accountToValueMap);

		private delegate string SwigDelegateStorage_2(IntPtr cb, string key, string localValue, string remoteValue);

		private delegate void SwigDelegateStorage_3(IntPtr cb, string key, string value);

		private delegate void SwigDelegateStorage_4(IntPtr cb, string key, int errorCode);

		private delegate void SwigDelegateStorage_5(IntPtr cb, string key);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SwigDelegateStorage_0 swigDelegate0;

		private SwigDelegateStorage_1 swigDelegate1;

		private SwigDelegateStorage_2 swigDelegate2;

		private SwigDelegateStorage_3 swigDelegate3;

		private SwigDelegateStorage_4 swigDelegate4;

		private SwigDelegateStorage_5 swigDelegate5;

		internal Storage(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public Storage(IdentitySessionBase identity, Scope scope)
			: this(RCSSDKPINVOKE.new_Storage_0(identity.SharedPtr.CPtr, (int)scope), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Storage(IdentitySessionBase identity)
			: this(RCSSDKPINVOKE.new_Storage_1(identity.SharedPtr.CPtr), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<Storage> callInfo)
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

		internal static IntPtr getCPtr(Storage obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~Storage()
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
						RCSSDKPINVOKE.delete_Storage(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public void Set(string key, string value, DataSetCallback onSuccess, DataSetFailedCallback onFailure, DataSetConflictCallback onConflict, UploadMode mode)
		{
			AsyncCallInfo<Storage> asyncCallInfo = new AsyncCallInfo<Storage>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onFailure);
			asyncCallInfo.AddHandler(onConflict);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Storage_Set_0(swigCPtr, key, value, intPtr, intPtr, intPtr, (int)mode);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Set(string key, string value, DataSetCallback onSuccess, DataSetFailedCallback onFailure, DataSetConflictCallback onConflict)
		{
			AsyncCallInfo<Storage> asyncCallInfo = new AsyncCallInfo<Storage>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onFailure);
			asyncCallInfo.AddHandler(onConflict);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Storage_Set_1(swigCPtr, key, value, intPtr, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Get(string key, DataGetCallback onSuccess, DataGetErrorCallback onError)
		{
			AsyncCallInfo<Storage> asyncCallInfo = new AsyncCallInfo<Storage>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Storage_Get_0(swigCPtr, key, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Get(List<string> accountIds, string key, DataByAccountIdGetCallback onSuccess, DataGetErrorCallback onError)
		{
			AsyncCallInfo<Storage> asyncCallInfo = new AsyncCallInfo<Storage>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Storage_Get_1(swigCPtr, StringList.getCPtr(new StringList(accountIds)), key, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		private static void OnDataGetErrorCallback(DataGetErrorCallback cb, string key, ErrorCode errorCode)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(key, errorCode);
		}

		private static void OnDataByAccountIdGetCallback(DataByAccountIdGetCallback cb, string key, StringDict accountToValueMap)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(key, accountToValueMap.ToDictionary());
		}

		private static string OnDataSetConflictCallback(DataSetConflictCallback cb, string key, string localValue, string remoteValue)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			return cb(key, localValue, remoteValue);
		}

		private static void OnDataGetCallback(DataGetCallback cb, string key, string value)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(key, value);
		}

		private static void OnDataSetFailedCallback(DataSetFailedCallback cb, string key, ErrorCode errorCode)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(key, errorCode);
		}

		private static void OnDataSetCallback(DataSetCallback cb, string key)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(key);
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnDataGetErrorCallback;
			swigDelegate1 = SwigDirectorOnDataByAccountIdGetCallback;
			swigDelegate2 = SwigDirectorOnDataSetConflictCallback;
			swigDelegate3 = SwigDirectorOnDataGetCallback;
			swigDelegate4 = SwigDirectorOnDataSetFailedCallback;
			swigDelegate5 = SwigDirectorOnDataSetCallback;
			RCSSDKPINVOKE.Storage_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1), Marshal.GetFunctionPointerForDelegate(swigDelegate2), Marshal.GetFunctionPointerForDelegate(swigDelegate3), Marshal.GetFunctionPointerForDelegate(swigDelegate4), Marshal.GetFunctionPointerForDelegate(swigDelegate5));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(Storage));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateStorage_0))]
		private static void SwigDirectorOnDataGetErrorCallback(IntPtr cb, string key, int errorCode)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Storage] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Storage> asyncCallInfo = (AsyncCallInfo<Storage>)gCHandle.Target;
			DataGetErrorCallback handler = asyncCallInfo.GetHandler<DataGetErrorCallback>();
			OnDataGetErrorCallback(handler, key, (ErrorCode)errorCode);
			gCHandle.Free();
			asyncCallInfo.Service.RemovePendingCallback(cb);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateStorage_1))]
		private static void SwigDirectorOnDataByAccountIdGetCallback(IntPtr cb, string key, IntPtr accountToValueMap)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Storage] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Storage> asyncCallInfo = (AsyncCallInfo<Storage>)gCHandle.Target;
			DataByAccountIdGetCallback handler = asyncCallInfo.GetHandler<DataByAccountIdGetCallback>();
			OnDataByAccountIdGetCallback(handler, key, new StringDict(accountToValueMap, false));
			gCHandle.Free();
			asyncCallInfo.Service.RemovePendingCallback(cb);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateStorage_2))]
		private static string SwigDirectorOnDataSetConflictCallback(IntPtr cb, string key, string localValue, string remoteValue)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Storage] Ignoring callback from previously disposed object instance");
				return string.Empty;
			}
			AsyncCallInfo<Storage> asyncCallInfo = (AsyncCallInfo<Storage>)gCHandle.Target;
			DataSetConflictCallback handler = asyncCallInfo.GetHandler<DataSetConflictCallback>();
			return OnDataSetConflictCallback(handler, key, localValue, remoteValue);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateStorage_3))]
		private static void SwigDirectorOnDataGetCallback(IntPtr cb, string key, string value)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Storage] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Storage> asyncCallInfo = (AsyncCallInfo<Storage>)gCHandle.Target;
			DataGetCallback handler = asyncCallInfo.GetHandler<DataGetCallback>();
			OnDataGetCallback(handler, key, value);
			gCHandle.Free();
			asyncCallInfo.Service.RemovePendingCallback(cb);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateStorage_4))]
		private static void SwigDirectorOnDataSetFailedCallback(IntPtr cb, string key, int errorCode)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Storage] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Storage> asyncCallInfo = (AsyncCallInfo<Storage>)gCHandle.Target;
			DataSetFailedCallback handler = asyncCallInfo.GetHandler<DataSetFailedCallback>();
			OnDataSetFailedCallback(handler, key, (ErrorCode)errorCode);
			gCHandle.Free();
			asyncCallInfo.Service.RemovePendingCallback(cb);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateStorage_5))]
		private static void SwigDirectorOnDataSetCallback(IntPtr cb, string key)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Storage] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Storage> asyncCallInfo = (AsyncCallInfo<Storage>)gCHandle.Target;
			DataSetCallback handler = asyncCallInfo.GetHandler<DataSetCallback>();
			OnDataSetCallback(handler, key);
			gCHandle.Free();
			asyncCallInfo.Service.RemovePendingCallback(cb);
		}
	}
}

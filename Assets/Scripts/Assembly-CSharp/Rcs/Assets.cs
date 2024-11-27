using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class Assets : IDisposable
	{
		public class Info : IDisposable
		{
			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			public string Name
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Assets_Info_Name_get(swigCPtr);
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
					RCSSDKPINVOKE.Assets_Info_Name_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string Hash
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Assets_Info_Hash_get(swigCPtr);
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
					RCSSDKPINVOKE.Assets_Info_Hash_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string CdnUrl
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Assets_Info_CdnUrl_get(swigCPtr);
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
					RCSSDKPINVOKE.Assets_Info_CdnUrl_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public long Size
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return RCSSDKPINVOKE.Assets_Info_Size_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Assets_Info_Size_set(swigCPtr, value);
				}
			}

			internal Info(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public Info()
				: this(RCSSDKPINVOKE.new_Assets_Info(), true)
			{
			}

			public Info(Info info)
				: this(RCSSDKPINVOKE.new_Assets_Info(), true)
			{
				Name = info.Name;
				Hash = info.Hash;
				CdnUrl = info.CdnUrl;
				Size = info.Size;
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
							RCSSDKPINVOKE.delete_Assets_Info(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}

			public override string ToString()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Assets_Info_ToString(swigCPtr);
			}
		}

		public class AssetsConfiguration : IDisposable
		{
			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			public SegmentBackend SegmentBackend
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return (SegmentBackend)RCSSDKPINVOKE.Assets_AssetsConfiguration_SegmentBackend_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Assets_AssetsConfiguration_SegmentBackend_set(swigCPtr, (int)value);
				}
			}

			public bool EnableResume
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return RCSSDKPINVOKE.Assets_AssetsConfiguration_EnableResume_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Assets_AssetsConfiguration_EnableResume_set(swigCPtr, value);
				}
			}

			internal AssetsConfiguration(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public AssetsConfiguration()
				: this(RCSSDKPINVOKE.new_Assets_AssetsConfiguration_0(), true)
			{
			}

			public AssetsConfiguration(SegmentBackend segmentBackend, bool enableResume)
				: this(RCSSDKPINVOKE.new_Assets_AssetsConfiguration_1((int)segmentBackend, enableResume), true)
			{
			}

			public AssetsConfiguration(SegmentBackend segmentBackend)
				: this(RCSSDKPINVOKE.new_Assets_AssetsConfiguration_2((int)segmentBackend), true)
			{
			}

			internal static IntPtr getCPtr(AssetsConfiguration obj)
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

			~AssetsConfiguration()
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
							RCSSDKPINVOKE.delete_Assets_AssetsConfiguration(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}
		}

		public enum ErrorCode
		{
			ErrorAssetNotFound = -1,
			ErrorOther = -100
		}

		public enum SegmentBackend
		{
			SegmentBackendSegmentation = 0,
			SegmentBackendSupermoon = 1,
			SegmentBackendProfiler = 2
		}

		public delegate void RemoveObsoleteAssetsFailedCallback(ErrorCode status, string message);

		public delegate void ProgressCallback(Dictionary<string, string> downloaded, List<string> loading, double totalToDownload, double nowDownloaded);

		public delegate void LoadMetadataSuccessCallback(Dictionary<string, Info> assets);

		public delegate void SuccessCallback(Dictionary<string, string> assets);

		public delegate void ErrorCallback(List<string> assetList, List<string> assetsMissing, ErrorCode status, string message);

		public delegate void RemoveObsoleteAssetsCompleteCallback(List<string> removedFiles);

		private delegate void SwigDelegateAssets_0(IntPtr cb, int status, string message);

		private delegate void SwigDelegateAssets_1(IntPtr cb, IntPtr downloaded, IntPtr loading, double totalToDownload, double nowDownloaded);

		private delegate void SwigDelegateAssets_2(IntPtr cb, IntPtr assets);

		private delegate void SwigDelegateAssets_3(IntPtr cb, IntPtr assets);

		private delegate void SwigDelegateAssets_4(IntPtr cb, IntPtr assetList, IntPtr assetsMissing, int status, string message);

		private delegate void SwigDelegateAssets_5(IntPtr cb, IntPtr removedFiles);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SwigDelegateAssets_0 swigDelegate0;

		private SwigDelegateAssets_1 swigDelegate1;

		private SwigDelegateAssets_2 swigDelegate2;

		private SwigDelegateAssets_3 swigDelegate3;

		private SwigDelegateAssets_4 swigDelegate4;

		private SwigDelegateAssets_5 swigDelegate5;

		internal Assets(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public Assets(IdentitySessionBase identity, SegmentBackend segmentBackend)
			: this(RCSSDKPINVOKE.new_Assets_0(identity.SharedPtr.CPtr, (int)segmentBackend), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Assets(IdentitySessionBase identity)
			: this(RCSSDKPINVOKE.new_Assets_1(identity.SharedPtr.CPtr), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Assets(IdentitySessionBase identity, AssetsConfiguration configuration)
			: this(RCSSDKPINVOKE.new_Assets_2(identity.SharedPtr.CPtr, AssetsConfiguration.getCPtr(configuration)), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<Assets> callInfo)
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

		internal static IntPtr getCPtr(Assets obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~Assets()
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
						RCSSDKPINVOKE.delete_Assets(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public string Get(string assetName)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			string result = RCSSDKPINVOKE.Assets_Get(swigCPtr, assetName);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public void Load(List<string> assetList, SuccessCallback onSuccess, ErrorCallback onError, ProgressCallback onProgress)
		{
			AsyncCallInfo<Assets> asyncCallInfo = new AsyncCallInfo<Assets>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onError);
			asyncCallInfo.AddHandler(onProgress);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Assets_Load_0(swigCPtr, StringList.getCPtr(new StringList(assetList)), intPtr, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Load(List<string> assetList, SuccessCallback onSuccess, ErrorCallback onError)
		{
			AsyncCallInfo<Assets> asyncCallInfo = new AsyncCallInfo<Assets>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Assets_Load_1(swigCPtr, StringList.getCPtr(new StringList(assetList)), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Load(List<string> assetList, int timeoutMs, SuccessCallback onSuccess, ErrorCallback onError, ProgressCallback onProgress)
		{
			AsyncCallInfo<Assets> asyncCallInfo = new AsyncCallInfo<Assets>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onError);
			asyncCallInfo.AddHandler(onProgress);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Assets_Load_2(swigCPtr, StringList.getCPtr(new StringList(assetList)), timeoutMs, intPtr, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Load(List<string> assetList, int timeoutMs, SuccessCallback onSuccess, ErrorCallback onError)
		{
			AsyncCallInfo<Assets> asyncCallInfo = new AsyncCallInfo<Assets>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Assets_Load_3(swigCPtr, StringList.getCPtr(new StringList(assetList)), timeoutMs, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void LoadMetadata(List<string> assetList, LoadMetadataSuccessCallback onSuccess, ErrorCallback onError)
		{
			AsyncCallInfo<Assets> asyncCallInfo = new AsyncCallInfo<Assets>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Assets_LoadMetadata_0(swigCPtr, StringList.getCPtr(new StringList(assetList)), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void LoadMetadata(LoadMetadataSuccessCallback onSuccess, ErrorCallback onError)
		{
			AsyncCallInfo<Assets> asyncCallInfo = new AsyncCallInfo<Assets>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Assets_LoadMetadata_1(swigCPtr, intPtr, intPtr);
		}

		public string GetChecksum(string assetName)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			string result = RCSSDKPINVOKE.Assets_GetChecksum(swigCPtr, assetName);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public void RemoveObsoleteAssets(RemoveObsoleteAssetsCompleteCallback onComplete, RemoveObsoleteAssetsFailedCallback onError)
		{
			AsyncCallInfo<Assets> asyncCallInfo = new AsyncCallInfo<Assets>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onComplete);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Assets_RemoveObsoleteAssets_0(swigCPtr, intPtr, intPtr);
		}

		public void RemoveObsoleteAssets(RemoveObsoleteAssetsCompleteCallback onComplete)
		{
			AsyncCallInfo<Assets> asyncCallInfo = new AsyncCallInfo<Assets>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onComplete);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Assets_RemoveObsoleteAssets_1(swigCPtr, jarg);
		}

		public void RemoveObsoleteAssets()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Assets_RemoveObsoleteAssets_2(swigCPtr);
		}

		private static void OnRemoveObsoleteAssetsFailedCallback(RemoveObsoleteAssetsFailedCallback cb, ErrorCode status, string message)
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

		private static void OnProgressCallback(ProgressCallback cb, Dictionary<string, string> downloaded, List<string> loading, double totalToDownload, double nowDownloaded)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(downloaded, loading, totalToDownload, nowDownloaded);
		}

		private static void OnLoadMetadataSuccessCallback(LoadMetadataSuccessCallback cb, Dictionary<string, Info> assets)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(assets);
		}

		private static void OnSuccessCallback(SuccessCallback cb, Dictionary<string, string> assets)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(assets);
		}

		private static void OnErrorCallback(ErrorCallback cb, List<string> assetList, List<string> assetsMissing, ErrorCode status, string message)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(assetList, assetsMissing, status, message);
		}

		private static void OnRemoveObsoleteAssetsCompleteCallback(RemoveObsoleteAssetsCompleteCallback cb, List<string> removedFiles)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(removedFiles);
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnRemoveObsoleteAssetsFailedCallback;
			swigDelegate1 = SwigDirectorOnProgressCallback;
			swigDelegate2 = SwigDirectorOnLoadMetadataSuccessCallback;
			swigDelegate3 = SwigDirectorOnSuccessCallback;
			swigDelegate4 = SwigDirectorOnErrorCallback;
			swigDelegate5 = SwigDirectorOnRemoveObsoleteAssetsCompleteCallback;
			RCSSDKPINVOKE.Assets_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1), Marshal.GetFunctionPointerForDelegate(swigDelegate2), Marshal.GetFunctionPointerForDelegate(swigDelegate3), Marshal.GetFunctionPointerForDelegate(swigDelegate4), Marshal.GetFunctionPointerForDelegate(swigDelegate5));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(Assets));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateAssets_0))]
		private static void SwigDirectorOnRemoveObsoleteAssetsFailedCallback(IntPtr cb, int status, string message)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Assets] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Assets> asyncCallInfo = (AsyncCallInfo<Assets>)gCHandle.Target;
			RemoveObsoleteAssetsFailedCallback handler = asyncCallInfo.GetHandler<RemoveObsoleteAssetsFailedCallback>();
			try
			{
				OnRemoveObsoleteAssetsFailedCallback(handler, (ErrorCode)status, message);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateAssets_1))]
		private static void SwigDirectorOnProgressCallback(IntPtr cb, IntPtr downloaded, IntPtr loading, double totalToDownload, double nowDownloaded)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Assets] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Assets> asyncCallInfo = (AsyncCallInfo<Assets>)gCHandle.Target;
			ProgressCallback handler = asyncCallInfo.GetHandler<ProgressCallback>();
			OnProgressCallback(handler, new StringDict(downloaded, false).ToDictionary(), new StringList(loading, false).ToList(), totalToDownload, nowDownloaded);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateAssets_2))]
		private static void SwigDirectorOnLoadMetadataSuccessCallback(IntPtr cb, IntPtr assets)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Assets] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Assets> asyncCallInfo = (AsyncCallInfo<Assets>)gCHandle.Target;
			LoadMetadataSuccessCallback handler = asyncCallInfo.GetHandler<LoadMetadataSuccessCallback>();
			try
			{
				OnLoadMetadataSuccessCallback(handler, new AssetsInfoDict(assets, false).ToDictionary());
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateAssets_3))]
		private static void SwigDirectorOnSuccessCallback(IntPtr cb, IntPtr assets)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Assets] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Assets> asyncCallInfo = (AsyncCallInfo<Assets>)gCHandle.Target;
			SuccessCallback handler = asyncCallInfo.GetHandler<SuccessCallback>();
			try
			{
				OnSuccessCallback(handler, new StringDict(assets, false).ToDictionary());
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateAssets_4))]
		private static void SwigDirectorOnErrorCallback(IntPtr cb, IntPtr assetList, IntPtr assetsMissing, int status, string message)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Assets] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Assets> asyncCallInfo = (AsyncCallInfo<Assets>)gCHandle.Target;
			ErrorCallback handler = asyncCallInfo.GetHandler<ErrorCallback>();
			try
			{
				OnErrorCallback(handler, new StringList(assetList, false).ToList(), new StringList(assetsMissing, false).ToList(), (ErrorCode)status, message);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateAssets_5))]
		private static void SwigDirectorOnRemoveObsoleteAssetsCompleteCallback(IntPtr cb, IntPtr removedFiles)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Assets] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Assets> asyncCallInfo = (AsyncCallInfo<Assets>)gCHandle.Target;
			RemoveObsoleteAssetsCompleteCallback handler = asyncCallInfo.GetHandler<RemoveObsoleteAssetsCompleteCallback>();
			try
			{
				OnRemoveObsoleteAssetsCompleteCallback(handler, new StringList(removedFiles, false).ToList());
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}
	}
}

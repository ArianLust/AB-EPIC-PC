using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class Channel : IDisposable
	{
		public class Params : IDisposable
		{
			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			public string Locale
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Channel_Params_Locale_get(swigCPtr);
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
					RCSSDKPINVOKE.Channel_Params_Locale_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public int Width
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return RCSSDKPINVOKE.Channel_Params_Width_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Channel_Params_Width_set(swigCPtr, value);
				}
			}

			public int Height
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return RCSSDKPINVOKE.Channel_Params_Height_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Channel_Params_Height_set(swigCPtr, value);
				}
			}

			public string PushNotificationContent
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Channel_Params_PushNotificationContent_get(swigCPtr);
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
					RCSSDKPINVOKE.Channel_Params_PushNotificationContent_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string EntryPoint
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Channel_Params_EntryPoint_get(swigCPtr);
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
					RCSSDKPINVOKE.Channel_Params_EntryPoint_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public bool SeparateVideoplayerActivityOnAndroid
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return RCSSDKPINVOKE.Channel_Params_SeparateVideoplayerActivityOnAndroid_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Channel_Params_SeparateVideoplayerActivityOnAndroid_set(swigCPtr, value);
				}
			}

			public string GroupId
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Channel_Params_GroupId_get(swigCPtr);
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
					RCSSDKPINVOKE.Channel_Params_GroupId_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string ChannelId
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Channel_Params_ChannelId_get(swigCPtr);
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
					RCSSDKPINVOKE.Channel_Params_ChannelId_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string VideoId
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Channel_Params_VideoId_get(swigCPtr);
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
					RCSSDKPINVOKE.Channel_Params_VideoId_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			internal Params(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public Params()
				: this(RCSSDKPINVOKE.new_Channel_Params(), true)
			{
			}

			internal static IntPtr getCPtr(Params obj)
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

			~Params()
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
							RCSSDKPINVOKE.delete_Channel_Params(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}
		}

		public enum AudioAction
		{
			Create = 0,
			Play = 1,
			Release = 2
		}

		public enum LoadResult
		{
			Success = 0,
			Cancelled = 1,
			Redirected = 2,
			Failed = 3,
			Closed = 4
		}

		public delegate void ChannelAudioHandler(AudioAction audioAction, string filename, string id);

		public delegate void ChannelLoadedHandler(LoadResult loadResult);

		private delegate void SwigDelegateChannel_0(IntPtr cb, int audioAction, string filename, string id);

		private delegate void SwigDelegateChannel_1(IntPtr cb, int loadResult);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SwigDelegateChannel_0 swigDelegate0;

		private SwigDelegateChannel_1 swigDelegate1;

		private GCHandle channelAudioGCHandle;

		private GCHandle channelLoaderGCHandler;

		internal Channel(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public Channel(IdentitySessionBase identity, string clientId)
			: this(RCSSDKPINVOKE.new_Channel_1(identity.SharedPtr.CPtr, clientId), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Channel(IdentitySessionBase identity)
			: this(RCSSDKPINVOKE.new_Channel_2(identity.SharedPtr.CPtr), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<Channel> callInfo)
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
				if (channelLoaderGCHandler.IsAllocated)
				{
					channelLoaderGCHandler.Free();
				}
				if (channelAudioGCHandle.IsAllocated)
				{
					channelAudioGCHandle.Free();
				}
				foreach (IntPtr pendingCallback in pendingCallbacks)
				{
					GCHandle.FromIntPtr(pendingCallback).Free();
				}
				pendingCallbacks.Clear();
			}
			disposed = true;
		}

		internal static IntPtr getCPtr(Channel obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~Channel()
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
						RCSSDKPINVOKE.delete_Channel(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public void SetClientId(string clientId)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Channel_SetClientId(swigCPtr, clientId);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void SetChannelAudioCallback(ChannelAudioHandler audioHandler)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			IntPtr jarg = IntPtr.Zero;
			if (channelAudioGCHandle.IsAllocated)
			{
				channelAudioGCHandle.Free();
			}
			if (audioHandler != null)
			{
				channelAudioGCHandle = GCHandle.Alloc(audioHandler);
				jarg = GCHandle.ToIntPtr(channelAudioGCHandle);
			}
			RCSSDKPINVOKE.Channel_SetChannelAudioCallback(swigCPtr, jarg);
		}

		public void SetDefaultAdsActionHandler(Ads ads)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Channel_SetDefaultAdsActionHandler(swigCPtr, Ads.getCPtr(ads));
		}

		public void OpenChannelView(Params arg0, ChannelLoadedHandler loadedHandler)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			IntPtr jarg = IntPtr.Zero;
			if (channelLoaderGCHandler.IsAllocated)
			{
				channelLoaderGCHandler.Free();
			}
			if (loadedHandler != null)
			{
				channelLoaderGCHandler = GCHandle.Alloc(loadedHandler);
				jarg = GCHandle.ToIntPtr(channelLoaderGCHandler);
			}
			RCSSDKPINVOKE.Channel_OpenChannelView(swigCPtr, Params.getCPtr(arg0), jarg);
		}

		public void CancelChannelViewLoading()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Channel_CancelChannelViewLoading(swigCPtr);
		}

		public bool IsChannelViewOpened()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Channel_IsChannelViewOpened(swigCPtr);
		}

		public void NavigateBackChannelView()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Channel_NavigateBackChannelView(swigCPtr);
		}

		public void LoadFromUrl(string url, int width, int height, string locale, string pushNotificationContent, string entryPoint, bool separateVideoplayerActivityOnAndroid, string clientId, string referrerAppUrl)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Channel_LoadFromUrl_0(swigCPtr, url, width, height, locale, pushNotificationContent, entryPoint, separateVideoplayerActivityOnAndroid, clientId, referrerAppUrl);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void LoadFromUrl(string url, int width, int height, string locale, string pushNotificationContent, string entryPoint, bool separateVideoplayerActivityOnAndroid, string clientId)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Channel_LoadFromUrl_1(swigCPtr, url, width, height, locale, pushNotificationContent, entryPoint, separateVideoplayerActivityOnAndroid, clientId);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void LoadFromUrl(string url, int width, int height, string locale, string pushNotificationContent, string entryPoint, bool separateVideoplayerActivityOnAndroid)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Channel_LoadFromUrl_2(swigCPtr, url, width, height, locale, pushNotificationContent, entryPoint, separateVideoplayerActivityOnAndroid);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void LoadFromUrl(string url, int width, int height, string locale, string pushNotificationContent, string entryPoint)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Channel_LoadFromUrl_3(swigCPtr, url, width, height, locale, pushNotificationContent, entryPoint);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void LoadFromUrl(string url, int width, int height, string locale, string pushNotificationContent)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Channel_LoadFromUrl_4(swigCPtr, url, width, height, locale, pushNotificationContent);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void LoadFromUrl(string url, int width, int height, string locale)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Channel_LoadFromUrl_5(swigCPtr, url, width, height, locale);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void LoadFromUrl(string url, int width, int height)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Channel_LoadFromUrl_6(swigCPtr, url, width, height);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		private static void OnChannelAudioHandler(ChannelAudioHandler cb, AudioAction audioAction, string filename, string id)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(audioAction, filename, id);
		}

		private static void OnChannelLoadedHandler(ChannelLoadedHandler cb, LoadResult loadResult)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(loadResult);
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnChannelAudioHandler;
			swigDelegate1 = SwigDirectorOnChannelLoadedHandler;
			RCSSDKPINVOKE.Channel_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(Channel));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateChannel_0))]
		private static void SwigDirectorOnChannelAudioHandler(IntPtr cb, int audioAction, string filename, string id)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Channel] Ignoring callback from previously disposed object instance");
				return;
			}
			ChannelAudioHandler cb2 = (ChannelAudioHandler)gCHandle.Target;
			OnChannelAudioHandler(cb2, (AudioAction)audioAction, filename, id);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateChannel_1))]
		private static void SwigDirectorOnChannelLoadedHandler(IntPtr cb, int loadResult)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Channel] Ignoring callback from previously disposed object instance");
				return;
			}
			ChannelLoadedHandler cb2 = (ChannelLoadedHandler)gCHandle.Target;
			OnChannelLoadedHandler(cb2, (LoadResult)loadResult);
		}
	}
}

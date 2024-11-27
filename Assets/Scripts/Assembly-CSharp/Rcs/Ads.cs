using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class Ads : IDisposable
	{
		public enum State
		{
			Hidden = 0,
			Shown = 1,
			Expanded = 2,
			Ready = 3,
			Failed = 4
		}

		public enum EventType
		{
			Impression = 0,
			Click = 1
		}

		public enum RewardResult
		{
			RewardCanceled = 0,
			RewardCompleted = 1,
			RewardConfirmed = 2,
			RewardFailed = 3
		}

		public delegate void StateChangedHandler(string placement, State state);

		public delegate bool ActionInvokedHandler(string placement, string action);

		public delegate void NewContentHandler(string placement, int numberOfNewItems);

		public delegate void RewardResultHandler(string placement, RewardResult result, string voucherId);

		public delegate bool RendererHandler(string placement, string contentType, List<byte> content);

		public delegate void SizeChangedHandler(string placement, int width, int height);

		private delegate void SwigDelegateAds_0(IntPtr cb, string placement, int width, int height);

		private delegate bool SwigDelegateAds_1(IntPtr cb, string placement, string action);

		private delegate bool SwigDelegateAds_2(IntPtr cb, string placement, string contentType, IntPtr content);

		private delegate void SwigDelegateAds_3(IntPtr cb, string placement, int state);

		private delegate void SwigDelegateAds_4(IntPtr cb, string placement, int numberOfNewItems);

		private delegate void SwigDelegateAds_5(IntPtr cb, string placement, int result, string voucherId);

		private bool disposed;

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private SwigDelegateAds_0 swigDelegate0;

		private SwigDelegateAds_1 swigDelegate1;

		private SwigDelegateAds_2 swigDelegate2;

		private SwigDelegateAds_3 swigDelegate3;

		private SwigDelegateAds_4 swigDelegate4;

		private SwigDelegateAds_5 swigDelegate5;

		private GCHandle stateChangedGCHandle;

		private GCHandle actionInvokedGCHandle;

		private GCHandle rewardResultGCHandle;

		private GCHandle newContentGCHandle;

		private List<GCHandle> rendererGCHandles = new List<GCHandle>();

		private GCHandle sizeChangedGCHandle;

		internal Ads(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public Ads(IdentitySessionBase identity)
			: this(RCSSDKPINVOKE.new_Ads(identity.SharedPtr.CPtr), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		internal static IntPtr getCPtr(Ads obj)
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
			if (disposed)
			{
				return;
			}
			_DisposeUnmanaged();
			lock (this)
			{
				if (stateChangedGCHandle.IsAllocated)
				{
					stateChangedGCHandle.Free();
				}
				if (actionInvokedGCHandle.IsAllocated)
				{
					actionInvokedGCHandle.Free();
				}
				if (rewardResultGCHandle.IsAllocated)
				{
					rewardResultGCHandle.Free();
				}
				if (newContentGCHandle.IsAllocated)
				{
					newContentGCHandle.Free();
				}
				if (sizeChangedGCHandle.IsAllocated)
				{
					sizeChangedGCHandle.Free();
				}
				foreach (GCHandle rendererGCHandle in rendererGCHandles)
				{
					if (rendererGCHandle.IsAllocated)
					{
						rendererGCHandle.Free();
					}
				}
			}
			disposed = true;
		}

		~Ads()
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
						RCSSDKPINVOKE.delete_Ads(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public void AddPlacement(string placement)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Ads_AddPlacement_0(swigCPtr, placement);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void AddPlacement(string placement, RendererHandler rendererHandler)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			IntPtr jarg = IntPtr.Zero;
			if (rendererHandler != null)
			{
				GCHandle gCHandle = GCHandle.Alloc(rendererHandler);
				rendererGCHandles.Add(gCHandle);
				jarg = GCHandle.ToIntPtr(gCHandle);
			}
			RCSSDKPINVOKE.Ads_AddPlacement_1(swigCPtr, placement, jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void AddPlacement(string placement, int x, int y, int width, int height)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Ads_AddPlacement_2(swigCPtr, placement, x, y, width, height);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void AddPlacementNormalized(string placement, float x, float y, float width, float height)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Ads_AddPlacementNormalized(swigCPtr, placement, x, y, width, height);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void StartSession()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Ads_StartSession(swigCPtr);
		}

		public bool Show(string placement)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			bool result = RCSSDKPINVOKE.Ads_Show(swigCPtr, placement);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public void Hide(string placement)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Ads_Hide(swigCPtr, placement);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void SetTargetingParams(Dictionary<string, string> arg0)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			StringDict obj = arg0.ToSwigDict();
			RCSSDKPINVOKE.Ads_SetTargetingParams_0(swigCPtr, StringDict.getCPtr(obj));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void SetTargetingParams(string placement, Dictionary<string, string> arg1)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			StringDict obj = arg1.ToSwigDict();
			RCSSDKPINVOKE.Ads_SetTargetingParams_1(swigCPtr, placement, StringDict.getCPtr(obj));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void SetStateChangedHandler(StateChangedHandler stateChangedHandler)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			IntPtr jarg = IntPtr.Zero;
			if (stateChangedGCHandle.IsAllocated)
			{
				stateChangedGCHandle.Free();
			}
			if (stateChangedHandler != null)
			{
				stateChangedGCHandle = GCHandle.Alloc(stateChangedHandler);
				jarg = GCHandle.ToIntPtr(stateChangedGCHandle);
			}
			RCSSDKPINVOKE.Ads_SetStateChangedHandler(swigCPtr, jarg);
		}

		public void SetSizeChangedHandler(SizeChangedHandler sizeChangedHandler)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			IntPtr jarg = IntPtr.Zero;
			if (sizeChangedGCHandle.IsAllocated)
			{
				sizeChangedGCHandle.Free();
			}
			if (sizeChangedHandler != null)
			{
				sizeChangedGCHandle = GCHandle.Alloc(sizeChangedHandler);
				jarg = GCHandle.ToIntPtr(sizeChangedGCHandle);
			}
			RCSSDKPINVOKE.Ads_SetSizeChangedHandler(swigCPtr, jarg);
		}

		public void SetActionInvokedHandler(ActionInvokedHandler actionInvokedHandler)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			IntPtr jarg = IntPtr.Zero;
			if (actionInvokedGCHandle.IsAllocated)
			{
				actionInvokedGCHandle.Free();
			}
			if (actionInvokedHandler != null)
			{
				actionInvokedGCHandle = GCHandle.Alloc(actionInvokedHandler);
				jarg = GCHandle.ToIntPtr(actionInvokedGCHandle);
			}
			RCSSDKPINVOKE.Ads_SetActionInvokedHandler(swigCPtr, jarg);
		}

		public void SetRewardResultHandler(RewardResultHandler rewardResultHandler)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			IntPtr jarg = IntPtr.Zero;
			if (rewardResultGCHandle.IsAllocated)
			{
				rewardResultGCHandle.Free();
			}
			if (rewardResultHandler != null)
			{
				rewardResultGCHandle = GCHandle.Alloc(rewardResultHandler);
				jarg = GCHandle.ToIntPtr(rewardResultGCHandle);
			}
			RCSSDKPINVOKE.Ads_SetRewardResultHandler(swigCPtr, jarg);
		}

		public void SetNewContentHandler(NewContentHandler newContentHandler)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			IntPtr jarg = IntPtr.Zero;
			if (newContentGCHandle.IsAllocated)
			{
				newContentGCHandle.Free();
			}
			if (newContentHandler != null)
			{
				newContentGCHandle = GCHandle.Alloc(newContentHandler);
				jarg = GCHandle.ToIntPtr(newContentGCHandle);
			}
			RCSSDKPINVOKE.Ads_SetNewContentHandler(swigCPtr, jarg);
		}

		public State GetState(string placement)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			State result = (State)RCSSDKPINVOKE.Ads_GetState(swigCPtr, placement);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public void HandleClick(string placement)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Ads_HandleClick(swigCPtr, placement);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void TrackEvent(string placement, EventType type)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Ads_TrackEvent_0(swigCPtr, placement, (int)type);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void TrackEvent(string placement, EventType type, string id)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Ads_TrackEvent_1(swigCPtr, placement, (int)type, id);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void SetTrackingParams(string placement, Dictionary<string, string> arg1)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			StringDict obj = arg1.ToSwigDict();
			RCSSDKPINVOKE.Ads_SetTrackingParams(swigCPtr, placement, StringDict.getCPtr(obj));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		private static void OnStateChangedHandler(StateChangedHandler cb, string placement, State state)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(placement, state);
		}

		private static bool OnActionInvokedHandler(ActionInvokedHandler cb, string placement, string action)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			return cb(placement, action);
		}

		private static void OnNewContentHandler(NewContentHandler cb, string placement, int numberOfNewItems)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(placement, numberOfNewItems);
		}

		private static void OnRewardResultHandler(RewardResultHandler cb, string placement, RewardResult result, string voucherId)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(placement, result, voucherId);
		}

		private static bool OnRendererHandler(RendererHandler cb, string placement, string contentType, ByteList content)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			return cb(placement, contentType, content.ToList());
		}

		private static void OnSizeChangedHandler(SizeChangedHandler cb, string placement, int width, int height)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(placement, width, height);
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnSizeChangedHandler;
			swigDelegate1 = SwigDirectorOnActionInvokedHandler;
			swigDelegate2 = SwigDirectorOnRendererHandler;
			swigDelegate3 = SwigDirectorOnStateChangedHandler;
			swigDelegate4 = SwigDirectorOnNewContentHandler;
			swigDelegate5 = SwigDirectorOnRewardResultHandler;
			RCSSDKPINVOKE.Ads_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1), Marshal.GetFunctionPointerForDelegate(swigDelegate2), Marshal.GetFunctionPointerForDelegate(swigDelegate3), Marshal.GetFunctionPointerForDelegate(swigDelegate4), Marshal.GetFunctionPointerForDelegate(swigDelegate5));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(Ads));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateAds_0))]
		private static void SwigDirectorOnSizeChangedHandler(IntPtr cb, string placement, int width, int height)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Ads] Ignoring callback from previously disposed object instance");
				return;
			}
			SizeChangedHandler cb2 = (SizeChangedHandler)gCHandle.Target;
			OnSizeChangedHandler(cb2, placement, width, height);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateAds_1))]
		private static bool SwigDirectorOnActionInvokedHandler(IntPtr cb, string placement, string action)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Ads] Ignoring callback from previously disposed object instance");
				return false;
			}
			ActionInvokedHandler cb2 = (ActionInvokedHandler)gCHandle.Target;
			return OnActionInvokedHandler(cb2, placement, action);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateAds_2))]
		private static bool SwigDirectorOnRendererHandler(IntPtr cb, string placement, string contentType, IntPtr content)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Ads] Ignoring callback from previously disposed object instance");
				return false;
			}
			RendererHandler cb2 = (RendererHandler)gCHandle.Target;
			return OnRendererHandler(cb2, placement, contentType, new ByteList(content, false));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateAds_3))]
		private static void SwigDirectorOnStateChangedHandler(IntPtr cb, string placement, int state)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Ads] Ignoring callback from previously disposed object instance");
				return;
			}
			StateChangedHandler cb2 = (StateChangedHandler)gCHandle.Target;
			OnStateChangedHandler(cb2, placement, (State)state);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateAds_4))]
		private static void SwigDirectorOnNewContentHandler(IntPtr cb, string placement, int numberOfNewItems)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Ads] Ignoring callback from previously disposed object instance");
				return;
			}
			NewContentHandler cb2 = (NewContentHandler)gCHandle.Target;
			OnNewContentHandler(cb2, placement, numberOfNewItems);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateAds_5))]
		private static void SwigDirectorOnRewardResultHandler(IntPtr cb, string placement, int result, string voucherId)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Ads] Ignoring callback from previously disposed object instance");
				return;
			}
			RewardResultHandler cb2 = (RewardResultHandler)gCHandle.Target;
			OnRewardResultHandler(cb2, placement, (RewardResult)result, voucherId);
		}
	}
}

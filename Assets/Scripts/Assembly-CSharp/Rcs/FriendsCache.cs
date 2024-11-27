using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class FriendsCache : IDisposable
	{
		public delegate void RefreshedCallback();

		private delegate void SwigDelegateFriendsCache_0(IntPtr cb);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private Friends friendsBackend;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SwigDelegateFriendsCache_0 swigDelegate0;

		private GCHandle refreshCallbackGCHandle;

		internal FriendsCache(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public FriendsCache(Friends backend)
			: this(RCSSDKPINVOKE.new_FriendsCache(Friends.getCPtr(backend)), true)
		{
			friendsBackend = backend;
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<FriendsCache> callInfo)
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
				if (refreshCallbackGCHandle.IsAllocated)
				{
					refreshCallbackGCHandle.Free();
				}
			}
			disposed = true;
		}

		internal static IntPtr getCPtr(FriendsCache obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~FriendsCache()
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
						RCSSDKPINVOKE.delete_FriendsCache(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public void Initialize(RefreshedCallback callback)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			IntPtr jarg = IntPtr.Zero;
			if (refreshCallbackGCHandle.IsAllocated)
			{
				refreshCallbackGCHandle.Free();
			}
			if (callback != null)
			{
				refreshCallbackGCHandle = GCHandle.Alloc(callback);
				jarg = GCHandle.ToIntPtr(refreshCallbackGCHandle);
			}
			RCSSDKPINVOKE.FriendsCache_Initialize(swigCPtr, jarg);
		}

		public List<User> GetFriends()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			Users srcList = new Users(RCSSDKPINVOKE.FriendsCache_GetFriends(swigCPtr), false);
			return srcList.ToList();
		}

		public User GetFriend(string accountId)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			IntPtr intPtr = RCSSDKPINVOKE.FriendsCache_GetFriend(swigCPtr, accountId);
			User user = ((!(intPtr == IntPtr.Zero)) ? new User(intPtr, false) : null);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return (user != null) ? new User(user) : null;
		}

		public List<User.SocialNetworkProfile> GetSocialNetworkFriends(User.SocialNetwork socialNetwork, ulong maxNumber)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			SocialNetworkProfiles srcList = new SocialNetworkProfiles(RCSSDKPINVOKE.FriendsCache_GetSocialNetworkFriends_0(swigCPtr, (int)socialNetwork, maxNumber), false);
			return srcList.ToList();
		}

		public List<User.SocialNetworkProfile> GetSocialNetworkFriends(User.SocialNetwork socialNetwork)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			SocialNetworkProfiles srcList = new SocialNetworkProfiles(RCSSDKPINVOKE.FriendsCache_GetSocialNetworkFriends_1(swigCPtr, (int)socialNetwork), false);
			return srcList.ToList();
		}

		public Friends GetBackend()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return friendsBackend;
		}

		private static void OnRefreshedCallback(RefreshedCallback cb)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb();
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnRefreshedCallback;
			RCSSDKPINVOKE.FriendsCache_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(FriendsCache));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateFriendsCache_0))]
		private static void SwigDirectorOnRefreshedCallback(IntPtr cb)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.FriendsCache] Ignoring callback from previously disposed object instance");
				return;
			}
			RefreshedCallback cb2 = (RefreshedCallback)gCHandle.Target;
			OnRefreshedCallback(cb2);
		}
	}
}

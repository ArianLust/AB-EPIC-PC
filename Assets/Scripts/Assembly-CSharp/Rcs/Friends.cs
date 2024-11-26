using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class Friends : IDisposable
	{
		public enum IsConnectedError
		{
			IsConnectedErrorNone = 0,
			IsConnectedErrorNotSupported = 1,
			IsConnectedErrorNoProfile = 2,
			IsConnectedErrorNotLoggedIn = 3,
			IsConnectedErrorUidNotMatched = 4
		}

		public enum ConnectError
		{
			ConnectErrorNone = 0,
			ConnectErrorNotSupported = 1,
			ConnectErrorAlreadyConnecting = 2,
			ConnectErrorFailed = 3
		}

		public enum GetFriendsError
		{
			GetFriendsErrorNone = 0,
			GetFriendsErrorServiceNotAvailable = 1,
			GetFriendsErrorNetworkFailure = 2
		}

		public delegate void ConnectSuccessCallback(User.SocialNetwork socialNetwork, User.SocialNetworkProfile profile);

		public delegate void IsConnectedSuccessCallback(User.SocialNetwork socialNetwork, User.SocialNetworkProfile profileInIdentity, User.SocialNetworkProfile profileInDevice);

		public delegate void IsConnectedErrorCallback(User.SocialNetwork socialNetwork, User.SocialNetworkProfile profileInIdentity, User.SocialNetworkProfile profileInDevice, IsConnectedError error);

		public delegate void GetFriendsErrorCallback(GetFriendsError error);

		public delegate void DisconnectSuccessCallback(User.SocialNetwork socialNetwork);

		public delegate void GetFriendsSuccessCallback(List<User> users);

		public delegate void DisconnectErrorCallback(User.SocialNetwork socialNetwork);

		public delegate void ConnectErrorCallback(User.SocialNetwork socialNetwork, ConnectError error);

		private delegate void SwigDelegateFriends_0(IntPtr cb, int socialNetwork, IntPtr profile);

		private delegate void SwigDelegateFriends_1(IntPtr cb, int socialNetwork, IntPtr profileInIdentity, IntPtr profileInDevice);

		private delegate void SwigDelegateFriends_2(IntPtr cb, int socialNetwork, IntPtr profileInIdentity, IntPtr profileInDevice, int error);

		private delegate void SwigDelegateFriends_3(IntPtr cb, int error);

		private delegate void SwigDelegateFriends_4(IntPtr cb, int socialNetwork);

		private delegate void SwigDelegateFriends_5(IntPtr cb, IntPtr users);

		private delegate void SwigDelegateFriends_6(IntPtr cb, int socialNetwork);

		private delegate void SwigDelegateFriends_7(IntPtr cb, int socialNetwork, int error);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SwigDelegateFriends_0 swigDelegate0;

		private SwigDelegateFriends_1 swigDelegate1;

		private SwigDelegateFriends_2 swigDelegate2;

		private SwigDelegateFriends_3 swigDelegate3;

		private SwigDelegateFriends_4 swigDelegate4;

		private SwigDelegateFriends_5 swigDelegate5;

		private SwigDelegateFriends_6 swigDelegate6;

		private SwigDelegateFriends_7 swigDelegate7;

		internal Friends(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public Friends(IdentitySessionBase identity, List<User.SocialNetwork> socialNetworks)
			: this(RCSSDKPINVOKE.new_Friends(identity.SharedPtr.CPtr, SocialNetworks.getCPtr(new SocialNetworks(socialNetworks))), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<Friends> callInfo)
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

		internal static IntPtr getCPtr(Friends obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~Friends()
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
						RCSSDKPINVOKE.delete_Friends(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public virtual bool IsInitialized()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Friends_IsInitialized(swigCPtr);
		}

		public virtual void IsConnected(User.SocialNetwork socialNetwork, IsConnectedSuccessCallback onSuccess, IsConnectedErrorCallback onError)
		{
			AsyncCallInfo<Friends> asyncCallInfo = new AsyncCallInfo<Friends>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Friends_IsConnected(swigCPtr, (int)socialNetwork, intPtr, intPtr);
		}

		public virtual void Connect(User.SocialNetwork socialNetwork, ConnectSuccessCallback onSuccess, ConnectErrorCallback onError)
		{
			AsyncCallInfo<Friends> asyncCallInfo = new AsyncCallInfo<Friends>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Friends_Connect(swigCPtr, (int)socialNetwork, intPtr, intPtr);
		}

		public virtual void Disconnect(User.SocialNetwork socialNetwork, DisconnectSuccessCallback onSuccess, DisconnectErrorCallback onError)
		{
			AsyncCallInfo<Friends> asyncCallInfo = new AsyncCallInfo<Friends>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Friends_Disconnect(swigCPtr, (int)socialNetwork, intPtr, intPtr);
		}

		public virtual void GetFriends(GetFriendsSuccessCallback onSuccess, GetFriendsErrorCallback onError)
		{
			AsyncCallInfo<Friends> asyncCallInfo = new AsyncCallInfo<Friends>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Friends_GetFriends(swigCPtr, intPtr, intPtr);
		}

		public virtual List<User.SocialNetwork> GetSocialNetworks()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			SocialNetworks srcList = new SocialNetworks(RCSSDKPINVOKE.Friends_GetSocialNetworks(swigCPtr), false);
			return srcList.ToList();
		}

		public static string AvatarUrl(User.SocialNetwork socialNetwork, string uid)
		{
			string result = RCSSDKPINVOKE.Friends_AvatarUrl((int)socialNetwork, uid);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		private static void OnConnectSuccessCallback(ConnectSuccessCallback cb, User.SocialNetwork socialNetwork, User.SocialNetworkProfile profile)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(socialNetwork, new User.SocialNetworkProfile(profile));
		}

		private static void OnIsConnectedSuccessCallback(IsConnectedSuccessCallback cb, User.SocialNetwork socialNetwork, User.SocialNetworkProfile profileInIdentity, User.SocialNetworkProfile profileInDevice)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(socialNetwork, new User.SocialNetworkProfile(profileInIdentity), new User.SocialNetworkProfile(profileInDevice));
		}

		private static void OnIsConnectedErrorCallback(IsConnectedErrorCallback cb, User.SocialNetwork socialNetwork, User.SocialNetworkProfile profileInIdentity, User.SocialNetworkProfile profileInDevice, IsConnectedError error)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(socialNetwork, new User.SocialNetworkProfile(profileInIdentity), new User.SocialNetworkProfile(profileInDevice), error);
		}

		private static void OnGetFriendsErrorCallback(GetFriendsErrorCallback cb, GetFriendsError error)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(error);
		}

		private static void OnDisconnectSuccessCallback(DisconnectSuccessCallback cb, User.SocialNetwork socialNetwork)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(socialNetwork);
		}

		private static void OnGetFriendsSuccessCallback(GetFriendsSuccessCallback cb, List<User> users)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(users);
		}

		private static void OnDisconnectErrorCallback(DisconnectErrorCallback cb, User.SocialNetwork socialNetwork)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(socialNetwork);
		}

		private static void OnConnectErrorCallback(ConnectErrorCallback cb, User.SocialNetwork socialNetwork, ConnectError error)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(socialNetwork, error);
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnConnectSuccessCallback;
			swigDelegate1 = SwigDirectorOnIsConnectedSuccessCallback;
			swigDelegate2 = SwigDirectorOnIsConnectedErrorCallback;
			swigDelegate3 = SwigDirectorOnGetFriendsErrorCallback;
			swigDelegate4 = SwigDirectorOnDisconnectSuccessCallback;
			swigDelegate5 = SwigDirectorOnGetFriendsSuccessCallback;
			swigDelegate6 = SwigDirectorOnDisconnectErrorCallback;
			swigDelegate7 = SwigDirectorOnConnectErrorCallback;
			RCSSDKPINVOKE.Friends_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1), Marshal.GetFunctionPointerForDelegate(swigDelegate2), Marshal.GetFunctionPointerForDelegate(swigDelegate3), Marshal.GetFunctionPointerForDelegate(swigDelegate4), Marshal.GetFunctionPointerForDelegate(swigDelegate5), Marshal.GetFunctionPointerForDelegate(swigDelegate6), Marshal.GetFunctionPointerForDelegate(swigDelegate7));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(Friends));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateFriends_0))]
		private static void SwigDirectorOnConnectSuccessCallback(IntPtr cb, int socialNetwork, IntPtr profile)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Friends] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Friends> asyncCallInfo = (AsyncCallInfo<Friends>)gCHandle.Target;
			ConnectSuccessCallback handler = asyncCallInfo.GetHandler<ConnectSuccessCallback>();
			try
			{
				OnConnectSuccessCallback(handler, (User.SocialNetwork)socialNetwork, new User.SocialNetworkProfile(profile, false));
			}
			finally
			{
				if (!"ConnectSuccessCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateFriends_1))]
		private static void SwigDirectorOnIsConnectedSuccessCallback(IntPtr cb, int socialNetwork, IntPtr profileInIdentity, IntPtr profileInDevice)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Friends] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Friends> asyncCallInfo = (AsyncCallInfo<Friends>)gCHandle.Target;
			IsConnectedSuccessCallback handler = asyncCallInfo.GetHandler<IsConnectedSuccessCallback>();
			try
			{
				OnIsConnectedSuccessCallback(handler, (User.SocialNetwork)socialNetwork, new User.SocialNetworkProfile(profileInIdentity, false), new User.SocialNetworkProfile(profileInDevice, false));
			}
			finally
			{
				if (!"IsConnectedSuccessCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateFriends_2))]
		private static void SwigDirectorOnIsConnectedErrorCallback(IntPtr cb, int socialNetwork, IntPtr profileInIdentity, IntPtr profileInDevice, int error)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Friends] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Friends> asyncCallInfo = (AsyncCallInfo<Friends>)gCHandle.Target;
			IsConnectedErrorCallback handler = asyncCallInfo.GetHandler<IsConnectedErrorCallback>();
			try
			{
				OnIsConnectedErrorCallback(handler, (User.SocialNetwork)socialNetwork, new User.SocialNetworkProfile(profileInIdentity, false), new User.SocialNetworkProfile(profileInDevice, false), (IsConnectedError)error);
			}
			finally
			{
				if (!"IsConnectedErrorCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateFriends_3))]
		private static void SwigDirectorOnGetFriendsErrorCallback(IntPtr cb, int error)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Friends] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Friends> asyncCallInfo = (AsyncCallInfo<Friends>)gCHandle.Target;
			GetFriendsErrorCallback handler = asyncCallInfo.GetHandler<GetFriendsErrorCallback>();
			try
			{
				OnGetFriendsErrorCallback(handler, (GetFriendsError)error);
			}
			finally
			{
				if (!"GetFriendsErrorCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateFriends_4))]
		private static void SwigDirectorOnDisconnectSuccessCallback(IntPtr cb, int socialNetwork)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Friends] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Friends> asyncCallInfo = (AsyncCallInfo<Friends>)gCHandle.Target;
			DisconnectSuccessCallback handler = asyncCallInfo.GetHandler<DisconnectSuccessCallback>();
			try
			{
				OnDisconnectSuccessCallback(handler, (User.SocialNetwork)socialNetwork);
			}
			finally
			{
				if (!"DisconnectSuccessCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateFriends_5))]
		private static void SwigDirectorOnGetFriendsSuccessCallback(IntPtr cb, IntPtr users)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Friends] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Friends> asyncCallInfo = (AsyncCallInfo<Friends>)gCHandle.Target;
			GetFriendsSuccessCallback handler = asyncCallInfo.GetHandler<GetFriendsSuccessCallback>();
			try
			{
				OnGetFriendsSuccessCallback(handler, new Users(users, false).ToList());
			}
			finally
			{
				if (!"GetFriendsSuccessCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateFriends_6))]
		private static void SwigDirectorOnDisconnectErrorCallback(IntPtr cb, int socialNetwork)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Friends] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Friends> asyncCallInfo = (AsyncCallInfo<Friends>)gCHandle.Target;
			DisconnectErrorCallback handler = asyncCallInfo.GetHandler<DisconnectErrorCallback>();
			try
			{
				OnDisconnectErrorCallback(handler, (User.SocialNetwork)socialNetwork);
			}
			finally
			{
				if (!"DisconnectErrorCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateFriends_7))]
		private static void SwigDirectorOnConnectErrorCallback(IntPtr cb, int socialNetwork, int error)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Friends] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Friends> asyncCallInfo = (AsyncCallInfo<Friends>)gCHandle.Target;
			ConnectErrorCallback handler = asyncCallInfo.GetHandler<ConnectErrorCallback>();
			try
			{
				OnConnectErrorCallback(handler, (User.SocialNetwork)socialNetwork, (ConnectError)error);
			}
			finally
			{
				if (!"ConnectErrorCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}
	}
}

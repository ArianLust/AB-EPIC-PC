using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public sealed class Session : IdentitySessionBase
	{
		public enum IdType
		{
			PlayerId = 0,
			FacebookId = 1,
			GameCenterId = 2,
			DummyId = 3
		}

		public enum ErrorCode
		{
			ErrorInvalidParameters = 0,
			ErrorPlayerNotFound = 1,
			ErrorPlayerDeleted = 2,
			ErrorNotAvailable = 3,
			ErrorNetworkFailure = 4,
			ErrorSessionAlreadyInitialized = 5,
			ErrorBanned = 6,
			ErrorOtherReason = 7
		}

		public delegate void FailureCallback(ErrorCode errorCode);

		public delegate void FindPlayersSuccessCallback(Dictionary<string, OtherPlayer> players);

		public delegate void NewSessionSuccessCallback();

		public delegate void UpdateAccessTokenCallback(AccessToken accessToken);

		public delegate string AttachedTokenUpdateRequestedCallback();

		private delegate void SwigDelegateSession_0(IntPtr cb, int errorCode);

		private delegate void SwigDelegateSession_1(IntPtr cb, IntPtr players);

		private delegate void SwigDelegateSession_2(IntPtr cb);

		private delegate void SwigDelegateSession_3(IntPtr cb, IntPtr accessToken);

		private delegate string SwigDelegateSession_4(IntPtr cb);

		private IntPtr swigCPtr;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SessionSharedPtr sessionSharedPtr;

		private SwigDelegateSession_0 swigDelegate0;

		private SwigDelegateSession_1 swigDelegate1;

		private SwigDelegateSession_2 swigDelegate2;

		private SwigDelegateSession_3 swigDelegate3;

		private SwigDelegateSession_4 swigDelegate4;

		private GCHandle attachedTokenUpdatedGCHandle;

		internal new SessionSharedPtr SharedPtr
		{
			get
			{
				return sessionSharedPtr;
			}
		}

		internal Session(IntPtr cPtr)
			: this(SwigTools.MakeSessionSharedPtr(cPtr))
		{
			swigCPtr = cPtr;
		}

		internal Session(SessionSharedPtr sessionPtr)
			: base(SwigTools.DowncastSessionSharedPtr(sessionPtr))
		{
			swigCPtr = SwigTools.GetSessionPtr(sessionPtr);
			sessionSharedPtr = sessionPtr;
		}

		public Session(IdentitySessionParameters arg0)
			: this(RCSSDKPINVOKE.new_Session(IdentitySessionParameters.getCPtr(arg0)))
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		internal static IntPtr getCPtr(Session obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		public override void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private new void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}
			_DisposeUnmanaged();
			lock (this)
			{
				if (attachedTokenUpdatedGCHandle.IsAllocated)
				{
					attachedTokenUpdatedGCHandle.Free();
				}
				foreach (IntPtr pendingCallback in pendingCallbacks)
				{
					GCHandle.FromIntPtr(pendingCallback).Free();
				}
				pendingCallbacks.Clear();
			}
			disposed = true;
			base.Dispose();
		}

		~Session()
		{
			Dispose(false);
		}

		private void _DisposeUnmanaged()
		{
			lock (this)
			{
				swigCPtr = IntPtr.Zero;
			}
		}

		public void RegisterPlayer(NewSessionSuccessCallback onSuccess, FailureCallback onFailure)
		{
			AsyncCallInfo<Session> asyncCallInfo = new AsyncCallInfo<Session>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onFailure);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Session_RegisterPlayer(swigCPtr, intPtr, intPtr);
		}

		public void Login(NetworkCredentials credentials, NewSessionSuccessCallback onSuccess, FailureCallback onFailure)
		{
			AsyncCallInfo<Session> asyncCallInfo = new AsyncCallInfo<Session>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onFailure);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Session_Login(swigCPtr, NetworkCredentials.getCPtr(credentials), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Restore(NewSessionSuccessCallback onSuccess, FailureCallback onFailure)
		{
			AsyncCallInfo<Session> asyncCallInfo = new AsyncCallInfo<Session>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onFailure);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Session_Restore_0(swigCPtr, intPtr, intPtr);
		}

		public void Restore(string refreshToken, NewSessionSuccessCallback onSuccess, FailureCallback onFailure)
		{
			AsyncCallInfo<Session> asyncCallInfo = new AsyncCallInfo<Session>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onFailure);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Session_Restore_1(swigCPtr, refreshToken, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Attach(AttachedTokenUpdateRequestedCallback onAttachedTokenUpdateRequested, FailureCallback onFailure)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Session> asyncCallInfo = new AsyncCallInfo<Session>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onFailure);
			IntPtr jarg2 = IntPtr.Zero;
			if (attachedTokenUpdatedGCHandle.IsAllocated)
			{
				attachedTokenUpdatedGCHandle.Free();
			}
			if (onAttachedTokenUpdateRequested != null)
			{
				attachedTokenUpdatedGCHandle = GCHandle.Alloc(onAttachedTokenUpdateRequested);
				jarg2 = GCHandle.ToIntPtr(attachedTokenUpdatedGCHandle);
			}
			RCSSDKPINVOKE.Session_Attach(swigCPtr, jarg2, jarg);
		}

		public static bool HasRestorableSession()
		{
			return RCSSDKPINVOKE.Session_HasRestorableSession();
		}

		public Player GetCurrentPlayer()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new Player(this, RCSSDKPINVOKE.Session_GetCurrentPlayer(swigCPtr), false);
		}

		public void FindPlayers(IdType type, List<string> ids, FindPlayersSuccessCallback onSuccess, FailureCallback onFailure)
		{
			AsyncCallInfo<Session> asyncCallInfo = new AsyncCallInfo<Session>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onFailure);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Session_FindPlayers(swigCPtr, (int)type, StringList.getCPtr(new StringList(ids)), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public string GetRefreshToken()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Session_GetRefreshToken(swigCPtr);
		}

		public AccessToken GetAccessToken()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new AccessToken(RCSSDKPINVOKE.Session_GetAccessToken(swigCPtr), true);
		}

		public void UpdateAccessToken(UpdateAccessTokenCallback onSuccess, FailureCallback onFailure)
		{
			AsyncCallInfo<Session> asyncCallInfo = new AsyncCallInfo<Session>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onFailure);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Session_UpdateAccessToken_0(swigCPtr, intPtr, intPtr);
		}

		public void UpdateAccessToken()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Session_UpdateAccessToken_1(swigCPtr);
		}

		public string GetEncodedAppEnv()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Session_GetEncodedAppEnv(swigCPtr);
		}

		public ulong GetSessionId()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Session_GetSessionId(swigCPtr);
		}

		public override string GetAccountId()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Session_GetAccountId(swigCPtr);
		}

		public override string GetSharedAccountId()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Session_GetSharedAccountId(swigCPtr);
		}

		public override string GetAccessTokenString()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Session_GetAccessTokenString(swigCPtr);
		}

		public override IdentitySessionParameters GetParams()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			IdentitySessionParameters idSessionParams = new IdentitySessionParameters(RCSSDKPINVOKE.Session_GetParams(swigCPtr), false);
			return new IdentitySessionParameters(idSessionParams);
		}

		private static void OnFailureCallback(FailureCallback cb, ErrorCode errorCode)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(errorCode);
		}

		private static void OnFindPlayersSuccessCallback(FindPlayersSuccessCallback cb, OtherPlayerDict players)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(players.ToDictionary());
		}

		private static void OnNewSessionSuccessCallback(NewSessionSuccessCallback cb)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb();
		}

		private static void OnUpdateAccessTokenCallback(UpdateAccessTokenCallback cb, AccessToken accessToken)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(new AccessToken(accessToken));
		}

		private static string OnAttachedTokenUpdateRequestedCallback(AttachedTokenUpdateRequestedCallback cb)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			return cb();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<Session> callInfo)
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

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnFailureCallback;
			swigDelegate1 = SwigDirectorOnFindPlayersSuccessCallback;
			swigDelegate2 = SwigDirectorOnNewSessionSuccessCallback;
			swigDelegate3 = SwigDirectorOnUpdateAccessTokenCallback;
			swigDelegate4 = SwigDirectorOnAttachedTokenUpdateRequestedCallback;
			RCSSDKPINVOKE.Session_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1), Marshal.GetFunctionPointerForDelegate(swigDelegate2), Marshal.GetFunctionPointerForDelegate(swigDelegate3), Marshal.GetFunctionPointerForDelegate(swigDelegate4));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(Session));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateSession_0))]
		private static void SwigDirectorOnFailureCallback(IntPtr cb, int errorCode)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Session] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Session> asyncCallInfo = (AsyncCallInfo<Session>)gCHandle.Target;
			FailureCallback handler = asyncCallInfo.GetHandler<FailureCallback>();
			OnFailureCallback(handler, (ErrorCode)errorCode);
			gCHandle.Free();
			asyncCallInfo.Service.RemovePendingCallback(cb);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateSession_1))]
		private static void SwigDirectorOnFindPlayersSuccessCallback(IntPtr cb, IntPtr players)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Session] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Session> asyncCallInfo = (AsyncCallInfo<Session>)gCHandle.Target;
			FindPlayersSuccessCallback handler = asyncCallInfo.GetHandler<FindPlayersSuccessCallback>();
			OnFindPlayersSuccessCallback(handler, new OtherPlayerDict(players, false));
			gCHandle.Free();
			asyncCallInfo.Service.RemovePendingCallback(cb);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateSession_2))]
		private static void SwigDirectorOnNewSessionSuccessCallback(IntPtr cb)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Session] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Session> asyncCallInfo = (AsyncCallInfo<Session>)gCHandle.Target;
			NewSessionSuccessCallback handler = asyncCallInfo.GetHandler<NewSessionSuccessCallback>();
			try
			{
				OnNewSessionSuccessCallback(handler);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateSession_3))]
		private static void SwigDirectorOnUpdateAccessTokenCallback(IntPtr cb, IntPtr accessToken)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Session] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Session> asyncCallInfo = (AsyncCallInfo<Session>)gCHandle.Target;
			UpdateAccessTokenCallback handler = asyncCallInfo.GetHandler<UpdateAccessTokenCallback>();
			OnUpdateAccessTokenCallback(handler, new AccessToken(accessToken, false));
			gCHandle.Free();
			asyncCallInfo.Service.RemovePendingCallback(cb);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateSession_4))]
		private static string SwigDirectorOnAttachedTokenUpdateRequestedCallback(IntPtr cb)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Session] Ignoring callback from previously disposed object instance");
				return string.Empty;
			}
			AttachedTokenUpdateRequestedCallback cb2 = (AttachedTokenUpdateRequestedCallback)gCHandle.Target;
			return OnAttachedTokenUpdateRequestedCallback(cb2);
		}
	}
}

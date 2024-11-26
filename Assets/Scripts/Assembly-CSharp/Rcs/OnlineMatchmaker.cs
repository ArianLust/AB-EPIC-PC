using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class OnlineMatchmaker : IDisposable
	{
		public class Response : IDisposable
		{
			public enum ResultType
			{
				Success = 0,
				Cancelled = 1,
				ErrorInvalidLobby = 2,
				ErrorTimeoutNoOtherPlayers = 3,
				ErrorTimeoutServerUnreachable = 4,
				ErrorInvalidTimeout = 5,
				ErrorInUse = 6,
				ErrorOtherReason = 7
			}

			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			public string Message
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.OnlineMatchmaker_Response_Message_get(swigCPtr);
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
					RCSSDKPINVOKE.OnlineMatchmaker_Response_Message_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public ResultType Result
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return (ResultType)RCSSDKPINVOKE.OnlineMatchmaker_Response_Result_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.OnlineMatchmaker_Response_Result_set(swigCPtr, (int)value);
				}
			}

			internal Response(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public Response()
				: this(RCSSDKPINVOKE.new_OnlineMatchmaker_Response(), true)
			{
			}

			public Response(Response response)
				: this(RCSSDKPINVOKE.new_OnlineMatchmaker_Response(), true)
			{
				Message = response.Message;
				Result = response.Result;
			}

			internal static IntPtr getCPtr(Response obj)
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

			~Response()
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
							RCSSDKPINVOKE.delete_OnlineMatchmaker_Response(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}

			public Response MakeCopy()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return new Response(this);
			}
		}

		public delegate void LeaveLobbyCallback(Response response);

		public delegate void FetchLobbiesCallback(Response response, List<string> lobbies);

		public delegate void JoinLobbyCallback(Response response, List<string> matchingAccountIds, string flowId);

		private delegate void SwigDelegateOnlineMatchmaker_0(IntPtr cb, IntPtr response);

		private delegate void SwigDelegateOnlineMatchmaker_1(IntPtr cb, IntPtr response, IntPtr lobbies);

		private delegate void SwigDelegateOnlineMatchmaker_2(IntPtr cb, IntPtr response, IntPtr matchingAccountIds, string flowId);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SwigDelegateOnlineMatchmaker_0 swigDelegate0;

		private SwigDelegateOnlineMatchmaker_1 swigDelegate1;

		private SwigDelegateOnlineMatchmaker_2 swigDelegate2;

		internal OnlineMatchmaker(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public OnlineMatchmaker(IdentitySessionBase identity)
			: this(RCSSDKPINVOKE.new_OnlineMatchmaker(identity.SharedPtr.CPtr), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<OnlineMatchmaker> callInfo)
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

		internal static IntPtr getCPtr(OnlineMatchmaker obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~OnlineMatchmaker()
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
						RCSSDKPINVOKE.delete_OnlineMatchmaker(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public void JoinLobby(string lobbyId, ulong lobbyWaitTimeoutInSeconds, JoinLobbyCallback callback)
		{
			AsyncCallInfo<OnlineMatchmaker> asyncCallInfo = new AsyncCallInfo<OnlineMatchmaker>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(callback);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.OnlineMatchmaker_JoinLobby(swigCPtr, lobbyId, lobbyWaitTimeoutInSeconds, jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void LeaveLobby(string lobbyId, LeaveLobbyCallback callback)
		{
			AsyncCallInfo<OnlineMatchmaker> asyncCallInfo = new AsyncCallInfo<OnlineMatchmaker>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(callback);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.OnlineMatchmaker_LeaveLobby(swigCPtr, lobbyId, jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void FetchLobbies(FetchLobbiesCallback callback)
		{
			AsyncCallInfo<OnlineMatchmaker> asyncCallInfo = new AsyncCallInfo<OnlineMatchmaker>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(callback);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.OnlineMatchmaker_FetchLobbies(swigCPtr, jarg);
		}

		private static void OnLeaveLobbyCallback(LeaveLobbyCallback cb, Response response)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(response.MakeCopy());
		}

		private static void OnFetchLobbiesCallback(FetchLobbiesCallback cb, Response response, StringList lobbies)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(response.MakeCopy(), lobbies.ToList());
		}

		private static void OnJoinLobbyCallback(JoinLobbyCallback cb, Response response, StringList matchingAccountIds, string flowId)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(response.MakeCopy(), matchingAccountIds.ToList(), flowId);
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnLeaveLobbyCallback;
			swigDelegate1 = SwigDirectorOnFetchLobbiesCallback;
			swigDelegate2 = SwigDirectorOnJoinLobbyCallback;
			RCSSDKPINVOKE.OnlineMatchmaker_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1), Marshal.GetFunctionPointerForDelegate(swigDelegate2));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(OnlineMatchmaker));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateOnlineMatchmaker_0))]
		private static void SwigDirectorOnLeaveLobbyCallback(IntPtr cb, IntPtr response)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.OnlineMatchmaker] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<OnlineMatchmaker> asyncCallInfo = (AsyncCallInfo<OnlineMatchmaker>)gCHandle.Target;
			LeaveLobbyCallback handler = asyncCallInfo.GetHandler<LeaveLobbyCallback>();
			try
			{
				OnLeaveLobbyCallback(handler, new Response(response, false));
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateOnlineMatchmaker_1))]
		private static void SwigDirectorOnFetchLobbiesCallback(IntPtr cb, IntPtr response, IntPtr lobbies)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.OnlineMatchmaker] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<OnlineMatchmaker> asyncCallInfo = (AsyncCallInfo<OnlineMatchmaker>)gCHandle.Target;
			FetchLobbiesCallback handler = asyncCallInfo.GetHandler<FetchLobbiesCallback>();
			try
			{
				OnFetchLobbiesCallback(handler, new Response(response, false), new StringList(lobbies, false));
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateOnlineMatchmaker_2))]
		private static void SwigDirectorOnJoinLobbyCallback(IntPtr cb, IntPtr response, IntPtr matchingAccountIds, string flowId)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.OnlineMatchmaker] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<OnlineMatchmaker> asyncCallInfo = (AsyncCallInfo<OnlineMatchmaker>)gCHandle.Target;
			JoinLobbyCallback handler = asyncCallInfo.GetHandler<JoinLobbyCallback>();
			try
			{
				OnJoinLobbyCallback(handler, new Response(response, false), new StringList(matchingAccountIds, false), flowId);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}
	}
}

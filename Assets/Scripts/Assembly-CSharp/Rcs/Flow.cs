using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class Flow : IDisposable
	{
		public class Response : IDisposable
		{
			public enum ResultType
			{
				Success = 0,
				ErrorFlowNotFound = 1,
				ErrorInUse = 2,
				ErrorExpired = 3,
				ErrorForbidden = 4,
				ErrorMultipleJoinAttempts = 5,
				ErrorNetworkFailure = 6,
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
					string result = RCSSDKPINVOKE.Flow_Response_Message_get(swigCPtr);
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
					RCSSDKPINVOKE.Flow_Response_Message_set(swigCPtr, value);
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
					return (ResultType)RCSSDKPINVOKE.Flow_Response_Result_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Flow_Response_Result_set(swigCPtr, (int)value);
				}
			}

			internal Response(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public Response()
				: this(RCSSDKPINVOKE.new_Flow_Response(), true)
			{
			}

			public Response(Response responce)
				: this(RCSSDKPINVOKE.new_Flow_Response(), true)
			{
				Message = responce.Message;
				Result = responce.Result;
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
							RCSSDKPINVOKE.delete_Flow_Response(swigCPtr);
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

		public class Participant : IDisposable
		{
			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			public string AccountId
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Flow_Participant_AccountId_get(swigCPtr);
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
					RCSSDKPINVOKE.Flow_Participant_AccountId_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public ConnectionStateType ConnectionState
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return (ConnectionStateType)RCSSDKPINVOKE.Flow_Participant_ConnectionState_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Flow_Participant_ConnectionState_set(swigCPtr, (int)value);
				}
			}

			internal Participant(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public Participant()
				: this(RCSSDKPINVOKE.new_Flow_Participant_0(), true)
			{
			}

			public Participant(Participant other)
				: this(RCSSDKPINVOKE.new_Flow_Participant_1(getCPtr(other)), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			internal static IntPtr getCPtr(Participant obj)
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

			~Participant()
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
							RCSSDKPINVOKE.delete_Flow_Participant(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}
		}

		public enum ConnectionStateType
		{
			NotYetConnected = 0,
			Connecting = 1,
			Connected = 2,
			Disconnected = 3
		}

		public delegate void JoinFlowCallback(Response response);

		public delegate void ConnectionStateChangedHandler(ConnectionStateType state);

		public delegate void CreateFlowCallback(Response response, string flowId);

		public delegate void DataReceivedHandler(List<byte> data);

		public delegate void ParticipantStateChangedHandler(Participant participant);

		public delegate void ReachabilityCallback(bool isReachable);

		private delegate void SwigDelegateFlow_0(IntPtr cb, IntPtr response);

		private delegate void SwigDelegateFlow_1(IntPtr cb, int state);

		private delegate void SwigDelegateFlow_2(IntPtr cb, IntPtr response, string flowId);

		private delegate void SwigDelegateFlow_3(IntPtr cb, IntPtr data);

		private delegate void SwigDelegateFlow_4(IntPtr cb, IntPtr participant);

		private delegate void SwigDelegateFlow_5(IntPtr cb, bool isReachable);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private GCHandle connectionStateChangedGCHandle;

		private GCHandle dataReceivedGCHandle;

		private GCHandle participantStateChangedGCHandle;

		private SwigDelegateFlow_0 swigDelegate0;

		private SwigDelegateFlow_1 swigDelegate1;

		private SwigDelegateFlow_2 swigDelegate2;

		private SwigDelegateFlow_3 swigDelegate3;

		private SwigDelegateFlow_4 swigDelegate4;

		private SwigDelegateFlow_5 swigDelegate5;

		internal Flow(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public Flow(IdentitySessionBase identity)
			: this(RCSSDKPINVOKE.new_Flow(identity.SharedPtr.CPtr), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<Flow> callInfo)
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
			if (connectionStateChangedGCHandle.IsAllocated)
			{
				connectionStateChangedGCHandle.Free();
			}
			if (dataReceivedGCHandle.IsAllocated)
			{
				dataReceivedGCHandle.Free();
			}
			if (participantStateChangedGCHandle.IsAllocated)
			{
				participantStateChangedGCHandle.Free();
			}
			disposed = true;
		}

		internal static IntPtr getCPtr(Flow obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~Flow()
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
						RCSSDKPINVOKE.delete_Flow(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public void SetConnectionStateChangedHandler(ConnectionStateChangedHandler connectionStateChangedHandler)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			IntPtr jarg = IntPtr.Zero;
			if (connectionStateChangedGCHandle.IsAllocated)
			{
				connectionStateChangedGCHandle.Free();
			}
			if (connectionStateChangedHandler != null)
			{
				connectionStateChangedGCHandle = GCHandle.Alloc(connectionStateChangedHandler);
				jarg = GCHandle.ToIntPtr(connectionStateChangedGCHandle);
			}
			RCSSDKPINVOKE.Flow_SetConnectionStateChangedHandler(swigCPtr, jarg);
		}

		public void SetDataReceivedHandler(DataReceivedHandler dataReceivedHandler)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			IntPtr jarg = IntPtr.Zero;
			if (dataReceivedGCHandle.IsAllocated)
			{
				dataReceivedGCHandle.Free();
			}
			if (dataReceivedHandler != null)
			{
				dataReceivedGCHandle = GCHandle.Alloc(dataReceivedHandler);
				jarg = GCHandle.ToIntPtr(dataReceivedGCHandle);
			}
			RCSSDKPINVOKE.Flow_SetDataReceivedHandler(swigCPtr, jarg);
		}

		public void SetParticipantStateChangedHandler(ParticipantStateChangedHandler participantStateChangedHandler)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			IntPtr jarg = IntPtr.Zero;
			if (participantStateChangedGCHandle.IsAllocated)
			{
				participantStateChangedGCHandle.Free();
			}
			if (participantStateChangedHandler != null)
			{
				participantStateChangedGCHandle = GCHandle.Alloc(participantStateChangedHandler);
				jarg = GCHandle.ToIntPtr(participantStateChangedGCHandle);
			}
			RCSSDKPINVOKE.Flow_SetParticipantStateChangedHandler(swigCPtr, jarg);
		}

		public void Create(List<string> playerAccountIds, long timeToLiveInSeconds, CreateFlowCallback callback)
		{
			AsyncCallInfo<Flow> asyncCallInfo = new AsyncCallInfo<Flow>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(callback);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Flow_Create(swigCPtr, StringList.getCPtr(new StringList(playerAccountIds)), timeToLiveInSeconds, jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Join(string flowId, JoinFlowCallback callback)
		{
			AsyncCallInfo<Flow> asyncCallInfo = new AsyncCallInfo<Flow>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(callback);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Flow_Join(swigCPtr, flowId, jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void SetRecipients(List<string> recipientAccountIds)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Flow_SetRecipients(swigCPtr, StringList.getCPtr(new StringList(recipientAccountIds)));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void ClearRecipients()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Flow_ClearRecipients(swigCPtr);
		}

		public void Send(List<byte> data)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Flow_Send(swigCPtr, ByteList.getCPtr(new ByteList(data)));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public string GetFlowId()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Flow_GetFlowId(swigCPtr);
		}

		public List<Participant> GetParticipants()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			FlowParticipants srcList = new FlowParticipants(RCSSDKPINVOKE.Flow_GetParticipants(swigCPtr), false);
			return srcList.ToList();
		}

		public ConnectionStateType GetConnectionState()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return (ConnectionStateType)RCSSDKPINVOKE.Flow_GetConnectionState(swigCPtr);
		}

		private static void OnJoinFlowCallback(JoinFlowCallback cb, Response response)
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

		private static void OnConnectionStateChangedHandler(ConnectionStateChangedHandler cb, ConnectionStateType state)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(state);
		}

		private static void OnCreateFlowCallback(CreateFlowCallback cb, Response response, string flowId)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(response.MakeCopy(), flowId);
		}

		private static void OnDataReceivedHandler(DataReceivedHandler cb, ByteList data)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(data.ToList());
		}

		private static void OnParticipantStateChangedHandler(ParticipantStateChangedHandler cb, Participant participant)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(participant);
		}

		private static void OnReachabilityCallback(ReachabilityCallback cb, bool isReachable)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(isReachable);
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnJoinFlowCallback;
			swigDelegate1 = SwigDirectorOnConnectionStateChangedHandler;
			swigDelegate2 = SwigDirectorOnCreateFlowCallback;
			swigDelegate3 = SwigDirectorOnDataReceivedHandler;
			swigDelegate4 = SwigDirectorOnParticipantStateChangedHandler;
			swigDelegate5 = SwigDirectorOnReachabilityCallback;
			RCSSDKPINVOKE.Flow_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1), Marshal.GetFunctionPointerForDelegate(swigDelegate2), Marshal.GetFunctionPointerForDelegate(swigDelegate3), Marshal.GetFunctionPointerForDelegate(swigDelegate4), Marshal.GetFunctionPointerForDelegate(swigDelegate5));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(Flow));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateFlow_0))]
		private static void SwigDirectorOnJoinFlowCallback(IntPtr cb, IntPtr response)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Flow] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Flow> asyncCallInfo = (AsyncCallInfo<Flow>)gCHandle.Target;
			JoinFlowCallback handler = asyncCallInfo.GetHandler<JoinFlowCallback>();
			try
			{
				OnJoinFlowCallback(handler, new Response(response, false));
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateFlow_1))]
		private static void SwigDirectorOnConnectionStateChangedHandler(IntPtr cb, int state)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Flow] Ignoring callback from previously disposed object instance");
				return;
			}
			ConnectionStateChangedHandler cb2 = (ConnectionStateChangedHandler)gCHandle.Target;
			OnConnectionStateChangedHandler(cb2, (ConnectionStateType)state);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateFlow_2))]
		private static void SwigDirectorOnCreateFlowCallback(IntPtr cb, IntPtr response, string flowId)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Flow] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Flow> asyncCallInfo = (AsyncCallInfo<Flow>)gCHandle.Target;
			CreateFlowCallback handler = asyncCallInfo.GetHandler<CreateFlowCallback>();
			try
			{
				OnCreateFlowCallback(handler, new Response(response, false), flowId);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateFlow_3))]
		private static void SwigDirectorOnDataReceivedHandler(IntPtr cb, IntPtr data)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Flow] Ignoring callback from previously disposed object instance");
				return;
			}
			DataReceivedHandler cb2 = (DataReceivedHandler)gCHandle.Target;
			OnDataReceivedHandler(cb2, new ByteList(data, false));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateFlow_4))]
		private static void SwigDirectorOnParticipantStateChangedHandler(IntPtr cb, IntPtr participant)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Flow] Ignoring callback from previously disposed object instance");
				return;
			}
			ParticipantStateChangedHandler cb2 = (ParticipantStateChangedHandler)gCHandle.Target;
			OnParticipantStateChangedHandler(cb2, new Participant(participant, false));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateFlow_5))]
		private static void SwigDirectorOnReachabilityCallback(IntPtr cb, bool isReachable)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Flow] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Flow> asyncCallInfo = (AsyncCallInfo<Flow>)gCHandle.Target;
			ReachabilityCallback handler = asyncCallInfo.GetHandler<ReachabilityCallback>();
			try
			{
				OnReachabilityCallback(handler, isReachable);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}
	}
}

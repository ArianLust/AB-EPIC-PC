using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class Mailbox : IDisposable
	{
		public enum StateType
		{
			StateUnsynchronized = 0,
			StateSynchronizing = 1,
			StateSynchronized = 2
		}

		public enum ErrorCode
		{
			ErrorUnspecified = 0,
			ErrorInvalidParameters = 1,
			ErrorNotPermitted = 2,
			ErrorServiceNotAvailable = 3
		}

		public delegate void SendErrorCallback(ErrorCode error);

		public delegate void EraseSuccessCallback();

		public delegate void StateChangedCallback(StateType state);

		public delegate void SendSuccessCallback();

		public delegate void MessagesReceivedCallback(List<Message> messages);

		public delegate void EraseErrorCallback(ErrorCode error);

		private delegate void SwigDelegateMailbox_0(IntPtr cb, int error);

		private delegate void SwigDelegateMailbox_1(IntPtr cb);

		private delegate void SwigDelegateMailbox_2(IntPtr cb, int state);

		private delegate void SwigDelegateMailbox_3(IntPtr cb);

		private delegate void SwigDelegateMailbox_4(IntPtr cb, IntPtr messages);

		private delegate void SwigDelegateMailbox_5(IntPtr cb, int error);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SwigDelegateMailbox_0 swigDelegate0;

		private SwigDelegateMailbox_1 swigDelegate1;

		private SwigDelegateMailbox_2 swigDelegate2;

		private SwigDelegateMailbox_3 swigDelegate3;

		private SwigDelegateMailbox_4 swigDelegate4;

		private SwigDelegateMailbox_5 swigDelegate5;

		private GCHandle stateChangedGCHandle;

		private GCHandle messagesReceivedGCHandle;

		internal Mailbox(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public Mailbox(IdentitySessionBase identity)
			: this(RCSSDKPINVOKE.new_Mailbox(identity.SharedPtr.CPtr), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<Mailbox> callInfo)
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
				if (stateChangedGCHandle.IsAllocated)
				{
					stateChangedGCHandle.Free();
				}
				if (messagesReceivedGCHandle.IsAllocated)
				{
					messagesReceivedGCHandle.Free();
				}
			}
			disposed = true;
		}

		internal static IntPtr getCPtr(Mailbox obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~Mailbox()
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
						RCSSDKPINVOKE.delete_Mailbox(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public StateType GetState()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return (StateType)RCSSDKPINVOKE.Mailbox_GetState(swigCPtr);
		}

		public void SetStateChangedCallback(StateChangedCallback onStateChanged)
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
			if (onStateChanged != null)
			{
				stateChangedGCHandle = GCHandle.Alloc(onStateChanged);
				jarg = GCHandle.ToIntPtr(stateChangedGCHandle);
			}
			RCSSDKPINVOKE.Mailbox_SetStateChangedCallback(swigCPtr, jarg);
		}

		public void SetMessagesReceivedCallback(MessagesReceivedCallback onMessagesReceived)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			IntPtr jarg = IntPtr.Zero;
			if (messagesReceivedGCHandle.IsAllocated)
			{
				messagesReceivedGCHandle.Free();
			}
			if (onMessagesReceived != null)
			{
				messagesReceivedGCHandle = GCHandle.Alloc(onMessagesReceived);
				jarg = GCHandle.ToIntPtr(messagesReceivedGCHandle);
			}
			RCSSDKPINVOKE.Mailbox_SetMessagesReceivedCallback(swigCPtr, jarg);
		}

		public void Send(string accountId, string content, SendSuccessCallback onSuccess, SendErrorCallback onError)
		{
			AsyncCallInfo<Mailbox> asyncCallInfo = new AsyncCallInfo<Mailbox>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Mailbox_Send(swigCPtr, accountId, content, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Erase(string messageId, EraseSuccessCallback onSuccess, EraseErrorCallback onError)
		{
			AsyncCallInfo<Mailbox> asyncCallInfo = new AsyncCallInfo<Mailbox>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Mailbox_Erase(swigCPtr, messageId, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public List<Message> GetMessages()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			Messages srcList = new Messages(RCSSDKPINVOKE.Mailbox_GetMessages(swigCPtr), false);
			return srcList.ToList();
		}

		public void Sync()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Mailbox_Sync(swigCPtr);
		}

		public void StartMonitoring()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Mailbox_StartMonitoring(swigCPtr);
		}

		public void StopMonitoring()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Mailbox_StopMonitoring(swigCPtr);
		}

		private static void OnSendErrorCallback(SendErrorCallback cb, ErrorCode error)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(error);
		}

		private static void OnEraseSuccessCallback(EraseSuccessCallback cb)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb();
		}

		private static void OnStateChangedCallback(StateChangedCallback cb, StateType state)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(state);
		}

		private static void OnSendSuccessCallback(SendSuccessCallback cb)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb();
		}

		private static void OnMessagesReceivedCallback(MessagesReceivedCallback cb, List<Message> messages)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(messages);
		}

		private static void OnEraseErrorCallback(EraseErrorCallback cb, ErrorCode error)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(error);
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnSendErrorCallback;
			swigDelegate1 = SwigDirectorOnEraseSuccessCallback;
			swigDelegate2 = SwigDirectorOnStateChangedCallback;
			swigDelegate3 = SwigDirectorOnSendSuccessCallback;
			swigDelegate4 = SwigDirectorOnMessagesReceivedCallback;
			swigDelegate5 = SwigDirectorOnEraseErrorCallback;
			RCSSDKPINVOKE.Mailbox_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1), Marshal.GetFunctionPointerForDelegate(swigDelegate2), Marshal.GetFunctionPointerForDelegate(swigDelegate3), Marshal.GetFunctionPointerForDelegate(swigDelegate4), Marshal.GetFunctionPointerForDelegate(swigDelegate5));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(Mailbox));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateMailbox_0))]
		private static void SwigDirectorOnSendErrorCallback(IntPtr cb, int error)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Mailbox] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Mailbox> asyncCallInfo = (AsyncCallInfo<Mailbox>)gCHandle.Target;
			SendErrorCallback handler = asyncCallInfo.GetHandler<SendErrorCallback>();
			try
			{
				OnSendErrorCallback(handler, (ErrorCode)error);
			}
			finally
			{
				if (!"SendErrorCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateMailbox_1))]
		private static void SwigDirectorOnEraseSuccessCallback(IntPtr cb)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Mailbox] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Mailbox> asyncCallInfo = (AsyncCallInfo<Mailbox>)gCHandle.Target;
			EraseSuccessCallback handler = asyncCallInfo.GetHandler<EraseSuccessCallback>();
			try
			{
				OnEraseSuccessCallback(handler);
			}
			finally
			{
				if (!"EraseSuccessCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateMailbox_2))]
		private static void SwigDirectorOnStateChangedCallback(IntPtr cb, int state)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Mailbox] Ignoring callback from previously disposed object instance");
				return;
			}
			StateChangedCallback cb2 = (StateChangedCallback)gCHandle.Target;
			OnStateChangedCallback(cb2, (StateType)state);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateMailbox_3))]
		private static void SwigDirectorOnSendSuccessCallback(IntPtr cb)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Mailbox] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Mailbox> asyncCallInfo = (AsyncCallInfo<Mailbox>)gCHandle.Target;
			SendSuccessCallback handler = asyncCallInfo.GetHandler<SendSuccessCallback>();
			try
			{
				OnSendSuccessCallback(handler);
			}
			finally
			{
				if (!"SendSuccessCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateMailbox_4))]
		private static void SwigDirectorOnMessagesReceivedCallback(IntPtr cb, IntPtr messages)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Mailbox] Ignoring callback from previously disposed object instance");
				return;
			}
			MessagesReceivedCallback cb2 = (MessagesReceivedCallback)gCHandle.Target;
			OnMessagesReceivedCallback(cb2, new Messages(messages, false).ToList());
		}

		[MonoPInvokeCallback(typeof(SwigDelegateMailbox_5))]
		private static void SwigDirectorOnEraseErrorCallback(IntPtr cb, int error)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Mailbox] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Mailbox> asyncCallInfo = (AsyncCallInfo<Mailbox>)gCHandle.Target;
			EraseErrorCallback handler = asyncCallInfo.GetHandler<EraseErrorCallback>();
			try
			{
				OnEraseErrorCallback(handler, (ErrorCode)error);
			}
			finally
			{
				if (!"EraseErrorCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}
	}
}

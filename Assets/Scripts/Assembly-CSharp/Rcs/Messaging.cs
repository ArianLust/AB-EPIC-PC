using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class Messaging : IDisposable
	{
		public class ActorHandle : IDisposable
		{
			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			internal ActorHandle(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public ActorHandle()
				: this(RCSSDKPINVOKE.new_Messaging_ActorHandle_0(), true)
			{
			}

			public ActorHandle(string type, string id)
				: this(RCSSDKPINVOKE.new_Messaging_ActorHandle_1(type, id), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			public ActorHandle(string type)
				: this(RCSSDKPINVOKE.new_Messaging_ActorHandle_2(type), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			public ActorHandle(ActorHandle other)
				: this(RCSSDKPINVOKE.new_Messaging_ActorHandle_3(getCPtr(other)), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			internal static IntPtr getCPtr(ActorHandle obj)
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

			~ActorHandle()
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
							RCSSDKPINVOKE.delete_Messaging_ActorHandle(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}

			public string GetActorType()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Messaging_ActorHandle_GetActorType(swigCPtr);
			}

			public void SetId(string id)
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				RCSSDKPINVOKE.Messaging_ActorHandle_SetId(swigCPtr, id);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			public string GetId()
			{
				return RCSSDKPINVOKE.Messaging_ActorHandle_GetId(swigCPtr);
			}
		}

		public class ActorPermissions : IDisposable
		{
			public enum Permission
			{
				Read = 1,
				Write = 2
			}

			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			internal ActorPermissions(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public ActorPermissions()
				: this(RCSSDKPINVOKE.new_Messaging_ActorPermissions_0(), true)
			{
			}

			public ActorPermissions(ActorPermissions other)
				: this(RCSSDKPINVOKE.new_Messaging_ActorPermissions_1(getCPtr(other)), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			internal static IntPtr getCPtr(ActorPermissions obj)
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

			~ActorPermissions()
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
							RCSSDKPINVOKE.delete_Messaging_ActorPermissions(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}

			public void SetPermission(string accountId, int permissions)
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				RCSSDKPINVOKE.Messaging_ActorPermissions_SetPermission(swigCPtr, accountId, permissions);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			public void RemovePermission(string accountId)
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				RCSSDKPINVOKE.Messaging_ActorPermissions_RemovePermission(swigCPtr, accountId);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			public Dictionary<string, int> GetPermissions()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				MessagingActorPermissionsDict srcDict = new MessagingActorPermissionsDict(RCSSDKPINVOKE.Messaging_ActorPermissions_GetPermissions(swigCPtr), false);
				return srcDict.ToDictionary();
			}
		}

		public class ActorInfo : IDisposable
		{
			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			internal ActorInfo(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public ActorInfo()
				: this(RCSSDKPINVOKE.new_Messaging_ActorInfo_0(), true)
			{
			}

			public ActorInfo(string ownerAccountId, Dictionary<string, string> relations, Dictionary<string, string> properties, ActorPermissions permissions, string metadata, int messageCount)
				: this(RCSSDKPINVOKE.new_Messaging_ActorInfo_1(ownerAccountId, StringDict.getCPtr(relations.ToSwigDict()), StringDict.getCPtr(properties.ToSwigDict()), ActorPermissions.getCPtr(permissions), metadata, messageCount), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			public ActorInfo(ActorInfo other)
				: this(RCSSDKPINVOKE.new_Messaging_ActorInfo_2(getCPtr(other)), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			internal static IntPtr getCPtr(ActorInfo obj)
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

			~ActorInfo()
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
							RCSSDKPINVOKE.delete_Messaging_ActorInfo(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}

			public string GetOwnerAccountId()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Messaging_ActorInfo_GetOwnerAccountId(swigCPtr);
			}

			public Dictionary<string, string> GetRelations()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				StringDict srcDict = new StringDict(RCSSDKPINVOKE.Messaging_ActorInfo_GetRelations(swigCPtr), false);
				return srcDict.ToDictionary();
			}

			public Dictionary<string, string> GetProperties()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				StringDict srcDict = new StringDict(RCSSDKPINVOKE.Messaging_ActorInfo_GetProperties(swigCPtr), false);
				return srcDict.ToDictionary();
			}

			public ActorPermissions GetPermissions()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return new ActorPermissions(RCSSDKPINVOKE.Messaging_ActorInfo_GetPermissions(swigCPtr), false);
			}

			public string GetMetadata()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Messaging_ActorInfo_GetMetadata(swigCPtr);
			}

			public int GetMessageCount()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Messaging_ActorInfo_GetMessageCount(swigCPtr);
			}
		}

		public class FetchRequest : IDisposable
		{
			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			internal FetchRequest(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public FetchRequest(ActorHandle actorHandle, string cursor, FetchDirection direction, uint amount)
				: this(RCSSDKPINVOKE.new_Messaging_FetchRequest_0(ActorHandle.getCPtr(actorHandle), cursor, (int)direction, amount), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			public FetchRequest(FetchRequest other)
				: this(RCSSDKPINVOKE.new_Messaging_FetchRequest_1(getCPtr(other)), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			internal static IntPtr getCPtr(FetchRequest obj)
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

			~FetchRequest()
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
							RCSSDKPINVOKE.delete_Messaging_FetchRequest(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}

			public ActorHandle GetActorHandle()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return new ActorHandle(RCSSDKPINVOKE.Messaging_FetchRequest_GetActorHandle(swigCPtr), false);
			}

			public string GetCursor()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Messaging_FetchRequest_GetCursor(swigCPtr);
			}

			public FetchDirection GetDirection()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return (FetchDirection)RCSSDKPINVOKE.Messaging_FetchRequest_GetDirection(swigCPtr);
			}

			public uint GetAmount()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Messaging_FetchRequest_GetAmount(swigCPtr);
			}
		}

		public class FetchResponse : IDisposable
		{
			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			internal FetchResponse(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public FetchResponse(ActorHandle actorHandle, List<Message> messages, string errorMessage)
				: this(RCSSDKPINVOKE.new_Messaging_FetchResponse_0(ActorHandle.getCPtr(actorHandle), Messages.getCPtr(new Messages(messages)), errorMessage), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			public FetchResponse(FetchResponse other)
				: this(RCSSDKPINVOKE.new_Messaging_FetchResponse_1(getCPtr(other)), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			internal static IntPtr getCPtr(FetchResponse obj)
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

			~FetchResponse()
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
							RCSSDKPINVOKE.delete_Messaging_FetchResponse(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}

			public ActorHandle GetActorHandle()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return new ActorHandle(RCSSDKPINVOKE.Messaging_FetchResponse_GetActorHandle(swigCPtr), false);
			}

			public List<Message> GetMessages()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				Messages srcList = new Messages(RCSSDKPINVOKE.Messaging_FetchResponse_GetMessages(swigCPtr), false);
				return srcList.ToList();
			}

			public string GetErrorMessage()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Messaging_FetchResponse_GetErrorMessage(swigCPtr);
			}
		}

		public enum FetchDirection
		{
			FetchForward = 0,
			FetchBackward = 1
		}

		public enum ErrorCode
		{
			ErrorInvalidCursor = 0,
			ErrorInvalidParameters = 1,
			ErrorNotPermitted = 2,
			ErrorServiceNotAvailable = 3,
			ErrorNetworkFailure = 4
		}

		public delegate void MessageSentCallback(Message message);

		public delegate void MessageFetchedCallback(List<Message> messages);

		public delegate void ActorDeletedCallback(ActorHandle handle);

		public delegate void MessagesFetchedCallback(List<FetchResponse> responses);

		public delegate void MessageResponseReceivedCallback(Message message);

		public delegate void MessageResponsesReceivedCallback(List<Message> messages);

		public delegate void MessagesSentCallback(List<Message> messages);

		public delegate void ActorPermissionsModifiedCallback();

		public delegate void ActorQueriedCallback(ActorInfo info);

		public delegate void ActorCreatedCallback(ActorHandle handle);

		public delegate void ErrorCallback(ErrorCode errorCode);

		public delegate void MessageDeletedCallback();

		private delegate void SwigDelegateMessaging_0(IntPtr cb, IntPtr message);

		private delegate void SwigDelegateMessaging_1(IntPtr cb, IntPtr messages);

		private delegate void SwigDelegateMessaging_2(IntPtr cb, IntPtr handle);

		private delegate void SwigDelegateMessaging_3(IntPtr cb, IntPtr responses);

		private delegate void SwigDelegateMessaging_4(IntPtr cb, IntPtr message);

		private delegate void SwigDelegateMessaging_5(IntPtr cb, IntPtr messages);

		private delegate void SwigDelegateMessaging_6(IntPtr cb, IntPtr messages);

		private delegate void SwigDelegateMessaging_7(IntPtr cb);

		private delegate void SwigDelegateMessaging_8(IntPtr cb, IntPtr info);

		private delegate void SwigDelegateMessaging_9(IntPtr cb, IntPtr handle);

		private delegate void SwigDelegateMessaging_10(IntPtr cb, int errorCode);

		private delegate void SwigDelegateMessaging_11(IntPtr cb);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SwigDelegateMessaging_0 swigDelegate0;

		private SwigDelegateMessaging_1 swigDelegate1;

		private SwigDelegateMessaging_2 swigDelegate2;

		private SwigDelegateMessaging_3 swigDelegate3;

		private SwigDelegateMessaging_4 swigDelegate4;

		private SwigDelegateMessaging_5 swigDelegate5;

		private SwigDelegateMessaging_6 swigDelegate6;

		private SwigDelegateMessaging_7 swigDelegate7;

		private SwigDelegateMessaging_8 swigDelegate8;

		private SwigDelegateMessaging_9 swigDelegate9;

		private SwigDelegateMessaging_10 swigDelegate10;

		private SwigDelegateMessaging_11 swigDelegate11;

		internal Messaging(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public Messaging(IdentitySessionBase identity, string serviceName)
			: this(RCSSDKPINVOKE.new_Messaging_0(identity.SharedPtr.CPtr, serviceName), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Messaging(IdentitySessionBase identity)
			: this(RCSSDKPINVOKE.new_Messaging_1(identity.SharedPtr.CPtr), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<Messaging> callInfo)
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

		internal static IntPtr getCPtr(Messaging obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~Messaging()
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
						RCSSDKPINVOKE.delete_Messaging(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public void CreateActor(string actorType, ActorPermissions permissions, string metadata, ActorCreatedCallback onCreated, ErrorCallback onError)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onCreated);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_CreateActor_0(swigCPtr, actorType, ActorPermissions.getCPtr(permissions), metadata, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void CreateActor(string actorType, ActorPermissions permissions, string metadata, ActorCreatedCallback onCreated)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onCreated);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_CreateActor_1(swigCPtr, actorType, ActorPermissions.getCPtr(permissions), metadata, jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void CreateActor(string actorType, ActorPermissions permissions, string metadata)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_CreateActor_2(swigCPtr, actorType, ActorPermissions.getCPtr(permissions), metadata);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void CreateActor(string actorType, ActorPermissions permissions, string metadata, ulong timeToLiveInSeconds, ulong timeToWriteInSeconds, ActorCreatedCallback onCreated, ErrorCallback onError)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onCreated);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_CreateActor_3(swigCPtr, actorType, ActorPermissions.getCPtr(permissions), metadata, timeToLiveInSeconds, timeToWriteInSeconds, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void CreateActor(string actorType, ActorPermissions permissions, string metadata, ulong timeToLiveInSeconds, ulong timeToWriteInSeconds, ActorCreatedCallback onCreated)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onCreated);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_CreateActor_4(swigCPtr, actorType, ActorPermissions.getCPtr(permissions), metadata, timeToLiveInSeconds, timeToWriteInSeconds, jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void CreateActor(string actorType, ActorPermissions permissions, string metadata, ulong timeToLiveInSeconds, ulong timeToWriteInSeconds)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_CreateActor_5(swigCPtr, actorType, ActorPermissions.getCPtr(permissions), metadata, timeToLiveInSeconds, timeToWriteInSeconds);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void DeleteActor(ActorHandle actorHandle, ActorDeletedCallback onDeleted, ErrorCallback onError)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onDeleted);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_DeleteActor_0(swigCPtr, ActorHandle.getCPtr(actorHandle), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void DeleteActor(ActorHandle actorHandle, ActorDeletedCallback onDeleted)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onDeleted);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_DeleteActor_1(swigCPtr, ActorHandle.getCPtr(actorHandle), jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void DeleteActor(ActorHandle actorHandle)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_DeleteActor_2(swigCPtr, ActorHandle.getCPtr(actorHandle));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void QueryActor(ActorHandle actorHandle, ActorQueriedCallback onQueried, ErrorCallback onError)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onQueried);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_QueryActor_0(swigCPtr, ActorHandle.getCPtr(actorHandle), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void QueryActor(ActorHandle actorHandle, ActorQueriedCallback onQueried)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onQueried);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_QueryActor_1(swigCPtr, ActorHandle.getCPtr(actorHandle), jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void QueryActor(ActorHandle actorHandle)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_QueryActor_2(swigCPtr, ActorHandle.getCPtr(actorHandle));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void ModifyActorPermissions(ActorHandle actorHandle, ActorPermissions permissions, string cursor, ActorPermissionsModifiedCallback onModified, ErrorCallback onError)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onModified);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_ModifyActorPermissions_0(swigCPtr, ActorHandle.getCPtr(actorHandle), ActorPermissions.getCPtr(permissions), cursor, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void ModifyActorPermissions(ActorHandle actorHandle, ActorPermissions permissions, string cursor, ActorPermissionsModifiedCallback onModified)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onModified);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_ModifyActorPermissions_1(swigCPtr, ActorHandle.getCPtr(actorHandle), ActorPermissions.getCPtr(permissions), cursor, jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void ModifyActorPermissions(ActorHandle actorHandle, ActorPermissions permissions, string cursor)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_ModifyActorPermissions_2(swigCPtr, ActorHandle.getCPtr(actorHandle), ActorPermissions.getCPtr(permissions), cursor);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Tell(ActorHandle actorHandle, Message message, MessageSentCallback onSent, ErrorCallback onError)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSent);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_Tell_0(swigCPtr, ActorHandle.getCPtr(actorHandle), Message.getCPtr(message), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Tell(ActorHandle actorHandle, Message message, MessageSentCallback onSent)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSent);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_Tell_1(swigCPtr, ActorHandle.getCPtr(actorHandle), Message.getCPtr(message), jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Tell(ActorHandle actorHandle, Message message)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_Tell_2(swigCPtr, ActorHandle.getCPtr(actorHandle), Message.getCPtr(message));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Tell(ActorHandle actorHandle, List<Message> messages, MessagesSentCallback onSent, ErrorCallback onError)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSent);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_Tell_3(swigCPtr, ActorHandle.getCPtr(actorHandle), Messages.getCPtr(new Messages(messages)), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Tell(ActorHandle actorHandle, List<Message> messages, MessagesSentCallback onSent)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSent);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_Tell_4(swigCPtr, ActorHandle.getCPtr(actorHandle), Messages.getCPtr(new Messages(messages)), jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Tell(ActorHandle actorHandle, List<Message> messages)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_Tell_5(swigCPtr, ActorHandle.getCPtr(actorHandle), Messages.getCPtr(new Messages(messages)));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Ask(ActorHandle actorHandle, Message message, MessageSentCallback onSent, MessageResponseReceivedCallback onResponseReceived, ErrorCallback onError)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSent);
			asyncCallInfo.AddHandler(onResponseReceived);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_Ask_0(swigCPtr, ActorHandle.getCPtr(actorHandle), Message.getCPtr(message), intPtr, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Ask(ActorHandle actorHandle, Message message, MessageSentCallback onSent, MessageResponseReceivedCallback onResponseReceived)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSent);
			asyncCallInfo.AddHandler(onResponseReceived);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_Ask_1(swigCPtr, ActorHandle.getCPtr(actorHandle), Message.getCPtr(message), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Ask(ActorHandle actorHandle, Message message, MessageSentCallback onSent)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSent);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_Ask_2(swigCPtr, ActorHandle.getCPtr(actorHandle), Message.getCPtr(message), jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Ask(ActorHandle actorHandle, Message message)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_Ask_3(swigCPtr, ActorHandle.getCPtr(actorHandle), Message.getCPtr(message));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Ask(ActorHandle actorHandle, List<Message> messages, MessagesSentCallback onSent, MessageResponsesReceivedCallback onResponseReceived, ErrorCallback onError)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSent);
			asyncCallInfo.AddHandler(onResponseReceived);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_Ask_4(swigCPtr, ActorHandle.getCPtr(actorHandle), Messages.getCPtr(new Messages(messages)), intPtr, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Ask(ActorHandle actorHandle, List<Message> messages, MessagesSentCallback onSent, MessageResponsesReceivedCallback onResponseReceived)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSent);
			asyncCallInfo.AddHandler(onResponseReceived);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_Ask_5(swigCPtr, ActorHandle.getCPtr(actorHandle), Messages.getCPtr(new Messages(messages)), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Ask(ActorHandle actorHandle, List<Message> messages, MessagesSentCallback onSent)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSent);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_Ask_6(swigCPtr, ActorHandle.getCPtr(actorHandle), Messages.getCPtr(new Messages(messages)), jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Ask(ActorHandle actorHandle, List<Message> messages)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_Ask_7(swigCPtr, ActorHandle.getCPtr(actorHandle), Messages.getCPtr(new Messages(messages)));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void DeleteMessage(ActorHandle actorHandle, string messageId, MessageDeletedCallback onDeleted, ErrorCallback onError)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onDeleted);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_DeleteMessage_0(swigCPtr, ActorHandle.getCPtr(actorHandle), messageId, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void DeleteMessage(ActorHandle actorHandle, string messageId, MessageDeletedCallback onDeleted)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onDeleted);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_DeleteMessage_1(swigCPtr, ActorHandle.getCPtr(actorHandle), messageId, jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void DeleteMessage(ActorHandle actorHandle, string messageId)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_DeleteMessage_2(swigCPtr, ActorHandle.getCPtr(actorHandle), messageId);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Fetch(ActorHandle actorHandle, string cursor, FetchDirection direction, uint amount, MessageFetchedCallback onFetched, ErrorCallback onError)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onFetched);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_Fetch_0(swigCPtr, ActorHandle.getCPtr(actorHandle), cursor, (int)direction, amount, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Fetch(ActorHandle actorHandle, string cursor, FetchDirection direction, uint amount, MessageFetchedCallback onFetched)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onFetched);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_Fetch_1(swigCPtr, ActorHandle.getCPtr(actorHandle), cursor, (int)direction, amount, jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Fetch(ActorHandle actorHandle, string cursor, FetchDirection direction, uint amount)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_Fetch_2(swigCPtr, ActorHandle.getCPtr(actorHandle), cursor, (int)direction, amount);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void FetchMany(List<FetchRequest> requests, MessagesFetchedCallback onFetched, ErrorCallback onError)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onFetched);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_FetchMany_0(swigCPtr, MessagingFetchRequests.getCPtr(new MessagingFetchRequests(requests)), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void FetchMany(List<FetchRequest> requests, MessagesFetchedCallback onFetched)
		{
			AsyncCallInfo<Messaging> asyncCallInfo = new AsyncCallInfo<Messaging>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onFetched);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_FetchMany_1(swigCPtr, MessagingFetchRequests.getCPtr(new MessagingFetchRequests(requests)), jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void FetchMany(List<FetchRequest> requests)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Messaging_FetchMany_2(swigCPtr, MessagingFetchRequests.getCPtr(new MessagingFetchRequests(requests)));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public string GetServiceName()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Messaging_GetServiceName(swigCPtr);
		}

		private static void OnMessageSentCallback(MessageSentCallback cb, Message message)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(message);
		}

		private static void OnMessageFetchedCallback(MessageFetchedCallback cb, List<Message> messages)
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

		private static void OnActorDeletedCallback(ActorDeletedCallback cb, ActorHandle handle)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(handle);
		}

		private static void OnMessagesFetchedCallback(MessagesFetchedCallback cb, List<FetchResponse> responses)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(responses);
		}

		private static void OnMessageResponseReceivedCallback(MessageResponseReceivedCallback cb, Message message)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(message);
		}

		private static void OnMessageResponsesReceivedCallback(MessageResponsesReceivedCallback cb, List<Message> messages)
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

		private static void OnMessagesSentCallback(MessagesSentCallback cb, List<Message> messages)
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

		private static void OnActorPermissionsModifiedCallback(ActorPermissionsModifiedCallback cb)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb();
		}

		private static void OnActorQueriedCallback(ActorQueriedCallback cb, ActorInfo info)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(info);
		}

		private static void OnActorCreatedCallback(ActorCreatedCallback cb, ActorHandle handle)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(handle);
		}

		private static void OnErrorCallback(ErrorCallback cb, ErrorCode errorCode)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(errorCode);
		}

		private static void OnMessageDeletedCallback(MessageDeletedCallback cb)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb();
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnMessageSentCallback;
			swigDelegate1 = SwigDirectorOnMessageFetchedCallback;
			swigDelegate2 = SwigDirectorOnActorDeletedCallback;
			swigDelegate3 = SwigDirectorOnMessagesFetchedCallback;
			swigDelegate4 = SwigDirectorOnMessageResponseReceivedCallback;
			swigDelegate5 = SwigDirectorOnMessageResponsesReceivedCallback;
			swigDelegate6 = SwigDirectorOnMessagesSentCallback;
			swigDelegate7 = SwigDirectorOnActorPermissionsModifiedCallback;
			swigDelegate8 = SwigDirectorOnActorQueriedCallback;
			swigDelegate9 = SwigDirectorOnActorCreatedCallback;
			swigDelegate10 = SwigDirectorOnErrorCallback;
			swigDelegate11 = SwigDirectorOnMessageDeletedCallback;
			RCSSDKPINVOKE.Messaging_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1), Marshal.GetFunctionPointerForDelegate(swigDelegate2), Marshal.GetFunctionPointerForDelegate(swigDelegate3), Marshal.GetFunctionPointerForDelegate(swigDelegate4), Marshal.GetFunctionPointerForDelegate(swigDelegate5), Marshal.GetFunctionPointerForDelegate(swigDelegate6), Marshal.GetFunctionPointerForDelegate(swigDelegate7), Marshal.GetFunctionPointerForDelegate(swigDelegate8), Marshal.GetFunctionPointerForDelegate(swigDelegate9), Marshal.GetFunctionPointerForDelegate(swigDelegate10), Marshal.GetFunctionPointerForDelegate(swigDelegate11));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(Messaging));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateMessaging_0))]
		private static void SwigDirectorOnMessageSentCallback(IntPtr cb, IntPtr message)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Messaging] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Messaging> asyncCallInfo = (AsyncCallInfo<Messaging>)gCHandle.Target;
			MessageSentCallback handler = asyncCallInfo.GetHandler<MessageSentCallback>();
			try
			{
				OnMessageSentCallback(handler, new Message(message, false));
			}
			finally
			{
				if (!"MessageSentCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateMessaging_1))]
		private static void SwigDirectorOnMessageFetchedCallback(IntPtr cb, IntPtr messages)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Messaging] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Messaging> asyncCallInfo = (AsyncCallInfo<Messaging>)gCHandle.Target;
			MessageFetchedCallback handler = asyncCallInfo.GetHandler<MessageFetchedCallback>();
			try
			{
				OnMessageFetchedCallback(handler, new Messages(messages, false).ToList());
			}
			finally
			{
				if (!"MessageFetchedCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateMessaging_2))]
		private static void SwigDirectorOnActorDeletedCallback(IntPtr cb, IntPtr handle)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Messaging] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Messaging> asyncCallInfo = (AsyncCallInfo<Messaging>)gCHandle.Target;
			ActorDeletedCallback handler = asyncCallInfo.GetHandler<ActorDeletedCallback>();
			try
			{
				OnActorDeletedCallback(handler, new ActorHandle(handle, false));
			}
			finally
			{
				if (!"ActorDeletedCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateMessaging_3))]
		private static void SwigDirectorOnMessagesFetchedCallback(IntPtr cb, IntPtr responses)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Messaging] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Messaging> asyncCallInfo = (AsyncCallInfo<Messaging>)gCHandle.Target;
			MessagesFetchedCallback handler = asyncCallInfo.GetHandler<MessagesFetchedCallback>();
			try
			{
				OnMessagesFetchedCallback(handler, new MessagingFetchResponses(responses, false).ToList());
			}
			finally
			{
				if (!"MessagesFetchedCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateMessaging_4))]
		private static void SwigDirectorOnMessageResponseReceivedCallback(IntPtr cb, IntPtr message)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Messaging] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Messaging> asyncCallInfo = (AsyncCallInfo<Messaging>)gCHandle.Target;
			MessageResponseReceivedCallback handler = asyncCallInfo.GetHandler<MessageResponseReceivedCallback>();
			try
			{
				OnMessageResponseReceivedCallback(handler, new Message(message, false));
			}
			finally
			{
				if (!"MessageResponseReceivedCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateMessaging_5))]
		private static void SwigDirectorOnMessageResponsesReceivedCallback(IntPtr cb, IntPtr messages)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Messaging] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Messaging> asyncCallInfo = (AsyncCallInfo<Messaging>)gCHandle.Target;
			MessageResponsesReceivedCallback handler = asyncCallInfo.GetHandler<MessageResponsesReceivedCallback>();
			try
			{
				OnMessageResponsesReceivedCallback(handler, new Messages(messages, false).ToList());
			}
			finally
			{
				if (!"MessageResponsesReceivedCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateMessaging_6))]
		private static void SwigDirectorOnMessagesSentCallback(IntPtr cb, IntPtr messages)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Messaging] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Messaging> asyncCallInfo = (AsyncCallInfo<Messaging>)gCHandle.Target;
			MessagesSentCallback handler = asyncCallInfo.GetHandler<MessagesSentCallback>();
			try
			{
				OnMessagesSentCallback(handler, new Messages(messages, false).ToList());
			}
			finally
			{
				if (!"MessagesSentCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateMessaging_7))]
		private static void SwigDirectorOnActorPermissionsModifiedCallback(IntPtr cb)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Messaging] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Messaging> asyncCallInfo = (AsyncCallInfo<Messaging>)gCHandle.Target;
			ActorPermissionsModifiedCallback handler = asyncCallInfo.GetHandler<ActorPermissionsModifiedCallback>();
			try
			{
				OnActorPermissionsModifiedCallback(handler);
			}
			finally
			{
				if (!"ActorPermissionsModifiedCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateMessaging_8))]
		private static void SwigDirectorOnActorQueriedCallback(IntPtr cb, IntPtr info)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Messaging] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Messaging> asyncCallInfo = (AsyncCallInfo<Messaging>)gCHandle.Target;
			ActorQueriedCallback handler = asyncCallInfo.GetHandler<ActorQueriedCallback>();
			try
			{
				OnActorQueriedCallback(handler, new ActorInfo(info, false));
			}
			finally
			{
				if (!"ActorQueriedCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateMessaging_9))]
		private static void SwigDirectorOnActorCreatedCallback(IntPtr cb, IntPtr handle)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Messaging] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Messaging> asyncCallInfo = (AsyncCallInfo<Messaging>)gCHandle.Target;
			ActorCreatedCallback handler = asyncCallInfo.GetHandler<ActorCreatedCallback>();
			try
			{
				OnActorCreatedCallback(handler, new ActorHandle(handle, false));
			}
			finally
			{
				if (!"ActorCreatedCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateMessaging_10))]
		private static void SwigDirectorOnErrorCallback(IntPtr cb, int errorCode)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Messaging] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Messaging> asyncCallInfo = (AsyncCallInfo<Messaging>)gCHandle.Target;
			ErrorCallback handler = asyncCallInfo.GetHandler<ErrorCallback>();
			try
			{
				OnErrorCallback(handler, (ErrorCode)errorCode);
			}
			finally
			{
				if (!"ErrorCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateMessaging_11))]
		private static void SwigDirectorOnMessageDeletedCallback(IntPtr cb)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Messaging] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Messaging> asyncCallInfo = (AsyncCallInfo<Messaging>)gCHandle.Target;
			MessageDeletedCallback handler = asyncCallInfo.GetHandler<MessageDeletedCallback>();
			try
			{
				OnMessageDeletedCallback(handler);
			}
			finally
			{
				if (!"MessageDeletedCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}
	}
}

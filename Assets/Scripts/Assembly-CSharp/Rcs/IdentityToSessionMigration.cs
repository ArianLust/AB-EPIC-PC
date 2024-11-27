using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class IdentityToSessionMigration : IDisposable
	{
		public delegate void FailureCallback(Session.ErrorCode errorCode);

		public delegate void SuccessCallback(Session session);

		private delegate void SwigDelegateIdentityToSessionMigration_0(IntPtr cb, int errorCode);

		private delegate void SwigDelegateIdentityToSessionMigration_1(IntPtr cb, IntPtr session);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SwigDelegateIdentityToSessionMigration_0 swigDelegate0;

		private SwigDelegateIdentityToSessionMigration_1 swigDelegate1;

		internal IdentityToSessionMigration(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public IdentityToSessionMigration(IdentitySessionParameters arg0)
			: this(RCSSDKPINVOKE.new_IdentityToSessionMigration(IdentitySessionParameters.getCPtr(arg0)), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<IdentityToSessionMigration> callInfo)
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

		internal static IntPtr getCPtr(IdentityToSessionMigration obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~IdentityToSessionMigration()
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
						RCSSDKPINVOKE.delete_IdentityToSessionMigration(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public static bool HasMigratableIdentity()
		{
			return RCSSDKPINVOKE.IdentityToSessionMigration_HasMigratableIdentity();
		}

		public void RestoreMigratableIdentity(SuccessCallback onSuccess, FailureCallback onFailure)
		{
			AsyncCallInfo<IdentityToSessionMigration> asyncCallInfo = new AsyncCallInfo<IdentityToSessionMigration>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onFailure);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.IdentityToSessionMigration_RestoreMigratableIdentity(swigCPtr, intPtr, intPtr);
		}

		public void LoginMigratableIdentity(NetworkCredentials credentials, SuccessCallback onSuccess, FailureCallback onFailure)
		{
			AsyncCallInfo<IdentityToSessionMigration> asyncCallInfo = new AsyncCallInfo<IdentityToSessionMigration>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onFailure);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.IdentityToSessionMigration_LoginMigratableIdentity(swigCPtr, NetworkCredentials.getCPtr(credentials), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		private static void OnFailureCallback(FailureCallback cb, Session.ErrorCode errorCode)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(errorCode);
		}

		private static void OnSuccessCallback(SuccessCallback cb, SessionSharedPtr sessionPtr)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			Session session = new Session(SwigTools.CopySessionSharedPtr(sessionPtr));
			cb(session);
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnFailureCallback;
			swigDelegate1 = SwigDirectorOnSuccessCallback;
			RCSSDKPINVOKE.IdentityToSessionMigration_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(IdentityToSessionMigration));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateIdentityToSessionMigration_0))]
		private static void SwigDirectorOnFailureCallback(IntPtr cb, int errorCode)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.IdentityToSessionMigration] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<IdentityToSessionMigration> asyncCallInfo = (AsyncCallInfo<IdentityToSessionMigration>)gCHandle.Target;
			FailureCallback handler = asyncCallInfo.GetHandler<FailureCallback>();
			try
			{
				OnFailureCallback(handler, (Session.ErrorCode)errorCode);
			}
			finally
			{
				if (!"FailureCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateIdentityToSessionMigration_1))]
		private static void SwigDirectorOnSuccessCallback(IntPtr cb, IntPtr session)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.IdentityToSessionMigration] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<IdentityToSessionMigration> asyncCallInfo = (AsyncCallInfo<IdentityToSessionMigration>)gCHandle.Target;
			SuccessCallback handler = asyncCallInfo.GetHandler<SuccessCallback>();
			try
			{
				OnSuccessCallback(handler, new SessionSharedPtr(session, false));
			}
			finally
			{
				if (!"SuccessCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}
	}
}

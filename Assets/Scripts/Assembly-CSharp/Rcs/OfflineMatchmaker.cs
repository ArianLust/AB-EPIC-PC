using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class OfflineMatchmaker : IDisposable
	{
		public enum ResultCode
		{
			Success = 0,
			ErrorNetworkFailure = 1,
			ErrorOtherReason = 2
		}

		public delegate void MatchUsersCallback(ResultCode result, List<string> matchedAccountIds);

		public delegate void SetAttributesCallback(ResultCode result);

		public delegate void GetAttributesCallback(ResultCode result, Dictionary<string, Variant> attributes);

		private delegate void SwigDelegateOfflineMatchmaker_0(IntPtr cb, int result, IntPtr matchedAccountIds);

		private delegate void SwigDelegateOfflineMatchmaker_1(IntPtr cb, int result);

		private delegate void SwigDelegateOfflineMatchmaker_2(IntPtr cb, int result, IntPtr attributes);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SwigDelegateOfflineMatchmaker_0 swigDelegate0;

		private SwigDelegateOfflineMatchmaker_1 swigDelegate1;

		private SwigDelegateOfflineMatchmaker_2 swigDelegate2;

		internal OfflineMatchmaker(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public OfflineMatchmaker(IdentitySessionBase identity)
			: this(RCSSDKPINVOKE.new_OfflineMatchmaker(identity.SharedPtr.CPtr), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<OfflineMatchmaker> callInfo)
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

		internal static IntPtr getCPtr(OfflineMatchmaker obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~OfflineMatchmaker()
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
						RCSSDKPINVOKE.delete_OfflineMatchmaker(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public void GetAttributes(GetAttributesCallback callback)
		{
			AsyncCallInfo<OfflineMatchmaker> asyncCallInfo = new AsyncCallInfo<OfflineMatchmaker>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(callback);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.OfflineMatchmaker_GetAttributes(swigCPtr, jarg);
		}

		public void SetAttributes(Dictionary<string, Variant> attributes, SetAttributesCallback callback)
		{
			AsyncCallInfo<OfflineMatchmaker> asyncCallInfo = new AsyncCallInfo<OfflineMatchmaker>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(callback);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.OfflineMatchmaker_SetAttributes(swigCPtr, VariantDict.getCPtr(attributes.ToSwigDict()), jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void MatchUsers(string matchingFunctionName, Dictionary<string, Variant> functionArguments, MatchUsersCallback callback, int maxResults)
		{
			AsyncCallInfo<OfflineMatchmaker> asyncCallInfo = new AsyncCallInfo<OfflineMatchmaker>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(callback);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.OfflineMatchmaker_MatchUsers_0(swigCPtr, matchingFunctionName, VariantDict.getCPtr(functionArguments.ToSwigDict()), jarg, maxResults);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void MatchUsers(string matchingFunctionName, Dictionary<string, Variant> functionArguments, MatchUsersCallback callback)
		{
			AsyncCallInfo<OfflineMatchmaker> asyncCallInfo = new AsyncCallInfo<OfflineMatchmaker>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(callback);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.OfflineMatchmaker_MatchUsers_1(swigCPtr, matchingFunctionName, VariantDict.getCPtr(functionArguments.ToSwigDict()), jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		private static void OnMatchUsersCallback(MatchUsersCallback cb, ResultCode result, List<string> matchedAccountIds)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(result, matchedAccountIds);
		}

		private static void OnSetAttributesCallback(SetAttributesCallback cb, ResultCode result)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(result);
		}

		private static void OnGetAttributesCallback(GetAttributesCallback cb, ResultCode result, Dictionary<string, Variant> attributes)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(result, attributes);
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnMatchUsersCallback;
			swigDelegate1 = SwigDirectorOnSetAttributesCallback;
			swigDelegate2 = SwigDirectorOnGetAttributesCallback;
			RCSSDKPINVOKE.OfflineMatchmaker_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1), Marshal.GetFunctionPointerForDelegate(swigDelegate2));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(OfflineMatchmaker));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateOfflineMatchmaker_0))]
		private static void SwigDirectorOnMatchUsersCallback(IntPtr cb, int result, IntPtr matchedAccountIds)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.OfflineMatchmaker] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<OfflineMatchmaker> asyncCallInfo = (AsyncCallInfo<OfflineMatchmaker>)gCHandle.Target;
			MatchUsersCallback handler = asyncCallInfo.GetHandler<MatchUsersCallback>();
			try
			{
				OnMatchUsersCallback(handler, (ResultCode)result, new StringList(matchedAccountIds, false).ToList());
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateOfflineMatchmaker_1))]
		private static void SwigDirectorOnSetAttributesCallback(IntPtr cb, int result)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.OfflineMatchmaker] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<OfflineMatchmaker> asyncCallInfo = (AsyncCallInfo<OfflineMatchmaker>)gCHandle.Target;
			SetAttributesCallback handler = asyncCallInfo.GetHandler<SetAttributesCallback>();
			try
			{
				OnSetAttributesCallback(handler, (ResultCode)result);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateOfflineMatchmaker_2))]
		private static void SwigDirectorOnGetAttributesCallback(IntPtr cb, int result, IntPtr attributes)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.OfflineMatchmaker] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<OfflineMatchmaker> asyncCallInfo = (AsyncCallInfo<OfflineMatchmaker>)gCHandle.Target;
			GetAttributesCallback handler = asyncCallInfo.GetHandler<GetAttributesCallback>();
			try
			{
				OnGetAttributesCallback(handler, (ResultCode)result, new VariantDict(attributes, false).ToDictionary());
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}
	}
}

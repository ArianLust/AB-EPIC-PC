using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public sealed class Identity : IdentitySessionBase
	{
		public enum LoginMethod
		{
			LoginAuto = 0,
			LoginGuest = 1,
			LoginFacebook = 2,
			LoginPlatformId = 3,
			LoginDigits = 4
		}

		public enum LoginView
		{
			ViewSignUp = 0,
			ViewSignIn = 1
		}

		public enum ErrorCode
		{
			ErrorUserCancelledLogin = 1,
			ErrorAccountInvalid = 2,
			ErrorAccountNotConfirmed = 3,
			ErrorInvalidClient = 4,
			ErrorOther = 5
		}

		public enum StatusCode
		{
			UserGuest = 0,
			UserRegistered = 1,
			UserNotAvailable = 2
		}

		public delegate void ValidateNicknameErrorCallback(string message);

		public delegate void GetUserProfilesSuccessCallback(List<User> users);

		public delegate void AccessTokenSuccessCallback(string accessToken);

		public delegate void GetUserProfilesErrorCallback(int errorCode, string message);

		public delegate void LoginFailureCallback(int errorCode, string message);

		public delegate void AccessTokenFailureCallback(ErrorCode errorCode, string message);

		public delegate void LoginSuccessCallback();

		public delegate void ValidateNicknameSuccessCallback(bool isValid, string validationMessage);

		private delegate void SwigDelegateIdentity_0(IntPtr cb, string message);

		private delegate void SwigDelegateIdentity_1(IntPtr cb, IntPtr users);

		private delegate void SwigDelegateIdentity_2(IntPtr cb, string accessToken);

		private delegate void SwigDelegateIdentity_3(IntPtr cb, int errorCode, string message);

		private delegate void SwigDelegateIdentity_4(IntPtr cb, int errorCode, string message);

		private delegate void SwigDelegateIdentity_5(IntPtr cb, int errorCode, string message);

		private delegate void SwigDelegateIdentity_6(IntPtr cb);

		private delegate void SwigDelegateIdentity_7(IntPtr cb, bool isValid, string validationMessage);

		private IntPtr swigCPtr;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SwigDelegateIdentity_0 swigDelegate0;

		private SwigDelegateIdentity_1 swigDelegate1;

		private SwigDelegateIdentity_2 swigDelegate2;

		private SwigDelegateIdentity_3 swigDelegate3;

		private SwigDelegateIdentity_4 swigDelegate4;

		private SwigDelegateIdentity_5 swigDelegate5;

		private SwigDelegateIdentity_6 swigDelegate6;

		private SwigDelegateIdentity_7 swigDelegate7;

		internal Identity(IntPtr cPtr)
			: base(RCSSDKPINVOKE.Identity_Upcast(cPtr))
		{
			swigCPtr = cPtr;
		}

		public Identity(IdentitySessionParameters arg0)
			: this(RCSSDKPINVOKE.new_Identity(IdentitySessionParameters.getCPtr(arg0)))
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		private void DefaultAccessTokenFailureCallback(ErrorCode errorCode, string message)
		{
		}

		internal static IntPtr getCPtr(Identity obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		public new void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private new void Dispose(bool disposing)
		{
			if (!disposed)
			{
				_DisposeUnmanaged();
				disposed = true;
				base.Dispose();
			}
		}

		~Identity()
		{
			Dispose(false);
		}

		private void _DisposeUnmanaged()
		{
			lock (this)
			{
				if (!(swigCPtr != IntPtr.Zero))
				{
					return;
				}
				swigCPtr = IntPtr.Zero;
				foreach (IntPtr pendingCallback in pendingCallbacks)
				{
					GCHandle.FromIntPtr(pendingCallback).Free();
				}
				pendingCallbacks.Clear();
			}
		}

		public void Login(LoginMethod method, LoginSuccessCallback onSuccess, LoginFailureCallback onFailure)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Identity> callInfo = new AsyncCallInfo<Identity>(this, onSuccess, onFailure);
			IntPtr intPtr = AddPendingCallback(callInfo);
			RCSSDKPINVOKE.Identity_Login(swigCPtr, (int)method, intPtr, intPtr);
		}

		public void LoginWithUi(LoginView view, LoginSuccessCallback onSuccess, LoginFailureCallback onFailure)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Identity> callInfo = new AsyncCallInfo<Identity>(this, onSuccess, onFailure);
			IntPtr intPtr = AddPendingCallback(callInfo);
			RCSSDKPINVOKE.Identity_LoginWithUi(swigCPtr, (int)view, intPtr, intPtr);
		}

		public void LoginWithParams(Dictionary<string, string> arg0, LoginSuccessCallback onSuccess, LoginFailureCallback onFailure)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Identity> callInfo = new AsyncCallInfo<Identity>(this, onSuccess, onFailure);
			IntPtr intPtr = AddPendingCallback(callInfo);
			RCSSDKPINVOKE.Identity_LoginWithParams(swigCPtr, StringDict.getCPtr(arg0.ToSwigDict()), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Logout()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Identity_Logout(swigCPtr);
		}

		public void FetchAccessToken(AccessTokenSuccessCallback onSuccess, AccessTokenFailureCallback onFailure)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Identity> callInfo = new AsyncCallInfo<Identity>(this, onSuccess, onFailure);
			IntPtr intPtr = AddPendingCallback(callInfo);
			RCSSDKPINVOKE.Identity_FetchAccessToken_0(swigCPtr, intPtr, intPtr);
		}

		public void FetchAccessToken(AccessTokenSuccessCallback onSuccess)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Identity> callInfo = new AsyncCallInfo<Identity>(this, onSuccess, new AccessTokenFailureCallback(DefaultAccessTokenFailureCallback));
			IntPtr intPtr = AddPendingCallback(callInfo);
			RCSSDKPINVOKE.Identity_FetchAccessToken_0(swigCPtr, intPtr, intPtr);
		}

		public string GetConfigurationParameter(string key)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			string result = RCSSDKPINVOKE.Identity_GetConfigurationParameter(swigCPtr, key);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public UserProfile GetUserProfile()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			UserProfile other = new UserProfile(RCSSDKPINVOKE.Identity_GetUserProfile(swigCPtr), false);
			return new UserProfile(other);
		}

		public void GetUserProfiles(List<string> accountIds, GetUserProfilesSuccessCallback onSuccess, GetUserProfilesErrorCallback onError)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Identity> asyncCallInfo = new AsyncCallInfo<Identity>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onError);
			RCSSDKPINVOKE.Identity_GetUserProfiles(swigCPtr, StringList.getCPtr(new StringList(accountIds)), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void ValidateNickname(string nickname, bool checkUnique, ValidateNicknameSuccessCallback onSuccess, ValidateNicknameErrorCallback onError)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Identity> asyncCallInfo = new AsyncCallInfo<Identity>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSuccess);
			asyncCallInfo.AddHandler(onError);
			RCSSDKPINVOKE.Identity_ValidateNickname(swigCPtr, nickname, checkUnique, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public StatusCode GetStatus()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return (StatusCode)RCSSDKPINVOKE.Identity_GetStatus(swigCPtr);
		}

		public override string GetSharedAccountId()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Identity_GetSharedAccountId(swigCPtr);
		}

		public override string GetAccountId()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Identity_GetAccountId(swigCPtr);
		}

		public string GetNickname()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Identity_GetNickname(swigCPtr);
		}

		public string GetAvatar(int dimension)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Identity_GetAvatar(swigCPtr, dimension);
		}

		public bool IsLoggedIn()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Identity_IsLoggedIn(swigCPtr);
		}

		public string GetAccessToken()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Identity_GetAccessToken(swigCPtr);
		}

		public string GetRefreshToken()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Identity_GetRefreshToken(swigCPtr);
		}

		public override IdentitySessionParameters GetParams()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			IdentitySessionParameters idSessionParams = new IdentitySessionParameters(RCSSDKPINVOKE.Identity_GetParams(swigCPtr), false);
			return new IdentitySessionParameters(idSessionParams);
		}

		public string GetSegment()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Identity_GetSegment(swigCPtr);
		}

		public override string GetAccessTokenString()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Identity_GetAccessTokenString(swigCPtr);
		}

		private static void OnValidateNicknameErrorCallback(ValidateNicknameErrorCallback cb, string message)
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

		private static void OnGetUserProfilesSuccessCallback(GetUserProfilesSuccessCallback cb, Users users)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(users.ToList());
		}

		private static void OnAccessTokenSuccessCallback(AccessTokenSuccessCallback cb, string accessToken)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(accessToken);
		}

		private static void OnGetUserProfilesErrorCallback(GetUserProfilesErrorCallback cb, int errorCode, string message)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(errorCode, message);
		}

		private static void OnLoginFailureCallback(LoginFailureCallback cb, int errorCode, string message)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(errorCode, message);
		}

		private static void OnAccessTokenFailureCallback(AccessTokenFailureCallback cb, ErrorCode errorCode, string message)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(errorCode, message);
		}

		private static void OnLoginSuccessCallback(LoginSuccessCallback cb)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb();
		}

		private static void OnValidateNicknameSuccessCallback(ValidateNicknameSuccessCallback cb, bool isValid, string validationMessage)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(isValid, validationMessage);
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<Identity> callInfo)
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
			swigDelegate0 = SwigDirectorOnValidateNicknameErrorCallback;
			swigDelegate1 = SwigDirectorOnGetUserProfilesSuccessCallback;
			swigDelegate2 = SwigDirectorOnAccessTokenSuccessCallback;
			swigDelegate3 = SwigDirectorOnGetUserProfilesErrorCallback;
			swigDelegate4 = SwigDirectorOnLoginFailureCallback;
			swigDelegate5 = SwigDirectorOnAccessTokenFailureCallback;
			swigDelegate6 = SwigDirectorOnLoginSuccessCallback;
			swigDelegate7 = SwigDirectorOnValidateNicknameSuccessCallback;
			RCSSDKPINVOKE.Identity_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1), Marshal.GetFunctionPointerForDelegate(swigDelegate2), Marshal.GetFunctionPointerForDelegate(swigDelegate3), Marshal.GetFunctionPointerForDelegate(swigDelegate4), Marshal.GetFunctionPointerForDelegate(swigDelegate5), Marshal.GetFunctionPointerForDelegate(swigDelegate6), Marshal.GetFunctionPointerForDelegate(swigDelegate7));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(Identity));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateIdentity_0))]
		private static void SwigDirectorOnValidateNicknameErrorCallback(IntPtr cb, string message)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Identity] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Identity> asyncCallInfo = (AsyncCallInfo<Identity>)gCHandle.Target;
			ValidateNicknameErrorCallback handler = asyncCallInfo.GetHandler<ValidateNicknameErrorCallback>();
			OnValidateNicknameErrorCallback(handler, message);
			gCHandle.Free();
			asyncCallInfo.Service.RemovePendingCallback(cb);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateIdentity_1))]
		private static void SwigDirectorOnGetUserProfilesSuccessCallback(IntPtr cb, IntPtr users)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Identity] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Identity> asyncCallInfo = (AsyncCallInfo<Identity>)gCHandle.Target;
			GetUserProfilesSuccessCallback handler = asyncCallInfo.GetHandler<GetUserProfilesSuccessCallback>();
			OnGetUserProfilesSuccessCallback(handler, new Users(users, false));
			gCHandle.Free();
			asyncCallInfo.Service.RemovePendingCallback(cb);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateIdentity_2))]
		private static void SwigDirectorOnAccessTokenSuccessCallback(IntPtr cb, string accessToken)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Identity] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Identity> asyncCallInfo = (AsyncCallInfo<Identity>)gCHandle.Target;
			AccessTokenSuccessCallback handler = asyncCallInfo.GetHandler<AccessTokenSuccessCallback>();
			OnAccessTokenSuccessCallback(handler, accessToken);
			gCHandle.Free();
			asyncCallInfo.Service.RemovePendingCallback(cb);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateIdentity_3))]
		private static void SwigDirectorOnGetUserProfilesErrorCallback(IntPtr cb, int errorCode, string message)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Identity] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Identity> asyncCallInfo = (AsyncCallInfo<Identity>)gCHandle.Target;
			GetUserProfilesErrorCallback handler = asyncCallInfo.GetHandler<GetUserProfilesErrorCallback>();
			OnGetUserProfilesErrorCallback(handler, errorCode, message);
			gCHandle.Free();
			asyncCallInfo.Service.RemovePendingCallback(cb);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateIdentity_4))]
		private static void SwigDirectorOnLoginFailureCallback(IntPtr cb, int errorCode, string message)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Identity] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Identity> asyncCallInfo = (AsyncCallInfo<Identity>)gCHandle.Target;
			LoginFailureCallback handler = asyncCallInfo.GetHandler<LoginFailureCallback>();
			OnLoginFailureCallback(handler, errorCode, message);
			gCHandle.Free();
			asyncCallInfo.Service.RemovePendingCallback(cb);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateIdentity_5))]
		private static void SwigDirectorOnAccessTokenFailureCallback(IntPtr cb, int errorCode, string message)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Identity] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Identity> asyncCallInfo = (AsyncCallInfo<Identity>)gCHandle.Target;
			AccessTokenFailureCallback handler = asyncCallInfo.GetHandler<AccessTokenFailureCallback>();
			OnAccessTokenFailureCallback(handler, (ErrorCode)errorCode, message);
			gCHandle.Free();
			asyncCallInfo.Service.RemovePendingCallback(cb);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateIdentity_6))]
		private static void SwigDirectorOnLoginSuccessCallback(IntPtr cb)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Identity] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Identity> asyncCallInfo = (AsyncCallInfo<Identity>)gCHandle.Target;
			LoginSuccessCallback handler = asyncCallInfo.GetHandler<LoginSuccessCallback>();
			OnLoginSuccessCallback(handler);
			gCHandle.Free();
			asyncCallInfo.Service.RemovePendingCallback(cb);
		}

		[MonoPInvokeCallback(typeof(SwigDelegateIdentity_7))]
		private static void SwigDirectorOnValidateNicknameSuccessCallback(IntPtr cb, bool isValid, string validationMessage)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Identity] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Identity> asyncCallInfo = (AsyncCallInfo<Identity>)gCHandle.Target;
			ValidateNicknameSuccessCallback handler = asyncCallInfo.GetHandler<ValidateNicknameSuccessCallback>();
			OnValidateNicknameSuccessCallback(handler, isValid, validationMessage);
			gCHandle.Free();
			asyncCallInfo.Service.RemovePendingCallback(cb);
		}
	}
}

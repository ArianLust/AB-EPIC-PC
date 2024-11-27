using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class Social : IDisposable
	{
		public class User : IDisposable
		{
			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			public string UserId
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Social_User_UserId_get(swigCPtr);
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
					RCSSDKPINVOKE.Social_User_UserId_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string UserName
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Social_User_UserName_get(swigCPtr);
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
					RCSSDKPINVOKE.Social_User_UserName_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string Name
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Social_User_Name_get(swigCPtr);
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
					RCSSDKPINVOKE.Social_User_Name_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string ProfileImageUrl
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Social_User_ProfileImageUrl_get(swigCPtr);
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
					RCSSDKPINVOKE.Social_User_ProfileImageUrl_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public Dictionary<string, string> CustomParams
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					IntPtr intPtr = RCSSDKPINVOKE.Social_User_CustomParams_get(swigCPtr);
					if (intPtr == IntPtr.Zero)
					{
						return null;
					}
					return new StringDict(intPtr, false).ToDictionary();
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Social_User_CustomParams_set(swigCPtr, StringDict.getCPtr(value.ToSwigDict()));
				}
			}

			internal User(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public User(User user)
				: this(RCSSDKPINVOKE.new_Social_User(), true)
			{
				UserId = user.UserId;
				UserName = user.UserName;
				Name = user.Name;
				ProfileImageUrl = user.ProfileImageUrl;
				CustomParams = user.CustomParams;
			}

			public User()
				: this(RCSSDKPINVOKE.new_Social_User(), true)
			{
			}

			internal static IntPtr getCPtr(User obj)
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

			~User()
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
							RCSSDKPINVOKE.delete_Social_User(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}

			public override string ToString()
			{
				return RCSSDKPINVOKE.Social_User_ToString(swigCPtr);
			}
		}

		public class Response : IDisposable
		{
			public enum ResultType
			{
				Cancelled = 0,
				Success = 1,
				Failed = 2
			}

			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			public ResultType Result
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return (ResultType)RCSSDKPINVOKE.Social_Response_Result_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Social_Response_Result_set(swigCPtr, (int)value);
				}
			}

			public Service Service
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return (Service)RCSSDKPINVOKE.Social_Response_Service_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Social_Response_Service_set(swigCPtr, (int)value);
				}
			}

			public int SocialNetworkReturnCode
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return RCSSDKPINVOKE.Social_Response_SocialNetworkReturnCode_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Social_Response_SocialNetworkReturnCode_set(swigCPtr, value);
				}
			}

			public string SocialNetworkMessage
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Social_Response_SocialNetworkMessage_get(swigCPtr);
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
					RCSSDKPINVOKE.Social_Response_SocialNetworkMessage_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			internal Response(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public Response()
				: this(RCSSDKPINVOKE.new_Social_Response_0(), true)
			{
			}

			public Response(Response response)
				: this(RCSSDKPINVOKE.new_Social_Response_1(getCPtr(response)), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
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
							RCSSDKPINVOKE.delete_Social_Response(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}

			public override string ToString()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Social_Response_ToString(swigCPtr);
			}
		}

		public class GetUserProfileResponse : Response
		{
			private IntPtr swigCPtr;

			private bool disposed;

			public User UserProfile
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					IntPtr intPtr = RCSSDKPINVOKE.Social_GetUserProfileResponse_UserProfile_get(swigCPtr);
					if (intPtr == IntPtr.Zero)
					{
						return null;
					}
					User user = new User(intPtr, false);
					return new User(user);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Social_GetUserProfileResponse_UserProfile_set(swigCPtr, User.getCPtr(value));
				}
			}

			public string AccessToken
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Social_GetUserProfileResponse_AccessToken_get(swigCPtr);
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
					RCSSDKPINVOKE.Social_GetUserProfileResponse_AccessToken_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string AppId
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Social_GetUserProfileResponse_AppId_get(swigCPtr);
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
					RCSSDKPINVOKE.Social_GetUserProfileResponse_AppId_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			internal GetUserProfileResponse(IntPtr cPtr, bool cMemoryOwn)
				: base(RCSSDKPINVOKE.Social_GetUserProfileResponse_Upcast(cPtr), cMemoryOwn)
			{
				swigCPtr = cPtr;
			}

			public GetUserProfileResponse()
				: this(RCSSDKPINVOKE.new_Social_GetUserProfileResponse_0(), true)
			{
			}

			public GetUserProfileResponse(GetUserProfileResponse responce)
				: this(RCSSDKPINVOKE.new_Social_GetUserProfileResponse_1(getCPtr(responce)), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			internal static IntPtr getCPtr(GetUserProfileResponse obj)
			{
				return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
			}

			protected new void Dispose(bool disposing)
			{
				if (!disposed)
				{
					_DisposeUnmanaged();
					disposed = true;
					Dispose();
				}
			}

			~GetUserProfileResponse()
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
							RCSSDKPINVOKE.delete_Social_GetUserProfileResponse(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}

			public override string ToString()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Social_GetUserProfileResponse_ToString(swigCPtr);
			}
		}

		public class SharingRequest : IDisposable
		{
			public enum ShareType
			{
				Status = 0,
				Video = 1,
				Score = 2
			}

			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			public ShareType SharingType
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return (ShareType)RCSSDKPINVOKE.Social_SharingRequest_SharingType_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Social_SharingRequest_SharingType_set(swigCPtr, (int)value);
				}
			}

			public string Title
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Social_SharingRequest_Title_get(swigCPtr);
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
					RCSSDKPINVOKE.Social_SharingRequest_Title_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string Text
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Social_SharingRequest_Text_get(swigCPtr);
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
					RCSSDKPINVOKE.Social_SharingRequest_Text_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string ImageUrl
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Social_SharingRequest_ImageUrl_get(swigCPtr);
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
					RCSSDKPINVOKE.Social_SharingRequest_ImageUrl_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string Url
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Social_SharingRequest_Url_get(swigCPtr);
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
					RCSSDKPINVOKE.Social_SharingRequest_Url_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			internal SharingRequest(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public SharingRequest()
				: this(RCSSDKPINVOKE.new_Social_SharingRequest(), true)
			{
			}

			internal static IntPtr getCPtr(SharingRequest obj)
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

			~SharingRequest()
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
							RCSSDKPINVOKE.delete_Social_SharingRequest(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}
		}

		public class SharingResponse : Response
		{
			private IntPtr swigCPtr;

			private bool disposed;

			public string SharedPostId
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Social_SharingResponse_SharedPostId_get(swigCPtr);
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
					RCSSDKPINVOKE.Social_SharingResponse_SharedPostId_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			internal SharingResponse(IntPtr cPtr, bool cMemoryOwn)
				: base(RCSSDKPINVOKE.Social_SharingResponse_Upcast(cPtr), cMemoryOwn)
			{
				swigCPtr = cPtr;
			}

			public SharingResponse()
				: this(RCSSDKPINVOKE.new_Social_SharingResponse_0(), true)
			{
			}

			public SharingResponse(SharingResponse response)
				: this(RCSSDKPINVOKE.new_Social_SharingResponse_1(getCPtr(response)), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			internal static IntPtr getCPtr(SharingResponse obj)
			{
				return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
			}

			protected new void Dispose(bool disposing)
			{
				if (!disposed)
				{
					_DisposeUnmanaged();
					disposed = true;
					Dispose();
				}
			}

			~SharingResponse()
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
							RCSSDKPINVOKE.delete_Social_SharingResponse(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}

			public override string ToString()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Social_SharingResponse_ToString(swigCPtr);
			}
		}

		public class GetFriendsRequest : IDisposable
		{
			public enum GetFriendsType
			{
				IdOnly = 0,
				FullProfile = 1
			}

			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			public GetFriendsType FriendsType
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return (GetFriendsType)RCSSDKPINVOKE.Social_GetFriendsRequest_FriendsType_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Social_GetFriendsRequest_FriendsType_set(swigCPtr, (int)value);
				}
			}

			public string Pagination
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Social_GetFriendsRequest_Pagination_get(swigCPtr);
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
					RCSSDKPINVOKE.Social_GetFriendsRequest_Pagination_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			internal GetFriendsRequest(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public GetFriendsRequest()
				: this(RCSSDKPINVOKE.new_Social_GetFriendsRequest(), true)
			{
			}

			internal static IntPtr getCPtr(GetFriendsRequest obj)
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

			~GetFriendsRequest()
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
							RCSSDKPINVOKE.delete_Social_GetFriendsRequest(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}
		}

		public class GetFriendsResponse : Response
		{
			private IntPtr swigCPtr;

			private bool disposed;

			public List<User> Friends
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					IntPtr intPtr = RCSSDKPINVOKE.Social_GetFriendsResponse_Friends_get(swigCPtr);
					if (intPtr == IntPtr.Zero)
					{
						return null;
					}
					return new SocialUser(intPtr, false).ToList();
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Social_GetFriendsResponse_Friends_set(swigCPtr, SocialUser.getCPtr(new SocialUser(value)));
				}
			}

			public string NextPage
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Social_GetFriendsResponse_NextPage_get(swigCPtr);
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
					RCSSDKPINVOKE.Social_GetFriendsResponse_NextPage_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			internal GetFriendsResponse(IntPtr cPtr, bool cMemoryOwn)
				: base(RCSSDKPINVOKE.Social_GetFriendsResponse_Upcast(cPtr), cMemoryOwn)
			{
				swigCPtr = cPtr;
			}

			public GetFriendsResponse()
				: this(RCSSDKPINVOKE.new_Social_GetFriendsResponse_0(), true)
			{
			}

			public GetFriendsResponse(GetFriendsResponse arg0)
				: this(RCSSDKPINVOKE.new_Social_GetFriendsResponse_1(getCPtr(arg0)), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			internal static IntPtr getCPtr(GetFriendsResponse obj)
			{
				return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
			}

			protected new void Dispose(bool disposing)
			{
				if (!disposed)
				{
					_DisposeUnmanaged();
					disposed = true;
					Dispose();
				}
			}

			~GetFriendsResponse()
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
							RCSSDKPINVOKE.delete_Social_GetFriendsResponse(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}

			public override string ToString()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Social_GetFriendsResponse_ToString(swigCPtr);
			}
		}

		public class AppRequest : IDisposable
		{
			public enum UserInteractionMode
			{
				PromptConfirmationDirected = 0,
				PromptConfirmationSuggested = 1,
				NoConfirmation = 2
			}

			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			public UserInteractionMode InteractionMode
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return (UserInteractionMode)RCSSDKPINVOKE.Social_AppRequest_InteractionMode_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Social_AppRequest_InteractionMode_set(swigCPtr, (int)value);
				}
			}

			public List<string> UserIds
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					IntPtr intPtr = RCSSDKPINVOKE.Social_AppRequest_UserIds_get(swigCPtr);
					if (intPtr == IntPtr.Zero)
					{
						return null;
					}
					return new StringList(intPtr, false).ToList();
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Social_AppRequest_UserIds_set(swigCPtr, StringList.getCPtr(new StringList(value)));
				}
			}

			public string Title
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Social_AppRequest_Title_get(swigCPtr);
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
					RCSSDKPINVOKE.Social_AppRequest_Title_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string Message
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Social_AppRequest_Message_get(swigCPtr);
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
					RCSSDKPINVOKE.Social_AppRequest_Message_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public Dictionary<string, string> CustomParams
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					IntPtr intPtr = RCSSDKPINVOKE.Social_AppRequest_CustomParams_get(swigCPtr);
					if (intPtr == IntPtr.Zero)
					{
						return null;
					}
					return new StringDict(intPtr, false).ToDictionary();
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Social_AppRequest_CustomParams_set(swigCPtr, StringDict.getCPtr(value.ToSwigDict()));
				}
			}

			internal AppRequest(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public AppRequest()
				: this(RCSSDKPINVOKE.new_Social_AppRequest(), true)
			{
			}

			internal static IntPtr getCPtr(AppRequest obj)
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

			~AppRequest()
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
							RCSSDKPINVOKE.delete_Social_AppRequest(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}
		}

		public class AppInviteRequest : IDisposable
		{
			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			public string AppLinkUrl
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Social_AppInviteRequest_AppLinkUrl_get(swigCPtr);
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
					RCSSDKPINVOKE.Social_AppInviteRequest_AppLinkUrl_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string PreviewImageUrl
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Social_AppInviteRequest_PreviewImageUrl_get(swigCPtr);
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
					RCSSDKPINVOKE.Social_AppInviteRequest_PreviewImageUrl_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			internal AppInviteRequest(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public AppInviteRequest()
				: this(RCSSDKPINVOKE.new_Social_AppInviteRequest(), true)
			{
			}

			internal static IntPtr getCPtr(AppInviteRequest obj)
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

			~AppInviteRequest()
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
							RCSSDKPINVOKE.delete_Social_AppInviteRequest(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}
		}

		public class AppLinkData : IDisposable
		{
			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			public SharingRequest.ShareType Type
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return (SharingRequest.ShareType)RCSSDKPINVOKE.Social_AppLinkData_Type_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.Social_AppLinkData_Type_set(swigCPtr, (int)value);
				}
			}

			public string BaseUrl
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.Social_AppLinkData_BaseUrl_get(swigCPtr);
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
					RCSSDKPINVOKE.Social_AppLinkData_BaseUrl_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			internal AppLinkData(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public AppLinkData()
				: this(RCSSDKPINVOKE.new_Social_AppLinkData(), true)
			{
			}

			internal static IntPtr getCPtr(AppLinkData obj)
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

			~AppLinkData()
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
							RCSSDKPINVOKE.delete_Social_AppLinkData(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}
		}

		public enum Service
		{
			ServiceUnknown = 0,
			ServiceFacebook = 1,
			ServiceOthers = 2,
			ServicePlatform = 3,
			ServiceDigits = 4
		}

		public delegate void LoginCallback(bool success, string account);

		public delegate void AppRequestCallback(Response response);

		public delegate void SharingStartCallback();

		public delegate void GetFriendsCallback(GetFriendsResponse response);

		public delegate void SharingCallback(SharingResponse response);

		public delegate void SharingAggregatedCallback(List<SharingResponse> responses);

		public delegate void GetUserProfileCallback(GetUserProfileResponse response);

		private delegate void SwigDelegateSocial_0(IntPtr cb, bool success, string account);

		private delegate void SwigDelegateSocial_1(IntPtr cb, IntPtr response);

		private delegate void SwigDelegateSocial_2(IntPtr cb);

		private delegate void SwigDelegateSocial_3(IntPtr cb, IntPtr response);

		private delegate void SwigDelegateSocial_4(IntPtr cb, IntPtr response);

		private delegate void SwigDelegateSocial_5(IntPtr cb, IntPtr responses);

		private delegate void SwigDelegateSocial_6(IntPtr cb, IntPtr response);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private static Social instance;

		private SwigDelegateSocial_0 swigDelegate0;

		private SwigDelegateSocial_1 swigDelegate1;

		private SwigDelegateSocial_2 swigDelegate2;

		private SwigDelegateSocial_3 swigDelegate3;

		private SwigDelegateSocial_4 swigDelegate4;

		private SwigDelegateSocial_5 swigDelegate5;

		private SwigDelegateSocial_6 swigDelegate6;

		internal Social(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<Social> callInfo)
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

		internal static IntPtr getCPtr(Social obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~Social()
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
						RCSSDKPINVOKE.delete_Social(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public static Social GetInstance()
		{
			if (instance == null || getCPtr(instance) == IntPtr.Zero)
			{
				IntPtr intPtr = RCSSDKPINVOKE.Social_getInstance_private();
				instance = ((!(intPtr == IntPtr.Zero)) ? new Social(intPtr, false) : null);
			}
			return instance;
		}

		private static Social getInstance_private()
		{
			IntPtr intPtr = RCSSDKPINVOKE.Social_getInstance_private();
			return (!(intPtr == IntPtr.Zero)) ? new Social(intPtr, false) : null;
		}

		public virtual void Configure(List<Service> services)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Social_Configure(swigCPtr, SocialServices.getCPtr(new SocialServices(services)));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public virtual int GetNumOfServices()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Social_GetNumOfServices(swigCPtr);
		}

		public virtual List<Service> GetServices()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			SocialServices srcList = new SocialServices(RCSSDKPINVOKE.Social_GetServices(swigCPtr), false);
			return srcList.ToList();
		}

		public virtual void Login(Service service, LoginCallback callback)
		{
			AsyncCallInfo<Social> asyncCallInfo = new AsyncCallInfo<Social>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(callback);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Social_Login(swigCPtr, (int)service, jarg);
		}

		public virtual void Logout(Service service)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Social_Logout(swigCPtr, (int)service);
		}

		public virtual bool IsLoggedIn(Service service)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Social_IsLoggedIn(swigCPtr, (int)service);
		}

		public virtual void Share(SharingRequest request, Service service, SharingCallback callback)
		{
			AsyncCallInfo<Social> asyncCallInfo = new AsyncCallInfo<Social>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(callback);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Social_Share(swigCPtr, SharingRequest.getCPtr(request), (int)service, jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public virtual void GetUserProfile(Service service, GetUserProfileCallback callback)
		{
			AsyncCallInfo<Social> asyncCallInfo = new AsyncCallInfo<Social>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(callback);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Social_GetUserProfile(swigCPtr, (int)service, jarg);
		}

		public virtual void GetFriends(GetFriendsRequest request, Service service, GetFriendsCallback callback)
		{
			AsyncCallInfo<Social> asyncCallInfo = new AsyncCallInfo<Social>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(callback);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Social_GetFriends(swigCPtr, GetFriendsRequest.getCPtr(request), (int)service, jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public virtual void SendAppRequest(AppRequest request, Service service, AppRequestCallback callback)
		{
			AsyncCallInfo<Social> asyncCallInfo = new AsyncCallInfo<Social>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(callback);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Social_SendAppRequest(swigCPtr, AppRequest.getCPtr(request), (int)service, jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public virtual void SendAppInviteRequest(AppInviteRequest request, Service service, AppRequestCallback callback)
		{
			AsyncCallInfo<Social> asyncCallInfo = new AsyncCallInfo<Social>(this);
			IntPtr jarg = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(callback);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Social_SendAppInviteRequest(swigCPtr, AppInviteRequest.getCPtr(request), (int)service, jarg);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public virtual bool OnOpenUrl(string url, AppLinkData data, string sourceApplication)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			bool result = RCSSDKPINVOKE.Social_OnOpenUrl_0(swigCPtr, url, AppLinkData.getCPtr(data), sourceApplication);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public virtual bool OnOpenUrl(string url, AppLinkData data)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			bool result = RCSSDKPINVOKE.Social_OnOpenUrl_1(swigCPtr, url, AppLinkData.getCPtr(data));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public virtual void OnActivate(bool active)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Social_OnActivate(swigCPtr, active);
		}

		public virtual Dictionary<string, string> GetSocialNetworkGlobalParameters()
		{
			StringDict srcDict = new StringDict(RCSSDKPINVOKE.Social_GetSocialNetworkGlobalParameters(swigCPtr), false);
			return srcDict.ToDictionary();
		}

		public virtual void SetSocialNetworkGlobalParameters(Dictionary<string, string> socialNetworkParameters)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Social_SetSocialNetworkGlobalParameters(swigCPtr, StringDict.getCPtr(socialNetworkParameters.ToSwigDict()));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public static string GetServiceName(Service service)
		{
			return RCSSDKPINVOKE.Social_GetServiceName((int)service);
		}

		public static Service GetServiceByName(string serviceName)
		{
			Service result = (Service)RCSSDKPINVOKE.Social_GetServiceByName(serviceName);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		private static void OnLoginCallback(LoginCallback cb, bool success, string account)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(success, account);
		}

		private static void OnAppRequestCallback(AppRequestCallback cb, Response response)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(response);
		}

		private static void OnSharingStartCallback(SharingStartCallback cb)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb();
		}

		private static void OnGetFriendsCallback(GetFriendsCallback cb, GetFriendsResponse response)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(response);
		}

		private static void OnSharingCallback(SharingCallback cb, SharingResponse response)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(response);
		}

		private static void OnSharingAggregatedCallback(SharingAggregatedCallback cb, List<SharingResponse> responses)
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

		private static void OnGetUserProfileCallback(GetUserProfileCallback cb, GetUserProfileResponse response)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(response);
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnLoginCallback;
			swigDelegate1 = SwigDirectorOnAppRequestCallback;
			swigDelegate2 = SwigDirectorOnSharingStartCallback;
			swigDelegate3 = SwigDirectorOnGetFriendsCallback;
			swigDelegate4 = SwigDirectorOnSharingCallback;
			swigDelegate5 = SwigDirectorOnSharingAggregatedCallback;
			swigDelegate6 = SwigDirectorOnGetUserProfileCallback;
			RCSSDKPINVOKE.Social_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1), Marshal.GetFunctionPointerForDelegate(swigDelegate2), Marshal.GetFunctionPointerForDelegate(swigDelegate3), Marshal.GetFunctionPointerForDelegate(swigDelegate4), Marshal.GetFunctionPointerForDelegate(swigDelegate5), Marshal.GetFunctionPointerForDelegate(swigDelegate6));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(Social));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateSocial_0))]
		private static void SwigDirectorOnLoginCallback(IntPtr cb, bool success, string account)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Social] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Social> asyncCallInfo = (AsyncCallInfo<Social>)gCHandle.Target;
			LoginCallback handler = asyncCallInfo.GetHandler<LoginCallback>();
			try
			{
				OnLoginCallback(handler, success, account);
			}
			finally
			{
				if (!"LoginCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateSocial_1))]
		private static void SwigDirectorOnAppRequestCallback(IntPtr cb, IntPtr response)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Social] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Social> asyncCallInfo = (AsyncCallInfo<Social>)gCHandle.Target;
			AppRequestCallback handler = asyncCallInfo.GetHandler<AppRequestCallback>();
			try
			{
				OnAppRequestCallback(handler, new Response(response, false));
			}
			finally
			{
				if (!"AppRequestCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateSocial_2))]
		private static void SwigDirectorOnSharingStartCallback(IntPtr cb)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Social] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Social> asyncCallInfo = (AsyncCallInfo<Social>)gCHandle.Target;
			SharingStartCallback handler = asyncCallInfo.GetHandler<SharingStartCallback>();
			try
			{
				OnSharingStartCallback(handler);
			}
			finally
			{
				if (!"SharingStartCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateSocial_3))]
		private static void SwigDirectorOnGetFriendsCallback(IntPtr cb, IntPtr response)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Social] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Social> asyncCallInfo = (AsyncCallInfo<Social>)gCHandle.Target;
			GetFriendsCallback handler = asyncCallInfo.GetHandler<GetFriendsCallback>();
			try
			{
				OnGetFriendsCallback(handler, new GetFriendsResponse(response, false));
			}
			finally
			{
				if (!"GetFriendsCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateSocial_4))]
		private static void SwigDirectorOnSharingCallback(IntPtr cb, IntPtr response)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Social] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Social> asyncCallInfo = (AsyncCallInfo<Social>)gCHandle.Target;
			SharingCallback handler = asyncCallInfo.GetHandler<SharingCallback>();
			try
			{
				OnSharingCallback(handler, new SharingResponse(response, false));
			}
			finally
			{
				if (!"SharingCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateSocial_5))]
		private static void SwigDirectorOnSharingAggregatedCallback(IntPtr cb, IntPtr responses)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Social] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Social> asyncCallInfo = (AsyncCallInfo<Social>)gCHandle.Target;
			SharingAggregatedCallback handler = asyncCallInfo.GetHandler<SharingAggregatedCallback>();
			try
			{
				OnSharingAggregatedCallback(handler, new SocialSharingResponses(responses, false).ToList());
			}
			finally
			{
				if (!"SharingAggregatedCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateSocial_6))]
		private static void SwigDirectorOnGetUserProfileCallback(IntPtr cb, IntPtr response)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Social] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Social> asyncCallInfo = (AsyncCallInfo<Social>)gCHandle.Target;
			GetUserProfileCallback handler = asyncCallInfo.GetHandler<GetUserProfileCallback>();
			try
			{
				OnGetUserProfileCallback(handler, new GetUserProfileResponse(response, false));
			}
			finally
			{
				if (!"GetUserProfileCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}
	}
}

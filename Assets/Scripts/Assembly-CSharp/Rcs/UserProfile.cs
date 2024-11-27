using System;
using System.Collections.Generic;

namespace Rcs
{
	public class UserProfile : IDisposable
	{
		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		public static string ProfileFirstName
		{
			get
			{
				return RCSSDKPINVOKE.UserProfile_ProfileFirstName_get();
			}
		}

		public static string ProfileLastName
		{
			get
			{
				return RCSSDKPINVOKE.UserProfile_ProfileLastName_get();
			}
		}

		public static string ProfileBirthday
		{
			get
			{
				return RCSSDKPINVOKE.UserProfile_ProfileBirthday_get();
			}
		}

		internal UserProfile(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		public UserProfile()
			: this(RCSSDKPINVOKE.new_UserProfile_0(), true)
		{
		}

		public UserProfile(UserProfile other)
			: this(RCSSDKPINVOKE.new_UserProfile_1(getCPtr(other)), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public UserProfile(string account, string sharedId, Dictionary<string, string> parameters, Dictionary<string, string> facebook_parameters, List<User.SocialNetworkProfile> connectedSocialNetworks, List<User.AvatarAsset> avatarAssets_parameters, User.SocialNetworkProfile loggedInSocialNetwork)
			: this(RCSSDKPINVOKE.new_UserProfile_2(account, sharedId, StringDict.getCPtr(parameters.ToSwigDict()), StringDict.getCPtr(facebook_parameters.ToSwigDict()), SocialNetworkProfiles.getCPtr(new SocialNetworkProfiles(connectedSocialNetworks)), AvatarAssets.getCPtr(new AvatarAssets(avatarAssets_parameters)), User.SocialNetworkProfile.getCPtr(loggedInSocialNetwork)), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public UserProfile(string account, string sharedId, Dictionary<string, string> parameters, Dictionary<string, string> facebook_parameters, List<User.SocialNetworkProfile> connectedSocialNetworks, List<User.AvatarAsset> avatarAssets_parameters)
			: this(RCSSDKPINVOKE.new_UserProfile_3(account, sharedId, StringDict.getCPtr(parameters.ToSwigDict()), StringDict.getCPtr(facebook_parameters.ToSwigDict()), SocialNetworkProfiles.getCPtr(new SocialNetworkProfiles(connectedSocialNetworks)), AvatarAssets.getCPtr(new AvatarAssets(avatarAssets_parameters))), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		internal static IntPtr getCPtr(UserProfile obj)
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

		~UserProfile()
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
						RCSSDKPINVOKE.delete_UserProfile(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public string GetSharedAccountId()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.UserProfile_GetSharedAccountId(swigCPtr);
		}

		public string GetNickname()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.UserProfile_GetNickname(swigCPtr);
		}

		public string GetEmailAddress()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.UserProfile_GetEmailAddress(swigCPtr);
		}

		public string GetAvatar(int dimension)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.UserProfile_GetAvatar(swigCPtr, dimension);
		}

		public void SetAvatarAssets(int key, string value)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.UserProfile_SetAvatarAssets(swigCPtr, key, value);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public List<User.AvatarAsset> GetAvatarAssetsParameters()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AvatarAssets srcList = new AvatarAssets(RCSSDKPINVOKE.UserProfile_GetAvatarAssetsParameters(swigCPtr), false);
			return srcList.ToList();
		}

		public User.SocialNetworkProfile GetLoggedInSocialNetwork()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new User.SocialNetworkProfile(RCSSDKPINVOKE.UserProfile_GetLoggedInSocialNetwork(swigCPtr), true);
		}

		public List<User.SocialNetworkProfile> GetConnectedSocialNetworks()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			SocialNetworkProfiles srcList = new SocialNetworkProfiles(RCSSDKPINVOKE.UserProfile_GetConnectedSocialNetworks(swigCPtr), false);
			return srcList.ToList();
		}

		public string GetParameter(string key)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			string result = RCSSDKPINVOKE.UserProfile_GetParameter(swigCPtr, key);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public void SetParameter(string key, string value)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.UserProfile_SetParameter(swigCPtr, key, value);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public Dictionary<string, string> GetParameters()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			StringDict srcDict = new StringDict(RCSSDKPINVOKE.UserProfile_GetParameters(swigCPtr), false);
			return srcDict.ToDictionary();
		}

		public string GetAccountId()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.UserProfile_GetAccountId(swigCPtr);
		}

		public Dictionary<string, string> GetFacebookParameters()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			StringDict srcDict = new StringDict(RCSSDKPINVOKE.UserProfile_GetFacebookParameters(swigCPtr), false);
			return srcDict.ToDictionary();
		}
	}
}

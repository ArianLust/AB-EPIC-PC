using System;
using System.Collections.Generic;

namespace Rcs
{
	public class User : IDisposable
	{
		public class SocialNetworkProfile : IDisposable
		{
			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			public SocialNetwork SocialNetwork
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return (SocialNetwork)RCSSDKPINVOKE.User_SocialNetworkProfile_SocialNetwork_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.User_SocialNetworkProfile_SocialNetwork_set(swigCPtr, (int)value);
				}
			}

			public string Uid
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.User_SocialNetworkProfile_Uid_get(swigCPtr);
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
					RCSSDKPINVOKE.User_SocialNetworkProfile_Uid_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string AvatarUrl
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.User_SocialNetworkProfile_AvatarUrl_get(swigCPtr);
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
					RCSSDKPINVOKE.User_SocialNetworkProfile_AvatarUrl_set(swigCPtr, value);
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
					string result = RCSSDKPINVOKE.User_SocialNetworkProfile_Name_get(swigCPtr);
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
					RCSSDKPINVOKE.User_SocialNetworkProfile_Name_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			internal SocialNetworkProfile(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public SocialNetworkProfile()
				: this(RCSSDKPINVOKE.new_User_SocialNetworkProfile_0(), true)
			{
			}

			public SocialNetworkProfile(SocialNetworkProfile profile)
				: this(RCSSDKPINVOKE.new_User_SocialNetworkProfile_1(getCPtr(profile)), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			internal static IntPtr getCPtr(SocialNetworkProfile obj)
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

			~SocialNetworkProfile()
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
							RCSSDKPINVOKE.delete_User_SocialNetworkProfile(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}

			public string GetDescription()
			{
				return RCSSDKPINVOKE.User_SocialNetworkProfile_GetDescription(swigCPtr);
			}
		}

		public class AvatarAsset : IDisposable
		{
			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			public string AvatarId
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.User_AvatarAsset_AvatarId_get(swigCPtr);
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
					RCSSDKPINVOKE.User_AvatarAsset_AvatarId_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string AvatarUrl
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.User_AvatarAsset_AvatarUrl_get(swigCPtr);
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
					RCSSDKPINVOKE.User_AvatarAsset_AvatarUrl_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string Hash
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.User_AvatarAsset_Hash_get(swigCPtr);
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
					RCSSDKPINVOKE.User_AvatarAsset_Hash_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public ulong Size
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return RCSSDKPINVOKE.User_AvatarAsset_Size_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.User_AvatarAsset_Size_set(swigCPtr, value);
				}
			}

			public int Dimension
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return RCSSDKPINVOKE.User_AvatarAsset_Dimension_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.User_AvatarAsset_Dimension_set(swigCPtr, value);
				}
			}

			internal AvatarAsset(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public AvatarAsset()
				: this(RCSSDKPINVOKE.new_User_AvatarAsset(), true)
			{
			}

			internal static IntPtr getCPtr(AvatarAsset obj)
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

			~AvatarAsset()
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
							RCSSDKPINVOKE.delete_User_AvatarAsset(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}
		}

		public enum Type
		{
			TypeGlobal = 0,
			TypeSocial = 1
		}

		public enum SocialNetwork
		{
			SocialNetworkUnknown = 0,
			SocialNetworkFacebook = 1,
			SocialNetworkPlatform = 2,
			SocialNetworkDigits = 3
		}

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		internal User(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		public User()
			: this(RCSSDKPINVOKE.new_User_0(), true)
		{
		}

		public User(User user)
			: this(RCSSDKPINVOKE.new_User_1(getCPtr(user)), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public User(string accountId)
			: this(RCSSDKPINVOKE.new_User_2(accountId), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
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
						RCSSDKPINVOKE.delete_User(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public string GetAccountId()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.User_GetAccountId(swigCPtr);
		}

		public List<SocialNetworkProfile> GetSocialNetworkProfiles()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			SocialNetworkProfiles srcList = new SocialNetworkProfiles(RCSSDKPINVOKE.User_GetSocialNetworkProfiles(swigCPtr), false);
			return srcList.ToList();
		}

		public void SetSocialNetworkProfiles(List<SocialNetworkProfile> profiles)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.User_SetSocialNetworkProfiles(swigCPtr, SocialNetworkProfiles.getCPtr(new SocialNetworkProfiles(profiles)));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public string GetName(Type type)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.User_GetName(swigCPtr, (int)type);
		}

		public string GetAvatarUrl(Type type, int dimension)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.User_GetAvatarUrl(swigCPtr, (int)type, dimension);
		}

		public void SetGlobalAvatarAssets(List<AvatarAsset> avatarAssets)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.User_SetGlobalAvatarAssets(swigCPtr, AvatarAssets.getCPtr(new AvatarAssets(avatarAssets)));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public List<AvatarAsset> GetGlobalAvatarAssets()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AvatarAssets srcList = new AvatarAssets(RCSSDKPINVOKE.User_GetGlobalAvatarAssets(swigCPtr), false);
			return srcList.ToList();
		}

		public string GetDescription()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.User_GetDescription(swigCPtr);
		}

		public void SetNickName(string nickName)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.User_SetNickName(swigCPtr, nickName);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}
	}
}

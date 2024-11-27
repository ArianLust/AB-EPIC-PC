using System;
using System.Runtime.CompilerServices;
using Interfaces.Identity;
using Rcs;

public class IdentityServiceBeaconImpl : IIdentityService
{
	private UserProfile m_userProfile;

	private Identity m_identity
	{
		get
		{
			return ContentLoader.Instance.m_BeaconConnectionMgr.Identiy;
		}
	}

	public IdentityCredentials UserCredentials
	{
		get
		{
			if (m_userProfile != null)
			{
				string emailAddress = m_userProfile.GetEmailAddress();
				string avatar = m_userProfile.GetAvatar(128);
				IdentityCredentials identityCredentials = new IdentityCredentials();
				identityCredentials.Password = string.Empty;
				identityCredentials.UserName = string.Empty;
				identityCredentials.AvatarAsset = avatar;
				identityCredentials.Email = emailAddress;
				return identityCredentials;
			}
			return null;
		}
	}

	public string AccountId
	{
		get
		{
			if (m_userProfile == null)
			{
				return null;
			}
			return m_userProfile.GetAccountId();
		}
	}

	public string SharedId
	{
		get
		{
			return m_identity.GetSharedAccountId();
		}
	}

	[method: MethodImpl(32)]
	public event Action OnLoggedIn;

	[method: MethodImpl(32)]
	public event Action<int, string> OnLoginError;

	public bool IsGuest()
	{
		return UserCredentials == null || string.IsNullOrEmpty(UserCredentials.Email);
	}

	public void Initialize()
	{
		DebugLog.Log(GetType(), "Initialize");
	}

	private void DoOnLoginSuccess()
	{
		DebugLog.Log(GetType(), "OnLoggedIn");
		m_userProfile = m_identity.GetUserProfile();
		if (this.OnLoggedIn != null)
		{
			this.OnLoggedIn();
		}
	}

	private void DoOnLoginError(int errorCode, string errorMessage)
	{
		m_userProfile = null;
		DebugLog.Log(GetType(), string.Concat("OnLoginError: ", (Identity.ErrorCode)errorCode, ",", errorMessage));
		if (this.OnLoginError != null)
		{
			this.OnLoginError(errorCode, errorMessage);
		}
	}

	public void Login(Identity.LoginView view)
	{
		DebugLog.Log(GetType(), "Login with view");
		m_identity.LoginWithUi(view, DoOnLoginSuccess, DoOnLoginError);
	}

	public void LoginGuest()
	{
		DebugLog.Log(GetType(), "Login as guest started");
		m_identity.Login(Identity.LoginMethod.LoginGuest, DoOnLoginSuccess, DoOnLoginError);
	}

	public void LoginAuto()
	{
		DebugLog.Log(GetType(), "automatic login started");
		m_identity.Login(Identity.LoginMethod.LoginAuto, DoOnLoginSuccess, DoOnLoginError);
	}

	public void LoginFacebook()
	{
		DebugLog.Log(GetType(), "Login with facebook started");
		m_identity.Login(Identity.LoginMethod.LoginFacebook, DoOnLoginSuccess, DoOnLoginError);
	}

	public void Logout()
	{
		m_identity.Logout();
	}

	public void ValidateNickname(string nickname, Identity.ValidateNicknameSuccessCallback OnSuccess, Identity.ValidateNicknameErrorCallback OnError)
	{
		m_identity.ValidateNickname(nickname, false, OnSuccess, OnError);
	}
}

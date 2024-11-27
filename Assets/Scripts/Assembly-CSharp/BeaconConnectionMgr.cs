using System.Collections.Generic;
using ABH.Shared.Models;
using Rcs;
using UnityEngine;

public class BeaconConnectionMgr : MonoBehaviour
{
	private IdentitySessionBase m_serviceManager;

	private Friends m_friends;

	private Ads m_ads;

	private TestDevice m_testDevice;

	public Identity Identiy
	{
		get
		{
			return (Identity)m_serviceManager;
		}
	}

	public SocialEnvironmentData CachedSocialEnvironmentGameData { get; set; }

	public bool IsBeaconLoggedIn { get; private set; }

	public bool RegistrationStarted { get; set; }

	public bool IsInitialized { get; private set; }

	public Friends FriendsService
	{
		get
		{
			return m_friends ?? (m_friends = new Friends(m_serviceManager, new List<User.SocialNetwork> { User.SocialNetwork.SocialNetworkFacebook }));
		}
	}

	public Ads AdsService
	{
		get
		{
			return m_ads ?? (m_ads = new Ads(m_serviceManager));
		}
	}

	public TestDevice TestDeviceInstance
	{
		get
		{
			return m_testDevice ?? (m_testDevice = new TestDevice(m_serviceManager));
		}
	}

	public BeaconConnectionMgr Init(IMonoBehaviourContainer nonDestroyableContainer)
	{
		DebugLog.Log(GetType(), "Init");
		//SetAndroidActivity();
		CachedSocialEnvironmentGameData = new SocialEnvironmentData();
		CachedSocialEnvironmentGameData.NameId = "default";
		DebugLog.Log(GetType(), "ConnectToRovioId. Initializing RCSSDK...");
		//RCSSDK.Initialize("com.rovio", "gold");
		//DebugLog.Log(GetType(), "Creating IdentitySessionParameters...");
		//IdentitySessionParameters identitySessionParameters = new IdentitySessionParameters();
		//identitySessionParameters.DistributionChannel = "GooglePlay";
		//identitySessionParameters.ServerUrl = Rcs.Application.ServerProduction;
		//DebugLog.Log(GetType(), "Init: Server Endpoint = " + identitySessionParameters.ServerUrl);
		//identitySessionParameters.ClientVersion = DIContainerInfrastructure.GetVersionService().StoreVersion;
		//DebugLog.Log(GetType(), "setting clientVersion to \"" + identitySessionParameters.ClientVersion + "\"");
		//identitySessionParameters.Locale = DIContainerInfrastructure.GetStartupLocaService().CurrentLanguageKey;
		//identitySessionParameters.BuildId = DIContainerInfrastructure.GetVersionService().BuildNumber.ToString();
		//string clientId;
		//string clientSecret;
		//if (DIContainerConfig.GetSkynestClientIdAndSecret(out clientId, out clientSecret))
		//{
		//	identitySessionParameters.ClientId = clientId;
		//	identitySessionParameters.ClientSecret = clientSecret;
		//	m_serviceManager = new Identity(identitySessionParameters);
		//	DebugLog.Log(GetType(), "Init - setting internal logger");
		//	Rcs.Application.DisableInternalLogger();
		DIContainerInfrastructure.GetAttributionService().Init();
		DIContainerInfrastructure.GetComScoreService().Init();
		IsInitialized = true;
		DebugLog.Log(GetType(), "Initialization successful: " + IsInitialized);
		return this;
		//}
		//DebugLog.Error(GetType(), "Beacon clientId and/or clientSecret not found!");
		//return null;
	}

	public bool ConnectToRovioId()
	{
		DebugLog.Log(GetType(), "ConnectToRovioId");
		DIContainerInfrastructure.IdentityService.Initialize();
		DebugLog.Log(GetType(), "IdentityService initialized");
		RegisterBeaconIdentityEventHandlers();
		DIContainerInfrastructure.IdentityService.LoginAuto();
		return true;
	}

	private void SetAndroidActivity()
	{
		DebugLog.Log(GetType(), "SetAndroidActivity");
		if (UnityEngine.Application.isEditor)
		{
			return;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("de.chimeraentertainment.unity.UnityPlayerActivity"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				androidJavaObject.Call("AddAdditionalBehaviour", "skynest");
				DebugLog.Log(GetType(), "Setting Activity: Calling setActivity in com.rovio.rcs.Application");
				AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.rovio.rcs.Application");
				androidJavaClass2.CallStatic("setActivity", androidJavaObject);
			}
		}
	}

	private void RegisterBeaconIdentityEventHandlers()
	{
		DeRegisterBeaconIdentityEventHandlers();
		DebugLog.Log(GetType(), "RegisterBeaconIdentityEventHandlers");
		DIContainerInfrastructure.IdentityService.OnLoggedIn += BeaconIdentityOnLoggedInOrRegistered;
		DIContainerInfrastructure.IdentityService.OnLoginError += BeaconIdentityOnLoginOrRegistrationError;
	}

	private void DeRegisterBeaconIdentityEventHandlers()
	{
		DebugLog.Log(GetType(), "DeRegisterBeaconIdentityEventHandlers");
		DIContainerInfrastructure.IdentityService.OnLoggedIn -= BeaconIdentityOnLoggedInOrRegistered;
		DIContainerInfrastructure.IdentityService.OnLoginError -= BeaconIdentityOnLoginOrRegistrationError;
	}

	private void BeaconIdentityOnLoginOrRegistrationError(int errorCode, string errorMessage)
	{
		IsBeaconLoggedIn = false;
		if (DIContainerInfrastructure.GetCurrentPlayer() != null)
		{
			if (errorCode == 1)
			{
				if (RegistrationStarted)
				{
					DIContainerInfrastructure.GetAsynchStatusService().ShowError(DIContainerInfrastructure.GetLocaService().Tr("toast_registration_cancelled", "Registration cancelled"), null);
				}
				else
				{
					DIContainerInfrastructure.GetAsynchStatusService().ShowError(DIContainerInfrastructure.GetLocaService().Tr("social_rovio_login_cancelled", "Login cancelled"), null);
				}
			}
			else
			{
				if (RegistrationStarted)
				{
					DIContainerInfrastructure.GetAsynchStatusService().ShowError(DIContainerInfrastructure.GetLocaService().Tr("toast_registration_failed", "Registration failed"), null);
				}
				else
				{
					DIContainerInfrastructure.GetAsynchStatusService().ShowError(DIContainerInfrastructure.GetLocaService().Tr("social_rovio_login_failed", "Login failed"), null);
				}
				DebugLog.Error(GetType(), "BeaconIdentityOnLoginOrRegistrationError: errorCode: " + errorCode + ", errorMessage: " + errorMessage);
			}
		}
		if (RegistrationStarted)
		{
			RegistrationStarted = false;
		}
	}

	private void BeaconIdentityOnLoggedInOrRegistered()
	{
		DIContainerInfrastructure.IdentityService.OnLoggedIn -= BeaconIdentityOnLoggedInOrRegistered;
		DebugLog.Log(GetType(), "BeaconIdentityOnLoggedInOrRegistered");
		IsBeaconLoggedIn = true;
		ShowLoggedInToaster();
		if (RegistrationStarted)
		{
			RegistrationStarted = false;
		}
		CachedSocialEnvironmentGameData.IdLoginEmail = GetAccountEmail();
	}

	public void ShowLoggedInToaster()
	{
		string text = DIContainerInfrastructure.IdentityService.UserCredentials.Email;
		DebugLog.Log(GetType(), "ShowLoggedInToaster - Account name = " + text);
		if (IsGuestAccount())
		{
			text = DIContainerInfrastructure.GetLocaService().Tr("gen_guest", "Guest");
		}
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_skynestloggedin", new Dictionary<string, string> { { "{value_1}", text } }), null, DispatchMessage.Status.Info);
	}

	public bool IsGuestAccount()
	{
		return DIContainerInfrastructure.IdentityService.IsGuest();
	}

	public string GetAccountEmail()
	{
		if (DIContainerInfrastructure.IdentityService.UserCredentials != null)
		{
			return DIContainerInfrastructure.IdentityService.UserCredentials.Email;
		}
		return string.Empty;
	}
}

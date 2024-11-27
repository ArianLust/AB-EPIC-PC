using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ABH.GameDatas;
using ABH.Shared.Models;
using Chimera.Library.Components.Interfaces;
using Chimera.Library.Components.Models;
using Prime31;
using Rcs;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContentLoader : MonoBehaviourContainerBase
{
	public class FakeProgress
	{
		private bool m_finished;

		private readonly float m_timeMax;

		private float m_timeRemaining;

		private readonly Action<float> m_changeProgressBar;

		private float lastLog;

		public FakeProgress(float timeMax, Action<float> changeProgressBar)
		{
			m_timeMax = timeMax;
			m_timeRemaining = m_timeMax;
			m_changeProgressBar = changeProgressBar;
			m_finished = false;
		}

		public void Finish()
		{
			m_finished = true;
		}

		public IEnumerator Start()
		{
			yield return null;
			while (!m_finished)
			{
				Update(Time.deltaTime);
				yield return null;
			}
			m_changeProgressBar(1f);
		}

		public void Update(float timeDelta)
		{
			m_timeRemaining -= timeDelta;
			float num = Math.Min(1f, Math.Max(0f, m_timeMax - m_timeRemaining)) / m_timeMax;
			m_changeProgressBar(num);
			lastLog += timeDelta;
			if (lastLog > 0.2f)
			{
				lastLog = 0f;
				Debug.Log("[ContentLoader] FAKE PROGRESS: " + num);
			}
		}
	}
	
	private const long SpaceNeededForBalancingDataInMB = 2L;

	private const string NotEnoughSpaceMessage = "Not enough free space.";

	private const float DownloadSizeFactorForAssetbundles = 2.5f;

	private const int MaxRetriesDownloadAssets = 10;

	[SerializeField]
	private const float m_SplashScreenShowTime = 2f;

	[SerializeField]
	private List<TextAsset> m_startUpLocaLinks;

	private int m_allAssetsDownloadRetryCount;

	private bool m_lastConnectionCheckFailed;

	private INetworkStatusService m_networkStatusService;

	public OnScreenDebugLog m_OnScreenDebugLog;

	public AudioSource InitialAudioSource;

	[SerializeField]
	private UIInputTrigger m_verificationLogoutInputTrigger;

	[SerializeField]
	private GameObject m_verificationRoot;

	[SerializeField]
	private UILabel m_verificationHeader;

	[SerializeField]
	private UILabel m_verificationDesc;

	[SerializeField]
	private UILabel m_verificationButtonText;

	[SerializeField]
	private UIInputTrigger m_reconnectInputTrigger;

	[SerializeField]
	private GameObject m_reconnectButton;

	[SerializeField]
	public GameObject m_contentLoaderUI;

	[SerializeField]
	private GameObject m_splashScreens;

	[SerializeField]
	private GameObject m_PopupRoot;

	[SerializeField]
	private GameObject m_ProgressRoot;

	private int m_SlowProgresses;

	[SerializeField]
	private GameObject m_HeaderRoot;

	[SerializeField]
	private GameObject m_ChimeraLogo;

	[SerializeField]
	private GameObject m_ErrorRoot;

	[SerializeField]
	private UILabel m_ErrorText;

	[SerializeField]
	private float m_alwaysOnCheckFrequencyContentLoader = 20f;

	[SerializeField]
	private float m_alwaysOnCheckFrequencyGame = 30f;

	private bool m_StoreVersionFileLoaded;

	private bool m_balancingDataLoaded;

	private float m_alwaysOnCheckFrequency;

	public bool LastAccountWasNew;

	public BeaconConnectionMgr m_BeaconConnectionMgr;

	public AsynchStatusService m_AsynchStatusService;

	[SerializeField]
	private UISprite m_downloadProgressBar;

	[SerializeField]
	private UILabel m_downloadProgressLabel;

	[SerializeField]
	private UILabel m_downloadTextUiLabel;

	[SerializeField]
	private UIPanel m_loadingProgressPanel;

	[SerializeField]
	public UIInputTrigger m_Collider;

	[Header("Initial Popup")]
	[SerializeField]
	private Animation m_InitialConfirmPopup;

	[SerializeField]
	private UIInputTrigger m_InitialConfirmButton;

	[SerializeField]
	private GameObject m_InitialPopupObject;

	private AndroidExpansionFileMgr m_androidExpansionFileMgr;

	private float m_startLoadTime;

	private string m_lastCachedDownloadText;

	private bool m_localChecked;

	private Action m_continueAfterVersionCheckAction;

	private readonly Dictionary<string, string> m_simplifications = new Dictionary<string, string>
	{
		{ "assetbundle", "container" },
		{ "audioassetprovider", "audio" },
		{ "serializedbalacingdataprovider", "balancing" },
		{ "bytes", "data" },
		{
			"dev",
			string.Empty
		},
		{
			"qa",
			string.Empty
		},
		{
			"gd",
			string.Empty
		},
		{
			"live",
			string.Empty
		}
	};

	private bool m_GotoRootTriggered;

	public static bool m_loginRetryAllowed;

	private bool WaitUntilLoggedIn;

	private bool m_IsWaitingForLoginRetry;

	private static Action m_loginCallback;

	private static Action<int, string> m_loginErrorCallback;

	private static int m_cachedLoginErrorCode;

	private static string m_cachedLoginErrorMessage;

	public static ContentLoader Instance { get; protected set; }

	public bool LoadedFonts { get; private set; }

	public bool IsDone { get; private set; }

	public static bool UseObb
	{
		get
		{
			//string url = "jar:file://" + UnityEngine.Application.dataPath + "!/assets/bin/Data/settings.xml";
			//string text = string.Empty;
			//try
			//{
			//	WWW wWW = new WWW(url);
			//	while (!wWW.isDone)
			//	{
			//	}
			//	text = wWW.text;
			//	if (!string.IsNullOrEmpty(wWW.error))
			//	{
			//		DebugLog.Error("[ContentLoader] " + wWW.error);
			//	}
			//}
			//catch (Exception ex)
			//{
			//	DebugLog.Error("[ContentLoader] " + ex);
			//}
			//string value = "<bool name=\"useObb\">True</bool>".Replace(" ", string.Empty).ToLower();
			//return text.Trim().Replace(" ", string.Empty).ToLower()
			//	.IndexOf(value, StringComparison.Ordinal) > 0;

			return false;
		}
	}

	[method: MethodImpl(32)]
	public event Action<bool> OnInternetConnectivityReceived;

	private void OnApplicationQuit()
	{
		if (UseObb && m_androidExpansionFileMgr != null)
		{
			m_androidExpansionFileMgr.OnApplicationQuit();
		}
	}

	private void OnDestroy()
	{
		if (UseObb && m_androidExpansionFileMgr != null)
		{
			m_androidExpansionFileMgr.Destroy();
		}
	}

	private IEnumerator Start()
	{
#if UNITY_ANDROID || UNITY_IOS
			UnityEngine.Application.targetFrameRate = 60;
#endif
		DebugLog.Log(GetType(), "Start...");
		m_downloadProgressLabel.gameObject.SetActive(false);
		m_alwaysOnCheckFrequency = m_alwaysOnCheckFrequencyContentLoader;
		ABHLocaService abhLocaService = DIContainerInfrastructure.GetStartupLocaService().InjectLocaAsset(m_startUpLocaLinks);
		ClientInfo.AddLoadingTracking("02_FirstStart");
		abhLocaService.InitDefaultLoca();
		SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_loading_checkconnection", "Checking internet connection..."));
		StartCoroutine(LeaveChimeraLogo(m_ChimeraLogo.PlayAnimationOrAnimatorState("SplashScreen_Chimera")));
		yield return StartCoroutine(RequestStoragePermission());
		if (UseObb)
		{
			ContinueWithOBBCheck(StartWithDeviceCompatibilityCheck);
		}
		else
		{
			StartWithDeviceCompatibilityCheck();
		}
		yield return new WaitForEndOfFrame();
		DebugLog.Log(GetType(), "... Start done.");
	}

	private IEnumerator LeaveChimeraLogo(float time)
	{
		yield return new WaitForSeconds(time);
		UnityEngine.Object.Destroy(m_ChimeraLogo.gameObject);
		m_HeaderRoot.GetComponent<Animation>().Play();
	}

	private void OnApplicationPause(bool paused)
	{
		if (m_androidExpansionFileMgr != null)
		{
			m_androidExpansionFileMgr.OnApplicationPause(paused);
		}
	}

	private void StartWithDeviceCompatibilityCheck()
	{
		DebugLog.Log("[ContentLoader] StartWithDeviceCompatibilityCheck");
		ClientInfo.AddLoadingTracking("03_DeviceCompatibility");
		if (!DIContainerInfrastructure.GetCompatibilityService().isCompatible())
		{
			HideDownloadProgress();
			SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_error_notsupported", "Your current Device is not supported!"), true);
		}
		else
		{
			PrepareInternetConnectivityCheck(StartWithInternetConnection);
		}
	}

	private IEnumerator RequestStoragePermission()
	{
		bool canContinue = true;
		if (GetAPIVersion() >= 23 && (EtceteraAndroid.shouldShowRequestPermissionRationale("android.permission.WRITE_EXTERNAL_STORAGE") || PlayerPrefs.GetInt("PermissionRequested", 0) == 0) && !EtceteraAndroid.checkSelfPermission("android.permission.WRITE_EXTERNAL_STORAGE"))
		{
			canContinue = false;
			ShowConfirmationPopup(delegate
			{
				string[] permissions = new string[2] { "android.permission.WRITE_EXTERNAL_STORAGE", "android.permission.READ_EXTERNAL_STORAGE" };
				EtceteraAndroid.requestPermissions(permissions);
				m_InitialConfirmPopup.Play("Popup_Leave");
				Invoke("DeactivatePopup", 0.15f);
				canContinue = true;
			});
			EtceteraAndroidManager.onRequestPermissionsResultEvent += delegate(EtceteraAndroidManager.PermissionsResult permissionResult)
			{
				DebugLog.Log(GetType(), "Permission request window closed with result: " + permissionResult.permissions[0] + " " + permissionResult.grantResults[0]);
			};
			PlayerPrefs.SetInt("PermissionRequested", 1);
		}
		while (!canContinue)
		{
			yield return null;
		}
	}

	private int GetAPIVersion()
	{
		return 15;
	}

	private void DeactivatePopup()
	{
		m_InitialPopupObject.SetActive(false);
	}

	private void ShowConfirmationPopup(Action onConfirm)
	{
		m_InitialPopupObject.SetActive(true);
		m_InitialConfirmPopup.Play("Popup_Enter");
		m_InitialConfirmButton.Clicked -= onConfirm;
		m_InitialConfirmButton.Clicked += onConfirm;
	}

	private void StartWithInternetConnection()
	{
		ClientInfo.AddLoadingTracking("04_InternetConnection");
		DebugLog.Log("[ContentLoader] StartWithInternetConnection");
		AddComponentSafely(ref m_AsynchStatusService);
		AddComponentSafely(ref m_BeaconConnectionMgr);
		if ((bool)m_BeaconConnectionMgr)
		{
			m_BeaconConnectionMgr.Init(this);
		}
		else
		{
			DebugLog.Error("[ContentLoader] Could not initialize BeaconConnectionMgr !!");
		}
		ContinueWithSkynestLogin();
		m_startLoadTime = Time.realtimeSinceStartup;
	}

	private void PrepareInternetConnectivityCheck(Action continueAfterPreparation)
	{
		this.OnInternetConnectivityReceived = (Action<bool>)Delegate.Combine(this.OnInternetConnectivityReceived, new Action<bool>(HandleInternetConnectivityStatusForContentLoader));
		DebugLog.Log("[ContentLoader] Starting HasNetworkConnectionCoroutine");
		StartCoroutine(DoInitialInternetConnectivityCheckAndGetServerTime(continueAfterPreparation));
		CancelInvoke("HasNetworkConnectionCoroutine");
		InvokeRepeating("HasNetworkConnectionCoroutine", m_alwaysOnCheckFrequency, m_alwaysOnCheckFrequency);
	}

	private IEnumerator DoInitialInternetConnectivityCheckAndGetServerTime(Action continueAfterPreparation)
	{
		DebugLog.Log("[ContentLoader] InitialInternetConnectivityCheck");
		m_networkStatusService = DIContainerInfrastructure.GetNetworkStatusService();
		m_networkStatusService.InternetAvailabilityResponseReceived += OnInternetAvailabilityResponseReceived;
		Action continueAfterPreparation2 = continueAfterPreparation;
		yield return StartCoroutine(m_networkStatusService.CheckInternetAvailability(null, delegate(WebRequest request, bool available)
		{
			if (available && continueAfterPreparation2 != null)
			{
				continueAfterPreparation2();
			}
			else if (this.OnInternetConnectivityReceived != null)
			{
				this.OnInternetConnectivityReceived(false);
			}
		}));
	}

	private void OnInternetAvailabilityResponseReceived(string jsonTime)
	{
		m_networkStatusService.InternetAvailabilityResponseReceived -= OnInternetAvailabilityResponseReceived;
		int num = new ABHTimingHandler().ProcessTimeFromSkynestTimeService(jsonTime);
		if (num != 0)
		{
			Debug.Log("[ContentLoader] Timing: setting timingservice servertime");
			DIContainerLogic.GetTimingService().SetTimeFromServer(num);
			Debug.Log("[ContentLoader] Timing: setting ServerOnlyTimingService servertime");
			DIContainerLogic.GetServerOnlyTimingService().SetTimeFromServer(num);
			Debug.Log("[ContentLoader] Timing: Get initial Server time: " + DIContainerLogic.GetServerOnlyTimingService().GetPresentTime());
		}
	}

	private void HandleInternetConnectivityStatusForContentLoader(bool internetAvailable)
	{
		if (!internetAvailable)
		{
			if ((bool)m_reconnectButton)
			{
				m_reconnectButton.SetActive(true);
				UILabel componentInChildren = m_reconnectButton.GetComponentInChildren<UILabel>();
				if ((bool)componentInChildren)
				{
					componentInChildren.text = DIContainerInfrastructure.GetStartupLocaService().Tr("startup_button_reconnect", "Reconnect");
				}
			}
			m_lastCachedDownloadText = m_downloadTextUiLabel.text;
			m_downloadProgressLabel.gameObject.SetActive(false);
			SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_error_noconnection", "Failed to connect to the Server! Please activate internet and press reconnect"), true);
			m_lastConnectionCheckFailed = true;
		}
		else
		{
			if (m_IsWaitingForLoginRetry)
			{
				return;
			}
			if ((bool)m_reconnectButton)
			{
				m_reconnectButton.SetActive(false);
			}
			if (m_lastCachedDownloadText != null)
			{
				SetDownloadProgressText(m_lastCachedDownloadText);
			}
			m_lastCachedDownloadText = null;
			if (m_lastConnectionCheckFailed)
			{
				if (m_BeaconConnectionMgr == null || !m_BeaconConnectionMgr.IsBeaconLoggedIn)
				{
					StartWithInternetConnection();
				}
				else
				{
					Hatch2_LoadAllAssets();
				}
				m_lastConnectionCheckFailed = false;
			}
		}
	}

	private void ContinueWithOBBCheck(Action doWhenObbLoadingFinished)
	{
		DebugLog.Log("[ContentLoader] ContinueWithOBBCheck");
		SetDownloadProgressText("Checking Expansion File");
		m_androidExpansionFileMgr = DIContainerInfrastructure.GetAndroidExpansionFileMgr().Init(base.gameObject.name, "ReportExpansionFileDownloadProgressFromJava", "ReportExpansionFileDownloadErrorFromJava", "ReportExpansionFileDownloadStatusChangedFromJava", "de.chimeraentertainment.unity.UnityPlayerProxyActivity");
		AndroidExpansionFileMgr androidExpansionFileMgr = m_androidExpansionFileMgr;
		androidExpansionFileMgr.OnDownloadError = (Action<string>)Delegate.Combine(androidExpansionFileMgr.OnDownloadError, new Action<string>(ReportExpansionFileDownloadErrorFromJava));
		AndroidExpansionFileMgr androidExpansionFileMgr2 = m_androidExpansionFileMgr;
		androidExpansionFileMgr2.OnDownloadFinished = (Action)Delegate.Combine(androidExpansionFileMgr2.OnDownloadFinished, (Action)delegate
		{
			SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_loading_expansion_finished", "Expansion File Downloaded"));
			if (doWhenObbLoadingFinished != null)
			{
				doWhenObbLoadingFinished();
			}
		});
		DebugLog.Log("[ContentLoader] Now checking for OBB.");
		m_androidExpansionFileMgr.CheckOBB(this);
	}

	private IEnumerator ContinueWithFakeEditorLoading()
	{
		DebugLog.Log("[ContentLoader] ContinueWithFakeEditorLoading");
		SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_loading_fake", "Loading Data..."));
		yield return null;
		LoadedFonts = true;
		InitBalancingThenContinue(GotoRootScene);
	}

	private void ContinueWithSkynestLogin()
	{
		DebugLog.Log("[ContentLoader] ContinueWithSkynestLogin");
		SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_loading_login", "Logging in..."));
		SetDownloadProgress(0.15f);
		DIContainerInfrastructure.IdentityService.OnLoggedIn -= ZeroTheGuestProfile;
		if (DIContainerInfrastructure.GetAssetData().ZeroTheGuestProfileOnNextLogin)
		{
			DIContainerInfrastructure.IdentityService.OnLoggedIn += ZeroTheGuestProfile;
		}
		DIContainerInfrastructure.IdentityService.OnLoggedIn += SkynestIdentityOnLoggedIn;
		DIContainerInfrastructure.IdentityService.OnLoginError += SkynestIdentityOnLoginError;
		if (m_loginRetryAllowed)
		{
			Debug.Log("Login is not retrying yet");
			m_loginRetryAllowed = false;
		}
		m_BeaconConnectionMgr.ConnectToRovioId();
	}

	private void SkynestIdentityOnLoginError(int errorCode, string errorMessage)
	{
		DebugLog.Error("[ContentLoader] Skynest Login failed with error: " + errorCode + " " + errorMessage);
		// AndroidTools.EnableImmersiveMode();
		if (errorCode == 3 || errorCode == 2)
		{
			if (m_IsWaitingForLoginRetry)
			{
				m_IsWaitingForLoginRetry = false;
				if ((bool)m_reconnectButton)
				{
					m_reconnectButton.SetActive(false);
				}
				if (m_lastCachedDownloadText != null)
				{
					SetDownloadProgressText(m_lastCachedDownloadText);
					m_lastCachedDownloadText = null;
				}
				m_lastCachedDownloadText = null;
				m_loginRetryAllowed = false;
			}
			if (errorCode == 3)
			{
				if (!string.IsNullOrEmpty(m_BeaconConnectionMgr.Identiy.GetAccessToken()))
				{
					SkynestIdentityOnLoggedIn();
					return;
				}
				if (!string.IsNullOrEmpty(m_BeaconConnectionMgr.Identiy.GetRefreshToken()))
				{
					StartCoroutine(SubmitErrorReport("This account have not been confirmed.\r\nRefresh token: " + m_BeaconConnectionMgr.Identiy.GetRefreshToken()));
				}
				if (m_loginRetryAllowed)
				{
					m_loginRetryAllowed = false;
				}
			}
			DebugLog.Log("[ContentLoader] Show Verification Dialog!");
			ShowVerificationDialog("startup_verify");
		}
		else
		{
			if (!(CoreStateMgr.Instance == null) && CoreStateMgr.Instance.m_isInitialized)
			{
				return;
			}
			m_loginErrorCallback = SkynestIdentityOnLoginError;
			m_cachedLoginErrorCode = errorCode;
			m_cachedLoginErrorMessage = errorMessage;
			if (!m_loginRetryAllowed)
			{
				m_loginRetryAllowed = true;
				if (!m_IsWaitingForLoginRetry && m_BeaconConnectionMgr.ConnectToRovioId())
				{
					return;
				}
			}
			if (!string.IsNullOrEmpty(errorMessage))
			{
				StartCoroutine(Instance.SubmitErrorReport(string.Concat((Identity.ErrorCode)errorCode, ": ", errorMessage)));
			}
			if ((bool)m_reconnectButton)
			{
				m_reconnectButton.SetActive(true);
				UILabel componentInChildren = m_reconnectButton.GetComponentInChildren<UILabel>();
				if ((bool)componentInChildren)
				{
					componentInChildren.text = DIContainerInfrastructure.GetStartupLocaService().Tr("startup_button_reconnect", "Reconnect");
				}
			}
			m_lastCachedDownloadText = m_downloadTextUiLabel.text;
			m_downloadProgressLabel.gameObject.SetActive(false);
			Instance.SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_error_login", "Failed to login please try again!"), true);
			m_IsWaitingForLoginRetry = true;
		}
	}

	private void ShowVerificationDialog(string baseLocaIdent)
	{
		if ((bool)m_ErrorRoot)
		{
			m_ErrorRoot.SetActive(false);
		}
		m_verificationRoot.SetActive(true);
		m_verificationHeader.text = DIContainerInfrastructure.GetStartupLocaService().Tr(baseLocaIdent + "_header", "Your account verification is pending!");
		m_verificationDesc.text = DIContainerInfrastructure.GetStartupLocaService().Tr(baseLocaIdent + "_desc", "You have to verify your account first to use it in the game. Please end the app and verify your account or log out from your account and play without. ATTENTION: The progress you made with this account is lost!");
		m_verificationButtonText.text = DIContainerInfrastructure.GetStartupLocaService().Tr(baseLocaIdent + "_btn", "Log out");
		m_verificationLogoutInputTrigger.Clicked -= VerificationLogoutInputTriggerClicked;
		m_verificationLogoutInputTrigger.Clicked += VerificationLogoutInputTriggerClicked;
	}

	private void VerificationLogoutInputTriggerClicked()
	{
		m_verificationLogoutInputTrigger.Clicked -= VerificationLogoutInputTriggerClicked;
		m_verificationRoot.SetActive(false);
		DIContainerInfrastructure.IdentityService.Logout();
		DebugLog.Log("[ContentLoader] Verification Dialog Logout done!");
		DIContainerInfrastructure.GetAssetData().FetchAndResetProfileOnNextLogin = true;
		DIContainerInfrastructure.GetAssetData().Save();
		DebugLog.Log("[ContentLoader] Verification Fetch Profile set!");
		m_BeaconConnectionMgr.Identiy.Login(Identity.LoginMethod.LoginGuest, SkynestIdentityOnLoggedIn, SkynestIdentityOnLoginError);
	}

	private void ZeroTheGuestProfile()
	{
		DIContainerInfrastructure.IdentityService.OnLoggedIn -= ZeroTheGuestProfile;
		if (!DIContainerInfrastructure.IdentityService.IsGuest())
		{
			DebugLog.Log("[ContentLoader] Skipping zeroing the guest account profile, because current profile isn't a guest profile.");
			return;
		}
		DIContainerInfrastructure.GetProfileMgr().RemoveProfile();
		AssetData assetData = DIContainerInfrastructure.GetAssetData();
		assetData.ZeroTheGuestProfileOnNextLogin = false;
		assetData.Save();
		DebugLog.Log("[ContentLoader] Completely zeroed the guest account profile.");
	}

    //private void SkynestIdentityOnLoggedIn()
    //{
    //	DebugLog.Log("[ContentLoader] SkynestIdentityOnLoggedIn");
    //	//AndroidTools.EnableImmersiveMode();
    //	ClientInfo.AddLoadingTracking("05_SkynestLogin");
    //	DIContainerInfrastructure.IdentityService.OnLoggedIn -= SkynestIdentityOnLoggedIn;
    //	if (m_loginCallback != null)
    //	{
    //		m_loginCallback = null;
    //	}
    //	if (m_loginErrorCallback != null)
    //	{
    //		m_loginErrorCallback = null;
    //	}
    //	if (m_cachedLoginErrorCode != 0)
    //	{
    //		m_cachedLoginErrorCode = 0;
    //	}
    //	if (m_cachedLoginErrorMessage != null)
    //	{
    //		m_cachedLoginErrorMessage = null;
    //	}
    //	if ((bool)m_verificationRoot)
    //	{
    //		UnityEngine.Object.Destroy(m_verificationRoot);
    //	}
    //	bool flag = DIContainerInfrastructure.GetAssetData() != null && DIContainerInfrastructure.GetAssetData().FetchAndResetProfileOnNextLogin;
    //	DebugLog.Log(GetType(), "Fetch Profile from Server? ->" + flag);
    //	if (flag)
    //	{
    //		try
    //		{
    //			FetchProfileThenWithStoreVersionsLoading();
    //			return;
    //		}
    //		catch
    //		{
    //			Hatch2_InitializeAssetLoading();
    //			return;
    //		}
    //	}
    //	Hatch2_InitializeAssetLoading();
    //}

    private void SkynestIdentityOnLoggedIn()
    {
        DebugLog.Log("[ContentLoader] SkynestIdentityOnLoggedIn");
        //AndroidTools.EnableImmersiveMode();
        DIContainerInfrastructure.IdentityService.OnLoggedIn -= SkynestIdentityOnLoggedIn;
        if ((bool)m_verificationRoot)
        {
            UnityEngine.Object.Destroy(m_verificationRoot);
        }
        if (DIContainerInfrastructure.GetAssetData().FetchAndResetProfileOnNextLogin)
        {
            FetchProfileThenWithStoreVersionsLoading();
        }
        else
        {
            ContinueWithBalancingDataLoading();
        }
    }

    private void LoadFontsIfNeccesarry()
	{
		string languageAssetProviderName = GetLanguageAssetProviderName();
		if (string.IsNullOrEmpty(languageAssetProviderName))
		{
			DoVersionCheckThenContinue();
			return;
		}
		string text = DIContainerInfrastructure.GetTargetBuildGroup() + "_" + languageAssetProviderName.ToLower() + ".assetbundle";
		if (!DIContainerInfrastructure.GetAssetsService().NeedToDownloadAsset(text))
		{
			LoadedFonts = true;
			DoVersionCheckThenContinue();
			return;
		}
		DIContainerInfrastructure.GetAssetsService().Load(text, delegate(string result)
		{
			DebugLog.Log(GetType(), "LoadFontsIfNecessary: Success!");
			m_SlowProgresses = 0;
			if (result != null)
			{
				DebugLog.Log("[ContentLoader] LoadFontsIfNeccesarry: Succesfull loaded font data");
				LoadedFonts = true;
				DoVersionCheckThenContinue();
			}
			else
			{
				DebugLog.Error("[ContentLoader] LoadFontsIfNeccesarry: Error loading font data");
				DoVersionCheckThenContinue();
			}
		}, delegate
		{
		}, delegate
		{
		});
	}

	private string GetLanguageAssetProviderName()
	{
		FontInitializer component = GetComponent<FontInitializer>();
		if (!component)
		{
			return string.Empty;
		}
		foreach (FontSet fontSet in component.m_FontSets)
		{
			if (!(fontSet.m_Language == DIContainerInfrastructure.GetStartupLocaService().CurrentLanguageKey))
			{
				continue;
			}
			foreach (FontPair fontPair in fontSet.m_FontPairs)
			{
				if ((bool)fontPair.m_FontReplacementAssetProvider)
				{
					return fontPair.m_FontReplacementAssetProvider.name;
				}
			}
		}
		return string.Empty;
	}

	private void FetchProfileThenWithStoreVersionsLoading()
	{
		DebugLog.Log("[ContentLoader] Fetch Profile instantly after login");
		// DIContainerInfrastructure.RemoteStorageService.GetPrivateProfile(OnProfileFetched, OnProfileFetchedError);
		// called when logging out from rovio acc, softlock here because GetPrivateProfile in StorageServiceOfflineImpl is literally empty

		DIContainerInfrastructure.GetAssetData().FetchAndResetProfileOnNextLogin = false;
		DIContainerInfrastructure.GetAssetData().Save();
		
		ContinueWithBalancingDataLoading();
		// jump straight to balancing loading, because we cant fetch profile
	}

	private void OnProfileFetchedError(string error)
	{
		DebugLog.Error("[ContentLoader] Fetched Profile error: " + error);
		if (error == "Deserialization Error")
		{
			DebugLog.Error("[ContentLoader] calling StartOpeningStoreForNewVersion()");
			ShowVerificationDialog("startup_newer_profile");
		}
	}

    //private void OnProfileFetched(ABH.Shared.Models.PlayerData playerData)
    //{
    //	DebugLog.Log(GetType(), "OnProfileFetched: Fetched Profile instantly after login and save");
    //	if (playerData != null)
    //	{
    //		DebugLog.Log(GetType(), "PlayerData != null => Save Profile...");
    //		DIContainerInfrastructure.GetProfileMgr().SaveProfile(playerData);
    //	}
    //	DIContainerInfrastructure.GetAssetData().FetchAndResetProfileOnNextLogin = false;
    //	DIContainerInfrastructure.GetAssetData().Save();
    //	Hatch2_InitializeAssetLoading();
    //}

    private void OnProfileFetched(ABH.Shared.Models.PlayerData playerData)
    {
        DebugLog.Log("[ContentLoader] Fetched Profile instantly after login and save");
        if (playerData != null)
        {
            DIContainerInfrastructure.GetProfileMgr().SaveProfile(playerData);
        }
        DIContainerInfrastructure.GetAssetData().FetchAndResetProfileOnNextLogin = false;
        DIContainerInfrastructure.GetAssetData().Save();
        ContinueWithBalancingDataLoading();
    }

    private void Hatch2_InitializeAssetLoading()
	{
		DebugLog.Log(GetType(), "Hatch2_InitializeAssetLoading");
		StartCoroutine(Hatch2_ContinueWithAssetLoading());
	}

	private IEnumerator Hatch2_ContinueWithAssetLoading()
	{
		if (m_IsWaitingForLoginRetry)
		{
			m_IsWaitingForLoginRetry = false;
			if ((bool)m_reconnectButton)
			{
				m_reconnectButton.SetActive(false);
			}
			if (m_lastCachedDownloadText != null)
			{
				SetDownloadProgressText(m_lastCachedDownloadText);
				m_lastCachedDownloadText = null;
			}
			m_lastCachedDownloadText = null;
		}
		DebugLog.Log(GetType(), "Hatch2_ContinueWithAssetLoading: Getting StoreVersion File to check for forced update");
		SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_loading_balancing", "Loading Balancing..."));
		m_balancingDataLoaded = false;
		DownloadSerializedBalancingDataContainer();
		while (!m_balancingDataLoaded)
		{
			yield return new WaitForEndOfFrame();
		}
		SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_syncing_version", "Synchronizing Version..."));
		m_StoreVersionFileLoaded = false;
		DIContainerInfrastructure.GetClientUpdateService().Hatch2_CheckForNewVersionAvailable(Hatch2_ContinueAfterVersionCheck);
		while (!m_StoreVersionFileLoaded)
		{
			yield return new WaitForEndOfFrame();
		}
		DebugLog.Log(GetType(), "Hatch2_ContinueWithAssetLoading: StoreVersion check completed => download remaining assets");
		SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_loading_assets", "Downloading assets..."));
		Hatch2_LoadAllAssets();
	}

	private void Hatch2_OnBalancingDataLoaded(string assetPath)
	{
		DebugLog.Log(GetType(), "Hatch2_OnBalancingDataLoaded: balancing received: " + assetPath);
		if (string.IsNullOrEmpty(assetPath))
		{
			DebugLog.Error(GetType(), "Hatch2_OnBalancingDataLoaded: Asset download failed!");
			SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_balancing_failed", "Error: Cannot load balancing data!"));
			return;
		}
		DIContainerBalancing.OnBalancingDataInitialized += delegate
		{
			m_balancingDataLoaded = true;
		};
		DIContainerBalancing.Init(BalancingInitErrorHandler);
	}

	private void BalancingInitErrorHandler(BalancingInitErrorCode errorCode)
	{
		if (errorCode == BalancingInitErrorCode.FILE_NOT_FOUND)
		{
			SetDownloadProgressText("Balancing File not found. Downloading...");
			DownloadSerializedBalancingDataContainer();
		}
	}

	private void DownloadSerializedBalancingDataContainer()
	{
		DIContainerInfrastructure.GetAssetsService().Load(DIContainerBalancing.BalancingDataAssetFilename, Hatch2_OnBalancingDataLoaded, SetDownloadProgress);
	}

	private void Hatch2_LoadAllAssets()
	{
		Caching.expirationDelay = 2419200;
		List<string> list = new List<string>();
		list.Add(DIContainerInfrastructure.GetTargetBuildGroup() + "_" + DIContainerInfrastructure.GetStartupLocaService().CurrentLanguageKey + ".bytes");
		list.Add(DIContainerInfrastructure.GetTargetBuildGroup() + "_shopiconatlasassetprovider.assetbundle");
		list.Add(DIContainerInfrastructure.GetTargetBuildGroup() + "_videoclipintro.mp4");
		List<string> list2 = list;
		string languageAssetProviderName = GetLanguageAssetProviderName();
		if (!string.IsNullOrEmpty(languageAssetProviderName))
		{
			list2.Add(DIContainerInfrastructure.GetTargetBuildGroup() + "_" + languageAssetProviderName.ToLower() + ".assetbundle");
		}
		StartCoroutine(CheckLocalAssets());
		DIContainerInfrastructure.GetAssetsService().Load(list2.ToArray(), Hatch2_OnAssetLoadingSuccess, Hatch2_OnAssetLoadingError, Hatch2_OnAssetLoadingProgress);
	}

    private void ContinueWithBalancingDataLoading()
    {
        DebugLog.Log("[ContentLoader] ContinueWithBalancingDataLoading");
        DIContainerInfrastructure.GetSkynestMetaDataCache().Init(delegate (bool success)
        {
            if (!success)
            {
                DebugLog.Error("[ContentLoader] Failed to initialize the Skynest metdadata cache!");
            }
            if (!DIContainerInfrastructure.GetAssetsService().NeedToDownloadAsset(DIContainerBalancing.BalancingDataAssetFilename))
            {
                DIContainerInfrastructure.GetStartupLocaService().Tr("startup_syncing_balancing", "Syncing Balancing...");
                InitBalancingThenContinue(ContinueWithLocaLoading);
            }
            else
            {
                SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_loading_balancing", "Loading Balancing..."));
                DIContainerInfrastructure.GetAssetsService().Load(DIContainerBalancing.BalancingDataAssetFilename, delegate (string result)
                {
                    m_SlowProgresses = 0;
                    if (result != null)
                    {
                        InitBalancingThenContinue(ContinueWithLocaLoading);
                    }
                    else
                    {
                        DebugLog.Error("[ContentLoader] ContinueWithBalancingDataLoading: Error loading balancing data");
                        SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_error_balancing", "Balancing data download failed. Please restart the app and try again later."), true);
                    }
                }, SetDownloadProgress, SetSlowProgress);
            }
        });
    }

    private void Hatch2_ContinueAfterVersionCheck(bool newerVersionAvailable)
	{
		DebugLog.Log(GetType(), "Hatch2_ContinueAfterVersionCheck: newer version forced? ---> " + newerVersionAvailable);
		if (!newerVersionAvailable)
		{
			StartCoroutine(StartOpeningStoreForNewVersion());
		}
		else
		{
			m_StoreVersionFileLoaded = true;
		}
	}

	private void Hatch2_OnStoreVersionFileLoadingProgress(float progress)
	{
		SetDownloadProgress(progress);
	}

	private void Hatch2_OnAssetLoadingSuccess(Dictionary<string, string> downloadedAssets)
	{
		DebugLog.Log(GetType(), "Hatch2_OnAssetLoadingSuccess. Downloaded: " + ((downloadedAssets != null) ? string.Join(",", downloadedAssets.Keys.ToArray()) : "0"));
		m_downloadProgressLabel.gameObject.SetActive(false);
		DIContainerBalancing.OnBalancingDataInitialized -= GotoRootScene;
		DIContainerBalancing.OnBalancingDataInitialized += GotoRootScene;
		DIContainerBalancing.Init(BalancingInitErrorHandler);
	}

	private void Hatch2_OnAssetLoadingError(string[] missingAssets, int errorCode)
	{
		m_allAssetsDownloadRetryCount++;
		DebugLog.Error(GetType(), "Hatch2_OnAssetLoadingError - errorCode " + errorCode + ", retry-count: " + m_allAssetsDownloadRetryCount + " missing assets: " + ((missingAssets != null) ? string.Join(", ", missingAssets) : "0"));
		switch (errorCode)
		{
		case -100:
			DebugLog.Warn(GetType(), "Hatch2_OnAssetLoadingError: assuming http timeout (message is unavailable here...) ");
			if (m_allAssetsDownloadRetryCount <= 10)
			{
				Hatch2_LoadAllAssets();
				return;
			}
			DebugLog.Warn(GetType(), "Hatch2_OnAssetLoadingError: Max retries reached! ");
			SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_loading_asseterror " + errorCode, "Error loading assets: Max Retries reached!"), true);
			break;
		case -7:
			DebugLog.Error(GetType(), "Hatch2_OnAssetLoadingError: Corrupt asset found!");
			if (m_allAssetsDownloadRetryCount <= 10)
			{
				Hatch2_LoadAllAssets();
				return;
			}
			DebugLog.Warn(GetType(), "Hatch2_OnAssetLoadingError: Max retries reached! ");
			SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_loading_asseterror " + errorCode, "Error loading assets: Max Retries reached!"), true);
			break;
		}
		SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_loading_asseterror " + errorCode, "Error loading assets: errorcode " + (Rcs.Assets.ErrorCode)errorCode), true);
	}

	private void Hatch2_OnAssetLoadingProgress(Dictionary<string, string> downloadedAssets, string[] currentlyLoadingAssets, double totalToDownload, double nowDownloaded)
	{
		if (m_localChecked)
		{
			SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_loading_assets", "Downloading assets..."));
			SetDownloadProgress((float)(nowDownloaded / totalToDownload));
			SetDownloadProgressLabel((float)nowDownloaded / 1000000f, (float)totalToDownload / 1000000f);
		}
	}

	private IEnumerator CheckLocalAssets()
	{
		SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_loading_local_assets", "Checking local assets..."));
		float delay = 0.5f;
		m_localChecked = false;
		float x = 0f;
		while (x < delay)
		{
			x += Time.deltaTime;
			SetDownloadProgress(x / delay);
			yield return new WaitForEndOfFrame();
		}
		m_localChecked = true;
	}

	private void SetSlowProgress(bool isSlow)
	{
		m_SlowProgresses = ((!isSlow) ? Mathf.Max(m_SlowProgresses - 1, 0) : (m_SlowProgresses + 1));
		if (m_SlowProgresses >= 20)
		{
			SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_error_slowconnection", "You currently have a very slow connection, maybe some Assets fail to download."), true);
		}
		else if (m_SlowProgresses <= 10)
		{
			ClearErrorProgressText();
		}
	}

	private void ClearErrorProgressText()
	{
		if ((bool)m_ErrorRoot)
		{
			m_ErrorRoot.SetActive(false);
		}
	}

	private void InitBalancingThenContinue(Action continueAction)
	{
		DebugLog.Log("[ContentLoader] InitBalancingThenContinue");
		ClientInfo.AddLoadingTracking("06_BalancingLoaded");
		m_continueAfterVersionCheckAction = continueAction;
		DIContainerBalancing.OnBalancingDataInitialized -= DoDownloadFont;
		DIContainerBalancing.OnBalancingDataInitialized += DoDownloadFont;
		DIContainerBalancing.Init();
	}

	private void DoDownloadFont()
	{
		DIContainerBalancing.OnBalancingDataInitialized -= DoDownloadFont;
		ClientInfo.AddLoadingTracking("07_BalancingInitialized");
		LoadFontsIfNeccesarry();
	}

	private void DoVersionCheckThenContinue()
	{
		DebugLog.Log("[ContentLoader] DoVersionCheckThenContinue");
		DIContainerBalancing.OnBalancingDataInitialized -= DoVersionCheckThenContinue;
		StartCoroutine(DoVersionCheckThenContinueCoroutine(m_continueAfterVersionCheckAction));
	}

	private IEnumerator DoVersionCheckThenContinueCoroutine(Action continueAction)
	{
		SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_syncing_version", "Synchronizing Version..."));
		bool downloadHappening = false;
		Action continueAction2 = continueAction;
		yield return StartCoroutine(DIContainerInfrastructure.GetClientUpdateService().CheckForNewVersionAvailable(this, delegate
		{
			SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_loading_version", "Retrieving latest Version..."));
			downloadHappening = true;
		}, delegate(bool isNew)
		{
			if (downloadHappening)
			{
				m_SlowProgresses = 0;
			}
			if (isNew)
			{
				StartCoroutine(StartOpeningStoreForNewVersion());
			}
			else if (continueAction2 != null)
			{
				continueAction2();
			}
		}, SetDownloadProgress, SetSlowProgress));
	}

	private IEnumerator StartOpeningStoreForNewVersion()
	{
		while (true)
		{
			SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_error_newerversion", "There is a new Version available in the store. Please update to continue!"), true);
			yield return new WaitForSeconds(4.5f);
			SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_loading_newerversion", "Opening Store.."));
			float timeTillOpenStore = 1.5f;
			float startTime = Time.realtimeSinceStartup;
			while (timeTillOpenStore > 0f)
			{
				SetDownloadProgress(1f - timeTillOpenStore / 1.5f);
				timeTillOpenStore -= Time.realtimeSinceStartup - startTime;
				startTime = Time.realtimeSinceStartup;
				yield return new WaitForEndOfFrame();
			}
			DIContainerInfrastructure.GetClientUpdateService().OpenAppropriateAppStoreForNewVersion();
			yield return new WaitForSeconds(1f);
			SetDownloadProgress(0f);
		}
	}

	private void ContinueWithLocaLoading()
	{
		DebugLog.Log("[ContentLoader] ContinueWithLocaLoading");
		ClientInfo.AddLoadingTracking("08_VersionChecked");
		DIContainerBalancing.OnBalancingDataInitialized -= ContinueWithLocaLoading;
		string text = DIContainerInfrastructure.GetTargetBuildGroup() + "_" + DIContainerInfrastructure.GetStartupLocaService().CurrentLanguageKey + ".bytes";
		if (!DIContainerInfrastructure.GetAssetsService().NeedToDownloadAsset(text))
		{
			DIContainerInfrastructure.GetStartupLocaService().Tr("startup_syncing_loca", "Syncing Loca...");
			ContinueWithAssetLoading();
			return;
		}
		SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_loading_loca", "Loading Localization..."));
		DIContainerInfrastructure.GetAssetsService().Load(text, delegate(string result)
		{
			DebugLog.Log(GetType(), "ContinueWithLocaLoading: success callback");
			m_SlowProgresses = 0;
			if (result != null)
			{
				ContinueWithAssetLoading();
			}
			else
			{
				DebugLog.Error("[ContentLoader] ContinueWithLocaLoading: Error loading loca data");
				SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_error_loca", "Loca download failed. Please restart the app and try again later."), true);
			}
		}, SetDownloadProgress, SetSlowProgress);
	}

	private void ContinueWithBattlegroundAssetLoading(AssetbundleIndex bundleIndex = AssetbundleIndex.A, int retryCount = 0)
	{
		DebugLog.Log(string.Concat("[ContentLoader] ContinueWithBattlegroundAssetLoading, Index = ", bundleIndex, " Try #", retryCount));
		ClientInfo.AddLoadingTracking("09_LocaDownloaded");
		string text = DIContainerInfrastructure.GetTargetBuildGroup() + "_battlegroundassetprovider" + bundleIndex.ToString().ToLower();
		if (!DIContainerInfrastructure.GetAssetsService().NeedToDownloadAsset(text))
		{
			DIContainerInfrastructure.GetStartupLocaService().Tr("startup_syncing_battlegroundassetprovider" + bundleIndex.ToString().ToLower(), "Syncing Audio...");
			ContinueWithNextAudioAssetOrRootScene(bundleIndex);
			return;
		}
		SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_loading_battlegroundssetproviderpriority" + bundleIndex.ToString().ToLower(), "Loading Audio..."));
		DIContainerInfrastructure.GetAssetsService().Load(text, delegate(string result)
		{
			m_SlowProgresses = 0;
			if (result != null)
			{
				LoadNextBattlegroundBundle(bundleIndex, LoadAudioAssets);
			}
			else
			{
				ContinueWithBattlegroundAssetLoading(bundleIndex, retryCount + 1);
			}
		}, SetDownloadProgress, SetSlowProgress);
	}

	private void LoadNextBattlegroundBundle(AssetbundleIndex index, Action continueAction)
	{
		if (index <= AssetbundleIndex.F)
		{
			ContinueWithBattlegroundAssetLoading(index + 1);
		}
		else
		{
			continueAction();
		}
	}

	private IEnumerator DoLater(float seconds, Action doLater)
	{
		yield return new WaitForSeconds(seconds);
		if (doLater != null)
		{
			doLater();
		}
	}

	private void LoadAudioAssets()
	{
		ContinueWithAssetLoading();
	}

	private void ContinueWithAssetLoading(AssetbundleIndex prio = AssetbundleIndex.A, int retryCount = 0)
	{
		DebugLog.Log("[ContentLoader] ContinueWithAssetLoading, Try #" + retryCount + ", Prio " + prio);
		ClientInfo.AddLoadingTracking("0X_BattlegroundsDownloaded");
		if (retryCount >= 10)
		{
			DebugLog.Log(GetType(), string.Concat("ContinueWithAssetLoading: max retry count for prio ", prio, " of ", 10, " reached!"));
			if (prio == AssetbundleIndex.E)
			{
				DebugLog.Error(GetType(), "ContinueWithAssetLoading failed " + retryCount + " times, not trying anymore.");
				SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_error_audio", "Some sounds failed to download. They will be downloaded next time."), true);
				StartCoroutine(DoLater(2f, ContinueWithVideoAssetLoading));
			}
			else
			{
				SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_error_audio", "Some sounds failed to download. They will be downloaded next time."), true);
				StartCoroutine(DoLater(2f, delegate
				{
					ContinueWithAssetLoading(prio + 1);
				}));
			}
			return;
		}
		string audioAsset = DIContainerInfrastructure.GetTargetBuildGroup() + "_audioassetproviderpriority" + prio.ToString().ToLower() + ".assetbundle";
		if (!DIContainerInfrastructure.GetAssetsService().NeedToDownloadAsset(audioAsset))
		{
			DIContainerInfrastructure.GetStartupLocaService().Tr("startup_syncing_audioassetproviderpriority" + prio.ToString().ToLower(), "Syncing Audio...");
			ContinueWithNextAudioAssetOrRootScene(prio);
			return;
		}
		SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_loading_audioassetproviderpriority" + prio.ToString().ToLower(), "Loading Audio..."));
		DIContainerInfrastructure.GetAssetsService().Load(audioAsset, delegate(string result)
		{
			DebugLog.Log(GetType(), string.Concat("Audio Asset ", audioAsset, " with prio ", prio, " successfully loaded"));
			m_SlowProgresses = 0;
			if (result != null)
			{
				ContinueWithNextAudioAssetOrRootScene(prio);
			}
			else
			{
				ContinueWithAssetLoading(prio, retryCount + 1);
			}
		}, SetDownloadProgress, SetSlowProgress);
	}

	private void ContinueWithNextAudioAssetOrRootScene(AssetbundleIndex currentPrio)
	{
		if (currentPrio < AssetbundleIndex.E)
		{
			ContinueWithAssetLoading(currentPrio + 1);
			return;
		}
		ClientInfo.AddLoadingTracking("10_AudioDownloaded");
		ContinueWithVideoAssetLoading();
	}

	private void ContinueWithVideoAssetLoading()
	{
		string text = ".mp4";
		string text2 = DIContainerInfrastructure.GetTargetBuildGroup() + "_videoclipintro" + text;
		if (!DIContainerInfrastructure.GetAssetsService().NeedToDownloadAsset(text2))
		{
			DIContainerInfrastructure.GetStartupLocaService().Tr("startup_syncing_videoclipintro", "Syncing Video...");
			GotoRootScene();
			return;
		}
		SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_loading_videoclipintro", "Loading Video..."));
		DIContainerInfrastructure.GetAssetsService().Load(text2, delegate(string result)
		{
			DebugLog.Log(GetType(), "Video asset success callback");
			m_SlowProgresses = 0;
			if (result != null)
			{
				ClientInfo.AddLoadingTracking("11_VideoDownloaded");
				GotoRootScene();
			}
			else
			{
				SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_error_video", "Video Intro file failed to download. Playing fallback instead."), true);
				StartCoroutine(DoLater(2f, GotoRootScene));
			}
		}, SetDownloadProgress, SetSlowProgress);
	}

	private void Awake()
	{
		Instance = this;
		ClientInfo.AddLoadingTracking("01_FirstAwake");
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		DIContainerInfrastructure.GetVersionService().Init();
		if ((bool)m_downloadProgressBar)
		{
			SetDownloadProgress(0f);
		}
		if ((bool)m_reconnectInputTrigger)
		{
			m_reconnectInputTrigger.Clicked -= OnReconnectInputTriggerOnClicked;
			m_reconnectInputTrigger.Clicked += OnReconnectInputTriggerOnClicked;
		}
		if ((bool)m_reconnectButton)
		{
			m_reconnectButton.SetActive(false);
		}
	}

	private void OnReconnectInputTriggerOnClicked()
	{
		DebugLog.Log("[ContentLoader] OnReconnectInputTriggerOnClicked");
		if (m_IsWaitingForLoginRetry && m_loginRetryAllowed)
		{
			m_loginRetryAllowed = false;
			m_BeaconConnectionMgr.ConnectToRovioId();
		}
		else if (!m_IsWaitingForLoginRetry)
		{
			HasNetworkConnectionCoroutine();
		}
	}

	public void HideDownloadProgress()
	{
		if ((bool)m_ProgressRoot)
		{
			m_ProgressRoot.gameObject.SetActive(false);
		}
	}

	public void SetDownloadProgressText(string localizedText, bool showAsError = false)
	{
		if (!showAsError && (bool)m_downloadTextUiLabel && !string.IsNullOrEmpty(localizedText))
		{
			m_downloadTextUiLabel.text = localizedText;
		}
		if ((bool)m_ErrorRoot)
		{
			m_ErrorRoot.SetActive(showAsError);
		}
		if (showAsError && (bool)m_ErrorText && !string.IsNullOrEmpty(localizedText))
		{
			m_ErrorText.text = localizedText;
		}
	}

	private void ReportExpansionFileDownloadStatusChangedFromJava(string statusStr)
	{
		int result;
		if (int.TryParse(statusStr, out result) && result == 5 && m_downloadProgressBar != null)
		{
			SetDownloadProgress(1f);
		}
		DebugLog.Log("ReportExpansionFileDownloadStatusChangedFromJava: " + result);
	}

	private void ReportExpansionFileDownloadProgressFromJava(string downloadProgressStr)
	{
		SetDownloadProgressText("Downloading Expansion File");
		AndroidExpansionFileMgr.DownloadProgressInfo downloadProgressInfo = new AndroidExpansionFileMgr.DownloadProgressInfo().FromString(downloadProgressStr);
		DebugLog.Log("Progressbar fillamount " + downloadProgressInfo.OverallProgress + "/" + downloadProgressInfo.OverallTotal + " = " + downloadProgressInfo.CurrentProgress);
		if (m_downloadProgressBar != null)
		{
			SetDownloadProgress(downloadProgressInfo.CurrentProgress);
		}
	}

	private void ReportExpansionFileDownloadErrorFromJava(string err)
	{
		SetDownloadProgressText(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_error_expansion", "Error Downloading Expansion File") + " (" + err + ")", true);
		DebugLog.Error("[ContentLoader] OnOBBDownloadError: " + err);
		m_androidExpansionFileMgr.StopWaitForFinishingObbDownload = true;
	}

	private void GotoRootScene()
	{
		DIContainerBalancing.OnBalancingDataInitialized -= GotoRootScene;
		if (!m_GotoRootTriggered)
		{
			m_GotoRootTriggered = true;
			DebugLog.Log("[ContentLoader] Continuing with root scene");
			if ((bool)m_downloadProgressBar)
			{
				SetDownloadProgress(1f);
			}
			if (DIContainerInfrastructure.GetCoreStateMgr() == null)
			{
				SceneManager.LoadScene(1, LoadSceneMode.Additive);
			}
			StartCoroutine(WaitAndGotoRootScene());
			IsDone = true;
		}
	}

	private IEnumerator WaitAndGotoRootScene()
	{
		while (DIContainerInfrastructure.GetCoreStateMgr() == null || !DIContainerInfrastructure.GetCoreStateMgr().m_isInitialized)
		{
			yield return new WaitForEndOfFrame();
		}
		DebugLog.Log(GetType(), "WaitAndGotoRootScene: Core State Manager initialized!");
		ContentLoader contentLoader = this;
		contentLoader.OnInternetConnectivityReceived = (Action<bool>)Delegate.Remove(contentLoader.OnInternetConnectivityReceived, new Action<bool>(HandleInternetConnectivityStatusForContentLoader));
		m_alwaysOnCheckFrequency = m_alwaysOnCheckFrequencyGame;
		CancelInvoke("HasNetworkConnectionCoroutine");
		InvokeRepeating("HasNetworkConnectionCoroutine", m_alwaysOnCheckFrequency, m_alwaysOnCheckFrequency);
		float timePassed = Time.realtimeSinceStartup - m_startLoadTime;
		if (timePassed < 2f)
		{
			yield return new WaitForSeconds(2f - timePassed);
		}
		if (DIContainerConfig.GetClientConfig().UseChimeraLeaderboards || (DIContainerInfrastructure.GetCurrentPlayer() != null && DIContainerInfrastructure.GetCurrentPlayer().Data != null && ((DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentEventManager != null && !string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentEventManager.LeaderboardId)) || (DIContainerInfrastructure.GetCurrentPlayer().Data.PvpSeasonManager != null && DIContainerInfrastructure.GetCurrentPlayer().Data.PvpSeasonManager.CurrentSeasonTurn != null && !string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().Data.PvpSeasonManager.CurrentSeasonTurn.LeaderboardId)))))
		{
			// DIContainerLogic.BackendService.Authenticate(delegate
			// {
			// 	DebugLog.Log(GetType(), "Authentication succesfull");
			// }, delegate(int errorCode)
			// {
			// 	DebugLog.Error(GetType(), "Failed to authenticate with errorcode " + errorCode);
			// }); disable auth because decomp is offline
		}
		InitialTracking();
		DIContainerInfrastructure.GetCoreStateMgr().GotoWorldMap();
	}

	private void InitialTracking()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
		ABHAnalyticsHelper.AddMasteryLevelsToTracking(dictionary);
		ABHAnalyticsHelper.AddEnchantmentLevelsToTracking(dictionary);
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("GameLoadInit", dictionary);
	}

	private void HasNetworkConnectionCoroutine()
	{
		DebugLog.Log("[ContentLoader] HasNetworkConnectionCoroutine");
		StartCoroutine(CheckConnectivity());
	}

	public IEnumerator CheckConnectivity()
	{
		return DIContainerInfrastructure.GetNetworkStatusService().CheckInternetAvailability(null, delegate(WebRequest request, bool available)
		{
			if (this.OnInternetConnectivityReceived != null)
			{
				this.OnInternetConnectivityReceived(available);
			}
		});
	}

	public void CheckConnectivityAsync()
	{
		StartCoroutine(DIContainerInfrastructure.GetNetworkStatusService().CheckInternetAvailability(null, delegate(WebRequest request, bool available)
		{
			if (this.OnInternetConnectivityReceived != null)
			{
				this.OnInternetConnectivityReceived(available);
			}
		}));
	}

	public void SetDownloadProgress(float loadingProgress)
	{
		if ((bool)m_downloadProgressBar)
		{
			m_downloadProgressBar.fillAmount = loadingProgress;
			if ((bool)m_loadingProgressPanel)
			{
				float num = Mathf.Max(m_downloadProgressBar.transform.localScale.x * m_downloadProgressBar.fillAmount, 1f);
				float x = num / 2f;
				m_loadingProgressPanel.clipRange = new Vector4(x, m_loadingProgressPanel.clipRange.y, num, m_loadingProgressPanel.clipRange.w);
			}
		}
	}

	public void SetDownloadProgressLabel(float current, float max)
	{
		if ((bool)m_downloadProgressLabel)
		{
			m_downloadProgressLabel.gameObject.SetActive(true);
			m_downloadProgressLabel.text = current.ToString("00.0") + " MB / " + max.ToString("00.0") + " MB";
		}
	}

	public bool DestroyLoadingScreen(float delayedDestroy)
	{
		if ((bool)Instance)
		{
			Instance.SetDownloadProgress(1f);
		}
		if ((bool)m_PopupRoot)
		{
			UnityEngine.Object.Destroy(m_PopupRoot, 0.15f);
		}
		if ((bool)m_splashScreens)
		{
			UnityEngine.Object.Destroy(m_splashScreens, delayedDestroy);
		}
		if ((bool)m_contentLoaderUI)
		{
			UnityEngine.Object.Destroy(m_contentLoaderUI, delayedDestroy);
		}
		return true;
	}

	public void BlockedByServerUnavailable()
	{
		DIContainerInfrastructure.GetCoreStateMgr().ShowConfirmationPopup(DIContainerInfrastructure.GetLocaService().Tr("startup_server_restart"), ReloadMap);
	}

	public void CheckforRestartApp()
	{
		DIContainerInfrastructure.GetAssetsService().ReloadBalancingIfneeded(Hatch2_OnBalancingDataLoadedQuickStart);
	}

	public void Hatch2_OnBalancingDataLoadedQuickStart()
	{
		DIContainerBalancing.OnBalancingDataInitialized -= OnBalancingInitQuickStart;
		DIContainerBalancing.OnBalancingDataInitialized += OnBalancingInitQuickStart;
		DIContainerBalancing.Reset();
		DIContainerBalancing.Init(null, true);
	}

	private void OnBalancingInitQuickStart()
	{
		DIContainerInfrastructure.GetLocaService().InitDefaultLoca(this);
		StopAllPopups();
		DIContainerBalancing.OnBalancingDataInitialized -= OnBalancingInitQuickStart;
		DIContainerLogic.GetShopService().ClearShopBalancingCache();
		DIContainerLogic.GetSalesManagerService().ClearSalesCache();
		DIContainerLogic.DailyLoginLogic.ClearDailyRewardCache();
		DIContainerInfrastructure.GetCoreStateMgr().m_DailyLoginUi.ClearItemDisplayCache();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.ReInitialize();
		DIContainerInfrastructure.GetPowerLevelCalculator().ClearCache();
		DIContainerBalancing.LootTableBalancingDataPovider.ResetCache();
		DIContainerInfrastructure.GetCoreStateMgr().ShowConfirmationPopup(DIContainerInfrastructure.GetLocaService().Tr("startup_balancing_restart"), ReloadMap);
	}

	public void ReloadMap()
	{
		DIContainerInfrastructure.TimeScaleMgr.RemoveTimeScale("battle_pause");
		DIContainerInfrastructure.TimeScaleMgr.RemoveTimeScale("story_speedup");
		DIContainerInfrastructure.TimeScaleMgr.RemoveTimeScale("slowmotion");
		StartCoroutine(ReloadEventBalancingAfterPlayerRestart());
	}

	private IEnumerator ReloadEventBalancingAfterPlayerRestart()
	{
		DIContainerInfrastructure.InitCurrentPlayerIfNecessary(null, true);
		PlayerGameData player = DIContainerInfrastructure.GetCurrentPlayer();
		DIContainerInfrastructure.EventSystemStateManager.ResetEventManager();
		DIContainerInfrastructure.PvPSeasonStateMgr.ResetPvPSystem();
		while (DIContainerBalancing.EventBalancingLoadingPending)
		{
			yield return new WaitForEndOfFrame();
		}
		player.GenerateEventManagerFromProfile();
		player.GeneratePvPManagerFromProfile();
		DIContainerInfrastructure.GetCoreStateMgr().RefreshFriends();
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("balancing_reload", string.Empty);
		StartCoroutine(DIContainerInfrastructure.GetCustomMessageService().Init(delegate
		{
		}, delegate
		{
		}, delegate
		{
		}));
		if ((bool)DIContainerInfrastructure.LocationStateMgr)
		{
			DIContainerInfrastructure.LocationStateMgr.BlockFeatureUnlocks = false;
		}
		DIContainerInfrastructure.GetCoreStateMgr().GotoWorldMap();
	}

	private void StopAllPopups()
	{
		if (DIContainerInfrastructure.LocationStateMgr != null)
		{
			DIContainerInfrastructure.LocationStateMgr.StopPopupCoroutine();
			DIContainerInfrastructure.LocationStateMgr.BlockFeatureUnlocks = true;
		}
		if (DIContainerInfrastructure.GetCoreStateMgr().m_FeatureUnlockedPopup != null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_FeatureUnlockedPopup.LeavePopup();
		}
		if (DIContainerInfrastructure.GetCoreStateMgr().m_SpecialOfferPopup != null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_SpecialOfferPopup.QuickLeave();
		}
	}

	public IEnumerator SubmitExceptionReport(Exception exception)
	{
		byte[] screenshot = null;
		StartCoroutine(GetScreenshot(delegate(byte[] result)
		{
			screenshot = result;
		}));
		while (screenshot == null)
		{
			yield return null;
		}
		string clientId;
		string clientSecret;
		if (!DIContainerConfig.GetSkynestClientIdAndSecret(out clientId, out clientSecret))
		{
			Debug.LogError("[ContentLoader] Error getting clientId, quiting submit exception report");
			yield break;
		}
		if (string.IsNullOrEmpty(clientId))
		{
			Debug.LogError("[ContentLoader] clientId is not yet set");
			yield break;
		}
		WWWForm form = new WWWForm();
		form.AddField("clientId", clientId);
		form.AddField("exception", Convert.ToBase64String(Encoding.UTF8.GetBytes(exception.ToString())));
		form.AddField("timestamp", ((DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000).ToString());
		form.AddField("codeLanguage", "C#");
		form.AddField("screenshot", Convert.ToBase64String(screenshot));
		string url = "https://error-reporter-dev7612.loca.lt";
		WWW www2 = new WWW(url + "/submitException", form);
		yield return www2;
		if (!www2.isDone || !www2.text.Contains("/continue/"))
		{
			yield break;
		}
		string[] array = www2.text.Split("\r\n".ToCharArray());
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].Contains("/continue/"))
			{
				continue;
			}
			string path = array[i];
			for (int j = 0; j < path.Length; j++)
			{
				if (!(path.Substring(j, 1) == "\""))
				{
					continue;
				}
				path = path.Substring(j + 1);
				for (int k = 0; k < path.Length; k++)
				{
					if (path.Substring(k, 1) == "\"")
					{
						path = path.Substring(0, k);
						break;
					}
				}
				break;
			}
			www2 = new WWW("https://ipv4.icanhazip.com/");
			yield return www2;
			if (www2.isDone && string.IsNullOrEmpty(www2.error))
			{
				WWWForm wWWForm = new WWWForm();
				wWWForm.AddField("endpoint", www2.text.Trim());
				www2 = new WWW(url + path, wWWForm);
				yield return www2;
			}
			try
			{
				if (!www2.isDone || !string.IsNullOrEmpty(www2.error))
				{
					continue;
				}
				Dictionary<string, object> dictionary = SimpleJsonConverter.DecodeJsonDict(www2.text);
				bool flag = false;
				string text = null;
				if (dictionary.ContainsKey("success"))
				{
					flag = Convert.ToBoolean(dictionary["success"]);
				}
				if (dictionary.ContainsKey("error"))
				{
					text = (string)dictionary["error"];
				}
				if (!flag)
				{
					if (!string.IsNullOrEmpty(text))
					{
						Debug.LogError("[ContentLoader] Error submitting exception report: " + text);
					}
					else
					{
						Debug.LogError("[ContentLoader] Error submitting exception report: Unknown error message");
					}
					yield break;
				}
			}
			catch
			{
				Debug.LogError("[ContentLoader] Error decoding JSON, quitting submit exception report");
				yield break;
			}
		}
		yield return new WWW(url + "/submitException", form);
	}

	public IEnumerator SubmitErrorReport(string errMsg)
	{
		string clientId;
		string clientSecret;
		if (!DIContainerConfig.GetSkynestClientIdAndSecret(out clientId, out clientSecret))
		{
			Debug.LogError("[ContentLoader] Error getting clientId, quiting submit error report");
			yield break;
		}
		if (string.IsNullOrEmpty(clientId))
		{
			Debug.LogError("[ContentLoader] clientId is not yet set");
			yield break;
		}
		WWWForm form = new WWWForm();
		form.AddField("clientId", clientId);
		form.AddField("errorMessage", Convert.ToBase64String(Encoding.UTF8.GetBytes(errMsg)));
		form.AddField("timestamp", ((DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000).ToString());
		string url = "https://error-reporter-dev7612.loca.lt";
		WWW www2 = new WWW(url + "/submitError", form);
		yield return www2;
		if (!www2.isDone || !www2.text.Contains("/continue/"))
		{
			yield break;
		}
		string[] array = www2.text.Split("\r\n".ToCharArray());
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].Contains("/continue/"))
			{
				continue;
			}
			string path = array[i];
			for (int j = 0; j < path.Length; j++)
			{
				if (!(path.Substring(j, 1) == "\""))
				{
					continue;
				}
				path = path.Substring(j + 1);
				for (int k = 0; k < path.Length; k++)
				{
					if (path.Substring(k, 1) == "\"")
					{
						path = path.Substring(0, k);
						break;
					}
				}
				break;
			}
			www2 = new WWW("https://ipv4.icanhazip.com/");
			yield return www2;
			if (www2.isDone && string.IsNullOrEmpty(www2.error))
			{
				WWWForm wWWForm = new WWWForm();
				wWWForm.AddField("endpoint", www2.text.Trim());
				www2 = new WWW(url + path, wWWForm);
				yield return www2;
			}
			try
			{
				if (!www2.isDone || !string.IsNullOrEmpty(www2.error))
				{
					continue;
				}
				Dictionary<string, object> dictionary = SimpleJsonConverter.DecodeJsonDict(www2.text);
				bool flag = false;
				string text = null;
				if (dictionary.ContainsKey("success"))
				{
					flag = Convert.ToBoolean(dictionary["success"]);
				}
				if (dictionary.ContainsKey("error"))
				{
					text = (string)dictionary["error"];
				}
				if (!flag)
				{
					if (!string.IsNullOrEmpty(text))
					{
						Debug.LogError("[ContentLoader] Error submitting error report: " + text);
					}
					else
					{
						Debug.LogError("[ContentLoader] Error submitting error report: Unknown error message");
					}
					yield break;
				}
			}
			catch
			{
				Debug.LogError("[ContentLoader] Error decoding JSON, quitting submit error report");
				yield break;
			}
		}
		yield return new WWW(url + "/submitError", form);
	}

	public IEnumerator SubmitExceptionReport(string exception)
	{
		byte[] screenshot = null;
		StartCoroutine(GetScreenshot(delegate(byte[] result)
		{
			screenshot = result;
		}));
		while (screenshot == null)
		{
			yield return null;
		}
		string clientId;
		string clientSecret;
		if (!DIContainerConfig.GetSkynestClientIdAndSecret(out clientId, out clientSecret))
		{
			Debug.LogError("[ContentLoader] Error getting clientId, quiting submit exception report");
			yield break;
		}
		if (string.IsNullOrEmpty(clientId))
		{
			Debug.LogError("[ContentLoader] clientId is not yet set");
			yield break;
		}
		WWWForm form = new WWWForm();
		form.AddField("clientId", clientId);
		form.AddField("exception", Convert.ToBase64String(Encoding.UTF8.GetBytes(exception)));
		form.AddField("timestamp", ((DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000).ToString());
		form.AddField("codeLanguage", "C#");
		form.AddField("screenshot", Convert.ToBase64String(screenshot));
		string url = "https://error-reporter-dev7612.loca.lt";
		WWW www2 = new WWW(url + "/submitException", form);
		yield return www2;
		if (!www2.isDone || !www2.text.Contains("/continue/"))
		{
			yield break;
		}
		string[] array = www2.text.Split("\r\n".ToCharArray());
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].Contains("/continue/"))
			{
				continue;
			}
			string path = array[i];
			for (int j = 0; j < path.Length; j++)
			{
				if (!(path.Substring(j, 1) == "\""))
				{
					continue;
				}
				path = path.Substring(j + 1);
				for (int k = 0; k < path.Length; k++)
				{
					if (path.Substring(k, 1) == "\"")
					{
						path = path.Substring(0, k);
						break;
					}
				}
				break;
			}
			www2 = new WWW("https://ipv4.icanhazip.com/");
			yield return www2;
			if (www2.isDone && string.IsNullOrEmpty(www2.error))
			{
				WWWForm wWWForm = new WWWForm();
				wWWForm.AddField("endpoint", www2.text.Trim());
				www2 = new WWW(url + path, wWWForm);
				yield return www2;
			}
			try
			{
				if (!www2.isDone || !string.IsNullOrEmpty(www2.error))
				{
					continue;
				}
				Dictionary<string, object> dictionary = SimpleJsonConverter.DecodeJsonDict(www2.text);
				bool flag = false;
				string text = null;
				if (dictionary.ContainsKey("success"))
				{
					flag = Convert.ToBoolean(dictionary["success"]);
				}
				if (dictionary.ContainsKey("error"))
				{
					text = (string)dictionary["error"];
				}
				if (!flag)
				{
					if (!string.IsNullOrEmpty(text))
					{
						Debug.LogError("[ContentLoader] Error submitting exception report: " + text);
					}
					else
					{
						Debug.LogError("[ContentLoader] Error submitting exception report: Unknown error message");
					}
					yield break;
				}
			}
			catch
			{
				Debug.LogError("[ContentLoader] Error decoding JSON, quitting submit exception report");
				yield break;
			}
		}
		yield return new WWW(url + "/submitException", form);
	}

	public bool SetIsConfirmed(bool isConfirmed, Action onSuccessCallback)
	{
		Identity identiy = m_BeaconConnectionMgr.Identiy;
		if (string.IsNullOrEmpty(identiy.GetAccessToken()))
		{
			return false;
		}
		identiy.SetProfileField("isConfirmed", new Variant(isConfirmed));
		if (identiy.GetProfileFieldsAsJson().Contains("\"isConfirmed\":" + isConfirmed))
		{
			if (onSuccessCallback != null)
			{
				onSuccessCallback();
			}
			return true;
		}
		return false;
	}

	public void SubmitErrorReportCoroutine(string errMsg)
	{
		StartCoroutine(SubmitErrorReport(errMsg));
	}

	private IEnumerator GetScreenshot(Action<byte[]> callback)
	{
		// no screenshots buddy
		yield return null;
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using ABH.Services.Logic;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Character;
using ABH.Shared.Models.Generic;
using Interfaces.Identity;
using SmoothMoves;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CoreStateMgr : MonoBehaviourContainerBase
{
	public PacingBalancing m_PacingBalancing;

	protected SceneLoadingMgr m_SceneLoadingMgr;

	public VisualEffectsBalancing m_VisualEffectsBalancing;

	public TutorialMgr m_TutorialMgr;

	public TutorialMgrNullImpl m_TutorialNullMgr;

	[HideInInspector]
	public GenericUIStateMgr m_GenericUI;

	[HideInInspector]
	public InfoOverlayMgr m_InfoOverlays;

	public Transform m_GenericInterfaceRoot;

	[HideInInspector]
	public WindowRootUI m_WindowRoot;

	[HideInInspector]
	public PopupRootUI m_PopupRoot;

	[HideInInspector]
	public PopupFeatureUnlockedStateMgr m_FeatureUnlockedPopup;

	[HideInInspector]
	public PopupSpecialOfferStateMgr m_SpecialOfferPopup;

	[HideInInspector]
	public PopupSpecialGachaStateMgr m_SpecialGachaPopup;

	[HideInInspector]
	public PopupSocialUnlockedStateMgr m_SocialUnlockedPopup;

	[HideInInspector]
	public PopupRankUpStateMgr m_RankUpPopup;

	[HideInInspector]
	public PopupMissingResourcesStateMgr m_MissingResourcesPopup;

	[HideInInspector]
	public PopupLowEnergy m_LowEnergyPopup;

	[HideInInspector]
	public PopupMissingEnergy m_MissingEnergyPopup;

	[HideInInspector]
	public PopupMissingCurrency m_MissingCurrencyPopup;

	[HideInInspector]
	public PopupRateAppStateMgr m_RateAppPopup;

	[HideInInspector]
	public PopupShowNotificationStateMgr m_NotificationPopup;

	[HideInInspector]
	public AlwaysOnRootUI m_AlwaysOnRoot;

	[HideInInspector]
	public PopupLevelUpStateMgr m_LevelUpPopup;

	[HideInInspector]
	public PvpSeasonResultUI m_SeasonEndPopup;

	[HideInInspector]
	public PopupWP8AchievementsUI m_popupWp8Achievements;

	[HideInInspector]
	public EventPreviewUI m_eventTeaserScreen;

	[HideInInspector]
	public ClassInfoPopup m_ClassInfoUI;

	[HideInInspector]
	public ShopInfoPopup m_ShopInfoPopup;

	[HideInInspector]
	public PopupArenaLocked m_ArenaLockedPopup;

	[HideInInspector]
	public DungeonsLockedPopup m_DungeonsLockedPopup;

	[HideInInspector]
	public PopupEventLocked m_EventLockedPopup;

	[HideInInspector]
	public PopupEnterName m_EnterNamePopup;

	[HideInInspector]
	public ShopWindowStateMgr m_ShopWindow;

	[HideInInspector]
	public ConfirmationPopupController m_ConfirmationPopup;

	[HideInInspector]
	public FriendBirdWindowStateMgr m_FriendListWindow;

	[HideInInspector]
	public BonusCodeManager m_BonusCodeManager;

	[HideInInspector]
	public DailyLoginUI m_DailyLoginUi;

	[HideInInspector]
	public GachaSetItemInfoPopup m_SetItemInfoUi;

	[HideInInspector]
	public EliteChestRewardUI m_EliteChestUnlockPopup;

	public Camera m_InterfaceCamera;

	public bool m_isInitialized;

	public bool m_DisableStorySequences;

	public bool m_ChronicleCave;

	public bool m_EventCampaign;

	public Action m_returnFromMainMenuAction;

	public bool m_WorldMapGenerated;

	public bool m_UseTutorial = false;

	public CronJob m_CronJob;

	public bool m_EnterOnce;

	private bool m_GotMe;

	public bool m_GameIsPaused;

	public AudioListener m_InterfaceAudioListener;

	public AudioListener m_SceneryAudioListener;

	public bool m_UseSwipeInteractionInBirdManager = true;

	private bool m_ShopIsLoading;

	public bool m_googlePlusAsked;

	public bool m_ByGoToWorldMap;

	public bool m_AllowCalendar;

	public int m_ZoomToDungeonId = 8;

	public bool m_GoToDojo;

	public CampStateMgr m_CampStateMgr;

	public ArenaCampStateMgr m_ArenaCampStateMgr;

	[SerializeField]
	private float m_friendRefreshCheckFrequency = 120f;

	[SerializeField]
	private List<GenericAssetProvider> m_LongLoadingAssetProviders;

	protected Dictionary<string, bool> m_LoadedLevels = new Dictionary<string, bool>();

	[SerializeField]
	private Transform m_TempObjectRoot;

	private IdentityCredentials m_CachedCredentials;

	private bool m_TimeSet;

	public bool UseDebug;

	public string m_CurrentPendingStorySequence;

	public bool m_AllInputBlocked;

	private bool m_LeaveShopAfterLoading;

	public string m_HotLinkItemName;

	public string m_ShopEnterSource;

	public bool m_PopupEntered;

	public DateTime m_NextShowNewsTime;

	public bool m_ReturnToCamp;

	public bool m_ReturnToArena;

	public bool m_IsWithinPvP;

	public bool m_FriendListIsLoading;

	private ClassItemGameData m_cachedClassItem;

	private SkinItemGameData m_cachedSkinItem;

	private static Dictionary<string, string> SharedIds;

	public static CoreStateMgr Instance { get; protected set; }

	public SceneLoadingMgr SceneLoadingMgr
	{
		get
		{
			return m_SceneLoadingMgr ?? (m_SceneLoadingMgr = GetComponent<SceneLoadingMgr>());
		}
	}

	public bool IsAnyPopupActive
	{
		get
		{
			if ((bool)DIContainerInfrastructure.LocationStateMgr && DIContainerInfrastructure.LocationStateMgr.FeatureUnlocksRunning)
			{
				return true;
			}
			WorldMapStateMgr worldMapStateMgr = DIContainerInfrastructure.LocationStateMgr as WorldMapStateMgr;
			if (worldMapStateMgr != null && worldMapStateMgr.m_EventPreviewUI != null && worldMapStateMgr.m_EventPreviewUI.gameObject.activeInHierarchy)
			{
				return true;
			}
			if (worldMapStateMgr != null && worldMapStateMgr.m_battlePreperation != null && worldMapStateMgr.m_battlePreperation.gameObject.activeInHierarchy)
			{
				return true;
			}
			if (m_DailyLoginUi.gameObject.activeSelf)
			{
				return true;
			}
			if (SceneLoadingMgr.IsLoading())
			{
				return true;
			}
			return false;
		}
	}

	[method: MethodImpl(32)]
	public event Action OnReCheckSpecialOffers;

	[method: MethodImpl(32)]
	public event Action<bool> OnPopupEnter;

	[method: MethodImpl(32)]
	public event Action CampEntered;

	[method: MethodImpl(32)]
	public event Action OnShopClosed;

	public Transform GetTempObjectRoot()
	{
		return m_TempObjectRoot;
	}

	protected virtual void Awake()
	{
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(AnimationManager));
		for (int num = array.Length - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(array[num]);
		}
		m_WorldMapGenerated = true;
		if (ContentLoader.Instance == null)
		{
			SceneManager.LoadScene("ContentLoader");
		}
		Application.logMessageReceived -= HandleAndReport;
		Application.logMessageReceivedThreaded -= HandleAndReport;
		Application.logMessageReceived += HandleAndReport;
		Application.logMessageReceivedThreaded += HandleAndReport;
	}

	private void OnApplicationPause(bool paused)
	{
		DebugLog.Log(GetType(), "OnApplicationPause: paused = " + paused);
		if (m_isInitialized)
		{
			if (!paused)
			{
				DIContainerInfrastructure.GetAppResumeService().OnAppResumed();
			}
			else
			{
				DIContainerInfrastructure.GetAppResumeService().OnAppPaused();
			}
			m_GameIsPaused = paused;
		}
	}

	

	private void Update()
	{
		UpdateAudioListeners();
	}

	public void ToggleDebug()
	{
		bool useDebug = !DIContainerInfrastructure.GetCoreStateMgr().UseDebug;
		DIContainerInfrastructure.GetCoreStateMgr().UseDebug = useDebug;
		GenericDebugUI[] array = UnityEngine.Object.FindObjectsOfType(typeof(GenericDebugUI)) as GenericDebugUI[];
		if (array != null)
		{
			GenericDebugUI[] array2 = array;
			foreach (GenericDebugUI genericDebugUI in array2)
			{
				genericDebugUI.enabled = useDebug;
			}
		}
		FPSDisplay fPSDisplay = UnityEngine.Object.FindObjectOfType(typeof(FPSDisplay)) as FPSDisplay;
		if (fPSDisplay != null)
		{
			fPSDisplay.enabled = useDebug;
		}
		TimeDisplay timeDisplay = UnityEngine.Object.FindObjectOfType(typeof(TimeDisplay)) as TimeDisplay;
		if (timeDisplay != null)
		{
			timeDisplay.enabled = useDebug;
		}
	}

	private void UpdateAudioListeners()
	{
		if (!(m_InterfaceAudioListener == null))
		{
			m_InterfaceAudioListener.enabled = m_SceneryAudioListener == null || (m_PopupRoot != null && m_PopupRoot.entered) || (m_WindowRoot != null && m_WindowRoot.entered) || (m_LevelUpPopup != null && m_LevelUpPopup.m_IsShowing) || (m_MissingResourcesPopup != null && m_MissingResourcesPopup.m_IsShowing);
			if (!(m_SceneryAudioListener == null))
			{
				m_SceneryAudioListener.enabled = !m_InterfaceAudioListener.enabled;
			}
		}
	}

	public void BlockAllInput(bool block, bool ignoreBackButton = false)
	{
		m_AllInputBlocked = block;
		if (!ignoreBackButton)
		{
			if (m_AllInputBlocked)
			{
				DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("all_input");
			}
			else
			{
				DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("all_input");
			}
		}
		UICamera component = m_InterfaceCamera.GetComponent<UICamera>();
		if ((bool)component)
		{
			component.enabled = !block;
		}
		TouchInputHandler component2 = GetComponent<TouchInputHandler>();
		if ((bool)component2)
		{
			component2.enabled = !block;
		}
	}

	private void SetDownloadProgressTextInContentLoader(string txt)
	{
		if (ContentLoader.Instance != null)
		{
			ContentLoader.Instance.SetDownloadProgressText(txt);
		}
	}

	protected virtual IEnumerator Start()
	{
		while (ContentLoader.Instance == null || !ContentLoader.Instance.IsDone)
		{
			yield return new WaitForEndOfFrame();
		}
		ClientInfo.AddLoadingTracking("11_RootLoaded");
		DebugLog.Log(GetType(), "Starting " + DIContainerConfig.GetAppDisplayName());
		DebugLog.Log(GetType(), "Waiting for audio providers to get initialized...");
		yield return StartCoroutine(WaitForAudioProviders());
		DebugLog.Log(GetType(), "... audio providers initialized.");
		ClientInfo.AddLoadingTracking("12_AudioInitialized");
		DebugLog.Log(GetType(), "Start initializing balancing...");
		yield return StartCoroutine(InitializeBalancing());
		DebugLog.Log(GetType(), "... balancing initialized.");
		DebugLog.Log(GetType(), "Initialize current player profile...");
		yield return StartCoroutine(InitializeCurrentPlayerProfile());
		DebugLog.Log(GetType(), "... current player profile initialized.");
		DebugLog.Log(GetType(), "Initialize sound settings...");
		InitializeSoundSettings();
		DebugLog.Log(GetType(), "Initialize online services...");
		InitializeOnlineServices();
		DebugLog.Log(GetType(), "Start initializing loca...");
		yield return StartCoroutine(InitializeLocalization());
		DebugLog.Log(GetType(), "... loca initialized.");
		ClientInfo.AddLoadingTracking("13_LocaInitialized");
		DebugLog.Log(GetType(), "Initialize achievement service...");
		DIContainerInfrastructure.GetAchievementService().Init(this, false);
		DebugLog.Log(GetType(), "Start initial scene setup...");
		yield return StartCoroutine(SceneLoadingMgr.LoadInitialStartupScenesCoroutine());
		DebugLog.Log(GetType(), "... initial scene setup done.");
		ClientInfo.AddLoadingTracking("14_GenericScenesLoaded");
		AddComponentSafely(ref m_CronJob);
		yield return new WaitForEndOfFrame();
		if (!DIContainerInfrastructure.GetAppResumeService().OnAppResumed())
		{
			SetDownloadProgressTextInContentLoader("Error syncing Player Profile!");
		}
		StartRefreshFriends();
		m_NextShowNewsTime = DIContainerLogic.GetTimingService().GetPresentTime().AddSeconds(60.0);
		yield return StartCoroutine(UnloadUnusedAssetsAndGCCollectCoroutine());
		ITutorialMgr tutmgr = DIContainerInfrastructure.TutorialMgr;
		DIContainerLogic.CustomerSupportService.Initialize();
		if (ContentLoader.Instance != null)
		{
			ContentLoader.Instance.OnInternetConnectivityReceived += OnInternetConnectivityReceived;
		}
		ClientInfo.AddLoadingTracking("15_RootDone");
		ShowAndSendLoadingTracking();
		DebugLog.Log(GetType(), "Loading tracking sent.");
		ReportInstallTime();
		DebugLog.Log(GetType(), "Install time reported.");
		DIContainerInfrastructure.NotificationService().Init();
		StartCoroutine(DIContainerInfrastructure.GetCustomMessageService().Init(delegate
		{
		}, delegate
		{
		}, delegate
		{
		}));
		DebugLog.Log(GetType(), "Custom message service initialized.");
		m_isInitialized = true;
		DebugLog.Log(GetType(), "... start done.");
	}

	private IEnumerator WaitForAudioProviders()
	{
		int notloadedAudioCount = m_LongLoadingAssetProviders.Count((GenericAssetProvider e) => !e.m_Initialized);
		SetDownloadProgressTextInContentLoader(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_memory_audio", "Loading Audio into memory..."));
		while (notloadedAudioCount > 0)
		{
			yield return new WaitForEndOfFrame();
			notloadedAudioCount = m_LongLoadingAssetProviders.Count((GenericAssetProvider e) => !e.m_Initialized);
			if (ContentLoader.Instance != null)
			{
				float downloadProgress = (float)(m_LongLoadingAssetProviders.Count - notloadedAudioCount) / (float)m_LongLoadingAssetProviders.Count;
				ContentLoader.Instance.SetDownloadProgress(downloadProgress);
			}
		}
	}

	private IEnumerator InitializeBalancing()
	{
		SetDownloadProgressTextInContentLoader(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_memory_balancing", "Loading Balancing into memory..."));
		DIContainerBalancing.Init();
		while (!DIContainerBalancing.IsInitialized)
		{
			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator InitializeCurrentPlayerProfile()
	{
		DIContainerInfrastructure.InitCurrentPlayerIfNecessary();
		while (!DIContainerInfrastructure.IsCurrentPlayerInitialized)
		{
			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator InitializeLocalization()
	{
		SetDownloadProgressTextInContentLoader(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_memory_loca", "Loading game texts..."));
		DIContainerInfrastructure.GetLocaService().InitDefaultLoca(this);
		DebugLog.Log(GetType(), "Starting loca outer loop.");
		ContentLoader.FakeProgress fakeProgress = new ContentLoader.FakeProgress(5f, ContentLoader.Instance.SetDownloadProgress);
		while (!DIContainerInfrastructure.GetLocaService().Initialized)
		{
			fakeProgress.Update(0.1f);
			yield return new WaitForSeconds(0.1f);
		}
	}

	private void InitializeSoundSettings()
	{
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.IsMusicMuted)
		{
			DIContainerInfrastructure.AudioManager.AddMuteReason(0, "Data.IsMusicMuted");
		}
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.IsSoundMuted)
		{
			DIContainerInfrastructure.AudioManager.AddMuteReason(1, "Data.IsSoundMuted");
		}
		DIContainerInfrastructure.AudioManager.PlayMusic("music_main");
	}

	private void InitializeOnlineServices()
	{
		DIContainerInfrastructure.GetFacebookWrapper().Initialize(null);
		DIContainerInfrastructure.MessagingService.Initialize();
		DIContainerInfrastructure.GetAttributionService().Init();
		DIContainerInfrastructure.AdService.Init();
		DIContainerInfrastructure.PurchasingService.Initialize(DIContainerConfig.GetClientConfig().BundleId);
	}

	public void OnInternetConnectivityReceived(bool connected)
	{
		if (connected && m_AlwaysOnRoot.entered)
		{
			m_AlwaysOnRoot.Leave();
			if (DIContainerInfrastructure.AdService.RestoreSuspendedNewsFeed())
			{
				DebugLog.Log(GetType(), "OnInternetConnectivityReceived: Found NewsFeeds to revive from suspension!");
			}
		}
		else if (!connected && !m_AlwaysOnRoot.entered)
		{
			m_AlwaysOnRoot.Enter();
			if (DIContainerInfrastructure.AdService.SuspendNewsFeeds())
			{
				DebugLog.Log(GetType(), "OnInternetConnectivityReceived: Found active NewsFeed placement - Suspending!");
			}
		}
	}

	public bool TryLoginOnFacebook()
	{
		if (!DIContainerInfrastructure.GetFacebookWrapper().IsUserAuthenticated())
		{
			StartFacebookOp();
			DIContainerInfrastructure.GetFacebookWrapper().BeginLogin();
			StartCoroutine(DIContainerInfrastructure.GetFacebookWrapper().TriggerFallbackLogin());
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
			DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("FacebookLoginTry", dictionary);
			return true;
		}
		return false;
	}

	public bool TryLogoutOfFacebook()
	{
		if (DIContainerInfrastructure.GetFacebookWrapper().IsUserAuthenticated())
		{
			DIContainerInfrastructure.GetFacebookWrapper().Logout();
			StartFacebookOp();
			return true;
		}
		return false;
	}

	public void FacebookLoginSucceeded()
	{
		EndFacebookOp();
	}

	public void FacebookLoginFailed(string str)
	{
		EndFacebookOp();
	}

	private void StartFacebookOp()
	{
		RemoveFacebookEventHandlers();
		DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent += FacebookLoginSucceeded;
		DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent += FacebookLoginFailed;
		DIContainerInfrastructure.GetFacebookWrapper().logoutFailedEvent += FacebookLogoutFailed;
		DIContainerInfrastructure.GetFacebookWrapper().logoutSucceededEvent += FacebookLogoutSucceeded;
		DIContainerInfrastructure.GetAsynchStatusService().ShowLocalLoading(DIContainerInfrastructure.GetLocaService().Tr("toast_fb_operation", "Please wait..."), true);
		Invoke("TimeOutFacebookLogin", 10f);
	}

	private void RemoveFacebookEventHandlers()
	{
		DIContainerInfrastructure.GetFacebookWrapper().logoutFailedEvent -= FacebookLogoutFailed;
		DIContainerInfrastructure.GetFacebookWrapper().logoutSucceededEvent -= FacebookLogoutSucceeded;
		DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent -= FacebookLoginSucceeded;
		DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent -= FacebookLoginFailed;
		CancelInvoke("TimeOutFacebookLogin");
	}

	private void EndFacebookOp()
	{
		RemoveFacebookEventHandlers();
		DIContainerInfrastructure.GetAsynchStatusService().HideLocalLoading();
	}

	private void FacebookLogoutSucceeded()
	{
		EndFacebookOp();
	}

	private void TimeOutFacebookLogin()
	{
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("social_facebook_login_failed", "Facebook login failed!"), "facebook", DispatchMessage.Status.Error);
		EndFacebookOp();
	}

	private void FacebookLogoutFailed()
	{
		EndFacebookOp();
	}

	public void StartRefreshFriends()
	{
		CancelInvoke("RefreshFriends");
		InvokeRepeating("RefreshFriends", 1f, m_friendRefreshCheckFrequency);
	}

	public void RefreshFriends()
	{
		if (DIContainerInfrastructure.GetCurrentPlayer() != null && DIContainerInfrastructure.GetFacebookWrapper().IsUserAuthenticated())
		{
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.GetFriendsSocialIds();
		}
	}

	public virtual void GoToBattleResultWon(bool isPvP)
	{
		if (!SceneLoadingMgr.IsLoading())
		{
			m_InfoOverlays.HideAllTooltips();
			SceneLoadingMgr.AddUILevel((!isPvP) ? "Popup_BattleWon" : "Popup_BattleWon_PvP");
		}
	}

	public virtual void GoToBattleResultLost(bool isPvP)
	{
		if (!SceneLoadingMgr.IsLoading())
		{
			m_InfoOverlays.HideAllTooltips();
			SceneLoadingMgr.AddUILevel((!isPvP) ? "Popup_BattleLost" : "Popup_BattleLost_PvP");
		}
	}

	public virtual void GoToSplashScreens()
	{
		SceneLoadingMgr.LoadGameScene("Menu_SplashScreens");
	}

	public virtual void GoToMainMenu()
	{
		SceneLoadingMgr.LoadGameScene("Menu_Main");
	}

	public virtual void GoToMainMenu(Action returnFromMainMentAction)
	{
		m_returnFromMainMenuAction = returnFromMainMentAction;
		SceneLoadingMgr.LoadGameScene("Menu_Main");
	}

	public virtual void GotoWorldMap()
	{
		try
		{
			List<IInventoryItemGameData> list = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.Story].Where((IInventoryItemGameData i) => i.ItemBalancing.NameId.StartsWith("comic_cutscene")).ToList();
			for (int j = 0; j < list.Count; j++)
			{
				IInventoryItemGameData inventoryItemGameData = list[j];
				if (inventoryItemGameData.ItemData.IsNew)
				{
					m_CurrentPendingStorySequence = inventoryItemGameData.ItemBalancing.NameId;
					m_ByGoToWorldMap = false;
					DebugLog.Log("Start Story Sequence: " + m_CurrentPendingStorySequence);
					SceneLoadingMgr.LoadGameScene(inventoryItemGameData.ItemBalancing.AssetBaseId, new List<string>());
					return;
				}
			}
		}
		catch
		{
		}
		if (!DIContainerInfrastructure.GetCurrentPlayer().Data.CinematricIntroStarted)
		{
			DIContainerLogic.WorldMapService.SetupCinematicBattle();
			Instance.GotoBattle(ClientInfo.CurrentBattleStartGameData.m_BackgroundAssetId);
			DIContainerInfrastructure.GetCurrentPlayer().Data.CinematricIntroStarted = true;
		}
		else
		{
			SceneLoadingMgr.LoadGameScene((!m_WorldMapGenerated) ? "WorldMap" : "WorldMap_Generated", new List<string>());
			DIContainerLogic.InventoryService.ReportDailyInventoryBalance();
		}
	}

	public virtual void GotoChronlicleCave()
	{
		SceneLoadingMgr.LoadGameScene("ChronicleCave", new List<string>());
	}

	public virtual void GoToMiniCampaign()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (DIContainerLogic.EventSystemService.IsCurrentEventAvailable(currentPlayer) && (DIContainerLogic.EventSystemService.IsWaitingForConfirmation(currentPlayer.CurrentEventManagerGameData) || !DIContainerLogic.EventSystemService.IsEventRunning(currentPlayer.CurrentEventManagerGameData)))
		{
			m_EventCampaign = false;
			GotoWorldMap();
		}
		else
		{
			SceneLoadingMgr.LoadGameScene("EventCampaign", new List<string>());
		}
	}

	public virtual void GotoCampScreen()
	{
		if (this.CampEntered != null)
		{
			this.CampEntered();
		}
		m_HotLinkItemName = null;
		m_ByGoToWorldMap = true;
		ClientInfo.IsFriend = false;
		ClientInfo.InspectedFriend = null;
		ClientInfo.CurrentCampBirds = DIContainerInfrastructure.GetCurrentPlayer().Birds;
		ClientInfo.CurrentCampInventory = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData;
		if (DIContainerInfrastructure.LocationStateMgr != null)
		{
			DIContainerInfrastructure.LocationStateMgr.WorldMenuUI.Leave();
		}
		DIContainerInfrastructure.GetCurrentPlayer().Data.HasNewOnWorlmap = false;
		SceneLoadingMgr.LoadGameScene("Camp", new List<string>());
	}

	public virtual void GotoPvpCampScreen()
	{
		if (this.CampEntered != null)
		{
			this.CampEntered();
		}
		m_HotLinkItemName = null;
		m_ByGoToWorldMap = true;
		ClientInfo.IsFriend = false;
		ClientInfo.InspectedFriend = null;
		ClientInfo.CurrentCampBirds = DIContainerInfrastructure.GetCurrentPlayer().Birds;
		ClientInfo.CurrentCampInventory = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData;
		ClientInfo.CurrentBanner = DIContainerInfrastructure.GetCurrentPlayer().BannerGameData;
		if (DIContainerInfrastructure.LocationStateMgr != null)
		{
			DIContainerInfrastructure.LocationStateMgr.WorldMenuUI.Leave();
		}
		DIContainerInfrastructure.GetCurrentPlayer().Data.HasNewOnWorlmap = false;
		DebugLog.Log(GetType(), "GotoPvpCampScreen - load game scene arena");
		SceneLoadingMgr.LoadGameScene("Arena", new List<string>());
		DebugLog.Log(GetType(), "GotoPvpCampScreen - load game scene arena done");
	}

	public virtual void GotoCampScreenViaHotlink(string hotlinkItemName, string shopEnterSource = "Standard")
	{
		if (this.CampEntered != null)
		{
			this.CampEntered();
		}
		ClientInfo.IsFriend = false;
		ClientInfo.InspectedFriend = null;
		ClientInfo.CurrentCampBirds = DIContainerInfrastructure.GetCurrentPlayer().Birds;
		ClientInfo.CurrentCampInventory = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData;
		DIContainerInfrastructure.GetCurrentPlayer().Data.HasNewOnWorlmap = false;
		m_HotLinkItemName = hotlinkItemName;
		m_ShopEnterSource = shopEnterSource;
		SceneLoadingMgr.LoadGameScene("Camp", new List<string>());
	}

	public virtual void GotoArenaScreenViaHotlink(string hotlinkItemName)
	{
		if (this.CampEntered != null)
		{
			this.CampEntered();
		}
		m_HotLinkItemName = hotlinkItemName;
		m_ByGoToWorldMap = true;
		ClientInfo.IsFriend = false;
		ClientInfo.InspectedFriend = null;
		ClientInfo.CurrentCampBirds = DIContainerInfrastructure.GetCurrentPlayer().Birds;
		ClientInfo.CurrentCampInventory = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData;
		ClientInfo.CurrentBanner = DIContainerInfrastructure.GetCurrentPlayer().BannerGameData;
		DIContainerInfrastructure.GetCurrentPlayer().Data.HasNewOnWorlmap = false;
		SceneLoadingMgr.LoadGameScene("Arena", new List<string>());
	}

	public virtual void GotoFirendArenaScreen(PublicPlayerData player, FriendGameData friend)
	{
		List<BirdGameData> list = new List<BirdGameData>();
		for (int i = 0; i < player.Birds.Count; i++)
		{
			BirdData instance = player.Birds[i];
			list.Add(new BirdGameData(instance));
		}
		m_HotLinkItemName = null;
		ClientInfo.IsFriend = true;
		ClientInfo.InspectedFriend = friend;
		ClientInfo.CurrentCampBirds = list;
		ClientInfo.CurrentCampInventory = new InventoryGameData(player.Inventory);
		ClientInfo.CurrentBanner = new BannerGameData(player.Banner);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
		ABHAnalyticsHelper.AddFriendsCountToTracking(dictionary);
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("FriendInspect", dictionary);
		SceneLoadingMgr.LoadGameScene("Arena_Friend", new List<string>());
	}

	public virtual void GotoFirendCampScreen(PublicPlayerData player, FriendGameData friend)
	{
		if (this.CampEntered != null)
		{
			this.CampEntered();
		}
		List<BirdGameData> list = new List<BirdGameData>();
		for (int i = 0; i < player.Birds.Count; i++)
		{
			BirdData instance = player.Birds[i];
			list.Add(new BirdGameData(instance));
		}
		m_HotLinkItemName = null;
		ClientInfo.IsFriend = true;
		ClientInfo.InspectedFriend = friend;
		ClientInfo.CurrentCampBirds = list;
		ClientInfo.CurrentCampInventory = new InventoryGameData(player.Inventory);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
		ABHAnalyticsHelper.AddFriendsCountToTracking(dictionary);
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("FriendInspect", dictionary);
		SceneLoadingMgr.LoadGameScene("Camp_Friend", new List<string>());
	}

	public void GotoIntro()
	{
		SceneLoadingMgr.LoadGameScene("VideoCutscene_01", new List<string>());
	}

	public virtual void GotoDefaultBattle()
	{
		SceneLoadingMgr.LoadGameScene("DefaultBattleground", new List<string>());
	}

	public virtual void GotoBattle(string battleGround)
	{
		SceneLoadingMgr.LoadGameScene(battleGround, new List<string>());
	}

	private IEnumerator LoadBattlegroundsAndGoToBattle(string battleGround)
	{
		if (string.IsNullOrEmpty(battleGround))
		{
			GotoDefaultBattle();
		}
		else
		{
			SceneLoadingMgr.LoadGameScene(battleGround, new List<string>());
		}
		yield break;
	}

	public virtual void ReturnFromBattle()
	{
		m_ByGoToWorldMap = true;
		ClientInfo.CurrentBattleGameData = null;
		if (m_ReturnToArena)
		{
			m_ReturnToArena = false;
			GotoPvpCampScreen();
		}
		else if (m_ReturnToCamp)
		{
			m_ReturnToCamp = false;
			GotoCampScreen();
		}
		else if (m_ChronicleCave)
		{
			GotoChronlicleCave();
		}
		else if (m_EventCampaign)
		{
			GoToMiniCampaign();
		}
		else
		{
			GotoWorldMap();
		}
	}

	public virtual ITutorialMgr InstantiateTutorialMgr()
	{
		DebugLog.Log("Instantiate new Tutorial Mgr");
		TutorialMgr tutorialMgr = UnityEngine.Object.Instantiate(m_TutorialMgr);
		UnityEngine.Object.DontDestroyOnLoad(tutorialMgr);
		return tutorialMgr;
	}

	public virtual ITutorialMgr InstantiateNullTutorialMgr()
	{
		TutorialMgrNullImpl tutorialMgrNullImpl = UnityEngine.Object.Instantiate(m_TutorialNullMgr);
		UnityEngine.Object.DontDestroyOnLoad(tutorialMgrNullImpl);
		return tutorialMgrNullImpl;
	}

	public void ShowDailyLoginUI()
	{
		m_DailyLoginUi.Show();
	}

	public void ShowDailyLoginUIOnStartUp()
	{
		if (DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "daily_chain_introduction") >= 1)
		{
			StartCoroutine("WaitForWorldMapToLoadAndShowDailyLogin");
		}
	}

	public void StopAutoDailyLoginPopup()
	{
		StopCoroutine("WaitForWorldMapToLoadAndShowDailyLogin");
	}

	private IEnumerator WaitForWorldMapToLoadAndShowDailyLogin()
	{
		if (!DIContainerInfrastructure.GetCurrentPlayer().m_UnlockDailyCalendarSessionFlag)
		{
			while (DIContainerInfrastructure.LocationStateMgr == null || DIContainerInfrastructure.LocationStateMgr.WorldMenuUI == null || !DIContainerInfrastructure.LocationStateMgr.WorldMenuUI.IsActive() || !m_AllowCalendar)
			{
				yield return new WaitForEndOfFrame();
			}
			m_DailyLoginUi.Show();
		}
	}

	public void ShowConfirmationPopup(string message, Action actionOnConfirm, Action actionOnAbort, bool autoClose = true, string atlasName = null, string spriteName = null)
	{
		SceneLoadingMgr.AddUILevel("Popup_Confirmation", delegate
		{
			SetConfirmationPopup(message, actionOnConfirm, actionOnAbort, autoClose, atlasName, spriteName);
		});
	}

	public void ShowConfirmationPopup(string message, Action actionOnConfirm)
	{
		m_GenericUI.LeaveNonInteractableTooltip();
		SceneLoadingMgr.AddUILevel("Popup_Confirmation", delegate
		{
			SetConfirmationPopup(message, actionOnConfirm);
		});
	}

	public void HideConfirmationPopup()
	{
		if ((bool)m_ConfirmationPopup)
		{
			m_ConfirmationPopup.Shutdown();
		}
	}

	private void SetConfirmationPopup(string message, Action actionOnConfirm, Action actionOnAbort, bool autoClose, string atlasName, string spriteName)
	{
		m_ConfirmationPopup = UnityEngine.Object.FindObjectOfType(typeof(ConfirmationPopupController)) as ConfirmationPopupController;
		if (m_ConfirmationPopup != null)
		{
			m_ConfirmationPopup.SetActions(actionOnConfirm, actionOnAbort).SetMessage(message, autoClose);
			if (atlasName != null && spriteName != null)
			{
				m_ConfirmationPopup.SetConfirmIcon(atlasName, spriteName);
			}
			m_ConfirmationPopup.Enter();
		}
		else
		{
			DebugLog.Error("[CoreStateMgr] Confirmation Popup not found");
		}
	}

	private void SetConfirmationPopup(string message, Action actionOnConfirm)
	{
		m_ConfirmationPopup = UnityEngine.Object.FindObjectOfType(typeof(ConfirmationPopupController)) as ConfirmationPopupController;
		if (m_ConfirmationPopup != null)
		{
			m_ConfirmationPopup.SetActions(actionOnConfirm, null).SetMessage(message);
			m_ConfirmationPopup.m_AbortButton.gameObject.SetActive(false);
			m_ConfirmationPopup.Enter();
		}
		else
		{
			DebugLog.Error("[CoreStateMgr] Confirmation Popup not found");
		}
	}

	public void RefreshConfirmationPopup(string message, Action actionOnConfirm, Action actionOnAbort, bool autoClose = true)
	{
		if ((bool)m_ConfirmationPopup)
		{
			m_ConfirmationPopup.SetActions(actionOnConfirm, actionOnAbort).SetMessage(message, autoClose);
		}
	}

	public void RegisterPopupEntered(bool entered)
	{
		m_PopupEntered = entered;
		if (this.OnPopupEnter != null)
		{
			this.OnPopupEnter(entered);
		}
	}

	public void RegisterShopClosed()
	{
		if (this.OnShopClosed != null)
		{
			this.OnShopClosed();
		}
	}

	public void CloseConfirmationPopup()
	{
		if ((bool)m_ConfirmationPopup)
		{
			m_ConfirmationPopup.Shutdown();
		}
	}

	public bool IsShopOpen()
	{
		return m_ShopWindow != null;
	}

	public bool IsShopLoading()
	{
		return m_ShopIsLoading;
	}

	public void LeaveShop()
	{
		if (IsShopOpen() && (bool)m_ShopWindow)
		{
			m_ShopWindow.Leave();
		}
		if (m_ShopIsLoading)
		{
			m_LeaveShopAfterLoading = true;
		}
	}

	public void ShowShop(string category, Action reEnterAction, int startIndex = 0, bool arena = false, string enterSource = "Standard")
	{
		if (DIContainerInfrastructure.GetCoreStateMgr().m_GameIsPaused || m_ShopIsLoading || m_FriendListIsLoading)
		{
			return;
		}
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameter("ShopEntered", "ShopCategory", category);
		if (IsShopOpen())
		{
			m_ShopWindow.SetStartScrollIndex(startIndex);
			m_ShopWindow.SetCategory(category);
			return;
		}
		m_ShopIsLoading = true;
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("shop_loading");
		SceneLoadingMgr.AddUILevel("Window_Shop", delegate
		{
			SetShopCategory(category, reEnterAction, enterSource, startIndex);
		});
		DIContainerInfrastructure.GetAsynchStatusService().ShowLocalLoading("Loading..", false);
	}

	private void SetShopCategory(string category, Action reEnterAction, string enterSource, int startIndex = 0)
	{
		m_ShopWindow = UnityEngine.Object.FindObjectOfType(typeof(ShopWindowStateMgr)) as ShopWindowStateMgr;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("shop_loading");
		DIContainerInfrastructure.GetAsynchStatusService().HideLocalLoading();
		if ((m_LeaveShopAfterLoading || SceneLoadingMgr.IsLoading()) && m_ShopWindow != null)
		{
			m_LeaveShopAfterLoading = false;
			m_ShopIsLoading = false;
			UnityEngine.Object.Destroy(m_ShopWindow.gameObject);
		}
		else
		{
			m_ShopWindow.SetStartScrollIndex(startIndex);
			m_ShopWindow.SetCategory(category, false).SetReEnterAction(reEnterAction);
			m_ShopWindow.Enter(enterSource);
			m_ShopIsLoading = false;
		}
	}

	public void ShowFriendList(FriendListType type, Action reEnterAction, Action<FriendGameData> selectFriendAction, FriendGameData selectedFriend)
	{
		if (!DIContainerInfrastructure.GetCoreStateMgr().m_GameIsPaused && !m_ShopIsLoading)
		{
			m_FriendListIsLoading = true;
			SceneLoadingMgr.AddUILevel("Window_BirdFromFriend", delegate
			{
				SetFriendList(type, reEnterAction, selectFriendAction, selectedFriend);
			});
			DIContainerInfrastructure.GetAsynchStatusService().ShowLocalLoading(DIContainerInfrastructure.GetLocaService().Tr("loadingscreen_loading", "Loading..."), true);
		}
	}

	private void SetFriendList(FriendListType type, Action reEnterAction, Action<FriendGameData> selectFriendAction, FriendGameData selectedFriend)
	{
		m_FriendListWindow = UnityEngine.Object.FindObjectOfType(typeof(FriendBirdWindowStateMgr)) as FriendBirdWindowStateMgr;
		DIContainerInfrastructure.GetAsynchStatusService().HideLocalLoading();
		m_FriendListWindow.SetReEnterAction(reEnterAction).SetSelectFriendAction(selectFriendAction).SetType(type);
		if (selectedFriend != null)
		{
			m_FriendListWindow.SetSelectedFriend(selectedFriend.FriendId);
		}
		m_FriendListIsLoading = false;
		m_FriendListWindow.Enter();
	}

	public void UnloadUnusedAssets()
	{
		if ((bool)DIContainerInfrastructure.GetEquipmentAssetProvider())
		{
			DIContainerInfrastructure.GetEquipmentAssetProvider().RemoveAssetLinks();
		}
		if ((bool)DIContainerInfrastructure.GetCharacterAssetProvider(true))
		{
			DIContainerInfrastructure.GetCharacterAssetProvider(true).RemoveAssetLinks();
		}
		if ((bool)DIContainerInfrastructure.GetCharacterAssetProvider(false))
		{
			DIContainerInfrastructure.GetCharacterAssetProvider(false).RemoveAssetLinks();
		}
		if ((bool)DIContainerInfrastructure.GetClassAssetProvider())
		{
			DIContainerInfrastructure.GetClassAssetProvider().RemoveAssetLinks();
		}
		if ((bool)DIContainerInfrastructure.GetBattleEffectAssetProvider())
		{
			DIContainerInfrastructure.GetBattleEffectAssetProvider().RemoveAssetLinks();
		}
		if ((bool)DIContainerInfrastructure.GetGenericIconAtlasAssetProvider())
		{
			DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().RemoveAssetLinks();
		}
		DIContainerInfrastructure.UnloadAudioAssetProviders();
		if ((bool)DIContainerInfrastructure.PropLiteAssetProvider())
		{
			DIContainerInfrastructure.PropLiteAssetProvider().RemoveAssetLinks();
		}
		if ((bool)DIContainerInfrastructure.ProjectileAssetProvider)
		{
			DIContainerInfrastructure.ProjectileAssetProvider.RemoveAssetLinks();
		}
		if ((bool)DIContainerInfrastructure.GetComicCutsceneAssetProvider())
		{
			DIContainerInfrastructure.GetComicCutsceneAssetProvider().RemoveAssetLinks();
		}
		if ((bool)DIContainerInfrastructure.GetBannerAssetProvider())
		{
			DIContainerInfrastructure.GetBannerAssetProvider().RemoveAssetLinks();
		}
	}

	public IEnumerator UnloadUnusedAssetsCoroutine()
	{
		UnloadUnusedAssets();
		yield return Resources.UnloadUnusedAssets();
		DebugLog.Log("Unloaded Assets!");
	}

	public IEnumerator UnloadUnusedAssetsAndGCCollectCoroutine()
	{
		yield return new WaitForEndOfFrame();
		if ((bool)DIContainerInfrastructure.GetEquipmentAssetProvider())
		{
			DIContainerInfrastructure.GetEquipmentAssetProvider().RemoveAssetLinks();
		}
		if ((bool)DIContainerInfrastructure.GetCharacterAssetProvider(true))
		{
			DIContainerInfrastructure.GetCharacterAssetProvider(true).RemoveAssetLinks();
		}
		if ((bool)DIContainerInfrastructure.GetCharacterAssetProvider(false))
		{
			DIContainerInfrastructure.GetCharacterAssetProvider(false).RemoveAssetLinks();
		}
		if ((bool)DIContainerInfrastructure.GetClassAssetProvider())
		{
			DIContainerInfrastructure.GetClassAssetProvider().RemoveAssetLinks();
		}
		if ((bool)DIContainerInfrastructure.GetBattleEffectAssetProvider())
		{
			DIContainerInfrastructure.GetBattleEffectAssetProvider().RemoveAssetLinks();
		}
		if ((bool)DIContainerInfrastructure.GetGenericIconAtlasAssetProvider())
		{
			DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().RemoveAssetLinks();
		}
		DIContainerInfrastructure.UnloadAudioAssetProviders();
		if ((bool)DIContainerInfrastructure.PropLiteAssetProvider())
		{
			DIContainerInfrastructure.PropLiteAssetProvider().RemoveAssetLinks();
		}
		if ((bool)DIContainerInfrastructure.ProjectileAssetProvider)
		{
			DIContainerInfrastructure.ProjectileAssetProvider.RemoveAssetLinks();
		}
		yield return Resources.UnloadUnusedAssets();
		DebugLog.Log("Unloaded Files");
		GC.Collect();
	}

	public void DelayedGotoWorldMapOrIntro()
	{
		StartCoroutine(DelayedGotoWorldMapCoroutine());
	}

	private IEnumerator DelayedGotoWorldMapCoroutine()
	{
		yield return new WaitForEndOfFrame();
		while (!m_isInitialized || SceneLoadingMgr.IsLoading())
		{
			yield return new WaitForEndOfFrame();
		}
		if (DIContainerInfrastructure.TimeScaleMgr != null)
		{
			DIContainerInfrastructure.TimeScaleMgr.ResetTimeScales();
		}
		yield return new WaitForEndOfFrame();
		GotoWorldMap();
	}

	public void ResetProfileAfterMessage(PlayerData remotePlayerData)
	{
		StartCoroutine(ResetProfileAfterMessageCoroutine(remotePlayerData));
	}

	private IEnumerator ResetProfileAfterMessageCoroutine(PlayerData remotePlayerData)
	{
		DIContainerInfrastructure.ResetWithNewProfile(remotePlayerData);
		DIContainerInfrastructure.GetCoreStateMgr().m_CronJob.DeRegisterCronJobBlockReason("app_resume");
		DIContainerInfrastructure.RemoteStorageService.EnableProfileSync("SyncProfileAndGetConflictedProfile");
		RegisterPopupEntered(true);
		yield return new WaitForSeconds(1f);
		while (SceneLoadingMgr == null || SceneLoadingMgr.IsLoading())
		{
			yield return new WaitForEndOfFrame();
		}
		DIContainerInfrastructure.GetAsynchStatusService().RemoveLastMessageAndDisplayNext();
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_profile_synch_executing", "Your device has been synchronized. The latest game progress is available."), "synch_executing", DispatchMessage.Status.Info);
		yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().ToasterTime);
	}

	private void ReportInstallTime()
	{
		long installedTimeSecondsUTC = DIContainerInfrastructure.GetSystemInfoService().GetInstalledTimeSecondsUTC();
		if (installedTimeSecondsUTC == 0L)
		{
			DebugLog.ForceWarn(GetType(), "ReportInstallTime: Not reporting because install time reported 0");
			return;
		}
		DebugLog.ForceLog(GetType(), "ReportInstallTime - reported " + installedTimeSecondsUTC);
		string text = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(installedTimeSecondsUTC).ToLocalTime().ToString("o", CultureInfo.InvariantCulture);
		DebugLog.ForceLog(GetType(), string.Format("ReportInstallTime {0}", text));
		DIContainerInfrastructure.GetAnalyticsSystem(true).LogEventWithParameter("InstallTime", "InstallTime", text);
	}

	internal void EvalulateAndShowHintPopup(BasicItemGameData basicItemGameData)
	{
		switch (basicItemGameData.BalancingData.NameId)
		{
		case "hint_defeat_cabinboy":
		{
			Requirement requirement = new Requirement();
			requirement.NameId = "class_samurai";
			requirement.RequirementType = RequirementType.NotHaveClass;
			requirement.Value = 0f;
			Requirement req3 = requirement;
			if (DIContainerLogic.RequirementService.CheckRequirement(DIContainerInfrastructure.GetCurrentPlayer(), req3))
			{
				basicItemGameData.BalancingData.LocaBaseId += "_01";
				StartCoroutine(m_SpecialOfferPopup.ShowSpecialOfferPopup(basicItemGameData, "samurai_shop").Run());
				break;
			}
			BirdGameData bird3 = DIContainerInfrastructure.GetCurrentPlayer().GetBird("bird_red");
			if (bird3 != null && DIContainerLogic.RequirementService.CheckRequirement(bird3.InventoryGameData, req3))
			{
				basicItemGameData.BalancingData.LocaBaseId += "_02";
				StartCoroutine(m_SpecialOfferPopup.ShowSpecialOfferPopup(basicItemGameData, "samurai_equip").Run());
			}
			else
			{
				basicItemGameData.BalancingData.LocaBaseId += "_03";
				StartCoroutine(m_FeatureUnlockedPopup.ShowUnlockFeaturePopup(basicItemGameData).Run());
			}
			break;
		}
		case "hint_lightningbird":
		case "hint_defeat_monty":
		{
			Requirement requirement = new Requirement();
			requirement.NameId = "class_lightningbird";
			requirement.RequirementType = RequirementType.NotHaveClass;
			requirement.Value = 0f;
			Requirement req4 = requirement;
			if (DIContainerLogic.RequirementService.CheckRequirement(DIContainerInfrastructure.GetCurrentPlayer(), req4))
			{
				basicItemGameData.BalancingData.LocaBaseId += "_01";
				StartCoroutine(m_SpecialOfferPopup.ShowSpecialOfferPopup(basicItemGameData, "lightningbird_shop").Run());
				break;
			}
			BirdGameData bird4 = DIContainerInfrastructure.GetCurrentPlayer().GetBird("bird_yellow");
			if (bird4 != null && DIContainerLogic.RequirementService.CheckRequirement(bird4.InventoryGameData, req4))
			{
				basicItemGameData.BalancingData.LocaBaseId += "_02";
				StartCoroutine(m_SpecialOfferPopup.ShowSpecialOfferPopup(basicItemGameData, "lightningbird_equip").Run());
			}
			else
			{
				basicItemGameData.BalancingData.LocaBaseId += "_03";
				StartCoroutine(m_FeatureUnlockedPopup.ShowUnlockFeaturePopup(basicItemGameData).Run());
			}
			break;
		}
		case "hint_defeat_porky_ship":
		{
			Requirement requirement = new Requirement();
			requirement.NameId = "class_samurai";
			requirement.RequirementType = RequirementType.NotHaveClass;
			requirement.Value = 0f;
			Requirement req = requirement;
			requirement = new Requirement();
			requirement.NameId = "class_druid";
			requirement.RequirementType = RequirementType.NotHaveClass;
			requirement.Value = 0f;
			Requirement req2 = requirement;
			if (DIContainerLogic.RequirementService.CheckRequirement(DIContainerInfrastructure.GetCurrentPlayer(), req) && DIContainerLogic.RequirementService.CheckRequirement(DIContainerInfrastructure.GetCurrentPlayer(), req2))
			{
				basicItemGameData.BalancingData.LocaBaseId += "_01";
				StartCoroutine(m_SpecialOfferPopup.ShowSpecialOfferPopup(basicItemGameData, "samurai_shop").Run());
				break;
			}
			BirdGameData bird = DIContainerInfrastructure.GetCurrentPlayer().GetBird("bird_red");
			BirdGameData bird2 = DIContainerInfrastructure.GetCurrentPlayer().GetBird("bird_white");
			if (bird != null && DIContainerLogic.RequirementService.CheckRequirement(bird.InventoryGameData, req) && bird2 != null && DIContainerLogic.RequirementService.CheckRequirement(bird2.InventoryGameData, req2))
			{
				basicItemGameData.BalancingData.LocaBaseId += "_02";
				StartCoroutine(m_SpecialOfferPopup.ShowSpecialOfferPopup(basicItemGameData, "samurai_equip").Run());
			}
			else
			{
				basicItemGameData.BalancingData.LocaBaseId += "_03";
				StartCoroutine(m_FeatureUnlockedPopup.ShowUnlockFeaturePopup(basicItemGameData).Run());
			}
			break;
		}
		default:
			StartCoroutine(m_FeatureUnlockedPopup.ShowUnlockFeaturePopup(basicItemGameData).Run());
			break;
		}
	}

	public void ShowClassInfoUi(ClassItemGameData classItem, SkinItemGameData skinItem = null)
	{
		m_cachedClassItem = classItem;
		m_cachedSkinItem = skinItem;
		if (m_ClassInfoUI == null)
		{
			SceneLoadingMgr.AddUILevel("Popup_ClassInfo", OnClassInfoLoaded);
		}
		else
		{
			m_ClassInfoUI.Show(classItem, skinItem);
		}
	}

	private void OnClassInfoLoaded()
	{
		m_ClassInfoUI = UnityEngine.Object.FindObjectOfType(typeof(ClassInfoPopup)) as ClassInfoPopup;
		m_ClassInfoUI.gameObject.SetActive(false);
		m_ClassInfoUI.Show(m_cachedClassItem, m_cachedSkinItem);
	}

	private void ShowAndSendLoadingTracking()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (string key in ClientInfo.LoadingTracking.Keys)
		{
			float num = ClientInfo.LoadingTracking[key];
			dictionary.Add(key, num.ToString("F1"));
			DebugLog.Log(GetType(), string.Format("Loading time for '{0}' was {1} seconds.", key, num));
		}
		DIContainerInfrastructure.GetAnalyticsSystem(true).LogEventWithParameters("StartupLoading", dictionary);
	}

	public void FixArenaSeason()
	{
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.PvpSeasonManager != null && DIContainerBalancing.EventBalancingService.GetBalancingData<PvPSeasonManagerBalancingData>(DIContainerInfrastructure.GetCurrentPlayer().Data.PvpSeasonManager.NameId) != null)
		{
			return;
		}
		try
		{
			if (DIContainerLogic.PvPSeasonService == null || DIContainerBalancing.EventBalancingService.GetBalancingDataList<PvPSeasonManagerBalancingData>() == null)
			{
				return;
			}
		}
		catch
		{
			return;
		}
		PvPSeasonManagerBalancingData pvPSeasonManagerBalancingData = DIContainerBalancing.EventBalancingService.GetBalancingDataList<PvPSeasonManagerBalancingData>()[0];
		int currentSeasonTurn = DIContainerLogic.PvPSeasonService.GetCurrentSeasonTurn(pvPSeasonManagerBalancingData);
		DIContainerInfrastructure.GetCurrentPlayer().Data.PvpSeasonManager = new PvPSeasonManagerData();
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.PvpSeasonManager.CurrentSeasonTurn == null)
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.PvpSeasonManager.CurrentSeasonTurn = new PvPTurnManagerData();
		}
		DIContainerInfrastructure.GetCurrentPlayer().Data.PvpSeasonManager.NameId = "pvp_season_01";
		DIContainerInfrastructure.GetCurrentPlayer().Data.PvpSeasonManager.CurrentLeague = 1;
		DIContainerInfrastructure.GetCurrentPlayer().Data.PvpSeasonManager.CurrentSeason = currentSeasonTurn;
		DIContainerInfrastructure.GetCurrentPlayer().Data.PvpSeasonManager.CurrentSeasonState = PvPSeasonState.Running;
		DIContainerInfrastructure.GetCurrentPlayer().Data.PvpSeasonManager.CurrentSeasonTurn.NameId = "pvp_season_01_turn_" + currentSeasonTurn.ToString("00");
		DIContainerInfrastructure.GetCurrentPlayer().Data.PvpSeasonManager.CurrentSeasonTurn.CurrentSeason = currentSeasonTurn;
		DIContainerInfrastructure.GetCurrentPlayer().Data.PvpSeasonManager.CurrentSeasonTurn.CurrentState = EventManagerState.Running;
		DIContainerInfrastructure.GetCurrentPlayer().Data.PvpSeasonManager.CurrentSeasonTurn.CachedRolledResultWheelIndex = -1;
		DIContainerInfrastructure.GetCurrentPlayer().Data.m_CachedSeasonName = DIContainerInfrastructure.GetLocaService().Tr(pvPSeasonManagerBalancingData.LocaBaseId + "_name", "Season 1");
		PvPSeasonManagerGameData pvPSeasonManagerGameData = DIContainerLogic.PvPSeasonService.StartNewSeason(pvPSeasonManagerBalancingData, DIContainerInfrastructure.GetCurrentPlayer());
		if (pvPSeasonManagerGameData != null)
		{
			DIContainerLogic.PvPSeasonService.StartNewPvPTurn(pvPSeasonManagerGameData, DIContainerInfrastructure.GetCurrentPlayer());
		}
	}

	public bool IsEventsUnlocked()
	{
		bool flag = false;
		foreach (BasicItemGameData item in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.Story])
		{
			if (item.BalancingData.NameId == "unlock_events")
			{
				return true;
			}
		}
		return false;
	}

	public void HandleAndReport(string condition, string stackTrace, LogType type)
	{
		if (type == LogType.Exception)
		{
			StartCoroutine(ContentLoader.Instance.SubmitExceptionReport("{\"condition\":\"" + condition.Replace("\n", "\\n") + "\",\"stackTrace\":\"" + stackTrace.Replace("\n", "\\n") + "\"}"));
		}
	}

	public void DoInvalidateNicknameThenReset()
	{
		string eventPlayerName = DIContainerInfrastructure.GetCurrentPlayer().Data.SocialEnvironment.EventPlayerName;
		if (string.IsNullOrEmpty(eventPlayerName))
		{
			return;
		}
		PopupEnterName popupEnterName = new PopupEnterName();
		if (popupEnterName.ContainsSymbol(eventPlayerName))
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.SocialEnvironment.EventPlayerName = null;
			if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data != null)
			{
				DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.EventPlayerName = null;
			}
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			return;
		}
		StartCoroutine(popupEnterName.TranslateNickname(eventPlayerName, delegate
		{
			if (!string.IsNullOrEmpty(popupEnterName.m_TranslatedNickname))
			{
				popupEnterName.ForceValidateNickname(eventPlayerName, null, delegate
				{
					DIContainerInfrastructure.GetCurrentPlayer().Data.SocialEnvironment.EventPlayerName = null;
					if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data != null)
					{
						DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.EventPlayerName = null;
					}
					DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
				});
			}
		}));
	}

	public void SaveSocialAvatar(WWW www, string accountOrSocialId)
	{
		if (www == null || !www.isDone || www.texture == null || www.bytes == null || HasValidAvatarCopy(accountOrSocialId, www.bytes))
		{
			return;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["width"] = www.texture.width.ToString();
		dictionary["height"] = www.texture.height.ToString();
		dictionary["avatar"] = Convert.ToBase64String(www.bytes);
		if (!string.IsNullOrEmpty(dictionary["avatar"]))
		{
			string text = Path.Combine(Application.persistentDataPath, "cachedAvatars");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			string value = SimpleJsonConverter.EncodeDictToJson(dictionary);
			StreamWriter streamWriter = new StreamWriter(Path.Combine(text, string.IsNullOrEmpty(accountOrSocialId) ? "default_avatar.json" : (accountOrSocialId + "_avatar.json")));
			streamWriter.Write(value);
			streamWriter.Flush();
			streamWriter.Close();
		}
	}

	public Texture2D LoadSocialAvatar(string accountOrSocialId)
	{
		string text = Path.Combine(Application.persistentDataPath, "cachedAvatars");
		string path = Path.Combine(text, string.IsNullOrEmpty(accountOrSocialId) ? "default_avatar.json" : (accountOrSocialId + "_avatar.json"));
		if (!Directory.Exists(text) || !File.Exists(path) || File.Exists(text))
		{
			return null;
		}
		StreamReader streamReader = new StreamReader(path);
		string text2 = streamReader.ReadToEnd();
		streamReader.Close();
		if (string.IsNullOrEmpty(text2))
		{
			return null;
		}
		Dictionary<string, string> dictionary = null;
		try
		{
			dictionary = SimpleJsonConverter.DecodeJsonDictString(text2);
		}
		catch
		{
			return null;
		}
		if (!dictionary.ContainsKey("width") || !dictionary.ContainsKey("height") || !dictionary.ContainsKey("avatar"))
		{
			return null;
		}
		Texture2D texture2D = new Texture2D(Convert.ToInt32(dictionary["width"]), Convert.ToInt32(dictionary["height"]));
		if (!texture2D.LoadImage(Convert.FromBase64String(dictionary["avatar"])))
		{
			return null;
		}
		return texture2D;
	}

	private bool HasValidAvatarCopy(string accountOrSocialId, byte[] bytes)
	{
		string text = Path.Combine(Application.persistentDataPath, "cachedAvatars");
		string path = Path.Combine(text, string.IsNullOrEmpty(accountOrSocialId) ? "default_avatar.json" : (accountOrSocialId + "_avatar.json"));
		if (!Directory.Exists(text) || !File.Exists(path) || File.Exists(text) || bytes == null)
		{
			return false;
		}
		uint num = 0u;
		for (int i = 0; i < bytes.Length; i++)
		{
			num += bytes[i];
		}
		StreamReader streamReader = new StreamReader(path);
		string json = streamReader.ReadToEnd();
		streamReader.Close();
		Dictionary<string, string> dictionary = null;
		byte[] array = null;
		try
		{
			dictionary = SimpleJsonConverter.DecodeJsonDictString(json);
			if (!dictionary.ContainsKey("avatar"))
			{
				return false;
			}
			array = Convert.FromBase64String(dictionary["avatar"]);
		}
		catch
		{
			return false;
		}
		uint num2 = 0u;
		for (int j = 0; j < array.Length; j++)
		{
			num2 += array[j];
		}
		if (num != num2)
		{
			return false;
		}
		return true;
	}

	static CoreStateMgr()
	{
		SharedIds = new Dictionary<string, string>();
	}
	
	internal void UpdateSharedIds(PublicPlayerData ppd, string sharedId)
	{
		string key = (string.IsNullOrEmpty(ppd.EventPlayerName) ? string.Empty : (ppd.EventPlayerName + "_")) + ppd.Level + "_" + ppd.LastSaveTime;
		for (int i = 0; i < SharedIds.Count; i++)
		{
			if (SharedIds.ElementAt(i).Value == sharedId)
			{
				SharedIds.Remove(SharedIds.ElementAt(i).Key);
			}
		}
		SharedIds[key] = sharedId;
	}

	internal void UpdateSharedIds()
	{
		PublicPlayerData publicPlayer = DIContainerInfrastructure.GetCurrentPlayer().PublicPlayer;
		if (publicPlayer == null)
		{
			return;
		}
		string key = (string.IsNullOrEmpty(publicPlayer.EventPlayerName) ? string.Empty : (publicPlayer.EventPlayerName + "_")) + publicPlayer.Level + "_" + publicPlayer.LastSaveTime;
		for (int i = 0; i < SharedIds.Count; i++)
		{
			if (SharedIds.ElementAt(i).Value == DIContainerInfrastructure.IdentityService.SharedId)
			{
				SharedIds.Remove(SharedIds.ElementAt(i).Key);
			}
		}
		SharedIds[key] = DIContainerInfrastructure.IdentityService.SharedId;
	}

	internal string GetSharedId(PublicPlayerData ppd)
	{
		string key = (string.IsNullOrEmpty(ppd.EventPlayerName) ? string.Empty : (ppd.EventPlayerName + "_")) + ppd.Level + "_" + ppd.LastSaveTime;
		if (SharedIds.ContainsKey(key))
		{
			return SharedIds[key];
		}
		return null;
	}
}

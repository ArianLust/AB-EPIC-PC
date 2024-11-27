using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using Prime31;
using UnityEngine;

public class WorldMapStateMgr : BaseLocationStateManager
{
	public HotSpotWorldMapViewBase m_startingHotSpot;

	public HotSpotWorldMapViewBase m_dojoHotSpot;

	public Transform m_CamRoot;

	public Vector3 m_WorldBirdScale = new Vector3(0.4f, 0.4f, 1f);

	public GameObject m_WorldMapCharacterController;

	public GenericSpeechBubble m_SpeechBubble;

	public GameObject m_GoldenPigSpawnEffectPrefab;

	[SerializeField]
	private List<HotSpotWorldMapViewBase> m_DungeonHotSpots;

	[SerializeField]
	private HotSpotWorldMapViewGoldenPig m_GoldenPigHotspotView;

	[SerializeField]
	private Transform m_GoldenPigRoot;

	[SerializeField]
	private List<MissingItemToPrefabMapping> m_MissingBubbles = new List<MissingItemToPrefabMapping>();

	public List<Animation> m_BirdAnimations;

	public float[] m_movementStartDelay;

	public float m_BirdSpeed = 100f;

	public Vector3[] m_HotSpotPositions;

	public List<GameObject> m_ZoneCloudings = new List<GameObject>();

	private HotSpotWorldMapViewBase m_currentHotSpot;

	private List<HotSpotWorldMapViewBase> m_currentPathList = new List<HotSpotWorldMapViewBase>();

	private bool[] m_walking;

	private bool m_inputEnabled = true;

	public LayerMask EventMask = -1;

	private Dictionary<string, bool> m_LoadedLevels = new Dictionary<string, bool>();

	public Transform m_CharacterRoot;

	[HideInInspector]
	public BattlePreperationUI m_battlePreperation;

	[HideInInspector]
	public FriendshipGateUI m_friendShipGate;

	[HideInInspector]
	public WorldMapShopMenuUI m_WorkShopUI;

	[HideInInspector]
	public WorldMapMenuUI m_WorldMenuUI;

	[HideInInspector]
	public EventDetailUI m_EventDetailUI;

	[HideInInspector]
	private LeaderboardUI m_LeaderBoardUI;

	[HideInInspector]
	public EventPreviewUI m_EventPreviewUI;

	[HideInInspector]
	public NewsUi m_EventNews;

	[HideInInspector]
	public NewsLocked m_NewsLockedPopup;

	[HideInInspector]
	public NewsLogic m_NewsLogic;

	[HideInInspector]
	public BonusCodeManager m_BonusCodeManager;

	[SerializeField]
	private GameObject m_Ship;

	[SerializeField]
	private Animation m_ShipAnimation;

	[SerializeField]
	private GameObject m_AirShip;

	[SerializeField]
	private Animation m_AirShipAnimation;

	[SerializeField]
	private GameObject m_Submarine;

	[SerializeField]
	private Animation m_m_SubmarineAnimation;

	[SerializeField]
	private float m_CheckForButtonMarkerFrequency = 10f;

	[SerializeField]
	private GameObject m_DamagedCampFx;

	[SerializeField]
	private OpponentInfoElement m_FlyingFriendIcon;

	[SerializeField]
	private Camera m_WorldCam;

	private BattleMgr m_BattleMgr;

	private Action m_ActionAfterWalkingDone;

	public bool m_ProgressIndicatorBlocked;

	private Dictionary<int, FriendProgressIndicator> m_FriendProgresses = new Dictionary<int, FriendProgressIndicator>();

	[HideInInspector]
	public Func<bool> m_isMovementPossible;

	private ShopBalancingData m_cachedShopBalancing;

	private ShopMenuType m_cachedShopType;

	private HotSpotWorldMapViewBase m_cachedHotSpotMapView;

	private HotspotGameData m_cachedHotSpot;

	private EventItemGameData m_cachedEventItem;

	private EventPlacementBalancingData m_cachedEventPlacement;

	private Action m_cachedAction;

	private NewsUi.NewsUiState m_cachedNewsStartingState;

	private float m_secondsSinceLoad;

	private bool m_Left;

	private DateTime lastPressTime = DateTime.MinValue;

	private double m_PushOnlyDelayOnBackButton = 10.0;

	public override IMapUI WorldMenuUI
	{
		get
		{
			return m_WorldMenuUI;
		}
	}

	private void Awake()
	{
		DIContainerInfrastructure.AdService.AddPlacement(BattlePreperationUI.BUFF_PLACEMENT);
		DIContainerInfrastructure.GetCoreStateMgr().m_ChronicleCave = false;
		DIContainerInfrastructure.GetCoreStateMgr().m_EventCampaign = false;
		DIContainerInfrastructure.GetCoreStateMgr().m_SceneryAudioListener = base.transform.GetComponentInChildren<AudioListener>();
		DIContainerInfrastructure.GetCoreStateMgr().m_IsWithinPvP = false;
		HotSpotWorldMapViewBase[] componentsInChildren = base.transform.GetComponentsInChildren<HotSpotWorldMapViewBase>(true);
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_LeaderBoard_Tabs", OnLeaderBoardLoaded);
		m_FeatureUnlockCoroutineInstance = HandleFeatureUnlocksAndLevelUps();
		DIContainerInfrastructure.GetPowerLevelCalculator();
		SynchBalancing(componentsInChildren);
		if (ContentLoader.Instance != null)
		{
			ContentLoader.Instance.SetDownloadProgress(0.625f);
		}
		for (int i = 0; i < Camera.allCameras.Length; i++)
		{
			Camera camera = Camera.allCameras[i];
			camera.eventMask = EventMask;
		}
		foreach (HotSpotWorldMapViewBase hotSpotWorldMapViewBase in componentsInChildren)
		{
			hotSpotWorldMapViewBase.Initialize();
		}
		if (!DIContainerInfrastructure.GetCoreStateMgr().m_EnterOnce)
		{
			SetNextProgressNodeActive();
		}
		if (DIContainerInfrastructure.GetCoreStateMgr().m_ZoomToDungeonId < m_DungeonHotSpots.Count)
		{
			m_currentHotSpot = m_DungeonHotSpots[DIContainerInfrastructure.GetCoreStateMgr().m_ZoomToDungeonId];
		}
		if (DIContainerInfrastructure.GetCoreStateMgr().m_GoToDojo)
		{
			m_currentHotSpot = (DIContainerInfrastructure.LocationStateMgr as WorldMapStateMgr).m_dojoHotSpot;
		}
		LoadBirdsIntoScene();
		m_CamRoot.transform.position = new Vector3(m_currentHotSpot.transform.position.x, m_currentHotSpot.transform.position.y, m_CamRoot.transform.position.z);
		base.SceneryCamera = m_CamRoot.GetComponentInChildren<Camera>();
		if (m_currentHotSpot.Model.Data.UnlockState == HotspotUnlockState.ResolvedNew || !m_startingHotSpot.IsCompleted())
		{
			ExecuteActionTree component = m_currentHotSpot.GetComponent<ExecuteActionTree>();
			if ((bool)component)
			{
				for (int k = 0; k < component.m_ActionTree.m_PreInstantiatedCharacterAssetIds.Count; k++)
				{
					string nameId = component.m_ActionTree.m_PreInstantiatedCharacterAssetIds[k];
					DIContainerInfrastructure.GetCharacterAssetProvider(true).PreCacheObject(nameId);
				}
			}
		}
		if (ContentLoader.Instance != null)
		{
			ContentLoader.Instance.CheckforRestartApp();
			ContentLoader.Instance.SetDownloadProgress(0.75f);
		}
	}

	public void ZoomToDungeon()
	{
		if (DIContainerInfrastructure.GetCoreStateMgr().m_ZoomToDungeonId < m_DungeonHotSpots.Count)
		{
			HotSpotWorldMapViewBase hotSpotWorldMapViewBase = m_DungeonHotSpots[DIContainerInfrastructure.GetCoreStateMgr().m_ZoomToDungeonId];
			m_CamRoot.transform.position = new Vector3(hotSpotWorldMapViewBase.transform.position.x, hotSpotWorldMapViewBase.transform.position.y, m_CamRoot.transform.position.z);
			SetNewHotSpot(hotSpotWorldMapViewBase);
		}
	}

	public void ZoomToDojo()
	{
		m_CamRoot.transform.position = new Vector3(m_dojoHotSpot.transform.position.x, m_dojoHotSpot.transform.position.y, m_CamRoot.transform.position.z);
		SetNewHotSpot(m_dojoHotSpot);
	}

	private void SetNextProgressNodeActive()
	{
		DebugLog.Log(GetType(), "SetNextProgressNodeActive: searching next progress node!");
		HotSpotWorldMapViewBase nextProgressHotspot = GetNextProgressHotspot(DIContainerInfrastructure.GetCurrentPlayer());
		if (nextProgressHotspot == null)
		{
			DebugLog.Log(GetType(), "SetNextProgressNodeActive: No progress node found to go to. Maybe user has already unlocked all hotspots!");
		}
		else if (nextProgressHotspot.Model.IsActive() && nextProgressHotspot.Model.BalancingData.AutoSpawnBirds)
		{
			DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.CurrentHotspotGameData = nextProgressHotspot.Model;
			m_currentHotSpot = nextProgressHotspot;
		}
		else if (nextProgressHotspot.Model.BalancingData.NameId == "hotspot_042_battleground" && !nextProgressHotspot.Model.IsActive())
		{
			nextProgressHotspot = m_startingHotSpot.GetHotspotWorldMapView("hotspot_041_piggate_yellow");
		}
	}

	public override float TweenCameraToTransform(Transform target)
	{
		m_CamRoot.transform.position = new Vector3(target.position.x, target.position.y, m_CamRoot.transform.position.z);
		return 0f;
	}

	private IEnumerator Start()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("worldmap_loading");
		if (!DIContainerInfrastructure.GetPlayerPrefsService().HasKey("played_intro"))
		{
			DIContainerInfrastructure.GetPlayerPrefsService().SetInt("played_intro", 1);
			DIContainerInfrastructure.GetCoreStateMgr().m_EnterOnce = true;
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.IsInBattle = false;
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_PopupRoot.Leave();
		m_NewsLogic = new NewsLogic();
		m_NewsLogic.SetNewContentUpdateHandler();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveAllBars(true);
		m_LoadedLevels.Add("Menu_WorldMap", false);
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Menu_WorldMap", OnMenuWorldMapLoaded);
		int notloadedCount = m_LoadedLevels.Values.Count((bool e) => !e);
		while (notloadedCount > 0)
		{
			yield return new WaitForEndOfFrame();
			notloadedCount = m_LoadedLevels.Values.Count((bool e) => !e);
			if (ContentLoader.Instance != null)
			{
				ContentLoader.Instance.SetDownloadProgress(0.75f + (float)(m_LoadedLevels.Count - notloadedCount) / (float)m_LoadedLevels.Count * 0.25f);
			}
		}
		m_GoldenPigHotspotView.SetModel(DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.DailyHotspotGameData).SetStateMgr(this);
		m_WorldMenuUI.m_campButtonStates.SetUpdateMarker(DIContainerInfrastructure.GetCurrentPlayer().Data.HasNewOnWorlmap);
		InvokeRepeating("CheckForSpecialOffer", 1f, m_CheckForSpecialOfferFrequency);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("worldmap_loading");
		WorldMapConditionalActionTreePlayerOffProgres unlockCutscene = GetComponentInChildren<WorldMapConditionalActionTreePlayerOffProgres>();
		if ((bool)unlockCutscene && unlockCutscene.PlayCutscene())
		{
			yield return new WaitForSeconds(1f);
			while (!DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera.gameObject.activeInHierarchy)
			{
				yield return new WaitForEndOfFrame();
			}
		}
		yield return StartCoroutine(ActivateHotspots());
		while (!DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera.gameObject.activeInHierarchy)
		{
			yield return new WaitForEndOfFrame();
		}
		if (!DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(DIContainerInfrastructure.GetCurrentPlayer()))
		{
			DIContainerInfrastructure.GetCoreStateMgr().FixArenaSeason();
		}
		DIContainerInfrastructure.GetCoreStateMgr().DoInvalidateNicknameThenReset();
		StartCoroutine(TryPlayWorldMapMusic());
		DebugLog.Log("WORLDMAP ENTERED");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 0u,
			showFriendshipEssence = true,
			showLuckyCoins = true,
			showSnoutlings = true
		}, true);
		RecheckGainedAchievements();
		m_WorldMenuUI.gameObject.SetActive(true);
		DIContainerInfrastructure.TutorialMgr.StartTutorial("point_out_next_hotspot");
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("reset_indicator", string.Empty);
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("enter_worldmap", string.Empty);
		m_isInitialized = true;
		yield return StartCoroutine(StoppablePopupCoroutine());
		yield return StartCoroutine(m_WorldMenuUI.EnterCoroutine());
		m_secondsSinceLoad = 0f;
		InvokeRepeating("CheckForButtonMarkers", 1f, m_CheckForButtonMarkerFrequency);
		InvokeRepeating("UpdateTimeSpentOnWM", 1f, 1f);
		InvokeRepeating("UpdateGoldenPigStatus", 0f, 60f);
		RegisterEventHandlers();
		Invoke("SetHotspotFriendProgress", 1.5f);
		if (DIContainerInfrastructure.GetCoreStateMgr().m_ZoomToDungeonId < m_DungeonHotSpots.Count)
		{
			m_currentHotSpot = m_DungeonHotSpots[DIContainerInfrastructure.GetCoreStateMgr().m_ZoomToDungeonId];
			DIContainerInfrastructure.GetCoreStateMgr().m_ZoomToDungeonId = m_DungeonHotSpots.Count;
			ShowBattlePreperationScreen();
		}
		if (DIContainerInfrastructure.GetCoreStateMgr().m_GoToDojo)
		{
			ZoomToDojo();
			DIContainerInfrastructure.GetCoreStateMgr().m_GoToDojo = false;
		}
	}

	private void UpdateTimeSpentOnWM()
	{
		m_secondsSinceLoad += 1f;
	}

	private IEnumerator RecheckGainedAchievements()
	{
		IAchievementService service = DIContainerInfrastructure.GetAchievementService();
		List<string> gainedAchievements;
		for (gainedAchievements = service.GetUnlockedAchievements(); gainedAchievements == null; gainedAchievements = service.GetUnlockedAchievements())
		{
			yield return new WaitForSeconds(0.25f);
		}
		AchievementData trackedAchievements = DIContainerInfrastructure.GetCurrentPlayer().Data.AchievementTracking;
		string achievementId17 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("completeBannerSet");
		if (trackedAchievements.BannerSetCompleted && !gainedAchievements.Contains(achievementId17))
		{
			service.ReportUnlocked(achievementId17);
		}
		achievementId17 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("completeCaves");
		if (trackedAchievements.ChronicleCavesCompletedAchieved && !gainedAchievements.Contains(achievementId17))
		{
			service.ReportUnlocked(achievementId17);
		}
		achievementId17 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("defeatAllClasses");
		if (trackedAchievements.DefeatedClasses.Contains("$AchievementTracked$") && !gainedAchievements.Contains(achievementId17))
		{
			service.ReportUnlocked(achievementId17);
		}
		achievementId17 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("event_invasion_ninjas");
		if (trackedAchievements.EventCompletedNinja && !gainedAchievements.Contains(achievementId17))
		{
			service.ReportUnlocked(achievementId17);
		}
		achievementId17 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("event_invasion_pirates");
		if (trackedAchievements.EventCompletedPirate && !gainedAchievements.Contains(achievementId17))
		{
			service.ReportUnlocked(achievementId17);
		}
		achievementId17 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("event_invasion_zombies");
		if (trackedAchievements.EventCompletedZombie && !gainedAchievements.Contains(achievementId17))
		{
			service.ReportUnlocked(achievementId17);
		}
		achievementId17 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("reachStone");
		if (trackedAchievements.MaxLeagueReached > 1 && !gainedAchievements.Contains(achievementId17))
		{
			service.ReportUnlocked(achievementId17);
		}
		achievementId17 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("reachSilver");
		if (trackedAchievements.MaxLeagueReached > 2 && !gainedAchievements.Contains(achievementId17))
		{
			service.ReportUnlocked(achievementId17);
		}
		achievementId17 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("reachGold");
		if (trackedAchievements.MaxLeagueReached > 3 && !gainedAchievements.Contains(achievementId17))
		{
			service.ReportUnlocked(achievementId17);
		}
		achievementId17 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("reachPlatinum");
		if (trackedAchievements.MaxLeagueReached > 4 && !gainedAchievements.Contains(achievementId17))
		{
			service.ReportUnlocked(achievementId17);
		}
		achievementId17 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("reachDiamond");
		if (trackedAchievements.MaxLeagueReached > 5 && !gainedAchievements.Contains(achievementId17))
		{
			service.ReportUnlocked(achievementId17);
		}
		achievementId17 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("completeObjectives");
		if (trackedAchievements.ObjectivesCompletedAchieved && !gainedAchievements.Contains(achievementId17))
		{
			service.ReportUnlocked(achievementId17);
		}
		achievementId17 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("playAllClasses");
		if (trackedAchievements.PlayedClasses.Contains("$AchievementTracked$") && !gainedAchievements.Contains(achievementId17))
		{
			service.ReportUnlocked(achievementId17);
		}
		achievementId17 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("winArenaBattles");
		if (trackedAchievements.PvpfightsWonAchieved && !gainedAchievements.Contains(achievementId17))
		{
			service.ReportUnlocked(achievementId17);
		}
		achievementId17 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("topSpotAnyLeague");
		if (trackedAchievements.ReachedTopSpotAnyLeague && !gainedAchievements.Contains(achievementId17))
		{
			service.ReportUnlocked(achievementId17);
		}
		achievementId17 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("topSpotDiamond");
		if (trackedAchievements.ReachedTopSpotDiamondLeague && !gainedAchievements.Contains(achievementId17))
		{
			service.ReportUnlocked(achievementId17);
		}
		achievementId17 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("topSpotEvent");
		if (trackedAchievements.ReachedTopSpotEvent && !gainedAchievements.Contains(achievementId17))
		{
			service.ReportUnlocked(achievementId17);
		}
	}

	private void UpdateGoldenPigStatus()
	{
		WorldBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island");
		if (balancingData.UseGoldenPigCloudBattle)
		{
			if (DIContainerLogic.WorldMapService.IsDailyHotspotAvailable(DIContainerInfrastructure.GetCurrentPlayer()))
			{
				m_GoldenPigRoot.gameObject.SetActive(true);
				m_GoldenPigHotspotView.Enter();
			}
			else
			{
				m_GoldenPigHotspotView.Leave();
			}
		}
		else
		{
			DIContainerLogic.WorldMapService.RefreshDailyGoldenPigHotspot(DIContainerInfrastructure.GetCurrentPlayer());
		}
	}

	public void ActivateGoldenPig()
	{
		m_GoldenPigRoot.gameObject.SetActive(true);
		m_GoldenPigHotspotView.RefreshAsset();
	}

	public IEnumerator LeaveGoldenPig()
	{
		m_GoldenPigHotspotView.RefreshAsset();
		yield return StartCoroutine(m_GoldenPigHotspotView.LeaveCoroutine());
	}

	private IEnumerator TryPlayWorldMapMusic()
	{
		bool waitForMainThemeEnd = !DIContainerInfrastructure.GetCoreStateMgr().m_EnterOnce;
		DIContainerInfrastructure.GetCoreStateMgr().m_EnterOnce = true;
		while (waitForMainThemeEnd && DIContainerInfrastructure.PrimaryMusicSource.isPlaying)
		{
			yield return new WaitForEndOfFrame();
		}
		DIContainerInfrastructure.AudioManager.PlayMusic("music_worldmap");
	}

	private void InventoryOfTypeChanged(InventoryItemType itemType, IInventoryItemGameData inventoryItemGameData)
	{
		if ((itemType == InventoryItemType.CraftingRecipes || itemType == InventoryItemType.Class) && inventoryItemGameData.ItemData.IsNew)
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.HasNewOnWorlmap = true;
			m_WorldMenuUI.m_campButtonStates.SetUpdateMarker(DIContainerInfrastructure.GetCurrentPlayer().Data.HasNewOnWorlmap);
		}
	}

	private void TrackWorldMapLeave()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("SecondsSpendOnWorldmap", m_secondsSinceLoad.ToString());
		ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("WorldMapLeft", dictionary);
	}

	private void OnDestroy()
	{
		TrackWorldMapLeave();
	}

	private void OnApplicationPause(bool paused)
	{
		TrackWorldMapLeave();
		if (!paused)
		{
			ContentLoader.Instance.CheckforRestartApp();
		}
		if (!paused)
		{
			CheckForSpecialOffer();
		}
	}

	protected override void CheckForSpecialOffer()
	{
		base.CheckForSpecialOffer();
		m_WorldMenuUI.CheckForNewGiftMarker();
	}

	private void CheckForButtonMarkers()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		uint gachaVideoTimespan = (uint)DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").GachaVideoTimespan;
		if (DIContainerLogic.InventoryService.GetItemValue(currentPlayer.InventoryGameData, "story_goldenpig") >= 1)
		{
			uint timeStampOfLastVideoGacha = DIContainerInfrastructure.GetCurrentPlayer().Data.TimeStampOfLastVideoGacha;
			uint num = timeStampOfLastVideoGacha + gachaVideoTimespan * 60;
			m_WorldMenuUI.m_campButtonStates.SetVideoGachaMarker(num <= DIContainerLogic.GetTimingService().GetCurrentTimestamp());
			m_WorldMenuUI.m_campButtonStates.SetFriendGachaMarker(DIContainerLogic.SocialService.HasAnyGachaFreeRoll(currentPlayer.SocialEnvironmentGameData));
			bool rainbowRiotMarker = DIContainerLogic.GetShopService().IsRainbowRiotRunning(DIContainerInfrastructure.GetCurrentPlayer());
			m_WorldMenuUI.m_campButtonStates.SetRainbowRiotMarker(rainbowRiotMarker);
			m_WorldMenuUI.m_arenaButtonStates.SetRainbowRiotMarker(rainbowRiotMarker);
		}
		uint timeStampOfLastVideoPvPGacha = DIContainerInfrastructure.GetCurrentPlayer().Data.TimeStampOfLastVideoPvPGacha;
		uint num2 = timeStampOfLastVideoPvPGacha + gachaVideoTimespan * 60;
		m_WorldMenuUI.m_arenaButtonStates.SetVideoGachaMarker(num2 <= DIContainerLogic.GetTimingService().GetCurrentTimestamp());
		m_WorldMenuUI.m_arenaButtonStates.SetFriendGachaMarker(DIContainerLogic.SocialService.HasAnyPvpGachaFreeRoll(currentPlayer.SocialEnvironmentGameData));
		PvPSeasonManagerGameData currentPvPSeasonGameData = currentPlayer.CurrentPvPSeasonGameData;
		m_WorldMenuUI.m_arenaButtonStates.SetObjectiveMarker(DIContainerLogic.PvPSeasonService.IsDailyPvpRefreshed(currentPlayer, currentPvPSeasonGameData));
		m_WorldMenuUI.m_arenaButtonStates.SetSeasonEndMarker(currentPlayer.Data.HasPendingSeasonendPopup);
		if (currentPvPSeasonGameData != null && currentPvPSeasonGameData.CurrentSeasonTurn != null)
		{
			bool turnEndMarker = currentPvPSeasonGameData.CurrentSeasonTurn.CurrentPvPTurnManagerState == EventManagerState.Finished || currentPvPSeasonGameData.CurrentSeasonTurn.CurrentPvPTurnManagerState == EventManagerState.FinishedAndResultIsValid;
			m_WorldMenuUI.m_arenaButtonStates.SetTurnEndMarker(turnEndMarker);
		}
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(0, HandleBackButton);
		DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.InventoryOfTypeChanged += InventoryOfTypeChanged;
	}

	private void DeRegisterEventHandlers()
	{
		if ((bool)DIContainerInfrastructure.GetCoreStateMgr())
		{
			DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(0);
		}
		DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.InventoryOfTypeChanged -= InventoryOfTypeChanged;
	}

	public override bool IsShowContentPossible()
	{
		return !IsBirdWalking();
	}

	private void HandleBackButton()
	{
		DebugLog.Log("Pressed Back Button: " + GetType());
		m_Left = true;
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		DIContainerInfrastructure.GetCoreStateMgr().ShowConfirmationPopup(DIContainerInfrastructure.GetLocaService().Tr("gen_desc_leaveapp", "Do you really want to exit?"), delegate
		{
			Application.Quit();
		}, delegate
		{
		});
	}

	private void EtceteraAndroidManager_alertButtonClickedEvent(string text)
	{
		m_Left = false;
		EtceteraAndroidManager.alertButtonClickedEvent -= EtceteraAndroidManager_alertButtonClickedEvent;
		if (!(text == DIContainerInfrastructure.GetLocaService().Tr("gen_btn_yes", "Yes")))
		{
			return;
		}
		ProcessThreadCollection threads = Process.GetCurrentProcess().Threads;
		foreach (ProcessThread item in threads)
		{
			item.Dispose();
		}
		Process.GetCurrentProcess().Kill();
	}

	private void LoadBirdsIntoScene()
	{
		if (DIContainerInfrastructure.GetCurrentPlayer().Birds != null)
		{
			for (int i = 0; i < DIContainerInfrastructure.GetCurrentPlayer().Birds.Count; i++)
			{
				BirdGameData birdGameData = DIContainerInfrastructure.GetCurrentPlayer().Birds[i];
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(m_WorldMapCharacterController, m_CharacterRoot.position, m_CharacterRoot.rotation);
				CharacterControllerWorldMap component = gameObject.GetComponent<CharacterControllerWorldMap>();
				component.SetModel(birdGameData);
				GameObject gameObject2 = new GameObject(birdGameData.BalancingData.AssetId);
				gameObject2.AddComponent<CHMotionTween>();
				gameObject2.transform.position = m_CharacterRoot.position;
				gameObject2.transform.parent = m_CharacterRoot;
				gameObject.transform.parent = gameObject2.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = m_WorldBirdScale;
				m_Birds.Add(gameObject2);
				m_BirdAnimations.Add(component.m_AssetController.GetComponent<Animation>());
			}
		}
		m_walking = new bool[m_Birds.Count];
		for (int j = 0; j < m_Birds.Count; j++)
		{
			m_Birds[j].transform.position = m_currentHotSpot.transform.position + m_currentHotSpot.m_HotSpotPositions[j];
			if ((bool)m_BirdAnimations[j]["Idle"])
			{
				m_BirdAnimations[j].Play("Idle");
			}
			m_walking[j] = false;
			m_currentHotSpot.HandleMovingObjectVisibility(m_Birds[j].gameObject, m_currentHotSpot);
		}
		m_Ship.transform.position = m_currentHotSpot.transform.position + m_currentHotSpot.m_HotSpotPositions[0];
		m_AirShip.transform.position = m_currentHotSpot.transform.position + m_currentHotSpot.m_HotSpotPositions[0];
		m_Submarine.transform.position = m_currentHotSpot.transform.position + m_currentHotSpot.m_HotSpotPositions[0];
		m_currentHotSpot.HandleMovingObjectVisibility(m_Ship, m_currentHotSpot);
		m_currentHotSpot.HandleMovingObjectVisibility(m_AirShip, m_currentHotSpot);
		m_currentHotSpot.HandleMovingObjectVisibility(m_Submarine, m_currentHotSpot);
	}

	private void SynchBalancing(HotSpotWorldMapViewBase[] hotspots)
	{
		foreach (HotSpotWorldMapViewBase hotSpotWorldMapViewBase in hotspots)
		{
			hotSpotWorldMapViewBase.SynchBalancing();
			if (hotSpotWorldMapViewBase.m_nameId == DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.CurrentHotspotGameData.BalancingData.NameId)
			{
				m_currentHotSpot = hotSpotWorldMapViewBase;
			}
			if (hotSpotWorldMapViewBase.m_nameId == DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.BalancingData.FirstHotspotNameId)
			{
				m_startingHotSpot = hotSpotWorldMapViewBase;
			}
		}
	}

	private void OnMenuWorldMapLoaded()
	{
		m_LoadedLevels["Menu_WorldMap"] = true;
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(WorldMapMenuUI));
		m_WorldMenuUI = @object as WorldMapMenuUI;
		m_WorldMenuUI.SetStateMgr(this);
		m_WorldMenuUI.gameObject.SetActive(false);
		DebugLog.Log("MenuWorldMap loaded!");
	}

	private void OnWindowWorkshopLoaded()
	{
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(WorldMapShopMenuUI));
		m_WorkShopUI = @object as WorldMapShopMenuUI;
		m_WorkShopUI.SetStateMgr(this);
		m_WorkShopUI.gameObject.SetActive(false);
		m_WorkShopUI.SetModel(m_cachedShopBalancing, m_cachedShopType, m_cachedHotSpotMapView);
		m_WorkShopUI.Enter();
		m_cachedShopBalancing = null;
		m_cachedHotSpotMapView = null;
	}

	private void OnEventDetailsLoaded(EventManagerGameData evt)
	{
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(EventDetailUI));
		m_EventDetailUI = @object as EventDetailUI;
		m_EventDetailUI.SetStateMgr(this);
		m_EventDetailUI.gameObject.SetActive(false);
		m_EventDetailUI.SetModel(evt);
		m_EventDetailUI.Enter();
	}

	private void OnLeaderBoardLoaded()
	{
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(LeaderboardUI));
		m_LeaderBoardUI = @object as LeaderboardUI;
		m_LeaderBoardUI.SetWorldMapStateMgr(this);
		m_LeaderBoardUI.gameObject.SetActive(false);
	}

	private void OnEventPreviewScreenLoaded(EventManagerGameData eMgr = null, bool starting = false, string origin = null)
	{
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(EventPreviewUI));
		m_EventPreviewUI = @object as EventPreviewUI;
		m_EventPreviewUI.SetStateMgr(this);
		m_EventPreviewUI.gameObject.SetActive(false);
		EventManagerGameData model = eMgr ?? DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
		m_EventPreviewUI.SetModel(model);
		m_EventPreviewUI.Enter(starting, origin);
	}

	private void OnEventStartPopupLoaded(EventManagerGameData eMgr = null)
	{
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(EventPreviewUI));
		m_EventPreviewUI = @object as EventPreviewUI;
		m_EventPreviewUI.SetStateMgr(this);
		m_EventPreviewUI.gameObject.SetActive(false);
		EventManagerGameData model = eMgr ?? DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
		m_EventPreviewUI.SetModel(model);
		m_EventPreviewUI.Enter();
	}

	private void OnNewsScreenLoaded()
	{
		m_EventNews = UnityEngine.Object.FindObjectOfType(typeof(NewsUi)) as NewsUi;
		m_EventNews.SetStateMgr(this, m_NewsLogic);
		m_EventNews.Enter(m_cachedNewsStartingState);
	}

	private void OnWindowBattlePreparationLoadedGoldenPig()
	{
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(BattlePreperationUI));
		m_battlePreperation = @object as BattlePreperationUI;
		m_battlePreperation.gameObject.SetActive(false);
		m_battlePreperation.SetHotSpot(DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.DailyHotspotGameData, this);
		m_battlePreperation.Enter();
	}

	private void OnWindowBattlePreparationLoaded()
	{
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(BattlePreperationUI));
		m_battlePreperation = @object as BattlePreperationUI;
		m_battlePreperation.gameObject.SetActive(false);
		m_battlePreperation.SetHotSpot(((HotSpotWorldMapViewBattle)m_currentHotSpot).Model, this);
		m_battlePreperation.Enter();
	}

	private void OnWindowBattlePreparationLoadedEvent()
	{
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(BattlePreperationUI));
		m_battlePreperation = @object as BattlePreperationUI;
		m_battlePreperation.gameObject.SetActive(false);
		m_battlePreperation.SetEventPlacement(m_cachedEventItem, m_cachedEventPlacement, this);
		m_battlePreperation.Enter();
		m_cachedEventItem = null;
		m_cachedEventPlacement = null;
	}

	private void OnWindowFriendshipGateLoadedShow()
	{
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(FriendshipGateUI));
		m_friendShipGate = @object as FriendshipGateUI;
		m_friendShipGate.SetHotSpot(m_cachedHotSpot, this);
		m_friendShipGate.SetReturnAction(m_cachedAction);
		m_friendShipGate.Enter();
		m_cachedHotSpot = null;
		m_cachedAction = null;
	}

	private void OnBonusCodeLoaded()
	{
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(BonusCodeManager));
		m_BonusCodeManager = @object as BonusCodeManager;
		m_BonusCodeManager.gameObject.SetActive(false);
	}

	private IEnumerator ActivateHotspots()
	{
		if (!m_startingHotSpot.IsCompleted())
		{
			DebugLog.Log("first start");
			DIContainerLogic.WorldMapService.CompleteHotSpot(DIContainerInfrastructure.GetCurrentPlayer(), m_startingHotSpot.Model, 3, 0);
		}
		else
		{
			yield return StartCoroutine(m_startingHotSpot.ActivateFollowUpStagesAsync(null, null));
		}
		InvokeRepeating("UpdateResourceNodeManager", 0.1f, 60f);
	}

	public override void StartBattle(HotspotGameData hotspot, List<BirdGameData> battleBirdList, BattleParticipantTableBalancingData addition, bool hardmode = false)
	{
		if (!DIContainerLogic.WorldMapService.SetupHotspotBattle(DIContainerInfrastructure.GetCurrentPlayer(), hotspot, battleBirdList, addition, hardmode))
		{
			DebugLog.Error("Failed to set up Hotspot Battle");
			return;
		}
		if (ClientInfo.CurrentBattleStartGameData != null)
		{
			DebugLog.Log("Started Battle ID: " + ClientInfo.CurrentBattleStartGameData.m_BattleBalancingNameId);
		}
		CoreStateMgr.Instance.GotoBattle(ClientInfo.CurrentBattleStartGameData.m_BackgroundAssetId);
	}

	public override void ShowEventDetailScreen(EventManagerGameData evt)
	{
		if (IsBirdWalking() || evt == null || !evt.IsValid || DIContainerLogic.EventSystemService.IsEventTeasing(evt.Balancing))
		{
			return;
		}
		if (m_EventDetailUI == null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_EventDetails", delegate
			{
				OnEventDetailsLoaded(evt);
			});
		}
		else
		{
			m_EventDetailUI.SetModel(evt);
			m_EventDetailUI.Enter();
		}
	}

	public override void ShowEventPreviewScreen(EventManagerGameData eMgr = null, bool showStarting = false, string origin = null)
	{
		EventManagerGameData model = eMgr ?? DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
		if (IsBirdWalking() || !model.IsValid || !model.IsAssetValid)
		{
			return;
		}
		if (m_EventPreviewUI == null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("EventPreviewScreen", delegate
			{
				OnEventPreviewScreenLoaded(model, showStarting, origin);
			});
		}
		else
		{
			m_EventPreviewUI.SetModel(model);
			m_EventPreviewUI.Enter(showStarting, origin);
		}
	}

	public override void ShowEventResultPopup()
	{
		if (!IsBirdWalking() && DIContainerLogic.EventSystemService.IsCurrentEventAvailable(DIContainerInfrastructure.GetCurrentPlayer()) && DIContainerLogic.EventSystemService.IsWaitingForConfirmation(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData))
		{
			base.IsEventResultRunning = true;
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddLevel("Popup_EventFinished", true, false, delegate
			{
			});
		}
	}

	public override void ShowLeaderBoardScreen(WorldBossTeamData ownTeam = null, WorldBossTeamData enemyTeam = null, EventDetailUI detailUi = null)
	{
		if (DIContainerLogic.EventSystemService.IsCurrentEventAvailable(DIContainerInfrastructure.GetCurrentPlayer()) && !DIContainerLogic.EventSystemService.IsEventTeasing(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Balancing))
		{
			DebugLog.Log(GetType(), "ShowLeaderBoardScreen: from WorldmapStateMgr, leaderboardUI cached, setting event model...");
			m_LeaderBoardUI.SetEventModel(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData);
			m_LeaderBoardUI.Enter(ownTeam, enemyTeam, false, detailUi);
		}
	}

	public override void ShowTrainerScreen(string shopNameId, HotSpotWorldMapViewBase hotspot)
	{
		ShopBalancingData balancing = null;
		if (DIContainerBalancing.Service.TryGetBalancingData<ShopBalancingData>(shopNameId, out balancing))
		{
			m_cachedShopBalancing = balancing;
			m_cachedShopType = ShopMenuType.Trainer;
			m_cachedHotSpotMapView = hotspot;
			ShowWorkShopUi();
		}
	}

	public override void ShowDojoScreen(string shopNameId, HotSpotWorldMapViewBase hotspot)
	{
		ShopBalancingData balancing = null;
		if (DIContainerBalancing.Service.TryGetBalancingData<ShopBalancingData>(shopNameId, out balancing))
		{
			m_cachedShopBalancing = balancing;
			m_cachedShopType = ShopMenuType.Dojo;
			m_cachedHotSpotMapView = hotspot;
			ShowWorkShopUi();
		}
	}

	public override void ShowWorkshopScreen(string shopNameId, HotSpotWorldMapViewBase hotspot)
	{
		ShopBalancingData balancing = null;
		if (DIContainerBalancing.Service.TryGetBalancingData<ShopBalancingData>(shopNameId, out balancing))
		{
			m_cachedShopBalancing = balancing;
			m_cachedShopType = ShopMenuType.Workshop;
			m_cachedHotSpotMapView = hotspot;
			ShowWorkShopUi();
		}
	}

	public override void ShowWitchHutScreen(string shopNameId, HotSpotWorldMapViewBase hotspot)
	{
		ShopBalancingData balancing = null;
		if (DIContainerBalancing.Service.TryGetBalancingData<ShopBalancingData>(shopNameId, out balancing))
		{
			m_cachedShopBalancing = balancing;
			m_cachedShopType = ShopMenuType.Witchhut;
			m_cachedHotSpotMapView = hotspot;
			ShowWorkShopUi();
		}
	}

	private void ShowWorkShopUi()
	{
		if (m_WorkShopUI == null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_WorldShop", OnWindowWorkshopLoaded);
			return;
		}
		m_WorkShopUI.SetModel(m_cachedShopBalancing, m_cachedShopType, m_cachedHotSpotMapView);
		m_WorkShopUI.Enter();
	}

	public void ShowBattlePreperationScreenForEvent(EventItemGameData eventItem, EventPlacementBalancingData placement)
	{
		if (m_battlePreperation == null)
		{
			m_cachedEventItem = eventItem;
			m_cachedEventPlacement = placement;
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_BattlePreparation", OnWindowBattlePreparationLoadedEvent);
		}
		else if (!m_battlePreperation.m_Entered)
		{
			m_battlePreperation.SetEventPlacement(eventItem, placement, this);
			m_battlePreperation.Enter();
		}
	}

	public override void ShowBattlePreperationScreen()
	{
		if (m_battlePreperation == null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_BattlePreparation", OnWindowBattlePreparationLoaded);
		}
		else if (!m_battlePreperation.m_Entered && !(m_currentHotSpot == null))
		{
			m_battlePreperation.SetHotSpot(((HotSpotWorldMapViewBattle)m_currentHotSpot).Model, this);
			m_battlePreperation.Enter();
		}
	}

	public void ShowGoldenPigBattlePreperationScreen()
	{
		if (IsBirdWalking())
		{
			return;
		}
		if (DIContainerLogic.WorldMapService.IsHotspotEnterable(DIContainerInfrastructure.GetCurrentPlayer(), DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.DailyHotspotGameData))
		{
			if (m_battlePreperation == null)
			{
				DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_BattlePreparation", OnWindowBattlePreparationLoadedGoldenPig);
			}
			else if (!m_battlePreperation.m_Entered)
			{
				m_battlePreperation.SetHotSpot(DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.DailyHotspotGameData, this);
				m_battlePreperation.Enter();
			}
		}
		else
		{
			DebugLog.Log("Daily Hotspot not available!");
		}
	}

	public override void ShowFriendshipGateScreen(Action actionOnReturn, HotspotGameData hotspot)
	{
		if (m_friendShipGate == null)
		{
			m_cachedHotSpot = hotspot;
			m_cachedAction = actionOnReturn;
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_FriendshipGate", OnWindowFriendshipGateLoadedShow);
		}
		else
		{
			m_friendShipGate.SetHotSpot(hotspot, this);
			m_friendShipGate.SetReturnAction(actionOnReturn);
			m_friendShipGate.Enter();
		}
	}

	public override void EnableInput(bool flag)
	{
		m_inputEnabled = flag;
	}

	public override void SetNewHotSpot(HotSpotWorldMapViewBase newSpot, Action actionAfterWalkingDone = null, bool instantMove = false)
	{
		if ((m_isMovementPossible != null && !m_isMovementPossible()) || IsBirdWalking() || !m_inputEnabled)
		{
			return;
		}
		if (actionAfterWalkingDone == null)
		{
			m_ActionAfterWalkingDone = newSpot.ShowContentView;
		}
		else
		{
			m_ActionAfterWalkingDone = actionAfterWalkingDone;
		}
		if (newSpot == m_currentHotSpot)
		{
			if (m_ActionAfterWalkingDone != null)
			{
				m_ActionAfterWalkingDone();
				m_ActionAfterWalkingDone = null;
			}
		}
		else if (m_currentPathList.Count <= 0)
		{
			DIContainerLogic.WorldMapService.TravelToHotSpot(DIContainerInfrastructure.GetCurrentPlayer(), newSpot.Model);
			m_currentPathList = CalculatePath(m_currentHotSpot, newSpot);
			m_currentHotSpot = newSpot;
			if (instantMove && m_ActionAfterWalkingDone != null)
			{
				m_ActionAfterWalkingDone();
				m_ActionAfterWalkingDone = null;
			}
			DebugLog.Log("[WorldMapStateMgr] walking to newSpot: " + ((!(newSpot == null)) ? newSpot.m_nameId : "null"));
			for (int i = 0; i < m_Birds.Count; i++)
			{
				m_walking[i] = true;
				PathMovingService.Instance.WalkAlongPath(m_currentPathList, m_Birds[i], m_BirdAnimations[i], m_BirdSpeed, i, m_movementStartDelay[i], this, "WalkDone");
			}
			PathMovingService.Instance.WalkAlongPath(m_currentPathList, m_Ship, m_ShipAnimation, m_BirdSpeed, 0, 1f * m_movementStartDelay[0], this, "WalkDone", false, "Move");
			PathMovingService.Instance.WalkAlongPath(m_currentPathList, m_AirShip, m_AirShipAnimation, m_BirdSpeed, 0, 1f * m_movementStartDelay[0], this, "WalkDone", false, "Move");
			PathMovingService.Instance.WalkAlongPath(m_currentPathList, m_Submarine, m_m_SubmarineAnimation, m_BirdSpeed, 0, 1f * m_movementStartDelay[0], this, "WalkDone", false, "Move");
		}
	}

	public override void SetNewHotSpotDuringCutscene(HotSpotWorldMapViewBase newSpot, Action actionAfterWalkingDone = null, bool instantMove = false)
	{
		if (actionAfterWalkingDone == null)
		{
			m_ActionAfterWalkingDone = newSpot.ShowContentView;
		}
		else
		{
			m_ActionAfterWalkingDone = actionAfterWalkingDone;
		}
		if (newSpot == m_currentHotSpot)
		{
			if (m_ActionAfterWalkingDone != null)
			{
				m_ActionAfterWalkingDone();
				m_ActionAfterWalkingDone = null;
			}
		}
		else if (m_currentPathList.Count <= 0)
		{
			DIContainerLogic.WorldMapService.TravelToHotSpot(DIContainerInfrastructure.GetCurrentPlayer(),
				newSpot.Model);
			m_currentPathList = CalculatePath(m_currentHotSpot, newSpot);
			m_currentHotSpot = newSpot;
			if (instantMove && m_ActionAfterWalkingDone != null)
			{
				m_ActionAfterWalkingDone();
				m_ActionAfterWalkingDone = null;
			}

			DebugLog.Log("[WorldMapStateMgr] walking to newSpot: " +
			             ((!(newSpot == null)) ? newSpot.m_nameId : "null"));
			for (int i = 0; i < m_Birds.Count; i++)
			{
				m_walking[i] = true;
				PathMovingService.Instance.WalkAlongPath(m_currentPathList, m_Birds[i], m_BirdAnimations[i],
					m_BirdSpeed, i, m_movementStartDelay[i], this, "WalkDone");
			}
		}
	}

	public void WalkDone(object o)
	{
		int num = (int)o;
		DebugLog.Log("WalkDone with index " + num + " at " + Time.time);
		m_walking[num] = false;
		CheckWalkingBirds();
	}

	public void WalkDone()
	{
		if (m_movementTargetIndex >= 0)
		{
			DebugLog.Log("Walk Done without params but using member variable m_movementTargetIndex with value " + m_movementTargetIndex + " as targetIndex");
			WalkDone(m_movementTargetIndex);
			m_movementTargetIndex = -1;
		}
		else
		{
			DebugLog.Log("Walk Done without params");
		}
	}

	private void firstWalkDone()
	{
		List<BirdGameData> birds = DIContainerInfrastructure.GetCurrentPlayer().Birds;
		StartBattle(m_currentHotSpot.Model, birds, null);
	}

	private bool CheckWalkingBirds()
	{
		if (IsBirdWalking())
		{
			return true;
		}
		m_currentPathList.Clear();
		if (m_ActionAfterWalkingDone != null)
		{
			m_ActionAfterWalkingDone();
			m_ActionAfterWalkingDone = null;
		}
		return false;
	}

	public override bool IsBirdWalking()
	{
		for (int i = 0; i < m_walking.Length; i++)
		{
			if (m_walking[i])
			{
				return true;
			}
		}
		return false;
	}

	public static List<HotSpotWorldMapViewBase> CalculatePath(HotSpotWorldMapViewBase start, HotSpotWorldMapViewBase end)
	{
		List<HotSpotWorldMapViewBase> list = new List<HotSpotWorldMapViewBase>();
		start.CalculatePath(start, end, ref list);
		list.Reverse();
		return list;
	}

	private void UpdateResourceNodeManager()
	{
		DebugLog.Log("Check Resource Node Cooldown");
		DIContainerLogic.GetResourceNodeManager().CheckGlobalCoolDown();
	}

	private void OnEnable()
	{
		CheckLeagueAchievements();
	}

	private void OnDisable()
	{
		DebugLog.Log("Clean up Worldmap");
		DeRegisterEventHandlers();
		DIContainerLogic.GetResourceNodeManager().ClearSpotList();
		if ((bool)DIContainerInfrastructure.GetCoreStateMgr())
		{
			DIContainerInfrastructure.GetCoreStateMgr().UnloadUnusedAssets();
		}
		// EtceteraAndroidManager.alertButtonClickedEvent -= EtceteraAndroidManager_alertButtonClickedEvent; no etc android 😡 (it causes annoying errors)
		DIContainerInfrastructure.GetCoreStateMgr().HideConfirmationPopup();
	}

	public override GameObject GetEmoteBubble(string spriteName, Vector3 offset, Transform root, UIAtlas atlas)
	{
		GameObject gameObject = null;
		for (int i = 0; i < m_MissingBubbles.Count; i++)
		{
			MissingItemToPrefabMapping missingItemToPrefabMapping = m_MissingBubbles[i];
			if (missingItemToPrefabMapping.NameId == spriteName)
			{
				gameObject = UnityEngine.Object.Instantiate(missingItemToPrefabMapping.Prefab);
				break;
			}
		}
		UnityHelper.SetLayerRecusively(gameObject.gameObject, root.gameObject.layer);
		gameObject.transform.parent = root;
		gameObject.transform.localPosition = offset;
		StartCoroutine(ShowAndHideBubble(gameObject));
		return gameObject;
	}

	private IEnumerator ShowAndHideBubble(GameObject bubble)
	{
		if (bubble.GetComponent<Animation>()["Bubble_Show"] != null)
		{
			bubble.GetComponent<Animation>().Play("Bubble_Show");
			yield return new WaitForSeconds(bubble.GetComponent<Animation>()["Bubble_Show"].length);
			if (bubble.GetComponent<Animation>()["Bubble_Hide"] != null)
			{
				yield return new WaitForSeconds(1f);
				bubble.GetComponent<Animation>().Play("Bubble_Hide");
				yield return new WaitForSeconds(bubble.GetComponent<Animation>()["Bubble_Show"].length);
			}
			UnityEngine.Object.Destroy(bubble);
		}
		else
		{
			bubble.GetComponent<Animation>().Play();
			while ((bool)bubble && bubble.GetComponent<Animation>().isPlaying)
			{
				yield return new WaitForEndOfFrame();
			}
			UnityEngine.Object.Destroy(bubble);
		}
	}

	public override Vector3 GetWorldBirdScale()
	{
		return m_WorldBirdScale;
	}

	public override void ResetBirdPositions()
	{
		m_currentHotSpot = m_startingHotSpot.GetHotspotWorldMapView(DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.CurrentHotspotGameData.BalancingData.NameId);
	}

	private void CheckLeagueAchievements()
	{
		if (DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData == null)
		{
			return;
		}
		bool flag = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.GetCurrentRank == 1 && DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.PublicOpponentDatas.Count > 2;
		AchievementData achievementTracking = DIContainerInfrastructure.GetCurrentPlayer().Data.AchievementTracking;
		if (!achievementTracking.ReachedTopSpotEvent && flag)
		{
			string achievementIdForStoryItemIfExists = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("topSpotEvent");
			if (!string.IsNullOrEmpty(achievementIdForStoryItemIfExists))
			{
				DIContainerInfrastructure.GetAchievementService().ReportUnlocked(achievementIdForStoryItemIfExists);
				achievementTracking.ReachedTopSpotEvent = true;
			}
		}
	}

	public HotSpotWorldMapViewBase GetNextProgressHotspot(PlayerGameData player)
	{
		string value = string.Empty;
		int num = player.SocialEnvironmentGameData.Data.LocationProgress[LocationType.World];
		if (num == 2 || num == 4)
		{
			num++;
		}
		if (player.WorldGameData.StoryProgressHotspotIds.TryGetValue(num + 1, out value))
		{
			return m_startingHotSpot.GetHotspotWorldMapView(value);
		}
		DebugLog.Log("No more progress hotspots found!");
		return null;
	}

	public IEnumerator ShowPlayerAttacksBossAnim(PublicPlayerData player)
	{
		EventSystemWorldMapStateMgr stateMgr = base.EventsWorldMapStateMgr;
		Transform target = null;
		Vector3 screenPosToStartFrom = Vector3.zero;
		bool bossVisible = BossIsVisible();
		if (bossVisible)
		{
			target = stateMgr.GetBossNode().transform;
			screenPosToStartFrom = m_WorldCam.ScreenToWorldPoint(new Vector3((UnityEngine.Random.Range(0, 2) >= 1) ? Screen.width : 0, (float)Screen.height * UnityEngine.Random.value, 0f));
			screenPosToStartFrom = new Vector3(screenPosToStartFrom.x, screenPosToStartFrom.y, base.transform.position.z - 100f);
		}
		else
		{
			target = ((m_WorldMenuUI.m_SpecialButtonGrid.transform.childCount <= 0) ? m_WorldMenuUI.m_SpecialButtonGrid.transform : m_WorldMenuUI.m_SpecialButtonGrid.transform.GetChild(0).transform);
			screenPosToStartFrom = DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera.ScreenToWorldPoint(new Vector3((UnityEngine.Random.Range(0, 2) >= 1) ? Screen.width : 0, (float)Screen.height / 2f, 0f));
			screenPosToStartFrom = new Vector3(screenPosToStartFrom.x, screenPosToStartFrom.y, base.transform.position.z - 10f);
		}
		OpponentInfoElement gachaFriendIcon = UnityEngine.Object.Instantiate(m_FlyingFriendIcon, screenPosToStartFrom, Quaternion.identity) as OpponentInfoElement;
		OpponentGameData flyingOpponent = new OpponentGameData(player, player.SocialId == DIContainerInfrastructure.IdentityService.SharedId);
		if (!bossVisible)
		{
			UnityHelper.SetLayerRecusively(gachaFriendIcon.gameObject, LayerMask.NameToLayer("Interface"));
			Transform trans = gachaFriendIcon.transform.GetChild(0).transform;
			gachaFriendIcon.transform.GetChild(0).transform.position = new Vector3(trans.position.x, trans.position.y + 50f, trans.position.z);
		}
		gachaFriendIcon.SetDefault(1, 1, 1, false, false, player.SocialId == DIContainerInfrastructure.IdentityService.SharedId);
		gachaFriendIcon.SetModel(flyingOpponent, player.SocialId == DIContainerInfrastructure.IdentityService.SharedId);
		CHMotionTween friendMotion = gachaFriendIcon.GetComponent<CHMotionTween>();
		if ((bool)friendMotion)
		{
			friendMotion.m_EndTransform = target;
			if (!bossVisible)
			{
				friendMotion.m_EndOffset = new Vector3(0f, -50f, 0f);
			}
			friendMotion.Play();
			yield return new WaitForSeconds(friendMotion.m_DurationInSeconds);
			UnityEngine.Object.Destroy(gachaFriendIcon.gameObject);
		}
	}

	private bool BossIsVisible()
	{
		EventSystemWorldMapStateMgr eventsWorldMapStateMgr = base.EventsWorldMapStateMgr;
		EventPositionNode bossNode = eventsWorldMapStateMgr.GetBossNode();
		if (bossNode == null)
		{
			return false;
		}
		Vector2 zero = Vector2.zero;
		Bounds rectA = default(Bounds);
		if ((bool)m_WorldCam)
		{
			zero = new Vector2(m_WorldCam.orthographicSize * ((float)Screen.width / (float)Screen.height), m_WorldCam.orthographicSize);
			rectA = new Bounds(m_WorldCam.transform.position, new Vector3(2f * zero.x, 2f * zero.y));
		}
		if (RectangleIntersect(rectA, bossNode.GetBoundingBox()))
		{
			return true;
		}
		return false;
	}

	private bool RectangleIntersect(Bounds rectA, Bounds rectB)
	{
		Bounds bounds = new Bounds(new Vector3(rectA.center.x, rectA.center.y, 100f), new Vector3(rectA.size.x, rectA.size.y, 100f));
		Bounds bounds2 = new Bounds(new Vector3(rectB.center.x, rectB.center.y, 100f), new Vector3(rectB.size.x, rectB.size.y, 100f));
		return bounds.Intersects(bounds2);
	}

	public override bool ShowNewsUi(NewsUi.NewsUiState startingState = NewsUi.NewsUiState.Events)
	{
		if (m_EventNews == null)
		{
			m_cachedNewsStartingState = startingState;
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_News", OnNewsScreenLoaded);
		}
		else
		{
			m_EventNews.Enter(startingState);
		}
		return true;
	}
}

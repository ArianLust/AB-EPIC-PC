using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.Shared.BalancingData;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class EventCampaignStateMgr : BaseLocationStateManager
{
	[HideInInspector]
	public HotSpotWorldMapViewBase m_startingHotSpot;

	protected HotSpotWorldMapViewBase m_currentHotSpot;

	private List<HotSpotWorldMapViewBase> m_currentPathList = new List<HotSpotWorldMapViewBase>();

	private HotSpotWorldMapViewPathNode[] m_gates = new HotSpotWorldMapViewPathNode[0];

	public Transform m_CamRoot;

	public LayerMask EventMask = -1;

	public DragController m_DragController;

	[SerializeField]
	private Transform m_MiniMapRoot;

	private EventCampaignGameData m_Model;

	[SerializeField]
	private EventCampaignMapManager m_MapManager;

	private bool m_inputEnabled = true;

	private Action m_ActionAfterWalkingDone;

	public Vector3 m_WorldBirdScale = new Vector3(0.4f, 0.4f, 1f);

	public GameObject m_WorldMapCharacterController;

	public Transform m_CharacterRoot;

	public List<Animation> m_BirdAnimations;

	public float[] m_movementStartDelay;

	public float m_BirdSpeed = 100f;

	public Vector3[] m_HotSpotPositions;

	private bool[] m_walking;

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
	private Animation m_SubmarineAnimation;

	[SerializeField]
	private FloatingTreasure m_floatingTreasure;

	private BattleMgr m_BattleMgr;

	[HideInInspector]
	public NewsUi m_EventNews;

	public NewsLogic m_NewsLogic;

	[SerializeField]
	private List<Vector2> m_DragDimensions;

	private Dictionary<string, bool> m_LoadedLevels = new Dictionary<string, bool>();

	[HideInInspector]
	public BattlePreperationUI m_battlePreperation;

	[HideInInspector]
	public FriendshipGateUI m_friendShipGate;

	[HideInInspector]
	public EventCampaignMenuUI m_MiniCampaignMenuUI;

	[HideInInspector]
	public EventDetailUI m_EventDetailUI;

	[HideInInspector]
	private LeaderboardUI m_LeaderBoardUI;

	[HideInInspector]
	private EventPreviewUI m_EventPreviewUI;

	[HideInInspector]
	public BonusCodeManager m_BonusCodeManager;

	private bool m_Left;

	private DateTime lastPressTime = DateTime.MinValue;

	private double m_PushOnlyDelayOnBackButton = 10.0;

	public bool InputEnabled
	{
		get
		{
			return m_inputEnabled;
		}
	}

	public override IMapUI WorldMenuUI
	{
		get
		{
			return m_MiniCampaignMenuUI;
		}
	}

	public override void StartBattle(HotspotGameData hotspot, List<BirdGameData> battleBirdList, BattleParticipantTableBalancingData addition, bool hardmode = false)
	{
		SetupHotspotBattle(DIContainerInfrastructure.GetCurrentPlayer(), hotspot, battleBirdList, addition);
		CoreStateMgr.Instance.GotoBattle(ClientInfo.CurrentBattleStartGameData.m_BackgroundAssetId);
	}

	public bool SetupHotspotBattle(PlayerGameData playerGameData, HotspotGameData hotspot, List<BirdGameData> battleBirdList, BattleParticipantTableBalancingData addition, bool hardmode = false)
	{
		if (!DIContainerLogic.WorldMapService.EnterHotspot(playerGameData, hotspot))
		{
			return false;
		}
		Dictionary<Faction, Dictionary<string, float>> factionBuffs = new Dictionary<Faction, Dictionary<string, float>>();
		if (hotspot.Data.RandomSeed == 0)
		{
			hotspot.Data.RandomSeed = UnityEngine.Random.Range(1, int.MaxValue);
		}
		string firstPossibleBattle = DIContainerLogic.GetBattleService().GetFirstPossibleBattle(hotspot.BalancingData.BattleId, playerGameData, false, hardmode);
		string backgroundAssetId = DIContainerBalancing.Service.GetBalancingData<BattleBalancingData>(firstPossibleBattle).BackgroundAssetId;
		int num = hotspot.BalancingData.BattleId.IndexOf(firstPossibleBattle);
		List<string> possibleFollowUpBattles = new List<string>();
		string currentSponsoredBuff = playerGameData.Data.CurrentSponsoredBuff;
		playerGameData.Data.CurrentSponsoredBuff = string.Empty;
		if (hotspot.BalancingData.BattleId.Count >= num + 1)
		{
			possibleFollowUpBattles = hotspot.BalancingData.BattleId.GetRange(num + 1, hotspot.BalancingData.BattleId.Count - (num + 1));
		}
		BattleStartGameData battleStartGameData = new BattleStartGameData();
		battleStartGameData.m_BackgroundAssetId = ((!string.IsNullOrEmpty(hotspot.OverrideBattleGround) && string.IsNullOrEmpty(backgroundAssetId)) ? hotspot.OverrideBattleGround : backgroundAssetId);
		battleStartGameData.m_RageAvailiable = DIContainerLogic.InventoryService.CheckForItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "unlock_rage");
		battleStartGameData.m_Birds = battleBirdList;
		battleStartGameData.m_BattleBalancingNameId = firstPossibleBattle;
		battleStartGameData.callback = delegate(IAsyncResult a)
		{
			OnHotspotBattleDone(playerGameData, hotspot, a);
		};
		battleStartGameData.m_Inventory = playerGameData.InventoryGameData;
		battleStartGameData.m_InvokerLevel = playerGameData.Data.Level;
		battleStartGameData.m_InjectableParticipantTable = addition;
		battleStartGameData.m_BattleRandomSeed = hotspot.Data.RandomSeed;
		battleStartGameData.m_PossibleFollowUpBattles = possibleFollowUpBattles;
		battleStartGameData.m_EnvironmentalEffects = DIContainerBalancing.Service.GetBalancingData<BattleBalancingData>(firstPossibleBattle).EnvironmentalEffects;
		battleStartGameData.m_SponsoredEnvironmentalEffect = currentSponsoredBuff;
		battleStartGameData.m_FactionBuffs = factionBuffs;
		ClientInfo.CurrentBattleStartGameData = battleStartGameData;
		return true;
	}

	public void OnHotspotBattleDone(PlayerGameData playerGameData, HotspotGameData hotspot, IAsyncResult result)
	{
		if (result == null)
		{
			return;
		}
		BattleEndGameData battleResult = DIContainerLogic.GetBattleService().EndBattle(result);
		if (battleResult.m_WinnerFaction != 0)
		{
			return;
		}
		DIContainerLogic.GetTimingService().GetTrustedTimeEx(delegate(DateTime trustedTime)
		{
			if (hotspot.BalancingData.CooldownInSeconds != 0)
			{
				hotspot.Data.LastVisitDateTime = trustedTime;
			}
			DIContainerLogic.WorldMapService.CompleteHotSpot(playerGameData, hotspot, battleResult.m_BattlePerformanceStars, battleResult.m_Score);
			DIContainerLogic.WorldMapService.SetCampaignProgress(playerGameData, hotspot);
		});
	}

	private void Awake()
	{
		DIContainerInfrastructure.AdService.AddPlacement(BattlePreperationUI.BUFF_PLACEMENT);
		DIContainerInfrastructure.GetCoreStateMgr().m_ChronicleCave = false;
		DIContainerInfrastructure.GetCoreStateMgr().m_EventCampaign = true;
		DIContainerInfrastructure.GetCoreStateMgr().m_IsWithinPvP = false;
		DIContainerInfrastructure.GetCoreStateMgr().m_SceneryAudioListener = base.transform.GetComponentInChildren<AudioListener>();
		m_FeatureUnlockCoroutineInstance = HandleFeatureUnlocksAndLevelUps();
		if (DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData != null)
		{
			m_Model = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.CurrentMiniCampaign;
			if (m_MapManager == null)
			{
				GameObject gameObject = DIContainerInfrastructure.EventSystemStateManager.InstantiateEventObject("Map", m_MiniMapRoot);
				m_MapManager = gameObject.GetComponent<EventCampaignMapManager>();
				m_MapManager.transform.parent = m_MiniMapRoot;
			}
			m_MapManager.SetStateMgr(this);
			SetDragControllerSize();
			m_gates = m_MapManager.gameObject.GetComponentsInChildren<HotSpotWorldMapViewPathNode>();
			HotSpotWorldMapViewBase[] componentsInChildren = m_MiniMapRoot.GetComponentsInChildren<HotSpotWorldMapViewBase>(true);
			SynchBalancing(componentsInChildren);
			m_startingHotSpot = m_MapManager.m_StartingHotSpot;
			m_currentHotSpot = ((m_Model.CurrentHotspotGameData == null) ? m_startingHotSpot : m_Model.CurrentHotspotGameData.WorldMapView);
			foreach (HotSpotWorldMapViewBase hotSpotWorldMapViewBase in componentsInChildren)
			{
				hotSpotWorldMapViewBase.Initialize();
			}
		}
		Vector2 outOfBoundsValue = m_DragController.GetOutOfBoundsValue(m_CamRoot.transform.position);
		m_CamRoot.transform.position = new Vector3(m_CamRoot.transform.position.x, m_currentHotSpot.transform.position.y, m_CamRoot.transform.position.z);
		outOfBoundsValue = m_DragController.GetOutOfBoundsValue(m_CamRoot.transform.position);
		m_CamRoot.transform.position = new Vector3(m_CamRoot.transform.position.x, m_CamRoot.transform.position.y + outOfBoundsValue.y, m_CamRoot.transform.position.z);
		for (int j = 0; j < Camera.allCameras.Length; j++)
		{
			Camera camera = Camera.allCameras[j];
			camera.eventMask = EventMask;
		}
		DIContainerInfrastructure.EventSystemStateManager.UpdateEventStars();
		if (m_currentHotSpot.Model.Data.UnlockState != HotspotUnlockState.ResolvedNew && m_startingHotSpot.IsCompleted())
		{
			return;
		}
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

	private void SetDragControllerSize()
	{
		m_DragController.SetDragAreaContainer(m_MapManager.m_DragControlInteractionSpace);
	}

	private IEnumerator Start()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("minicampaign_enter");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.IsInBattle = false;
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_PopupRoot.Leave();
		m_NewsLogic = new NewsLogic();
		m_NewsLogic.SetNewContentUpdateHandler();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 0u,
			showFriendshipEssence = true,
			showLuckyCoins = true,
			showSnoutlings = true
		}, true);
		m_LoadedLevels.Add("Window_BattlePreparation", false);
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_BattlePreparation", OnWindowBattlePreparationLoaded);
		m_LoadedLevels.Add("Window_FriendshipGate", false);
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_FriendshipGate", OnWindowFriendshipGateLoaded);
		m_LoadedLevels.Add("Menu_EventCampaign", false);
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Menu_EventCampaign", OnMenuLoaded);
		m_LoadedLevels.Add("Window_EventDetails", false);
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_EventDetails", OnEventDetailsLoadedLoaded);
		m_LoadedLevels.Add("Window_LeaderBoard_Tabs", false);
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_LeaderBoard_Tabs", OnLeaderBoardLoadedLoaded);
		m_LoadedLevels.Add("Window_News", false);
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_News", OnNewsScreenLoaded);
		StartCoroutine(UpdateAndStartGateTimers(2f));
		while (m_LoadedLevels.Values.Count((bool e) => !e) > 0)
		{
			yield return new WaitForEndOfFrame();
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveAllBars(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 0u,
			showFriendshipEssence = true,
			showLuckyCoins = true,
			showSnoutlings = true
		}, true);
		m_MiniCampaignMenuUI.ShowNewMarkerOnCampButton(DIContainerInfrastructure.GetCurrentPlayer().Data.HasNewOnWorlmap);
		InvokeRepeating("CheckForSpecialOffer", 1f, m_CheckForSpecialOfferFrequency);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("minicampaign_enter");
		LoadBirdsIntoScene();
		yield return StartCoroutine(m_MapManager.ActivateHotspots());
		while (!DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera.gameObject.activeInHierarchy)
		{
			yield return new WaitForEndOfFrame();
		}
		m_floatingTreasure.InitChestCampaign();
		m_MiniCampaignMenuUI.Enter();
		m_DragController.CalculateBounds();
		StartCoroutine(TryPlayWorldMapMusic());
		m_isInitialized = true;
		yield return StartCoroutine(StoppablePopupCoroutine());
		ContentLoader.Instance.CheckforRestartApp();
		RegisterEventHandlers();
	}

	private IEnumerator TryPlayWorldMapMusic()
	{
		bool waitForMainThemeEnd = !DIContainerInfrastructure.GetCoreStateMgr().m_EnterOnce;
		DIContainerInfrastructure.GetCoreStateMgr().m_EnterOnce = true;
		while (waitForMainThemeEnd && DIContainerInfrastructure.PrimaryMusicSource.isPlaying)
		{
			yield return new WaitForEndOfFrame();
		}
		string musicTitle = m_Model.BalancingData.MusicTitle;
		if (musicTitle != null)
		{
			DIContainerInfrastructure.AudioManager.PlayMusic(musicTitle);
		}
	}

	private void InventoryOfTypeChanged(InventoryItemType itemType, IInventoryItemGameData inventoryItemGameData)
	{
		if ((itemType == InventoryItemType.CraftingRecipes || itemType == InventoryItemType.Class) && inventoryItemGameData.ItemData.IsNew)
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.HasNewOnWorlmap = true;
			m_MiniCampaignMenuUI.ShowNewMarkerOnCampButton(DIContainerInfrastructure.GetCurrentPlayer().Data.HasNewOnWorlmap);
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			ContentLoader.Instance.CheckforRestartApp();
		}
		if (!paused)
		{
			CheckForSpecialOffer();
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

	private void HandleBackButton()
	{
		DebugLog.Log("Pressed Back Button: " + GetType());
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("back_button_pressed", string.Empty);
		m_Left = true;
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		DIContainerInfrastructure.GetCoreStateMgr().GotoWorldMap();
	}

	private void SynchBalancing(HotSpotWorldMapViewBase[] hotspots)
	{
		if (m_Model != null)
		{
			foreach (HotSpotWorldMapViewBase hotSpotWorldMapViewBase in hotspots)
			{
				hotSpotWorldMapViewBase.SyncWithMiniCampaign(m_Model);
			}
		}
	}

	private void OnMenuLoaded()
	{
		m_LoadedLevels["Menu_EventCampaign"] = true;
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(EventCampaignMenuUI));
		m_MiniCampaignMenuUI = @object as EventCampaignMenuUI;
		m_MiniCampaignMenuUI.SetStateMgr(this);
		m_MiniCampaignMenuUI.gameObject.SetActive(false);
		DebugLog.Log("MiniCampaign loaded!");
	}

	private void OnEventDetailsLoadedLoaded()
	{
		m_LoadedLevels["Window_EventDetails"] = true;
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(EventDetailUI));
		m_EventDetailUI = @object as EventDetailUI;
		m_EventDetailUI.SetStateMgr(this);
		m_EventDetailUI.gameObject.SetActive(false);
		DebugLog.Log("Window_EventDetails loaded!");
		m_LoadedLevels["Window_WorldShop"] = true;
	}

	private void OnLeaderBoardLoadedLoaded()
	{
		m_LoadedLevels["Window_LeaderBoard_Tabs"] = true;
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(LeaderboardUI));
		m_LeaderBoardUI = @object as LeaderboardUI;
		m_LeaderBoardUI.SetWorldMapStateMgr(this);
		m_LeaderBoardUI.gameObject.SetActive(false);
		DebugLog.Log("Window_LeaderBoard_Tabs loaded!");
		m_LoadedLevels["Window_LeaderBoard_Tabs"] = true;
	}

	private void OnNewsScreenLoaded()
	{
		m_LoadedLevels["Window_News"] = true;
		m_EventNews = UnityEngine.Object.FindObjectOfType(typeof(NewsUi)) as NewsUi;
		m_EventNews.SetStateMgr(this, m_NewsLogic);
		m_EventNews.gameObject.SetActive(false);
		DebugLog.Log("Window_News loaded!");
		m_LoadedLevels["Window_News"] = true;
	}

	private void OnEventPreviewScreenLoaded(EventManagerGameData model, string origin = null)
	{
		m_LoadedLevels["EventPreviewScreen"] = true;
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(EventPreviewUI));
		m_EventPreviewUI = @object as EventPreviewUI;
		m_EventPreviewUI.SetStateMgr(this);
		m_EventPreviewUI.gameObject.SetActive(false);
		DebugLog.Log("EventPreviewScreen loaded!");
		m_LoadedLevels["EventPreviewScreen"] = true;
		m_EventPreviewUI.SetModel(model);
		m_EventPreviewUI.Enter(false, origin);
	}

	private void OnWindowBattlePreparationLoaded()
	{
		m_LoadedLevels["Window_BattlePreparation"] = true;
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(BattlePreperationUI));
		m_battlePreperation = @object as BattlePreperationUI;
		m_battlePreperation.gameObject.SetActive(false);
		DebugLog.Log("BattlePreperationUI loaded!");
	}

	private void OnWindowFriendshipGateLoaded()
	{
		m_LoadedLevels["Window_FriendshipGate"] = true;
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(FriendshipGateUI));
		m_friendShipGate = @object as FriendshipGateUI;
		m_friendShipGate.gameObject.SetActive(false);
		DebugLog.Log("Window_FriendshipGate loaded!");
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
			if ((bool)m_currentHotSpot)
			{
				m_Birds[j].transform.position = m_currentHotSpot.transform.position + m_currentHotSpot.m_HotSpotPositions[j];
			}
			if ((bool)m_BirdAnimations[j]["Idle"])
			{
				m_BirdAnimations[j].Play("Idle");
			}
			if ((bool)m_currentHotSpot)
			{
				m_currentHotSpot.HandleMovingObjectVisibility(m_Birds[j].gameObject, m_currentHotSpot);
			}
			m_walking[j] = false;
		}
		if ((bool)m_currentHotSpot)
		{
			m_Ship.transform.position = m_currentHotSpot.transform.position + m_currentHotSpot.m_HotSpotPositions[0];
			m_AirShip.transform.position = m_currentHotSpot.transform.position + m_currentHotSpot.m_HotSpotPositions[0];
			m_Submarine.transform.position = m_currentHotSpot.transform.position + m_currentHotSpot.m_HotSpotPositions[0];
			m_currentHotSpot.HandleMovingObjectVisibility(m_Ship, m_currentHotSpot);
			m_currentHotSpot.HandleMovingObjectVisibility(m_AirShip, m_currentHotSpot);
			m_currentHotSpot.HandleMovingObjectVisibility(m_Submarine, m_currentHotSpot);
		}
	}

	public override Vector3 GetWorldBirdScale()
	{
		return m_WorldBirdScale;
	}

	public override void SetFriendshipGateHotspot(HotspotGameData hotspot)
	{
		m_friendShipGate.SetHotSpot(hotspot, this);
	}

	public override void SetNewHotSpot(HotSpotWorldMapViewBase newSpot, Action actionAfterWalkingDone, bool instantMove = false)
	{
		DebugLog.Log("Set new Hotspot started!");
		if ((newSpot == m_currentHotSpot && IsBirdWalking()) || m_MapManager == null)
		{
			return;
		}
		DebugLog.Log("Not Current Hotspot!");
		if (!m_inputEnabled)
		{
			return;
		}
		DebugLog.Log("Input Enabled!");
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
			DIContainerLogic.EventSystemService.TravelToMiniCampaignHotspot(DIContainerInfrastructure.GetCurrentPlayer(), newSpot.Model);
			DebugLog.Log("Set Hotspot: Written currentHotspot state and saved player profile! Calculating path");
			m_currentPathList = CalculatePath(m_currentHotSpot, newSpot);
			m_currentHotSpot = newSpot;
			for (int i = 0; i < m_Birds.Count; i++)
			{
				m_walking[i] = true;
				PathMovingService.Instance.WalkAlongPath(m_currentPathList, m_Birds[i], m_BirdAnimations[i], m_BirdSpeed, i, (float)(i + 1) * 0.2f, this, "WalkDone");
			}
			PathMovingService.Instance.WalkAlongPath(m_currentPathList, m_Ship, m_ShipAnimation, m_BirdSpeed, 0, 0.2f, this, "WalkDone", false, "Move");
			PathMovingService.Instance.WalkAlongPath(m_currentPathList, m_AirShip, m_AirShipAnimation, m_BirdSpeed, 0, 0.2f, this, "WalkDone", false, "Move");
			PathMovingService.Instance.WalkAlongPath(m_currentPathList, m_Submarine, m_SubmarineAnimation, m_BirdSpeed, 0, 0.2f, this, "WalkDone", false, "Move");
		}
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

	public new bool IsBirdWalking()
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

	public override void ShowEventDetailScreen(EventManagerGameData evt)
	{
		if (!IsBirdWalking() && evt != null && evt.IsValid && !DIContainerLogic.EventSystemService.IsEventTeasing(evt.Balancing))
		{
			m_EventDetailUI.gameObject.SetActive(true);
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
				OnEventPreviewScreenLoaded(model, origin);
			});
		}
		else
		{
			m_EventPreviewUI.SetModel(model);
			m_EventPreviewUI.Enter(false, origin);
		}
	}

	public override void ShowEventResultPopup()
	{
		if (!IsBirdWalking() && DIContainerLogic.EventSystemService.IsCurrentEventAvailable(DIContainerInfrastructure.GetCurrentPlayer()) && DIContainerLogic.EventSystemService.IsWaitingForConfirmation(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData))
		{
			base.IsEventResultRunning = true;
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveNonInteractableTooltip();
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddLevel("Popup_EventFinished", true, false, delegate
			{
			});
		}
	}

	public override void ShowLeaderBoardScreen(WorldBossTeamData ownTeam = null, WorldBossTeamData enemyTeam = null, EventDetailUI detailUi = null)
	{
		if (DIContainerLogic.EventSystemService.IsCurrentEventAvailable(DIContainerInfrastructure.GetCurrentPlayer()) && !DIContainerLogic.EventSystemService.IsEventTeasing(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Balancing))
		{
			m_LeaderBoardUI.SetEventModel(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData);
			m_LeaderBoardUI.Enter(ownTeam, enemyTeam, false, detailUi);
		}
	}

	public void ShowBattlePreperationScreenForEvent(EventItemGameData eventItem, EventPlacementBalancingData placement)
	{
	}

	public override void ShowBattlePreperationScreen()
	{
		if (!m_battlePreperation.m_Entered && m_MapManager != null)
		{
			m_battlePreperation.SetMiniCampaignHotSpot(((HotSpotWorldMapViewBattle)m_currentHotSpot).Model, this);
			m_battlePreperation.Enter();
		}
	}

	public void UpdateCollectionProgressBar()
	{
		m_MiniCampaignMenuUI.UpdateCollectionBar();
	}

	public override void ShowFriendshipGateScreen(Action actionOnReturn, HotspotGameData hotspot)
	{
		m_friendShipGate.SetReturnAction(actionOnReturn);
		m_friendShipGate.Enter();
	}

	public override void EnableInput(bool flag)
	{
		m_inputEnabled = flag;
		m_DragController.SetActiveDepth(flag, 1);
	}

	public new bool IsShowContentPossible()
	{
		return !IsBirdWalking();
	}

	private void OnDisable()
	{
		DeRegisterEventHandlers();
		DIContainerLogic.GetResourceNodeManager().ClearSpotList();
		if ((bool)DIContainerInfrastructure.GetCoreStateMgr())
		{
			DIContainerInfrastructure.GetCoreStateMgr().UnloadUnusedAssets();
		}
	}

	public override void ResetBirdPositions()
	{
		m_currentHotSpot = m_startingHotSpot.GetHotspotWorldMapView(DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.CurrentHotspotGameData.BalancingData.NameId);
	}

	private IEnumerator UpdateAndStartGateTimers(float delay, float extraDelay = 0.3f)
	{
		yield return new WaitForSeconds(delay);
		int unlocked = DIContainerInfrastructure.EventSystemStateManager.GetNumberOfCampaignGateUnlocks();
		bool nextGateInLineFound = false;
		for (int index = 0; index < m_gates.Length; index++)
		{
			HotSpotWorldMapViewPathNode anyGate = m_gates[index];
			yield return new WaitForSeconds(extraDelay);
			if (anyGate is HotSpotWorldMapViewTimedPathNode)
			{
				HotSpotWorldMapViewTimedPathNode gate = anyGate as HotSpotWorldMapViewTimedPathNode;
				Requirement req = gate.Model.BalancingData.EnterRequirements.Where((Requirement r) => r.RequirementType == RequirementType.HaveItem && r.NameId == "event_gate_unlock").FirstOrDefault();
				int requiredUnlockItems = (int)req.Value;
				bool timerOver = requiredUnlockItems <= unlocked;
				if (!timerOver && !nextGateInLineFound)
				{
					nextGateInLineFound = true;
					gate.m_timerVisible = true;
					gate.SetAnimationState(HotspotAnimationState.Active);
				}
				else if (timerOver)
				{
					gate.m_timerVisible = true;
					gate.SetAnimationState(HotspotAnimationState.Open);
				}
				else
				{
					gate.m_timerVisible = false;
					gate.SetAnimationState(HotspotAnimationState.Inactive);
				}
			}
			else if (anyGate is HotSpotWorldMapViewCampaignStarPathNode)
			{
				HotSpotWorldMapViewCampaignStarPathNode stargate = anyGate as HotSpotWorldMapViewCampaignStarPathNode;
				stargate.SetStarModelAndState();
			}
		}
	}

	public override bool ShowNewsUi(NewsUi.NewsUiState startingState = NewsUi.NewsUiState.Events)
	{
		m_EventNews.Enter(startingState);
		return true;
	}
}

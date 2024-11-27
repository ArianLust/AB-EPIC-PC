using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChronicleCaveStateMgr : BaseLocationStateManager
{
	public List<ChronicleCaveFloorSlot> m_Floors = new List<ChronicleCaveFloorSlot>();

	public Transform m_CamRoot;

	public GameObject m_RedKeyBubble;

	private HotSpotWorldMapViewBase m_currentHotSpot;

	public ContainerControl m_InteractionSpace;

	public Vector3 m_WorldBirdScale = new Vector3(0.4f, 0.4f, 1f);

	private int m_CurrentFloorIndex;

	private Dictionary<string, bool> m_LoadedLevels = new Dictionary<string, bool>();

	[SerializeField]
	private Transform m_OffscreenPositionRoot;

	[SerializeField]
	private FloatingTreasure m_floatingTreasure;

	public Vector3 m_CurrentFloorPosition = new Vector3(0f, 0f, 0f);

	public Vector3 m_UpperFloorPosition = new Vector3(0f, 1024f, 0f);

	public Vector3 m_LowerFloorPosition = new Vector3(0f, -1024f, 0f);

	public ChronicleCaveFloorSlot m_UpperFloor;

	public ChronicleCaveFloorSlot m_LowerFloor;

	public ChronicleCaveFloorSlot m_CurrentFloor;

	public Transform m_FloorShiftRoot;

	public Transform m_FloorRoot;

	public CHMotionTween m_FloorFirstTween;

	public DragController m_DragController;

	public LayerMask EventMask = -1;

	public Transform m_CharacterRoot;

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

	public CharacterControllerWorldMap m_WorldMapCharacterController;

	public UISwipeInputTrigger m_SwipeHandler;

	private HotSpotWorldMapViewBase m_CurrentHotSpot;

	private List<HotSpotWorldMapViewBase> m_currentPathList = new List<HotSpotWorldMapViewBase>();

	private bool[] m_walking;

	public List<Animation> m_BirdAnimations;

	public float[] m_movementStartDelay;

	public float m_BirdSpeed = 100f;

	public Vector3[] m_HotSpotPositions;

	[HideInInspector]
	public BattlePreperationUI m_battlePreperation;

	[HideInInspector]
	public WorldMapShopMenuUI m_WorkShopUI;

	[HideInInspector]
	public ChronicleCaveMenuUI m_ChronicleCaveMenuUI;

	public LoadingLevelState m_LoadedCurrentFloor;

	private bool m_Loading;

	public float m_SwipeHalfDuration = 1f;

	private ChronicleCaveGameData m_Model;

	public ChronicleCaveFloorGameData m_CurrentCronicleCaveFloor;

	private bool m_inputEnabled = true;

	private Action m_ActionAfterWalkingDone;

	private bool m_Initialized;

	private bool m_FirstEnter;

	private Dictionary<int, FriendProgressIndicator> m_FriendProgresses = new Dictionary<int, FriendProgressIndicator>();

	private bool m_Left;

	private bool m_TransitionAnimationRunning;

	private bool m_WalkingToNextCave;

	public int CurrentViewedFloor { get; private set; }

	public override IMapUI WorldMenuUI
	{
		get
		{
			return m_ChronicleCaveMenuUI;
		}
	}

	private void Awake()
	{
		DIContainerInfrastructure.AdService.AddPlacement(BattlePreperationUI.BUFF_PLACEMENT);
		DIContainerInfrastructure.GetCoreStateMgr().m_DailyLoginUi.StopEnterCoroutine();
		LoadBirdsIntoScene();
		for (int i = 0; i < Camera.allCameras.Length; i++)
		{
			Camera camera = Camera.allCameras[i];
			camera.eventMask = EventMask;
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_SceneryAudioListener = base.transform.GetComponentInChildren<AudioListener>();
		DIContainerInfrastructure.GetCoreStateMgr().m_ChronicleCave = true;
		DIContainerInfrastructure.GetCoreStateMgr().m_IsWithinPvP = false;
		DIContainerInfrastructure.GetCoreStateMgr().m_EventCampaign = false;
		SetModel(DIContainerInfrastructure.GetCurrentPlayer().ChronicleCaveGameData);
		m_FeatureUnlockCoroutineInstance = HandleFeatureUnlocksAndLevelUps();
	}

	private IEnumerator Start()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.IsInBattle = false;
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("chronicle_cave_enter");
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_PopupRoot.Leave();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 0u,
			showFriendshipEssence = true,
			showLuckyCoins = true,
			showSnoutlings = true
		}, true);
		while (m_Floors.Count < DIContainerBalancing.Service.GetBalancingDataList<ChronicleCaveFloorBalancingData>().Count)
		{
			yield return new WaitForEndOfFrame();
		}
		for (int i = 1; i < m_Floors.Count; i++)
		{
			ChronicleCaveFloorSlot lastFloor = m_Floors[i - 1];
			ChronicleCaveFloorSlot floor = m_Floors[i];
			if (floor.m_EnteringHotSpot != null)
			{
				floor.m_EnteringNode.m_litePath = lastFloor.m_LeavingPath;
				if ((bool)lastFloor.m_LeavingNode)
				{
					lastFloor.m_LeavingNode.m_outgoingHotSpots.Add(floor.m_EnteringNode);
					floor.m_EnteringNode.m_previousHotSpot = lastFloor.m_LeavingNode;
				}
				else
				{
					floor.m_EnteringNode.m_previousHotSpot = lastFloor.m_LeavingHotSpot;
					lastFloor.m_LeavingHotSpot.m_outgoingHotSpots.Add(floor.m_EnteringNode);
				}
			}
		}
		m_InteractionSpace.m_Size.y = m_UpperFloorPosition.y * (float)m_Floors.Count;
		m_InteractionSpace.transform.localScale = new Vector3(m_InteractionSpace.transform.localScale.x, m_InteractionSpace.m_Size.y, m_InteractionSpace.transform.localScale.z);
		m_InteractionSpace.transform.localPosition = new Vector3(m_InteractionSpace.transform.localPosition.x, (m_InteractionSpace.m_Size.y - m_UpperFloorPosition.y) / 2f, m_InteractionSpace.transform.localPosition.z);
		m_DragController.CalculateBounds();
		m_currentHotSpot = m_Floors[m_Model.Data.CurrentBirdFloorIndex].m_EnteringHotSpot.GetHotspotWorldMapView(m_Model.CurrentHotspotGameData.BalancingData.NameId);
		PlaceBirdsAndVessels(m_currentHotSpot.transform, m_currentHotSpot);
		m_CamRoot.transform.position = new Vector3(m_CamRoot.transform.position.x, Mathf.Clamp(m_currentHotSpot.transform.position.y, 0f, m_UpperFloorPosition.y * (float)m_Floors.Count - m_UpperFloorPosition.y), m_CamRoot.transform.position.z);
		DIContainerInfrastructure.AudioManager.PlayMusic("music_chroniclecave");
		m_LoadedLevels.Add("Window_BattlePreparation", false);
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_BattlePreparation", OnWindowBattlePreparationLoaded);
		m_LoadedLevels.Add("Menu_ChronicleCave", false);
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Menu_ChronicleCave", OnMenuWorldMapLoaded);
		while (m_LoadedLevels.Values.Count((bool e) => !e) > 0)
		{
			yield return new WaitForEndOfFrame();
		}
		m_ChronicleCaveMenuUI.ShowNewMarkerOnCampButton(DIContainerInfrastructure.GetCurrentPlayer().Data.HasNewOnWorlmap);
		InvokeRepeating("CheckForSpecialOffer", 1f, m_CheckForSpecialOfferFrequency);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("chronicle_cave_enter");
		RegisterEventHandlers();
		m_Initialized = true;
		#if !ENABLE_IL2CPP
		m_isInitialized = true; // set m_isInitialized to true earlier than usual to allow clicking the levels while loading
		#endif
		SetUpperMostDoorOpened(false);
		for (int j = 0; j < m_Floors.Count; j++)
		{
			ChronicleCaveFloorSlot floor2 = m_Floors[j];
			yield return StartCoroutine(floor2.ActivateHotspots());
		}
		while (m_TransitionAnimationRunning)
		{
			yield return new WaitForEndOfFrame();
		}
		DebugLog.Log("Try to start Intro Cutscene!");
		for (int index = 0; index < DIContainerInfrastructure.GetCurrentPlayer().Data.PendingFeatureUnlocks.Count; index++)
		{
			string pendingUnlocks = DIContainerInfrastructure.GetCurrentPlayer().Data.PendingFeatureUnlocks[index];
			if (!pendingUnlocks.StartsWith("skill_env_cc_floor_"))
			{
				continue;
			}
			DebugLog.Log("Found unlock feature Item: " + pendingUnlocks + " starting cutscene for: " + m_Model.CurrentFloorIndex);
			ActionTree introCutsene = m_Floors[Mathf.Max(0, DIContainerInfrastructure.GetCurrentPlayer().ChronicleCaveGameData.Data.CurrentBirdFloorIndex)].GetIntroCutscene();
			if (!introCutsene)
			{
				DebugLog.Log("No cutscene found!");
				continue;
			}
			if (DIContainerInfrastructure.GetCoreStateMgr().m_DisableStorySequences)
			{
				DebugLog.Log("No cutscene enabled!");
				introCutsene.isFinished = true;
				continue;
			}
			DebugLog.Log("Start Intro Cutscene!");
			EnableInput(false);
			introCutsene.Load(introCutsene.startNode);
			while (!introCutsene.isFinished && introCutsene.node != null)
			{
				yield return new WaitForEndOfFrame();
			}
			EnableInput(true);
		}
		yield return StartCoroutine(StoppablePopupCoroutine());
		if ((bool)m_Floors[Mathf.Max(0, DIContainerInfrastructure.GetCurrentPlayer().ChronicleCaveGameData.Data.CurrentBirdFloorIndex)])
		{
			m_Floors[Mathf.Max(0, DIContainerInfrastructure.GetCurrentPlayer().ChronicleCaveGameData.Data.CurrentBirdFloorIndex)].InstantiateRedKeyBubble();
		}
		Invoke("InvokableHandleFeatureUnlocksAndLevelUps", 2f);
		Invoke("SetHotspotFriendProgress", 1.5f);
		AchievementData playerRecord = DIContainerInfrastructure.GetCurrentPlayer().Data.AchievementTracking;
		if (!playerRecord.ChronicleCavesCompletedAchieved && DIContainerInfrastructure.GetCurrentPlayer().ChronicleCaveGameData.Data.CronicleCaveFloors.Count > DIContainerBalancing.Service.GetBalancingData<AchievementBalancingData>("completeCaves").Value)
		{
			string achievementId = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("completeCaves");
			if (!string.IsNullOrEmpty(achievementId))
			{
				DIContainerInfrastructure.GetAchievementService().ReportUnlocked(achievementId);
				playerRecord.ChronicleCavesCompletedAchieved = true;
			}
		}
		ContentLoader.Instance.CheckforRestartApp();
		m_isInitialized = true;
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

	private void InventoryOfTypeChanged(InventoryItemType itemType, IInventoryItemGameData inventoryItemGameData)
	{
		if ((itemType == InventoryItemType.CraftingRecipes || itemType == InventoryItemType.Class) && inventoryItemGameData.ItemData.IsNew)
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.HasNewOnWorlmap = true;
			m_ChronicleCaveMenuUI.ShowNewMarkerOnCampButton(DIContainerInfrastructure.GetCurrentPlayer().Data.HasNewOnWorlmap);
		}
	}

	private void InvokableHandleFeatureUnlocksAndLevelUps()
	{
		StartCoroutine(StoppablePopupCoroutine());
	}

	private void SetHotspotFriendProgress()
	{
		foreach (PublicPlayerData value in DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.PublicPlayerDatas.Values)
		{
			InstatiateHotspotFriendProgress(value);
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
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(0);
		DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.InventoryOfTypeChanged -= InventoryOfTypeChanged;
	}

	private void PublicPlayersRefreshed()
	{
		foreach (PublicPlayerData value in DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.PublicPlayerDatas.Values)
		{
			InstatiateHotspotFriendProgress(value);
		}
	}

	private void InstatiateHotspotFriendProgress(PublicPlayerData publicPlayerData)
	{
		if (!publicPlayerData.LocationProgress.ContainsKey(LocationType.ChronicleCave) || publicPlayerData.LocationProgress[LocationType.ChronicleCave] == 0)
		{
			return;
		}
		if (m_FriendProgresses.ContainsKey(publicPlayerData.LocationProgress[LocationType.ChronicleCave]))
		{
			m_FriendProgresses[publicPlayerData.LocationProgress[LocationType.ChronicleCave]].SetModel(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends[publicPlayerData.SocialId]);
			return;
		}
		HotspotBalancingData hotspotBalancingData = null;
		List<ChronicleCaveFloorBalancingData> list = DIContainerBalancing.Service.GetBalancingDataList<ChronicleCaveFloorBalancingData>().ToList();
		ChronicleCaveFloorBalancingData chronicleCaveFloorBalancingData = null;
		int i;
		for (i = 0; i < list.Count; i++)
		{
			bool flag = false;
			for (int j = 0; j < list[i].ChronicleCaveHotspotIds.Count; j++)
			{
				string nameId = list[i].ChronicleCaveHotspotIds[j];
				hotspotBalancingData = DIContainerBalancing.Service.GetBalancingData<ChronicleCaveHotspotBalancingData>(nameId);
				if (hotspotBalancingData.ProgressId == publicPlayerData.LocationProgress[LocationType.ChronicleCave])
				{
					chronicleCaveFloorBalancingData = list[i];
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		if (chronicleCaveFloorBalancingData != null && hotspotBalancingData != null)
		{
			FriendProgressIndicator friendProgressIndicator = UnityEngine.Object.Instantiate(m_FriendProgressIndicatorPrefab);
			m_Floors[i].SetFriendProgressMarker(chronicleCaveFloorBalancingData, friendProgressIndicator, hotspotBalancingData);
			m_FriendProgresses.Add(publicPlayerData.LocationProgress[LocationType.ChronicleCave], friendProgressIndicator);
			friendProgressIndicator.SetModel(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends[publicPlayerData.SocialId]);
		}
	}

	private void OnMenuWorldMapLoaded()
	{
		m_LoadedLevels["Menu_ChronicleCave"] = true;
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(ChronicleCaveMenuUI));
		m_ChronicleCaveMenuUI = @object as ChronicleCaveMenuUI;
		m_ChronicleCaveMenuUI.SetStateMgr(this);
		m_ChronicleCaveMenuUI.gameObject.SetActive(false);
		DebugLog.Log("ChronicleCaveMenuUI loaded!");
		m_ChronicleCaveMenuUI.Enter();
	}

	private void OnWindowBattlePreparationLoaded()
	{
		m_LoadedLevels["Window_BattlePreparation"] = true;
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(BattlePreperationUI));
		m_battlePreperation = @object as BattlePreperationUI;
		m_battlePreperation.gameObject.SetActive(false);
		DebugLog.Log("BattlePreperationUI loaded!");
	}

	private void LateUpdate()
	{
		if (m_Initialized)
		{
			CurrentViewedFloor = Mathf.Max(1, Mathf.RoundToInt(m_CamRoot.position.y / m_UpperFloorPosition.y) + 1);
		}
	}

	public ChronicleCaveStateMgr SetModel(ChronicleCaveGameData model)
	{
		m_Model = model;
		InitAll();
		return this;
	}

	private void LoadBirdsIntoScene()
	{
		if (DIContainerInfrastructure.GetCurrentPlayer().Birds != null)
		{
			for (int i = 0; i < DIContainerInfrastructure.GetCurrentPlayer().Birds.Count; i++)
			{
				BirdGameData birdGameData = DIContainerInfrastructure.GetCurrentPlayer().Birds[i];
				CharacterControllerWorldMap characterControllerWorldMap = UnityEngine.Object.Instantiate(m_WorldMapCharacterController, m_CharacterRoot.position, m_CharacterRoot.rotation) as CharacterControllerWorldMap;
				characterControllerWorldMap.SetModel(birdGameData);
				GameObject gameObject = new GameObject(birdGameData.BalancingData.AssetId);
				gameObject.AddComponent<CHMotionTween>();
				gameObject.transform.position = m_CharacterRoot.position;
				gameObject.transform.parent = m_CharacterRoot;
				characterControllerWorldMap.transform.parent = gameObject.transform;
				characterControllerWorldMap.transform.localPosition = Vector3.zero;
				characterControllerWorldMap.transform.localScale = m_WorldBirdScale;
				m_Birds.Add(gameObject);
				m_BirdAnimations.Add(characterControllerWorldMap.m_AssetController.GetComponent<Animation>());
			}
		}
		PlaceBirdsAndVessels(m_OffscreenPositionRoot, null);
	}

	private void PlaceBirdsAndVessels(Transform positionTransform, HotSpotWorldMapViewBase hotspot)
	{
		m_walking = new bool[m_Birds.Count];
		for (int i = 0; i < m_Birds.Count; i++)
		{
			m_Birds[i].transform.position = positionTransform.position + m_HotSpotPositions[i];
			if ((bool)m_BirdAnimations[i]["Idle"])
			{
				m_BirdAnimations[i].Play("Idle");
			}
			m_walking[i] = false;
			if ((bool)m_currentHotSpot)
			{
				m_currentHotSpot.HandleMovingObjectVisibility(m_Birds[i].gameObject, m_currentHotSpot);
			}
		}
		if ((bool)m_currentHotSpot)
		{
			m_Ship.transform.position = positionTransform.position + m_currentHotSpot.m_HotSpotPositions[0];
			m_AirShip.transform.position = positionTransform.transform.position + m_currentHotSpot.m_HotSpotPositions[0];
			m_Submarine.transform.position = positionTransform.transform.position + m_currentHotSpot.m_HotSpotPositions[0];
			m_currentHotSpot.HandleMovingObjectVisibility(m_Ship, m_currentHotSpot);
			m_currentHotSpot.HandleMovingObjectVisibility(m_AirShip, m_currentHotSpot);
			m_currentHotSpot.HandleMovingObjectVisibility(m_Submarine, m_currentHotSpot);
		}
	}

	public void InitAll()
	{
		InitNextIfDone(0);
	}

	public void InitNextIfDone(int floorIndex)
	{
		m_CurrentCronicleCaveFloor = m_Model.GetFloorAndCreateIfNext(floorIndex);
		SceneManager.LoadScene(m_CurrentCronicleCaveFloor.BalancingData.BackgroundId, LoadSceneMode.Additive);
	}

	public void InitNextProxyFloor(int floorIndex)
	{
		SceneManager.LoadScene("ChronicleCave_Floor_Preview", LoadSceneMode.Additive);
	}

	public IEnumerator InitOnlyThree()
	{
		m_Model.CurrentFloorIndex = m_Model.Data.CurrentBirdFloorIndex;
		m_CurrentCronicleCaveFloor = m_Model.GetFloorAndCreateIfNext(m_Model.CurrentFloorIndex);
		string levelToLoad = m_CurrentCronicleCaveFloor.BalancingData.BackgroundId + "_" + m_CurrentCronicleCaveFloor.BalancingData.TemplateId.ToString("00");
		yield return SceneManager.LoadSceneAsync(levelToLoad, LoadSceneMode.Additive);
		m_LoadedCurrentFloor = LoadingLevelState.UpperFloor;
		m_currentHotSpot = m_CurrentFloor.m_EnteringHotSpot.GetHotspotWorldMapView(m_Model.CurrentHotspotGameData.BalancingData.NameId);
		PlaceBirdsAndVessels(m_currentHotSpot.transform, m_currentHotSpot);
		m_CurrentCronicleCaveFloor = m_Model.GetFloorAndCreateIfNext(m_Model.CurrentFloorIndex + 1);
		levelToLoad = m_CurrentCronicleCaveFloor.BalancingData.BackgroundId + "_" + m_CurrentCronicleCaveFloor.BalancingData.TemplateId.ToString("00");
		yield return SceneManager.LoadSceneAsync(levelToLoad, LoadSceneMode.Additive);
		if (m_Model.CurrentFloorIndex > 0)
		{
			m_LoadedCurrentFloor = LoadingLevelState.LowerFloor;
			m_CurrentCronicleCaveFloor = m_Model.GetFloorAndCreateIfNext(m_Model.CurrentFloorIndex - 1);
			levelToLoad = m_CurrentCronicleCaveFloor.BalancingData.BackgroundId + "_" + m_CurrentCronicleCaveFloor.BalancingData.TemplateId.ToString("00");
			yield return SceneManager.LoadSceneAsync(levelToLoad, LoadSceneMode.Additive);
		}
		m_LoadedCurrentFloor = LoadingLevelState.CurrentFloor;
		m_SwipeHandler.SwipeDown += m_SwipeHandler_SwipeDown;
		m_SwipeHandler.SwipeUp += m_SwipeHandler_SwipeUp;
		int i = 0;
		m_CurrentCronicleCaveFloor = m_Model.GetFloorAndCreateIfNext(m_Model.CurrentFloorIndex);
		for (int index = 0; index < m_Model.ChronicleCaveFloorGameDatas.Count; index++)
		{
			ChronicleCaveFloorGameData cave = m_Model.ChronicleCaveFloorGameDatas[index];
			DebugLog.Log("ChronicleCave #" + i + " Balancing#" + cave.BalancingData.NameId);
			foreach (HotspotGameData hotspot in cave.HotspotGameDatas.Values)
			{
				DebugLog.Log(" Hotspot #" + hotspot.BalancingData.NameId);
			}
			i++;
		}
	}

	private void m_SwipeHandler_SwipeUp()
	{
		if (m_Model.CurrentFloorIndex > 0)
		{
			StartCoroutine(FloorDown());
		}
	}

	private void m_SwipeHandler_SwipeDown()
	{
		StartCoroutine(FloorUp());
	}

	public IEnumerator FloorUp()
	{
		if (!m_Model.GetFloor(m_Model.CurrentFloorIndex).IsFinished() || m_Loading || IsBirdWalking())
		{
			yield break;
		}
		m_Loading = true;
		if (!m_Model.SwitchToFloor(m_Model.CurrentFloorIndex + 1))
		{
			m_Loading = false;
			yield break;
		}
		m_CurrentFloorIndex++;
		ChronicleCaveFloorGameData newFloor = m_Model.GetFloorAndCreateIfNext(m_Model.CurrentFloorIndex + 1);
		if (newFloor == null)
		{
			m_Loading = false;
			yield break;
		}
		m_FloorRoot.transform.localPosition += m_UpperFloorPosition;
		m_UpperFloor.transform.localPosition -= m_UpperFloorPosition;
		if ((bool)m_LowerFloor)
		{
			m_LowerFloor.transform.localPosition -= m_UpperFloorPosition;
		}
		m_CurrentFloor.transform.localPosition -= m_UpperFloorPosition;
		ChronicleCaveFloorSlot floorToDestroy = m_LowerFloor;
		m_LowerFloor = m_CurrentFloor;
		m_CurrentFloor = m_UpperFloor;
		m_LoadedCurrentFloor = LoadingLevelState.UpperFloor;
		m_FloorFirstTween.m_EndOffset = -m_FloorRoot.transform.localPosition;
		m_FloorFirstTween.m_DurationInSeconds = m_SwipeHalfDuration;
		m_FloorFirstTween.Play();
		float currentTime = Time.realtimeSinceStartup;
		if (m_CurrentFloor.GetModel().Data.FloorId == m_Model.Data.CurrentBirdFloorIndex)
		{
			m_currentHotSpot = m_CurrentFloor.m_EnteringHotSpot.GetHotspotWorldMapView(m_Model.CurrentHotspotGameData.BalancingData.NameId);
			PlaceBirdsAndVessels(m_currentHotSpot.transform, m_currentHotSpot);
		}
		m_CurrentCronicleCaveFloor = newFloor;
		string levelToLoad = newFloor.BalancingData.BackgroundId + "_" + newFloor.BalancingData.TemplateId.ToString("00");
		yield return SceneManager.LoadSceneAsync(levelToLoad, LoadSceneMode.Additive);
		yield return Resources.UnloadUnusedAssets();
		float timeDone = Time.realtimeSinceStartup - currentTime;
		if (timeDone < m_SwipeHalfDuration)
		{
			yield return new WaitForSeconds(m_SwipeHalfDuration - timeDone);
		}
		m_LoadedCurrentFloor = LoadingLevelState.CurrentFloor;
		if (m_CurrentFloor.GetModel().Data.FloorId != m_Model.Data.CurrentBirdFloorIndex)
		{
			m_currentHotSpot = null;
			PlaceBirdsAndVessels(m_OffscreenPositionRoot, null);
		}
		if ((bool)floorToDestroy)
		{
			UnityEngine.Object.Destroy(floorToDestroy.gameObject);
		}
		m_Loading = false;
		int i = 0;
		for (int index = 0; index < m_Model.ChronicleCaveFloorGameDatas.Count; index++)
		{
			ChronicleCaveFloorGameData cave = m_Model.ChronicleCaveFloorGameDatas[index];
			DebugLog.Log("ChronicleCave #" + i + " Balancing#" + cave.BalancingData.NameId);
			foreach (HotspotGameData hotspot in cave.HotspotGameDatas.Values)
			{
				DebugLog.Log(" Hotspot #" + hotspot.BalancingData.NameId);
			}
			i++;
		}
	}

	public override ChronicleCaveFloorGameData GetCurrentFloor()
	{
		return m_CurrentCronicleCaveFloor;
	}

	public IEnumerator FloorDown()
	{
		if (m_Loading || IsBirdWalking())
		{
			yield break;
		}
		m_Loading = true;
		if (!m_Model.SwitchToFloor(m_Model.CurrentFloorIndex - 1))
		{
			m_Loading = false;
			yield break;
		}
		m_CurrentFloorIndex--;
		ChronicleCaveFloorGameData newFloor = m_Model.GetFloorAndCreateIfNext(m_Model.CurrentFloorIndex - 1);
		m_FloorRoot.transform.localPosition += m_LowerFloorPosition;
		m_UpperFloor.transform.localPosition -= m_LowerFloorPosition;
		m_LowerFloor.transform.localPosition -= m_LowerFloorPosition;
		m_CurrentFloor.transform.localPosition -= m_LowerFloorPosition;
		ChronicleCaveFloorSlot floorToDestroy = m_UpperFloor;
		m_UpperFloor = m_CurrentFloor;
		m_CurrentFloor = m_LowerFloor;
		m_LoadedCurrentFloor = LoadingLevelState.LowerFloor;
		if (m_CurrentFloor.GetModel().Data.FloorId == m_Model.Data.CurrentBirdFloorIndex)
		{
			m_currentHotSpot = m_CurrentFloor.m_EnteringHotSpot.GetHotspotWorldMapView(m_Model.CurrentHotspotGameData.BalancingData.NameId);
			PlaceBirdsAndVessels(m_currentHotSpot.transform, m_currentHotSpot);
		}
		m_FloorFirstTween.m_EndOffset = -m_FloorRoot.transform.localPosition;
		m_FloorFirstTween.m_DurationInSeconds = m_SwipeHalfDuration;
		m_FloorFirstTween.Play();
		float currentTime = Time.realtimeSinceStartup;
		if (newFloor != null)
		{
			m_CurrentCronicleCaveFloor = newFloor;
			string sceneNameToLoad = newFloor.BalancingData.BackgroundId + "_" + newFloor.BalancingData.TemplateId.ToString("00");
			yield return SceneManager.LoadSceneAsync(sceneNameToLoad, LoadSceneMode.Additive);
		}
		else
		{
			m_LowerFloor = null;
		}
		yield return Resources.UnloadUnusedAssets();
		float timeDone = Time.realtimeSinceStartup - currentTime;
		if (timeDone < m_SwipeHalfDuration)
		{
			yield return new WaitForSeconds(m_SwipeHalfDuration - timeDone);
		}
		m_LoadedCurrentFloor = LoadingLevelState.CurrentFloor;
		if (m_CurrentFloor.GetModel().Data.FloorId != m_Model.Data.CurrentBirdFloorIndex)
		{
			m_currentHotSpot = null;
			PlaceBirdsAndVessels(m_OffscreenPositionRoot, null);
		}
		UnityEngine.Object.Destroy(floorToDestroy.gameObject);
		yield return Resources.UnloadUnusedAssets();
		m_Loading = false;
		int i = 0;
		for (int index = 0; index < m_Model.ChronicleCaveFloorGameDatas.Count; index++)
		{
			ChronicleCaveFloorGameData cave = m_Model.ChronicleCaveFloorGameDatas[index];
			DebugLog.Log("ChronicleCave #" + i + " Balancing#" + cave.BalancingData.NameId);
			foreach (HotspotGameData hotspot in cave.HotspotGameDatas.Values)
			{
				DebugLog.Log(" Hotspot #" + hotspot.BalancingData.NameId);
			}
			i++;
		}
	}

	public override void EnableInput(bool enable)
	{
		m_inputEnabled = enable;
		m_DragController.SetActiveDepth(enable, 1);
	}

	public override Vector3 GetWorldBirdScale()
	{
		return m_WorldBirdScale;
	}

	public override void SetNewHotSpot(HotSpotWorldMapViewBase newSpot, Action actionAfterWalkingDone, bool instantMove = false)
	{
		DebugLog.Log("Set new Hotspot started!");
		if (newSpot == m_currentHotSpot && IsBirdWalking())
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
			DIContainerLogic.ChronicleCaveService.TravelToHotSpot(DIContainerInfrastructure.GetCurrentPlayer(), newSpot.ChronicleCaveFloor, newSpot.Model);
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

	private List<HotSpotWorldMapViewBase> CalculatePath(HotSpotWorldMapViewBase start, HotSpotWorldMapViewBase end)
	{
		List<HotSpotWorldMapViewBase> list = new List<HotSpotWorldMapViewBase>();
		start.CalculatePath(start, end, ref list);
		list.Reverse();
		return list;
	}

	public void WalkDone(object o)
	{
		int num = (int)o;
		DebugLog.Warn("WalkDone with index " + num + " at " + Time.time);
		m_walking[num] = false;
		CheckWalkingBirds();
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

	public override bool IsShowContentPossible()
	{
		return !IsBirdWalking();
	}

	private void OnDisable()
	{
		DeRegisterEventHandlers();
		if ((bool)DIContainerInfrastructure.GetCoreStateMgr())
		{
			DIContainerInfrastructure.GetCoreStateMgr().UnloadUnusedAssets();
		}
	}

	public void SetUpperMostDoorOpened(bool opened)
	{
		GameObject gate = m_Floors[m_Model.ChronicleCaveFloorGameDatas.Count - 1].GetGate();
		if ((bool)gate)
		{
			if (opened)
			{
				gate.PlayAnimationOrAnimatorState("FloorGate_SetOpen");
			}
			else
			{
				gate.PlayAnimationOrAnimatorState("FloorGate_SetClosed");
			}
		}
	}

	public IEnumerator PlayGotoNextCaveAnimation()
	{
		m_TransitionAnimationRunning = true;
		base.BlockFeatureUnlocks = true;
		m_WalkingToNextCave = true;
		int nextCave = DIContainerInfrastructure.GetCurrentPlayer().ChronicleCaveGameData.Data.CronicleCaveFloors.Count;
		AchievementData playerRecord = DIContainerInfrastructure.GetCurrentPlayer().Data.AchievementTracking;
		if (!playerRecord.ChronicleCavesCompletedAchieved && nextCave > DIContainerBalancing.Service.GetBalancingData<AchievementBalancingData>("completeCaves").Value)
		{
			string achievementId = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("completeCaves");
			if (!string.IsNullOrEmpty(achievementId))
			{
				DIContainerInfrastructure.GetAchievementService().ReportUnlocked(achievementId);
				playerRecord.ChronicleCavesCompletedAchieved = true;
			}
		}
		CHMotionTween tween = m_CamRoot.GetComponentInChildren<CHMotionTween>();
		if (DIContainerBalancing.Service.GetBalancingDataList<ChronicleCaveFloorBalancingData>().Count == nextCave && m_Floors.Last().m_LeavingHotSpot.Model.Data.UnlockState == HotspotUnlockState.ResolvedNew)
		{
			SetUpperMostDoorOpened(true);
			base.BlockFeatureUnlocks = false;
			m_TransitionAnimationRunning = false;
			yield break;
		}
		ChronicleCaveFloorSlot targetFloor = m_Floors[nextCave - 1];
		targetFloor.HideBoss(true);
		SetUpperMostDoorOpened(true);
		tween.m_EndOffset = new Vector3(0f, new Vector3(m_CamRoot.transform.position.x, Mathf.Clamp(targetFloor.transform.position.y, 0f, m_UpperFloorPosition.y * (float)m_Floors.Count - m_UpperFloorPosition.y), m_CamRoot.transform.position.z).y - m_CamRoot.transform.position.y);
		tween.m_DurationInSeconds = 2f;
		tween.Play();
		SetNewHotSpot(m_Floors[nextCave - 1].m_EnteringHotSpot, ActionAfterWalkingDone);
		yield return new WaitForSeconds(tween.m_DurationInSeconds);
		while (m_WalkingToNextCave)
		{
			yield return new WaitForEndOfFrame();
		}
		base.BlockFeatureUnlocks = false;
		m_TransitionAnimationRunning = false;
	}

	private void ActionAfterWalkingDone()
	{
		m_WalkingToNextCave = false;
	}

	private void HandleBackButton()
	{
		DebugLog.Log("Pressed Back Button: " + GetType());
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("back_button_pressed", string.Empty);
		m_Left = true;
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		DIContainerInfrastructure.GetCoreStateMgr().GotoWorldMap();
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

	public override void ShowWorkshopScreen(string shopNameId, HotSpotWorldMapViewBase hotspot)
	{
		ShopBalancingData balancing = null;
		if (DIContainerBalancing.Service.TryGetBalancingData<ShopBalancingData>(shopNameId, out balancing))
		{
			m_WorkShopUI.SetModel(balancing, ShopMenuType.Workshop, hotspot);
			m_WorkShopUI.Enter();
		}
	}

	public override void ShowWitchHutScreen(string shopNameId, HotSpotWorldMapViewBase hotspot)
	{
		ShopBalancingData balancing = null;
		if (DIContainerBalancing.Service.TryGetBalancingData<ShopBalancingData>(shopNameId, out balancing))
		{
			m_WorkShopUI.SetModel(balancing, ShopMenuType.Witchhut, hotspot);
			m_WorkShopUI.Enter();
		}
	}

	public override void ShowTrainerScreen(string shopNameId, HotSpotWorldMapViewBase hotspot)
	{
		ShopBalancingData balancing = null;
		if (DIContainerBalancing.Service.TryGetBalancingData<ShopBalancingData>(shopNameId, out balancing))
		{
			m_WorkShopUI.SetModel(balancing, ShopMenuType.Trainer, hotspot);
			m_WorkShopUI.Enter();
		}
	}

	public override void ShowDojoScreen(string shopNameId, HotSpotWorldMapViewBase hotspot)
	{
		ShopBalancingData balancing = null;
		if (DIContainerBalancing.Service.TryGetBalancingData<ShopBalancingData>(shopNameId, out balancing))
		{
			m_WorkShopUI.SetModel(balancing, ShopMenuType.Dojo, hotspot);
			m_WorkShopUI.Enter();
		}
	}

	public override void ShowBattlePreperationScreen()
	{
		m_battlePreperation.SetChronicleCaveHotSpot(m_Model.CurrentHotspotGameData, m_Model, this);
		m_battlePreperation.Enter();
	}

	public override void StartBattle(HotspotGameData hotspot, List<BirdGameData> battleBirdList, BattleParticipantTableBalancingData addition, bool hardmode = false)
	{
		DIContainerLogic.ChronicleCaveService.SetupHotspotBattle(DIContainerInfrastructure.GetCurrentPlayer(), hotspot, battleBirdList, addition, m_Model.ChronicleCaveFloorGameDatas[m_Model.Data.CurrentBirdFloorIndex].BalancingData);
		CoreStateMgr.Instance.GotoBattle(ClientInfo.CurrentBattleStartGameData.m_BackgroundAssetId);
	}
}

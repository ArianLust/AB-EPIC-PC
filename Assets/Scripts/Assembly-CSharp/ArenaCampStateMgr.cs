using System;
using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Character;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class ArenaCampStateMgr : BaseCampStateMgr
{
	[SerializeField]
	private UILabel m_objectiveTimer;

	[SerializeField]
	private GameObject m_objectiveTimerRoot;

	[SerializeField]
	private UILabel m_arenaRanking;

	[SerializeField]
	private Transform m_leagueCrownParent;

	[SerializeField]
	private CrownProp m_leagueCrownButton;

	[SerializeField]
	private TrophyProp m_trophyButton;

	[SerializeField]
	private GameObject m_trophyObject;

	[SerializeField]
	public Transform m_objectiveGrid;

	[SerializeField]
	private DailyObjectiveBoardElement m_objectivePrefab;

	[SerializeField]
	private UIInputTrigger m_ObjectiveTrigger;

	[SerializeField]
	private GameObject m_ObjectiveObjectWeekFinished;

	[SerializeField]
	private UIInputTrigger m_ObjectiveTriggerWeekFinished;

	[HideInInspector]
	public FriendCampMenuUI m_FriendCampUI;

	[HideInInspector]
	public ArenaBattlePreperationUI m_battlePreperation;

	private PvpDetailUI m_PvpDetailUI;

	[SerializeField]
	private Transform m_BannerRoot;

	[HideInInspector]
	public ArenaCampMenuUI m_CampUI;

	[HideInInspector]
	public BannerWindowUI m_BannerMgr;

	[HideInInspector]
	public TrophyWindowUI m_TrophyMgr;

	[HideInInspector]
	public LeaderboardUI m_PvpLeaderboardlUI;

	[SerializeField]
	private UILabel m_ShardBonusFromEventLabel;

	[SerializeField]
	private Animator m_ShardBonusAnimator;

	[SerializeField]
	public List<GameObject> m_LeagueCrowns = new List<GameObject>();

	private PopupResetObjectives m_ResetObjectivesPopup;

	private bool m_demotionPopupshowing;

	private bool m_seasonendPopupshowing;

	private bool m_leaderboardDirectly;

	private CharacterControllerCamp m_bannerControllerWorldMap { get; set; }

	private void Awake()
	{
		DIContainerInfrastructure.AdService.AddPlacement(GachaPopupUI.PVPGACHA_PLACEMENT);
		DebugLog.Log(GetType(), "Awake start");
		DIContainerInfrastructure.GetCoreStateMgr().m_SceneryAudioListener = base.transform.GetComponentInChildren<AudioListener>();
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_SeasonLeaderBoard_Tabs", OnPvpLeaderboardLoaded);
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.ForceLoading = true;
		m_demotionPopupshowing = false;
		m_seasonendPopupshowing = false;
		if ((bool)m_ShopCamp)
		{
			m_CampProps.Add(m_ShopCamp);
		}
		if ((bool)m_MailBoxCamp)
		{
			m_CampProps.Add(m_MailBoxCamp);
		}
		if ((bool)m_FriendListCamp)
		{
			m_CampProps.Add(m_FriendListCamp);
		}
		if ((bool)m_RovioIdCamp)
		{
			m_CampProps.Add(m_RovioIdCamp);
		}
		if ((bool)m_leagueCrownButton)
		{
			m_leagueCrownButton.Init();
		}
		if ((bool)m_trophyButton)
		{
			m_trophyButton.Init();
		}
		DebugLog.Log(GetType(), "Awake done");
		DIContainerInfrastructure.GetCoreStateMgr().m_ArenaCampStateMgr = this;
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			ContentLoader.Instance.CheckforRestartApp();
		}
	}

	public override void RovioIdCampOnPropClicked(BasicItemGameData obj)
	{
		GoToSocial(SocialWindowCategory.PvPInfo);
	}

	private void OnPossibleRankChange()
	{
		HandlePvPRank();
	}

	private IEnumerator Start()
	{
		DebugLog.Log(GetType(), "Start start");
		CheckForAdvancedGacha();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.IsInBattle = false;
		DIContainerInfrastructure.GetCoreStateMgr().m_IsWithinPvP = true;
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.ResetRegistration();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("camp_enter");
		DIContainerInfrastructure.AudioManager.PlayMusic("Music_ArenaCamp");
		if (DIContainerInfrastructure.PurchasingService.IsSupported() && !DIContainerInfrastructure.PurchasingService.IsInitializing() && !DIContainerInfrastructure.PurchasingService.IsInitialized() && !string.IsNullOrEmpty(DIContainerConfig.GetClientConfig().BundleId))
		{
			DIContainerInfrastructure.PurchasingService.Initialize(DIContainerConfig.GetClientConfig().BundleId);
		}
		if (!ClientInfo.IsFriend)
		{
			UpdateEnergy();
			StartCoroutine(FillObjectiveBoard());
		}
		if ((bool)m_FriendInfo && ClientInfo.IsFriend)
		{
			m_FriendInfo.SetDefault();
			m_FriendInfo.SetModel(ClientInfo.InspectedFriend);
		}
		if (ClientInfo.IsFriend)
		{
			m_LoadedLevels.Add("Menu_FriendCamp", false);
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Menu_FriendCamp", OnMenuFriendCampLoaded);
			yield return new WaitForEndOfFrame();
		}
		else
		{
			m_LoadedLevels.Add("Menu_Arena", false);
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Menu_Arena", OnMenuCampLoaded);
			yield return new WaitForEndOfFrame();
		}
		m_LoadedLevels.Add("Window_ArenaBattlePreparation", false);
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_ArenaBattlePreparation", OnWindowBattlePreparationLoaded);
		yield return new WaitForEndOfFrame();
		if (!ClientInfo.IsFriend)
		{
			if (DIContainerInfrastructure.GetCurrentPlayer().Data.PvPTrophy == null)
			{
				m_trophyObject.SetActive(false);
			}
			else
			{
				RefreshTrophy();
			}
		}
		else if (ClientInfo.InspectedFriend.PublicPlayerData.Trophy == null && !IsValidSeasonTrophy())
		{
			m_trophyObject.SetActive(false);
		}
		else
		{
			RefreshTrophy();
		}
		m_Birds = ClientInfo.CurrentCampBirds;
		DebugLog.Log("Birds: " + m_Birds.Count);
		DebugLog.Log("BirdPositions: " + m_BirdPositionsByCount.Count);
		if (m_Birds.Count > 0)
		{
			List<Vector3> positions = ((m_BirdPositionsByCount.Count <= m_Birds.Count - 1) ? m_BirdPositionsByCount[0].Vectors : m_BirdPositionsByCount[m_Birds.Count - 1].Vectors);
			for (int i = 0; i < m_Birds.Count; i++)
			{
				yield return new WaitForEndOfFrame();
				BirdGameData bird = m_Birds[i];
				CharacterControllerCamp characterController = UnityEngine.Object.Instantiate(m_CharacterCampPrefab);
				characterController.SetModel(bird);
				characterController.transform.parent = m_CharacterRoot;
				characterController.transform.localPosition = positions[i];
				if (m_IsBirdMirrored.Count > i && m_IsBirdMirrored[i])
				{
					characterController.transform.localScale = new Vector3(-1f, 1f, 1f);
				}
				m_CharactersCamp.Add(characterController);
			}
			DIContainerLogic.InventoryService.FixInventoryOfBirds(DIContainerInfrastructure.GetCurrentPlayer());
		}
		if (ClientInfo.CurrentBanner != null)
		{
			m_bannerControllerWorldMap = UnityEngine.Object.Instantiate(m_CharacterCampPrefab);
			if ((bool)m_bannerControllerWorldMap)
			{
				m_bannerControllerWorldMap.transform.parent = m_BannerRoot;
				m_bannerControllerWorldMap.transform.localPosition = Vector3.one;
				m_bannerControllerWorldMap.SetModel(ClientInfo.CurrentBanner);
			}
		}
		DeRegisterEventHandler();
		foreach (CampProp prop in m_CampProps)
		{
			if (!prop.m_IsInitialized)
			{
				prop.Awake();
			}
		}
		UpdateLoggedInIndicator();
		while (m_LoadedLevels.Values.Count((bool e) => !e) > 0 || m_CampProps.Count((CampProp p) => !p.m_IsInitialized) > 0)
		{
			yield return new WaitForEndOfFrame();
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_DailyLoginUi.StopEnterCoroutine();
		HandlePvPRank();
		HandlePvPLeague();
		if ((bool)m_MailBoxCamp)
		{
			m_MailBoxCamp.SetCounter(GetViewableMessagesCount());
		}
		if ((bool)m_FriendListCamp)
		{
			m_FriendListCamp.SetCounter(0);
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave();
		DIContainerInfrastructure.GetCoreStateMgr().m_PopupRoot.Leave();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 0u,
			showFriendshipEssence = true,
			showLuckyCoins = true,
			showSnoutlings = true
		}, true);
		if (!ClientInfo.IsFriend)
		{
			RefreshBirdMarkers();
			RefreshBannerMarkers();
			CheckForPiggieMcCoolVisits();
			RequestLeaderboardUpdate(RefreshCampContent);
			InvokeRepeating("UpdateMatchmakingOpponent", 0.1f, 20f);
		}
		RegisterEventHandler();
		DebugLog.Log("Camp Initialized!");
		PlayerGameData player = DIContainerInfrastructure.GetCurrentPlayer();
		DIContainerLogic.SocialService.UpdateFreePvpGachaRolls(player, player.SocialEnvironmentGameData);
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("enter_arena_camp", string.Empty);
		if (!string.IsNullOrEmpty(DIContainerInfrastructure.GetCoreStateMgr().m_HotLinkItemName))
		{
			if (DIContainerInfrastructure.GetCoreStateMgr().m_HotLinkItemName == "RovioId")
			{
				GoToSocial(SocialWindowCategory.RovioId);
			}
			else if (DIContainerInfrastructure.GetCoreStateMgr().m_HotLinkItemName == "story_goldenpig")
			{
				GoToGacha();
			}
			else if (DIContainerInfrastructure.GetCoreStateMgr().m_HotLinkItemName == "Trophy")
			{
				ShowTrophyManager();
			}
			else
			{
				DIContainerInfrastructure.GetCoreStateMgr().ShowShop(DIContainerInfrastructure.GetCoreStateMgr().m_HotLinkItemName, delegate
				{
				}, 0, true, DIContainerInfrastructure.GetCoreStateMgr().m_ShopEnterSource);
				DIContainerInfrastructure.GetCoreStateMgr().m_ShopEnterSource = "Standard";
			}
			DIContainerInfrastructure.GetCoreStateMgr().m_HotLinkItemName = null;
		}
		InvokeRepeating("UpdateFreeGachaSign", 1f, 10f);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("camp_enter");
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.ForceLoading = false;
		InvokeRepeating("CheckLeagueAchievements", 0f, 6f);
		yield return new WaitForSeconds(1f);
		if (player.Data.EnterNicknameTutorialDone == 1)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_EnterNamePopup.ShowEnterNamePopup();
			player.Data.EnterNicknameTutorialDone = 2u;
		}
		ContentLoader.Instance.CheckforRestartApp();
		StartCoroutine(CheckForSeasonEndPopup());
		DebugLog.Log(GetType(), "Start done");
	}

	private IEnumerator CheckForSeasonEndPopup()
	{
		yield return new WaitForSeconds(1f);
		if (!DIContainerInfrastructure.GetCoreStateMgr().m_SeasonEndPopup.m_SeasonendPopupshowing && DIContainerInfrastructure.GetCurrentPlayer().Data.HasPendingSeasonendPopup)
		{
			DeRegisterEventHandler();
			DIContainerInfrastructure.GetCurrentPlayer().Data.HasPendingSeasonendPopup = false;
			DIContainerInfrastructure.GetCoreStateMgr().m_SeasonEndPopup.Init();
			while (DIContainerInfrastructure.GetCoreStateMgr().m_SeasonEndPopup.m_SeasonendPopupshowing)
			{
				yield return new WaitForEndOfFrame();
			}
			RegisterEventHandler();
			RefreshTrophy();
		}
	}

	public void RefreshBannerMarkers()
	{
		if (ClientInfo.IsFriend)
		{
			return;
		}
		bool flag = false;
		foreach (IInventoryItemGameData item in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.BannerTip])
		{
			if (item.ItemData.IsNew)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			m_bannerControllerWorldMap.ShowNewMarker(true);
			return;
		}
		foreach (IInventoryItemGameData item2 in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.BannerEmblem])
		{
			if (item2.ItemData.IsNew)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			m_bannerControllerWorldMap.ShowNewMarker(true);
			return;
		}
		foreach (IInventoryItemGameData item3 in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.Banner])
		{
			if (item3.ItemData.IsNew)
			{
				flag = true;
				break;
			}
		}
		m_bannerControllerWorldMap.ShowNewMarker(flag);
	}

	public void RemoveAllNewMarkersFromBanner(BannerGameData banner)
	{
		foreach (IInventoryItemGameData item in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.Banner])
		{
			if (item.ItemData.IsNew)
			{
				item.ItemData.IsNew = false;
			}
		}
		foreach (IInventoryItemGameData item2 in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.BannerTip])
		{
			if (item2.ItemData.IsNew)
			{
				item2.ItemData.IsNew = false;
			}
		}
		foreach (IInventoryItemGameData item3 in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.BannerEmblem])
		{
			if (item3.ItemData.IsNew)
			{
				item3.ItemData.IsNew = false;
			}
		}
		m_bannerControllerWorldMap.ShowNewMarker(false);
	}

	private void UpdateMatchmakingOpponent()
	{
		if (DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(DIContainerInfrastructure.GetCurrentPlayer()))
		{
			PvPTurnManagerGameData currentSeasonTurn = DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData.CurrentSeasonTurn;
			if (currentSeasonTurn.CurrentPvPOpponent == null || DIContainerLogic.GetDeviceTimingService().IsAfter(currentSeasonTurn.LastOpponentUpdateTime.AddSeconds(DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData.Balancing.TimeTillMatchmakingBattleRefreshes)))
			{
				DIContainerLogic.PvPSeasonService.UpdateCurrentPvPOpponent(DIContainerInfrastructure.GetCurrentPlayer(), DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData);
			}
		}
	}

	public IEnumerator FillObjectiveBoard()
	{
		if (DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData.CurrentSeasonTurn.CurrentPvPTurnManagerState == EventManagerState.Finished)
		{
			m_ObjectiveTrigger.gameObject.SetActive(false);
			m_ObjectiveObjectWeekFinished.SetActive(true);
			yield break;
		}
		m_ObjectiveTrigger.gameObject.SetActive(true);
		m_ObjectiveObjectWeekFinished.SetActive(false);
		int solvedObjectives = 0;
		foreach (Transform child in m_objectiveGrid)
		{
			UnityEngine.Object.Destroy(child.gameObject);
		}
		yield return new WaitForSeconds(1.5f);
		List<PvPObjectivesGameData> objectives = DIContainerLogic.GetPvpObjectivesService().GetDailyObjectives();
		float animationDelay = 0f;
		int i = 0;
		foreach (PvPObjectivesGameData gameData in objectives)
		{
			if (gameData.Data.Solved)
			{
				solvedObjectives++;
			}
			DailyObjectiveBoardElement element = UnityEngine.Object.Instantiate(m_objectivePrefab);
			Transform objectiveTransform = element.transform;
			objectiveTransform.parent = m_objectiveGrid;
			objectiveTransform.localScale = Vector3.one;
			objectiveTransform.localPosition = Vector3.zero;
			animationDelay = element.Init(gameData, animationDelay);
			i++;
			if (i == 3)
			{
				break;
			}
		}
		BonusEventBalancingData bonusBalancing = DIContainerLogic.GetBonusEventService.m_CurrentValidBalancing;
		if (solvedObjectives == 3 && bonusBalancing != null && bonusBalancing.BonusType == BonusEventType.ShardsForObjective && !DIContainerInfrastructure.GetCurrentPlayer().Data.BonusShardsGainedToday)
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.BonusShardsGainedToday = true;
			DIContainerLogic.InventoryService.AddItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, 1, "shard", (int)bonusBalancing.BonusFactor, "objectives solved with bonus event");
			m_ShardBonusFromEventLabel.text = "+" + bonusBalancing.BonusFactor;
			m_ShardBonusAnimator.Play("Finished");
		}
		m_objectiveGrid.GetComponent<UIGrid>().Reposition();
		StopCoroutine("CountDownPvpObjectiveAndEnergyTimer");
		StartCoroutine("CountDownPvpObjectiveAndEnergyTimer");
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("daily_objectives_ready", string.Empty);
	}

	private IEnumerator CountDownPvpObjectiveAndEnergyTimer()
	{
		DateTime trustedTime;
		while (!DIContainerLogic.GetServerOnlyTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		DateTime targetTime = DIContainerLogic.GetServerOnlyTimingService().GetPresentTime().AddSeconds(DIContainerLogic.PvPSeasonService.GetDailyPvpRefreshTimeLeft(DIContainerInfrastructure.GetCurrentPlayer(), DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData).TotalSeconds);
		while (targetTime > trustedTime)
		{
			if (DIContainerLogic.GetServerOnlyTimingService().TryGetTrustedTime(out trustedTime))
			{
				TimeSpan timeLeft = targetTime - trustedTime;
				m_objectiveTimer.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(DIContainerLogic.GetServerOnlyTimingService().TimeLeftUntil(targetTime));
				if (timeLeft.TotalSeconds < 900.0 && !m_objectiveTimerRoot.GetComponent<Animation>().isPlaying)
				{
					m_objectiveTimerRoot.PlayAnimationOrAnimatorState("Timer_Loop");
				}
				else if (m_objectiveTimerRoot.GetComponent<Animation>().isPlaying)
				{
					m_objectiveTimerRoot.PlayAnimationOrAnimatorState("Timer_Stop");
				}
			}
			yield return new WaitForSeconds(1f);
		}
		m_ObjectiveTrigger.gameObject.SetActive(false);
		m_ObjectiveObjectWeekFinished.SetActive(true);
		if (!ClientInfo.IsFriend)
		{
			UpdateEnergy();
			RefreshObjectives();
		}
	}

	private void HandlePvPLeague()
	{
		IInventoryItemGameData data = null;
		if (ClientInfo.CurrentCampInventory == null || m_LeagueCrowns == null || !DIContainerLogic.InventoryService.TryGetItemGameData(ClientInfo.CurrentCampInventory, "pvp_league_crown", out data))
		{
			return;
		}
		for (int i = 0; i < m_LeagueCrowns.Count; i++)
		{
			GameObject gameObject = m_LeagueCrowns[i];
			if (i == data.ItemData.Level - 1)
			{
				gameObject.gameObject.SetActive(true);
			}
			else
			{
				gameObject.gameObject.SetActive(false);
			}
		}
	}

	private void HandlePvPRank()
	{
		if (ClientInfo.IsFriend)
		{
			m_arenaRanking.text = "#" + ClientInfo.InspectedFriend.PublicPlayerData.PvPRank.ToString("0");
		}
		else if (DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(DIContainerInfrastructure.GetCurrentPlayer()))
		{
			m_arenaRanking.text = ((DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData.CurrentSeasonTurn.Data.CurrentScore != 0) ? ("#" + DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData.CurrentSeasonTurn.GetCurrentRank) : ("#" + (DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData.Balancing.MaximumMatchmakingPlayers + 1).ToString("0")));
		}
		else
		{
			m_arenaRanking.text = "#15";
		}
	}

	private void BirdClicked(ICharacter character)
	{
		if (!ClientInfo.IsFriend)
		{
			ShowBannerManager();
		}
	}

	private void ShowBannerManager()
	{
		if (m_BannerMgr == null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_BannerManager", OnBannerManagerLoaded);
			return;
		}
		m_BannerMgr.SetStateMgr(this);
		m_BannerMgr.SetModel(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, DIContainerInfrastructure.GetCurrentPlayer().BannerGameData, InventoryItemType.BannerTip);
	}

	private void OnBannerManagerLoaded()
	{
		m_BannerMgr = UnityEngine.Object.FindObjectOfType(typeof(BannerWindowUI)) as BannerWindowUI;
		m_BannerMgr.SetStateMgr(this);
		m_BannerMgr.SetModel(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, DIContainerInfrastructure.GetCurrentPlayer().BannerGameData, InventoryItemType.BannerTip);
	}

	private void TrophyClicked()
	{
		if (!ClientInfo.IsFriend)
		{
			ShowTrophyManager();
		}
	}

	public void ShowTrophyManager()
	{
		if (m_TrophyMgr == null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_SeasonEndRewardManager", OnTrophyManagerLoaded);
			return;
		}
		m_TrophyMgr.SetStateMgr(this);
		m_TrophyMgr.SetModel(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, DIContainerInfrastructure.GetCurrentPlayer().Data.PvPTrophy);
	}

	private void OnTrophyManagerLoaded()
	{
		m_TrophyMgr = UnityEngine.Object.FindObjectOfType(typeof(TrophyWindowUI)) as TrophyWindowUI;
		m_TrophyMgr.gameObject.SetActive(false);
		DebugLog.Log("Window_SeasonEndRewardManager loaded!");
		m_TrophyMgr.SetStateMgr(this);
		m_TrophyMgr.SetModel(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, DIContainerInfrastructure.GetCurrentPlayer().Data.PvPTrophy);
	}

	public override void RefreshCampContent()
	{
		HandlePvPLeague();
		HandlePvPRank();
		UpdateEnergy();
	}

	public override void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		base.RegisterEventHandler();
		if (ClientInfo.CurrentBanner != null)
		{
			ClientInfo.CurrentBanner.InventoryGameData.InventoryOfTypeChanged += BannerInventoryOfTypeChanged;
		}
		if ((bool)m_bannerControllerWorldMap)
		{
			m_bannerControllerWorldMap.BirdClicked += BirdClicked;
		}
		if ((bool)m_leagueCrownButton)
		{
			m_leagueCrownButton.OnPropClicked += LeagueCrownButtonClicked;
		}
		if ((bool)m_trophyButton)
		{
			m_trophyButton.OnPropClicked += TrophyClicked;
		}
		if ((bool)m_ObjectiveTrigger)
		{
			m_ObjectiveTrigger.Clicked += OnObjectiveBoardClicked;
		}
		if ((bool)m_ObjectiveTriggerWeekFinished)
		{
			m_ObjectiveTriggerWeekFinished.Clicked += ShowPvPTurnResultScreen;
		}
		DIContainerInfrastructure.GetCurrentPlayer().GlobalPvPStateChanged += OnGlobalPvPStateChanged;
		DIContainerInfrastructure.GetCurrentPlayer().GlobalPvPScoresUpdated += OnPossibleRankChange;
	}

	private void OnObjectiveBoardClicked()
	{
		if (!DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(DIContainerInfrastructure.GetCurrentPlayer()) || DIContainerLogic.PvPSeasonService.IsPvPTurnRunning(DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData))
		{
			ShowObjectiveRefreshPopup();
		}
	}

	public void ShowObjectiveRefreshPopup()
	{
		if ((bool)m_ResetObjectivesPopup)
		{
			DeRegisterEventHandler();
			m_CampUI.Leave();
			m_CampUI.m_PvpBanner.WaitThenLeave();
			m_ResetObjectivesPopup.Enter(this);
		}
		else
		{
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddLevel("Popup_ArenaObjectives", true, false, OnObjectiveResetPopupLoaded);
		}
	}

	private void OnObjectiveResetPopupLoaded()
	{
		m_ResetObjectivesPopup = UnityEngine.Object.FindObjectOfType(typeof(PopupResetObjectives)) as PopupResetObjectives;
		DeRegisterEventHandler();
		m_CampUI.Leave();
		m_CampUI.m_PvpBanner.WaitThenLeave();
		m_ResetObjectivesPopup.Enter(this);
	}

	public void RefreshObjectivesPopupClosed()
	{
		RegisterEventHandler();
		m_CampUI.Enter();
		StartCoroutine(m_CampUI.m_PvpBanner.EnterCoroutine(this));
	}

	private void LeagueCrownButtonClicked()
	{
		if (DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(DIContainerInfrastructure.GetCurrentPlayer()))
		{
			ShowLeaderBoardScreen(true);
		}
	}

	public override void DeRegisterEventHandler()
	{
		base.DeRegisterEventHandler();
		if (ClientInfo.CurrentBanner != null)
		{
			ClientInfo.CurrentBanner.InventoryGameData.InventoryOfTypeChanged -= BannerInventoryOfTypeChanged;
		}
		if ((bool)m_bannerControllerWorldMap)
		{
			m_bannerControllerWorldMap.BirdClicked -= BirdClicked;
		}
		if ((bool)m_leagueCrownButton)
		{
			m_leagueCrownButton.OnPropClicked -= LeagueCrownButtonClicked;
		}
		if ((bool)m_trophyButton)
		{
			m_trophyButton.OnPropClicked -= TrophyClicked;
		}
		if ((bool)m_ObjectiveTrigger)
		{
			m_ObjectiveTrigger.Clicked -= OnObjectiveBoardClicked;
		}
		if ((bool)m_ObjectiveTriggerWeekFinished)
		{
			m_ObjectiveTriggerWeekFinished.Clicked -= ShowPvPTurnResultScreen;
		}
		DIContainerInfrastructure.GetCurrentPlayer().GlobalPvPStateChanged -= OnGlobalPvPStateChanged;
		DIContainerInfrastructure.GetCurrentPlayer().GlobalPvPScoresUpdated -= OnPossibleRankChange;
	}

	private void OnGlobalPvPStateChanged(CurrentGlobalEventState oldState, CurrentGlobalEventState newState)
	{
		RefreshCampContent();
		CancelInvoke("TryToShowDemotionPopup");
		InvokeRepeating("TryToShowDemotionPopup", 0.1f, 2f);
		if (newState == CurrentGlobalEventState.RunningEvent)
		{
			UpdateEnergy();
			RefreshObjectives();
		}
	}

	private void TryToShowDemotionPopup()
	{
		if (IsShowDemotionPopUpPossible())
		{
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			PvPSeasonManagerGameData currentPvPSeasonGameData = currentPlayer.CurrentPvPSeasonGameData;
			if (currentPvPSeasonGameData != null && currentPvPSeasonGameData.Data.HasPendingDemotionPopup && !m_demotionPopupshowing)
			{
				m_demotionPopupshowing = true;
				DIContainerLogic.PvPSeasonService.SetDemotionPopupShown(currentPlayer);
				ShowDemotionPopup();
				CancelInvoke("TryToShowDemotionPopup");
			}
		}
	}

	private void ShowDemotionPopup()
	{
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddLevel("Popup_NoLeagueParticipation", true, false, delegate
		{
		});
	}

	private bool IsShowDemotionPopUpPossible()
	{
		return !DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.gameObject.activeInHierarchy && !DIContainerInfrastructure.GetCoreStateMgr().m_PopupRoot.gameObject.activeInHierarchy && !DIContainerInfrastructure.GetCoreStateMgr().IsShopOpen() && !DIContainerInfrastructure.GetCoreStateMgr().IsShopLoading();
	}

	private void OnDisable()
	{
		PigBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<PigBalancingData>("pig_instructor");
		BirdBalancingData balancingData2 = DIContainerBalancing.Service.GetBalancingData<BirdBalancingData>("bird_instructor");
		DIContainerInfrastructure.GetCharacterAssetProvider(false).DestroyCachedObjects(balancingData.AssetId);
		DIContainerInfrastructure.GetCharacterAssetProvider(false).DestroyCachedObjects(balancingData2.AssetId);
		DIContainerInfrastructure.GetCoreStateMgr().UnloadUnusedAssets();
		DeRegisterEventHandler();
	}

	private void BannerInventoryOfTypeChanged(InventoryItemType type, IInventoryItemGameData item)
	{
		if ((bool)m_bannerControllerWorldMap && ClientInfo.CurrentBanner != null)
		{
			m_bannerControllerWorldMap.SetModel(ClientInfo.CurrentBanner);
		}
	}

	public void OnMenuCampLoaded()
	{
		m_LoadedLevels["Menu_Arena"] = true;
		m_CampUI = UnityEngine.Object.FindObjectOfType(typeof(ArenaCampMenuUI)) as ArenaCampMenuUI;
		m_CampUI.SetCampStateMgr(this);
		DebugLog.Log("Menu_Arena loaded!");
	}

	private void OnPvpDetailsLoaded()
	{
		m_PvpDetailUI = UnityEngine.Object.FindObjectOfType(typeof(PvpDetailUI)) as PvpDetailUI;
		m_PvpDetailUI.SetStateMgr(this);
		m_PvpDetailUI.gameObject.SetActive(false);
		DebugLog.Log("Window_ArenaDetails loaded!");
		m_PvpDetailUI.SetModel(DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData);
		m_PvpDetailUI.Enter();
	}

	private void OnPvpLeaderboardLoaded()
	{
		m_PvpLeaderboardlUI = UnityEngine.Object.FindObjectOfType(typeof(LeaderboardUI)) as LeaderboardUI;
		m_PvpLeaderboardlUI.gameObject.SetActive(false);
		DebugLog.Log("Window_SeasonLeaderBoard_Tabs loaded!");
	}

	public void OnMenuFriendCampLoaded()
	{
		m_LoadedLevels["Menu_FriendCamp"] = true;
		m_FriendCampUI = UnityEngine.Object.FindObjectOfType(typeof(FriendCampMenuUI)) as FriendCampMenuUI;
		m_FriendCampUI.SetCampStateMgr(this);
		DebugLog.Log("Menu_FriendCamp loaded!");
	}

	private void OnWindowBattlePreparationLoaded()
	{
		m_LoadedLevels["Window_ArenaBattlePreparation"] = true;
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(ArenaBattlePreperationUI));
		m_battlePreperation = @object as ArenaBattlePreperationUI;
		m_battlePreperation.gameObject.SetActive(false);
		DebugLog.Log("ArenaBattlePreperationUI loaded!");
	}

	public void ShowPvpInfoScreen()
	{
		if (m_PvpDetailUI == null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_ArenaDetails", OnPvpDetailsLoaded);
			return;
		}
		m_PvpDetailUI.SetModel(DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData);
		m_PvpDetailUI.Enter();
	}

	public override void ShowLeaderBoardScreen(bool directly)
	{
		m_leaderboardDirectly = directly;
		m_PvpLeaderboardlUI.SetPvPModel(DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData);
		m_PvpLeaderboardlUI.Enter(null, null, m_leaderboardDirectly);
	}

	public void StartRankedMatch()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		DeRegisterEventHandler();
		if (currentPlayer.Data.PvPTutorialDisplayState == 1)
		{
			FriendGameData friend = new FriendGameData("NPC_Low");
			PublicPlayerData nPCPlayer = DIContainerLogic.SocialService.GetNPCPlayer(friend, currentPlayer.Data.Level, true);
			m_battlePreperation.Enter(this, nPCPlayer, nPCPlayer.PvPIndices);
		}
		else
		{
			m_battlePreperation.Enter(this, currentPlayer.CurrentPvPSeasonGameData.CurrentSeasonTurn.CurrentPvPOpponent, currentPlayer.CurrentPvPSeasonGameData.CurrentSeasonTurn.CurrentBirdIndices);
		}
	}

	public void StartUnrankedMatch(PublicPlayerData opponent, FriendGameData friend = null)
	{
		m_battlePreperation.Enter(this, opponent, opponent.PvPIndices, true, friend);
	}

	private FriendGameData GetDebugOpponent()
	{
		FriendData highNPCFriend = DIContainerLogic.SocialService.GetHighNPCFriend(DIContainerInfrastructure.GetCurrentPlayer().Data.Level);
		if (highNPCFriend != null)
		{
			FriendGameData value;
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends.TryGetValue(highNPCFriend.Id, out value);
			BannerData bannerData = new BannerData();
			bannerData.NameId = "bird_banner";
			bannerData.Level = DIContainerInfrastructure.GetCurrentPlayer().PublicPlayer.Level;
			bannerData.Inventory = new InventoryGameData("bird_banner").Data;
			value.PublicPlayerData.Banner = bannerData;
			List<BirdData> list = new List<BirdData>();
			for (int i = 0; i < 3; i++)
			{
				list.Add(GetRandomBird());
			}
			value.PublicPlayerData.Birds = list;
			return value;
		}
		return null;
	}

	private BirdData GetRandomBird()
	{
		BirdData birdData = new BirdData();
		birdData.Level = DIContainerInfrastructure.GetCurrentPlayer().PublicPlayer.Level;
		switch (UnityEngine.Random.Range(0, 5))
		{
		case 0:
			birdData.Inventory = new InventoryGameData("red_bird_start").Data;
			birdData.NameId = "bird_red";
			break;
		case 1:
			birdData.Inventory = new InventoryGameData("yellow_bird_start").Data;
			birdData.NameId = "bird_yellow";
			break;
		case 2:
			birdData.Inventory = new InventoryGameData("white_bird_start").Data;
			birdData.NameId = "bird_white";
			break;
		case 3:
			birdData.Inventory = new InventoryGameData("black_bird_start").Data;
			birdData.NameId = "bird_black";
			break;
		case 4:
			birdData.Inventory = new InventoryGameData("blue_bird_start").Data;
			birdData.NameId = "bird_blue";
			break;
		}
		return birdData;
	}

	public void ShowPvPTurnResultScreen()
	{
		if (DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData.CurrentSeasonTurn.Data.CurrentScore == 0)
		{
			DIContainerLogic.PvPSeasonService.ClearPvPTurn(DIContainerInfrastructure.GetCurrentPlayer());
			return;
		}
		CheckLeagueFinishedAchievements();
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddLevel("Popup_LeagueFinished", true, false, delegate
		{
		});
	}

	public void ShowTooltip()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(currentPlayer))
		{
			if (ClientInfo.IsFriend)
			{
				PublicPlayerData publicPlayerData = ClientInfo.InspectedFriend.PublicPlayerData;
				DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowArenaLeagueOverlay(base.transform, publicPlayerData.League, publicPlayerData.PvPRank, true);
			}
			else
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowArenaLeagueOverlay(base.transform, currentPlayer.CurrentPvPSeasonGameData.Data.CurrentLeague, currentPlayer.CurrentPvPSeasonGameData.Data.CurrentRank, true);
			}
		}
	}

	public void ShowTrophyTooltip()
	{
		if (ClientInfo.IsFriend)
		{
			PublicPlayerData publicPlayerData = ClientInfo.InspectedFriend.PublicPlayerData;
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowTrophyOverlay(base.transform, publicPlayerData.Trophy, true);
		}
		else
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowTrophyOverlay(base.transform, DIContainerInfrastructure.GetCurrentPlayer().Data.PvPTrophy, true);
		}
	}

	public void HideAllTooltips()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
	}

	public bool RefreshObjectives(bool onlyUnsolved = false)
	{
		if (DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(DIContainerInfrastructure.GetCurrentPlayer()) && !DIContainerLogic.PvPSeasonService.IsPvPTurnRunning(DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData))
		{
			return false;
		}
		if (onlyUnsolved)
		{
			return ShuffleUnsolvedObjectives();
		}
		StartCoroutine(FillObjectiveBoard());
		return true;
	}

	private bool ShuffleUnsolvedObjectives()
	{
		Requirement requirement = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").RerollPvpObjectivesRequirement;
		if (DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "free_refresh_use") >= 1)
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.ObjectiveVideoFreeRefreshUsed = true;
			DIContainerLogic.InventoryService.RemoveItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "free_refresh_use", 1, "used free refresh");
			Requirement requirement2 = new Requirement();
			requirement2.RequirementType = requirement.RequirementType;
			requirement2.NameId = requirement.NameId;
			requirement2.Value = 0f;
			requirement = requirement2;
		}
		if (!DIContainerLogic.RequirementService.ExecuteRequirements(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, new List<Requirement> { requirement }, "reroll_pvp_objectives"))
		{
			DIContainerInfrastructure.GetCoreStateMgr().ShowShop("shop_premium", delegate
			{
			}, 0, true, DIContainerInfrastructure.GetCoreStateMgr().m_ShopEnterSource);
			DIContainerInfrastructure.GetCoreStateMgr().m_ShopEnterSource = "Standard";
			return false;
		}
		DIContainerLogic.GetPvpObjectivesService().RerollUnsolved();
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		StartCoroutine(FillObjectiveBoard());
		return true;
	}

	private void UpdateEnergy()
	{
		if (RefreshDailyPvpEnergy())
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.BonusShardsGainedToday = false;
			DIContainerLogic.GetPvpObjectivesService().RollForNewObjectives();
			DIContainerInfrastructure.GetCurrentPlayer().Data.ObjectiveVideoFreeRefreshUsed = false;
			DIContainerLogic.InventoryService.RemoveItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "free_refresh_use", 1, "free refresh expired");
			DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData.CurrentSeasonTurn.Data.CurrentMatchingDifficulty = 0;
			DIContainerLogic.InventoryService.SetItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, 1, "pvp_energy", 3, "New_day_new_energy");
		}
	}

	public bool RefreshDailyPvpEnergy()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (!DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(currentPlayer) || !DIContainerLogic.PvPSeasonService.IsPvPTurnRunning(currentPlayer.CurrentPvPSeasonGameData))
		{
			return false;
		}
		DateTime trustedTime;
		if (!DIContainerLogic.GetServerOnlyTimingService().TryGetTrustedTime(out trustedTime))
		{
			return false;
		}
		bool flag = false;
		flag = DIContainerLogic.PvPSeasonService.IsDailyPvpRefreshed(currentPlayer, currentPlayer.CurrentPvPSeasonGameData);
		DebugLog.Log("[PvPEnergy] Is next day: " + flag);
		if (flag)
		{
			currentPlayer.CurrentPvPSeasonGameData.CurrentSeasonTurn.Data.LastUsedPvpEnergy = DIContainerLogic.GetServerOnlyTimingService().GetCurrentTimestamp();
			currentPlayer.SavePlayerData();
		}
		return flag;
	}

	private void CheckLeagueFinishedAchievements()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (!DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(currentPlayer))
		{
			return;
		}
		bool flag = currentPlayer.CurrentPvPSeasonGameData.CurrentSeasonTurn.GetCurrentRank == 1 && currentPlayer.CurrentPvPSeasonGameData.CurrentSeasonTurn.PublicOpponentDatas.Count > 1;
		int num = 0;
		IInventoryItemGameData data = null;
		AchievementData achievementTracking = currentPlayer.Data.AchievementTracking;
		if (DIContainerLogic.InventoryService.TryGetItemGameData(currentPlayer.InventoryGameData, "pvp_league_crown", out data))
		{
			num = data.ItemData.Level;
		}
		if (!achievementTracking.ReachedTopSpotDiamondLeague && flag && num == 6)
		{
			string achievementIdForStoryItemIfExists = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("topSpotDiamond");
			if (!string.IsNullOrEmpty(achievementIdForStoryItemIfExists))
			{
				DIContainerInfrastructure.GetAchievementService().ReportUnlocked(achievementIdForStoryItemIfExists);
				achievementTracking.ReachedTopSpotDiamondLeague = true;
			}
		}
		if (!achievementTracking.ReachedTopSpotAnyLeague && flag)
		{
			string achievementIdForStoryItemIfExists2 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("topSpotAnyLeague");
			if (!string.IsNullOrEmpty(achievementIdForStoryItemIfExists2))
			{
				DIContainerInfrastructure.GetAchievementService().ReportUnlocked(achievementIdForStoryItemIfExists2);
				achievementTracking.ReachedTopSpotAnyLeague = true;
			}
		}
		currentPlayer.SavePlayerData();
	}

	private void CheckLeagueAchievements()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		int currentLeague = 0;
		IInventoryItemGameData data = null;
		AchievementData achievementTracking = currentPlayer.Data.AchievementTracking;
		if (DIContainerLogic.InventoryService.TryGetItemGameData(currentPlayer.InventoryGameData, "pvp_league_crown", out data))
		{
			currentLeague = data.ItemData.Level;
		}
		int num = 2;
		CheckForLeagueReached(currentLeague, num, "reachStone");
		num++;
		CheckForLeagueReached(currentLeague, num, "reachSilver");
		num++;
		CheckForLeagueReached(currentLeague, num, "reachGold");
		num++;
		CheckForLeagueReached(currentLeague, num, "reachPlatinum");
		num++;
		CheckForLeagueReached(currentLeague, num, "reachDiamond");
	}

	private void CheckForLeagueReached(int currentLeague, int leagueChecked, string achievementIdForLeague)
	{
		AchievementData achievementTracking = DIContainerInfrastructure.GetCurrentPlayer().Data.AchievementTracking;
		if (achievementTracking.MaxLeagueReached <= leagueChecked - 1 && currentLeague >= leagueChecked)
		{
			string achievementIdForStoryItemIfExists = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists(achievementIdForLeague);
			if (!string.IsNullOrEmpty(achievementIdForStoryItemIfExists))
			{
				DIContainerInfrastructure.GetAchievementService().ReportUnlocked(achievementIdForStoryItemIfExists);
				achievementTracking.MaxLeagueReached = leagueChecked;
				DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			}
		}
	}

	public void RefreshTrophy()
	{
		m_trophyObject.SetActive(true);
		CHMeshSprite componentInChildren = m_trophyObject.GetComponentInChildren<CHMeshSprite>();
		if (componentInChildren != null)
		{
			if (ClientInfo.IsFriend)
			{
				PublicPlayerData publicPlayerData = ClientInfo.InspectedFriend.PublicPlayerData;
				componentInChildren.m_SpriteName = publicPlayerData.Trophy.NameId;
			}
			else
			{
				componentInChildren.m_SpriteName = DIContainerInfrastructure.GetCurrentPlayer().Data.PvPTrophy.NameId;
			}
			componentInChildren.UpdateSprite(false, true);
		}
	}

	private void RequestLeaderboardUpdate(Action responseCallback)
	{
		DIContainerLogic.PvPSeasonService.GetLeaderboardScores(DIContainerInfrastructure.GetCurrentPlayer(), DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData.CurrentSeasonTurn.Data.CurrentOpponents, DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData, delegate(RESTResultEnum result)
		{
			if (result == RESTResultEnum.Success && responseCallback != null)
			{
				responseCallback();
			}
		});
	}

	private bool IsValidSeasonTrophy()
	{
		int count = DIContainerBalancing.EventBalancingService.GetBalancingDataList<PvPSeasonManagerBalancingData>().Count;
		for (int i = 0; i > count; i++)
		{
			if (DIContainerInfrastructure.GetCurrentPlayer().Data.PvPTrophy.Seasonid == i)
			{
				return true;
			}
		}
		for (int j = 0; j > count; j++)
		{
			for (int k = 0; k > DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Data.TrophyItems.Count; k++)
			{
				if (DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Data.TrophyItems[k].Level == j)
				{
					string nameId = string.Empty;
					switch (DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Data.TrophyItems[k].Quality)
					{
					case 1:
						nameId = "Season" + j + "Wood";
						break;
					case 2:
						nameId = "Season" + j + "Stone";
						break;
					case 3:
						nameId = "Season" + j + "Silver";
						break;
					case 4:
						nameId = "Season" + j + "Gold";
						break;
					case 5:
						nameId = "Season" + j + "Platinum";
						break;
					case 6:
						nameId = "Season" + j + "Diamond";
						break;
					}
					DIContainerInfrastructure.GetCurrentPlayer().Data.PvPTrophy.NameId = nameId;
					DIContainerInfrastructure.GetCurrentPlayer().Data.PvPTrophy.Seasonid = j;
					DIContainerInfrastructure.GetCurrentPlayer().Data.PvPTrophy.FinishedLeagueId = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Data.TrophyItems[k].Quality;
					return true;
				}
			}
		}
		return false;
	}
}

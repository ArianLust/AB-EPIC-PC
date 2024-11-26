using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Interfaces;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class PvPTurnResultUI : MonoBehaviour
{
	private const float m_ServerTimeoutThreshold = 5f;

	private int starCount = 3;

	private PvPSeasonManagerGameData m_Model;

	[SerializeField]
	public UIInputTrigger m_WheelButton;

	[SerializeField]
	private Animation m_WheelAnimation;

	[SerializeField]
	private Animation m_SpinningWheelAnimation;

	[SerializeField]
	private Animation[] m_RewardItemAnimation;

	[SerializeField]
	private Animation m_LootAndButtonAnimationRoot;

	[SerializeField]
	private LootDisplayContoller[] m_LootItemSlots;

	[SerializeField]
	private UIInputTrigger m_ContinueButton;

	[SerializeField]
	private UIInputTrigger m_ConfirmEventButton;

	[SerializeField]
	public UIInputTrigger m_RerollButton;

	[SerializeField]
	private UIInputTrigger m_LeaderBoardButton;

	[SerializeField]
	private LootDisplayContoller m_MinorLootItemSlot;

	[SerializeField]
	private LootDisplayContoller m_MajorLootItemSlot;

	private List<LootDisplayContoller> m_LootResultItemSlots;

	[SerializeField]
	private Animation[] m_StarGainedAnimation;

	[SerializeField]
	private UISprite[] m_StarSprite;

	[SerializeField]
	private ParticleSystem[] m_StarGainedParticle;

	[SerializeField]
	private Transform m_WheelRotateTransform;

	[SerializeField]
	private GameObject[] m_LootObjects;

	[SerializeField]
	private GameObject m_WheelOfLootRoot;

	[SerializeField]
	private GameObject m_LootRoot;

	[SerializeField]
	private GameObject m_RankingRoot;

	[SerializeField]
	private GameObject m_FinalOptionsRoot;

	[SerializeField]
	private GameObject m_FirstOptionsRoot;

	[SerializeField]
	private GameObject m_NextLeaguePreviewRoot;

	[SerializeField]
	private GameObject m_LeagueStayRoot;

	[SerializeField]
	private GameObject m_FullPreviewTabRoot;

	[SerializeField]
	private UILabel m_NextLeaguePreviewHeader;

	[SerializeField]
	private UILabel m_NextLeaguePreviewName;

	[SerializeField]
	private Animator m_NextLeaguePreviewEnterAnimator;

	[SerializeField]
	private Animator m_NextLeaguePreviewAnimator;

	[SerializeField]
	private UISprite m_NextLeaguePreviewCrown;

	[SerializeField]
	private UISprite m_CurrentLeaguePreviewCrown;

	private List<bool> m_starList = new List<bool>();

	[SerializeField]
	private UILabel m_PvPSeasonName;

	[SerializeField]
	private UILabel m_RankLabel;

	[SerializeField]
	private ResourceCostBlind m_PvPPoints;

	[SerializeField]
	private ResourceCostBlind m_RankBonus;

	[SerializeField]
	private ResourceCostBlind m_CostBlind;

	[SerializeField]
	private SoundTriggerList m_SoundTriggers;

	[SerializeField]
	[Header("ServerRelated")]
	private UIInputTrigger m_ServerTimeoutButton;

	private ServerResponseState m_ServerResponseState;

	private string m_LeaveAnimationName = "Popup_EventFinished_Step3_Leave";

	private List<LootDisplayContoller> m_explodedObjects = new List<LootDisplayContoller>();

	private bool m_initalAnimationSquenceDone;

	private List<string> m_LootIconList = new List<string>();

	private List<List<IInventoryItemGameData>> m_itemListContainer;

	private LootDisplayContoller[] gainedItemSlots;

	private bool m_SpinDone;

	private bool m_ShowSeasonEnd;

	private float m_initialRotation;

	private void Awake()
	{
		m_initialRotation = m_WheelRotateTransform.transform.rotation.eulerAngles.z;
		UISprite[] starSprite = m_StarSprite;
		foreach (UISprite uISprite in starSprite)
		{
			uISprite.spriteName = uISprite.spriteName.Replace("_Desaturated", string.Empty);
			uISprite.spriteName += "_Desaturated";
		}
		m_Model = DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData;
	}

	private void SetDragControllerActive(bool flag)
	{
		if (DIContainerInfrastructure.CurrentDragController != null)
		{
			DIContainerInfrastructure.CurrentDragController.SetActiveDepth(flag, 2);
		}
	}

	private IEnumerator Start()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("result_event_animate");
		SetDragControllerActive(false);
		SetupHeaderLabel();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 2u
		}, true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveLevelDisplay();
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_LeagueFinished_Step0_Enter"));
		RegisterEventHandlers();
		m_ServerResponseState = ServerResponseState.Waiting;
		if (m_Model.CurrentSeasonTurn.IsLegacyLeaderboard || string.IsNullOrEmpty(m_Model.CurrentSeasonTurn.Data.LeaderboardId))
		{
			DebugLog.Warn(GetType(), "Start: Leaderboard ID is empty! Trying to get scores from Hatch directly...");
			DIContainerLogic.PvPSeasonService.GetLeaderboardScores(DIContainerInfrastructure.GetCurrentPlayer(), m_Model.CurrentSeasonTurn.Data.CurrentOpponents, m_Model, FetchingScoresCallback);
		}
		else
		{
			DebugLog.Log(GetType(), "Start: Pulling Leaderboard Update");
			DIContainerLogic.PvPSeasonService.PullLeaderboardUpdate(m_Model, FetchingScoresCallback);
		}
		yield return new WaitForSeconds(5f);
		if (m_ServerResponseState == ServerResponseState.Waiting)
		{
			m_ServerResponseState = ServerResponseState.TimedOut;
			yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_LeagueFinished_Step9_Enter"));
			DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("result_event_animate");
		}
	}

	private void SetupHeaderLabel()
	{
		string value = DIContainerInfrastructure.GetLocaService().Tr(m_Model.Balancing.LocaBaseId + "_name");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("{value_1}", value);
		dictionary.Add("{value_2}", m_Model.Data.CurrentSeason.ToString());
		dictionary.Add("{value_3}", m_Model.Balancing.SeasonTurnAmount.ToString());
		m_PvPSeasonName.text = DIContainerInfrastructure.GetLocaService().Tr("header_pvpinfo", dictionary);
	}

	private void EnterStarRatingState()
	{
		if (m_ServerResponseState != ServerResponseState.TimedOut)
		{
			m_ServerResponseState = ServerResponseState.SuccessReceived;
			m_LeaveAnimationName = "Popup_LeagueFinished_Step3_Leave";
			SetInitialContent();
			StartCoroutine(EnterStarRatingCoroutine());
		}
	}

	private IEnumerator EnterStarRatingCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("result_event_animate");
		float animationLength = base.gameObject.PlayAnimationOrAnimatorState("Popup_LeagueFinished_Step1_Enter");
		yield return new WaitForSeconds(0.5f);
		for (int i = 0; i < 3; i++)
		{
			if (m_starList[i])
			{
				m_StarGainedParticle[i].Play();
			}
		}
		yield return new WaitForSeconds(animationLength - 0.5f);
		gainedItemSlots = new LootDisplayContoller[0];
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, OnContinueButtonClicked);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("result_event_animate");
	}

	private void FetchingScoresCallback(RESTResultEnum result)
	{
		if (m_ServerResponseState != ServerResponseState.TimedOut)
		{
			if (result != RESTResultEnum.Success)
			{
				m_ServerResponseState = ServerResponseState.ErrorReceived;
				DebugLog.Error(GetType(), string.Concat("FetchingScoresError: We got the following errorcode: ", result, ". But we're still going to give the player his reward because we're good guys like that 8)"));
				EnterStarRatingState();
			}
			else
			{
				EnterStarRatingState();
			}
		}
	}

	private void SetInitialContent()
	{
		m_starList.Clear();
		m_ShowSeasonEnd = false;
		for (int i = 0; i < 3; i++)
		{
			m_starList.Add(false);
		}
		int num = 0;
		for (int j = 0; j < m_Model.CurrentSeasonTurn.ResultStars; j++)
		{
			m_starList[j] = true;
			num++;
		}
		DIContainerInfrastructure.AudioManager.PlaySound("UI_Camp_Crafting_" + num + "Star");
		InitialSetupLeagueChange();
		if ((bool)m_RankBonus)
		{
			if (DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(DIContainerInfrastructure.GetCurrentPlayer()) && m_Model.Balancing.PvPBonusLootTablesPerRank.Count > m_Model.CurrentSeasonTurn.ResultRank - 1)
			{
				List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(new Dictionary<string, int> { 
				{
					m_Model.CurrentSeasonTurn.GetScalingRankRewardLootTable(),
					1
				} }, DIContainerInfrastructure.GetCurrentPlayer().Data.Level));
				IInventoryItemGameData inventoryItemGameData = itemsFromLoot.FirstOrDefault();
				if (inventoryItemGameData != null)
				{
					m_RankBonus.gameObject.SetActive(true);
					m_RankBonus.SetModel(inventoryItemGameData.ItemAssetName, null, inventoryItemGameData.ItemValue, string.Empty);
				}
				else
				{
					m_RankBonus.gameObject.SetActive(false);
				}
			}
			else
			{
				m_RankBonus.gameObject.SetActive(false);
			}
		}
		m_RankLabel.text = m_Model.CurrentSeasonTurn.ResultRank.ToString("0");
		m_PvPPoints.SetModel(string.Empty, null, m_Model.CurrentSeasonTurn.Data.CurrentScore, string.Empty);
		int num2 = 0;
		for (int k = 0; k < 3; k++)
		{
			m_StarSprite[k].spriteName = m_StarSprite[k].spriteName.Replace("_Desaturated", string.Empty);
			if (!m_starList[k])
			{
				m_StarSprite[k].spriteName = m_StarSprite[k].spriteName + "_Desaturated";
			}
			else
			{
				num2++;
			}
		}
		if ((bool)m_SoundTriggers)
		{
			m_SoundTriggers.OnTriggerEventFired("star_result_" + num2);
		}
	}

	private void InitialSetupLeagueChange()
	{
		if (!m_NextLeaguePreviewRoot || !DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(DIContainerInfrastructure.GetCurrentPlayer()))
		{
			return;
		}
		PvPSeasonManagerGameData currentPvPSeasonGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData;
		m_CurrentLeaguePreviewCrown.spriteName = PvPSeasonManagerGameData.GetLeagueAssetName(currentPvPSeasonGameData.Data.CurrentLeague);
		int rankChangeByRanking = DIContainerLogic.PvPSeasonService.GetRankChangeByRanking(currentPvPSeasonGameData);
		int rankChangeByMissedSeasons = DIContainerLogic.PvPSeasonService.GetRankChangeByMissedSeasons(currentPvPSeasonGameData);
		int num = rankChangeByRanking + rankChangeByMissedSeasons;
		int league = Mathf.Max(1, currentPvPSeasonGameData.Data.CurrentLeague + num);
		m_FullPreviewTabRoot.SetActive(true);
		if (num == 0 && currentPvPSeasonGameData.Data.CurrentLeague < currentPvPSeasonGameData.Balancing.MaxLeague)
		{
			m_NextLeaguePreviewRoot.SetActive(false);
			m_LeagueStayRoot.SetActive(true);
			m_NextLeaguePreviewHeader.text = DIContainerInfrastructure.GetLocaService().Tr("pvp_league_information_name");
			return;
		}
		if (num == 0 && currentPvPSeasonGameData.Data.CurrentLeague >= currentPvPSeasonGameData.Balancing.MaxLeague)
		{
			m_FullPreviewTabRoot.SetActive(false);
			return;
		}
		bool flag = num > 0;
		m_NextLeaguePreviewRoot.SetActive(true);
		m_LeagueStayRoot.SetActive(false);
		m_NextLeaguePreviewEnterAnimator.Play("NextLeaguePreview_Enter", 0, 0f);
		m_NextLeaguePreviewName.text = DIContainerInfrastructure.GetLocaService().GetLeagueName(league);
		m_NextLeaguePreviewCrown.spriteName = PvPSeasonManagerGameData.GetLeagueAssetName(league);
		m_NextLeaguePreviewHeader.text = DIContainerInfrastructure.GetLocaService().Tr((!flag) ? "pvp_league_demotion" : "pvp_league_promotion");
		if ((bool)m_NextLeaguePreviewAnimator)
		{
			m_NextLeaguePreviewAnimator.Play((!flag) ? "LeaguePreview_Demotion" : "LeaguePreview_Promotion", 0, 0f);
		}
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		m_WheelButton.Clicked += OnWheelButtonClicked;
		if ((bool)m_ContinueButton)
		{
			m_ContinueButton.Clicked += OnContinueButtonClicked;
		}
		if ((bool)m_LeaderBoardButton)
		{
			m_LeaderBoardButton.Clicked += OnLeaderBoardButtonClicked;
		}
		if ((bool)m_RerollButton)
		{
			m_RerollButton.Clicked += OnRerollButtonClicked;
		}
		if ((bool)m_ConfirmEventButton)
		{
			m_ConfirmEventButton.Clicked += OnConfirmEventButtonClicked;
		}
		if ((bool)m_ServerTimeoutButton)
		{
			m_ServerTimeoutButton.Clicked += OnServerTimeoutButtonClicked;
		}
	}

	private void OnServerTimeoutButtonClicked()
	{
		m_LeaveAnimationName = "Popup_EventFinished_Step9_Leave";
		StartCoroutine(LeaveCoroutine());
	}

	private void DeRegisterEventHandlers(bool buttonsOnly = false)
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(2);
		if (!buttonsOnly)
		{
			m_WheelButton.Clicked -= OnWheelButtonClicked;
		}
		if ((bool)m_ContinueButton)
		{
			m_ContinueButton.Clicked -= OnContinueButtonClicked;
		}
		if ((bool)m_RerollButton)
		{
			m_RerollButton.Clicked -= OnRerollButtonClicked;
		}
		if ((bool)m_LeaderBoardButton)
		{
			m_LeaderBoardButton.Clicked -= OnLeaderBoardButtonClicked;
		}
		if ((bool)m_ConfirmEventButton)
		{
			m_ConfirmEventButton.Clicked -= OnConfirmEventButtonClicked;
		}
		if ((bool)m_ServerTimeoutButton)
		{
			m_ServerTimeoutButton.Clicked -= OnServerTimeoutButtonClicked;
		}
	}

	private void OnContinueButtonClicked()
	{
		DeRegisterEventHandlers();
		if (m_Model != null && m_Model.CurrentSeasonTurn != null && m_Model.CurrentSeasonTurn.ResultStars == 0)
		{
			if ((bool)m_NextLeaguePreviewRoot && m_NextLeaguePreviewRoot.activeInHierarchy)
			{
				m_NextLeaguePreviewEnterAnimator.Play("NextLeaguePreview_Leave", 0, 0f);
			}
			OnConfirmEventButtonClicked();
		}
		else
		{
			StartCoroutine(EnterResultLootWheel());
		}
	}

	private void OnLeaderBoardButtonClicked()
	{
		DIContainerInfrastructure.BaseStateMgr.ShowLeaderBoardScreen(false);
	}

	private void OnRerollButtonClicked()
	{
		DebugLog.Log("Reroll");
		m_SpinDone = false;
		if (DIContainerLogic.PvPSeasonService.IsResultRerollPossible(m_Model, DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData) && DIContainerLogic.PvPSeasonService.ExecuteResultRerollCost(m_Model, DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData))
		{
			DeRegisterEventHandlers(true);
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateFriendshipEssenceBar();
			base.gameObject.PlayAnimationOrAnimatorState("Popup_LeagueFinished_Step3_Step2");
			for (int i = 0; i < m_explodedObjects.Count; i++)
			{
				m_explodedObjects[i].HideThenDestroy();
			}
			m_explodedObjects.Clear();
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
			{
				Depth = 2u
			}, true);
			DIContainerLogic.PvPSeasonService.RerollPvPSeasonResultLoot(m_Model, DIContainerInfrastructure.GetCurrentPlayer());
			SetLootIcons(0);
			m_initalAnimationSquenceDone = true;
			StartCoroutine(ReSpinWheelSequence());
			return;
		}
		Requirement rerollResultRequirement = m_Model.GetRerollResultRequirement();
		if (rerollResultRequirement == null || rerollResultRequirement.RequirementType != RequirementType.PayItem)
		{
			return;
		}
		IInventoryItemGameData data = null;
		if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, rerollResultRequirement.NameId, out data))
		{
			if (data.ItemBalancing.NameId == "lucky_coin")
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.m_PlayerStatsController[1].m_StatBar.SwitchToShop();
			}
			else if (data.ItemBalancing.NameId == "gold")
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.m_PlayerStatsController[0].m_StatBar.SwitchToShop();
			}
			else if (data.ItemBalancing.NameId == "friendship_essence")
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.m_PlayerStatsController[2].m_StatBar.SwitchToShop();
			}
		}
	}

	private void OnConfirmEventButtonClicked()
	{
		DeRegisterEventHandlers();
		DIContainerLogic.NotificationPopupController.RequestNotificationPopupForReason(NotificationPopupTrigger.Pvpfinish);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(2u);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("result_won_enter_result");
		PvPSeasonManagerGameData currentPvPSeasonGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData;
		DIContainerLogic.PvPSeasonService.ConfirmCurrentPvPTurn(DIContainerInfrastructure.GetCurrentPlayer());
		DIContainerInfrastructure.BaseStateMgr.RefreshCampContent();
		if (DIContainerInfrastructure.BaseStateMgr is ArenaCampStateMgr)
		{
			(DIContainerInfrastructure.BaseStateMgr as ArenaCampStateMgr).RefreshBannerMarkers();
		}
		StartCoroutine(LeaveCoroutine());
	}

	private IEnumerator LeaveCoroutine()
	{
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState(m_LeaveAnimationName));
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.EnterLevelDisplay();
		Object.Destroy(base.gameObject);
	}

	private IEnumerator ReSpinWheelSequence()
	{
		yield return StartCoroutine(ResetWheelSequence());
	}

	private void OnWheelButtonClicked()
	{
		if (m_initalAnimationSquenceDone)
		{
			DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(2);
			StartCoroutine(SpinWheelSequence());
			m_initalAnimationSquenceDone = false;
		}
	}

	private IEnumerator EnterResultLootWheel()
	{
		m_WheelOfLootRoot.SetActive(true);
		SetLootIcons(0);
		if ((bool)m_NextLeaguePreviewRoot && m_NextLeaguePreviewRoot.activeInHierarchy)
		{
			m_NextLeaguePreviewEnterAnimator.Play("NextLeaguePreview_Leave", 0, 0f);
		}
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_LeagueFinished_Step1_Step2"));
		for (int i = 0; i < m_Model.CurrentSeasonTurn.ResultStars; i++)
		{
			m_RewardItemAnimation[i].Play("WheelOfLoot_RewardUnlocked");
		}
		m_RankingRoot.SetActive(false);
		m_FirstOptionsRoot.SetActive(false);
		m_initalAnimationSquenceDone = true;
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(3, OnWheelButtonClicked);
		RegisterEventHandlers();
	}

	private void SetLootIcons(int offset)
	{
		m_itemListContainer = new List<List<IInventoryItemGameData>>();
		Dictionary<string, LootInfoData> dictionary = new Dictionary<string, LootInfoData>();
		int num = -1;
		int resultStars = m_Model.CurrentSeasonTurn.ResultStars;
		foreach (string key in m_Model.CurrentSeasonTurn.RolledResultLoot.Keys)
		{
			LootInfoData lootInfoData = m_Model.CurrentSeasonTurn.RolledResultLoot[key];
			dictionary.Add(key, new LootInfoData
			{
				Level = lootInfoData.Level,
				Quality = lootInfoData.Quality,
				Value = lootInfoData.Value
			});
		}
		LootTableBalancingData balancing = null;
		if (DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(m_Model.GetSeasonTurnLootTableWheel(), out balancing))
		{
			if (balancing.LootTableEntries.Count != 8)
			{
				DebugLog.Log("Wheel LootTable for Battles does not contains 8 entrys instead it has " + balancing.LootTableEntries.Count);
			}
		}
		else
		{
			DebugLog.Error("No Wheel LootTable set for battle ");
		}
		for (int i = 0; i < balancing.LootTableEntries.Count; i++)
		{
			LootTableEntry lootTableEntry = balancing.LootTableEntries[i];
			LootTableBalancingData balancing2 = null;
			if (DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(lootTableEntry.NameId, out balancing2))
			{
				DebugLog.Log("Entry was Chest: " + lootTableEntry.NameId);
				m_itemListContainer.Add(new List<IInventoryItemGameData>());
				num = m_itemListContainer.Count - 1;
			}
			else
			{
				m_itemListContainer.Add(new List<IInventoryItemGameData> { DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, DIContainerInfrastructure.GetCurrentPlayer().Data.Level, 1, lootTableEntry.NameId, lootTableEntry.BaseValue, EquipmentSource.LootBird) });
			}
		}
		int num2 = m_Model.CurrentSeasonTurn.Data.CachedRolledResultWheelIndex;
		bool flag = false;
		for (int j = 1; j <= resultStars; j++)
		{
			List<IInventoryItemGameData> list = m_itemListContainer[num2];
			if (num != num2)
			{
				dictionary[list[0].ItemBalancing.NameId].Value -= list[0].ItemValue;
			}
			else
			{
				flag = true;
			}
			num2++;
			if (num2 >= 8)
			{
				num2 -= 8;
			}
		}
		if (flag)
		{
			foreach (string key2 in dictionary.Keys)
			{
				LootInfoData lootInfoData2 = dictionary[key2];
				IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(key2);
				if (balancingData != null && balancingData.ItemType == InventoryItemType.PlayerStats && num >= 0)
				{
					m_itemListContainer[num].Add(DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, DIContainerInfrastructure.GetCurrentPlayer().Data.Level, 1, key2, lootInfoData2.Value, EquipmentSource.LootBird));
					continue;
				}
				for (int k = 0; k < lootInfoData2.Value; k++)
				{
					m_itemListContainer[num].Add(DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, DIContainerInfrastructure.GetCurrentPlayer().Data.Level, 1, key2, 1, EquipmentSource.LootBird));
				}
			}
		}
		num2 = 0;
		for (int l = 0; l < m_itemListContainer.Count; l++)
		{
			LootDisplayType displayType = LootDisplayType.None;
			if ((l + 1) % 8 == 1)
			{
				displayType = LootDisplayType.Major;
			}
			if (m_itemListContainer[l].Count == 1)
			{
				m_LootItemSlots[(l + 1) % 8].SetModel(m_itemListContainer[l][0], new List<IInventoryItemGameData>(), displayType);
			}
			else if (m_itemListContainer[l].Count == 0)
			{
				DebugLog.Log("Empty Chest");
				m_LootItemSlots[(l + 1) % 8].SetModel(null, m_itemListContainer[l], displayType);
			}
			else
			{
				m_LootItemSlots[(l + 1) % 8].SetModel(null, m_itemListContainer[l], displayType);
			}
		}
	}

	private void SetResultLootIcons()
	{
		int num = m_Model.CurrentSeasonTurn.Data.CachedRolledResultWheelIndex;
		DebugLog.Log(gainedItemSlots.Length);
		gainedItemSlots = new LootDisplayContoller[m_Model.CurrentSeasonTurn.ResultStars];
		for (int i = 1; i <= m_Model.CurrentSeasonTurn.ResultStars; i++)
		{
			Transform transform = m_LootObjects[i - 1].transform;
			gainedItemSlots[i - 1] = Object.Instantiate(m_MajorLootItemSlot, transform.position, Quaternion.identity) as LootDisplayContoller;
			gainedItemSlots[i - 1].transform.parent = transform;
			LootDisplayType displayType = LootDisplayType.Minor;
			if (num == 0)
			{
				displayType = LootDisplayType.Major;
			}
			if (m_itemListContainer[num].Count == 1)
			{
				gainedItemSlots[i - 1].SetModel(m_itemListContainer[num][0], new List<IInventoryItemGameData>(), displayType);
			}
			else
			{
				gainedItemSlots[i - 1].SetModel(null, m_itemListContainer[num], displayType);
			}
			num++;
			if (num >= 8)
			{
				num -= 8;
			}
		}
	}

	private IEnumerator ResetWheelSequence()
	{
		for (int k = 0; k < m_Model.CurrentSeasonTurn.ResultStars; k++)
		{
			m_RewardItemAnimation[k].Play("WheelOfLoot_RewardReset");
		}
		yield return new WaitForEndOfFrame();
		for (int j = 0; j < m_Model.CurrentSeasonTurn.ResultStars; j++)
		{
			m_RewardItemAnimation[j].Play("WheelOfLoot_RewardUnlocked");
		}
		for (int i = 0; i < gainedItemSlots.Length; i++)
		{
			gainedItemSlots[i].HideThenDestroy();
		}
		m_WheelOfLootRoot.SetActive(true);
		do
		{
			yield return null;
		}
		while (m_RewardItemAnimation[0].isPlaying);
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(3, OnWheelButtonClicked);
	}

	private IEnumerator SpinWheelSequence()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("result_won_spin");
		ISound wheel = DIContainerInfrastructure.AudioManager.PlaySound("UI_Wheel");
		m_SpinningWheelAnimation.Play("WheelOfLoot_StartSpinning");
		if ((bool)m_SoundTriggers)
		{
			m_SoundTriggers.OnTriggerEventFired("wheel_spinning");
		}
		do
		{
			yield return null;
		}
		while (m_SpinningWheelAnimation.isPlaying);
		m_WheelRotateTransform.transform.localEulerAngles = new Vector3(0f, 0f, m_initialRotation);
		m_WheelRotateTransform.Rotate(0f, 0f, (float)(m_Model.CurrentSeasonTurn.Data.CachedRolledResultWheelIndex + 1) * 45f);
		m_SpinningWheelAnimation.Play("WheelOfLoot_EndSpinning");
		do
		{
			yield return null;
		}
		while (m_SpinningWheelAnimation.isPlaying);
		if (wheel != null)
		{
			wheel.Stop();
		}
		yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeForShowRewardWheel * 0.5f);
		int stars = m_Model.CurrentSeasonTurn.ResultStars;
		int wheelIndex = m_Model.CurrentSeasonTurn.Data.CachedRolledResultWheelIndex;
		if ((stars >= 1 && wheelIndex == 0) || (stars >= 2 && wheelIndex == 7) || (stars >= 3 && wheelIndex == 6))
		{
			if ((bool)m_SoundTriggers)
			{
				m_SoundTriggers.OnTriggerEventFired("reward_main");
			}
		}
		else if ((bool)m_SoundTriggers)
		{
			m_SoundTriggers.OnTriggerEventFired("reward_base");
		}
		for (int i = 0; i < m_Model.CurrentSeasonTurn.ResultStars; i++)
		{
			m_RewardItemAnimation[i].Play("WheelOfLoot_RewardGained");
		}
		do
		{
			yield return null;
		}
		while (m_RewardItemAnimation[0].isPlaying);
		yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeForShowRewardWheel);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("result_won_spin");
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(3);
		StartCoroutine(ShowRollResult());
	}

	private IEnumerator ShowRollResult()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("result_won_enter_result");
		m_SpinDone = true;
		m_LootRoot.SetActive(true);
		SetResultLootIcons();
		Requirement rerollCost = DIContainerLogic.PvPSeasonService.GetRerollRequirement(m_Model);
		m_CostBlind.SetModel(DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(rerollCost.NameId).AssetBaseId, null, rerollCost.Value, string.Empty);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 2u,
			showFriendshipEssence = true
		}, true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		m_FinalOptionsRoot.SetActive(true);
		float timeForAnimation = base.gameObject.PlayAnimationOrAnimatorState("Popup_LeagueFinished_Step2_Step3");
		for (int j = 0; j < 3; j++)
		{
			if (j < m_Model.CurrentSeasonTurn.ResultStars)
			{
				m_LootObjects[j].GetComponentInChildren<LootDisplayContoller>().PlayGainedAnimation();
			}
		}
		yield return new WaitForSeconds(timeForAnimation);
		m_WheelOfLootRoot.SetActive(false);
		RegisterEventHandlers();
		yield return new WaitForSeconds(1f);
		m_explodedObjects = new List<LootDisplayContoller>();
		for (int i = 0; i < 3; i++)
		{
			if (m_Model != null && m_Model.CurrentSeasonTurn != null && i < m_Model.CurrentSeasonTurn.ResultStars)
			{
				LootDisplayContoller lootDisplay = m_LootObjects[i].GetComponentInChildren<LootDisplayContoller>();
				if ((bool)lootDisplay)
				{
					m_explodedObjects.AddRange(lootDisplay.Explode(true, true, 0.5f, true, 0f, 0f));
				}
			}
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("result_won_enter_result");
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, OnConfirmEventButtonClicked);
	}

	private void OnDestroy()
	{
		DeRegisterEventHandlers();
		if ((bool)DIContainerInfrastructure.GetCoreStateMgr())
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(2u);
		}
		m_WheelButton.Clicked -= OnWheelButtonClicked;
	}

	public void ShowCrownTooltip()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(currentPlayer))
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowArenaLeagueOverlay(m_CurrentLeaguePreviewCrown.transform, currentPlayer.CurrentPvPSeasonGameData.Data.CurrentLeague, currentPlayer.CurrentPvPSeasonGameData.Data.CurrentRank, true);
		}
	}
}

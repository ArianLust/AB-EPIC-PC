using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using Rcs;
using UnityEngine;

public class EventResultUI : MonoBehaviour
{
	private const float m_ServerTimeoutThreshold = 5f;

	private int starCount = 3;

	private EventManagerGameData m_Model;

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

	private List<bool> m_starList = new List<bool>();

	[SerializeField]
	private UILabel m_EventName;

	[SerializeField]
	private UILabel m_RankLabel;

	[SerializeField]
	private ResourceCostBlind m_EventPoints;

	[SerializeField]
	private ResourceCostBlind m_RankBonus;

	[SerializeField]
	private ResourceCostBlind m_CostBlind;

	[SerializeField]
	private SoundTriggerList m_SoundTriggers;

	[SerializeField]
	private GameObject m_EventCampaignInfo;

	[SerializeField]
	private UILabel m_EventCampaignInfoText;

	[SerializeField]
	private UILabel m_EventCampaignInfoHeader;

	[SerializeField]
	private CollectionItemSlot m_EventCampaignRewardSlot;

	[SerializeField]
	private GameObject m_BossInfoObject;

	[SerializeField]
	private UILabel m_OwnTeamLabel;

	[SerializeField]
	private UILabel m_EnemyTeamLabel;

	[SerializeField]
	private UILabel m_OwnTeamScore;

	[SerializeField]
	private UILabel m_EnemyTeamScore;

	[SerializeField]
	private GameObject m_OwnTeamHighlight;

	[SerializeField]
	private GameObject m_EnemyTeamHighlight;

	[SerializeField]
	private UILabel m_BossDescription;

	[SerializeField]
	private Animation m_TeamWinAnimation;

	[SerializeField]
	private List<Color> m_TeamColors;

	[SerializeField]
	private UIInputTrigger m_ServerTimeoutButton;

	private ServerResponseState m_ServerResponseState;

	private string m_LeaveAnimationName = "Popup_EventFinished_Step3_Leave";

	private List<LootDisplayContoller> m_explodedObjects = new List<LootDisplayContoller>();

	private bool m_initalAnimationSquenceDone;

	private List<string> m_LootIconList = new List<string>();

	private List<List<IInventoryItemGameData>> m_itemListContainer;

	private LootDisplayContoller[] gainedItemSlots;

	private bool m_SpinDone;

	private bool m_OwnTeamWonInBossEvent;

	private float m_initialRotation;

	private WorldBossTeamData m_ownTeam;

	private WorldBossTeamData m_enemyTeam;

	private void Awake()
	{
		m_initialRotation = m_WheelRotateTransform.transform.rotation.eulerAngles.z;
		UISprite[] starSprite = m_StarSprite;
		foreach (UISprite uISprite in starSprite)
		{
			uISprite.spriteName = uISprite.spriteName.Replace("_Desaturated", string.Empty);
			uISprite.spriteName += "_Desaturated";
		}
		m_Model = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
	}

	private void SetDragControllerActive(bool flag)
	{
		if (DIContainerInfrastructure.CurrentDragController != null)
		{
			DIContainerInfrastructure.CurrentDragController.SetActiveDepth(flag, 1);
		}
	}

	private IEnumerator Start()
	{
		if (m_Model == null)
		{
			DebugLog.Error(GetType(), "Start: Model is null! cancelling start routine...");
			yield break;
		}
		DIContainerInfrastructure.LocationStateMgr.WorldMenuUI.Leave();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("result_event_animate");
		SetDragControllerActive(false);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 2u
		}, true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveLevelDisplay();
		m_starList.Clear();
		m_EventName.text = DIContainerInfrastructure.GetLocaService().Tr(m_Model.EventBalancing.LocaBaseId + "_name");
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_EventFinished_Step0_Enter"));
		RegisterEventHandlers();
		m_ServerResponseState = ServerResponseState.Waiting;
		if (string.IsNullOrEmpty(m_Model.Data.LeaderboardId))
		{
			DebugLog.Warn(GetType(), "Start: Leaderboard ID is empty! Trying to get scores from Hatch directly...");
			DIContainerLogic.EventSystemService.GetLeaderboardScores(DIContainerInfrastructure.GetCurrentPlayer(), m_Model.Data.CurrentOpponents, m_Model, EnterStarRatingState, FetchingScoresError);
		}
		else
		{
			DebugLog.Log(GetType(), "Start: Pulling Leaderboard Update");
			DIContainerLogic.EventSystemService.PullLeaderboardUpdate(m_Model, EnterStarRatingState, FetchingScoresError);
		}
		yield return new WaitForSeconds(5f);
		if (m_ServerResponseState == ServerResponseState.Waiting)
		{
			m_ServerResponseState = ServerResponseState.TimedOut;
			base.gameObject.PlayAnimationOrAnimatorState("Popup_EventFinished_Step9_Enter");
		}
	}

	private void OnServerTimeoutButtonClicked()
	{
		m_LeaveAnimationName = "Popup_EventFinished_Step9_Leave";
		StartCoroutine(LeaveCoroutine());
	}

	private void FetchingScoresError(int errorCode)
	{
		if (m_ServerResponseState != ServerResponseState.TimedOut)
		{
			m_ServerResponseState = ServerResponseState.ErrorReceived;
			DebugLog.Error(GetType(), "FetchingScoresError: We got the following errorcode: " + errorCode + ". But we're still going to give the player his reward because we're good guys like that 8)");
			EnterStarRatingState();
		}
	}

	private void EnterStarRatingState()
	{
		if (m_ServerResponseState != ServerResponseState.TimedOut)
		{
			m_ServerResponseState = ServerResponseState.SuccessReceived;
			m_LeaveAnimationName = "Popup_EventFinished_Step3_Leave";
			StartCoroutine(EnterStarRatingStateCoroutine());
		}
	}

	private IEnumerator EnterStarRatingStateCoroutine()
	{
		for (int l = 0; l < 3; l++)
		{
			m_starList.Add(false);
		}
		int staramount = 0;
		for (int k = 0; k < m_Model.ResultStars; k++)
		{
			m_starList[k] = true;
			staramount++;
		}
		DIContainerInfrastructure.AudioManager.PlaySound("UI_Camp_Crafting_" + staramount + "Star");
		HandleEventAchievements();
		if (m_Model.IsBossEvent)
		{
			m_OwnTeamWonInBossEvent = SetupBossBanner();
		}
		HandleEnterRewardNote();
		HandleEnterRankBonus();
		m_EventPoints.SetModel(string.Empty, null, m_Model.Data.CurrentScore, string.Empty);
		int stars = 0;
		for (int j = 0; j < 3; j++)
		{
			m_StarSprite[j].spriteName = m_StarSprite[j].spriteName.Replace("_Desaturated", string.Empty);
			if (!m_starList[j])
			{
				m_StarSprite[j].spriteName = m_StarSprite[j].spriteName + "_Desaturated";
			}
			else
			{
				stars++;
			}
		}
		if ((bool)m_SoundTriggers)
		{
			m_SoundTriggers.OnTriggerEventFired("star_result_" + stars);
		}
		float animationLength = base.gameObject.PlayAnimationOrAnimatorState("Popup_EventFinished_Step1_Enter");
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
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, ContinueButtonClicked);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("result_event_animate");
	}

	private void HandleEnterRankBonus()
	{
		if ((bool)m_RankBonus)
		{
			if (DIContainerLogic.EventSystemService.IsCurrentEventAvailable(DIContainerInfrastructure.GetCurrentPlayer()) && DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.EventBalancing.EventBonusLootTablesPerRank.Count > m_Model.ResultRank - 1)
			{
				List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(new Dictionary<string, int> { 
				{
					DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.GetScalingRankRewardLootTable(),
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
		m_RankLabel.text = m_Model.ResultRank.ToString("0");
	}

	private void HandleEnterRewardNote()
	{
		DebugLog.Log(GetType(), "HandleEnterRewardNote: event = " + m_Model);
		if (DIContainerLogic.EventSystemService.GetCollectionGroupBalancingForEvent() != null && ((m_Model.CurrentMiniCampaign != null && m_Model.CurrentMiniCampaign.Data.RewardStatus == EventCampaignRewardStatus.locked) || (m_Model.IsBossEvent && DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss.RewardStatus == EventCampaignRewardStatus.locked)))
		{
			CollectionGroupBalancingData collectionGroupBalancingForEvent = DIContainerLogic.EventSystemService.GetCollectionGroupBalancingForEvent();
			Dictionary<string, int> loot = ((!DIContainerInfrastructure.EventSystemStateManager.UseCollectionFallbackReward(m_Model) && DIContainerLogic.EventSystemService.GetAvailableEliteChestReward(DIContainerInfrastructure.GetCurrentPlayer()).Count != 0) ? collectionGroupBalancingForEvent.Reward : collectionGroupBalancingForEvent.FallbackReward);
			List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(loot, 1));
			m_EventCampaignRewardSlot.SetModel(itemsFromLoot[0]);
			ClassItemGameData classItemGameData = itemsFromLoot[0] as ClassItemGameData;
			SkinItemGameData skinItemGameData = itemsFromLoot[0] as SkinItemGameData;
			if (classItemGameData != null || skinItemGameData != null)
			{
				string text = ((classItemGameData == null) ? skinItemGameData.ItemLocalizedName : classItemGameData.ItemLocalizedName);
				string text2 = ((classItemGameData == null) ? skinItemGameData.GetOriginalClassBalancingData().RestrictedBirdId : classItemGameData.BalancingData.RestrictedBirdId);
				DebugLog.Log(GetType(), "HandleEnterRewardNote: reward for this event = " + text + " for " + text2);
				ABHLocaService locaService = DIContainerInfrastructure.GetLocaService();
				string text3 = locaService.Tr("event_end_information_desc", DIContainerInfrastructure.EventSystemStateManager.GetCollectionRewardReplacementDict());
				string locaId = DIContainerBalancing.Service.GetBalancingData<BirdBalancingData>(text2).LocaId;
				text3 = text3.Replace("{value_1}", text);
				text3 = text3.Replace("{value_2}", locaService.GetCharacterName(locaId));
				m_EventCampaignInfoText.text = text3;
				m_EventCampaignInfoHeader.text = locaService.Tr("event_end_information_name");
				m_EventCampaignInfo.SetActive(true);
			}
		}
	}

	private void HandleEventAchievements()
	{
		AchievementData achievementTracking = DIContainerInfrastructure.GetCurrentPlayer().Data.AchievementTracking;
		if (!achievementTracking.EventCompletedNinja && m_Model.EventBalancing.NameId.Contains("event_invasion_ninjas"))
		{
			string achievementIdForStoryItemIfExists = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("eventNinja");
			if (!string.IsNullOrEmpty(achievementIdForStoryItemIfExists))
			{
				DIContainerInfrastructure.GetAchievementService().ReportUnlocked(achievementIdForStoryItemIfExists);
				achievementTracking.EventCompletedNinja = true;
			}
		}
		else if (!achievementTracking.EventCompletedZombie && m_Model.EventBalancing.NameId.Contains("event_invasion_zombies"))
		{
			string achievementIdForStoryItemIfExists2 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("eventZombie");
			if (!string.IsNullOrEmpty(achievementIdForStoryItemIfExists2))
			{
				DIContainerInfrastructure.GetAchievementService().ReportUnlocked(achievementIdForStoryItemIfExists2);
				achievementTracking.EventCompletedZombie = true;
			}
		}
		else if (!achievementTracking.EventCompletedPirate && m_Model.EventBalancing.NameId.Contains("event_invasion_pirates"))
		{
			string achievementIdForStoryItemIfExists3 = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("eventPirates");
			if (!string.IsNullOrEmpty(achievementIdForStoryItemIfExists3))
			{
				DIContainerInfrastructure.GetAchievementService().ReportUnlocked(achievementIdForStoryItemIfExists3);
				achievementTracking.EventCompletedPirate = true;
			}
		}
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		m_WheelButton.Clicked += WheelButtonClicked;
		if ((bool)m_ContinueButton)
		{
			m_ContinueButton.Clicked += ContinueButtonClicked;
		}
		if ((bool)m_LeaderBoardButton)
		{
			m_LeaderBoardButton.Clicked += LeaderBoardButtonClicked;
		}
		if ((bool)m_RerollButton)
		{
			m_RerollButton.Clicked += RerollButtonClicked;
		}
		if ((bool)m_ConfirmEventButton)
		{
			m_ConfirmEventButton.Clicked += ConfirmEventButtonClicked;
		}
		if ((bool)m_ServerTimeoutButton)
		{
			m_ServerTimeoutButton.Clicked += OnServerTimeoutButtonClicked;
		}
	}

	private void DeRegisterEventHandlers(bool buttonsOnly = false)
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(2);
		if (!buttonsOnly)
		{
			m_WheelButton.Clicked -= WheelButtonClicked;
		}
		if ((bool)m_ContinueButton)
		{
			m_ContinueButton.Clicked -= ContinueButtonClicked;
		}
		if ((bool)m_RerollButton)
		{
			m_RerollButton.Clicked -= RerollButtonClicked;
		}
		if ((bool)m_LeaderBoardButton)
		{
			m_LeaderBoardButton.Clicked -= LeaderBoardButtonClicked;
		}
		if ((bool)m_ConfirmEventButton)
		{
			m_ConfirmEventButton.Clicked -= ConfirmEventButtonClicked;
		}
		if ((bool)m_ServerTimeoutButton)
		{
			m_ServerTimeoutButton.Clicked -= OnServerTimeoutButtonClicked;
		}
	}

	private void ContinueButtonClicked()
	{
		DeRegisterEventHandlers();
		if (m_Model != null && m_Model.ResultStars == 0)
		{
			ConfirmEventButtonClicked();
		}
		else
		{
			StartCoroutine(EnterResultLootWheel());
		}
	}

	private void LeaderBoardButtonClicked()
	{
		DIContainerInfrastructure.LocationStateMgr.ShowLeaderBoardScreen(m_ownTeam, m_enemyTeam);
	}

	private void RerollButtonClicked()
	{
		DebugLog.Log("Reroll");
		m_SpinDone = false;
		if (DIContainerLogic.EventSystemService.IsResultRerollPossible(m_Model, DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData) && DIContainerLogic.EventSystemService.ExecuteResultRerollCost(m_Model, DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData))
		{
			DeRegisterEventHandlers(true);
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateFriendshipEssenceBar();
			base.gameObject.PlayAnimationOrAnimatorState("Popup_EventFinished_Step3_Step2");
			for (int i = 0; i < m_explodedObjects.Count; i++)
			{
				m_explodedObjects[i].HideThenDestroy();
			}
			m_explodedObjects.Clear();
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
			{
				Depth = 2u
			}, true);
			DIContainerLogic.EventSystemService.RerollEventResultLoot(m_Model, DIContainerInfrastructure.GetCurrentPlayer());
			SetLootIcons(0);
			m_initalAnimationSquenceDone = true;
			StartCoroutine(ReSpinWheelSequence());
			return;
		}
		Requirement rerollResultRequirement = m_Model.EventBalancing.RerollResultRequirement;
		if (rerollResultRequirement != null && rerollResultRequirement.RequirementType == RequirementType.PayItem)
		{
			IInventoryItemGameData data = null;
			if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, rerollResultRequirement.NameId, out data))
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_MissingCurrencyPopup.EnterPopup(data.ItemBalancing.NameId, rerollResultRequirement.Value);
			}
		}
	}

	private void ConfirmEventButtonClicked()
	{
		DeRegisterEventHandlers();
		DIContainerLogic.NotificationPopupController.RequestNotificationPopupForReason(NotificationPopupTrigger.Eventfinish);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(2u);
		DIContainerLogic.EventSystemService.ConfirmCurrentEvent(DIContainerInfrastructure.GetCurrentPlayer());
		if (DIContainerInfrastructure.LocationStateMgr != null)
		{
			EventSystemWorldMapStateMgr eventsWorldMapStateMgr = DIContainerInfrastructure.LocationStateMgr.EventsWorldMapStateMgr;
			if (eventsWorldMapStateMgr != null)
			{
				eventsWorldMapStateMgr.Restart();
			}
		}
		StartCoroutine(LeaveCoroutine());
	}

	private IEnumerator LeaveCoroutine()
	{
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState(m_LeaveAnimationName));
		if (DIContainerInfrastructure.GetCoreStateMgr().m_EventCampaign)
		{
			DIContainerInfrastructure.GetCoreStateMgr().GotoWorldMap();
		}
		else
		{
			SetDragControllerActive(true);
			DIContainerInfrastructure.LocationStateMgr.WorldMenuUI.Enter();
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.EnterLevelDisplay();
			DIContainerInfrastructure.LocationStateMgr.ProcessRankUpPopUp();
		}
		DIContainerInfrastructure.LocationStateMgr.IsEventResultRunning = false;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private IEnumerator ReSpinWheelSequence()
	{
		yield return StartCoroutine(ResetWheelSequence());
	}

	private void WheelButtonClicked()
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
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_EventFinished_Step1_Step2"));
		for (int i = 0; i < m_Model.ResultStars; i++)
		{
			m_RewardItemAnimation[i].Play("WheelOfLoot_RewardUnlocked");
		}
		m_RankingRoot.SetActive(false);
		m_FirstOptionsRoot.SetActive(false);
		m_initalAnimationSquenceDone = true;
		RegisterEventHandlers();
	}

	private void SetLootIcons(int offset)
	{
		m_itemListContainer = new List<List<IInventoryItemGameData>>();
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		Dictionary<string, LootInfoData> dictionary = new Dictionary<string, LootInfoData>();
		int num = -1;
		int resultStars = m_Model.ResultStars;
		foreach (string key in m_Model.RolledResultLoot.Keys)
		{
			LootInfoData lootInfoData = m_Model.RolledResultLoot[key];
			if (m_OwnTeamWonInBossEvent)
			{
				lootInfoData.Value *= 2;
			}
			LootInfoData lootInfoData2 = new LootInfoData();
			lootInfoData2.Level = lootInfoData.Level;
			lootInfoData2.Quality = lootInfoData.Quality;
			lootInfoData2.Value = lootInfoData.Value;
			LootInfoData value = lootInfoData2;
			dictionary.Add(key, value);
		}
		LootTableBalancingData balancing = null;
		if (DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(m_Model.EventBalancing.EventRewardLootTableWheel.Keys.FirstOrDefault(), out balancing))
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
				m_itemListContainer.Add(new List<IInventoryItemGameData> { DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(currentPlayer.InventoryGameData, currentPlayer.Data.Level, 1, lootTableEntry.NameId, lootTableEntry.BaseValue, EquipmentSource.LootBird) });
			}
		}
		int cachedRolledResultWheelIndex = m_Model.Data.CachedRolledResultWheelIndex;
		for (int j = 0; j < m_itemListContainer.Count; j++)
		{
			LootDisplayType displayType = LootDisplayType.None;
			if ((j + 1) % 8 == 1)
			{
				displayType = LootDisplayType.Major;
			}
			if (m_itemListContainer[j].Count == 1)
			{
				m_LootItemSlots[(j + 1) % 8].SetModel(m_itemListContainer[j][0], new List<IInventoryItemGameData>(), displayType, "_Large", false, false, false, null, false, m_OwnTeamWonInBossEvent);
			}
			else if (m_itemListContainer[j].Count == 0)
			{
				DebugLog.Log("Empty Chest");
				m_LootItemSlots[(j + 1) % 8].SetModel(null, m_itemListContainer[j], displayType, "_Large", false, false, false, null, false, m_OwnTeamWonInBossEvent);
			}
			else
			{
				m_LootItemSlots[(j + 1) % 8].SetModel(null, m_itemListContainer[j], displayType, "_Large", false, false, false, null, false, m_OwnTeamWonInBossEvent);
			}
		}
	}

	private void SetResultLootIcons()
	{
		int num = m_Model.Data.CachedRolledResultWheelIndex;
		DebugLog.Log(gainedItemSlots.Length);
		gainedItemSlots = new LootDisplayContoller[m_Model.ResultStars];
		for (int i = 1; i <= m_Model.ResultStars; i++)
		{
			Transform transform = m_LootObjects[i - 1].transform;
			gainedItemSlots[i - 1] = UnityEngine.Object.Instantiate(m_MajorLootItemSlot, transform.position, Quaternion.identity) as LootDisplayContoller;
			gainedItemSlots[i - 1].transform.parent = transform;
			LootDisplayType displayType = LootDisplayType.Minor;
			if (num == 0)
			{
				displayType = LootDisplayType.Major;
			}
			if (m_itemListContainer[num].Count == 1)
			{
				gainedItemSlots[i - 1].SetModel(m_itemListContainer[num][0], new List<IInventoryItemGameData>(), displayType, "_Large", false, false, false, null, false, m_OwnTeamWonInBossEvent);
			}
			else
			{
				gainedItemSlots[i - 1].SetModel(null, m_itemListContainer[num], displayType, "_Large", false, false, false, null, false, m_OwnTeamWonInBossEvent);
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
		for (int k = 0; k < m_Model.ResultStars; k++)
		{
			m_RewardItemAnimation[k].Play("WheelOfLoot_RewardReset");
		}
		yield return new WaitForEndOfFrame();
		for (int j = 0; j < m_Model.ResultStars; j++)
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
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, WheelButtonClicked);
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
		m_WheelRotateTransform.Rotate(0f, 0f, (float)(m_Model.Data.CachedRolledResultWheelIndex + 1) * 45f);
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
		int stars = m_Model.ResultStars;
		int wheelIndex = m_Model.Data.CachedRolledResultWheelIndex;
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
		for (int i = 0; i < m_Model.ResultStars; i++)
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
		StartCoroutine(ShowRollResult());
	}

	private void ShowRerollButtons()
	{
		Requirement rerollRequirement = DIContainerLogic.CraftingService.GetRerollRequirement();
		m_CostBlind.SetModel(DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(rerollRequirement.NameId).AssetBaseId, null, rerollRequirement.Value, string.Empty);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 2u,
			showFriendshipEssence = true
		}, true);
		m_WheelAnimation.Play("BattleWon_Step2_ShowButtons");
	}

	private IEnumerator ShowRollResult()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("result_won_enter_result");
		m_SpinDone = true;
		m_LootRoot.SetActive(true);
		SetResultLootIcons();
		Requirement rerollCost = DIContainerLogic.EventSystemService.GetRerollRequirement(m_Model.EventBalancing);
		m_CostBlind.SetModel(DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(rerollCost.NameId).AssetBaseId, null, rerollCost.Value, string.Empty);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 2u,
			showFriendshipEssence = true
		}, true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		m_FinalOptionsRoot.SetActive(true);
		float timeForAnimation = base.gameObject.PlayAnimationOrAnimatorState("Popup_EventFinished_Step2_Step3");
		for (int j = 0; j < 3; j++)
		{
			if (j < m_Model.ResultStars)
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
			if (i < m_Model.ResultStars)
			{
				LootDisplayContoller lootDisplay = m_LootObjects[i].GetComponentInChildren<LootDisplayContoller>();
				if ((bool)lootDisplay)
				{
					m_explodedObjects.AddRange(lootDisplay.Explode(true, true, 0.5f, true, 0f, 0f));
				}
			}
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("result_won_enter_result");
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, ConfirmEventButtonClicked);
	}

	private void OnDestroy()
	{
		DeRegisterEventHandlers();
		if ((bool)DIContainerInfrastructure.GetCoreStateMgr())
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(2u);
		}
		m_WheelButton.Clicked -= WheelButtonClicked;
	}

	private bool SetupBossBanner()
	{
		m_BossInfoObject.SetActive(true);
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		List<Leaderboard.Score> list = new List<Leaderboard.Score>(currentPlayer.CurrentEventManagerGameData.GetRankedPlayers(true));
		list.AddRange(Enumerable.ToList(m_Model.ScoresByPlayerEnemyTeam.Values));
		bool result = true;
		if (currentPlayer.Data.WorldBoss == null || currentPlayer.Data.WorldBoss.OwnTeamId == 0 || currentPlayer.Data.WorldBoss.Team1 == null || currentPlayer.Data.WorldBoss.Team2 == null)
		{
			m_BossInfoObject.SetActive(false);
			return result;
		}
		float num = 0f;
		float num2 = 0f;
		foreach (Leaderboard.Score item in list)
		{
			if (item.GetAccountId() == "current")
			{
				if (currentPlayer.Data.WorldBoss.OwnTeamId == 1)
				{
					num += (float)item.GetPoints();
				}
				else if (currentPlayer.Data.WorldBoss.OwnTeamId == 2)
				{
					num2 += (float)item.GetPoints();
				}
			}
			else if (currentPlayer.Data.WorldBoss.Team1.TeamPlayerIds.Contains(item.GetAccountId()))
			{
				num += (float)item.GetPoints();
			}
			else if (currentPlayer.Data.WorldBoss.Team2.TeamPlayerIds.Contains(item.GetAccountId()))
			{
				num2 += (float)item.GetPoints();
			}
		}
		if (currentPlayer.Data.WorldBoss.OwnTeamId == 1)
		{
			m_ownTeam = currentPlayer.Data.WorldBoss.Team1;
			m_enemyTeam = currentPlayer.Data.WorldBoss.Team2;
		}
		else
		{
			m_ownTeam = currentPlayer.Data.WorldBoss.Team2;
			m_enemyTeam = currentPlayer.Data.WorldBoss.Team1;
			float num3 = num;
			num = num2;
			num2 = num3;
		}
		result = num >= num2;
		num = Math.Max(num, 0f);
		num2 = Math.Max(num2, 0f);
		m_OwnTeamLabel.text = m_ownTeam.NameId.Replace("{value_2}", DIContainerInfrastructure.GetLocaService().Tr("worldboss_playerteam_name"));
		m_EnemyTeamLabel.text = m_enemyTeam.NameId.Replace("{value_2}", DIContainerInfrastructure.GetLocaService().Tr("worldboss_enemyteam_name"));
		m_OwnTeamLabel.color = m_TeamColors[m_ownTeam.TeamColor];
		m_EnemyTeamLabel.color = m_TeamColors[m_enemyTeam.TeamColor];
		m_OwnTeamHighlight.SetActive(result);
		m_EnemyTeamHighlight.SetActive(!result);
		m_OwnTeamScore.text = num.ToString();
		m_EnemyTeamScore.text = num2.ToString();
		m_OwnTeamScore.color = m_TeamColors[m_ownTeam.TeamColor];
		m_EnemyTeamScore.color = m_TeamColors[m_enemyTeam.TeamColor];
		if (result)
		{
			m_BossDescription.text = DIContainerInfrastructure.GetLocaService().Tr("worldboss_result_desc_01");
			m_TeamWinAnimation.Play("YourTeamWins");
		}
		else
		{
			m_BossDescription.text = DIContainerInfrastructure.GetLocaService().Tr("worldboss_result_desc_02");
			m_TeamWinAnimation.Play("EnemyTeamWins");
		}
		return result;
	}
}

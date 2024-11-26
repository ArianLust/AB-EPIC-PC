using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using Rcs;
using UnityEngine;

public class EventDetailUI : MonoBehaviour
{
	[SerializeField]
	private List<UILabel> playernamesLabel;

	[SerializeField]
	private UILabel m_EventName;

	[SerializeField]
	private UILabel m_TimeLeft;

	[SerializeField]
	private GameObject m_PigObjectivesRoot;

	[SerializeField]
	private GameObject m_CollectibleRoot;

	[SerializeField]
	private GameObject m_CampaignRoot;

	[SerializeField]
	private ResourceCostBlind m_EnergyDisplay;

	[SerializeField]
	private UILabel m_EnergyTimer;

	[SerializeField]
	private UIGrid m_ObjectiveTable;

	[SerializeField]
	private EventObjectivesSlot m_ObjectivesSlotPrefab;

	private bool m_useCollectionDetails;

	[SerializeField]
	private LootWheelController m_LootWheelPreview;

	[SerializeField]
	private UIInputTrigger m_LeaderBoardsButton;

	[SerializeField]
	private UIInputTrigger m_LeaveButton;

	[SerializeField]
	private EventCollectibleSlot m_CollectibleItem;

	[SerializeField]
	private Transform m_AdditionRoot;

	[SerializeField]
	private ResourceCostBlind m_BonusReward;

	[SerializeField]
	private UILabel m_NeededRankText;

	[SerializeField]
	private List<Transform> m_RankingRay = new List<Transform>();

	private BaseLocationStateManager m_StateMgr;

	private EventManagerGameData m_Model;

	private UIPanel m_CharacterPanel;

	[SerializeField]
	private OpponentInfoElement m_OpponentInfoPrefab;

	[SerializeField]
	private OpponentInfoElement m_SelfInfoPrefab;

	[SerializeField]
	private List<CollectionItemSlot> m_CollectionResources;

	[SerializeField]
	private CollectionItemSlot m_CollectionReward;

	[SerializeField]
	private UILabel m_CollectionRewardLabel;

	[SerializeField]
	private UIInputTrigger m_GotoCampaignButton;

	[SerializeField]
	private GameObject m_WaitingForServerObject;

	[SerializeField]
	public UILabel m_WaitingForServerLabel;

	[SerializeField]
	private Transform m_CampaignEntranceRoot;

	[SerializeField]
	private GameObject m_WaitingLoadingSpinner;

	[SerializeField]
	private GameObject m_WaitingScoreIcon;

	[SerializeField]
	[Header("Boss UI")]
	private GameObject m_RankingPanel;

	[SerializeField]
	private GameObject m_TeamRewardObject;

	[SerializeField]
	private GameObject m_TeamCompetitionPanel;

	[SerializeField]
	private UILabel m_Team1Name;

	[SerializeField]
	private UILabel m_Team2Name;

	[SerializeField]
	private UILabel m_Team1Points;

	[SerializeField]
	private UILabel m_Team2Points;

	[SerializeField]
	private GameObject m_Team1LeadingSliderObject;

	[SerializeField]
	private GameObject m_Team2LeadingSliderObject;

	[SerializeField]
	private List<OpponentInfoElement> m_LeaderboardPreviews;

	[SerializeField]
	private GameObject m_BossObjectivesRoot;

	[SerializeField]
	private Transform m_BossPrefabContainer;

	[SerializeField]
	private UIInputTrigger m_BossInfoButton;

	[SerializeField]
	private UIInputTrigger m_CloseBossInfoScreenButton;

	[SerializeField]
	private Animation m_BossInfoScreenAnimation;

	[SerializeField]
	private List<Color> m_TeamColors;

	[SerializeField]
	private CharacterControllerWorldMap m_CharacterControllerPrefab;

	[SerializeField]
	private OpponentInfoElement m_LeaderBoardBlindPrefab;

	[SerializeField]
	private UIGrid m_BossPreviewLeaderboardGrid;

	[SerializeField]
	private UIInputTrigger m_StartBossButton;

	[SerializeField]
	private Animation m_HealthBarAnim;

	[SerializeField]
	private Animation m_CooldownAnim;

	[SerializeField]
	private UILabel m_CooldownText;

	[SerializeField]
	private UILabel m_BossRewardCollectionLabel;

	[SerializeField]
	private UISprite m_BossRewardCollectionFiller;

	[SerializeField]
	private Animator m_BossRewardCollectionAnimator;

	[SerializeField]
	private CollectionItemSlot m_BossRewardSlot;

	[SerializeField]
	private GameObject m_BossButtonActiveState;

	[SerializeField]
	private GameObject m_BossButtonInactiveState;

	[SerializeField]
	private UILabel m_BossButtonFightLabel;

	[SerializeField]
	private UILabel m_BossPopupDescription;

	[SerializeField]
	private UIInputTrigger m_BossEliteChestButton;

	[SerializeField]
	[Header("Elite Chest Popup")]
	private GameObject m_ChestInfoRoot;

	[SerializeField]
	private EliteChestInfoPopup m_ChestInfoPopup;

	[SerializeField]
	private UIInputTrigger m_ChestInfoCloseButton;

	private List<EventItemGameData> m_PossibleEventItems = new List<EventItemGameData>();

	private bool m_EventHasChanged;

	private bool m_uptoDate;

	private List<Leaderboard.Score> m_sortedScores;

	private WorldBossTeamData m_ownTeam;

	private WorldBossTeamData m_enemyTeam;

	private List<string> m_cheaters;

	private bool m_IsUpdatingLeaderboard;

	private EliteChestInfoPopup m_chestContentPreviewPopup;

	private void Awake()
	{
		m_CharacterPanel = m_ObjectiveTable.GetComponentInParent<UIPanel>();
		base.transform.position += DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.transform.position;
		UIPanel[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIPanel>();
		UIPanel[] array = componentsInChildren;
		foreach (UIPanel uIPanel in array)
		{
			uIPanel.enabled = false;
		}
	}

	public void SetStateMgr(BaseLocationStateManager locationStateMgr)
	{
		m_StateMgr = locationStateMgr;
	}

	public void SetModel(EventManagerGameData eventManagerGameData)
	{
		if (DIContainerLogic.EventSystemService.IsEventRunning(eventManagerGameData.Balancing) && eventManagerGameData.Data.CurrentState != EventManagerState.Running)
		{
			eventManagerGameData.Data.CurrentState = EventManagerState.Running;
		}
		base.gameObject.SetActive(true);
		m_EventHasChanged = m_Model != eventManagerGameData;
		m_PossibleEventItems.Clear();
		m_Model = eventManagerGameData;
		m_sortedScores = m_Model.GetRankedPlayers(true);
		m_cheaters = eventManagerGameData.Data.CheatingOpponents ?? new List<string>();
		m_EventName.text = DIContainerInfrastructure.GetLocaService().Tr(eventManagerGameData.EventBalancing.LocaBaseId + "_name");
		if (m_Model.IsBossEvent)
		{
			m_PigObjectivesRoot.SetActive(false);
			m_BossObjectivesRoot.SetActive(true);
			m_TeamRewardObject.SetActive(true);
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			SpawnBossPrefab();
			SetupBossReward();
			SetupPopupDescLabel();
			SetupBossButton();
		}
		else
		{
			m_TeamRewardObject.SetActive(false);
			m_PigObjectivesRoot.SetActive(true);
			m_BossObjectivesRoot.SetActive(false);
		}
		if (eventManagerGameData.IsCampaignEvent)
		{
			Dictionary<string, LootInfoData> loot = DIContainerLogic.GetLootOperationService().GenerateLootPreview(eventManagerGameData.EventBalancing.EventMiniCampaignItemLootTable, DIContainerInfrastructure.GetCurrentPlayer().Data.Level);
			List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(loot);
			foreach (IInventoryItemGameData item in itemsFromLoot)
			{
				if (item is EventItemGameData)
				{
					m_PossibleEventItems.Add(item as EventItemGameData);
				}
			}
			m_useCollectionDetails = true;
		}
		else
		{
			Dictionary<string, LootInfoData> loot2 = DIContainerLogic.GetLootOperationService().GenerateLootPreview(eventManagerGameData.EventBalancing.EventGeneratorItemLootTable, DIContainerInfrastructure.GetCurrentPlayer().Data.Level);
			List<IInventoryItemGameData> itemsFromLoot2 = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(loot2);
			foreach (IInventoryItemGameData item2 in itemsFromLoot2)
			{
				if (item2 is EventItemGameData)
				{
					m_PossibleEventItems.Add(item2 as EventItemGameData);
				}
			}
			Dictionary<string, LootInfoData> loot3 = DIContainerLogic.GetLootOperationService().GenerateLootPreview(eventManagerGameData.EventBalancing.EventCollectibleGeneratorItemLootTable, DIContainerInfrastructure.GetCurrentPlayer().Data.Level);
			List<IInventoryItemGameData> itemsFromLoot3 = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(loot3);
			foreach (IInventoryItemGameData item3 in itemsFromLoot3)
			{
				if (item3 is EventItemGameData)
				{
					m_PossibleEventItems.Add(item3 as EventItemGameData);
				}
			}
		}
		m_TeamCompetitionPanel.SetActive(false);
		m_RankingPanel.SetActive(false);
		m_LeaderBoardsButton.gameObject.SetActive(false);
		m_WaitingForServerObject.SetActive(true);
		RequestLeaderboard();
	}

	private void SetLeaderboardInfo()
	{
		m_WaitingLoadingSpinner.SetActive(false);
		if (m_Model.IsBossEvent)
		{
			if (DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss.OwnTeamId == 0)
			{
				DebugLog.Log("Own team is not yet set");
				this.m_TeamCompetitionPanel.SetActive(false);
				return;
			}
			StartCoroutine(this.SetupBossCooldownOrHealthBar());
			m_TeamCompetitionPanel.SetActive(true);
			m_RankingPanel.SetActive(false);
			SetupBossCompetitionSlider();
			StartCoroutine("ResetLeaderboardWithDelay");
			if (!DIContainerLogic.EventSystemService.IsBossOnCooldown())
			{
				DIContainerLogic.EventSystemService.GetEventBossDefeatLog(this.m_Model, delegate(RESTResultEnum result)
				{
					if (result == RESTResultEnum.Success)
					{
						DIContainerInfrastructure.LocationStateMgr.EventsWorldMapStateMgr.UpdateWorldBossHP();
					}
				});
			}
		}
		else
		{
			m_RankingPanel.SetActive(true);
			m_TeamCompetitionPanel.SetActive(false);
			SetupRankingRay();
		}
		m_WaitingForServerObject.SetActive(false);
		m_LeaderBoardsButton.gameObject.SetActive(true);
	}

	private void RequestLeaderboard()
	{
		if (m_IsUpdatingLeaderboard)
		{
			return;
		}
		if (!m_Model.IsQualifiedForLeaderboard)
		{
			m_WaitingLoadingSpinner.SetActive(false);
			m_WaitingScoreIcon.SetActive(true);
			m_WaitingForServerLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_leaderboard_nopoints");
			return;
		}
		m_WaitingScoreIcon.SetActive(false);
		m_WaitingLoadingSpinner.SetActive(true);
		if (m_Model.Data.CurrentOpponents != null && m_Model.Data.CurrentOpponents.Count > 1)
		{
			m_WaitingForServerLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_leaderboard_fetching");
		}
		else if (!m_IsUpdatingLeaderboard)
		{
			m_WaitingForServerLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_leaderboard_looking");
		}
		else
		{
			m_WaitingForServerLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_leaderboard_alone");
		}
		m_IsUpdatingLeaderboard = true;
		if (m_Model.Data.CurrentOpponents != null && m_Model.Data.CurrentOpponents.Count < 1)
		{
			DIContainerLogic.EventSystemService.SubmitEventScore(DIContainerInfrastructure.GetCurrentPlayer(), m_Model, OnLeaderboardFound);
		}
		else
		{
			DIContainerLogic.EventSystemService.GetLeaderboardScores(DIContainerInfrastructure.GetCurrentPlayer(), m_Model.Data.CurrentOpponents, m_Model, OnLeaderboardUpdated, OnLeaderboardUpdateFailed);
		}
	}

	private void OnLeaderboardUpdateFailed(int errorcode)
	{
		DebugLog.Error(GetType(), "OnLeaderboardUpdateFailed: Errorcode = " + (RESTResultEnum)errorcode);
		if (m_WaitingForServerLabel != null)
		{
			m_WaitingForServerLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_leaderboard_error");
		}
	}

	private void OnLeaderboardUpdated()
	{
		SetLeaderboardInfo();
	}

	private void OnLeaderboardFound(RESTResultEnum result)
	{
		RESTResultEnum rESTResultEnum = result;
		if (rESTResultEnum == RESTResultEnum.Success)
		{
			SetLeaderboardInfo();
		}
		else if (m_WaitingForServerLabel != null)
		{
			m_WaitingForServerLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_leaderboard_error");
		}
	}

	private void SetupPopupDescLabel()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		CollectionGroupBalancingData collectionGroupBalancingForEvent = DIContainerLogic.EventSystemService.GetCollectionGroupBalancingForEvent(m_Model);
		if (collectionGroupBalancingForEvent != null)
		{
			DebugLog.Log(GetType(), "SetupPopupDescLabel: collection found! name = " + collectionGroupBalancingForEvent.NameId);
			dictionary.Add("{value_1}", collectionGroupBalancingForEvent.ComponentRequirements.Where((Requirement r) => r.RequirementType == RequirementType.HaveEventScore).FirstOrDefault().Value.ToString());
			Dictionary<string, int> loot = ((!DIContainerInfrastructure.EventSystemStateManager.UseCollectionFallbackReward() && DIContainerLogic.EventSystemService.GetAvailableEliteChestReward(DIContainerInfrastructure.GetCurrentPlayer()).Count != 0) ? collectionGroupBalancingForEvent.Reward : collectionGroupBalancingForEvent.FallbackReward);
			Dictionary<string, LootInfoData> loot2 = DIContainerLogic.GetLootOperationService().GenerateLoot(loot, 1);
			List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(loot2);
			dictionary.Add("{value_2}", itemsFromLoot[0].ItemLocalizedName);
		}
		else
		{
			DebugLog.Log(GetType(), "SetupPopupDescLabel: no collection found for boss popup!");
			dictionary.Add("{value_1}", "10000");
			dictionary.Add("{value_2}", "Dominic");
		}
		DebugLog.Log(GetType(), "SetupPopupDescLabel: Setting label to " + DIContainerInfrastructure.GetLocaService().Tr("eventboss_info_desc_01", dictionary));
		m_BossPopupDescription.text = DIContainerInfrastructure.GetLocaService().Tr("eventboss_info_desc_01", dictionary);
	}

	private void SetupBossButton()
	{
		if (DIContainerLogic.EventSystemService.IsBossOnCooldown())
		{
			m_BossButtonFightLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_detailscreen_cooldown");
			m_StartBossButton.GetComponent<BoxCollider>().enabled = false;
			m_BossButtonInactiveState.SetActive(true);
			m_BossButtonActiveState.SetActive(false);
		}
		else
		{
			m_BossButtonFightLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_detailscreen_fight");
			m_StartBossButton.GetComponent<BoxCollider>().enabled = true;
			m_BossButtonInactiveState.SetActive(false);
			m_BossButtonActiveState.SetActive(true);
		}
	}

	private IEnumerator SetupBossCooldownOrHealthBar()
	{
		if (DIContainerLogic.EventSystemService.IsBossOnCooldown())
		{
			if ((bool)m_HealthBarAnim)
			{
				m_HealthBarAnim.gameObject.SetActive(false);
			}
			m_CooldownAnim.gameObject.SetActive(true);
			m_CooldownAnim.gameObject.PlayAnimationOrAnimatorState("Show");
			yield return new WaitForSeconds(0.25f);
			StartCoroutine(BossCooldownTimer());
			yield break;
		}
		if ((bool)m_HealthBarAnim)
		{
			m_HealthBarAnim.gameObject.SetActive(true);
		}
		m_CooldownAnim.gameObject.SetActive(false);
		m_HealthBarAnim.Play("HealthBar_Show");
		m_CooldownAnim.Play("Hide");
		yield return new WaitForSeconds(0.25f);
		m_HealthBarAnim.GetComponent<CharacterChunkHealthBar>().SetModel(DIContainerInfrastructure.LocationStateMgr.EventsWorldMapStateMgr.m_WorldMapBossCombatant);
		m_HealthBarAnim.GetComponent<CharacterChunkHealthBar>().SetHealthInstant();
	}

	private IEnumerator BossCooldownTimer()
	{
		uint targetTime = DIContainerInfrastructure.GetCurrentPlayer().Data.BossStartTime + (uint)m_Model.CurrentEventBoss.BalancingData.TimeToReactivate;
		while (DIContainerLogic.GetTimingService().GetCurrentTimestamp() < targetTime)
		{
			string locaIdent = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.CurrentEventBoss.BalancingData.DefeatedLabelLocaId;
			m_CooldownText.text = DIContainerInfrastructure.GetLocaService().Tr(locaIdent) + DIContainerLogic.EventSystemService.GetFormattedBossCooldown();
			yield return new WaitForSeconds(1f);
		}
		StartCoroutine(SetupBossCooldownOrHealthBar());
	}

	private void SetupBossCompetitionSlider()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		List<Leaderboard.Score> list = new List<Leaderboard.Score>(m_sortedScores);
		list.AddRange(Enumerable.ToList(m_Model.ScoresByPlayerEnemyTeam.Values));
		DebugLog.Log(GetType(), "[EPIC SERVER] ------------ All player scores = " + list.Count);
		int num = 0;
		int num2 = 0;
		foreach (Leaderboard.Score item in list)
		{
			if (item.GetPoints() <= 0)
			{
				continue;
			}
			DebugLog.Log(GetType(), "[EPIC SERVER] ------------ score for " + item.GetAccountId() + " = " + item.GetPoints() + " ---- Own Team = " + currentPlayer.Data.WorldBoss.OwnTeamId);
			if (currentPlayer.Data.WorldBoss.Team1.TeamPlayerIds.Contains(item.GetAccountId()))
			{
				num += (int)item.GetPoints();
			}
			else if (currentPlayer.Data.WorldBoss.Team2.TeamPlayerIds.Contains(item.GetAccountId()))
			{
				num2 += (int)item.GetPoints();
			}
			else if (item.GetAccountId() == "current")
			{
				if (currentPlayer.Data.WorldBoss.OwnTeamId == 1)
				{
					num += (int)item.GetPoints();
				}
				else if (currentPlayer.Data.WorldBoss.OwnTeamId == 2)
				{
					num2 += (int)item.GetPoints();
				}
			}
			DebugLog.Log(GetType(), "[EPIC SERVER] ------------ score for OWN team (team1score)= " + num);
			DebugLog.Log(GetType(), "[EPIC SERVER] ------------ score for OPPONENT team (team2score)= " + num2);
		}
		bool flag = true;
		m_ownTeam = null;
		m_enemyTeam = null;
		if (currentPlayer.Data.WorldBoss.OwnTeamId == 1)
		{
			m_ownTeam = currentPlayer.Data.WorldBoss.Team1;
			m_enemyTeam = currentPlayer.Data.WorldBoss.Team2;
		}
		else
		{
			m_ownTeam = currentPlayer.Data.WorldBoss.Team2;
			m_enemyTeam = currentPlayer.Data.WorldBoss.Team1;
			int num3 = num;
			num = num2;
			num2 = num3;
		}
		num = Math.Max(num, 0);
		num2 = Math.Max(num2, 0);
		flag = num >= num2;
		m_Team1LeadingSliderObject.SetActive(flag);
		m_Team2LeadingSliderObject.SetActive(!flag);
		m_Team1Points.text = num.ToString();
		m_Team2Points.text = num2.ToString();
		m_Team1Name.text = m_ownTeam.NameId.Replace("{value_2}", DIContainerInfrastructure.GetLocaService().Tr("worldboss_playerteam_name"));
		m_Team2Name.text = m_enemyTeam.NameId.Replace("{value_2}", DIContainerInfrastructure.GetLocaService().Tr("worldboss_enemyteam_name"));
		if (flag)
		{
			if (num == 0)
			{
				m_Team1LeadingSliderObject.GetComponent<UISlider>().sliderValue = 0.5f;
			}
			else
			{
				m_Team1LeadingSliderObject.GetComponent<UISlider>().sliderValue = 0.5f * ((float)num2 / (float)num);
			}
		}
		else
		{
			m_Team2LeadingSliderObject.GetComponent<UISlider>().sliderValue = 0.5f * ((float)num / (float)num2);
		}
		int teamColor = m_ownTeam.TeamColor;
		int teamColor2 = m_enemyTeam.TeamColor;
		if (teamColor >= m_TeamColors.Count || teamColor2 >= m_TeamColors.Count)
		{
			DebugLog.Error(GetType(), "SetupBossCompetitionSlider: team colors are out of bounds! Need to stay between 0 - 4. Found " + teamColor + " and " + teamColor2);
			return;
		}
		List<Transform> list2 = Enumerable.ToList(m_TeamCompetitionPanel.GetComponentsInChildren<Transform>(true));
		foreach (Transform item2 in list2)
		{
			if (item2.name.Contains("(TeamA)"))
			{
				if (item2.GetComponent<UILabel>() != null)
				{
					item2.GetComponent<UILabel>().color = m_TeamColors[teamColor];
				}
				if (item2.GetComponent<UISprite>() != null)
				{
					item2.GetComponent<UISprite>().color = m_TeamColors[teamColor];
				}
			}
			else if (item2.name.Contains("(TeamB)"))
			{
				if (item2.GetComponent<UILabel>() != null)
				{
					item2.GetComponent<UILabel>().color = m_TeamColors[teamColor2];
				}
				if (item2.GetComponent<UISprite>() != null)
				{
					item2.GetComponent<UISprite>().color = m_TeamColors[teamColor2];
				}
			}
		}
	}

	private void SetupBossReward()
	{
		if (this.m_Model == null)
		{
			DebugLog.Error(base.GetType(), "SetupBossReward: No model found. Cannot proceed.");
			return;
		}
		CollectionGroupBalancingData collectionBalancing = this.m_Model.GetCollectionBalancing();
		float num = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Data.CurrentScore;
		if (collectionBalancing != null)
		{
			float value = collectionBalancing.ComponentRequirements.Where((Requirement r) => r.RequirementType == RequirementType.HaveEventScore).FirstOrDefault<Requirement>().Value;
			this.m_BossRewardCollectionFiller.fillAmount = Mathf.Min(1f, num / value);
			this.m_BossRewardCollectionLabel.text = Math.Min(num, value) + "/" + value;
			if (this.m_BossRewardSlot != null && DIContainerLogic.EventSystemService.GetCurrentCollectionRewardStatus() >= EventCampaignRewardStatus.chest_claimed)
			{
				string confirmedChestLootId = this.m_Model.Data.ConfirmedChestLootId;
				IInventoryItemGameData inventoryItemGameData = DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(1, 1, confirmedChestLootId, 1);
				this.m_BossEliteChestButton.gameObject.SetActive(false);
				this.m_BossRewardSlot.gameObject.SetActive(true);
				this.m_BossRewardSlot.SetModel(inventoryItemGameData, null);
			}
			if (!DIContainerLogic.EventSystemService.IsChestCollectionReward(this.m_Model))
			{
				Dictionary<string, int> dictionary = ((!DIContainerInfrastructure.EventSystemStateManager.UseCollectionFallbackReward(null)) ? collectionBalancing.Reward : collectionBalancing.FallbackReward);
				Dictionary<string, LootInfoData> dictionary2 = DIContainerLogic.GetLootOperationService().GenerateLoot(dictionary, 1);
				List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(dictionary2, EquipmentSource.LootBird, false);
				this.m_BossRewardSlot.SetModel(itemsFromLoot[0], null);
			}
			if (num >= value)
			{
				this.m_BossRewardCollectionAnimator.Play("Material_Idle_Active");
			}
			else
			{
				this.m_BossRewardCollectionAnimator.Play("Material_Idle_Inactive");
			}
		}
		else
		{
			this.m_BossRewardCollectionFiller.fillAmount = 0f;
			this.m_BossRewardCollectionLabel.text = num.ToString();
		}
	}

	private void SetupLeaderBoardPreview()
	{
		DebugLog.Log(GetType(), "SetupLeaderBoardPreview: checking m_sortedScores..");
		bool flag = false;
		for (int i = 0; i < m_sortedScores.Count; i++)
		{
			if (m_BossPreviewLeaderboardGrid.transform.childCount > 3 && flag)
			{
				break;
			}
			Leaderboard.Score score = m_sortedScores[i];
			flag = true;
			OpponentInfoElement opponentInfoElement = UnityEngine.Object.Instantiate(m_LeaderBoardBlindPrefab);
			opponentInfoElement.transform.parent = m_BossPreviewLeaderboardGrid.transform;
			opponentInfoElement.transform.localPosition = Vector3.zero;
			opponentInfoElement.SetDefault((int)score.GetPoints(), i + 1, 0, false, false, true);
			opponentInfoElement.SetModel(new OpponentGameData(DIContainerInfrastructure.GetCurrentPlayer().PublicPlayer, true), true);
			opponentInfoElement.SetCheater(false);
		}
	}

	private void SpawnBossPrefab()
	{
		if (m_Model.CurrentEventBoss == null || m_BossPrefabContainer.transform.childCount > 0)
		{
			DebugLog.Error(GetType(), "SpawnBossPrefab: EventBoss not yet set up!");
			return;
		}
		CharacterControllerWorldMap characterControllerWorldMap = UnityEngine.Object.Instantiate(m_CharacterControllerPrefab);
		characterControllerWorldMap.SetModel(m_Model.CurrentEventBoss.BalancingData.NameId, false);
		characterControllerWorldMap.transform.parent = m_BossPrefabContainer;
		characterControllerWorldMap.transform.localPosition = Vector3.zero;
		characterControllerWorldMap.transform.localScale = Vector3.one;
		if (DIContainerLogic.EventSystemService.IsBossOnCooldown())
		{
			characterControllerWorldMap.m_AssetController.PlayAnimation("SetDefeated");
		}
		UnityHelper.SetLayerRecusively(characterControllerWorldMap.gameObject, LayerMask.NameToLayer("Interface"));
	}

	public void ComeBackFromLeaderBoard()
	{
		if (!m_Model.IsBossEvent)
		{
			return;
		}
		foreach (Transform item in m_BossPreviewLeaderboardGrid.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		StartCoroutine("ResetLeaderboardWithDelay");
	}

	private IEnumerator ResetLeaderboardWithDelay()
	{
		yield return new WaitForEndOfFrame();
		SetupLeaderBoardPreview();
		yield return new WaitForEndOfFrame();
		m_BossPreviewLeaderboardGrid.Reposition();
	}

	private void OpenInfoScreen()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(4, CloseInfoScreen);
		m_BossInfoScreenAnimation.gameObject.SetActive(true);
		m_BossInfoScreenAnimation.Play("Popup_Enter");
	}

	private void CloseInfoScreen()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(4);
		m_BossInfoScreenAnimation.Play("Popup_Leave");
		Invoke("DeactiveInfoScreen", m_BossInfoScreenAnimation["Popup_Leave"].length);
	}

	private void DeactiveInfoScreen()
	{
		m_BossInfoScreenAnimation.gameObject.SetActive(false);
	}

	private IEnumerator CountDownTimer()
	{
		DateTime trustedTime;
		while (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		DateTime targetTime = DIContainerLogic.EventSystemService.GetEventEndTime(m_Model.Balancing);
		while (targetTime > trustedTime)
		{
			if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				TimeSpan timeLeft = targetTime - trustedTime;
				m_TimeLeft.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(DIContainerLogic.GetTimingService().TimeLeftUntil(targetTime));
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private void SetEnergyTimer()
	{
		m_EnergyDisplay.SetModel(string.Empty, null, DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "event_energy"), string.Empty, true);
		if (!DIContainerLogic.EventSystemService.HasMaximumEnergy(DIContainerInfrastructure.GetCurrentPlayer()))
		{
			m_EnergyTimer.gameObject.SetActive(true);
			StopCoroutine("CountDownEnergyTimer");
			StartCoroutine("CountDownEnergyTimer");
		}
		else
		{
			m_EnergyTimer.gameObject.SetActive(false);
		}
	}

	private IEnumerator CountDownEnergyTimer()
	{
		DateTime trustedTime;
		while (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		DateTime targetTime = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(DIContainerInfrastructure.GetCurrentPlayer().Data.LastEnergyAddTime).AddSeconds(DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.BalancingData.EnergyRefreshTimeInSeconds);
		while (targetTime > trustedTime)
		{
			if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				TimeSpan timeLeft = targetTime - trustedTime;
				m_EnergyTimer.text = DIContainerInfrastructure.GetLocaService().Tr("eventdetail_next_energy", new Dictionary<string, string> { 
				{
					"{value_1}",
					DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(timeLeft)
				} });
			}
			yield return new WaitForSeconds(1f);
		}
		EnergyUpdateMgr energyUpdateManager = DIContainerInfrastructure.GetCoreStateMgr().GetComponent<EnergyUpdateMgr>();
		if ((bool)energyUpdateManager)
		{
			energyUpdateManager.Run();
		}
		SetEnergyTimer();
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, HandleBackButton);
		foreach (Transform item in m_RankingRay)
		{
			foreach (Transform item2 in item)
			{
				if (item2 != null)
				{
					item2.GetComponent<OpponentInfoElement>().m_ElementPressedTrigger.Clicked += LeaderBoardsButtonClicked;
				}
			}
		}
		DIContainerInfrastructure.GetCurrentPlayer().GlobalEventStateChanged += GlobalEventStateChanged;
		m_LeaderBoardsButton.Clicked += LeaderBoardsButtonClicked;
		m_LeaveButton.Clicked += LeaveButtonClicked;
		m_BossInfoButton.Clicked += OpenInfoScreen;
		m_CloseBossInfoScreenButton.Clicked += CloseInfoScreen;
		m_BossEliteChestButton.Clicked += OpenChestContentInfo;
		m_ChestInfoCloseButton.Clicked += LeaveChestContentInfo;
		if (m_StartBossButton != null)
		{
			m_StartBossButton.Clicked += StartBossBattle;
		}
		if (m_GotoCampaignButton != null)
		{
			m_GotoCampaignButton.Clicked += GotoCampaignClicked;
		}
	}

	private void OpenChestContentInfo()
	{
		m_ChestInfoRoot.SetActive(true);
		m_ChestInfoPopup.InitializeItems(DIContainerLogic.EventSystemService.GetAvailableEliteChestReward(DIContainerInfrastructure.GetCurrentPlayer()));
		m_ChestInfoRoot.PlayAnimationOrAnimatorState("Popup_Enter");
	}

	private void LeaveChestContentInfo()
	{
		StartCoroutine(LeaveChestContentInfoCoroutine());
	}

	private IEnumerator LeaveChestContentInfoCoroutine()
	{
		float closeDur = m_ChestInfoRoot.PlayAnimationOrAnimatorState("Popup_Leave");
		yield return new WaitForSeconds(closeDur);
		m_ChestInfoRoot.SetActive(false);
	}

	private void GlobalEventStateChanged(CurrentGlobalEventState arg1, CurrentGlobalEventState arg2)
	{
		SetupTimer();
	}

	private void HandleBackButton()
	{
		LeaveButtonClicked();
	}

	private void DeRegisterEventHandler()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(2);
		foreach (Transform item in m_RankingRay)
		{
			foreach (Transform item2 in item)
			{
				if (item2 != null)
				{
					item2.GetComponent<OpponentInfoElement>().m_ElementPressedTrigger.Clicked -= LeaderBoardsButtonClicked;
				}
			}
		}
		DIContainerInfrastructure.GetCurrentPlayer().GlobalEventStateChanged -= GlobalEventStateChanged;
		m_LeaderBoardsButton.Clicked -= LeaderBoardsButtonClicked;
		m_LeaveButton.Clicked -= LeaveButtonClicked;
		m_BossInfoButton.Clicked -= OpenInfoScreen;
		m_CloseBossInfoScreenButton.Clicked -= CloseInfoScreen;
		m_BossEliteChestButton.Clicked -= OpenChestContentInfo;
		m_ChestInfoCloseButton.Clicked -= LeaveChestContentInfo;
		if (m_StartBossButton != null)
		{
			m_StartBossButton.Clicked -= StartBossBattle;
		}
		if (m_GotoCampaignButton != null)
		{
			m_GotoCampaignButton.Clicked -= GotoCampaignClicked;
		}
	}

	private void StartBossBattle()
	{
		EventPlacementBalancingData balancing;
		DIContainerBalancing.EventBalancingService.TryGetBalancingData<EventPlacementBalancingData>("bosslocation_01", out balancing);
		EventItemGameData eventItemGameData = null;
		List<IInventoryItemGameData> list = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.EventBossItem];
		eventItemGameData = ((list.Count <= 0) ? DIContainerLogic.EventSystemService.GenerateEventBoss(m_Model.EventBalancing, DIContainerInfrastructure.GetCurrentPlayer()) : (list[0] as EventItemGameData));
		WorldMapStateMgr worldMapStateMgr = DIContainerInfrastructure.LocationStateMgr as WorldMapStateMgr;
		if (worldMapStateMgr != null)
		{
			worldMapStateMgr.ShowBattlePreperationScreenForEvent(eventItemGameData, balancing);
		}
		StartCoroutine(LeaveCoroutine(false));
	}

	private void LeaveButtonClicked()
	{
		Leave();
	}

	private void LeaderBoardsButtonClicked()
	{
		m_StateMgr.ShowLeaderBoardScreen(m_ownTeam, m_enemyTeam, this);
	}

	public void Leave()
	{
		StartCoroutine(LeaveCoroutine());
	}

	private IEnumerator LeaveCoroutine(bool leaveBackground = true)
	{
		DeRegisterEventHandler();
		if (m_Model.IsBossEvent && m_BossPrefabContainer.childCount > 0)
		{
			UnityEngine.Object.Destroy(m_BossPrefabContainer.GetChild(0).gameObject);
		}
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("event_details_animate");
		if (leaveBackground)
		{
			m_StateMgr.WorldMenuUI.Enter();
			DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave();
		}
		else
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Enter();
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.EnterLevelDisplay();
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Window_EventDetails_Leave"));
		foreach (Transform child in m_BossPreviewLeaderboardGrid.transform)
		{
			UnityEngine.Object.Destroy(child.gameObject);
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("event_details_animate");
		m_IsUpdatingLeaderboard = false;
		base.gameObject.SetActive(false);
	}

	public void Enter()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Enter();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveLevelDisplay();
		base.gameObject.SetActive(true);
		UIPanel[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIPanel>();
		UIPanel[] array = componentsInChildren;
		foreach (UIPanel uIPanel in array)
		{
			uIPanel.enabled = true;
		}
		m_StateMgr.WorldMenuUI.Leave();
		StartCoroutine(EnterCoroutine());
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("event_details_animate");
		SetEnergyTimer();
		if (m_useCollectionDetails)
		{
			SetupCollectionDetails();
		}
		if (m_EventHasChanged && !m_useCollectionDetails)
		{
			SetupInvasionDetails();
		}
		SetupRankingRay();
		SetupLootWheel();
		SetupRankingBonus();
		SetupTimer();
		RegisterEventHandler();
		if (m_Model.IsBossEvent)
		{
			m_BossPreviewLeaderboardGrid.Reposition();
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveNonInteractableTooltip();
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Window_EventDetails_Enter"));
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("event_details_animate");
	}

	private void SetupCollectionDetails()
	{
		m_CollectibleRoot.SetActive(false);
		m_PigObjectivesRoot.SetActive(false);
		GameObject gameObject = DIContainerInfrastructure.EventSystemStateManager.InstantiateEventObject("Entrance", m_CampaignEntranceRoot);
		gameObject.transform.localScale = Vector3.one;
		gameObject.gameObject.layer = LayerMask.NameToLayer("Interface");
		m_CampaignRoot.SetActive(true);
		m_GotoCampaignButton.gameObject.SetActive(!DIContainerInfrastructure.GetCoreStateMgr().m_EventCampaign);
		if (m_Model.CurrentMiniCampaign == null)
		{
			DebugLog.Error("No EventCampaign found in Event Detail Screen!");
		}
		for (int i = 0; i < m_Model.CurrentMiniCampaign.CollectionGroupBalancing.ComponentRequirements.Count; i++)
		{
			CollectionItemSlot collectionItemSlot = m_CollectionResources[i];
			string nameId = m_Model.CurrentMiniCampaign.CollectionGroupBalancing.ComponentRequirements[i].NameId;
			float value = m_Model.CurrentMiniCampaign.CollectionGroupBalancing.ComponentRequirements[i].Value;
			InventoryGameData inventoryGameData = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData;
			IInventoryItemGameData data = new BasicItemGameData(nameId);
			if (DIContainerLogic.InventoryService.CheckForItem(inventoryGameData, nameId))
			{
				DIContainerLogic.InventoryService.TryGetItemGameData(inventoryGameData, nameId, out data);
			}
			collectionItemSlot.SetModel(data, m_Model.CurrentMiniCampaign.CollectionGroupBalancing.ComponentRequirements[i]);
		}
		Dictionary<string, int> loot = ((!DIContainerInfrastructure.EventSystemStateManager.UseCollectionFallbackReward()) ? m_Model.CurrentMiniCampaign.CollectionGroupBalancing.Reward : m_Model.CurrentMiniCampaign.CollectionGroupBalancing.FallbackReward);
		List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(loot, 1));
		m_CollectionReward.SetModel(itemsFromLoot[0]);
		m_CollectionRewardLabel.text = DIContainerInfrastructure.GetLocaService().Tr("eventwindow_collectitems_01_desc", DIContainerInfrastructure.EventSystemStateManager.GetCollectionRewardReplacementDict());
	}

	private void SetupInvasionDetails()
	{
		bool active = false;
		m_CampaignRoot.SetActive(false);
		foreach (Transform item in m_ObjectiveTable.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		float num = m_CharacterPanel.clipRange.z;
		int num2 = 1;
		foreach (EventItemGameData possibleEventItem in m_PossibleEventItems)
		{
			if (possibleEventItem.BalancingData.ItemType == InventoryItemType.EventBattleItem)
			{
				EventObjectivesSlot eventObjectivesSlot = UnityEngine.Object.Instantiate(m_ObjectivesSlotPrefab);
				eventObjectivesSlot.transform.parent = m_ObjectiveTable.transform;
				eventObjectivesSlot.transform.localPosition = Vector3.zero;
				string firstPossibleBattle = DIContainerLogic.GetBattleService().GetFirstPossibleBattle(possibleEventItem.BalancingData.EventParameters, DIContainerInfrastructure.GetCurrentPlayer());
				BattleBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<BattleBalancingData>(firstPossibleBattle);
				float energyCost = 0f;
				Requirement requirement = balancingData.BattleRequirements.FirstOrDefault((Requirement br) => br.RequirementType == RequirementType.PayItem && br.NameId == "event_energy");
				if (requirement != null)
				{
					energyCost = requirement.Value;
				}
				num = Mathf.Max(0f, num - eventObjectivesSlot.SetModelAndGetExtent(possibleEventItem.BalancingData.AssetBaseId, num2, energyCost));
				num2++;
			}
			else
			{
				if (possibleEventItem.BalancingData.ItemType != InventoryItemType.EventCollectible)
				{
					continue;
				}
				active = true;
				if (possibleEventItem.BalancingData.SortPriority != 0)
				{
					string assetBaseId = possibleEventItem.BalancingData.AssetBaseId;
					GameObject gameObject = DIContainerInfrastructure.PropLiteAssetProvider().InstantiateObject(assetBaseId, (!m_AdditionRoot) ? base.transform : m_AdditionRoot, Vector3.zero, Quaternion.identity);
					if ((bool)gameObject)
					{
						gameObject.transform.localPosition = Vector3.zero;
						UnityHelper.SetLayerRecusively(gameObject.gameObject, base.gameObject.layer);
					}
					m_CollectibleItem.SetModel(possibleEventItem, gameObject);
				}
			}
		}
		m_ObjectiveTable.repositionNow = true;
		m_CollectibleRoot.gameObject.SetActive(active);
	}

	private IEnumerator CenterObjectives(float offset)
	{
		yield return new WaitForEndOfFrame();
		m_ObjectiveTable.transform.localPosition = new Vector3(offset * 0.5f, m_ObjectiveTable.transform.localPosition.y, m_ObjectiveTable.transform.localPosition.z);
	}

	private void SetupLootWheel()
	{
		if (m_Model != null)
		{
			m_LootWheelPreview.SetLootIcons(m_Model.EventBalancing.EventRewardLootTableWheel.Keys.FirstOrDefault(), DIContainerInfrastructure.GetCurrentPlayer().Data.Level, 3);
		}
	}

	private void SetupTimer()
	{
		StopCoroutine("CountDownTimer");
		if (m_Model != null)
		{
			switch (m_Model.CurrentEventManagerState)
			{
			case EventManagerState.Teasing:
				m_TimeLeft.text = DIContainerInfrastructure.GetLocaService().Tr("event_teasing", "Coming Soon!");
				break;
			case EventManagerState.Running:
				StartCoroutine("CountDownTimer");
				break;
			case EventManagerState.Finished:
			case EventManagerState.FinishedWithoutPoints:
				m_TimeLeft.text = DIContainerInfrastructure.GetLocaService().Tr("event_finished", "Finished!");
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	private void SetupRankingBonus()
	{
		if (m_Model != null && m_Model.Data.CurrentScore != 0 && m_Model.EventBalancing.EventBonusLootTablesPerRank.Count > m_Model.GetCurrentRank - 1)
		{
			List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(new Dictionary<string, int> { 
			{
				m_Model.GetScalingRankRewardLootTable(),
				1
			} }, DIContainerInfrastructure.GetCurrentPlayer().Data.Level));
			IInventoryItemGameData inventoryItemGameData = itemsFromLoot.FirstOrDefault();
			if (inventoryItemGameData != null)
			{
				m_BonusReward.gameObject.SetActive(true);
				m_BonusReward.SetModel(inventoryItemGameData.ItemAssetName, null, inventoryItemGameData.ItemValue, string.Empty);
				m_NeededRankText.text = DIContainerInfrastructure.GetLocaService().Tr("eventwindow_bonusreward_rankinfo", new Dictionary<string, string> { 
				{
					"{value_1}",
					m_Model.GetCurrentRank.ToString("0")
				} });
			}
			else
			{
				m_BonusReward.gameObject.SetActive(false);
			}
		}
		else
		{
			m_BonusReward.gameObject.SetActive(false);
		}
	}

	private void SetupRankingRay()
	{
		foreach (Transform item in m_RankingRay)
		{
			foreach (Transform item2 in item)
			{
				UnityEngine.Object.Destroy(item2.gameObject);
			}
		}
		Dictionary<int, OpponentInfoAlignment> dictionary = EvaluateOpponentScoreOffsets();
		List<Leaderboard.Score> rankedPlayers = m_Model.GetRankedPlayers(false);
		for (int i = 0; i < rankedPlayers.Count && i < m_RankingRay.Count; i++)
		{
			Leaderboard.Score score = rankedPlayers[i];
			if (!dictionary.ContainsKey(i))
			{
				continue;
			}
			OpponentInfoElement opponentInfoElement = null;
			if (score.GetAccountId() == "current")
			{
				opponentInfoElement = UnityEngine.Object.Instantiate(m_SelfInfoPrefab);
				opponentInfoElement.SetDefault((int)score.GetPoints(), i + 1, 1, false);
				DIContainerInfrastructure.GetCoreStateMgr().UpdateSharedIds();
				opponentInfoElement.SetModel(new OpponentGameData(DIContainerInfrastructure.GetCurrentPlayer().PublicPlayer, true), true);
			}
			else if (!string.IsNullOrEmpty(score.GetAccountId()))
			{
				opponentInfoElement = UnityEngine.Object.Instantiate(m_OpponentInfoPrefab);
				opponentInfoElement.SetDefault((int)score.GetPoints(), i + 1, 1, false);
				if (m_Model.PublicOpponentDatas.ContainsKey(score.GetAccountId()))
				{
					DIContainerInfrastructure.GetCoreStateMgr().UpdateSharedIds(m_Model.PublicOpponentDatas[score.GetAccountId()], score.GetAccountId());
					opponentInfoElement.SetModel(new OpponentGameData(m_Model.PublicOpponentDatas[score.GetAccountId()]), false);
				}
			}
			if (opponentInfoElement != null)
			{
				opponentInfoElement.transform.parent = m_RankingRay[i].transform;
				opponentInfoElement.transform.localPosition = Vector3.zero;
				opponentInfoElement.gameObject.PlayAnimationOrAnimatorState("PlayerMarker_" + dictionary[i]);
				opponentInfoElement.m_ElementPressedTrigger.Clicked -= LeaderBoardsButtonClicked;
				opponentInfoElement.m_ElementPressedTrigger.Clicked += LeaderBoardsButtonClicked;
			}
		}
	}

	private Dictionary<int, OpponentInfoAlignment> EvaluateOpponentScoreOffsets()
	{
		Dictionary<int, OpponentInfoAlignment> dictionary = new Dictionary<int, OpponentInfoAlignment>();
		if (m_Model.GetCurrentRank == m_RankingRay.Count)
		{
			dictionary.Add(m_Model.GetCurrentRank - 1, OpponentInfoAlignment.Self_Center);
			dictionary.Add(0, OpponentInfoAlignment.Other_Center);
			dictionary.Add(m_Model.GetCurrentRank - 2, OpponentInfoAlignment.Other_Right_2);
		}
		else if (m_Model.GetCurrentRank == m_RankingRay.Count - 1)
		{
			dictionary.Add(m_Model.GetCurrentRank - 1, OpponentInfoAlignment.Self_Right_1);
			dictionary.Add(0, OpponentInfoAlignment.Other_Center);
			dictionary.Add(m_Model.GetCurrentRank, OpponentInfoAlignment.Other_Center);
			dictionary.Add(m_Model.GetCurrentRank - 2, OpponentInfoAlignment.Other_Right_3);
		}
		else if (m_Model.GetCurrentRank == 1)
		{
			dictionary.Add(1, OpponentInfoAlignment.Other_Left_2);
			dictionary.Add(0, OpponentInfoAlignment.Self_Center);
		}
		else if (m_Model.GetCurrentRank == 2)
		{
			dictionary.Add(0, OpponentInfoAlignment.Other_Center);
			dictionary.Add(1, OpponentInfoAlignment.Self_Left_1);
			dictionary.Add(2, OpponentInfoAlignment.Other_Left_3);
		}
		else if (m_Model.GetCurrentRank == 3)
		{
			dictionary.Add(0, OpponentInfoAlignment.Other_Center);
			dictionary.Add(1, OpponentInfoAlignment.Other_Left_1);
			dictionary.Add(2, OpponentInfoAlignment.Self_Left_2);
			dictionary.Add(3, OpponentInfoAlignment.Other_Left_4);
		}
		else if (m_Model.GetCurrentRank == 4)
		{
			dictionary.Add(0, OpponentInfoAlignment.Other_Center);
			dictionary.Add(2, OpponentInfoAlignment.Other_Center);
			dictionary.Add(3, OpponentInfoAlignment.Self_Left_1);
			dictionary.Add(4, OpponentInfoAlignment.Other_Left_4);
		}
		else
		{
			dictionary.Add(m_Model.GetCurrentRank - 1, OpponentInfoAlignment.Self_Center);
			dictionary.Add(0, OpponentInfoAlignment.Other_Center);
			dictionary.Add(m_Model.GetCurrentRank - 2, OpponentInfoAlignment.Other_Right_2);
			dictionary.Add(m_Model.GetCurrentRank, OpponentInfoAlignment.Other_Left_2);
		}
		return dictionary;
	}

	public void ShowEnergyTooltip()
	{
		if (DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "event_energy") >= DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").ItemMaxCaps["event_energy"])
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowGenericOverlay(m_EnergyDisplay.transform, DIContainerInfrastructure.GetLocaService().Tr("eventdetail_full_energy"), true);
			return;
		}
		DateTime dateTime = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(DIContainerInfrastructure.GetCurrentPlayer().Data.LastEnergyAddTime).AddSeconds(DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.BalancingData.EnergyRefreshTimeInSeconds);
		DateTime trustedTime;
		if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			TimeSpan span = dateTime - trustedTime;
			string localizedText = DIContainerInfrastructure.GetLocaService().Tr("eventdetail_next_energy").Replace("{value_1}", DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(span));
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowGenericOverlay(m_EnergyDisplay.transform, localizedText, true);
		}
	}

	public void OnDestroy()
	{
		DeRegisterEventHandler();
	}

	private void GotoCampaignClicked()
	{
		DIContainerInfrastructure.GetCoreStateMgr().GoToMiniCampaign();
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using Rcs;
using UnityEngine;

public class PvpDetailUI : MonoBehaviour
{
	[SerializeField]
	private UILabel m_objectiveTimer;

	[SerializeField]
	private UILabel m_PvpLeagueName;

	[SerializeField]
	private UISprite m_PvpLeagueIcon;

	[SerializeField]
	private UILabel m_TimeLeft;

	[SerializeField]
	private LootWheelController m_LootWheelPreview;

	[SerializeField]
	private UIInputTrigger m_LeaderBoardsButton;

	[SerializeField]
	private UIInputTrigger m_LeaveButton;

	[SerializeField]
	private LootDisplayContoller m_CollectibleItem;

	[SerializeField]
	private ResourceCostBlind m_BonusReward;

	[SerializeField]
	private UILabel m_NeededRankText;

	[SerializeField]
	private UILabel m_CurrentLeagueText;

	[SerializeField]
	private UISprite m_CurrentLeagueSprite;

	[SerializeField]
	private List<Transform> m_RankingRay = new List<Transform>();

	[SerializeField]
	private Transform m_objectivesGrid;

	[SerializeField]
	private DailyObjectiveDetailElement m_objectivePrefab;

	private ArenaCampStateMgr m_StateMgr;

	private PvPSeasonManagerGameData m_Model;

	private PvPTurnManagerGameData m_TurnModel;

	private UIPanel m_CharacterPanel;

	[SerializeField]
	private OpponentInfoElement m_OpponentInfoPrefab;

	[SerializeField]
	private OpponentInfoElement m_SelfInfoPrefab;

	[SerializeField]
	private UIInputTrigger m_ResetObjectivesTrigger;

	[SerializeField]
	[Header("Server Update Elements")]
	private GameObject m_RankingPanel;

	[SerializeField]
	private GameObject m_WaitingLoadingSpinner;

	[SerializeField]
	private GameObject m_WaitingScoreIcon;

	[SerializeField]
	private GameObject m_UnrankedInfoRoot;

	[SerializeField]
	private UILabel m_WaitingForServerLabel;

	private bool m_uptoDate;

	private bool m_IsUpdatingLeaderboard;

	private void Awake()
	{
		base.transform.position += DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.transform.position;
		UIPanel[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIPanel>();
		UIPanel[] array = componentsInChildren;
		foreach (UIPanel uIPanel in array)
		{
			uIPanel.enabled = false;
		}
	}

	public PvpDetailUI SetStateMgr(ArenaCampStateMgr arenaCampStateMgr)
	{
		m_StateMgr = arenaCampStateMgr;
		return this;
	}

	public void SetModel(PvPSeasonManagerGameData seasonManager)
	{
		m_Model = seasonManager;
		m_TurnModel = seasonManager.CurrentSeasonTurn;
		Dictionary<string, LootInfoData> dictionary = DIContainerLogic.GetLootOperationService().GenerateLootPreview(new Dictionary<string, int> { 
		{
			seasonManager.GetSeasonTurnLootTableWheel(),
			1
		} }, DIContainerInfrastructure.GetCurrentPlayer().Data.Level);
		IInventoryItemGameData data;
		if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "pvp_league_crown", out data))
		{
			int level = data.ItemData.Level;
			m_PvpLeagueName.text = DIContainerInfrastructure.GetLocaService().Tr("pvp_league_" + level.ToString("00") + "_name");
			m_PvpLeagueIcon.spriteName = PvPSeasonManagerGameData.GetLeagueAssetName(level);
		}
		m_RankingPanel.SetActive(false);
		m_LeaderBoardsButton.gameObject.SetActive(false);
		m_UnrankedInfoRoot.SetActive(true);
		RequestLeaderboard();
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
		if (m_Model.CurrentSeasonTurn.Data.CurrentOpponents != null && m_Model.CurrentSeasonTurn.Data.CurrentOpponents.Count > 1)
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
		if (m_Model.CurrentSeasonTurn.Data.CurrentOpponents != null && m_Model.CurrentSeasonTurn.Data.CurrentOpponents.Count < 1)
		{
			DIContainerLogic.PvPSeasonService.SubmitPvPTurnScore(DIContainerInfrastructure.GetCurrentPlayer(), m_Model, OnLeaderboardFound);
		}
		else
		{
			DIContainerLogic.PvPSeasonService.GetLeaderboardScores(DIContainerInfrastructure.GetCurrentPlayer(), m_Model.CurrentSeasonTurn.Data.CurrentOpponents, m_Model, OnLeaderboardFound);
		}
	}

	private void OnLeaderboardUpdated(RESTResultEnum result)
	{
		SetLeaderboardInfo();
	}

	private void OnLeaderboardFound(RESTResultEnum result)
	{
		if (result == RESTResultEnum.Success)
		{
			SetLeaderboardInfo();
		}
		else if (m_WaitingForServerLabel != null)
		{
			m_WaitingForServerLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_leaderboard_error");
		}
	}

	private void SetLeaderboardInfo()
	{
		m_WaitingLoadingSpinner.SetActive(false);
		m_RankingPanel.SetActive(true);
		SetupRankingRay();
		m_UnrankedInfoRoot.SetActive(false);
		m_LeaderBoardsButton.gameObject.SetActive(true);
	}

	private IEnumerator CountDownTimer()
	{
		DateTime trustedTime;
		while (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		DateTime targetTime = DIContainerLogic.PvPSeasonService.GetPvpTurnEndTime(m_Model);
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

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, HandleBackButton);
		DIContainerInfrastructure.GetCurrentPlayer().GlobalPvPStateChanged += GlobalPvPStateChanged;
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
		m_LeaderBoardsButton.Clicked += LeaderBoardsButtonClicked;
		m_LeaveButton.Clicked += LeaveButtonClicked;
		m_ResetObjectivesTrigger.Clicked += ResetObjectivesClicked;
	}

	private void ResetObjectivesClicked()
	{
		Leave(false);
		m_StateMgr.ShowObjectiveRefreshPopup();
	}

	private void GlobalPvPStateChanged(CurrentGlobalEventState arg1, CurrentGlobalEventState arg2)
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
		DIContainerInfrastructure.GetCurrentPlayer().GlobalPvPStateChanged -= GlobalPvPStateChanged;
		m_LeaderBoardsButton.Clicked -= LeaderBoardsButtonClicked;
		m_LeaveButton.Clicked -= LeaveButtonClicked;
		m_ResetObjectivesTrigger.Clicked -= ResetObjectivesClicked;
	}

	private void ShowClockTooltip()
	{
		PvPSeasonManagerGameData currentPvPSeasonGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData;
		int seasonTurnAmount = currentPvPSeasonGameData.Balancing.SeasonTurnAmount;
		string localizedText = DIContainerInfrastructure.GetLocaService().Tr("pvp_clock_tt").Replace("{value_1}", seasonTurnAmount.ToString());
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowGenericOverlay(m_TimeLeft.transform.parent, localizedText, true);
	}

	private void LeaveButtonClicked()
	{
		Leave(true);
	}

	private void LeaderBoardsButtonClicked()
	{
		m_StateMgr.ShowLeaderBoardScreen(false);
	}

	public void Leave(bool andEnterUi)
	{
		StartCoroutine(LeaveCoroutine(andEnterUi));
	}

	private IEnumerator LeaveCoroutine(bool andEnterUi)
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("pvp_details_animate");
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave(!andEnterUi);
		if (andEnterUi)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.EnterLevelDisplay();
		}
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Window_EventDetails_Leave"));
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("pvp_details_animate");
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
		StartCoroutine(EnterCoroutine());
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("pvp_details_animate");
		SetupRankingRay();
		SetupLootWheel();
		SetupRankingBonus();
		SetupTimer();
		StartCoroutine(FillObjectiveBoard());
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Window_EventDetails_Enter"));
		RegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("pvp_details_animate");
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
			}
			yield return new WaitForSeconds(1f);
		}
		StartCoroutine(FillObjectiveBoard());
	}

	private void SetupLeagueInfo()
	{
		m_CurrentLeagueText.text = DIContainerInfrastructure.GetLocaService().Tr("pvp_league_" + m_Model.Data.CurrentLeague.ToString("00") + "_name");
		m_CurrentLeagueSprite.spriteName = PvPSeasonManagerGameData.GetLeagueAssetName(m_Model.Data.CurrentLeague);
	}

	private void SetupLootWheel()
	{
		if (m_Model != null)
		{
			m_LootWheelPreview.SetLootIcons(m_Model.GetSeasonTurnLootTableWheel(), DIContainerInfrastructure.GetCurrentPlayer().Data.Level, 3);
			UnityHelper.SetLayerRecusively(m_LootWheelPreview.gameObject, LayerMask.NameToLayer("Interface"));
		}
	}

	private void SetupTimer()
	{
		StopCoroutine("CountDownTimer");
		if (m_Model != null)
		{
			switch (m_Model.CurrentSeasonTurn.CurrentPvPTurnManagerState)
			{
			case EventManagerState.Running:
				StartCoroutine("CountDownTimer");
				break;
			case EventManagerState.Finished:
			case EventManagerState.FinishedWithoutPoints:
				m_TimeLeft.text = DIContainerInfrastructure.GetLocaService().Tr("event_banner_finished", "Finished!");
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	private void SetupRankingBonus()
	{
		if (m_Model != null && m_Model.CurrentSeasonTurn.Data.CurrentScore != 0 && m_Model.Balancing.PvPBonusLootTablesPerRank.Count > m_Model.CurrentSeasonTurn.GetCurrentRank - 1)
		{
			List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(new Dictionary<string, int> { 
			{
				m_Model.CurrentSeasonTurn.GetScalingRankRewardLootTable(),
				1
			} }, m_Model.Data.CurrentLeague));
			IInventoryItemGameData inventoryItemGameData = itemsFromLoot.FirstOrDefault();
			if (inventoryItemGameData != null)
			{
				m_BonusReward.gameObject.SetActive(true);
				m_BonusReward.SetModel(inventoryItemGameData.ItemAssetName, null, inventoryItemGameData.ItemValue, string.Empty);
				m_NeededRankText.text = DIContainerInfrastructure.GetLocaService().Tr("eventwindow_bonusreward_rankinfo", new Dictionary<string, string> { 
				{
					"{value_1}",
					m_Model.CurrentSeasonTurn.GetCurrentRank.ToString("0")
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
		List<Leaderboard.Score> rankedPlayers = m_Model.CurrentSeasonTurn.GetRankedPlayers(false);
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
				opponentInfoElement.SetDefault((int)score.GetPoints(), i + 1, 1, false, true);
				DIContainerInfrastructure.GetCoreStateMgr().UpdateSharedIds();
				opponentInfoElement.SetModel(new OpponentGameData(DIContainerInfrastructure.GetCurrentPlayer().PublicPlayer, true), true);
			}
			else if (!string.IsNullOrEmpty(score.GetAccountId()))
			{
				opponentInfoElement = UnityEngine.Object.Instantiate(m_OpponentInfoPrefab);
				opponentInfoElement.SetDefault((int)score.GetPoints(), i + 1, 1, true);
				if (m_Model.CurrentSeasonTurn.PublicOpponentDatas.ContainsKey(score.GetAccountId()))
				{
					DIContainerInfrastructure.GetCoreStateMgr().UpdateSharedIds(m_Model.CurrentSeasonTurn.PublicOpponentDatas[score.GetAccountId()], score.GetAccountId());
					opponentInfoElement.SetModel(new OpponentGameData(m_Model.CurrentSeasonTurn.PublicOpponentDatas[score.GetAccountId()]), false);
				}
			}
			if (opponentInfoElement != null)
			{
				opponentInfoElement.transform.parent = m_RankingRay[i].transform;
				opponentInfoElement.transform.localPosition = Vector3.zero;
				opponentInfoElement.gameObject.PlayAnimationOrAnimatorState("PlayerMarker_" + dictionary[i]);
			}
		}
	}

	private Dictionary<int, OpponentInfoAlignment> EvaluateOpponentScoreOffsets()
	{
		Dictionary<int, OpponentInfoAlignment> dictionary = new Dictionary<int, OpponentInfoAlignment>();
		if (m_Model.CurrentSeasonTurn.GetCurrentRank == m_RankingRay.Count)
		{
			dictionary.Add(m_Model.CurrentSeasonTurn.GetCurrentRank - 1, OpponentInfoAlignment.Self_Center);
			dictionary.Add(0, OpponentInfoAlignment.Other_Center);
			dictionary.Add(m_Model.CurrentSeasonTurn.GetCurrentRank - 2, OpponentInfoAlignment.Other_Right_2);
		}
		else if (m_Model.CurrentSeasonTurn.GetCurrentRank == m_RankingRay.Count - 1)
		{
			dictionary.Add(m_Model.CurrentSeasonTurn.GetCurrentRank - 1, OpponentInfoAlignment.Self_Right_1);
			dictionary.Add(0, OpponentInfoAlignment.Other_Center);
			dictionary.Add(m_Model.CurrentSeasonTurn.GetCurrentRank, OpponentInfoAlignment.Other_Center);
			dictionary.Add(m_Model.CurrentSeasonTurn.GetCurrentRank - 2, OpponentInfoAlignment.Other_Right_3);
		}
		else if (m_Model.CurrentSeasonTurn.GetCurrentRank == 1)
		{
			dictionary.Add(1, OpponentInfoAlignment.Other_Left_2);
			dictionary.Add(0, OpponentInfoAlignment.Self_Center);
		}
		else if (m_Model.CurrentSeasonTurn.GetCurrentRank == 2)
		{
			dictionary.Add(0, OpponentInfoAlignment.Other_Center);
			dictionary.Add(1, OpponentInfoAlignment.Self_Left_1);
			dictionary.Add(2, OpponentInfoAlignment.Other_Left_3);
		}
		else if (m_Model.CurrentSeasonTurn.GetCurrentRank == 3)
		{
			dictionary.Add(0, OpponentInfoAlignment.Other_Center);
			dictionary.Add(1, OpponentInfoAlignment.Other_Left_1);
			dictionary.Add(2, OpponentInfoAlignment.Self_Left_2);
			dictionary.Add(3, OpponentInfoAlignment.Other_Left_4);
		}
		else if (m_Model.CurrentSeasonTurn.GetCurrentRank == 4)
		{
			dictionary.Add(0, OpponentInfoAlignment.Other_Center);
			dictionary.Add(2, OpponentInfoAlignment.Other_Center);
			dictionary.Add(3, OpponentInfoAlignment.Self_Left_1);
			dictionary.Add(4, OpponentInfoAlignment.Other_Left_4);
		}
		else
		{
			dictionary.Add(m_Model.CurrentSeasonTurn.GetCurrentRank - 1, OpponentInfoAlignment.Self_Center);
			dictionary.Add(0, OpponentInfoAlignment.Other_Center);
			dictionary.Add(m_Model.CurrentSeasonTurn.GetCurrentRank - 2, OpponentInfoAlignment.Other_Right_2);
			dictionary.Add(m_Model.CurrentSeasonTurn.GetCurrentRank, OpponentInfoAlignment.Other_Left_2);
		}
		return dictionary;
	}

	private IEnumerator FillObjectiveBoard()
	{
		foreach (Transform child in m_objectivesGrid)
		{
			UnityEngine.Object.Destroy(child.gameObject);
		}
		yield return new WaitForEndOfFrame();
		List<PvPObjectivesGameData> objectives = DIContainerLogic.GetPvpObjectivesService().GetDailyObjectives();
		foreach (PvPObjectivesGameData gameData in objectives)
		{
			DailyObjectiveDetailElement element = UnityEngine.Object.Instantiate(m_objectivePrefab);
			Transform objectiveTransform = element.transform;
			objectiveTransform.parent = m_objectivesGrid;
			objectiveTransform.localScale = Vector3.one;
			objectiveTransform.localPosition = Vector3.zero;
			element.Init(gameData);
		}
		m_objectivesGrid.GetComponent<UIGrid>().Reposition();
		StopCoroutine("CountDownPvpObjectiveAndEnergyTimer");
		StartCoroutine("CountDownPvpObjectiveAndEnergyTimer");
	}
}

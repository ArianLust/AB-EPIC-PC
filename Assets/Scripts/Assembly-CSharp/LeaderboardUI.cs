using System;
using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using Rcs;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
	private EventManagerGameData m_EventModel;

	private PvPSeasonManagerGameData m_PvPModel;

	[SerializeField]
	private UIInputTrigger m_LeaveButton;

	[SerializeField]
	private UILabel m_TimeLeft;

	[SerializeField]
	private UILabel m_HeaderLabel;

	[SerializeField]
	private GameObject m_HeaderNormal;

	[SerializeField]
	private GameObject m_HeaderFriends;

	[SerializeField]
	private UISprite m_HeaderSprite;

	[SerializeField]
	private OpponentInfoElement m_LeaderBoardBlindPrefab;

	[SerializeField]
	private GameObject m_LeaderboardInactiveSumBlind;

	[SerializeField]
	private UIGrid m_Grid;

	private BaseLocationStateManager m_worldmapStateMgr;

	private PanelClippingLayoutTLBRControl m_clippingControl;

	private bool m_directly;

	private int m_activeTab;

	[SerializeField]
	private UIInputTrigger[] m_leagueTabs;

	[SerializeField]
	private Animator[] m_TabAnimators;

	[SerializeField]
	private GameObject m_emptyFriendListIndicator;

	[SerializeField]
	private UIInputTrigger m_pageLeftTrigger;

	[SerializeField]
	private UIInputTrigger m_pageRightTrigger;

	[SerializeField]
	private UILabel m_Tab1Label;

	[SerializeField]
	private UILabel m_Tab2Label;

	[SerializeField]
	private UILabel m_Tab3Label;

	[SerializeField]
	private List<Color> m_TeamColors;

	[SerializeField]
	private List<AspectClippingPair> m_aspectClippingPairs;

	private int m_currentPage;

	private int m_maxPages;

	private EventDetailUI m_eventDetail;

	[SerializeField]
	private GameObject m_CheaterBoardLabelRoot;

	private void Awake()
	{
		UIPanel[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIPanel>();
		UIPanel[] array = componentsInChildren;
		foreach (UIPanel uIPanel in array)
		{
			uIPanel.enabled = false;
		}
		m_clippingControl = m_Grid.GetComponentInParent<PanelClippingLayoutTLBRControl>();
		m_currentPage = 0;
	}

	public void Enter(WorldBossTeamData ownTeam, WorldBossTeamData enemyTeam, bool directly = false, EventDetailUI detailUI = null)
	{
		base.gameObject.SetActive(true);
		m_eventDetail = detailUI;
		m_directly = directly;
		UIPanel[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIPanel>();
		UIPanel[] array = componentsInChildren;
		foreach (UIPanel uIPanel in array)
		{
			uIPanel.enabled = true;
		}
		if ((bool)m_worldmapStateMgr)
		{
			m_worldmapStateMgr.WorldMenuUI.Leave();
		}
		SetTabs(ownTeam, enemyTeam);
		StartCoroutine(EnterCoroutine());
	}

	private void OnLeagueTabClicked()
	{
		if (m_activeTab != 0)
		{
			m_activeTab = 0;
			m_TabAnimators[0].Play("SetActive");
			m_TabAnimators[1].Play("SetInactive");
			if (m_TabAnimators.Length >= 3)
			{
				m_TabAnimators[2].Play("SetInactive");
			}
			StartCoroutine(SetupLeagueBlinds(false));
		}
	}

	private void OnFriendTabClicked()
	{
		if (m_activeTab != 1)
		{
			m_activeTab = 1;
			m_TabAnimators[0].Play("SetInactive");
			if (m_EventModel != null && m_EventModel.IsBossEvent)
			{
				m_TabAnimators[1].Play("SetInactive");
				m_TabAnimators[2].Play("SetActive");
			}
			else
			{
				m_TabAnimators[1].Play("SetActive");
			}
			StartCoroutine(SetupFriendBlinds(m_currentPage));
		}
	}

	private void OnEnemyLeagueTabClicked()
	{
		if (m_activeTab != 2)
		{
			m_activeTab = 2;
			m_TabAnimators[0].Play("SetInactive");
			m_TabAnimators[1].Play("SetActive");
			m_TabAnimators[2].Play("SetInactive");
			StartCoroutine(SetupLeagueBlinds(true));
		}
	}

	public void SetEventModel(EventManagerGameData eventManagerGameData)
	{
		m_EventModel = eventManagerGameData;
		m_PvPModel = null;
	}

	public void SetPvPModel(PvPSeasonManagerGameData pvpManagerGameData)
	{
		m_EventModel = null;
		m_PvPModel = pvpManagerGameData;
	}

	private void SetTabs(WorldBossTeamData ownTeam, WorldBossTeamData enemyTeam)
	{
		bool enableFriendLeaderboards = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").EnableFriendLeaderboards;
		if (m_EventModel != null)
		{
			if (m_EventModel.IsBossEvent && ownTeam != null && enemyTeam != null)
			{
				m_Tab1Label.text = ownTeam.NameId.Replace("{value_2}", string.Empty).Replace("\n", string.Empty);
				m_Tab1Label.color = m_TeamColors[ownTeam.TeamColor];
				m_Tab2Label.text = enemyTeam.NameId.Replace("{value_2}", string.Empty).Replace("\n", string.Empty);
				m_Tab2Label.color = m_TeamColors[enemyTeam.TeamColor];
				m_Tab3Label.text = DIContainerInfrastructure.GetLocaService().Tr("leaderboard_tab_friends");
				m_TabAnimators[2].gameObject.SetActive(enableFriendLeaderboards);
			}
			else
			{
				m_Tab1Label.text = DIContainerInfrastructure.GetLocaService().Tr("leaderboard_tab_league");
				m_Tab2Label.text = DIContainerInfrastructure.GetLocaService().Tr("leaderboard_tab_friends");
				m_TabAnimators[1].gameObject.SetActive(enableFriendLeaderboards);
				m_TabAnimators[2].gameObject.SetActive(false);
			}
		}
		else
		{
			m_Tab1Label.text = DIContainerInfrastructure.GetLocaService().Tr("leaderboard_tab_league");
			m_Tab2Label.text = DIContainerInfrastructure.GetLocaService().Tr("leaderboard_tab_friends");
			m_TabAnimators[1].gameObject.SetActive(enableFriendLeaderboards);
		}
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(3, HandleBackButton);
		DIContainerInfrastructure.GetCurrentPlayer().GlobalEventStateChanged += GlobalEventStateChanged;
		m_LeaveButton.Clicked += LeaveButtonClicked;
		if (m_EventModel != null && m_EventModel.IsBossEvent)
		{
			m_leagueTabs[0].Clicked += OnLeagueTabClicked;
			m_leagueTabs[1].Clicked += OnEnemyLeagueTabClicked;
			m_leagueTabs[2].Clicked += OnFriendTabClicked;
		}
		else
		{
			m_leagueTabs[0].Clicked += OnLeagueTabClicked;
			m_leagueTabs[1].Clicked += OnFriendTabClicked;
		}
		m_pageLeftTrigger.Clicked += OnPageLeftButtonClicked;
		m_pageRightTrigger.Clicked += OnPageRightButtonClicked;
	}

	private void HandleBackButton()
	{
		LeaveButtonClicked();
	}

	private void OnDestroy()
	{
		DeRegisterEventHandler();
	}

	private void DeRegisterEventHandler()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(3);
		DIContainerInfrastructure.GetCurrentPlayer().GlobalEventStateChanged -= GlobalEventStateChanged;
		m_LeaveButton.Clicked -= LeaveButtonClicked;
		if (m_EventModel != null && m_EventModel.IsBossEvent)
		{
			m_leagueTabs[0].Clicked -= OnLeagueTabClicked;
			m_leagueTabs[1].Clicked -= OnEnemyLeagueTabClicked;
			m_leagueTabs[2].Clicked -= OnFriendTabClicked;
		}
		else
		{
			m_leagueTabs[0].Clicked -= OnLeagueTabClicked;
			m_leagueTabs[1].Clicked -= OnFriendTabClicked;
		}
		m_pageLeftTrigger.Clicked -= OnPageLeftButtonClicked;
		m_pageRightTrigger.Clicked -= OnPageRightButtonClicked;
	}

	private void OnPageLeftButtonClicked()
	{
		m_currentPage--;
		StartCoroutine(SetupFriendBlinds(m_currentPage));
	}

	private void OnPageRightButtonClicked()
	{
		m_currentPage++;
		StartCoroutine(SetupFriendBlinds(m_currentPage));
	}

	private void LeaveButtonClicked()
	{
		Leave();
	}

	public void Leave()
	{
		StartCoroutine(LeaveCoroutine());
	}

	private IEnumerator LeaveCoroutine()
	{
		DeRegisterEventHandler();
		if (m_directly)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(8u);
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.EnterLevelDisplay();
		}
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("leaderboard_animate");
		if (m_eventDetail != null)
		{
			m_eventDetail.ComeBackFromLeaderBoard();
		}
		if ((bool)m_worldmapStateMgr)
		{
			m_worldmapStateMgr.WorldMenuUI.Leave();
		}
		foreach (Transform child in m_Grid.transform)
		{
			UnityEngine.Object.Destroy(child.gameObject);
		}
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Window_LeaderBoard_Leave"));
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("leaderboard_animate");
		if (UIInput.current != null)
		{
			UIInput.current.enabled = false;
		}
		base.gameObject.SetActive(false);
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("leaderboard_animate");
		SetupHeader();
		SetupTimer();
		m_activeTab = -1;
		OnLeagueTabClicked();
		if (m_directly)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
			{
				Depth = 8u
			}, false);
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveLevelDisplay();
		}
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Window_LeaderBoard_Enter"));
		float uiPanelOffset = GetStandardUiPanelOffset();
		m_clippingControl.transform.localPosition = Vector3.zero;
		m_clippingControl.transform.GetComponent<UIPanel>().clipOffset = new Vector2(0f, uiPanelOffset);
		RegisterEventHandler();
		m_clippingControl.enabled = true;
		m_clippingControl.transform.localPosition = Vector3.zero;
		m_clippingControl.enabled = false;
		m_Grid.Reposition();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("leaderboard_animate");
	}

	private void SetupHeader()
	{
		if (m_PvPModel != null)
		{
			if ((bool)m_HeaderLabel)
			{
				string text = DIContainerInfrastructure.GetLocaService().Tr(DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData.Balancing.LocaBaseId + "_name");
				IInventoryItemGameData data = null;
				if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "pvp_league_crown", out data))
				{
					int level = data.ItemData.Level;
					if (m_HeaderSprite != null)
					{
						m_HeaderSprite.spriteName = PvPSeasonManagerGameData.GetLeagueAssetName(level);
					}
					if (m_HeaderLabel != null)
					{
						m_HeaderLabel.text = DIContainerInfrastructure.GetLocaService().GetLeagueName(level);
					}
				}
			}
			if (m_CheaterBoardLabelRoot != null)
			{
				m_CheaterBoardLabelRoot.SetActive(m_PvPModel.CurrentSeasonTurn.IsCheaterboard);
			}
		}
		else if (m_EventModel != null && m_CheaterBoardLabelRoot != null)
		{
			m_CheaterBoardLabelRoot.SetActive(m_EventModel.IsCheaterboard);
		}
	}

	private List<Leaderboard.Score> GetRankedPlayers(bool alsoGetZero = false, bool enemyTeam = false)
	{
		if (m_EventModel != null)
		{
			if (!enemyTeam)
			{
				return m_EventModel.GetRankedPlayers(alsoGetZero);
			}
			return m_EventModel.GetRankedPlayersEnemys(alsoGetZero);
		}
		if (m_PvPModel != null)
		{
			return m_PvPModel.CurrentSeasonTurn.GetRankedPlayers(alsoGetZero);
		}
		return new List<Leaderboard.Score>();
	}

	private Dictionary<string, PublicPlayerData> GetPublicOpponentDatas()
	{
		if (m_EventModel != null)
		{
			return m_EventModel.PublicOpponentDatas;
		}
		if (m_PvPModel != null)
		{
			return m_PvPModel.CurrentSeasonTurn.PublicOpponentDatas;
		}
		return new Dictionary<string, PublicPlayerData>();
	}

	private uint GetMaximumMatchmakingPlayers()
	{
		if (m_EventModel != null)
		{
			return m_EventModel.Balancing.MaximumMatchmakingPlayers;
		}
		if (m_PvPModel != null)
		{
			return m_PvPModel.Balancing.MaximumMatchmakingPlayers;
		}
		return 0u;
	}

	private uint GetCurrentScore()
	{
		if (m_EventModel != null)
		{
			return m_EventModel.Data.CurrentScore;
		}
		if (m_PvPModel != null)
		{
			return m_PvPModel.CurrentSeasonTurn.Data.CurrentScore;
		}
		return 0u;
	}

	private int GetStarRatingForRank(int rank)
	{
		if (m_EventModel != null && m_EventModel.EventBalancing.StarRatingForRanking.ContainsKey(rank))
		{
			return m_EventModel.EventBalancing.StarRatingForRanking[rank];
		}
		if (m_PvPModel != null && m_PvPModel.Balancing.StarRatingForRanking.ContainsKey(rank))
		{
			return m_PvPModel.Balancing.StarRatingForRanking[rank];
		}
		return 0;
	}

	private IEnumerator SetupLeagueBlinds(bool enemyTeam)
	{
		foreach (Transform child in m_Grid.transform)
		{
			UnityEngine.Object.Destroy(child.gameObject);
		}
		if ((bool)m_emptyFriendListIndicator)
		{
			m_emptyFriendListIndicator.SetActive(false);
		}
		if (m_PvPModel == null)
		{
			m_HeaderLabel.text = DIContainerInfrastructure.GetLocaService().Tr("leaderboard_header");
		}
		m_pageRightTrigger.gameObject.SetActive(false);
		m_pageLeftTrigger.gameObject.SetActive(false);
		if ((bool)m_HeaderFriends)
		{
			m_HeaderFriends.SetActive(false);
		}
		if ((bool)m_HeaderNormal)
		{
			m_HeaderNormal.SetActive(true);
		}
		m_Grid.transform.parent.GetComponent<UIScrollView>().enabled = true;
		yield return new WaitForEndOfFrame();
		List<Leaderboard.Score> players = GetRankedPlayers(true, enemyTeam);
		for (int rank = 0; rank < players.Count; rank++)
		{
			Leaderboard.Score score = players[rank];
			DebugLog.Log(GetType(), "SetupLeagueBlinds: user= " + score.GetAccountId() + " --- rank= " + rank);
			OpponentInfoElement oppInfo2 = null;
			oppInfo2 = UnityEngine.Object.Instantiate(m_LeaderBoardBlindPrefab);
			oppInfo2.transform.parent = m_Grid.transform;
			oppInfo2.transform.localPosition = Vector3.zero;
			if (score.GetAccountId() == "current")
			{
				oppInfo2.SetDefault((int)score.GetPoints(), rank + 1, ((int)score.GetPoints() > 0) ? GetStarRatingForRank(rank + 1) : 0, m_PvPModel != null, false, true);
				DIContainerInfrastructure.GetCoreStateMgr().UpdateSharedIds();
				oppInfo2.SetModel(new OpponentGameData(DIContainerInfrastructure.GetCurrentPlayer().PublicPlayer, true), true);
				oppInfo2.SetCheater(IsCheaterInRespectiveModel(DIContainerInfrastructure.IdentityService.SharedId));
			}
			else if (!string.IsNullOrEmpty(score.GetAccountId()))
			{
				oppInfo2.SetDefault((int)score.GetPoints(), rank + 1, ((int)score.GetPoints() > 0) ? GetStarRatingForRank(rank + 1) : 0, m_PvPModel != null, enemyTeam);
				if (GetPublicOpponentDatas().ContainsKey(score.GetAccountId()))
				{
					PublicPlayerData ppd = GetPublicOpponentDatas()[score.GetAccountId()];
					DIContainerInfrastructure.GetCoreStateMgr().UpdateSharedIds(ppd, score.GetAccountId());
					oppInfo2.SetModel(new OpponentGameData(ppd), false, enemyTeam);
					oppInfo2.SetCheater(IsCheaterInRespectiveModel(score.GetAccountId()));
				}
			}
		}
		m_Grid.Reposition();
	}

	private bool IsCheaterInRespectiveModel(string playerId)
	{
		if (m_EventModel != null && m_EventModel.Data.CheatingOpponents != null)
		{
			return !m_EventModel.IsCheaterboard && m_EventModel.Data.CheatingOpponents.Contains(playerId);
		}
		if (m_PvPModel != null && m_PvPModel.CurrentSeasonTurn.Data.CheatingOpponents != null)
		{
			return !m_PvPModel.CurrentSeasonTurn.IsCheaterboard && m_PvPModel.CurrentSeasonTurn.Data.CheatingOpponents.Contains(playerId);
		}
		return false;
	}

	private IEnumerator SetupFriendBlinds(int pageNum)
	{
		if (m_activeTab != 1)
		{
			yield break;
		}
		foreach (Transform child in m_Grid.transform)
		{
			UnityEngine.Object.Destroy(child.gameObject);
		}
		yield return new WaitForEndOfFrame();
		Dictionary<int, FriendGameData> sortedFriends = null;
		Dictionary<string, int> friendScores = null;
		if (m_EventModel != null)
		{
			sortedFriends = m_EventModel.GetFriendScoresByRank();
			friendScores = m_EventModel.GetFriendScoresById();
		}
		else if (m_PvPModel != null)
		{
			sortedFriends = m_PvPModel.GetFriendScoresByRank();
			friendScores = m_PvPModel.GetFriendScoresById();
		}
		if (sortedFriends == null || sortedFriends.Count == 0)
		{
			yield return new WaitForSeconds(1f);
			StartCoroutine(SetupFriendBlinds(pageNum));
			yield break;
		}
		if ((bool)m_emptyFriendListIndicator)
		{
			m_emptyFriendListIndicator.SetActive(sortedFriends.Count == 1);
		}
		m_maxPages = sortedFriends.Count / 15;
		bool hasPages = m_maxPages == 0;
		m_pageLeftTrigger.gameObject.SetActive(m_currentPage > 0);
		m_pageRightTrigger.gameObject.SetActive(m_currentPage < m_maxPages);
		if ((bool)m_HeaderFriends)
		{
			m_HeaderFriends.SetActive(true);
		}
		if ((bool)m_HeaderNormal)
		{
			m_HeaderNormal.SetActive(false);
		}
		if (m_maxPages > 0)
		{
			string pageText = m_currentPage + 1 + "/" + (m_maxPages + 1);
			if (m_HeaderFriends != null)
			{
				m_HeaderFriends.GetComponentInChildren<UILabel>().text = DIContainerInfrastructure.GetLocaService().Tr("leaderboard_header_friends " + pageText);
			}
			else
			{
				m_HeaderLabel.text = DIContainerInfrastructure.GetLocaService().Tr("leaderboard_header_friends " + pageText);
			}
		}
		else if (m_HeaderFriends != null)
		{
			m_HeaderFriends.GetComponentInChildren<UILabel>().text = DIContainerInfrastructure.GetLocaService().Tr("leaderboard_header_friends");
		}
		else
		{
			m_HeaderLabel.text = DIContainerInfrastructure.GetLocaService().Tr("leaderboard_header_friends");
		}
		for (int rank = pageNum * 15; rank < (pageNum + 1) * 15 && sortedFriends.Count > rank && sortedFriends.ContainsKey(rank); rank++)
		{
			FriendGameData friend = sortedFriends[rank];
			if (friend != null && friend.isNpcFriend)
			{
				continue;
			}
			OpponentInfoElement oppInfo2 = null;
			oppInfo2 = UnityEngine.Object.Instantiate(m_LeaderBoardBlindPrefab);
			oppInfo2.transform.parent = m_Grid.transform;
			oppInfo2.transform.localPosition = Vector3.zero;
			if (friend == null)
			{
				PlayerGameData player = DIContainerInfrastructure.GetCurrentPlayer();
				int playerscore2 = 0;
				playerscore2 = (int)((m_PvPModel == null) ? player.CurrentEventManagerGameData.Data.CurrentScore : player.CurrentPvPSeasonGameData.CurrentSeasonTurn.Data.CurrentScore);
				oppInfo2.SetDefault(playerscore2, rank + 1, (playerscore2 > 0) ? GetStarRatingForRank(rank + 1) : 0, m_PvPModel != null, true, true);
				oppInfo2.SetModel(new OpponentGameData(player.PublicPlayer), true, true);
			}
			else
			{
				if (!friendScores.ContainsKey(friend.FriendId))
				{
					oppInfo2.SetDefault(0, rank + 1, 0, m_PvPModel != null, true);
				}
				else
				{
					oppInfo2.SetDefault(friendScores[friend.FriendId], rank + 1, (friendScores[friend.FriendId] > 0) ? GetStarRatingForRank(rank + 1) : 0, m_PvPModel != null, true);
				}
				oppInfo2.SetModel(new OpponentGameData(friend.PublicPlayerData), false, true);
			}
		}
		m_Grid.Reposition();
		if (m_Grid.transform.childCount <= 4)
		{
			m_Grid.transform.parent.GetComponent<UIScrollView>().enabled = false;
		}
		else
		{
			m_Grid.transform.parent.GetComponent<UIScrollView>().enabled = true;
		}
	}

	private IEnumerator CountDownTimer(DateTime targetTime)
	{
		DateTime trustedTime;
		while (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		while (targetTime > trustedTime)
		{
			if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				TimeSpan timeLeft = targetTime - trustedTime;
				m_TimeLeft.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(DIContainerLogic.GetTimingService().TimeLeftUntil(targetTime));
			}
			yield return new WaitForSeconds(1f);
		}
		yield return new WaitForSeconds(0.5f);
		SetupTimer();
	}

	private void GlobalEventStateChanged(CurrentGlobalEventState arg1, CurrentGlobalEventState arg2)
	{
		SetupTimer();
	}

	private void SetupTimer()
	{
		if (m_EventModel != null)
		{
			switch (m_EventModel.CurrentEventManagerState)
			{
			case EventManagerState.Teasing:
				m_TimeLeft.text = DIContainerInfrastructure.GetLocaService().Tr("event_teasing", "Coming Soon!");
				break;
			case EventManagerState.Running:
				StartCoroutine(CountDownTimer(DIContainerLogic.EventSystemService.GetEventEndTime(m_EventModel.Balancing)));
				break;
			case EventManagerState.Finished:
			case EventManagerState.FinishedWithoutPoints:
				m_TimeLeft.text = DIContainerInfrastructure.GetLocaService().Tr("event_finished", "Finished!");
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		else if (m_PvPModel != null)
		{
			switch (m_PvPModel.CurrentSeasonTurn.CurrentPvPTurnManagerState)
			{
			case EventManagerState.Running:
				StartCoroutine(CountDownTimer(DIContainerLogic.PvPSeasonService.GetPvpTurnEndTime(m_PvPModel)));
				break;
			case EventManagerState.Finished:
			case EventManagerState.FinishedWithoutPoints:
				m_TimeLeft.text = DIContainerInfrastructure.GetLocaService().Tr("event_finished", "Finished!");
				break;
			default:
				m_TimeLeft.text = DIContainerInfrastructure.GetLocaService().Tr("event_finished", "Finished!");
				break;
			}
		}
	}

	private void ShowClockTooltip()
	{
		PvPSeasonManagerGameData currentPvPSeasonGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData;
		int seasonTurnAmount = currentPvPSeasonGameData.Balancing.SeasonTurnAmount;
		string localizedText = DIContainerInfrastructure.GetLocaService().Tr("pvp_clock_tt").Replace("{value_1}", seasonTurnAmount.ToString());
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowGenericOverlay(m_TimeLeft.transform.parent, localizedText, true);
	}

	public void SetWorldMapStateMgr(BaseLocationStateManager worldMapStateMgr)
	{
		m_worldmapStateMgr = worldMapStateMgr;
	}

	private float GetStandardUiPanelOffset()
	{
		float num = 1.5f;
		Camera[] allCameras = Camera.allCameras;
		foreach (Camera camera in allCameras)
		{
			if (camera.tag == "UICamera")
			{
				num = camera.aspect;
			}
		}
		if ((double)num >= 1.7)
		{
			foreach (AspectClippingPair aspectClippingPair in m_aspectClippingPairs)
			{
				if (aspectClippingPair.Aspect.x == 16f && aspectClippingPair.Aspect.y == 9f)
				{
					return aspectClippingPair.UiPanelYOffset;
				}
			}
		}
		else if ((double)num >= 1.6)
		{
			foreach (AspectClippingPair aspectClippingPair2 in m_aspectClippingPairs)
			{
				if (aspectClippingPair2.Aspect.x == 16f && aspectClippingPair2.Aspect.y == 10f)
				{
					return aspectClippingPair2.UiPanelYOffset;
				}
			}
		}
		else if ((double)num >= 1.5)
		{
			foreach (AspectClippingPair aspectClippingPair3 in m_aspectClippingPairs)
			{
				if (aspectClippingPair3.Aspect.x == 3f && aspectClippingPair3.Aspect.y == 2f)
				{
					return aspectClippingPair3.UiPanelYOffset;
				}
			}
		}
		else if (num >= 1.33f)
		{
			foreach (AspectClippingPair aspectClippingPair4 in m_aspectClippingPairs)
			{
				if (aspectClippingPair4.Aspect.x == 4f && aspectClippingPair4.Aspect.y == 3f)
				{
					return aspectClippingPair4.UiPanelYOffset;
				}
			}
		}
		else
		{
			foreach (AspectClippingPair aspectClippingPair5 in m_aspectClippingPairs)
			{
				if (aspectClippingPair5.Aspect.x == 5f && aspectClippingPair5.Aspect.y == 4f)
				{
					return aspectClippingPair5.UiPanelYOffset;
				}
			}
		}
		return -100f;
	}
}

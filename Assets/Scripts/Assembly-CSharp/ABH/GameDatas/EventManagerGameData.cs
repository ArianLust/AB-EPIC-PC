using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.Shared.BalancingData;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using Interfaces.GameClient;
using Rcs;
using UnityEngine;

namespace ABH.GameDatas
{
	public class EventManagerGameData
	{
		public Dictionary<string, Leaderboard.Score> ScoresByPlayer = new Dictionary<string, Leaderboard.Score>();

		public Dictionary<string, Leaderboard.Score> ScoresByPlayerEnemyTeam = new Dictionary<string, Leaderboard.Score>();

		public Dictionary<string, PublicPlayerData> PublicOpponentDatas = new Dictionary<string, PublicPlayerData>();

		public Dictionary<string, LootInfoData> RolledResultLoot = new Dictionary<string, LootInfoData>();

		public Dictionary<string, LootInfoData> FinalRankBonusLoot = new Dictionary<string, LootInfoData>();

		private bool m_scoresChanged = true;

		private int m_rank = 15;

		private EventCampaignGameData m_CurrentMiniCampaign;

		private BossGameData m_CurrentEventBoss;

		private Dictionary<int, FriendGameData> m_sortedFriends;

		private Dictionary<string, FriendGameData> m_friendsDictionary;

		private List<FriendGameData> m_friends;

		private Dictionary<string, int> m_serverScores;

		private Dictionary<string, int> m_friendScores;

		public bool IsValid
		{
			get
			{
				return Balancing != null && Data != null && Balancing.NameId == Data.NameId;
			}
		}

		public EventManagerData Data { get; private set; }

		public int FailedOnlineMatchmakeCount { get; set; }

		public EventManagerBalancingData Balancing { get; private set; }

		public EventBalancingData EventBalancing { get; private set; }

		public EventManagerState CurrentEventManagerState
		{
			get
			{
				return Data.CurrentState;
			}
			set
			{
				EventManagerState currentState = Data.CurrentState;
				Data.CurrentState = value;
				OnStateChanged(currentState, Data.CurrentState);
			}
		}

		public int ResultRank { get; set; }

		public int ResultStars
		{
			get
			{
				if (EventBalancing.StarRatingForRanking.ContainsKey(ResultRank))
				{
					return EventBalancing.StarRatingForRanking[ResultRank];
				}
				return 0;
			}
		}

		public bool CalledMatchmakeOnce { get; set; }

		public bool IsResultValid { get; set; }

		public int GetCurrentRank
		{
			get
			{
				if (!IsCheaterboard && Data.CheatingOpponents != null && Data.CheatingOpponents.Contains(DIContainerInfrastructure.IdentityService.SharedId))
				{
					int rank = (int)(((!IsBossEvent) ? Balancing.MaximumMatchmakingPlayers : (Balancing.MaximumMatchmakingPlayers / 2)) + 1);
					m_rank = rank;
				}
				else
				{
					m_rank = ScoresByPlayer.Values.Count((Leaderboard.Score s) => s.GetPoints() > Data.CurrentScore) + 1;
				}
				return m_rank;
			}
		}

		public bool IsAssetValid { get; set; }

		public EventCampaignGameData CurrentMiniCampaign
		{
			get
			{
				if (m_CurrentMiniCampaign != null)
				{
					return m_CurrentMiniCampaign;
				}
				if (Data != null && Data.EventCampaignData != null)
				{
					m_CurrentMiniCampaign = new EventCampaignGameData(Data.EventCampaignData);
					return m_CurrentMiniCampaign;
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					m_CurrentMiniCampaign = value;
				}
				Data.EventCampaignData = m_CurrentMiniCampaign.Data;
			}
		}

		public BossGameData CurrentEventBoss
		{
			get
			{
				if (m_CurrentEventBoss != null)
				{
					return m_CurrentEventBoss;
				}
				if (Data.EventBossData == null)
				{
					m_CurrentEventBoss = new BossGameData(EventBalancing.BossId);
					Data.EventBossData = m_CurrentEventBoss.Data;
				}
				else
				{
					m_CurrentEventBoss = new BossGameData(Data.EventBossData);
				}
				return m_CurrentEventBoss;
			}
			set
			{
				if (value != null)
				{
					m_CurrentEventBoss = value;
				}
				Data.EventBossData = m_CurrentEventBoss.Data;
			}
		}

		public bool IsBossEvent
		{
			get
			{
				if (EventBalancing == null)
				{
					return false;
				}
				return EventBalancing.EventBossItemLootTable != null;
			}
		}

		public bool IsCampaignEvent
		{
			get
			{
				return EventBalancing != null && EventBalancing.EventMiniCampaignItemLootTable != null;
			}
		}

		public bool IsQualifiedForLeaderboard
		{
			get
			{
				return Data.CurrentScore != 0;
			}
		}

		public bool IsCheaterboard
		{
			get
			{
				return !string.IsNullOrEmpty(Data.LeaderboardId) && Data.LeaderboardId.Contains("cheater");
			}
		}

		[method: MethodImpl(32)]
		public event Action<EventManagerState, EventManagerState> StateChanged;

		[method: MethodImpl(32)]
		public event Action<List<LeaderboardScore>> ScoresUpdated;

		[method: MethodImpl(32)]
		public event Action<uint, uint> OwnScoreChanged;

		private void OnStateChanged(EventManagerState oldState, EventManagerState newState)
		{
			if (this.StateChanged != null)
			{
				this.StateChanged(oldState, newState);
			}
		}

		public void RegisterScoreChanged(uint oldScore, uint newScore)
		{
			if (this.OwnScoreChanged != null)
			{
				this.OwnScoreChanged(oldScore, newScore);
			}
			UpdateOwnScoreAndSubmit(newScore, DIContainerInfrastructure.GetCurrentPlayer());
			m_scoresChanged = true;
		}

		public EventManagerGameData SetInstancedData(EventManagerData instance)
		{
			Data = instance;
			return this;
		}

		public EventManagerGameData SetBalancingData(EventManagerBalancingData balancing)
		{
			if (balancing != null)
			{
				Balancing = balancing;
				EventBalancing = DIContainerBalancing.EventBalancingService.GetBalancingData<EventBalancingData>(balancing.EventId);
			}
			return this;
		}

		public EventManagerGameData CreateNewInstance()
		{
			Data = new EventManagerData
			{
				CurentEventInstance = null,
				CurrentScore = 0u,
				NameId = Balancing.NameId,
				CachedRolledResultWheelIndex = -1
			};
			return this;
		}

		public EventManagerGameData CreateFromInstance(EventManagerData instance)
		{
			if (DIContainerBalancing.EventBalancingService == null)
			{
				Debug.LogError("Tried to instantiate the PvP game data without working event balancing service!!");
				return null;
			}
			return SetInstancedData(instance).SetBalancingData(DIContainerBalancing.EventBalancingService.GetBalancingData<EventManagerBalancingData>(instance.NameId));
		}

		public EventManagerGameData CreateNewInstance(string nameId)
		{
			return SetBalancingData(DIContainerBalancing.EventBalancingService.GetBalancingData<EventManagerBalancingData>(nameId)).CreateNewInstance();
		}

		public void UpdateOwnScoreAndSubmit(uint score, PlayerGameData player)
		{
			DIContainerLogic.EventSystemService.SubmitEventScore(player, this);
		}

		public void UpdateOpponentScores(List<Leaderboard.Result> scores)
		{
			if (Data.CurrentOpponents == null)
			{
				Data.CurrentOpponents = new List<string>();
			}
			if (Data.CheatingOpponents == null)
			{
				Data.CheatingOpponents = new List<string>();
			}
			List<string> list = new List<string>(Data.CurrentOpponents);
			List<string> list2 = new List<string>();
			if (IsBossEvent && IsQualifiedForLeaderboard && string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentEventManager.LeaderboardId) && DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss != null && DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss.OwnTeamId > 0)
			{
				scores = DIContainerLogic.EventSystemService.GetTeamScores(scores);
			}
			DebugLog.Log(GetType(), "UpdateOpponentScores: #of scores= " + scores.Count);
			for (int i = 0; i < scores.Count; i++)
			{
				string accountId = scores[i].GetScore().GetAccountId();
				if (scores[i].GetScore().GetPoints() > 100000 && DIContainerLogic.EventSystemService.PreviousCurrentOpponents != null && !IsBossEvent)
				{
					DebugLog.Log(GetType(), "Player score is bigger than normal obtainable score, setting previous opponents instead");
					Data.CurrentOpponents = DIContainerLogic.EventSystemService.PreviousCurrentOpponents;
					DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
					DIContainerLogic.EventSystemService.PreviousCurrentOpponents = null;
					return;
				}
				if (scores[i].GetScore().GetPoints() > 100000000 && !Data.CheatingOpponents.Contains(accountId))
				{
					Data.CheatingOpponents.Add(accountId);
				}
				if ((int)Data.CurrentScore > 100000000 && !Data.CheatingOpponents.Contains(DIContainerInfrastructure.IdentityService.SharedId))
				{
					Data.CheatingOpponents.Add(DIContainerInfrastructure.IdentityService.SharedId);
				}
				if ((PublicOpponentDatas.ContainsKey(accountId) && PublicOpponentDatas[accountId].Level > DIContainerBalancing.Service.GetBalancingDataList<ExperienceLevelBalancingData>().Count + 1) || (PublicOpponentDatas.ContainsKey(accountId) && (int)PublicOpponentDatas[accountId].LastSaveTime < (int)Balancing.EventStartTimeStamp))
				{
					if (PublicOpponentDatas.ContainsKey(accountId))
					{
						PublicOpponentDatas.Remove(accountId);
					}
					if (Data.CurrentOpponents.Contains(accountId))
					{
						Data.CurrentOpponents.Remove(accountId);
						if (list.Contains(accountId))
						{
							list.Remove(accountId);
						}
					}
					if (ScoresByPlayer.ContainsKey(accountId))
					{
						ScoresByPlayer.Remove(accountId);
					}
					continue;
				}
				if (Data.CheatingOpponents != null && Data.CheatingOpponents.Contains(accountId) && !IsCheaterboard)
				{
					if (!(accountId == DIContainerInfrastructure.IdentityService.SharedId))
					{
						list2.Add(accountId);
						if (ScoresByPlayer.ContainsKey(accountId))
						{
							ScoresByPlayer[accountId] = new Leaderboard.Score(scores[i].GetScore().GetLevelName(), accountId);
						}
						else
						{
							ScoresByPlayer.Add(accountId, new Leaderboard.Score(scores[i].GetScore().GetLevelName(), accountId));
						}
					}
					continue;
				}
				list2.Add(accountId);
				if (ScoresByPlayer.ContainsKey(accountId))
				{
					ScoresByPlayer[accountId] = new Leaderboard.Score(scores[i].GetScore());
					DebugLog.Log("New Score of Player: " + ScoresByPlayer[accountId].GetAccountId() + " with score: " + scores[i].GetScore().GetPoints());
				}
				else if (Data.CurrentOpponents.Contains(accountId))
				{
					Leaderboard.Score value = new Leaderboard.Score(scores[i].GetScore());
					ScoresByPlayer.Add(accountId, value);
				}
				list.Remove(accountId);
			}
			foreach (string item in list)
			{
				if (!ScoresByPlayer.ContainsKey(item))
				{
					Leaderboard.Score value2 = new Leaderboard.Score(Balancing.NameId, item);
					ScoresByPlayer.Add(item, value2);
				}
			}
			DebugLog.Log(GetType(), "UpdateOpponentScores: #of total scores before boss-handling = " + ScoresByPlayer.Count);
			if (IsBossEvent)
			{
				if (IsQualifiedForLeaderboard && string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentEventManager.LeaderboardId))
				{
					DIContainerLogic.EventSystemService.UpdateBossLeaderboardEpicServer(list2);
				}
				RemoveEnemyTeamFromScores();
			}
			DebugLog.Log(GetType(), "UpdateOpponentScores: #of own-team scores AFTER boss-handling = " + ScoresByPlayer.Count);
			DebugLog.Log(GetType(), "UpdateOpponentScores: #of opponent-team scores AFTER boss-handling = " + ScoresByPlayerEnemyTeam.Count);
			if (DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").EnableFriendLeaderboards)
			{
				InitFriendLeaderboard();
			}
		}

		public void UpdateOpponents(Dictionary<string, PublicPlayerData> playerDatas)
		{
			PublicOpponentDatas = playerDatas;
			DebugLog.Log("Updated Opponent Datas!");
		}

		public void RemoveEnemyTeamFromScores()
		{
			DebugLog.Log("[BossSystem] Removing enemy team from own scores");
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			if (currentPlayer.Data.WorldBoss == null || currentPlayer.Data.WorldBoss.OwnTeamId == 0)
			{
				DebugLog.Log("[BossSystem] own boss not yet set up");
				return;
			}
			Dictionary<string, Leaderboard.Score> dictionary = new Dictionary<string, Leaderboard.Score>();
			Dictionary<string, Leaderboard.Score> dictionary2 = new Dictionary<string, Leaderboard.Score>();
			WorldBossTeamData worldBossTeamData = ((currentPlayer.Data.WorldBoss.OwnTeamId != 1) ? currentPlayer.Data.WorldBoss.Team2 : currentPlayer.Data.WorldBoss.Team1);
			if (worldBossTeamData == null || worldBossTeamData.TeamPlayerIds == null)
			{
				DebugLog.Error(GetType(), "RemvoeEnemyTeamFromScores: Team or teamPlayerIds == null!!! Team = " + worldBossTeamData);
				ScoresByPlayerEnemyTeam = ScoresByPlayerEnemyTeam ?? dictionary2;
				return;
			}
			foreach (string key in ScoresByPlayer.Keys)
			{
				if (worldBossTeamData.TeamPlayerIds.Contains(key))
				{
					DebugLog.Log("[BossSystem] found player in own team");
					dictionary.Add(key, ScoresByPlayer[key]);
				}
				else
				{
					DebugLog.Log("[BossSystem] found player in enemy team");
					dictionary2.Add(key, ScoresByPlayer[key]);
				}
			}
			ScoresByPlayer = dictionary;
			ScoresByPlayerEnemyTeam = dictionary2;
		}

		public List<Leaderboard.Score> GetRankedPlayers(bool alsoGetZero)
		{
			List<Leaderboard.Score> list = new List<Leaderboard.Score>(ScoresByPlayer.Values).FindAll((Leaderboard.Score s) => alsoGetZero || s.GetPoints() > 0);
			DebugLog.Log(GetType(), "[EPIC SERVER] --- GetRankedPlayers scores count: " + list.Count);
			list = Enumerable.ToList(list.OrderByDescending((Leaderboard.Score s) => s.GetPoints()));
			if (Data.CurrentScore != 0 || alsoGetZero)
			{
				// Leaderboard.Score score = new Leaderboard.Score("current", "current");
				// if (!IsCheaterboard && Data.CheatingOpponents != null && Data.CheatingOpponents.Contains(DIContainerInfrastructure.IdentityService.SharedId))
				// {
				// 	score.SetPoints(0L);
				// 	list.Add(score);
				// }
				// else
				// {
				// 	score.SetPoints(Data.CurrentScore);
				// 	list.Insert(GetCurrentRank - 1, score);
				// }
			}
			return list;
		}

		public List<Leaderboard.Score> GetRankedPlayersEnemys(bool alsoGetZero)
		{
			List<Leaderboard.Score> source = Enumerable.ToList(new List<Leaderboard.Score>(ScoresByPlayerEnemyTeam.Values).Where((Leaderboard.Score s) => alsoGetZero || s.GetPoints() > 0));
			return Enumerable.ToList(source.OrderByDescending((Leaderboard.Score s) => s.GetPoints()));
		}

		public void InitFriendLeaderboard()
		{
			m_sortedFriends = new Dictionary<int, FriendGameData>();
			m_friendScores = new Dictionary<string, int>();
			m_serverScores = new Dictionary<string, int>();
			m_friendsDictionary = new Dictionary<string, FriendGameData>();
			foreach (KeyValuePair<string, FriendGameData> friend in DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends)
			{
				m_friendsDictionary.Add(friend.Key, friend.Value);
			}
			m_friends = Enumerable.ToList(m_friendsDictionary.Values);
			GetFriendScoresByStepsOfTen();
		}

		public Dictionary<string, int> GetFriendScoresById()
		{
			return m_friendScores;
		}

		public Dictionary<int, FriendGameData> GetFriendScoresByRank()
		{
			return m_sortedFriends;
		}

		private void GetFriendScoresByStepsOfTen()
		{
			if (m_friends == null)
			{
				m_friendsDictionary = new Dictionary<string, FriendGameData>();
				foreach (KeyValuePair<string, FriendGameData> friend in DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends)
				{
					m_friendsDictionary.Add(friend.Key, friend.Value);
				}
				m_friends = Enumerable.ToList(m_friendsDictionary.Values);
			}
			if (m_friends.Count <= 0)
			{
				CalculateServerScores();
				return;
			}
			List<string> list = new List<string>();
			for (int i = 0; i < 10; i++)
			{
				if (m_friends.Count <= 0)
				{
					break;
				}
				FriendGameData friendGameData = m_friends.FirstOrDefault();
				m_friends.Remove(friendGameData);
				if (!friendGameData.isNpcFriend)
				{
					list.Add(friendGameData.FriendId);
				}
			}
			if (list.Count > 0)
			{
				DIContainerInfrastructure.ScoringService.FetchScores(list.ToArray(), DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Balancing.NameId, OnFetchScoresSuccess, OnFetchScoresError);
			}
			else
			{
				DebugLog.Log(GetType(), "not fetching score for friends because there are none");
			}
		}

		private void OnFetchScoresError(int errorCode)
		{
			DebugLog.Log("Error fetching Scores from server: " + errorCode);
			GetFriendScoresByStepsOfTen();
		}

		private void OnFetchScoresSuccess(Dictionary<string, int> scores)
		{
			foreach (KeyValuePair<string, int> score in scores)
			{
				m_serverScores.SaveAdd(score.Key, score.Value);
			}
			DebugLog.Log("[LeaderboardUI] success fetching scores from server: " + scores.Count + " scores found");
			GetFriendScoresByStepsOfTen();
		}

		private void CalculateServerScores()
		{
			SortedDictionary<float, List<string>> sortedDictionary = new SortedDictionary<float, List<string>>();
			foreach (KeyValuePair<string, int> serverScore in m_serverScores)
			{
				if (m_friendsDictionary.ContainsKey(serverScore.Key))
				{
					DebugLog.Log("[LeaderboardUI] processing score: " + serverScore.Key + "   " + serverScore.Value);
					m_friendScores.SaveAdd(serverScore.Key, serverScore.Value);
					if (!sortedDictionary.ContainsKey(serverScore.Value))
					{
						sortedDictionary.Add(serverScore.Value, new List<string>());
					}
					sortedDictionary[serverScore.Value].Add(serverScore.Key);
				}
			}
			AddSelfToFriendList(sortedDictionary);
			int num = m_friendScores.Count + 1;
			foreach (List<string> value in sortedDictionary.Values)
			{
				foreach (string item in value)
				{
					num--;
					if (item != "current")
					{
						DebugLog.Log("[LeaderboardUI] CalculateServerScores: adding friend to m_sortedFriends: " + item);
						m_sortedFriends.SaveAdd(num, m_friendsDictionary[item]);
					}
					else
					{
						DebugLog.Log("[LeaderboardUI] CalculateServerScores: adding 'null' to m_sortedFriends because this is us: " + item);
						m_sortedFriends.SaveAdd(num, null);
					}
				}
			}
		}

		private void AddSelfToFriendList(SortedDictionary<float, List<string>> friendList)
		{
			List<Leaderboard.Score> rankedPlayers = GetRankedPlayers(true);
			for (int i = 0; i < rankedPlayers.Count; i++)
			{
				Leaderboard.Score score = rankedPlayers[i];
				if (score.GetAccountId() == "current")
				{
					if (!friendList.ContainsKey(score.GetPoints()))
					{
						friendList.Add(score.GetPoints(), new List<string>());
					}
					friendList[score.GetPoints()].Add(score.GetAccountId());
				}
			}
		}

		public CollectionGroupBalancingData GetCollectionBalancing()
		{
			if (IsCampaignEvent && m_CurrentMiniCampaign != null)
			{
				return m_CurrentMiniCampaign.CollectionGroupBalancing;
			}
			if (IsBossEvent && m_CurrentEventBoss != null)
			{
				return m_CurrentEventBoss.CollectionGroupBalancing;
			}
			return null;
		}

		public string GetScalingRankRewardLootTable()
		{
			return GetScalingRankRewardLootTable(GetCurrentRank);
		}

		public string GetScalingRankRewardLootTable(int rank)
		{
			if (rank > EventBalancing.EventBonusLootTablesPerRank.Count)
			{
				return null;
			}
			string text = EventBalancing.EventBonusLootTablesPerRank[rank - 1];
			int num = 0;
			int num2 = ((Data.StartingPlayerLevel != 0) ? Data.StartingPlayerLevel : DIContainerInfrastructure.GetCurrentPlayer().Data.Level);
			ExperienceLevelBalancingData balancing;
			if (DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + num2.ToString("00"), out balancing) || DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + (num2 - 1).ToString("00"), out balancing))
			{
				num = balancing.MatchmakingRangeIndex;
			}
			return text.Replace("{levelrange}", num.ToString("00"));
		}
	}
}

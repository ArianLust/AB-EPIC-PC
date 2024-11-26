using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using Rcs;
using UnityEngine;

namespace ABH.GameDatas
{
	public class PvPSeasonManagerGameData
	{
		public PvPTurnManagerGameData CurrentSeasonTurn;

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

		public PvPSeasonManagerData Data { get; private set; }

		public int LastLeagueChangeByRanking { get; set; }

		public int LastLeagueChangeByTime { get; set; }

		public PvPSeasonManagerBalancingData Balancing { get; private set; }

		public PvPSeasonState CurrentPvPSeasonState
		{
			get
			{
				return Data.CurrentSeasonState;
			}
			set
			{
				PvPSeasonState currentSeasonState = Data.CurrentSeasonState;
				Data.CurrentSeasonState = value;
				OnStateChanged(currentSeasonState, Data.CurrentSeasonState);
			}
		}

		public bool IsQualifiedForLeaderboard
		{
			get
			{
				return CurrentSeasonTurn != null && CurrentSeasonTurn.Data.CurrentScore != 0 && CurrentSeasonTurn.Data.CurrentSeason == DIContainerLogic.PvPSeasonService.GetCurrentSeasonTurn(Balancing);
			}
		}

		[method: MethodImpl(32)]
		public event Action<PvPSeasonState, PvPSeasonState> StateChanged;

		private void OnStateChanged(PvPSeasonState oldState, PvPSeasonState newState)
		{
			if (this.StateChanged != null)
			{
				this.StateChanged(oldState, newState);
			}
		}

		public PvPSeasonManagerGameData SetInstancedData(PvPSeasonManagerData instance)
		{
			Data = instance;
			return this;
		}

		public PvPSeasonManagerGameData SetBalancingData(PvPSeasonManagerBalancingData balancing)
		{
			Balancing = balancing;
			return this;
		}

		public PvPSeasonManagerGameData CreateNewInstance()
		{
			Data = new PvPSeasonManagerData
			{
				NameId = Balancing.NameId,
				CurrentLeague = 1,
				CurrentSeason = 1
			};
			return this;
		}

		public PvPSeasonManagerGameData CreateFromInstance(PvPSeasonManagerData instance)
		{
			if (DIContainerBalancing.EventBalancingService == null)
			{
				Debug.LogError("Tried to instantiate the PvP game data without working event balancing service!!");
				return null;
			}
			PvPSeasonManagerBalancingData balancing = null;
			if (DIContainerBalancing.EventBalancingService.TryGetBalancingData<PvPSeasonManagerBalancingData>(instance.NameId, out balancing))
			{
				SetInstancedData(instance).SetBalancingData(balancing);
				if (instance.CurrentSeasonTurn != null)
				{
					CurrentSeasonTurn = new PvPTurnManagerGameData().CreateFromInstance(instance.CurrentSeasonTurn, this);
				}
				return this;
			}
			return null;
		}

		public void SetCurrentSeasonTurn()
		{
			CurrentSeasonTurn = new PvPTurnManagerGameData().CreateNewInstance(this);
			int currentSeasonTurn = DIContainerLogic.PvPSeasonService.GetCurrentSeasonTurn(Balancing);
			Data.CurrentSeason = currentSeasonTurn;
			CurrentSeasonTurn.Data.NameId = Balancing.NameId + "_turn_" + Data.CurrentSeason.ToString("00");
			Data.CurrentSeasonTurn = CurrentSeasonTurn.Data;
			CurrentSeasonTurn.Data.CurrentSeason = currentSeasonTurn;
		}

		public PvPSeasonManagerGameData CreateNewInstance(string nameId)
		{
			return SetBalancingData(DIContainerBalancing.EventBalancingService.GetBalancingData<PvPSeasonManagerBalancingData>(nameId)).CreateNewInstance();
		}

		public string GetSeasonTurnLootTableWheel()
		{
			string empty = string.Empty;
			if (Balancing.PvPRewardLootTablesPerLeague == null)
			{
				return string.Empty;
			}
			if (Balancing.PvPRewardLootTablesPerLeague.Count < Data.CurrentLeague)
			{
				return Balancing.PvPRewardLootTablesPerLeague.LastOrDefault();
			}
			return Balancing.PvPRewardLootTablesPerLeague[Data.CurrentLeague - 1];
		}

		public Requirement GetRerollResultRequirement()
		{
			Requirement result = new Requirement();
			if (Balancing.RerollResultRequirement == null)
			{
				return result;
			}
			if (Balancing.RerollResultRequirement.Count < Data.CurrentLeague)
			{
				return Balancing.RerollResultRequirement.LastOrDefault();
			}
			return Balancing.RerollResultRequirement[Data.CurrentLeague - 1];
		}

		public static string GetLeagueAssetName(int league)
		{
			switch (league)
			{
			case 0:
				return "LeagueCrown_Wood";
			case 1:
				return "LeagueCrown_Wood";
			case 2:
				return "LeagueCrown_Stone";
			case 3:
				return "LeagueCrown_Silver";
			case 4:
				return "LeagueCrown_Gold";
			case 5:
				return "LeagueCrown_Platinum";
			case 6:
				return "LeagueCrown_Diamond";
			default:
				return string.Empty;
			}
		}

		public void InitFriendLeaderboard()
		{
			m_friendScores = new Dictionary<string, int>();
			m_serverScores = new Dictionary<string, int>();
			m_sortedFriends = new Dictionary<int, FriendGameData>();
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
			DebugLog.Log("[FriendLeaderboard] getting friends by steps of Ten");
			if (m_friends == null)
			{
				m_friendsDictionary = new Dictionary<string, FriendGameData>();
				foreach (KeyValuePair<string, FriendGameData> friend in DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends)
				{
					DebugLog.Log("[FriendLeaderboard] Adding this social friend: " + friend.Key + "   " + friend.Value.FriendName);
					m_friendsDictionary.Add(friend.Key, friend.Value);
				}
				m_friends = Enumerable.ToList(m_friendsDictionary.Values);
			}
			if (m_friends.Count <= 0)
			{
				DebugLog.Log("[FriendLeaderboard] friends is now empty, calculating server scores ?!?");
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
					DebugLog.Log("[FriendLeaderboard] Adding this friend to local list: " + friendGameData.FriendId);
					list.Add(friendGameData.FriendId);
				}
			}
			if (list.Count > 0)
			{
				DIContainerInfrastructure.ScoringService.FetchScores(list.ToArray(), DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData.CurrentSeasonTurn.Data.NameId, OnFetchFriendScoresSuccess, OnFetchFriendsScoresError);
			}
			else
			{
				DebugLog.Log(GetType(), "not fetching score for friends because there are none");
			}
		}

		private void OnFetchFriendsScoresError(int errorCode)
		{
			DebugLog.Error("Error fetching Scores from server: " + errorCode);
			GetFriendScoresByStepsOfTen();
		}

		private void OnFetchFriendScoresSuccess(Dictionary<string, int> scores)
		{
			foreach (KeyValuePair<string, int> score in scores)
			{
				if (m_serverScores.ContainsKey(score.Key))
				{
					m_serverScores[score.Key] = score.Value;
				}
				else
				{
					m_serverScores.Add(score.Key, score.Value);
				}
			}
			DebugLog.Log("[FriendLeaderboard] success fetching scores from server: " + scores.Count + " scores found");
			GetFriendScoresByStepsOfTen();
		}

		private void CalculateServerScores()
		{
			DebugLog.Log("[FriendLeaderboard] calculating server scores... " + m_serverScores.Count);
			SortedDictionary<float, List<string>> sortedDictionary = new SortedDictionary<float, List<string>>();
			foreach (KeyValuePair<string, int> serverScore in m_serverScores)
			{
				if (!m_friendsDictionary.ContainsKey(serverScore.Key))
				{
					DebugLog.Log("[FriendLeaderboard] friends dic doesnt know user: " + serverScore.Key);
					continue;
				}
				DebugLog.Log("[FriendLeaderboard] processing score: " + serverScore.Key + "   " + serverScore.Value);
				m_friendScores.Add(serverScore.Key, serverScore.Value);
				if (!sortedDictionary.ContainsKey(serverScore.Value))
				{
					sortedDictionary.Add(serverScore.Value, new List<string>());
				}
				sortedDictionary[serverScore.Value].Add(serverScore.Key);
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
						m_sortedFriends.Add(num, m_friendsDictionary[item]);
					}
					else
					{
						m_sortedFriends.Add(num, null);
					}
				}
			}
		}

		private void AddSelfToFriendList(SortedDictionary<float, List<string>> friendList)
		{
			List<Leaderboard.Score> rankedPlayers = CurrentSeasonTurn.GetRankedPlayers(true);
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
	}
}

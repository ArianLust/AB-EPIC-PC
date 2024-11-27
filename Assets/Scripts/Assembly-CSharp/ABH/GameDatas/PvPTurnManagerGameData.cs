using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using Interfaces.GameClient;
using Rcs;

namespace ABH.GameDatas
{
	public class PvPTurnManagerGameData
	{
		public List<int> CurrentBirdIndices = new List<int>();

		public Dictionary<string, Leaderboard.Score> ScoresByPlayer = new Dictionary<string, Leaderboard.Score>();

		public Dictionary<string, PublicPlayerData> PublicOpponentDatas = new Dictionary<string, PublicPlayerData>();

		public Dictionary<string, LootInfoData> RolledResultLoot = new Dictionary<string, LootInfoData>();

		public Dictionary<string, LootInfoData> FinalRankBonusLoot = new Dictionary<string, LootInfoData>();

		private bool m_scoresChanged = true;

		private int m_rank = 15;

		public PublicPlayerData CurrentPvPOpponent { get; set; }

		public string FallbackOpponentReason { get; set; }

		public DateTime LastOpponentUpdateTime { get; set; }

		public bool IsValid
		{
			get
			{
				return Data != null;
			}
		}

		public int FailedOnlineMatchmakeCount { get; set; }

		public PvPTurnManagerData Data { get; private set; }

		public PvPSeasonManagerGameData SeasonGameData { get; private set; }

		public EventManagerState CurrentPvPTurnManagerState
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
				if (SeasonGameData.Balancing.StarRatingForRanking.ContainsKey(ResultRank))
				{
					return SeasonGameData.Balancing.StarRatingForRanking[ResultRank];
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
				m_rank = ScoresByPlayer.Values.Count((Leaderboard.Score s) => s.GetPoints() > Data.CurrentScore) + 1;
				return m_rank;
			}
		}

		public bool IsAssetValid { get; set; }

		public bool IsCheaterboard
		{
			get
			{
				return !string.IsNullOrEmpty(Data.LeaderboardId) && Data.LeaderboardId.Contains("cheater");
			}
		}

		public bool IsLegacyLeaderboard
		{
			get
			{
				return Data.CurrentOpponents != null && Data.CurrentOpponents.Count > 1 && string.IsNullOrEmpty(Data.LeaderboardId);
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

		public PvPTurnManagerGameData SetInstancedData(PvPTurnManagerData instance)
		{
			Data = instance;
			return this;
		}

		public PvPTurnManagerGameData SetSeasonGameData(PvPSeasonManagerGameData season)
		{
			SeasonGameData = season;
			return this;
		}

		public PvPTurnManagerGameData CreateNewInstance(PvPSeasonManagerGameData seasonManagerGameData)
		{
			SetSeasonGameData(seasonManagerGameData);
			Data = new PvPTurnManagerData
			{
				CurrentScore = 0u,
				NameId = seasonManagerGameData.Balancing.NameId + seasonManagerGameData.Data.CurrentSeason.ToString("00"),
				CachedRolledResultWheelIndex = -1
			};
			return this;
		}

		public PvPTurnManagerGameData CreateFromInstance(PvPTurnManagerData instance, PvPSeasonManagerGameData season)
		{
			return SetInstancedData(instance).SetSeasonGameData(season);
		}

		public void UpdateOwnScoreAndSubmit(uint score, PlayerGameData player)
		{
			DIContainerLogic.PvPSeasonService.SubmitPvPTurnScore(player, SeasonGameData);
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
			for (int i = 0; i < scores.Count; i++)
			{
				string accountId = scores[i].GetScore().GetAccountId();
				if (scores[i].GetScore().GetPoints() > 100000000 && !Data.CheatingOpponents.Contains(accountId))
				{
					Data.CheatingOpponents.Add(accountId);
				}
				if (Data.CurrentScore > 100000000 && !Data.CheatingOpponents.Contains(DIContainerInfrastructure.IdentityService.SharedId))
				{
					Data.CheatingOpponents.Add(DIContainerInfrastructure.IdentityService.SharedId);
				}
				if ((PublicOpponentDatas.ContainsKey(accountId) && PublicOpponentDatas[accountId].Level > DIContainerBalancing.Service.GetBalancingDataList<ExperienceLevelBalancingData>().Count + 1) || (PublicOpponentDatas.ContainsKey(accountId) && (int)PublicOpponentDatas[accountId].LastSaveTime < (int)DIContainerLogic.PvPSeasonService.GetPvPTurnStartTime(DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData)))
				{
					RemovePublicOpponentByAccountId(accountId);
				}
				else if (Data.CheatingOpponents != null && Data.CheatingOpponents.Contains(accountId) && !IsCheaterboard)
				{
					if (accountId == DIContainerInfrastructure.IdentityService.SharedId)
					{
						if (ScoresByPlayer.ContainsKey(accountId))
						{
							ScoresByPlayer[accountId] = new Leaderboard.Score(Data.NameId, accountId);
						}
						else
						{
							ScoresByPlayer.Add(accountId, new Leaderboard.Score(Data.NameId, accountId));
						}
					}
				}
				else if (ScoresByPlayer.ContainsKey(accountId))
				{
					ScoresByPlayer[accountId] = new Leaderboard.Score(scores[i].GetScore());
				}
				else if (Data.CurrentOpponents.Contains(accountId))
				{
					ScoresByPlayer.Add(accountId, new Leaderboard.Score(scores[i].GetScore()));
				}
			}
			if (DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").EnableFriendLeaderboards)
			{
				SeasonGameData.InitFriendLeaderboard();
			}
		}

		public void UpdateOpponents(Dictionary<string, PublicPlayerData> playerDatas)
		{
			PublicOpponentDatas = playerDatas;
			DebugLog.Log("Updated Opponent Datas!");
		}

		public List<Leaderboard.Score> GetRankedPlayers(bool alsoGetZero)
		{
			List<Leaderboard.Score> source = Enumerable.ToList(new List<Leaderboard.Score>(ScoresByPlayer.Values).Where((Leaderboard.Score s) => alsoGetZero || s.GetPoints() > 0));
			source = Enumerable.ToList(source.OrderByDescending((Leaderboard.Score s) => s.GetPoints()));
			if (Data.CurrentScore != 0 || alsoGetZero)
			{
				//Leaderboard.Score score = new Leaderboard.Score("current", "current");
				if (!IsCheaterboard && Data.CheatingOpponents != null && Data.CheatingOpponents.Contains(DIContainerInfrastructure.IdentityService.SharedId))
				{
					// score.SetPoints(0L);
					// source.Add(score);
				}
				else
				{
					// score.SetPoints(Data.CurrentScore);
					// source.Insert(GetCurrentRank - 1, score);
				}
			}
			return source;
		}

		public string GetScalingRankRewardLootTable()
		{
			return GetScalingRankRewardLootTable(GetCurrentRank);
		}

		public string GetScalingRankRewardLootTable(int rank)
		{
			if (SeasonGameData == null || rank > SeasonGameData.Balancing.PvPBonusLootTablesPerRank.Count)
			{
				return null;
			}
			string text = SeasonGameData.Balancing.PvPBonusLootTablesPerRank[rank - 1];
			int num = 0;
			int num2 = ((Data.StartingPlayerLevel != 0) ? Data.StartingPlayerLevel : DIContainerInfrastructure.GetCurrentPlayer().Data.Level);
			ExperienceLevelBalancingData balancing;
			if (DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + num2.ToString("00"), out balancing) || DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + (num2 - 1).ToString("00"), out balancing))
			{
				num = balancing.MatchmakingRangeIndex;
			}
			return text.Replace("{levelrange}", num.ToString("00"));
		}

		private void RemovePublicOpponentByAccountId(string accountId)
		{
			if (PublicOpponentDatas.ContainsKey(accountId))
			{
				PublicOpponentDatas.Remove(accountId);
			}
			if (Data.CurrentOpponents.Contains(accountId))
			{
				Data.CurrentOpponents.Remove(accountId);
			}
			if (ScoresByPlayer.ContainsKey(accountId))
			{
				ScoresByPlayer.Remove(accountId);
			}
		}
	}
}

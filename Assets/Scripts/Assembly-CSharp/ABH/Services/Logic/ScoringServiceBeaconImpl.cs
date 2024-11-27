using System;
using System.Collections.Generic;
using ABH.Services.Logic.Interfaces;
using Rcs;

namespace ABH.Services.Logic
{
	public class ScoringServiceBeaconImpl : IScoringService
	{
		private Action<string> DebugLog;

		private Action<string> ErrorLog;

		private Leaderboard m_LeaderBoard;

		public ScoringServiceBeaconImpl()
		{
			m_LeaderBoard = new Leaderboard(ContentLoader.Instance.m_BeaconConnectionMgr.Identiy);
		}

		public IScoringService SetDebugLog(Action<string> debugLog)
		{
			DebugLog = debugLog;
			return this;
		}

		public IScoringService SetErrorLog(Action<string> errorLog)
		{
			ErrorLog = errorLog;
			return this;
		}

		private void LogDebug(string message)
		{
			if (DebugLog != null)
			{
				DebugLog(message);
			}
		}

		private void LogError(string message)
		{
			if (ErrorLog != null)
			{
				ErrorLog(message);
			}
		}

		public void SubmitScore(Leaderboard.Score score, Leaderboard.ScoreSubmittedCallback onSuccess, Leaderboard.ErrorCallback onError)
		{
			LogDebug("SubmitScore called");
			m_LeaderBoard.SubmitScore(score, onSuccess, onError);
		}

		public void SubmitScore(string boardName, long score, Action onSuccess, Action<int> onError)
		{
			Leaderboard.Score score2 = new Leaderboard.Score(boardName);
			score2.SetPoints(score);
			score2.SetProperty("player_name", DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.MatchmakingPlayerName);
			m_LeaderBoard.SubmitScore(score2, delegate
			{
				onSuccess();
			}, delegate(Leaderboard.ErrorCode errorCode)
			{
				onError((int)errorCode);
			});
		}

		public void Matchmake(string level, int offset, uint limit, Leaderboard.ScoresFetchedCallback onSuccess, Leaderboard.ErrorCallback onError)
		{
			LogDebug("Matchmake called");
			m_LeaderBoard.Matchmake(level, offset, limit, onSuccess, onError);
		}

		public void FetchScore(string level, Leaderboard.ScoreFetchedCallback onSuccess, Leaderboard.ErrorCallback onError)
		{
			LogDebug("FetchScore called");
			m_LeaderBoard.FetchScore(level, onSuccess, onError);
		}

		public void FetchScore(string boardName, Action<int> onSuccess, Action<int> onError)
		{
			m_LeaderBoard.FetchScore(boardName, delegate(Leaderboard.Result rcsResult)
			{
				onSuccess((int)rcsResult.GetScore().GetPoints());
			}, delegate(Leaderboard.ErrorCode errorCode)
			{
				onError((int)errorCode);
			});
		}

		public void FetchScores(string[] accountIds, string boardName, Action<Dictionary<string, int>> onSuccess, Action<int> onError)
		{
			m_LeaderBoard.FetchScores(new List<string>(accountIds), boardName, delegate(List<Leaderboard.Result> lbResults)
			{
				DebugLog(string.Concat(GetType(), ": FetchScores: Successfully fetched scores. Now copying values..."));
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (Leaderboard.Result lbResult in lbResults)
				{
					Leaderboard.Result result = new Leaderboard.Result(lbResult);
					DebugLog("adding " + result.GetScore().GetAccountId() + " with score " + (int)result.GetScore().GetPoints());
					dictionary.SaveAdd(result.GetScore().GetAccountId(), (int)result.GetScore().GetPoints());
				}
				onSuccess(dictionary);
			}, delegate(Leaderboard.ErrorCode errorCode)
			{
				onError((int)errorCode);
			});
		}
	}
}

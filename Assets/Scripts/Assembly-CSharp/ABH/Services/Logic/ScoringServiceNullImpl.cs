using System;
using System.Collections.Generic;
using ABH.Services.Logic.Interfaces;
using Rcs;

namespace ABH.Services.Logic
{
	public class ScoringServiceNullImpl : IScoringService
	{
		private Action<string> DebugLog;

		private Action<string> ErrorLog;

		public int m_ScoreFactor = 1;

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
			onSuccess();
		}

		public void SubmitScore(string boardName, long score, Action onSuccess, Action<int> onError)
		{
			onSuccess();
		}

		public void Matchmake(string level, int offset, uint limit, Leaderboard.ScoresFetchedCallback onSuccess, Leaderboard.ErrorCallback onError)
		{
			if (DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData != null && DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Data.CurrentScore != 0)
			{
				onSuccess(GetHatchDebugScores(level, limit));
			}
		}

		private List<Leaderboard.Result> GetHatchDebugScores(string level, uint limit)
		{
			List<Leaderboard.Result> list = new List<Leaderboard.Result>();
			for (int i = 0; i < limit; i++)
			{
				list.Add(new Leaderboard.Result(i + 1, new Leaderboard.Score(level, "debug_" + i.ToString("00"))));
				list[i].GetScore().SetPoints((i + 1) * 100);
			}
			return list;
		}

		private Dictionary<string, int> GetDebugScores(string boardName, int limit)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			for (int i = 0; i < limit; i++)
			{
				dictionary.Add("debug_" + i.ToString("00"), i + 100 * m_ScoreFactor);
			}
			return dictionary;
		}

		public void FetchScore(string level, Leaderboard.ScoreFetchedCallback onSuccess, Leaderboard.ErrorCallback onError)
		{
		}

		public void FetchScore(string boardName, Action<int> onSuccess, Action<int> onError)
		{
		}

		public void FetchScores(string[] accountIds, string level, Leaderboard.ScoresFetchedCallback onSuccess, Leaderboard.ErrorCallback onError)
		{
			onSuccess(GetHatchDebugScores(level, (uint)accountIds.Length));
		}

		public void FetchScores(string[] accountIds, string boardName, Action<Dictionary<string, int>> onSuccess, Action<int> onError)
		{
			onSuccess(GetDebugScores(boardName, accountIds.Length));
		}
	}
}

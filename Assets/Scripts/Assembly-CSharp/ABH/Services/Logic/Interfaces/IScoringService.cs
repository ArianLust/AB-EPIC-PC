using System;
using System.Collections.Generic;
using Rcs;

namespace ABH.Services.Logic.Interfaces
{
	public interface IScoringService
	{
		IScoringService SetDebugLog(Action<string> debugLog);

		IScoringService SetErrorLog(Action<string> errorLog);

		void SubmitScore(string boardName, long score, Action onSuccess, Action<int> onError);

		void Matchmake(string level, int offset, uint limit, Leaderboard.ScoresFetchedCallback onSuccess, Leaderboard.ErrorCallback onError);

		void FetchScore(string boardName, Action<int> onSuccess, Action<int> onError);

		void FetchScores(string[] accountIds, string boardName, Action<Dictionary<string, int>> onSuccess, Action<int> onError);
	}
}

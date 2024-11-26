using System;
using System.Collections.Generic;
using Rcs;

namespace ABH.Services.Logic.Interfaces
{
	public interface IMatchmakingService
	{
		IMatchmakingService SetDebugLog(Action<string> debugLog);

		IMatchmakingService SetErrorLog(Action<string> errorLog);

		void JoinLobby(string lobbyId, int lobbyWaitTimeoutInSeconds, int amountOfPlayers, OnlineMatchmaker.JoinLobbyCallback callback);

		void LeaveLobby(string lobbyId, OnlineMatchmaker.LeaveLobbyCallback callback);

		void FetchLobbies(OnlineMatchmaker.FetchLobbiesCallback callback);

		void SetOfflineAttributes(Dictionary<string, object> attributes, OfflineMatchmaker.SetAttributesCallback callback);

		void MatchOfflineUsers(string matchingFunctionName, Dictionary<string, object> functionArguments, OfflineMatchmaker.MatchUsersCallback callback, int maxResults = 20);

		void MatchUsingPlayerScore(string boardName, int matchmakingScore, Action<Dictionary<string, int>> onSuccess, Action<int> onError, int numberOfPlayers = 15);
	}
}

using System;
using System.Collections.Generic;
using ABH.Services.Logic.Interfaces;
using ABH.Shared.Generic;
using Rcs;

namespace ABH.Services.Logic
{
	public class MatchmakingServiceNullImpl : IMatchmakingService
	{
		private Action<string> DebugLog;

		private Action<string> ErrorLog;

		public IMatchmakingService SetDebugLog(Action<string> debugLog)
		{
			DebugLog = debugLog;
			return this;
		}

		public IMatchmakingService SetErrorLog(Action<string> errorLog)
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

		public void JoinLobby(string lobbyId, int lobbyWaitTimeoutInSeconds, int amountOfPlayers, OnlineMatchmaker.JoinLobbyCallback callback)
		{
			LogDebug("JoinLobby called");
			OnlineMatchmaker.Response response = new OnlineMatchmaker.Response();
			response.Message = string.Empty;
			response.Result = OnlineMatchmaker.Response.ResultType.Success;
			callback(response, new List<string>(GetDebugAccounts(amountOfPlayers)), string.Empty);
		}

		public void JoinLobby(string lobbyId, int lobbyWaitTimeoutInSeconds, int amountOfPlayers, Action<RESTResultEnum, List<string>> callback)
		{
			LogDebug("JoinLobby called");
			List<string> debugAccounts = GetDebugAccounts(amountOfPlayers);
			callback(RESTResultEnum.Success, debugAccounts);
		}

		public void LeaveLobby(string lobbyId, OnlineMatchmaker.LeaveLobbyCallback callback)
		{
			LogDebug("LeaveLobby called");
		}

		public void LeaveLobby(string lobbyId, Action<RESTResultEnum> callback)
		{
			LogDebug("LeaveLobby called");
		}

		public void FetchLobbies(OnlineMatchmaker.FetchLobbiesCallback callback)
		{
			LogDebug("FetchLobbies called");
		}

		public void FetchLobbies(Action<RESTResultEnum, List<string>> callback)
		{
			LogDebug("FetchLobbies called");
		}

		public void SetOfflineAttributes(Dictionary<string, object> attributes, OfflineMatchmaker.SetAttributesCallback callback)
		{
			LogDebug("SetOfflineAttributes called");
		}

		public void SetOfflineAttributes(Dictionary<string, object> attributes, Action<RESTResultEnum> callback)
		{
			LogDebug("SetOfflineAttributes called");
		}

		public void MatchOfflineUsers(string matchingFunctionName, Dictionary<string, object> functionArguments, OfflineMatchmaker.MatchUsersCallback callback, int maxResults = 20)
		{
			LogDebug("MatchOfflineUsers called");
			callback(OfflineMatchmaker.ResultCode.Success, new List<string>(GetDebugAccounts(maxResults)));
		}

		public void MatchOfflineUsers(string matchingFunctionName, Dictionary<string, object> functionArguments, Action<RESTResultEnum, List<string>> callback, int maxResults = 20)
		{
			LogDebug("MatchOfflineUsers called");
			callback(RESTResultEnum.Success, GetDebugAccounts(maxResults));
		}

		public void MatchUsingPlayerScore(string boardName, int matchmakingScore, Action<Dictionary<string, int>> onSuccess, Action<int> onError, int numberOfPlayers = 15)
		{
			LogDebug("MatchUsingPlayerScore called! TODO: debug result!");
			onError(0);
		}

		private List<string> GetDebugAccounts(int limit)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < limit; i++)
			{
				list.Add("debug_" + i.ToString("00"));
			}
			return list;
		}
	}
}

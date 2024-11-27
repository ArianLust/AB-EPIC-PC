using System;
using System.Collections.Generic;
using ABH.Services.Logic.Interfaces;
using Rcs;

namespace ABH.Services.Logic
{
	public class MatchmakingService : IMatchmakingService
	{
		private Action<string> DebugLog;

		private Action<string> ErrorLog;

		private OnlineMatchmaker m_onlineMatchmaker;

		private OfflineMatchmaker m_offlineMatchmaker;

		public MatchmakingService()
		{
			m_onlineMatchmaker = new OnlineMatchmaker(ContentLoader.Instance.m_BeaconConnectionMgr.Identiy);
			m_offlineMatchmaker = new OfflineMatchmaker(ContentLoader.Instance.m_BeaconConnectionMgr.Identiy);
		}

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
			m_onlineMatchmaker.JoinLobby(lobbyId, (ulong)lobbyWaitTimeoutInSeconds, callback);
		}

		public void LeaveLobby(string lobbyId, OnlineMatchmaker.LeaveLobbyCallback callback)
		{
			LogDebug("LeaveLobby called");
			m_onlineMatchmaker.LeaveLobby(lobbyId, callback);
		}

		public void FetchLobbies(OnlineMatchmaker.FetchLobbiesCallback callback)
		{
			LogDebug("FetchLobbies called");
			m_onlineMatchmaker.FetchLobbies(callback);
		}

		public void SetOfflineAttributes(Dictionary<string, object> attributes, OfflineMatchmaker.SetAttributesCallback callback)
		{
			LogDebug("SetOfflineAttributes called");
			m_offlineMatchmaker.SetAttributes(HatchHelper.ConvertToVariantDic(attributes), callback);
		}

		public void MatchOfflineUsers(string matchingFunctionName, Dictionary<string, object> functionArguments, OfflineMatchmaker.MatchUsersCallback callback, int maxResults = 20)
		{
			LogDebug("MatchOfflineUsers called");
			m_offlineMatchmaker.MatchUsers(matchingFunctionName, HatchHelper.ConvertToVariantDic(functionArguments), callback, maxResults);
		}

		public void MatchUsingPlayerScore(string boardName, int matchmakingScore, Action<Dictionary<string, int>> onSuccess, Action<int> onError, int numberOfPlayers = 15)
		{
			LogDebug("MatchUsingPlayerScore: TODO!");
		}
	}
}

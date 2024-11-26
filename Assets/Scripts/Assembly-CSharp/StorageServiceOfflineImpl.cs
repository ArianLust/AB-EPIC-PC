using System;
using System.Collections.Generic;
using ABH.Shared.Models;

public class StorageServiceOfflineImpl : IRemoteStorageService
{
	private Dictionary<string, PublicPlayerData> publicPlayers = new Dictionary<string, PublicPlayerData>();

	public void EnableProfileSync(string reason)
	{
	}

	public void DisableProfileSync(string reason, bool publicProfileAlso)
	{
	}

	public void SyncProfile(PlayerData playerData, Action<bool> callback = null, bool force = false)
	{
	}

	public void GetPrivateProfile(Action<PlayerData> callback, Action<string> error)
	{
	}

	public void PushPublicProfile(PublicPlayerData publicPlayerData)
	{
	}

	public void RefreshPublicPlayerDataFromFriends(IEnumerable<string> friendIds)
	{
	}

	public void SyncProfileAndGetConflictedProfile(PlayerData playerData, Action<bool, PlayerData> callback = null)
	{
		if (callback != null)
		{
			callback(true, null);
		}
	}

	public void SyncProfileAndResolveConflict(PlayerData playerData, Func<PlayerData, PlayerData, PlayerData> resolver, Action completedCallback, bool force = false)
	{
		if (completedCallback != null)
		{
			completedCallback();
		}
	}

	public void PushProfile(PlayerData playerData)
	{
	}

	public void GetPublicPlayerDatas(string[] ids, Action<Dictionary<string, PublicPlayerData>> callback, Action<string> error)
	{
		PlayerData data = DIContainerInfrastructure.GetCurrentPlayer().Data;
		if (ids.Length == 1)
		{
			publicPlayers = new Dictionary<string, PublicPlayerData>();
			publicPlayers.Add(ids[0], DIContainerLogic.PvPSeasonService.GetFallbackPvPOpponent(DIContainerInfrastructure.GetCurrentPlayer()));
			callback(publicPlayers);
			return;
		}
		for (int i = 0; i < ids.Length; i++)
		{
			string text = ids[i];
			if (!publicPlayers.ContainsKey(text))
			{
				publicPlayers.Add(text, DIContainerLogic.SocialService.GetNPCPlayer(new FriendData
				{
					FirstName = "debug_friend_" + i.ToString("00"),
					Id = text,
					// PictureUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRSYDTdf3ByIjS9TLlC4bxNTbmgWIo9gKms4TrOKdxHID2KzdlTuw" <-- wtf is this photo chimera??
					PictureUrl = ""
				}));
			}
		}
		if (DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData != null && DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.IsBossEvent && data.WorldBoss != null && data.WorldBoss.OwnTeamId != 0)
		{
			foreach (PublicPlayerData value in publicPlayers.Values)
			{
				if (value.WorldBoss == null)
				{
					value.WorldBoss = new WorldEventBossData();
					value.WorldBoss.NumberOfAttacks = 0;
					value.WorldBoss.NameId = data.WorldBoss.NameId;
					value.WorldBoss.Team1 = data.WorldBoss.Team1;
					value.WorldBoss.Team2 = data.WorldBoss.Team2;
					value.WorldBoss.VictoryCount = 0;
					value.WorldBoss.DefeatedTimestamp = new List<uint>();
				}
			}
		}
		callback(new Dictionary<string, PublicPlayerData>(publicPlayers));
	}
}

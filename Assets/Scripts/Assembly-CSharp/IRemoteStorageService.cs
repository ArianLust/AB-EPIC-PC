using System;
using System.Collections.Generic;
using ABH.Shared.Models;

public interface IRemoteStorageService
{
	void EnableProfileSync(string reason);

	void DisableProfileSync(string reason, bool publicProfileAlso);

	void SyncProfile(PlayerData playerData, Action<bool> callback = null, bool force = false);

	void SyncProfileAndGetConflictedProfile(PlayerData playerData, Action<bool, PlayerData> callback = null);

	void SyncProfileAndResolveConflict(PlayerData playerData, Func<PlayerData, PlayerData, PlayerData> resolver, Action completedCallback, bool force = false);

	void PushProfile(PlayerData playerData);

	void GetPrivateProfile(Action<PlayerData> callback, Action<string> error);

	void GetPublicPlayerDatas(string[] ids, Action<Dictionary<string, PublicPlayerData>> callback, Action<string> error);

	void PushPublicProfile(PublicPlayerData publicPlayerData);

	void RefreshPublicPlayerDataFromFriends(IEnumerable<string> friendIds);
}

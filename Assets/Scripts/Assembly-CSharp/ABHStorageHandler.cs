using System;
using System.Collections.Generic;
using ABH.Shared.Models;

public class ABHStorageHandler
{
	public void OnGotPublicProfiles(string key, Dictionary<string, string> accountToValueDictionary, Action<Dictionary<string, PublicPlayerData>> callback)
	{
		DebugLog.Log("[ABHStorageHandler] Get Profiles succeeded!");
		Dictionary<string, PublicPlayerData> dictionary = new Dictionary<string, PublicPlayerData>();
		foreach (string key2 in accountToValueDictionary.Keys)
		{
			string text = accountToValueDictionary[key2];
			if (string.IsNullOrEmpty(key2) || string.IsNullOrEmpty(text))
			{
				continue;
			}
			try
			{
				PublicPlayerData publicPlayerData = DIContainerInfrastructure.GetStringSerializer().Deserialize<PublicPlayerData>(text);
				DebugLog.Log(string.Concat("[ABHStorageHandler] Profile ", publicPlayerData, " deserialized"));
				if (!isValid(publicPlayerData))
				{
					DebugLog.Error(string.Concat("[ABHStorageHandler] Profile ", publicPlayerData, " is not valid"));
				}
				else if (!dictionary.ContainsKey(key2))
				{
					dictionary.Add(key2, publicPlayerData);
				}
			}
			catch (Exception ex)
			{
				DebugLog.Error("[ABHStorageHandler] " + ex.Message + " " + ex.StackTrace);
				DebugLog.Error("Deserializing Friend Profile Error!");
			}
		}
		callback(dictionary);
	}

	public void OnGetPublicPlayerDataFromFriendsSuccess(string key, Dictionary<string, string> accountToValueDictionary)
	{
		DebugLog.Log("[ABHStorageHandler] Get Profiles succeeded!");
		Dictionary<string, PublicPlayerData> dictionary = new Dictionary<string, PublicPlayerData>();
		List<string> list = new List<string>();
		foreach (string key2 in accountToValueDictionary.Keys)
		{
			string text = accountToValueDictionary[key2];
			if (string.IsNullOrEmpty(key2))
			{
				continue;
			}
			if (string.IsNullOrEmpty(text))
			{
				DebugLog.Error("Invalid Profile for Friend: " + key2);
				list.Add(key2);
				continue;
			}
			try
			{
				PublicPlayerData publicPlayerData = DIContainerInfrastructure.GetStringSerializer().Deserialize<PublicPlayerData>(text);
				DebugLog.Log(string.Concat("[ABHStorageHandler] Profile ", publicPlayerData, " deserialized"));
				if (!isValid(publicPlayerData))
				{
					DebugLog.Error(string.Concat("[ABHStorageHandler] Profile ", publicPlayerData, " is not valid"));
					list.Add(key2);
				}
				else if (!dictionary.ContainsKey(key2))
				{
					dictionary.Add(key2, publicPlayerData);
				}
			}
			catch (Exception ex)
			{
				DebugLog.Error("[ABHStorageHandler] " + ex.Message + " " + ex.StackTrace);
				DebugLog.Error("Deserializing Friend Profile Error!");
				list.Add(key2);
			}
		}
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.RefreshPublicPlayerDatas(dictionary, list);
	}

	private bool isValid(PublicPlayerData profile)
	{
		if (profile == null)
		{
			return false;
		}
		if (profile.Birds == null || profile.Inventory == null || profile.LocationProgress == null)
		{
			return false;
		}
		return true;
	}

	public void OnGetPublicPlayerDataFromFriendsError(string key, string errorCode)
	{
		DebugLog.Error("[ABHStorageHandler] OnGetPublicPlayerDataFromFriendsError: " + key + ", " + errorCode);
	}
}

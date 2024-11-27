using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.Generic;
using ABH.Shared.Models;

public class ABHAnalyticsHelper
{
	public static void AddPlayerStatusToTracking(Dictionary<string, string> trackingDictionary)
	{
		if (DIContainerInfrastructure.GetCurrentPlayer() == null)
		{
			trackingDictionary.SaveAdd("PlayerLevel", "0");
			trackingDictionary.SaveAdd("CurrentProgressWorldMap", "0");
			trackingDictionary.SaveAdd("CurrentProgressChronicleCave", "0");
			trackingDictionary.SaveAdd("HighestPowerLevel", "0");
			return;
		}
		trackingDictionary.SaveAdd("PlayerLevel", DIContainerInfrastructure.GetCurrentPlayer().Data.Level.ToString());
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.SocialEnvironment.LocationProgress.ContainsKey(LocationType.World))
		{
			trackingDictionary.SaveAdd("CurrentProgressWorldMap", DIContainerInfrastructure.GetCurrentPlayer().Data.SocialEnvironment.LocationProgress[LocationType.World].ToString());
		}
		else
		{
			trackingDictionary.SaveAdd("CurrentProgressWorldMap", "0");
		}
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.SocialEnvironment.LocationProgress.ContainsKey(LocationType.ChronicleCave))
		{
			trackingDictionary.SaveAdd("CurrentProgressChronicleCave", DIContainerInfrastructure.GetCurrentPlayer().Data.SocialEnvironment.LocationProgress[LocationType.ChronicleCave].ToString());
		}
		else
		{
			trackingDictionary.SaveAdd("CurrentProgressChronicleCave", "0");
		}
		trackingDictionary.SaveAdd("HighestPowerLevel", DIContainerInfrastructure.GetCurrentPlayer().Data.HighestPowerLevelEver.ToString());
	}

	public static void AddFriendsCountToTracking(Dictionary<string, string> trackingDictionary)
	{
		if (DIContainerInfrastructure.GetCurrentPlayer() == null || DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData == null || DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends == null)
		{
			trackingDictionary.SaveAdd("FriendCount", "0");
		}
		else
		{
			trackingDictionary.SaveAdd("FriendCount", (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends.Count - 4).ToString("0"));
		}
	}

	public static void AddSocialStatusToTracking(Dictionary<string, string> trackingDictionary)
	{
		trackingDictionary.Add("social_network", string.Empty);
		trackingDictionary.Add("social_network_friend_id", string.Empty);
		trackingDictionary.Add("social_network_request_id", string.Empty);
		trackingDictionary.Add("social_network_request_info", string.Empty);
	}

	public static void SendSocialEvent(MessageDataIncoming message, FriendData friendData)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string text = ((DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData == null) ? "0" : ((DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data != null) ? DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.SocialId : "-1"));
		string text2 = ((friendData != null) ? friendData.Id : "broadcast");
		// FacebookWrapperHatchImpl facebookWrapperHatchImpl = DIContainerInfrastructure.GetFacebookWrapper() as FacebookWrapperHatchImpl;
		FacebookWrapperHatchImpl facebookWrapperHatchImpl = DIContainerInfrastructure.GetFacebookWrapper() as FacebookWrapperHatchImpl;
		if (facebookWrapperHatchImpl != null)
		{
			// string facebookIdForFriendRovioAccId = facebookWrapperHatchImpl.GetFacebookIdForFriendRovioAccId(text2);
			string facebookIdForFriendRovioAccId = string.Empty;
			if (!string.IsNullOrEmpty(facebookIdForFriendRovioAccId))
			{
				text2 = facebookIdForFriendRovioAccId;
				DebugLog.Log("[ABHAnalyticsHelper] SendSocialEvent: found facebook id for friend: " + text2);
			}
			else
			{
				DebugLog.Log("[ABHAnalyticsHelper] SendSocialEvent: found no facebook id for friend: " + text2);
			}
		}
		else
		{
			DebugLog.Error("[ABHAnalyticsHelper] SendSocialEvent: facebook wrapper is no FacebookWrapperHatchImpl");
		}
		DebugLog.Log(string.Concat("[ABHAnalyticsHelper] SendSocialEvent: ", message.MessageType, ", ownSocialId = ", text, ", friendId = ", text2));
		dictionary.Add("social_network", DIContainerInfrastructure.GetFacebookWrapper().GetNetwork());
		dictionary.Add("social_network_id", text);
		dictionary.Add("social_network_friend_id", text2);
		dictionary.Add("social_network_request_id", message.MessageType.ToString());
		dictionary.Add("social_network_request_info", message.toShortParameterString());
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("SocialEvent", dictionary);
	}

	public static void AddMasteryLevelsToTracking(Dictionary<string, string> trackingDictionary)
	{
		foreach (ClassItemGameData item in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.Class])
		{
			trackingDictionary.SaveAdd(item.Name, item.Data.Level.ToString());
		}
	}

	public static void AddEnchantmentLevelsToTracking(Dictionary<string, string> trackingDictionary)
	{
		foreach (BirdGameData bird in DIContainerInfrastructure.GetCurrentPlayer().Birds)
		{
			int enchantementLevel = bird.MainHandItem.EnchantementLevel;
			trackingDictionary.SaveAdd(bird.Name + "_mainhand", enchantementLevel.ToString());
			int enchantementLevel2 = bird.OffHandItem.EnchantementLevel;
			trackingDictionary.SaveAdd(bird.Name + "_offhand", enchantementLevel.ToString());
		}
	}
}

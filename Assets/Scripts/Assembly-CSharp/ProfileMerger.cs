using System.Linq;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.InventoryItems;

internal class ProfileMerger : IProfileMerger
{
	private const int MaxLuckyCoinsAwardedPerNonIAP = 10;

	public bool TryMergeProfile(PlayerData currentProfile, PlayerData otherProfile, out PlayerData mergedProfile)
	{
		int luckyCoinCount;
		bool flag = TryGetLuckyCoinCount(currentProfile, out luckyCoinCount);
		int luckyCoinCount2;
		bool flag2 = TryGetLuckyCoinCount(otherProfile, out luckyCoinCount2);
		luckyCoinCount += currentProfile.HardCurrencySpent;
		luckyCoinCount2 += otherProfile.HardCurrencySpent;
		if (!HasOtherProfileMoreProgessThanCurrent(currentProfile, otherProfile))
		{
			DebugLog.Log(GetType(), "determined that the current profile has more progress than the other one");
			mergedProfile = currentProfile;
			if (!flag2 || !flag || luckyCoinCount2 - luckyCoinCount < 10)
			{
				if (!flag2 || !flag)
				{
					DebugLog.Warn(GetType(), "currentProfileHasLuckyCoins: " + flag + ", otherProfileHasLuckyCoins: " + flag2);
				}
				DebugLog.Log(GetType(), string.Format("the other profile does not have more hard currency than we have: Current: {0}, Other: {1}, Current-Spent: {2}, Other-Spent: {3}", luckyCoinCount, luckyCoinCount2, luckyCoinCount - currentProfile.HardCurrencySpent, luckyCoinCount2 - otherProfile.HardCurrencySpent));
				if (otherProfile.OverrideProfileMerger)
				{
					DebugLog.Log(GetType(), "TryMergeProfile: OverrideProfileMerger flag found! Using Other profile!");
					mergedProfile = otherProfile;
					mergedProfile.OverrideProfileMerger = false;
					return true;
				}
				return false;
			}
			DebugLog.Log(GetType(), "transferring the lucky coins and items from the other profile to the current one... lucky coins diff: " + (luckyCoinCount - luckyCoinCount2));
			if (!TrySetLuckyCoinCount(mergedProfile, otherProfile))
			{
				return false;
			}
			TransferBirdEquipmentAndBannerItemsFrom(otherProfile, mergedProfile);
			DebugLog.Log(GetType(), "transferred the lucky coins and items from the other profile to the current one");
		}
		else
		{
			DebugLog.Log(GetType(), "determined that the other profile has more progress than the current one");
			mergedProfile = otherProfile;
			if (!flag2 || !flag || luckyCoinCount - luckyCoinCount2 < 10)
			{
				if (!flag2 || !flag)
				{
					DebugLog.Warn(GetType(), "currentProfileHasLuckyCoins: " + flag + ", otherProfileHasLuckyCoins: " + flag2);
				}
				DebugLog.Log(GetType(), string.Format("current profile does not have more hard currency than the remote one: Current: {0}, Other: {1}, Current-Spent: {2}, Other-Spent: {3}", luckyCoinCount, luckyCoinCount2, luckyCoinCount - currentProfile.HardCurrencySpent, luckyCoinCount2 - otherProfile.HardCurrencySpent));
				return true;
			}
			DebugLog.Log(GetType(), "transferring the lucky coins and items from the current profile to the other one... lucky coins diff: " + (luckyCoinCount2 - luckyCoinCount));
			if (!TrySetLuckyCoinCount(mergedProfile, currentProfile))
			{
				return true;
			}
			TransferBirdEquipmentAndBannerItemsFrom(currentProfile, mergedProfile);
			DebugLog.Log(GetType(), "transferred the lucky coins and items from the current profile to the other one");
		}
		return true;
	}

	public PlayerData MergeProfilesSilent(PlayerData current, PlayerData remote)
	{
		DebugLog.Log(GetType(), "MergeProfilesSilent: trying to merge profile...");
		PlayerData mergedProfile;
		if (!TryMergeProfile(current, remote, out mergedProfile))
		{
			return current;
		}
		return mergedProfile;
	}

	private bool HasOtherProfileMoreProgessThanCurrent(PlayerData current, PlayerData other)
	{
		int value;
		current.SocialEnvironment.LocationProgress.TryGetValue(LocationType.World, out value);
		int value2;
		current.SocialEnvironment.LocationProgress.TryGetValue(LocationType.ChronicleCave, out value2);
		int value3;
		current.SocialEnvironment.LocationProgress.TryGetValue(LocationType.EventCampaign, out value3);
		int value4;
		other.SocialEnvironment.LocationProgress.TryGetValue(LocationType.World, out value4);
		int value5;
		other.SocialEnvironment.LocationProgress.TryGetValue(LocationType.ChronicleCave, out value5);
		int value6;
		other.SocialEnvironment.LocationProgress.TryGetValue(LocationType.EventCampaign, out value6);
		if (value < value4 || value2 < value5 || value3 < value6)
		{
			return true;
		}
		if (value == value4 && value2 == value5)
		{
			return current.Level < other.Level || (current.Level == other.Level && current.Experience < other.Experience);
		}
		DebugLog.Log(GetType(), "HasOtherProfileMoreProgressThanCurrent Own has more progress!");
		return false;
	}

	private void TransferBirdEquipmentAndBannerItemsFrom(PlayerData from, PlayerData to)
	{
		EquipmentData mainHandItem;
		foreach (EquipmentData mainHandItem2 in from.Inventory.MainHandItems)
		{
			mainHandItem = mainHandItem2;
			if (mainHandItem.ItemSource == EquipmentSource.Gatcha && !to.Inventory.MainHandItems.Any((EquipmentData toItem) => toItem.NameId == mainHandItem.NameId && toItem.Level == mainHandItem.Level))
			{
				DebugLog.Log(GetType(), "transferring main hand item " + mainHandItem.NameId);
				to.Inventory.MainHandItems.Add(mainHandItem);
			}
		}
		EquipmentData offHandItem;
		foreach (EquipmentData offHandItem2 in from.Inventory.OffHandItems)
		{
			offHandItem = offHandItem2;
			if (offHandItem.ItemSource == EquipmentSource.Gatcha && !to.Inventory.MainHandItems.Any((EquipmentData toItem) => toItem.NameId == offHandItem.NameId && toItem.Level == offHandItem.Level))
			{
				DebugLog.Log(GetType(), "transferring offhand item " + offHandItem.NameId);
				to.Inventory.OffHandItems.Add(offHandItem);
			}
		}
		BannerItemData bannerItem;
		foreach (BannerItemData bannerItem2 in from.Inventory.BannerItems)
		{
			bannerItem = bannerItem2;
			if (bannerItem.ItemSource == EquipmentSource.Gatcha && !to.Inventory.MainHandItems.Any((EquipmentData toItem) => toItem.NameId == bannerItem.NameId && toItem.Level == bannerItem.Level))
			{
				DebugLog.Log(GetType(), "transferring banner item " + bannerItem.NameId);
				to.Inventory.BannerItems.Add(bannerItem);
			}
		}
	}

	private bool TryGetLuckyCoinCount(PlayerData profile, out int luckyCoinCount)
	{
		luckyCoinCount = 0;
		BasicItemData luckyCoinItem;
		if (!TryGetLuckyCoinItem(profile, out luckyCoinItem))
		{
			return false;
		}
		luckyCoinCount = DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(luckyCoinItem).ItemValue;
		return true;
	}

	private bool TryGetLuckyCoinItem(PlayerData profile, out BasicItemData luckyCoinItem)
	{
		luckyCoinItem = null;
		foreach (BasicItemData playerStat in profile.Inventory.PlayerStats)
		{
			if (playerStat.NameId == "lucky_coin")
			{
				luckyCoinItem = playerStat;
				break;
			}
		}
		if (luckyCoinItem == null)
		{
			DebugLog.Error(GetType(), "Cannot retrieve lucky coins from profile");
			return false;
		}
		return true;
	}

	private bool TrySetLuckyCoinCount(PlayerData profile, PlayerData takeLuckyCoinsFromThisProfile)
	{
		BasicItemData luckyCoinItem;
		if (!TryGetLuckyCoinItem(profile, out luckyCoinItem))
		{
			DebugLog.Error(GetType(), "TrySetLuckyCoinCount: cannot retrieve lucky coins from profile");
			return false;
		}
		BasicItemData luckyCoinItem2;
		if (!TryGetLuckyCoinItem(takeLuckyCoinsFromThisProfile, out luckyCoinItem2))
		{
			DebugLog.Error(GetType(), "TrySetLuckyCoinCount: cannot retrieve lucky coins from takeLuckyCoinsFromThisProfile");
			return false;
		}
		profile.Inventory.PlayerStats.Remove(luckyCoinItem);
		profile.Inventory.PlayerStats.Add(luckyCoinItem2);
		return true;
	}
}

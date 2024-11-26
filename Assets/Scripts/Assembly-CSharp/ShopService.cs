using System;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

public class ShopService
{
	private List<BasicShopOfferBalancingData> m_allShopOffers = new List<BasicShopOfferBalancingData>();
	
	public List<BasicShopOfferBalancingData> GetShopOffers(PlayerGameData playerGameData, string shopNameId, bool onlyFirst = true, bool includeBoughtFinal = false)
	{
		ShopBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<ShopBalancingData>(shopNameId);
		List<BasicShopOfferBalancingData> list = new List<BasicShopOfferBalancingData>();
		foreach (string category in balancingData.Categories)
		{
			list.AddRange(GetAllShopOffersInCategory(category));
		}
		List<BasicShopOfferBalancingData> list2 = new List<BasicShopOfferBalancingData>();
		if (includeBoughtFinal)
		{
			AddBoughtAndCompletedOffersToList(list2, playerGameData, list);
		}
		if (playerGameData.Data.UniqueSpecialShopOffers == null)
		{
			playerGameData.Data.UniqueSpecialShopOffers = new List<string>();
		}
		list = list.Where((BasicShopOfferBalancingData a) => DIContainerLogic.RequirementService.CheckGenericRequirements(playerGameData, GetModifiedShowRequirements(a)) && !playerGameData.Data.UniqueSpecialShopOffers.Contains(a.NameId)).ToList();
		list.AddRange(list2);
		if (balancingData.Slots <= 0)
		{
			return list.OrderBy((BasicShopOfferBalancingData a) => a.SortPriority).ToList();
		}
		SortedDictionary<int, List<BasicShopOfferBalancingData>> sortedDictionary = new SortedDictionary<int, List<BasicShopOfferBalancingData>>();
		List<BasicShopOfferBalancingData> list3 = new List<BasicShopOfferBalancingData>();
		foreach (BasicShopOfferBalancingData item in list)
		{
			if (!IsExclusiveOfferHidden(item))
			{
				if (!sortedDictionary.ContainsKey(item.SlotId))
				{
					sortedDictionary.Add(item.SlotId, new List<BasicShopOfferBalancingData>());
				}
				sortedDictionary[item.SlotId].Add(item);
			}
		}
		foreach (KeyValuePair<int, List<BasicShopOfferBalancingData>> item2 in sortedDictionary)
		{
			if (onlyFirst)
			{
				list3.Add(item2.Value.OrderBy((BasicShopOfferBalancingData a) => a.SortPriority).ToList().FirstOrDefault());
			}
			else
			{
				list3.AddRange(item2.Value.OrderBy((BasicShopOfferBalancingData a) => a.SortPriority).ToList());
			}
		}
		return list3;
	}

	private void AddBoughtAndCompletedOffersToList(List<BasicShopOfferBalancingData> AlreadyPurchasedOffers, PlayerGameData playerGameData, List<BasicShopOfferBalancingData> offerList)
	{
		foreach (BasicShopOfferBalancingData item in offerList.Where((BasicShopOfferBalancingData o) => o is BuyableShopOfferBalancingData && (o as BuyableShopOfferBalancingData).DisplayAfterPurchase))
		{
			List<Requirement> remainingReqs;
			if (WasOfferBought(item, playerGameData, out remainingReqs) && DIContainerLogic.RequirementService.CheckGenericRequirements(playerGameData, remainingReqs))
			{
				AlreadyPurchasedOffers.Add(item);
			}
		}
	}

	public bool WasOfferBought(BasicShopOfferBalancingData offer, PlayerGameData playerGameData, out List<Requirement> remainingReqs)
	{
		remainingReqs = new List<Requirement>();
		if (offer.ShowRequirements == null || (offer is BuyableShopOfferBalancingData && !(offer as BuyableShopOfferBalancingData).DisplayAfterPurchase))
		{
			return false;
		}
		bool result = false;
		foreach (Requirement showRequirement in offer.ShowRequirements)
		{
			if ((showRequirement.RequirementType == RequirementType.NotHaveClass || showRequirement.RequirementType == RequirementType.NotHaveItem) && DIContainerLogic.InventoryService.CheckForItem(playerGameData.InventoryGameData, showRequirement.NameId))
			{
				result = true;
			}
			else if (showRequirement.RequirementType == RequirementType.NotHaveItemWithLevel)
			{
				IInventoryItemGameData data = null;
				if (DIContainerLogic.InventoryService.TryGetItemGameData(playerGameData.InventoryGameData, showRequirement.NameId, out data))
				{
					result = (float)data.ItemData.Level >= showRequirement.Value;
				}
			}
			else
			{
				remainingReqs.Add(showRequirement);
			}
		}
		return result;
	}

	public bool IsExclusiveOfferHidden(BasicShopOfferBalancingData offer)
	{
		if (offer.HideUnlessOnSale)
		{
			return !DIContainerLogic.GetSalesManagerService().IsItemOnSale(offer.NameId);
		}
		return false;
	}

	public List<Requirement> GetModifiedBuyRequirementsIgnoreCostAndLevel(BasicShopOfferBalancingData offer)
	{
		return GetModifiedRequirements(offer.BuyRequirements.Where((Requirement r) => r.RequirementType != RequirementType.PayItem && r.RequirementType != RequirementType.Level).ToList());
	}

	public List<Requirement> GetModifiedBuyRequirementsIgnoreCost(BasicShopOfferBalancingData offer)
	{
		return GetModifiedRequirements(offer.BuyRequirements.Where((Requirement r) => r.RequirementType != RequirementType.PayItem).ToList());
	}

	public List<Requirement> GetModifiedBuyRequirements(BasicShopOfferBalancingData offer)
	{
		return GetModifiedRequirements(offer.BuyRequirements);
	}

	public List<Requirement> GetModifiedShowRequirements(BasicShopOfferBalancingData offer)
	{
		return GetModifiedRequirements(offer.ShowRequirements);
	}

	public List<Requirement> GetModifiedRequirements(List<Requirement> requirementsToModify)
	{
		if (requirementsToModify == null)
		{
			return new List<Requirement>();
		}
		List<Requirement> list = new List<Requirement>();
		foreach (Requirement item in requirementsToModify)
		{
			list.Add(new Requirement
			{
				NameId = item.NameId,
				RequirementType = item.RequirementType,
				Value = item.Value
			});
		}
		return list;
	}

	public List<Requirement> GetAllModifiedBuyRequirements(PlayerGameData player, BasicShopOfferBalancingData offer, bool apply = true)
	{
		List<Requirement> modifiedBuyRequirements = GetModifiedBuyRequirements(offer);
		SaleOfferTupel offerSaleDetails = DIContainerLogic.GetSalesManagerService().GetOfferSaleDetails(offer.NameId);
		if (!apply || offerSaleDetails.IsEmpty() || offerSaleDetails.OfferDetails.SaleParameter != 0)
		{
			return modifiedBuyRequirements;
		}
		Requirement requirement = modifiedBuyRequirements.Find((Requirement req) => req.RequirementType == RequirementType.PayItem);
		if (requirement != null)
		{
			requirement.Value = offerSaleDetails.OfferDetails.ChangedValue;
		}
		return modifiedBuyRequirements;
	}

	public int GetOfferLevel(int playerLevel, BasicShopOfferBalancingData offerBalancing)
	{
		if (offerBalancing is GachaShopOfferBalancingData)
		{
			return Math.Max(1, playerLevel + offerBalancing.Level);
		}
		return offerBalancing.Level;
	}

	public BasicShopOfferBalancingData GetInstantOffersForCategoryAndLevel(string categoryId, int level)
	{
		if (string.IsNullOrEmpty(categoryId))
		{
			return null;
		}
		List<BasicShopOfferBalancingData> allShopOffersInCategory = GetAllShopOffersInCategory(categoryId);
		List<BasicShopOfferBalancingData> source = allShopOffersInCategory.Where((BasicShopOfferBalancingData o) => GetOfferLevel(level, o) <= level).ToList();
		source = source.OrderBy((BasicShopOfferBalancingData o) => level - GetOfferLevel(level, o)).ToList();
		return source.FirstOrDefault();
	}

	public List<BasicShopOfferBalancingData> GetAllShopOffers()
	{
		if (m_allShopOffers != null && m_allShopOffers.Count > 0)
		{
			return m_allShopOffers;
		}
		List<GachaShopOfferBalancingData> list = (from o in DIContainerBalancing.Service.GetBalancingDataList<GachaShopOfferBalancingData>()
			where !string.IsNullOrEmpty(o.Category) && o.Category != "none"
			select o).ToList();
		List<PremiumShopOfferBalancingData> list2 = (from o in DIContainerBalancing.Service.GetBalancingDataList<PremiumShopOfferBalancingData>()
			where !string.IsNullOrEmpty(o.Category) && o.Category != "none"
			select o).ToList();
		List<BuyableShopOfferBalancingData> list3 = (from o in DIContainerBalancing.Service.GetBalancingDataList<BuyableShopOfferBalancingData>()
			where !string.IsNullOrEmpty(o.Category) && o.Category != "none"
			select o).ToList();
		List<BasicShopOfferBalancingData> list4 = new List<BasicShopOfferBalancingData>();
		for (int i = 0; i < list.Count; i++)
		{
			list4.Add(list[i]);
		}
		for (int j = 0; j < list2.Count; j++)
		{
			list4.Add(list2[j]);
		}
		for (int k = 0; k < list3.Count; k++)
		{
			list4.Add(list3[k]);
		}
		m_allShopOffers = list4;
		return m_allShopOffers;
	}

	public BasicShopOfferBalancingData GetShopOffer(string nameId)
	{
		GachaShopOfferBalancingData balancing = null;
		PremiumShopOfferBalancingData balancing2 = null;
		BuyableShopOfferBalancingData balancing3 = null;
		if (DIContainerBalancing.Service.TryGetBalancingData<GachaShopOfferBalancingData>(nameId, out balancing))
		{
			return balancing;
		}
		if (DIContainerBalancing.Service.TryGetBalancingData<PremiumShopOfferBalancingData>(nameId, out balancing2))
		{
			return balancing2;
		}
		if (DIContainerBalancing.Service.TryGetBalancingData<BuyableShopOfferBalancingData>(nameId, out balancing3))
		{
			return balancing3;
		}
		return null;
	}

	public List<BasicShopOfferBalancingData> GetAllShopOffersInCategory(string categoryId)
	{
		if (string.IsNullOrEmpty(categoryId))
		{
			return null;
		}
		List<GachaShopOfferBalancingData> list = (from a in DIContainerBalancing.Service.GetBalancingDataList<GachaShopOfferBalancingData>()
			where a.Category == categoryId
			select a).ToList();
		List<PremiumShopOfferBalancingData> list2 = (from a in DIContainerBalancing.Service.GetBalancingDataList<PremiumShopOfferBalancingData>()
			where a.Category == categoryId
			select a).ToList();
		List<BuyableShopOfferBalancingData> list3 = (from a in DIContainerBalancing.Service.GetBalancingDataList<BuyableShopOfferBalancingData>()
			where a.Category == categoryId
			select a).ToList();
		List<BasicShopOfferBalancingData> list4 = new List<BasicShopOfferBalancingData>();
		for (int i = 0; i < list.Count; i++)
		{
			list4.Add(list[i]);
		}
		for (int j = 0; j < list2.Count; j++)
		{
			list4.Add(list2[j]);
		}
		for (int k = 0; k < list3.Count; k++)
		{
			list4.Add(list3[k]);
		}
		return list4;
	}

	public List<IInventoryItemGameData> BuyShopOffers(PlayerGameData playerGameData, List<BasicShopOfferBalancingData> offers, string reason = "buyShopOffers")
	{
		List<Requirement> summedBuyRequirements = GetSummedBuyRequirements(playerGameData, offers);
		if (DIContainerLogic.RequirementService.ExecuteRequirements(playerGameData.InventoryGameData, summedBuyRequirements, reason))
		{
			DebugLog.Log("Offers paid!");
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
			foreach (BasicShopOfferBalancingData offer in offers)
			{
				if (!dictionary2.ContainsKey(offer.NameId))
				{
					dictionary2.Add(offer.NameId, 0);
				}
				Dictionary<string, int> dictionary3;
				Dictionary<string, int> dictionary4 = (dictionary3 = dictionary2);
				string nameId;
				string key = (nameId = offer.NameId);
				int num = dictionary3[nameId];
				dictionary4[key] = num + 1;
			}
			string value = "instant_buy_offer";
			dictionary.SaveAdd("OfferName", value);
			BasicShopOfferBalancingData basicShopOfferBalancingData = offers.FirstOrDefault();
			if (basicShopOfferBalancingData != null)
			{
				dictionary.SaveAdd("OfferType", basicShopOfferBalancingData.Category);
			}
			dictionary.SaveAdd("PlayerLevel", DIContainerInfrastructure.GetCurrentPlayer().Data.Level.ToString());
			DIContainerInfrastructure.GetAnalyticsSystem(true).LogEventWithParameters("ShopOfferBought", dictionary);
			dictionary.SaveAdd("TypeOfGain", "ShopOfferBought");
			List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
			foreach (BasicShopOfferBalancingData offer2 in offers)
			{
				Dictionary<string, string> dictionary5 = new Dictionary<string, string>(dictionary);
				int num2 = 0;
				foreach (Requirement item in from r in GetAllModifiedBuyRequirements(playerGameData, offer2)
					where r.RequirementType == RequirementType.PayItem
					select r)
				{
					dictionary5.Add("SpendResourceNameX" + num2, item.NameId);
					dictionary5.Add("SpendValueX" + num2, item.Value.ToString());
					num2++;
				}
				list.AddRange(DIContainerLogic.GetLootOperationService().RewardLoot(playerGameData.InventoryGameData, 2, DIContainerLogic.GetLootOperationService().GenerateLoot(offer2.OfferContents, GetOfferLevel(playerGameData.Data.Level, offer2)), dictionary5, EquipmentSource.Gatcha));
				if (offer2.UniqueOffer && playerGameData.Data.CurrentSpecialShopOffers != null && playerGameData.Data.CurrentSpecialShopOffers.ContainsKey(offer2.NameId))
				{
					playerGameData.Data.CurrentSpecialShopOffers.Remove(offer2.NameId);
					if (playerGameData.Data.UniqueSpecialShopOffers == null)
					{
						playerGameData.Data.UniqueSpecialShopOffers = new List<string>();
					}
					playerGameData.Data.UniqueSpecialShopOffers.Add(offer2.NameId);
				}
			}
			playerGameData.SavePlayerData();
			return list;
		}
		return new List<IInventoryItemGameData>();
	}

	public List<Requirement> GetSummedBuyRequirements(PlayerGameData playerGameData, List<BasicShopOfferBalancingData> offers)
	{
		List<Requirement> list = new List<Requirement>();
		foreach (BasicShopOfferBalancingData offer in offers)
		{
			List<Requirement> allModifiedBuyRequirements = GetAllModifiedBuyRequirements(playerGameData, offer);
			AccumulateRequirements(allModifiedBuyRequirements, list);
		}
		return list;
	}

	private static void AccumulateRequirements(List<Requirement> modifiedBuyRequirements, List<Requirement> summedRequirements)
	{
		foreach (Requirement modifiedBuyRequirement in modifiedBuyRequirements)
		{
			bool flag = false;
			foreach (Requirement summedRequirement in summedRequirements)
			{
				if (summedRequirement.RequirementType == modifiedBuyRequirement.RequirementType && summedRequirement.NameId == modifiedBuyRequirement.NameId)
				{
					summedRequirement.Value += modifiedBuyRequirement.Value;
					flag = true;
				}
			}
			if (!flag)
			{
				summedRequirements.Add(new Requirement
				{
					NameId = modifiedBuyRequirement.NameId,
					RequirementType = modifiedBuyRequirement.RequirementType,
					Value = modifiedBuyRequirement.Value
				});
			}
		}
	}

	public List<IInventoryItemGameData> BuyShopOffer(PlayerGameData playerGameData, BasicShopOfferBalancingData offer, string reason = "buyShopOffer", bool ignoreReqs = false, int amount = 0, string entersource = "Standard")
	{
		List<Requirement> allModifiedBuyRequirements = GetAllModifiedBuyRequirements(playerGameData, offer);
		if (ignoreReqs || DIContainerLogic.RequirementService.ExecuteRequirements(playerGameData.InventoryGameData, allModifiedBuyRequirements, reason))
		{
			DebugLog.Log("Offer paid!");
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.SaveAdd("OfferName", offer.NameId);
			dictionary.SaveAdd("OfferType", offer.Category);
			dictionary.SaveAdd("PlayerLevel", DIContainerInfrastructure.GetCurrentPlayer().Data.Level.ToString());
			int multiGachaAmount = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").MultiGachaAmount;
			int num = 0;
			foreach (Requirement item in allModifiedBuyRequirements.Where((Requirement r) => r.RequirementType == RequirementType.PayItem))
			{
				switch (amount)
				{
				case 6:
					dictionary.SaveAdd("SpendValueX" + num, "0");
					break;
				default:
					dictionary.SaveAdd("SpendValueX" + num, (item.Value / (float)(multiGachaAmount - 1)).ToString());
					break;
				case 0:
					dictionary.SaveAdd("SpendValueX" + num, item.Value.ToString());
					break;
				}
				dictionary.SaveAdd("SpendResourceNameX" + num, item.NameId);
				num++;
			}
			dictionary.SaveAdd("Enterway", entersource);
			DIContainerInfrastructure.GetAnalyticsSystem(true).LogEventWithParameters("ShopOfferBought", dictionary);
			dictionary.SaveAdd("TypeOfGain", "ShopOfferBought");
			Dictionary<string, LootInfoData> dictionary2 = DIContainerLogic.GetLootOperationService().GenerateLoot(offer.OfferContents, GetOfferLevel(playerGameData.Data.Level, offer));
			foreach (KeyValuePair<string, LootInfoData> item2 in dictionary2)
			{
				SaleOfferTupel saleOffer;
				if (GetActiveSaleDetailsForOffer(offer.NameId, out saleOffer) && saleOffer.OfferDetails.SaleParameter == SaleParameter.Value)
				{
					item2.Value.Value = saleOffer.OfferDetails.ChangedValue;
				}
				if (!(offer is GachaShopOfferBalancingData) && (item2.Key.Contains("weapon") || item2.Key.Contains("offhand") || item2.Key.Contains("banner")) && !item2.Key.Contains("recipe"))
				{
					item2.Value.Level = playerGameData.Data.Level + 2;
				}
			}
			List<IInventoryItemGameData> result = DIContainerLogic.GetLootOperationService().RewardLoot(playerGameData.InventoryGameData, 2, dictionary2, dictionary, EquipmentSource.Gatcha);
			if (offer.UniqueOffer && playerGameData.Data.CurrentSpecialShopOffers != null && playerGameData.Data.CurrentSpecialShopOffers.ContainsKey(offer.NameId))
			{
				playerGameData.Data.CurrentSpecialShopOffers.Remove(offer.NameId);
				if (playerGameData.Data.UniqueSpecialShopOffers == null)
				{
					playerGameData.Data.UniqueSpecialShopOffers = new List<string>();
				}
				playerGameData.Data.UniqueSpecialShopOffers.Add(offer.NameId);
			}
			playerGameData.SavePlayerData();
			return result;
		}
		return null;
	}

	public bool IsOfferBuyableIgnoreCostAndLevel(PlayerGameData playerGameData, BasicShopOfferBalancingData offer)
	{
		List<Requirement> failed;
		return IsOfferBuyableIgnoreCostAndlevel(playerGameData, offer, out failed);
	}

	public bool IsOfferBuyableIgnoreCost(PlayerGameData playerGameData, BasicShopOfferBalancingData offer)
	{
		List<Requirement> failed;
		return IsOfferBuyableIgnoreCost(playerGameData, offer, out failed);
	}

	public bool IsOfferBuyableIgnoreCost(PlayerGameData playerGameData, BasicShopOfferBalancingData offer, out List<Requirement> failed)
	{
		return DIContainerLogic.RequirementService.CheckGenericRequirements(playerGameData, GetModifiedBuyRequirementsIgnoreCost(offer), out failed);
	}

	public bool IsOfferBuyableIgnoreCostAndlevel(PlayerGameData playerGameData, BasicShopOfferBalancingData offer, out List<Requirement> failed)
	{
		return DIContainerLogic.RequirementService.CheckGenericRequirements(playerGameData, GetModifiedBuyRequirementsIgnoreCostAndLevel(offer), out failed);
	}

	public bool AreOffersBuyable(PlayerGameData playerGameData, List<BasicShopOfferBalancingData> offers, out List<Requirement> failed)
	{
		return DIContainerLogic.RequirementService.CheckGenericRequirements(playerGameData, GetSummedBuyRequirements(playerGameData, offers), out failed);
	}

	public bool IsOfferBuyable(PlayerGameData playerGameData, BasicShopOfferBalancingData offer, out List<Requirement> failed)
	{
		return DIContainerLogic.RequirementService.CheckGenericRequirements(playerGameData, GetAllModifiedBuyRequirements(playerGameData, offer), out failed);
	}

	public bool IsOfferShowable(PlayerGameData playerGameData, BasicShopOfferBalancingData offer)
	{
		List<Requirement> failed;
		return IsOfferShowable(playerGameData, offer, out failed);
	}

	public bool IsOfferShowable(PlayerGameData playerGameData, BasicShopOfferBalancingData offer, out List<Requirement> failed)
	{
		return DIContainerLogic.RequirementService.CheckGenericRequirements(playerGameData, GetModifiedShowRequirements(offer), out failed);
	}

	public List<IInventoryItemGameData> GetShopOfferContent(PlayerGameData player, BasicShopOfferBalancingData offer, SaleOfferTupel offerSaleData)
	{
		List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(offer.OfferContents, GetOfferLevel(player.Data.Level, offer)), EquipmentSource.Gatcha);
		if (offerSaleData.IsEmpty() || offerSaleData.OfferDetails.SaleParameter != SaleParameter.Value)
		{
			return itemsFromLoot;
		}
		for (int i = 0; i < itemsFromLoot.Count; i++)
		{
			IInventoryItemGameData inventoryItemGameData = itemsFromLoot[i];
			if (inventoryItemGameData != null)
			{
				inventoryItemGameData.ItemValue = offerSaleData.OfferDetails.ChangedValue;
			}
		}
		return itemsFromLoot;
	}

	public List<Requirement> GetBuyResourcesRequirements(int level, BasicShopOfferBalancingData offer, bool applyDiscount = true)
	{
		return (from r in GetAllModifiedBuyRequirements(DIContainerInfrastructure.GetCurrentPlayer(), offer, applyDiscount)
			where r.RequirementType == RequirementType.PayItem
			select r).ToList();
	}

	public BasicShopOfferBalancingData GetGachaOffer(bool arena, PlayerGameData player, bool advanced, bool high = false)
	{
		if (arena)
		{
			if (high && advanced)
			{
				return GetShopOffers(player, "pvp_adv_gacha_high").FirstOrDefault();
			}
			if (advanced)
			{
				return GetShopOffers(player, "pvp_adv_gacha").FirstOrDefault();
			}
			if (high)
			{
				return GetShopOffers(player, "pvp_gacha_high").FirstOrDefault();
			}
			return GetShopOffers(player, "pvp_gacha").FirstOrDefault();
		}
		if (high && advanced)
		{
			return GetShopOffers(player, "adv_gacha_high").FirstOrDefault();
		}
		if (advanced)
		{
			return GetShopOffers(player, "adv_gacha").FirstOrDefault();
		}
		if (high)
		{
			return GetShopOffers(player, "gacha_high").FirstOrDefault();
		}
		return GetShopOffers(player, "gacha").FirstOrDefault();
	}

	public bool IsGachaOfferBuyAble(bool arena, PlayerGameData player, out List<Requirement> failed, bool advanced, bool high = false)
	{
		return IsOfferBuyable(player, GetGachaOffer(arena, player, advanced, high), out failed);
	}

	public List<IInventoryItemGameData> BuyGachaOffer(bool arena, PlayerGameData player, bool free, bool advanced, out int starCount, bool high = false, int amount = 1, bool ignoreReqs = false)
	{
		BasicShopOfferBalancingData gachaOffer = GetGachaOffer(arena, player, advanced, high);
		starCount = 0;
		if (gachaOffer == null)
		{
			return null;
		}
		DebugLog.Log("[ShopService] Got Gacha Offer: " + gachaOffer.NameId);
		List<IInventoryItemGameData> list2;
		if (!high && free && ((!arena && DIContainerLogic.SocialService.HasFreeGachaRoll(ClientInfo.InspectedFriend, player)) || (arena && DIContainerLogic.SocialService.HasFreePvpGachaRoll(ClientInfo.InspectedFriend, player))))
		{
			if (!arena)
			{
				List<Requirement> list = (from r in GetAllModifiedBuyRequirements(player, gachaOffer)
					where r.NameId == "gacha_standard_uses" || r.NameId == "gacha_set_uses"
					select r).ToList();
				if (list.Count > 0)
				{
					DIContainerLogic.RequirementService.ExecuteRequirements(player.InventoryGameData, list, "free_gacha_offer");
				}
				list2 = DIContainerLogic.GetLootOperationService().RewardLoot(player.InventoryGameData, 2, DIContainerLogic.GetLootOperationService().GenerateLoot(gachaOffer.OfferContents, GetOfferLevel(player.Data.Level, gachaOffer)), gachaOffer.Category, EquipmentSource.Gatcha);
			}
			else
			{
				List<Requirement> list3 = (from r in GetAllModifiedBuyRequirements(player, gachaOffer)
					where r.NameId == "pvpgacha_standard_uses" || r.NameId == "pvpgacha_set_uses"
					select r).ToList();
				if (list3.Count > 0)
				{
					DIContainerLogic.RequirementService.ExecuteRequirements(player.InventoryGameData, list3, "free_pvpgacha_offer");
				}
				list2 = DIContainerLogic.GetLootOperationService().RewardLoot(player.InventoryGameData, 2, DIContainerLogic.GetLootOperationService().GenerateLoot(gachaOffer.OfferContents, GetOfferLevel(player.Data.Level, gachaOffer)), gachaOffer.Category, EquipmentSource.Gatcha);
			}
		}
		else
		{
			list2 = BuyShopOffer(player, gachaOffer, gachaOffer.NameId, ignoreReqs, amount);
		}
		if (list2 == null)
		{
			return null;
		}
		IInventoryItemGameData inventoryItemGameData = list2.FirstOrDefault();
		if (inventoryItemGameData != null)
		{
			starCount = inventoryItemGameData.ItemData.Level - player.Data.Level + 2;
			if (!arena)
			{
				inventoryItemGameData.ItemData.Quality = starCount;
			}
			DebugLog.Log("Got gatcha item: " + inventoryItemGameData.ItemData.NameId + " with level: " + inventoryItemGameData.ItemData.Level + " and quality: " + inventoryItemGameData.ItemData.Quality + " and stars: " + starCount + " and Player Level: " + player.Data.Level);
		}
		if (high || !free || (!arena && !DIContainerLogic.SocialService.HasFreeGachaRoll(ClientInfo.InspectedFriend, DIContainerInfrastructure.GetCurrentPlayer())) || (arena && !DIContainerLogic.SocialService.HasFreePvpGachaRoll(ClientInfo.InspectedFriend, DIContainerInfrastructure.GetCurrentPlayer())))
		{
			if (!arena)
			{
				int num = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").GachaUsesFromNormalOffer;
				if (high)
				{
					num = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").GachaUsesFromHighOffer;
				}
				BonusEventBalancingData currentValidBalancing = DIContainerLogic.GetBonusEventService.m_CurrentValidBalancing;
				if (currentValidBalancing != null && currentValidBalancing.BonusType == BonusEventType.RainbowbarBonus)
				{
					num += (int)((float)num * (currentValidBalancing.BonusFactor / 100f));
				}
				if (!gachaOffer.Category.Contains("_set"))
				{
					DIContainerLogic.InventoryService.AddItem(player.InventoryGameData, 0, 0, "gacha_standard_uses", num, "gacha_use");
				}
				else
				{
					DIContainerLogic.InventoryService.RemoveItem(player.InventoryGameData, "gacha_standard_uses", DIContainerLogic.InventoryService.GetItemValue(player.InventoryGameData, "gacha_standard_uses"), "clear_gacha_uses");
					DIContainerLogic.InventoryService.AddItem(player.InventoryGameData, 0, 0, "gacha_standard_uses", num, "gacha_use");
					DIContainerLogic.InventoryService.AddItem(player.InventoryGameData, 0, 0, "gacha_set_uses", 1, "gacha_use");
				}
			}
			else
			{
				int num2 = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").PvpGachaUsesFromNormalOffer;
				if (high)
				{
					num2 = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").PvpGachaUsesFromHighOffer;
				}
				BonusEventBalancingData currentValidBalancing2 = DIContainerLogic.GetBonusEventService.m_CurrentValidBalancing;
				if (currentValidBalancing2 != null && currentValidBalancing2.BonusType == BonusEventType.RainbowbarBonus)
				{
					num2 += (int)((float)num2 * (currentValidBalancing2.BonusFactor / 100f));
				}
				if (!gachaOffer.Category.Contains("_set"))
				{
					DIContainerLogic.InventoryService.AddItem(player.InventoryGameData, 0, 0, "pvpgacha_standard_uses", num2, "pvpgacha_use");
				}
				else
				{
					DIContainerLogic.InventoryService.RemoveItem(player.InventoryGameData, "pvpgacha_standard_uses", DIContainerLogic.InventoryService.GetItemValue(player.InventoryGameData, "pvpgacha_standard_uses"), "clear_pvpgacha_uses");
					DIContainerLogic.InventoryService.AddItem(player.InventoryGameData, 0, 0, "pvpgacha_standard_uses", num2, "pvpgacha_use");
					DIContainerLogic.InventoryService.AddItem(player.InventoryGameData, 0, 0, "pvpgacha_set_uses", 1, "gacha_use");
				}
			}
		}
		player.SavePlayerData();
		if (arena)
		{
			(list2.FirstOrDefault() as BannerItemGameData).Data.Stars = starCount + 1;
		}
		if (amount == 1)
		{
			List<IInventoryItemGameData> list4 = new List<IInventoryItemGameData>();
			list4.Add(list2.FirstOrDefault());
			return list4;
		}
		List<IInventoryItemGameData> list5 = new List<IInventoryItemGameData>();
		list5.Add(list2.FirstOrDefault());
		while (amount > 1)
		{
			list5.Add(BuyGachaOffer(arena, player, free, advanced, out starCount, high, 1, true).FirstOrDefault());
			amount--;
		}
		return list5;
	}

	public bool IsNextClassUpgradeAvaliable(PlayerGameData playerGameData)
	{
		return DIContainerLogic.GetTimingService().IsAfter(DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(playerGameData.Data.LastClassSwitchTime).AddSeconds(DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").TimeForNextClassUpgrade));
	}

	private void RemoveInvalidSpecialOffers(PlayerGameData player)
	{
		Dictionary<string, DateTime> dictionary = new Dictionary<string, DateTime>(player.Data.CurrentSpecialShopOffers);
		for (int i = 0; i < dictionary.Count; i++)
		{
			BasicShopOfferBalancingData shopOffer = GetShopOffer(dictionary.ElementAt(i).Key);
			if (shopOffer != null)
			{
				if ((!IsDiscountValid(shopOffer) && !IsOneTimeOfferValid(shopOffer)) || !DIContainerLogic.RequirementService.CheckGenericRequirements(player, shopOffer.ShowRequirements))
				{
					player.Data.CurrentSpecialShopOffers.Remove(shopOffer.NameId);
					if (shopOffer.NameId == "offer_world_potion_event_energy_discount")
					{
						DIContainerLogic.InventoryService.RemoveItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "show_energy_discount", 1, "energy_discount_only_once");
					}
					DebugLog.Log("[SpecialOffers] Removed Special Offer: " + shopOffer.NameId);
					if (shopOffer.UniqueOffer || shopOffer.Duration > 0 || shopOffer.EndDate > 0)
					{
						player.Data.UniqueSpecialShopOffers.Add(shopOffer.NameId);
					}
					if (shopOffer.DiscountCooldown > 0)
					{
						player.Data.CurrentCooldownOffers.Add(shopOffer.NameId, DIContainerLogic.GetTimingService().GetPresentTime());
					}
				}
			}
			else
			{
				if (shopOffer != null && shopOffer.NameId == "offer_world_potion_event_energy_discount")
				{
					DIContainerLogic.InventoryService.RemoveItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "show_energy_discount", 1, "energy_discount_only_once");
				}
				player.Data.CurrentSpecialShopOffers.Remove(dictionary.ElementAt(i).Key);
			}
		}
	}

	private void CheckCooldownOffers(PlayerGameData player)
	{
		Dictionary<string, DateTime> dictionary = new Dictionary<string, DateTime>(player.Data.CurrentCooldownOffers);
		for (int i = 0; i < dictionary.Count; i++)
		{
			string key = dictionary.ElementAt(i).Key;
			BasicShopOfferBalancingData shopOffer = GetShopOffer(key);
			if (shopOffer != null && dictionary[key].AddSeconds(shopOffer.DiscountCooldown) <= DIContainerLogic.GetTimingService().GetPresentTime())
			{
				player.Data.CurrentCooldownOffers.Remove(key);
			}
		}
	}

	public bool HasRainbowRiot(PlayerGameData playerGameData)
	{
		if ((DIContainerLogic.InventoryService.GetItemValue(playerGameData.InventoryGameData, "special_offer_rainbow_riot") > 0 || DIContainerLogic.InventoryService.GetItemValue(playerGameData.InventoryGameData, "special_offer_rainbow_riot_02") > 0) && IsRainbowRiotRunning(playerGameData))
		{
			return true;
		}
		int itemValue = DIContainerLogic.InventoryService.GetItemValue(playerGameData.InventoryGameData, "special_offer_rainbow_riot");
		int itemValue2 = DIContainerLogic.InventoryService.GetItemValue(playerGameData.InventoryGameData, "special_offer_rainbow_riot_02");
		if (itemValue > 0)
		{
			DebugLog.Log("[ShopService] Ended Rainbow Riot!");
			DIContainerLogic.InventoryService.RemoveItem(playerGameData.InventoryGameData, "special_offer_rainbow_riot", itemValue, "riot_timer_passed");
		}
		else if (itemValue2 > 0)
		{
			DebugLog.Log("[ShopService] Ended Rainbow Riot!");
			DIContainerLogic.InventoryService.RemoveItem(playerGameData.InventoryGameData, "special_offer_rainbow_riot_02", itemValue2, "riot_timer02_passed");
		}
		return false;
	}

	public DateTime GetRainbowRiotEndTime(PlayerGameData playerGameData)
	{
		int itemValue = DIContainerLogic.InventoryService.GetItemValue(playerGameData.InventoryGameData, "special_offer_rainbow_riot");
		if (itemValue == 0)
		{
			itemValue = DIContainerLogic.InventoryService.GetItemValue(playerGameData.InventoryGameData, "special_offer_rainbow_riot_02");
		}
		return DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(playerGameData.Data.LastRainbowRiotTime).AddMinutes(itemValue);
	}

	public float GetRainbowRiotTimeLeft(PlayerGameData playerGameData)
	{
		DateTime trustedTime;
		if (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			return 86400f;
		}
		return (float)(GetRainbowRiotEndTime(playerGameData) - trustedTime).TotalSeconds;
	}

	public bool IsRainbowRiotRunning(PlayerGameData playerGameData)
	{
		DateTime trustedTime;
		if (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			return false;
		}
		return trustedTime < GetRainbowRiotEndTime(playerGameData);
	}

	public void StartRainbowRiot(PlayerGameData playerGameData)
	{
		DebugLog.Log("[ShopService] Started Rainbow Riot!");
		DIContainerLogic.GetTimingService().GetTrustedTimeEx(delegate(DateTime trustedTime)
		{
			playerGameData.Data.LastRainbowRiotTime = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime);
			playerGameData.Data.IsExtraRainbowRiot = false;
			playerGameData.SavePlayerData();
		});
	}

	public void StartRainbowRiot2(PlayerGameData playerGameData)
	{
		DebugLog.Log("[ShopService] Started extra Rainbow Riot x5!");
		DIContainerLogic.GetTimingService().GetTrustedTimeEx(delegate(DateTime trustedTime)
		{
			playerGameData.Data.LastRainbowRiotTime = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime);
			playerGameData.Data.IsExtraRainbowRiot = true;
			playerGameData.SavePlayerData();
		});
	}

	public TimeSpan GetClassSwitchTimeLeft(PlayerGameData playerGameData)
	{
		DateTime trustedTime;
		if (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			trustedTime = DIContainerLogic.GetTimingService().GetPresentTime();
		}
		DateTime dateTimeFromTimestamp = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(playerGameData.Data.LastClassSwitchTime);
		uint timeForNextClassUpgrade = playerGameData.WorldGameData.BalancingData.TimeForNextClassUpgrade;
		if (timeForNextClassUpgrade != 0)
		{
			return dateTimeFromTimestamp.AddSeconds(timeForNextClassUpgrade) - trustedTime;
		}
		DateTime dateTime = dateTimeFromTimestamp.ToLocalTime().AddDays(1.0);
		DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Local).ToUniversalTime();
		return dateTime2 - trustedTime;
	}

	public bool IsOneTimeOfferValid(BasicShopOfferBalancingData offer)
	{
		int startDate = offer.StartDate;
		int endDate = offer.EndDate;
		int duration = offer.Duration;
		if (startDate == 0 && endDate == 0 && duration == 0)
		{
			return false;
		}
		uint currentTimestamp = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
		if (startDate > 0 && currentTimestamp < startDate)
		{
			return false;
		}
		if (endDate > 0 && currentTimestamp > endDate)
		{
			return false;
		}
		if (duration > 0)
		{
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			if (currentPlayer.Data.CurrentSpecialShopOffers.ContainsKey(offer.NameId))
			{
				if (currentPlayer.Data.CurrentSpecialShopOffers[offer.NameId].AddSeconds(duration) < DIContainerLogic.GetTimingService().GetPresentTime())
				{
					return false;
				}
			}
			else if (startDate > 0 && startDate + duration < currentTimestamp)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsDiscountValid(BasicShopOfferBalancingData offer)
	{
		return DIContainerLogic.GetSalesManagerService().IsItemOnSale(offer.NameId);
	}

	public void ClearShopBalancingCache()
	{
		if (m_allShopOffers != null)
		{
			m_allShopOffers.Clear();
		}
	}

	public bool IsValueDiscount(BasicShopOfferBalancingData offer)
	{
		SaleOfferTupel offerSaleDetails = DIContainerLogic.GetSalesManagerService().GetOfferSaleDetails(offer.NameId);
		if (offerSaleDetails.IsEmpty())
		{
			return false;
		}
		return offerSaleDetails.OfferDetails.SaleParameter == SaleParameter.Value;
	}

	public bool IsPriceDiscount(BasicShopOfferBalancingData offer)
	{
		SaleOfferTupel offerSaleDetails = DIContainerLogic.GetSalesManagerService().GetOfferSaleDetails(offer.NameId);
		if (offerSaleDetails.IsEmpty())
		{
			return false;
		}
		return offerSaleDetails.OfferDetails.SaleParameter == SaleParameter.Price;
	}

	public bool IsSetBundle(BasicShopOfferBalancingData offer)
	{
		SaleOfferTupel offerSaleDetails = DIContainerLogic.GetSalesManagerService().GetOfferSaleDetails(offer.NameId);
		if (offerSaleDetails.IsEmpty())
		{
			return false;
		}
		return offerSaleDetails.SaleBalancing.ContentType == SaleContentType.SetBundle;
	}

	public bool GetActiveSaleDetailsForOffer(string offerNameId, out SaleOfferTupel saleOffer)
	{
		saleOffer = DIContainerLogic.GetSalesManagerService().GetOfferSaleDetails(offerNameId);
		if (saleOffer.IsEmpty())
		{
			return false;
		}
		return true;
	}
}

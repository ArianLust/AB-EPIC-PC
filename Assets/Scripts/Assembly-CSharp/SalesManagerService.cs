using System;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using Facebook.Unity;

internal class SalesManagerService
{
	private List<SalesManagerBalancingData> m_allSales = new List<SalesManagerBalancingData>();

	private List<SalesManagerBalancingData> m_activeSales = new List<SalesManagerBalancingData>();

	public List<SalesManagerBalancingData> ActiveSales
	{
		get
		{
			return m_activeSales;
		}
	}

	public bool HandleSpecialShopOffer(SalesManagerBalancingData saleBalancingData)
	{
		if (saleBalancingData.ContentType == SaleContentType.RainbowRiot)
		{
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			SaleItemDetails saleItemDetails = saleBalancingData.SaleDetails.FirstOrDefault();
			if (saleItemDetails == null)
			{
				return false;
			}
			string subjectId = saleItemDetails.SubjectId;
			int changedValue = saleItemDetails.ChangedValue;
			DIContainerLogic.GetShopService().HasRainbowRiot(currentPlayer);
			IInventoryItemGameData inventoryItemGameData = DIContainerLogic.InventoryService.AddItem(currentPlayer.InventoryGameData, 1, 1, subjectId, changedValue, "Special_Content_Sale");
			currentPlayer.Data.PendingFeatureUnlocks.Remove(subjectId);
			return true;
		}
		DebugLog.Error(GetType(), "HandleSpecialShopOffer: ContentType is invalid: saleBalancingData.ContentType == " + saleBalancingData.ContentType);
		return false;
	}

	public bool UpdateSales()
	{
		DateTime trustedTime;
		if (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			return false;
		}
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (currentPlayer.Data.SalesHistory == null)
		{
			currentPlayer.Data.SalesHistory = new Dictionary<string, DateTime>();
		}
		RemoveExpiredSales(currentPlayer);
		if (m_allSales.Count == 0)
		{
			m_allSales.AddRange(DIContainerBalancing.Service.GetBalancingDataList<SalesManagerBalancingData>());
		}
		for (int i = 0; i < m_allSales.Count; i++)
		{
			SalesManagerBalancingData salesManagerBalancingData = m_allSales[i];
			if (!ValidateSale(salesManagerBalancingData))
			{
				continue;
			}
			RegisterActiveSale(salesManagerBalancingData);
			if (!currentPlayer.Data.SalesHistory.ContainsKey(salesManagerBalancingData.NameId))
			{
				currentPlayer.Data.SalesHistory.Add(salesManagerBalancingData.NameId, DIContainerLogic.GetTimingService().GetPresentTime());
				if (salesManagerBalancingData.ContentType == SaleContentType.RainbowRiot)
				{
					HandleSpecialShopOffer(salesManagerBalancingData);
				}
			}
		}
		currentPlayer.SavePlayerData();
		return true;
	}

	private void RegisterActiveSale(SalesManagerBalancingData saleBalancing)
	{
		if (!m_activeSales.Exists((SalesManagerBalancingData sale) => sale.NameId == saleBalancing.NameId))
		{
			m_activeSales.Add(saleBalancing);
		}
	}

	private bool ValidateSale(SalesManagerBalancingData cSale)
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (!DIContainerLogic.RequirementService.CheckGenericRequirements(currentPlayer, cSale.Requirements))
		{
			return false;
		}
		if (!ValidateSaleContent(cSale))
		{
			return false;
		}
		if (!ValidateSaleType(cSale))
		{
			return false;
		}
		return true;
	}

	private bool ValidateSaleType(SalesManagerBalancingData cSale)
	{
		switch (cSale.SaleType)
		{
		case SaleAvailabilityType.Timed:
		case SaleAvailabilityType.PersonalTimeWindow:
		case SaleAvailabilityType.TimedSequence:
			return ValidateTimedSale(cSale);
		case SaleAvailabilityType.Conditional:
		case SaleAvailabilityType.ConditionalCooldown:
			return ValidateConditionalSale(cSale);
		default:
			return false;
		}
	}

	private bool ValidateConditionalSale(SalesManagerBalancingData cSale)
	{
		if ((cSale.SaleType == SaleAvailabilityType.ConditionalCooldown && cSale.Cooldown <= 0) || (cSale.SaleType == SaleAvailabilityType.Conditional && cSale.Cooldown > 0))
		{
			DebugLog.Error(GetType(), string.Format("ValidateConditionalSale: Invalid balancing parameters for sale {0}: Saletype={1}, Cooldown={2}", cSale.NameId, cSale.SaleType, cSale.Cooldown));
			return false;
		}
		DateTime value;
		if (cSale.SaleType == SaleAvailabilityType.ConditionalCooldown && DIContainerInfrastructure.GetCurrentPlayer().Data.SalesHistory.TryGetValue(cSale.NameId, out value))
		{
			DateTime dateTimeFromTimestamp = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(DIContainerLogic.GetTimingService().GetCurrentTimestamp());
			if (value.AddSeconds(cSale.Cooldown) > dateTimeFromTimestamp)
			{
				return false;
			}
		}
		else if (cSale.SaleType == SaleAvailabilityType.Conditional && cSale.Duration != 0)
		{
			return GetRemainingConditionalSaleDuration(cSale) > 0;
		}
		return true;
	}

	private bool ValidateTimedSale(SalesManagerBalancingData cSale)
	{
		uint currentTimestamp = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
		bool flag = currentTimestamp > cSale.StartTime && currentTimestamp < cSale.EndTime;
		if (!flag && cSale.SaleType == SaleAvailabilityType.PersonalTimeWindow)
		{
			DateTime value;
			DIContainerInfrastructure.GetCurrentPlayer().Data.SalesHistory.TryGetValue(cSale.NameId, out value);
			if (value.AddSeconds(cSale.Duration) > DIContainerLogic.GetTimingService().GetPresentTime())
			{
				return flag;
			}
		}
		return flag;
	}

	private bool ValidateSaleContent(SalesManagerBalancingData cSale)
	{
		switch (cSale.ContentType)
		{
		case SaleContentType.ShopItems:
		case SaleContentType.GenericBundle:
		case SaleContentType.ClassBundle:
		case SaleContentType.SetBundle:
		case SaleContentType.LuckyCoinDiscount:
		{
			if (cSale.SaleDetails == null || cSale.SaleDetails.Count == 0)
			{
				return false;
			}
			bool flag = false;
			foreach (BasicShopOfferBalancingData item in GetOfferBalancingsInSale(cSale))
			{
				if ((!item.UniqueOffer || !DIContainerInfrastructure.GetCurrentPlayer().Data.UniqueSpecialShopOffers.Contains(item.NameId)) && DIContainerLogic.GetShopService().IsOfferShowable(DIContainerInfrastructure.GetCurrentPlayer(), item))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return false;
			}
			break;
		}
		case SaleContentType.Mastery:
			if (cSale.SaleDetails == null || cSale.SaleDetails.Count == 0)
			{
				return false;
			}
			break;
		case SaleContentType.RainbowRiot:
			if (!cSale.ContainsShopOffer("special_offer_rainbow_riot_01") && !cSale.ContainsShopOffer("special_offer_rainbow_riot_02"))
			{
				return false;
			}
			break;
		}
		return true;
	}

	private List<BasicShopOfferBalancingData> GetOfferBalancingsInSale(SalesManagerBalancingData cSale)
	{
		List<BasicShopOfferBalancingData> list = new List<BasicShopOfferBalancingData>();
		foreach (SaleItemDetails saleDetail in cSale.SaleDetails)
		{
			PremiumShopOfferBalancingData balancing = null;
			BuyableShopOfferBalancingData balancing2 = null;
			if (!DIContainerBalancing.Service.TryGetBalancingData<PremiumShopOfferBalancingData>(saleDetail.SubjectId, out balancing))
			{
				DIContainerBalancing.Service.TryGetBalancingData<BuyableShopOfferBalancingData>(saleDetail.SubjectId, out balancing2);
			}
			if (balancing != null)
			{
				list.Add(balancing);
			}
			if (balancing2 != null)
			{
				list.Add(balancing2);
			}
		}
		return list;
	}

	private void RemoveExpiredSales(PlayerGameData player)
	{
		Dictionary<string, DateTime> dictionary = new Dictionary<string, DateTime>(player.Data.SalesHistory);
		foreach (KeyValuePair<string, DateTime> item in dictionary)
		{
			SalesManagerBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<SalesManagerBalancingData>(item.Key);
			if (balancingData == null || (balancingData != null && !ValidateSale(balancingData)))
			{
				DebugLog.Log(GetType(), "RemoveExpiredSales: removed the sale '" + item.Key + "'");
				m_activeSales.Remove(balancingData);
			}
		}
	}

	public List<SalesManagerBalancingData> GetAllActiveSales(bool sorted = false)
	{
		if (m_allSales == null)
		{
			return null;
		}
		if (m_activeSales == null)
		{
			return null;
		}
		List<SalesManagerBalancingData> list = m_activeSales.Where((SalesManagerBalancingData s) => DIContainerInfrastructure.GetCurrentPlayer().Data.SalesHistory.ContainsKey(s.NameId) && ValidateSale(s)).ToList();
		if (sorted && list.Count > 1)
		{
			list = list.OrderBy((SalesManagerBalancingData sale) => sale.SortPriority).ToList();
		}
		return list;
	}

	public void ClearSalesCache()
	{
		if (m_allSales != null)
		{
			m_allSales.Clear();
		}
	}

	public SalesManagerBalancingData GetActiveSaleForOffer(BasicShopOfferBalancingData offer)
	{
		for (int i = 0; i < ActiveSales.Count; i++)
		{
			SalesManagerBalancingData salesManagerBalancingData = ActiveSales[i];
			if (salesManagerBalancingData.ContainsShopOffer(offer.NameId))
			{
				return salesManagerBalancingData;
			}
		}
		return null;
	}

	private int GetRemainingTimedSaleDuration(SalesManagerBalancingData sale)
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		uint currentTimestamp = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
		if (currentTimestamp > sale.EndTime)
		{
			if (sale.SaleType != SaleAvailabilityType.PersonalTimeWindow)
			{
				return 0;
			}
			DateTime value;
			currentPlayer.Data.SalesHistory.TryGetValue(sale.NameId, out value);
			if (value.AddSeconds(sale.Duration) > DIContainerLogic.GetTimingService().GetPresentTime())
			{
				long num = currentTimestamp - value.TotalSeconds();
				return (int)(sale.Duration - num);
			}
		}
		return (int)(sale.EndTime - currentTimestamp);
	}

	private int GetRemainingConditionalSaleDuration(SalesManagerBalancingData sale)
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		uint currentTimestamp = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
		DateTime value;
		if (currentPlayer.Data.SalesHistory.TryGetValue(sale.NameId, out value))
		{
			return (int)(value.AddSeconds(sale.Duration).TotalSeconds() - currentTimestamp);
		}
		return sale.Duration;
	}

	public int GetRemainingSaleDuration(SalesManagerBalancingData sale)
	{
		if (sale == null)
		{
			return 0;
		}
		int result = 0;
		switch (sale.SaleType)
		{
		case SaleAvailabilityType.Timed:
		case SaleAvailabilityType.TimedSequence:
			result = GetRemainingTimedSaleDuration(sale);
			break;
		case SaleAvailabilityType.PersonalTimeWindow:
		case SaleAvailabilityType.Conditional:
		case SaleAvailabilityType.ConditionalCooldown:
			result = GetRemainingConditionalSaleDuration(sale);
			break;
		}
		return result;
	}

	public int GetRemainingSaleDuration(BasicShopOfferBalancingData shopOfferBalancing)
	{
		SalesManagerBalancingData activeSaleForOffer = GetActiveSaleForOffer(shopOfferBalancing);
		if (activeSaleForOffer == null)
		{
			return 0;
		}
		return GetRemainingSaleDuration(activeSaleForOffer);
	}

	public SaleOfferTupel GetOfferSaleDetails(string shopOfferId)
	{
		List<SalesManagerBalancingData> allActiveSales = GetAllActiveSales(true);
		SaleOfferTupel result = default(SaleOfferTupel);
		foreach (SalesManagerBalancingData item in allActiveSales)
		{
			SaleItemDetails saleItemDetails = item.SaleDetails.Find((SaleItemDetails details) => details.SubjectId == shopOfferId);
			if (saleItemDetails != null)
			{
				result.OfferDetails = saleItemDetails;
				result.SaleBalancing = item;
				break;
			}
		}
		return result;
	}

	public bool IsItemOnSale(string needleId)
	{
		for (int i = 0; i < ActiveSales.Count; i++)
		{
			SalesManagerBalancingData salesManagerBalancingData = ActiveSales[i];
			for (int j = 0; j < salesManagerBalancingData.SaleDetails.Count; j++)
			{
				SaleItemDetails saleItemDetails = salesManagerBalancingData.SaleDetails[j];
				if (saleItemDetails.SubjectId == needleId)
				{
					return ValidateSale(salesManagerBalancingData);
				}
			}
		}
		return false;
	}

	public bool IsShopSaleActive()
	{
		foreach (SalesManagerBalancingData activeSale in m_activeSales)
		{
			if (activeSale.ContentType == SaleContentType.ShopItems || activeSale.ContentType == SaleContentType.ClassBundle || activeSale.ContentType == SaleContentType.GenericBundle || activeSale.ContentType == SaleContentType.LuckyCoinDiscount || activeSale.ContentType == SaleContentType.SetBundle)
			{
				return true;
			}
		}
		return false;
	}
}

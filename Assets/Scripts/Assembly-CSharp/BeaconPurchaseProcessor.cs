using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using Interfaces.Purchasing;
using Rcs;

public class BeaconPurchaseProcessor
{
	private readonly Dictionary<string, KeyValuePair<int, Purchase>> m_purchases = new Dictionary<string, KeyValuePair<int, Purchase>>();

	private readonly Dictionary<string, Purchase> m_pendingPurchases = new Dictionary<string, Purchase>();

	private bool m_fetchWalletRunning;

	private uint m_lastWalletFetchTimestamp;

	[method: MethodImpl(32)]
	public event Action<List<IInventoryItemGameData>> RewardedItemsFromBonusCode;

	public BeaconPurchaseProcessor()
	{
		DIContainerInfrastructure.PurchasingService.FetchWalletSuccess += OnFetchWalletSuccess;
		DIContainerInfrastructure.PurchasingService.FetchWalletError += OnFetchWalletError;
		DIContainerInfrastructure.PurchasingService.ConsumeVoucherSuccess += OnConsumeVoucherSuccess;
		DIContainerInfrastructure.PurchasingService.ConsumeVoucherError += OnConsumeVoucherError;
		DIContainerInfrastructure.PurchasingService.PurchaseProgress += PurchasingService_PurchaseProgress;
	}

	protected virtual void OnRewardedItemsFromBonusCode(List<IInventoryItemGameData> items)
	{
		Action<List<IInventoryItemGameData>> rewardedItemsFromBonusCode = this.RewardedItemsFromBonusCode;
		if (rewardedItemsFromBonusCode != null)
		{
			rewardedItemsFromBonusCode(items);
		}
	}

	private void PurchasingService_PurchaseProgress(Purchase purchaseInfo)
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		string text = string.Empty;
		foreach (ThirdPartyIdBalancingData balancingData2 in DIContainerBalancing.Service.GetBalancingDataList<ThirdPartyIdBalancingData>())
		{
			if (balancingData2.PaymentProductId == purchaseInfo.productID)
			{
				text = balancingData2.NameId;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			DebugLog.Error("No Item mapping found for " + purchaseInfo.productID);
			return;
		}
		PremiumShopOfferBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<PremiumShopOfferBalancingData>(text);
		if (balancingData == null)
		{
			DebugLog.Error("No Item mapping found for " + purchaseInfo.productID);
			return;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		switch (purchaseInfo.status)
		{
		case PurchaseStatus.PURCHASE_SUCCEEDED:
		{
			dictionary.Add("TypeOfGain", "ShopOfferBought");
			RemoveWaitingPurchase(purchaseInfo);
			Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
			uint currentTimestamp = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
			RewardLoot(balancingData, currentPlayer, dictionary);
			AddPurchase(purchaseInfo);
			if ((bool)DIContainerInfrastructure.GetCoreStateMgr().m_CronJob)
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_CronJob.Run();
			}
			if (text.Contains("_playmob"))
			{
				DIContainerInfrastructure.GetPlaymobService().TrackPurchase(false);
			}
			LogIapToAnalytics(purchaseInfo);
			break;
		}
		case PurchaseStatus.PURCHASE_FAILED:
			RemoveWaitingPurchase(purchaseInfo);
			break;
		case PurchaseStatus.PURCHASE_CANCELED:
			RemoveWaitingPurchase(purchaseInfo);
			DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("shop_purchase_canceled", "Purchase Product has been canceled!"), "shop_purchase_canceled", DispatchMessage.Status.Info);
			break;
		case PurchaseStatus.PURCHASE_PENDING:
			break;
		case PurchaseStatus.PURCHASE_REFUNDED:
			RemoveWaitingPurchase(purchaseInfo);
			break;
		case PurchaseStatus.PURCHASE_RESTORED:
			RemoveWaitingPurchase(purchaseInfo);
			dictionary.Add("TypeOfGain", "ShopOfferRestored");
			RewardLoot(balancingData, currentPlayer, dictionary);
			break;
		case PurchaseStatus.PURCHASE_WAITING:
			AddWaitingPurchase(purchaseInfo);
			DebugLog.Warn("Purchase Waiting!");
			break;
		}
	}

	private void RewardLoot(PremiumShopOfferBalancingData offer, PlayerGameData player, Dictionary<string, string> values)
	{
		values.Add("OfferName", offer.NameId);
		values.Add("OfferType", offer.Category);
		values.Add("PlayerLevel", player.Data.Level.ToString());
		Dictionary<string, LootInfoData> dictionary = DIContainerLogic.GetLootOperationService().GenerateLoot(offer.OfferContents, player.Data.Level + 2);
		foreach (KeyValuePair<string, LootInfoData> item in dictionary)
		{
			SaleOfferTupel saleOffer;
			if (DIContainerLogic.GetShopService().GetActiveSaleDetailsForOffer(offer.NameId, out saleOffer) && saleOffer.OfferDetails.SaleParameter == SaleParameter.Value)
			{
				item.Value.Value = saleOffer.OfferDetails.ChangedValue;
			}
			if (item.Key.StartsWith("class_") || item.Key.StartsWith("recipe_"))
			{
				item.Value.Level = 1;
			}
		}
		List<IInventoryItemGameData> list = DIContainerLogic.GetLootOperationService().RewardLoot(player.InventoryGameData, 2, dictionary, values, EquipmentSource.Gatcha);
		player.SavePlayerData();
	}

	private void LogIapToAnalytics(Purchase purchaseInfo)
	{
		DebugLog.Log(GetType(), "LogIapToAnalytics: Get Catalog from server");
		List<Product> catalog = DIContainerInfrastructure.PurchasingService.GetCatalog();
		Product product = catalog.FirstOrDefault((Product p) => p.productId == purchaseInfo.productID);
		string value = "USD";
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("ProductId", purchaseInfo.productID);
		dictionary.Add("Price", product.referencePrice.ToString());
		dictionary.Add("Currency", value);
		dictionary.Add("ReceiptId", purchaseInfo.receiptID);
		if (!DIContainerInfrastructure.GetCurrentPlayer().Data.IsUserConverted)
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.IsUserConverted = true;
			dictionary.Add("UserConverted", DIContainerInfrastructure.GetCurrentPlayer().Data.IsUserConverted.ToString());
			DIContainerInfrastructure.GetAnalyticsSystem().LogEvent("UserConverted", false);
		}
		ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("IapBought", dictionary);
		DIContainerInfrastructure.GetAttributionService().TrackSaleEvent(Convert.ToDouble(product.referencePrice), purchaseInfo.transactionID);
	}

	public void CheckForVoucherChanges()
	{
		DebugLog.Log(GetType(), "CheckForVoucherChanges, m_fetchWalletRunning == " + m_fetchWalletRunning);
		if (!m_fetchWalletRunning)
		{
			StartFetchingWallet();
		}
		else if (DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp() - m_lastWalletFetchTimestamp > 120)
		{
			DebugLog.Log(GetType(), "m_fetchWalletRunning == true, but more than 120 seconds old; assume timeout and try again...");
			m_fetchWalletRunning = false;
			CheckForVoucherChanges();
		}
	}

	private void StartFetchingWallet()
	{
		m_fetchWalletRunning = true;
		DIContainerInfrastructure.PurchasingService.FetchWallet();
		m_lastWalletFetchTimestamp = DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp();
	}

	public void AddPurchase(Purchase purchase)
	{
		DebugLog.Log(GetType(), "AddPurchase, productId == " + purchase.productID);
		int num = 0;
		KeyValuePair<int, Purchase> value;
		if (m_purchases.TryGetValue(purchase.productID, out value))
		{
			num = value.Key;
		}
		m_purchases.Remove(purchase.productID);
		m_purchases.Add(purchase.productID, new KeyValuePair<int, Purchase>(num + 1, purchase));
		CheckForVoucherChanges();
	}

	private void AssignLoot(string productId, Payment.Voucher.SourceType voucherSourceType)
	{
		DebugLog.Log(GetType(), "AssignLoot, productId == " + productId);
		string text = string.Empty;
		foreach (ThirdPartyIdBalancingData balancingData2 in DIContainerBalancing.Service.GetBalancingDataList<ThirdPartyIdBalancingData>())
		{
			if (balancingData2.PaymentProductId == productId)
			{
				text = balancingData2.NameId;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			DebugLog.Error("No Item mapping found for " + productId);
			return;
		}
		PremiumShopOfferBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<PremiumShopOfferBalancingData>(text);
		if (balancingData == null)
		{
			DebugLog.Error("No Shop mapping found for " + productId);
			return;
		}
		DebugLog.Log(GetType(), "AssignLoot, nameId == " + text);
		List<IInventoryItemGameData> items = DIContainerLogic.GetLootOperationService().RewardLootGetInputCopy(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 2, DIContainerLogic.GetLootOperationService().GenerateLoot(balancingData.OfferContents, DIContainerLogic.GetShopService().GetOfferLevel(DIContainerInfrastructure.GetCurrentPlayer().Data.Level, balancingData)), new Dictionary<string, string>
		{
			{
				"TypeOfGain",
				voucherSourceType.ToString()
			},
			{ "OfferName", balancingData.NameId }
		}, EquipmentSource.Gatcha);
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		switch (voucherSourceType)
		{
		case Payment.Voucher.SourceType.Purchase:
			UpdateGuiAfterDelayedPurchase(items);
			break;
		case Payment.Voucher.SourceType.Reward:
			UpdateGuiAfterReward(items);
			break;
		case Payment.Voucher.SourceType.Donation:
			UpdateGuiAfterDonation(items);
			break;
		case Payment.Voucher.SourceType.Codes:
			UpdateGuiAfterCodeReward(items);
			break;
		case Payment.Voucher.SourceType.Other:
			break;
		}
	}

	private void UpdateGuiAfterDelayedPurchase(List<IInventoryItemGameData> items)
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateLuckyCoinsBar();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateCoinsBar();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateFriendshipEssenceBar();
	}

	private void UpdateGuiAfterCodeReward(List<IInventoryItemGameData> items)
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateLuckyCoinsBar();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateCoinsBar();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateFriendshipEssenceBar();
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfoAndLootItems(DIContainerInfrastructure.GetLocaService().Tr("toast_voucher_code", "You got a present:"), items, items.Aggregate(string.Empty, (string acc, IInventoryItemGameData item) => acc + item.ItemAssetName));
	}

	private void UpdateGuiAfterReward(List<IInventoryItemGameData> items)
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateLuckyCoinsBar();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateCoinsBar();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateFriendshipEssenceBar();
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfoAndLootItems(DIContainerInfrastructure.GetLocaService().Tr("toast_voucher_code", "You got a present:"), items, items.Aggregate(string.Empty, (string acc, IInventoryItemGameData item) => acc + item.ItemAssetName));
	}

	private void UpdateGuiAfterDonation(List<IInventoryItemGameData> items)
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateLuckyCoinsBar();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateCoinsBar();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateFriendshipEssenceBar();
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfoAndLootItems(DIContainerInfrastructure.GetLocaService().Tr("toast_donation", "You got a donation:"), items, items.Aggregate(string.Empty, (string acc, IInventoryItemGameData item) => acc + item.ItemAssetName));
	}

	private void ProcessWallet()
	{
		PurchasingServiceBeaconImpl purchasingServiceBeaconImpl = DIContainerInfrastructure.PurchasingService as PurchasingServiceBeaconImpl;
		if (purchasingServiceBeaconImpl == null)
		{
			DebugLog.Log(GetType(), "ProcessWallet: Could not cast PurchasingService to PurchasingServiceBeaconImpl, aborting!");
			return;
		}
		List<Payment.Voucher> vouchers = purchasingServiceBeaconImpl.m_payment.GetVouchers();
		Payment.Voucher voucher;
		foreach (Payment.Voucher item in vouchers)
		{
			voucher = item;
			DebugLog.Log(GetType(), "ProcessWallet: contents for voucher productId = " + voucher.GetProductId() + ", voucherId = " + voucher.GetId() + ", contents: " + voucher.GetClientData().Aggregate(string.Empty, (string acc, KeyValuePair<string, string> kvp) => acc + "[" + kvp.Key + " -> " + kvp.Value + "]"));
			KeyValuePair<int, Purchase> value;
			if (m_pendingPurchases.Count((KeyValuePair<string, Purchase> a) => a.Value.productID == voucher.GetProductId()) > 0)
			{
				DebugLog.Log(GetType(), "There is a pending purchase for a voucher of the same name. Skipping this voucher for now. productId: " + voucher.GetProductId());
			}
			else if (voucher.GetSourceType() == Payment.Voucher.SourceType.Purchase && m_purchases.TryGetValue(voucher.GetProductId(), out value))
			{
				RemovePurchase(value.Value);
				DebugLog.Log(GetType(), string.Concat("ProcessWallet: Voucher-Product match. Consuming the voucher. productID: ", value.Value.productID, ", receiptID: ", value.Value.receiptID, ", status: ", value.Value.status, ", statusString: ", value.Value.statusString, ", transactionID: ", value.Value.transactionID));
				purchasingServiceBeaconImpl.m_payment.ConsumeVoucher(voucher, purchasingServiceBeaconImpl.OnConsumeSuccessMsg, purchasingServiceBeaconImpl.OnConsumeErrorMsg);
				DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_purchase_success", "Purchase successful"), "purchase_success", DispatchMessage.Status.Info);
			}
			else
			{
				DebugLog.Log(GetType(), string.Concat("ProcessWallet: Voucher Type: ", voucher.GetSourceType(), ". Assigning loot to the player and consuming the voucher"));
				AssignLoot(voucher.GetProductId(), voucher.GetSourceType());
				purchasingServiceBeaconImpl.m_payment.ConsumeVoucher(voucher, purchasingServiceBeaconImpl.OnConsumeSuccessMsg, purchasingServiceBeaconImpl.OnConsumeErrorMsg);
			}
		}
	}

	private void RemovePurchase(Purchase purchase)
	{
		DebugLog.Log(GetType(), "RemovePurchase, productId == " + purchase.productID);
		int num = 0;
		KeyValuePair<int, Purchase> value;
		if (m_purchases.TryGetValue(purchase.productID, out value))
		{
			num = value.Key;
		}
		m_purchases.Remove(purchase.productID);
		if (num > 1)
		{
			m_purchases.Add(purchase.productID, new KeyValuePair<int, Purchase>(num - 1, purchase));
		}
	}

	private void ConsumeVoucher(Voucher voucher)
	{
		DebugLog.Log(GetType(), "ConsumeVoucher, productId == " + voucher.productId);
		DIContainerInfrastructure.PurchasingService.ConsumeVoucher(voucher.voucherId);
	}

	private void OnFetchWalletError(Payment.ErrorCode error, string managedString)
	{
		DebugLog.Error(GetType(), string.Concat("OnFetchWalletError, error = ", error, ", managedString = ", managedString));
		m_fetchWalletRunning = false;
	}

	private void OnFetchWalletSuccess(string managedString)
	{
		DebugLog.Log(GetType(), "OnFetchWalletSuccess, managedString = " + managedString);
		m_fetchWalletRunning = false;
		ProcessWallet();
	}

	private void OnConsumeVoucherError(Payment.ErrorCode error, string managedString)
	{
		DebugLog.Error(GetType(), string.Concat("OnFetchWalletError, error = ", error, ", managedString = ", managedString));
	}

	private void OnConsumeVoucherSuccess(string managedString)
	{
		DebugLog.Log(GetType(), "OnConsumeVoucherSuccess, managedString = " + managedString);
	}

	private void AddWaitingPurchase(Purchase purchaseInfo)
	{
		if (!m_pendingPurchases.ContainsKey(purchaseInfo.transactionID))
		{
			m_pendingPurchases.Add(purchaseInfo.transactionID, purchaseInfo);
		}
	}

	private void RemoveWaitingPurchase(Purchase purchaseInfo)
	{
		if (m_pendingPurchases.ContainsKey(purchaseInfo.transactionID))
		{
			m_pendingPurchases.Remove(purchaseInfo.transactionID);
		}
	}
}

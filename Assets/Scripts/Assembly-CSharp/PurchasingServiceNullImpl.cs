using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ABH.Shared.BalancingData;
using Interfaces.Purchasing;
using Rcs;

public class PurchasingServiceNullImpl : IPurchasingService
{
	private bool m_Initialized;
	
	public bool AutoRestorePurchasesAfterInit { get; set; }

	[method: MethodImpl(32)]
	public event Action<string> InitializeSuccess;

	[method: MethodImpl(32)]
	public event Action<Payment.Info> InitializeProgress;

	[method: MethodImpl(32)]
	public event Action<Payment.ErrorCode, string> InitializeError;

	[method: MethodImpl(32)]
	public event Action<string> SelectProviderSuccess;

	[method: MethodImpl(32)]
	public event Action<Purchase> PurchaseProgress;

	[method: MethodImpl(32)]
	public event Action<string> RestorePurchaseCompletion;

	[method: MethodImpl(32)]
	public event Action<Purchase> RestorePurchaseProgress;

	[method: MethodImpl(32)]
	public event Action<List<string>> PurchaseHistoryRetrive;

	[method: MethodImpl(32)]
	public event Action<string> FetchWalletSuccess;

	[method: MethodImpl(32)]
	public event Action<Payment.ErrorCode, string> FetchWalletError;

	[method: MethodImpl(32)]
	public event Action<string> MoveWalletSuccess;

	[method: MethodImpl(32)]
	public event Action<Payment.ErrorCode, string> MoveWalletError;

	[method: MethodImpl(32)]
	public event Action<string> ConsumeVoucherSuccess;

	[method: MethodImpl(32)]
	public event Action<string> PurchaseSuccess;

	[method: MethodImpl(32)]
	public event Action<Payment.ErrorCode, string> PurchaseError;

	[method: MethodImpl(32)]
	public event Action<Payment.ErrorCode, string> ConsumeVoucherError;

	[method: MethodImpl(32)]
	public event Action<string> ProcessRedeemCode;

	[method: MethodImpl(32)]
	public event Action<string, string> RedeemCodeSucces;

	[method: MethodImpl(32)]
	public event Action<Payment.ErrorCode, string> RedeemCodeError;

	public void Initialize(string bundleId)
	{
		DebugLog.Log("[PurchasingServiceNullImpl] Initialize " + bundleId);
		DIContainerInfrastructure.GetPurchaseProcessor();
		OnInitSuccessfulMsg("null");
		m_Initialized = true;
	}

	public bool IsSupported()
	{
		return true;
	}

	public bool IsEnabled()
	{
		return true;
	}

	public bool IsInitialized()
	{
		return m_Initialized;
	}

	public void RestorePurchases()
	{
		OnRestoreCompleteMsg("succes");
	}

	public void FetchWallet()
	{
		OnFetchSuccessMsg("null");
	}

	public string GetCurrentProvider()
	{
		return "default";
	}

	public List<string> GetProviders()
	{
		List<string> list = new List<string>();
		list.Add("default");
		return list;
	}

	public List<Product> GetCatalogFromServer()
	{
		return GetCatalog();
	}

	public List<Product> GetCatalog()
	{
		List<Product> list = new List<Product>();
		string[] array = new string[11]
		{
			"4,99 €", "9,99 €", "19,99 €", "49,99 €", "99,99 €", "0,19 €", "0,00 €", "1,99 €", "4,99 €", "9,99 €",
			"24.99 €"
		};
		int num = 0;
		foreach (PremiumShopOfferBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<PremiumShopOfferBalancingData>())
		{
			float result = 0f;
			float.TryParse(array[num].Replace(" €", string.Empty).Replace(",", "."), out result);
			list.Add(new Product
			{
				name = DIContainerInfrastructure.GetLocaService().GetShopOfferName(balancingData.LocaId),
				description = DIContainerInfrastructure.GetLocaService().GetShopOfferDesc(balancingData.LocaId),
				productId = DIContainerBalancing.Service.GetBalancingData<ThirdPartyIdBalancingData>(balancingData.NameId).PaymentProductId,
				price = array[num],
				referencePrice = result,
				type = "offer",
				providerId = balancingData.Category
			});
			num = (num + 1) % array.Length;
		}
		return list;
	}

	public List<Voucher> GetVouchers()
	{
		return new List<Voucher>();
	}

	public void ConsumeVoucher(string voucherId)
	{
		OnConsumeSuccessMsg(voucherId);
	}

	public void PurchaseProduct(string productId)
	{
		Purchase obj = default(Purchase);
		obj.productID = productId;
		obj.receiptID = "null-user";
		obj.status = PurchaseStatus.PURCHASE_SUCCEEDED;
		obj.statusString = string.Empty;
		obj.transactionID = "0";
		if (this.PurchaseProgress != null)
		{
			this.PurchaseProgress(obj);
		}
	}

	public void OnInitSuccessfulMsg(string statusManagedString)
	{
		DebugLog.Log("OnInitSuccessfulMsg called " + statusManagedString);
		if (this.InitializeSuccess != null)
		{
			this.InitializeSuccess(statusManagedString);
		}
	}

	public void OnInitProgressMsg(Payment.Info info)
	{
		DebugLog.Log("OnInitProgressMsg called " + info.GetProductId() + " " + info.GetStatus());
		if (this.InitializeProgress != null)
		{
			this.InitializeProgress(info);
		}
	}

	public void OnInitErrorMsg(Payment.ErrorCode error, string statusManagedString)
	{
		DebugLog.Log("OnInitErrorMsg called " + statusManagedString + " ErrorID: " + error);
		if (this.InitializeError != null)
		{
			this.InitializeError(error, statusManagedString);
		}
	}

	public void OnProviderSelectedSuccessMsg(string managedString)
	{
		DebugLog.Log("OnProviderSelectedSuccessMsg called " + managedString);
		if (this.SelectProviderSuccess != null)
		{
			this.SelectProviderSuccess(managedString);
		}
	}

	public void OnRestoreProgressMsg(Purchase info)
	{
		DebugLog.Log("OnRestoreProgressMsg called " + info.productID + " " + info.status);
		if (this.RestorePurchaseProgress != null)
		{
			this.RestorePurchaseProgress(info);
		}
	}

	public void OnRestoreCompleteMsg(string result)
	{
		DebugLog.Log("OnRestoreCompleteMsg called and succeded: " + result);
		if (this.RestorePurchaseCompletion != null)
		{
			this.RestorePurchaseCompletion(result);
		}
	}

	public void OnFetchSuccessMsg(string managedString)
	{
		DebugLog.Log("OnFetchSuccessMsg called " + managedString);
		if (this.FetchWalletSuccess != null)
		{
			this.FetchWalletSuccess(managedString);
		}
	}

	public void OnFetchErrorMsg(Payment.ErrorCode error, string managedString)
	{
		DebugLog.Log("OnFetchErrorMsg called " + managedString + " ErrorID: " + error);
		if (this.FetchWalletError != null)
		{
			this.FetchWalletError(error, managedString);
		}
	}

	public void OnPurchaseProgressMsg(Payment.Info info)
	{
		DebugLog.Log("OnPurchaseProgressMsg called " + info.GetProductId() + " " + info.GetStatus());
		if (this.PurchaseProgress != null)
		{
			this.PurchaseProgress(HatchHelper.CreatePurchaseFromInfo(info));
		}
	}

	public void OnConsumeSuccessMsg(string managedString)
	{
		DebugLog.Log("OnProviderSelectedSuccessMsg called " + managedString);
		if (this.ConsumeVoucherSuccess != null)
		{
			this.ConsumeVoucherSuccess(managedString);
		}
	}

	public void OnConsumeErrorMsg(Payment.ErrorCode error, string managedString)
	{
		DebugLog.Log("OnConsumeErrorMsg called " + managedString + " ErrorID: " + error);
		if (this.ConsumeVoucherError != null)
		{
			this.ConsumeVoucherError(error, managedString);
		}
	}

	public bool IsInitializing()
	{
		return false;
	}

	public IPurchasingService SetPaymentProvider(string paymentProvider)
	{
		return this;
	}
	
	public bool RedeemCode(string code)
	{
		this.OnRedeemCodeSucces(code, "com.rovio.gold.luckycoins.1");
		return true;
	}
	
	public void OnRedeemCodeSucces(string code, string voucherId)
	{
		if (this.RedeemCodeSucces != null)
		{
			this.RedeemCodeSucces(code, voucherId);
		}
	}

	public void OnRedeemCodeError(Payment.ErrorCode status, string code)
	{
		if (this.RedeemCodeError != null)
		{
			this.RedeemCodeError(status, code);
		}
	}

	public void OnPurchaseError(Payment.ErrorCode status, Payment.Info failedPurchase)
	{
		if (this.PurchaseError != null)
		{
			this.PurchaseError(status, failedPurchase.GetStatus().ToString());
		}
	}

	public void OnPurchaseSuccess(Payment.Info succeededPurchase)
	{
		if (this.PurchaseSuccess != null)
		{
			this.PurchaseSuccess(succeededPurchase.GetProductId());
		}
	}
}

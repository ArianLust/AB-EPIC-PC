using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Interfaces.Purchasing;
using Rcs;

public class PurchasingServiceBeaconImpl : IPurchasingService
{
	private bool m_initializing;

	private string m_paymentProvider = string.Empty;

	public Payment m_payment;

	private List<Product> m_CatalogCache;

	private Dictionary<string, Payment.Product> m_beaconProducts;

	private Dictionary<string, Payment.Voucher> m_beaconVouchers;

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
	public event Action<string> PurchaseSuccess;

	[method: MethodImpl(32)]
	public event Action<Payment.ErrorCode, string> PurchaseError;

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
	public event Action<Payment.ErrorCode, string> ConsumeVoucherError;

	[method: MethodImpl(32)]
	public event Action<string, string> RedeemCodeSucces;

	[method: MethodImpl(32)]
	public event Action<Payment.ErrorCode, string> RedeemCodeError;

	public IPurchasingService SetPaymentProvider(string paymentProvider)
	{
		m_paymentProvider = paymentProvider;
		return this;
	}

	public void Initialize(string bundleId)
	{
		DebugLog.Log(GetType(), "Initialize Beacon with bundle Id: " + bundleId);
		if (IsInitialized())
		{
			DebugLog.Log(GetType(), "Initialize: Already initialized. Returning...");
			return;
		}
		m_beaconProducts = new Dictionary<string, Payment.Product>();
		m_beaconVouchers = new Dictionary<string, Payment.Voucher>();
		m_payment = new Payment(ContentLoader.Instance.m_BeaconConnectionMgr.Identiy, Payment.CatalogBackend.FlightdeckcatalogBackend, bundleId, m_paymentProvider);
		m_payment.Initialize(OnInitSuccessfulMsg, OnInitErrorMsg, OnInitProgressMsg);
		DIContainerInfrastructure.GetPurchaseProcessor();
		DebugLog.Log(GetType(), "INITIALIZE: payment provider = " + GetCurrentProvider());
		m_initializing = true;
	}

	public bool IsSupported()
	{
		return Payment.IsSupported();
	}

	public bool IsEnabled()
	{
		return m_payment.IsEnabled();
	}

	public bool IsInitializing()
	{
		return m_initializing;
	}

	public bool IsInitialized()
	{
		return m_payment != null && m_payment.IsInitialized();
	}

	public void RestorePurchases()
	{
		if ((m_payment.GetCapabilities() & Payment.PaymentCapabilities.CapabilityFlagRestore) == Payment.PaymentCapabilities.CapabilityFlagRestore)
		{
			DebugLog.Log(GetType(), "calling Rcs.Payment.RestorePurchases()");
			m_payment.RestorePurchases(OnRestorePurchasesSuccess, OnRestorePurchasesError);
		}
		else
		{
			DebugLog.Warn(GetType(), "RestorePurchases: Restore not available for this payment provider!");
		}
	}

	private void OnRestorePurchasesSuccess(string providerName)
	{
		DebugLog.Log(GetType(), "OnRestorePurchasesSuccess: providerName = " + providerName);
		OnRestoreCompleteMsg("success");
	}

	private void OnRestorePurchasesError(Payment.ErrorCode status, string errorMessage)
	{
		DebugLog.Log(GetType(), string.Concat("OnRestorePurchasesError: status = ", status, "  :  ", errorMessage));
		OnRestoreCompleteMsg(string.Empty);
	}

	public void FetchWallet()
	{
		Payment.ErrorCode errorCode = m_payment.FetchWallet();
		if (errorCode == Payment.ErrorCode.NoError)
		{
			OnFetchSuccessMsg(errorCode.ToString());
		}
		else
		{
			OnFetchErrorMsg(errorCode, errorCode.ToString());
		}
		DebugLog.Log(GetType(), "Rcs.Payment.FetchWallet() returned: " + errorCode);
		Payment.ErrorCode errorCode2 = m_payment.FetchCatalog();
		DebugLog.Log(GetType(), "Rcs.Payment.FetchCatalog() returned: " + errorCode2);
	}

	public string GetCurrentProvider()
	{
		return m_payment.GetProviderName();
	}

	public List<string> GetProviders()
	{
		return Payment.GetProviders();
	}

	public List<Product> GetCatalogFromServer()
	{
		List<Payment.Product> catalog = m_payment.GetCatalog();
		m_beaconProducts.Clear();
		if (m_CatalogCache == null)
		{
			m_CatalogCache = new List<Product>();
		}
		foreach (Payment.Product item in catalog)
		{
			DebugLog.Log(GetType(), "GetCatalog: got pid --- " + item.GetId() + " --- for --- " + item.GetProviderId());
			m_CatalogCache.Add(new Product
			{
				clientData = item.GetClientData(),
				description = item.GetDescription(),
				name = item.GetName(),
				price = item.GetPrice(),
				referencePrice = item.GetReferencePrice(),
				productId = item.GetId(),
				providerData = item.GetProviderData(),
				providerId = item.GetProviderId(),
				token = item.GetToken(),
				type = item.GetProductType().ToString()
			});
			m_beaconProducts.Add(item.GetId(), new Payment.Product(item));
		}
		return m_CatalogCache;
	}

	public List<Product> GetCatalog()
	{
		if (m_beaconProducts == null || m_CatalogCache == null || m_beaconProducts.Count == 0 || m_CatalogCache.Count == 0)
		{
			DebugLog.Log(GetType(), "GetCatalog: No catalog data cached => Requesting from server...");
			GetCatalogFromServer();
		}
		return m_CatalogCache;
	}

	public List<Voucher> GetVouchers()
	{
		DebugLog.Log(GetType(), "GetVouchers");
		List<Payment.Voucher> vouchers = m_payment.GetVouchers();
		List<Voucher> list = new List<Voucher>();
		DebugLog.Log(GetType(), "GetVouchers iterating over the voucher list");
		foreach (Payment.Voucher item in vouchers)
		{
			list.Add(new Voucher
			{
				clientData = item.GetClientData(),
				isConsumable = item.IsConsumable(),
				productId = item.GetProductId(),
				voucherId = item.GetId(),
				sourceType = (SourceType)item.GetSourceType(),
				sourceId = item.GetSourceId()
			});
			m_beaconVouchers.Add(item.GetProductId(), item);
		}
		DebugLog.Log(GetType(), "GetVouchers done, returning " + list.Count + " elements");
		return list;
	}

	private Payment.Voucher GetVoucherById(string id)
	{
		if (m_beaconVouchers.ContainsKey(id))
		{
			return m_beaconVouchers[id];
		}
		return null;
	}

	private Payment.Product GetProductById(string id)
	{
		if (m_beaconProducts.ContainsKey(id))
		{
			DebugLog.Log(GetType(), "GetProductById: Search for " + id + " SUCCESSFUL: " + m_beaconProducts[id].GetName());
			return m_beaconProducts[id];
		}
		DebugLog.Log(GetType(), "GetProductById: Searching for " + id + " FAILED. Products count = " + m_beaconProducts.Count);
		return null;
	}

	public void ConsumeVoucher(string voucherId)
	{
		m_payment.ConsumeVoucher(GetVoucherById(voucherId), OnConsumeSuccessMsg, OnConsumeErrorMsg);
	}

	public bool RedeemCode(string code)
	{
		//Payment.ErrorCode errorCode = m_payment.RedeemCode(code, OnRedeemCodeSucces, OnRedeemCodeError);
		//return errorCode == Payment.ErrorCode.NoError;
		return true;
	}

	public void PurchaseProduct(string productId)
	{
		DebugLog.Log(GetType(), "PurchaseProduct");
		m_payment.PurchaseProduct(GetProductById(productId), OnPurchaseSuccess, OnPurchaseError, OnPurchaseProgressMsg);
	}

	public void OnInitSuccessfulMsg(string statusManagedString)
	{
		m_initializing = false;
		DebugLog.Log(GetType(), "OnInitSuccessfulMsg: " + statusManagedString);
		if (this.InitializeSuccess != null)
		{
			this.InitializeSuccess(statusManagedString);
		}
	}

	public void OnInitProgressMsg(Payment.Info info)
	{
		DebugLog.Log(GetType(), "OnInitProgressMsg: " + info.GetProductId());
		if (this.InitializeProgress != null)
		{
			this.InitializeProgress(info);
		}
	}

	public void OnInitErrorMsg(Payment.ErrorCode errorCode, string errorMessage)
	{
		DebugLog.Error(GetType(), string.Concat("OnInitErrorMsg: ", errorCode, ", ", errorMessage));
		m_initializing = false;
		if (this.InitializeError != null)
		{
			this.InitializeError(errorCode, errorMessage);
		}
	}

	public void OnProviderSelectedSuccessMsg(string managedString)
	{
		m_initializing = true;
		m_initializing = false;
		if (this.SelectProviderSuccess != null)
		{
			this.SelectProviderSuccess(managedString);
		}
	}

	public void OnRestoreProgressMsg(Purchase info)
	{
		if (this.RestorePurchaseProgress != null)
		{
			this.RestorePurchaseProgress(info);
		}
	}

	public void OnRestoreCompleteMsg(string result)
	{
		DebugLog.Log(GetType(), "OnRestoreCompleteMsg " + result);
		if (this.RestorePurchaseCompletion != null)
		{
			this.RestorePurchaseCompletion(result);
		}
	}

	public void OnFetchSuccessMsg(string managedString)
	{
		if (this.FetchWalletSuccess != null)
		{
			this.FetchWalletSuccess(managedString);
		}
	}

	public void OnFetchErrorMsg(Payment.ErrorCode error, string managedString)
	{
		if (this.FetchWalletError != null)
		{
			this.FetchWalletError(error, managedString);
		}
	}

	public void OnPurchaseProgressMsg(Payment.Info info)
	{
		if (this.PurchaseProgress != null)
		{
			this.PurchaseProgress(HatchHelper.CreatePurchaseFromInfo(info));
		}
	}

	public void OnPurchaseError(Payment.ErrorCode status, Payment.Info failedPurchase)
	{
		DebugLog.Error(GetType(), "OnPurchaseError: " + status);
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

	public void OnConsumeSuccessMsg(string managedString)
	{
		if (this.ConsumeVoucherSuccess != null)
		{
			this.ConsumeVoucherSuccess(managedString);
		}
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

	public void OnConsumeErrorMsg(Payment.ErrorCode error, string managedString)
	{
		if (this.ConsumeVoucherError != null)
		{
			this.ConsumeVoucherError(error, managedString);
		}
	}
}

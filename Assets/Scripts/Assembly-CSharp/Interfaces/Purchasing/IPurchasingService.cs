using System;
using System.Collections.Generic;
using Rcs;

namespace Interfaces.Purchasing
{
	public interface IPurchasingService
	{
		bool AutoRestorePurchasesAfterInit { get; set; }

		event Action<string> InitializeSuccess;

		event Action<Payment.Info> InitializeProgress;

		event Action<Payment.ErrorCode, string> InitializeError;

		event Action<string> SelectProviderSuccess;

		event Action<Purchase> PurchaseProgress;

		event Action<string> PurchaseSuccess;

		event Action<Payment.ErrorCode, string> PurchaseError;

		event Action<string> RestorePurchaseCompletion;

		event Action<Purchase> RestorePurchaseProgress;

		event Action<List<string>> PurchaseHistoryRetrive;

		event Action<string> FetchWalletSuccess;

		event Action<Payment.ErrorCode, string> FetchWalletError;

		event Action<string> MoveWalletSuccess;

		event Action<Payment.ErrorCode, string> MoveWalletError;

		event Action<string> ConsumeVoucherSuccess;

		event Action<Payment.ErrorCode, string> ConsumeVoucherError;

		event Action<string, string> RedeemCodeSucces;

		event Action<Payment.ErrorCode, string> RedeemCodeError;

		IPurchasingService SetPaymentProvider(string paymentProvider);

		void Initialize(string bundleId);

		bool IsSupported();

		bool IsEnabled();

		bool IsInitialized();

		bool IsInitializing();

		void RestorePurchases();

		bool RedeemCode(string code);

		void FetchWallet();

		string GetCurrentProvider();

		List<string> GetProviders();

		List<Product> GetCatalog();

		List<Product> GetCatalogFromServer();

		List<Voucher> GetVouchers();

		void ConsumeVoucher(string voucherId);

		void PurchaseProduct(string productId);

		void OnInitSuccessfulMsg(string statusManagedString);

		void OnInitProgressMsg(Payment.Info info);

		void OnInitErrorMsg(Payment.ErrorCode error, string statusManagedString);

		void OnProviderSelectedSuccessMsg(string managedString);

		void OnRestoreProgressMsg(Purchase info);

		void OnRestoreCompleteMsg(string result);

		void OnFetchSuccessMsg(string managedString);

		void OnFetchErrorMsg(Payment.ErrorCode error, string managedString);

		void OnPurchaseProgressMsg(Payment.Info info);

		void OnConsumeSuccessMsg(string managedString);

		void OnConsumeErrorMsg(Payment.ErrorCode error, string managedString);

		void OnRedeemCodeSucces(string code, string voucherId);

		void OnRedeemCodeError(Payment.ErrorCode status, string code);

		void OnPurchaseError(Payment.ErrorCode status, Payment.Info failedPurchase);

		void OnPurchaseSuccess(Payment.Info succeededPurchase);
	}
}

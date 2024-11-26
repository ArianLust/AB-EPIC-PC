using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class Payment : IDisposable
	{
		public class Info : IDisposable
		{
			public enum PurchaseStatus
			{
				PurchaseSucceeded = 0,
				PurchaseFailed = 1,
				PurchaseCanceled = 2,
				PurchasePending = 3,
				PurchaseRestored = 4,
				PurchaseWaiting = 5
			}

			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			internal Info(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public Info(Info arg0)
				: this(RCSSDKPINVOKE.new_Payment_Info(getCPtr(arg0)), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			internal static IntPtr getCPtr(Info obj)
			{
				return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (!disposed)
				{
					_DisposeUnmanaged();
					disposed = true;
				}
			}

			~Info()
			{
				Dispose(false);
			}

			private void _DisposeUnmanaged()
			{
				lock (this)
				{
					if (swigCPtr != IntPtr.Zero)
					{
						if (swigCMemOwn)
						{
							swigCMemOwn = false;
							RCSSDKPINVOKE.delete_Payment_Info(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}

			public PurchaseStatus GetStatus()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return (PurchaseStatus)RCSSDKPINVOKE.Payment_Info_GetStatus(swigCPtr);
			}

			public string GetTransactionId()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Info_GetTransactionId(swigCPtr);
			}

			public string GetProductId()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Info_GetProductId(swigCPtr);
			}

			public string GetReceiptId()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Info_GetReceiptId(swigCPtr);
			}

			public string GetPurchaseId()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Info_GetPurchaseId(swigCPtr);
			}

			public string GetVoucherId()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Info_GetVoucherId(swigCPtr);
			}

			public static string StatusToString(PurchaseStatus status)
			{
				return RCSSDKPINVOKE.Payment_Info_StatusToString((int)status);
			}
		}

		public class Product : IDisposable
		{
			public enum ProductType
			{
				Consumable = 0,
				Nonconsumable = 1
			}

			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			internal Product(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public Product(Product arg0)
				: this(RCSSDKPINVOKE.new_Payment_Product_0(getCPtr(arg0)), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			public Product(string productJSON)
				: this(RCSSDKPINVOKE.new_Payment_Product_1(productJSON), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			internal static IntPtr getCPtr(Product obj)
			{
				return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (!disposed)
				{
					_DisposeUnmanaged();
					disposed = true;
				}
			}

			~Product()
			{
				Dispose(false);
			}

			private void _DisposeUnmanaged()
			{
				lock (this)
				{
					if (swigCPtr != IntPtr.Zero)
					{
						if (swigCMemOwn)
						{
							swigCMemOwn = false;
							RCSSDKPINVOKE.delete_Payment_Product(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}

			public string GetId()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Product_GetId(swigCPtr);
			}

			public string GetProviderId()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Product_GetProviderId(swigCPtr);
			}

			public ProductType GetProductType()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return (ProductType)RCSSDKPINVOKE.Payment_Product_GetProductType(swigCPtr);
			}

			public string GetToken()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Product_GetToken(swigCPtr);
			}

			public string GetName()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Product_GetName(swigCPtr);
			}

			public string GetReferenceName()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Product_GetReferenceName(swigCPtr);
			}

			public string GetPrice()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Product_GetPrice(swigCPtr);
			}

			public string GetUnformattedPrice()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Product_GetUnformattedPrice(swigCPtr);
			}

			public string GetCurrencyCode()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Product_GetCurrencyCode(swigCPtr);
			}

			public string GetCountryCode()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Product_GetCountryCode(swigCPtr);
			}

			public float GetReferencePrice()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Product_GetReferencePrice(swigCPtr);
			}

			public string GetDescription()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Product_GetDescription(swigCPtr);
			}

			public Dictionary<string, string> GetProviderData()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				StringDict srcDict = new StringDict(RCSSDKPINVOKE.Payment_Product_GetProviderData(swigCPtr), false);
				return srcDict.ToDictionary();
			}

			public string GetProviderDataString()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Product_GetProviderDataString(swigCPtr);
			}

			public Dictionary<string, string> GetClientData()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				StringDict srcDict = new StringDict(RCSSDKPINVOKE.Payment_Product_GetClientData(swigCPtr), false);
				return srcDict.ToDictionary();
			}

			public string GetClientDataString()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Product_GetClientDataString(swigCPtr);
			}

			public string ToJson()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Product_ToJson(swigCPtr);
			}

			public static string TypeToString(ProductType type)
			{
				return RCSSDKPINVOKE.Payment_Product_TypeToString((int)type);
			}

			public void SetProviderInfo(string name, string localizedPrice, string description, string unformattedPrice, string currencyCode, string countryCode)
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				RCSSDKPINVOKE.Payment_Product_SetProviderInfo_0(swigCPtr, name, localizedPrice, description, unformattedPrice, currencyCode, countryCode);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			public void SetProviderInfo(string name, string localizedPrice, string description, string unformattedPrice, string currencyCode)
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				RCSSDKPINVOKE.Payment_Product_SetProviderInfo_1(swigCPtr, name, localizedPrice, description, unformattedPrice, currencyCode);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			public void SetProviderInfo(string name, string localizedPrice, string description, string unformattedPrice)
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				RCSSDKPINVOKE.Payment_Product_SetProviderInfo_2(swigCPtr, name, localizedPrice, description, unformattedPrice);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			public void SetProviderInfo(string name, string localizedPrice, string description)
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				RCSSDKPINVOKE.Payment_Product_SetProviderInfo_3(swigCPtr, name, localizedPrice, description);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}
		}

		public class Voucher : IDisposable
		{
			public enum SourceType
			{
				Purchase = 0,
				Reward = 1,
				Donation = 2,
				Codes = 3,
				Other = 4
			}

			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			internal Voucher(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public Voucher(Voucher arg0)
				: this(RCSSDKPINVOKE.new_Payment_Voucher_0(getCPtr(arg0)), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			public Voucher(string id, string productId, bool isConsumable, string clientDataString, SourceType sourceType, string sourceId)
				: this(RCSSDKPINVOKE.new_Payment_Voucher_1(id, productId, isConsumable, clientDataString, (int)sourceType, sourceId), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			internal static IntPtr getCPtr(Voucher obj)
			{
				return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (!disposed)
				{
					_DisposeUnmanaged();
					disposed = true;
				}
			}

			~Voucher()
			{
				Dispose(false);
			}

			private void _DisposeUnmanaged()
			{
				lock (this)
				{
					if (swigCPtr != IntPtr.Zero)
					{
						if (swigCMemOwn)
						{
							swigCMemOwn = false;
							RCSSDKPINVOKE.delete_Payment_Voucher(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}

			public bool IsConsumable()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Voucher_IsConsumable(swigCPtr);
			}

			public string GetId()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Voucher_GetId(swigCPtr);
			}

			public string GetProductId()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Voucher_GetProductId(swigCPtr);
			}

			public Dictionary<string, string> GetClientData()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				StringDict srcDict = new StringDict(RCSSDKPINVOKE.Payment_Voucher_GetClientData(swigCPtr), false);
				return srcDict.ToDictionary();
			}

			public string GetClientDataString()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Voucher_GetClientDataString(swigCPtr);
			}

			public SourceType GetSourceType()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return (SourceType)RCSSDKPINVOKE.Payment_Voucher_GetSourceType(swigCPtr);
			}

			public string GetSourceId()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Payment_Voucher_GetSourceId(swigCPtr);
			}

			public static string TypeToString(SourceType type)
			{
				return RCSSDKPINVOKE.Payment_Voucher_TypeToString((int)type);
			}
		}

		public enum PaymentCapabilities
		{
			CapabilityFlagRestore = 1,
			CapabilityFlagRestoreInteractive = 2,
			CapabilityFlagWallet = 4,
			CapabilityFlagApcatalog = 8,
			CapabilityFlagFlightdeckcatalog = 0x10,
			CapabilityFlagOfflinecatalog = 0x20
		}

		public enum ErrorCode
		{
			NoError = 0,
			ErrorNotInitialized = 1,
			ErrorMethodNotAvailable = 2,
			ErrorInvalidCallback = 3,
			ErrorOperationRunning = 4,
			ErrorOperationCanceled = 5,
			ErrorOperationFailed = 6
		}

		public enum CatalogBackend
		{
			ApcatalogBackend = 0,
			FlightdeckcatalogBackend = 1
		}

		public delegate void VerifySuccessCallback(string productId, int timeout);

		public delegate void SuccessCallback(string providerName);

		public delegate void ProgressCallback(Info purchaseInProgess);

		public delegate void PurchaseErrorCallback(ErrorCode status, Info failedPurchase);

		public delegate void RedeemSuccessCallback(string code, string voucherId);

		public delegate void PurchaseSuccessCallback(Info succeededPurchase);

		public delegate void ErrorCallback(ErrorCode status, string errorMessage);

		private delegate void SwigDelegatePayment_0(IntPtr cb, string productId, int timeout);

		private delegate void SwigDelegatePayment_1(IntPtr cb, string id);

		private delegate void SwigDelegatePayment_2(IntPtr cb, IntPtr purchaseInProgess);

		private delegate void SwigDelegatePayment_3(IntPtr cb, int status, IntPtr failedPurchase);

		private delegate void SwigDelegatePayment_4(IntPtr cb, string code, string voucherId);

		private delegate void SwigDelegatePayment_5(IntPtr cb, IntPtr succeededPurchase);

		private delegate void SwigDelegatePayment_6(IntPtr cb, int status, string errorInfo);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SwigDelegatePayment_0 swigDelegate0;

		private SwigDelegatePayment_1 swigDelegate1;

		private SwigDelegatePayment_2 swigDelegate2;

		private SwigDelegatePayment_3 swigDelegate3;

		private SwigDelegatePayment_4 swigDelegate4;

		private SwigDelegatePayment_5 swigDelegate5;

		private SwigDelegatePayment_6 swigDelegate6;

		private GCHandle pendingPurchasesUpdateCallbackGCHandle;

		internal Payment(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public Payment(string jsonCatalog, string providerName)
			: this(RCSSDKPINVOKE.new_Payment_0(jsonCatalog, providerName), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Payment(string jsonCatalog)
			: this(RCSSDKPINVOKE.new_Payment_1(jsonCatalog), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Payment(IdentitySessionBase identity, CatalogBackend catalogBackend, string bundleId, string providerName, bool isWalletEnabled)
			: this(RCSSDKPINVOKE.new_Payment_2(identity.SharedPtr.CPtr, (int)catalogBackend, bundleId, providerName, isWalletEnabled), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Payment(IdentitySessionBase identity, CatalogBackend catalogBackend, string bundleId, string providerName)
			: this(RCSSDKPINVOKE.new_Payment_3(identity.SharedPtr.CPtr, (int)catalogBackend, bundleId, providerName), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Payment(IdentitySessionBase identity, CatalogBackend catalogBackend, string bundleId)
			: this(RCSSDKPINVOKE.new_Payment_4(identity.SharedPtr.CPtr, (int)catalogBackend, bundleId), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public Payment(IdentitySessionBase identity, CatalogBackend catalogBackend)
			: this(RCSSDKPINVOKE.new_Payment_5(identity.SharedPtr.CPtr, (int)catalogBackend), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<Payment> callInfo)
		{
			lock (this)
			{
				IntPtr intPtr = callInfo.Pin();
				pendingCallbacks.Add(intPtr);
				return intPtr;
			}
		}

		private void RemovePendingCallback(IntPtr callbackInfoId)
		{
			lock (this)
			{
				pendingCallbacks.Remove(callbackInfoId);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}
			_DisposeUnmanaged();
			lock (this)
			{
				foreach (IntPtr pendingCallback in pendingCallbacks)
				{
					GCHandle.FromIntPtr(pendingCallback).Free();
				}
				pendingCallbacks.Clear();
				if (pendingPurchasesUpdateCallbackGCHandle.IsAllocated)
				{
					pendingPurchasesUpdateCallbackGCHandle.Free();
				}
			}
			disposed = true;
		}

		internal static IntPtr getCPtr(Payment obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~Payment()
		{
			Dispose(false);
		}

		private void _DisposeUnmanaged()
		{
			lock (this)
			{
				if (swigCPtr != IntPtr.Zero)
				{
					if (swigCMemOwn)
					{
						swigCMemOwn = false;
						RCSSDKPINVOKE.delete_Payment(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public static bool IsSupported()
		{
			return RCSSDKPINVOKE.Payment_IsSupported();
		}

		public static List<string> GetProviders()
		{
			StringList srcList = new StringList(RCSSDKPINVOKE.Payment_GetProviders(), true);
			return srcList.ToList();
		}

		private void DefaultSuccessCallback(string voucherId)
		{
		}

		private void DefaultErrorCallback(ErrorCode status, string errorMessage)
		{
		}

		private void DefaultProgressCallback(Info purchaseInProgess)
		{
		}

		public ErrorCode Initialize(SuccessCallback onSuccess, ErrorCallback onError, ProgressCallback onProgress)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, onError);
			IntPtr intPtr = AddPendingCallback(callInfo);
			IntPtr jarg = IntPtr.Zero;
			if (pendingPurchasesUpdateCallbackGCHandle.IsAllocated)
			{
				pendingPurchasesUpdateCallbackGCHandle.Free();
			}
			if (onProgress != null)
			{
				pendingPurchasesUpdateCallbackGCHandle = GCHandle.Alloc(onProgress);
				jarg = GCHandle.ToIntPtr(pendingPurchasesUpdateCallbackGCHandle);
			}
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_Initialize(swigCPtr, intPtr, intPtr, jarg);
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
				if (pendingPurchasesUpdateCallbackGCHandle.IsAllocated)
				{
					pendingPurchasesUpdateCallbackGCHandle.Free();
				}
			}
			return errorCode;
		}

		public bool IsInitialized()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Payment_IsInitialized(swigCPtr);
		}

		public bool IsEnabled()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Payment_IsEnabled(swigCPtr);
		}

		public PaymentCapabilities GetCapabilities()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return (PaymentCapabilities)RCSSDKPINVOKE.Payment_GetCapabilities(swigCPtr);
		}

		public string GetProviderName()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Payment_GetProviderName(swigCPtr);
		}

		public ErrorCode FetchCatalog(SuccessCallback onSuccess, ErrorCallback onError)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, onError);
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_FetchCatalog_0(swigCPtr, intPtr, intPtr);
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode FetchCatalog(SuccessCallback onSuccess)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, new ErrorCallback(DefaultErrorCallback));
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_FetchCatalog_0(swigCPtr, intPtr, intPtr);
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode FetchCatalog()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, new SuccessCallback(DefaultSuccessCallback), new ErrorCallback(DefaultErrorCallback));
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_FetchCatalog_0(swigCPtr, intPtr, intPtr);
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public static List<Product> GetCachedCatalog(string bundleId, string providerId)
		{
			CatalogProducts srcList = new CatalogProducts(RCSSDKPINVOKE.Payment_GetCachedCatalog_0(bundleId, providerId), true);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return srcList.ToList();
		}

		public static List<Product> GetCachedCatalog(string bundleId)
		{
			CatalogProducts srcList = new CatalogProducts(RCSSDKPINVOKE.Payment_GetCachedCatalog_1(bundleId), true);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return srcList.ToList();
		}

		public List<Product> GetCatalog()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			CatalogProducts srcList = new CatalogProducts(RCSSDKPINVOKE.Payment_GetCatalog(swigCPtr), false);
			return srcList.ToList();
		}

		public List<Product> GetRewards()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			CatalogProducts srcList = new CatalogProducts(RCSSDKPINVOKE.Payment_GetRewards(swigCPtr), false);
			return srcList.ToList();
		}

		public ErrorCode PurchaseProduct(Product product, PurchaseSuccessCallback onSuccess, PurchaseErrorCallback onError, ProgressCallback onProgress)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, onError, onProgress);
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_PurchaseProduct_0(swigCPtr, Product.getCPtr(product), intPtr, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode PurchaseProduct(Product product, PurchaseSuccessCallback onSuccess, PurchaseErrorCallback onError)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, onError, new ProgressCallback(DefaultProgressCallback));
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_PurchaseProduct_0(swigCPtr, Product.getCPtr(product), intPtr, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode PurchaseProduct(Product product, PurchaseSuccessCallback onSuccess, PurchaseErrorCallback onError, out string transactionId, ProgressCallback onProgress)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, onError, onProgress);
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_PurchaseProduct_2(swigCPtr, Product.getCPtr(product), intPtr, intPtr, out transactionId, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode PurchaseProduct(Product product, PurchaseSuccessCallback onSuccess, PurchaseErrorCallback onError, out string transactionId)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, onError, new ProgressCallback(DefaultProgressCallback));
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_PurchaseProduct_2(swigCPtr, Product.getCPtr(product), intPtr, intPtr, out transactionId, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode RestorePurchases(SuccessCallback onSuccess, ErrorCallback onError, ProgressCallback onProgress)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, onError, onProgress);
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_RestorePurchases_0(swigCPtr, intPtr, intPtr, intPtr);
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode RestorePurchases(SuccessCallback onSuccess, ErrorCallback onError)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, onError, new ProgressCallback(DefaultProgressCallback));
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_RestorePurchases_0(swigCPtr, intPtr, intPtr, intPtr);
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode RestorePurchases(SuccessCallback onSuccess)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, new ErrorCallback(DefaultErrorCallback), new ProgressCallback(DefaultProgressCallback));
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_RestorePurchases_0(swigCPtr, intPtr, intPtr, intPtr);
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode RestorePurchases()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, new SuccessCallback(DefaultSuccessCallback), new ErrorCallback(DefaultErrorCallback), new ProgressCallback(DefaultProgressCallback));
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_RestorePurchases_0(swigCPtr, intPtr, intPtr, intPtr);
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode FetchWallet(SuccessCallback onSuccess, ErrorCallback onError)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, onError);
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_FetchWallet_0(swigCPtr, intPtr, intPtr);
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode FetchWallet(SuccessCallback onSuccess)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, new ErrorCallback(DefaultErrorCallback));
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_FetchWallet_0(swigCPtr, intPtr, intPtr);
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode FetchWallet()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, new SuccessCallback(DefaultSuccessCallback), new ErrorCallback(DefaultErrorCallback));
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_FetchWallet_0(swigCPtr, intPtr, intPtr);
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public List<Voucher> GetVouchers()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			WalletVouchers srcList = new WalletVouchers(RCSSDKPINVOKE.Payment_GetVouchers(swigCPtr), false);
			return srcList.ToList();
		}

		public ErrorCode ConsumeVoucher(Voucher voucher, SuccessCallback onSuccess, ErrorCallback onError)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, onError);
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_ConsumeVoucher_0(swigCPtr, Voucher.getCPtr(voucher), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode ConsumeVoucher(Voucher voucher, SuccessCallback onSuccess)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, new ErrorCallback(DefaultErrorCallback));
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_ConsumeVoucher_0(swigCPtr, Voucher.getCPtr(voucher), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode ConsumeVoucher(Voucher voucher)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, new SuccessCallback(DefaultSuccessCallback), new ErrorCallback(DefaultErrorCallback));
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_ConsumeVoucher_0(swigCPtr, Voucher.getCPtr(voucher), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode RedeemCode(string code, RedeemSuccessCallback onSuccess, ErrorCallback onError)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, onError);
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_RedeemCode(swigCPtr, code, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode VerifyCode(string code, SuccessCallback onSuccess, ErrorCallback onError)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, onError);
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_VerifyCode(swigCPtr, code, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode SendGift(string rewardRuleId, string targetAccountId, SuccessCallback onSuccess, ErrorCallback onError)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, onError);
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_SendGift_0(swigCPtr, rewardRuleId, targetAccountId, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode SendGift(string rewardRuleId, string targetAccountId, SuccessCallback onSuccess)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, new ErrorCallback(DefaultErrorCallback));
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_SendGift_0(swigCPtr, rewardRuleId, targetAccountId, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode SendGift(string rewardRuleId, string targetAccountId)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, new SuccessCallback(DefaultSuccessCallback), new ErrorCallback(DefaultErrorCallback));
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_SendGift_0(swigCPtr, rewardRuleId, targetAccountId, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode VerifyReward(string rewardRuleId, VerifySuccessCallback onSuccess, ErrorCallback onError)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, onError);
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_VerifyReward(swigCPtr, rewardRuleId, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode ReportReward(string rewardRuleId, SuccessCallback onSuccess, ErrorCallback onError)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, onError);
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_ReportReward_0(swigCPtr, rewardRuleId, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode ReportReward(string rewardRuleId, SuccessCallback onSuccess)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, new ErrorCallback(DefaultErrorCallback));
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_ReportReward_0(swigCPtr, rewardRuleId, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode ReportReward(string rewardRuleId)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, new SuccessCallback(DefaultSuccessCallback), new ErrorCallback(DefaultErrorCallback));
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_ReportReward_0(swigCPtr, rewardRuleId, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode DeliverReward(string rewardRuleId, SuccessCallback onSuccess, ErrorCallback onError)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, onError);
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_DeliverReward_0(swigCPtr, rewardRuleId, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode DeliverReward(string rewardRuleId, SuccessCallback onSuccess)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, onSuccess, new ErrorCallback(DefaultErrorCallback));
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_DeliverReward_0(swigCPtr, rewardRuleId, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public ErrorCode DeliverReward(string rewardRuleId)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			AsyncCallInfo<Payment> callInfo = new AsyncCallInfo<Payment>(this, new SuccessCallback(DefaultSuccessCallback), new ErrorCallback(DefaultErrorCallback));
			IntPtr intPtr = AddPendingCallback(callInfo);
			ErrorCode errorCode = (ErrorCode)RCSSDKPINVOKE.Payment_DeliverReward_0(swigCPtr, rewardRuleId, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (errorCode != 0)
			{
				RemovePendingCallback(intPtr);
				GCHandle.FromIntPtr(intPtr).Free();
			}
			return errorCode;
		}

		public void SetStealthMode()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Payment_SetStealthMode(swigCPtr);
		}

		private static void OnVerifySuccessCallback(VerifySuccessCallback cb, string productId, int timeout)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(productId, timeout);
		}

		private static void OnSuccessCallback(SuccessCallback cb, string providerName)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(providerName);
		}

		private static void OnProgressCallback(ProgressCallback cb, Info purchaseInProgess)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(new Info(purchaseInProgess));
		}

		private static void OnPurchaseErrorCallback(PurchaseErrorCallback cb, ErrorCode status, Info failedPurchase)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(status, new Info(failedPurchase));
		}

		private static void OnRedeemSuccessCallback(RedeemSuccessCallback cb, string code, string voucherId)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(code, voucherId);
		}

		private static void OnPurchaseSuccessCallback(PurchaseSuccessCallback cb, Info succeededPurchase)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(new Info(succeededPurchase));
		}

		private static void OnErrorCallback(ErrorCallback cb, ErrorCode status, string errorMessage)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(status, errorMessage);
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnVerifySuccessCallback;
			swigDelegate1 = SwigDirectorOnSuccessCallback;
			swigDelegate2 = SwigDirectorOnProgressCallback;
			swigDelegate3 = SwigDirectorOnPurchaseErrorCallback;
			swigDelegate4 = SwigDirectorOnRedeemSuccessCallback;
			swigDelegate5 = SwigDirectorOnPurchaseSuccessCallback;
			swigDelegate6 = SwigDirectorOnErrorCallback;
			RCSSDKPINVOKE.Payment_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1), Marshal.GetFunctionPointerForDelegate(swigDelegate2), Marshal.GetFunctionPointerForDelegate(swigDelegate3), Marshal.GetFunctionPointerForDelegate(swigDelegate4), Marshal.GetFunctionPointerForDelegate(swigDelegate5), Marshal.GetFunctionPointerForDelegate(swigDelegate6));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(Payment));
		}

		[MonoPInvokeCallback(typeof(SwigDelegatePayment_0))]
		private static void SwigDirectorOnVerifySuccessCallback(IntPtr cb, string productId, int timeout)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Payment] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Payment> asyncCallInfo = (AsyncCallInfo<Payment>)gCHandle.Target;
			VerifySuccessCallback handler = asyncCallInfo.GetHandler<VerifySuccessCallback>();
			try
			{
				OnVerifySuccessCallback(handler, productId, timeout);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegatePayment_1))]
		private static void SwigDirectorOnSuccessCallback(IntPtr cb, string providerName)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Payment] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Payment> asyncCallInfo = (AsyncCallInfo<Payment>)gCHandle.Target;
			SuccessCallback handler = asyncCallInfo.GetHandler<SuccessCallback>();
			try
			{
				OnSuccessCallback(handler, providerName);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegatePayment_2))]
		private static void SwigDirectorOnProgressCallback(IntPtr cb, IntPtr purchaseInProgess)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Payment] Ignoring callback from previously disposed object instance");
				return;
			}
			ProgressCallback progressCallback = null;
			if (gCHandle.Target.GetType() == typeof(AsyncCallInfo<Payment>))
			{
				AsyncCallInfo<Payment> asyncCallInfo = (AsyncCallInfo<Payment>)gCHandle.Target;
				progressCallback = asyncCallInfo.GetHandler<ProgressCallback>();
			}
			else
			{
				progressCallback = (ProgressCallback)gCHandle.Target;
			}
			OnProgressCallback(progressCallback, new Info(purchaseInProgess, false));
		}

		[MonoPInvokeCallback(typeof(SwigDelegatePayment_3))]
		private static void SwigDirectorOnPurchaseErrorCallback(IntPtr cb, int status, IntPtr failedPurchase)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Payment] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Payment> asyncCallInfo = (AsyncCallInfo<Payment>)gCHandle.Target;
			PurchaseErrorCallback handler = asyncCallInfo.GetHandler<PurchaseErrorCallback>();
			try
			{
				OnPurchaseErrorCallback(handler, (ErrorCode)status, new Info(failedPurchase, false));
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegatePayment_4))]
		private static void SwigDirectorOnRedeemSuccessCallback(IntPtr cb, string code, string voucherId)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Payment] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Payment> asyncCallInfo = (AsyncCallInfo<Payment>)gCHandle.Target;
			RedeemSuccessCallback handler = asyncCallInfo.GetHandler<RedeemSuccessCallback>();
			try
			{
				OnRedeemSuccessCallback(handler, code, voucherId);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegatePayment_5))]
		private static void SwigDirectorOnPurchaseSuccessCallback(IntPtr cb, IntPtr succeededPurchase)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Payment] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Payment> asyncCallInfo = (AsyncCallInfo<Payment>)gCHandle.Target;
			PurchaseSuccessCallback handler = asyncCallInfo.GetHandler<PurchaseSuccessCallback>();
			try
			{
				OnPurchaseSuccessCallback(handler, new Info(succeededPurchase, false));
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegatePayment_6))]
		private static void SwigDirectorOnErrorCallback(IntPtr cb, int status, string errorMessage)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Payment] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Payment> asyncCallInfo = (AsyncCallInfo<Payment>)gCHandle.Target;
			ErrorCallback handler = asyncCallInfo.GetHandler<ErrorCallback>();
			try
			{
				OnErrorCallback(handler, (ErrorCode)status, errorMessage);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}
	}
}

using System.Collections.Generic;
using System.Text;
using Interfaces.Purchasing;
using Rcs;

public class HatchHelper
{
	public static Dictionary<string, Rcs.Assets.Info> ConvertAssetInfoDict(Dictionary<string, Rcs.Assets.Info> hatchDic)
	{
		Dictionary<string, Rcs.Assets.Info> dictionary = new Dictionary<string, Rcs.Assets.Info>();
		foreach (string key in hatchDic.Keys)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(hatchDic[key].Name);
			stringBuilder.AppendLine(hatchDic[key].CdnUrl);
			stringBuilder.AppendLine(hatchDic[key].Size.ToString());
			stringBuilder.AppendLine(hatchDic[key].Hash);
			dictionary.Add(key, hatchDic[key]);
		}
		return dictionary;
	}

	public static Purchase CreatePurchaseFromInfo(Payment.Info info)
	{
		Purchase result = default(Purchase);
		result.productID = info.GetProductId();
		result.receiptID = info.GetReceiptId();
		result.status = ConvertStatus(info.GetStatus());
		result.transactionID = info.GetTransactionId();
		return result;
	}

	private static PurchaseStatus ConvertStatus(Payment.Info.PurchaseStatus purchaseStatus)
	{
		switch (purchaseStatus)
		{
		case Payment.Info.PurchaseStatus.PurchaseCanceled:
			return PurchaseStatus.PURCHASE_CANCELED;
		case Payment.Info.PurchaseStatus.PurchaseFailed:
			return PurchaseStatus.PURCHASE_FAILED;
		case Payment.Info.PurchaseStatus.PurchasePending:
			return PurchaseStatus.PURCHASE_PENDING;
		case Payment.Info.PurchaseStatus.PurchaseRestored:
			return PurchaseStatus.PURCHASE_RESTORED;
		case Payment.Info.PurchaseStatus.PurchaseSucceeded:
			return PurchaseStatus.PURCHASE_SUCCEEDED;
		case Payment.Info.PurchaseStatus.PurchaseWaiting:
			return PurchaseStatus.PURCHASE_WAITING;
		default:
			return PurchaseStatus.PURCHASE_FAILED;
		}
	}

	public static Dictionary<string, Variant> ConvertToVariantDic(Dictionary<string, object> attributes)
	{
		Dictionary<string, Variant> dictionary = new Dictionary<string, Variant>();
		foreach (string key in attributes.Keys)
		{
			if (!string.IsNullOrEmpty(key))
			{
				dictionary.Add(key, new Variant(attributes[key].ToString()));
			}
		}
		return dictionary;
	}
}

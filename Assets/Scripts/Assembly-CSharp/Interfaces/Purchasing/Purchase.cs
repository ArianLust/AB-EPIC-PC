namespace Interfaces.Purchasing
{
	public struct Purchase
	{
		public PurchaseStatus status;

		public string transactionID;

		public string productID;

		public string receiptID;

		public string statusString;
	}
}

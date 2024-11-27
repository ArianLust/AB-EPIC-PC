public interface IAppTrackService
{
	void Init();

	void ReportEvent(string eventName, string numericParam, string currencyParam);

	void SendAndValidateSaleEvent(string priceParam, string currencyParam, int status, string receiptId, string transactionId);
}

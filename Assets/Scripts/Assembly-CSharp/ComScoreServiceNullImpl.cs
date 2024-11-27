public class ComScoreServiceNullImpl : IAppTrackService
{
	public void Init()
	{
		DebugLog.Log(GetType(), "NullImpl Init");
	}

	public void ReportEvent(string eventName, string numericParam, string currencyParam)
	{
		DebugLog.Log(GetType(), string.Format("NullImpl ReportEvent: '{0}' with numericParam: '{1}' and currencyParam: '{2}'", eventName, numericParam, currencyParam));
	}

	public void SendAndValidateSaleEvent(string priceParam, string currencyParam, int status, string receiptId, string transactionId)
	{
		DebugLog.Log(GetType(), string.Format("NullImpl SendAndValidateSaleEvent.priceParam: '{0}', currencyParam: '{1}', status: '{2}', receiptID: '{3}', transactionID: '{4}' ", priceParam, currencyParam, status, receiptId, transactionId));
	}
}

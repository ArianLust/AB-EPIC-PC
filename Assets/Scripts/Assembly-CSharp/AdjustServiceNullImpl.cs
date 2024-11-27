public class AdjustServiceNullImpl : IAppAttributionService
{
	public void Init()
	{
		DebugLog.Log(GetType(), "NullImpl Init");
	}

	public void TrackEvent(AdjustTrackingEvent adjustTrackingEvent)
	{
		DebugLog.Log(GetType(), string.Format("NullImpl TrackEvent: '{0}'", adjustTrackingEvent));
	}

	public void TrackPlayerLevelProgress(int playerLevel)
	{
		DebugLog.Log(GetType(), string.Format("NullImpl TrackEventLevelprogress for level: '{0}'", playerLevel));
	}

	public void TrackSaleEvent(double price, string transactionId)
	{
		DebugLog.Log(GetType(), string.Format("NullImpl TrackSaleEvent for price: '{0}' and transactionId: '{1}'", price, transactionId));
	}
}

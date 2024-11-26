using System.Collections.Generic;

public class AdjustServiceiOSAndroidImpl : IAppAttributionService
{
	private Dictionary<AdjustTrackingEvent, string> m_eventTrackingTokenMapping = new Dictionary<AdjustTrackingEvent, string>();

	private bool m_initialized;

	public void Init()
	{
	}

	public void TrackEvent(AdjustTrackingEvent adjustTrackingEvent)
	{
		DebugLog.Log(GetType(), string.Format("TrackEvent: '{0}'", adjustTrackingEvent));
	}

	public void TrackPlayerLevelProgress(int playerLevel)
	{
	}

	public void TrackSaleEvent(double price, string transactionId)
	{
	}
}

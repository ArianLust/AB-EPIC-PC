using Rcs;

public class ComScoreServiceiOSAndroidImpl : IAppTrackService
{
	private AppTrack m_ComScoreAppTrack;

	public void Init()
	{
		DebugLog.Log(GetType(), "[AppTrack] Init ComScoreServiceiOSAndroidImpl");
		AppTrack.Params @params = new AppTrack.Params();
		@params.Vendor = "ComScore";
		@params.ClientId = "19014625";
		@params.PublisherId = "5da0c1a5e30f4be8088bed810ab32afb";
		bool debugMode = false;
		m_ComScoreAppTrack = new AppTrack(ContentLoader.Instance.m_BeaconConnectionMgr.Identiy, @params, debugMode);
	}

	public void ReportEvent(string eventName, string numericParam, string currencyParam)
	{
		DebugLog.Log(GetType(), "Ignoring Event report: " + eventName + ", " + numericParam + ", " + currencyParam);
	}

	public void SendAndValidateSaleEvent(string priceParam, string currencyParam, int status, string receiptId, string transactionId)
	{
	}
}

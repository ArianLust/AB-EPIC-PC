public interface IAppAttributionService
{
	void Init();

	void TrackEvent(AdjustTrackingEvent adjustTrackingEvent);

	void TrackPlayerLevelProgress(int playerLevel);

	void TrackSaleEvent(double price, string transactionId);
}

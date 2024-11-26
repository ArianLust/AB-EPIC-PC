using System;
using Rcs;

public interface IAdService
{
	event Ads.RewardResultHandler RewardResult;

	event Action<string> AdReady;

	event Action<string, int> NewsFeedContentUpdate;

	bool Init();

	bool StartSession();

	bool AddPlacement(string placementId, Ads.RendererHandler onRenderableReadyCallback = null);

	bool AddPlacement(string placementId, int x, int y, int width, int height);

	bool AddPlacement(string placementId, float x, float y, float width, float height);

	bool ShowAd(string placementId);

	bool HideAd(string placementId);

	bool HandleClick(string placementId);

	bool TrackNativeAdImpression(string placementId);

	bool MutedGameSoundForPlacement(string placementId);

	bool RemoveRenderer(string placementId);

	bool IsAdShowPossible(string placementId);

	bool IsNewsFeedShowPossible(string placementId);

	int GetState(string placementId);

	bool SuspendNewsFeeds();

	bool RestoreSuspendedNewsFeed();
}

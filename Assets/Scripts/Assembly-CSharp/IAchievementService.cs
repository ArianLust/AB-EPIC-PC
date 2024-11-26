using System;
using System.Collections.Generic;

public interface IAchievementService
{
	bool? IsSignedIn { get; }

	void Init(IMonoBehaviourContainer mainInstance, bool mayUseUI);

	void ShowAchievementUI();

	void ReportProgress(string achievementId, double progress);

	void ReportUnlocked(string achievementId);

	string GetAchievementIdForStoryItemIfExists(string storyItem);

	void GetGlobalAchievementProgress(Action<float> progressCallback);

	List<string> GetUnlockedAchievements();
}

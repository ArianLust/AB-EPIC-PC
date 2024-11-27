using System;
using System.Collections.Generic;

public class AchievementServiceNullImpl : IAchievementService
{
	public bool? IsSignedIn
	{
		get
		{
			return true;
		}
	}

	public void Init(IMonoBehaviourContainer mainInstance, bool mayUseUI)
	{
		DebugLog.Log("Init achievement service with ui? " + mayUseUI);
	}

	public void ShowAchievementUI()
	{
		DebugLog.Log("Open achievement ui.");
	}

	public void ReportProgress(string achievementId, double progress)
	{
		DebugLog.Log("Report achievement progress " + progress);
	}

	public void ReportUnlocked(string achievementId)
	{
		DebugLog.Log("Report achievement as unlocked " + achievementId);
	}

	public string GetAchievementIdForStoryItemIfExists(string storyItem)
	{
		return storyItem;
	}

	public void GetGlobalAchievementProgress(Action<float> progressCallback)
	{
		if (progressCallback != null)
		{
			progressCallback(0.5f);
		}
	}

	public List<string> GetUnlockedAchievements()
	{
		return new List<string>();
	}
}

using System;
using System.Collections.Generic;
using ABH.Shared.BalancingData;
using UnityEngine.SocialPlatforms;

public class AchievementServiceGooglePlayServicesImpl : AchievementServiceBase, IAchievementService
{

	private List<string> m_unlockedAchievements;

	public bool? IsSignedIn { get; private set; }

	public void Init(IMonoBehaviourContainer mainInstance, bool mayUseUI)
	{
	}

	private void OnGotAllAchievements(IAchievement[] achievements)
	{
	}

	private void DoWhenLoggedIn(Action callback)
	{
		callback();
	}

	public void ShowAchievementUI()
	{
	}

	public void ReportProgress(string achievementId, double progress)
	{
	}

	public override void ReportUnlocked(string achievementId)
	{

	}

	public string GetAchievementIdForStoryItemIfExists(string storyItem)
	{
		return "";
	}

	public void GetGlobalAchievementProgress(Action<float> progressCallback)
	{
	}

	public List<string> GetUnlockedAchievements()
	{
		return m_unlockedAchievements;
	}
}

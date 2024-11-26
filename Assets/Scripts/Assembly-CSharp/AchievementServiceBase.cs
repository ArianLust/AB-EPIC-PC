using ABH.Shared.BalancingData;

public class AchievementServiceBase
{
	protected void ReReportAllUnlockedAchievements()
	{
		DebugLog.Log("[AchievementServiceBase] Login successful; checking achievements...");
		foreach (ThirdPartyIdBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<ThirdPartyIdBalancingData>())
		{
			string text = null;
			text = balancingData.RovioGooglePlayAchievementId;
			if (!string.IsNullOrEmpty(text) && DIContainerLogic.InventoryService.CheckForItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, balancingData.NameId))
			{
				ReportUnlocked(text);
			}
		}
	}

	public virtual void ReportUnlocked(string achievementId)
	{
	}
}

using System;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.Generic;

[Serializable]
public class ConditionalActionTree
{
	public ActionTree ActionTree;

	public bool TriggerInstant;

	public bool IsEventCampaign;

	public string StartNodeId;

	public string EndNodeId;

	public bool IsActive()
	{
		if (IsEventCampaign)
		{
			return CheckConditionsEventCampaign();
		}
		return CheckConditionsWorldMap();
	}

	private bool CheckConditionsEventCampaign()
	{
		bool flag = false;
		bool flag2 = false;
		if (string.IsNullOrEmpty(StartNodeId))
		{
			flag = true;
		}
		EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
		if (currentEventManagerGameData == null || !currentEventManagerGameData.IsCampaignEvent)
		{
			DebugLog.Warn(GetType(), "CheckConditionsEventCampaign: Trying to access current event campaign, but found non!");
			return false;
		}
		Dictionary<string, HotspotGameData> hotspotGameDatas = currentEventManagerGameData.CurrentMiniCampaign.HotspotGameDatas;
		HotspotGameData value = null;
		if (hotspotGameDatas.TryGetValue(StartNodeId, out value))
		{
			flag = (TriggerInstant && value.Data.UnlockState == HotspotUnlockState.ResolvedNew) || value.Data.UnlockState >= HotspotUnlockState.Resolved;
		}
		if (hotspotGameDatas.TryGetValue(EndNodeId, out value))
		{
			flag2 = value.Data.UnlockState >= HotspotUnlockState.ResolvedNew;
		}
		return flag && !flag2;
	}

	private bool CheckConditionsWorldMap()
	{
		bool flag = false;
		bool flag2 = false;
		if (string.IsNullOrEmpty(StartNodeId))
		{
			flag = true;
		}
		Dictionary<string, HotspotGameData> hotspotGameDatas = DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.HotspotGameDatas;
		HotspotGameData value = null;
		if (hotspotGameDatas.TryGetValue(StartNodeId, out value))
		{
			flag = (TriggerInstant && value.Data.UnlockState == HotspotUnlockState.ResolvedNew) || value.Data.UnlockState >= HotspotUnlockState.Resolved;
		}
		if (hotspotGameDatas.TryGetValue(EndNodeId, out value))
		{
			flag2 = value.Data.UnlockState >= HotspotUnlockState.ResolvedNew;
		}
		return flag && !flag2;
	}
}

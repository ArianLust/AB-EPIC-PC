using System;
using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

[Serializable]
public class ZoneCloudingActiveState
{
	public string EndNodeId;

	public GameObject CloudSector;

	public bool IsActive()
	{
		bool flag = false;
		HotspotGameData value = null;
		if (DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.HotspotGameDatas.TryGetValue(EndNodeId, out value))
		{
			flag = value.Data.UnlockState >= HotspotUnlockState.ResolvedNew;
		}
		return !flag;
	}
}

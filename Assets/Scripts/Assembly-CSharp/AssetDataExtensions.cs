using System.Collections.Generic;
using ABH.Shared.Models;
using UnityEngine;

internal static class AssetDataExtensions
{
	internal const string m_AssetDataPlayerPrefsKey = "assetData";

	public static void Save(this AssetData data)
	{
		Debug.Log("[AssetDataExtensions] Storing asset data information locally to assetData");
		foreach (KeyValuePair<string, AssetInfo> asset in data.Assets)
		{
			DebugLog.Log("[AssetDataExtensions] current assetInfos: " + asset);
		}
		DIContainerInfrastructure.GetPlayerPrefsService().SetString("assetData", DIContainerInfrastructure.GetStringSerializer().Serialize(data));
	}
}

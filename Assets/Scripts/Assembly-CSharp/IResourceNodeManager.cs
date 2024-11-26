using ABH.GameDatas;
using UnityEngine;

public interface IResourceNodeManager
{
	void AddResourceSpot(HotSpotWorldMapViewResource spot);

	void NodeHarvested(HotspotGameData hotspot);

	void RemoveResourceSpot(HotSpotWorldMapViewResource spot);

	void SpawnResource(GameObject go, Vector3 offset);

	void CheckGlobalCoolDown();

	void ClearSpotList();
}

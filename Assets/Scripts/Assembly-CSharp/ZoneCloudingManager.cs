using System.Collections.Generic;
using UnityEngine;

public class ZoneCloudingManager : MonoBehaviour
{
	public List<ZoneCloudingActiveState> m_ZoneCloudingActiveStates;

	public void Awake()
	{
		foreach (ZoneCloudingActiveState zoneCloudingActiveState in m_ZoneCloudingActiveStates)
		{
			zoneCloudingActiveState.CloudSector.SetActive(zoneCloudingActiveState.IsActive());
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
	public float m_UpdateTime = 60f;

	private List<HotSpotWorldMapViewResource> m_spotList = new List<HotSpotWorldMapViewResource>();

	private List<DateTime> m_timeStampList = new List<DateTime>();

	private int m_amountResourcesPossible;

	private int m_currentAmount;

	private static ResourceManager _instance;

	[SerializeField]
	private GameObject[] m_resourcePrefabs;

	public static ResourceManager Instance
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
	}

	public void AddResourceSpot(HotSpotWorldMapViewResource spot)
	{
		if (m_spotList.Contains(spot))
		{
			DebugLog.Warn("spot " + spot.gameObject.name + " already added to ResourceManager - something went wrong");
			return;
		}
		DebugLog.Log("Add Resource spot " + spot.gameObject.name);
		m_spotList.Add(spot);
	}

	public void RemoveResourceSpot(HotSpotWorldMapViewResource spot)
	{
		if (!m_spotList.Contains(spot))
		{
			DebugLog.Warn("try to remove node that is not in list in ResourceManager - something went wrong");
			return;
		}
		LeaveWorldMap();
		m_spotList.Remove(spot);
	}

	public void SpawnResource(GameObject go)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(m_resourcePrefabs[0]);
		gameObject.transform.position = go.transform.position + new Vector3(0f, 0f, -4f);
		UnityEngine.Object.Destroy(gameObject, 2f);
	}

	public void LateInit()
	{
		m_amountResourcesPossible = GetCurrentMaxResources(m_spotList.Count);
		for (int i = 0; i < m_spotList.Count; i++)
		{
			HotSpotWorldMapViewResource hotSpotWorldMapViewResource = m_spotList[i];
			if (hotSpotWorldMapViewResource.IsResourceAvaible())
			{
				m_currentAmount++;
			}
		}
		StartCoroutine("UpdateCooldowns");
	}

	private IEnumerator UpdateCooldowns()
	{
		while (true)
		{
			CheckGlobalCoolDown();
			yield return new WaitForSeconds(m_UpdateTime);
		}
	}

	public void LeaveWorldMap()
	{
		StopCoroutine("UpdateCooldowns");
	}

	public void NodeHarvested()
	{
		StartCoroutine(DIContainerLogic.GetTimingService().GetTrustedTime(delegate(DateTime trustedTime)
		{
			m_timeStampList.Add(trustedTime + TimeSpan.FromSeconds(m_UpdateTime));
			DebugLog.Log("Add cooldown " + m_timeStampList[m_timeStampList.Count - 1]);
		}));
	}

	private int GetCurrentMaxResources(int currentSpotAmount)
	{
		DebugLog.Warn("Not Final Implemented - need connection to balancing data");
		return (m_currentAmount + 1) / 2;
	}

	private void CheckGlobalCoolDown()
	{
		DateTime trustedTime;
		if (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			return;
		}
		for (int i = 0; i < m_timeStampList.Count; i++)
		{
			DebugLog.Log(string.Concat("CheckCooldown ", m_timeStampList[i], "    at ", trustedTime));
			if (trustedTime > m_timeStampList[i])
			{
				DebugLog.Log("Respawn");
				m_timeStampList.RemoveAt(i);
				SpawnNewNode();
				i--;
			}
		}
	}

	private void SpawnNewNode()
	{
		if (m_spotList.Count == 0)
		{
			DebugLog.Error("spotlist is 0");
			return;
		}
		int num = 0;
		do
		{
			int index = UnityEngine.Random.Range(0, m_spotList.Count);
			if (!m_spotList[index].IsResourceAvaible())
			{
				DebugLog.Log(m_spotList[index].gameObject.name + " respawn ");
				m_spotList[index].Respawn();
				return;
			}
			num++;
		}
		while (num < 1000);
		DebugLog.Error("sec coutner reached 1000");
	}

	private void OnDestroy()
	{
		_instance = null;
	}
}

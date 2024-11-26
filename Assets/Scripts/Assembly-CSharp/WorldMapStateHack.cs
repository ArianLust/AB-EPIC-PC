using System.Collections;
using UnityEngine;

public class WorldMapStateHack : MonoBehaviour
{
	public Transform m_Anvil;

	public Transform m_BirdCage;

	public Transform m_RoguePigAnvil;

	public Transform m_RoguePigBoss;

	public GameObject m_AnvilPrefab;

	public CharacterControllerWorldMap m_CharacterControllerWorldmap;

	public IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();
		ExecuteHack();
		yield return new WaitForSeconds(1.1f);
		RefreshHack();
	}

	private void RefreshHack()
	{
		if (IsUnlockedHack("hotspot_003_battleground"))
		{
			Object.Destroy(m_Anvil.gameObject);
			Object.Destroy(m_RoguePigAnvil.gameObject);
		}
		if (IsUnlockedHack("hotspot_005_battleground"))
		{
			Object.Destroy(m_BirdCage.gameObject);
		}
		if (IsUnlockedHack("hotspot_007_battleground"))
		{
			Object.Destroy(m_RoguePigBoss.gameObject);
		}
	}

	private void ExecuteHack()
	{
		if (IsUnlockedHack("hotspot_001_battleground"))
		{
			DebugLog.Log("S2");
			if (!IsUnlockedHack("hotspot_003_battleground"))
			{
				CharacterControllerWorldMap characterControllerWorldMap = Object.Instantiate(m_CharacterControllerWorldmap, m_RoguePigAnvil.position, Quaternion.identity) as CharacterControllerWorldMap;
				characterControllerWorldMap.transform.parent = m_RoguePigAnvil;
				characterControllerWorldMap.SetModel("pig_minion_rogue");
				characterControllerWorldMap.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
				GameObject gameObject = (GameObject)Object.Instantiate(m_AnvilPrefab, m_Anvil.position, Quaternion.identity);
				gameObject.transform.parent = m_Anvil;
			}
			if (!IsUnlockedHack("hotspot_005_battleground"))
			{
				CharacterControllerWorldMap characterControllerWorldMap2 = Object.Instantiate(m_CharacterControllerWorldmap, m_BirdCage.position, Quaternion.identity) as CharacterControllerWorldMap;
				characterControllerWorldMap2.transform.parent = m_BirdCage;
				characterControllerWorldMap2.SetModel("pig_yellow_cage");
				characterControllerWorldMap2.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
			}
			if (!IsUnlockedHack("hotspot_007_battleground"))
			{
				CharacterControllerWorldMap characterControllerWorldMap3 = Object.Instantiate(m_CharacterControllerWorldmap, m_RoguePigBoss.position, Quaternion.identity) as CharacterControllerWorldMap;
				characterControllerWorldMap3.transform.parent = m_RoguePigBoss;
				characterControllerWorldMap3.SetModel("pig_minion_rogue_boss_firstencounter");
				characterControllerWorldMap3.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
			}
		}
	}

	private bool IsUnlockedHack(string hotspotId)
	{
		HotSpotWorldMapViewBase[] componentsInChildren = base.gameObject.GetComponentsInChildren<HotSpotWorldMapViewBase>(true);
		foreach (HotSpotWorldMapViewBase hotSpotWorldMapViewBase in componentsInChildren)
		{
			if (hotSpotWorldMapViewBase.Model.BalancingData.NameId == hotspotId && hotSpotWorldMapViewBase.Model.GetStarCount() > 0)
			{
				return true;
			}
		}
		return false;
	}
}

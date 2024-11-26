using System.Collections.Generic;
using ABH.GameDatas;
using UnityEngine;

public class UpgradeDetailInfo : MonoBehaviour
{
	[SerializeField]
	private UILabel m_UpgradeName;

	[SerializeField]
	private List<UISprite> m_CheckMarks = new List<UISprite>();

	[SerializeField]
	private List<GameObject> m_AnvilPrefabs = new List<GameObject>();

	[SerializeField]
	private List<GameObject> m_CauldronPrefabs = new List<GameObject>();

	[SerializeField]
	public UIInputTrigger m_BuyButton;

	private IInventoryItemGameData m_Model;

	public bool SetModel(IInventoryItemGameData item)
	{
		if (item == null)
		{
			base.gameObject.SetActive(false);
			return false;
		}
		if (item.ItemBalancing.NameId == "cauldron_leveled")
		{
			m_UpgradeName.text = DIContainerInfrastructure.GetLocaService().Tr("offer_cauldron_upgrade_" + item.ItemData.Level.ToString("00") + "_name");
		}
		else if (item.ItemBalancing.NameId == "forge_leveled")
		{
			m_UpgradeName.text = DIContainerInfrastructure.GetLocaService().Tr("offer_forge_upgrade_" + item.ItemData.Level.ToString("00") + "_name");
		}
		m_Model = item;
		if (item.ItemData.Level >= 3)
		{
			foreach (GameObject cauldronPrefab in m_CauldronPrefabs)
			{
				cauldronPrefab.SetActive(false);
			}
			foreach (GameObject anvilPrefab in m_AnvilPrefabs)
			{
				anvilPrefab.SetActive(false);
			}
			base.gameObject.SetActive(false);
			return false;
		}
		if (item.ItemBalancing.NameId == "forge_leveled")
		{
			base.gameObject.SetActive(true);
			for (int i = 0; i < m_AnvilPrefabs.Count; i++)
			{
				GameObject gameObject = m_AnvilPrefabs[i];
				gameObject.SetActive(true);
				if (item.ItemData.Level >= i + 1)
				{
					m_CheckMarks[i].gameObject.SetActive(true);
				}
				else
				{
					m_CheckMarks[i].gameObject.SetActive(false);
				}
			}
			foreach (GameObject cauldronPrefab2 in m_CauldronPrefabs)
			{
				cauldronPrefab2.SetActive(false);
			}
		}
		else if (item.ItemBalancing.NameId == "cauldron_leveled")
		{
			base.gameObject.SetActive(true);
			for (int j = 0; j < m_CauldronPrefabs.Count; j++)
			{
				GameObject gameObject2 = m_CauldronPrefabs[j];
				gameObject2.SetActive(true);
				if (item.ItemData.Level >= j + 1)
				{
					m_CheckMarks[j].gameObject.SetActive(true);
				}
				else
				{
					m_CheckMarks[j].gameObject.SetActive(false);
				}
			}
			foreach (GameObject anvilPrefab2 in m_AnvilPrefabs)
			{
				anvilPrefab2.SetActive(false);
			}
		}
		return true;
	}
}

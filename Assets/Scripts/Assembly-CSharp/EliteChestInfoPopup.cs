using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

public class EliteChestInfoPopup : MonoBehaviour
{
	public UIGrid m_ContentGrid;

	public GameObject m_ClassUpgradePrefab;

	public GameObject m_MasteryPrefab;

	public GameObject m_ResourcePrefab;

	public UILabel m_InfoLabelPreview;

	public UILabel m_InfoLabelGained;

	public void InitializeItems(List<IInventoryItemGameData> items)
	{
		StartCoroutine(InitializeItemsCoroutine(items));
	}

	private IEnumerator InitializeItemsCoroutine(List<IInventoryItemGameData> items)
	{
		ClearGrid();
		yield return new WaitForEndOfFrame();
		foreach (IInventoryItemGameData item in items)
		{
			LootDisplayContoller cPreview;
			switch (item.ItemBalancing.ItemType)
			{
			case InventoryItemType.Skin:
				cPreview = InstantiatePrefab(m_ClassUpgradePrefab);
				SetLabel("01");
				break;
			case InventoryItemType.Mastery:
				cPreview = InstantiatePrefab(m_MasteryPrefab);
				SetLabel("02");
				break;
			default:
				cPreview = InstantiatePrefab(m_ResourcePrefab);
				SetLabel("03");
				break;
			}
			if ((bool)cPreview)
			{
				cPreview.SetModel(item, null, LootDisplayType.Major);
			}
		}
		m_ContentGrid.repositionNow = true;
	}

	private void SetLabel(string suffix)
	{
		if (m_InfoLabelPreview != null)
		{
			m_InfoLabelPreview.text = DIContainerInfrastructure.GetLocaService().Tr("elitechestcollection_desc_" + suffix);
		}
		else if (m_InfoLabelGained != null)
		{
			m_InfoLabelGained.text = DIContainerInfrastructure.GetLocaService().Tr("elitechestcollection_desc_small_" + suffix);
		}
	}

	private void ClearGrid()
	{
		foreach (Transform item in m_ContentGrid.transform)
		{
			Object.Destroy(item.gameObject);
		}
	}

	private LootDisplayContoller InstantiatePrefab(GameObject typePrefab)
	{
		GameObject gameObject = Object.Instantiate(typePrefab);
		gameObject.transform.parent = m_ContentGrid.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		return gameObject.transform.GetComponent<LootDisplayContoller>();
	}
}

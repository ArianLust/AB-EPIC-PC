using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

public class MaterialOverlay : MonoBehaviour
{
	private Camera m_InterfaceCamera;

	public UILabel m_Header;

	public UILabel m_ItemDesc;

	public UILabel m_ObtainDesc;

	public List<LootDisplayContoller> m_NeededLoot;

	public List<UILabel> m_NeededLootCurrently;

	public LootDisplayContoller m_ResultLoot;

	public ContainerControl m_ContainerControl;

	public ContainerControl m_AllOverlaysContainerControl;

	private Vector3 initialSize;

	private Vector3 initialContainerControlPos;

	private Vector3 initialContainerControlSize;

	private Vector3 initialArrowSize;

	public float m_OffsetLeft = 50f;

	private void Awake()
	{
		m_InterfaceCamera = DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera;
		initialSize = m_ContainerControl.m_Size;
		initialContainerControlPos = m_ContainerControl.transform.localPosition;
		initialContainerControlSize = m_ContainerControl.m_Size;
	}

	public void ShowCraftingItemOverlay(Transform root, CraftingItemGameData craftingItem, Camera orientatedCamera)
	{
		SetContent(craftingItem);
		DebugLog.Log("Begin show Crafting Item Overlay " + craftingItem.ItemLocalizedName);
		Show(root, orientatedCamera);
	}

	public void ShowEventCampaignCollectionComponentItemOverlay(Transform root, BasicItemGameData collectionComponent, Camera orientatedCamera)
	{
		SetContent(collectionComponent);
		DebugLog.Log("Begin show Crafting Item Overlay for Collection item " + collectionComponent.ItemLocalizedName);
		Show(root, orientatedCamera);
	}

	private void Show(Transform root, Camera orientatedCamera)
	{
		Vector3 anchorPosition = m_InterfaceCamera.ScreenToWorldPoint(orientatedCamera.WorldToScreenPoint(root.position));
		base.transform.localPosition = new Vector3(anchorPosition.x, anchorPosition.y, base.transform.localPosition.z);
		m_ContainerControl.transform.localPosition = PositionRelativeToAnchorPositionFixedOnAnchor(anchorPosition, m_OffsetLeft);
		GetComponent<Animation>().Play("InfoOverlay_Enter");
	}

	private void SetContent(CraftingItemGameData cItem)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if ((bool)m_Header)
		{
			m_Header.text = cItem.ItemLocalizedName;
		}
		m_ItemDesc.text = cItem.ItemLocalizedTooltipDesc(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData);
		Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
		List<KeyValuePair<string, int>> craftingCosts = new List<KeyValuePair<string, int>>();
		DIContainerLogic.CraftingService.AddCraftingCostsForItem(ref craftingCosts, cItem.BalancingData.NameId, cItem.Data.Level);
		foreach (KeyValuePair<string, int> item in craftingCosts)
		{
			dictionary2.Add(item.Key, item.Value);
		}
		List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLootPreview(dictionary2, 1));
		if (m_NeededLoot != null && m_ResultLoot != null)
		{
			int num = 0;
			for (num = 0; num < Mathf.Min(itemsFromLoot.Count, m_NeededLoot.Count); num++)
			{
				m_NeededLoot[num].gameObject.SetActive(true);
				IInventoryItemGameData inventoryItemGameData = itemsFromLoot[num];
				m_NeededLoot[num].SetModel(inventoryItemGameData, new List<IInventoryItemGameData>(), LootDisplayType.None, "_Large", true);
				m_NeededLootCurrently[num].text = "[" + DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, inventoryItemGameData)) + "]";
			}
			for (int i = num; i < m_NeededLoot.Count; i++)
			{
				m_NeededLoot[num].gameObject.SetActive(false);
			}
			m_ResultLoot.SetModel(cItem, new List<IInventoryItemGameData>(), LootDisplayType.None);
		}
		m_ObtainDesc.text = cItem.ItemLocalizedDesc;
	}

	private void SetContent(BasicItemGameData cItem)
	{
		if (cItem != null && cItem.BalancingData.ItemType == InventoryItemType.CollectionComponent)
		{
			if ((bool)m_Header)
			{
				m_Header.text = cItem.ItemLocalizedName;
			}
			m_ItemDesc.text = cItem.ItemLocalizedTooltipDesc(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData);
			m_ObtainDesc.text = cItem.ItemLocalizedDesc;
		}
	}

	private Vector3 PositionRelativeToAnchorPositionFixedOnAnchor(Vector3 anchorPosition, float offset)
	{
		if (Mathf.Sign(anchorPosition.x) < 0f)
		{
			return new Vector3(initialContainerControlPos.x, initialContainerControlPos.y + InfoOverlayMgr.GetOverlayY(anchorPosition.y, new Vector2(initialContainerControlSize.y * 0.5f, (0f - initialContainerControlSize.y) * 0.5f), m_AllOverlaysContainerControl) - anchorPosition.y, initialContainerControlPos.z);
		}
		return new Vector3(0f - initialContainerControlPos.x, initialContainerControlPos.y + InfoOverlayMgr.GetOverlayY(anchorPosition.y, new Vector2(initialContainerControlSize.y * 0.5f, (0f - initialContainerControlSize.y) * 0.5f), m_AllOverlaysContainerControl) - anchorPosition.y, initialContainerControlPos.z);
	}

	private Vector3 PositionRelativeToAnchorPositionFixedOnScreen(Vector3 anchorPosition, float offset)
	{
		return new Vector3(Mathf.Sign(anchorPosition.x) * -1f * (m_AllOverlaysContainerControl.m_Size.x * 0.5f + -1f * (m_ContainerControl.m_Size.x * 0.5f + offset)), initialContainerControlPos.y, initialContainerControlPos.z);
	}

	public void Hide()
	{
		if (base.gameObject.activeInHierarchy)
		{
			GetComponent<Animation>().Play("InfoOverlay_Leave");
			Invoke("Disable", GetComponent<Animation>()["InfoOverlay_Leave"].length);
		}
	}

	private void Disable()
	{
		base.gameObject.SetActive(false);
	}
}

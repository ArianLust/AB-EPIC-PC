using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using UnityEngine;

public class ConsumableOverlay : MonoBehaviour
{
	[SerializeField]
	private GameObject m_SmallBody;

	[SerializeField]
	private GameObject m_LargeBody;

	private Camera m_InterfaceCamera;

	public UILabel m_Header;

	public UILabel m_ConsumableDesc;

	public UILabel m_ObtainDesc;

	public UILabel m_LevelNumber;

	public UILabel m_LevelNeededDesc;

	public UILabel m_ObtainText;

	public List<LootDisplayContoller> m_LootDisplays = new List<LootDisplayContoller>(3);

	public List<UILabel> m_LootCurrentLabel = new List<UILabel>(3);

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

	public void ShowConsumableOverlay(Transform root, ConsumableItemGameData consumable, Camera orientatedCamera)
	{
		SetContent(consumable);
		Show(root, orientatedCamera);
	}

	public void ShowConsumableOverlay(Transform root, ConsumableItemGameData consumable, Camera orientatedCamera, int levelReq)
	{
		SetContent(consumable, levelReq);
		Show(root, orientatedCamera);
	}

	public void ShowItemBundleOverlay(Transform root, List<IInventoryItemGameData> items, BasicShopOfferBalancingData shopoffer, Camera orientatedCamera)
	{
		SetItemListContent(items, shopoffer);
		Show(root, orientatedCamera);
	}

	private void Show(Transform root, Camera orientatedCamera)
	{
		Vector3 anchorPosition = m_InterfaceCamera.ScreenToWorldPoint(orientatedCamera.WorldToScreenPoint(root.position));
		base.transform.localPosition = new Vector3(anchorPosition.x, anchorPosition.y, base.transform.localPosition.z);
		m_ContainerControl.transform.localPosition = PositionRelativeToAnchorPositionFixedOnAnchor(anchorPosition, m_OffsetLeft);
		GetComponent<Animation>().Play("InfoOverlay_Enter");
	}

	private void SetContent(ConsumableItemGameData consumable, int levelReq = 0)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if ((bool)m_Header)
		{
			m_Header.text = consumable.ItemLocalizedName;
		}
		if ((bool)m_LevelNumber)
		{
			m_LevelNumber.text = levelReq.ToString("0");
		}
		if ((bool)m_LevelNeededDesc)
		{
			m_LevelNeededDesc.text = DIContainerInfrastructure.GetLocaService().Tr("recipe_levelrequirement", new Dictionary<string, string> { 
			{
				"{value_1}",
				levelReq.ToString("0")
			} });
		}
		m_ConsumableDesc.text = consumable.ItemLocalizedDesc;
		Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
		List<KeyValuePair<string, int>> craftingCosts = new List<KeyValuePair<string, int>>();
		DIContainerLogic.CraftingService.AddCraftingCostsForItem(ref craftingCosts, consumable.BalancingData.NameId, consumable.Data.Level);
		foreach (KeyValuePair<string, int> item in craftingCosts)
		{
			dictionary2.Add(item.Key, item.Value);
		}
		List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLootPreview(dictionary2, 1));
		if ((bool)m_SmallBody)
		{
			m_SmallBody.SetActive(itemsFromLoot.Count <= 3);
		}
		if ((bool)m_LargeBody)
		{
			m_LargeBody.SetActive(itemsFromLoot.Count > 3);
		}
		for (int i = 0; i < Mathf.Min(m_LootDisplays.Count, m_LootCurrentLabel.Count); i++)
		{
			if (itemsFromLoot.Count > i)
			{
				m_LootDisplays[i].gameObject.SetActive(true);
				m_LootDisplays[i].SetModel(itemsFromLoot[i], new List<IInventoryItemGameData>(), LootDisplayType.None, "_Small", true);
				m_LootCurrentLabel[i].gameObject.SetActive(true);
			}
			else
			{
				m_LootCurrentLabel[i].gameObject.SetActive(false);
				m_LootDisplays[i].gameObject.SetActive(false);
			}
		}
		if (itemsFromLoot.Count <= 0)
		{
			m_ObtainText.text = DIContainerInfrastructure.GetLocaService().Tr("tt_item_obtained_for", "Obtained for");
			m_ObtainDesc.gameObject.SetActive(true);
			m_ObtainDesc.text = DIContainerInfrastructure.GetLocaService().GetItemTooltipDesc(consumable.BalancingData.LocaBaseId, new Dictionary<string, string>());
		}
		else
		{
			m_ObtainText.text = DIContainerInfrastructure.GetLocaService().Tr("tt_item_craft_for", "Craft for");
			m_ObtainDesc.gameObject.SetActive(false);
		}
	}

	private void SetItemListContent(List<IInventoryItemGameData> items, BasicShopOfferBalancingData shopoffer)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if ((bool)m_Header)
		{
			m_Header.text = DIContainerInfrastructure.GetLocaService().GetConsumableName(shopoffer.LocaId);
		}
		m_ConsumableDesc.text = DIContainerInfrastructure.GetLocaService().GetConsumableDesc(shopoffer.LocaId, new Dictionary<string, string>());
		List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
		foreach (IInventoryItemGameData item in items)
		{
			if (!(item.Name == "unlock_skin"))
			{
				if (item is SkinItemGameData || item is ClassItemGameData)
				{
					list.Insert(0, item);
				}
				else
				{
					list.Add(item);
				}
			}
		}
		m_SmallBody.SetActive(list.Count <= 3);
		m_LargeBody.SetActive(list.Count > 3);
		for (int i = 0; i < Mathf.Min(m_LootDisplays.Count, m_LootCurrentLabel.Count); i++)
		{
			if (list.Count > i)
			{
				m_LootDisplays[i].gameObject.SetActive(true);
				m_LootDisplays[i].SetModel(list[i], new List<IInventoryItemGameData>(), LootDisplayType.None, "_Small");
				m_LootCurrentLabel[i].gameObject.SetActive(false);
			}
			else
			{
				m_LootCurrentLabel[i].gameObject.SetActive(false);
				m_LootDisplays[i].gameObject.SetActive(false);
			}
		}
		m_ObtainText.text = DIContainerInfrastructure.GetLocaService().Tr("tt_bundle_contains");
		m_ObtainDesc.gameObject.SetActive(false);
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

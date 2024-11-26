using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

public class EquipmentOverlay : MonoBehaviour
{
	private Camera m_InterfaceCamera;

	public UILabel m_Header;

	public GameObject m_EnchantmentParent;

	public UILabel m_EnchantmentLabel;

	public UISprite m_EnchantmentProgress;

	public UISprite m_EnchantmentSprite;

	public StatisticsElement m_Stats;

	public UILabel m_EquipmentDesc;

	public UILabel m_LevelText;

	public SetItemInfo m_SetItemInfo;

	public List<LootDisplayContoller> m_LootDisplays = new List<LootDisplayContoller>(3);

	public List<UILabel> m_LootCurrentLabel = new List<UILabel>(3);

	public UILabel m_PerkName;

	public UILabel m_PerkDesc;

	public UISprite m_PerkIcon;

	public UISprite m_Arrow;

	public ContainerControl m_ContainerControl;

	public ContainerControl m_AllOverlaysContainerControl;

	private Vector3 initialSize;

	public Vector2 blindSize;

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
		if ((bool)m_Arrow)
		{
			initialArrowSize = m_Arrow.cachedTransform.localScale;
		}
	}

	public void ShowEquipmentOverlay(Transform root, EquipmentGameData equip, Camera orientatedCamera, bool isArena)
	{
		FillEquipmentContent(equip, isArena);
		if ((bool)m_LevelText)
		{
			m_LevelText.gameObject.SetActive(true);
			m_LevelText.text = DIContainerInfrastructure.GetLocaService().Tr("player_stat_itemlevel").Replace("{value_1}", equip.Data.Level.ToString());
		}
		if (equip.AllowEnchanting() && (bool)m_EnchantmentParent)
		{
			m_EnchantmentParent.SetActive(true);
			m_EnchantmentLabel.enabled = true;
			m_EnchantmentLabel.text = equip.EnchantementLevel.ToString();
			m_EnchantmentProgress.fillAmount = equip.EnchantmentProgress;
			if (equip.IsMaxEnchanted() && equip.EnchantementLevel == 0)
			{
				m_EnchantmentLabel.enabled = false;
				m_EnchantmentSprite.spriteName = "Enchantment_NA";
			}
			else if (equip.IsMaxEnchanted())
			{
				m_EnchantmentSprite.spriteName = "Enchantment_Max";
			}
			else
			{
				m_EnchantmentSprite.spriteName = "Enchantment";
			}
		}
		else if ((bool)m_EnchantmentParent)
		{
			m_EnchantmentParent.SetActive(false);
		}
		Vector3 anchorPosition = m_InterfaceCamera.ScreenToWorldPoint(orientatedCamera.WorldToScreenPoint(root.position));
		DebugLog.Log("Begin show Equipment Overlay for: " + equip.Name);
		base.transform.localPosition = new Vector3(anchorPosition.x, anchorPosition.y, base.transform.localPosition.z);
		m_ContainerControl.transform.localPosition = PositionRelativeToAnchorPositionFixedOnAnchor(anchorPosition, m_OffsetLeft);
		if ((bool)m_Arrow)
		{
			m_Arrow.cachedTransform.localPosition = PositionArrowRelativeToAnchorPositionFixedOnScreen(anchorPosition, m_OffsetLeft);
			m_Arrow.cachedTransform.localScale = Vector3.Scale(initialArrowSize, new Vector3(-1f * Mathf.Sign(anchorPosition.x), 1f, 1f));
			m_Arrow.gameObject.SetActive(false);
		}
		GetComponent<Animation>().Play("InfoOverlay_Enter");
	}

	private void FillEquipmentContent(EquipmentGameData equip, bool isArena)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		BirdGameData birdGameData = DIContainerInfrastructure.GetCurrentPlayer().GetBird(equip.BalancingData.RestrictedBirdId, true);
		if (birdGameData == null)
		{
			birdGameData = new BirdGameData(equip.BalancingData.RestrictedBirdId);
		}
		if ((bool)m_Stats)
		{
			switch (equip.BalancingData.ItemType)
			{
			case InventoryItemType.MainHandEquipment:
				if ((bool)m_Stats)
				{
					m_Stats.SetIconSprite("Character_Damage_Large");
					m_Stats.RefreshStat(false, birdGameData != null, equip.ItemMainStat, (birdGameData == null) ? 0f : birdGameData.MainHandItem.ItemMainStat);
				}
				break;
			case InventoryItemType.OffHandEquipment:
				if ((bool)m_Stats)
				{
					m_Stats.SetIconSprite("Character_Health_Large");
					m_Stats.RefreshStat(false, birdGameData != null, equip.ItemMainStat, (birdGameData == null) ? 0f : birdGameData.OffHandItem.ItemMainStat);
				}
				break;
			}
		}
		if ((bool)m_SetItemInfo)
		{
			m_SetItemInfo.SetModel(equip, birdGameData, isArena);
		}
		List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
		if (m_LootDisplays.Count > 0)
		{
			if (equip.GetScrapLoot() != null)
			{
				list = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(equip.GetScrapLoot(), 0));
			}
			for (int i = 0; i < Mathf.Min(m_LootDisplays.Count, m_LootCurrentLabel.Count); i++)
			{
				if (list.Count > i)
				{
					m_LootDisplays[i].gameObject.SetActive(true);
					m_LootDisplays[i].SetModel(list[i], new List<IInventoryItemGameData>(), LootDisplayType.None, "_Small");
					m_LootCurrentLabel[i].gameObject.SetActive(true);
				}
				else
				{
					m_LootCurrentLabel[i].gameObject.SetActive(false);
					m_LootDisplays[i].gameObject.SetActive(false);
				}
			}
		}
		if ((bool)m_PerkName)
		{
			m_PerkName.text = EquipmentGameData.GetPerkName(equip);
		}
		if ((bool)m_PerkIcon)
		{
			m_PerkIcon.spriteName = EquipmentGameData.GetPerkIcon(equip);
		}
		if ((bool)m_PerkDesc)
		{
			m_PerkDesc.text = EquipmentGameData.GetPerkDesc(equip);
		}
		string text = DIContainerInfrastructure.GetLocaService().Tr("tt_item_equipment_desc", "Scrap for");
		if ((bool)m_EquipmentDesc)
		{
			if ((bool)m_Header)
			{
				m_Header.text = equip.ItemLocalizedName;
			}
			m_EquipmentDesc.text = text;
		}
		else if ((bool)m_Header)
		{
			m_Header.text = DIContainerInfrastructure.GetLocaService().Tr("prefix_blueprint", "?Blueprint: ?") + equip.ItemLocalizedName;
		}
	}

	public void ShowRecipeOverlay(Transform root, CraftingRecipeGameData recipe, EquipmentGameData equip, Camera orientatedCamera)
	{
		if (m_InterfaceCamera == null)
		{
			DebugLog.Error("Interface Camera is missing");
		}
		if (orientatedCamera == null)
		{
			DebugLog.Error("orientated Camera is missing");
		}
		if (root == null)
		{
			DebugLog.Error("Root is missing");
		}
		if ((bool)m_LevelText)
		{
			m_LevelText.gameObject.SetActive(false);
		}
		m_EnchantmentParent.SetActive(false);
		m_EnchantmentLabel.enabled = false;
		Vector3 anchorPosition = m_InterfaceCamera.ScreenToWorldPoint(orientatedCamera.WorldToScreenPoint(root.position));
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if ((bool)m_Header)
		{
			m_Header.text = DIContainerInfrastructure.GetLocaService().Tr("prefix_blueprint", "?Blueprint: ?") + equip.ItemLocalizedName;
		}
		BirdGameData birdGameData = DIContainerInfrastructure.GetCurrentPlayer().Birds.FirstOrDefault((BirdGameData b) => b.BalancingData.NameId == equip.BalancingData.RestrictedBirdId);
		switch (equip.BalancingData.ItemType)
		{
		case InventoryItemType.MainHandEquipment:
			if ((bool)m_Stats)
			{
				m_Stats.SetIconSprite("Character_Damage_Large");
				m_Stats.RefreshStat(false, true, EquipmentGameData.GetItemMainStat(equip, 3), (birdGameData == null) ? 0f : birdGameData.MainHandItem.ItemMainStat);
			}
			break;
		case InventoryItemType.OffHandEquipment:
			if ((bool)m_Stats)
			{
				m_Stats.SetIconSprite("Character_Health_Large");
				m_Stats.RefreshStat(false, true, EquipmentGameData.GetItemMainStat(equip, 3), (birdGameData == null) ? 0f : birdGameData.OffHandItem.ItemMainStat);
			}
			break;
		}
		if ((bool)m_PerkIcon)
		{
			m_PerkIcon.spriteName = EquipmentGameData.GetPerkIcon(equip);
		}
		if ((bool)m_PerkDesc)
		{
			m_PerkDesc.text = EquipmentGameData.GetPerkDesc(equip);
		}
		string text = DIContainerInfrastructure.GetLocaService().Tr("tt_item_craft_for", "Craft for");
		if ((bool)m_EquipmentDesc)
		{
			m_EquipmentDesc.text = text;
		}
		Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
		List<KeyValuePair<string, int>> craftingCosts = new List<KeyValuePair<string, int>>();
		DIContainerLogic.CraftingService.AddCraftingCostsForItem(ref craftingCosts, equip.BalancingData.NameId, equip.Data.Level);
		foreach (KeyValuePair<string, int> item in craftingCosts)
		{
			dictionary2.Add(item.Key, item.Value);
		}
		List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLootPreview(dictionary2, 1));
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
		DebugLog.Log("Begin show Equipment Recipe Overlay for: " + recipe.Name);
		base.transform.localPosition = new Vector3(anchorPosition.x, anchorPosition.y, base.transform.localPosition.z);
		Transform obj = m_ContainerControl.transform;
		Vector3 localPosition = PositionRelativeToAnchorPositionFixedOnAnchor(anchorPosition, m_OffsetLeft);
		m_ContainerControl.transform.localPosition = localPosition;
		obj.localPosition = localPosition;
		if ((bool)m_Arrow)
		{
			m_Arrow.cachedTransform.localPosition = PositionArrowRelativeToAnchorPositionFixedOnScreen(anchorPosition, m_OffsetLeft);
			m_Arrow.cachedTransform.localScale = Vector3.Scale(initialArrowSize, new Vector3(-1f * Mathf.Sign(anchorPosition.x), 1f, 1f));
			m_Arrow.gameObject.SetActive(false);
		}
		GetComponent<Animation>().Play("InfoOverlay_Enter");
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

	private Vector3 PositionArrowRelativeToAnchorPositionFixedOnAnchor(Vector3 anchorPosition)
	{
		return new Vector3(anchorPosition.x + -1f * Mathf.Sign(anchorPosition.x) * initialArrowSize.x, anchorPosition.y, m_Arrow.cachedTransform.localPosition.z);
	}

	private Vector3 PositionArrowRelativeToAnchorPositionFixedOnScreen(Vector3 anchorPosition, float offset)
	{
		return new Vector3(Mathf.Sign(anchorPosition.x) * -1f * (m_AllOverlaysContainerControl.m_Size.x * 0.5f + -1f * (initialArrowSize.x + m_ContainerControl.m_Size.x + offset)), anchorPosition.y, m_Arrow.cachedTransform.localPosition.z);
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

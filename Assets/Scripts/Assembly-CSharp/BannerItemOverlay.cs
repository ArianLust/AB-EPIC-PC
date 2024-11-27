using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

public class BannerItemOverlay : MonoBehaviour
{
	public GameObject m_EnchantmentParent;

	public UILabel m_EnchantmentLabel;

	public UISprite m_EnchantmentProgress;

	public UISprite m_EnchantmentSprite;

	private Camera m_InterfaceCamera;

	public UILabel m_Header;

	public StatisticsElement m_Stats;

	public UILabel m_EquipmentDesc;

	public UILabel m_ItemLevelText;

	public SetBannerInfo m_SetBannerInfo;

	public SkillBlind m_EliteEmblemInfo;

	public List<LootDisplayContoller> m_LootDisplays = new List<LootDisplayContoller>(3);

	public List<UILabel> m_LootCurrentLabel = new List<UILabel>(3);

	public UISprite m_Arrow;

	public ContainerControl m_ContainerControl;

	public ContainerControl m_AllOverlaysContainerControl;

	private Vector3 initialSize;

	public Vector2 blindSize;

	private Vector3 initialContainerControlPos;

	private Vector3 initialContainerControlSize;

	private Vector3 initialArrowSize;

	public float m_OffsetLeft = 50f;

	public SkillBlind m_SkillBlind;

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

	public void ShowBannerItemOverlay(Transform root, BannerItemGameData equip, BannerGameData currentBanner, Camera orientatedCamera)
	{
		FillEquipmentContent(equip, currentBanner);
		if (equip.AllowEnchanting())
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
		else
		{
			m_EnchantmentParent.SetActive(false);
		}
		m_ItemLevelText.text = DIContainerInfrastructure.GetLocaService().Tr("player_stat_itemlevel").Replace("{value_1}", equip.Data.Level.ToString());
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

	private void FillEquipmentContent(BannerItemGameData equip, BannerGameData currentBanner)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if ((bool)m_Stats)
		{
			m_Stats.SetIconSprite("Character_Health_Large");
			BannerItemGameData bannerItemGameData = currentBanner.BannerCenter;
			switch (equip.BalancingData.ItemType)
			{
			case InventoryItemType.Banner:
				bannerItemGameData = currentBanner.BannerCenter;
				break;
			case InventoryItemType.BannerEmblem:
				bannerItemGameData = currentBanner.BannerEmblem;
				break;
			case InventoryItemType.BannerTip:
				bannerItemGameData = currentBanner.BannerTip;
				break;
			}
			m_Stats.RefreshStat(false, currentBanner != null, equip.ItemMainStat, (currentBanner == null) ? 0f : bannerItemGameData.ItemMainStat);
		}
		if (equip != null && equip.IsSetItem)
		{
			if ((bool)m_SetBannerInfo)
			{
				m_SetBannerInfo.SetModel(equip, currentBanner);
			}
			if ((bool)m_EliteEmblemInfo)
			{
				SkillBattleDataBase skill = equip.SetItemSkill.GenerateSkillBattleData();
				ICombatant invoker = null;
				if (currentBanner != null)
				{
					if (currentBanner is BannerGameData)
					{
						invoker = new BannerCombatant(currentBanner);
					}
					m_EliteEmblemInfo.gameObject.SetActive(true);
					m_EliteEmblemInfo.ShowSkillOverlay(skill, invoker, false);
				}
			}
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
			UIGrid component = m_LootDisplays[0].transform.parent.GetComponent<UIGrid>();
			if (component != null)
			{
				component.Reposition();
			}
		}
		SkillGameData primarySkill = equip.PrimarySkill;
		if (primarySkill != null)
		{
			SkillBattleDataBase skill2 = primarySkill.GenerateSkillBattleData();
			ICombatant invoker2 = null;
			if (currentBanner != null)
			{
				if (currentBanner is BannerGameData)
				{
					invoker2 = new BannerCombatant(currentBanner);
				}
				m_SkillBlind.gameObject.SetActive(true);
				m_SkillBlind.ShowSkillOverlay(skill2, invoker2, false);
			}
		}
		else
		{
			m_SkillBlind.gameObject.SetActive(false);
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

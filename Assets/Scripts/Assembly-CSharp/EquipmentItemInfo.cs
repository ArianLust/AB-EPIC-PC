using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

public class EquipmentItemInfo : MonoBehaviour
{
	private float m_OldStat;

	[SerializeField]
	private List<LootDisplayContoller> m_LootDisplays = new List<LootDisplayContoller>();

	[SerializeField]
	private StatisticsElement m_RegularStats;

	[SerializeField]
	private StatisticsElement m_SetInfoStats;

	[SerializeField]
	private GameObject m_RegularInfo;

	[SerializeField]
	private UILabel m_SetEffectName;

	[SerializeField]
	private GameObject m_SetInfo;

	[SerializeField]
	private UISprite m_PerkSprite;

	[SerializeField]
	private UILabel m_PerkName;

	[SerializeField]
	private UILabel m_EquipmentName;

	private EquipmentGameData m_Item;

	private bool m_isPvp;

	public void ShowPerkTooltip()
	{
		if (m_Item != null && (bool)m_PerkSprite)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowPerkOverlay(m_PerkSprite.cachedTransform, m_Item, true);
		}
	}

	public void ShowSetPerkTooltip()
	{
		if (m_Item != null && (bool)m_PerkSprite)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowSetPerkOverlay(m_PerkSprite.cachedTransform, m_Item, true, m_isPvp);
		}
	}

	public void ShowEquipmentTooltip()
	{
		if (m_Item != null && (bool)m_PerkSprite)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowItemOverlay(base.transform, m_Item, true, m_isPvp);
		}
	}

	public void SetModel(EquipmentGameData equipmentGameData, BirdGameData selectedBird, bool isPvp)
	{
		m_Item = equipmentGameData;
		m_isPvp = isPvp;
		StatisticsElement statisticsElement;
		if (!m_Item.IsSetItem)
		{
			m_RegularInfo.SetActive(true);
			if ((bool)m_SetInfo)
			{
				m_SetInfo.SetActive(false);
			}
			statisticsElement = m_RegularStats;
			DebugLog.Log("Regular Stats activated");
		}
		else
		{
			if ((bool)m_SetInfo)
			{
				m_SetInfo.SetActive(true);
				if ((bool)m_RegularInfo)
				{
					m_RegularInfo.SetActive(false);
				}
				statisticsElement = m_SetInfoStats;
			}
			else
			{
				if ((bool)m_RegularInfo)
				{
					m_RegularInfo.SetActive(true);
				}
				statisticsElement = m_RegularStats;
			}
			if ((bool)m_SetEffectName)
			{
				m_SetEffectName.text = m_Item.SetItemSkill.SkillLocalizedName;
			}
		}
		if ((bool)statisticsElement)
		{
			switch (equipmentGameData.BalancingData.ItemType)
			{
			case InventoryItemType.MainHandEquipment:
				statisticsElement.SetIconSprite("Character_Damage_Large");
				statisticsElement.SetValueLabel(m_Item.ItemMainStat);
				break;
			case InventoryItemType.OffHandEquipment:
				statisticsElement.SetIconSprite("Character_Health_Large");
				statisticsElement.SetValueLabel(m_Item.ItemMainStat);
				break;
			}
		}
		List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
		if (equipmentGameData.GetScrapLoot() != null)
		{
			list = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(equipmentGameData.GetScrapLoot(), 0));
		}
		if (m_LootDisplays != null)
		{
			for (int i = 0; i < m_LootDisplays.Count; i++)
			{
				if (list.Count > i)
				{
					m_LootDisplays[i].gameObject.SetActive(true);
					m_LootDisplays[i].SetModel(list[i], new List<IInventoryItemGameData>(), LootDisplayType.None, "_Small");
				}
				else
				{
					m_LootDisplays[i].gameObject.SetActive(false);
				}
			}
		}
		if ((bool)m_PerkSprite)
		{
			m_PerkSprite.spriteName = EquipmentGameData.GetPerkIcon(equipmentGameData);
		}
		if ((bool)m_PerkName)
		{
			m_PerkName.text = EquipmentGameData.GetPerkName(equipmentGameData);
		}
		if ((bool)statisticsElement)
		{
			statisticsElement.RefreshStat(false, false, equipmentGameData.ItemMainStat, m_OldStat);
		}
		if ((bool)m_EquipmentName)
		{
			m_EquipmentName.text = equipmentGameData.ItemLocalizedName;
		}
		m_OldStat = equipmentGameData.ItemMainStat;
	}
}

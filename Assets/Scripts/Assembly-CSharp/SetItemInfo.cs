using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.Shared.Generic;
using UnityEngine;

public class SetItemInfo : MonoBehaviour
{
	[SerializeField]
	private LootDisplayContoller m_MainHandItemLootDisplay;

	[SerializeField]
	private LootDisplayContoller m_OffHandItemLootDisplay;

	[SerializeField]
	private UILabel m_SetBonusTextLabel;

	private EquipmentGameData m_Item;

	public void SetModel(EquipmentGameData equipmentGameData, BirdGameData selectedBird, bool isArena)
	{
		base.gameObject.SetActive(selectedBird != null && equipmentGameData.IsSetItem);
		if (!equipmentGameData.IsSetItem)
		{
			return;
		}
		m_Item = equipmentGameData;
		if ((bool)m_SetBonusTextLabel)
		{
			if (isArena)
			{
				SkillBattleDataBase skillBattleDataBase = m_Item.PvpSetItemSkill.GenerateSkillBattleData();
				m_SetBonusTextLabel.text = skillBattleDataBase.GetLocalizedDescription(new BirdCombatant(selectedBird).SetPvPBird(DIContainerInfrastructure.GetCoreStateMgr().m_IsWithinPvP));
			}
			else
			{
				SkillBattleDataBase skillBattleDataBase2 = m_Item.SetItemSkill.GenerateSkillBattleData();
				m_SetBonusTextLabel.text = skillBattleDataBase2.GetLocalizedDescription(new BirdCombatant(selectedBird).SetPvPBird(DIContainerInfrastructure.GetCoreStateMgr().m_IsWithinPvP));
			}
		}
		if (m_Item.BalancingData.ItemType == InventoryItemType.MainHandEquipment)
		{
			m_MainHandItemLootDisplay.gameObject.SetActive(true);
			m_MainHandItemLootDisplay.SetModel(equipmentGameData, new List<IInventoryItemGameData>(), LootDisplayType.None);
			if (selectedBird.OffHandItem.BalancingData.NameId == equipmentGameData.CorrespondingSetItem.NameId)
			{
				m_OffHandItemLootDisplay.gameObject.SetActive(true);
				m_OffHandItemLootDisplay.SetModel(selectedBird.OffHandItem, new List<IInventoryItemGameData>(), LootDisplayType.None);
			}
			else
			{
				m_OffHandItemLootDisplay.gameObject.SetActive(true);
				m_OffHandItemLootDisplay.SetModel(DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(DIContainerInfrastructure.GetCurrentPlayer().Data.Level, 2, equipmentGameData.CorrespondingSetItem.NameId, 1), new List<IInventoryItemGameData>(), LootDisplayType.None);
			}
		}
		else
		{
			m_OffHandItemLootDisplay.gameObject.SetActive(true);
			m_OffHandItemLootDisplay.SetModel(equipmentGameData, new List<IInventoryItemGameData>(), LootDisplayType.None);
			if (selectedBird.MainHandItem.BalancingData.NameId == equipmentGameData.CorrespondingSetItem.NameId)
			{
				m_MainHandItemLootDisplay.gameObject.SetActive(true);
				m_MainHandItemLootDisplay.SetModel(selectedBird.MainHandItem, new List<IInventoryItemGameData>(), LootDisplayType.None);
			}
			else
			{
				m_MainHandItemLootDisplay.gameObject.SetActive(true);
				m_MainHandItemLootDisplay.SetModel(DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(DIContainerInfrastructure.GetCurrentPlayer().Data.Level, 2, equipmentGameData.CorrespondingSetItem.NameId, 1), new List<IInventoryItemGameData>(), LootDisplayType.None);
			}
		}
	}
}

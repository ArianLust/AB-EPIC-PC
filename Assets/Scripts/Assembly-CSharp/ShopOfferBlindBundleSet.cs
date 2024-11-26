using System.Linq;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using UnityEngine;

public class ShopOfferBlindBundleSet : ShopOfferBlindBundleBase
{
	[Header("Background")]
	[SerializeField]
	private UILabel m_subItemsLabelA;

	[SerializeField]
	private UILabel m_subItemsLabelB;

	[SerializeField]
	private UILabel m_setPerkDescription;

	[SerializeField]
	private BundleSaleSetInfo m_setItemInfoA;

	[SerializeField]
	private BundleSaleSetInfo m_setItemInfoB;

	public override void SetModel(BasicShopOfferBalancingData model, ShopWindowStateMgr stateMgr)
	{
		base.SetModel(model, stateMgr);
		EquipmentGameData equipment = m_items.FirstOrDefault((IInventoryItemGameData i) => i.ItemBalancing.ItemType == InventoryItemType.MainHandEquipment) as EquipmentGameData;
		EquipmentGameData equipment2 = m_items.FirstOrDefault((IInventoryItemGameData i) => i.ItemBalancing.ItemType == InventoryItemType.OffHandEquipment) as EquipmentGameData;
		HandleSetItemInfo(m_setItemInfoA, equipment);
		HandleSetItemInfo(m_setItemInfoB, equipment2);
		if (m_items.Count > 3)
		{
			m_subItemsLabelB.text = m_items[3].ItemLocalizedName;
		}
		if (m_items.Count > 2)
		{
			m_subItemsLabelA.text = m_items[2].ItemLocalizedName;
		}
	}

	private void HandleSetItemInfo(BundleSaleSetInfo setItemInfo, EquipmentGameData equipment)
	{
		setItemInfo.m_NameLabel.text = equipment.ItemLocalizedName;
		setItemInfo.m_MainStatsLabel.text = equipment.ItemMainStat.ToString();
		SkillBattleDataBase skillBattleDataBase = equipment.SetItemSkill.GenerateSkillBattleData();
		m_setPerkDescription.text = skillBattleDataBase.GetLocalizedDescription(new BirdCombatant(new BirdGameData(equipment.BalancingData.RestrictedBirdId)));
	}
}

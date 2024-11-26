using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.Shared.Generic;
using UnityEngine;

public class SetBannerInfo : MonoBehaviour
{
	[SerializeField]
	private LootDisplayContoller m_TipLootDisplay;

	[SerializeField]
	private LootDisplayContoller m_FlagLootDisplay;

	[SerializeField]
	private UILabel m_SetBonusTextLabel;

	[SerializeField]
	private UISprite m_SetPerkIcon;

	private BannerItemGameData m_Item;

	public void SetModel(BannerItemGameData bannerItemGameData, BannerGameData selectedBanner)
	{
		base.gameObject.SetActive(selectedBanner != null && bannerItemGameData.IsSetItem);
		if (!bannerItemGameData.IsSetItem)
		{
			return;
		}
		m_SetPerkIcon.spriteName = BannerItemGameData.GetPerkIconNameByPerk(bannerItemGameData.SetItemSkill.GetPerkType());
		m_Item = bannerItemGameData;
		if ((bool)m_SetBonusTextLabel)
		{
			SkillBattleDataBase skillBattleDataBase = m_Item.SetItemSkill.GenerateSkillBattleData();
			m_SetBonusTextLabel.text = skillBattleDataBase.GetLocalizedDescription(new BannerCombatant(selectedBanner));
		}
		if (m_Item.BalancingData.ItemType == InventoryItemType.BannerTip)
		{
			m_TipLootDisplay.gameObject.SetActive(true);
			m_TipLootDisplay.SetModel(bannerItemGameData, new List<IInventoryItemGameData>(), LootDisplayType.None);
			if (selectedBanner.BannerCenter.BalancingData.NameId == bannerItemGameData.CorrespondingSetItem.NameId)
			{
				m_FlagLootDisplay.gameObject.SetActive(true);
				m_FlagLootDisplay.SetModel(selectedBanner.BannerCenter, new List<IInventoryItemGameData>(), LootDisplayType.None);
			}
			else
			{
				m_FlagLootDisplay.gameObject.SetActive(true);
				m_FlagLootDisplay.SetModel(DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(DIContainerInfrastructure.GetCurrentPlayer().Data.Level, 2, bannerItemGameData.CorrespondingSetItem.NameId, 1), new List<IInventoryItemGameData>(), LootDisplayType.None);
			}
			SetLayer();
		}
		else
		{
			m_FlagLootDisplay.gameObject.SetActive(true);
			m_FlagLootDisplay.SetModel(bannerItemGameData, new List<IInventoryItemGameData>(), LootDisplayType.None);
			if (selectedBanner.BannerTip.BalancingData.NameId == bannerItemGameData.CorrespondingSetItem.NameId)
			{
				m_TipLootDisplay.gameObject.SetActive(true);
				m_TipLootDisplay.SetModel(selectedBanner.BannerTip, new List<IInventoryItemGameData>(), LootDisplayType.None);
			}
			else
			{
				m_TipLootDisplay.gameObject.SetActive(true);
				m_TipLootDisplay.SetModel(DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(DIContainerInfrastructure.GetCurrentPlayer().Data.Level, 2, bannerItemGameData.CorrespondingSetItem.NameId, 1), new List<IInventoryItemGameData>(), LootDisplayType.None);
			}
			SetLayer();
		}
	}

	private void SetLayer()
	{
		UnityHelper.SetLayerRecusively(m_FlagLootDisplay.gameObject, LayerMask.NameToLayer("Interface"));
		UnityHelper.SetLayerRecusively(m_TipLootDisplay.gameObject, LayerMask.NameToLayer("Interface"));
	}
}

using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

public class EquipmentStatsSpeechBubble : MonoBehaviour
{
	public StatisticsElement m_StatisticsComparison;

	public Animator m_ComparisionAnimation;

	public UISprite m_EquipmentSprite;

	private InventoryItemType m_CachedType;

	public void SetComparisionValues(string statsIconId, InventoryItemType type, float newValue, PerkType perk)
	{
		m_StatisticsComparison.SetIconSprite(statsIconId);
		m_StatisticsComparison.RefreshStat(false, false, newValue, 0f);
		m_EquipmentSprite.gameObject.SetActive(type != InventoryItemType.BannerEmblem);
		m_EquipmentSprite.spriteName = EquipmentGameData.GetPerkIconNameByPerk(perk);
		m_CachedType = type;
	}

	public float Show()
	{
		switch (m_CachedType)
		{
		case InventoryItemType.MainHandEquipment:
			m_ComparisionAnimation.Play("Bubble_EquippedItem_MainHand_Enter");
			break;
		case InventoryItemType.OffHandEquipment:
			m_ComparisionAnimation.Play("Bubble_EquippedItem_OffHand_Enter");
			break;
		}
		return 0f;
	}

	public float Hide()
	{
		switch (m_CachedType)
		{
		case InventoryItemType.MainHandEquipment:
			m_ComparisionAnimation.Play("Bubble_EquippedItem_MainHand_Leave");
			break;
		case InventoryItemType.OffHandEquipment:
			m_ComparisionAnimation.Play("Bubble_EquippedItem_OffHand_Leave");
			break;
		}
		return 0f;
	}
}

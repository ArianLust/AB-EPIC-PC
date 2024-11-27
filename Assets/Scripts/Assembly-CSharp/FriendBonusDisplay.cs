using System.Collections.Generic;
using ABH.GameDatas;
using UnityEngine;

public class FriendBonusDisplay : MonoBehaviour
{
	public UISprite m_Icon;

	public UILabel m_Text;

	public UISprite m_Body;

	public LootDisplayContoller m_Loot;

	public Animation m_AccomplishedAnimation;

	public string ActiveState = "RewardSlot_Active";

	public string InactiveState = "RewardSlot_Inactive";

	private UIScrollView m_dragPanel;

	public void SetModel(string nameId, string text, UIScrollView dragPanel = null)
	{
		m_dragPanel = dragPanel;
		if ((bool)m_Icon)
		{
			m_Icon.spriteName = nameId;
		}
		if ((bool)m_Loot)
		{
			m_Loot.SetModel(DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(1, 0, nameId, 1), new List<IInventoryItemGameData>(), LootDisplayType.None);
			UnityHelper.SetLayerRecusively(m_Loot.gameObject, LayerMask.NameToLayer("SkillPreview"));
		}
		if ((bool)m_Text)
		{
			m_Text.text = text;
		}
	}

	public void SetActive(bool active)
	{
		if (active)
		{
			m_Body.spriteName = ActiveState;
		}
		else
		{
			m_Body.spriteName = InactiveState;
		}
	}

	public void PlayAccomplishedAnimation()
	{
		m_AccomplishedAnimation.Play();
	}
}

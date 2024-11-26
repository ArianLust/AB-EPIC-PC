using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Services.Logic;
using UnityEngine;

public class FriendCountBonusInfoUI : MonoBehaviour
{
	[SerializeField]
	private UILabel m_HealthBonusLabel;

	[SerializeField]
	private UILabel m_AttackBonusLabel;

	[SerializeField]
	private UILabel m_XPBonusLabel;

	[SerializeField]
	private List<Transform> m_ClassItemsRoot = new List<Transform>();

	[SerializeField]
	private LootDisplayContoller m_ClassItemsLootPrefab;

	private List<LootDisplayContoller> m_ClassItems = new List<LootDisplayContoller>();

	private FriendCountBonusInfo m_Model;

	public void SetModel(FriendCountBonusInfo bonusInfo)
	{
		m_Model = bonusInfo;
		FriendCountBonusInfo maxFriendBonuses = DIContainerLogic.SocialService.GetMaxFriendBonuses();
		for (int i = 0; i < m_Model.Classes.Count; i++)
		{
			LootDisplayContoller lootDisplayContoller = Object.Instantiate(m_ClassItemsLootPrefab);
			lootDisplayContoller.transform.parent = m_ClassItemsRoot[i];
			lootDisplayContoller.transform.localPosition = Vector3.zero;
			lootDisplayContoller.SetModel(DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(1, 1, m_Model.Classes[i], 1), new List<IInventoryItemGameData>(), LootDisplayType.None);
			lootDisplayContoller.transform.localScale = Vector3.one;
		}
		m_HealthBonusLabel.text = bonusInfo.HealthBonus.ToString("0") + "/" + maxFriendBonuses.HealthBonus.ToString("0");
		m_AttackBonusLabel.text = bonusInfo.AttackBonus.ToString("0") + "/" + maxFriendBonuses.AttackBonus.ToString("0");
		m_XPBonusLabel.text = bonusInfo.XPBonus.ToString("0") + "/" + maxFriendBonuses.XPBonus.ToString("0") + "%";
	}
}

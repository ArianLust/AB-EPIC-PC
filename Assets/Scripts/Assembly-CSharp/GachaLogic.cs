using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

public class GachaLogic
{
	private bool m_arenaGacha;

	private IInventoryItemGameData m_item;

	public static List<string> m_itemsGotThisSession = new List<string>();

	public GachaLogic(bool arenaGacha)
	{
		m_arenaGacha = arenaGacha;
	}

	public IInventoryItemGameData CheckForDuplicateSetItems(IInventoryItemGameData item)
	{
		m_item = item;
		bool flag = false;
		EquipmentGameData equipmentGameData = m_item as EquipmentGameData;
		BannerItemGameData bannerItemGameData = m_item as BannerItemGameData;
		if (equipmentGameData != null)
		{
			flag = equipmentGameData.IsSetItem;
		}
		else if (bannerItemGameData != null)
		{
			flag = bannerItemGameData.IsSetItem;
		}
		if (!flag)
		{
			return m_item;
		}
		if (!m_itemsGotThisSession.Contains(item.Name))
		{
			m_itemsGotThisSession.Add(m_item.Name);
			return m_item;
		}
		if (PlayerHasAllSets())
		{
			m_itemsGotThisSession.Add(m_item.Name);
			return m_item;
		}
		if (!RemoveDoubledSetItem())
		{
			m_itemsGotThisSession.Add(m_item.Name);
			return m_item;
		}
		int num = 0;
		while (num < 50 && AlreadyOwned(m_item))
		{
			num++;
			m_item = NewSetItem();
		}
		m_item = DIContainerLogic.InventoryService.AddItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_item.ItemData.Level, 4, m_item.ItemBalancing.NameId, 1, "new gacha item");
		m_itemsGotThisSession.Add(m_item.Name);
		return m_item;
	}

	private bool RemoveDoubledSetItem()
	{
		int num = 0;
		List<IInventoryItemGameData> list = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[m_item.ItemBalancing.ItemType];
		foreach (IInventoryItemGameData item in list)
		{
			if (item.ItemBalancing.NameId == m_item.ItemBalancing.NameId && item.ItemData.Level == m_item.ItemData.Level)
			{
				num++;
				if (num == 2)
				{
					DIContainerLogic.InventoryService.RemoveItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_item, 1, "duplicated gacha item");
					return true;
				}
			}
		}
		return false;
	}

	private IInventoryItemGameData NewSetItem()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		string key = string.Empty;
		if (!m_arenaGacha)
		{
			key = ((currentPlayer.GetBird("bird_yellow") == null) ? "loot_gacha_set_red_bird" : ((currentPlayer.GetBird("bird_white") == null) ? "loot_gacha_set_yellow_bird" : ((currentPlayer.GetBird("bird_black") == null) ? "loot_gacha_set_white_bird" : ((currentPlayer.GetBird("bird_blue") != null) ? "loot_gacha_set_blue_bird" : "loot_gacha_set_black_bird"))));
		}
		else
		{
			IInventoryItemGameData data = null;
			if (DIContainerLogic.InventoryService.TryGetItemGameData(currentPlayer.InventoryGameData, "pvp_league_crown", out data))
			{
				int level = data.ItemData.Level;
				key = "loot_pvpgacha_set_content_l" + level;
			}
		}
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		dictionary.Add(key, 1);
		Dictionary<string, LootInfoData> loot = DIContainerLogic.GetLootOperationService().GenerateLoot(dictionary, currentPlayer.Data.Level + 2);
		List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(loot);
		return itemsFromLoot.FirstOrDefault();
	}

	private bool AlreadyOwned(IInventoryItemGameData m_item)
	{
		List<IInventoryItemGameData> list = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[m_item.ItemBalancing.ItemType];
		foreach (IInventoryItemGameData item in list)
		{
			if (item.ItemBalancing.NameId == m_item.ItemBalancing.NameId && item.ItemData.Level == m_item.ItemData.Level)
			{
				return true;
			}
		}
		return false;
	}

	private bool PlayerHasAllSets()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (!m_arenaGacha)
		{
			int num = (from e in DIContainerBalancing.Service.GetBalancingDataList<EquipmentBalancingData>()
				where e.ItemType == InventoryItemType.MainHandEquipment && !string.IsNullOrEmpty(e.SetItemSkill)
				select e).Count();
			if (CountUniqueSetsOfPlayer(InventoryItemType.MainHandEquipment) < num)
			{
				return false;
			}
			int num2 = (from e in DIContainerBalancing.Service.GetBalancingDataList<EquipmentBalancingData>()
				where e.ItemType == InventoryItemType.OffHandEquipment && !string.IsNullOrEmpty(e.SetItemSkill)
				select e).Count();
			if (CountUniqueSetsOfPlayer(InventoryItemType.OffHandEquipment) > num2)
			{
				return true;
			}
		}
		else
		{
			int num3 = (from e in DIContainerBalancing.Service.GetBalancingDataList<BannerItemBalancingData>()
				where e.ItemType == InventoryItemType.BannerTip && !string.IsNullOrEmpty(e.CorrespondingSetItem)
				select e).Count();
			if (CountUniqueSetsOfPlayer(InventoryItemType.BannerTip) < num3)
			{
				return false;
			}
			int num4 = (from e in DIContainerBalancing.Service.GetBalancingDataList<BannerItemBalancingData>()
				where e.ItemType == InventoryItemType.Banner && !string.IsNullOrEmpty(e.CorrespondingSetItem)
				select e).Count();
			if (CountUniqueSetsOfPlayer(InventoryItemType.Banner) >= num4)
			{
				return true;
			}
		}
		return false;
	}

	private int CountUniqueSetsOfPlayer(InventoryItemType ItemType)
	{
		PlayerGameData player = DIContainerInfrastructure.GetCurrentPlayer();
		List<IInventoryItemGameData> list = player.InventoryGameData.Items[ItemType].Where((IInventoryItemGameData i) => i.ItemData.Level == player.Data.Level).ToList();
		List<string> list2 = new List<string>();
		if (ItemType == InventoryItemType.BannerTip || ItemType == InventoryItemType.Banner)
		{
			foreach (IInventoryItemGameData item in list)
			{
				if (((BannerItemGameData)item).IsSetItem)
				{
					list2.Add(item.Name);
				}
			}
		}
		else
		{
			foreach (IInventoryItemGameData item2 in list)
			{
				if (((EquipmentGameData)item2).IsSetItem)
				{
					list2.Add(item2.Name);
				}
			}
		}
		return list2.Count;
	}
}

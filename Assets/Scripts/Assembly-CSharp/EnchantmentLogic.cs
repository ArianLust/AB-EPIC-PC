using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;

public class EnchantmentLogic
{
	public EnchantingBalancingData GetBalancing(int enchantmentLevel)
	{
		return GetEnchantingBalancing(enchantmentLevel, 0);
	}

	public EnchantingBalancingData GetBalancing(EquipmentGameData equipment, int enchantmentLevel = -1)
	{
		if (enchantmentLevel == -1)
		{
			enchantmentLevel = equipment.EnchantementLevel;
		}
		int levelRange = equipment.Data.Level / 10;
		return GetEnchantingBalancing(enchantmentLevel, levelRange);
	}

	public EnchantingBalancingData GetBalancing(BannerItemGameData bannerItem, int enchantmentLevel = -1)
	{
		if (enchantmentLevel == -1)
		{
			enchantmentLevel = bannerItem.EnchantementLevel;
		}
		int levelRange = bannerItem.Data.Level / 10;
		return GetEnchantingBalancing(enchantmentLevel, levelRange);
	}

	private EnchantingBalancingData GetEnchantingBalancing(int enchantmentLevel, int levelRange)
	{
		return (from b in DIContainerBalancing.Service.GetBalancingDataList<EnchantingBalancingData>()
			where b.EnchantmentLevel == enchantmentLevel && b.Levelrange == levelRange
			select b).FirstOrDefault();
	}

	public int GetMaxEnchantmentLevel(EquipmentGameData equipment)
	{
		List<EnchantingBalancingData> enchList = (from b in DIContainerBalancing.Service.GetBalancingDataList<EnchantingBalancingData>()
			where b.Levelrange == equipment.Data.Level / 10
			select b).ToList();
		return CountBalancingEntriesForItem(enchList, equipment.IsSetItem, equipment.Data.Quality);
	}

	public int GetMaxEnchantmentLevel(BannerItemGameData bannerItem)
	{
		List<EnchantingBalancingData> enchList = (from b in DIContainerBalancing.Service.GetBalancingDataList<EnchantingBalancingData>()
			where b.Levelrange == bannerItem.Data.Level / 10
			select b).ToList();
		return CountBalancingEntriesForItem(enchList, bannerItem.IsSetItem, bannerItem.GetStars());
	}

	private int CountBalancingEntriesForItem(List<EnchantingBalancingData> enchList, bool isSetItem, int quality)
	{
		if (isSetItem)
		{
			return enchList.Where((EnchantingBalancingData b) => b.SetAllowed).Count();
		}
		switch (quality)
		{
		case 0:
			return enchList.Where((EnchantingBalancingData b) => b.Stars0Allowed).Count();
		case 1:
			return enchList.Where((EnchantingBalancingData b) => b.Stars1Allowed).Count();
		case 2:
			return enchList.Where((EnchantingBalancingData b) => b.Stars2Allowed).Count();
		case 3:
			return enchList.Where((EnchantingBalancingData b) => b.Stars3Allowed).Count();
		default:
			return 0;
		}
	}
}

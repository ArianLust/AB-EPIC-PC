using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.Shared.BalancingData;
using ABH.Shared.Interfaces;
using ABH.Shared.Models.InventoryItems;

namespace ABH.GameDatas
{
	public class CraftingRecipeGameData : GameDataBase<CraftingRecipeBalancingData, CraftingRecipeData>, IInventoryItemGameData
	{
		private Dictionary<string, int> m_recalculatedResultLoot;

		public string Name
		{
			get
			{
				return BalancingData.LocaBaseId;
			}
		}

		public IInventoryItemBalancingData ItemBalancing
		{
			get
			{
				return BalancingData;
			}
		}

		public IInventoryItemData ItemData
		{
			get
			{
				return Data;
			}
		}

		public int ItemValue
		{
			get
			{
				return Data.Value;
			}
			set
			{
				Data.Value = value;
			}
		}

		public float ItemMainStat
		{
			get
			{
				return 0f;
			}
		}

		public SkillGameData PrimarySkill
		{
			get
			{
				return null;
			}
		}

		public SkillGameData SecondarySkill
		{
			get
			{
				return null;
			}
		}

		public string ItemAssetName
		{
			get
			{
				return BalancingData.AssetBaseId;
			}
		}

		public string ItemLocalizedName
		{
			get
			{
				return DIContainerInfrastructure.GetLocaService().GetRecipeName(BalancingData.LocaBaseId);
			}
		}

		public string ItemLocalizedDesc
		{
			get
			{
				return DIContainerInfrastructure.GetLocaService().GetRecipeDesc(BalancingData.LocaBaseId);
			}
		}

		public string ItemIconAtlasName
		{
			get
			{
				return string.Empty;
			}
		}

		[method: MethodImpl(32)]
		public event Action<IInventoryItemGameData, float> ItemDataChanged;

		public CraftingRecipeGameData(string nameId)
			: base(nameId)
		{
		}

		public CraftingRecipeGameData(CraftingRecipeData instance)
			: base(instance)
		{
		}

		protected override CraftingRecipeData CreateNewInstance(string nameId)
		{
			return new CraftingRecipeData();
		}

		public void RaiseItemDataChanged(float delta)
		{
			if (this.ItemDataChanged != null)
			{
				this.ItemDataChanged(this, delta);
			}
		}

		public bool IsValidForBird(BirdGameData bird)
		{
			return false;
		}

		public string ItemLocalizedTooltipDesc(InventoryGameData inventory)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", DIContainerLogic.InventoryService.GetItemValue(inventory, BalancingData.NameId).ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetItemTooltipDesc(BalancingData.LocaBaseId, dictionary);
		}

		public void ResetValue()
		{
			Data.Value = 1;
		}

		public void IncreaseByValue(int amount)
		{
			m_recalculatedResultLoot = new Dictionary<string, int>();
			foreach (string key in BalancingData.ResultLoot.Keys)
			{
				m_recalculatedResultLoot.Add(key, BalancingData.ResultLoot[key] * amount);
			}
		}

		public Dictionary<string, int> GetResultLoot()
		{
			if (m_recalculatedResultLoot == null || m_recalculatedResultLoot.Count == 0)
			{
				m_recalculatedResultLoot = BalancingData.ResultLoot;
			}
			return m_recalculatedResultLoot;
		}

		public int MaxRecipeRank()
		{
			IEnumerable<BuyableShopOfferBalancingData> enumerable = from o in DIContainerBalancing.Service.GetBalancingDataList<BuyableShopOfferBalancingData>()
				where o.OfferContents != null && o.OfferContents.ContainsKey(BalancingData.ResultLoot.Keys.FirstOrDefault())
				select o;
			if (enumerable != null && enumerable.Count() > 0)
			{
				return enumerable.OrderBy((BuyableShopOfferBalancingData o) => o.Level).LastOrDefault().Level;
			}
			return 1;
		}
	}
}

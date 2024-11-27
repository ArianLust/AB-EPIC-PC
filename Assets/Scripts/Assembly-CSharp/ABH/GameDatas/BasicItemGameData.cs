using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Interfaces;
using ABH.Shared.Models.InventoryItems;

namespace ABH.GameDatas
{
	public class BasicItemGameData : GameDataBase<BasicItemBalancingData, BasicItemData>, IInventoryItemGameData
	{
		private List<int> m_StoreLinks = new List<int>();

		public string Name
		{
			get
			{
				return BalancingData.LocaBaseId;
			}
		}

		public int ItemValue
		{
			get
			{
				if (BalancingData.SetAsNewInShop == 0 && Data.IsNewInShop == 0)
				{
					return Data.Value;
				}
				if (BalancingData.SetAsNewInShop > Data.IsNewInShop)
				{
					SetAsNewInShopAndPersist(Data.Value);
					return GetValueAsNewInShop();
				}
				return GetValueAsNewInShop();
			}
			set
			{
				if (BalancingData.SetAsNewInShop != Data.IsNewInShop)
				{
					SetAsNewInShopAndPersist(value);
				}
				else
				{
					Data.Value = value;
				}
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
				return DIContainerInfrastructure.GetLocaService().GetItemName(BalancingData.LocaBaseId);
			}
		}

		public string ItemLocalizedDesc
		{
			get
			{
				return DIContainerInfrastructure.GetLocaService().GetItemDesc(BalancingData.LocaBaseId);
			}
		}

		public string ItemIconAtlasName
		{
			get
			{
				return "GenericElements";
			}
		}

		[method: MethodImpl(32)]
		public event Action<IInventoryItemGameData, float> ItemDataChanged;

		public BasicItemGameData(string nameId)
			: base(nameId)
		{
			FillStoreLinks();
		}

		public BasicItemGameData(BasicItemData instance)
			: base(instance)
		{
			FillStoreLinks();
		}

		private void FillStoreLinks()
		{
			if (BalancingData.SetAsNewInShop > 0 || Data.IsNewInShop > 0)
			{
				for (int i = 0; i < 10; i++)
				{
					m_StoreLinks.Add(195225786 * (i + 1));
				}
			}
		}

		protected override BasicItemData CreateNewInstance(string nameId)
		{
			BasicItemData basicItemData = new BasicItemData();
			basicItemData.NameId = nameId;
			basicItemData.Level = 0;
			basicItemData.Quality = 0;
			basicItemData.Value = 0;
			return basicItemData;
		}

		public void ResetValue()
		{
			Data.IsNewInShop = 0;
			if (Data.NameId == "gold")
			{
				Data.Value = 1000;
			}
			else if (Data.NameId == "lucky_coin")
			{
				Data.Value = 80;
			}
			else if (Data.NameId == "friendship_essence")
			{
				Data.Value = 25;
			}
			else
			{
				Data.Value = 1;
			}
		}

		private int GetValueAsNewInShop()
		{
			return ShopTo(Data.IsNewInShop, Data.Value);
		}

		private void SetAsNewInShopAndPersist(int value)
		{
			Data.Value = ShopFromTo(Data.IsNewInShop, BalancingData.SetAsNewInShop, value);
			Data.IsNewInShop = BalancingData.SetAsNewInShop;
		}

		private int ShopFromTo(int from, int to, int value)
		{
			int num = 0;
			for (int i = from; i < m_StoreLinks.Count && i < to; i++)
			{
				num += m_StoreLinks[i];
			}
			return value ^ num;
		}

		private int ShopTo(int to, int value)
		{
			if (to <= 0)
			{
				return value;
			}
			int num = 0;
			for (int i = 0; i < m_StoreLinks.Count && i < to; i++)
			{
				num += m_StoreLinks[i];
			}
			return value ^ num;
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
			if (BalancingData.NameId == "special_offer_rainbow_riot")
			{
				dictionary.Add("{value_1}", DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").RainbowRiot1Multi.ToString());
			}
			else if (BalancingData.NameId == "special_offer_rainbow_riot_02")
			{
				dictionary.Add("{value_1}", DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").RainbowRiot2Multi.ToString());
			}
			else if (BalancingData.ItemType == InventoryItemType.CollectionComponent)
			{
				dictionary = DIContainerInfrastructure.EventSystemStateManager.GetCollectionRewardReplacementDict();
			}
			else
			{
				dictionary.Add("{value_1}", DIContainerLogic.InventoryService.GetItemValue(inventory, BalancingData.NameId).ToString("0"));
			}
			return DIContainerInfrastructure.GetLocaService().GetItemTooltipDesc(BalancingData.LocaBaseId, dictionary);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Interfaces;
using ABH.Shared.Models;
using ABH.Shared.Models.InventoryItems;

namespace ABH.GameDatas
{
	public class InventoryGameData : GameDataBase<InventoryBalancingData, InventoryData>
	{
		public Dictionary<InventoryItemType, List<IInventoryItemGameData>> Items;

		public Dictionary<InventoryItemType, Dictionary<string, List<IInventoryItemGameData>>> CraftingRecipes;

		[method: MethodImpl(32)]
		public event Action<InventoryItemType, IInventoryItemGameData> InventoryOfTypeChanged;

		[method: MethodImpl(32)]
		public event Action<IInventoryItemGameData> StoryItemGained;

		public InventoryGameData(string nameId)
			: base(nameId)
		{
		}

		public InventoryGameData(InventoryData instance)
			: base(instance)
		{
			Items = new Dictionary<InventoryItemType, List<IInventoryItemGameData>>();
			CraftingRecipes = new Dictionary<InventoryItemType, Dictionary<string, List<IInventoryItemGameData>>>();
			foreach (int value3 in Enum.GetValues(typeof(InventoryItemType)))
			{
				if (value3 == 12)
				{
					List<IInventoryItemGameData> value = new List<IInventoryItemGameData>();
					Items.Add((InventoryItemType)value3, value);
					CraftingRecipes.Add(InventoryItemType.Resources, new Dictionary<string, List<IInventoryItemGameData>>());
					CraftingRecipes.Add(InventoryItemType.Ingredients, new Dictionary<string, List<IInventoryItemGameData>>());
					CraftingRecipes.Add(InventoryItemType.MainHandEquipment, new Dictionary<string, List<IInventoryItemGameData>>());
					CraftingRecipes.Add(InventoryItemType.OffHandEquipment, new Dictionary<string, List<IInventoryItemGameData>>());
					CraftingRecipes.Add(InventoryItemType.Consumable, new Dictionary<string, List<IInventoryItemGameData>>());
				}
				else
				{
					List<IInventoryItemGameData> value2 = new List<IInventoryItemGameData>();
					Items.Add((InventoryItemType)value3, value2);
				}
				GenerateGameDatasFromItemDatas(instance, (InventoryItemType)value3);
			}
			DIContainerLogic.InventoryService.RewardDelayedRewards(this);
		}

		public void RaiseInventoryChanged(InventoryItemType itype, IInventoryItemGameData item)
		{
			if (this.InventoryOfTypeChanged != null)
			{
				this.InventoryOfTypeChanged(itype, item);
			}
		}

		public void RaiseStoryItemGained(IInventoryItemGameData item)
		{
			if (this.StoryItemGained != null)
			{
				this.StoryItemGained(item);
			}
		}

		private List<SkinItemGameData> GetClassSkin(string className, int maxprio)
		{
			List<SkinItemGameData> list = new List<SkinItemGameData>();
			foreach (ClassSkinBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<ClassSkinBalancingData>())
			{
				if (balancingData.SortPriority <= maxprio && balancingData.OriginalClass == className)
				{
					list.Add(DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(1, 1, balancingData.NameId, 1) as SkinItemGameData);
				}
			}
			return list;
		}

		public void AddNewItemToInventory(IInventoryItemGameData itemGameData, bool birdInventory = false)
		{
			if (itemGameData == null || (itemGameData is SkinItemGameData && ItemsContainsSkin(itemGameData as SkinItemGameData)))
			{
				return;
			}
			Items[itemGameData.ItemBalancing.ItemType].Add(itemGameData);
			AddItemDataToCategory(itemGameData.ItemBalancing.ItemType, Data, itemGameData.ItemData);
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			if (itemGameData is ClassItemGameData && !birdInventory)
			{
				SkinItemGameData skinItemGameData = GetClassSkin(itemGameData.ItemBalancing.NameId, 0).FirstOrDefault();
				if (skinItemGameData != null)
				{
					skinItemGameData.Data.IsNew = false;
					if (currentPlayer != null && currentPlayer.Data.EquippedSkins != null && !currentPlayer.Data.EquippedSkins.ContainsKey(itemGameData.ItemBalancing.NameId))
					{
						currentPlayer.Data.EquippedSkins.SaveAdd(itemGameData.ItemBalancing.NameId, skinItemGameData.BalancingData.NameId);
					}
					AddNewItemToInventory(skinItemGameData);
				}
				string replacementClassNameId = (itemGameData as ClassItemGameData).BalancingData.ReplacementClassNameId;
				if (!string.IsNullOrEmpty(replacementClassNameId))
				{
					List<IInventoryItemGameData> list = new List<IInventoryItemGameData>(currentPlayer.InventoryGameData.Items[InventoryItemType.Skin]);
					foreach (SkinItemGameData item in list)
					{
						if (!(item.BalancingData.OriginalClass == replacementClassNameId) || item.BalancingData.SortPriority == 0)
						{
							continue;
						}
						foreach (SkinItemGameData item2 in GetClassSkin(itemGameData.ItemBalancing.NameId, 1000))
						{
							currentPlayer.Data.EquippedSkins.SaveAdd(itemGameData.ItemBalancing.NameId, item2.BalancingData.NameId);
							AddNewItemToInventory(item2);
						}
					}
				}
			}
			if (!(itemGameData is CraftingRecipeGameData))
			{
				return;
			}
			CraftingRecipeGameData craftingRecipeGameData = (CraftingRecipeGameData)itemGameData;
			foreach (string key in craftingRecipeGameData.GetResultLoot().Keys)
			{
				List<IInventoryItemGameData> value = null;
				if (CraftingRecipes[craftingRecipeGameData.BalancingData.RecipeCategoryType].TryGetValue(key, out value))
				{
					value.Add(craftingRecipeGameData);
					continue;
				}
				CraftingRecipes[craftingRecipeGameData.BalancingData.RecipeCategoryType].Add(key, new List<IInventoryItemGameData> { craftingRecipeGameData });
			}
		}

		private bool ItemsContainsSkin(SkinItemGameData basicSkin)
		{
			List<IInventoryItemGameData> list = Items[InventoryItemType.Skin];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].ItemBalancing.NameId == basicSkin.BalancingData.NameId)
				{
					return true;
				}
			}
			return false;
		}

		public bool RemoveItemFromInventory(IInventoryItemGameData itemGameData)
		{
			if (itemGameData.ItemBalancing.ItemType == InventoryItemType.CraftingRecipes)
			{
				CraftingRecipeGameData craftingRecipeGameData = (CraftingRecipeGameData)itemGameData;
				bool flag = true;
				foreach (string key in craftingRecipeGameData.GetResultLoot().Keys)
				{
					flag &= CraftingRecipes[craftingRecipeGameData.BalancingData.RecipeCategoryType].Remove(key);
				}
				return (flag & Items[itemGameData.ItemBalancing.ItemType].Remove(itemGameData)) && RemoveItemDataFromCategory(itemGameData.ItemBalancing.ItemType, Data, itemGameData.ItemData);
			}
			return Items[itemGameData.ItemBalancing.ItemType].Remove(itemGameData) && RemoveItemDataFromCategory(itemGameData.ItemBalancing.ItemType, Data, itemGameData.ItemData);
		}

		private void GenerateGameDatasFromItemDatas(InventoryData instance, InventoryItemType itemcategory)
		{
			switch (itemcategory)
			{
			case InventoryItemType.Class:
				if (instance.ClassItems == null)
				{
					instance.ClassItems = new List<ClassItemData>();
				}
				{
					foreach (ClassItemData classItem in instance.ClassItems)
					{
						Items[itemcategory].Add(DIContainerLogic.InventoryService.ReinitNewInventoryItemGameData(this, classItem));
					}
					break;
				}
			case InventoryItemType.Consumable:
				if (instance.ConsumableItems == null)
				{
					instance.ConsumableItems = new List<ConsumableItemData>();
				}
				{
					foreach (ConsumableItemData consumableItem in instance.ConsumableItems)
					{
						Items[itemcategory].Add(DIContainerLogic.InventoryService.ReinitNewInventoryItemGameData(this, consumableItem));
					}
					break;
				}
			case InventoryItemType.Ingredients:
				if (instance.CraftingIngredientItems == null)
				{
					instance.CraftingIngredientItems = new List<CraftingItemData>();
				}
				{
					foreach (CraftingItemData craftingIngredientItem in instance.CraftingIngredientItems)
					{
						Items[itemcategory].Add(DIContainerLogic.InventoryService.ReinitNewInventoryItemGameData(this, craftingIngredientItem));
					}
					break;
				}
			case InventoryItemType.MainHandEquipment:
				if (instance.MainHandItems == null)
				{
					instance.MainHandItems = new List<EquipmentData>();
				}
				{
					foreach (EquipmentData mainHandItem in instance.MainHandItems)
					{
						Items[itemcategory].Add(DIContainerLogic.InventoryService.ReinitNewInventoryItemGameData(this, mainHandItem));
					}
					break;
				}
			case InventoryItemType.OffHandEquipment:
				if (instance.OffHandItems == null)
				{
					instance.OffHandItems = new List<EquipmentData>();
				}
				{
					foreach (EquipmentData offHandItem in instance.OffHandItems)
					{
						Items[itemcategory].Add(DIContainerLogic.InventoryService.ReinitNewInventoryItemGameData(this, offHandItem));
					}
					break;
				}
			case InventoryItemType.PlayerStats:
				if (instance.PlayerStats == null)
				{
					instance.PlayerStats = new List<BasicItemData>();
				}
				{
					foreach (BasicItemData playerStat in instance.PlayerStats)
					{
						Items[itemcategory].Add(DIContainerLogic.InventoryService.ReinitNewInventoryItemGameData(this, playerStat));
					}
					break;
				}
			case InventoryItemType.CollectionComponent:
				if (instance.CollectionComponents == null)
				{
					instance.CollectionComponents = new List<BasicItemData>();
				}
				{
					foreach (BasicItemData collectionComponent in instance.CollectionComponents)
					{
						Items[itemcategory].Add(DIContainerLogic.InventoryService.ReinitNewInventoryItemGameData(this, collectionComponent));
					}
					break;
				}
			case InventoryItemType.CraftingRecipes:
				if (instance.CraftingRecipesItems == null)
				{
					instance.CraftingRecipesItems = new List<CraftingRecipeData>();
				}
				{
					foreach (CraftingRecipeData craftingRecipesItem in instance.CraftingRecipesItems)
					{
						CraftingRecipeGameData craftingRecipeGameData = DIContainerLogic.InventoryService.ReinitNewInventoryItemGameData(this, craftingRecipesItem) as CraftingRecipeGameData;
						if (craftingRecipeGameData == null)
						{
							continue;
						}
						Items[itemcategory].Add(craftingRecipeGameData);
						foreach (string key in craftingRecipeGameData.GetResultLoot().Keys)
						{
							List<IInventoryItemGameData> value = null;
							if (CraftingRecipes[craftingRecipeGameData.BalancingData.RecipeCategoryType].TryGetValue(key, out value))
							{
								value.Add(craftingRecipeGameData);
								continue;
							}
							CraftingRecipes[craftingRecipeGameData.BalancingData.RecipeCategoryType].Add(key, new List<IInventoryItemGameData> { craftingRecipeGameData });
						}
					}
					break;
				}
			case InventoryItemType.PlayerToken:
				break;
			case InventoryItemType.Points:
				break;
			case InventoryItemType.Premium:
				break;
			case InventoryItemType.Resources:
				if (instance.CraftingResourceItems == null)
				{
					instance.CraftingResourceItems = new List<CraftingItemData>();
				}
				{
					foreach (CraftingItemData craftingResourceItem in instance.CraftingResourceItems)
					{
						Items[itemcategory].Add(DIContainerLogic.InventoryService.ReinitNewInventoryItemGameData(this, craftingResourceItem));
					}
					break;
				}
			case InventoryItemType.Trophy:
				if (instance.TrophyItems == null)
				{
					instance.TrophyItems = new List<BasicItemData>();
				}
				{
					foreach (BasicItemData trophyItem in instance.TrophyItems)
					{
						Items[itemcategory].Add(DIContainerLogic.InventoryService.ReinitNewInventoryItemGameData(this, trophyItem));
					}
					break;
				}
			case InventoryItemType.Story:
				if (instance.StoryItems == null)
				{
					instance.StoryItems = new List<BasicItemData>();
				}
				{
					foreach (BasicItemData storyItem in instance.StoryItems)
					{
						Items[itemcategory].Add(DIContainerLogic.InventoryService.ReinitNewInventoryItemGameData(this, storyItem));
					}
					break;
				}
			case InventoryItemType.EventBattleItem:
			case InventoryItemType.EventCollectible:
			case InventoryItemType.EventCampaignItem:
			case InventoryItemType.EventBossItem:
				if (instance.EventItems == null)
				{
					instance.EventItems = new List<EventItemData>();
				}
				{
					foreach (EventItemData eventItem in instance.EventItems)
					{
						EventItemBalancingData balancing2 = null;
						if (DIContainerBalancing.Service.TryGetBalancingData<EventItemBalancingData>(eventItem.NameId, out balancing2) && balancing2.ItemType == itemcategory)
						{
							Items[itemcategory].Add(DIContainerLogic.InventoryService.ReinitNewInventoryItemGameData(this, eventItem));
						}
					}
					break;
				}
			case InventoryItemType.BannerTip:
			case InventoryItemType.Banner:
			case InventoryItemType.BannerEmblem:
				if (instance.BannerItems == null)
				{
					instance.BannerItems = new List<BannerItemData>();
				}
				{
					foreach (BannerItemData bannerItem in instance.BannerItems)
					{
						BannerItemBalancingData balancing = null;
						if (DIContainerBalancing.Service.TryGetBalancingData<BannerItemBalancingData>(bannerItem.NameId, out balancing) && balancing.ItemType == itemcategory)
						{
							Items[itemcategory].Add(DIContainerLogic.InventoryService.ReinitNewInventoryItemGameData(this, bannerItem));
						}
					}
					break;
				}
			case InventoryItemType.Mastery:
				if (instance.MasteryItems == null)
				{
					instance.MasteryItems = new List<MasteryItemData>();
				}
				{
					foreach (MasteryItemData masteryItem in instance.MasteryItems)
					{
						Items[itemcategory].Add(DIContainerLogic.InventoryService.ReinitNewInventoryItemGameData(this, masteryItem));
					}
					break;
				}
			case InventoryItemType.Skin:
				if (instance.SkinItems == null)
				{
					instance.SkinItems = new List<SkinItemData>();
				}
				{
					foreach (SkinItemData skinItem in instance.SkinItems)
					{
						Items[itemcategory].Add(DIContainerLogic.InventoryService.ReinitNewInventoryItemGameData(this, skinItem));
					}
					break;
				}
			case InventoryItemType.None:
				break;
			default:
				DebugLog.Error("Unhandled IteCategoryType in GenerateGameDatasFromItemDatas: " + itemcategory);
				break;
			}
		}

		private void AddItemDataToCategory(InventoryItemType itemcategory, InventoryData inventoryData, IInventoryItemData itemData)
		{
			switch (itemcategory)
			{
			case InventoryItemType.Class:
				inventoryData.ClassItems.Add(itemData as ClassItemData);
				break;
			case InventoryItemType.Consumable:
				inventoryData.ConsumableItems.Add(itemData as ConsumableItemData);
				break;
			case InventoryItemType.Ingredients:
				inventoryData.CraftingIngredientItems.Add(itemData as CraftingItemData);
				break;
			case InventoryItemType.MainHandEquipment:
				inventoryData.MainHandItems.Add(itemData as EquipmentData);
				break;
			case InventoryItemType.OffHandEquipment:
				inventoryData.OffHandItems.Add(itemData as EquipmentData);
				break;
			case InventoryItemType.PlayerStats:
				inventoryData.PlayerStats.Add(itemData as BasicItemData);
				break;
			case InventoryItemType.PlayerToken:
				break;
			case InventoryItemType.Points:
				break;
			case InventoryItemType.Premium:
				break;
			case InventoryItemType.CraftingRecipes:
				inventoryData.CraftingRecipesItems.Add(itemData as CraftingRecipeData);
				break;
			case InventoryItemType.Resources:
				inventoryData.CraftingResourceItems.Add(itemData as CraftingItemData);
				break;
			case InventoryItemType.Story:
				inventoryData.StoryItems.Add(itemData as BasicItemData);
				break;
			case InventoryItemType.Trophy:
				inventoryData.TrophyItems.Add(itemData as BasicItemData);
				break;
			case InventoryItemType.EventBattleItem:
			case InventoryItemType.EventCollectible:
			case InventoryItemType.EventCampaignItem:
			case InventoryItemType.EventBossItem:
				inventoryData.EventItems.Add(itemData as EventItemData);
				break;
			case InventoryItemType.BannerTip:
			case InventoryItemType.Banner:
			case InventoryItemType.BannerEmblem:
				inventoryData.BannerItems.Add(itemData as BannerItemData);
				break;
			case InventoryItemType.Mastery:
				inventoryData.MasteryItems.Add(itemData as MasteryItemData);
				break;
			case InventoryItemType.CollectionComponent:
				inventoryData.CollectionComponents.Add(itemData as BasicItemData);
				break;
			case InventoryItemType.Skin:
				inventoryData.SkinItems.Add(itemData as SkinItemData);
				break;
			default:
				DebugLog.Error(string.Concat("InventoryGameData::AddItemDataToCategory: Unhandled ItemCategoryType \"", itemcategory, "\" in AddItemDataToCategory"));
				break;
			}
		}

		public bool HasNewItemForge(InventoryItemType currentItemType)
		{
			foreach (CraftingRecipeGameData item in Items[InventoryItemType.CraftingRecipes])
			{
				IInventoryItemGameData data = null;
				if ((item.BalancingData.RecipeCategoryType == currentItemType && item.ItemData.IsNew) || (item.BalancingData.RecipeCategoryType == currentItemType && (item.BalancingData.RecipeCategoryType == InventoryItemType.Ingredients || item.BalancingData.RecipeCategoryType == InventoryItemType.Consumable || item.BalancingData.RecipeCategoryType == InventoryItemType.Resources) && DIContainerLogic.InventoryService.TryGetItemGameData(this, item.GetResultLoot().Keys.FirstOrDefault(), out data) && data.ItemData.IsNew))
				{
					return true;
				}
			}
			return false;
		}

		public bool HasNewItemForge()
		{
			return HasNewItemForge(InventoryItemType.MainHandEquipment) || HasNewItemForge(InventoryItemType.OffHandEquipment) || HasNewItemForge(InventoryItemType.Resources);
		}

		public bool HasNewItemAlchemy()
		{
			return HasNewItemForge(InventoryItemType.Consumable) || HasNewItemForge(InventoryItemType.Ingredients);
		}

		public bool HasNewItemBird(InventoryItemType currentItemType, BirdGameData selectedBird)
		{
			try
			{
				return (currentItemType == InventoryItemType.Class && Items[InventoryItemType.Skin].FirstOrDefault((IInventoryItemGameData i) => i.IsValidForBird(selectedBird) && i.ItemData.IsNew) != null) || Items[currentItemType].FirstOrDefault((IInventoryItemGameData i) => i.IsValidForBird(selectedBird) && i.ItemData.IsNew) != null;
			}
			catch
			{
				return false;
			}
		}

		public bool HasNewItemBanner(InventoryItemType currentItemType, BannerGameData selectedBanner)
		{
			if (Items[currentItemType].FirstOrDefault((IInventoryItemGameData i) => i.ItemData.IsNew) != null)
			{
				return true;
			}
			return false;
		}

		public bool HasNewItemBird(BirdGameData selectedBird)
		{
			return HasNewItemBird(InventoryItemType.Class, selectedBird) || HasNewItemBird(InventoryItemType.MainHandEquipment, selectedBird) || HasNewItemBird(InventoryItemType.OffHandEquipment, selectedBird);
		}

		private bool RemoveItemDataFromCategory(InventoryItemType itemcategory, InventoryData inventoryData, IInventoryItemData itemData)
		{
			switch (itemcategory)
			{
			case InventoryItemType.Class:
				return inventoryData.ClassItems.Remove(itemData as ClassItemData);
			case InventoryItemType.Skin:
				return inventoryData.SkinItems.Remove(itemData as SkinItemData);
			case InventoryItemType.Consumable:
				return inventoryData.ConsumableItems.Remove(itemData as ConsumableItemData);
			case InventoryItemType.Ingredients:
				return inventoryData.CraftingIngredientItems.Remove(itemData as CraftingItemData);
			case InventoryItemType.MainHandEquipment:
				return inventoryData.MainHandItems.Remove(itemData as EquipmentData);
			case InventoryItemType.OffHandEquipment:
				return inventoryData.OffHandItems.Remove(itemData as EquipmentData);
			case InventoryItemType.PlayerStats:
				return inventoryData.PlayerStats.Remove(itemData as BasicItemData);
			case InventoryItemType.CollectionComponent:
				return inventoryData.CollectionComponents.Remove(itemData as BasicItemData);
			case InventoryItemType.Resources:
				return inventoryData.CraftingResourceItems.Remove(itemData as CraftingItemData);
			case InventoryItemType.Story:
				return inventoryData.StoryItems.Remove(itemData as BasicItemData);
			case InventoryItemType.Trophy:
				return inventoryData.TrophyItems.Remove(itemData as BasicItemData);
			case InventoryItemType.CraftingRecipes:
				return inventoryData.CraftingRecipesItems.Remove(itemData as CraftingRecipeData);
			case InventoryItemType.EventBattleItem:
			case InventoryItemType.EventCollectible:
			case InventoryItemType.EventCampaignItem:
			case InventoryItemType.EventBossItem:
				return inventoryData.EventItems.Remove(itemData as EventItemData);
			case InventoryItemType.BannerTip:
			case InventoryItemType.Banner:
			case InventoryItemType.BannerEmblem:
				return inventoryData.BannerItems.Remove(itemData as BannerItemData);
			case InventoryItemType.Mastery:
				return inventoryData.MasteryItems.Remove(itemData as MasteryItemData);
			default:
				DebugLog.Error("Unhandled IteCategoryType in RemoveItemDataFromCategory");
				return true;
			}
		}

		protected override InventoryData CreateNewInstance(string nameId)
		{
			Items = new Dictionary<InventoryItemType, List<IInventoryItemGameData>>();
			foreach (int value in Enum.GetValues(typeof(InventoryItemType)))
			{
				Items.Add((InventoryItemType)value, new List<IInventoryItemGameData>());
			}
			CraftingRecipes = new Dictionary<InventoryItemType, Dictionary<string, List<IInventoryItemGameData>>>();
			CraftingRecipes.Add(InventoryItemType.Resources, new Dictionary<string, List<IInventoryItemGameData>>());
			CraftingRecipes.Add(InventoryItemType.Ingredients, new Dictionary<string, List<IInventoryItemGameData>>());
			CraftingRecipes.Add(InventoryItemType.MainHandEquipment, new Dictionary<string, List<IInventoryItemGameData>>());
			CraftingRecipes.Add(InventoryItemType.OffHandEquipment, new Dictionary<string, List<IInventoryItemGameData>>());
			CraftingRecipes.Add(InventoryItemType.Consumable, new Dictionary<string, List<IInventoryItemGameData>>());
			InventoryData inventoryData = new InventoryData();
			inventoryData.StoryItems = new List<BasicItemData>();
			inventoryData.ClassItems = new List<ClassItemData>();
			inventoryData.CraftingResourceItems = new List<CraftingItemData>();
			inventoryData.PlayerStats = new List<BasicItemData>();
			inventoryData.OffHandItems = new List<EquipmentData>();
			inventoryData.MainHandItems = new List<EquipmentData>();
			inventoryData.CraftingIngredientItems = new List<CraftingItemData>();
			inventoryData.ConsumableItems = new List<ConsumableItemData>();
			inventoryData.CraftingRecipesItems = new List<CraftingRecipeData>();
			inventoryData.EventItems = new List<EventItemData>();
			inventoryData.BannerItems = new List<BannerItemData>();
			inventoryData.MasteryItems = new List<MasteryItemData>();
			inventoryData.CollectionComponents = new List<BasicItemData>();
			inventoryData.TrophyItems = new List<BasicItemData>();
			inventoryData.SkinItems = new List<SkinItemData>();
			inventoryData.NameId = nameId;
			return inventoryData;
		}

		public void ClearInventoryOfType(InventoryItemType inventoryItemType)
		{
			List<IInventoryItemGameData> list = new List<IInventoryItemGameData>(Items[inventoryItemType]);
			foreach (IInventoryItemGameData item in list)
			{
				RemoveItemFromInventory(item);
			}
		}
	}
}

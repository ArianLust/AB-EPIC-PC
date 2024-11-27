using System;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using ABH.Services.Logic.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Interfaces;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using ABH.Shared.Models.InventoryItems;
using UnityEngine;

namespace ABH.Services.Logic
{
	public class InventoryOperationServiceRealImpl : IInventoryOperationService
	{
		private CraftingService m_craftingService;

		private bool m_restedBonus;

		private readonly Dictionary<string, Func<InventoryGameData, IInventoryItemGameData, ItemAddInfo, Dictionary<string, string>, int>> OnAddAction = new Dictionary<string, Func<InventoryGameData, IInventoryItemGameData, ItemAddInfo, Dictionary<string, string>, int>>();

		private readonly Dictionary<string, Func<InventoryGameData, IInventoryItemGameData, int, Dictionary<string, string>, int>> OnRemoveAction = new Dictionary<string, Func<InventoryGameData, IInventoryItemGameData, int, Dictionary<string, string>, int>>();

		private int extraDays = 1;

		public string m_previousPlayerReason;

		public bool m_savePreviousPlayer;

		internal bool m_previousPlayerSaved;

		public bool RestedBonus
		{
			get
			{
				return false;
			}
		}

		public InventoryOperationServiceRealImpl()
		{
			InitializeInventoryOperations();
		}

		public InventoryOperationServiceRealImpl SetCraftingService(CraftingService craftingService)
		{
			if (craftingService == null)
			{
				DebugLog.Error("Crafting Service is null");
			}
			m_craftingService = craftingService;
			return this;
		}

		public bool InitializeInventoryOperations()
		{
			InitOnAddMethods();
			InitOnRemoveMethods();
			return true;
		}

		public void InitializeNewInventory(InventoryGameData inventory)
		{
		}

		public bool TryGetItemGameData(InventoryGameData inventory, string itemName, out IInventoryItemGameData data)
		{
			IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(itemName.ToLower());
			if (balancingData == null)
			{
				DebugLog.Error(GetType(), "TryGetItemGameData: Item Balancing is missing: " + itemName);
			}
			data = null;
			return balancingData != null && TryGetItemGameData(inventory, balancingData, out data);
		}

		public bool TryGetItemGameData(InventoryGameData inventory, IInventoryItemBalancingData itembal, out IInventoryItemGameData data)
		{
			if (itembal == null || inventory == null)
			{
				data = null;
				return false;
			}
			try
			{
				if (CheckForItem(inventory, itembal))
				{
					data = inventory.Items[itembal.ItemType].FirstOrDefault((IInventoryItemGameData i) => i.ItemBalancing.NameId.ToLower() == itembal.NameId.ToLower());
					return true;
				}
			}
			catch
			{
				data = null;
				return false;
			}
			data = null;
			return false;
		}

		public bool CheckForItem(InventoryGameData inventory, string itemName)
		{
			IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(itemName.ToLower());
			if (balancingData == null)
			{
				DebugLog.Error(GetType(), "CheckForItem: Item Balancing is missing: " + itemName);
			}
			try
			{
				return balancingData != null && CheckForItem(inventory, balancingData);
			}
			catch
			{
				return false;
			}
		}

		private bool CheckForItem(InventoryGameData inventory, IInventoryItemBalancingData balancing)
		{
			if (!inventory.Items.ContainsKey(balancing.ItemType))
			{
				return false;
			}
			if (balancing is EquipmentBalancingData || balancing is EventItemBalancingData || balancing is BannerItemBalancingData)
			{
				return false;
			}
			IInventoryItemGameData inventoryItemGameData = null;
			int count = inventory.Items[balancing.ItemType].Count;
			for (int i = 0; i < count; i++)
			{
				IInventoryItemGameData inventoryItemGameData2 = inventory.Items[balancing.ItemType][i];
				if (string.Compare(inventoryItemGameData2.ItemBalancing.NameId, balancing.NameId, true) == 0)
				{
					inventoryItemGameData = inventoryItemGameData2;
					break;
				}
			}
			return inventoryItemGameData != null;
		}

		private bool CheckForItem(InventoryData inventory, IInventoryItemBalancingData balancing)
		{
			return true;
		}

		public int GetItemValue(InventoryGameData inventory, string itemName)
		{
			IInventoryItemBalancingData balancing = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(itemName.ToLower());
			if (balancing == null)
			{
				DebugLog.Error(GetType(), "GetItemValue: Item Balancing is missing: " + itemName);
				return 0;
			}
			try
			{
				if (!CheckForItem(inventory, balancing))
				{
					return 0;
				}
			}
			catch
			{
				return 0;
			}
			return inventory.Items[balancing.ItemType].FirstOrDefault((IInventoryItemGameData i) => i.ItemBalancing.NameId.ToLower() == balancing.NameId.ToLower()).ItemValue;
		}

		public int GetItemValue(InventoryData inventory, string itemName)
		{
			Debug.LogError("Wrong inventory implementation");
			return 0;
		}

		public int GetItemValue(InventoryGameData inventory, IInventoryItemGameData item)
		{
			return GetItemValue(inventory, item.ItemBalancing.NameId);
		}

		public IInventoryItemGameData AddItem(InventoryGameData inventory, int level, int quality, string itemName, int added, string addReason, EquipmentSource source = EquipmentSource.Loot)
		{
			return AddItem(inventory, level, quality, itemName, added, new Dictionary<string, string> { { "TypeOfUse", addReason } }, source);
		}

		public IInventoryItemGameData AddItem(InventoryGameData inventory, int level, int quality, string itemName, int added, Dictionary<string, string> addTrackingInfo, EquipmentSource source = EquipmentSource.Loot)
		{
			if (added < 0)
			{
				RemoveItem(inventory, itemName, added, addTrackingInfo);
				return null;
			}
			IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(itemName);
			IInventoryItemGameData data = null;
			ItemAddInfo addInfo = new ItemAddInfo
			{
				added = added,
				level = level,
				quality = quality
			};
			if (balancingData == null)
			{
				DebugLog.Error(GetType(), "AddItem: Item Balancing is missing: " + itemName);
			}
			try
			{
				if (!CheckForItem(inventory, balancingData))
				{
					return AddItem(inventory, InitItem(inventory, GenerateNewInventoryItemGameData(inventory, level, quality, itemName, 0, source)), addInfo, addTrackingInfo, source);
				}
			}
			catch
			{
				RemoveItem(inventory, itemName, added, addTrackingInfo);
				return null;
			}
			if (TryGetItemGameData(inventory, itemName, out data))
			{
				return AddItem(inventory, data, addInfo, addTrackingInfo, source);
			}
			return AddItem(inventory, InitItem(inventory, GenerateNewInventoryItemGameData(inventory, level, quality, itemName, 0, source)), addInfo, addTrackingInfo, source);
		}

		public IInventoryItemGameData AddItem(InventoryGameData inventory, IInventoryItemGameData item, ItemAddInfo addInfo, string addReason, EquipmentSource source = EquipmentSource.Loot)
		{
			return AddItem(inventory, item, addInfo, new Dictionary<string, string> { { "TypeOfUse", addReason } }, source);
		}

		public IInventoryItemGameData AddItem(InventoryGameData inventory, IInventoryItemGameData item, ItemAddInfo addInfo, Dictionary<string, string> addTrackingInfo, EquipmentSource source = EquipmentSource.Loot)
		{
			if (addInfo.added < 0)
			{
				RemoveItem(inventory, item, addInfo.added, addTrackingInfo);
				return null;
			}
			if (inventory.BalancingData.NameId == "player_inventory" && addInfo.added > 0 && addTrackingInfo.ContainsKey("TypeOfGain") && !string.IsNullOrEmpty(addTrackingInfo["TypeOfGain"]))
			{
				bool includeFlurry = false;
				if (addTrackingInfo.ContainsKey("TypeOfGain"))
				{
					includeFlurry = !addTrackingInfo["TypeOfGain"].StartsWith("defeated_pig");
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.SaveAdd("ItemName", item.ItemBalancing.NameId);
				dictionary.SaveAdd("TypeOfItem", item.ItemBalancing.ItemType.ToString());
				dictionary.SaveAdd("Amount", addInfo.added.ToString());
				foreach (KeyValuePair<string, string> item2 in addTrackingInfo)
				{
					dictionary.SaveAdd(item2.Key, addTrackingInfo[item2.Key]);
				}
				dictionary.SaveAdd("ItemQuality", item.ItemData.Quality.ToString());
				dictionary.SaveAdd("ItemLevel", item.ItemData.Level.ToString());
				ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
				DIContainerInfrastructure.GetAnalyticsSystem(includeFlurry).LogEventWithParameters("InventoryGained", dictionary);
			}
			int itemValue = item.ItemValue;
			if (OnAddAction.ContainsKey(item.ItemBalancing.ItemType.ToString().ToLower()))
			{
				item.ItemValue = OnAddAction[item.ItemBalancing.ItemType.ToString().ToLower()](inventory, item, addInfo, addTrackingInfo);
			}
			ClassItemGameData classItemGameData = item as ClassItemGameData;
			if (classItemGameData != null && string.IsNullOrEmpty(classItemGameData.BalancingData.ReplacementClassNameId) && DIContainerInfrastructure.GetCurrentPlayer() != null)
			{
				DIContainerInfrastructure.GetCurrentPlayer().AdvanceBirdMasteryToHalfOfHighest(classItemGameData);
			}
			ItemAddInfo itemAddInfo = new ItemAddInfo();
			itemAddInfo.added = item.ItemValue - itemValue;
			itemAddInfo.level = addInfo.level;
			itemAddInfo.quality = addInfo.quality;
			ItemAddInfo arg = itemAddInfo;
			item.ItemValue = OnAddAction["OnEachAdd"](inventory, item, arg, addTrackingInfo);
			item.RaiseItemDataChanged(item.ItemValue - itemValue);
			inventory.RaiseInventoryChanged(item.ItemBalancing.ItemType, item);
			if (item.ItemBalancing.ItemType == InventoryItemType.Story)
			{
				inventory.RaiseStoryItemGained(item);
			}
			if (item.ItemBalancing.ItemType == InventoryItemType.CollectionComponent && DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData != null)
			{
				DIContainerInfrastructure.EventSystemStateManager.UpdateEventRewardStatus();
			}
			return item;
		}

		public void SetItem(InventoryGameData inventory, int level, int quality, string itemName, int setValue, string setReason, EquipmentSource source = EquipmentSource.Loot)
		{
			SetItem(inventory, level, quality, itemName, setValue, new Dictionary<string, string> { { "TypeOfUse", setReason } }, source);
		}

		public void SetItem(InventoryGameData inventory, int level, int quality, string itemName, int setValue, Dictionary<string, string> addTrackingInfo, EquipmentSource source = EquipmentSource.Loot)
		{
			int itemValue = GetItemValue(inventory, itemName);
			if (setValue > itemValue)
			{
				AddItem(inventory, level, quality, itemName, setValue - itemValue, addTrackingInfo, source);
			}
			else if (setValue < itemValue)
			{
				RemoveItem(inventory, itemName, itemValue - setValue, addTrackingInfo);
			}
			else
			{
				DebugLog.Log("Item set value is identical to inventory value");
			}
		}

		public IInventoryItemGameData InitItem(InventoryGameData inventory, IInventoryItemGameData item)
		{
			item.ItemData.IsNew = true;
			inventory.AddNewItemToInventory(item);
			return item;
		}

		public bool IsPersitentItem(IInventoryItemGameData item)
		{
			return true;
		}

		public void AddMastery(InventoryGameData inventory, PlayerGameData player, int amount)
		{
			for (int i = 0; i < player.Birds.Count; i++)
			{
				BirdGameData bird = player.Birds[i];
				AddMastery(inventory, bird, amount);
			}
		}

		public void AddMastery(InventoryGameData inventory, BirdGameData bird, int amount)
		{
			List<ClassItemGameData> classItemDataList = (from o in inventory.Items[InventoryItemType.Class]
				where o.IsValidForBird(bird)
				select o as ClassItemGameData).ToList();
			AddMastery(inventory, classItemDataList, amount);
		}

		public void AddMastery(InventoryGameData inventory, List<ClassItemGameData> classItemDataList, int amount)
		{
			for (int i = 0; i < classItemDataList.Count; i++)
			{
				ClassItemGameData c = classItemDataList[i];
				AddMastery(inventory, c, amount);
			}
		}

		public void AddMastery(InventoryGameData inventory, ClassItemGameData c, int amount)
		{
			if (c == null)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < DIContainerInfrastructure.GetCurrentPlayer().Birds.Count; i++)
			{
				BirdGameData birdGameData = DIContainerInfrastructure.GetCurrentPlayer().Birds[i];
				if (birdGameData.Name == c.BalancingData.RestrictedBirdId)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				c.AddMastery(amount);
			}
		}

		public void InitOnAddMethods()
		{
			AddOnAddAction(InventoryItemType.PlayerStats.ToString().ToLower(), OnAddPlayerStats);
			AddOnAddAction(InventoryItemType.Points.ToString().ToLower(), OnAddPoints);
			AddOnAddAction(InventoryItemType.MainHandEquipment.ToString().ToLower(), OnAddAnything);
			AddOnAddAction(InventoryItemType.OffHandEquipment.ToString().ToLower(), OnAddAnything);
			AddOnAddAction(InventoryItemType.Banner.ToString().ToLower(), OnAddBannerItem);
			AddOnAddAction(InventoryItemType.BannerEmblem.ToString().ToLower(), OnAddBannerItem);
			AddOnAddAction(InventoryItemType.BannerTip.ToString().ToLower(), OnAddBannerItem);
			AddOnAddAction(InventoryItemType.Class.ToString().ToLower(), OnAddClass);
			AddOnAddAction(InventoryItemType.Skin.ToString().ToLower(), OnAddAnything);
			AddOnAddAction(InventoryItemType.Ingredients.ToString().ToLower(), OnAddResourceOrIngredient);
			AddOnAddAction(InventoryItemType.Resources.ToString().ToLower(), OnAddResourceOrIngredient);
			AddOnAddAction(InventoryItemType.CraftingRecipes.ToString().ToLower(), OnAddRecipes);
			AddOnAddAction(InventoryItemType.Consumable.ToString().ToLower(), OnAddConsumables);
			AddOnAddAction(InventoryItemType.Story.ToString().ToLower(), OnAddStoryItem);
			AddOnAddAction(InventoryItemType.Mastery.ToString().ToLower(), OnAddMasteryItem);
			AddOnAddAction(InventoryItemType.Points.ToString().ToLower(), OnAddAnything);
			AddOnAddAction(InventoryItemType.CollectionComponent.ToString().ToLower(), OnAddCollectionComponentItem);
			AddOnAddAction(InventoryItemType.Trophy.ToString().ToLower(), OnAddAnything);
			AddOnAddAction(InventoryItemType.Skin.ToString().ToLower(), OnAddSkinItem);
			AddOnAddAction("OnEachAdd", OnEachAdd);
		}

		private int OnAddSkinItem(InventoryGameData inventory, IInventoryItemGameData item, ItemAddInfo added, Dictionary<string, string> addreason)
		{
			if (DIContainerInfrastructure.GetCurrentPlayer() != null && DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData == inventory && (item as SkinItemGameData).BalancingData.SortPriority != 0 && !DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey("tutorial_equip_skin") && !DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey("tutorial_skin_shopped"))
			{
				ClassItemBalancingData classItemBalancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData((item as SkinItemGameData).BalancingData.OriginalClass) as ClassItemBalancingData;
				if (classItemBalancingData != null && DIContainerInfrastructure.GetCurrentPlayer().GetBird(classItemBalancingData.RestrictedBirdId) != null)
				{
					if (addreason.ContainsKey("CollectionReward"))
					{
						DebugLog.Log(GetType(), "OnAddSkinItem: Reason = CollectionReward - Start tutorial_equip_skin");
						DIContainerInfrastructure.TutorialMgr.StartTutorial("tutorial_equip_skin");
					}
					else if (addreason.ContainsKey("WorldmapShop"))
					{
						DebugLog.Log(GetType(), "OnAddSkinItem: Reason = CollectionReward - Start tutorial_equip_skin");
						DIContainerInfrastructure.TutorialMgr.StartTutorial("tutorial_skin_shopped_worldmap");
					}
					else
					{
						DebugLog.Log(GetType(), "OnAddSkinItem: Reason = something else - Start tutorial_skin_shopped");
						DIContainerInfrastructure.TutorialMgr.StartTutorial("tutorial_skin_shopped");
					}
				}
			}
			return item.ItemValue + added.added;
		}

		private int OnAddCollectionComponentItem(InventoryGameData inventory, IInventoryItemGameData item, ItemAddInfo added, Dictionary<string, string> addreason)
		{
			if (DIContainerLogic.EventSystemService.IsCollectionGroupAvailable())
			{
				if (DIContainerInfrastructure.EventSystemStateManager.IsCollectionComponentFull(item))
				{
					bool flag = addreason.ContainsKey("secondaryPrize");
					PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
					CollectionGroupBalancingData collectionGroupBalancing = currentPlayer.CurrentEventManagerGameData.CurrentMiniCampaign.CollectionGroupBalancing;
					Dictionary<string, int> dictionary = new Dictionary<string, int>();
					Dictionary<string, int> dictionary2 = ((!flag) ? DIContainerLogic.EventSystemService.m_CachedFallBackLoot : DIContainerLogic.EventSystemService.m_CachedSecondaryFallBackLoot);
					if (dictionary2 != null && dictionary2.Count > 0)
					{
						for (int i = 0; i < dictionary2.Count; i++)
						{
							string key = dictionary2.Keys.ToList()[i];
							int num = dictionary2.Values.ToList()[i];
							dictionary[key] = num * added.added;
						}
					}
					else
					{
						for (int j = 0; j < collectionGroupBalancing.ComponentFallbackLoot.Count; j++)
						{
							string key2 = collectionGroupBalancing.ComponentFallbackLoot.Keys.ToList()[j];
							int num2 = collectionGroupBalancing.ComponentFallbackLoot.Values.ToList()[j];
							dictionary[key2] = num2 * added.added;
						}
					}
					DIContainerLogic.GetLootOperationService().RewardLoot(inventory, 1, DIContainerLogic.GetLootOperationService().GenerateLoot(dictionary, currentPlayer.Data.Level), "collection_item_fallback");
					return item.ItemValue;
				}
				if (DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData != null)
				{
					DIContainerInfrastructure.EventSystemStateManager.UpdateEventRewardStatus();
				}
				return item.ItemValue + added.added;
			}
			return 0;
		}

		private int OnAddBannerItem(InventoryGameData inventory, IInventoryItemGameData item, ItemAddInfo added, Dictionary<string, string> addreason)
		{
			IInventoryItemGameData data = null;
			bool flag = false;
			foreach (KeyValuePair<string, string> item2 in addreason)
			{
				if (item2.Value == "pvpgacha" || item2.Value == "pvpadvgacha" || item2.Value == "pvpgacha_high" || item2.Value == "pvpadvgacha_high" || item2.Value == "pvpgacha_set" || item2.Value == "pvpadv_gacha_set" || item2.Value == "pvpgacha_high_set" || item2.Value == "pvpadvgacha_high_set")
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				if (TryGetItemGameData(inventory, "pvp_league_crown_max", out data))
				{
					item.ItemData.Quality = data.ItemData.Level;
				}
			}
			else if (TryGetItemGameData(inventory, "pvp_league_crown", out data))
			{
				item.ItemData.Quality = data.ItemData.Level;
			}
			return item.ItemValue + added.added;
		}

		private int OnAddMasteryItem(InventoryGameData inventory, IInventoryItemGameData item, ItemAddInfo added, Dictionary<string, string> addreason)
		{
			if (DIContainerLogic.InventoryService.GetItemValue(inventory, "unlock_mastery_badge") <= 0)
			{
				return 0;
			}
			MasteryItemGameData masteryItemGameData = item as MasteryItemGameData;
			string associatedClass = masteryItemGameData.BalancingData.AssociatedClass;
			if (associatedClass.Equals("ALL"))
			{
				if (IsAddMasteryPossible(masteryItemGameData.BalancingData, DIContainerInfrastructure.GetCurrentPlayer()))
				{
					AddMastery(inventory, DIContainerInfrastructure.GetCurrentPlayer(), added.added);
				}
				else
				{
					AddMasteryFallBackItem(inventory, DIContainerInfrastructure.GetCurrentPlayer(), masteryItemGameData);
				}
			}
			else if (associatedClass.Equals("current"))
			{
				if (ClientInfo.CurrentBattleGameData != null)
				{
					List<ICombatant> list = ClientInfo.CurrentBattleGameData.m_CombatantsPerFaction[Faction.Birds].Where((ICombatant c) => c.IsParticipating && !c.CharacterModel.IsNPC).ToList();
					for (int i = 0; i < list.Count; i++)
					{
						ICombatant combatant = list[i];
						AddMastery(inventory, combatant.CombatantClass, added.added);
					}
				}
			}
			else if (associatedClass.StartsWith("bird_"))
			{
				BirdGameData bird = DIContainerInfrastructure.GetCurrentPlayer().GetBird(masteryItemGameData.BalancingData.AssociatedClass);
				if (IsAddMasteryPossible(masteryItemGameData.BalancingData, DIContainerInfrastructure.GetCurrentPlayer()))
				{
					AddMastery(inventory, bird, added.added);
				}
				else
				{
					AddMasteryFallBackItem(inventory, DIContainerInfrastructure.GetCurrentPlayer(), masteryItemGameData);
				}
			}
			else
			{
				Requirement requirement = new Requirement();
				requirement.NameId = associatedClass;
				requirement.RequirementType = RequirementType.NotHaveAllUpgrades;
				requirement.Value = DIContainerBalancing.Service.GetBalancingDataList<ExperienceMasteryBalancingData>().Count();
				Requirement requirement2 = requirement;
				IInventoryItemGameData data;
				if (IsAddMasteryPossible(masteryItemGameData.BalancingData, DIContainerInfrastructure.GetCurrentPlayer()) && TryGetItemGameData(inventory, masteryItemGameData.BalancingData.AssociatedClass, out data))
				{
					AddMastery(inventory, data as ClassItemGameData, added.added);
				}
				else
				{
					AddMasteryFallBackItem(inventory, DIContainerInfrastructure.GetCurrentPlayer(), masteryItemGameData);
				}
			}
			return item.ItemValue + added.added;
		}

		public bool IsAddMasteryPossible(MasteryItemBalancingData masteryBalancing, PlayerGameData player)
		{
			if (DIContainerLogic.InventoryService.GetItemValue(player.InventoryGameData, "unlock_mastery_badge") <= 0)
			{
				return false;
			}
			string associatedClass = masteryBalancing.AssociatedClass;
			if (associatedClass.Equals("ALL"))
			{
				int count = DIContainerBalancing.Service.GetBalancingDataList<ExperienceMasteryBalancingData>().Count;
				Requirement requirement = new Requirement();
				requirement.NameId = associatedClass;
				requirement.RequirementType = RequirementType.NotHaveAllUpgrades;
				requirement.Value = count;
				Requirement req = requirement;
				if (DIContainerLogic.RequirementService.CheckRequirement(player, req))
				{
					return true;
				}
			}
			else if (associatedClass.StartsWith("bird_"))
			{
				BirdGameData bird = DIContainerInfrastructure.GetCurrentPlayer().GetBird(masteryBalancing.AssociatedClass);
				Requirement requirement = new Requirement();
				requirement.NameId = associatedClass;
				requirement.RequirementType = RequirementType.NotHaveAllUpgrades;
				requirement.Value = DIContainerBalancing.Service.GetBalancingDataList<ExperienceMasteryBalancingData>().Count();
				Requirement req2 = requirement;
				if (DIContainerLogic.RequirementService.CheckRequirement(player, req2))
				{
					return true;
				}
			}
			else
			{
				if (associatedClass.Equals("current"))
				{
					return true;
				}
				Requirement requirement = new Requirement();
				requirement.NameId = associatedClass;
				requirement.RequirementType = RequirementType.NotHaveItemWithLevel;
				requirement.Value = DIContainerBalancing.Service.GetBalancingDataList<ExperienceMasteryBalancingData>().Count();
				Requirement req3 = requirement;
				if (DIContainerLogic.RequirementService.CheckRequirement(player, req3))
				{
					return true;
				}
			}
			return false;
		}

		private void AddMasteryFallBackItem(InventoryGameData inventory, PlayerGameData playerGameData, MasteryItemGameData masteryItem)
		{
			DIContainerLogic.GetLootOperationService().RewardLoot(inventory, 1, DIContainerLogic.GetLootOperationService().GenerateLoot(masteryItem.BalancingData.FallbackLootTable, playerGameData.Data.Level), "mastery_conversion");
		}

		public List<IInventoryItemGameData> GetMasteryFallBackItem(PlayerGameData playerGameData, MasteryItemBalancingData masteryItem)
		{
			return DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(masteryItem.FallbackLootTable, playerGameData.Data.Level));
		}

		private int OnAddClass(InventoryGameData inventory, IInventoryItemGameData item, ItemAddInfo added, Dictionary<string, string> addreason)
		{
			int num = 1;
			if (item is ClassItemGameData)
			{
				ClassItemGameData classItemGameData = item as ClassItemGameData;
				if (!string.IsNullOrEmpty(classItemGameData.BalancingData.ReplacementClassNameId))
				{
					IInventoryItemGameData data = null;
					if (TryGetItemGameData(inventory, classItemGameData.BalancingData.ReplacementClassNameId, out data))
					{
						num = data.ItemValue;
						RemoveItem(inventory, classItemGameData.BalancingData.ReplacementClassNameId, GetItemValue(inventory, classItemGameData.BalancingData.ReplacementClassNameId), "replacement_class");
						classItemGameData.Data.Quality = data.ItemData.Quality;
						classItemGameData.Data.Level = data.ItemData.Level;
						classItemGameData.ItemValue = num;
						ClassItemGameData classItemGameData2 = data as ClassItemGameData;
						if (classItemGameData2 != null && DIContainerInfrastructure.GetCurrentPlayer() != null)
						{
							BirdGameData bird = DIContainerInfrastructure.GetCurrentPlayer().GetBird(classItemGameData2.BalancingData.RestrictedBirdId);
							if (bird != null)
							{
								IInventoryItemGameData data2 = null;
								if (TryGetItemGameData(bird.InventoryGameData, classItemGameData.BalancingData.ReplacementClassNameId, out data2))
								{
									EquipBirdWithItem(new List<IInventoryItemGameData> { classItemGameData }, InventoryItemType.Class, bird.InventoryGameData);
								}
							}
						}
					}
				}
				for (int j = 0; j < inventory.Items[InventoryItemType.Class].Count; j++)
				{
					ClassItemGameData classItemGameData3 = inventory.Items[InventoryItemType.Class][j] as ClassItemGameData;
					if (!(classItemGameData3.BalancingData.ReplacementClassNameId == classItemGameData.BalancingData.NameId))
					{
						continue;
					}
					RemoveItem(inventory, classItemGameData.BalancingData.NameId, GetItemValue(inventory, classItemGameData.BalancingData.NameId), "replacement_class_found");
					BirdGameData bird2 = DIContainerInfrastructure.GetCurrentPlayer().GetBird(classItemGameData3.BalancingData.RestrictedBirdId);
					if (bird2 != null)
					{
						IInventoryItemGameData data3 = null;
						if (TryGetItemGameData(bird2.InventoryGameData, classItemGameData3.BalancingData.ReplacementClassNameId, out data3))
						{
							EquipBirdWithItem(new List<IInventoryItemGameData> { classItemGameData3 }, InventoryItemType.Class, bird2.InventoryGameData);
						}
					}
				}
			}
			if (item.ItemData.Level != added.level)
			{
				item.ItemData.IsNew = true;
			}
			if (item.ItemData.Level < added.level && item.ItemData.Value > 0)
			{
				item.ItemData.Level = added.level;
			}
			if (addreason.Values.Any((string v) => v.StartsWith("bird_creation_")))
			{
				item.ItemData.Level = 1;
			}
			IInventoryItemGameData data4 = null;
			if (TryGetItemGameData(inventory, "unlock_mastery_badge", out data4))
			{
				data4.ItemData.Level = inventory.Items[InventoryItemType.Class].Sum((IInventoryItemGameData i) => i.ItemData.Level);
			}
			return num;
		}

		private IInventoryItemGameData HandleUpgradeClassItem(InventoryGameData inventory, string classUpgradeItemName, IInventoryItemGameData classUpgradeItem)
		{
			if (!TryGetItemGameData(inventory, classUpgradeItemName, out classUpgradeItem))
			{
				classUpgradeItem = AddItem(inventory, 1, 1, classUpgradeItemName, 1, "class_upgraded");
			}
			else
			{
				classUpgradeItem.ItemData.Level = classUpgradeItem.ItemData.Level + 1;
			}
			return classUpgradeItem;
		}

		private int OnAddRecipes(InventoryGameData inventory, IInventoryItemGameData item, ItemAddInfo added, Dictionary<string, string> addreason)
		{
			CraftingRecipeGameData craftingRecipeGameData = item as CraftingRecipeGameData;
			List<KeyValuePair<string, int>> craftingCosts = new List<KeyValuePair<string, int>>();
			if (craftingRecipeGameData == null)
			{
				return 1;
			}
			List<IInventoryItemGameData> list = inventory.Items[InventoryItemType.CraftingRecipes];
			if (craftingRecipeGameData.BalancingData.RecipeCategoryType == InventoryItemType.Consumable && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					IInventoryItemGameData inventoryItemGameData = list[i];
					CraftingRecipeGameData craftingRecipeGameData2 = inventoryItemGameData as CraftingRecipeGameData;
					if (craftingRecipeGameData2 != null)
					{
						if (craftingRecipeGameData2.BalancingData.NameId == craftingRecipeGameData.BalancingData.NameId && added.level < craftingRecipeGameData2.Data.Level)
						{
							DebugLog.Error("[InventoryOperationServiceRealImpl] Found a better recipe of same item, therfore do nothing");
							return 1;
						}
						List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(craftingRecipeGameData2.GetResultLoot(), craftingRecipeGameData2.Data.Level));
						ConsumableItemGameData consumableItemGameData = itemsFromLoot[0] as ConsumableItemGameData;
						List<IInventoryItemGameData> itemsFromLoot2 = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(craftingRecipeGameData.GetResultLoot(), craftingRecipeGameData.Data.Level));
						ConsumableItemGameData consumableItemGameData2 = itemsFromLoot2[0] as ConsumableItemGameData;
						if (consumableItemGameData != null && consumableItemGameData2 != null && !string.IsNullOrEmpty(consumableItemGameData2.BalancingData.ConsumableStatckingType) && consumableItemGameData.BalancingData.ConsumableStatckingType == consumableItemGameData2.BalancingData.ConsumableStatckingType && consumableItemGameData.BalancingData.ConversionPoints > consumableItemGameData2.BalancingData.ConversionPoints)
						{
							DebugLog.Error("[InventoryOperationServiceRealImpl] Found a better recipe of same type, therfore remove this item");
							inventory.RemoveItemFromInventory(craftingRecipeGameData);
							return 0;
						}
					}
				}
			}
			foreach (string key in craftingRecipeGameData.GetResultLoot().Keys)
			{
				DIContainerLogic.CraftingService.AddCraftingCostsForItem(ref craftingCosts, key, craftingRecipeGameData.Data.Level);
			}
			for (int j = 0; j < craftingCosts.Count; j++)
			{
				KeyValuePair<string, int> keyValuePair = craftingCosts[j];
				if (GetItemValue(inventory, keyValuePair.Key) <= 0)
				{
					CraftingItemBalancingData craftingItemBalancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(keyValuePair.Key) as CraftingItemBalancingData;
					if (craftingItemBalancingData != null && GetItemValue(inventory, craftingItemBalancingData.Recipe) <= 0)
					{
						AddItem(inventory, 1, 1, craftingItemBalancingData.Recipe, 1, "fallback_add_crafting_recipe");
					}
				}
			}
			if ((craftingRecipeGameData.BalancingData.RecipeCategoryType == InventoryItemType.MainHandEquipment || craftingRecipeGameData.BalancingData.RecipeCategoryType == InventoryItemType.OffHandEquipment) && GetItemValue(inventory, craftingRecipeGameData.ItemBalancing.NameId) > 0 && added.level <= item.ItemData.Level)
			{
				int level = added.level;
				CraftingRecipeBalancingData balancingData = craftingRecipeGameData.BalancingData;
				DebugLog.Log("Try to reward fallback Loot for Recipe: " + craftingRecipeGameData.BalancingData.NameId + " with level: " + added.level);
				Dictionary<string, LootInfoData> fallbackLootFromRecipe = GetFallbackLootFromRecipe(craftingRecipeGameData, level);
				DIContainerLogic.GetLootOperationService().RewardLoot(inventory, 1, fallbackLootFromRecipe, "fallback_recipe");
				return 1;
			}
			if (item.ItemData.Level != added.level)
			{
				item.ItemData.IsNew = true;
			}
			item.ItemData.Level = added.level;
			CraftingRecipeData craftingRecipeData = item.ItemData as CraftingRecipeData;
			if (craftingRecipeData != null)
			{
				craftingRecipeData.IsNewInShop = true;
			}
			DebugLog.Log("Add Recipe");
			if (craftingRecipeGameData.BalancingData.RecipeCategoryType == InventoryItemType.Consumable)
			{
				DebugLog.Log("Add Consumable Recipe");
				List<IInventoryItemGameData> itemsFromLoot3 = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(craftingRecipeGameData.GetResultLoot(), craftingRecipeGameData.Data.Level));
				ConsumableItemGameData consumableItemGameData3 = itemsFromLoot3[0] as ConsumableItemGameData;
				if (consumableItemGameData3 == null || consumableItemGameData3.BalancingData.ConversionPoints <= 0)
				{
					return 1;
				}
				DebugLog.Log("For Consumable: " + consumableItemGameData3.BalancingData.NameId);
				List<IInventoryItemGameData> list2 = inventory.Items[InventoryItemType.Consumable];
				List<ConsumableItemGameData> list3 = new List<ConsumableItemGameData>();
				for (int k = 0; k < list2.Count; k++)
				{
					ConsumableItemGameData consumableItemGameData4 = list2[k] as ConsumableItemGameData;
					if (consumableItemGameData4 != null && !(consumableItemGameData3.BalancingData.ConsumableStatckingType != consumableItemGameData4.BalancingData.ConsumableStatckingType))
					{
						if (consumableItemGameData3.BalancingData.NameId == consumableItemGameData4.BalancingData.NameId)
						{
							item.ItemData.Level = added.level;
							consumableItemGameData4.ItemData.Level = added.level;
							consumableItemGameData4.SetResultingSkill();
						}
						else
						{
							list3.Add(consumableItemGameData4);
						}
					}
				}
				int num = 0;
				for (int num2 = list3.Count - 1; num2 >= 0; num2--)
				{
					if (list3[num2].BalancingData.ConversionPoints > consumableItemGameData3.BalancingData.ConversionPoints)
					{
						DebugLog.Log("Not Add lower Level Consumables");
						return 0;
					}
					num += list3[num2].ItemValue;
					DIContainerLogic.InventoryService.RemoveItem(inventory, list3[num2].BalancingData.NameId, list3[num2].ItemValue, "consumable_conversion");
				}
				int num3 = num;
				DebugLog.Log("Conversion Points: " + num);
				List<IInventoryItemGameData> list4 = inventory.Items[InventoryItemType.CraftingRecipes];
				for (int num4 = list4.Count - 1; num4 >= 0; num4--)
				{
					CraftingRecipeGameData craftingRecipeGameData3 = list4[num4] as CraftingRecipeGameData;
					if (craftingRecipeGameData3 != null && !(craftingRecipeGameData3.BalancingData.NameId == craftingRecipeGameData.BalancingData.NameId) && craftingRecipeGameData3.BalancingData.RecipeCategoryType == InventoryItemType.Consumable)
					{
						List<IInventoryItemGameData> itemsFromLoot4 = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(craftingRecipeGameData3.GetResultLoot(), craftingRecipeGameData3.Data.Level));
						ConsumableItemGameData consumableItemGameData5 = itemsFromLoot4[0] as ConsumableItemGameData;
						if (consumableItemGameData5 != null && consumableItemGameData5.BalancingData.ConsumableStatckingType == consumableItemGameData3.BalancingData.ConsumableStatckingType && consumableItemGameData5.BalancingData.ConversionPoints <= consumableItemGameData3.BalancingData.ConversionPoints && inventory.RemoveItemFromInventory(craftingRecipeGameData3))
						{
							IInventoryItemGameData data = null;
							if (DIContainerLogic.InventoryService.TryGetItemGameData(inventory, consumableItemGameData5.BalancingData.NameId, out data))
							{
								inventory.RemoveItemFromInventory(data);
							}
							DebugLog.Log("Removed old Consumable Recipe: " + craftingRecipeGameData3.BalancingData.NameId);
						}
					}
				}
				if (num3 > 0)
				{
					DIContainerLogic.InventoryService.AddItem(inventory, consumableItemGameData3.Data.Level, consumableItemGameData3.Data.Quality, consumableItemGameData3.BalancingData.NameId, num3, "consumable_conversion");
				}
			}
			return 1;
		}

		public Dictionary<string, LootInfoData> GetFallbackLootFromRecipe(CraftingRecipeGameData newRecipe, int level)
		{
			KeyValuePair<string, int> fallbackRecipeItem = m_craftingService.GetFallbackRecipeItem(newRecipe.BalancingData, level);
			if (fallbackRecipeItem.Key == null)
			{
				return new Dictionary<string, LootInfoData>();
			}
			return DIContainerLogic.GetLootOperationService().GenerateLoot(new Dictionary<string, int> { { fallbackRecipeItem.Key, fallbackRecipeItem.Value } }, level);
		}

		private int OnAddConsumables(InventoryGameData inventory, IInventoryItemGameData item, ItemAddInfo added, Dictionary<string, string> addreason)
		{
			ConsumableItemGameData consumableItemGameData = item as ConsumableItemGameData;
			if (consumableItemGameData != null && string.IsNullOrEmpty(consumableItemGameData.BalancingData.ConsumableStatckingType))
			{
				return item.ItemValue + added.added;
			}
			if (consumableItemGameData == null)
			{
				return 0;
			}
			List<IInventoryItemGameData> list = inventory.Items[InventoryItemType.CraftingRecipes];
			List<CraftingRecipeGameData> list2 = new List<CraftingRecipeGameData>();
			List<ConsumableItemGameData> list3 = new List<ConsumableItemGameData>();
			for (int i = 0; i < list.Count; i++)
			{
				IInventoryItemGameData inventoryItemGameData = list[i];
				CraftingRecipeGameData craftingRecipeGameData = inventoryItemGameData as CraftingRecipeGameData;
				if (craftingRecipeGameData != null && craftingRecipeGameData.BalancingData.RecipeCategoryType == InventoryItemType.Consumable)
				{
					list2.Add(craftingRecipeGameData);
					List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(craftingRecipeGameData.GetResultLoot(), craftingRecipeGameData.Data.Level));
					ConsumableItemGameData item2 = itemsFromLoot[0] as ConsumableItemGameData;
					list3.Add(item2);
				}
			}
			for (int j = 0; j < list3.Count; j++)
			{
				ConsumableItemGameData consumableItemGameData2 = list3[j];
				if (consumableItemGameData2.BalancingData.ConsumableStatckingType == consumableItemGameData.BalancingData.ConsumableStatckingType && consumableItemGameData2.BalancingData.ConversionPoints != consumableItemGameData.BalancingData.ConversionPoints && consumableItemGameData2.BalancingData.NameId != consumableItemGameData.BalancingData.NameId)
				{
					DIContainerLogic.InventoryService.AddItem(inventory, consumableItemGameData2.Data.Level, consumableItemGameData2.Data.Quality, consumableItemGameData2.BalancingData.NameId, added.added, "consumable_conversion");
					inventory.RemoveItemFromInventory(consumableItemGameData);
					return 0;
				}
			}
			return item.ItemValue + added.added;
		}

		public void MapPotionRecipes(InventoryGameData inventory)
		{
			List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
			List<string> list2 = new List<string>();
			for (int i = 0; i < inventory.Items[InventoryItemType.CraftingRecipes].Count; i++)
			{
				IInventoryItemGameData inventoryItemGameData = inventory.Items[InventoryItemType.CraftingRecipes][i];
				CraftingRecipeGameData craftingRecipeGameData = inventoryItemGameData as CraftingRecipeGameData;
				if (craftingRecipeGameData != null && craftingRecipeGameData.BalancingData.RecipeCategoryType == InventoryItemType.Consumable)
				{
					string name = craftingRecipeGameData.Name;
					string text = MapRecipeChange(name);
					if (name != text)
					{
						list.Add(craftingRecipeGameData);
						list2.Add(text);
					}
				}
			}
			foreach (IInventoryItemGameData item in list)
			{
				inventory.RemoveItemFromInventory(item);
			}
			foreach (string item2 in list2)
			{
				DIContainerLogic.InventoryService.AddItem(inventory, 1, 1, item2, 1, "convertion_1.5.3");
			}
		}

		private string MapRecipeChange(string oldRecipeName)
		{
			switch (oldRecipeName)
			{
			case "recipe_potion_healing_01":
				return "recipe_potion_healing_01";
			case "recipe_potion_healing_02":
				return "recipe_potion_healing_03";
			case "recipe_potion_healing_02_02":
				return "recipe_potion_healing_03";
			case "recipe_potion_healing_02_03":
				return "recipe_potion_healing_03";
			case "recipe_potion_healing_03":
				return "recipe_potion_healing_05";
			case "recipe_potion_healing_03_02":
				return "recipe_potion_healing_05";
			case "recipe_potion_healing_04":
				return "recipe_potion_healing_07";
			case "recipe_potion_healing_04_02":
				return "recipe_potion_healing_07";
			case "recipe_potion_healing_04_03":
				return "recipe_potion_healing_07";
			case "recipe_potion_healing_04_04":
				return "recipe_potion_healing_07";
			case "recipe_potion_healing_04_05":
				return "recipe_potion_healing_07";
			case "recipe_potion_healing_04_06":
				return "recipe_potion_healing_07";
			case "recipe_potion_healing_05":
				return "recipe_potion_healing_13";
			case "recipe_potion_healing_05_02":
				return "recipe_potion_healing_13";
			case "recipe_potion_healing_all_01":
				return "recipe_potion_healing_all_01";
			case "recipe_potion_healing_all_01_02":
				return "recipe_potion_healing_all_02";
			case "recipe_potion_healing_all_01_03":
				return "recipe_potion_healing_all_02";
			case "recipe_potion_healing_all_01_04":
				return "recipe_potion_healing_all_02";
			case "recipe_potion_healing_all_01_05":
				return "recipe_potion_healing_all_03";
			case "recipe_potion_healing_all_01_06":
				return "recipe_potion_healing_all_04";
			case "recipe_potion_healing_all_01_07":
				return "recipe_potion_healing_all_06";
			case "recipe_potion_healing_all_02":
				return "recipe_potion_healing_all_08";
			case "recipe_potion_healing_all_02_02":
				return "recipe_potion_healing_all_08";
			default:
				return oldRecipeName;
			}
		}

		public bool FixPotionsInInventory(InventoryGameData inventory)
		{
			Dictionary<string, ConsumableItemGameData> dictionary = new Dictionary<string, ConsumableItemGameData>();
			Dictionary<string, CraftingRecipeGameData> dictionary2 = new Dictionary<string, CraftingRecipeGameData>();
			if (inventory.CraftingRecipes == null || !inventory.CraftingRecipes.ContainsKey(InventoryItemType.Consumable))
			{
				return false;
			}
			List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
			for (int i = 0; i < inventory.Items[InventoryItemType.CraftingRecipes].Count; i++)
			{
				IInventoryItemGameData inventoryItemGameData = inventory.Items[InventoryItemType.CraftingRecipes][i];
				CraftingRecipeGameData craftingRecipeGameData = inventoryItemGameData as CraftingRecipeGameData;
				if (craftingRecipeGameData == null || craftingRecipeGameData.BalancingData.RecipeCategoryType != InventoryItemType.Consumable)
				{
					continue;
				}
				List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(craftingRecipeGameData.GetResultLoot(), craftingRecipeGameData.Data.Level));
				ConsumableItemGameData consumableItemGameData = itemsFromLoot[0] as ConsumableItemGameData;
				if (consumableItemGameData == null || string.IsNullOrEmpty(consumableItemGameData.BalancingData.ConsumableStatckingType))
				{
					continue;
				}
				if (dictionary.ContainsKey(consumableItemGameData.BalancingData.ConsumableStatckingType))
				{
					DebugLog.Error("[FixPotionsInInventory] Consumable Recipe is doubled: " + consumableItemGameData.BalancingData.ConsumableStatckingType);
					if (dictionary[consumableItemGameData.BalancingData.ConsumableStatckingType].BalancingData.ConversionPoints < consumableItemGameData.BalancingData.ConversionPoints)
					{
						list.Add(dictionary2[consumableItemGameData.BalancingData.ConsumableStatckingType]);
						dictionary2[consumableItemGameData.BalancingData.ConsumableStatckingType] = craftingRecipeGameData;
						dictionary[consumableItemGameData.BalancingData.ConsumableStatckingType] = consumableItemGameData;
					}
					else if (dictionary[consumableItemGameData.BalancingData.ConsumableStatckingType].BalancingData.ConversionPoints > consumableItemGameData.BalancingData.ConversionPoints)
					{
						list.Add(dictionary2[consumableItemGameData.BalancingData.ConsumableStatckingType]);
					}
				}
				else
				{
					dictionary2.Add(consumableItemGameData.BalancingData.ConsumableStatckingType, craftingRecipeGameData);
					dictionary.Add(consumableItemGameData.BalancingData.ConsumableStatckingType, consumableItemGameData);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				IInventoryItemGameData itemGameData = list[j];
				inventory.RemoveItemFromInventory(itemGameData);
			}
			for (int num = inventory.Items[InventoryItemType.Consumable].Count - 1; num >= 0; num--)
			{
				ConsumableItemGameData consumableItemGameData2 = inventory.Items[InventoryItemType.Consumable][num] as ConsumableItemGameData;
				if (consumableItemGameData2 != null && dictionary.ContainsKey(consumableItemGameData2.BalancingData.ConsumableStatckingType) && dictionary[consumableItemGameData2.BalancingData.ConsumableStatckingType].BalancingData.NameId != consumableItemGameData2.BalancingData.NameId)
				{
					ConsumableItemGameData consumableItemGameData3 = dictionary[consumableItemGameData2.BalancingData.ConsumableStatckingType];
					DIContainerLogic.InventoryService.AddItem(inventory, consumableItemGameData3.Data.Level, consumableItemGameData3.Data.Quality, consumableItemGameData3.BalancingData.NameId, consumableItemGameData2.ItemValue, "consumable_conversion");
					list.Add(consumableItemGameData2);
				}
			}
			for (int k = 0; k < list.Count; k++)
			{
				IInventoryItemGameData itemGameData2 = list[k];
				inventory.RemoveItemFromInventory(itemGameData2);
			}
			return true;
		}

		private int OnAddStoryItem(InventoryGameData inventory, IInventoryItemGameData item, ItemAddInfo added, Dictionary<string, string> addreason)
		{
			item.ItemData.IsNew = true;
			item.ItemData.Level = added.level;
			DebugLog.Log("Story Item added the first time: " + item.ItemBalancing.NameId);
			EvaluateRemoveBirdItem(item);
			EvaluateAddBirdItem(item);
			if (item.ItemBalancing.NameId == "pvp_won_battles")
			{
				return item.ItemValue + added.added;
			}
			if (item.ItemBalancing.NameId.StartsWith("daily_"))
			{
				if (item.ItemBalancing.NameId == "daily_chain_stamp")
				{
					int num = item.ItemValue + added.added;
					if (num >= DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.BalancingData.DailyChainLength)
					{
						DebugLog.Log("Daily Login Bonus Finished with Count: " + item.ItemValue + added.added);
						IInventoryItemGameData inventoryItemGameData = AddItem(inventory, 1, 1, "daily_post_card", 1, "daily_chain_finished");
						DebugLog.Log("Daily Login Bonus Post Cards Count: " + inventoryItemGameData.ItemValue);
						int itemValue = GetItemValue(inventory, "daily_post_card_max");
						if (inventoryItemGameData.ItemValue > itemValue)
						{
							IInventoryItemGameData inventoryItemGameData2 = AddItem(inventory, 1, 1, "daily_post_card_max", inventoryItemGameData.ItemValue - itemValue, "daily_chain_finished_new_max");
							DebugLog.Log("Daily Login Bonus Max Post Cards Count: " + inventoryItemGameData2.ItemValue);
						}
						return 0;
					}
					DebugLog.Log("Daily Login Bonus count now: " + item.ItemValue + added.added);
					return num;
				}
				if (item.ItemBalancing.NameId.StartsWith("daily_post_card"))
				{
					return item.ItemValue + added.added;
				}
			}
			if (item.ItemValue < 1)
			{
				inventory.RaiseStoryItemGained(item);
				if (item.ItemBalancing.NameId.ToLower() == "unlock_mastery_badge")
				{
					List<int> classUpgradeToMasteryMapping = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").ClassUpgradeToMasteryMapping;
					if (classUpgradeToMasteryMapping != null)
					{
						for (int j = 0; j < inventory.Items[InventoryItemType.Class].Count; j++)
						{
							IInventoryItemGameData inventoryItemGameData3 = inventory.Items[InventoryItemType.Class][j];
							if (inventoryItemGameData3.ItemData.Level != 1)
							{
								if (inventoryItemGameData3.ItemData.Level > classUpgradeToMasteryMapping.Count && classUpgradeToMasteryMapping.Count > 0)
								{
									DebugLog.Error("Class " + inventoryItemGameData3.ItemBalancing.NameId + " has an invalid level: " + inventoryItemGameData3.ItemData.Level + " setting it to max Level " + classUpgradeToMasteryMapping[classUpgradeToMasteryMapping.Count - 1]);
									inventoryItemGameData3.ItemData.Level = classUpgradeToMasteryMapping[classUpgradeToMasteryMapping.Count - 1];
								}
								else
								{
									inventoryItemGameData3.ItemData.Level = classUpgradeToMasteryMapping[inventoryItemGameData3.ItemData.Level - 1];
								}
								ClientInfo.ShowMasteryConversionPopup = true;
							}
						}
						if (DIContainerInfrastructure.GetCurrentPlayer() != null && DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.LocationProgress[LocationType.World] >= 55)
						{
							for (int k = 0; k < inventory.Items[InventoryItemType.Class].Count; k++)
							{
								IInventoryItemGameData inventoryItemGameData4 = inventory.Items[InventoryItemType.Class][k];
								if (inventoryItemGameData4.ItemData.Level == 1)
								{
									inventoryItemGameData4.ItemData.Level = 2;
								}
							}
						}
						if (ClientInfo.ShowMasteryConversionPopup)
						{
							AddItem(inventory, 1, 1, "unlock_mastery_conversion", 1, "mastery_conversion");
						}
					}
				}
				if (string.Compare(item.ItemBalancing.NameId, "special_offer_rainbow_riot", true) == 0)
				{
					DIContainerLogic.GetShopService().StartRainbowRiot(DIContainerInfrastructure.GetCurrentPlayer());
				}
				else if (string.Compare(item.ItemBalancing.NameId, "special_offer_rainbow_riot_02", true) == 0)
				{
					DIContainerLogic.GetShopService().StartRainbowRiot2(DIContainerInfrastructure.GetCurrentPlayer());
				}
				if (string.Compare(item.ItemBalancing.NameId, "unlock_mastery_badge", true) == 0)
				{
					item.ItemData.Level = inventory.Items[InventoryItemType.Class].Sum((IInventoryItemGameData i) => i.ItemData.Level);
				}
				if (string.Compare(item.ItemBalancing.NameId, "story_forge", true) == 0)
				{
					DIContainerInfrastructure.TutorialMgr.StartTutorial("tutorial_craft_first_item");
					AddItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, 0, "forge_leveled", 1, "forge_basic_added");
				}
				if (string.Compare(item.ItemBalancing.NameId, "story_cauldron", true) == 0)
				{
					DIContainerInfrastructure.TutorialMgr.StartTutorial("tutorial_craft_healing_potion");
					AddItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, 0, "cauldron_leveled", 1, "cauldron_basic_added");
				}
				if (string.Compare(item.ItemBalancing.NameId, "unlock_goldenpigspawn", true) == 0)
				{
					DIContainerLogic.GetTimingService().GetTrustedTimeEx(delegate
					{
						DIContainerInfrastructure.GetCurrentPlayer().Data.GoldenPigHotspotId = null;
					});
				}
				if (string.Compare(item.ItemBalancing.NameId, "story_cauldron", true) == 0)
				{
					AddItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, 0, "cauldron_leveled", 1, "cauldron_basic_added");
				}
				if (string.Compare(item.ItemBalancing.NameId, "story_goldenpig", true) == 0)
				{
					DIContainerInfrastructure.TutorialMgr.StartTutorial("tutorial_use_gacha");
				}
				if (string.Compare(item.ItemBalancing.NameId, "key_yellow", true) == 0)
				{
					DIContainerInfrastructure.TutorialMgr.StartTutorial("tutorial_unlock_piggate");
				}
				if (string.Compare(item.ItemBalancing.NameId, "unlock_reroll_tutorial", true) == 0)
				{
					DIContainerInfrastructure.TutorialMgr.StartTutorial("tutorial_reroll_battle");
					AddItem(inventory, 1, 1, "unlock_reroll_first_reroll", 1, "tutorial_reroll_battle");
					AddItem(inventory, 1, 1, "unlock_reroll_failed_roll", 1, "tutorial_reroll_battle");
				}
				string achievementIdForStoryItemIfExists = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists(item.ItemBalancing.NameId.ToLower());
				if (!string.IsNullOrEmpty(achievementIdForStoryItemIfExists))
				{
					DIContainerInfrastructure.GetAchievementService().ReportUnlocked(achievementIdForStoryItemIfExists);
				}
			}
			if (string.Compare(item.ItemBalancing.NameId, "unlock_pvp", true) == 0)
			{
			}
			if (item.ItemBalancing.NameId.Contains("special_offer_rainbow_riot"))
			{
				return added.added;
			}
			return 1;
		}

		private void EvaluateRemoveBirdItem(IInventoryItemGameData item)
		{
			if (item.ItemBalancing.NameId.ToLower().Contains("removebird"))
			{
				DebugLog.Log("Remove Bird Item: " + item.ItemBalancing.AssetBaseId);
				DIContainerInfrastructure.GetCurrentPlayer().SetBirdUnavailable(item.ItemBalancing.AssetBaseId);
			}
		}

		private void EvaluateAddBirdItem(IInventoryItemGameData item)
		{
			if (!item.ItemBalancing.NameId.ToLower().Contains("addbird"))
			{
				return;
			}
			DebugLog.Log("Add Bird Item: " + item.ItemBalancing.AssetBaseId);
			DIContainerInfrastructure.TutorialMgr.FinishTutorialStep("tooltip_character");
			BirdGameData birdGameData = DIContainerInfrastructure.GetCurrentPlayer().AddBird(item.ItemBalancing.AssetBaseId);
			if (item.ItemValue < 1)
			{
				if (birdGameData.Name == "bird_yellow")
				{
					DIContainerInfrastructure.TutorialMgr.StartTutorialStep("tutorial_use_supportskill_on_ally");
				}
				if (birdGameData.Name == "bird_black")
				{
					DIContainerInfrastructure.TutorialMgr.StartTutorialStep("enter_friendlist");
				}
			}
		}

		private void AddOnAddAction(string itemName, Func<InventoryGameData, IInventoryItemGameData, ItemAddInfo, Dictionary<string, string>, int> func)
		{
			if (OnAddAction.ContainsKey(itemName))
			{
				OnAddAction.Remove(itemName);
			}
			OnAddAction.Add(itemName, func);
		}

		private int OnAddPlayerStats(InventoryGameData inventory, IInventoryItemGameData item, ItemAddInfo added, Dictionary<string, string> addreason)
		{
			if (item.ItemBalancing.NameId.StartsWith("delayed_"))
			{
				DebugLog.Log("[InventoryOperationServiceRealImpl] Added delayed currency: " + item.ItemBalancing.NameId);
				if (inventory.Data.DelayedRewards == null)
				{
					inventory.Data.DelayedRewards = new Dictionary<string, int>();
				}
				string text = item.ItemBalancing.NameId.Replace("delayed_", string.Empty);
				if (!inventory.Data.DelayedRewards.ContainsKey(text))
				{
					inventory.Data.DelayedRewards.Add(text, added.added);
				}
				else
				{
					Dictionary<string, int> delayedRewards;
					Dictionary<string, int> dictionary = (delayedRewards = inventory.Data.DelayedRewards);
					string key;
					string key2 = (key = text);
					int num = delayedRewards[key];
					dictionary[key2] = num + added.added;
				}
				return 0;
			}
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			if (item.ItemBalancing.NameId.StartsWith("event_points") && currentPlayer != null)
			{
				if (!DIContainerLogic.EventSystemService.IsCurrentEventAvailable(currentPlayer) && !DIContainerLogic.EventSystemService.IsEventRunning(currentPlayer.CurrentEventManagerGameData.Balancing))
				{
					return item.ItemValue;
				}
				uint currentScore = currentPlayer.CurrentEventManagerGameData.Data.CurrentScore;
				if (currentScore < 1000000000)
				{
					DIContainerLogic.EventSystemService.RemoveCheatingEventScoreFix();
					currentPlayer.CurrentEventManagerGameData.Data.CurrentScore += (uint)added.added;
				}
				else
				{
					currentPlayer.CurrentEventManagerGameData.Data.CurrentScore = (uint)added.added;
				}
				if (currentPlayer.CurrentEventManagerGameData.Data.MatchmakingScore != 0L)
				{
					if (currentPlayer.CurrentEventManagerGameData.Data.MatchmakingScore.ToString().Length >= 9)
					{
						DIContainerLogic.EventSystemService.OldEventScore = currentPlayer.CurrentEventManagerGameData.Data.CurrentScore ^ (Convert.ToUInt32(currentPlayer.CurrentEventManagerGameData.Data.MatchmakingScore.ToString().Substring(0, 9)) + 195225786);
						DIContainerLogic.EventSystemService.ServerEventScore = currentPlayer.CurrentEventManagerGameData.Data.CurrentScore ^ (Convert.ToInt64(currentPlayer.CurrentEventManagerGameData.Data.MatchmakingScore.ToString().Substring(0, 8)) + 1856747893);
					}
					else
					{
						DIContainerLogic.EventSystemService.OldEventScore = currentPlayer.CurrentEventManagerGameData.Data.CurrentScore ^ (Convert.ToUInt32(currentPlayer.CurrentEventManagerGameData.Data.MatchmakingScore) + 195225786);
						DIContainerLogic.EventSystemService.ServerEventScore = currentPlayer.CurrentEventManagerGameData.Data.CurrentScore ^ (Convert.ToInt64(currentPlayer.CurrentEventManagerGameData.Data.MatchmakingScore) + 1856747893);
					}
				}
				currentPlayer.CurrentEventManagerGameData.RegisterScoreChanged(currentScore, currentPlayer.CurrentEventManagerGameData.Data.CurrentScore);
				if (item.ItemValue < 1000000000)
				{
					return item.ItemValue + added.added;
				}
				return added.added;
			}
			if (item.ItemBalancing.NameId.StartsWith("pvp_points") && currentPlayer != null)
			{
				if (!DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(currentPlayer) || !DIContainerLogic.PvPSeasonService.IsPvPTurnRunning(currentPlayer.CurrentPvPSeasonGameData))
				{
					return item.ItemValue;
				}
				uint currentScore2 = currentPlayer.CurrentPvPSeasonGameData.CurrentSeasonTurn.Data.CurrentScore;
				if (currentScore2 < 100000000)
				{
					DIContainerLogic.PvPSeasonService.RemoveCheatingPvPScoreFix();
					currentPlayer.CurrentPvPSeasonGameData.CurrentSeasonTurn.Data.CurrentScore += (uint)added.added;
				}
				else
				{
					currentPlayer.CurrentPvPSeasonGameData.CurrentSeasonTurn.Data.CurrentScore = (uint)added.added;
				}
				if (currentPlayer.CurrentPvPSeasonGameData.CurrentSeasonTurn.Data.MatchmakingScore != 0L)
				{
					DIContainerLogic.PvPSeasonService.OldPvPScore = currentPlayer.CurrentPvPSeasonGameData.CurrentSeasonTurn.Data.CurrentScore ^ (Convert.ToUInt32(currentPlayer.CurrentPvPSeasonGameData.CurrentSeasonTurn.Data.MatchmakingScore.ToString().Substring(0, 9)) + 195225786);
					DIContainerLogic.PvPSeasonService.ServerPvPScore = currentPlayer.CurrentPvPSeasonGameData.CurrentSeasonTurn.Data.CurrentScore ^ (Convert.ToInt64(currentPlayer.CurrentPvPSeasonGameData.CurrentSeasonTurn.Data.MatchmakingScore.ToString().Substring(0, 8)) + 1856747893);
				}
				PvPSeasonManagerGameData currentPvPSeasonGameData = currentPlayer.CurrentPvPSeasonGameData;
				if (currentPlayer.Data.OverAllSeasonPvpPoints == null)
				{
					currentPlayer.Data.OverAllSeasonPvpPoints = new Dictionary<string, int>();
				}
				if (!currentPlayer.Data.OverAllSeasonPvpPoints.ContainsKey(currentPvPSeasonGameData.Balancing.NameId))
				{
					currentPlayer.Data.OverAllSeasonPvpPoints.Add(currentPvPSeasonGameData.Balancing.NameId, added.added);
				}
				else
				{
					Dictionary<string, int> overAllSeasonPvpPoints;
					Dictionary<string, int> dictionary2 = (overAllSeasonPvpPoints = currentPlayer.Data.OverAllSeasonPvpPoints);
					string key;
					string key3 = (key = currentPvPSeasonGameData.Balancing.NameId);
					int num = overAllSeasonPvpPoints[key];
					dictionary2[key3] = num + added.added;
				}
				currentPlayer.CurrentPvPSeasonGameData.CurrentSeasonTurn.RegisterScoreChanged(currentScore2, currentPlayer.CurrentPvPSeasonGameData.CurrentSeasonTurn.Data.CurrentScore);
				if (item.ItemValue < 1000000000)
				{
					return item.ItemValue + added.added;
				}
				return added.added;
			}
			if (item.ItemBalancing.NameId.StartsWith("event_energy"))
			{
				if (DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").ItemMaxCaps.ContainsKey(item.ItemBalancing.NameId))
				{
					return Mathf.Min(item.ItemValue + added.added, DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").ItemMaxCaps[item.ItemBalancing.NameId] + GetItemValue(inventory, item.ItemBalancing.NameId + "_cap_extension"));
				}
				return item.ItemValue + added.added;
			}
			if (item.ItemBalancing.NameId.Equals("experience"))
			{
				float num2 = 0f;
				if (currentPlayer != null)
				{
					num2 = DIContainerLogic.PlayerOperationsService.GetXPMultiplierForLevelDifference(currentPlayer, added.level);
					added.added = (int)Math.Round((float)added.added + (float)added.added * (num2 / 100f));
				}
				num2 = GetItemValue(inventory, "xp_multiplier_consumable_01");
				added.added = (int)Math.Floor((float)added.added + (float)added.added * (num2 / 100f));
				num2 = GetItemValue(inventory, "xp_multiplier_global");
				added.added = (int)Math.Floor((float)added.added + (float)added.added * (num2 / 100f));
				num2 = (RestedBonus ? 100 : 0);
				added.added = (int)Math.Floor((float)added.added + (float)added.added * (num2 / 100f));
			}
			else if (item.ItemBalancing.NameId.StartsWith("daily_"))
			{
				if (item.ItemBalancing.NameId == "daily_chain_stamp")
				{
					int num3 = item.ItemValue + added.added;
					if (num3 >= 7)
					{
						DebugLog.Log("Daily Login Bonus Finished with Count: " + item.ItemValue + added.added);
						IInventoryItemGameData inventoryItemGameData = AddItem(inventory, 1, 1, "daily_post_card", 1, "daily_chain_finished");
						DebugLog.Log("Daily Login Bonus Post Cards Count: " + inventoryItemGameData.ItemValue);
						int itemValue = GetItemValue(inventory, "daily_post_card_max");
						if (inventoryItemGameData.ItemValue > itemValue)
						{
							IInventoryItemGameData inventoryItemGameData2 = AddItem(inventory, 1, 1, "daily_post_card_max", inventoryItemGameData.ItemValue - itemValue, "daily_chain_finished_new_max");
							DebugLog.Log("Daily Login Bonus Max Post Cards Count: " + inventoryItemGameData2.ItemValue);
						}
						return 0;
					}
					return num3;
				}
				if (item.ItemBalancing.NameId == "daily_chain_introduction" && currentPlayer != null)
				{
					currentPlayer.m_UnlockDailyCalendarSessionFlag = true;
				}
			}
			return item.ItemValue + added.added;
		}

		private int OnAddPoints(InventoryGameData inventory, IInventoryItemGameData item, ItemAddInfo added, Dictionary<string, string> addreason)
		{
			return item.ItemValue + added.added;
		}

		private int OnAddAnything(InventoryGameData inventory, IInventoryItemGameData item, ItemAddInfo added, Dictionary<string, string> addreason)
		{
			return item.ItemValue + added.added;
		}

		private int OnAddResourceOrIngredient(InventoryGameData inventory, IInventoryItemGameData item, ItemAddInfo added, Dictionary<string, string> addreason)
		{
			if (GetItemValue(inventory, item.ItemBalancing.NameId) <= 0)
			{
				CraftingRecipeBalancingData craftingRecipeBalancingData = DIContainerBalancing.Service.GetBalancingDataList<CraftingRecipeBalancingData>().FirstOrDefault((CraftingRecipeBalancingData r) => r.ResultLoot.ContainsKey(item.ItemBalancing.NameId));
				if (craftingRecipeBalancingData != null)
				{
					AddItem(inventory, 1, 1, craftingRecipeBalancingData.NameId, 1, "add_missing_recipe_for_item");
				}
			}
			return item.ItemValue + added.added;
		}

		private int OnEachAdd(InventoryGameData inventory, IInventoryItemGameData item, ItemAddInfo added, Dictionary<string, string> addreason)
		{
			return item.ItemValue;
		}

		private int OnEachRemove(InventoryGameData inventory, IInventoryItemGameData item, int removed, Dictionary<string, string> removereason)
		{
			return item.ItemValue;
		}

		public bool RemoveItem(InventoryGameData inventory, string itemName, int removed, string removeReason)
		{
			return RemoveItem(inventory, itemName, removed, new Dictionary<string, string> { { "TypeOfUse", removeReason } });
		}

		public bool RemoveItem(InventoryGameData inventory, string itemName, int removed, Dictionary<string, string> removeTrackingInfo)
		{
			IInventoryItemGameData data = null;
			if (TryGetItemGameData(inventory, itemName, out data))
			{
				return RemoveItem(inventory, data, removed, removeTrackingInfo);
			}
			return false;
		}

		public bool RemoveItem(InventoryGameData inventory, IInventoryItemGameData item, int removed, string removeReason, bool ignoreInventoryChange = false)
		{
			return RemoveItem(inventory, item, removed, new Dictionary<string, string> { { "TypeOfUse", removeReason } }, ignoreInventoryChange);
		}

		public bool RemoveItem(InventoryGameData inventory, IInventoryItemGameData item, int removed, Dictionary<string, string> removeTrackingInfo, bool ignoreInventoryChange = false)
		{
			if (item == null)
			{
				return false;
			}
			if (removed < 0)
			{
				removed *= -1;
			}
			if (item.ItemValue - removed < 0)
			{
				return false;
			}
			if (inventory.BalancingData.NameId == "player_inventory" && item.ItemValue > 0)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.SaveAdd("ItemName", item.ItemBalancing.NameId);
				dictionary.SaveAdd("TypeOfItem", item.ItemBalancing.ItemType.ToString());
				dictionary.SaveAdd("Amount", removed.ToString());
				foreach (KeyValuePair<string, string> item2 in removeTrackingInfo)
				{
					dictionary.SaveAdd(item2.Key, removeTrackingInfo[item2.Key]);
				}
				dictionary.SaveAdd("ItemQuality", item.ItemData.Quality.ToString());
				dictionary.SaveAdd("ItemLevel", item.ItemData.Level.ToString());
				ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
				DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("InventoryUsed", dictionary);
			}
			int itemValue = item.ItemValue;
			if (OnRemoveAction.ContainsKey(item.ItemBalancing.ItemType.ToString().ToLower()))
			{
				item.ItemValue = OnRemoveAction[item.ItemBalancing.ItemType.ToString().ToLower()](inventory, item, removed, removeTrackingInfo);
			}
			item.ItemValue = OnRemoveAction["OnEachRemove"](inventory, item, itemValue - item.ItemValue, removeTrackingInfo);
			if (!ignoreInventoryChange)
			{
				item.RaiseItemDataChanged(itemValue - item.ItemValue);
				inventory.RaiseInventoryChanged(item.ItemBalancing.ItemType, item);
			}
			if (IsNonStackableItem(item) && item.ItemValue == 0)
			{
				inventory.RemoveItemFromInventory(item);
			}
			return true;
		}

		private static bool IsNonStackableItem(IInventoryItemGameData item)
		{
			return item.ItemBalancing.ItemType == InventoryItemType.EventBattleItem || item.ItemBalancing.ItemType == InventoryItemType.EventCollectible || item.ItemBalancing.ItemType == InventoryItemType.EventCampaignItem || item.ItemBalancing.ItemType == InventoryItemType.EventBossItem || item.ItemBalancing.ItemType == InventoryItemType.Class || item.ItemBalancing.ItemType == InventoryItemType.MainHandEquipment || item.ItemBalancing.ItemType == InventoryItemType.OffHandEquipment || item.ItemBalancing.ItemType == InventoryItemType.CraftingRecipes || item.ItemBalancing.ItemType == InventoryItemType.Banner || item.ItemBalancing.ItemType == InventoryItemType.BannerEmblem || item.ItemBalancing.ItemType == InventoryItemType.BannerTip;
		}

		private void InitOnRemoveMethods()
		{
			AddOnRemoveAction(InventoryItemType.Consumable.ToString().ToLower(), OnRemoveResources);
			AddOnRemoveAction(InventoryItemType.CraftingRecipes.ToString().ToLower(), OnRemoveResources);
			AddOnRemoveAction(InventoryItemType.Ingredients.ToString().ToLower(), OnRemoveResources);
			AddOnRemoveAction(InventoryItemType.PlayerStats.ToString().ToLower(), OnRemoveResources);
			AddOnRemoveAction(InventoryItemType.Resources.ToString().ToLower(), OnRemoveResources);
			AddOnRemoveAction(InventoryItemType.Story.ToString().ToLower(), OnRemoveResources);
			AddOnRemoveAction(InventoryItemType.Trophy.ToString().ToLower(), OnRemoveResources);
			AddOnRemoveAction(InventoryItemType.Premium.ToString().ToLower(), OnRemoveResources);
			AddOnRemoveAction(InventoryItemType.OffHandEquipment.ToString().ToLower(), OnRemoveEquipment);
			AddOnRemoveAction(InventoryItemType.MainHandEquipment.ToString().ToLower(), OnRemoveEquipment);
			AddOnRemoveAction(InventoryItemType.Class.ToString().ToLower(), OnRemoveClass);
			AddOnRemoveAction(InventoryItemType.Skin.ToString().ToLower(), OnRemoveClass);
			AddOnRemoveAction(InventoryItemType.Points.ToString().ToLower(), OnRemoveResources);
			AddOnRemoveAction(InventoryItemType.Banner.ToString().ToLower(), OnRemoveBannerItems);
			AddOnRemoveAction(InventoryItemType.BannerEmblem.ToString().ToLower(), OnRemoveBannerItems);
			AddOnRemoveAction(InventoryItemType.BannerTip.ToString().ToLower(), OnRemoveBannerItems);
			AddOnRemoveAction(InventoryItemType.CollectionComponent.ToString().ToLower(), OnRemoveResources);
			AddOnRemoveAction("OnEachRemove", OnEachRemove);
		}

		private int OnRemoveResources(InventoryGameData inventory, IInventoryItemGameData data, int removed, Dictionary<string, string> reason)
		{
			return Mathf.Max(0, data.ItemValue - removed);
		}

		private int OnRemoveClass(InventoryGameData inventory, IInventoryItemGameData data, int removed, Dictionary<string, string> reason)
		{
			return Mathf.Max(0, data.ItemValue - removed);
		}

		private int OnRemoveEquipment(InventoryGameData inventory, IInventoryItemGameData data, int removed, Dictionary<string, string> reason)
		{
			return Mathf.Max(0, data.ItemValue - removed);
		}

		private int OnRemoveBannerItems(InventoryGameData inventory, IInventoryItemGameData data, int removed, Dictionary<string, string> reason)
		{
			return Mathf.Max(0, data.ItemValue - removed);
		}

		private void AddOnRemoveAction(string itemName, Func<InventoryGameData, IInventoryItemGameData, int, Dictionary<string, string>, int> func)
		{
			if (OnRemoveAction.ContainsKey(itemName))
			{
				OnRemoveAction.Remove(itemName);
			}
			OnRemoveAction.Add(itemName, func);
		}

		public int GetGlobalUpgradeLevel(InventoryGameData inventory, string upgradeName)
		{
			return 0;
		}

		public void AddUpgradeLevel(InventoryGameData inventory, string upgrade, int nextLevel)
		{
		}

		public IInventoryItemData GenerateNewInventoryItem(InventoryGameData inventory, int level, int quality, string nameId, int value)
		{
			IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(nameId);
			switch (balancingData.ItemType)
			{
			case InventoryItemType.Consumable:
				return GenerateConsumable(level, quality, nameId, value);
			case InventoryItemType.Skin:
				return GenerateSkinItem(level, quality, nameId, value);
			case InventoryItemType.Resources:
			case InventoryItemType.Ingredients:
				return GenerateCraftingItem(level, quality, nameId, value);
			case InventoryItemType.MainHandEquipment:
			case InventoryItemType.OffHandEquipment:
				return GenerateEquipment(level, quality, nameId, value);
			case InventoryItemType.CraftingRecipes:
				return GenerateCraftingRecipe(level, quality, nameId, value);
			case InventoryItemType.EventBattleItem:
			case InventoryItemType.EventCollectible:
			case InventoryItemType.EventCampaignItem:
			case InventoryItemType.EventBossItem:
				return GenerateEventItem(level, quality, nameId, value);
			case InventoryItemType.BannerTip:
			case InventoryItemType.Banner:
			case InventoryItemType.BannerEmblem:
				return GenerateBannerItem(level, quality, nameId, value);
			case InventoryItemType.Mastery:
				return GenerateMasteryItem(level, quality, nameId, value);
			case InventoryItemType.Premium:
			case InventoryItemType.Story:
			case InventoryItemType.PlayerToken:
			case InventoryItemType.Points:
			case InventoryItemType.CollectionComponent:
			case InventoryItemType.Trophy:
				return GenerateBasicItem(level, quality, nameId, value);
			default:
				return GenerateBasicItem(level, quality, nameId, value);
			}
		}

		private BannerItemData GenerateBannerItem(int level, int quality, string nameId, int value, EquipmentSource source = EquipmentSource.Loot)
		{
			BannerItemData bannerItemData = new BannerItemData();
			bannerItemData.Level = level;
			bannerItemData.NameId = nameId;
			bannerItemData.Quality = quality;
			bannerItemData.Value = value;
			bannerItemData.IsNew = true;
			bannerItemData.ItemSource = source;
			return bannerItemData;
		}

		private EventItemData GenerateEventItem(int level, int quality, string nameId, int value)
		{
			EventItemData eventItemData = new EventItemData();
			eventItemData.Level = level;
			eventItemData.NameId = nameId;
			eventItemData.Value = value;
			eventItemData.IsNew = true;
			return eventItemData;
		}

		private MasteryItemData GenerateMasteryItem(int level, int quality, string nameId, int value)
		{
			MasteryItemData masteryItemData = new MasteryItemData();
			masteryItemData.Level = level;
			masteryItemData.NameId = nameId;
			masteryItemData.Value = value;
			masteryItemData.IsNew = true;
			return masteryItemData;
		}

		private BasicItemData GenerateBasicItem(int level, int quality, string nameId, int value)
		{
			BasicItemData basicItemData = new BasicItemData();
			basicItemData.Level = level;
			basicItemData.NameId = nameId;
			basicItemData.Value = value;
			basicItemData.IsNew = true;
			return basicItemData;
		}

		private EquipmentData GenerateEquipment(int level, int quality, string nameId, int value, EquipmentSource source = EquipmentSource.Loot)
		{
			EquipmentData equipmentData = new EquipmentData();
			equipmentData.Level = level;
			equipmentData.NameId = nameId;
			equipmentData.Value = value;
			equipmentData.Quality = quality;
			equipmentData.IsNew = true;
			equipmentData.ItemSource = source;
			return equipmentData;
		}

		private CraftingItemData GenerateCraftingItem(int level, int quality, string nameId, int value)
		{
			CraftingItemData craftingItemData = new CraftingItemData();
			craftingItemData.Level = level;
			craftingItemData.NameId = nameId;
			craftingItemData.Value = value;
			craftingItemData.IsNew = true;
			return craftingItemData;
		}

		private ConsumableItemData GenerateConsumable(int level, int quality, string nameId, int value)
		{
			ConsumableItemData consumableItemData = new ConsumableItemData();
			consumableItemData.Level = level;
			consumableItemData.NameId = nameId;
			consumableItemData.Value = value;
			consumableItemData.IsNew = true;
			return consumableItemData;
		}

		private SkinItemData GenerateSkinItem(int level, int quality, string nameId, int value)
		{
			SkinItemData skinItemData = new SkinItemData();
			skinItemData.Level = level;
			skinItemData.NameId = nameId;
			skinItemData.Value = value;
			skinItemData.Quality = quality;
			skinItemData.IsNew = true;
			return skinItemData;
		}

		private ClassItemData GenerateClassItem(int level, int quality, string nameId, int value)
		{
			ClassItemData classItemData = new ClassItemData();
			classItemData.Level = level;
			classItemData.NameId = nameId;
			classItemData.Value = value;
			classItemData.Quality = quality;
			classItemData.IsNew = true;
			return classItemData;
		}

		private CraftingRecipeData GenerateCraftingRecipe(int level, int quality, string nameId, int value)
		{
			CraftingRecipeData craftingRecipeData = new CraftingRecipeData();
			craftingRecipeData.Level = level;
			craftingRecipeData.NameId = nameId;
			craftingRecipeData.Value = value;
			craftingRecipeData.Quality = quality;
			craftingRecipeData.IsNew = true;
			return craftingRecipeData;
		}

		public IInventoryItemGameData GenerateNewInventoryItemGameData(IInventoryItemData item)
		{
			return GenerateNewInventoryItemGameData(null, item.Level, item.Quality, item.NameId, item.Value);
		}

		public IInventoryItemGameData GenerateNewInventoryItemGameData(InventoryGameData inventory, IInventoryItemData item)
		{
			return GenerateNewInventoryItemGameData(inventory, item.Level, item.Quality, item.NameId, item.Value);
		}

		public IInventoryItemGameData GenerateNewInventoryItemGameData(int level, int quality, string nameId, int value)
		{
			return GenerateNewInventoryItemGameData(null, level, quality, nameId, value);
		}

		public IInventoryItemGameData GenerateNewInventoryItemGameData(InventoryGameData inventory, int level, int quality, string nameId, int value, EquipmentSource source = EquipmentSource.Loot)
		{
			IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(nameId);
			if (balancingData == null)
			{
				DebugLog.Error(GetType(), "GenerateNewInventoryItemGameData: Item Balancing is missing: " + nameId);
			}
			switch (balancingData.ItemType)
			{
			case InventoryItemType.Consumable:
				return new ConsumableItemGameData(GenerateConsumable(level, quality, nameId, value));
			case InventoryItemType.Resources:
			case InventoryItemType.Ingredients:
				return new CraftingItemGameData(GenerateCraftingItem(level, quality, nameId, value));
			case InventoryItemType.MainHandEquipment:
			case InventoryItemType.OffHandEquipment:
				return GenerateAndSetNewEquipmentGameData(level, quality, nameId, value, source);
			case InventoryItemType.Class:
				return new ClassItemGameData(GenerateClassItem(level, quality, nameId, value));
			case InventoryItemType.Skin:
				return new SkinItemGameData(GenerateSkinItem(level, quality, nameId, value));
			case InventoryItemType.CraftingRecipes:
				return new CraftingRecipeGameData(GenerateCraftingRecipe(level, quality, nameId, value));
			case InventoryItemType.EventBattleItem:
			case InventoryItemType.EventCollectible:
			case InventoryItemType.EventCampaignItem:
			case InventoryItemType.EventBossItem:
				return new EventItemGameData(GenerateEventItem(level, quality, nameId, value));
			case InventoryItemType.BannerTip:
			case InventoryItemType.Banner:
			case InventoryItemType.BannerEmblem:
				return new BannerItemGameData(GenerateBannerItem(level, quality, nameId, value, source));
			case InventoryItemType.Mastery:
				return new MasteryItemGameData(GenerateMasteryItem(level, quality, nameId, value));
			default:
				return new BasicItemGameData(GenerateBasicItem(level, quality, nameId, value));
			}
		}

		private IInventoryItemGameData GenerateAndSetNewEquipmentGameData(int level, int quality, string nameId, int value, EquipmentSource source)
		{
			if (m_craftingService == null)
			{
				DebugLog.Error("Not every needed Service is implemented!");
				return null;
			}
			EquipmentGameData equipmentGameData = new EquipmentGameData(GenerateEquipment(level, quality, nameId, value, source));
			equipmentGameData.Data.ScrapLoot = m_craftingService.GenerateScrapLootOnNewEquipment(level, (!equipmentGameData.IsSetItem) ? source : EquipmentSource.SetItem, equipmentGameData.ItemBalancing.NameId, equipmentGameData.ItemBalancing.ItemType);
			return equipmentGameData;
		}

		public IInventoryItemGameData ReinitNewInventoryItemGameData(InventoryGameData inventory, IInventoryItemData item)
		{
			IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(item.NameId);
			if (balancingData == null)
			{
				DebugLog.Error("[InventoryOperationServiceRealImpl] ReinitNewInventoryItemGameData balancing==null Name: " + item.NameId);
				return null;
			}
			switch (balancingData.ItemType)
			{
			case InventoryItemType.Consumable:
				return new ConsumableItemGameData((ConsumableItemData)item);
			case InventoryItemType.Resources:
			case InventoryItemType.Ingredients:
				return new CraftingItemGameData((CraftingItemData)item);
			case InventoryItemType.MainHandEquipment:
			case InventoryItemType.OffHandEquipment:
				return new EquipmentGameData((EquipmentData)item);
			case InventoryItemType.Class:
				return new ClassItemGameData((ClassItemData)item);
			case InventoryItemType.CraftingRecipes:
				return new CraftingRecipeGameData((CraftingRecipeData)item);
			case InventoryItemType.EventBattleItem:
			case InventoryItemType.EventCollectible:
			case InventoryItemType.EventCampaignItem:
			case InventoryItemType.EventBossItem:
				return new EventItemGameData((EventItemData)item);
			case InventoryItemType.BannerTip:
			case InventoryItemType.Banner:
			case InventoryItemType.BannerEmblem:
				return new BannerItemGameData((BannerItemData)item);
			case InventoryItemType.Mastery:
				return new MasteryItemGameData((MasteryItemData)item);
			case InventoryItemType.Skin:
				return new SkinItemGameData((SkinItemData)item);
			default:
				return new BasicItemGameData((BasicItemData)item);
			}
		}

		public Dictionary<string, LootInfoData> GetDelayedReward(InventoryGameData inventory)
		{
			if (inventory.Data.DelayedRewards == null || inventory.Data.DelayedRewards.Count == 0)
			{
				return null;
			}
			return DIContainerLogic.GetLootOperationService().GenerateLoot(inventory.Data.DelayedRewards, 1);
		}

		public bool RewardDelayedRewards(InventoryGameData inventory)
		{
			if (inventory.Data.DelayedRewards == null || inventory.Data.DelayedRewards.Count == 0)
			{
				return false;
			}
			DIContainerLogic.GetLootOperationService().RewardLoot(inventory, 1, DIContainerLogic.GetLootOperationService().GenerateLoot(inventory.Data.DelayedRewards, 1), "delayed_reward");
			inventory.Data.DelayedRewards.Clear();
			if (DIContainerInfrastructure.GetCurrentPlayer() != null)
			{
				DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			}
			return true;
		}

		public bool EquipBirdWithItem(List<IInventoryItemGameData> newContent, InventoryItemType category, InventoryGameData targetInventory)
		{
			List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
			bool flag = false;
			for (int i = 0; i < newContent.Count; i++)
			{
				IInventoryItemGameData inventoryItemGameData = newContent[i];
				if (inventoryItemGameData != null && inventoryItemGameData.ItemBalancing.ItemType == category)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return false;
			}
			if (targetInventory.Items.ContainsKey(category))
			{
				list = targetInventory.Items[category];
			}
			bool flag2 = true;
			for (int num = list.Count - 1; num >= 0; num--)
			{
				flag2 = flag2 && targetInventory.RemoveItemFromInventory(list[num]);
			}
			for (int j = 0; j < newContent.Count; j++)
			{
				IInventoryItemGameData inventoryItemGameData2 = newContent[j];
				targetInventory.AddNewItemToInventory(inventoryItemGameData2, true);
				targetInventory.RaiseInventoryChanged(category, inventoryItemGameData2);
			}
			return flag2;
		}

		internal void ReportDailyInventoryBalance()
		{
			if (DIContainerInfrastructure.GetCurrentPlayer() == null || DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData == null || DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items == null)
			{
				return;
			}
			DateTime dateTimeFromTimestamp = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(DIContainerInfrastructure.GetCurrentPlayer().Data.LastInventoryBalanceTime);
			if (!(DIContainerLogic.GetTimingService().TimeSince(dateTimeFromTimestamp).TotalHours > 24.0))
			{
				return;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			try
			{
				for (int i = 0; i < DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.Resources].Count; i++)
				{
					IInventoryItemGameData inventoryItemGameData = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.Resources][i];
					dictionary.Add(inventoryItemGameData.ItemBalancing.NameId, inventoryItemGameData.ItemValue.ToString());
				}
			}
			catch
			{
			}
			try
			{
				foreach (IInventoryItemGameData item in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.Story].Where((IInventoryItemGameData s) => s.ItemBalancing.NameId == "daily_post_card"))
				{
					dictionary.Add(item.ItemBalancing.NameId, item.ItemValue.ToString());
				}
			}
			catch
			{
			}
			try
			{
				for (int j = 0; j < DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.PlayerStats].Count; j++)
				{
					IInventoryItemGameData inventoryItemGameData2 = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.PlayerStats][j];
					dictionary.Add(inventoryItemGameData2.ItemBalancing.NameId, inventoryItemGameData2.ItemValue.ToString());
				}
			}
			catch
			{
			}
			ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
			ABHAnalyticsHelper.AddFriendsCountToTracking(dictionary);
			DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("InventoryBalance", dictionary);
			DIContainerInfrastructure.GetCurrentPlayer().Data.LastInventoryBalanceTime = DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp();
		}

		public void FixInventory(InventoryGameData inventory)
		{
			foreach (List<IInventoryItemGameData> value in inventory.Items.Values)
			{
				for (int i = 0; i < value.Count; i++)
				{
					try
					{
						IInventoryItemGameData inventoryItemGameData = value[i];
						if (inventoryItemGameData.ItemValue < -1000 || (float)inventoryItemGameData.ItemValue > (float)DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").LimeGreenValue * 20f)
						{
							DebugLog.Error("Erroneous Item value: " + inventoryItemGameData.ItemValue + " Item Name: " + inventoryItemGameData.Name + " Item Quality: " + inventoryItemGameData.ItemData.Quality + " Item Level: " + inventoryItemGameData.ItemData.Level);
							inventoryItemGameData.ResetValue();
							if (inventoryItemGameData is EquipmentGameData)
							{
								EquipmentGameData equipmentGameData = inventoryItemGameData as EquipmentGameData;
								DebugLog.Error("Erroneous Equipment: " + inventoryItemGameData.ItemValue + " Item Name: " + inventoryItemGameData.Name + " Item Quality: " + inventoryItemGameData.ItemData.Quality + " Item Level: " + inventoryItemGameData.ItemData.Level + " Item Source: " + equipmentGameData.Data.ItemSource.ToString() + " Item ScrapLoot: " + equipmentGameData.Data.ScrapLoot.FirstOrDefault().Key.ToString());
							}
						}
						EquipmentGameData equipmentGameData2 = inventoryItemGameData as EquipmentGameData;
						if (equipmentGameData2 != null && equipmentGameData2.IsSetItem)
						{
							Dictionary<string, int> scrapLoot = equipmentGameData2.Data.ScrapLoot;
							if (scrapLoot != null)
							{
								for (int j = 0; j < scrapLoot.Count; j++)
								{
									KeyValuePair<string, int> keyValuePair = scrapLoot.ElementAt(j);
									if (keyValuePair.Key == "friendship_essence")
									{
										equipmentGameData2.Data.ScrapLoot.Add("shard", keyValuePair.Value);
										equipmentGameData2.Data.ScrapLoot.Remove("friendship_essence");
									}
								}
							}
						}
						ClassItemGameData classItemGameData = inventoryItemGameData as ClassItemGameData;
						if (classItemGameData != null && classItemGameData.Data.Level > classItemGameData.MasteryMaxRank())
						{
							classItemGameData.Data.Level = 1;
						}
						ConsumableItemGameData consumableItemGameData = inventoryItemGameData as ConsumableItemGameData;
						if (consumableItemGameData != null && consumableItemGameData.Data.Level > consumableItemGameData.MaxRecipeRank())
						{
							consumableItemGameData.Data.Level = 1;
						}
						CraftingRecipeGameData craftingRecipeGameData = inventoryItemGameData as CraftingRecipeGameData;
						if (craftingRecipeGameData != null && craftingRecipeGameData.BalancingData.RecipeCategoryType == InventoryItemType.Consumable && craftingRecipeGameData.Data.Level > craftingRecipeGameData.MaxRecipeRank())
						{
							craftingRecipeGameData.Data.Level = 1;
						}
					}
					catch
					{
					}
				}
			}
		}

		internal void FixInventoryOfBirds(PlayerGameData player)
		{
			for (int j = 0; j < player.Birds.Count; j++)
			{
				BirdGameData bird = player.Birds[j];
				FixInventory(bird.InventoryGameData);
				if (bird.MainHandItem == null || CheckForInvalidInventoryOfBirds(player, bird.Data.NameId))
				{
					EquipBirdWithItem(new List<IInventoryItemGameData> { player.InventoryGameData.Items[InventoryItemType.MainHandEquipment].FirstOrDefault((IInventoryItemGameData i) => i.IsValidForBird(bird)) }, InventoryItemType.MainHandEquipment, bird.InventoryGameData);
				}
				if (bird.OffHandItem == null || CheckForInvalidInventoryOfBirds(player, bird.Data.NameId))
				{
					EquipBirdWithItem(new List<IInventoryItemGameData> { player.InventoryGameData.Items[InventoryItemType.OffHandEquipment].FirstOrDefault((IInventoryItemGameData i) => i.IsValidForBird(bird)) }, InventoryItemType.OffHandEquipment, bird.InventoryGameData);
				}
				if (bird.ClassItem == null || !bird.ClassItem.IsValidForBird(bird) || CheckForInvalidInventoryOfBirds(player, bird.Data.NameId))
				{
					EquipBirdWithItem(new List<IInventoryItemGameData> { player.InventoryGameData.Items[InventoryItemType.Class].FirstOrDefault((IInventoryItemGameData i) => i.IsValidForBird(bird)) }, InventoryItemType.Class, bird.InventoryGameData);
				}
			}
		}

		public void FixStampCount(PlayerGameData player)
		{
			int itemValue = GetItemValue(player.InventoryGameData, "daily_post_card");
			int num = Mathf.FloorToInt((float)((DateTime.Now - new DateTime(2014, 6, 11)).TotalDays / 7.0));
			DebugLog.Log("Weeks since Launch: " + num);
			if (itemValue > num + extraDays)
			{
				SetItem(player.InventoryGameData, 1, 1, "daily_post_card", num + extraDays, "StampcardOverload");
			}
		}

		internal void RemoveClassUpgradesOutsideBalancingData(PlayerData player)
		{
			for (int num = player.Inventory.SkinItems.Count - 1; num > -1; num--)
			{
				ClassSkinBalancingData balancing;
				if (!DIContainerBalancing.Service.TryGetBalancingData<ClassSkinBalancingData>(player.Inventory.SkinItems[num].NameId, out balancing))
				{
					player.Inventory.SkinItems.RemoveAt(num);
					if (!m_savePreviousPlayer)
					{
						m_savePreviousPlayer = true;
						m_previousPlayerReason = "NO_CLASS_UPGRADES_FROM_BALANCING_DATA";
					}
				}
			}
			for (int i = 0; i < player.Birds.Count; i++)
			{
				for (int num2 = player.Birds[i].Inventory.SkinItems.Count - 1; num2 > -1; num2--)
				{
					ClassSkinBalancingData balancing2;
					if (!DIContainerBalancing.Service.TryGetBalancingData<ClassSkinBalancingData>(player.Birds[i].Inventory.SkinItems[num2].NameId, out balancing2))
					{
						player.Birds[i].Inventory.SkinItems.RemoveAt(num2);
						if (!m_savePreviousPlayer)
						{
							m_savePreviousPlayer = true;
							m_previousPlayerReason = "NO_CLASS_UPGRADES_FROM_BALANCING_DATA";
						}
					}
				}
			}
			for (int j = 0; j < player.Inventory.ClassItems.Count; j++)
			{
				string nameId = player.Inventory.ClassItems[j].NameId;
				ClassSkinBalancingData balancing3;
				if (!DIContainerBalancing.Service.TryGetBalancingData<ClassSkinBalancingData>(player.EquippedSkins[nameId], out balancing3) && player.EquippedSkins.ContainsKey(nameId))
				{
					player.EquippedSkins.Remove(player.Inventory.ClassItems[j].NameId);
					if (!m_savePreviousPlayer)
					{
						m_savePreviousPlayer = true;
						m_previousPlayerReason = "NO_CLASS_UPGRADES_FROM_BALANCING_DATA";
					}
				}
			}
			savePreviousPlayer();
		}

		internal bool CheckForInvalidInventoryOfBirds(PlayerGameData player, string nameId)
		{
			for (int i = 0; i < player.Birds.Count; i++)
			{
				for (int j = 0; j < player.Birds[i].InventoryGameData.Data.ClassItems.Count; j++)
				{
					string restrictedBirdId = DIContainerBalancing.Service.GetBalancingData<ClassItemBalancingData>(player.Birds[i].InventoryGameData.Data.ClassItems[j].NameId).RestrictedBirdId;
					if (player.Birds[i].BalancingData.NameId == nameId && player.Birds[i].BalancingData.NameId != restrictedBirdId)
					{
						return true;
					}
				}
				for (int k = 0; k < player.Birds[i].InventoryGameData.Data.OffHandItems.Count; k++)
				{
					string restrictedBirdId2 = DIContainerBalancing.Service.GetBalancingData<EquipmentBalancingData>(player.Birds[i].InventoryGameData.Data.OffHandItems[k].NameId).RestrictedBirdId;
					if (player.Birds[i].BalancingData.NameId == nameId && player.Birds[i].BalancingData.NameId != restrictedBirdId2)
					{
						return true;
					}
				}
				for (int l = 0; l < player.Birds[i].InventoryGameData.Data.MainHandItems.Count; l++)
				{
					string restrictedBirdId3 = DIContainerBalancing.Service.GetBalancingData<EquipmentBalancingData>(player.Birds[i].InventoryGameData.Data.MainHandItems[l].NameId).RestrictedBirdId;
					if (player.Birds[i].BalancingData.NameId == nameId && player.Birds[i].BalancingData.NameId != restrictedBirdId3)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal void RemoveEquipmentOutsideBalancingData(PlayerData player)
		{
			for (int num = player.Inventory.MainHandItems.Count - 1; num > -1; num--)
			{
				EquipmentBalancingData balancing;
				if (!DIContainerBalancing.Service.TryGetBalancingData<EquipmentBalancingData>(player.Inventory.MainHandItems[num].NameId, out balancing))
				{
					player.Inventory.MainHandItems.RemoveAt(num);
					if (!m_savePreviousPlayer)
					{
						m_savePreviousPlayer = true;
						m_previousPlayerReason = "NO_MAIN_HAND_FROM_BALANCING_DATA";
					}
				}
			}
			for (int num2 = player.Inventory.OffHandItems.Count - 1; num2 > -1; num2--)
			{
				EquipmentBalancingData balancing2;
				if (!DIContainerBalancing.Service.TryGetBalancingData<EquipmentBalancingData>(player.Inventory.OffHandItems[num2].NameId, out balancing2))
				{
					player.Inventory.MainHandItems.RemoveAt(num2);
					if (!m_savePreviousPlayer)
					{
						m_savePreviousPlayer = true;
						m_previousPlayerReason = "NO_OFFHAND_EQUIPMENT_FROM_BALANCING_DATA";
					}
				}
			}
			for (int i = 0; i < player.Birds.Count; i++)
			{
				for (int num3 = player.Birds[i].Inventory.MainHandItems.Count - 1; num3 > -1; num3--)
				{
					EquipmentBalancingData balancing3;
					if (!DIContainerBalancing.Service.TryGetBalancingData<EquipmentBalancingData>(player.Birds[i].Inventory.MainHandItems[num3].NameId, out balancing3))
					{
						player.Birds[i].Inventory.MainHandItems.RemoveAt(num3);
						if (!m_savePreviousPlayer)
						{
							m_savePreviousPlayer = true;
							m_previousPlayerReason = "NO_MAIN_HAND_FROM_BALANCING_DATA";
						}
					}
				}
				for (int num4 = player.Birds[i].Inventory.OffHandItems.Count - 1; num4 > -1; num4--)
				{
					EquipmentBalancingData balancing4;
					if (!DIContainerBalancing.Service.TryGetBalancingData<EquipmentBalancingData>(player.Birds[i].Inventory.OffHandItems[num4].NameId, out balancing4))
					{
						player.Birds[i].Inventory.OffHandItems.RemoveAt(num4);
						if (!m_savePreviousPlayer)
						{
							m_savePreviousPlayer = true;
							m_previousPlayerReason = "NO_OFFHAND_FROM_BALANCING_DATA";
						}
					}
				}
			}
			savePreviousPlayer();
		}

		internal void RemoveCraftingItemsOutsideBalancingData(PlayerData player)
		{
			for (int num = player.Inventory.CraftingResourceItems.Count - 1; num > -1; num--)
			{
				CraftingItemBalancingData balancing;
				if (!DIContainerBalancing.Service.TryGetBalancingData<CraftingItemBalancingData>(player.Inventory.CraftingResourceItems[num].NameId, out balancing))
				{
					player.Inventory.CraftingResourceItems.RemoveAt(num);
					if (!m_savePreviousPlayer)
					{
						m_savePreviousPlayer = true;
						m_previousPlayerReason = "NO_RESOURCES_FROM_BALANCING_DATA";
					}
				}
			}
			for (int num2 = player.Inventory.CraftingResourceItems.Count - 1; num2 > -1; num2--)
			{
				CraftingItemBalancingData balancing2;
				if (!DIContainerBalancing.Service.TryGetBalancingData<CraftingItemBalancingData>(player.Inventory.CraftingIngredientItems[num2].NameId, out balancing2))
				{
					player.Inventory.CraftingIngredientItems.RemoveAt(num2);
					if (!m_savePreviousPlayer)
					{
						m_savePreviousPlayer = true;
						m_previousPlayerReason = "NO_INGREDIENTS_FROM_BALANCING_DATA";
					}
				}
			}
			savePreviousPlayer();
		}

		private void savePreviousPlayer()
		{
			if (DIContainerConfig.GetConstants() == null)
			{
				Debug.LogError("Constants is null!");
			}
			else if (DIContainerInfrastructure.GetProfileMgr() == null)
			{
				Debug.LogError("Profile mgr is null!");
			}
			else if (DIContainerInfrastructure.GetProfileMgr().CurrentProfile == null)
			{
				Debug.LogError("Current profile is null!");
			}
			else if (DIContainerInfrastructure.GetPlayerPrefsService() == null)
			{
				Debug.LogError("Player prefs service is null!");
			}
			else if (!m_previousPlayerSaved && m_savePreviousPlayer)
			{
				string @string = DIContainerInfrastructure.GetPlayerPrefsService().GetString(DIContainerConfig.GetConstants().ProfileKey);
				DIContainerInfrastructure.GetPlayerPrefsService().SetString(DIContainerConfig.GetConstants().ProfileKey + "_" + DIContainerInfrastructure.GetProfileMgr().CurrentProfile.ClientVersion, @string);
				if (ContentLoader.Instance != null)
				{
					ContentLoader.Instance.SubmitErrorReportCoroutine(string.Concat(new object[6]
					{
						"Reason: ",
						m_previousPlayerReason,
						"\r\nClient version: ",
						DIContainerInfrastructure.GetProfileMgr().CurrentProfile.ClientVersion,
						"\r\nPrevious player: ",
						@string
					}));
				}
				m_previousPlayerSaved = true;
				m_savePreviousPlayer = false;
				DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			}
		}
	}
}

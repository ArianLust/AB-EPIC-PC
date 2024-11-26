using System;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using ABH.Services.Logic.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.Services.Logic
{
	public class RequirementOperationServiceRealImpl : IRequirementOperationService
	{
		private Dictionary<Type, Dictionary<RequirementType, Func<object, Requirement, bool>>> CheckFunctionsByRequirement = new Dictionary<Type, Dictionary<RequirementType, Func<object, Requirement, bool>>>();

		public void InitializeRequirementOperations()
		{
			CheckFunctionsByRequirement.Clear();
			Dictionary<RequirementType, Func<object, Requirement, bool>> dictionary = new Dictionary<RequirementType, Func<object, Requirement, bool>>();
			CheckFunctionsByRequirement.Add(typeof(PlayerGameData), dictionary);
			dictionary.Add(RequirementType.Level, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData26 = o as PlayerGameData;
				return (r.NameId == "max") ? ((float)playerGameData26.Data.Level < r.Value) : ((float)playerGameData26.Data.Level >= r.Value);
			});
			dictionary.Add(RequirementType.HaveMasteryFactor, (object o, Requirement r) => EvaluateMasteryFactorRequirement(o, r));
			dictionary.Add(RequirementType.NotHaveMasteryFactor, (object o, Requirement r) => !EvaluateMasteryFactorRequirement(o, r));
			dictionary.Add(RequirementType.UsedFriends, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData25 = o as PlayerGameData;
				List<string> value = new List<string>();
				return playerGameData25.Data.SocialEnvironment.FriendShipGateUnlocks.TryGetValue(r.NameId, out value) && (float)value.Count >= r.Value;
			});
			dictionary.Add(RequirementType.NotUseBirdInBattle, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData24 = o as PlayerGameData;
				for (int j = 0; j < playerGameData24.Data.SelectedBirdIndices.Count; j++)
				{
					string nameId2 = playerGameData24.Birds[playerGameData24.Data.SelectedBirdIndices[j]].BalancingData.NameId;
					if (nameId2 == r.NameId)
					{
						return false;
					}
				}
				return true;
			});
			dictionary.Add(RequirementType.UseBirdInBattle, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData23 = o as PlayerGameData;
				for (int i = 0; i < playerGameData23.Data.SelectedBirdIndices.Count; i++)
				{
					string nameId = playerGameData23.Birds[playerGameData23.Data.SelectedBirdIndices[i]].BalancingData.NameId;
					if (nameId == r.NameId)
					{
						return true;
					}
				}
				return false;
			});
			dictionary.Add(RequirementType.LostPvpBattle, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData22 = o as PlayerGameData;
				return playerGameData22.Data.LostAnyPvpBattle;
			});
			dictionary.Add(RequirementType.HaveEventScore, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData21 = o as PlayerGameData;
				return playerGameData21.CurrentEventManagerGameData != null && (float)playerGameData21.CurrentEventManagerGameData.Data.CurrentScore >= r.Value;
			});
			dictionary.Add(RequirementType.HavePassedCycleTime, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData20 = o as PlayerGameData;
				DateTime trustedTime4;
				return r.NameId == "shop" && DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime4) && (double)r.Value < (trustedTime4 - DateTime.MinValue).TotalSeconds % (double)playerGameData20.WorldGameData.BalancingData.ShopFullRefreshCycleTimeInSec;
			});
			dictionary.Add(RequirementType.NotHavePassedCycleTime, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData19 = o as PlayerGameData;
				if (r.NameId == "shop")
				{
					DateTime trustedTime3;
					if (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime3))
					{
						return false;
					}
					uint num2 = (uint)(trustedTime3 - DateTime.MinValue).TotalSeconds;
					uint num3 = num2 % (uint)playerGameData19.WorldGameData.BalancingData.ShopFullRefreshCycleTimeInSec;
					uint num4 = (uint)r.Value;
					return num4 >= num3;
				}
				return false;
			});
			dictionary.Add(RequirementType.HaveCurrentHotpsotState, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData18 = o as PlayerGameData;
				return (playerGameData18.WorldGameData.CurrentHotspotGameData != null && playerGameData18.WorldGameData.CurrentHotspotGameData.Data.UnlockState.ToString().ToLower() == r.NameId.ToLower()) ? true : false;
			});
			dictionary.Add(RequirementType.HaveCurrentChronicleCaveState, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData17 = o as PlayerGameData;
				return (playerGameData17.ChronicleCaveGameData.CurrentHotspotGameData != null && playerGameData17.ChronicleCaveGameData.CurrentHotspotGameData.Data.UnlockState.ToString().ToLower() == r.NameId.ToLower()) ? true : false;
			});
			dictionary.Add(RequirementType.TutorialCompleted, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData16 = o as PlayerGameData;
				return playerGameData16.Data.TutorialTracks != null && playerGameData16.Data.TutorialTracks.ContainsKey(r.NameId) && (float)playerGameData16.Data.TutorialTracks[r.NameId] == r.Value;
			});
			dictionary.Add(RequirementType.HaveEventCampaignHotspotState, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData15 = o as PlayerGameData;
				return (DIContainerLogic.EventSystemService.IsCurrentEventAvailable(playerGameData15) && playerGameData15.CurrentEventManagerGameData.CurrentMiniCampaign != null && playerGameData15.CurrentEventManagerGameData.CurrentMiniCampaign.CurrentHotspotGameData != null && playerGameData15.CurrentEventManagerGameData.CurrentMiniCampaign.CurrentHotspotGameData.Data.UnlockState.ToString().ToLower() == r.NameId.ToLower()) ? true : false;
			});
			dictionary.Add(RequirementType.HaveUnlockedHotpsot, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData14 = o as PlayerGameData;
				return (playerGameData14.WorldGameData.HotspotGameDatas.ContainsKey(r.NameId) && playerGameData14.WorldGameData.HotspotGameDatas[r.NameId].Data.UnlockState >= HotspotUnlockState.ResolvedNew) ? true : false;
			});
			dictionary.Add(RequirementType.NotHaveUnlockedHotpsot, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData13 = o as PlayerGameData;
				return (playerGameData13.WorldGameData.HotspotGameDatas.ContainsKey(r.NameId) && playerGameData13.WorldGameData.HotspotGameDatas[r.NameId].Data.UnlockState < HotspotUnlockState.ResolvedNew) ? true : false;
			});
			dictionary.Add(RequirementType.HaveBird, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData12 = o as PlayerGameData;
				BirdGameData bird = playerGameData12.GetBird(r.NameId);
				return (r.Value == 0f) ? (playerGameData12.GetBird(r.NameId) == null) : (playerGameData12.GetBird(r.NameId) != null);
			});
			dictionary.Add(RequirementType.HaveBirdCount, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData11 = o as PlayerGameData;
				int count = playerGameData11.Birds.Count;
				if (r.NameId.Contains("g"))
				{
					return r.Value > (float)count;
				}
				return r.NameId.Contains("e") ? (Math.Abs(r.Value - (float)count) < 0.5f) : (r.Value <= (float)count);
			});
			dictionary.Add(RequirementType.HaveItem, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData10 = o as PlayerGameData;
				return (float)DIContainerLogic.InventoryService.GetItemValue(playerGameData10.InventoryGameData, r.NameId) >= r.Value;
			});
			dictionary.Add(RequirementType.NotHaveClass, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData9 = o as PlayerGameData;
				return !DIContainerLogic.InventoryService.CheckForItem(playerGameData9.InventoryGameData, r.NameId);
			});
			dictionary.Add(RequirementType.HaveClass, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData8 = o as PlayerGameData;
				return DIContainerLogic.InventoryService.CheckForItem(playerGameData8.InventoryGameData, r.NameId);
			});
			dictionary.Add(RequirementType.NotHaveItem, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData7 = o as PlayerGameData;
				return (float)DIContainerLogic.InventoryService.GetItemValue(playerGameData7.InventoryGameData, r.NameId) < r.Value;
			});
			dictionary.Add(RequirementType.HaveItemWithLevel, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData6 = o as PlayerGameData;
				IInventoryItemGameData data2 = null;
				return DIContainerLogic.InventoryService.TryGetItemGameData(playerGameData6.InventoryGameData, r.NameId, out data2) && (float)data2.ItemData.Level >= r.Value;
			});
			dictionary.Add(RequirementType.NotHaveItemWithLevel, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData5 = o as PlayerGameData;
				IInventoryItemGameData data = null;
				return !DIContainerLogic.InventoryService.TryGetItemGameData(playerGameData5.InventoryGameData, r.NameId, out data) || (float)data.ItemData.Level < r.Value;
			});
			dictionary.Add(RequirementType.PayItem, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData4 = o as PlayerGameData;
				return (float)DIContainerLogic.InventoryService.GetItemValue(playerGameData4.InventoryGameData, r.NameId) >= r.Value;
			});
			DateTime trustedTime2;
			dictionary.Add(RequirementType.IsSpecificWeekday, (object o, Requirement r) => DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime2) && trustedTime2.DayOfWeek == (DayOfWeek)(int)Enum.Parse(typeof(DayOfWeek), r.NameId, true));
			DateTime trustedTime;
			dictionary.Add(RequirementType.IsNotSpecificWeekday, (object o, Requirement r) => DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime) && trustedTime.DayOfWeek != (DayOfWeek)(int)Enum.Parse(typeof(DayOfWeek), r.NameId, true));
			dictionary.Add(RequirementType.HaveLessThan, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData3 = o as PlayerGameData;
				return (float)DIContainerLogic.InventoryService.GetItemValue(playerGameData3.InventoryGameData, r.NameId) <= r.Value;
			});
			dictionary.Add(RequirementType.IsConverted, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData2 = o as PlayerGameData;
				return (r.Value == 1f) ? playerGameData2.Data.IsUserConverted : (!playerGameData2.Data.IsUserConverted);
			});
			dictionary.Add(RequirementType.HaveAllUpgrades, delegate(object o, Requirement r)
			{
				PlayerGameData player2 = o as PlayerGameData;
				return HaveAllUpgrades(r, player2);
			});
			dictionary.Add(RequirementType.NotHaveAllUpgrades, delegate(object o, Requirement r)
			{
				PlayerGameData player = o as PlayerGameData;
				return !HaveAllUpgrades(r, player);
			});
			dictionary.Add(RequirementType.HaveTotalItemsInCollection, delegate(object o, Requirement r)
			{
				PlayerGameData playerGameData = o as PlayerGameData;
				if (playerGameData.Data.CollectiblesPerEvent != null && playerGameData.Data.CollectiblesPerEvent.ContainsKey(r.NameId))
				{
					return (float)playerGameData.Data.CollectiblesPerEvent[r.NameId] >= r.Value;
				}
				if (playerGameData.CurrentEventManagerGameData != null && playerGameData.CurrentEventManagerGameData.Balancing != null && playerGameData.CurrentEventManagerGameData.Balancing.NameId == r.NameId)
				{
					int num = 0;
					foreach (IInventoryItemGameData item in playerGameData.InventoryGameData.Items[InventoryItemType.CollectionComponent])
					{
						if (item.Name != "collection_event_stars")
						{
							num += item.ItemValue;
						}
					}
					return (float)num >= r.Value;
				}
				return false;
			});
			Dictionary<RequirementType, Func<object, Requirement, bool>> dictionary2 = new Dictionary<RequirementType, Func<object, Requirement, bool>>();
			CheckFunctionsByRequirement.Add(typeof(InventoryGameData), dictionary2);
			dictionary2.Add(RequirementType.HaveItem, delegate(object o, Requirement r)
			{
				InventoryGameData inventory5 = o as InventoryGameData;
				return (float)DIContainerLogic.InventoryService.GetItemValue(inventory5, r.NameId) >= r.Value;
			});
			dictionary2.Add(RequirementType.NotHaveItem, delegate(object o, Requirement r)
			{
				InventoryGameData inventory4 = o as InventoryGameData;
				return (float)DIContainerLogic.InventoryService.GetItemValue(inventory4, r.NameId) < r.Value;
			});
			dictionary2.Add(RequirementType.NotHaveClass, delegate(object o, Requirement r)
			{
				InventoryGameData inventory3 = o as InventoryGameData;
				return !DIContainerLogic.InventoryService.CheckForItem(inventory3, r.NameId);
			});
			dictionary2.Add(RequirementType.HaveClass, delegate(object o, Requirement r)
			{
				InventoryGameData inventory2 = o as InventoryGameData;
				return DIContainerLogic.InventoryService.CheckForItem(inventory2, r.NameId);
			});
			dictionary2.Add(RequirementType.PayItem, delegate(object o, Requirement r)
			{
				InventoryGameData inventory = o as InventoryGameData;
				return (float)DIContainerLogic.InventoryService.GetItemValue(inventory, r.NameId) >= r.Value;
			});
		}

		private static bool EvaluateMasteryFactorRequirement(object o, Requirement r)
		{
			PlayerGameData player = o as PlayerGameData;
			if (r.NameId == "maxmastery")
			{
				return (float)GetMaxMasteryValue(player) >= r.Value;
			}
			if (r.NameId == "avg")
			{
				return GetAverageMasteryValue(player) >= (double)r.Value;
			}
			if (r.NameId == "highavg")
			{
				float highAverageMasteryValue = GetHighAverageMasteryValue(player);
				return highAverageMasteryValue >= r.Value;
			}
			double averageHighest5MasteryValue = GetAverageHighest5MasteryValue(player);
			return averageHighest5MasteryValue >= (double)r.Value;
		}

		public static double GetAverageHighest5MasteryValue(PlayerGameData player)
		{
			List<IInventoryItemGameData> list = player.InventoryGameData.Items[InventoryItemType.Class].OrderBy((IInventoryItemGameData c) => c.ItemData.Level).ToList();
			List<IInventoryItemGameData> list2 = new List<IInventoryItemGameData>();
			for (int i = 0; i < list.Count; i++)
			{
				if (i > list.Count - 6)
				{
					list2.Add(list[i]);
				}
			}
			return list2.Average((IInventoryItemGameData c) => c.ItemData.Level);
		}

		public static float GetHighAverageMasteryValue(PlayerGameData player)
		{
			int num = Mathf.FloorToInt((float)player.InventoryGameData.Items[InventoryItemType.Class].Average((IInventoryItemGameData c) => c.ItemData.Level));
			float num2 = 0f;
			for (int i = 0; i < player.InventoryGameData.Items[InventoryItemType.Class].Count; i++)
			{
				IInventoryItemGameData inventoryItemGameData = player.InventoryGameData.Items[InventoryItemType.Class][i];
				num2 += Mathf.Pow(inventoryItemGameData.ItemData.Level - num, 2f);
			}
			num2 = Mathf.Sqrt(num2 / (float)player.InventoryGameData.Items[InventoryItemType.Class].Count);
			return (float)num + num2 / 2f;
		}

		public static double GetAverageMasteryValue(PlayerGameData player)
		{
			return player.InventoryGameData.Items[InventoryItemType.Class].Average((IInventoryItemGameData c) => c.ItemData.Level);
		}

		public static int GetMaxMasteryValue(PlayerGameData player)
		{
			return player.InventoryGameData.Items[InventoryItemType.Class].Max((IInventoryItemGameData c) => c.ItemData.Level);
		}

		private static bool HaveAllUpgrades(Requirement r, PlayerGameData player)
		{
			if (!string.IsNullOrEmpty(r.NameId) && r.NameId.ToLower() == "all")
			{
				for (int i = 0; i < player.InventoryGameData.Items[InventoryItemType.Class].Count; i++)
				{
					IInventoryItemGameData inventoryItemGameData = player.InventoryGameData.Items[InventoryItemType.Class][i];
					if (!((float)inventoryItemGameData.ItemData.Level >= r.Value))
					{
						return false;
					}
				}
				return true;
			}
			List<ClassItemGameData> list = new List<ClassItemGameData>();
			for (int j = 0; j < player.InventoryGameData.Items[InventoryItemType.Class].Count; j++)
			{
				IInventoryItemGameData inventoryItemGameData2 = player.InventoryGameData.Items[InventoryItemType.Class][j];
				list.Add(inventoryItemGameData2 as ClassItemGameData);
			}
			for (int k = 0; k < list.Count; k++)
			{
				ClassItemGameData classItemGameData = list[k];
				if (classItemGameData != null && classItemGameData.BalancingData.RestrictedBirdId == r.NameId && !((float)classItemGameData.ItemData.Level >= r.Value))
				{
					return false;
				}
			}
			return true;
		}

		public bool CheckGenericRequirements(object owner, List<Requirement> requirementsToCheck)
		{
			List<Requirement> failedRequirements = new List<Requirement>();
			return CheckGenericRequirements(owner, requirementsToCheck, out failedRequirements);
		}

		public List<Requirement> AddRequirement(object owner, List<Requirement> requirementsBefore, Requirement requirementToAdd)
		{
			List<Requirement> list = new List<Requirement>(requirementsBefore);
			list.Add(requirementToAdd);
			return list;
		}

		public List<Requirement> GetRequirementDelta(object owner, List<Requirement> requirementsBefore, Requirement requirementToRemove)
		{
			List<Requirement> list = new List<Requirement>();
			for (int i = 0; i < requirementsBefore.Count; i++)
			{
				Requirement requirement = requirementsBefore[i];
				list.Add(new Requirement
				{
					Value = requirement.Value,
					NameId = requirement.NameId,
					RequirementType = requirement.RequirementType
				});
			}
			Requirement requirement2 = list.FirstOrDefault((Requirement r) => r.RequirementType == requirementToRemove.RequirementType && r.NameId == requirementToRemove.NameId);
			if (requirement2 != null)
			{
				GetRequirementValueDelta(owner, requirement2);
			}
			return list;
		}

		public Requirement GetRequirementDelta(object owner, Requirement requirementBefore)
		{
			Requirement requirement = new Requirement();
			requirement.NameId = requirementBefore.NameId;
			requirement.Value = requirementBefore.Value;
			requirement.RequirementType = requirementBefore.RequirementType;
			Requirement requirement2 = requirement;
			if (requirement2 != null)
			{
				GetRequirementValueDelta(owner, requirement2);
			}
			return requirement2;
		}

		private void GetRequirementValueDelta(object owner, Requirement toModify)
		{
			InventoryGameData inventoryGameData = null;
			if (owner is InventoryGameData)
			{
				inventoryGameData = owner as InventoryGameData;
				switch (toModify.RequirementType)
				{
				case RequirementType.HaveItem:
					toModify.Value = Mathf.Max(toModify.Value - (float)DIContainerLogic.InventoryService.GetItemValue(inventoryGameData, toModify.NameId), 0f);
					break;
				case RequirementType.PayItem:
					toModify.Value = Mathf.Max(toModify.Value - (float)DIContainerLogic.InventoryService.GetItemValue(inventoryGameData, toModify.NameId), 0f);
					break;
				}
			}
		}

		public bool CheckGenericRequirements(object owner, List<Requirement> requirementsToCheck, out List<Requirement> failedRequirements)
		{
			failedRequirements = new List<Requirement>();
			if (requirementsToCheck == null)
			{
				return true;
			}
			for (int i = 0; i < requirementsToCheck.Count; i++)
			{
				Requirement requirement = requirementsToCheck[i];
				if (!CheckRequirement(owner, requirement))
				{
					failedRequirements.Add(requirement);
					return false;
				}
			}
			return true;
		}

		public bool CheckFailRequirements(object owner, List<Requirement> failConditionRequirements)
		{
			if (failConditionRequirements == null || failConditionRequirements.Count == 0)
			{
				return false;
			}
			return !CheckGenericRequirements(owner, failConditionRequirements);
		}

		public bool CheckRequirement(object owner, Requirement req)
		{
			Func<object, Requirement, bool> value = null;
			if (CheckFunctionsByRequirement.ContainsKey(owner.GetType()) && CheckFunctionsByRequirement[owner.GetType()].TryGetValue(req.RequirementType, out value))
			{
				return value(owner, req);
			}
			return true;
		}

		public bool ExecuteRequirements(object owner, List<Requirement> requirements, string removeReason)
		{
			bool flag = true;
			if (requirements != null)
			{
				for (int i = 0; i < requirements.Count; i++)
				{
					Requirement requirement = requirements[i];
					if (requirement.RequirementType == RequirementType.PayItem && owner is InventoryGameData)
					{
						flag &= DIContainerLogic.InventoryService.RemoveItem(owner as InventoryGameData, requirement.NameId, (int)requirement.Value, removeReason);
						if (flag && requirement.NameId == "lucky_coin")
						{
							DIContainerInfrastructure.GetProfileMgr().CurrentProfile.HardCurrencySpent += (int)requirement.Value;
						}
					}
				}
			}
			return flag;
		}

		public bool ExecuteRequirements(object owner, List<Requirement> requirements, Dictionary<string, string> trackingDictionary)
		{
			bool flag = true;
			if (requirements != null)
			{
				for (int i = 0; i < requirements.Count; i++)
				{
					Requirement requirement = requirements[i];
					if (requirement.RequirementType == RequirementType.PayItem && owner is InventoryGameData)
					{
						flag &= DIContainerLogic.InventoryService.RemoveItem(owner as InventoryGameData, requirement.NameId, (int)requirement.Value, trackingDictionary);
					}
				}
			}
			return flag;
		}

		public string ToString(Requirement req)
		{
			return req.RequirementType.ToString() + " " + req.NameId + " " + req.Value;
		}

		public string GetRequirementListString(List<Requirement> reqList)
		{
			string text = string.Empty;
			for (int i = 0; i < reqList.Count; i++)
			{
				Requirement requirement = reqList[i];
				text += ToString(requirement);
				text += "\n";
			}
			return text;
		}

		public float GetRequirementValue(List<Requirement> reqList, RequirementType type, string nameId)
		{
			if (reqList == null)
			{
				return 0f;
			}
			for (int i = 0; i < reqList.Count; i++)
			{
				Requirement requirement = reqList[i];
				if (requirement.RequirementType == type && requirement.NameId == nameId)
				{
					return requirement.Value;
				}
			}
			return 0f;
		}

		public static float GetWeaponEnchantmentAverage(List<ICombatant> birdList)
		{
			float num = 0f;
			for (int i = 0; i < birdList.Count; i++)
			{
				ICombatant combatant = birdList[i];
				num += (float)combatant.CombatantMainHandEquipment.EnchantementLevel;
			}
			return num / (float)birdList.Count;
		}

		public static float GetWeaponEnchantmentHighAverage(List<BirdGameData> birdList)
		{
			float num = 0f;
			for (int i = 0; i < birdList.Count; i++)
			{
				BirdGameData birdGameData = birdList[i];
				int num2 = 0;
				foreach (IInventoryItemGameData item in birdGameData.InventoryGameData.Items[InventoryItemType.MainHandEquipment])
				{
					EquipmentGameData equipmentGameData = item as EquipmentGameData;
					if (equipmentGameData != null && equipmentGameData.EnchantementLevel > num2)
					{
						num2 = equipmentGameData.EnchantementLevel;
					}
				}
				num += (float)num2;
			}
			return num / (float)birdList.Count;
		}
	}
}

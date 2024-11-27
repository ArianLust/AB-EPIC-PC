using System;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Services.Logic.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.Services.Logic
{
	public class RequirementOperationServiceInjectableImpl : IRequirementOperationService
	{
		private Dictionary<Type, Dictionary<RequirementType, Func<object, Requirement, bool>>> CheckFunctionsByRequirement = new Dictionary<Type, Dictionary<RequirementType, Func<object, Requirement, bool>>>();

		public void InitializeRequirementOperations()
		{
			CheckFunctionsByRequirement.Clear();
			Dictionary<RequirementType, Func<object, Requirement, bool>> dictionary = new Dictionary<RequirementType, Func<object, Requirement, bool>>();
			CheckFunctionsByRequirement.Add(typeof(PlayerGameData), dictionary);
			dictionary.Add(RequirementType.Level, (object o, Requirement r) => false);
			dictionary.Add(RequirementType.UsedFriends, (object o, Requirement r) => false);
			dictionary.Add(RequirementType.HavePassedCycleTime, (object o, Requirement r) => false);
			dictionary.Add(RequirementType.NotHavePassedCycleTime, (object o, Requirement r) => false);
			dictionary.Add(RequirementType.HaveCurrentHotpsotState, (object o, Requirement r) => false);
			dictionary.Add(RequirementType.HaveUnlockedHotpsot, (object o, Requirement r) => false);
			dictionary.Add(RequirementType.NotHaveUnlockedHotpsot, (object o, Requirement r) => true);
			dictionary.Add(RequirementType.HaveBird, (object o, Requirement r) => false);
			dictionary.Add(RequirementType.HaveBirdCount, (object o, Requirement r) => false);
			dictionary.Add(RequirementType.HaveItem, (object o, Requirement r) => false);
			dictionary.Add(RequirementType.NotHaveItem, (object o, Requirement r) => true);
			dictionary.Add(RequirementType.NotHaveClass, (object o, Requirement r) => true);
			dictionary.Add(RequirementType.HaveClass, (object o, Requirement r) => true);
			dictionary.Add(RequirementType.HaveItemWithLevel, (object o, Requirement r) => false);
			dictionary.Add(RequirementType.NotHaveItemWithLevel, (object o, Requirement r) => true);
			dictionary.Add(RequirementType.PayItem, (object o, Requirement r) => false);
			dictionary.Add(RequirementType.IsSpecificWeekday, (object o, Requirement r) => false);
			dictionary.Add(RequirementType.IsNotSpecificWeekday, (object o, Requirement r) => true);
			dictionary.Add(RequirementType.HaveLessThan, (object o, Requirement r) => true);
			dictionary.Add(RequirementType.IsConverted, (object o, Requirement r) => false);
			dictionary.Add(RequirementType.LostPvpBattle, (object o, Requirement r) => false);
			dictionary.Add(RequirementType.HaveEventScore, (object o, Requirement r) => false);
			Dictionary<RequirementType, Func<object, Requirement, bool>> dictionary2 = new Dictionary<RequirementType, Func<object, Requirement, bool>>();
			CheckFunctionsByRequirement.Add(typeof(InventoryGameData), dictionary2);
			dictionary2.Add(RequirementType.HaveItem, (object o, Requirement r) => false);
			dictionary2.Add(RequirementType.NotHaveClass, (object o, Requirement r) => true);
			dictionary2.Add(RequirementType.HaveClass, (object o, Requirement r) => true);
			dictionary2.Add(RequirementType.NotHaveItem, (object o, Requirement r) => true);
			dictionary2.Add(RequirementType.PayItem, (object o, Requirement r) => false);
		}

		public void OverrideRequirementFunction(object owner, RequirementType requirementType, Func<object, Requirement, bool> function)
		{
			Dictionary<RequirementType, Func<object, Requirement, bool>> value = null;
			if (CheckFunctionsByRequirement.TryGetValue(owner.GetType(), out value) && value.ContainsKey(requirementType))
			{
				value[requirementType] = function;
			}
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
			foreach (Requirement item in requirementsBefore)
			{
				list.Add(new Requirement
				{
					Value = item.Value,
					NameId = item.NameId,
					RequirementType = item.RequirementType
				});
			}
			Requirement requirement = list.FirstOrDefault((Requirement r) => r.RequirementType == requirementToRemove.RequirementType && r.NameId == requirementToRemove.NameId);
			if (requirement != null)
			{
				GetRequirementValueDelta(owner, requirement);
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
			foreach (Requirement item in requirementsToCheck)
			{
				if (!CheckRequirement(owner, item))
				{
					failedRequirements.Add(item);
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
				foreach (Requirement requirement in requirements)
				{
					if (requirement.RequirementType == RequirementType.PayItem && owner is InventoryGameData)
					{
						flag &= DIContainerLogic.InventoryService.RemoveItem(owner as InventoryGameData, requirement.NameId, (int)requirement.Value, removeReason);
					}
				}
			}
			return flag;
		}

		public bool ExecuteRequirements(object owner, List<Requirement> requirements, Dictionary<string, string> trackingDictionary)
		{
			return true;
		}

		public string ToString(Requirement req)
		{
			return req.RequirementType.ToString() + " " + req.NameId + " " + req.Value;
		}

		public string GetRequirementListString(List<Requirement> reqList)
		{
			string text = string.Empty;
			foreach (Requirement req in reqList)
			{
				text += ToString(req);
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
			foreach (Requirement req in reqList)
			{
				if (req.RequirementType == type && req.NameId == nameId)
				{
					return req.Value;
				}
			}
			return 0f;
		}
	}
}

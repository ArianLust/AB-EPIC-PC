using System.Collections.Generic;
using ABH.Services.Logic.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.Services.Logic
{
	public class RequirementOperationServiceNullImpl : IRequirementOperationService
	{
		public void InitializeRequirementOperations()
		{
		}

		public float GetRequirementValue(List<Requirement> reqList, RequirementType type, string nameId)
		{
			return 0f;
		}

		public List<Requirement> AddRequirement(object owner, List<Requirement> requirementsBefore, Requirement requirementToAdd)
		{
			return requirementsBefore;
		}

		public bool CheckFailRequirements(object owner, List<Requirement> failConditionRequirements)
		{
			return !CheckGenericRequirements(owner, failConditionRequirements);
		}

		public bool CheckGenericRequirements(object owner, List<Requirement> requirementsToCheck)
		{
			return true;
		}

		public bool CheckGenericRequirements(object owner, List<Requirement> requirementsToCheck, out List<Requirement> failedRequirements)
		{
			failedRequirements = new List<Requirement>();
			return true;
		}

		public bool CheckRequirement(object owner, Requirement req)
		{
			return true;
		}

		public bool ExecuteRequirements(object owner, List<Requirement> requirements, string removeReason)
		{
			return true;
		}

		public bool ExecuteRequirements(object owner, List<Requirement> requirements, Dictionary<string, string> trackingDictionary)
		{
			return true;
		}

		public string GetRequirementListString(List<Requirement> reqList)
		{
			return "RequirementOperations Not Implemented";
		}

		public string ToString(Requirement req)
		{
			return "RequirementOperations Not Implemented";
		}
	}
}

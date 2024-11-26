using System.Collections.Generic;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.Services.Logic.Interfaces
{
	public interface IRequirementOperationService
	{
		void InitializeRequirementOperations();

		float GetRequirementValue(List<Requirement> reqList, RequirementType type, string nameId);

		List<Requirement> AddRequirement(object owner, List<Requirement> requirementsBefore, Requirement requirementToAdd);

		bool CheckFailRequirements(object owner, List<Requirement> failConditionRequirements);

		bool CheckGenericRequirements(object owner, List<Requirement> requirementsToCheck);

		bool CheckGenericRequirements(object owner, List<Requirement> requirementsToCheck, out List<Requirement> failedRequirements);

		bool CheckRequirement(object owner, Requirement req);

		bool ExecuteRequirements(object owner, List<Requirement> requirements, string removeReason);

		bool ExecuteRequirements(object owner, List<Requirement> requirements, Dictionary<string, string> trackingDictionary);

		string GetRequirementListString(List<Requirement> reqList);

		string ToString(Requirement req);
	}
}

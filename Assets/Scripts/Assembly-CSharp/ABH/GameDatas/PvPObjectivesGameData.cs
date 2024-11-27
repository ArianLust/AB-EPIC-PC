using System.Collections.Generic;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;

namespace ABH.GameDatas
{
	public class PvPObjectivesGameData : GameDataBase<PvPObjectivesBalancingData, PvPObjectiveData>
	{
		public int m_SuccessDuringBattle;

		public string IconSpriteName
		{
			get
			{
				return BalancingData.AssetIconID;
			}
		}

		public int Amount
		{
			get
			{
				return BalancingData.Amount;
			}
		}

		public string Requirement1
		{
			get
			{
				return BalancingData.Requirementvalue;
			}
		}

		public string Requirement2
		{
			get
			{
				return BalancingData.Requirementvalue2;
			}
		}

		public ObjectivesRequirement RequirementType
		{
			get
			{
				return BalancingData.Requirement;
			}
		}

		public int Reward
		{
			get
			{
				return BalancingData.Reward;
			}
		}

		public PvPObjectivesGameData(string nameId)
			: base(nameId)
		{
		}

		public PvPObjectivesGameData(PvPObjectiveData instance)
			: base(instance)
		{
			if (string.IsNullOrEmpty(instance.Difficulty))
			{
				Data.Difficulty = BalancingData.Difficulty;
			}
		}

		protected override PvPObjectiveData CreateNewInstance(string nameId)
		{
			PvPObjectiveData pvPObjectiveData = new PvPObjectiveData();
			pvPObjectiveData.NameId = nameId;
			pvPObjectiveData.Progress = 0;
			pvPObjectiveData.Solved = false;
			pvPObjectiveData.Difficulty = BalancingData.Difficulty;
			return pvPObjectiveData;
		}

		public string GetDifficulty()
		{
			if (string.IsNullOrEmpty(Data.Difficulty))
			{
				return BalancingData.Difficulty;
			}
			return Data.Difficulty;
		}

		public string GetTooltipText()
		{
			ABHLocaService locaService = DIContainerInfrastructure.GetLocaService();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{amount}", BalancingData.Amount.ToString());
			switch (BalancingData.Requirement)
			{
			case ObjectivesRequirement.useBird:
			case ObjectivesRequirement.notUseBird:
			case ObjectivesRequirement.killBird:
			case ObjectivesRequirement.protectBird:
			case ObjectivesRequirement.killWithBird:
				dictionary.Add("{value_1}", locaService.GetCharacterName(BalancingData.Requirementvalue));
				dictionary.Add("{value_2}", locaService.GetCharacterName(BalancingData.Requirementvalue2));
				break;
			case ObjectivesRequirement.getAmountStars:
			case ObjectivesRequirement.withBirdsAlive:
			case ObjectivesRequirement.killAtOnce:
			case ObjectivesRequirement.killBirdsInBattle:
			case ObjectivesRequirement.useRage:
			case ObjectivesRequirement.killBirdsInRound:
				dictionary.Add("{value_1}", BalancingData.Requirementvalue);
				break;
			case ObjectivesRequirement.useClass:
			{
				string className = locaService.GetClassName("bird_class_" + BalancingData.Requirementvalue);
				string className2 = locaService.GetClassName("bird_class_" + BalancingData.Requirementvalue2);
				string replacementName = string.Empty;
				if (CheckForReplacement("class_" + Requirement1, out replacementName))
				{
					className = locaService.GetClassName(replacementName);
				}
				if (CheckForReplacement("class_" + Requirement2, out replacementName))
				{
					className2 = locaService.GetClassName(replacementName);
				}
				dictionary.Add("{value_1}", className);
				dictionary.Add("{value_2}", className2);
				break;
			}
			}
			return locaService.Tr(BalancingData.LocaIdent, dictionary);
		}

		private bool CheckForReplacement(string className, out string replacementName)
		{
			string text = string.Empty;
			replacementName = string.Empty;
			foreach (BirdGameData allBird in DIContainerInfrastructure.GetCurrentPlayer().AllBirds)
			{
				if (allBird.ClassItem.BalancingData.NameId == className && allBird.ClassSkin != null && allBird.ClassSkin.BalancingData.SortPriority > 0)
				{
					replacementName = allBird.ClassSkin.BalancingData.LocaBaseId;
					return true;
				}
			}
			foreach (ClassItemBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<ClassItemBalancingData>())
			{
				if (balancingData.ReplacementClassNameId == className)
				{
					text = balancingData.NameId;
					replacementName = balancingData.LocaBaseId;
					break;
				}
			}
			if (!string.IsNullOrEmpty(text) && DIContainerLogic.InventoryService.CheckForItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, text))
			{
				return true;
			}
			return false;
		}

		public List<string> GetProgressList()
		{
			if (Data.ProgressList == null)
			{
				Data.ProgressList = new List<string>();
			}
			return Data.ProgressList;
		}
	}
}

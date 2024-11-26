using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Interfaces;

namespace ABH.GameDatas.Battle.Skills
{
	public class XPMultiplierSkill : SkillBattleDataBase
	{
		private float m_Percent;

		private string m_ItemName = string.Empty;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			KeyValuePair<string, float> keyValuePair = base.Model.SkillParameters.FirstOrDefault();
			if (!keyValuePair.Equals(default(KeyValuePair<string, float>)))
			{
				m_ItemName = keyValuePair.Key;
				m_Percent = keyValuePair.Value;
			}
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DebugLog.Log("Trigger support skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			IInventoryItemBalancingData bal = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(m_ItemName);
			ICombatant source2 = source;
			m_Targets = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == source2.CombatantFaction).ToList();
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting, m_Targets);
			foreach (ICombatant combatant in m_Targets)
			{
				combatant.CombatantView.PlayCheerCharacter();
			}
			if (!string.IsNullOrEmpty(m_ItemName) && bal != null)
			{
				battle.m_AppliedXPModifiers.Add(DIContainerLogic.InventoryService.AddItem(battle.m_ControllerInventory, 1, 1, bal.NameId, (int)m_Percent, "xp_modifer_applied"));
			}
			yield break;
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", string.Empty + m_Percent);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

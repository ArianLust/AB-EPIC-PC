using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle.Skills
{
	public class PassiveResurrectAura : SkillBattleDataBase
	{
		private float m_RevivePercentage;

		private float m_ReviveChance;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			model.SkillParameters.TryGetValue("revive_percentage", out m_RevivePercentage);
			model.SkillParameters.TryGetValue("chance_to_revive", out m_ReviveChance);
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DoActionInstant(battle, source, target);
			yield break;
		}

		public override void DoActionInstant(BattleGameData battle, ICombatant source, ICombatant target)
		{
			DebugLog.Log("Trigger banner skill: " + base.Model.Balancing.NameId);
			m_Source = source;
			m_Targets = new List<ICombatant>();
			m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target.CombatantFaction && c != m_Source).ToList());
			foreach (ICombatant target2 in m_Targets)
			{
				List<float> list = new List<float>();
				list.Add(m_RevivePercentage);
				list.Add(m_ReviveChance);
				List<float> values = list;
				BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, target2, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.OnAllHealthLost,
						EffectType = BattleEffectType.ReviveOnDeath,
						Values = values,
						AfflicionType = base.Model.Balancing.EffectType,
						Duration = base.Model.Balancing.EffectDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
				battleEffectGameData.AddEffect();
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			WorldBalancingData worldBalancingData = DIContainerBalancing.Service.GetBalancingDataList<WorldBalancingData>().FirstOrDefault();
			dictionary.Add("{value_1}", m_ReviveChance.ToString("0"));
			dictionary.Add("{value_2}", m_RevivePercentage.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

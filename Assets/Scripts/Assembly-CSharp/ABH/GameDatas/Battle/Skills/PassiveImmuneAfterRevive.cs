using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle.Skills
{
	public class PassiveImmuneAfterRevive : SkillBattleDataBase
	{
		private float m_ImmunityDuration;

		private float m_DebuffImmunity;

		private float m_DamageReduction;

		private float m_Chance;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			model.SkillParameters.TryGetValue("chance", out m_Chance);
			model.SkillParameters.TryGetValue("duration", out m_ImmunityDuration);
			model.SkillParameters.TryGetValue("immune_to_debuffs", out m_DebuffImmunity);
			model.SkillParameters.TryGetValue("damage_reduction", out m_DamageReduction);
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
			m_Targets.AddRange(battle.m_CombatantsPerFaction[target.CombatantFaction].Where((ICombatant c) => c != m_Source).ToList());
			List<float> list = new List<float>();
			list.Add(m_Chance);
			list.Add(m_ImmunityDuration);
			list.Add(m_DebuffImmunity);
			list.Add(m_DamageReduction);
			List<float> values = list;
			foreach (ICombatant target2 in m_Targets)
			{
				BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, target2, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.OnRevive,
						EffectType = BattleEffectType.Immunity,
						AfflicionType = base.Model.Balancing.EffectType,
						Values = values,
						Duration = base.Model.Balancing.EffectDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, base.Model.Balancing.EffectDuration, battle, string.Empty, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
				battleEffectGameData.AddEffect();
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", m_Chance.ToString("0"));
			dictionary.Add("{value_2}", m_ImmunityDuration.ToString("0"));
			dictionary.Add("{value_3}", m_DebuffImmunity.ToString("0"));
			dictionary.Add("{value_4}", m_DamageReduction.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

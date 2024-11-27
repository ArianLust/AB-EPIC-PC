using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class SelfDestructAndApplyDot : SkillBattleDataBase
	{
		private float m_ChargeTurns;

		private float m_DotChance = 100f;

		private float m_DotModifier;

		private bool m_All;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("dot_chance", out m_DotChance);
			base.Model.SkillParameters.TryGetValue("dot_damage_in_percent", out m_DotModifier);
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
			m_All = base.Model.SkillParameters.ContainsKey("all");
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DebugLog.Log("Trigger attack skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			if (EvaluateCharge(battle, source, m_Targets, target))
			{
				yield break;
			}
			List<ICombatant> possibleTargets = new List<ICombatant>();
			if (m_All)
			{
				ICombatant source2 = source;
				possibleTargets = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction != source2.CombatantFaction).ToList();
			}
			else
			{
				possibleTargets.Add(target);
			}
			if (m_DotChance / 100f > Random.value)
			{
				for (int i = 0; i < possibleTargets.Count; i++)
				{
					List<float> valueList = new List<float> { m_DotModifier };
					BattleEffectGameData effect = new BattleEffectGameData(source, possibleTargets[i], new List<BattleEffect>
					{
						new BattleEffect
						{
							EffectTrigger = EffectTriggerType.OnDealDamagePerTurn,
							EffectType = BattleEffectType.DoDamage,
							Values = valueList,
							AfflicionType = base.Model.Balancing.EffectType,
							Duration = base.Model.Balancing.EffectDuration,
							EffectAssetId = base.Model.Balancing.EffectIconAssetId,
							EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
						}
					}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, SkillEffectTypes.Curse, GetLocalizedName(), base.Model.SkillNameId);
					effect.AddEffect();
				}
			}
			source.ReceiveDamage(source.CurrentHealth, source);
			DIContainerLogic.GetBattleService().DealDamageFromCurrentTurn(source, battle, source);
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", string.Empty + m_DotModifier);
			dictionary.Add("{value_2}", string.Empty + m_ChargeTurns);
			dictionary.Add("{value_3}", string.Empty + m_DotChance);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

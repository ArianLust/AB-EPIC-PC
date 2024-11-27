using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class RageIncreaseSkill : SkillBattleDataBase
	{
		protected float m_Percent;

		protected bool m_All;

		protected bool m_Self;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("rage_in_percent", out m_Percent);
			m_All = base.Model.SkillParameters.ContainsKey("all");
			m_Self = base.Model.SkillParameters.ContainsKey("self");
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DebugLog.Log("Trigger support skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			m_InitialTarget = target;
			if (!m_All)
			{
				if (m_Self)
				{
					m_Targets = new List<ICombatant> { source };
					m_InitialTarget = source;
				}
				else
				{
					m_Targets = new List<ICombatant> { target };
				}
			}
			else
			{
				m_Targets = new List<ICombatant>();
				ICombatant target2 = target;
				m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target2.CombatantFaction).ToList());
			}
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			yield return new WaitForSeconds(source.CombatantView.PlaySupportAnimation());
			foreach (ICombatant skillTarget in m_Targets)
			{
				List<BattleEffect> battleEffectlist = new List<BattleEffect>();
				float effectValue = 0f;
				if (base.Model.SkillParameters.TryGetValue("stun_chance", out effectValue))
				{
					float stunDuration = 1f;
					base.Model.SkillParameters.TryGetValue("stun_duration", out stunDuration);
					if (skillTarget.CombatantFaction == Faction.Pigs)
					{
						stunDuration -= 1f;
					}
					List<float> valueList2 = new List<float> { effectValue, stunDuration };
					battleEffectlist.Add(new BattleEffect
					{
						EffectTrigger = EffectTriggerType.OnReceiveDamage,
						EffectType = BattleEffectType.StunAttacker,
						AfflicionType = base.Model.Balancing.EffectType,
						Values = valueList2,
						Duration = base.Model.Balancing.EffectDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					});
				}
				if (m_Percent != 0f)
				{
					skillTarget.CombatantView.PlayRageAnimation();
					List<float> valueList = new List<float> { m_Percent };
					battleEffectlist.Add(new BattleEffect
					{
						EffectTrigger = EffectTriggerType.OnProduceRageByAttacked,
						EffectType = BattleEffectType.IncreaseRageOnAttacked,
						Values = valueList,
						AfflicionType = base.Model.Balancing.EffectType,
						Duration = base.Model.Balancing.EffectDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					});
				}
				BattleEffectGameData effect = new BattleEffectGameData(source, skillTarget, battleEffectlist, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, SkillEffectTypes.Blessing, GetLocalizedName(), base.Model.SkillNameId);
				effect.AddEffect();
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", m_Percent.ToString("0"));
			dictionary.Add("{value_2}", base.Model.Balancing.EffectDuration.ToString("0"));
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("stun_chance", out value);
			dictionary.Add("{value_3}", value.ToString("0"));
			float value2 = 0f;
			base.Model.SkillParameters.TryGetValue("stun_duration", out value2);
			dictionary.Add("{value_5}", (value2 - 1f).ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

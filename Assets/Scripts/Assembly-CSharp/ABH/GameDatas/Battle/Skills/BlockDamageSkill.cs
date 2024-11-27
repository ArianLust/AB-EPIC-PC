using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using SmoothMoves;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class BlockDamageSkill : SkillBattleDataBase
	{
		private int m_BuffDuration;

		private float m_Percent;

		private float m_ChargeTurns;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("percent", out m_Percent);
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
			m_BuffDuration = base.Model.Balancing.EffectDuration;
		}

		public override void BoneAnimationUserTrigger(UserTriggerEvent triggerEvent)
		{
			base.BoneAnimationUserTrigger(triggerEvent);
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DebugLog.Log("Trigger support skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			if (EvaluateCharge(battle, source, m_Targets, target))
			{
				yield break;
			}
			if (!base.Model.SkillParameters.ContainsKey("all"))
			{
				m_Targets = new List<ICombatant> { target };
			}
			else
			{
				m_Targets = new List<ICombatant>();
				ICombatant source2 = source;
				m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == source2.CombatantFaction).ToList());
			}
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			yield return new WaitForSeconds(source.CombatantView.PlaySupportAnimation());
			foreach (ICombatant skillTarget in m_Targets)
			{
				if (skillTarget != source)
				{
					skillTarget.CombatantView.PlayCheerCharacter();
				}
				float modifyablePercent = m_Percent;
				float additionalModifierForAimedTarget = 0f;
				if (skillTarget == target && base.Model.SkillParameters.TryGetValue("extra_percent_on_target", out additionalModifierForAimedTarget))
				{
					modifyablePercent += additionalModifierForAimedTarget;
				}
				List<float> valueList = new List<float> { modifyablePercent };
				BattleEffectGameData effect = new BattleEffectGameData(source, skillTarget, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.OnReceiveDamage,
						EffectType = BattleEffectType.ReduceDamageReceived,
						AfflicionType = base.Model.Balancing.EffectType,
						Values = valueList,
						Duration = m_BuffDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, m_BuffDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
				effect.AddEffect();
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", string.Empty + m_Percent);
			dictionary.Add("{value_2}", string.Empty + m_BuffDuration);
			dictionary.Add("{value_5}", string.Empty + m_ChargeTurns);
			float value = 0f;
			if (base.Model.SkillParameters.TryGetValue("extra_percent_on_target", out value))
			{
				dictionary.Add("{value_7}", string.Empty + (m_Percent + value));
			}
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

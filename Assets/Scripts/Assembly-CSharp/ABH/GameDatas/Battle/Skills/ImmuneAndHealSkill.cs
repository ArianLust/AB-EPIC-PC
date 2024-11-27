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
	public class ImmuneAndHealSkill : SkillBattleDataBase
	{
		private int m_BuffDuration;

		private float m_Percent;

		protected bool m_Self;

		protected bool m_All;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("heal_percent_of_damage", out m_Percent);
			m_Self = base.Model.SkillParameters.ContainsKey("self");
			m_All = base.Model.SkillParameters.ContainsKey("all");
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
			yield return new WaitForSeconds(source.CombatantView.PlaySupportAnimation());
			foreach (ICombatant skillTarget in m_Targets)
			{
				List<float> valueList = new List<float> { m_Percent };
				BattleEffectGameData effect = new BattleEffectGameData(source, skillTarget, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.OnReceiveDamage,
						EffectType = BattleEffectType.TurnDamageIntoHealing,
						AfflicionType = base.Model.Balancing.EffectType,
						Values = valueList,
						Duration = m_BuffDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, m_BuffDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
				effect.AddEffect();
				SpawnVisualEffects(VisualEffectSpawnTiming.Affected, m_VisualEffectSetting);
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", string.Empty + m_Percent);
			dictionary.Add("{value_2}", string.Empty + m_BuffDuration);
			dictionary.Add("{value_3}", string.Empty + m_Percent);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

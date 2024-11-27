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
	public class ApplyCritChanceSkill : SkillBattleDataBase
	{
		private int m_BuffDuration;

		private float m_Percent;

		private float m_CritChance;

		private float m_ExtraDamage;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("percent", out m_Percent);
			base.Model.SkillParameters.TryGetValue("critchance_in_percent", out m_CritChance);
			base.Model.SkillParameters.TryGetValue("extra_damage", out m_ExtraDamage);
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
				List<float> valueList = new List<float> { m_CritChance, m_ExtraDamage };
				BattleEffectGameData effect = new BattleEffectGameData(source, skillTarget, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.OnDealDamage,
						EffectType = BattleEffectType.Crit,
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
			dictionary.Add("{value_1}", string.Empty + m_ExtraDamage);
			dictionary.Add("{value_3}", string.Empty + m_CritChance);
			dictionary.Add("{value_2}", string.Empty + m_BuffDuration);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

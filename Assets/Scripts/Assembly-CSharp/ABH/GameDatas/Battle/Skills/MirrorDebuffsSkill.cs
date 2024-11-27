using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class MirrorDebuffsSkill : SkillBattleDataBase
	{
		private int m_BuffDuration = 1;

		private float m_Chance = 100f;

		private float m_CleanseOnCast;

		private float m_TargetImmune;

		private bool m_All;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("chance", out m_Chance);
			base.Model.SkillParameters.TryGetValue("cleanse_on_cast", out m_CleanseOnCast);
			base.Model.SkillParameters.TryGetValue("target_immune", out m_TargetImmune);
			m_All = base.Model.SkillParameters.ContainsKey("all");
			m_BuffDuration = base.Model.Balancing.EffectDuration;
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DebugLog.Log("Trigger support skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			m_InitialTarget = target;
			if (!m_All)
			{
				m_Targets = new List<ICombatant> { target };
			}
			else
			{
				m_Targets = new List<ICombatant>();
				ICombatant target2 = target;
				m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target2.CombatantFaction).ToList());
			}
			if (EvaluateCharge(battle, source, m_Targets, target))
			{
				m_Source.CombatantView.targetSheltered = null;
				yield break;
			}
			yield return new WaitForSeconds(source.CombatantView.PlaySupportAnimation());
			foreach (ICombatant skillTarget in m_Targets)
			{
				if (skillTarget != source)
				{
					skillTarget.CombatantView.PlayCheerCharacter();
				}
				if (m_CleanseOnCast == 1f)
				{
					int count = DIContainerLogic.GetBattleService().RemoveBattleEffects(skillTarget, SkillEffectTypes.Curse);
					if (count > 0)
					{
						VisualEffectSetting setting = null;
						if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting("Cleanse", out setting))
						{
							SpawnVisualEffects(VisualEffectSpawnTiming.Affected, setting, new List<ICombatant> { skillTarget });
						}
					}
				}
				m_Source = source;
				List<float> valueList = new List<float> { m_TargetImmune, m_Chance };
				BattleEffectGameData effect = new BattleEffectGameData(source, target, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.OnAddEffect,
						EffectType = BattleEffectType.MirrorCurse,
						AfflicionType = base.Model.Balancing.EffectType,
						Values = valueList,
						Duration = base.Model.Balancing.EffectDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
				effect.AddEffect();
			}
			SpawnVisualEffects(VisualEffectSpawnTiming.Affected, m_VisualEffectSetting);
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", m_Chance.ToString("0"));
			dictionary.Add("{value_2}", string.Empty + m_BuffDuration);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using SmoothMoves;

namespace ABH.GameDatas.Battle.Skills
{
	public class ApplyEvadeAndSuicideSkill : SkillBattleDataBase
	{
		private int m_BuffDuration;

		private float m_Percent;

		public override void BoneAnimationUserTrigger(UserTriggerEvent triggerEvent)
		{
			base.BoneAnimationUserTrigger(triggerEvent);
		}

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("percent", out m_Percent);
			m_BuffDuration = base.Model.Balancing.EffectDuration;
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DebugLog.Log("Trigger support skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			ICombatant source2 = source;
			foreach (ICombatant combatant in battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction != source2.CombatantFaction))
			{
				combatant.CombatantView.PlaySurprisedAnimation();
			}
			source.ReceiveDamage(source.CurrentHealth, source);
			DIContainerLogic.GetBattleService().DealDamageFromCurrentTurn(source, battle, source);
			if (!base.Model.SkillParameters.ContainsKey("all"))
			{
				m_Targets = new List<ICombatant> { target };
			}
			else
			{
				m_Targets = new List<ICombatant>();
				m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == source2.CombatantFaction).ToList());
			}
			foreach (ICombatant skillTarget in m_Targets)
			{
				if (skillTarget != source)
				{
					skillTarget.CombatantView.PlayCheerCharacter();
				}
				if (m_Percent > 0f)
				{
					List<float> valueList = new List<float> { m_Percent };
					BattleEffectGameData effect = new BattleEffectGameData(source, skillTarget, new List<BattleEffect>
					{
						new BattleEffect
						{
							EffectTrigger = EffectTriggerType.BeforeReceiveDamage,
							EffectType = BattleEffectType.Evade,
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
			yield break;
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_3}", string.Empty + m_Percent);
			dictionary.Add("{value_2}", string.Empty + m_BuffDuration);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

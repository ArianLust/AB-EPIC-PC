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
	public class TauntAndDefendSkill : SkillBattleDataBase
	{
		private float m_Percent;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			DIContainerLogic.GetVisualEffectsBalancing().TryGetBubbleSetting("IntimidatingStrike", out m_BubbleSetting);
			base.Model.SkillParameters.TryGetValue("percent", out m_Percent);
		}

		public override void BoneAnimationUserTrigger(UserTriggerEvent triggerEvent)
		{
			base.BoneAnimationUserTrigger(triggerEvent);
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DebugLog.Log("Trigger support skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			ICombatant source2 = source;
			if (!base.Model.SkillParameters.ContainsKey("all"))
			{
				m_Targets = new List<ICombatant> { target };
			}
			else
			{
				m_Targets = new List<ICombatant>();
				m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == source2.CombatantFaction).ToList());
			}
			float effectValue = 0f;
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			yield return new WaitForSeconds(source.CombatantView.PlaySupportAnimation());
			foreach (ICombatant skillTarget2 in m_Targets)
			{
				if (skillTarget2 != source)
				{
					skillTarget2.CombatantView.PlayCheerCharacter();
				}
				if (base.Model.SkillParameters.TryGetValue("percent", out effectValue))
				{
					List<float> valueList2 = new List<float> { effectValue };
					BattleEffectGameData effect = new BattleEffectGameData(source, skillTarget2, new List<BattleEffect>
					{
						new BattleEffect
						{
							EffectTrigger = EffectTriggerType.OnReceiveDamage,
							EffectType = BattleEffectType.ReduceDamageReceived,
							AfflicionType = base.Model.Balancing.EffectType,
							Values = valueList2,
							Duration = base.Model.Balancing.EffectDuration,
							EffectAssetId = base.Model.Balancing.EffectIconAssetId,
							EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
						}
					}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
					effect.SetPersistanceAfterDefeat(false).AddEffect();
				}
			}
			m_Targets = new List<ICombatant>();
			m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction != source2.CombatantFaction).ToList());
			foreach (ICombatant skillTarget in m_Targets)
			{
				if (!skillTarget.IsBanner)
				{
					List<float> valueList = new List<float> { 1f };
					BattleEffectGameData tauntEffect = new BattleEffectGameData(target, skillTarget, source, new List<BattleEffect>
					{
						new BattleEffect
						{
							AfflicionType = SkillEffectTypes.Curse,
							EffectTrigger = EffectTriggerType.OnTarget,
							EffectType = BattleEffectType.Taunt,
							Values = valueList,
							Duration = base.Model.Balancing.EffectDuration,
							EffectAssetId = base.Model.Balancing.EffectIconAssetId,
							EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
						}
					}, base.Model.Balancing.EffectDuration, battle, "Force_Target", SkillEffectTypes.Curse, GetLocalizedName(), base.Model.SkillNameId);
					tauntEffect.SetPersistanceAfterDefeat(false).AddEffect();
					CharacterSpeechBubble targetingBubble = skillTarget.CombatantView.m_SpeechBubbles.Values.FirstOrDefault();
					if (targetingBubble != null && targetingBubble.m_IsTargetedBubble)
					{
						targetingBubble.SetTargetIcon("Target_" + target.CombatantAssetId);
						targetingBubble.UpdateSkill();
					}
				}
			}
			m_Source = target;
			VisualEffectSetting vEffect = null;
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_3}", m_Percent.ToString("0"));
			dictionary.Add("{value_2}", base.Model.Balancing.EffectDuration.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

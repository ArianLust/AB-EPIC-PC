using System;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class SimpleAttackWithTauntSkill : AttackSkillTemplate
	{
		private float m_TimeStarted;

		private int m_BuffDuration;

		private float m_DamageMod;

		private float m_AttackCount;

		private bool m_TauntStrongest;

		private bool m_TargetStrongest;

		private ICombatant tauntTarget;

		private ICombatant tauntingCharacter;

		private bool m_UseSkillAssetId;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			m_BuffDuration = base.Model.Balancing.EffectDuration;
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			m_DamageMod = value / 100f;
			m_TauntStrongest = base.Model.SkillParameters.ContainsKey("taunt_strongest");
			m_TargetStrongest = base.Model.SkillParameters.ContainsKey("target_strongest");
			m_UseSkillAssetId = base.Model.SkillParameters.ContainsKey("use_skill_asset");
			base.Model.SkillParameters.TryGetValue("attack_count", out m_AttackCount);
			ActionsOnStartSkill.Add(delegate
			{
				m_TimeStarted = Time.time;
				return 0f;
			});
			ActionsAfterDamageDealt.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				ShareableActionPart(battle, source, target, false);
			});
			ActionsOnEnd.Add(delegate(BattleGameData battle, ICombatant source, ICombatant initialTarget)
			{
				float time = Time.time;
				return source.CombatantView.m_AssetController.GetAttackAnimationLength() - (time - m_TimeStarted);
			});
		}

		public override void ShareableActionPart(BattleGameData battle, ICombatant source, ICombatant target, bool isShared)
		{
			tauntTarget = target;
			tauntingCharacter = source;
			if (m_TauntStrongest)
			{
				List<ICombatant> list = new List<ICombatant>();
				list.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == tauntTarget.CombatantFaction).ToList());
				foreach (ICombatant item in list)
				{
					if (item.CurrentHealth > tauntTarget.CurrentHealth)
					{
						tauntTarget = item;
					}
				}
			}
			if (m_TargetStrongest)
			{
				List<ICombatant> list2 = new List<ICombatant>();
				list2.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == tauntingCharacter.CombatantFaction).ToList());
				foreach (ICombatant item2 in list2)
				{
					if (item2.CurrentHealth > tauntingCharacter.CurrentHealth)
					{
						tauntingCharacter = item2;
					}
				}
			}
			if (tauntTarget.IsBanner)
			{
				return;
			}
			List<float> list3 = new List<float>();
			list3.Add(1f);
			List<float> values = list3;
			BattleEffectGameData battleEffectGameData = new BattleEffectGameData(tauntingCharacter, tauntTarget, new List<BattleEffect>
			{
				new BattleEffect
				{
					EffectTrigger = EffectTriggerType.OnTarget,
					EffectType = BattleEffectType.Taunt,
					AfflicionType = base.Model.Balancing.EffectType,
					Values = values,
					Duration = m_BuffDuration,
					EffectAssetId = base.Model.Balancing.EffectIconAssetId,
					EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
				}
			}, m_BuffDuration, battle, (!m_UseSkillAssetId) ? "Force_Target" : base.Model.Balancing.EffectIconAssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
			battleEffectGameData.SetPersistanceAfterDefeat(false).AddEffect();
			if (tauntTarget.CombatantView == null)
			{
				return;
			}
			CharacterSpeechBubble characterSpeechBubble = tauntTarget.CombatantView.m_SpeechBubbles.Values.FirstOrDefault();
			if (characterSpeechBubble != null && characterSpeechBubble.m_IsTargetedBubble)
			{
				characterSpeechBubble.SetTargetIcon("Target_" + tauntingCharacter.CombatantAssetId);
				characterSpeechBubble.UpdateSkill();
			}
			foreach (ICombatant item3 in battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == Faction.Birds))
			{
				item3.CombatantView.CheckForTauntTarget();
			}
		}

		private void ShowTauntBubble()
		{
			m_Targets = new List<ICombatant> { tauntTarget };
			m_Source = tauntingCharacter;
			if (!m_UseSkillAssetId)
			{
				DIContainerLogic.GetVisualEffectsBalancing().TryGetBubbleSetting("Taunt", out m_BubbleSetting);
			}
			SpawnBubble(base.Model.Balancing.EffectDuration);
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num);
			dictionary.Add("{value_2}", string.Empty + m_BuffDuration);
			dictionary.Add("{value_4}", string.Empty + m_AttackCount);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

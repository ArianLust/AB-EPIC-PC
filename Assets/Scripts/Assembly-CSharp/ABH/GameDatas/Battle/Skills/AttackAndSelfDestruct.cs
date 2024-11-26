using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class AttackAndSelfDestruct : AttackSkillTemplate
	{
		private float m_DamageInPercent;

		private float m_ChargeTurns;

		private float m_DamageMod;

		private float m_AttackCount;

		private bool m_All;

		private float m_StunChance;

		private float m_StunDuration;

		private float m_RageReduce;

		private bool m_UseSkillAssetId;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			float value;
			base.Model.SkillParameters.TryGetValue("use_support_anim", out value);
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out m_DamageInPercent);
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
			base.Model.SkillParameters.TryGetValue("attack_count", out m_AttackCount);
			base.Model.SkillParameters.TryGetValue("stun_chance", out m_StunChance);
			base.Model.SkillParameters.TryGetValue("stun_duration", out m_StunDuration);
			base.Model.SkillParameters.TryGetValue("ragereduce", out m_RageReduce);
			m_All = base.Model.SkillParameters.ContainsKey("all");
			m_DamageMod = m_DamageInPercent / 100f;
			m_UseCenterPosition = true;
			m_UseSkillAssetId = base.Model.SkillParameters.ContainsKey("use_skill_asset");
			ActionsAfterDamageDealt.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				ShareableActionPart(battle, source, target, false);
			});
			if (value == 1f)
			{
				m_AttackAnimation = (ICombatant c) => c.CombatantView.PlaySupportAnimation();
			}
		}

		public override void ShareableActionPart(BattleGameData battle, ICombatant source, ICombatant target, bool isShared)
		{
			if (!(m_StunChance / 100f < UnityEngine.Random.value))
			{
				DebugLog.Log("Stunned");
				string text = "Stun";
				List<float> list = new List<float>();
				list.Add(1f);
				List<float> values = list;
				BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, target, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.BeforeStartOfTurn,
						EffectType = BattleEffectType.Stun,
						AfflicionType = SkillEffectTypes.Curse,
						Values = values,
						Duration = (int)m_StunDuration,
						EffectAssetId = ((!m_UseSkillAssetId) ? text : base.Model.Balancing.EffectIconAssetId),
						EffectAtlasId = ((!m_UseSkillAssetId) ? "Skills_Generic" : base.Model.Balancing.EffectIconAtlasId)
					}
				}, (int)m_StunDuration, battle, text, SkillEffectTypes.Curse, GetLocalizedName(), base.Model.SkillNameId);
				battleEffectGameData.AddEffect();
				battleEffectGameData.EffectRemovedAction = delegate(BattleEffectGameData e)
				{
					e.m_Target.CombatantView.PlayIdle();
				};
				battleEffectGameData.m_Target.CombatantView.PlayStunnedAnimation();
			}
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			m_Source = source;
			if (EvaluateCharge(battle, source, m_Targets, target))
			{
				yield break;
			}
			float modificationreceived = 1f;
			float modificationdealt = 1f;
			float attackValue = source.ModifiedAttack * (m_DamageInPercent / 100f);
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
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting, possibleTargets);
			for (int i = 0; i < possibleTargets.Count; i++)
			{
				ICombatant skillTarget = possibleTargets[i];
				DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(1f, EffectTriggerType.BeforeDealDamage, m_Source, m_Source);
				DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(1f, EffectTriggerType.BeforeReceiveDamage, skillTarget, source);
				modificationdealt = DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(modificationdealt, EffectTriggerType.OnDealDamage, m_Source, skillTarget);
				attackValue *= modificationdealt;
				modificationreceived = DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(modificationreceived, EffectTriggerType.OnReceiveDamage, skillTarget, m_Source);
				float damage = attackValue * modificationreceived;
				skillTarget.ReceiveDamage(damage, source);
				DIContainerLogic.GetBattleService().DealDamageFromCurrentTurn(skillTarget, battle, source);
				DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(damage, EffectTriggerType.AfterReceiveDamage, skillTarget, m_Source);
			}
			if (m_RageReduce > 0f)
			{
				DIContainerLogic.GetBattleService().ReduceRageFromAttack(m_RageReduce, battle, target);
				if (target.IsAlive)
				{
					target.CombatantView.PlayCheerCharacter();
				}
			}
			source.ReceiveDamage(source.CurrentHealth, source);
			DIContainerLogic.GetBattleService().DealDamageFromCurrentTurn(source, battle, source);
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num);
			dictionary.Add("{value_2}", string.Empty + m_ChargeTurns);
			dictionary.Add("{value_3}", string.Empty + m_StunChance);
			dictionary.Add("{value_4}", string.Empty + m_AttackCount);
			dictionary.Add("{value_5}", string.Empty + m_StunDuration);
			dictionary.Add("{value_6}", string.Empty + m_RageReduce);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

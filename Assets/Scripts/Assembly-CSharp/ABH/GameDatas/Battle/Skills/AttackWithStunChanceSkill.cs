using System;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class AttackWithStunChanceSkill : AttackSkillTemplate
	{
		private float m_DamageMod;

		private float m_ChargeTurns;

		private float m_AttackCount;

		private float m_StunChance;

		private float m_StunVsWetChance;

		private float m_StunDuration;

		private bool m_UseSkillAssetId;

		private bool m_StunVsWet;

		private bool m_Freeze;

		private bool m_TentacleGrab;

		private bool m_CleanseAfterDeath;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			m_DamageMod = value / 100f;
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
			base.Model.SkillParameters.TryGetValue("attack_count", out m_AttackCount);
			base.Model.SkillParameters.TryGetValue("chance", out m_StunChance);
			base.Model.SkillParameters.TryGetValue("duration", out m_StunDuration);
			m_Freeze = base.Model.SkillParameters.ContainsKey("freeze");
			m_TentacleGrab = base.Model.SkillParameters.ContainsKey("tentacle_grab");
			m_CleanseAfterDeath = base.Model.SkillParameters.ContainsKey("release_on_death");
			if (base.Model.SkillParameters.TryGetValue("chance_to_stun_vs_wet", out m_StunVsWetChance) && m_StunVsWetChance > 0f)
			{
				m_StunVsWet = true;
				m_StunChance = m_StunVsWetChance;
			}
			m_UseSkillAssetId = base.Model.SkillParameters.ContainsKey("use_skill_asset");
			ActionsBeforeTargetSelection.Add(delegate(BattleGameData battle, ICombatant source, ICombatant target)
			{
				if (source.CombatantView.m_AssetController is TentacleAssetController)
				{
					(source.CombatantView.m_AssetController as TentacleAssetController).m_IsStunning = true;
				}
				return 0f;
			});
			ActionsAfterDamageDealt.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				ShareableActionPart(battle, source, target, false);
			});
		}

		public override void ShareableActionPart(BattleGameData battle, ICombatant source, ICombatant target, bool isShared)
		{
			if ((!m_StunVsWet || target.CurrrentEffects.ContainsKey("WaterBomb")) && !(m_StunChance / 100f < UnityEngine.Random.value))
			{
				DebugLog.Log("Stunned");
				string effectIdent = "Stun";
				if (m_StunVsWet || m_Freeze)
				{
					effectIdent = "Frozen";
				}
				if (m_TentacleGrab)
				{
					effectIdent = "TentacleGrab";
				}
				List<float> list = new List<float>();
				list.Add(1f);
				List<float> values = list;
				target.CombatantView.PlayStunnedAnimation();
				BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, target, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.BeforeStartOfTurn,
						EffectType = BattleEffectType.Stun,
						AfflicionType = SkillEffectTypes.Curse,
						Values = values,
						Duration = (int)m_StunDuration,
						EffectAssetId = ((!m_UseSkillAssetId) ? "Stun" : base.Model.Balancing.EffectIconAssetId),
						EffectAtlasId = ((!m_UseSkillAssetId) ? "Skills_Generic" : base.Model.Balancing.EffectIconAtlasId)
					}
				}, (int)m_StunDuration, battle, effectIdent, SkillEffectTypes.Curse, GetLocalizedName(), base.Model.SkillNameId);
				battleEffectGameData.AddEffect();
				battleEffectGameData.EffectRemovedAction = delegate(BattleEffectGameData e)
				{
					e.m_Target.CombatantView.PlayIdle();
				};
				if (source.CombatantView.m_AssetController is TentacleAssetController)
				{
					battleEffectGameData.EffectRemovedAction = OnTentacleStunRemoved;
				}
				if (m_CleanseAfterDeath)
				{
					battleEffectGameData.SetPersistanceAfterDefeat(false);
				}
			}
		}

		private void OnTentacleStunRemoved(BattleEffectGameData stunEffect)
		{
			ICombatant target = stunEffect.m_Target;
			ICombatant source = stunEffect.m_Source;
			target.CombatantView.PlayIdle();
			(source.CombatantView.m_AssetController as TentacleAssetController).m_IsStunning = false;
			if (source.CurrentHealth > 0f)
			{
				if (source.IsStunned)
				{
					source.CombatantView.PlayStunnedAnimation();
				}
				else
				{
					source.CombatantView.PlayIdle();
				}
			}
			else
			{
				target.CombatantView.RefreshFromStun();
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num);
			dictionary.Add("{value_5}", string.Empty + m_ChargeTurns);
			dictionary.Add("{value_3}", string.Empty + m_StunChance);
			dictionary.Add("{value_4}", string.Empty + m_AttackCount);
			dictionary.Add("{value_2}", string.Empty + m_StunDuration);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

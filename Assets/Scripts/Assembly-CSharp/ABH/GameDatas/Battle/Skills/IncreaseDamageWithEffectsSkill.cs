using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class IncreaseDamageWithEffectsSkill : SkillBattleDataBase
	{
		protected float m_Percent;

		protected float m_Chance = 100f;

		protected float m_StunChance;

		protected float m_StunDuration;

		protected float m_DamageFriend;

		protected float m_ChargeTurns;

		protected bool m_All;

		protected bool m_Self;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("damage_increase", out m_Percent);
			base.Model.SkillParameters.TryGetValue("chance", out m_Chance);
			base.Model.SkillParameters.TryGetValue("stun_chance", out m_StunChance);
			base.Model.SkillParameters.TryGetValue("stun_duration", out m_StunDuration);
			base.Model.SkillParameters.TryGetValue("damage_friend", out m_DamageFriend);
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
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
				ICombatant target2 = target;
				m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target2.CombatantFaction).ToList());
			}
			float effectValue = 0f;
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			yield return new WaitForSeconds(source.CombatantView.PlaySupportAnimation());
			List<BattleEffect> battleEffectlist = new List<BattleEffect>();
			if (base.Model.SkillParameters.TryGetValue("damage_increase", out effectValue))
			{
				List<float> valueList2 = new List<float> { effectValue };
				battleEffectlist.Add(new BattleEffect
				{
					EffectTrigger = EffectTriggerType.OnDealDamage,
					EffectType = BattleEffectType.IncreaseDamage,
					AfflicionType = base.Model.Balancing.EffectType,
					Values = valueList2,
					Duration = base.Model.Balancing.EffectDuration,
					EffectAssetId = base.Model.Balancing.EffectIconAssetId,
					EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
				});
			}
			if (base.Model.SkillParameters.TryGetValue("stun_chance", out effectValue))
			{
				float stunDuration = 1f;
				base.Model.SkillParameters.TryGetValue("stun_duration", out stunDuration);
				List<float> valueList = new List<float> { effectValue, stunDuration };
				battleEffectlist.Add(new BattleEffect
				{
					EffectTrigger = EffectTriggerType.OnReceiveDamage,
					EffectType = BattleEffectType.StunAttacker,
					AfflicionType = base.Model.Balancing.EffectType,
					Values = valueList,
					Duration = base.Model.Balancing.EffectDuration,
					EffectAssetId = base.Model.Balancing.EffectIconAssetId,
					EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
				});
			}
			if (base.Model.SkillParameters.TryGetValue("damage_friend", out effectValue))
			{
				float damage = target.ModifiedHealth / (100f / effectValue);
				target.ReceiveDamage(damage, source);
				DIContainerLogic.GetBattleService().DealDamageFromCurrentTurn(target, battle, source, true);
				if (target.IsAlive)
				{
					yield return new WaitForSeconds(target.CombatantView.PlaySurprisedAnimation() + 0.25f);
				}
			}
			foreach (ICombatant skillTarget in m_Targets)
			{
				if (skillTarget != source && skillTarget.IsAlive)
				{
					skillTarget.CombatantView.PlayCheerCharacter();
				}
				BattleEffectGameData effect = new BattleEffectGameData(source, skillTarget, battleEffectlist, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
				effect.AddEffect();
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", m_Percent.ToString("0"));
			dictionary.Add("{value_2}", base.Model.Balancing.EffectDuration.ToString("0"));
			dictionary.Add("{value_3}", m_StunChance.ToString("0"));
			dictionary.Add("{value_4}", m_ChargeTurns.ToString("0"));
			dictionary.Add("{value_7}", m_StunChance.ToString("0"));
			dictionary.Add("{value_5}", m_StunDuration.ToString("0"));
			dictionary.Add("{value_6}", m_DamageFriend.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

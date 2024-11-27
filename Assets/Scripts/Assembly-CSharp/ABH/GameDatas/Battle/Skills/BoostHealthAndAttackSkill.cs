using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class BoostHealthAndAttackSkill : SkillBattleDataBase
	{
		protected float m_AttackPercent;

		protected float m_HealthPercent;

		protected float m_BannerHealthPercent;

		protected float m_GrowPercent;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("damage_increase", out m_AttackPercent);
			base.Model.SkillParameters.TryGetValue("total_health_bonus", out m_HealthPercent);
			base.Model.SkillParameters.TryGetValue("percent_on_banner", out m_BannerHealthPercent);
			base.Model.SkillParameters.TryGetValue("grow_percent", out m_GrowPercent);
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
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			yield return new WaitForSeconds(source.CombatantView.PlaySupportAnimation());
			List<BattleEffect> battleEffectlist = new List<BattleEffect>();
			if (m_AttackPercent > 0f)
			{
				List<float> valueList = new List<float> { m_AttackPercent };
				battleEffectlist.Add(new BattleEffect
				{
					EffectTrigger = EffectTriggerType.OnDealDamage,
					EffectType = BattleEffectType.IncreaseDamage,
					AfflicionType = base.Model.Balancing.EffectType,
					Values = valueList,
					Duration = base.Model.Balancing.EffectDuration,
					EffectAssetId = base.Model.Balancing.EffectIconAssetId,
					EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
				});
			}
			foreach (ICombatant skillTarget in m_Targets)
			{
				if (skillTarget != source)
				{
					skillTarget.CombatantView.PlayCheerCharacter();
				}
				int healthBonus3 = 0;
				if (m_BannerHealthPercent > 0f && skillTarget.IsBanner)
				{
					healthBonus3 = (int)(skillTarget.ModifiedHealth * (m_BannerHealthPercent / 100f));
					skillTarget.HealthBuff += healthBonus3;
					skillTarget.HealDamage(healthBonus3, source);
					DIContainerLogic.GetBattleService().HealCurrentTurn(skillTarget, battle, true, false, false, false, true, source);
				}
				else if (m_HealthPercent > 0f)
				{
					healthBonus3 = (int)(skillTarget.ModifiedHealth * (m_HealthPercent / 100f));
					skillTarget.HealthBuff += healthBonus3;
					skillTarget.HealDamage(healthBonus3, source);
					DIContainerLogic.GetBattleService().HealCurrentTurn(skillTarget, battle, true, false, false, false, true, source);
				}
				BattleEffectGameData effect = new BattleEffectGameData(source, skillTarget, battleEffectlist, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
				effect.AddEffect();
				skillTarget.CombatantView.GrowCharacter(m_GrowPercent / 100f);
				effect.EffectRemovedAction = delegate(BattleEffectGameData e)
				{
					OnEffectRemoved(e.m_Target, healthBonus3, battle);
				};
			}
		}

		private void OnEffectRemoved(ICombatant target, int healthBonus, BattleGameData battle)
		{
			target.CombatantView.GrowCharacter(0f - m_GrowPercent / 100f);
			target.HealthBuff -= healthBonus;
			target.CombatantView.UpdateHealthBar();
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", m_AttackPercent.ToString("0"));
			dictionary.Add("{value_2}", string.Empty + base.Model.Balancing.EffectDuration);
			dictionary.Add("{value_3}", m_HealthPercent.ToString("0"));
			dictionary.Add("{value_4}", m_BannerHealthPercent.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

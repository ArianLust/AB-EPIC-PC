using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class DamageFoesWithFriendsHP : AttackSkillTemplate
	{
		private float m_Percent1 = 100f;

		private float m_Percent2 = 100f;

		private float m_BannerPercent1 = 100f;

		private float m_BannerPercent2 = 100f;

		private string m_AddtionalEffectInfo = string.Empty;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("percent", out m_Percent1);
			base.Model.SkillParameters.TryGetValue("damage_friend", out m_Percent2);
			base.Model.SkillParameters.TryGetValue("percent_on_banner", out m_BannerPercent1);
			base.Model.SkillParameters.TryGetValue("damage_banner", out m_BannerPercent2);
			foreach (string key in base.Model.SkillParameters.Keys)
			{
				if (base.Model.SkillParameters[key] == -1f)
				{
					m_AddtionalEffectInfo = key;
				}
			}
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DebugLog.Log("Trigger support skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			ICombatant target2 = target;
			if (!base.Model.SkillParameters.ContainsKey("all"))
			{
				m_Targets = new List<ICombatant> { target };
			}
			else
			{
				m_Targets = new List<ICombatant>();
				m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target2.CombatantFaction).ToList());
			}
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			List<ICombatant> enemyList = new List<ICombatant>();
			enemyList.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction != target2.CombatantFaction).ToList());
			yield return new WaitForSeconds(source.CombatantView.PlaySupportAnimation());
			foreach (ICombatant skillTarget in m_Targets)
			{
				float bonusValue = m_Percent1 / 100f;
				float effectValue = m_Percent2;
				if (skillTarget.IsBanner)
				{
					bonusValue = m_BannerPercent1 / 100f;
					effectValue = m_BannerPercent2;
				}
				float damageToFriend = skillTarget.ModifiedHealth / (100f / effectValue);
				skillTarget.ReceiveDamage(damageToFriend, source);
				DIContainerLogic.GetBattleService().DealDamageFromCurrentTurn(skillTarget, battle, source, true);
				foreach (ICombatant enemy in enemyList)
				{
					float modificationreceived2 = 1f;
					float modificationdealt2 = 1f;
					float damageToEnemy2 = damageToFriend * bonusValue;
					m_Source.CurrentSkillAttackValue = damageToEnemy2;
					modificationdealt2 = DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(modificationdealt2, EffectTriggerType.OnDealDamage, m_Source, enemy);
					DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(1f, EffectTriggerType.BeforeReceiveDamage, enemy, source);
					damageToEnemy2 *= modificationdealt2;
					m_Source.CurrentSkillAttackValue = damageToEnemy2;
					modificationreceived2 = DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(modificationreceived2, EffectTriggerType.OnReceiveDamage, enemy, m_Source);
					enemy.ReceiveDamage(damageToEnemy2 * modificationreceived2, m_Source);
					DIContainerLogic.GetBattleService().DealDamageFromCurrentTurn(enemy, battle, m_Source, true);
				}
				if (skillTarget != source)
				{
					skillTarget.CombatantView.PlaySurprisedAnimation();
				}
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_3}", string.Empty + m_Percent1);
			dictionary.Add("{value_6}", string.Empty + m_Percent2);
			dictionary.Add("{value_7}", string.Empty + m_BannerPercent1);
			dictionary.Add("{value_8}", string.Empty + m_BannerPercent2);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class HealAndHotSkill : SkillBattleDataBase
	{
		protected float m_Percent;

		protected float m_Fixed;

		protected float m_HotChance;

		protected float m_HotPercent;

		protected bool m_All;

		protected bool m_Self;

		protected float m_InvokersHealth;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("health_in_percent", out m_Percent);
			base.Model.SkillParameters.TryGetValue("hot_in_percent", out m_HotPercent);
			base.Model.SkillParameters.TryGetValue("hot_chance", out m_HotChance);
			m_All = base.Model.SkillParameters.ContainsKey("all");
			m_Self = base.Model.SkillParameters.ContainsKey("self");
			if (base.Model.SkillParameters.ContainsKey("invoker"))
			{
				m_InvokersHealth = 1f;
			}
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DebugLog.Log("Trigger support skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			m_InitialTarget = target;
			if (!m_All)
			{
				if (m_Self)
				{
					m_Targets = new List<ICombatant> { source };
					m_InitialTarget = source;
				}
				else
				{
					m_Targets = new List<ICombatant> { target };
				}
			}
			else
			{
				m_Targets = new List<ICombatant>();
				ICombatant target2 = target;
				m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target2.CombatantFaction).ToList());
			}
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			yield return new WaitForSeconds(source.CombatantView.PlaySupportAnimation());
			foreach (ICombatant skillTarget in m_Targets)
			{
				float modifiedPercent = m_Percent;
				float referencedHealth = ((!(m_InvokersHealth >= 0f)) ? skillTarget.ModifiedHealth : m_Source.ModifiedHealth);
				skillTarget.HealDamage(referencedHealth * modifiedPercent / 100f, skillTarget);
				if (Random.value <= m_HotChance / 100f)
				{
					skillTarget.CombatantView.PlayCheerCharacter();
					List<float> valueList = new List<float> { m_HotPercent, m_InvokersHealth };
					BattleEffectGameData effect = new BattleEffectGameData(source, skillTarget, new List<BattleEffect>
					{
						new BattleEffect
						{
							EffectTrigger = EffectTriggerType.OnDealDamagePerTurn,
							EffectType = BattleEffectType.DoHeal,
							Values = valueList,
							AfflicionType = base.Model.Balancing.EffectType,
							Duration = base.Model.Balancing.EffectDuration,
							EffectAssetId = base.Model.Balancing.EffectIconAssetId,
							EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
						}
					}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, SkillEffectTypes.Blessing, GetLocalizedName(), base.Model.SkillNameId);
					effect.AddEffect();
				}
				DIContainerLogic.GetBattleService().HealCurrentTurn(skillTarget, battle, true, false, false, false, true, source);
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (m_InvokersHealth > 0f)
			{
				dictionary.Add("{value_1}", Mathf.RoundToInt(invoker.ModifiedHealth * m_Percent / 100f).ToString("0"));
				dictionary.Add("{value_7}", Mathf.RoundToInt(invoker.ModifiedHealth * m_HotPercent / 100f).ToString("0"));
			}
			else
			{
				dictionary.Add("{value_1}", m_Percent.ToString("0"));
				dictionary.Add("{value_7}", m_HotPercent.ToString("0"));
			}
			dictionary.Add("{value_2}", base.Model.Balancing.EffectDuration.ToString("0"));
			dictionary.Add("{value_3}", m_HotChance.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle.Skills
{
	public class PassiveIncreaseStatsAuraSet : SkillBattleDataBase
	{
		private float m_DamageIncrease;

		private float m_HealthIncrease;

		private float m_TargetRed;

		private float m_TargetYellow;

		private float m_TargetWhite;

		private float m_TargetBlack;

		private float m_TargetBlue;

		private bool m_TargetAllButself;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			model.SkillParameters.TryGetValue("increase_damage_in_percent", out m_DamageIncrease);
			model.SkillParameters.TryGetValue("increase_health_in_percent", out m_HealthIncrease);
			model.SkillParameters.TryGetValue("target_red_only", out m_TargetRed);
			model.SkillParameters.TryGetValue("target_yellow_only", out m_TargetYellow);
			model.SkillParameters.TryGetValue("target_white_only", out m_TargetWhite);
			model.SkillParameters.TryGetValue("target_black_only", out m_TargetBlack);
			model.SkillParameters.TryGetValue("target_blue_only", out m_TargetBlue);
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DoActionInstant(battle, source, target);
			yield break;
		}

		public override void DoActionInstant(BattleGameData battle, ICombatant source, ICombatant target)
		{
			DebugLog.Log("Trigger banner skill: " + base.Model.Balancing.NameId);
			m_Source = source;
			m_Targets = new List<ICombatant>();
			m_TargetAllButself = m_TargetRed + m_TargetYellow + m_TargetWhite + m_TargetBlack + m_TargetBlue == 0f;
			if (m_TargetAllButself)
			{
				m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target.CombatantFaction && c != m_Source).ToList());
			}
			else
			{
				foreach (ICombatant item in battle.m_CombatantsByInitiative)
				{
					if (item.CombatantFaction == m_Source.CombatantFaction)
					{
						if (item.CombatantNameId.Contains("_red") && m_TargetRed == 1f)
						{
							m_Targets.Add(item);
							break;
						}
						if (item.CombatantNameId.Contains("_yellow") && m_TargetYellow == 1f)
						{
							m_Targets.Add(item);
							break;
						}
						if (item.CombatantNameId.Contains("_white") && m_TargetWhite == 1f)
						{
							m_Targets.Add(item);
							break;
						}
						if (item.CombatantNameId.Contains("_black") && m_TargetBlack == 1f)
						{
							m_Targets.Add(item);
							break;
						}
						if (item.CombatantNameId.Contains("_blue") && m_TargetBlue == 1f)
						{
							m_Targets.Add(item);
							break;
						}
					}
				}
			}
			List<BattleEffect> list = new List<BattleEffect>();
			if (m_DamageIncrease > 0f)
			{
				List<float> list2 = new List<float>();
				list2.Add(m_DamageIncrease);
				List<float> values = list2;
				list.Add(new BattleEffect
				{
					EffectTrigger = EffectTriggerType.OnDealDamage,
					EffectType = BattleEffectType.IncreaseDamageSet,
					AfflicionType = base.Model.Balancing.EffectType,
					Values = values,
					Duration = base.Model.Balancing.EffectDuration,
					EffectAssetId = base.Model.Balancing.EffectIconAssetId,
					EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
				});
			}
			if (m_HealthIncrease > 0f)
			{
				List<float> list2 = new List<float>();
				list2.Add(m_HealthIncrease);
				List<float> values2 = list2;
				list.Add(new BattleEffect
				{
					EffectTrigger = EffectTriggerType.Instant,
					EffectType = BattleEffectType.IncreaseHealthPermanentOnceSet,
					AfflicionType = base.Model.Balancing.EffectType,
					Values = values2,
					Duration = base.Model.Balancing.EffectDuration,
					EffectAssetId = base.Model.Balancing.EffectIconAssetId,
					EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
				});
			}
			foreach (ICombatant target2 in m_Targets)
			{
				BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, target2, list, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
				battleEffectGameData.AddEffect();
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			WorldBalancingData worldBalancingData = DIContainerBalancing.Service.GetBalancingDataList<WorldBalancingData>().FirstOrDefault();
			dictionary.Add("{value_1}", m_DamageIncrease.ToString("0"));
			dictionary.Add("{value_2}", m_HealthIncrease.ToString("0"));
			if (m_TargetRed == 1f)
			{
				dictionary.Add("{value_3}", DIContainerInfrastructure.GetLocaService().GetCharacterName("bird_red"));
			}
			else if (m_TargetYellow == 1f)
			{
				dictionary.Add("{value_3}", DIContainerInfrastructure.GetLocaService().GetCharacterName("bird_yellow"));
			}
			else if (m_TargetWhite == 1f)
			{
				dictionary.Add("{value_3}", DIContainerInfrastructure.GetLocaService().GetCharacterName("bird_white"));
			}
			else if (m_TargetBlack == 1f)
			{
				dictionary.Add("{value_3}", DIContainerInfrastructure.GetLocaService().GetCharacterName("bird_black"));
			}
			else if (m_TargetBlue == 1f)
			{
				dictionary.Add("{value_3}", DIContainerInfrastructure.GetLocaService().GetCharacterName("bird_blue"));
			}
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle.Skills
{
	public class HealOverTime : SkillBattleDataBase
	{
		private float m_FixedHeal;

		private float m_Delay;

		private float m_Random;

		private float m_Percentage;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			model.SkillParameters.TryGetValue("fixed_heal", out m_FixedHeal);
			model.SkillParameters.TryGetValue("delay_in_turns", out m_Delay);
			model.SkillParameters.TryGetValue("choose_random_combatant", out m_Random);
			model.SkillParameters.TryGetValue("percentage", out m_Percentage);
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			yield break;
		}

		public override void DoActionInstant(BattleGameData battle, ICombatant source, ICombatant target)
		{
			if (!source.IsAlive)
			{
				return;
			}
			DebugLog.Log("Trigger environmental skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			List<ICombatant> list = new List<ICombatant>();
			if (base.Model.SkillParameters.ContainsKey("all"))
			{
				list.AddRange(battle.m_CombatantsPerFaction[target.CombatantFaction].ToList());
			}
			else
			{
				list.Add(target);
			}
			foreach (ICombatant item in list)
			{
				if (m_Percentage == 0f)
				{
					List<float> list2 = new List<float>();
					list2.Add(m_FixedHeal);
					list2.Add(m_Delay);
					list2.Add(m_Random);
					List<float> values = list2;
					BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, item, new List<BattleEffect>
					{
						new BattleEffect
						{
							EffectTrigger = EffectTriggerType.OnDealDamagePerTurn,
							EffectType = BattleEffectType.DoFixedHealDelayed,
							AfflicionType = base.Model.Balancing.EffectType,
							Values = values,
							Duration = base.Model.Balancing.EffectDuration,
							EffectAssetId = base.Model.Balancing.EffectIconAssetId,
							EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
						}
					}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
					battleEffectGameData.AddEffect();
				}
				else
				{
					List<float> list2 = new List<float>();
					list2.Add(m_Percentage);
					list2.Add(m_Delay);
					list2.Add(100f);
					List<float> values2 = list2;
					BattleEffectGameData battleEffectGameData2 = new BattleEffectGameData(source, item, new List<BattleEffect>
					{
						new BattleEffect
						{
							EffectTrigger = EffectTriggerType.OnDealDamagePerTurn,
							EffectType = BattleEffectType.DoHealDelayed,
							AfflicionType = base.Model.Balancing.EffectType,
							Values = values2,
							Duration = base.Model.Balancing.EffectDuration,
							EffectAssetId = base.Model.Balancing.EffectIconAssetId,
							EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
						}
					}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
					battleEffectGameData2.SetPersistanceAfterDefeat(false);
					battleEffectGameData2.AddEffect();
				}
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			WorldBalancingData worldBalancingData = DIContainerBalancing.Service.GetBalancingDataList<WorldBalancingData>().FirstOrDefault();
			dictionary.Add("{value_1}", m_FixedHeal.ToString("0"));
			dictionary.Add("{value_2}", m_Delay.ToString("0"));
			dictionary.Add("{value_3}", m_Percentage.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

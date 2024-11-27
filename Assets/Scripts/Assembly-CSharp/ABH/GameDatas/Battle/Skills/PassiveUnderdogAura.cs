using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle.Skills
{
	public class PassiveUnderdogAura : SkillBattleDataBase
	{
		private float m_AttackBuff;

		private float m_HealthBuff;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			model.SkillParameters.TryGetValue("increase_damage_in_percent", out m_AttackBuff);
			model.SkillParameters.TryGetValue("increase_health_in_percent", out m_HealthBuff);
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
			m_Targets.AddRange(battle.m_CombatantsPerFaction[target.CombatantFaction].Where((ICombatant c) => c != m_Source).ToList());
			if (m_Targets.Count > 2)
			{
				return;
			}
			List<BattleEffect> list = new List<BattleEffect>();
			if (m_AttackBuff > 0f)
			{
				List<float> list2 = new List<float>();
				list2.Add(m_AttackBuff);
				List<float> values = list2;
				list.Add(new BattleEffect
				{
					EffectTrigger = EffectTriggerType.OnDealDamage,
					EffectType = BattleEffectType.IncreaseDamage,
					AfflicionType = base.Model.Balancing.EffectType,
					Values = values,
					Duration = base.Model.Balancing.EffectDuration,
					EffectAssetId = base.Model.Balancing.EffectIconAssetId,
					EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
				});
			}
			if (m_HealthBuff > 0f)
			{
				List<float> list2 = new List<float>();
				list2.Add(m_HealthBuff);
				List<float> values2 = list2;
				list.Add(new BattleEffect
				{
					EffectTrigger = EffectTriggerType.Instant,
					EffectType = BattleEffectType.IncreaseHealthPermanentOnce,
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
			dictionary.Add("{value_1}", m_AttackBuff.ToString("0"));
			dictionary.Add("{value_2}", m_HealthBuff.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

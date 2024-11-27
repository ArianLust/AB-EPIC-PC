using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle.Skills
{
	public class PassiveIncreaseBirdDamageOnLowHP : SkillBattleDataBase
	{
		private float m_DamageIncrease;

		private float m_HpThreshold;

		private float m_HealIncrease;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			model.SkillParameters.TryGetValue("damage_increase", out m_DamageIncrease);
			model.SkillParameters.TryGetValue("hp_treshold", out m_HpThreshold);
			model.SkillParameters.TryGetValue("heal_increase", out m_HealIncrease);
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
			m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target.CombatantFaction && c != m_Source).ToList());
			List<float> list = new List<float>();
			list.Add(m_DamageIncrease);
			list.Add(m_HpThreshold);
			List<float> values = list;
			foreach (ICombatant target2 in m_Targets)
			{
				BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, target2, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.OnDealDamage,
						EffectType = BattleEffectType.ModifyDamageByHealthTreshold,
						AfflicionType = base.Model.Balancing.EffectType,
						Values = values,
						Duration = base.Model.Balancing.EffectDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
				battleEffectGameData.AddEffect();
			}
			list = new List<float>();
			list.Add(m_HealIncrease);
			list.Add(m_HpThreshold);
			values = list;
			foreach (ICombatant target3 in m_Targets)
			{
				BattleEffectGameData battleEffectGameData2 = new BattleEffectGameData(source, target3, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.OnSupportHealUsed,
						EffectType = BattleEffectType.ModifyHealingByHealthTreshold,
						AfflicionType = base.Model.Balancing.EffectType,
						Values = values,
						Duration = base.Model.Balancing.EffectDuration
					}
				}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId + "_heal", base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
				battleEffectGameData2.AddEffect();
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", DIContainerInfrastructure.GetFormatProvider().GetBattleStatsFractionalFormat(m_DamageIncrease));
			dictionary.Add("{value_2}", m_HpThreshold.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

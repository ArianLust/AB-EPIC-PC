using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle.Skills
{
	public class DamageAndStun : SkillBattleDataBase
	{
		private float m_StunDuration;

		private float m_Delay;

		private float m_Random;

		private float m_Chance = 100f;

		private float m_FixedDamage;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			model.SkillParameters.TryGetValue("stun_duration", out m_StunDuration);
			model.SkillParameters.TryGetValue("stun_chance", out m_Chance);
			model.SkillParameters.TryGetValue("delay_in_turns", out m_Delay);
			model.SkillParameters.TryGetValue("fixed_damage", out m_FixedDamage);
			model.SkillParameters.TryGetValue("choose_random_combatant", out m_Random);
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			yield break;
		}

		public override void DoActionInstant(BattleGameData battle, ICombatant source, ICombatant target)
		{
			DebugLog.Log("Trigger environmental skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			string effectIdent = base.Model.Balancing.AssetId + "_damage";
			List<float> list = new List<float>();
			list.Add(m_FixedDamage);
			list.Add(m_Delay);
			list.Add(m_Random);
			list.Add(m_Chance);
			list.Add(m_StunDuration);
			List<float> values = list;
			BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, target, new List<BattleEffect>
			{
				new BattleEffect
				{
					EffectTrigger = EffectTriggerType.OnDealDamagePerTurn,
					EffectType = BattleEffectType.DealDamageAndApplyStunDelayed,
					AfflicionType = base.Model.Balancing.EffectType,
					Values = values,
					Duration = base.Model.Balancing.EffectDuration,
					EffectAssetId = base.Model.Balancing.EffectIconAssetId,
					EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
				}
			}, base.Model.Balancing.EffectDuration, battle, effectIdent, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
			battleEffectGameData.AddEffect();
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			WorldBalancingData worldBalancingData = DIContainerBalancing.Service.GetBalancingDataList<WorldBalancingData>().FirstOrDefault();
			dictionary.Add("{value_1}", m_StunDuration.ToString("0"));
			dictionary.Add("{value_2}", m_Delay.ToString("0"));
			dictionary.Add("{value_3}", m_FixedDamage.ToString("0"));
			dictionary.Add("{value_4}", m_Chance.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class BirdShieldAfterCoinloss : SkillBattleDataBase
	{
		private float m_Duration;

		private float m_Chance;

		private float m_DamageReduction = 100f;

		private bool m_wasTriggered;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			model.SkillParameters.TryGetValue("chance", out m_Chance);
			model.SkillParameters.TryGetValue("damage_reduction", out m_DamageReduction);
			model.SkillParameters.TryGetValue("duration", out m_Duration);
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DoActionInstant(battle, source, target);
			yield break;
		}

		public override void DoActionInstant(BattleGameData battle, ICombatant source, ICombatant target)
		{
			m_Source = source;
			m_Battle = battle;
			if (m_wasTriggered)
			{
				return;
			}
			m_wasTriggered = true;
			if (m_Chance < (float)Random.Range(0, 100))
			{
				return;
			}
			m_Targets = new List<ICombatant>();
			m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target.CombatantFaction && !c.IsBanner).ToList());
			if ((source.CombatantFaction != Faction.Pigs || battle.m_PigsStartTurn) && (source.CombatantFaction != 0 || !battle.m_PigsStartTurn))
			{
				return;
			}
			int duration = ((source.CombatantFaction != Faction.Pigs) ? ((int)m_Duration) : ((int)m_Duration - 1));
			foreach (ICombatant target2 in m_Targets)
			{
				BattleEffectGameData battleEffectGameData = new BattleEffectGameData(m_Source, target2, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.OnReceiveDamage,
						EffectType = BattleEffectType.ReduceDamageReceived,
						AfflicionType = SkillEffectTypes.Blessing,
						Values = new List<float> { m_DamageReduction },
						Duration = duration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.IconAtlasId
					}
				}, duration, m_Battle, base.Model.Balancing.AssetId, SkillEffectTypes.SetPassive, base.Model.SkillLocalizedName, base.Model.Balancing.AssetId);
				battleEffectGameData.AddEffect();
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", m_Chance.ToString("0"));
			dictionary.Add("{value_2}", m_DamageReduction.ToString("0"));
			dictionary.Add("{value_3}", m_Duration.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

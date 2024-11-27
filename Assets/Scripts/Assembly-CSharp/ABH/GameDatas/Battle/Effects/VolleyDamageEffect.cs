using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle.Effects
{
	public class VolleyDamageEffect : BattleEffectDataBase
	{
		protected override void Init()
		{
			m_triggerTypes = new List<EffectTriggerType> { EffectTriggerType.OnReceiveDamage };
		}

		public override float ApplyBattleEffect(EffectTriggerType trigger, BattleGameData battle, float param, BattleEffectGameData effectGameData, BattleEffect singleEffect, ICombatant attacker)
		{
			if (attacker != null)
			{
				DIContainerLogic.GetBattleService().LogEffect("Triggered by: " + attacker.CombatantNameId + "_" + attacker.CurrentInitiative.ToString("00"));
			}
			float modifiedAttack = effectGameData.m_Source.ModifiedAttack;
			float effectedParam = 1f;
			effectedParam = DIContainerLogic.GetBattleService().ApplyEffectsOfTypeOnTriggerType(effectedParam, new List<BattleEffectType>
			{
				BattleEffectType.ReduceDamageReceived,
				BattleEffectType.IncreaseDamageReceived
			}, EffectTriggerType.OnReceiveDamage, attacker, effectGameData.m_Target);
			effectGameData.EvaluateEffect();
			effectGameData.m_Target.ReceiveDamage(effectedParam * modifiedAttack * singleEffect.Values[0] / 100f, effectGameData.m_Source);
			DIContainerLogic.GetBattleService().DealDamageFromCurrentTurn(effectGameData.m_Target, battle, effectGameData.m_Source);
			return param;
		}
	}
}

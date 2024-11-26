using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle.Effects
{
	public class ThornEffect : BattleEffectDataBase
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
			float num = singleEffect.Values[0];
			float effectedParam = 1f;
			effectedParam = DIContainerLogic.GetBattleService().ApplyEffectsOfTypeOnTriggerType(effectedParam, new List<BattleEffectType>
			{
				BattleEffectType.ReduceDamageReceived,
				BattleEffectType.IncreaseDamageReceived
			}, EffectTriggerType.OnReceiveDamage, attacker, effectGameData.m_Target);
			effectGameData.EvaluateEffect();
			if (attacker != null)
			{
				attacker.ReceiveDamage(effectedParam * num, effectGameData.m_Target);
				DIContainerLogic.GetBattleService().DealDamageFromCurrentTurn(attacker, effectGameData.m_Battle, effectGameData.m_Source);
			}
			return param;
		}
	}
}

using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle
{
	public abstract class BattleEffectDataBase
	{
		protected List<EffectTriggerType> m_triggerTypes = new List<EffectTriggerType>();

		protected abstract void Init();

		public float ApplyBattleEffectIfNecessary(EffectTriggerType trigger, BattleGameData battle, float param, BattleEffectGameData effectGameData, BattleEffect singleEffect, ICombatant attacker)
		{
			if (m_triggerTypes.Contains(trigger))
			{
				DIContainerLogic.GetBattleService().LogEffect(string.Concat("E: ", GetType(), " Tr: ", trigger, " Own: ", effectGameData.m_Target.CombatantNameId, "_", effectGameData.m_Target.CurrentInitiative.ToString("00")));
				return ApplyBattleEffect(trigger, battle, param, effectGameData, singleEffect, attacker);
			}
			return param;
		}

		public abstract float ApplyBattleEffect(EffectTriggerType trigger, BattleGameData battle, float param, BattleEffectGameData effectGameData, BattleEffect singleEffect, ICombatant attacker);
	}
}

using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class SimpleAttackWithPurgeSkill : AttackSkillTemplate
	{
		public override void Init(SkillGameData model)
		{
			base.Init(model);
			ModificationsOnDamageDealtCalculation.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				ShareableActionPart(battle, source, target, false);
				return damage;
			});
		}

		public override void ShareableActionPart(BattleGameData battle, ICombatant source, ICombatant target, bool isShared)
		{
			bool flag = false;
			float value = 0f;
			if (base.Model.SkillParameters.TryGetValue("purge", out value))
			{
				flag = Random.value <= value / 100f;
			}
			if (flag && target.CurrrentEffects != null)
			{
				List<BattleEffectGameData> list = target.CurrrentEffects.Values.Where((BattleEffectGameData e) => e.m_EffectType == SkillEffectTypes.Blessing).ToList();
				for (int num = list.Count - 1; num >= 0; num--)
				{
					list[num].RemoveEffect();
				}
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, new Dictionary<string, string>());
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

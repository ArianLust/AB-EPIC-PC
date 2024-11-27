using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle.Skills
{
	public class PassiveReduceCharge : SkillBattleDataBase
	{
		private float m_TurnReduce;

		private float m_Chance;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("reduce_charge_turns", out m_TurnReduce);
			base.Model.SkillParameters.TryGetValue("chance", out m_Chance);
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DoActionInstant(battle, source, target);
			yield break;
		}

		public override void DoActionInstant(BattleGameData battle, ICombatant source, ICombatant target)
		{
			DebugLog.Log("Trigger passive skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			foreach (ICombatant item in battle.m_CombatantsPerFaction[source.CombatantFaction].Where((ICombatant c) => c.IsAlive))
			{
				List<float> list = new List<float>();
				list.Add(m_TurnReduce);
				list.Add(m_Chance);
				List<float> values = list;
				BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, item, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.Instant,
						EffectType = BattleEffectType.ReduceCharge,
						Values = values,
						AfflicionType = base.Model.Balancing.EffectType,
						Duration = base.Model.Balancing.EffectDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, SkillEffectTypes.Blessing, GetLocalizedName(), base.Model.SkillNameId);
				battleEffectGameData.AddEffect();
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", string.Empty + m_TurnReduce);
			dictionary.Add("{value_2}", string.Empty + m_Chance);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

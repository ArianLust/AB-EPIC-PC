using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle.Skills
{
	public class SpawnPigsOnDeath : SkillBattleDataBase
	{
		private float m_SummonedAmount;

		private string m_TableKey;

		private bool m_Spawn;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			m_Spawn = base.Model.SkillParameters.ContainsKey("spawn");
			m_TableKey = base.Model.SkillParameters.Keys.FirstOrDefault();
			base.Model.SkillParameters.TryGetValue(m_TableKey, out m_SummonedAmount);
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DoActionInstant(battle, source, target);
			yield break;
		}

		public override void DoActionInstant(BattleGameData battle, ICombatant source, ICombatant target)
		{
			DebugLog.Log("Trigger passive skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			List<float> list = new List<float>();
			if (m_Spawn)
			{
				list.Add(1f);
			}
			else
			{
				list.Add(0f);
			}
			list.Add(m_SummonedAmount);
			BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, target, new List<BattleEffect>
			{
				new BattleEffect
				{
					EffectTrigger = EffectTriggerType.OnAllHealthLost,
					EffectType = BattleEffectType.SummonCombatant,
					AfflicionType = base.Model.Balancing.EffectType,
					Values = list,
					Duration = base.Model.Balancing.EffectDuration,
					EffectAssetId = base.Model.Balancing.EffectIconAssetId,
					EffectAtlasId = base.Model.Balancing.EffectIconAtlasId,
					extraString = m_TableKey
				}
			}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
			battleEffectGameData.AddEffect();
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", string.Empty + m_SummonedAmount);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

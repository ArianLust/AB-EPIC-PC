using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle.Skills
{
	public class PassiveMocking : SkillBattleDataBase
	{
		private float m_ChanceToTaunt;

		private float m_TauntDuration = 1f;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			model.SkillParameters.TryGetValue("chance_each_turn", out m_ChanceToTaunt);
			model.SkillParameters.TryGetValue("duration", out m_TauntDuration);
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
			List<float> list = new List<float>();
			list.Add(m_ChanceToTaunt);
			list.Add(m_TauntDuration);
			List<float> values = list;
			BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, target, new List<BattleEffect>
			{
				new BattleEffect
				{
					EffectTrigger = EffectTriggerType.OnSupportSkillReceived,
					EffectType = BattleEffectType.Mocking,
					AfflicionType = base.Model.Balancing.EffectType,
					Values = values,
					Duration = base.Model.Balancing.EffectDuration,
					EffectAssetId = base.Model.Balancing.EffectIconAssetId,
					EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
				}
			}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
			battleEffectGameData.AddEffect();
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", m_ChanceToTaunt.ToString("0"));
			dictionary.Add("{value_2}", m_TauntDuration.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle.Skills
{
	public class PassiveBirdShelterAura : SkillBattleDataBase
	{
		private float m_ChanceToBlock;

		private bool m_Red;

		private bool m_Yellow;

		private bool m_White;

		private bool m_Black;

		private bool m_Blue;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			model.SkillParameters.TryGetValue("chance_to_block", out m_ChanceToBlock);
			m_Red = (base.Model.SkillParameters.ContainsKey("red_block") ? true : false);
			m_Yellow = (base.Model.SkillParameters.ContainsKey("yellow_block") ? true : false);
			m_White = (base.Model.SkillParameters.ContainsKey("white_block") ? true : false);
			m_Black = (base.Model.SkillParameters.ContainsKey("black_block") ? true : false);
			m_Blue = (base.Model.SkillParameters.ContainsKey("blue_block") ? true : false);
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
			list.Add(0f);
			list.Add(m_ChanceToBlock);
			List<float> values = list;
			List<ICombatant> list2 = new List<ICombatant>();
			foreach (ICombatant item in battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target.CombatantFaction))
			{
				if (m_Red && item.CombatantNameId.Contains("bird_red"))
				{
					list2.Add(item);
				}
				if (m_Yellow && item.CombatantNameId.Contains("bird_yellow"))
				{
					list2.Add(item);
				}
				if (m_White && item.CombatantNameId.Contains("bird_white"))
				{
					list2.Add(item);
				}
				if (m_Black && item.CombatantNameId.Contains("bird_black"))
				{
					list2.Add(item);
				}
				if (m_Blue && item.CombatantNameId.Contains("bird_blue"))
				{
					list2.Add(item);
				}
			}
			foreach (ICombatant item2 in list2)
			{
				ICombatant source2 = item2;
				BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source2, source, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.AfterTargetSelection,
						EffectType = BattleEffectType.Sheltered,
						AfflicionType = base.Model.Balancing.EffectType,
						Values = values,
						Duration = base.Model.Balancing.EffectDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
				battleEffectGameData.SetPersistanceAfterDefeat(false).AddEffect();
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			WorldBalancingData worldBalancingData = DIContainerBalancing.Service.GetBalancingDataList<WorldBalancingData>().FirstOrDefault();
			dictionary.Add("{value_1}", m_ChanceToBlock.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

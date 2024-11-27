using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle.Skills
{
	public class HealGroupPerTurn : SkillBattleDataBase
	{
		private float m_HealPercentage;

		private string m_TableKey;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			model.SkillParameters.TryGetValue("heal_in_percent", out m_HealPercentage);
			m_TableKey = base.Model.SkillParameters.Keys.FirstOrDefault();
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DoActionInstant(battle, source, target);
			yield break;
		}

		public override void DoActionInstant(BattleGameData battle, ICombatant source, ICombatant target)
		{
			if (!source.IsAlive)
			{
				return;
			}
			DebugLog.Log("Trigger passive skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			BattleParticipantTableBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<BattleParticipantTableBalancingData>(m_TableKey);
			List<string> list = new List<string>();
			foreach (BattleParticipantTableEntry battleParticipant in balancingData.BattleParticipants)
			{
				list.Add(battleParticipant.NameId);
			}
			m_Source = source;
			m_Targets = new List<ICombatant>();
			m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target.CombatantFaction).ToList());
			foreach (ICombatant target2 in m_Targets)
			{
				if (list.Contains(target2.CombatantNameId))
				{
					List<float> list2 = new List<float>();
					list2.Add(m_HealPercentage);
					List<float> values = list2;
					BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, target2, new List<BattleEffect>
					{
						new BattleEffect
						{
							EffectTrigger = EffectTriggerType.OnHealPerTurn,
							EffectType = BattleEffectType.DoHeal,
							AfflicionType = base.Model.Balancing.EffectType,
							Values = values,
							Duration = base.Model.Balancing.EffectDuration,
							EffectAssetId = base.Model.Balancing.EffectIconAssetId,
							EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
						}
					}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
					battleEffectGameData.SetPersistanceAfterDefeat(false);
					battleEffectGameData.AddEffect();
				}
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", m_HealPercentage.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

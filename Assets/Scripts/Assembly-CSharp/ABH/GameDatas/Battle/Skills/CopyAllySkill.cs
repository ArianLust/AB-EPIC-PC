using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class CopyAllySkill : SkillBattleDataBase
	{
		private float m_HitpointsPercentage;

		private float m_IncludeBuffs;

		private float m_IncludeDebuffs;

		private float m_ChargeTurns;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
			base.Model.SkillParameters.TryGetValue("percentage_of_hitpoints", out m_HitpointsPercentage);
			base.Model.SkillParameters.TryGetValue("include_buffs", out m_IncludeBuffs);
			base.Model.SkillParameters.TryGetValue("include_debuffs", out m_IncludeDebuffs);
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			m_Source = source;
			m_Targets = new List<ICombatant>();
			if (EvaluateCharge(battle, source, m_Targets, target))
			{
				yield break;
			}
			if (target is BossCombatant)
			{
				source.CombatantView.PlayMournAnimation();
				yield break;
			}
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			yield return new WaitForSeconds(source.CombatantView.PlaySupportAnimation());
			BattleParticipantTableBalancingData waveBalancing2 = DIContainerBalancing.Service.GetBalancingData<BattleParticipantTableBalancingData>("environmental_summon_table_21");
			BattleParticipantTableBalancingData waveBalancing = new BattleParticipantTableBalancingData();
			waveBalancing.NameId = "copy_ally";
			waveBalancing.VictoryCondition = new VictoryCondition
			{
				Type = VictoryConditionTypes.DefeatAll,
				NameId = string.Empty,
				Value = 1f
			};
			waveBalancing.Type = BattleParticipantTableType.IgnoreStrength;
			waveBalancing.BattleParticipants = new List<BattleParticipantTableEntry>();
			waveBalancing.BattleParticipants.Add(new BattleParticipantTableEntry
			{
				Amount = 1f,
				NameId = target.CombatantNameId,
				LevelDifference = 0,
				Probability = 1f,
				ForcePercent = false,
				Unique = false
			});
			List<ICombatant> summonedList2 = new List<ICombatant>();
			summonedList2 = DIContainerLogic.GetBattleService().GenerateSummonsWeighted(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == m_Source.CombatantFaction).ToList(), waveBalancing, battle, 1, DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").MaxPigsInBattle);
			foreach (ICombatant summon in summonedList2)
			{
				if (!battle.m_CombatantsPerFaction.ContainsKey(summon.CombatantFaction))
				{
					battle.m_CombatantsPerFaction.Add(summon.CombatantFaction, new List<ICombatant>());
				}
				summon.CurrentInitiative = m_Source.CurrentInitiative + 1;
				battle.m_CombatantsPerFaction[summon.CombatantFaction].Add(summon);
				summon.HasUsageDelay = true;
				summon.summoningType = SummoningType.Summoned;
			}
			if (summonedList2.Count == 0)
			{
				source.CombatantView.m_AssetController.PlayMournAnim();
				yield return new WaitForSeconds(source.CombatantView.m_AssetController.GetMournAnimationLength());
				yield break;
			}
			yield return m_Source.CombatantView.m_BattleMgr.StartCoroutine(m_Source.CombatantView.m_BattleMgr.PlaceCharacter(m_Source.CombatantView.m_BattleMgr.m_PigCenterPosition, Faction.Pigs));
			yield return m_Source.CombatantView.m_BattleMgr.StartCoroutine(m_Source.CombatantView.m_BattleMgr.SpawnPigs());
			foreach (PigCombatant pig in summonedList2)
			{
				if (pig.PassiveSkill != null)
				{
					pig.CombatantView.StartCoroutine(pig.PassiveSkill.DoAction(battle, pig, pig));
					pig.IsAttacking = false;
				}
				float newHealth = pig.ModifiedHealth / (100f / m_HitpointsPercentage);
				pig.SetHealthWithoutLogic(newHealth);
				foreach (BattleEffectGameData effectGameData in target.CurrrentEffects.Values)
				{
					if (effectGameData.m_EffectType == SkillEffectTypes.Curse && m_IncludeDebuffs == 1f)
					{
						BattleEffectGameData copyEffect = new BattleEffectGameData(effectGameData);
						copyEffect.m_Target = pig;
						copyEffect.AddEffect();
					}
					else if (effectGameData.m_EffectType == SkillEffectTypes.Blessing && m_IncludeBuffs == 1f)
					{
						BattleEffectGameData copyEffect2 = new BattleEffectGameData(effectGameData);
						copyEffect2.m_Target = pig;
						copyEffect2.AddEffect();
					}
				}
				pig.CombatantView.PlayIdle();
			}
			DIContainerLogic.GetBattleService().AddPassiveEffects(battle);
			m_Source.CombatantView.m_BattleMgr.SpawnHealthBars();
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", string.Empty + m_HitpointsPercentage);
			dictionary.Add("{value_2}", string.Empty + m_ChargeTurns);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_2}", string.Empty + m_ChargeTurns);
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, dictionary);
		}
	}
}

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
	public class SummonDrone : SkillBattleDataBase
	{
		private float m_DamageMod;

		private float m_ChargeTurns;

		private float m_AttackCount;

		private float m_SummonedAmount;

		private string m_TableKey;

		private bool m_Spawn;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
			m_Spawn = base.Model.SkillParameters.ContainsKey("spawn");
			m_TableKey = base.Model.SkillParameters.Keys.FirstOrDefault();
			base.Model.SkillParameters.TryGetValue(m_TableKey, out m_SummonedAmount);
			switch (Random.Range(0, 5))
			{
			case 0:
				m_TableKey += "_red";
				break;
			case 1:
				m_TableKey += "_yellow";
				break;
			case 2:
				m_TableKey += "_white";
				break;
			case 3:
				m_TableKey += "_black";
				break;
			case 4:
				m_TableKey += "_blues";
				break;
			}
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			m_Source = source;
			m_Targets = new List<ICombatant>();
			if (EvaluateCharge(battle, source, m_Targets, target))
			{
				m_Source.CombatantView.targetSheltered = null;
				yield break;
			}
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			yield return new WaitForSeconds(source.CombatantView.PlaySupportAnimation());
			BattleParticipantTableBalancingData waveBalancing = DIContainerBalancing.Service.GetBalancingData<BattleParticipantTableBalancingData>(m_TableKey);
			List<PigGameData> pigList = new List<PigGameData>();
			List<ICombatant> summonedList2 = new List<ICombatant>();
			foreach (BattleParticipantTableEntry entry in waveBalancing.BattleParticipants)
			{
				pigList.Add(new PigGameData(entry.NameId).SetDifficulties(battle.GetPlayerLevelForHotSpot(), battle.Balancing));
			}
			summonedList2 = DIContainerLogic.GetBattleService().GenerateSummonsWeighted(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == m_Source.CombatantFaction).ToList(), waveBalancing, battle, (int)m_SummonedAmount, DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").MaxPigsInBattle);
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
			if (m_Spawn)
			{
				yield return m_Source.CombatantView.m_BattleMgr.StartCoroutine(m_Source.CombatantView.m_BattleMgr.SpawnPigs());
			}
			else
			{
				yield return m_Source.CombatantView.m_BattleMgr.StartCoroutine(m_Source.CombatantView.m_BattleMgr.EnterPigs(0f));
			}
			foreach (PigCombatant pig in summonedList2)
			{
				if (pig.PassiveSkill != null)
				{
					pig.CombatantView.StartCoroutine(pig.PassiveSkill.DoAction(battle, pig, pig));
					pig.IsAttacking = false;
				}
			}
			DIContainerLogic.GetBattleService().AddPassiveEffects(battle);
			m_Source.CombatantView.m_BattleMgr.SpawnHealthBars();
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", string.Empty + m_SummonedAmount);
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

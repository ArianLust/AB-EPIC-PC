using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using SmoothMoves;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class RageYellowBird : SkillBattleDataBase
	{
		private int m_AttackCount;

		protected float m_Percent;

		protected float m_Fixed;

		protected bool m_All;

		protected float m_ChargeRounds;

		protected bool m_SpawnImage;

		private bool m_ImpactDone;

		private CharacterAssetController m_copy;

		private ICombatant m_currentOriginal;

		private List<ICombatant> m_possibleAttackingCharacters;

		public override void BoneAnimationUserTrigger(UserTriggerEvent triggerEvent)
		{
			if (triggerEvent.tag == "Impact")
			{
				DebugLog.Log("Impact occured");
				SpawnVisualEffects(VisualEffectSpawnTiming.Impact, m_VisualEffectSetting);
			}
		}

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			float value = 0f;
			model.SkillParameters.TryGetValue("attack_count", out value);
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out m_Percent);
			m_AttackCount = (int)value;
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			List<ICombatant> aliveEnemiesBefore = battle.m_CombatantsPerFaction[Faction.Pigs].Where((ICombatant c) => c.IsAlive).ToList();
			DebugLog.Log("Trigger attack skill: " + base.Model.Balancing.NameId + "; Attack: " + target.CombatantName);
			m_Source = source;
			m_possibleAttackingCharacters = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target.CombatantFaction && !c.IsStunned && c.IsParticipating).ToList();
			m_possibleAttackingCharacters.Shuffle();
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			yield return new WaitForSeconds(source.CombatantView.PlayRageSkillAnimation());
			battle.SetRageAvailable(source.CombatantFaction, false);
			if (source.CombatantFaction == Faction.Pigs)
			{
				battle.SetFactionRage(Faction.Pigs, 0f);
				battle.RegisterRageUsed(100f, source);
			}
			for (int i = 0; i < m_AttackCount; i++)
			{
				if (m_possibleAttackingCharacters.Count <= 0)
				{
					break;
				}
				m_possibleAttackingCharacters = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target.CombatantFaction && !c.IsStunned && c.IsParticipating && !c.IsBanner).ToList();
				if (m_possibleAttackingCharacters.Count <= 0)
				{
					break;
				}
				ICombatant skillTarget = m_possibleAttackingCharacters[i % m_possibleAttackingCharacters.Count];
				List<ICombatant> possibleEnemies = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction != skillTarget.CombatantFaction).ToList();
				if (possibleEnemies.Count <= 0)
				{
					continue;
				}
				ICombatant orderTarget = possibleEnemies[Random.Range(0, possibleEnemies.Count)];
				SkillBattleDataBase skill = skillTarget.GetSkills().FirstOrDefault((SkillBattleDataBase s) => s.Model.Balancing.TargetType == SkillTargetTypes.Attack);
				if (skill == null)
				{
					continue;
				}
				m_copy = null;
				if (m_SpawnImage)
				{
					m_copy = DIContainerInfrastructure.GetCharacterAssetProvider(false).InstantiateObject(skillTarget.CombatantAssetId, null, Vector3.zero, Quaternion.identity).GetComponent<CharacterAssetController>();
					m_copy.transform.position = new Vector3(skillTarget.CombatantView.transform.position.x + Mathf.Sign(skillTarget.CombatantView.transform.position.x) * 100f, skillTarget.CombatantView.transform.position.y, skillTarget.CombatantView.transform.position.z);
					m_copy.transform.parent = skillTarget.CombatantView.m_BattleMgr.m_BattleArea;
					m_copy.transform.rotation = skillTarget.CombatantView.transform.rotation;
					m_currentOriginal = skillTarget;
					m_copy.SetModel(skillTarget.CharacterModel, false);
					yield return new WaitForSeconds(0.5f);
				}
				List<float> valueList = new List<float> { 100f - m_Percent };
				BattleEffectGameData effect = new BattleEffectGameData(source, skillTarget, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.OnDealDamage,
						EffectType = BattleEffectType.ReduceDamageDealt,
						Values = valueList,
						AfflicionType = base.Model.Balancing.EffectType,
						Duration = base.Model.Balancing.EffectDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, SkillEffectTypes.Blessing, GetLocalizedName(), base.Model.SkillNameId);
				effect.AddEffect();
				if ((bool)skillTarget.CombatantView.m_AssetController)
				{
					skillTarget.CombatantView.m_AssetController.m_BoneAnimation.UnregisterUserTriggerDelegate(skill.BoneAnimationUserTrigger);
					skillTarget.CombatantView.m_AssetController.m_BoneAnimation.RegisterUserTriggerDelegate(skill.BoneAnimationUserTrigger);
				}
				ICombatant cachedTarget = skillTarget.AttackTarget;
				skillTarget.AttackTarget = orderTarget;
				DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(0f, EffectTriggerType.OnTarget, skillTarget, skillTarget.AttackTarget);
				skillTarget.Defeated -= skillTarget_Defeated;
				skillTarget.Defeated += skillTarget_Defeated;
				PigCombatant pig = skillTarget as PigCombatant;
				if (pig != null && pig.ChachedSkill != null && skill.EvaluateCharge(battle, pig, new List<ICombatant> { pig.AttackTarget }, null))
				{
					yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeForPigToChooseTargetAndDoSkillInSec);
				}
				DebugLog.Log("Character: " + skillTarget.CombatantNameId + " starts his attack!");
				yield return skillTarget.CombatantView.StartCoroutine(skill.DoAction(battle, skillTarget, orderTarget));
				skillTarget.IsAttacking = false;
				skillTarget.AttackTarget = cachedTarget;
				DebugLog.Log("Character: " + skillTarget.CombatantNameId + " finished his attack!");
				if ((bool)skillTarget.CombatantView.m_AssetController)
				{
					skillTarget.CombatantView.m_AssetController.m_BoneAnimation.UnregisterUserTriggerDelegate(skill.BoneAnimationUserTrigger);
				}
				effect.RemoveEffect();
				skillTarget.Defeated -= skillTarget_Defeated;
				if (skillTarget.IsParticipating)
				{
					if (skillTarget is BirdCombatant)
					{
						yield return new WaitForSeconds(skillTarget.CombatantView.PlayGoToFocusPosition());
					}
					else
					{
						yield return new WaitForSeconds(skillTarget.CombatantView.PlayGoToBasePosition());
					}
				}
				else if (skillTarget == source)
				{
					battle.SetRageAvailable(source.CombatantFaction, true);
					yield break;
				}
			}
			battle.SetRageAvailable(source.CombatantFaction, true);
			if (battle.IsPvP && source.CombatantFaction == Faction.Birds && !battle.IsUnranked)
			{
				CalculateRageKills(battle, aliveEnemiesBefore);
			}
		}

		private void CalculateRageKills(BattleGameData battle, List<ICombatant> aliveEnemiesBefore)
		{
			List<ICombatant> list = battle.m_CombatantsPerFaction[Faction.Pigs].Where((ICombatant c) => c.IsAlive).ToList();
			foreach (ICombatant item in list)
			{
				aliveEnemiesBefore.Remove(item);
			}
			if (aliveEnemiesBefore.Count <= 0)
			{
				return;
			}
			foreach (ICombatant item2 in aliveEnemiesBefore)
			{
				DIContainerLogic.GetPvpObjectivesService().RageUsedToKill(item2);
			}
		}

		private void skillTarget_Defeated()
		{
			if (m_currentOriginal != null)
			{
				m_currentOriginal.Defeated -= skillTarget_Defeated;
			}
			if (m_SpawnImage && (bool)m_copy && m_currentOriginal != null)
			{
				Object.Destroy(m_copy.gameObject);
			}
			for (int num = m_possibleAttackingCharacters.Count - 1; num >= 0; num--)
			{
				if (!m_possibleAttackingCharacters[num].IsParticipating)
				{
					m_possibleAttackingCharacters.RemoveAt(num);
				}
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", string.Empty + m_AttackCount);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

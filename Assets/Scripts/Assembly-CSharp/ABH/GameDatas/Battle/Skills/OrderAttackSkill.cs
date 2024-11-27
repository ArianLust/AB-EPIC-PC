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
	public class OrderAttackSkill : SkillBattleDataBase
	{
		protected float m_Percent;

		protected float m_Fixed;

		protected bool m_All;

		protected bool m_AllButSelf;

		protected bool m_FocusTarget;

		protected float m_ChargeRounds;

		protected bool m_SpawnImage;

		private bool m_ImpactDone;

		private bool m_ShareEffect;

		private CharacterAssetController m_copy;

		private ICombatant m_currentOriginal;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out m_Percent);
			m_All = base.Model.SkillParameters.ContainsKey("all");
			m_AllButSelf = base.Model.SkillParameters.ContainsKey("all_but_self");
			m_FocusTarget = base.Model.SkillParameters.ContainsKey("focus_target");
			m_SpawnImage = base.Model.SkillParameters.ContainsKey("image");
			m_ShareEffect = base.Model.SkillParameters.ContainsKey("share_effect");
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeRounds);
		}

		public override void BoneAnimationUserTrigger(UserTriggerEvent triggerEvent)
		{
			base.BoneAnimationUserTrigger(triggerEvent);
			if (triggerEvent.tag == "Impact")
			{
				m_ImpactDone = true;
			}
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			m_ImpactDone = false;
			DebugLog.Log("Trigger support skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			m_InitialTarget = target;
			ICombatant source2 = source;
			if (m_AllButSelf)
			{
				m_Targets = new List<ICombatant>();
				m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == source2.CombatantFaction && !c.IsStunned && c.IsParticipating && c != source2).ToList());
			}
			else if (!m_All)
			{
				if (target == null || !target.IsParticipating || target.IsStunned || target.IsBanner)
				{
					m_Targets = new List<ICombatant>();
				}
				else
				{
					m_Targets = new List<ICombatant> { target };
				}
			}
			else
			{
				m_Targets = new List<ICombatant>();
				m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == source2.CombatantFaction && !c.IsStunned && c.IsParticipating).ToList());
			}
			if (EvaluateCharge(battle, source, m_Targets, target))
			{
				m_Source.CombatantView.targetSheltered = null;
				yield break;
			}
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			if (m_Targets.Count == 0)
			{
				source.CombatantView.PlayMournAnimation();
				yield break;
			}
			source.CombatantView.PlaySupportAnimation();
			while (!m_ImpactDone)
			{
				yield return new WaitForEndOfFrame();
			}
			foreach (ICombatant skillTarget in m_Targets)
			{
				List<ICombatant> possibleEnemies = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction != skillTarget.CombatantFaction).ToList();
				if (possibleEnemies.Count <= 0)
				{
					continue;
				}
				ICombatant orderTarget = possibleEnemies[Random.Range(0, possibleEnemies.Count)];
				if (m_FocusTarget)
				{
					orderTarget = m_InitialTarget;
				}
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
					m_copy.transform.parent = skillTarget.CombatantView.transform.parent;
					m_copy.transform.rotation = skillTarget.CombatantView.transform.rotation;
					m_currentOriginal = skillTarget;
					m_copy.SetModel(skillTarget.CharacterModel, false);
					switch (skillTarget.CombatantNameId)
					{
					default:
						if (skillTarget.CombatantFaction != Faction.Pigs)
						{
							break;
						}
						goto case "bird_prince_porky";
					case "bird_prince_porky":
					case "bird_merchant":
					case "bird_adventurer":
						if (m_copy != null)
						{
							ScaleController sc = m_copy.GetComponent<ScaleController>();
							if (sc != null)
							{
								sc.m_BaseScale = new Vector3(-1f, 1f, 1f);
							}
						}
						break;
					}
					yield return new WaitForSeconds(0.5f);
				}
				if (m_SpawnImage && (bool)m_copy)
				{
					skillTarget.Defeated -= skillTarget_Defeated;
					skillTarget.Defeated += skillTarget_Defeated;
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
				ICombatant previoustarget = skillTarget.AttackTarget;
				skillTarget.AttackTarget = orderTarget;
				DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(0f, EffectTriggerType.OnTarget, skillTarget, skillTarget.AttackTarget);
				PigCombatant pig = skillTarget as PigCombatant;
				if (pig != null)
				{
					DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(battle.m_CurrentTurn, EffectTriggerType.OnCharge, pig, null);
				}
				yield return skillTarget.CombatantView.StartCoroutine(skill.DoAction(battle, skillTarget, orderTarget));
				skillTarget.IsAttacking = false;
				if (m_ShareEffect)
				{
					List<ICombatant> otherCombatants = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.IsParticipating && c.CombatantFaction == skillTarget.AttackTarget.CombatantFaction && c != skillTarget.AttackTarget).ToList();
					if (otherCombatants.Count > 0)
					{
						foreach (ICombatant otherCombatant in otherCombatants)
						{
							skill.ShareableActionPart(battle, skillTarget, otherCombatant, true);
						}
					}
				}
				if ((bool)skillTarget.CombatantView.m_AssetController)
				{
					skillTarget.CombatantView.m_AssetController.m_BoneAnimation.UnregisterUserTriggerDelegate(skill.BoneAnimationUserTrigger);
				}
				if (pig != null)
				{
					List<BattleEffectGameData> chargeEffects = pig.CurrrentEffects.Values.Where((BattleEffectGameData e) => e.m_Effects.Any((BattleEffect ef) => ef.EffectTrigger == EffectTriggerType.OnCharge)).ToList();
					if (pig is PigCombatant && chargeEffects.Count > 0)
					{
						pig.CombatantView.PlayAffectedAnimation();
						for (int i = chargeEffects.Count - 1; i >= 0; i--)
						{
							BattleEffectGameData effectGameData = chargeEffects[i];
							effectGameData.IncrementCurrentTurnManual(false);
							if (pig.ChargeDone)
							{
								effectGameData.IncrementCurrentTurnManual(false);
							}
							yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeForFocusInituativeAndDOTS);
						}
					}
				}
				if (skillTarget is BirdCombatant)
				{
					yield return new WaitForSeconds(skillTarget.CombatantView.PlayGoToFocusPosition());
				}
				else
				{
					yield return new WaitForSeconds(skillTarget.CombatantView.PlayGoToBasePosition());
				}
				if (m_SpawnImage && (bool)m_copy)
				{
					skillTarget.Defeated -= skillTarget_Defeated;
					m_copy.PlayAnimation("Move_Once");
					CHMotionTween motion = m_copy.gameObject.AddComponent<CHMotionTween>();
					motion.m_DurationInSeconds = DIContainerLogic.GetPacingBalancing().TimeFromBasePosToFocusPosInSec;
					motion.m_EndTransform = skillTarget.CombatantView.transform;
					motion.Play();
					yield return new WaitForSeconds(motion.m_DurationInSeconds);
					Object.Destroy(m_copy.gameObject);
				}
				skillTarget.AttackTarget = previoustarget;
				effect.RemoveEffect();
			}
		}

		private void skillTarget_Defeated()
		{
			if (m_SpawnImage && (bool)m_copy && m_currentOriginal != null)
			{
				m_currentOriginal.Defeated -= skillTarget_Defeated;
				Object.Destroy(m_copy.gameObject);
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_3}", m_Percent.ToString("0"));
			dictionary.Add("{value_2}", m_ChargeRounds.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using SmoothMoves;
using UnityEngine;

namespace ABH.GameDatas.Battle
{
	public abstract class SkillBattleDataBase
	{
		private SkillGameData m_Model;

		protected VisualEffectSetting m_VisualEffectSetting;

		protected BubbleSetting m_BubbleSetting;

		protected BattleGameData m_Battle;

		protected ICombatant m_InitialTarget;

		protected bool m_ForceSourceIcon;

		protected string m_SkillProjectileAssetId = string.Empty;

		public bool m_IsRageSkill;

		public int m_SkillStartTurn;

		public int m_CurrentTurn;

		public ICombatant m_Source;

		public List<ICombatant> m_Targets;

		public ICombatant m_InitialTargetSelection;

		public SkillGameData Model
		{
			get
			{
				return m_Model;
			}
		}

		public SkillBattleDataBase()
		{
		}

		public SkillBattleDataBase(SkillGameData model)
		{
			Init(model);
		}

		public virtual void BoneAnimationUserTrigger(UserTriggerEvent triggerEvent)
		{
			if (triggerEvent.tag == "Movement")
			{
				SpawnVisualEffects(VisualEffectSpawnTiming.Movement, m_VisualEffectSetting);
			}
			if (triggerEvent.tag == "Impact")
			{
				DebugLog.Log("Impact occured");
				SpawnVisualEffects(VisualEffectSpawnTiming.Impact, m_VisualEffectSetting);
			}
			if (triggerEvent.tag == "Projectile")
			{
				DebugLog.Log("Projectile fired");
				SpawnProjectile(m_Battle, m_Source, m_Targets, m_SkillProjectileAssetId);
			}
		}

		public void SpawnProjectile(BattleGameData battle, ICombatant source, List<ICombatant> targets, string skillProjectileAssetId, bool useOffhand = false)
		{
			if (string.IsNullOrEmpty(skillProjectileAssetId))
			{
				DebugLog.Log("no asset ID");
				return;
			}
			Transform transform = ((!useOffhand || !(source.CombatantView.m_AssetController.OffHandBone != null)) ? source.CombatantView.m_AssetController.MainHandBone : source.CombatantView.m_AssetController.OffHandBone);
			if (targets != null && targets.Count == 1)
			{
				ICombatant combatant = targets[0];
				GameObject gameObject = DIContainerInfrastructure.ProjectileAssetProvider.InstantiateObject(skillProjectileAssetId, source.CombatantView.m_BattleMgr.m_BattleArea, transform.position, Quaternion.identity);
				if (gameObject == null)
				{
					return;
				}
				gameObject.transform.position = transform.position;
				gameObject.transform.localScale = source.CombatantView.m_AssetController.transform.localScale;
				UnityHelper.SetLayerRecusively(gameObject, source.CombatantView.m_AssetController.gameObject.layer);
				CHMotionTween componentInChildren = gameObject.GetComponentInChildren<CHMotionTween>();
				if (!componentInChildren)
				{
					Object.Destroy(gameObject);
					return;
				}
				if (!combatant.CombatantView || !combatant.CombatantView.m_AssetController.BodyCenter)
				{
					DebugLog.Error("Combatant view or its Body Center is Missing!");
					Object.Destroy(gameObject);
					return;
				}
				componentInChildren.m_EndTransform = combatant.CombatantView.m_AssetController.BodyCenter;
				if (componentInChildren.m_EndTransform.position.y < gameObject.transform.position.y)
				{
					componentInChildren.InvertCurves(true);
				}
				componentInChildren.Play();
				Object.Destroy(gameObject, componentInChildren.m_DurationInSeconds);
			}
			else
			{
				if (targets == null || targets.Count <= 1)
				{
					return;
				}
				GameObject gameObject2 = DIContainerInfrastructure.ProjectileAssetProvider.InstantiateObject(skillProjectileAssetId, source.CombatantView.m_BattleMgr.m_BattleArea, transform.position, Quaternion.identity);
				gameObject2.transform.position = transform.position;
				gameObject2.transform.localScale = source.CombatantView.m_AssetController.transform.localScale;
				UnityHelper.SetLayerRecusively(gameObject2, source.CombatantView.m_AssetController.gameObject.layer);
				CHMotionTween componentInChildren2 = gameObject2.GetComponentInChildren<CHMotionTween>();
				if (!componentInChildren2)
				{
					Object.Destroy(gameObject2);
					return;
				}
				componentInChildren2.m_EndTransform = ((source.CombatantFaction != 0) ? source.CombatantView.m_BattleMgr.m_BirdCenterPosition : source.CombatantView.m_BattleMgr.m_PigCenterPosition);
				if (componentInChildren2.m_EndTransform.position.y < gameObject2.transform.position.y)
				{
					componentInChildren2.InvertCurves(true);
				}
				componentInChildren2.Play();
				Object.Destroy(gameObject2, componentInChildren2.m_DurationInSeconds);
			}
		}

		public int GetChargeDuration()
		{
			float value = 0f;
			if (Model.SkillParameters != null && Model.SkillParameters.TryGetValue("charge", out value))
			{
				return (int)value;
			}
			return 0;
		}

		public void SetChargeDuration(int turns)
		{
			if (Model.SkillParameters != null && Model.SkillParameters.ContainsKey("charge"))
			{
				m_Model = new SkillGameData(m_Model);
				Model.SkillParameters["charge"] = turns;
			}
		}

		public bool TryGetChargeDurationLeft(out int duration, ICombatant source)
		{
			duration = 0;
			BattleEffectGameData value = null;
			if (Model.SkillParameters != null && source != null && source.CurrrentEffects != null && Model.SkillParameters.ContainsKey("charge") && source.CurrrentEffects.TryGetValue("Charge_" + Model.Balancing.AssetId, out value))
			{
				duration = value.GetTurnsLeft();
				return true;
			}
			return false;
		}

		public virtual void ShareableActionPart(BattleGameData battle, ICombatant source, ICombatant target, bool isShared)
		{
		}

		public abstract IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false);

		public virtual void Init(SkillGameData model)
		{
			m_Model = model;
			DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting(m_Model.Balancing.AssetId, out m_VisualEffectSetting);
			DIContainerLogic.GetVisualEffectsBalancing().TryGetBubbleSetting(m_Model.Balancing.AssetId, out m_BubbleSetting);
		}

		public virtual void DoActionInstant(BattleGameData battle, ICombatant source, ICombatant target)
		{
		}

		public void SpawnBubble(int duration)
		{
			if (m_BubbleSetting != null && m_Source != null)
			{
				SpawnBubble(m_BubbleSetting, duration);
			}
		}

		public void SpawnBubble(BubbleSetting setting, int duration)
		{
			if (setting != null && m_Source != null)
			{
				m_Source.CombatantView.InstantiateBubble(setting, m_Targets, m_Source, duration);
			}
		}

		public void SpawnChargeBubble(int duration, List<ICombatant> targets, ICombatant source, BattleGameData battle)
		{
			if (source != null)
			{
				BubbleSetting bubbleSetting = new BubbleSetting();
				bubbleSetting.TargetCombatant = VisualEffectTargetCombatant.Origin;
				if (battle.IsPvP && source == targets[0] && !source.IsAlive)
				{
					bubbleSetting.Type = BubbleType.KnockedOut;
				}
				else
				{
					bubbleSetting.Type = BubbleType.Large;
				}
				bubbleSetting.CharacterIcon = true;
				bubbleSetting.BalancingId = "Charge_" + Model.Balancing.AssetId;
				bubbleSetting.ShowDuration = true;
				bubbleSetting.AtAll = targets != null && targets.Count > 0 && Model.SkillParameters.ContainsKey("all");
				bubbleSetting.AtPigs = targets == null || targets.FirstOrDefault().CombatantFaction == Faction.Pigs;
				source.CombatantView.InstantiateBubble(bubbleSetting, targets, source, duration);
			}
		}

		public void SpawnVisualEffects(VisualEffectSpawnTiming timing, VisualEffectSetting visualEffectSetting, List<ICombatant> targets, bool isCurse = false)
		{
			if (visualEffectSetting == null)
			{
				return;
			}
			foreach (VisualEffect visualEffect in visualEffectSetting.VisualEffects)
			{
				if (visualEffect.SpawnTiming != timing)
				{
					continue;
				}
				if (m_Source.CombatantView != null)
				{
					m_Source.CombatantView.m_BattleMgr.InstantiateEffect(m_Source, visualEffect, visualEffectSetting, targets, isCurse);
				}
				else if (targets.Count > 0)
				{
					ICombatant combatant = targets.FirstOrDefault();
					if (combatant != null && combatant.CombatantView != null)
					{
						combatant.CombatantView.m_BattleMgr.InstantiateEffect(m_Source, visualEffect, visualEffectSetting, targets, isCurse);
					}
				}
			}
		}

		public void SpawnVisualEffects(VisualEffectSpawnTiming timing, VisualEffectSetting visualEffectSetting, bool isCurse = false)
		{
			if (visualEffectSetting == null)
			{
				return;
			}
			foreach (VisualEffect visualEffect in visualEffectSetting.VisualEffects)
			{
				if (visualEffect.SpawnTiming != timing)
				{
					continue;
				}
				if (m_Source != null && m_Source.CombatantView != null)
				{
					m_Source.CombatantView.m_BattleMgr.InstantiateEffect(m_Source, visualEffect, visualEffectSetting, m_Targets, isCurse);
				}
				else if (m_Targets != null && m_Targets.Count > 0)
				{
					ICombatant combatant = m_Targets.FirstOrDefault();
					if (combatant != null && combatant.CombatantView != null)
					{
						combatant.CombatantView.m_BattleMgr.InstantiateEffect(m_Source, visualEffect, visualEffectSetting, m_Targets, isCurse);
					}
				}
			}
		}

		public bool EvaluateCharge(BattleGameData battle, ICombatant source, List<ICombatant> targets, ICombatant initialSelectionTarget)
		{
			if (Model.SkillParameters == null || !Model.SkillParameters.ContainsKey("charge"))
			{
				return false;
			}
			BattleEffectGameData value = null;
			if (source.CurrrentEffects != null && !source.CurrrentEffects.TryGetValue("Charge_" + Model.Balancing.AssetId, out value))
			{
				bool flag = false;
				float num = 1f;
				foreach (BattleEffectGameData value2 in source.CurrrentEffects.Values)
				{
					foreach (BattleEffect effect in value2.m_Effects)
					{
						if (effect.EffectType == BattleEffectType.ReduceCharge && value2.m_Source.IsAlive && effect.Values[1] >= (float)Random.Range(0, 100))
						{
							flag = true;
							num = effect.Values[0];
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
				if (flag)
				{
					SetChargeDuration((int)Model.SkillParameters["charge"] - (int)num);
				}
				else if (!battle.IsPvP)
				{
					SetChargeDuration((int)Model.Balancing.SkillParameters["charge"]);
				}
			}
			if (source.ChargeDone)
			{
				source.ChargeDone = false;
				source.ChachedSkill = null;
				m_InitialTargetSelection = null;
				if (source.CurrrentEffects != null && source.CurrrentEffects.TryGetValue("Charge_" + Model.Balancing.AssetId, out value))
				{
					value.IncrementCurrentTurnManual(true);
				}
				return false;
			}
			if (source.CurrrentEffects != null && source.CurrrentEffects.TryGetValue("Charge_" + Model.Balancing.AssetId, out value))
			{
				if (value.GetTurnsLeft() <= 1)
				{
					source.ChargeDone = true;
					return EvaluateCharge(battle, source, (!m_ForceSourceIcon) ? targets : new List<ICombatant> { source }, initialSelectionTarget);
				}
				value.IncrementCurrentTurnManual(true);
				return true;
			}
			source.ChachedSkill = this;
			m_InitialTargetSelection = initialSelectionTarget;
			List<float> list = new List<float>();
			list.Add(0f);
			List<float> values = list;
			BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, source, new List<BattleEffect>
			{
				new BattleEffect
				{
					Duration = (int)Model.SkillParameters["charge"],
					EffectTrigger = EffectTriggerType.OnCharge,
					EffectType = BattleEffectType.Charge,
					AfflicionType = SkillEffectTypes.None,
					Values = values
				}
			}, (int)Model.SkillParameters["charge"], battle, "Charge_" + Model.Balancing.AssetId, SkillEffectTypes.None, GetLocalizedName(), Model.SkillNameId);
			SpawnChargeBubble(battleEffectGameData.GetTurnsLeft(), (!m_ForceSourceIcon) ? targets : new List<ICombatant> { source }, source, battle);
			battleEffectGameData.SetManualIncrement(true).AddEffect();
			return true;
		}

		public abstract string GetLocalizedDescription(ICombatant invoker);

		public virtual bool IsPseudoPerk()
		{
			return false;
		}

		public abstract string GetLocalizedName();

		public virtual string GetLocalizedName(ICombatant invoker)
		{
			return GetLocalizedName();
		}

		public virtual SkillBattleDataBase CheckForReplacement(ICombatant invoker)
		{
			return this;
		}
	}
}

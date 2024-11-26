using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.GameDatas.Interfaces;
using ABH.Services.Logic;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle
{
	public class BattleEffectGameData
	{
		public List<BattleEffect> m_Effects;

		public BattleGameData m_Battle;

		public SkillEffectTypes m_EffectType;

		public Action<BattleEffectGameData> EffectRemovedAction;

		public Action<BattleEffectGameData> EffectAppliedAction;

		public string m_EffectIdent;

		public ICombatant m_Target;

		public ICombatant m_Source;

		public string m_SkillIdent;

		public string m_LocalizedName;

		public string m_IconAssetId;

		public string m_IconAtlasId;

		public bool m_IsRemoved;

		public bool m_HasManualTurns;

		public bool m_TargetAll;

		private bool m_PersistIfDefeated = true;

		private int m_StartTurn;

		private int m_EndTurn;

		private int m_Duration;

		private int m_LastTurn;

		private int currentTurn;

		private BattleEffectGameData m_LinkedEffect;

		[method: MethodImpl(32)]
		public event Action<BattleEffectGameData> EffectRemoved;

		public BattleEffectGameData(BattleEffectGameData original, ICombatant source = null, ICombatant target = null)
		{
			m_Effects = new List<BattleEffect>();
			foreach (BattleEffect effect in original.m_Effects)
			{
				m_Effects.Add(new BattleEffect
				{
					AfflicionType = effect.AfflicionType,
					Duration = effect.Duration,
					EffectAssetId = effect.EffectAssetId,
					EffectAtlasId = effect.EffectAtlasId,
					EffectTrigger = effect.EffectTrigger,
					EffectType = effect.EffectType,
					Values = effect.Values
				});
			}
			m_Battle = original.m_Battle;
			m_EffectType = original.m_EffectType;
			m_EffectIdent = original.m_EffectIdent;
			if (target != null)
			{
				m_Target = target;
			}
			else
			{
				m_Target = original.m_Target;
			}
			if (source != null)
			{
				m_Source = source;
			}
			else
			{
				m_Source = original.m_Source;
			}
			m_SkillIdent = original.m_SkillIdent;
			m_LocalizedName = original.m_LocalizedName;
			m_IconAssetId = original.m_IconAssetId;
			m_IconAtlasId = original.m_IconAtlasId;
			m_IsRemoved = original.m_IsRemoved;
			m_HasManualTurns = original.m_HasManualTurns;
			m_TargetAll = original.m_TargetAll;
			m_LastTurn = original.m_LastTurn;
			m_StartTurn = original.m_StartTurn;
			m_Duration = original.m_Duration;
			m_EndTurn = original.m_EndTurn;
			currentTurn = original.currentTurn;
			InitActions(m_Source, m_Battle);
		}

		public BattleEffectGameData(ICombatant source, ICombatant target, List<BattleEffect> effects, int duration, BattleGameData battle, string effectIdent, SkillEffectTypes type, string locaIdent, string skillIdent)
			: this(source, target, source, effects, duration, battle, effectIdent, type, locaIdent, skillIdent)
		{
		}

		public BattleEffectGameData(ICombatant source, ICombatant target, ICombatant caster, List<BattleEffect> effects, int duration, BattleGameData battle, string effectIdent, SkillEffectTypes type, string locaIdent, string skillIdent)
		{
			m_Effects = effects;
			m_Battle = battle;
			m_SkillIdent = skillIdent;
			m_LocalizedName = locaIdent;
			m_EffectType = type;
			m_Target = target;
			m_Source = source;
			m_StartTurn = battle.m_CurrentTurn;
			m_IconAssetId = effects.FirstOrDefault().EffectAssetId;
			m_IconAtlasId = effects.FirstOrDefault().EffectAtlasId;
			m_Duration = duration;
			m_EndTurn = m_Duration;
			m_EffectIdent = effectIdent;
			InitActions(source, battle);
			m_LastTurn = battle.m_CurrentTurn;
		}

		private void InitActions(ICombatant source, BattleGameData battle)
		{
			if (source.CombatantFaction == Faction.Birds)
			{
				battle.BirdsTurnStarted -= OnInvokerStartTurn;
				battle.BirdsTurnStarted += OnInvokerStartTurn;
			}
			else if (source.CombatantFaction == Faction.Pigs)
			{
				battle.PigsTurnStarted -= OnInvokerStartTurn;
				battle.PigsTurnStarted += OnInvokerStartTurn;
				battle.WaveDoneForEffect -= OnInvokerStartTurn;
				battle.WaveDoneForEffect += OnInvokerStartTurn;
			}
			source.Defeated -= OnInvokerDefeated;
			source.Defeated += OnInvokerDefeated;
			source.KnockedOut -= OnInvokerDefeated;
			source.KnockedOut += OnInvokerDefeated;
		}

		public BattleEffectGameData SetPersistanceAfterDefeat(bool persist)
		{
			m_PersistIfDefeated = persist;
			return this;
		}

		public BattleEffectGameData SetTargetAll(bool targetAll)
		{
			m_TargetAll = targetAll;
			return this;
		}

		public void IncrementDuration()
		{
			m_Duration++;
		}

		private void OnInvokerDefeated()
		{
			if (m_Effects.Any((BattleEffect e) => e.EffectType == BattleEffectType.Taunt) || !m_PersistIfDefeated)
			{
				RemoveEffect();
				return;
			}
			m_Source.Defeated -= OnInvokerDefeated;
			m_Source.KnockedOut -= OnInvokerDefeated;
		}

		public BattleEffectGameData SetManualIncrement(bool manualIncrement)
		{
			m_HasManualTurns = manualIncrement;
			return this;
		}

		public void UpdateChargeTurnLeft()
		{
			UpdateEffect(GetTurnsLeft());
		}

		public void IncreaseChargeByTurns(int turns)
		{
			currentTurn -= turns;
			if (currentTurn < 0)
			{
				currentTurn = 0;
			}
			UpdateEffect(GetTurnsLeft());
		}

		public void IncrementCurrentTurnManual(bool remove)
		{
			m_HasManualTurns = true;
			m_LastTurn = currentTurn;
			currentTurn++;
			UpdateEffect(GetTurnsLeft());
			if (m_Duration >= 0 && currentTurn >= m_Duration)
			{
				RemoveEffect(false, !remove);
			}
		}

		public void OnInvokerStartTurn(int turn)
		{
			if (m_Target != null && (!m_Target.IsStunned || !m_Effects.Any((BattleEffect e) => e.AfflicionType == SkillEffectTypes.None)))
			{
				if (!m_HasManualTurns)
				{
					currentTurn += turn - m_LastTurn;
				}
				m_LastTurn = turn;
				UpdateEffect(m_Duration - currentTurn);
				if (m_Duration >= 0 && currentTurn >= m_Duration)
				{
					RemoveEffect();
				}
			}
		}

		public int GetTurnsLeft()
		{
			return Mathf.Max(m_Duration - currentTurn, 0);
		}

		private void UpdateEffect(int turn)
		{
			foreach (BattleEffect effect in m_Effects)
			{
				effect.Duration = GetTurnsLeft();
			}
			if (m_Target.CombatantView != null && m_Target.CombatantView.m_SpeechBubbles.ContainsKey(m_EffectIdent))
			{
				CharacterSpeechBubble characterSpeechBubble = m_Target.CombatantView.m_SpeechBubbles[m_EffectIdent];
				characterSpeechBubble.UpdateBubbleTurnValue(GetTurnsLeft());
				if (m_Target.IsParticipating && (m_EffectType == SkillEffectTypes.Blessing || m_EffectType == SkillEffectTypes.Curse))
				{
					m_Source.CombatantView.PlayAffectedAnimation();
				}
			}
		}

		public bool AddEffect(bool withVisuals = true)
		{
			if (m_EffectIdent == "SinisterSmite")
			{
				foreach (string key in m_Target.CurrrentEffects.Keys)
				{
					if (key == "SinisterSmite" && m_Target.CurrrentEffects[key].currentTurn == 0)
					{
						return false;
					}
				}
			}
			AddEffectValue(m_Target, m_EffectIdent, this);
			if (DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(0f, EffectTriggerType.OnAddEffect, m_Target, m_Target) > 0f)
			{
				return true;
			}
			DIContainerLogic.GetBattleService().LogDebug("Added E: " + m_EffectIdent + " to " + m_Target.CombatantNameId, BattleLogTypes.BattleEffect);
			if (withVisuals)
			{
				VisualEffectSetting setting = null;
				if (m_EffectType == SkillEffectTypes.Curse && m_Target.CombatantView != null && m_Target.IsParticipating && m_Target.CombatantFaction == Faction.Pigs && !m_Target.IsPvPBird)
				{
					m_Target.CombatantView.PlayAffectedAnimationQueued();
				}
				if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting(m_EffectIdent, out setting))
				{
					SpawnVisualEffects(VisualEffectSpawnTiming.Affected, setting, m_EffectType == SkillEffectTypes.Curse);
				}
			}
			return true;
		}

		public void SpawnVisualEffects(VisualEffectSpawnTiming timing, VisualEffectSetting visualEffectSetting, bool isCurse)
		{
			if (visualEffectSetting == null)
			{
				return;
			}
			foreach (VisualEffect visualEffect in visualEffectSetting.VisualEffects)
			{
				if (visualEffect.SpawnTiming == timing)
				{
					if (m_Target != null && m_Target.CombatantView != null)
					{
						m_Target.CombatantView.m_BattleMgr.InstantiateEffect(m_Source, visualEffect, visualEffectSetting, new List<ICombatant> { m_Target }, isCurse);
					}
					else if (m_Source.CombatantView != null)
					{
						m_Source.CombatantView.m_BattleMgr.InstantiateEffect(m_Source, visualEffect, visualEffectSetting, new List<ICombatant> { m_Target }, isCurse);
					}
				}
			}
		}

		public void RemoveEffect(bool onlyLogical = false, bool onlyVisual = false)
		{
			if (!onlyVisual)
			{
				m_IsRemoved = true;
				if (m_Source != null && m_Battle != null)
				{
					if (m_Source.CombatantFaction == Faction.Birds)
					{
						m_Battle.BirdsTurnStarted -= OnInvokerStartTurn;
					}
					else if (m_Source.CombatantFaction == Faction.Pigs)
					{
						m_Battle.PigsTurnStarted -= OnInvokerStartTurn;
						m_Battle.WaveDoneForEffect -= OnInvokerStartTurn;
					}
					m_Source.Defeated -= OnInvokerDefeated;
					m_Source.KnockedOut -= OnInvokerDefeated;
				}
				DIContainerLogic.GetBattleService().LogDebug("Removed E: " + m_EffectIdent + " to " + m_Target.CombatantNameId, BattleLogTypes.BattleEffect);
				m_Target.CurrrentEffects.Remove(m_EffectIdent);
				if (m_LinkedEffect != null)
				{
					m_LinkedEffect.m_LinkedEffect = null;
					m_LinkedEffect.RemoveEffect();
					m_LinkedEffect = null;
				}
				if (this.EffectRemoved != null)
				{
					this.EffectRemoved(this);
				}
				if (EffectRemovedAction != null)
				{
					EffectRemovedAction(this);
				}
				if (onlyLogical)
				{
					return;
				}
			}
			if (m_Effects.Count((BattleEffect e) => e.EffectType == BattleEffectType.Taunt) > 0)
			{
				m_Target.CombatantView.ResetBubbleTargetIcon();
				CharacterControllerBattleGround characterControllerBattleGround = m_Target.CombatantView as CharacterControllerBattleGround;
				if ((bool)characterControllerBattleGround)
				{
					characterControllerBattleGround.m_tauntedTarget = null;
				}
			}
			if (m_Target.CombatantView.CheckAndContainsVisualEffect(m_EffectIdent))
			{
				GameObject gameObject = m_Target.CombatantView.LastingVisualEffects[m_EffectIdent];
				if (gameObject != null)
				{
					float t = gameObject.PlayAnimationOrAnimatorState("End");
					UnityEngine.Object.Destroy(gameObject, t);
				}
				DebugLog.Log("Remove Visual and Logical Effect");
				m_Target.CombatantView.LastingVisualEffects.Remove(m_EffectIdent);
			}
			if (m_Source != null && m_Source.CombatantView.m_SpeechBubbles.ContainsKey(m_EffectIdent))
			{
				m_Source.CombatantView.m_SpeechBubbles[m_EffectIdent].RemoveBubble();
				m_Source.CombatantView.m_SpeechBubbles.Remove(m_EffectIdent);
			}
			if (m_Target.CombatantView.m_SpeechBubbles.ContainsKey(m_EffectIdent))
			{
				m_Target.CombatantView.m_SpeechBubbles[m_EffectIdent].RemoveBubble();
				m_Target.CombatantView.m_SpeechBubbles.Remove(m_EffectIdent);
			}
		}

		public void EvaluateEffect(ICombatant target = null)
		{
			if (target == null)
			{
				target = m_Target;
			}
			if (target.CombatantView.CheckAndContainsVisualEffect(m_EffectIdent))
			{
				GameObject gameObject = target.CombatantView.LastingVisualEffects[m_EffectIdent];
				if (gameObject.HasAnimation("Trigger"))
				{
					gameObject.PlayAnimationOrAnimatorStateQueued(new List<string> { "Trigger", "Loop" }, target.CombatantView);
				}
			}
			VisualEffectSetting setting = null;
			if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting(m_EffectIdent, out setting))
			{
				SpawnVisualEffects(VisualEffectSpawnTiming.Triggered, setting, m_EffectType == SkillEffectTypes.Curse);
			}
			if (m_EffectType == SkillEffectTypes.Environmental)
			{
				m_Battle.RegisterEnvironmentalEffectTriggered(m_SkillIdent);
			}
		}

		public void AddEffectValue(ICombatant owner, string ident, BattleEffectGameData effect)
		{
			if (owner.CurrrentEffects == null)
			{
				owner.CurrrentEffects = new Dictionary<string, BattleEffectGameData>();
			}
			if (owner.CurrrentEffects.ContainsKey(ident))
			{
				owner.CurrrentEffects[ident].RemoveEffect(true);
				owner.CurrrentEffects[ident] = effect;
				owner.LastAddedEffect = ident;
			}
			else
			{
				owner.CurrrentEffects.Add(ident, effect);
				owner.LastAddedEffect = ident;
			}
		}

		public BattleEffectGameData LinkEffect(BattleEffectGameData effect)
		{
			m_LinkedEffect = effect;
			effect.m_LinkedEffect = this;
			return this;
		}
	}
}

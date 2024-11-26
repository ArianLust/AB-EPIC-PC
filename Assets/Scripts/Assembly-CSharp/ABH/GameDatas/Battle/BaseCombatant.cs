using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle
{
	public abstract class BaseCombatant<T> : ICombatant where T : ICharacter
	{
		protected T Model;

		private ComboInfo m_ComboInfo;

		protected SkillBattleDataBase m_ChachedSkill;

		protected ClassItemGameData m_CombatantClass;

		protected EquipmentGameData m_CombatantMainHandEquipment;

		protected EquipmentGameData m_CombatantOffHandEquipment;

		protected float m_BaseHealth;

		protected float m_BaseAttack;

		protected float m_HealthBuff;

		protected float m_ModifiedHealth;

		protected bool m_buffsHealthChanged = true;

		protected float m_AttackBuff;

		protected float m_ModifiedAttack;

		protected bool m_buffsAttackChanged = true;

		protected float m_TemporaryHealth;

		protected SummoningType m_SummonedType;

		protected float m_damageModifier = 1f;

		protected float m_currentHealth;

		public List<SkillBattleDataBase> m_Skills = new List<SkillBattleDataBase>();

		protected ICombatant m_AttackTarget;

		protected bool m_IsRevivedOnce;

		protected float m_SummedDamagePerTurn;

		protected float m_SummedHealPerTurn;

		protected Dictionary<string, BattleEffectGameData> m_CurrentEffects = new Dictionary<string, BattleEffectGameData>();

		protected string m_LastAddedEffect;

		protected int m_StunTurns;

		private bool m_Participating = true;

		private Dictionary<string, float> m_CurrentStatBuffs = new Dictionary<string, float>();

		private SkillBattleDataBase m_SetItemSkill;

		private SkillBattleDataBase m_PvpSetItemSkill;

		private bool m_reviveTriggeredDoNotExplode;

		public ICombatant m_LastDamageSource { get; set; }

		public SkillBattleDataBase ChachedSkill
		{
			get
			{
				return m_ChachedSkill;
			}
			set
			{
				m_ChachedSkill = value;
			}
		}

		public ComboInfo ComboInfo
		{
			get
			{
				return m_ComboInfo;
			}
			set
			{
				m_ComboInfo = value;
			}
		}

		public virtual List<AiCombo> AiCombos
		{
			get
			{
				return Model.SkillCombos;
			}
		}

		public ClassItemGameData CombatantClass
		{
			get
			{
				if (m_CombatantClass == null)
				{
					m_CombatantClass = Model.ClassItem;
				}
				return m_CombatantClass;
			}
		}

		public EquipmentGameData CombatantMainHandEquipment
		{
			get
			{
				if (m_CombatantMainHandEquipment == null)
				{
					m_CombatantMainHandEquipment = Model.MainHandItem;
				}
				return m_CombatantMainHandEquipment;
			}
		}

		public EquipmentGameData CombatantOffHandEquipment
		{
			get
			{
				if (m_CombatantOffHandEquipment == null)
				{
					m_CombatantOffHandEquipment = Model.OffHandItem;
				}
				return m_CombatantOffHandEquipment;
			}
		}

		public float BaseHealth
		{
			get
			{
				return Model.BaseHealth;
			}
		}

		public float BaseAttack
		{
			get
			{
				return Model.BaseAttack;
			}
		}

		public float HealthBuff
		{
			get
			{
				return m_HealthBuff;
			}
			set
			{
				m_HealthBuff = value;
				FlagBuffsChanged();
			}
		}

		public float ModifiedHealth
		{
			get
			{
				if (m_buffsHealthChanged)
				{
					float value = 0f;
					CurrentStatBuffs.TryGetValue("health", out value);
					m_ModifiedHealth = DIContainerLogic.GetBattleService().ApplyBuffsOnHealth(BaseHealth, this, CurrrentEffects, value);
					m_currentHealth = m_ModifiedHealth;
					m_ModifiedHealth += m_HealthBuff;
					m_buffsHealthChanged = false;
				}
				return m_ModifiedHealth;
			}
		}

		public float AttackBuff
		{
			get
			{
				return m_AttackBuff;
			}
			set
			{
				m_AttackBuff = value;
				FlagBuffsChanged();
			}
		}

		public float ModifiedAttack
		{
			get
			{
				if (m_buffsAttackChanged)
				{
					float value = 0f;
					CurrentStatBuffs.TryGetValue("attack", out value);
					m_ModifiedAttack = DIContainerLogic.GetBattleService().ApplyBuffsOnAttack(BaseAttack, this, CurrrentEffects, value);
					m_ModifiedAttack += m_AttackBuff;
					m_buffsAttackChanged = false;
				}
				return m_ModifiedAttack;
			}
		}

		public float TemporaryHealth
		{
			get
			{
				return m_TemporaryHealth;
			}
			set
			{
				m_TemporaryHealth = value;
			}
		}

		public SummoningType summoningType
		{
			get
			{
				return m_SummonedType;
			}
			set
			{
				m_SummonedType = value;
			}
		}

		public float DamageModifier
		{
			get
			{
				return m_damageModifier;
			}
			set
			{
				m_damageModifier = value;
			}
		}

		public float CurrentHealth
		{
			get
			{
				if (BaseHealth < 0f)
				{
					return 100f;
				}
				m_currentHealth = Math.Min(m_currentHealth, ModifiedHealth);
				return m_currentHealth;
			}
			set
			{
				if (BaseHealth < 0f)
				{
					m_currentHealth = 100f;
					RaiseHealthChanged(m_currentHealth, value);
					return;
				}
				float currentHealth = m_currentHealth;
				m_currentHealth = value;
				if (m_currentHealth < 1f && m_currentHealth >= 0f)
				{
					m_currentHealth = 0f;
				}
				RaiseHealthChanged(currentHealth, m_currentHealth);
				m_currentHealth = Math.Min(m_currentHealth, ModifiedHealth);
				if (!(m_currentHealth <= 0f))
				{
					return;
				}
				if (currentHealth > 0f)
				{
					if (KnockOutOnDefeat)
					{
						IsKnockedOut = true;
						RaiseCombatantKnockedOut();
					}
					else
					{
						IsKnockedOut = false;
						RaiseCombatantDefeated();
					}
				}
				m_currentHealth = 0f;
			}
		}

		public abstract Faction CombatantFaction { get; set; }

		public bool IsAlive
		{
			get
			{
				return CurrentHealth > 0f;
			}
		}

		public ICharacter CharacterModel
		{
			get
			{
				return Model;
			}
		}

		public CharacterControllerBattleGroundBase CombatantView { get; set; }

		public ICombatant AttackTarget
		{
			get
			{
				return m_AttackTarget;
			}
			set
			{
				m_AttackTarget = value;
			}
		}

		public int CurrentInitiative { get; set; }

		public bool IsRevivedOnce
		{
			get
			{
				return true;
			}
			set
			{
				m_IsRevivedOnce = value;
			}
		}

		public float SummedDamagePerTurn
		{
			get
			{
				return m_SummedDamagePerTurn;
			}
			set
			{
				m_SummedDamagePerTurn = value;
			}
		}

		public float SummedHealPerTurn
		{
			get
			{
				return m_SummedHealPerTurn;
			}
			set
			{
				m_SummedHealPerTurn = value;
			}
		}

		public Dictionary<string, BattleEffectGameData> CurrrentEffects
		{
			get
			{
				return m_CurrentEffects;
			}
			set
			{
				m_CurrentEffects = value;
			}
		}

		public string LastAddedEffect
		{
			get
			{
				return m_LastAddedEffect;
			}
			set
			{
				m_LastAddedEffect = value;
			}
		}

		public bool OverrideHitAnimation { get; set; }

		public abstract string CombatantNameId { get; }

		public abstract string CombatantName { get; }

		public abstract string CombatantAssetId { get; }

		public int StunTurns
		{
			get
			{
				return m_StunTurns;
			}
			set
			{
				m_StunTurns = value;
			}
		}

		public float CurrentSkillAttackValue { get; set; }

		public abstract bool UsedConsumable { get; set; }

		public SkillExecutionInfo CurrentSkillExecutionInfo { get; set; }

		public bool IsParticipating
		{
			get
			{
				return IsAlive && !IsKnockedOut && m_Participating;
			}
			set
			{
				m_Participating = value;
			}
		}

		public bool IsKnockedOut { get; set; }

		public Dictionary<string, float> CurrentStatBuffs
		{
			get
			{
				return m_CurrentStatBuffs;
			}
			set
			{
				m_CurrentStatBuffs = value;
				RaiseBuffsChanged();
			}
		}

		public bool KnockOutOnDefeat { get; set; }

		public SkillBattleDataBase KnockedOutSkill { get; set; }

		public bool IsStunned
		{
			get
			{
				return CurrrentEffects.Values.Any((BattleEffectGameData e) => e.m_Effects.Any((BattleEffect ei) => ei.EffectType == BattleEffectType.Stun));
			}
		}

		public bool HasUsageDelay { get; set; }

		public virtual bool HasSetCompleted
		{
			get
			{
				return CombatantMainHandEquipment != null && CombatantOffHandEquipment != null && CombatantMainHandEquipment.IsSetItem && CombatantMainHandEquipment.CorrespondingSetItem.NameId == CombatantOffHandEquipment.BalancingData.NameId;
			}
		}

		public virtual bool IsBanner
		{
			get
			{
				return false;
			}
		}

		public virtual bool ActedThisTurn { get; set; }

		public int ExtraTurns { get; set; }

		public virtual bool IsRageAvailiable
		{
			get
			{
				if (ActedThisTurn)
				{
					return false;
				}
				foreach (BattleEffectGameData value in CurrrentEffects.Values)
				{
					foreach (BattleEffect effect in value.m_Effects)
					{
						if (effect.EffectType == BattleEffectType.RageBlocked)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public virtual bool CanUseConsumable
		{
			get
			{
				return !UsedConsumable;
			}
		}

		public bool StartedHisTurn { get; set; }

		public bool Entered { get; set; }

		public float Depth
		{
			get
			{
				if ((bool)CombatantView && (bool)CombatantView.m_CachedTransform)
				{
					return CombatantView.m_CachedTransform.position.z;
				}
				return 0f;
			}
		}

		public bool IsAttacking { get; set; }

		public virtual InterruptAction InterruptAction
		{
			get
			{
				return InterruptAction.none;
			}
		}

		public virtual List<InterruptCondition> InterruptCondition
		{
			get
			{
				return new List<InterruptCondition>();
			}
		}

		public virtual bool UseRage
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool ChargeDone { get; set; }

		public bool IsPvPBird
		{
			get
			{
				return Model.IsPvPBird;
			}
		}

		public virtual bool IsCharging
		{
			get
			{
				return false;
			}
		}

		public bool ReviveTriggeredDoNotExplode
		{
			get
			{
				return m_reviveTriggeredDoNotExplode;
			}
			set
			{
				m_reviveTriggeredDoNotExplode = value;
			}
		}

		[method: MethodImpl(32)]
		public event Action BuffsChanged;

		[method: MethodImpl(32)]
		public event Action<float, float> HealthChanged;

		[method: MethodImpl(32)]
		public event Action Defeated;

		[method: MethodImpl(32)]
		public event Action<ICombatant, ICombatant> SkillTriggered;

		[method: MethodImpl(32)]
		public event Action<PerkType, ICombatant> PerkUsed;

		[method: MethodImpl(32)]
		public event Action<List<IInventoryItemGameData>> DropItems;

		[method: MethodImpl(32)]
		public event Action<int> TurnStarted;

		[method: MethodImpl(32)]
		public event Action<int, int> LevelChanged;

		[method: MethodImpl(32)]
		public event Action Runaway;

		[method: MethodImpl(32)]
		public event Action KnockedOut;

		[method: MethodImpl(32)]
		public event Action<int, ICombatant> TurnEnded;

		public BaseCombatant(T model)
		{
			Model = model;
			CurrentHealth = ModifiedHealth;
		}

		public void RefreshHealth()
		{
			CurrentHealth = ModifiedHealth;
		}

		public abstract bool AutoBattleReadyForRage();

		public abstract bool AutoBattleDoRage();

		public void SetHealthWithoutLogic(float value)
		{
			m_ModifiedHealth = value;
			m_currentHealth = value;
		}

		public void RaisePerkUsed(PerkType perkType, ICombatant target)
		{
			if (this.PerkUsed != null)
			{
				this.PerkUsed(perkType, target);
			}
		}

		public void RaiseDropItems(List<IInventoryItemGameData> items)
		{
			if (this.DropItems != null)
			{
				this.DropItems(items);
			}
		}

		public void RaiseHealthChanged(float oldHealth, float currentHealth)
		{
			if (this.HealthChanged != null)
			{
				this.HealthChanged(oldHealth, currentHealth);
			}
		}

		public abstract SkillBattleDataBase GetSkill(int index);

		protected SkillBattleDataBase GenerateSkillBattleData(SkillGameData data)
		{
			if (data == null)
			{
				return null;
			}
			SkillBattleDataBase skillBattleDataBase = Activator.CreateInstance(Type.GetType("ABH.GameDatas.Battle.Skills." + data.Balancing.SkillTemplateType, true, true)) as SkillBattleDataBase;
			if (skillBattleDataBase != null)
			{
				skillBattleDataBase.Init(data);
			}
			return skillBattleDataBase;
		}

		protected void AddBattleSkillImpl(SkillGameData data)
		{
			SkillBattleDataBase skillBattleDataBase = Activator.CreateInstance(Type.GetType("ABH.GameDatas.Battle.Skills." + data.Balancing.SkillTemplateType, true, true)) as SkillBattleDataBase;
			if (skillBattleDataBase != null)
			{
				skillBattleDataBase.Init(data);
				m_Skills.Add(skillBattleDataBase);
			}
			else
			{
				DebugLog.Error("Couldn't create skill of type: ABH.GameDatas.Battle.Skills." + data.Balancing.SkillTemplateType);
			}
		}

		public void RaiseBuffsChanged()
		{
			m_buffsAttackChanged = true;
			m_buffsHealthChanged = true;
			if (this.BuffsChanged != null)
			{
				this.BuffsChanged();
			}
		}

		public void FlagBuffsChanged()
		{
			m_buffsAttackChanged = true;
			m_buffsHealthChanged = true;
		}

		public void EvaluateNextAttackTarget()
		{
		}

		public void ReceiveDamage(float damage, ICombatant attacker)
		{
			m_LastDamageSource = attacker;
			float num = DIContainerLogic.GetBattleService().ApplyEffectsOfTypeOnTriggerType(damage, new List<BattleEffectType> { BattleEffectType.DoNotGetKilledByBird }, EffectTriggerType.OnReceiveDamageImmediately, this, attacker);
			m_SummedDamagePerTurn += num;
		}

		public void HealDamage(float heal, ICombatant attacker)
		{
			m_SummedHealPerTurn += heal;
		}

		public float GetRealDamage(float dealtDamage, ICombatant attacker)
		{
			return dealtDamage;
		}

		public void RaiseTurnStarted(int turn)
		{
			if (this.TurnStarted != null)
			{
				this.TurnStarted(turn);
			}
		}

		public List<SkillBattleDataBase> GetSkills()
		{
			for (int i = 0; i < Model.Skills.Count; i++)
			{
				GetSkill(i);
			}
			return m_Skills;
		}

		public virtual void RaiseCombatantDefeated()
		{
			RemoveAppliedEffects();
			if (this.Defeated != null)
			{
				this.Defeated();
			}
		}

		public virtual void RaiseCombatantKnockedOut()
		{
			RemoveAppliedEffects();
			if (this.KnockedOut != null)
			{
				this.KnockedOut();
			}
		}

		public void RemoveAppliedEffects()
		{
			if (CurrrentEffects != null)
			{
				List<BattleEffectGameData> list = CurrrentEffects.Values.ToList();
				for (int num = list.Count - 1; num >= 0; num--)
				{
					list[num].RemoveEffect();
				}
			}
		}

		public SkillBattleDataBase GetSkill(string name)
		{
			List<SkillBattleDataBase> skills = GetSkills();
			return skills.FirstOrDefault((SkillBattleDataBase s) => s.Model.SkillNameId == name);
		}

		public void RaiseLevelChanged(int oldValue, int newValue)
		{
			if (this.LevelChanged != null)
			{
				this.LevelChanged(oldValue, newValue);
			}
		}

		public void RaiseRunaway()
		{
			if (this.Runaway != null)
			{
				this.Runaway();
			}
		}

		public virtual SkillBattleDataBase GetSetItemSkill(bool isPvp)
		{
			if (m_SetItemSkill != null && !isPvp)
			{
				return m_SetItemSkill;
			}
			if (m_PvpSetItemSkill != null && isPvp)
			{
				return m_PvpSetItemSkill;
			}
			if (!HasSetCompleted || CombatantMainHandEquipment.SetItemSkill.Balancing == null)
			{
				return null;
			}
			if (isPvp)
			{
				m_PvpSetItemSkill = GenerateSkillBattleData(CombatantMainHandEquipment.PvpSetItemSkill);
				return m_PvpSetItemSkill;
			}
			m_SetItemSkill = GenerateSkillBattleData(CombatantMainHandEquipment.SetItemSkill);
			return m_SetItemSkill;
		}

		public virtual void RaiseSkillTriggered(ICombatant invoker, ICombatant target)
		{
			if (this.SkillTriggered != null)
			{
				this.SkillTriggered(invoker, target);
			}
		}

		public abstract bool AddBonusTurns(int turns);

		public void RaiseTurnEnded(int turn)
		{
			if (this.TurnEnded != null)
			{
				this.TurnEnded(turn, this);
			}
		}

		public virtual void CleansedCurse(BattleEffect effect, bool effectFromStun = false)
		{
		}

		public ICombatant GetTauntTarget()
		{
			foreach (BattleEffectGameData value in CurrrentEffects.Values)
			{
				foreach (BattleEffect effect in value.m_Effects)
				{
					if (effect.EffectType == BattleEffectType.Taunt)
					{
						return value.m_Source;
					}
				}
			}
			return null;
		}

		public void ReduceChargeBy(int chargeTurns)
		{
			foreach (BattleEffectGameData value in CurrrentEffects.Values)
			{
				for (int i = 0; i < value.m_Effects.Count; i++)
				{
					if (value.m_Effects[i].EffectType == BattleEffectType.Charge)
					{
						value.IncreaseChargeByTurns(chargeTurns);
						return;
					}
				}
			}
		}
	}
}

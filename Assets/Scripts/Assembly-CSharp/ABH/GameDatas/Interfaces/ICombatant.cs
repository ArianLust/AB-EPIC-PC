using System;
using System.Collections.Generic;
using ABH.GameDatas.Battle;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Interfaces
{
	public interface ICombatant
	{
		InterruptAction InterruptAction { get; }

		List<InterruptCondition> InterruptCondition { get; }

		bool UseRage { get; set; }

		SkillBattleDataBase ChachedSkill { get; set; }

		List<AiCombo> AiCombos { get; }

		ComboInfo ComboInfo { get; set; }

		ICharacter CharacterModel { get; }

		string CombatantNameId { get; }

		string CombatantName { get; }

		string CombatantAssetId { get; }

		ClassItemGameData CombatantClass { get; }

		EquipmentGameData CombatantMainHandEquipment { get; }

		EquipmentGameData CombatantOffHandEquipment { get; }

		CharacterControllerBattleGroundBase CombatantView { get; set; }

		bool IsBanner { get; }

		bool IsPvPBird { get; }

		float BaseHealth { get; }

		float BaseAttack { get; }

		float HealthBuff { get; set; }

		float ModifiedHealth { get; }

		float AttackBuff { get; set; }

		float ModifiedAttack { get; }

		float DamageModifier { get; set; }

		float CurrentHealth { get; set; }

		float TemporaryHealth { get; set; }

		SummoningType summoningType { get; set; }

		float SummedDamagePerTurn { get; set; }

		float SummedHealPerTurn { get; set; }

		int CurrentInitiative { get; set; }

		bool HasUsageDelay { get; set; }

		SkillExecutionInfo CurrentSkillExecutionInfo { get; set; }

		Faction CombatantFaction { get; set; }

		Dictionary<string, BattleEffectGameData> CurrrentEffects { get; set; }

		string LastAddedEffect { get; set; }

		Dictionary<string, float> CurrentStatBuffs { get; set; }

		SkillBattleDataBase KnockedOutSkill { get; set; }

		bool IsAlive { get; }

		bool IsParticipating { get; set; }

		bool IsKnockedOut { get; set; }

		bool IsRevivedOnce { get; set; }

		bool OverrideHitAnimation { get; set; }

		bool UsedConsumable { get; set; }

		bool KnockOutOnDefeat { get; set; }

		ICombatant AttackTarget { get; set; }

		int StunTurns { get; set; }

		float CurrentSkillAttackValue { get; set; }

		bool IsRageAvailiable { get; }

		bool CanUseConsumable { get; }

		bool IsStunned { get; }

		bool IsCharging { get; }

		bool HasSetCompleted { get; }

		bool ActedThisTurn { get; set; }

		bool StartedHisTurn { get; set; }

		int ExtraTurns { get; set; }

		bool Entered { get; set; }

		float Depth { get; }

		bool IsAttacking { get; set; }

		bool ChargeDone { get; set; }

		ICombatant m_LastDamageSource { get; set; }

		bool ReviveTriggeredDoNotExplode { get; set; }

		event Action BuffsChanged;

		event Action<float, float> HealthChanged;

		event Action<int, int> LevelChanged;

		event Action<int, ICombatant> TurnEnded;

		event Action<int> TurnStarted;

		event Action Defeated;

		event Action Runaway;

		event Action KnockedOut;

		event Action<PerkType, ICombatant> PerkUsed;

		event Action<List<IInventoryItemGameData>> DropItems;

		event Action<ICombatant, ICombatant> SkillTriggered;

		void RefreshHealth();

		SkillBattleDataBase GetSetItemSkill(bool pvp);

		SkillBattleDataBase GetSkill(int index);

		SkillBattleDataBase GetSkill(string name);

		List<SkillBattleDataBase> GetSkills();

		bool AutoBattleReadyForRage();

		bool AutoBattleDoRage();

		bool AddBonusTurns(int turns);

		void RaiseCombatantDefeated();

		void RaiseCombatantKnockedOut();

		void RaiseTurnStarted(int turn);

		void RaiseTurnEnded(int turn);

		void RaiseLevelChanged(int oldValue, int newValue);

		void RaiseBuffsChanged();

		void FlagBuffsChanged();

		void RaiseHealthChanged(float oldValue, float newValue);

		void RaiseRunaway();

		void RaisePerkUsed(PerkType perkType, ICombatant target);

		void RaiseDropItems(List<IInventoryItemGameData> items);

		void RaiseSkillTriggered(ICombatant invoker, ICombatant target);

		void ReceiveDamage(float damage, ICombatant attacker);

		void HealDamage(float heal, ICombatant attacker);

		float GetRealDamage(float dealtDamage, ICombatant attacker);

		void EvaluateNextAttackTarget();

		void CleansedCurse(BattleEffect effect, bool effectFromStun = false);

		ICombatant GetTauntTarget();

		void ReduceChargeBy(int chargeTurns);
	}
}

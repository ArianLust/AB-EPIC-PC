using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Services.Logic;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle
{
	public class BirdCombatant : BaseCombatant<BirdGameData>
	{
		private bool m_useRage;

		private bool m_pvpBird;

		public override InterruptAction InterruptAction
		{
			get
			{
				return Model.ClassItem.BalancingData.InterruptAction;
			}
		}

		public override List<InterruptCondition> InterruptCondition
		{
			get
			{
				return Model.ClassItem.m_InterruptCondition;
			}
		}

		public override List<AiCombo> AiCombos
		{
			get
			{
				return m_pvpBird ? Model.PvPSkillCombos : Model.SkillCombos;
			}
		}

		public override bool UseRage
		{
			get
			{
				return m_useRage;
			}
			set
			{
				m_useRage = value;
			}
		}

		public override string CombatantNameId
		{
			get
			{
				return Model.BalancingData.NameId;
			}
		}

		public override string CombatantName
		{
			get
			{
				return DIContainerInfrastructure.GetLocaService().GetCharacterName(Model.BalancingData.LocaId);
			}
		}

		public override string CombatantAssetId
		{
			get
			{
				return Model.BalancingData.AssetId;
			}
		}

		public override Faction CombatantFaction
		{
			get
			{
				return Faction.Birds;
			}
			set
			{
			}
		}

		public override bool IsCharging
		{
			get
			{
				bool result = false;
				foreach (BattleEffectGameData value in CurrrentEffects.Values)
				{
					for (int i = 0; i < value.m_Effects.Count; i++)
					{
						if (value.m_Effects[i].EffectType == BattleEffectType.Charge)
						{
							result = true;
						}
					}
				}
				return result;
			}
		}

		public override bool UsedConsumable { get; set; }

		public BirdCombatant(BirdGameData model)
			: base(model)
		{
			KnockOutOnDefeat = true;
		}

		public BirdCombatant SetPvPBird(bool isPvP)
		{
			m_pvpBird = isPvP;
			return this;
		}

		public override bool AutoBattleDoRage()
		{
			return (CombatantView.m_BattleMgr as BattleMgr).AutoBattleDoRage();
		}

		public override bool AutoBattleReadyForRage()
		{
			if (!IsRageAvailiable)
			{
				return false;
			}
			if (!(CombatantView.m_BattleMgr as BattleMgr).IsLastWave() && !CombatantView.m_BattleMgr.Model.m_CombatantsPerFaction[Faction.Pigs].Any((ICombatant p) => p.CurrentHealth / p.ModifiedHealth > 0.5f))
			{
				return false;
			}
			switch (Model.BalancingData.LocaId)
			{
			case "bird_red":
			{
				float value2 = 0f;
				GetSkill(2).Model.SkillParameters.TryGetValue("damage_in_percent", out value2);
				float damage = value2 / 100f * ModifiedAttack;
				damage /= 2f;
				if (CombatantView.m_BattleMgr.Model.m_CombatantsPerFaction[DIContainerLogic.GetBattleService().GetOppositeFaction(CombatantFaction)].All((ICombatant p) => p.CurrentHealth <= damage))
				{
					return false;
				}
				break;
			}
			case "bird_black":
				if (CombatantView.m_BattleMgr.Model.m_CombatantsPerFaction[DIContainerLogic.GetBattleService().GetOppositeFaction(CombatantFaction)].Count((ICombatant p) => p.CurrentHealth > 0f) == 1 || CombatantView.m_BattleMgr.Model.m_CombatantsPerFaction[Faction.Birds].Any((ICombatant b) => b.CharacterModel.Name.Contains("bird_merchant") || b.CharacterModel.Name.Contains("bird_prince_porky")))
				{
					return false;
				}
				break;
			case "bird_merchant":
			case "bird_white":
			{
				float value = 0f;
				GetSkill(2).Model.SkillParameters.TryGetValue("health_in_percent", out value);
				float num = value / 100f;
				float num2 = 0f;
				float num3 = 0f;
				foreach (ICombatant item in CombatantView.m_BattleMgr.Model.m_CombatantsPerFaction[CombatantFaction])
				{
					if (item.CurrentHealth > 0f)
					{
						num2 += Mathf.Max(0f, item.ModifiedHealth * num - (item.ModifiedHealth - item.CurrentHealth));
						num3 += item.ModifiedHealth * num;
					}
				}
				if (num2 > num3 / 2f)
				{
					return false;
				}
				break;
			}
			case "bird_blue":
				if (CombatantView.m_BattleMgr.Model.m_CombatantsPerFaction[DIContainerLogic.GetBattleService().GetOppositeFaction(CombatantFaction)].Count((ICombatant p) => p.CurrentHealth > 0f) > 2 && CombatantView.m_BattleMgr.Model.m_CombatantsPerFaction[CombatantFaction].Any((ICombatant b) => b.CharacterModel.Name.Contains("bird_black") || b.CharacterModel.Name.Contains("bird_yellow")))
				{
					return false;
				}
				break;
			case "bird_yellow":
				if (CombatantView.m_BattleMgr.Model.m_CombatantsPerFaction[DIContainerLogic.GetBattleService().GetOppositeFaction(CombatantFaction)].Count((ICombatant p) => p.CurrentHealth > 0f) == 1 && CombatantView.m_BattleMgr.Model.m_CombatantsPerFaction[CombatantFaction].Any((ICombatant b) => b.CharacterModel.Name.Contains("bird_red")))
				{
					return false;
				}
				break;
			case "bird_adventurer":
			case "bird_prince_porky":
				if (CombatantView.m_BattleMgr.Model.m_CombatantsPerFaction[DIContainerLogic.GetBattleService().GetOppositeFaction(CombatantFaction)].Count((ICombatant p) => p.CurrentHealth > 0f) == 1)
				{
					return false;
				}
				break;
			}
			return CurrentHealth > 0f && !ActedThisTurn;
		}

		private void model_LevelChanged(int arg1, int arg2)
		{
			RaiseLevelChanged(arg1, arg2);
			RaiseBuffsChanged();
		}

		public override void RaiseCombatantKnockedOut()
		{
			base.RaiseCombatantKnockedOut();
			if (m_pvpBird && ClientInfo.CurrentBattleGameData != null && ClientInfo.CurrentBattleGameData.CurrentCombatant != null && ClientInfo.CurrentBattleGameData.CurrentCombatant.CombatantFaction == Faction.Pigs)
			{
				Debug.Log("Trying to recalculate turn...");
				DIContainerLogic.GetBattleService().m_PvpIntelligence.CalculateTurn();
			}
			KnockOutOnDefeat = false;
			DIContainerLogic.GetBattleService().LogDebug("Bird knocked out: " + Model.BalancingData.NameId, BattleLogTypes.Bird);
		}

		public override void RaiseCombatantDefeated()
		{
			base.RaiseCombatantDefeated();
			DIContainerLogic.GetBattleService().LogDebug("Bird defeated: " + Model.BalancingData.NameId, BattleLogTypes.Bird);
		}

		public override SkillBattleDataBase GetSkill(int index)
		{
			if (Model.ClassItem == null)
			{
				DebugLog.Log("Class Item is null!");
				return null;
			}
			if (m_Skills.Count == 0)
			{
				AddBattleSkillImpl((!m_pvpBird) ? Model.ClassItem.PrimarySkill : Model.ClassItem.PrimaryPvPSkill);
				AddBattleSkillImpl((!m_pvpBird) ? Model.ClassItem.SecondarySkill : Model.ClassItem.SecondaryPvPSkill);
				AddBattleSkillImpl((!m_pvpBird) ? Model.RageSkill : Model.PvPRageSkill);
			}
			SkillBattleDataBase skillBattleDataBase = ((m_Skills.Count <= index) ? null : m_Skills[index]);
			if (skillBattleDataBase != null)
			{
				skillBattleDataBase = skillBattleDataBase.CheckForReplacement(this);
			}
			return skillBattleDataBase;
		}

		public override void RaiseSkillTriggered(ICombatant invoker, ICombatant target)
		{
			if (ExtraTurns > 0)
			{
				ExtraTurns--;
			}
			else
			{
				ActedThisTurn = true;
				RaiseTurnEnded(0);
			}
			base.RaiseSkillTriggered(invoker, target);
		}

		public override bool AddBonusTurns(int turns)
		{
			ExtraTurns += turns;
			return false;
		}

		public override void CleansedCurse(BattleEffect effect, bool ignoreAfterStunEffect = false)
		{
			base.CleansedCurse(effect);
			if (effect.EffectType == BattleEffectType.Stun && !ignoreAfterStunEffect)
			{
				CombatantView.RefreshFromStun();
			}
		}
	}
}

using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle
{
	public class BossCombatant : BaseCombatant<BossGameData>
	{
		private SkillBattleDataBase m_PassiveSkill;

		public new List<AiCombo> AiCombos
		{
			get
			{
				return Model.SkillCombos;
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
				return Model.BalancingData.Faction;
			}
			set
			{
			}
		}

		public SkillBattleDataBase PassiveSkill
		{
			get
			{
				if (m_PassiveSkill == null)
				{
					m_PassiveSkill = GenerateSkillBattleData(Model.PassiveSkill);
				}
				return m_PassiveSkill;
			}
		}

		public override bool UsedConsumable
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		public override bool IsRageAvailiable
		{
			get
			{
				return false;
			}
		}

		public override bool CanUseConsumable
		{
			get
			{
				return false;
			}
		}

		public override InterruptAction InterruptAction
		{
			get
			{
				return (Model.ClassItem != null) ? Model.ClassItem.BalancingData.InterruptAction : InterruptAction.none;
			}
		}

		public override List<InterruptCondition> InterruptCondition
		{
			get
			{
				return (Model.ClassItem != null) ? Model.ClassItem.m_InterruptCondition : new List<InterruptCondition>();
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

		public override bool UseRage
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public BossCombatant(BossGameData model)
			: base(model)
		{
		}

		public override void RaiseCombatantDefeated()
		{
			base.RaiseCombatantDefeated();
		}

		public override bool AutoBattleDoRage()
		{
			return false;
		}

		public override bool AutoBattleReadyForRage()
		{
			return false;
		}

		public override void RaiseCombatantKnockedOut()
		{
			base.RaiseCombatantKnockedOut();
		}

		public override SkillBattleDataBase GetSkill(int index)
		{
			if (Model.Skills == null || Model.Skills.Count <= index)
			{
				return null;
			}
			if (m_Skills.Count != Model.Skills.Count)
			{
				foreach (SkillGameData skill in Model.Skills)
				{
					AddBattleSkillImpl(skill);
				}
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
		}

		public override bool AddBonusTurns(int turns)
		{
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

		public void SetWeaponIndex(int i)
		{
			m_CombatantMainHandEquipment = null;
			m_CombatantOffHandEquipment = null;
			Model.m_WeaponIndex = i;
		}
	}
}

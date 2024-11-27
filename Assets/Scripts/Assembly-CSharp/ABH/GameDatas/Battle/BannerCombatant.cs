using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Services.Logic;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle
{
	public class BannerCombatant : BaseCombatant<BannerGameData>
	{
		private bool m_useRage;

		private SkillBattleDataBase m_EmblemSetItemSkill;

		private SkillBattleDataBase m_SetItemSkill;

		public override InterruptAction InterruptAction
		{
			get
			{
				return InterruptAction.none;
			}
		}

		public override List<InterruptCondition> InterruptCondition
		{
			get
			{
				return null;
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
				return Model.CharacterFaction;
			}
			set
			{
			}
		}

		public override bool HasSetCompleted
		{
			get
			{
				return Model.BannerCenter.IsSetCompleted(Model) || Model.BannerTip.IsSetCompleted(Model);
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

		public override bool IsBanner
		{
			get
			{
				return true;
			}
		}

		public BannerCombatant(BannerGameData model)
			: base(model)
		{
			KnockOutOnDefeat = true;
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
			KnockOutOnDefeat = false;
			DIContainerLogic.GetBattleService().LogDebug("Banner knocked out: " + Model.BalancingData.NameId, BattleLogTypes.Bird);
		}

		public override void RaiseCombatantDefeated()
		{
			base.RaiseCombatantDefeated();
			DIContainerLogic.GetBattleService().LogDebug("Banner defeated: " + Model.BalancingData.NameId, BattleLogTypes.Bird);
		}

		public override SkillBattleDataBase GetSkill(int index)
		{
			if (m_Skills.Count == 0)
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

		public SkillBattleDataBase GetEmblemSetItemSkill()
		{
			if (!Model.BannerEmblem.IsSetItem)
			{
				return null;
			}
			return m_EmblemSetItemSkill ?? (m_EmblemSetItemSkill = GenerateSkillBattleData(Model.BannerEmblem.SetItemSkill));
		}

		public override SkillBattleDataBase GetSetItemSkill(bool isPvp)
		{
			if (m_SetItemSkill != null)
			{
				return m_SetItemSkill;
			}
			if (!HasSetCompleted)
			{
				return null;
			}
			if (Model.BannerTip.SetItemSkill != null)
			{
				m_SetItemSkill = GenerateSkillBattleData(Model.BannerTip.SetItemSkill);
			}
			else if (Model.BannerCenter.SetItemSkill != null)
			{
				m_SetItemSkill = GenerateSkillBattleData(Model.BannerCenter.SetItemSkill);
			}
			return m_SetItemSkill;
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

		public override void CleansedCurse(BattleEffect effect, bool effectFromStun = false)
		{
			base.CleansedCurse(effect);
			if (effect.EffectType == BattleEffectType.Stun)
			{
				CombatantView.RefreshFromStun();
			}
		}
	}
}

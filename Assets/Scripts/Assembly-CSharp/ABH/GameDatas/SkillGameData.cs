using System;
using System.Collections.Generic;
using ABH.GameDatas.Battle;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;

namespace ABH.GameDatas
{
	public class SkillGameData
	{
		private Dictionary<string, float> m_OverrideSkillParameters;

		public SkillTypes m_SkillType;

		public SkillBalancingData Balancing { get; protected set; }

		public Dictionary<string, float> SkillParameters
		{
			get
			{
				if (Balancing.SkillParameters == null)
				{
					Balancing.SkillParameters = new Dictionary<string, float>();
				}
				if (m_OverrideSkillParameters != null)
				{
					return m_OverrideSkillParameters;
				}
				return Balancing.SkillParameters;
			}
		}

		public int SkillDuration
		{
			get
			{
				float value = 1f;
				if (SkillParameters != null && SkillParameters.TryGetValue("durationInTurns", out value))
				{
					return (int)value;
				}
				return 1;
			}
		}

		public string SkillLocalizedName
		{
			get
			{
				if (string.IsNullOrEmpty(Balancing.LocaId))
				{
					return "none";
				}
				return DIContainerInfrastructure.GetLocaService().GetSkillName(Balancing.LocaId, new Dictionary<string, string>());
			}
		}

		public string SkillNameId
		{
			get
			{
				return Balancing.NameId;
			}
		}

		public string SkillDescription
		{
			get
			{
				if (string.IsNullOrEmpty(Balancing.LocaId))
				{
					return "none";
				}
				return Balancing.LocaId;
			}
		}

		public string m_SkillIconName
		{
			get
			{
				return Balancing.IconAssetId;
			}
		}

		public SkillGameData(string nameId)
		{
			Balancing = DIContainerBalancing.Service.GetBalancingData<SkillBalancingData>(nameId);
		}

		public SkillGameData(string nameId, Dictionary<string, float> overrideSkillParams)
		{
			Balancing = DIContainerBalancing.Service.GetBalancingData<SkillBalancingData>(nameId);
			m_OverrideSkillParameters = overrideSkillParams;
		}

		public SkillGameData(SkillGameData origin)
		{
			Balancing = origin.Balancing;
			m_OverrideSkillParameters = new Dictionary<string, float>(origin.Balancing.SkillParameters);
		}

		public SkillBattleDataBase GenerateSkillBattleData()
		{
			SkillBattleDataBase skillBattleDataBase = Activator.CreateInstance(Type.GetType("ABH.GameDatas.Battle.Skills." + Balancing.SkillTemplateType, true, true)) as SkillBattleDataBase;
			if (skillBattleDataBase != null)
			{
				skillBattleDataBase.Init(this);
			}
			return skillBattleDataBase;
		}

		public SkillGameData SetOverrideSkillParamerters(Dictionary<string, float> overrideSkillParams)
		{
			m_OverrideSkillParameters = overrideSkillParams;
			return this;
		}

		public string GetIndepthDescription()
		{
			return DIContainerInfrastructure.GetLocaService().GetExtendedSkillDescriptions(Balancing.LocaId);
		}

		public bool IsPseudoPerk()
		{
			return Balancing.NameId.StartsWith("banner_tip_") || Balancing.NameId.StartsWith("banner_banner_") || Balancing.NameId.StartsWith("banner_set_");
		}

		public PerkType GetPerkType()
		{
			switch (Balancing.SkillTemplateType)
			{
			case "PassiveIncreaseHealingAura":
				return PerkType.IncreaseHealing;
			case "PassiveIncreaseStatsAura":
				if (Balancing.SkillParameters.ContainsKey("increase_health_in_percent"))
				{
					return PerkType.Vitality;
				}
				return PerkType.Might;
			case "PassiveIncreaseStatsAuraSet":
				return PerkType.Might;
			case "PassiveIncreaseRageAura":
				return PerkType.IncreaseRage;
			case "PassiveReduceDamageAura":
				return PerkType.Vigor;
			case "PassiveChainAura":
				return PerkType.ChainAttack;
			case "PassiveCritAura":
				return PerkType.CriticalStrike;
			case "PassiveVampiricAura":
				return PerkType.HocusPokus;
			case "PassiveDispelAura":
				return PerkType.Dispel;
			case "PassiveStunAura":
				return PerkType.Bedtime;
			case "PassiveIncreaseBirdDamageOnLowHP":
				return PerkType.Enrage;
			case "PassiveTakeDamageFromBird":
				return PerkType.ShareBirdDamage;
			case "PassiveReduceRespawn":
				return PerkType.ReduceRespawn;
			case "BirdShieldAfterCoinloss":
				return PerkType.MythicProtection;
			case "PassiveFinishTarget":
				return PerkType.Finisher;
			case "PassiveImmuneAfterRevive":
				return PerkType.Stronghold;
			case "PassiveShieldAfterAttack":
				return PerkType.Justice;
			default:
				return PerkType.None;
			}
		}
	}
}

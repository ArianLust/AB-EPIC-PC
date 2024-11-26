using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle
{
	public class BattleGameData
	{
		public AsyncCallback CallbackWhenDone;

		private BattleBalancingData m_Balancing;

		public List<string> m_PossibleFollowUpBattles = new List<string>();

		public BattleEndGameData m_BattleEndData;

		public bool m_ChronicleCaveBattle;

		public int m_CurrentTurn;

		public int m_CurrentWaveTurn;

		public int m_NextInitiativeIndex;

		public Random m_RandomBattleGenerator;

		public int m_SumOfInitalHealth;

		public bool m_PigsStartTurn;

		public bool m_BirdTurnCheated;

		public bool m_PigTurnCheated;

		public List<BirdGameData> m_PvPBirds = new List<BirdGameData>();

		public BannerCombatant m_BirdBanner;

		public BannerCombatant m_PigBanner;

		public List<ICombatant> m_BirdsByInitiative = new List<ICombatant>();

		public List<ICombatant> m_PigsByInitiative = new List<ICombatant>();

		public List<ICombatant> m_CombatantsByInitiative = new List<ICombatant>();

		public List<ICombatant> m_CombatantsOutOfInitiativeOrder = new List<ICombatant>();

		public List<ICombatant> m_CombatantsNotActed = new List<ICombatant>();

		public List<IInventoryItemGameData> m_AppliedXPModifiers = new List<IInventoryItemGameData>();

		public Dictionary<Faction, List<ICombatant>> m_CombatantsPerFaction = new Dictionary<Faction, List<ICombatant>>();

		public Dictionary<Faction, Dictionary<string, float>> m_FactionBuffs = new Dictionary<Faction, Dictionary<string, float>>();

		public Dictionary<Faction, SkillGameData> m_EnvironmentalEffects = new Dictionary<Faction, SkillGameData>();

		public Dictionary<string, int> m_PotionsUsed = new Dictionary<string, int>();

		public InventoryGameData m_ControllerInventory;

		public int m_ControllerLevel;

		public DateTime m_BattleStartTime;

		public string m_LastUsedConsumable;

		public float m_CoinsAtBattleStart;

		public float m_DamageLastTurn;

		public bool m_EnvEffectTriggered;

		public bool m_IsConsumableBlocked;

		public int m_DefeatedPvPBirds;

		public string m_CurrentOpponentId;

		private int m_lastPigTurn;

		public float m_CurrentRage;

		public float m_CurrentPvPEnemyRage;

		public float m_AllBirdDamageInCurrentTurn;

		public float m_AllPigDamageInCurrentTurn;

		public string m_BattleGroundName;

		public VictoryCondition m_CurrentVictoryCondition;

		private bool m_IsRageAvailiablePigs = true;

		private bool m_IsRageAvailiableBirds = true;

		public SkillGameData m_SponsoredEnvironmentalEffect;

		public bool m_CachedTurnEnd;

		private HotspotBalancingData m_hotSpotBalancing;

		public BattleBalancingData Balancing
		{
			get
			{
				return m_Balancing;
			}
		}

		public int CurrentWaveIndex { get; set; }

		public int m_AccumulatedStrengthPoints { get; set; }

		public ICombatant CurrentCombatant { get; set; }

		public ICombatant NextCombatant { get; set; }

		public int BattleLevel
		{
			get
			{
				if (Balancing.BaseLevel > 0)
				{
					return Balancing.BaseLevel;
				}
				return m_ControllerLevel;
			}
		}

		public bool IsPvP { get; set; }

		public bool IsBossBattle { get; set; }

		public bool IsHardMode { get; set; }

		public bool IsDungeon { get; set; }

		public bool IsChronicleCave { get; set; }

		public bool IsUnranked { get; set; }

		public float m_dodgeValue { get; set; }

		public float m_ironCladValue { get; set; }

		public BattleParticipantTableBalancingData InjectedParticipantTable { get; set; }

		[method: MethodImpl(32)]
		public event Action<int> WaveDone;

		[method: MethodImpl(32)]
		public event Action<int> WaveDoneForEffect;

		[method: MethodImpl(32)]
		public event Action<string> EnvironmentalEffectTriggered;

		[method: MethodImpl(32)]
		public event Action<float, ICombatant, bool, SkillBattleDataBase> RageIncreased;

		[method: MethodImpl(32)]
		public event Action<float, ICombatant> RageDecreasedByOpponent;

		[method: MethodImpl(32)]
		public event Action<float, ICombatant> RageUsed;

		[method: MethodImpl(32)]
		public event Action InitiativeChanged;

		[method: MethodImpl(32)]
		public event Action<BattleGameData> CombatantsOutOfInitiativeDone;

		[method: MethodImpl(32)]
		public event Action<List<float>, Faction, int, BossAssetController> SummonCombatant;

		[method: MethodImpl(32)]
		public event Action<int> BirdsTurnStarted;

		[method: MethodImpl(32)]
		public event Action<int> PigsTurnStarted;

		[method: MethodImpl(32)]
		public event Action<Faction> BattleEndedWithWinner;

		public BattleGameData(BattleBalancingData balancing, BattleEndGameData battleEndData)
		{
			m_BattleStartTime = DIContainerLogic.GetTimingService().GetPresentTime();
			m_Balancing = balancing;
			m_BattleEndData = battleEndData;
			m_BattleGroundName = m_Balancing.BackgroundAssetId;
		}

		public BattleGameData(string nameId, bool chronicleCaveBattle = false)
		{
			m_BattleStartTime = DIContainerLogic.GetTimingService().GetPresentTime();
			m_Balancing = DIContainerBalancing.Service.GetBalancingData<BattleBalancingData>(nameId);
			if (chronicleCaveBattle && m_Balancing == null)
			{
				m_Balancing = DIContainerBalancing.Service.GetBalancingData<ChronicleCaveBattleBalancingData>(nameId);
			}
			m_BattleGroundName = m_Balancing.BackgroundAssetId;
			m_BattleEndData = new BattleEndGameData
			{
				m_Level = m_Balancing.BaseLevel,
				m_additionalLootTable = new Dictionary<string, int>(),
				m_wheelLoot = new Dictionary<string, LootInfoData>(),
				m_BattlePerformanceStars = 0,
				m_WinnerFaction = Faction.None,
				m_wheelLootEntries = new List<LootTableEntry>(),
				m_LastWaveIndex = 0
			};
		}

		public void SetRageAvailable(Faction faction, bool available)
		{
			if (faction == Faction.Pigs)
			{
				m_IsRageAvailiablePigs = available;
			}
			else
			{
				m_IsRageAvailiableBirds = available;
			}
		}

		public bool GetRageAvailable(Faction faction)
		{
			if (faction == Faction.Pigs)
			{
				return m_IsRageAvailiablePigs;
			}
			return m_IsRageAvailiableBirds;
		}

		public bool IsRageFull(Faction faction)
		{
			return GetFactionRage(faction) >= 100f;
		}

		public void SetFactionRage(Faction faction, float rage)
		{
			if (faction == Faction.Pigs)
			{
				m_CurrentPvPEnemyRage = rage;
			}
			else
			{
				m_CurrentRage = rage;
			}
		}

		public float GetFactionRage(Faction faction)
		{
			switch (faction)
			{
			case Faction.Birds:
				return m_CurrentRage;
			case Faction.Pigs:
				return m_CurrentPvPEnemyRage;
			case Faction.None:
				return m_CurrentRage;
			case Faction.NonAttackablePig:
				return m_CurrentRage;
			default:
				return m_CurrentRage;
			}
		}

		public int GetPlayerLevelForHotSpot()
		{
			if (Balancing.NameId == "battle_000")
			{
				return 60;
			}
			if (m_hotSpotBalancing == null)
			{
				m_hotSpotBalancing = (from h in DIContainerBalancing.Service.GetBalancingDataList<HotspotBalancingData>()
					where h.BattleId != null && h.BattleId.Contains(Balancing.NameId)
					select h).FirstOrDefault();
				if (m_hotSpotBalancing == null)
				{
					m_hotSpotBalancing = (from ch in DIContainerBalancing.Service.GetBalancingDataList<ChronicleCaveHotspotBalancingData>()
						where ch.BattleId != null && ch.BattleId.Contains(Balancing.NameId)
						select ch).FirstOrDefault();
				}
			}
			if (m_hotSpotBalancing == null)
			{
				return DIContainerInfrastructure.GetCurrentPlayer().Data.Level;
			}
			if (DIContainerInfrastructure.GetCurrentPlayer().Data.GoldenPigHotspotId == m_hotSpotBalancing.NameId)
			{
				return DIContainerInfrastructure.GetCurrentPlayer().Data.Level;
			}
			HotspotGameData value = null;
			if (IsChronicleCave && DIContainerInfrastructure.GetCurrentPlayer().ChronicleCaveGameData.GetAllHotSpots().TryGetValue(m_hotSpotBalancing.NameId, out value))
			{
				return value.GetPigLevelForHotspot(IsHardMode);
			}
			if (DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.HotspotGameDatas.TryGetValue(m_hotSpotBalancing.NameId, out value))
			{
				return value.GetPigLevelForHotspot(IsHardMode);
			}
			if (DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.CurrentMiniCampaign.HotspotGameDatas.TryGetValue(m_hotSpotBalancing.NameId, out value))
			{
				return value.GetPigLevelForHotspot(IsHardMode);
			}
			return DIContainerInfrastructure.GetCurrentPlayer().Data.Level;
		}

		public void OnControllerLevelUp(int level)
		{
			m_ControllerLevel = level;
		}

		public void RaiseWaveDone(int waveIndex)
		{
			if (this.WaveDoneForEffect != null)
			{
				this.WaveDoneForEffect(m_lastPigTurn + 1);
			}
			if (this.WaveDone != null)
			{
				this.WaveDone(waveIndex);
			}
		}

		public void RaiseInitiativeChanged()
		{
			if (this.InitiativeChanged != null)
			{
				this.InitiativeChanged();
			}
		}

		public void RaiseBirdsTurnStarted(int turn)
		{
			if (this.BirdsTurnStarted != null)
			{
				this.BirdsTurnStarted(turn);
			}
		}

		public void RaisePigsTurnStarted(int turn)
		{
			if (this.PigsTurnStarted != null)
			{
				this.PigsTurnStarted(turn);
			}
			m_lastPigTurn = turn;
		}

		public void RaiseCombatantsOutOfInitiativeDone()
		{
			if (this.CombatantsOutOfInitiativeDone != null)
			{
				this.CombatantsOutOfInitiativeDone(this);
			}
		}

		public void RaiseBattleEnded(Faction winningFaction)
		{
			if (this.BattleEndedWithWinner != null)
			{
				this.BattleEndedWithWinner(winningFaction);
			}
		}

		public bool AllDeadOfFaction(Faction faction)
		{
			if (!m_CombatantsPerFaction.ContainsKey(faction))
			{
				return true;
			}
			foreach (ICombatant item in m_CombatantsByInitiative)
			{
				if (item.IsParticipating && item.CombatantFaction == faction)
				{
					return false;
				}
			}
			return true;
		}

		public void RegisterRageIncrease(float rageIncrease, ICombatant source, bool isAttacking, SkillBattleDataBase skill)
		{
			if (this.RageIncreased != null)
			{
				this.RageIncreased(rageIncrease, source, isAttacking, skill);
			}
		}

		public void RegisterRageDecreaseByEnemy(float rageDecrease, ICombatant enemy)
		{
			if (this.RageDecreasedByOpponent != null)
			{
				this.RageDecreasedByOpponent(rageDecrease, enemy);
			}
		}

		public void RegisterEnvironmentalEffectTriggered(string skillName)
		{
			if (this.EnvironmentalEffectTriggered != null)
			{
				this.EnvironmentalEffectTriggered(skillName);
			}
		}

		public void RegisterRageUsed(float rageDecrease, ICombatant source)
		{
			if (IsPvP && source.CombatantFaction == Faction.Birds && !IsUnranked)
			{
				DIContainerLogic.GetPvpObjectivesService().RageUsed();
			}
			if (this.RageUsed != null)
			{
				this.RageUsed(rageDecrease, source);
			}
		}

		public void RaiseSummonCombatant(List<float> values, Faction faction, int init, BossAssetController bossController)
		{
			if (this.SummonCombatant != null)
			{
				this.SummonCombatant(values, faction, init, bossController);
			}
		}
	}
}

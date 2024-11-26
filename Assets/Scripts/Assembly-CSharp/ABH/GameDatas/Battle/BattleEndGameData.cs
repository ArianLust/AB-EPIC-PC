using System;
using System.Collections.Generic;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Battle
{
	public class BattleEndGameData
	{
		public List<LootTableEntry> m_wheelLootEntries = new List<LootTableEntry>();

		public Dictionary<string, int> m_additionalLootTable = new Dictionary<string, int>();

		public Dictionary<string, LootInfoData> m_wheelLoot = new Dictionary<string, LootInfoData>();

		public Dictionary<string, LootInfoData> m_additionalLoot = new Dictionary<string, LootInfoData>();

		public Dictionary<string, LootInfoData> m_lostLoot = new Dictionary<string, LootInfoData>();

		public Faction m_WinnerFaction;

		public int m_BattlePerformanceStars;

		public int m_NeededScoreFor3Stars;

		public int m_ThrownWheelIndex;

		public int m_LastWaveIndex;

		public int m_Score;

		public int m_DisplayScore;

		public DateTime m_BattleEndTime;

		public int m_Level;

		public GoldenPigFinishState m_GoldenPigFinishState;

		public bool m_UnrankedBattle;

		public bool m_IsDungeon;

		public bool m_IsPvp;

		public int m_ReviveUsed;

		public int m_DamageDealt;

		public int m_DamageTaken;

		public int m_PigsLeft;

		public int m_PigsHealthLeft;

		public int m_HealedHealth;

		public List<string> m_RageUsedByBird = new List<string>();

		public LootTableBalancingData m_wheelLootTable;
	}
}

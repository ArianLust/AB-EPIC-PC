using System;
using System.Collections.Generic;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;

namespace ABH.GameDatas.Battle
{
	public class BattleStartGameData
	{
		public AsyncCallback callback;

		public List<BirdGameData> m_Birds;

		public List<BirdGameData> m_PvPBirds = new List<BirdGameData>();

		public BannerGameData m_BirdBanner;

		public BannerGameData m_PigBanner;

		public InventoryGameData m_Inventory;

		public string m_BattleBalancingNameId;

		public List<string> m_PossibleFollowUpBattles = new List<string>();

		public string m_BackgroundAssetId;

		public bool m_RageAvailiable = true;

		public bool m_ChronicleCaveBattle;

		public bool m_IsUnranked;

		public int m_InvokerLevel;

		public int m_BattleRandomSeed;

		public BattleParticipantTableBalancingData m_InjectableParticipantTable;

		public Dictionary<Faction, Dictionary<string, float>> m_FactionBuffs = new Dictionary<Faction, Dictionary<string, float>>();

		public Dictionary<Faction, string> m_EnvironmentalEffects = new Dictionary<Faction, string>();

		public string m_SponsoredEnvironmentalEffect;

		public string m_OpponentId;

		public bool m_IsHardMode;

		public bool m_IsDungeon;

		public bool m_IsChronicleCave;
	}
}

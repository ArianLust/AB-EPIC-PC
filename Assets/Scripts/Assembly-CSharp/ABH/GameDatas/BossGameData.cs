using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Character;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas
{
	public class BossGameData : GameDataBase<BossBalancingData, BossData>, ICharacter
	{
		private float difficultyPercentAttack = 100f;

		private float difficultyPercentHealth = 100f;

		public int m_WeaponIndex;

		private PowerLevelBalancingData m_powerLevelBalancing;

		private int m_difficultyPercentage;

		private List<SkillGameData> m_Skills;

		private SkillGameData m_PassiveSkill;

		private Dictionary<string, LootInfoData> m_DefeatLoot;

		public InventoryGameData InventoryGameData { get; set; }

		public int LastMoveTimestamp { get; set; }

		public CollectionGroupBalancingData CollectionGroupBalancing
		{
			get
			{
				if (BalancingData.CollectionGroupId != string.Empty)
				{
					return DIContainerBalancing.Service.GetBalancingData<CollectionGroupBalancingData>(BalancingData.CollectionGroupId);
				}
				return null;
			}
		}

		public List<AiCombo> SkillCombos
		{
			get
			{
				return BalancingData.SkillCombos;
			}
		}

		public float BaseAttack
		{
			get
			{
				return Mathf.Round(ApplyDifficultyOnAttack(BalancingData.BaseAttack));
			}
		}

		public float BaseHealth
		{
			get
			{
				return Mathf.Round(ApplyDifficultyOnHealth(BalancingData.BaseHealth));
			}
		}

		public List<SkillGameData> Skills
		{
			get
			{
				if (m_Skills == null && BalancingData.SkillNameIds != null)
				{
					m_Skills = new List<SkillGameData>();
					for (int i = 0; i < BalancingData.SkillNameIds.Count; i++)
					{
						string nameId = BalancingData.SkillNameIds[i];
						m_Skills.Add(new SkillGameData(nameId));
					}
					if (PassiveSkill != null)
					{
						m_Skills.Add(PassiveSkill);
					}
				}
				return m_Skills;
			}
		}

		public SkillGameData PassiveSkill
		{
			get
			{
				if (m_PassiveSkill == null && !string.IsNullOrEmpty(BalancingData.PassiveSkillNameId))
				{
					m_PassiveSkill = new SkillGameData(BalancingData.PassiveSkillNameId);
				}
				return m_PassiveSkill;
			}
		}

		public Dictionary<string, LootInfoData> DefeatLoot
		{
			get
			{
				if (m_DefeatLoot != null)
				{
					return m_DefeatLoot;
				}
				m_DefeatLoot = DIContainerLogic.GetLootOperationService().GenerateLoot(BalancingData.LootTableDefeatBonus, Data.Level);
				return m_DefeatLoot;
			}
		}

		public float Scale
		{
			get
			{
				return BalancingData.SizeScale;
			}
		}

		public string Name
		{
			get
			{
				return Data.NameId;
			}
		}

		public string AssetName
		{
			get
			{
				return BalancingData.AssetId;
			}
		}

		public CharacterSizeType CharacterSize
		{
			get
			{
				return BalancingData.SizeType;
			}
		}

		public int Level
		{
			get
			{
				return Data.Level;
			}
		}

		public Faction CharacterFaction
		{
			get
			{
				return Faction.Pigs;
			}
		}

		public bool IsPvPBird { get; set; }

		public bool IsNPC
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		public ClassItemGameData ClassItem { get; set; }

		public EquipmentGameData MainHandItem
		{
			get
			{
				if (InventoryGameData.Items.ContainsKey(InventoryItemType.MainHandEquipment) && InventoryGameData.Items[InventoryItemType.MainHandEquipment].Count > 0)
				{
					return InventoryGameData.Items[InventoryItemType.MainHandEquipment][m_WeaponIndex] as EquipmentGameData;
				}
				return null;
			}
		}

		public EquipmentGameData OffHandItem
		{
			get
			{
				if (InventoryGameData == null || InventoryGameData.Items == null)
				{
					return null;
				}
				if (InventoryGameData.Items.ContainsKey(InventoryItemType.OffHandEquipment) && InventoryGameData.Items[InventoryItemType.OffHandEquipment].Count > 0)
				{
					return InventoryGameData.Items[InventoryItemType.OffHandEquipment][m_WeaponIndex] as EquipmentGameData;
				}
				return null;
			}
		}

		public string EventNodeId
		{
			get
			{
				return Data.EventNodeId;
			}
			set
			{
				Data.EventNodeId = value;
			}
		}

		[method: MethodImpl(32)]
		public event Action<int, int> LevelChanged;

		[method: MethodImpl(32)]
		public event Action ClassItemChanged;

		public BossGameData(string nameId, int level = 1)
		{
			_balancingData = DIContainerBalancing.Service.GetBalancingData<BossBalancingData>(nameId);
			_instancedData = CreateNewInstance(nameId, level);
		}

		public BossGameData(BossData data)
			: base(data)
		{
		}

		public BossGameData SetDifficulties(int playerlevel, BattleBalancingData battle)
		{
			if (battle == null)
			{
				m_difficultyPercentage = 0;
				return this;
			}
			int difficulty = battle.Difficulty;
			PowerLevelBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<PowerLevelBalancingData>("PlayerLevel_" + playerlevel.ToString("00"));
			if (balancingData != null)
			{
				m_powerLevelBalancing = balancingData;
			}
			float num = (float)(DIContainerInfrastructure.GetCurrentPlayer().Data.HighestPowerLevelEver - m_powerLevelBalancing.ExpectedPlayerPowerlevel) / (float)m_powerLevelBalancing.ExpectedPlayerPowerlevel * 100f;
			int num2 = Mathf.RoundToInt((!(num < 0f)) ? (num * battle.PowerLevelThresholdHigh / 100f) : (num * battle.PowerLevelThresholdLow / 100f));
			int difficultyPercentage = difficulty + num2;
			Data.Level = playerlevel;
			if (BalancingData.IgnoreDifficulty)
			{
				return this;
			}
			m_difficultyPercentage = difficultyPercentage;
			return this;
		}

		protected override BossData CreateNewInstance(string nameId)
		{
			return CreateNewInstance(nameId, 1);
		}

		public BossData CreateNewInstance(string nameId, int level)
		{
			InventoryGameData = new InventoryGameData(BalancingData.DefaultInventoryNameId);
			int wheelIndex = 0;
			m_WeaponIndex = 0;
			BossData bossData = new BossData();
			bossData.NameId = nameId;
			bossData.Inventory = InventoryGameData.Data;
			bossData.Level = level;
			BossData bossData2 = bossData;
			if (InventoryGameData != null && InventoryGameData.BalancingData != null)
			{
				DIContainerLogic.GetLootOperationService().RewardLoot(InventoryGameData, 0, DIContainerLogic.GetLootOperationService().GenerateLoot(InventoryGameData.BalancingData.DefaultInventoryContent, bossData2.Level, ref wheelIndex), new Dictionary<string, string>(), EquipmentSource.LootBird);
			}
			return bossData2;
		}

		private float ApplyDifficultyOnAttack(float bossAttack)
		{
			float num = ApplyPowerLevelModifierAttack(bossAttack);
			num += (float)m_difficultyPercentage / 100f * num;
			return Mathf.Round(num);
		}

		private float ApplyDifficultyOnHealth(float bossHealth)
		{
			float num = ApplyPowerLevelModifierHealth(bossHealth);
			num += (float)m_difficultyPercentage / 100f * num;
			return Mathf.Round(num);
		}

		private float ApplyPowerLevelModifierAttack(float attack)
		{
			if (m_powerLevelBalancing != null)
			{
				attack += attack * (m_powerLevelBalancing.AttackModifier / 100f);
			}
			attack += attack * ((float)m_difficultyPercentage / 100f);
			return attack;
		}

		private float ApplyPowerLevelModifierHealth(float health)
		{
			if (m_powerLevelBalancing != null)
			{
				health += health * (m_powerLevelBalancing.HealthModifier / 100f);
			}
			return health;
		}

		public void RaiseLevelChanged(int oldLevel, int newLevel)
		{
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Character;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas
{
	public class PigGameData : ICharacter
	{
		private bool m_ClassChanged;

		private PowerLevelBalancingData m_powerLevelBalancing;

		private int m_difficultyPercentage;

		private PigBalancingData _balancingData;

		protected PigData _instancedData;

		private List<SkillGameData> m_Skills;

		private SkillGameData m_PassiveSkill;

		private Dictionary<string, LootInfoData> m_DefeatLoot;

		public InventoryGameData InventoryGameData { get; set; }

		public PigBalancingData BalancingData
		{
			get
			{
				return _balancingData;
			}
		}

		public PigData Data
		{
			get
			{
				return _instancedData;
			}
		}

		public List<AiCombo> SkillCombos
		{
			get
			{
				if (ClassItem != null && ClassItem.BalancingData.PvPSkillCombos != null && ClassItem.BalancingData.PvPSkillCombos.Count > 0)
				{
					return ClassItem.BalancingData.PvPSkillCombos;
				}
				return BalancingData.SkillCombos;
			}
		}

		public SkinItemGameData ClassSkin
		{
			get
			{
				if (InventoryGameData.Items.ContainsKey(InventoryItemType.Skin))
				{
					return InventoryGameData.Items[InventoryItemType.Skin].FirstOrDefault() as SkinItemGameData;
				}
				return null;
			}
		}

		public ClassItemGameData ClassItem
		{
			get
			{
				if (InventoryGameData.Items.ContainsKey(InventoryItemType.Class))
				{
					return InventoryGameData.Items[InventoryItemType.Class].FirstOrDefault() as ClassItemGameData;
				}
				return null;
			}
		}

		public EquipmentGameData MainHandItem
		{
			get
			{
				if (InventoryGameData.Items.ContainsKey(InventoryItemType.MainHandEquipment))
				{
					return InventoryGameData.Items[InventoryItemType.MainHandEquipment].FirstOrDefault() as EquipmentGameData;
				}
				return null;
			}
		}

		public EquipmentGameData OffHandItem
		{
			get
			{
				if (InventoryGameData.Items.ContainsKey(InventoryItemType.OffHandEquipment))
				{
					return InventoryGameData.Items[InventoryItemType.OffHandEquipment].FirstOrDefault() as EquipmentGameData;
				}
				return null;
			}
		}

		private float CharacterAttack
		{
			get
			{
				return Mathf.Round((float)BalancingData.BaseAttack + (float)BalancingData.BaseAttack * ((float)BalancingData.PerLevelAttack / 100f) * (float)(Data.Level - 1));
			}
		}

		private float ClassIndependentAttack
		{
			get
			{
				float characterAttack = CharacterAttack;
				float num = ((MainHandItem == null) ? 0f : MainHandItem.ItemMainStat);
				return characterAttack + num;
			}
		}

		private float ClassAttack
		{
			get
			{
				if (ClassItem != null)
				{
					ExperienceMasteryBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<ExperienceMasteryBalancingData>("Level_" + ClassItem.Data.Level.ToString("00"));
					if (balancingData == null)
					{
						return 0f;
					}
					return (float)balancingData.StatBonus / 100f * ClassIndependentAttack;
				}
				return 0f;
			}
		}

		public float BaseAttack
		{
			get
			{
				if (IsPvPBird)
				{
					float characterAttack = CharacterAttack;
					float num = ((MainHandItem == null) ? 0f : MainHandItem.ItemMainStat);
					float classAttack = ClassAttack;
					float num2 = Mathf.Round(ApplyDifficultyOnAttack(characterAttack) + num + classAttack);
					float num3 = 0f;
					if (ClassSkin != null)
					{
						num3 = num2 * (ClassSkin.BalancingData.BonusDamage / 100f);
					}
					return num2 + num3;
				}
				return Mathf.Round(ApplyDifficultyOnHealth(BalancingData.BaseAttack));
			}
		}

		private float CharacterHealth
		{
			get
			{
				return Mathf.Round((float)BalancingData.BaseHealth + (float)BalancingData.BaseHealth * ((float)BalancingData.PerLevelHealth / 100f) * (float)(Data.Level - 1));
			}
		}

		private float ClassHealth
		{
			get
			{
				if (ClassItem != null)
				{
					ExperienceMasteryBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<ExperienceMasteryBalancingData>("Level_" + ClassItem.Data.Level.ToString("00"));
					if (balancingData == null)
					{
						return 0f;
					}
					return (float)balancingData.StatBonus / 100f * ClassIndependentHealth;
				}
				return 0f;
			}
		}

		private float ClassIndependentHealth
		{
			get
			{
				float characterHealth = CharacterHealth;
				float num = ((OffHandItem == null) ? 0f : OffHandItem.ItemMainStat);
				return characterHealth + num;
			}
		}

		public float BaseHealth
		{
			get
			{
				if (IsPvPBird)
				{
					float classIndependentHealth = ClassIndependentHealth;
					float classHealth = ClassHealth;
					float num = 0f;
					if (ClassSkin != null)
					{
						num = (classIndependentHealth + classHealth) * (ClassSkin.BalancingData.BonusHp / 100f);
					}
					return classIndependentHealth + classHealth + num;
				}
				return Mathf.Round(ApplyDifficultyOnHealth(BalancingData.BaseHealth));
			}
		}

		public List<SkillGameData> Skills
		{
			get
			{
				if (m_Skills == null && ClassItem != null && ClassItem.PrimaryPvPSkill != null && ClassItem.SecondaryPvPSkill != null)
				{
					m_Skills = new List<SkillGameData>();
					m_Skills.Add(ClassItem.PrimaryPvPSkill);
					m_Skills.Add(ClassItem.SecondaryPvPSkill);
					m_Skills.Add(PassiveSkill);
					return m_Skills;
				}
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

		[method: MethodImpl(32)]
		public event Action ClassItemChanged;

		[method: MethodImpl(32)]
		public event Action<int, int> LevelChanged;

		public PigGameData(BirdGameData bird)
		{
			_balancingData = new PigBalancingData
			{
				AssetId = bird.BalancingData.AssetId,
				BaseHealth = bird.BalancingData.BaseHealth,
				PerLevelAttack = bird.BalancingData.PerLevelAttack,
				PerLevelHealth = bird.BalancingData.PerLevelHealth,
				BaseAttack = bird.BalancingData.BaseAttack,
				Faction = Faction.Pigs,
				LocaId = bird.BalancingData.LocaId,
				NameId = "pvp_" + bird.BalancingData.NameId,
				SizeScale = 1f,
				SizeType = bird.BalancingData.SizeType,
				PigStrength = 1,
				IgnoreDifficulty = true,
				PassiveSkillNameId = bird.BalancingData.PvPRageSkillIdent,
				LootTableDefeatBonus = new Dictionary<string, int>()
			};
			_instancedData = CreateNewInstance(bird);
		}

		public PigGameData(string nameId)
		{
			_balancingData = DIContainerBalancing.Service.GetBalancingData<PigBalancingData>(nameId);
			_instancedData = CreateNewInstance(nameId);
		}

		public PigGameData(string nameId, int level)
		{
			_balancingData = DIContainerBalancing.Service.GetBalancingData<PigBalancingData>(nameId);
			_instancedData = CreateNewInstance(nameId, level);
		}

		public PigGameData SetDifficulties(int playerlevel, BattleBalancingData battle)
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
			else if (playerlevel > DIContainerBalancing.Service.GetBalancingDataList<ExperienceLevelBalancingData>().Count)
			{
				m_powerLevelBalancing = DIContainerBalancing.Service.GetBalancingDataList<PowerLevelBalancingData>().Last();
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

		protected PigData CreateNewInstance(BirdGameData bird)
		{
			InventoryGameData = bird.InventoryGameData;
			int num = 0;
			PigData pigData = new PigData();
			pigData.NameId = "pvp_" + bird.BalancingData.NameId;
			pigData.Inventory = InventoryGameData.Data;
			pigData.Level = bird.Data.Level;
			PigData result = pigData;
			IsPvPBird = true;
			return result;
		}

		protected PigData CreateNewInstance(string nameId, int level = 1)
		{
			InventoryGameData = new InventoryGameData(BalancingData.DefaultInventoryNameId);
			int wheelIndex = 0;
			PigData pigData = new PigData();
			pigData.NameId = nameId;
			pigData.Inventory = InventoryGameData.Data;
			pigData.Level = level;
			PigData pigData2 = pigData;
			if (InventoryGameData != null && InventoryGameData.BalancingData != null)
			{
				DIContainerLogic.GetLootOperationService().RewardLoot(InventoryGameData, 0, DIContainerLogic.GetLootOperationService().GenerateLoot(InventoryGameData.BalancingData.DefaultInventoryContent, pigData2.Level, ref wheelIndex), new Dictionary<string, string>(), EquipmentSource.LootBird);
			}
			return pigData2;
		}

		public string GetClassName()
		{
			if (ClassSkin != null)
			{
				return ClassSkin.ItemLocalizedName;
			}
			return ClassItem.ItemLocalizedName;
		}

		private float ApplyDifficultyOnAttack(float pigAttack)
		{
			float num = ApplyPowerLevelModifierAttack(pigAttack);
			num += (float)m_difficultyPercentage / 100f * num;
			return Mathf.Round(num);
		}

		private float ApplyDifficultyOnHealth(float pigHealth)
		{
			float num = ApplyPowerLevelModifierHealth(pigHealth);
			num += (float)m_difficultyPercentage / 100f * num;
			return Mathf.Round(num);
		}

		public void RaiseLevelChanged(int oldLevel, int newLevel)
		{
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
	}
}

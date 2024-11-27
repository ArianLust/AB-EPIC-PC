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
	public class BirdGameData : GameDataBase<BirdBalancingData, BirdData>, ICharacter
	{
		private bool m_ClassChanged;

		private List<SkillGameData> m_Skills;

		private List<SkillGameData> m_PvPSkills;

		private SkillGameData m_RageSkill;

		private SkillGameData m_PvPRageSkill;

		private BirdGameData bird;

		public InventoryGameData InventoryGameData { get; set; }

		public bool IsNPC { get; set; }

		public List<AiCombo> SkillCombos
		{
			get
			{
				return ClassItem.BalancingData.SkillCombos;
			}
		}

		public List<AiCombo> PvPSkillCombos
		{
			get
			{
				return ClassItem.BalancingData.PvPSkillCombos;
			}
		}

		public List<string> ConditionInteruptCombos
		{
			get
			{
				return ClassItem.BalancingData.InterruptConditionCombos;
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
			set
			{
				InventoryGameData.Items[InventoryItemType.Skin].Clear();
				InventoryGameData.Items[InventoryItemType.Skin].Add(value);
			}
		}

		public ClassItemGameData OverrideClassItem { get; set; }

		public ClassItemGameData ClassItem
		{
			get
			{
				if (OverrideClassItem != null)
				{
					return OverrideClassItem;
				}
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

		public float CharacterAttack
		{
			get
			{
				return Mathf.Round((float)BalancingData.BaseAttack + (float)BalancingData.BaseAttack * ((float)BalancingData.PerLevelAttack / 100f) * (float)(Data.Level - 1));
			}
		}

		public float ClassIndependentAttack
		{
			get
			{
				float characterAttack = CharacterAttack;
				float num = ((MainHandItem == null) ? 0f : MainHandItem.ItemMainStat);
				return characterAttack + num;
			}
		}

		public float ClassAttack
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
				float classIndependentAttack = ClassIndependentAttack;
				float classAttack = ClassAttack;
				float num = 0f;
				if (ClassSkin != null)
				{
					num = (classIndependentAttack + classAttack) * (ClassSkin.BalancingData.BonusDamage / 100f);
				}
				return classIndependentAttack + classAttack + num;
			}
		}

		public float AttackBuff
		{
			get
			{
				return 0f;
			}
		}

		public float CharacterHealth
		{
			get
			{
				return Mathf.Round((float)BalancingData.BaseHealth + (float)BalancingData.BaseHealth * ((float)BalancingData.PerLevelHealth / 100f) * (float)(Data.Level - 1));
			}
		}

		public float BaseHealth
		{
			get
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
		}

		public float ClassHealth
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

		public float ClassIndependentHealth
		{
			get
			{
				float characterHealth = CharacterHealth;
				float num = ((OffHandItem == null) ? 0f : OffHandItem.ItemMainStat);
				return characterHealth + num;
			}
		}

		public float HealthBuff
		{
			get
			{
				return 0f;
			}
		}

		public List<SkillGameData> Skills
		{
			get
			{
				if (m_Skills == null || m_ClassChanged)
				{
					m_Skills = new List<SkillGameData>();
					if (ClassItem.PrimarySkill != null)
					{
						m_Skills.Add(ClassItem.PrimarySkill);
					}
					if (ClassItem.SecondarySkill != null)
					{
						m_Skills.Add(ClassItem.SecondarySkill);
					}
					if (RageSkill != null)
					{
						m_Skills.Add(RageSkill);
					}
					m_ClassChanged = false;
				}
				return m_Skills;
			}
		}

		public List<SkillGameData> PvPSkills
		{
			get
			{
				if (m_PvPSkills == null || m_ClassChanged)
				{
					m_PvPSkills = new List<SkillGameData>();
					if (ClassItem.PrimaryPvPSkill != null)
					{
						m_PvPSkills.Add(ClassItem.PrimaryPvPSkill);
					}
					if (ClassItem.SecondaryPvPSkill != null)
					{
						m_PvPSkills.Add(ClassItem.SecondaryPvPSkill);
					}
					if (PvPRageSkill != null)
					{
						m_PvPSkills.Add(PvPRageSkill);
					}
					m_ClassChanged = false;
				}
				return m_PvPSkills;
			}
		}

		public Dictionary<string, LootInfoData> DefeatLoot
		{
			get
			{
				return null;
			}
		}

		public SkillGameData RageSkill
		{
			get
			{
				if (m_RageSkill == null && BalancingData.RageSkillIdent != null)
				{
					m_RageSkill = new SkillGameData(BalancingData.RageSkillIdent);
				}
				return m_RageSkill;
			}
		}

		public SkillGameData PvPRageSkill
		{
			get
			{
				if (m_PvPRageSkill == null && BalancingData.PvPRageSkillIdent != null)
				{
					m_PvPRageSkill = new SkillGameData(BalancingData.PvPRageSkillIdent);
				}
				return m_PvPRageSkill;
			}
		}

		public float Scale
		{
			get
			{
				return 1f;
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
				return Faction.Birds;
			}
		}

		public bool IsPvPBird
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[method: MethodImpl(32)]
		public event Action ClassItemChanged;

		[method: MethodImpl(32)]
		public event Action<int, int> LevelChanged;

		public BirdGameData(string nameId, int level = 1)
		{
			_balancingData = DIContainerBalancing.Service.GetBalancingData<BirdBalancingData>(nameId);
			_instancedData = CreateNewInstance(nameId, level);
		}

		public BirdGameData(BirdData instance)
			: base(instance)
		{
			InventoryGameData = new InventoryGameData(Data.Inventory);
			InventoryGameData.InventoryOfTypeChanged -= InventoryOfTypeChanged;
			InventoryGameData.InventoryOfTypeChanged += InventoryOfTypeChanged;
		}

		public BirdGameData(BirdGameData bird)
		{
			InventoryGameData = new InventoryGameData(bird.BalancingData.DefaultInventoryNameId);
			InventoryGameData.Items[InventoryItemType.Class] = new List<IInventoryItemGameData>(bird.InventoryGameData.Items[InventoryItemType.Class]);
			InventoryGameData.Items[InventoryItemType.Skin] = new List<IInventoryItemGameData>(bird.InventoryGameData.Items[InventoryItemType.Skin]);
			InventoryGameData.Items[InventoryItemType.MainHandEquipment] = new List<IInventoryItemGameData>(bird.InventoryGameData.Items[InventoryItemType.MainHandEquipment]);
			InventoryGameData.Items[InventoryItemType.OffHandEquipment] = new List<IInventoryItemGameData>(bird.InventoryGameData.Items[InventoryItemType.OffHandEquipment]);
			_balancingData = bird.BalancingData;
			_instancedData = new BirdData();
			Data.IsUnavaliable = bird.Data.IsUnavaliable;
			Data.Level = bird.Data.Level;
			Data.NameId = bird.Data.NameId;
		}

		public void InventoryOfTypeChanged(InventoryItemType type, IInventoryItemGameData item)
		{
			DebugLog.Log("Bird Equipment changed: " + type);
			if (type == InventoryItemType.Class)
			{
				RaiseClassItemChanged();
			}
		}

		public void RaiseClassItemChanged()
		{
			PlayerGameData player = DIContainerInfrastructure.GetCurrentPlayer();
			if (player != null && player.Data.EquippedSkins.ContainsKey(ClassItem.BalancingData.NameId) && !string.IsNullOrEmpty(player.Data.EquippedSkins[ClassItem.BalancingData.NameId]))
			{
				SkinItemGameData item = player.InventoryGameData.Items[InventoryItemType.Skin].Where((IInventoryItemGameData s) => s.ItemBalancing.NameId == player.Data.EquippedSkins[ClassItem.BalancingData.NameId]).FirstOrDefault() as SkinItemGameData;
				DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { item }, InventoryItemType.Skin, InventoryGameData);
			}
			m_ClassChanged = true;
			if (this.ClassItemChanged != null)
			{
				this.ClassItemChanged();
			}
		}

		protected override BirdData CreateNewInstance(string nameId)
		{
			return CreateNewInstance(nameId, 1);
		}

		public BirdData CreateNewInstance(string nameId, int level)
		{
			InventoryGameData = new InventoryGameData(BalancingData.DefaultInventoryNameId);
			int wheelIndex = 0;
			BirdData birdData = new BirdData();
			birdData.NameId = nameId;
			birdData.Inventory = InventoryGameData.Data;
			birdData.Level = level;
			BirdData birdData2 = birdData;
			try
			{
				List<IInventoryItemGameData> list = DIContainerLogic.GetLootOperationService().RewardLoot(InventoryGameData, 0, DIContainerLogic.GetLootOperationService().GenerateLoot(InventoryGameData.BalancingData.DefaultInventoryContent, (InventoryGameData.BalancingData.InitializingLevel != 0) ? InventoryGameData.BalancingData.InitializingLevel : birdData2.Level, ref wheelIndex), "bird_creation_" + BalancingData.NameId, EquipmentSource.LootBird);
				foreach (IInventoryItemGameData item in list)
				{
					item.ItemData.IsNew = false;
				}
			}
			catch
			{
			}
			InventoryGameData.InventoryOfTypeChanged -= InventoryOfTypeChanged;
			InventoryGameData.InventoryOfTypeChanged += InventoryOfTypeChanged;
			return birdData2;
		}

		public string GetClassName()
		{
			if (ClassSkin != null)
			{
				return ClassSkin.ItemLocalizedName;
			}
			return ClassItem.ItemLocalizedName;
		}

		public void RaiseLevelChanged(int oldLevel, int newLevel)
		{
			if (this.LevelChanged != null)
			{
				this.LevelChanged(oldLevel, newLevel);
			}
		}

		public bool HasSkinsAvailable()
		{
			bool flag = (from cb in DIContainerBalancing.Service.GetBalancingDataList<ClassSkinBalancingData>()
				where cb.OriginalClass == ClassItem.BalancingData.NameId && cb.ShowPreview
				select cb).Count() > 1;
			bool flag2 = GetClassSkins().Length > 1;
			return flag || flag2;
		}

		public SkinItemGameData GetNextSkin()
		{
			SkinItemGameData[] classSkins = GetClassSkins();
			if (classSkins.Length == 1)
			{
				return null;
			}
			int sortPriority = ClassSkin.BalancingData.SortPriority;
			if (sortPriority + 1 == classSkins.Length)
			{
				return classSkins[0];
			}
			return classSkins[sortPriority + 1];
		}

		private SkinItemGameData[] GetClassSkins()
		{
			List<IInventoryItemGameData> list = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.Skin].Where((IInventoryItemGameData i) => i.IsValidForBird(this)).ToList();
			List<SkinItemGameData> list2 = new List<SkinItemGameData>();
			for (int j = 0; j < list.Count; j++)
			{
				SkinItemGameData skinItemGameData = list[j] as SkinItemGameData;
				if (skinItemGameData != null && !(skinItemGameData.BalancingData.OriginalClass != ClassItem.BalancingData.NameId))
				{
					list2.Add(skinItemGameData);
				}
			}
			SkinItemGameData[] array = new SkinItemGameData[list2.Count];
			for (int k = 0; k < list2.Count; k++)
			{
				SkinItemGameData skinItemGameData2 = list2[k];
				if (skinItemGameData2.BalancingData.SortPriority > array.Length)
				{
					DebugLog.Error("Balancing error! Sortpriority of skins need to be consecutive!");
				}
				else
				{
					array[skinItemGameData2.BalancingData.SortPriority] = skinItemGameData2;
				}
			}
			return array;
		}
	}
}

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
	public class BannerGameData : ICharacter
	{
		private bool m_ClassChanged;

		private float difficultyPercentAttack = 100f;

		private float difficultyPercentHealth = 100f;

		protected BannerBalancingData _balancingData;

		protected BannerData _instancedData;

		private BannerItemGameData m_bannerTip;

		private BannerItemGameData m_bannerCenter;

		private BannerItemGameData m_bannerEmblem;

		private List<SkillGameData> m_Skills;

		private List<SkillGameData> m_BannerEmblemSkills;

		private List<SkillGameData> m_BannerCenterSkills;

		private Dictionary<string, LootInfoData> m_DefeatLoot;

		private Faction m_CharacterFaction;

		public InventoryGameData InventoryGameData { get; set; }

		public BannerBalancingData BalancingData
		{
			get
			{
				return _balancingData;
			}
		}

		public BannerData Data
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
				return null;
			}
		}

		public ClassItemGameData ClassItem
		{
			get
			{
				return null;
			}
		}

		public EquipmentGameData MainHandItem
		{
			get
			{
				return null;
			}
		}

		public EquipmentGameData OffHandItem
		{
			get
			{
				return null;
			}
		}

		public float BaseAttack
		{
			get
			{
				return 0f;
			}
		}

		public float BaseHealth
		{
			get
			{
				return CharacterHealth + ((BannerCenter == null) ? 0f : BannerCenter.ItemMainStat) + ((BannerEmblem == null) ? 0f : BannerEmblem.ItemMainStat) + ((BannerTip == null) ? 0f : BannerTip.ItemMainStat);
			}
		}

		public float CharacterHealth
		{
			get
			{
				return Mathf.Round((float)BalancingData.BaseHealth + (float)BalancingData.BaseHealth * ((float)BalancingData.PerLevelHealth / 100f) * (float)(Data.Level - 1));
			}
		}

		public BannerItemGameData BannerTip
		{
			get
			{
				if (InventoryGameData.Items.ContainsKey(InventoryItemType.BannerTip))
				{
					BannerItemGameData bannerItemGameData = InventoryGameData.Items[InventoryItemType.BannerTip].FirstOrDefault() as BannerItemGameData;
					if (bannerItemGameData != m_bannerTip)
					{
						m_bannerTip = bannerItemGameData;
						EquipmentChanged();
					}
					return m_bannerTip;
				}
				return null;
			}
		}

		public BannerItemGameData BannerCenter
		{
			get
			{
				if (InventoryGameData.Items.ContainsKey(InventoryItemType.Banner))
				{
					BannerItemGameData bannerItemGameData = InventoryGameData.Items[InventoryItemType.Banner].FirstOrDefault() as BannerItemGameData;
					if (bannerItemGameData != m_bannerCenter)
					{
						m_bannerCenter = bannerItemGameData;
						EquipmentChanged();
					}
					return m_bannerCenter;
				}
				return null;
			}
		}

		public BannerItemGameData BannerEmblem
		{
			get
			{
				if (InventoryGameData.Items.ContainsKey(InventoryItemType.BannerEmblem))
				{
					BannerItemGameData bannerItemGameData = InventoryGameData.Items[InventoryItemType.BannerEmblem].FirstOrDefault() as BannerItemGameData;
					if (bannerItemGameData != m_bannerEmblem)
					{
						m_bannerEmblem = bannerItemGameData;
						EquipmentChanged();
					}
					return m_bannerEmblem;
				}
				return null;
			}
		}

		public List<SkillGameData> Skills
		{
			get
			{
				if (m_Skills == null)
				{
					m_Skills = new List<SkillGameData>();
					if (BannerTip != null && BannerTip.BalancingData.SkillNameIds != null)
					{
						for (int i = 0; i < BannerTip.BalancingData.SkillNameIds.Count; i++)
						{
							string nameId = BannerTip.BalancingData.SkillNameIds[i];
							m_Skills.Add(new SkillGameData(nameId));
						}
					}
					if (BannerCenter != null)
					{
						m_Skills.AddRange(BannerCenterSkills);
					}
					if (BannerEmblem != null)
					{
						m_Skills.AddRange(BannerEmblemSkills);
					}
				}
				return m_Skills;
			}
		}

		private List<SkillGameData> BannerEmblemSkills
		{
			get
			{
				if (m_BannerEmblemSkills == null && BannerEmblem != null)
				{
					m_BannerEmblemSkills = new List<SkillGameData>();
					if (BannerEmblem.BalancingData.SkillNameIds != null)
					{
						for (int i = 0; i < BannerEmblem.BalancingData.SkillNameIds.Count; i++)
						{
							string nameId = BannerEmblem.BalancingData.SkillNameIds[i];
							m_BannerEmblemSkills.Add(new SkillGameData(nameId));
						}
					}
				}
				return m_BannerEmblemSkills;
			}
		}

		private List<SkillGameData> BannerCenterSkills
		{
			get
			{
				if (m_BannerCenterSkills == null && BannerCenter != null)
				{
					m_BannerCenterSkills = new List<SkillGameData>();
					if (BannerCenter.BalancingData.SkillNameIds != null)
					{
						for (int i = 0; i < BannerCenter.BalancingData.SkillNameIds.Count; i++)
						{
							string nameId = BannerCenter.BalancingData.SkillNameIds[i];
							m_BannerCenterSkills.Add(new SkillGameData(nameId));
						}
					}
				}
				return m_BannerCenterSkills;
			}
		}

		public Dictionary<string, LootInfoData> DefeatLoot
		{
			get
			{
				if (m_DefeatLoot != null)
				{
					return new Dictionary<string, LootInfoData>();
				}
				return m_DefeatLoot;
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
				return m_CharacterFaction;
			}
		}

		public bool IsPvPBird { get; set; }

		public bool IsNPC
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

		public BannerGameData(string nameId, int level = 1)
		{
			_balancingData = DIContainerBalancing.Service.GetBalancingData<BannerBalancingData>(nameId);
			_instancedData = CreateNewInstance(nameId, level);
		}

		public BannerGameData(BannerData instance)
		{
			_instancedData = instance;
			_balancingData = DIContainerBalancing.Service.GetBalancingData<BannerBalancingData>(instance.NameId);
			InventoryGameData = new InventoryGameData(instance.Inventory);
		}

		public BannerGameData(BannerGameData banner)
		{
			InventoryGameData = new InventoryGameData(banner.BalancingData.DefaultInventoryNameId);
			InventoryGameData.Items[InventoryItemType.BannerEmblem] = banner.InventoryGameData.Items[InventoryItemType.BannerEmblem];
			_balancingData = banner.BalancingData;
			_instancedData = new BannerData();
			Data.Level = banner.Data.Level;
			Data.NameId = banner.Data.NameId;
		}

		public BannerGameData SetFaction(Faction faction)
		{
			m_CharacterFaction = faction;
			return this;
		}

		protected BannerData CreateNewInstance(string nameId, int level)
		{
			InventoryGameData = new InventoryGameData(BalancingData.DefaultInventoryNameId);
			int wheelIndex = 0;
			BannerData bannerData = new BannerData();
			bannerData.NameId = nameId;
			bannerData.Inventory = InventoryGameData.Data;
			bannerData.Level = level;
			BannerData bannerData2 = bannerData;
			if (InventoryGameData != null && InventoryGameData.BalancingData != null)
			{
				DIContainerLogic.GetLootOperationService().RewardLoot(InventoryGameData, 0, DIContainerLogic.GetLootOperationService().GenerateLoot(InventoryGameData.BalancingData.DefaultInventoryContent, bannerData2.Level, ref wheelIndex), new Dictionary<string, string>(), EquipmentSource.LootBird);
			}
			return bannerData2;
		}

		private void EquipmentChanged()
		{
			m_Skills = null;
			m_BannerEmblemSkills = null;
			m_BannerCenterSkills = null;
		}

		public void RaiseLevelChanged(int oldLevel, int newLevel)
		{
		}
	}
}

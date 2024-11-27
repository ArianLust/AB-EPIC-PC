using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ABH.Shared.BalancingData;
using ABH.Shared.Interfaces;
using ABH.Shared.Models.InventoryItems;

namespace ABH.GameDatas
{
	public class SkinItemGameData : GameDataBase<ClassSkinBalancingData, SkinItemData>, IInventoryItemGameData
	{
		public string Name
		{
			get
			{
				return BalancingData.LocaBaseId;
			}
		}

		public IInventoryItemBalancingData ItemBalancing
		{
			get
			{
				return BalancingData;
			}
		}

		public IInventoryItemData ItemData
		{
			get
			{
				return Data;
			}
		}

		public string ItemAssetName
		{
			get
			{
				return BalancingData.AssetBaseId;
			}
		}

		public string ItemLocalizedName
		{
			get
			{
				return DIContainerInfrastructure.GetLocaService().GetClassName(BalancingData.LocaBaseId);
			}
		}

		public string ItemLocalizedDesc
		{
			get
			{
				return DIContainerInfrastructure.GetLocaService().GetClassDesc(BalancingData.LocaBaseId);
			}
		}

		public float ItemMainStat
		{
			get
			{
				return 0f;
			}
		}

		public SkillGameData PrimarySkill
		{
			get
			{
				return null;
			}
		}

		public SkillGameData SecondarySkill
		{
			get
			{
				return null;
			}
		}

		public int ItemValue
		{
			get
			{
				return Data.Value;
			}
			set
			{
				Data.Value = value;
			}
		}

		public string ItemIconAtlasName
		{
			get
			{
				return null;
			}
		}

		[method: MethodImpl(32)]
		public event Action<IInventoryItemGameData, float> ItemDataChanged;

		public SkinItemGameData(string nameId)
			: base(nameId)
		{
		}

		public SkinItemGameData(SkinItemData instance)
			: base(instance)
		{
		}

		protected override SkinItemData CreateNewInstance(string nameId)
		{
			return new SkinItemData();
		}

		public void RaiseItemDataChanged(float delta)
		{
			if (this.ItemDataChanged != null)
			{
				this.ItemDataChanged(this, delta);
			}
		}

		public bool IsValidForBird(BirdGameData bird)
		{
			if (bird == null)
			{
				return false;
			}
			if (string.IsNullOrEmpty(BalancingData.OriginalClass))
			{
				return false;
			}
			ClassItemBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<ClassItemBalancingData>(BalancingData.OriginalClass);
			return balancingData.RestrictedBirdId.Equals(bird.BalancingData.NameId);
		}

		public ClassItemBalancingData GetOriginalClassBalancingData()
		{
			return DIContainerBalancing.Service.GetBalancingData<ClassItemBalancingData>(BalancingData.OriginalClass);
		}

		public string ItemLocalizedTooltipDesc(InventoryGameData inventory)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", DIContainerLogic.InventoryService.GetItemValue(inventory, BalancingData.NameId).ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetItemTooltipDesc(BalancingData.LocaBaseId, dictionary);
		}

		public void ResetValue()
		{
		}
	}
}

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ABH.Shared.BalancingData;
using ABH.Shared.Interfaces;
using ABH.Shared.Models.InventoryItems;

namespace ABH.GameDatas
{
	public class MasteryItemGameData : GameDataBase<MasteryItemBalancingData, MasteryItemData>, IInventoryItemGameData
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
				return DIContainerInfrastructure.GetLocaService().GetMasteryItemName(BalancingData.LocaBaseId);
			}
		}

		public string ItemLocalizedDesc
		{
			get
			{
				return DIContainerInfrastructure.GetLocaService().GetMasteryItemDesc(BalancingData.LocaBaseId);
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

		public MasteryItemGameData(string nameId)
			: base(nameId)
		{
		}

		public MasteryItemGameData(MasteryItemData instance)
			: base(instance)
		{
		}

		protected override MasteryItemData CreateNewInstance(string nameId)
		{
			return new MasteryItemData();
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
			return false;
		}

		public string ItemLocalizedTooltipDesc(InventoryGameData inventory)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", DIContainerLogic.InventoryService.GetItemValue(inventory, BalancingData.NameId).ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetItemTooltipDesc(BalancingData.LocaBaseId, dictionary);
		}

		public void ResetValue()
		{
			Data.Value = 0;
		}

		public string GetMasteryClassName(PlayerGameData player)
		{
			if (!player.Data.EquippedSkins.ContainsKey(BalancingData.AssociatedClass))
			{
				return string.Empty;
			}
			ClassSkinBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<ClassSkinBalancingData>(player.Data.EquippedSkins[BalancingData.AssociatedClass]);
			return DIContainerInfrastructure.GetLocaService().GetClassName(balancingData.LocaBaseId);
		}
	}
}

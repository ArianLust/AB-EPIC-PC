using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ABH.Shared.BalancingData;
using ABH.Shared.Interfaces;
using ABH.Shared.Models.InventoryItems;

namespace ABH.GameDatas
{
	public class EventItemGameData : GameDataBase<EventItemBalancingData, EventItemData>, IInventoryItemGameData
	{
		public string Name
		{
			get
			{
				return BalancingData.LocaBaseId;
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
				return DIContainerInfrastructure.GetLocaService().GetItemName(BalancingData.LocaBaseId);
			}
		}

		public string ItemLocalizedDesc
		{
			get
			{
				return DIContainerInfrastructure.GetLocaService().GetItemDesc(BalancingData.LocaBaseId);
			}
		}

		public string ItemIconAtlasName
		{
			get
			{
				return "GenericElements";
			}
		}

		[method: MethodImpl(32)]
		public event Action<IInventoryItemGameData, float> ItemDataChanged;

		public EventItemGameData(string nameId)
			: base(nameId)
		{
		}

		public EventItemGameData(EventItemData instance)
			: base(instance)
		{
		}

		protected override EventItemData CreateNewInstance(string nameId)
		{
			EventItemData eventItemData = new EventItemData();
			eventItemData.NameId = nameId;
			eventItemData.Level = 0;
			eventItemData.Quality = 0;
			eventItemData.Value = 0;
			return eventItemData;
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
		}
	}
}

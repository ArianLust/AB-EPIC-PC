using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

public class BirdWindowUIBase : MonoBehaviour
{
	[SerializeField]
	public UIInputTrigger m_ButtonClose;

	[HideInInspector]
	public InventoryItemSlot m_SelectedSlot;

	public virtual UIGrid getItemGrid()
	{
		return null;
	}

	public virtual BirdWindowUIBase SetStateMgr(BaseCampStateMgr stateMgr)
	{
		return this;
	}

	public virtual void SetModel(InventoryGameData inventory, List<BirdGameData> birds, int selectedIndex, InventoryItemType defaultItemType = InventoryItemType.Class)
	{
	}

	public virtual IEnumerator SoftRefresh(IInventoryItemGameData removed, List<IInventoryItemGameData> scrapLoot)
	{
		yield break;
	}

	public virtual void UpdateSlotIndicators()
	{
	}
}

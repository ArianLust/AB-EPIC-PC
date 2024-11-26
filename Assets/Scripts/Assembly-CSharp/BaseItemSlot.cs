using ABH.GameDatas;
using UnityEngine;

public class BaseItemSlot : MonoBehaviour
{
	public virtual bool SetModel(IInventoryItemGameData item, bool isPvp)
	{
		return false;
	}

	public virtual IInventoryItemGameData GetModel()
	{
		return null;
	}

	public virtual void Select(bool classPreviewIsThis = false)
	{
	}

	public virtual void Deselect(bool classPreviewIsNext = false)
	{
	}
}

using ABH.GameDatas;
using UnityEngine;

internal class CampMasteryBadge : MonoBehaviour
{
	private IInventoryItemGameData m_masteryBadge;

	public void Start()
	{
		if (!DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "unlock_mastery_badge", out m_masteryBadge))
		{
			DebugLog.Error("CampMasteryBadge:Start:Cannot find mastery badge");
		}
	}

	public void ShowMasteryBadgeOverlay()
	{
		DebugLog.Log("BUTTON PRESSED");
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowMasteryBadgeOverlay(base.transform, m_masteryBadge, true);
	}
}

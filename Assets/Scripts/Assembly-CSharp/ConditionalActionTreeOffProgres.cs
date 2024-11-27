using System;
using ABH.GameDatas;

[Serializable]
public class ConditionalActionTreeOffProgres
{
	public ActionTree ActionTree;

	public string TriggeringItemName;

	public bool IsActive()
	{
		IInventoryItemGameData data = null;
		if (!DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, TriggeringItemName, out data))
		{
			return false;
		}
		return DIContainerInfrastructure.GetCurrentPlayer().Data.PendingFeatureUnlocks.Contains(TriggeringItemName);
	}
}

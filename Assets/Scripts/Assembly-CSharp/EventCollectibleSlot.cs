using ABH.GameDatas;
using UnityEngine;

public class EventCollectibleSlot : MonoBehaviour
{
	private EventItemGameData m_EventItemData;

	[HideInInspector]
	public GameObject m_DisplayAsset;

	public void SetModel(EventItemGameData data, GameObject display)
	{
		m_EventItemData = data;
		m_DisplayAsset = display;
	}

	public void ShowCollectibleTooltip()
	{
		if ((bool)m_DisplayAsset)
		{
			string localizedText = DIContainerInfrastructure.GetLocaService().Tr(m_EventItemData.BalancingData.LocaBaseId + "_tt");
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowGenericOverlay(m_DisplayAsset.transform, localizedText, true);
		}
	}
}

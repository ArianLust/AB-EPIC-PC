using System.Collections.Generic;
using ABH.Shared.BalancingData;
using UnityEngine;

public class ChronicleCaveFloorProxy : ChronicleCaveFloorSlot
{
	public List<GameObject> m_ProxyHotspots = new List<GameObject>();

	private void Awake()
	{
		m_StateMgr = DIContainerInfrastructure.LocationStateMgr as ChronicleCaveStateMgr;
		base.transform.parent = m_StateMgr.m_FloorRoot;
		m_StateMgr.m_CurrentFloor = this;
		m_StateMgr.m_Floors.Add(this);
		base.transform.localPosition += (m_StateMgr.m_Floors.Count - 1) * m_StateMgr.m_UpperFloorPosition;
		if (DIContainerBalancing.Service.GetBalancingDataList<ChronicleCaveFloorBalancingData>().Count > m_StateMgr.m_Floors.Count)
		{
			m_StateMgr.InitNextProxyFloor(m_StateMgr.m_Floors.Count);
		}
	}

	public override void SetFriendProgressMarker(ChronicleCaveFloorBalancingData ccfbd, FriendProgressIndicator fpi, HotspotBalancingData hotspot)
	{
		int num = hotspot.ProgressId - DIContainerBalancing.Service.GetBalancingData<ChronicleCaveHotspotBalancingData>(ccfbd.FirstChronicleCaveHotspotId).ProgressId;
		if (m_ProxyHotspots.Count > num)
		{
			GameObject gameObject = m_ProxyHotspots[num];
			fpi.transform.parent = gameObject.transform;
			fpi.transform.localPosition = Vector3.zero;
			fpi.GetComponent<Animation>().Play("FriendMarker_Show");
			fpi.GetComponent<Animation>().PlayQueued("FriendMarker_Idle");
		}
	}
}

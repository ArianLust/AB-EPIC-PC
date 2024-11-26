using System.Collections;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using UnityEngine;

public class ChronicleCaveFloorSlot : MonoBehaviour
{
	public RageSplineLite m_LeavingPath;

	public HotSpotWorldMapViewBase m_EnteringNode;

	public HotSpotWorldMapViewBase m_EnteringHotSpot;

	public HotSpotWorldMapViewBase m_LeavingHotSpot;

	public HotSpotWorldMapViewBase m_LeavingNode;

	protected ChronicleCaveStateMgr m_StateMgr;

	protected ChronicleCaveFloorGameData m_Model;

	public ChronicleCaveFloorSlot SetStateMgr(ChronicleCaveStateMgr stateMgr)
	{
		m_StateMgr = stateMgr;
		return this;
	}

	public virtual void InstantiateRedKeyBubble()
	{
	}

	public ChronicleCaveFloorGameData GetModel()
	{
		return m_Model;
	}

	public virtual void SetFriendProgressMarker(ChronicleCaveFloorBalancingData ccfbd, FriendProgressIndicator fpi, HotspotBalancingData hotspot)
	{
	}

	public virtual IEnumerator ActivateHotspots()
	{
		yield break;
	}

	public virtual ActionTree GetIntroCutscene()
	{
		return null;
	}

	public virtual void HideBoss(bool hide)
	{
	}

	public virtual GameObject GetGate()
	{
		return null;
	}
}

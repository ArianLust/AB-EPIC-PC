using System;
using System.Collections;
using ABH.GameDatas;
using UnityEngine;

public class EventCampaignMapManager : MonoBehaviour
{
	protected EventCampaignGameData m_Model;

	protected EventCampaignStateMgr m_StateMgr;

	public HotSpotWorldMapViewBase m_StartingHotSpot;

	public HotSpotWorldMapViewBase m_EndHotSpot;

	public ActionTree m_Intro;

	public ActionTree m_Outro;

	public ContainerControl m_DragControlInteractionSpace;

	[HideInInspector]
	public Func<bool> m_isMovementPossible;

	public IEnumerator ActivateHotspots()
	{
		if (!m_StartingHotSpot.IsCompleted())
		{
			DebugLog.Log("first start");
			DIContainerLogic.WorldMapService.CompleteHotSpot(DIContainerInfrastructure.GetCurrentPlayer(), m_StartingHotSpot.Model, 3, 0);
		}
		yield return StartCoroutine(m_StartingHotSpot.ActivateFollowUpStagesAsync(null, null));
	}

	public EventCampaignMapManager SetStateMgr(EventCampaignStateMgr stateMgr)
	{
		m_StateMgr = stateMgr;
		return this;
	}

	public virtual void InstantiateRedKeyBubble()
	{
	}

	public EventCampaignGameData GetModel()
	{
		return m_Model;
	}

	public virtual ActionTree GetIntroCutscene()
	{
		return null;
	}

	public virtual GameObject GetGate()
	{
		return null;
	}
}

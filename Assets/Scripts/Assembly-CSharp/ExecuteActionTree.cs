using ABH.Shared.Generic;
using UnityEngine;

public class ExecuteActionTree : MonoBehaviour
{
	[SerializeField]
	public ActionTree m_ActionTree;

	public bool m_executeBeforeUnlock;

	public bool m_propagateRateAppPopup;

	private BaseLocationStateManager m_WorldMapStatMgr;

	private MonoBehaviour m_callingScript;

	public void StartActionTree(MonoBehaviour callingScript)
	{
		if (DIContainerInfrastructure.GetCoreStateMgr().m_DisableStorySequences)
		{
			m_ActionTree.isFinished = true;
			return;
		}
		if ((bool)m_WorldMapStatMgr)
		{
			m_WorldMapStatMgr.EnableInput(false);
		}
		if (m_propagateRateAppPopup)
		{
			DIContainerLogic.RateAppController.RequestRatePopupForReason(RatePopupTrigger.ChronicleCave);
		}
		m_ActionTree.Load(m_ActionTree.startNode);
	}

	public bool IsDone()
	{
		return m_ActionTree.isFinished || m_ActionTree.node == null;
	}

	internal void SetStateMgr(BaseLocationStateManager stateMgr)
	{
		m_WorldMapStatMgr = stateMgr;
	}
}

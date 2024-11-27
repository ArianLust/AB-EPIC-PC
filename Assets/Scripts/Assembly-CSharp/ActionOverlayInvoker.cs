using UnityEngine;

[RequireComponent(typeof(UITapHoldTrigger), typeof(BoxCollider))]
public class ActionOverlayInvoker : MonoBehaviour
{
	private UITapHoldTrigger m_TapHoldTrigger;

	public string m_OpenActionName = "ShowTooltip";

	public string m_HideActionName = "HideAllTooltips";

	public GameObject m_ReceiverObject;

	public bool m_IsTapping;

	private void Awake()
	{
		m_TapHoldTrigger = GetComponent<UITapHoldTrigger>();
		RegisterEventHandlers();
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		m_TapHoldTrigger.OnTapBegin += OnTapBegin;
		m_TapHoldTrigger.OnTapReleased += OnTapEnd;
	}

	private void OnTapEnd()
	{
		if ((bool)DIContainerInfrastructure.GetCoreStateMgr() && (bool)DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.SendMessage(m_HideActionName);
		}
		m_IsTapping = false;
	}

	private void OnTapBegin()
	{
		if ((bool)m_ReceiverObject)
		{
			m_ReceiverObject.SendMessage(m_OpenActionName, SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			SendMessage(m_OpenActionName, SendMessageOptions.DontRequireReceiver);
		}
		m_IsTapping = true;
	}

	private void DeRegisterEventHandlers()
	{
		if ((bool)m_TapHoldTrigger)
		{
			m_TapHoldTrigger.OnTapBegin -= OnTapBegin;
			m_TapHoldTrigger.OnTapReleased -= OnTapEnd;
		}
	}

	private void OnDestroy()
	{
		if (m_IsTapping)
		{
			OnTapEnd();
			if ((bool)m_TapHoldTrigger)
			{
				m_TapHoldTrigger.ResetUICamera();
			}
		}
		DeRegisterEventHandlers();
	}
}

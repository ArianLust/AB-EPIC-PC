using UnityEngine;

[RequireComponent(typeof(UITapHoldTrigger), typeof(BoxCollider))]
public class GenericOverlayInvoker : MonoBehaviour
{
	private UITapHoldTrigger m_TapHoldTrigger;

	public bool m_UseHeader;

	public string m_HeaderLocaIdent;

	public string m_HeaderDefaultText;

	public string m_LocaIdent;

	public string m_DefaultText;

	public Transform m_Root;

	public bool m_IsTapping;

	private void Awake()
	{
		m_TapHoldTrigger = GetComponent<UITapHoldTrigger>();
		RegisterEventHandlers();
		if (!m_Root)
		{
			m_Root = base.transform;
		}
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		m_TapHoldTrigger.OnTapBegin += OnTapBegin;
		m_TapHoldTrigger.OnTapReleased += OnTapEnd;
	}

	public void OnTapEnd()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideGenericOverlay();
		m_IsTapping = false;
	}

	public void OnTapBegin()
	{
		if (m_Root.gameObject.activeInHierarchy)
		{
			m_IsTapping = true;
			string localizedText = ((!string.IsNullOrEmpty(m_DefaultText)) ? DIContainerInfrastructure.GetLocaService().Tr(m_LocaIdent, m_DefaultText) : DIContainerInfrastructure.GetLocaService().Tr(m_LocaIdent));
			if (m_UseHeader)
			{
				string text = ((!string.IsNullOrEmpty(m_HeaderDefaultText)) ? DIContainerInfrastructure.GetLocaService().Tr(m_LocaIdent, m_HeaderLocaIdent) : DIContainerInfrastructure.GetLocaService().Tr(m_HeaderLocaIdent));
			}
			else
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowGenericOverlay(m_Root, localizedText, base.gameObject.layer == LayerMask.NameToLayer("Interface"));
			}
		}
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

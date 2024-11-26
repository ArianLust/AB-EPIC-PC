using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ClickTooltipInvoker : MonoBehaviour
{
	public string m_AssetName;

	public string m_HeaderLocaIdent;

	public string m_DescLocaIdent;

	public float m_Duration = 3f;

	private void OnTouchClicked()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.ShowNonInteractableTooltip(m_AssetName, m_HeaderLocaIdent, m_DescLocaIdent, m_Duration);
	}
}

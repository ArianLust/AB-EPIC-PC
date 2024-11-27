using UnityEngine;

internal class Wp8SocialWindowTooltip : MonoBehaviour
{
	public void ShowWp8DependentTooltip()
	{
		string localizedText = DIContainerInfrastructure.GetLocaService().Tr("camp_tt_rovioid", "Rovio Account Log into your Rovio Account to share your game data on multiple devices! Awesome!");
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowGenericOverlay(base.transform, localizedText, base.gameObject.layer == LayerMask.NameToLayer("Interface"));
	}
}

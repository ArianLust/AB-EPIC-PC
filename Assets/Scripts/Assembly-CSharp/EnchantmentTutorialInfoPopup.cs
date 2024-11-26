using System.Collections;
using UnityEngine;

public class EnchantmentTutorialInfoPopup : MonoBehaviour
{
	[SerializeField]
	private UIInputTrigger m_CloseButton;

	[SerializeField]
	private Animation m_PopupAnimation;

	private void OnDestroy()
	{
		DeregisterEventHandler();
	}

	private void RegisterEventHandler()
	{
		DeregisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(7, ClosePopup);
		m_CloseButton.Clicked += ClosePopup;
	}

	private void DeregisterEventHandler()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(7);
		m_CloseButton.Clicked -= ClosePopup;
	}

	public void ClosePopup()
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(LeaveCoroutine());
		}
	}

	private IEnumerator LeaveCoroutine()
	{
		DeregisterEventHandler();
		m_PopupAnimation.Play("Popup_Leave");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(6u);
		yield return new WaitForSeconds(0.5f);
		base.gameObject.SetActive(false);
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		StartCoroutine(EnterCoroutine());
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("enchantment_tutorial_info");
		yield return new WaitForSeconds(0.1f);
		m_PopupAnimation.Play("Popup_Enter");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 6u,
			showEnergy = false,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false
		}, false);
		yield return new WaitForSeconds(0.5f);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("enchantment_tutorial_info");
		RegisterEventHandler();
	}
}

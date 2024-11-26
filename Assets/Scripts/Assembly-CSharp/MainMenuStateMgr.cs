using System;
using System.Collections;
using ABH.Shared.Generic;
using UnityEngine;

public class MainMenuStateMgr : MonoBehaviour
{
	[SerializeField]
	public Animation m_LogoAnimation;

	[SerializeField]
	public Animation m_PlayButtonAnimation;

	[SerializeField]
	public Animation m_OptionsPanelAnimation;

	[SerializeField]
	public Animation m_MiscPanelAnimation;

	[SerializeField]
	public UIInputTrigger m_PlayButton;

	[SerializeField]
	public UIInputTrigger m_OptionsButton;

	[SerializeField]
	public UIInputTrigger m_MiscButton;

	[SerializeField]
	public OptionsMgr m_OptionsMgr;

	[SerializeField]
	public MiscMgr m_MiscMgr;

	[SerializeField]
	public GameObject m_OptionsButtonMgr;

	[SerializeField]
	public GameObject m_MiscButtonMgr;

	private bool m_bOptionsOpen;

	private bool m_bMiscOpen;

	private bool m_Left;

	private void Awake()
	{
		RegisterEventHandler();
	}

	private IEnumerator Start()
	{
		m_LogoAnimation.gameObject.SetActive(false);
		m_PlayButtonAnimation.gameObject.SetActive(false);
		m_OptionsPanelAnimation.gameObject.SetActive(false);
		m_MiscPanelAnimation.gameObject.SetActive(false);
		while (Application.isLoadingLevel || DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.LoadingScreen.m_Animation.isPlaying)
		{
			yield return new WaitForEndOfFrame();
		}
		Enter();
	}

	private void Update()
	{
		if (!m_Left && !Application.isLoadingLevel) // && Input.GetKeyDown(KeyCode.Escape)
		{
			HandleBackButton();
		}
	}

	private void HandleBackButton()
	{
		DebugLog.Log("Pressed Back Button: " + GetType());
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("back_button_pressed", string.Empty);
		m_Left = true;
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		Application.Quit();
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		m_PlayButton.Clicked += m_PlayButton_Clicked;
		if ((bool)m_OptionsButton)
		{
			m_OptionsButton.Clicked += m_OptionsButton_Clicked;
		}
		m_MiscButton.Clicked += m_MiscButton_Clicked;
	}

	public void Enter()
	{
		base.gameObject.SetActive(true);
		StartCoroutine(EnterCoroutine());
	}

	private IEnumerator EnterCoroutine()
	{
		m_LogoAnimation.gameObject.SetActive(true);
		m_PlayButtonAnimation.gameObject.SetActive(true);
		m_OptionsPanelAnimation.gameObject.SetActive(true);
		m_MiscPanelAnimation.gameObject.SetActive(true);
		m_OptionsPanelAnimation.Play("OptionsPanel_Enter");
		m_PlayButtonAnimation.Play("PlayButton_Enter");
		m_LogoAnimation.Play("Logo_Enter");
		m_MiscPanelAnimation.Play("MiscPanel_Enter");
		yield break;
	}

	public void Leave()
	{
		StartCoroutine(LeaveCoroutine(null));
	}

	public void Leave(Action actionAfterLeave)
	{
		StartCoroutine(LeaveCoroutine(actionAfterLeave));
	}

	private IEnumerator LeaveCoroutine(Action actionAfterLeave)
	{
		m_LogoAnimation.Play("Logo_Leave");
		m_PlayButtonAnimation.Play("PlayButton_Leave");
		m_OptionsPanelAnimation.Play("OptionsPanel_Leave");
		m_MiscPanelAnimation.Play("MiscPanel_Leave");
		yield return new WaitForSeconds(m_LogoAnimation["Logo_Leave"].clip.length);
		if (actionAfterLeave != null)
		{
			actionAfterLeave();
		}
	}

	private void m_MiscButton_Clicked()
	{
		if (m_bMiscOpen)
		{
			m_bMiscOpen = false;
			m_MiscMgr.GetComponent<Animation>().Play("Panel_Close");
			m_MiscButtonMgr.GetComponent<Animation>().Play("Released_Close");
		}
		else
		{
			m_bMiscOpen = true;
			m_MiscMgr.GetComponent<Animation>().Play("Panel_Open");
			m_MiscButtonMgr.GetComponent<Animation>().Play("Released_Open");
		}
	}

	private void m_OptionsButton_Clicked()
	{
		if (m_bOptionsOpen)
		{
			m_bOptionsOpen = false;
			m_OptionsMgr.GetComponent<Animation>().Play("Panel_Close");
			m_OptionsButtonMgr.GetComponent<Animation>().Play("Released_Close");
		}
		else
		{
			m_bOptionsOpen = true;
			m_OptionsMgr.GetComponent<Animation>().Play("Panel_Open");
			m_OptionsButtonMgr.GetComponent<Animation>().Play("Released_Open");
		}
	}

	private void m_PlayButton_Clicked()
	{
		if (DIContainerInfrastructure.GetCoreStateMgr().m_returnFromMainMenuAction != null)
		{
			Leave(delegate
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_returnFromMainMenuAction();
			});
		}
		else if (DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.CurrentHotspotGameData.Data.UnlockState != HotspotUnlockState.Resolved && DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.CurrentHotspotGameData.Data.NameId == "hotspot_001_camp")
		{
			Leave(delegate
			{
				DIContainerInfrastructure.GetCoreStateMgr().GotoIntro();
			});
		}
		else
		{
			Leave(delegate
			{
				DIContainerInfrastructure.GetCoreStateMgr().GotoWorldMap();
			});
		}
	}

	private void DeRegisterEventHandler()
	{
		if ((bool)m_PlayButton)
		{
			m_PlayButton.Clicked -= m_PlayButton_Clicked;
		}
		if ((bool)m_OptionsButton)
		{
			m_OptionsButton.Clicked -= m_OptionsButton_Clicked;
		}
		if ((bool)m_MiscButton)
		{
			m_MiscButton.Clicked -= m_MiscButton_Clicked;
		}
	}

	private void OnDestroy()
	{
		DeRegisterEventHandler();
	}
}

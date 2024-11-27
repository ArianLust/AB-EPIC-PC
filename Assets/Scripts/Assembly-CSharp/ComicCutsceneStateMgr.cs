using System;
using System.Collections;
using ABH.GameDatas;
using Prime31;
using UnityEngine;

public class ComicCutsceneStateMgr : MonoBehaviour
{
	[SerializeField]
	public Animation m_IntroAnimation;

	[SerializeField]
	public BoxCollider m_PlayButton;

	private GameObject m_HandRoot;

	private IInventoryItemGameData m_storySequenceItem;

	private char m_currentPartChar;

	private bool m_Left;

	private void Awake()
	{
		DIContainerInfrastructure.BackButtonMgr.Reset();
		m_PlayButton.enabled = false;
		if (ContentLoader.Instance != null)
		{
			ContentLoader.Instance.SetDownloadProgress(1f);
		}
	}

	private void SearchForHand()
	{
		m_HandRoot = base.transform.parent.Find("Guide_Tap_BR").gameObject;
	}

	private void HandleBackButton()
	{
		DebugLog.Log("Pressed Back Button: " + GetType());
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("back_button_pressed", string.Empty);
		m_Left = true;
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		DIContainerInfrastructure.GetCoreStateMgr().ShowConfirmationPopup(DIContainerInfrastructure.GetLocaService().Tr("gen_desc_leaveapp", "Do you really want to exit?"), delegate
		{
			Application.Quit();
		}, delegate
		{
		});
	}

	private void EtceteraAndroidManager_alertButtonClickedEvent(string text)
	{
		m_Left = false;
		EtceteraAndroidManager.alertButtonClickedEvent -= EtceteraAndroidManager_alertButtonClickedEvent;
		if (text == DIContainerInfrastructure.GetLocaService().Tr("gen_btn_yes", "Yes"))
		{
			Application.Quit();
		}
	}

	private IEnumerator Start()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 0u
		}, true);
		SearchForHand();
		m_HandRoot.SetActive(false);
		yield return new WaitForSeconds(DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.LoadingScreen.HideLength());
		Enter();
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		m_PlayButton.enabled = true;
	}

	public void Enter()
	{
		base.gameObject.SetActive(true);
		StartCoroutine(EnterCoroutine());
	}

	private IEnumerator EnterCoroutine()
	{
		m_IntroAnimation.gameObject.SetActive(true);
		m_currentPartChar = 'A';
		m_IntroAnimation.Play();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(0, HandleBackButton);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(0u);
		RegisterEventHandler();
		yield return new WaitForSeconds(m_IntroAnimation.clip.length);
		m_HandRoot.SetActive(true);
	}

	public void Leave()
	{
		StartCoroutine(LeaveCoroutine(null));
	}

	public void Leave(Action actionAfterLeave)
	{
		DeRegisterEventHandler();
		StartCoroutine(LeaveCoroutine(actionAfterLeave));
	}

	private IEnumerator LeaveCoroutine(Action actionAfterLeave)
	{
		if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, DIContainerInfrastructure.GetCoreStateMgr().m_CurrentPendingStorySequence, out m_storySequenceItem))
		{
			m_storySequenceItem.ItemData.IsNew = false;
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_CurrentPendingStorySequence = null;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(0);
		if (actionAfterLeave != null)
		{
			actionAfterLeave();
		}
		yield break;
	}

	private void OnTouchClicked()
	{
		HandleClicked();
	}

	private void HandleClicked()
	{
		string text = m_IntroAnimation.clip.name;
		string[] array = text.Split('_');
		if (array.Length != 3)
		{
			DebugLog.Error("this Animation has a wrong name: " + text);
			return;
		}
		string text2 = array[0] + "_" + array[1] + "_";
		string text3 = array[2];
		char currentPartChar = m_currentPartChar;
		currentPartChar = (char)(currentPartChar + 1);
		if (!m_IntroAnimation.isPlaying)
		{
			if (m_IntroAnimation.GetClip(text2 + currentPartChar) == null)
			{
				Leave(delegate
				{
					DIContainerInfrastructure.GetCoreStateMgr().GotoWorldMap();
				});
				return;
			}
			m_IntroAnimation.Play(text2 + currentPartChar);
			m_HandRoot.SetActive(false);
			Invoke("ActivateHand", m_IntroAnimation[text2 + currentPartChar].length);
			m_currentPartChar = currentPartChar;
		}
		else
		{
			m_IntroAnimation.Play(text2 + "Set" + m_currentPartChar);
			m_HandRoot.SetActive(true);
		}
	}

	private void ActivateHand()
	{
		m_HandRoot.SetActive(true);
	}

	private void DeRegisterEventHandler()
	{
		if ((bool)m_PlayButton)
		{
			m_PlayButton.enabled = false;
		}
	}

	private void OnDestroy()
	{
		DeRegisterEventHandler();
	}
}

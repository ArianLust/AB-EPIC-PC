using System;
using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas.Battle;
using ABH.Services.Logic;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using UnityEngine;

public class BattlePausedPopup : MonoBehaviour
{
	private BattleMgrBase m_BattleMgr;

	private bool m_isInitialized;

	private bool m_entered;

	private bool m_IsLeaving;

	private bool m_Entering;

	[SerializeField]
	public ContainerControl m_AdContainer;

	[SerializeField]
	private GameObject m_AdBackground;

	[SerializeField]
	public string m_SoundOnSprite;

	[SerializeField]
	public string m_SoundOffSprite;

	[SerializeField]
	public UISprite m_SoundButtonIcon;

	[SerializeField]
	public UIInputTrigger m_SoundButton;

	[SerializeField]
	public UIInputTrigger m_RetryButton;

	[SerializeField]
	public AdCanvas m_AdCanvas;

	[SerializeField]
	public UIInputTrigger m_WorldMapButton;

	[SerializeField]
	private UISprite m_WorldMapButtonSprite;

	[SerializeField]
	public UIInputTrigger m_ContinueButton;

	[SerializeField]
	public GameObject m_SoundtrackRoot;

	[SerializeField]
	public UIInputTrigger m_SoundtrackButton;

	private bool m_SoundOn;

	private bool m_MusicOn;

	private Vector3 m_topleftPixel;

	private Vector3 m_lowerRightPixel;

	private Vector3 m_sizePixel;

	private bool m_AlertShown;

	private string m_PauseAdPlacementId = "NewsFeed.pause";

	public bool IsVisible
	{
		get
		{
			return m_entered || m_Entering || m_IsLeaving;
		}
	}

	public void SetBattleMgr(BattleMgrBase battleMgr)
	{
		m_BattleMgr = battleMgr;
		base.gameObject.SetActive(false);
		UIPanel[] components = base.gameObject.GetComponents<UIPanel>();
		foreach (UIPanel uIPanel in components)
		{
			uIPanel.enabled = true;
		}
		m_isInitialized = true;
		DIContainerInfrastructure.GetCoreStateMgr().m_GameIsPaused = false;
		if (DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "unlock_replay") > 0 && DIContainerLogic.GetBattleService().IsReplayAllowed(m_BattleMgr.Model))
		{
			m_RetryButton.gameObject.SetActive(true);
		}
		else
		{
			m_RetryButton.gameObject.SetActive(false);
		}
		m_topleftPixel = DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera.WorldToScreenPoint(m_AdContainer.transform.position - new Vector3(m_AdContainer.m_Size.x / 2f, m_AdContainer.m_Size.y / 2f, 0f));
		m_lowerRightPixel = DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera.WorldToScreenPoint(m_AdContainer.transform.position + new Vector3(m_AdContainer.m_Size.x / 2f, m_AdContainer.m_Size.y / 2f, 0f));
		Vector3 vector = m_lowerRightPixel - m_topleftPixel;
		float x = m_topleftPixel.x / (float)Screen.width;
		float y = ((float)Screen.height - m_lowerRightPixel.y) / (float)Screen.height;
		float width = vector.x / (float)Screen.width;
		float height = vector.y / (float)Screen.height;
		m_sizePixel = m_lowerRightPixel - m_topleftPixel;
		DIContainerInfrastructure.AdService.AddPlacement(m_PauseAdPlacementId, x, y, width, height);
	}

	private void SoundTrackButtonClicked()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Button", "SoundtrackButton");
		dictionary.Add("Destination", "SoundtrackShop");
		dictionary.Add("URL", DIContainerConfig.GetClientConfig().SoundtrackURL);
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("LinkClicked", dictionary);
		Application.OpenURL(DIContainerConfig.GetClientConfig().SoundtrackURL);
	}

	private void m_SoundButton_Clicked()
	{
		SwitchSound();
	}

	private void SwitchSound()
	{
		DebugLog.Log("DIContainerInfrastructure.AudioManager.IsMuted(0): " + DIContainerInfrastructure.AudioManager.IsMuted(0));
		DebugLog.Log("DIContainerInfrastructure.AudioManager.IsMuted(1): " + DIContainerInfrastructure.AudioManager.IsMuted(1));
		DIContainerInfrastructure.GetCurrentPlayer().Data.IsMusicMuted = !DIContainerInfrastructure.GetCurrentPlayer().Data.IsMusicMuted;
		DIContainerInfrastructure.GetCurrentPlayer().Data.IsSoundMuted = DIContainerInfrastructure.GetCurrentPlayer().Data.IsMusicMuted;
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.IsMusicMuted)
		{
			DIContainerInfrastructure.AudioManager.AddMuteReason(0, "Data.IsMusicMuted");
		}
		else
		{
			DIContainerInfrastructure.AudioManager.RemoveMuteReason(0, "Data.IsMusicMuted");
		}
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.IsSoundMuted)
		{
			DIContainerInfrastructure.AudioManager.AddMuteReason(1, "Data.IsSoundMuted");
		}
		else
		{
			DIContainerInfrastructure.AudioManager.RemoveMuteReason(1, "Data.IsSoundMuted");
		}
		DebugLog.Log("DIContainerInfrastructure.AudioManager.IsMuted(0) afterwards: " + DIContainerInfrastructure.AudioManager.IsMuted(0));
		DebugLog.Log("DIContainerInfrastructure.AudioManager.IsMuted(1) afterwards: " + DIContainerInfrastructure.AudioManager.IsMuted(1));
		DIContainerInfrastructure.GetPlayerPrefsService().SetInt("audio_mute", DIContainerInfrastructure.GetCurrentPlayer().Data.IsSoundMuted ? 1 : 0);
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.IsMusicMuted)
		{
			m_SoundButtonIcon.spriteName = m_SoundOffSprite;
			m_SoundOn = false;
			m_MusicOn = false;
		}
		else
		{
			m_SoundButtonIcon.spriteName = m_SoundOnSprite;
			m_SoundOn = true;
			m_MusicOn = true;
			DIContainerInfrastructure.AudioManager.PlaySound("UI_Gen_Button_Released");
		}
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		if ((bool)m_SoundButton)
		{
			m_SoundButton.Clicked += m_SoundButton_Clicked;
			m_SoundButton.Pressed += PlayPressedSound;
		}
		if ((bool)m_SoundtrackButton)
		{
			m_SoundtrackButton.Clicked += SoundTrackButtonClicked;
			m_SoundtrackButton.Pressed += PlayPressedSound;
		}
		if ((bool)m_WorldMapButton)
		{
			m_WorldMapButton.Clicked += m_WorldMapButton_Clicked;
			m_WorldMapButton.Pressed += PlayPressedSound;
		}
		if ((bool)m_ContinueButton)
		{
			m_ContinueButton.Clicked += m_ContinueButton_Clicked;
			m_ContinueButton.Pressed += PlayPressedSound;
		}
		if ((bool)m_RetryButton)
		{
			m_RetryButton.Clicked += m_ReplayButton_Clicked;
			m_RetryButton.Pressed += PlayPressedSound;
		}
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(3, HandleBackButton);
		DIContainerInfrastructure.GetCoreStateMgr().OnPopupEnter += BattlePausedPopup_OnPopupEnter;
	}

	private void DeRegisterEventHandler()
	{
		if ((bool)m_SoundButton)
		{
			m_SoundButton.Clicked -= m_SoundButton_Clicked;
			m_SoundButton.Pressed -= PlayPressedSound;
		}
		if ((bool)m_SoundtrackButton)
		{
			m_SoundtrackButton.Clicked -= SoundTrackButtonClicked;
			m_SoundtrackButton.Pressed -= PlayPressedSound;
		}
		if ((bool)m_WorldMapButton)
		{
			m_WorldMapButton.Clicked -= m_WorldMapButton_Clicked;
			m_WorldMapButton.Pressed -= PlayPressedSound;
		}
		if ((bool)m_ContinueButton)
		{
			m_ContinueButton.Clicked -= m_ContinueButton_Clicked;
			m_ContinueButton.Pressed -= PlayPressedSound;
		}
		if ((bool)m_RetryButton)
		{
			m_RetryButton.Clicked -= m_ReplayButton_Clicked;
			m_RetryButton.Pressed -= PlayPressedSound;
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(3);
		DIContainerInfrastructure.GetCoreStateMgr().OnPopupEnter -= BattlePausedPopup_OnPopupEnter;
	}

	private void PlayPressedSound(bool pressed)
	{
		DIContainerInfrastructure.AudioManager.PlaySound((!pressed) ? "UI_Gen_Button_Released" : "UI_Gen_Button_Pressed");
	}

	private void BattlePausedPopup_OnPopupEnter(bool entered)
	{
		if (entered)
		{
			DIContainerInfrastructure.AdService.HideAd(m_PauseAdPlacementId);
		}
		else if (m_entered)
		{
			DIContainerInfrastructure.AdService.ShowAd(m_PauseAdPlacementId);
		}
	}

	private void HandleBackButton()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		Leave();
	}

	private void m_ContinueButton_Clicked()
	{
		DIContainerInfrastructure.AudioManager.PlaySound("ui_gen_buttonclick_01");
		Leave();
	}

	private void m_ReplayButton_Clicked()
	{
		if (DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "unlock_replay") > 0)
		{
			DIContainerInfrastructure.AudioManager.PlaySound("ui_gen_buttonclick_01");
			DeRegisterEventHandler();
			StartCoroutine(LeaveAndApplyActionCoroutine(RestartBattle));
		}
	}

	private void m_WorldMapButton_Clicked()
	{
		DIContainerInfrastructure.AudioManager.PlaySound("ui_gen_buttonclick_01");
		DeRegisterEventHandler();
		DIContainerInfrastructure.AdService.HideAd(m_PauseAdPlacementId);
		m_AdBackground.SetActive(false);
		DIContainerInfrastructure.GetCoreStateMgr().ShowConfirmationPopup(DIContainerInfrastructure.GetLocaService().Tr("gen_desc_leavebattle", "Do you really want to abort this Battle?"), delegate
		{
			StartCoroutine(LeaveAndApplyActionCoroutine(GoToWorldMap));
		}, delegate
		{
			RegisterEventHandler();
		});
	}

	private void OnApplicationPause(bool paused)
	{
		DebugLog.Log("Application paused: " + paused);
		if (m_AlertShown && !paused)
		{
			RegisterEventHandler();
			m_AlertShown = false;
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		DebugLog.Log("Application focused: " + focus);
		if (m_AlertShown && focus)
		{
			RegisterEventHandler();
			m_AlertShown = false;
		}
	}

	public void GoToWorldMap()
	{
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("battle_to_worldmap", string.Empty);
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("battle_leave", string.Empty);
		if (ClientInfo.CurrentBattleGameData != null)
		{
			BattleGameData currentBattleGameData = ClientInfo.CurrentBattleGameData;
			if (currentBattleGameData.IsPvP && currentBattleGameData.IsUnranked && !string.IsNullOrEmpty(currentBattleGameData.m_CurrentOpponentId))
			{
				MessageDataIncoming messageDataIncoming = new MessageDataIncoming();
				messageDataIncoming.Sender = DIContainerInfrastructure.GetCurrentPlayer().GetFriendData();
				messageDataIncoming.MessageType = MessageType.DefeatedFriendMessage;
				messageDataIncoming.SentAt = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
				MessageDataIncoming message = messageDataIncoming;
				DIContainerInfrastructure.MessagingService.SendMessages(message, new List<string> { currentBattleGameData.m_CurrentOpponentId });
			}
			if (currentBattleGameData.m_BattleEndData != null && currentBattleGameData.m_BattleEndData.m_WinnerFaction != Faction.None)
			{
				DIContainerLogic.GetBattleService().RewardBattleLoot(currentBattleGameData.m_BattleEndData, DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData);
			}
			DIContainerLogic.GetBattleService().RegisterBattleEnded(currentBattleGameData);
			if (currentBattleGameData.IsPvP && !currentBattleGameData.IsUnranked)
			{
				DIContainerLogic.GetPvpObjectivesService().BattleLost();
			}
			currentBattleGameData = null;
		}
		DIContainerLogic.GetBattleService().ReportBattleToAnalytics(m_BattleMgr.Model, BattleResultTypes.Aborted);
		DIContainerInfrastructure.GetCoreStateMgr().ReturnFromBattle();
	}

	public void RestartBattle()
	{
		if (ClientInfo.CurrentBattleGameData != null)
		{
			if (ClientInfo.CurrentBattleGameData.m_BattleEndData != null && ClientInfo.CurrentBattleGameData.m_BattleEndData.m_WinnerFaction != Faction.None)
			{
				DIContainerLogic.GetBattleService().RewardBattleLoot(ClientInfo.CurrentBattleGameData.m_BattleEndData, DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData);
				if (ClientInfo.CurrentBattleStartGameData != null)
				{
					ClientInfo.CurrentBattleStartGameData.m_BattleRandomSeed = 0;
				}
			}
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.IsInBattle = false;
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveAllBars();
			DIContainerLogic.GetBattleService().RegisterBattleEnded(ClientInfo.CurrentBattleGameData);
			ClientInfo.CurrentBattleGameData = null;
		}
		if (ClientInfo.CurrentBattleStartGameData != null)
		{
			ClientInfo.CurrentBattleStartGameData.m_InjectableParticipantTable = null;
		}
		DIContainerLogic.GetBattleService().ReportBattleToAnalytics(m_BattleMgr.Model, BattleResultTypes.Restarted);
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("battle_leave", string.Empty);
		CoreStateMgr.Instance.GotoBattle(m_BattleMgr.Model.m_BattleGroundName);
	}

	private void OnApplicationQuit()
	{
		BattleGameData currentBattleGameData = ClientInfo.CurrentBattleGameData;
		if (currentBattleGameData.IsPvP && !currentBattleGameData.IsUnranked)
		{
			DIContainerLogic.GetPvpObjectivesService().BattleLost();
		}
	}

	private void OnDisable()
	{
		DeRegisterEventHandler();
	}

	public void Enter()
	{
		if (!IsVisible && !DIContainerInfrastructure.GetCoreStateMgr().IsShopLoading() && !DIContainerInfrastructure.GetCoreStateMgr().IsShopOpen() && !DIContainerInfrastructure.GetCoreStateMgr().m_PopupRoot.entered && !m_BattleMgr.m_BattleMainLoopDone && m_BattleMgr.m_InterfaceCamera.gameObject.activeInHierarchy && !DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.IsLoading() && !Application.isLoadingLevel)
		{
			m_entered = true;
			base.gameObject.SetActive(true);
			if (m_BattleMgr.Model.IsPvP)
			{
				m_WorldMapButtonSprite.spriteName = "Arena";
				m_WorldMapButtonSprite.MakePixelPerfect();
			}
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.DisableBlocker();
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltipsInstant();
			if (!DIContainerInfrastructure.GetCoreStateMgr().m_PopupEntered && DIContainerInfrastructure.AdService.IsNewsFeedShowPossible(m_PauseAdPlacementId))
			{
				m_AdBackground.SetActive(true);
				DIContainerInfrastructure.AdService.ShowAd(m_PauseAdPlacementId);
			}
			else
			{
				m_AdBackground.SetActive(false);
			}
			DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("enter_pause_menu", string.Empty);
			DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("pause_animate");
			StartCoroutine(EnterCoroutine());
		}
	}

	public IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GameIsPaused = true;
		m_Entering = true;
		DIContainerInfrastructure.TimeScaleMgr.AddTimeScale("battle_pause", 0f);
		m_SoundButtonIcon.spriteName = (DIContainerInfrastructure.GetCurrentPlayer().Data.IsMusicMuted ? m_SoundOffSprite : m_SoundOnSprite);
		if (!string.IsNullOrEmpty(DIContainerConfig.GetClientConfig().SoundtrackURL))
		{
			m_SoundtrackRoot.GetComponent<Animation>().Play("SoundtrackLink_Enter");
		}
		GetComponent<Animation>().Play("Popup_BattlePaused_Enter");
		while (GetComponent<Animation>().IsPlaying("Popup_BattlePaused_Enter"))
		{
			yield return new WaitForEndOfFrame();
		}
		RegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("pause_animate");
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("battle_consumable_enter");
		m_Entering = false;
	}

	public void Leave()
	{
		StartCoroutine(LeaveAndApplyActionCoroutine(delegate
		{
		}));
	}

	private IEnumerator LeaveCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("pause_animate");
		DeRegisterEventHandler();
		if (!string.IsNullOrEmpty(DIContainerConfig.GetClientConfig().SoundtrackURL))
		{
			m_SoundtrackRoot.GetComponent<Animation>().Play("SoundtrackLink_Leave");
		}
		GetComponent<Animation>().Play("Popup_BattlePaused_Leave");
		DIContainerInfrastructure.AdService.HideAd(m_PauseAdPlacementId);
		m_AdBackground.SetActive(false);
		while (GetComponent<Animation>().IsPlaying("Popup_BattlePaused_Leave"))
		{
			yield return new WaitForEndOfFrame();
		}
		DIContainerInfrastructure.TimeScaleMgr.RemoveTimeScale("battle_pause");
		DIContainerInfrastructure.GetCoreStateMgr().m_GameIsPaused = false;
		m_entered = false;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("pause_animate");
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("leave_pause_menu", string.Empty);
	}

	private IEnumerator LeaveAndApplyActionCoroutine(Action action)
	{
		DebugLog.Log(GetType(), "LeaveAndApplyActionCoroutine: Waiting for animation...");
		yield return StartCoroutine(LeaveCoroutine());
		m_BattleMgr.DestroyActionTree();
		DebugLog.Log(GetType(), "LeaveAndApplyActionCoroutine: calling back...");
		action();
		base.gameObject.SetActive(false);
	}
}

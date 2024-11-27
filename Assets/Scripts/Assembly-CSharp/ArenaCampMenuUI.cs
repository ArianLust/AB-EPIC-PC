using System.Collections;
using ABH.GameDatas;
using UnityEngine;

public class ArenaCampMenuUI : MonoBehaviour
{
	[SerializeField]
	public UIInputTrigger WorldMapButton;

	[SerializeField]
	public GameObject m_RankedMatchLoadingSpinner;

	[SerializeField]
	public GameObject m_RankedMatchButtonLoadingBody;

	[SerializeField]
	public GameObject m_RankedMatchButtonStandardBody;

	[SerializeField]
	public Material m_DesaturatedMaterial;

	private Material m_saturatedMaterial;

	[SerializeField]
	public UIInputTrigger PvECampButton;

	[SerializeField]
	public UIInputTrigger StartPvpButton;

	[SerializeField]
	public Animation m_WorldMapButtonAnimation;

	[SerializeField]
	public Animation m_PvpButtonAnimation;

	[SerializeField]
	public PvpBannerUI m_PvpBanner;

	[SerializeField]
	public int m_MenuDepth;

	private ArenaCampStateMgr m_CampStateMgr;

	private bool m_IsLeaving;

	private IEnumerator Start()
	{
		GetComponent<UIPanel>().enabled = true;
		yield return StartCoroutine(EnterCoroutine());
	}

	public void Enter()
	{
		StartCoroutine(EnterCoroutine());
	}

	public void Leave()
	{
		StartCoroutine(LeaveCoroutine());
	}

	private IEnumerator EnterCoroutine()
	{
		m_WorldMapButtonAnimation.Play("Button_Medium_BL_Enter");
		m_PvpButtonAnimation.Play("Button_Medium_BR_Enter");
		if (!ClientInfo.IsFriend)
		{
			WorldMapButton.transform.Find("Animation/Icon").GetComponent<UISprite>().spriteName = "WorldMap";
			PvECampButton.gameObject.SetActive(true);
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.EnterLevelDisplay();
		}
		else
		{
			WorldMapButton.transform.Find("Animation/Icon").GetComponent<UISprite>().spriteName = "Back";
			PvECampButton.gameObject.SetActive(false);
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveLevelDisplay();
		}
		SetupRankedMatchButton();
		InvokeRepeating("CheckIfOpponentIsAvailable", 1f, 1f);
		m_PvpBanner.gameObject.SetActive(true);
		StartCoroutine(m_PvpBanner.EnterCoroutine(m_CampStateMgr));
		yield return new WaitForSeconds(m_WorldMapButtonAnimation["Button_Medium_BL_Enter"].length);
		RegisterEventHandlers();
	}

	private void CheckIfOpponentIsAvailable()
	{
		if (!DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(DIContainerInfrastructure.GetCurrentPlayer()) || DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData.CurrentSeasonTurn.CurrentPvPOpponent == null)
		{
			if (!m_RankedMatchLoadingSpinner.activeInHierarchy)
			{
				SetupRankedMatchButton();
			}
		}
		else if (m_RankedMatchLoadingSpinner.activeInHierarchy)
		{
			SetupRankedMatchButton();
		}
	}

	public void SetupRankedMatchButton()
	{
		if (!DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(DIContainerInfrastructure.GetCurrentPlayer()) || DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData.CurrentSeasonTurn.CurrentPvPOpponent == null)
		{
			m_RankedMatchButtonStandardBody.gameObject.SetActive(false);
			m_RankedMatchLoadingSpinner.gameObject.SetActive(true);
		}
		else
		{
			m_RankedMatchButtonStandardBody.gameObject.SetActive(true);
			m_RankedMatchLoadingSpinner.gameObject.SetActive(false);
		}
	}

	private IEnumerator LeaveCoroutine()
	{
		DeRegisterEventHandlers();
		m_WorldMapButtonAnimation.Play("Button_Medium_BL_Leave");
		m_PvpButtonAnimation.Play("Button_Medium_BR_Leave");
		if (!ClientInfo.IsFriend)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveLevelDisplay();
		}
		yield return new WaitForSeconds(m_WorldMapButtonAnimation["Button_Medium_BL_Leave"].length);
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(0, HandleBackButton);
		if ((bool)WorldMapButton)
		{
			WorldMapButton.Clicked += OnWorldMapButtonClicked;
		}
		if ((bool)StartPvpButton)
		{
			StartPvpButton.Clicked += StartRankedMatch;
		}
		if ((bool)PvECampButton)
		{
			PvECampButton.Clicked += OnCampButtonClicked;
		}
	}

	private void DeRegisterEventHandlers()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(0);
		if ((bool)WorldMapButton)
		{
			WorldMapButton.Clicked -= OnWorldMapButtonClicked;
		}
		if ((bool)StartPvpButton)
		{
			StartPvpButton.Clicked -= StartRankedMatch;
		}
		if ((bool)PvECampButton)
		{
			PvECampButton.Clicked -= OnCampButtonClicked;
		}
	}

	private void OnCampButtonClicked()
	{
		DeRegisterEventHandlers();
		StartCoroutine(LeaveCoroutine());
		DIContainerInfrastructure.GetCoreStateMgr().GotoCampScreen();
	}

	private void HandleBackButton()
	{
		DebugLog.Log("Pressed Back Button: " + GetType());
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("back_button_pressed", string.Empty);
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		m_IsLeaving = true;
		OnWorldMapButtonClicked();
	}

	private void OnMainMenuButtonClicked()
	{
		DeRegisterEventHandlers();
		StartCoroutine(LeaveCoroutine());
		DIContainerInfrastructure.GetCoreStateMgr().GoToMainMenu(DIContainerInfrastructure.GetCoreStateMgr().GotoCampScreen);
	}

	private void OnWorldMapButtonClicked()
	{
		DeRegisterEventHandlers();
		StartCoroutine(LeaveCoroutine());
		if (ClientInfo.IsFriend)
		{
			DIContainerInfrastructure.GetCoreStateMgr().GotoCampScreen();
		}
		else if (DIContainerInfrastructure.GetCoreStateMgr().m_ChronicleCave)
		{
			DIContainerInfrastructure.GetCoreStateMgr().GotoChronlicleCave();
		}
		else
		{
			DIContainerInfrastructure.GetCoreStateMgr().GotoWorldMap();
		}
	}

	private void OnDisable()
	{
		DeRegisterEventHandlers();
	}

	public void SetCampStateMgr(ArenaCampStateMgr mgr)
	{
		m_CampStateMgr = mgr;
		StartCoroutine(m_PvpBanner.EnterCoroutine(mgr));
	}

	private void StartRankedMatch()
	{
		PvPSeasonManagerGameData currentPvPSeasonGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData;
		if (DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(DIContainerInfrastructure.GetCurrentPlayer()) && DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData.CurrentSeasonTurn.CurrentPvPOpponent != null)
		{
			if (DIContainerLogic.PvPSeasonService.IsWaitingForConfirmation(currentPvPSeasonGameData) && currentPvPSeasonGameData.CurrentSeasonTurn.IsResultValid)
			{
				m_CampStateMgr.ShowPvPTurnResultScreen();
			}
			else if (DIContainerLogic.PvPSeasonService.IsPvPTurnRunning(currentPvPSeasonGameData))
			{
				m_CampStateMgr.StartRankedMatch();
			}
		}
	}
}

using System.Collections;
using UnityEngine;

public class CampMenuUI : MonoBehaviour
{
	public UIInputTrigger WorldMapButton;

	public UIInputTrigger FriendListButton;

	public UIInputTrigger PvPCampButton;

	public Animation m_WorldMapButtonAnimation;

	public int m_MenuDepth;

	private BaseCampStateMgr m_CampStateMgr;

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
		m_WorldMapButtonAnimation.Play("Button_Medium_BR_Enter");
		if (!ClientInfo.IsFriend)
		{
			WorldMapButton.transform.Find("Animation/Icon").GetComponent<UISprite>().spriteName = "WorldMap";
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.EnterLevelDisplay();
		}
		else
		{
			WorldMapButton.transform.Find("Animation/Icon").GetComponent<UISprite>().spriteName = "Back";
			if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset("GenericElements"))
			{
				GameObject atlasGob = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject("GenericElements") as GameObject;
				if (atlasGob != null)
				{
					WorldMapButton.transform.Find("Animation/Icon").GetComponent<UISprite>().atlas = atlasGob.GetComponent<UIAtlas>();
				}
			}
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveLevelDisplay();
		}
		PvPCampButton.gameObject.SetActive(DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(DIContainerInfrastructure.GetCurrentPlayer()) && !ClientInfo.IsFriend);
		yield return new WaitForSeconds(m_WorldMapButtonAnimation["Button_Medium_BR_Enter"].length);
		RegisterEventHandlers();
	}

	private IEnumerator LeaveCoroutine()
	{
		DeRegisterEventHandlers();
		m_WorldMapButtonAnimation.Play("Button_Medium_BR_Leave");
		if (!ClientInfo.IsFriend)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveLevelDisplay();
		}
		yield return new WaitForSeconds(m_WorldMapButtonAnimation["Button_Medium_BR_Leave"].length);
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(0, HandleBackButton);
		if ((bool)WorldMapButton)
		{
			WorldMapButton.Clicked += WorldMapButton_Clicked;
		}
		if ((bool)PvPCampButton)
		{
			PvPCampButton.Clicked += OnArenaButtonClicked;
		}
	}

	private void DeRegisterEventHandlers()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(0);
		if ((bool)WorldMapButton)
		{
			WorldMapButton.Clicked -= WorldMapButton_Clicked;
		}
		if ((bool)PvPCampButton)
		{
			PvPCampButton.Clicked -= OnArenaButtonClicked;
		}
	}

	private void HandleBackButton()
	{
		DebugLog.Log("Pressed Back Button: " + GetType());
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("back_button_pressed", string.Empty);
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		m_IsLeaving = true;
		WorldMapButton_Clicked();
	}

	private void MainMenuButton_Clicked()
	{
		DeRegisterEventHandlers();
		StartCoroutine(LeaveCoroutine());
		DIContainerInfrastructure.GetCoreStateMgr().GoToMainMenu(DIContainerInfrastructure.GetCoreStateMgr().GotoCampScreen);
	}

	private void WorldMapButton_Clicked()
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
		else if (DIContainerInfrastructure.GetCoreStateMgr().m_EventCampaign)
		{
			DIContainerInfrastructure.GetCoreStateMgr().GoToMiniCampaign();
		}
		else
		{
			DIContainerInfrastructure.GetCoreStateMgr().GotoWorldMap();
		}
	}

	private void OnArenaButtonClicked()
	{
		DeRegisterEventHandlers();
		StartCoroutine(LeaveCoroutine());
		DIContainerInfrastructure.GetCoreStateMgr().GotoPvpCampScreen();
	}

	private void OnDisable()
	{
		DeRegisterEventHandlers();
	}

	public void SetCampStateMgr(BaseCampStateMgr mgr)
	{
		m_CampStateMgr = mgr;
	}
}

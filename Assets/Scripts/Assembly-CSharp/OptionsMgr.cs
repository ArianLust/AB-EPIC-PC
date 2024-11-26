using System.Collections;
using UnityEngine;

public class OptionsMgr : MonoBehaviour
{
	public const string FACEBOOK_URL = "https://www.facebook.com/angrybirdsepic";

	[SerializeField]
	public CreditsManager m_CreditsMgr;

	[SerializeField]
	public string m_SoundOnSprite;

	[SerializeField]
	public string m_SoundOffSprite;

	[SerializeField]
	public UISprite m_SoundButtonIcon;

	[SerializeField]
	public UIInputTrigger m_OptionsButton;

	[SerializeField]
	public UIInputTrigger m_SoundButton;

	[SerializeField]
	public UIInputTrigger m_RestorePurchaseButton;

	[SerializeField]
	public UIInputTrigger m_InfoButton;

	[SerializeField]
	public UIInputTrigger m_ToonsButton;

	[SerializeField]
	public UIInputTrigger m_BonusCodeButton;

	[SerializeField]
	public UIInputTrigger m_FacebookButton;

	[SerializeField]
	public Animation m_PanelAnimation;

	[SerializeField]
	public Animation m_OptionsButtonAnimation;

	[SerializeField]
	public Animation m_OptionsPanelAnimation;

	[SerializeField]
	public GameObject m_Background;

	[SerializeField]
	public UIGrid m_ButtonGrid;

	private bool m_SoundOn;

	private bool m_MusicOn;

	public bool m_Open;

	private bool m_IsLoading;

	private bool m_CheckIsLoading;

	public AdCanvas m_AdCanvas;

	[SerializeField]
	private float m_RestorePurchaseTimeout = 8f;

	public bool IsAnimationRunning
	{
		get
		{
			return m_PanelAnimation.isPlaying;
		}
	}

	public void Enter(bool openOptions = false)
	{
		base.gameObject.SetActive(true);
		StartCoroutine(EnterCoroutine(openOptions));
	}

	public void Start()
	{
	}

	private IEnumerator EnterCoroutine(bool openOptions = false)
	{
		RegisterEventHandler();
		if ((bool)m_OptionsPanelAnimation)
		{
			m_OptionsPanelAnimation.Play("OptionsPanel_Enter");
		}
		m_SoundButtonIcon.spriteName = (DIContainerInfrastructure.GetCurrentPlayer().Data.IsMusicMuted ? m_SoundOffSprite : m_SoundOnSprite);
		if (!m_Open && openOptions)
		{
			OpenOptions();
		}
		yield return null;
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		if ((bool)m_SoundButton)
		{
			m_SoundButton.Clicked += m_SoundButton_Clicked;
		}
		if ((bool)m_OptionsButton)
		{
			m_OptionsButton.Clicked += m_OptionsButton_Clicked;
		}
		if ((bool)m_BonusCodeButton)
		{
			m_BonusCodeButton.Clicked += BonusCodeButtonClicked;
		}
		if ((bool)m_RestorePurchaseButton)
		{
			m_RestorePurchaseButton.Clicked -= m_RestorePurchaseButton_Clicked;
			m_RestorePurchaseButton.Clicked += m_RestorePurchaseButton_Clicked;
		}
		if ((bool)m_InfoButton)
		{
			m_InfoButton.Clicked += m_InfoButton_Clicked;
		}
		if ((bool)m_ToonsButton)
		{
			m_ToonsButton.Clicked += m_ToonsButton_Clicked;
		}
		if ((bool)m_FacebookButton)
		{
			m_FacebookButton.Clicked += FacebookButtonClicked;
		}
	}

	private void FacebookButtonClicked()
	{
		// Application.OpenURL("https://www.facebook.com/angrybirdsepic"); yeahhh.. nah
	}

	private void BonusCodeButtonClicked()
	{
		if (!DIContainerInfrastructure.LocationStateMgr || !DIContainerInfrastructure.LocationStateMgr.IsBirdWalking())
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_BonusCodeManager.Enter();
		}
	}

	private void m_SoundButton_Clicked()
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
		}
	}

	private void m_RestorePurchaseButton_Clicked()
	{
		if (!DIContainerInfrastructure.PurchasingService.IsInitializing() && DIContainerInfrastructure.PurchasingService.IsInitialized())
		{
			DIContainerInfrastructure.PurchasingService.RestorePurchaseCompletion -= PurchasingService_RestorePurchaseCompletion;
			DIContainerInfrastructure.PurchasingService.RestorePurchaseCompletion += PurchasingService_RestorePurchaseCompletion;
			DIContainerInfrastructure.GetAsynchStatusService().ShowLocalLoading(DIContainerInfrastructure.GetLocaService().Tr("loadingscreen_loading", "Loading..."), true);
			DIContainerInfrastructure.PurchasingService.RestorePurchases();
			CoreStateMgr coreStateMgr = DIContainerInfrastructure.GetCoreStateMgr();
			if ((bool)coreStateMgr)
			{
				coreStateMgr.StartCoroutine("RestoreTimeout");
			}
		}
	}

	private IEnumerator RestoreTimeout()
	{
		yield return new WaitForSeconds(m_RestorePurchaseTimeout);
		DIContainerInfrastructure.GetAsynchStatusService().HideLocalLoading();
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_restorepurchase_failed"), "restore_purchase", DispatchMessage.Status.Info);
	}

	private void PurchasingService_RestorePurchaseCompletion(string result)
	{
		CoreStateMgr coreStateMgr = DIContainerInfrastructure.GetCoreStateMgr();
		if ((bool)coreStateMgr)
		{
			coreStateMgr.StopCoroutine("RestoreTimeout");
		}
		DIContainerInfrastructure.GetAsynchStatusService().HideLocalLoading();
		DIContainerInfrastructure.PurchasingService.RestorePurchaseCompletion -= PurchasingService_RestorePurchaseCompletion;
		if (!string.IsNullOrEmpty(result))
		{
			DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_restorepurchase_succesfull"), "restore_purchase", DispatchMessage.Status.Info);
		}
		else
		{
			DIContainerInfrastructure.GetAsynchStatusService().ShowError(DIContainerInfrastructure.GetLocaService().Tr("toast_restorepurchase_failed"), "restore_purchase");
		}
	}

	private void m_OptionsButton_Clicked()
	{
		if (!m_Open)
		{
			OpenOptions();
		}
		else
		{
			CloseOptions();
		}
	}

	public void CloseOptions()
	{
		m_PanelAnimation.Play("Panel_Close");
		m_OptionsButtonAnimation.Play("Released_Close");
		m_Open = false;
	}

	private void OpenOptions()
	{
		m_PanelAnimation.Play("Panel_Open");
		m_OptionsButtonAnimation.Play("Released_Open");
		m_Open = true;
	}

	private void m_InfoButton_Clicked()
	{
		if (!DIContainerInfrastructure.LocationStateMgr || !DIContainerInfrastructure.LocationStateMgr.IsBirdWalking())
		{
			m_CreditsMgr.Enter();
			StartCoroutine(LeaveCoroutine());
		}
	}

	private void m_ToonsButton_Clicked()
	{
		if (!DIContainerInfrastructure.LocationStateMgr || !DIContainerInfrastructure.LocationStateMgr.IsBirdWalking())
		{
			ShowToons();
		}
	}

	private void ShowToons()
	{
		if (DIContainerLogic.InventoryService.CheckForItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "news_introduction"))
		{
			DIContainerInfrastructure.LocationStateMgr.ShowNewsUi(NewsUi.NewsUiState.Toons);
		}
		else
		{
			DIContainerInfrastructure.GetChannelService().DisplayToonsTv(string.Empty, string.Empty, string.Empty);
		}
		m_IsLoading = false;
		m_CheckIsLoading = true;
	}

	public void Leave()
	{
		m_CreditsMgr.Leave();
		StartCoroutine(LeaveCoroutine());
	}

	public float GetLeaveTime()
	{
		float num = 0f;
		if (m_Open)
		{
			num = m_PanelAnimation["Panel_Close"].length;
		}
		if ((bool)m_OptionsPanelAnimation && num < m_OptionsPanelAnimation["OptionsPanel_Leave"].length)
		{
			return m_OptionsPanelAnimation["OptionsPanel_Leave"].length;
		}
		return num;
	}

	private IEnumerator LeaveCoroutine()
	{
		float waitTime = 0f;
		DeRegisterEventHandler();
		if (m_Open)
		{
			CloseOptions();
			waitTime = m_PanelAnimation["Panel_Close"].length;
			yield return new WaitForSeconds(waitTime);
		}
		if ((bool)m_OptionsPanelAnimation)
		{
			m_OptionsPanelAnimation.Play("OptionsPanel_Leave");
			if (waitTime < m_OptionsPanelAnimation["OptionsPanel_Leave"].length)
			{
				yield return new WaitForSeconds(m_OptionsPanelAnimation["OptionsPanel_Leave"].length - waitTime);
			}
		}
		base.gameObject.SetActive(false);
	}

	private void DeRegisterEventHandler()
	{
		if ((bool)m_SoundButton)
		{
			m_SoundButton.Clicked -= m_SoundButton_Clicked;
		}
		if ((bool)m_OptionsButton)
		{
			m_OptionsButton.Clicked -= m_OptionsButton_Clicked;
		}
		if ((bool)m_BonusCodeButton)
		{
			m_BonusCodeButton.Clicked -= BonusCodeButtonClicked;
		}
		if ((bool)m_RestorePurchaseButton)
		{
			m_RestorePurchaseButton.Clicked -= m_RestorePurchaseButton_Clicked;
		}
		if ((bool)m_InfoButton)
		{
			m_InfoButton.Clicked -= m_InfoButton_Clicked;
		}
		if ((bool)m_ToonsButton)
		{
			m_ToonsButton.Clicked -= m_ToonsButton_Clicked;
		}
	}

	private void OnDestroy()
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.AdService.RemoveRenderer(m_AdCanvas.m_Placement);
	}
}

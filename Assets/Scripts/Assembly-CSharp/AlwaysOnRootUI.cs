using System.Collections;
using UnityEngine;

public class AlwaysOnRootUI : MonoBehaviour
{
	[HideInInspector]
	public bool entered;

	private float m_CachedTimeScale;

	[SerializeField]
	private string m_animationPrefix;

	[SerializeField]
	private UIInputTrigger m_ReconnectButton;

	private void Awake()
	{
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_AlwaysOnRoot = this;
		RegisterEventHandlers();
		base.gameObject.SetActive(false);
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		if ((bool)m_ReconnectButton)
		{
			m_ReconnectButton.Clicked += m_ReconnectButton_Clicked;
		}
	}

	private void DeRegisterEventHandlers()
	{
		if ((bool)m_ReconnectButton)
		{
			m_ReconnectButton.Clicked -= m_ReconnectButton_Clicked;
		}
	}

	private void m_ReconnectButton_Clicked()
	{
		if (ContentLoader.Instance != null)
		{
			StartCoroutine(ContentLoader.Instance.CheckConnectivity());
		}
	}

	public IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("no_connection");
		if ((bool)GetComponent<Animation>()[m_animationPrefix + "Enter"])
		{
			GetComponent<Animation>().Play(m_animationPrefix + "Enter");
			while (GetComponent<Animation>().IsPlaying(m_animationPrefix + "Enter"))
			{
				yield return new WaitForEndOfFrame();
			}
		}
	}

	public void Enter()
	{
		if (!entered && !Application.isLoadingLevel && DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera.gameObject.activeInHierarchy)
		{
			BattleMgr battleMgr = Object.FindObjectOfType<BattleMgr>();
			if (battleMgr != null)
			{
				battleMgr.AutoBattle = false;
			}
			entered = true;
			DIContainerInfrastructure.GetCoreStateMgr().RegisterPopupEntered(true);
			DebugLog.Log("Enter No Connection Root");
			SetDragControllerActive(false);
			base.gameObject.SetActive(true);
			StartCoroutine(EnterCoroutine());
		}
	}

	public void Leave()
	{
		if (entered)
		{
			entered = false;
			DIContainerInfrastructure.GetCoreStateMgr().RegisterPopupEntered(false);
			DebugLog.Log("Leave No Connection Root");
			SetDragControllerActive(true);
			if (this != null && base.gameObject != null && base.gameObject.activeInHierarchy)
			{
				StartCoroutine(LeaveCoroutine());
			}
		}
	}

	private void SetDragControllerActive(bool flag)
	{
		if (DIContainerInfrastructure.CurrentDragController != null)
		{
			DIContainerInfrastructure.CurrentDragController.SetActiveDepth(flag, 1);
		}
	}

	private IEnumerator LeaveCoroutine()
	{
		if ((bool)GetComponent<Animation>()[m_animationPrefix + "Leave"])
		{
			GetComponent<Animation>().Play(m_animationPrefix + "Leave");
			while (GetComponent<Animation>().IsPlaying(m_animationPrefix + "Leave"))
			{
				yield return new WaitForEndOfFrame();
			}
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("no_connection");
		base.gameObject.SetActive(false);
	}
}

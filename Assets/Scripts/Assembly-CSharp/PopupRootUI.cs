using System.Collections;
using UnityEngine;

public class PopupRootUI : MonoBehaviour
{
	public bool entered;

	public GameObject m_background;

	private void Awake()
	{
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_PopupRoot = this;
		base.gameObject.SetActive(false);
	}

	public void Enter(bool enterbackground = true)
	{
		if (!entered)
		{
			entered = true;
			SetDragControllerActive(false);
			base.gameObject.SetActive(true);
			m_background.SetActive(enterbackground);
			GetComponent<Animation>().Play("RootPopup_Enter");
		}
	}

	public void Leave()
	{
		if (entered)
		{
			entered = false;
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
		GetComponent<Animation>().Play("RootPopup_Leave");
		yield return new WaitForSeconds(GetComponent<Animation>()["RootPopup_Leave"].clip.length);
		base.gameObject.SetActive(false);
	}
}

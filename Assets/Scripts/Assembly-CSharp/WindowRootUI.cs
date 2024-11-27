using System.Collections;
using UnityEngine;

public class WindowRootUI : MonoBehaviour
{
	public bool entered;

	private void Awake()
	{
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot = this;
		base.gameObject.SetActive(false);
	}

	public void Enter(bool leaveBars = true)
	{
		if (!entered)
		{
			StopCoroutine("LeaveCoroutine");
			StopCoroutine("LeaveCoroutineForced");
			entered = true;
			SetDragControllerActive(false);
			base.gameObject.SetActive(true);
			GetComponent<Animation>().Play("RootWindow_Enter");
			if (leaveBars)
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
				{
					Depth = 1u
				}, true);
			}
		}
	}

	private void SetDragControllerActive(bool flag)
	{
		if (DIContainerInfrastructure.CurrentDragController != null)
		{
			DIContainerInfrastructure.CurrentDragController.SetActiveDepth(flag, 0);
		}
	}

	public void Leave(bool forced = false)
	{
		if (entered || forced)
		{
			entered = false;
			SetDragControllerActive(true);
			if (forced)
			{
			}
			if (this != null && base.gameObject != null && base.gameObject.activeInHierarchy)
			{
				StartCoroutine((!forced) ? "LeaveCoroutine" : "LeaveCoroutineForced");
			}
		}
	}

	private IEnumerator LeaveCoroutine()
	{
		GetComponent<Animation>().Play("RootWindow_Leave");
		yield return new WaitForSeconds(GetComponent<Animation>()["RootWindow_Leave"].clip.length);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(1u);
		base.gameObject.SetActive(false);
	}

	private IEnumerator LeaveCoroutineForced()
	{
		GetComponent<Animation>().Play("RootWindow_Leave");
		yield return new WaitForSeconds(GetComponent<Animation>()["RootWindow_Leave"].clip.length);
		base.gameObject.SetActive(false);
	}

	public float GetLeaveLength()
	{
		return GetComponent<Animation>()["RootWindow_Leave"].clip.length;
	}
}

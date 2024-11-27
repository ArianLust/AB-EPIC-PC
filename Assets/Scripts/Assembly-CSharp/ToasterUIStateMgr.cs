using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using UnityEngine;

public class ToasterUIStateMgr : MonoBehaviour
{
	public ToasterController m_LoadingToaster;

	public ToasterController m_InfoToaster;

	public ToasterController m_InfoIconToaster;

	public ToasterController m_InfoLootToaster;

	public ToasterController m_NonBlockingToaster;

	public GameObject m_InputBlockerScenery;

	public GameObject m_InputBlockerInterface;

	private void Awake()
	{
		if (DIContainerInfrastructure.GetCoreStateMgr() != null)
		{
			base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		}
		DIContainerInfrastructure.GetAsynchStatusService().SetToasterUIStateMgr(this);
	}

	public void ShowInfo(string message)
	{
		m_InfoToaster.SetMessage(message);
		StartCoroutine(ShowInfoCoroutine(m_InfoToaster));
	}

	public void ShowInfo(string message, string messageAssetId)
	{
		m_InfoIconToaster.SetMessage(message, messageAssetId);
		StartCoroutine(ShowInfoCoroutine(m_InfoIconToaster));
	}

	internal void ShowInfoWithLoot(string message, List<IInventoryItemGameData> items)
	{
		m_InfoLootToaster.SetMessage(message, items);
		StartCoroutine(ShowInfoCoroutine(m_InfoLootToaster));
	}

	public void ShowLoading(string message)
	{
		m_LoadingToaster.Enter();
		BlockInput(true);
		m_LoadingToaster.SetMessage(message);
	}

	public void BlockInput(bool block)
	{
		if (block)
		{
			DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("loading");
		}
		else
		{
			DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("loading");
		}
		SetDragControllerActive(!block);
		m_InputBlockerScenery.SetActive(block);
		m_InputBlockerInterface.SetActive(block);
	}

	public void HideLoading()
	{
		if (m_LoadingToaster.gameObject.activeSelf)
		{
			m_LoadingToaster.Leave();
		}
		else if (m_NonBlockingToaster.gameObject.activeSelf)
		{
			m_NonBlockingToaster.Leave();
		}
	}

	public void ShowLoadingNonBlocking()
	{
		m_NonBlockingToaster.Enter();
	}

	public void HideLoadingNonBlocking()
	{
		m_NonBlockingToaster.Leave();
	}

	private IEnumerator ShowInfoCoroutine(ToasterController toaster)
	{
		if (Time.timeScale <= 0f)
		{
			DIContainerInfrastructure.GetAsynchStatusService().RemoveLastMessageAndDisplayNext();
			yield break;
		}
		if (DIContainerInfrastructure.GetCoreStateMgr() != null && (bool)DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideMissingCurrencyOverlay();
		}
		yield return new WaitForSeconds(toaster.Enter());
		yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().ToasterTime);
		yield return new WaitForSeconds(toaster.Leave());
		DIContainerInfrastructure.GetAsynchStatusService().RemoveLastMessageAndDisplayNext();
	}

	private void SetDragControllerActive(bool flag)
	{
		if (DIContainerInfrastructure.CurrentDragController != null)
		{
			DIContainerInfrastructure.CurrentDragController.SetActiveDepth(flag, 2);
		}
	}

	public void HideAll()
	{
		if (m_LoadingToaster.gameObject.activeSelf)
		{
			m_LoadingToaster.Leave();
		}
		if (m_NonBlockingToaster.gameObject.activeSelf)
		{
			m_NonBlockingToaster.Leave();
		}
		if (m_InfoIconToaster.gameObject.activeSelf)
		{
			m_InfoIconToaster.Leave();
		}
		if (m_InfoLootToaster.gameObject.activeSelf)
		{
			m_InfoLootToaster.Leave();
		}
		if (m_InfoToaster.gameObject.activeSelf)
		{
			m_InfoToaster.Leave();
		}
	}
}

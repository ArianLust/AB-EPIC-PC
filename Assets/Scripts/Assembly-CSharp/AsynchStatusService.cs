using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using UnityEngine;

public class AsynchStatusService : MonoBehaviour
{
	private const int m_minimumDisplayLoadingSeconds = 3;

	private const int m_maximumWaitTimeLoadingModalDialogSeconds = 10;

	private Queue<DispatchMessage> m_dispatchMessages = new Queue<DispatchMessage>();

	private ToasterUIStateMgr m_toasterUIStateMgr;

	private float m_lastLoadingDisplay;

	public void SetToasterUIStateMgr(ToasterUIStateMgr toasterUIStatMgr)
	{
		m_toasterUIStateMgr = toasterUIStatMgr;
	}

	public void RemoveLastMessageAndDisplayNext()
	{
		DebugLog.Log("Removed Last Message");
		if (m_dispatchMessages.Count > 0)
		{
			m_dispatchMessages.Dequeue();
		}
		if (m_dispatchMessages.Count > 0)
		{
			Dispatch(m_dispatchMessages.Peek());
		}
	}

	public string ShowInfoAndLootItems(string message, List<IInventoryItemGameData> items, string compareId)
	{
		DebugLog.Log(message);
		DispatchMessage newMessage = new DispatchMessage();
		newMessage.m_DispatchStatus = DispatchMessage.Status.InfoAndLoot;
		newMessage.m_DispatchMessage = message;
		newMessage.m_DispatchItems = items;
		newMessage.m_CompareTag = compareId;
		if (m_dispatchMessages.Count((DispatchMessage m) => m.m_CompareTag == newMessage.m_CompareTag) > 0)
		{
			return null;
		}
		if (m_dispatchMessages.Count > 0)
		{
			m_dispatchMessages.Enqueue(newMessage);
		}
		else
		{
			m_dispatchMessages.Enqueue(newMessage);
			Dispatch(newMessage);
		}
		return newMessage.MessageId;
	}

	public string ShowInfoAndIcon(string message, string icon, string compareId)
	{
		DebugLog.Log(message);
		DispatchMessage newMessage = new DispatchMessage();
		newMessage.m_DispatchStatus = DispatchMessage.Status.InfoAndIcon;
		newMessage.m_DispatchMessage = message;
		newMessage.m_DispatchAsset = icon;
		newMessage.m_CompareTag = compareId;
		if (m_dispatchMessages.Count((DispatchMessage m) => m.m_CompareTag == newMessage.m_CompareTag) > 0)
		{
			return null;
		}
		if (m_dispatchMessages.Count > 0)
		{
			m_dispatchMessages.Enqueue(newMessage);
		}
		else
		{
			m_dispatchMessages.Enqueue(newMessage);
			Dispatch(newMessage);
		}
		return newMessage.MessageId;
	}

	public string ShowInfo(string message, string compareId, DispatchMessage.Status status)
	{
		DebugLog.Log(message);
		DispatchMessage newMessage = new DispatchMessage();
		newMessage.m_DispatchStatus = status;
		newMessage.m_DispatchMessage = message;
		newMessage.m_CompareTag = compareId;
		if (!string.IsNullOrEmpty(newMessage.m_CompareTag) && m_dispatchMessages.Count((DispatchMessage m) => m.m_CompareTag == newMessage.m_CompareTag) > 0)
		{
			return null;
		}
		if (m_dispatchMessages.Count > 0)
		{
			m_dispatchMessages.Enqueue(newMessage);
		}
		else
		{
			m_dispatchMessages.Enqueue(newMessage);
			Dispatch(newMessage);
		}
		return newMessage.MessageId;
	}

	public string ShowError(string message)
	{
		return ShowError(message, null);
	}

	public string ShowError(string message, string compareId)
	{
		DebugLog.Error(message);
		DispatchMessage newMessage = new DispatchMessage();
		newMessage.m_DispatchStatus = DispatchMessage.Status.Error;
		newMessage.m_DispatchMessage = message;
		newMessage.m_CompareTag = compareId;
		if (m_dispatchMessages.Count((DispatchMessage m) => m.m_CompareTag == newMessage.m_CompareTag) > 0)
		{
			return null;
		}
		if (m_dispatchMessages.Count > 0)
		{
			m_dispatchMessages.Enqueue(newMessage);
		}
		else
		{
			m_dispatchMessages.Enqueue(newMessage);
			Dispatch(newMessage);
		}
		return newMessage.MessageId;
	}

	public string ShowLocalLoading(string message, bool blocking)
	{
		DispatchMessage dispatchMessage = new DispatchMessage();
		if (blocking)
		{
			dispatchMessage.m_DispatchStatus = DispatchMessage.Status.LocalLoading;
		}
		else
		{
			dispatchMessage.m_DispatchStatus = DispatchMessage.Status.LocalLoadingNonBlocking;
		}
		dispatchMessage.m_DispatchMessage = message;
		m_lastLoadingDisplay = Time.realtimeSinceStartup;
		if (blocking)
		{
			InvokeRepeating("HandleLocalLoadingAfterTimeout", 3f, 3f);
		}
		Dispatch(dispatchMessage);
		return dispatchMessage.MessageId;
	}

	public void HideLocalLoading()
	{
		StartCoroutine(DoHideLocalLoading());
	}

	private IEnumerator HandleLocalLoadingAfterTimeout()
	{
		float delta = Time.realtimeSinceStartup - m_lastLoadingDisplay;
		if (delta < 10f)
		{
			yield return new WaitForSeconds(3f - delta);
		}
		HideLocalLoading();
		ShowError(DIContainerInfrastructure.GetLocaService().Tr("err_loading_too_long", "Loading took too long, cancelled..."), null);
	}

	private IEnumerator DoHideLocalLoading()
	{
		float delta = Time.realtimeSinceStartup - m_lastLoadingDisplay;
		m_toasterUIStateMgr.BlockInput(false);
		if (delta < 3f)
		{
			yield return new WaitForSeconds(3f - delta);
		}
		if (m_toasterUIStateMgr != null)
		{
			m_toasterUIStateMgr.HideLoading();
		}
		else
		{
			RemoveLastMessageAndDisplayNext();
		}
		yield return null;
	}

	private void Dispatch(DispatchMessage message)
	{
		switch (message.m_DispatchStatus)
		{
		case DispatchMessage.Status.Error:
			if (m_toasterUIStateMgr != null)
			{
				m_toasterUIStateMgr.ShowInfo("[c][FF0000]" + message.m_DispatchMessage + "[/c]");
			}
			else
			{
				RemoveLastMessageAndDisplayNext();
			}
			break;
		case DispatchMessage.Status.Warning:
			if (m_toasterUIStateMgr != null)
			{
				m_toasterUIStateMgr.ShowInfo("[c][00FF00]" + message.m_DispatchMessage + "[/c]");
			}
			else
			{
				RemoveLastMessageAndDisplayNext();
			}
			break;
		case DispatchMessage.Status.Exception:
			if (m_toasterUIStateMgr != null)
			{
				m_toasterUIStateMgr.ShowInfo("[c][FF0000]" + message.m_DispatchMessage + "[/c]");
			}
			else
			{
				RemoveLastMessageAndDisplayNext();
			}
			break;
		case DispatchMessage.Status.Info:
			if (m_toasterUIStateMgr != null)
			{
				m_toasterUIStateMgr.ShowInfo(message.m_DispatchMessage);
			}
			else
			{
				RemoveLastMessageAndDisplayNext();
			}
			break;
		case DispatchMessage.Status.InfoAndIcon:
			if (m_toasterUIStateMgr != null)
			{
				m_toasterUIStateMgr.ShowInfo(message.m_DispatchMessage, message.m_DispatchAsset);
			}
			else
			{
				RemoveLastMessageAndDisplayNext();
			}
			break;
		case DispatchMessage.Status.InfoAndLoot:
			if (m_toasterUIStateMgr != null)
			{
				m_toasterUIStateMgr.ShowInfoWithLoot(message.m_DispatchMessage, message.m_DispatchItems);
			}
			else
			{
				RemoveLastMessageAndDisplayNext();
			}
			break;
		case DispatchMessage.Status.LocalLoading:
			if (m_toasterUIStateMgr != null)
			{
				m_toasterUIStateMgr.ShowLoading(message.m_DispatchMessage);
			}
			else
			{
				RemoveLastMessageAndDisplayNext();
			}
			break;
		case DispatchMessage.Status.LocalLoadingNonBlocking:
			if (m_toasterUIStateMgr != null)
			{
				m_toasterUIStateMgr.ShowLoadingNonBlocking();
			}
			else
			{
				RemoveLastMessageAndDisplayNext();
			}
			break;
		default:
			RemoveLastMessageAndDisplayNext();
			break;
		}
	}

	public void ClearAfterSomeTime()
	{
		Invoke("HideAll", 5f);
	}

	private void HideAll()
	{
		m_toasterUIStateMgr.HideAll();
	}
}

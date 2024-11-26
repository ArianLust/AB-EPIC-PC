using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

public class EventPreviewContent : MonoBehaviour
{
	[SerializeField]
	private CharacterControllerCamp m_CharacterControllerPrefab;

	[SerializeField]
	private UILabel m_TimerLabel;

	[SerializeField]
	private Transform m_CharacterRoot;

	private EventManagerGameData m_model;

	private List<EventPreviewCharacterSlot> m_CharacterSlots = new List<EventPreviewCharacterSlot>();

	[SerializeField]
	private GameObject m_ClassRewardRoot;

	[SerializeField]
	private GameObject m_SkinRewardRoot;

	public void SetModel(EventManagerGameData eventManager)
	{
		m_model = eventManager;
		SetupTimer();
		m_CharacterSlots = m_CharacterRoot.GetComponentsInChildren<EventPreviewCharacterSlot>(true).ToList();
		foreach (EventPreviewCharacterSlot characterSlot in m_CharacterSlots)
		{
			CharacterControllerCamp model = UnityEngine.Object.Instantiate(m_CharacterControllerPrefab);
			characterSlot.SetModel(model);
		}
		SetEventRewardOption();
		DIContainerInfrastructure.GetCurrentPlayer().GlobalEventStateChanged -= GlobalEventStateChanged;
		DIContainerInfrastructure.GetCurrentPlayer().GlobalEventStateChanged += GlobalEventStateChanged;
	}

	private void SetEventRewardOption()
	{
		if (m_ClassRewardRoot == null || m_SkinRewardRoot == null)
		{
			DebugLog.Log(GetType(), "SetEventRewardOption: This Teaser doesn't have options!");
			return;
		}
		bool flag = DIContainerInfrastructure.EventSystemStateManager.UseCollectionFallbackReward(m_model);
		m_ClassRewardRoot.SetActive(!flag);
		m_SkinRewardRoot.SetActive(flag);
	}

	private void GlobalEventStateChanged(CurrentGlobalEventState arg1, CurrentGlobalEventState arg2)
	{
		SetupTimer();
	}

	public void Refresh()
	{
		if (m_model == null)
		{
			return;
		}
		SetupTimer();
		m_CharacterSlots = m_CharacterRoot.GetComponentsInChildren<EventPreviewCharacterSlot>(true).ToList();
		foreach (EventPreviewCharacterSlot characterSlot in m_CharacterSlots)
		{
			characterSlot.ReInitialize();
		}
	}

	private void SetupTimer()
	{
		if (m_TimerLabel == null)
		{
			return;
		}
		DebugLog.Log("Setting Preview Timer again");
		StopCoroutine("CountDownTimer");
		if (m_model != null)
		{
			switch (m_model.CurrentEventManagerState)
			{
			case EventManagerState.Teasing:
				StartCoroutine("CountDownTimer");
				break;
			case EventManagerState.Running:
				m_TimerLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_running", "Running!");
				break;
			case EventManagerState.Finished:
			case EventManagerState.FinishedWithoutPoints:
				m_TimerLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_finished", "Finished!");
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	private IEnumerator CountDownTimer()
	{
		DateTime trustedTime;
		while (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		DateTime targetTime = DIContainerLogic.EventSystemService.GetTeasingEndTime(m_model.Balancing);
		while (targetTime > trustedTime)
		{
			if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				TimeSpan timeLeft = targetTime - trustedTime;
				m_TimerLabel.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandardDown(DIContainerLogic.GetTimingService().TimeLeftUntil(targetTime));
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private void OnDestroy()
	{
		DIContainerInfrastructure.GetCurrentPlayer().GlobalEventStateChanged -= GlobalEventStateChanged;
	}
}

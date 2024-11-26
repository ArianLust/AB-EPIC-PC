using System;
using System.Collections.Generic;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;

public class NotificationPopupController
{
	public List<NotificationPopupTrigger> m_notificationRequestReasons;

	public NotificationPopupController()
	{
		m_notificationRequestReasons = new List<NotificationPopupTrigger>();
	}

	public void RequestNotificationPopupForReason(NotificationPopupTrigger reason)
	{
		DebugLog.Log(GetType(), "RequestNotificationPopupForReason: " + reason);
		m_notificationRequestReasons.Add(reason);
	}

	public bool IsPopupAvailable()
	{
		PlayerData data = DIContainerInfrastructure.GetCurrentPlayer().Data;
		if ((m_notificationRequestReasons != null && m_notificationRequestReasons.Count == 0) || m_notificationRequestReasons == null)
		{
			DebugLog.Log(GetType(), "IsPopUpAvailable: NOPE! No Reason to show a Notification App Topup!");
			return false;
		}
		if (data.NotificationUsageState == 2)
		{
			DebugLog.Log(GetType(), "IsPopUpAvailable: NOPE! Notifications are already allowed!");
			return false;
		}
		List<int> notificationPopupCooldowns = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").NotificationPopupCooldowns;
		int num = Math.Min(notificationPopupCooldowns.Count - 1, data.NotificationPopupsAmount);
		if (DIContainerLogic.GetTimingService().GetCurrentTimestamp() < data.NotificationPopupShown + notificationPopupCooldowns[num])
		{
			DebugLog.Log(GetType(), "IsPopUpAvailable: NOPE! Still on cooldown!");
			m_notificationRequestReasons.Clear();
			return false;
		}
		data.NotificationPopupsAmount = Math.Min(num + 1, notificationPopupCooldowns.Count);
		data.NotificationPopupShown = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
		return true;
	}
}

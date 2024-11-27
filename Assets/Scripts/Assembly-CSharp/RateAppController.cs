using System.Collections.Generic;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using Chimera.Library.Components.ClientLib.CrossPlatformLib.Source.Models;
using UnityEngine;

public class RateAppController
{
	public List<RatePopupTrigger> m_rateRequestReasons;

	public RateAppController()
	{
		m_rateRequestReasons = new List<RatePopupTrigger>();
	}

	public void SetRatedVersion()
	{
		DIContainerInfrastructure.GetCurrentPlayer().Data.LastRatingSuccessVersion = DIContainerInfrastructure.GetVersionService().StoreVersion;
	}

	public ChimeraVersionNumber GetRatedVersion()
	{
		return new ChimeraVersionNumber().FromString(DIContainerInfrastructure.GetCurrentPlayer().Data.LastRatingSuccessVersion);
	}

	public void RequestRatePopupForReason(RatePopupTrigger reason)
	{
		DebugLog.Log(GetType(), "RequestRatePopupForReason: " + reason);
		m_rateRequestReasons.Add(reason);
	}

	public bool IsPopupAvailable()
	{
		if ((m_rateRequestReasons != null && m_rateRequestReasons.Count == 0) || m_rateRequestReasons == null)
		{
			DebugLog.Log(GetType(), "IsPopUpAvailable: NOPE! No Reason to show a Rape App Topup!");
			return false;
		}
		if (!DIContainerLogic.InventoryService.CheckForItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "rate_app_01"))
		{
			DebugLog.Log(GetType(), "IsPopUpAvailable: NOPE! Rape Att nor yet unlockeded");
			m_rateRequestReasons.Clear();
			return false;
		}
		if (GetRatedVersion().Equals(DIContainerInfrastructure.GetVersionService().StoreVersion))
		{
			DebugLog.Log(GetType(), "IsPopUpAvailable: NOPE! Already rated this version! " + GetRatedVersion().ToString());
			m_rateRequestReasons.Clear();
			return false;
		}
		int rateAppAbortCooldown = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").RateAppAbortCooldown;
		if (DIContainerLogic.GetTimingService().GetCurrentTimestamp() < DIContainerInfrastructure.GetCurrentPlayer().Data.LastRatingFailTimestamp + rateAppAbortCooldown)
		{
			DebugLog.Log(GetType(), "IsPopUpAvailable: NOPE! Already clicked it away in the last hour!");
			m_rateRequestReasons.Clear();
			return false;
		}
		foreach (RatePopupTrigger rateRequestReason in m_rateRequestReasons)
		{
			Debug.Log("[RateAppController] IsPopupAvailable: reason for ratePopup = " + rateRequestReason);
		}
		return true;
	}

	public void InitiateFeedbackEmail()
	{
		DebugLog.Log(GetType(), "InitiateFeedbackEmail: Launching email client");
		string text = "bheisserer@chimera-entertainment.com";
		string text2 = WWW.EscapeURL("p to the is").Replace("+", "%20");
		string text3 = WWW.EscapeURL("My Body\r\nFull of non-escaped chars").Replace("+", "%20");
		Application.OpenURL("mailto:" + text + "?subject=" + text2 + "&body=" + text3);
	}
}

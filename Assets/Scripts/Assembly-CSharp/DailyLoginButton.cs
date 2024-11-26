using System;
using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using UnityEngine;

public class DailyLoginButton : MonoBehaviour
{
	[SerializeField]
	private UILabel m_TimeUntilTomorrowLabel;

	[SerializeField]
	private GameObject m_LabelHeader;

	[SerializeField]
	private GameObject m_ExplodeLootItems;

	[SerializeField]
	private UIInputTrigger m_claimGiftButton;

	[SerializeField]
	private UILabel m_NumberLabel;

	[SerializeField]
	private LootDisplayContoller m_lootDisplayController;

	private int m_giftNmr;

	private DailyLoginUI m_loginUi;

	private List<IInventoryItemGameData> m_lootItems;

	private Transform m_setChest;

	private string m_setChestIdent;

	public DailyLoginButtonState m_State;

	private void Awake()
	{
	}

	private void Start()
	{
		m_claimGiftButton.Clicked -= ClaimGift;
		m_claimGiftButton.Clicked += ClaimGift;
	}

	private void OnDestroy()
	{
		m_claimGiftButton.Clicked -= ClaimGift;
	}

	public void Init(DailyLoginUI dailyLoginUI, int day, List<IInventoryItemGameData> lootItems, Transform setChest, string setChestIdent)
	{
		m_loginUi = dailyLoginUI;
		m_giftNmr = day;
		m_lootItems = lootItems;
		m_setChest = setChest;
		m_setChestIdent = setChestIdent;
		UIPlayAnimation[] components = GetComponents<UIPlayAnimation>();
		foreach (UIPlayAnimation uIPlayAnimation in components)
		{
			bool flag = m_State == DailyLoginButtonState.CURRENT;
			uIPlayAnimation.enabled = flag;
		}
		m_NumberLabel.text = day.ToString();
		switch (m_State)
		{
		case DailyLoginButtonState.CURRENT:
			TimerToClaim();
			break;
		case DailyLoginButtonState.NEXT:
			InitTimer();
			break;
		case DailyLoginButtonState.OPEN:
			RemoveTimer();
			break;
		case DailyLoginButtonState.COMPLETED:
			break;
		}
	}

	public void ClaimFromVideo()
	{
		List<IInventoryItemGameData> wonItemFromChest = DIContainerLogic.DailyLoginLogic.ClaimGift(true);
		if (m_lootItems != null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().StartCoroutine(CreateChestLoot(wonItemFromChest));
			base.gameObject.PlayAnimationOrAnimatorState("Complete");
		}
		else
		{
			Invoke("AfterClaiming", base.gameObject.PlayAnimationOrAnimatorState("Complete"));
		}
	}

	public void ClaimGift()
	{
		if (!DIContainerLogic.DailyLoginLogic.m_ClaimedToday && DIContainerInfrastructure.GetCurrentPlayer().Data.GiftsClaimedThisMonth + 1 == m_giftNmr)
		{
			List<IInventoryItemGameData> wonItemFromChest = DIContainerLogic.DailyLoginLogic.ClaimGift();
			if (m_lootItems != null)
			{
				DIContainerInfrastructure.GetCoreStateMgr().StartCoroutine(CreateChestLoot(wonItemFromChest));
			}
			else
			{
				Invoke("AfterClaiming", base.gameObject.GetAnimationOrAnimatorStateLength("Complete"));
			}
			m_claimGiftButton.Clicked -= ClaimGift;
			UIPlayAnimation[] components = GetComponents<UIPlayAnimation>();
			foreach (UIPlayAnimation uIPlayAnimation in components)
			{
				uIPlayAnimation.enabled = false;
			}
			base.gameObject.PlayAnimationOrAnimatorState("Complete");
			m_loginUi.ClaimGift();
		}
	}

	private IEnumerator CreateChestLoot(List<IInventoryItemGameData> wonItemFromChest)
	{
		if (m_ExplodeLootItems == null)
		{
			yield break;
		}
		if (m_setChest != null)
		{
			m_setChest.GetComponent<Animation>().Play("SetItemChest_Open");
			m_setChest.GetComponent<Animation>().PlayQueued("SetItemChest_Idle_Inactive");
		}
		GameObject explodingLoot = UnityEngine.Object.Instantiate(m_ExplodeLootItems, base.transform.position, Quaternion.identity) as GameObject;
		explodingLoot.GetComponent<LootDisplayContoller>().SetModel(null, wonItemFromChest, LootDisplayType.Major);
		explodingLoot.transform.parent = base.transform;
		explodingLoot.transform.localPosition = new Vector3(0f, 0f, -200f);
		explodingLoot.SetActive(true);
		List<LootDisplayContoller> explodedLoot = explodingLoot.GetComponent<LootDisplayContoller>().Explode(true, false, 0.5f, false, 0f, 0f);
		UnityEngine.Object.Destroy(explodingLoot.gameObject);
		yield return new WaitForSeconds(3f);
		foreach (LootDisplayContoller explodedItem in explodedLoot)
		{
			if (explodedItem.gameObject.GetComponent<Animation>() != null)
			{
				explodedItem.gameObject.GetComponent<Animation>().Play("Display_Loot_TreasureChest_Hide");
			}
			UnityEngine.Object.Destroy(explodedItem.gameObject, 0.17f);
		}
		yield return new WaitForSeconds(0.17f);
		AfterClaiming();
	}

	private void AfterClaiming()
	{
		m_claimGiftButton.Clicked -= ClaimGift;
		m_claimGiftButton.Clicked += ClaimGift;
		m_loginUi.SetupGifts();
	}

	public void RemoveTimer()
	{
		if (m_LabelHeader != null)
		{
			m_LabelHeader.gameObject.SetActive(false);
		}
		StopCoroutine("CountDownTimer");
	}

	public void InitTimer()
	{
		if (m_LabelHeader != null)
		{
			m_LabelHeader.gameObject.SetActive(true);
		}
		StartCoroutine("CountDownTimer");
	}

	public void TimerToClaim()
	{
		if (m_LabelHeader != null)
		{
			m_LabelHeader.gameObject.SetActive(true);
		}
		if (m_TimeUntilTomorrowLabel != null)
		{
			m_TimeUntilTomorrowLabel.text = DIContainerInfrastructure.GetLocaService().Tr("daily_active_reward");
		}
		RemoveTimer();
	}

	private IEnumerator CountDownTimer()
	{
		DateTime trustedTime;
		while (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		DateTime nextDay = trustedTime.AddDays(1.0);
		DateTime targetTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 0, 0, 0);
		while (targetTime > trustedTime)
		{
			if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime) && m_TimeUntilTomorrowLabel != null)
			{
				m_TimeUntilTomorrowLabel.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(DIContainerLogic.GetTimingService().TimeLeftUntil(targetTime));
			}
			yield return new WaitForSeconds(1f);
		}
		m_loginUi.SetupGifts();
	}

	private void OnDisable()
	{
		RemoveTimer();
	}

	private void ShowTooltip()
	{
		if (m_setChest == null)
		{
			m_lootDisplayController.ShowTooltip();
			return;
		}
		string text = m_setChestIdent.Replace("bird_", string.Empty);
		string ident = "hotspot_tt_setitem_chest_" + text;
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowGenericOverlay(base.transform, DIContainerInfrastructure.GetLocaService().Tr(ident), true);
	}
}

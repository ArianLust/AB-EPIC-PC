using System;
using System.Collections;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using UnityEngine;

public class CoinBarController : MonoBehaviour
{
	private bool IsEntering;

	private bool HasLeft;

	private float coinValue;

	private float TimeTillLeave;

	[SerializeField]
	private bool hasMaximum;

	public string m_ItemName = "gold";

	private BattleUIStateMgr m_BattleUI;

	public Transform GoldCoinDisplay;

	public Transform m_CachedTransform;

	public UILabel m_TimerLabel;

	public GameObject m_TimerRoot;

	[SerializeField]
	private UILabel m_CoinLabel;

	[SerializeField]
	private ParticleSystem m_CoinFX;

	[SerializeField]
	private Animation m_UpdateAnimation;

	[SerializeField]
	private Animation m_LabelUpdate;

	[SerializeField]
	private UIInputTrigger m_ShopLinkButton;

	private float m_CurrentValue;

	private Action m_ReturnAction;

	private Action m_StartAction;

	private bool m_Leaving;

	private InventoryGameData m_Inventory
	{
		get
		{
			if (DIContainerInfrastructure.GetCurrentPlayer() == null)
			{
				return null;
			}
			return DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData;
		}
	}

	private void Awake()
	{
		m_CachedTransform = base.transform;
	}

	public CoinBarController SetInventory(InventoryGameData inventory)
	{
		coinValue = 1f;
		TimeTillLeave = DIContainerLogic.GetPacingBalancing().XpAndGoldBarStayDuration;
		m_CurrentValue = DIContainerLogic.InventoryService.GetItemValue(m_Inventory, m_ItemName);
		m_CoinLabel.text = m_CurrentValue.ToString("0");
		return this;
	}

	public CoinBarController SetReEnterAction(Action action)
	{
		m_ReturnAction = action;
		return this;
	}

	public CoinBarController SetEnterAction(Action action)
	{
		m_StartAction = action;
		return this;
	}

	public void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		if ((bool)m_ShopLinkButton)
		{
			m_ShopLinkButton.Clicked += m_ShopLinkButton_Clicked;
		}
	}

	public void DeRegisterEventHandlers()
	{
		if ((bool)m_ShopLinkButton)
		{
			m_ShopLinkButton.Clicked -= m_ShopLinkButton_Clicked;
		}
	}

	private void OnDestroy()
	{
		DeRegisterEventHandlers();
	}

	private void m_ShopLinkButton_Clicked()
	{
		SwitchToShop();
	}

	public void SwitchToShop(string enterSource = "Standard")
	{
		int startIndex = 0;
		string category = "shop_premium";
		if (m_ItemName == "gold")
		{
			startIndex = 5;
		}
		else if (m_ItemName == "lucky_coin")
		{
			startIndex = 0;
		}
		else if (m_ItemName == "friendship_essence")
		{
			startIndex = 6;
		}
		else if (m_ItemName == "event_energy")
		{
			startIndex = 0;
			category = "shop_global_consumables";
		}
		else if (m_ItemName == "shard")
		{
			startIndex = 0;
			category = "shop_global_specials";
		}
		DIContainerInfrastructure.GetCoreStateMgr().ShowShop(category, m_ReturnAction, startIndex, false, enterSource);
		m_ReturnAction = null;
		if (m_StartAction != null)
		{
			m_StartAction();
		}
	}

	public void EnterUpdateAndLeave()
	{
		StartCoroutine(EnterUpdateAndLeaveCoroutine());
	}

	private IEnumerator EnterUpdateAndLeaveCoroutine()
	{
		base.gameObject.SetActive(true);
		m_ShopLinkButton.gameObject.SetActive(false);
		GetComponent<Animation>().Play("Display_Top_Enter");
		yield return new WaitForSeconds(GetComponent<Animation>()["Display_Top_Enter"].length);
		yield return new WaitForSeconds(UpdateAnim());
		GetComponent<Animation>().Play("Display_Top_Leave");
	}

	public void StopBattleEntering()
	{
		if (this != null)
		{
			CancelInvoke();
		}
	}

	public void Enter(BattleUIStateMgr battleUI = null)
	{
		base.gameObject.SetActive(true);
		m_ShopLinkButton.gameObject.SetActive(false);
		m_BattleUI = battleUI;
		if ((bool)m_BattleUI)
		{
			StartCoroutine(m_BattleUI.LeavePauseOnly());
			StartCoroutine(m_BattleUI.LeaveAutoBattleOnly());
		}
		if (!IsEntering)
		{
			GetComponent<Animation>().Play("Display_Top_Enter");
			IsEntering = true;
			HasLeft = false;
		}
		CancelInvoke();
		Invoke("Leave", TimeTillLeave);
	}

	public float EnterAndStay(bool showShopLink, bool forced = false)
	{
		base.gameObject.SetActive(true);
		float result = 0f;
		if (!IsEntering || forced)
		{
			GetComponent<Animation>().Play("Display_Top_Enter");
			result = GetComponent<Animation>()["Display_Top_Enter"].length;
			IsEntering = true;
			HasLeft = false;
		}
		SetShopLink(showShopLink);
		return result;
	}

	public void SetShopLink(bool showShopLink)
	{
		m_ShopLinkButton.gameObject.SetActive(showShopLink);
	}

	public float GetEnterDuration()
	{
		return GetComponent<Animation>()["Display_Top_Enter"].length;
	}

	public float Leave()
	{
		m_LabelUpdate.Stop();
		GetComponent<Animation>().Play("Display_Top_Leave");
		Invoke("SetLeft", GetComponent<Animation>()["Display_Top_Leave"].length);
		StopCoroutine("UpdateCoinBarValue");
		m_BattleUI = null;
		return GetComponent<Animation>()["Display_Top_Leave"].length;
	}

	public float UpdateAnim(bool singleStep = false)
	{
		if ((float)DIContainerLogic.InventoryService.GetItemValue(m_Inventory, m_ItemName) == m_CurrentValue)
		{
			return 0f;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			UpdateValueOnly();
			return 0f;
		}
		m_UpdateAnimation.Play("Display_Update");
		m_CoinFX.Play();
		if (!singleStep)
		{
			StopCoroutine("UpdateCoinBarValue");
			StartCoroutine("UpdateCoinBarValue");
		}
		else
		{
			StopCoroutine("UpdateCoinBarValueSingleStep");
			StartCoroutine("UpdateCoinBarValueSingleStep");
		}
		return m_UpdateAnimation["Display_Update"].length;
	}

	private IEnumerator UpdateCoinBarValue()
	{
		m_CurrentValue = DIContainerLogic.InventoryService.GetItemValue(m_Inventory, m_ItemName);
		m_LabelUpdate.Play("Value_Counting");
		SetCoinLabelText();
		yield return new WaitForSeconds(m_LabelUpdate["Value_Counting"].length);
		m_LabelUpdate.Stop();
	}

	private IEnumerator UpdateCoinBarValueSingleStep()
	{
		m_CurrentValue += 1f;
		m_LabelUpdate.Play("Value_Counting");
		SetCoinLabelText();
		yield return new WaitForSeconds(m_LabelUpdate["Value_Counting"].length);
		m_LabelUpdate.Stop();
	}

	public void UpdateValueOnly()
	{
		m_CurrentValue = DIContainerLogic.InventoryService.GetItemValue(m_Inventory, m_ItemName);
		SetCoinLabelText();
	}

	private void SetCoinLabelText()
	{
		string text = string.Empty;
		if (hasMaximum && DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").ItemMaxCaps != null && DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").ItemMaxCaps.ContainsKey(m_ItemName))
		{
			int itemValue = DIContainerLogic.InventoryService.GetItemValue(m_Inventory, m_ItemName + "_cap_extension");
			text = "/" + DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").ItemMaxCaps[m_ItemName] + itemValue);
		}
		m_CoinLabel.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(Mathf.RoundToInt(m_CurrentValue)) + text;
	}

	public void SetLeft()
	{
		IsEntering = false;
		base.gameObject.SetActive(false);
	}

	public bool HasEntered()
	{
		return IsEntering;
	}

	public void AddOnlyUI(int amount)
	{
		m_CoinLabel.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(Mathf.RoundToInt(m_CurrentValue + (float)amount));
	}
}

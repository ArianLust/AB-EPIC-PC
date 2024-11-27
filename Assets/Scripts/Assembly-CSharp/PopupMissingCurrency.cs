using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using UnityEngine;

public class PopupMissingCurrency : MonoBehaviour
{
	[SerializeField]
	private UIInputTrigger m_shopButtonTrigger;

	[SerializeField]
	private UIInputTrigger m_closeTrigger;

	[SerializeField]
	private UISprite m_currencySprite;

	[SerializeField]
	private UILabel m_descriptionLabel;

	private string m_currencyIdent;

	private float m_currencyValue;

	private void Awake()
	{
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_MissingCurrencyPopup = this;
		base.gameObject.SetActive(false);
	}

	private void RegisterPopupTriggers()
	{
		DeregisterPopupTriggers();
		m_shopButtonTrigger.Clicked += OpenShop;
		m_closeTrigger.Clicked += Close;
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, Close);
	}

	private void DeregisterPopupTriggers()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(2);
		m_shopButtonTrigger.Clicked -= OpenShop;
		m_closeTrigger.Clicked -= Close;
	}

	private void OnDestroy()
	{
		DeregisterPopupTriggers();
	}

	public void EnterPopup(string missingCurrencyIdent, float missingCurrencyValue)
	{
		base.gameObject.SetActive(true);
		m_currencyIdent = missingCurrencyIdent;
		m_currencyValue = missingCurrencyValue;
		SetupIcon();
		SetupDescription();
		StartCoroutine(ShowPopup());
	}

	private void SetupDescription()
	{
		BasicItemGameData basicItemGameData = new BasicItemGameData(m_currencyIdent);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("value_01", m_currencyValue.ToString());
		dictionary.Add("value_02", basicItemGameData.ItemLocalizedName);
		Dictionary<string, string> replacementStrings = dictionary;
		m_descriptionLabel.text = DIContainerInfrastructure.GetLocaService().Tr("missingcurrency_desc", replacementStrings);
	}

	private void SetupIcon()
	{
		switch (m_currencyIdent)
		{
		case "gold":
			m_currencySprite.spriteName = "ShopOffer_Snoutlings_2";
			break;
		case "lucky_coin":
			m_currencySprite.spriteName = "ShopOffer_LuckyCoins_3";
			break;
		case "friendship_essence":
			m_currencySprite.spriteName = "ShopOffer_FriendshipEssence_2";
			break;
		}
		m_currencySprite.MakePixelPerfect();
	}

	private IEnumerator ShowPopup()
	{
		StopCoroutine("Deactivate");
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("animate_missingcurrency");
		base.gameObject.PlayAnimationOrAnimatorState("Popup_EnergyMissing_Enter");
		yield return new WaitForSeconds(0.75f);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("animate_missingcurrency");
		RegisterPopupTriggers();
	}

	private void Close()
	{
		StartCoroutine("Deactivate");
	}

	private IEnumerator Deactivate()
	{
		DeregisterPopupTriggers();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("animate_missingcurrency");
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_EnergyMissing_Leave"));
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("animate_missingcurrency");
		base.gameObject.SetActive(false);
	}

	private IEnumerator DeactivateAndGotoShop()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("animate_missingcurrency");
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_EnergyMissing_Leave"));
		int startIndex = 0;
		if (m_currencyIdent == "gold")
		{
			startIndex = 5;
		}
		else if (m_currencyIdent == "friendship_essence")
		{
			startIndex = 6;
		}
		DIContainerInfrastructure.GetCoreStateMgr().ShowShop("shop_premium", delegate
		{
		}, startIndex);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("animate_missingcurrency");
		base.gameObject.SetActive(false);
	}

	private void OpenShop()
	{
		DeregisterPopupTriggers();
		StartCoroutine("DeactivateAndGotoShop");
	}
}

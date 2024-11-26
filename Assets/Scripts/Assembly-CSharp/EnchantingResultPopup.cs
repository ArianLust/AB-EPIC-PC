using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

public class EnchantingResultPopup : MonoBehaviour
{
	[SerializeField]
	private GameObject m_stepOneObject;

	[SerializeField]
	private GameObject m_stepTwoObject;

	[SerializeField]
	private Animation m_firstAnimation;

	[SerializeField]
	private Transform m_slotEquipment;

	[SerializeField]
	private Transform m_slotEmblem;

	[SerializeField]
	private Transform m_slotFlag;

	[SerializeField]
	private Transform m_slotTip;

	[SerializeField]
	private UILabel m_enchantmenLabelOne;

	[SerializeField]
	private UISprite m_enchantmentSpriteOne;

	[SerializeField]
	private Animation m_secondAnimation;

	[SerializeField]
	private UILabel m_enchantmenLabelTwo;

	[SerializeField]
	private UISprite m_enchantmentSpriteTwo;

	[SerializeField]
	private LootDisplayContoller m_normalDisplayController;

	[SerializeField]
	private LootDisplayContoller m_setDisplayController;

	[SerializeField]
	private UIInputTrigger m_okButtonTrigger;

	[SerializeField]
	private UILabel m_totalStatsLabel;

	[SerializeField]
	private UILabel m_improvedLabel;

	[SerializeField]
	private UISprite m_mainIcon;

	[SerializeField]
	private GameObject m_skipCollider;

	private bool m_isSetItem;

	private IInventoryItemGameData m_enchantedItem;

	public void Enter(BannerItemGameData enchantedItem, int oldlevel)
	{
		base.gameObject.SetActive(true);
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("EnchantmentPopup");
		m_stepOneObject.SetActive(true);
		m_stepTwoObject.SetActive(true);
		SetResultStats(enchantedItem, oldlevel);
		m_enchantmentSpriteTwo.gameObject.SetActive(true);
		m_enchantmenLabelTwo.gameObject.SetActive(true);
		m_enchantmenLabelOne.text = oldlevel.ToString();
		m_enchantmenLabelTwo.text = enchantedItem.EnchantementLevel.ToString();
		if (enchantedItem.IsMaxEnchanted())
		{
			m_enchantmentSpriteTwo.spriteName = "Enchantment_Max";
		}
		else
		{
			m_enchantmentSpriteTwo.spriteName = "Enchantment";
		}
		m_enchantmentSpriteOne.spriteName = "Enchantment";
		GameObject gameObject;
		switch (enchantedItem.BalancingData.ItemType)
		{
		case InventoryItemType.Banner:
			gameObject = DIContainerInfrastructure.GetBannerAssetProvider().InstantiateObject(enchantedItem.ItemBalancing.AssetBaseId, m_slotFlag, Vector3.zero, Quaternion.identity);
			break;
		case InventoryItemType.BannerEmblem:
			gameObject = DIContainerInfrastructure.GetBannerAssetProvider().InstantiateObject(enchantedItem.ItemBalancing.AssetBaseId, m_slotEmblem, Vector3.zero, Quaternion.identity);
			break;
		case InventoryItemType.BannerTip:
			gameObject = DIContainerInfrastructure.GetBannerAssetProvider().InstantiateObject(enchantedItem.ItemBalancing.AssetBaseId, m_slotTip, Vector3.zero, Quaternion.identity);
			break;
		default:
			gameObject = DIContainerInfrastructure.GetEquipmentAssetProvider().InstantiateObject(enchantedItem.ItemAssetName, m_slotEquipment, Vector3.zero, Quaternion.identity, false);
			break;
		}
		BannerFlagAssetController component = gameObject.GetComponent<BannerFlagAssetController>();
		if ((bool)component)
		{
			gameObject.GetComponent<BannerFlagAssetController>().SetColors(component.GetColorFromList(enchantedItem.BalancingData.ColorVector));
		}
		gameObject.transform.localScale = Vector3.one;
		UnityHelper.SetLayerRecusively(gameObject, LayerMask.NameToLayer("Interface"));
		m_stepTwoObject.SetActive(false);
		m_enchantedItem = enchantedItem;
		m_isSetItem = enchantedItem.IsSetItem;
		StartCoroutine("ShowCoroutine");
	}

	public void Enter(EquipmentGameData enchantedItem, int oldlevel)
	{
		base.gameObject.SetActive(true);
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("EnchantmentPopup");
		m_stepOneObject.SetActive(true);
		m_stepTwoObject.SetActive(true);
		SetResultStats(enchantedItem, oldlevel);
		m_enchantmentSpriteTwo.gameObject.SetActive(true);
		m_enchantmenLabelTwo.gameObject.SetActive(true);
		m_enchantmenLabelOne.text = oldlevel.ToString();
		m_enchantmenLabelTwo.text = enchantedItem.EnchantementLevel.ToString();
		if (enchantedItem.IsMaxEnchanted())
		{
			m_enchantmentSpriteTwo.spriteName = "Enchantment_Max";
		}
		else
		{
			m_enchantmentSpriteTwo.spriteName = "Enchantment";
		}
		m_enchantmentSpriteOne.spriteName = "Enchantment";
		GameObject gameObject = DIContainerInfrastructure.GetEquipmentAssetProvider().InstantiateObject(enchantedItem.ItemAssetName, m_slotEquipment, Vector3.zero, Quaternion.identity, false);
		gameObject.transform.localScale = Vector3.one;
		m_stepTwoObject.SetActive(false);
		m_enchantedItem = enchantedItem;
		m_isSetItem = enchantedItem.IsSetItem;
		StartCoroutine("ShowCoroutine");
	}

	private IEnumerator ShowCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_PopupRoot.Enter();
		m_firstAnimation.Play("Enchantment_Step1");
		m_skipCollider.SetActive(true);
		m_skipCollider.GetComponent<UIInputTrigger>().Clicked -= Skip;
		m_skipCollider.GetComponent<UIInputTrigger>().Clicked += Skip;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("EnchantmentPopup");
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(5, Skip);
		yield return new WaitForSeconds(2.75f);
		DestroyOldAssets();
		m_stepOneObject.SetActive(false);
		m_stepTwoObject.SetActive(true);
		m_secondAnimation.Play("Enchantment_Step2_Enter");
		if (m_isSetItem)
		{
			m_normalDisplayController.gameObject.SetActive(false);
			m_setDisplayController.gameObject.SetActive(true);
			m_setDisplayController.SetModel(m_enchantedItem, new List<IInventoryItemGameData>(), LootDisplayType.Set);
			m_setDisplayController.PlayGainedAnimation();
		}
		else
		{
			m_normalDisplayController.gameObject.SetActive(true);
			m_setDisplayController.gameObject.SetActive(false);
			m_normalDisplayController.SetModel(m_enchantedItem, new List<IInventoryItemGameData>(), LootDisplayType.Minor);
			m_normalDisplayController.PlayGainedAnimation();
		}
		yield return new WaitForSeconds(2f);
		m_skipCollider.SetActive(false);
		m_okButtonTrigger.Clicked -= LeavePopup;
		m_okButtonTrigger.Clicked += LeavePopup;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(5);
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(5, LeavePopup);
	}

	private void DestroyOldAssets()
	{
		if (m_slotFlag.childCount > 0)
		{
			Object.Destroy(m_slotFlag.GetChild(0).gameObject);
		}
		if (m_slotEmblem.childCount > 0)
		{
			Object.Destroy(m_slotEmblem.GetChild(0).gameObject);
		}
		if (m_slotTip.childCount > 0)
		{
			Object.Destroy(m_slotTip.GetChild(0).gameObject);
		}
		if (m_slotEquipment.childCount > 0)
		{
			Object.Destroy(m_slotEquipment.GetChild(0).gameObject);
		}
	}

	private void LeavePopup()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(5);
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("EnchantmentLeave");
		m_okButtonTrigger.Clicked -= LeavePopup;
		if (m_normalDisplayController.gameObject.activeSelf)
		{
			m_normalDisplayController.PlayHideAnimation();
		}
		else if (m_setDisplayController.gameObject.activeSelf)
		{
			m_setDisplayController.PlayHideAnimation();
		}
		m_enchantmentSpriteTwo.gameObject.SetActive(false);
		m_enchantmenLabelTwo.gameObject.SetActive(false);
		StartCoroutine(LeaveCoroutine());
	}

	private IEnumerator LeaveCoroutine()
	{
		m_secondAnimation.Play("Enchantment_Step2_Leave");
		DIContainerInfrastructure.GetCoreStateMgr().m_PopupRoot.Leave();
		yield return new WaitForSeconds(0.25f);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("EnchantmentLeave");
		base.gameObject.SetActive(false);
	}

	private void SetResultStats(EquipmentGameData enchantedItem, int oldLevel)
	{
		if (enchantedItem.BalancingData.ItemType == InventoryItemType.MainHandEquipment)
		{
			m_mainIcon.spriteName = "Character_Damage_Large";
		}
		else
		{
			m_mainIcon.spriteName = "Character_Health_Large";
		}
		m_totalStatsLabel.text = enchantedItem.ItemMainStat.ToString("0");
		m_improvedLabel.text = (enchantedItem.ItemMainStat - enchantedItem.GetItemMainStatWithEnchantmentLevel(oldLevel)).ToString("0");
	}

	private void SetResultStats(BannerItemGameData enchantedItem, int oldlevel)
	{
		m_mainIcon.spriteName = "Character_Health_Large";
		m_totalStatsLabel.text = enchantedItem.ItemMainStat.ToString("0");
		m_improvedLabel.text = (enchantedItem.ItemMainStat - enchantedItem.GetItemMainStatWithEnchantmentLevel(oldlevel)).ToString("0");
	}

	private void Skip()
	{
		StopCoroutine("ShowCoroutine");
		m_skipCollider.GetComponent<UIInputTrigger>().Clicked -= Skip;
		m_skipCollider.SetActive(false);
		DestroyOldAssets();
		m_stepOneObject.SetActive(false);
		m_stepTwoObject.SetActive(true);
		if (m_isSetItem)
		{
			m_normalDisplayController.gameObject.SetActive(false);
			m_setDisplayController.gameObject.SetActive(true);
			m_setDisplayController.SetModel(m_enchantedItem, new List<IInventoryItemGameData>(), LootDisplayType.Set);
			m_setDisplayController.PlayGainedAnimation();
		}
		else
		{
			m_normalDisplayController.gameObject.SetActive(true);
			m_setDisplayController.gameObject.SetActive(false);
			m_normalDisplayController.SetModel(m_enchantedItem, new List<IInventoryItemGameData>(), LootDisplayType.Minor);
			m_normalDisplayController.PlayGainedAnimation();
		}
		m_secondAnimation.Play("Enchantment_Step2_EnterFast");
		m_okButtonTrigger.Clicked -= LeavePopup;
		m_okButtonTrigger.Clicked += LeavePopup;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(5);
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(5, LeavePopup);
	}

	private void OnDestroy()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(5);
		m_okButtonTrigger.Clicked -= LeavePopup;
	}
}

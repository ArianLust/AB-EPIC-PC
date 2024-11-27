using System.Collections;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using UnityEngine;

public class PopupMissingEnergy : MonoBehaviour
{
	[SerializeField]
	private UISprite m_OkIcon;

	[SerializeField]
	private UILabel m_Description;

	[SerializeField]
	private UILabel m_YouHaveLabel;

	[SerializeField]
	private UILabel m_Title;

	[SerializeField]
	private UIInputTrigger m_ButtonTrigger;

	[SerializeField]
	private UIInputTrigger m_CloseTrigger;

	private bool m_goToShop;

	private BattlePreperationUI m_preparationUi;

	private float m_energyRequirement;

	private IInventoryItemGameData m_energyItem;

	private IInventoryItemGameData m_energyPotionItem;

	private void Awake()
	{
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_MissingEnergyPopup = this;
		base.gameObject.SetActive(false);
	}

	private void RegisterPopupTriggers()
	{
		DeregisterPopupTriggers();
		m_ButtonTrigger.Clicked += OpenShop;
		m_CloseTrigger.Clicked += Close;
		if (m_energyItem == null)
		{
			if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "event_energy", out m_energyItem))
			{
				m_energyItem.ItemDataChanged += OnRefreshContent;
			}
		}
		else
		{
			m_energyItem.ItemDataChanged += OnRefreshContent;
		}
		if (m_energyPotionItem == null)
		{
			if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "potion_energy", out m_energyPotionItem))
			{
				m_energyPotionItem.ItemDataChanged += OnRefreshContent;
			}
		}
		else
		{
			m_energyPotionItem.ItemDataChanged += OnRefreshContent;
		}
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, Close);
	}

	private void OnRefreshContent(IInventoryItemGameData arg1, float arg2)
	{
		RefreshContent();
	}

	private void OnDestroy()
	{
		DeregisterPopupTriggers();
	}

	public IEnumerator ShowPopup(float energyRequirement, BattlePreperationUI ui)
	{
		StopCoroutine("Deactivate");
		m_energyRequirement = energyRequirement;
		m_preparationUi = ui;
		RefreshContent();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("animate_missingenergy");
		base.gameObject.PlayAnimationOrAnimatorState("Popup_EnergyMissing_Enter");
		yield return new WaitForSeconds(0.75f);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("animate_missingenergy");
		RegisterPopupTriggers();
	}

	private void RefreshContent()
	{
		int itemValue = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "event_energy");
		int itemValue2 = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "potion_energy");
		m_goToShop = (float)(itemValue + itemValue2) < m_energyRequirement;
		m_YouHaveLabel.text = DIContainerInfrastructure.GetLocaService().Tr("popup_stamina_missing_amount", "?You have {value_1}?").Replace("{value_1}", itemValue2.ToString("0"));
		if (!m_goToShop)
		{
			if ((float)itemValue >= m_energyRequirement)
			{
				m_OkIcon.spriteName = "Check_Small";
				m_Description.text = DIContainerInfrastructure.GetLocaService().Tr("popup_stamina_missing_enough_desc", "?You have enough energy to start the battle?");
				m_Title.text = DIContainerInfrastructure.GetLocaService().Tr("popup_stamina_missing_enough_header", "?Start Battle?");
			}
			else
			{
				m_OkIcon.spriteName = "Check_Small";
				m_Description.text = DIContainerInfrastructure.GetLocaService().Tr("popup_stamina_missing_use_desc", "?Use {value_1} energy drink(s) to start the battle?").Replace("{value_1}", (m_energyRequirement - (float)itemValue).ToString("0"));
				m_Title.text = DIContainerInfrastructure.GetLocaService().Tr("popup_stamina_missing_header", "?{value_1} Stamina missing").Replace("{value_1}", (m_energyRequirement - (float)itemValue).ToString("0"));
			}
		}
		else
		{
			m_OkIcon.spriteName = "Shop";
			m_Description.text = DIContainerInfrastructure.GetLocaService().Tr("popup_stamina_missing_shop_desc", "?Get more stamina from the shop?");
			m_Title.text = DIContainerInfrastructure.GetLocaService().Tr("popup_stamina_missing_header", "?{value_1} Stamina missing").Replace("{value_1}", (m_energyRequirement - (float)itemValue).ToString("0"));
		}
		m_OkIcon.MakePixelPerfect();
	}

	private void DeregisterPopupTriggers()
	{
		if (m_energyItem != null)
		{
			m_energyItem.ItemDataChanged -= OnRefreshContent;
		}
		if (m_energyPotionItem != null)
		{
			m_energyPotionItem.ItemDataChanged -= OnRefreshContent;
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(2);
		m_ButtonTrigger.Clicked -= OpenShop;
		m_CloseTrigger.Clicked -= Close;
	}

	private void Close()
	{
		StartCoroutine("Deactivate");
	}

	private void CloseAndRefresh()
	{
		m_preparationUi.RefreshEnergyValues();
		StartCoroutine("Deactivate");
	}

	private IEnumerator Deactivate()
	{
		DeregisterPopupTriggers();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("animate_missingenergy");
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_EnergyMissing_Leave"));
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("animate_missingenergy");
		base.gameObject.SetActive(false);
	}

	private void OpenShop()
	{
		DeregisterPopupTriggers();
		if (m_goToShop)
		{
			int startIndex = 0;
			string category = "shop_global_consumables";
			DIContainerInfrastructure.GetCoreStateMgr().ShowShop(category, CloseAndRefresh, startIndex);
			return;
		}
		for (int i = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "event_energy"); m_energyRequirement > (float)i; i++)
		{
			ConsumePotion();
		}
		CloseAndRefresh();
		m_preparationUi.PopupMissingEnergyUIStartHandler();
	}

	private void ConsumePotion()
	{
		IInventoryItemGameData data = null;
		if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "potion_energy", out data))
		{
			ConsumableItemGameData consumableItemGameData = data as ConsumableItemGameData;
			SkillBattleDataBase skillBattleDataBase = consumableItemGameData.ConsumableSkill.GenerateSkillBattleData();
			skillBattleDataBase.DoActionInstant(null, null, null);
			DIContainerLogic.InventoryService.RemoveItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "potion_energy", 1, "used_energy_potion");
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateEventEnergyBar();
			m_preparationUi.RefreshEnergyValues();
		}
	}
}

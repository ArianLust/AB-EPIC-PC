using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using UnityEngine;

public class BirdWindowUI : BirdWindowUIBase
{
	[SerializeField]
	private InventoryItemSlot ItemSlotPrefab;

	[SerializeField]
	private InventoryItemSlot ClassSlotPrefab;

	[SerializeField]
	private InventoryItemSlot EquipmentSlotPrefab;

	[SerializeField]
	private GameObject m_PerkRoot;

	[SerializeField]
	private GameObject m_PerkAndSetRoot;

	[SerializeField]
	private EquipmentItemInfo m_SetItemInfor;

	[SerializeField]
	private GameObject m_PerkRootEnch;

	[SerializeField]
	private GameObject m_PerkAndSetRootEnch;

	[SerializeField]
	private EquipmentItemInfo m_SetItemInforEnch;

	[SerializeField]
	private UILabel m_EquipmentInfoText;

	[SerializeField]
	public ClassItemInfo m_ClassInfo;

	[SerializeField]
	private BuyClassItemInfo m_PremiumClassBuyInfo;

	[SerializeField]
	private BirdEquipmentPreviewUI m_BirdEquipmentPreviewUI;

	[SerializeField]
	private UILabel m_HeaderLevelText;

	[SerializeField]
	private UILabel m_HeaderNameAndClassName;

	[SerializeField]
	private GameObject m_ExplodedFXPrefab;

	[SerializeField]
	private List<OpenInventoryButton> m_OpenInventoryButtons = new List<OpenInventoryButton>();

	private List<IInventoryItemGameData> m_GameDatas = new List<IInventoryItemGameData>();

	private List<BirdGameData> m_BirdsList = new List<BirdGameData>();

	private InventoryGameData m_Inventory;

	private BirdGameData m_SelectedBird;

	private InventoryItemType m_CurrentItemType = InventoryItemType.Class;

	private IInventoryItemGameData m_SelectedItem;

	private bool m_NewPremium;

	private bool m_AlreadyEquipped;

	private bool m_UpdateAnimBlocked;

	private bool m_SwitchBirdsBlocked;

	[SerializeField]
	private List<InventoryItemSlot> m_ItemSlots = new List<InventoryItemSlot>();

	[SerializeField]
	private UIGrid m_ItemGrid;

	[SerializeField]
	private UIScrollView m_ItemPanel;

	[SerializeField]
	private LootDisplayContoller m_LootForExplosionPrefab;

	[SerializeField]
	private Animation m_HeaderAnimation;

	[SerializeField]
	private Animation m_ItemCategoryButtonsAnimation;

	[SerializeField]
	private Animation m_ItemGridAnimation;

	[SerializeField]
	private Animation m_ItemInfoAnimation;

	private int m_UpperNodButtonIndex;

	[SerializeField]
	private GameObject m_ScrapItemRoot;

	[SerializeField]
	private UIInputTrigger m_ScrapItemButton;

	[SerializeField]
	private GameObject m_ScrapItemRootEnch;

	[SerializeField]
	private UIInputTrigger m_ScrapItemButtonEnch;

	[SerializeField]
	private EquipmentItemInfo m_EquipmentInfo;

	[SerializeField]
	private EquipmentItemInfo m_EnchantableEquipmentInfo;

	[SerializeField]
	private UILabel m_EnchantmentLabel;

	[SerializeField]
	private UISprite m_EnchantmentProgress;

	[SerializeField]
	private UISprite m_EnchantmentSprite;

	[SerializeField]
	private GameObject m_EnchantmentMaxLabel;

	[SerializeField]
	public UIInputTrigger m_EnchantItemButton;

	[SerializeField]
	private UISprite m_EnchantItemButtonSprite;

	[SerializeField]
	private SoundTriggerList m_SoundTriggers;

	[SerializeField]
	private UIInputTrigger m_ClassInfoButton;

	private bool m_IsRefreshing;

	private int m_SelectedBirdIndex;

	private bool m_Entered;

	private int m_ScrappingLocks;

	private bool m_finishedSpring;

	private BaseCampStateMgr m_StateMgr;

	private Color m_colorWhite = new Color(1f, 1f, 1f);

	private Color m_colorDarkGreen = new Color(0.5f, 1f, 0f);

	public BirdEquipmentPreviewUI BirdEquipmentPreview
	{
		get
		{
			return m_BirdEquipmentPreviewUI;
		}
	}

	private void Awake()
	{
		base.transform.position += DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.transform.position;
		UIPanel[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIPanel>();
		UIPanel[] array = componentsInChildren;
		foreach (UIPanel uIPanel in array)
		{
			uIPanel.enabled = false;
		}
	}

	public override BirdWindowUIBase SetStateMgr(BaseCampStateMgr stateMgr)
	{
		m_StateMgr = stateMgr;
		return this;
	}

	public override UIGrid getItemGrid()
	{
		return m_ItemGrid;
	}

	public override void SetModel(InventoryGameData inventory, List<BirdGameData> birds, int selectedIndex, InventoryItemType defaultItemType = InventoryItemType.Class)
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Enter();
		base.gameObject.SetActive(true);
		m_Inventory = inventory;
		if (m_BirdsList == null || m_BirdsList.Count == 0)
		{
			m_BirdsList = birds;
		}
		m_NewPremium = false;
		StartCoroutine(InitializeBirdWindowUI(selectedIndex));
	}

	public override void UpdateSlotIndicators()
	{
		foreach (OpenInventoryButton openInventoryButton in m_OpenInventoryButtons)
		{
			if (m_Inventory.HasNewItemBird(openInventoryButton.m_ItemType, m_SelectedBird))
			{
				if ((bool)openInventoryButton)
				{
					openInventoryButton.SetNewMarker(true);
				}
			}
			else if ((bool)openInventoryButton)
			{
				openInventoryButton.SetNewMarker(false);
			}
		}
	}

	public void RefreshAll()
	{
		m_NewPremium = false;
		StartCoroutine(InitializeBirdWindowUI(m_SelectedBirdIndex));
	}

	private IEnumerator InitializeBirdWindowUI(int selectedIndex)
	{
		DebugLog.Log("Selected Index: " + selectedIndex);
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("bird_manager_enter");
		yield return new WaitForEndOfFrame();
		m_SelectedBirdIndex = selectedIndex;
		m_SelectedBird = m_BirdsList[m_SelectedBirdIndex];
		if ((bool)m_StateMgr)
		{
			m_StateMgr.HideNewMarkerForBird(m_SelectedBird);
		}
		m_BirdEquipmentPreviewUI.SetModels(m_BirdsList);
		SetItemListContent();
		foreach (OpenInventoryButton oib2 in m_OpenInventoryButtons)
		{
			if (m_Inventory.HasNewItemBird(oib2.m_ItemType, m_SelectedBird))
			{
				if ((bool)oib2)
				{
					oib2.SetNewMarker(true);
				}
			}
			else if ((bool)oib2)
			{
				oib2.SetNewMarker(false);
			}
			oib2.Activate(false);
		}
		if (m_BirdsList.Count <= 1)
		{
			m_BirdEquipmentPreviewUI.ShowButtons(false);
		}
		else
		{
			m_BirdEquipmentPreviewUI.ShowButtons(true);
		}
		UIPanel[] panels = base.gameObject.GetComponentsInChildren<UIPanel>();
		UIPanel[] array = panels;
		foreach (UIPanel p in array)
		{
			p.enabled = true;
		}
		SelectDefaultSlot(false);
		SetItemInfo();
		yield return StartCoroutine(EnterItemList());
		m_BirdEquipmentPreviewUI.SetCharacter(m_SelectedBird);
		StartCoroutine(SetCharacterHeader());
		yield return StartCoroutine(RestorePosition(m_ItemPanel, m_SelectedSlot.transform, m_ItemGrid));
		yield return StartCoroutine(m_BirdEquipmentPreviewUI.Enter());
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("bird_manager_enter");
		foreach (OpenInventoryButton oib in m_OpenInventoryButtons)
		{
			oib.Activate(true);
		}
		m_Entered = true;
		RegisterEventHandler();
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("birdmanager_entered", string.Empty);
	}

	private IEnumerator RestorePosition(UIScrollView panel, Transform targetTransform, UIGrid containingGrid)
	{
		panel.DisableSpring();
		panel.ResetPosition();
		yield return new WaitForEndOfFrame();
		containingGrid.Reposition();
		yield return new WaitForEndOfFrame();
		if (panel.shouldMoveHorizontally)
		{
			panel.MoveAbsolute(-Vector3.Scale(targetTransform.localPosition + panel.transform.localPosition - new Vector3(panel.panel.clipRange.z / 2f - containingGrid.cellWidth / 2f, 0f, 0f), new Vector3(1f, 0f, 0f)));
		}
		else
		{
			panel.ResetPosition();
		}
		yield return new WaitForEndOfFrame();
		panel.RestrictWithinBounds(true);
	}

	private IEnumerator EnterItemList()
	{
		yield return new WaitForEndOfFrame();
		SetUpperNodIndex();
		StartCoroutine(PlayEnterAnimation());
	}

	private IEnumerator PlayEnterAnimation()
	{
		m_ItemGridAnimation.Play("CategoryContent_Enter");
		m_ItemInfoAnimation.Play("ItemInfo_Enter");
		m_BirdEquipmentPreviewUI.GetComponent<Animation>().Play("CharacterDisplay_Enter");
		m_HeaderAnimation.Play("Header_Enter");
		m_ItemCategoryButtonsAnimation.Play("Categories_Enter");
		yield break;
	}

	private IEnumerator PlayLeaveAnimation()
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("bird_manager_leave");
		m_ItemGridAnimation.Play("CategoryContent_Leave");
		m_ItemInfoAnimation.Play("ItemInfo_Leave");
		m_BirdEquipmentPreviewUI.GetComponent<Animation>().Play("CharacterDisplay_Leave");
		m_HeaderAnimation.Play("Header_Leave");
		m_ItemCategoryButtonsAnimation.Play("Categories_Leave");
		if (m_SelectedBird != null)
		{
			m_StateMgr.RemoveAllNewMarkersFromBird(m_SelectedBird);
		}
		yield return StartCoroutine(m_BirdEquipmentPreviewUI.Leave());
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("bird_manager_leave");
		base.gameObject.SetActive(false);
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave();
	}

	private IEnumerator PlayGridChangeAnimation(bool moveIn)
	{
		string postFix = ((!moveIn) ? "Out" : "In");
		m_ItemGridAnimation.Play("CategoryContent_Change_" + postFix);
		yield return StartCoroutine(PlayItemInfoChangeAnimation(moveIn));
	}

	private IEnumerator PlayItemInfoChangeAnimation(bool moveIn)
	{
		string postFix = ((!moveIn) ? "Out" : "In");
		m_ItemInfoAnimation.Play("ItemInfo_Change_" + postFix);
		yield return new WaitForSeconds(m_ItemInfoAnimation["ItemInfo_Change_" + postFix].clip.length);
	}

	private IEnumerator RefreshItemList(bool showUpdateAnim = true)
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("bird_manager_refresh");
		if (m_IsRefreshing)
		{
			yield break;
		}
		m_IsRefreshing = true;
		if (showUpdateAnim)
		{
			yield return StartCoroutine(PlayGridChangeAnimation(false));
		}
		SetItemListContent();
		foreach (OpenInventoryButton oib in m_OpenInventoryButtons)
		{
			if (m_Inventory.HasNewItemBird(oib.m_ItemType, m_SelectedBird))
			{
				if ((bool)oib)
				{
					oib.SetNewMarker(true);
				}
			}
			else if ((bool)oib)
			{
				oib.SetNewMarker(false);
			}
		}
		SetItemInfo();
		if (showUpdateAnim)
		{
			m_BirdEquipmentPreviewUI.RefreshStats(true);
		}
		SelectDefaultSlot(false);
		yield return StartCoroutine(RestorePosition(m_ItemPanel, m_SelectedSlot.transform, m_ItemGrid));
		StartCoroutine(SetCharacterHeader());
		SelectDefaultSlot(false);
		if (showUpdateAnim)
		{
			yield return StartCoroutine(PlayGridChangeAnimation(true));
		}
		m_IsRefreshing = false;
		RegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("bird_manager_refresh");
	}

	private IEnumerator RefreshItemInfo(bool playUpdateAnim)
	{
		if (!playUpdateAnim || !m_UpdateAnimBlocked)
		{
		}
		yield return new WaitForEndOfFrame();
		m_UpdateAnimBlocked = false;
		SetItemInfo();
		if (!m_AlreadyEquipped && !m_NewPremium)
		{
			m_BirdEquipmentPreviewUI.RefreshStats(true);
		}
	}

	private IEnumerator SetCharacterHeader()
	{
		if (!(m_HeaderNameAndClassName.text == DIContainerInfrastructure.GetLocaService().GetInventoryItemTypeName(m_CurrentItemType)) || m_HeaderLevelText.text == DIContainerInfrastructure.GetCurrentPlayer().Data.Level.ToString("0"))
		{
		}
		m_HeaderAnimation.Stop();
		m_HeaderAnimation.Play("Header_Change_Out");
		yield return new WaitForSeconds(m_HeaderAnimation["Header_Change_Out"].clip.length);
		m_HeaderNameAndClassName.text = DIContainerInfrastructure.GetLocaService().GetInventoryItemTypeName(m_CurrentItemType);
		m_HeaderLevelText.text = DIContainerInfrastructure.GetCurrentPlayer().Data.Level.ToString("0");
		m_HeaderAnimation.Play("Header_Change_In");
		yield return new WaitForSeconds(m_HeaderAnimation["Header_Change_In"].clip.length);
	}

	private void SetItemInfo()
	{
		bool flag = false;
		EquipmentGameData equipmentGameData = m_SelectedItem as EquipmentGameData;
		if (equipmentGameData != null)
		{
			flag = equipmentGameData.AllowEnchanting();
		}
		switch (m_SelectedItem.ItemBalancing.ItemType)
		{
		case InventoryItemType.Class:
			if (m_NewPremium)
			{
				m_ClassInfo.gameObject.SetActive(false);
				m_PremiumClassBuyInfo.gameObject.SetActive(true);
				m_EquipmentInfo.gameObject.SetActive(false);
				m_EnchantableEquipmentInfo.gameObject.SetActive(false);
				ClassItemGameData classItemGameData = m_SelectedItem as ClassItemGameData;
				m_PremiumClassBuyInfo.m_BirdUI = this;
				m_PremiumClassBuyInfo.SetModel(classItemGameData, m_SelectedBird);
			}
			else
			{
				m_ClassInfo.gameObject.SetActive(true);
				m_PremiumClassBuyInfo.gameObject.SetActive(false);
				m_EquipmentInfo.gameObject.SetActive(false);
				m_EnchantableEquipmentInfo.gameObject.SetActive(false);
				ClassItemGameData classItemGameData2 = m_SelectedItem as ClassItemGameData;
				m_ClassInfo.m_BirdUI = this;
				m_ClassInfo.SetModel(classItemGameData2, m_SelectedBird, m_BirdEquipmentPreviewUI, m_SelectedSlot);
			}
			break;
		case InventoryItemType.MainHandEquipment:
		case InventoryItemType.OffHandEquipment:
		{
			m_EquipmentInfo.gameObject.SetActive(true);
			m_ClassInfo.gameObject.SetActive(false);
			m_PremiumClassBuyInfo.gameObject.SetActive(false);
			EquipmentGameData equipmentGameData2 = m_SelectedItem as EquipmentGameData;
			if (flag)
			{
				m_EnchantableEquipmentInfo.SetModel(equipmentGameData2, m_SelectedBird, m_StateMgr is ArenaCampStateMgr);
			}
			else
			{
				m_EquipmentInfo.SetModel(equipmentGameData2, m_SelectedBird, m_StateMgr is ArenaCampStateMgr);
			}
			if (equipmentGameData2.IsSetItem)
			{
				m_PerkRoot.SetActive(false);
				m_PerkRootEnch.SetActive(false);
				m_PerkAndSetRoot.SetActive(true);
				m_PerkAndSetRootEnch.SetActive(true);
				if (flag)
				{
					m_SetItemInforEnch.SetModel(equipmentGameData2, m_SelectedBird, m_StateMgr is ArenaCampStateMgr);
				}
				else
				{
					m_SetItemInfor.SetModel(equipmentGameData2, m_SelectedBird, m_StateMgr is ArenaCampStateMgr);
				}
			}
			else
			{
				m_PerkRoot.SetActive(true);
				m_PerkRootEnch.SetActive(true);
				m_PerkAndSetRoot.SetActive(false);
				m_PerkAndSetRootEnch.SetActive(false);
			}
			break;
		}
		default:
			m_EquipmentInfo.gameObject.SetActive(false);
			m_EnchantableEquipmentInfo.gameObject.SetActive(false);
			m_ClassInfo.gameObject.SetActive(false);
			m_PremiumClassBuyInfo.gameObject.SetActive(false);
			break;
		}
		if (flag)
		{
			m_EnchantableEquipmentInfo.gameObject.SetActive(true);
			m_EquipmentInfo.gameObject.SetActive(false);
			m_EnchantmentLabel.enabled = true;
			m_EnchantmentLabel.text = equipmentGameData.EnchantementLevel.ToString();
			m_EnchantmentProgress.fillAmount = equipmentGameData.EnchantmentProgress;
			bool flag2 = equipmentGameData.IsMaxEnchanted();
			if (flag2 && equipmentGameData.EnchantementLevel == 0)
			{
				m_EnchantmentLabel.enabled = false;
				m_EnchantmentSprite.spriteName = "Enchantment_NA";
				m_EnchantableEquipmentInfo.gameObject.SetActive(false);
				m_EquipmentInfo.gameObject.SetActive(true);
				m_EquipmentInfo.SetModel(m_SelectedItem as EquipmentGameData, m_SelectedBird, m_StateMgr is ArenaCampStateMgr);
			}
			else if (flag2)
			{
				m_EnchantmentMaxLabel.SetActive(true);
				m_EnchantmentSprite.spriteName = "Enchantment_Max";
				m_EnchantmentProgress.fillAmount = 1f;
				m_EnchantItemButtonSprite.spriteName = "Button_Square_Large_D";
				m_EnchantItemButton.GetComponent<BoxCollider>().enabled = false;
			}
			else
			{
				m_EnchantmentMaxLabel.SetActive(false);
				m_EnchantmentSprite.spriteName = "Enchantment";
				m_EnchantItemButtonSprite.spriteName = "Button_Square_Large";
				m_EnchantItemButton.GetComponent<BoxCollider>().enabled = true;
			}
		}
	}

	private void SelectDefaultSlot(bool playUpdateAnim)
	{
		m_NewPremium = false;
		IInventoryItemGameData equippedItem = m_SelectedBird.InventoryGameData.Items[m_CurrentItemType].FirstOrDefault();
		InventoryItemSlot inventoryItemSlot = m_ItemSlots.FirstOrDefault((InventoryItemSlot s) => s.GetModel().ItemBalancing.NameId.Equals(equippedItem.ItemBalancing.NameId) && s.GetModel().ItemData.Level.Equals(equippedItem.ItemData.Level) && s.GetModel().ItemData.Quality.Equals(equippedItem.ItemData.Quality));
		if (!inventoryItemSlot)
		{
			inventoryItemSlot = m_ItemSlots.FirstOrDefault((InventoryItemSlot birdClass) => birdClass.GetModel().ItemBalancing is ClassItemBalancingData && !(birdClass.GetModel().ItemBalancing as ClassItemBalancingData).IsPremium);
		}
		if ((bool)m_SelectedSlot)
		{
			m_SelectedSlot.Deselect(m_NewPremium);
		}
		SelectSlot(inventoryItemSlot, playUpdateAnim);
	}

	private void SelectSlot(InventoryItemSlot inventoryItemSlot, bool playUpdateAnim)
	{
		if (!inventoryItemSlot)
		{
			return;
		}
		if ((bool)m_SelectedSlot)
		{
			m_SelectedSlot.Deselect(m_NewPremium);
		}
		foreach (InventoryItemSlot itemSlot in m_ItemSlots)
		{
			if (itemSlot != inventoryItemSlot)
			{
				itemSlot.RemoveLeftOverSelection();
			}
		}
		m_UpdateAnimBlocked = true;
		inventoryItemSlot.SelectItemData();
		if ((bool)m_SelectedSlot)
		{
			m_SelectedSlot.Select(m_NewPremium);
			m_SelectedSlot.SetUsed(true);
			foreach (InventoryItemSlot itemSlot2 in m_ItemSlots)
			{
				itemSlot2.RefreshStat();
			}
		}
		OpenInventoryButton openInventoryButton = m_OpenInventoryButtons.FirstOrDefault((OpenInventoryButton o) => o.m_ItemType == m_SelectedSlot.GetModel().ItemBalancing.ItemType);
		if (m_Inventory.HasNewItemBird(openInventoryButton.m_ItemType, m_SelectedBird))
		{
			if ((bool)openInventoryButton)
			{
				openInventoryButton.SetNewMarker(true);
			}
		}
		else if ((bool)openInventoryButton)
		{
			openInventoryButton.SetNewMarker(false);
		}
	}

	private void OnSlotUsed(InventoryItemSlot slot)
	{
		slot.SetUsed(true);
		StartCoroutine(SelectSlotBySwipe(slot));
	}

	private IEnumerator SelectSlotBySwipe(InventoryItemSlot slot)
	{
		Vector3 offset = Vector3.zero;
		Transform root = null;
		m_SelectedItem = slot.GetModel();
		if (m_SelectedItem.ItemBalancing is ClassItemBalancingData)
		{
			ClassItemBalancingData classData = (ClassItemBalancingData)m_SelectedItem.ItemBalancing;
			m_NewPremium = classData.IsPremium && !DIContainerLogic.InventoryService.CheckForItem(m_Inventory, m_SelectedItem.ItemBalancing.NameId);
			foreach (BirdGameData bird in DIContainerInfrastructure.GetCurrentPlayer().Birds)
			{
				if (bird.BalancingData.NameId == classData.RestrictedBirdId)
				{
					m_AlreadyEquipped = bird.ClassItem.BalancingData.NameId == classData.NameId;
					break;
				}
			}
		}
		else
		{
			m_AlreadyEquipped = false;
			m_NewPremium = false;
		}
		switch (m_CurrentItemType)
		{
		case InventoryItemType.Class:
			root = m_BirdEquipmentPreviewUI.m_CurrentCharacterController.m_AssetController.HeadGearBone;
			break;
		case InventoryItemType.MainHandEquipment:
			root = m_BirdEquipmentPreviewUI.m_CurrentCharacterController.m_AssetController.MainHandBone;
			break;
		case InventoryItemType.OffHandEquipment:
			root = m_BirdEquipmentPreviewUI.m_CurrentCharacterController.m_AssetController.OffHandBone;
			break;
		}
		if (!m_NewPremium && !m_AlreadyEquipped)
		{
			m_SwitchBirdsBlocked = true;
			yield return new WaitForSeconds(slot.FlyToTransform(root, offset));
		}
		yield return new WaitForEndOfFrame();
		m_SwitchBirdsBlocked = false;
		if (!slot || slot.GetModel() == null || m_CurrentItemType != slot.GetModel().ItemBalancing.ItemType)
		{
			yield break;
		}
		slot.ResetFromFly();
		OnSlotSelected(slot);
		if ((bool)m_SoundTriggers)
		{
			m_SoundTriggers.OnTriggerEventFired("item_equipped");
		}
		yield return new WaitForEndOfFrame();
		switch (m_CurrentItemType)
		{
		case InventoryItemType.Class:
			if (!m_NewPremium && !m_AlreadyEquipped)
			{
				m_BirdEquipmentPreviewUI.m_CurrentCharacterController.m_AssetController.PlayCheerAnim();
				m_BirdEquipmentPreviewUI.PlayCharacterChanged();
			}
			break;
		case InventoryItemType.MainHandEquipment:
			m_BirdEquipmentPreviewUI.m_CurrentCharacterController.m_AssetController.PlayFocusWeaponAnimation();
			break;
		case InventoryItemType.OffHandEquipment:
			m_BirdEquipmentPreviewUI.m_CurrentCharacterController.m_AssetController.PlayFocusOffHandAnimation();
			break;
		}
	}

	private void OnSlotToScrap(InventoryItemSlot slot)
	{
		slot.SetUsed(true);
		StartCoroutine(OnSlotScrappedCoroutine(slot));
	}

	private IEnumerator OnSlotScrappedCoroutine(InventoryItemSlot slot)
	{
		if (!slot || slot.GetModel() == null || m_CurrentItemType != slot.GetModel().ItemBalancing.ItemType || (slot.GetModel().ItemBalancing.ItemType != InventoryItemType.MainHandEquipment && slot.GetModel().ItemBalancing.ItemType != InventoryItemType.OffHandEquipment))
		{
			yield break;
		}
		if (!DIContainerLogic.CraftingService.IsScrapPossible(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, slot.GetModel()))
		{
			DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_scrap_failed"), "scrapFailed", DispatchMessage.Status.Info);
			if (slot.GetModel() != null)
			{
				DebugLog.Warn("Can not scrap item: " + m_SelectedItem.ItemBalancing.NameId);
				RefreshItemList();
			}
			else
			{
				DebugLog.Warn("No valid Item Selected!");
			}
		}
		else if (IsSetItemSlot(slot))
		{
			DIContainerInfrastructure.GetCoreStateMgr().ShowConfirmationPopup(DIContainerInfrastructure.GetLocaService().Tr("confirmation_scrap_setitem", "Do you really want to scrap a set item?"), delegate
			{
				ScrapEquipment(slot);
			}, delegate
			{
			});
		}
		else if (IsBestItemSlot(slot))
		{
			DIContainerInfrastructure.GetCoreStateMgr().ShowConfirmationPopup(DIContainerInfrastructure.GetLocaService().Tr("confirmation_scrap_bestitem", "Do you really want to scrap your best item?"), delegate
			{
				ScrapEquipment(slot);
			}, delegate
			{
			});
		}
		else
		{
			ScrapEquipment(slot);
		}
	}

	private void ScrapEquipment(InventoryItemSlot slot)
	{
		List<IInventoryItemGameData> scrapLoot = DIContainerLogic.CraftingService.ScrapEquipment(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, slot.GetModel() as EquipmentGameData);
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_scrap_success", "?Item successfully scrapped?"), "scrapSuccess", DispatchMessage.Status.Info);
		OnSlotScrapped(slot, scrapLoot);
	}

	private bool IsSetItemSlot(InventoryItemSlot slot)
	{
		return slot.GetModel() is EquipmentGameData && ((EquipmentGameData)slot.GetModel()).IsSetItem;
	}

	private bool IsBestItemSlot(InventoryItemSlot slot)
	{
		InventoryItemSlot inventoryItemSlot = m_ItemSlots.FirstOrDefault();
		if (inventoryItemSlot != null)
		{
			EquipmentGameData equipmentGameData = inventoryItemSlot.GetModel() as EquipmentGameData;
			if (equipmentGameData == null)
			{
				return false;
			}
			if (Math.Abs(slot.GetModel().ItemMainStat - equipmentGameData.ItemMainStat) < 0.1f)
			{
				return true;
			}
		}
		return false;
	}

	private void OnSlotScrapped(InventoryItemSlot slot, List<IInventoryItemGameData> scrapLoot)
	{
		StartCoroutine(SoftRefresh(slot.GetModel(), scrapLoot));
	}

	private void SetItemListContent()
	{
		m_GameDatas = (from i in m_Inventory.Items[m_CurrentItemType]
			where i.IsValidForBird(m_SelectedBird)
			select i into d
			orderby d.ItemMainStat descending
			select d).ToList();
		IList<ClassItemBalancingData> list = (from c in DIContainerBalancing.Service.GetBalancingDataList<ClassItemBalancingData>()
			where c.RestrictedBirdId == m_SelectedBird.BalancingData.NameId
			select c).ToList();
		for (int j = 0; j < m_GameDatas.Count; j++)
		{
			for (int num = list.Count - 1; num >= 0; num--)
			{
				ClassItemBalancingData classItemBalancingData = list[num];
				ClassItemGameData classItemGameData = m_GameDatas[j] as ClassItemGameData;
				if (classItemBalancingData == null || !string.IsNullOrEmpty(classItemBalancingData.ReplacementClassNameId) || m_GameDatas[j].ItemBalancing.NameId == list[num].NameId || (classItemGameData != null && classItemGameData.BalancingData.ReplacementClassNameId == list[num].NameId))
				{
					list.RemoveAt(num);
				}
				IInventoryItemGameData inventoryItemGameData = new ClassItemGameData(classItemBalancingData.NameId);
				foreach (IInventoryItemGameData gameData in m_GameDatas)
				{
					if (gameData.Name == inventoryItemGameData.Name)
					{
						break;
					}
				}
				if (classItemBalancingData.Inactive)
				{
					list.RemoveAt(num);
				}
			}
		}
		list = list.OrderBy((ClassItemBalancingData d) => d.SortPriority).ToList();
		if (m_CurrentItemType == InventoryItemType.Class)
		{
			m_GameDatas = m_GameDatas.OrderBy((IInventoryItemGameData d) => d.ItemBalancing.SortPriority).ToList();
		}
		for (int num2 = m_ItemSlots.Count - 1; num2 >= 0; num2--)
		{
			InventoryItemSlot inventoryItemSlot = m_ItemSlots[num2];
			DeRegisterEventHandlerFromSlot(inventoryItemSlot);
			m_ItemSlots.Remove(inventoryItemSlot);
			UnityEngine.Object.Destroy(inventoryItemSlot.gameObject);
		}
		int num3 = 0;
		for (int k = 0; k < m_GameDatas.Count; k++)
		{
			IInventoryItemGameData inventoryItemGameData2 = m_GameDatas[k];
			InventoryItemSlot inventoryItemSlot2 = InstantiateItemSlot(inventoryItemGameData2);
			inventoryItemSlot2.name = (k + 1).ToString("000") + inventoryItemGameData2.ItemBalancing.SortPriority.ToString("00") + "_" + inventoryItemSlot2.name;
			m_ItemSlots.Add(inventoryItemSlot2);
			inventoryItemSlot2.transform.parent = m_ItemGrid.transform;
			inventoryItemSlot2.transform.localPosition = Vector3.zero;
			inventoryItemSlot2.SetModel(inventoryItemGameData2, m_StateMgr is ArenaCampStateMgr);
			DeRegisterEventHandlerFromSlot(inventoryItemSlot2);
			RegisterEventHandlerFromSlot(inventoryItemSlot2);
			num3++;
		}
		if (m_CurrentItemType != InventoryItemType.Class)
		{
			return;
		}
		for (int l = 0; l < list.Count; l++)
		{
			ClassItemGameData classItemGameData2 = new ClassItemGameData(list[l].NameId);
			InventoryItemSlot inventoryItemSlot3 = InstantiateItemSlot(classItemGameData2);
			if (classItemGameData2.BalancingData.IsPremium && classItemGameData2.ItemData.Level == 0 && DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "unlock_mastery_badge") > 0)
			{
				DIContainerInfrastructure.GetCurrentPlayer().AdvanceBirdMasteryToHalfOfHighest(classItemGameData2);
			}
			inventoryItemSlot3.name = (num3 + 1).ToString("000") + classItemGameData2.ItemBalancing.SortPriority.ToString("00") + "_" + inventoryItemSlot3.name;
			m_ItemSlots.Add(inventoryItemSlot3);
			inventoryItemSlot3.transform.parent = m_ItemGrid.transform;
			inventoryItemSlot3.transform.localPosition = Vector3.zero;
			inventoryItemSlot3.SetModel(classItemGameData2, m_StateMgr is ArenaCampStateMgr);
			if (classItemGameData2.BalancingData.IsPremium)
			{
				inventoryItemSlot3.m_purchaseIndicator.SetActive(true);
				bool flag = DIContainerLogic.GetSalesManagerService().IsItemOnSale(classItemGameData2.ItemBalancing.NameId);
				inventoryItemSlot3.m_purchaseIndicatorBody.color = ((!flag) ? m_colorWhite : m_colorDarkGreen);
			}
			else
			{
				inventoryItemSlot3.SetSlotUnavailable();
			}
			DeRegisterEventHandlerFromSlot(inventoryItemSlot3);
			RegisterEventHandlerFromSlot(inventoryItemSlot3);
			num3++;
		}
	}

	private InventoryItemSlot InstantiateItemSlot(IInventoryItemGameData item)
	{
		switch (item.ItemBalancing.ItemType)
		{
		case InventoryItemType.Class:
			return UnityEngine.Object.Instantiate(ClassSlotPrefab);
		case InventoryItemType.MainHandEquipment:
		case InventoryItemType.OffHandEquipment:
			return UnityEngine.Object.Instantiate(EquipmentSlotPrefab);
		default:
			return UnityEngine.Object.Instantiate(ItemSlotPrefab);
		}
	}

	private void DeRegisterEventHandlerFromSlot(InventoryItemSlot slot)
	{
		slot.OnUsed -= OnSlotUsed;
		slot.OnScrap -= OnSlotToScrap;
		slot.OnSelected -= OnSlotSelected;
		m_EnchantItemButton.Clicked -= OpenEnchantmentUi;
		if ((bool)m_ClassInfoButton)
		{
			m_ClassInfoButton.Clicked -= OpenClassInfo;
		}
	}

	private void RegisterEventHandlerFromSlot(InventoryItemSlot slot)
	{
		DeRegisterEventHandlerFromSlot(slot);
		slot.OnUsed += OnSlotUsed;
		slot.OnScrap += OnSlotToScrap;
		slot.OnSelected += OnSlotSelected;
		m_EnchantItemButton.Clicked += OpenEnchantmentUi;
		if ((bool)m_ClassInfoButton)
		{
			m_ClassInfoButton.Clicked += OpenClassInfo;
		}
	}

	private void OnSlotSelected(InventoryItemSlot slot)
	{
		m_SelectedItem = slot.GetModel();
		if (!m_NewPremium)
		{
			DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { m_SelectedItem }, m_CurrentItemType, m_SelectedBird.InventoryGameData);
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			foreach (InventoryItemSlot itemSlot in m_ItemSlots)
			{
				if (itemSlot != slot)
				{
					itemSlot.RemoveLeftOverSelection();
				}
			}
		}
		if ((bool)m_SelectedSlot)
		{
			m_SelectedSlot.Deselect(m_NewPremium);
		}
		m_SelectedSlot = slot;
		m_SelectedSlot.Select(m_NewPremium);
		foreach (InventoryItemSlot itemSlot2 in m_ItemSlots)
		{
			itemSlot2.RefreshStat();
		}
		StartCoroutine(RefreshItemInfo(true));
		OpenInventoryButton openInventoryButton = m_OpenInventoryButtons.FirstOrDefault((OpenInventoryButton o) => o.m_ItemType == m_SelectedSlot.GetModel().ItemBalancing.ItemType);
		if (m_Inventory.HasNewItemBird(openInventoryButton.m_ItemType, m_SelectedBird))
		{
			if ((bool)openInventoryButton)
			{
				openInventoryButton.SetNewMarker(true);
			}
		}
		else if ((bool)openInventoryButton)
		{
			openInventoryButton.SetNewMarker(false);
		}
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, HandleBackButton);
		m_ButtonClose.Clicked += m_ButtonClose_Clicked;
		m_ScrapItemButton.Clicked += m_ScrapItemButton_Clicked;
		m_ScrapItemButtonEnch.Clicked += m_ScrapItemButton_Clicked;
		RegisterNextCharacterButtons();
		RegisterCategoryButtons();
		m_Inventory.InventoryOfTypeChanged -= OnInventoryOfTypeChanged;
		m_Inventory.InventoryOfTypeChanged += OnInventoryOfTypeChanged;
	}

	private void DeRegisterNextCharacterButtons()
	{
		if (m_BirdEquipmentPreviewUI.m_NextCharacterButton != null && m_BirdEquipmentPreviewUI.m_PreviousCharacterButton != null)
		{
			m_BirdEquipmentPreviewUI.m_NextCharacterButton.Clicked -= m_NextCharacterButton_Clicked;
			m_BirdEquipmentPreviewUI.m_PreviousCharacterButton.Clicked -= m_PreviousCharacterButton_Clicked;
		}
	}

	private void RegisterNextCharacterButtons()
	{
		if (m_BirdEquipmentPreviewUI.m_NextCharacterButton != null && m_BirdEquipmentPreviewUI.m_PreviousCharacterButton != null)
		{
			DeRegisterNextCharacterButtons();
			m_BirdEquipmentPreviewUI.m_NextCharacterButton.Clicked += m_NextCharacterButton_Clicked;
			m_BirdEquipmentPreviewUI.m_PreviousCharacterButton.Clicked += m_PreviousCharacterButton_Clicked;
		}
	}

	private void DeRegisterCategoryButtons()
	{
		foreach (OpenInventoryButton openInventoryButton in m_OpenInventoryButtons)
		{
			openInventoryButton.SetUsable(false).OnButtonClicked -= OnOpenInventoryButtonClicked;
		}
	}

	private void RegisterCategoryButtons()
	{
		DeRegisterCategoryButtons();
		foreach (OpenInventoryButton openInventoryButton in m_OpenInventoryButtons)
		{
			openInventoryButton.SetUsable(true).OnButtonClicked += OnOpenInventoryButtonClicked;
		}
	}

	private void m_PreviousCharacterButton_Clicked()
	{
		if (!m_SwitchBirdsBlocked)
		{
			for (int i = 0; i < m_ItemSlots.Count; i++)
			{
				m_ItemSlots[i].SetIsNew(false);
			}
			if (m_SelectedBirdIndex == 0)
			{
				m_SelectedBirdIndex = m_BirdsList.Count - 1;
			}
			else
			{
				m_SelectedBirdIndex--;
			}
			if (m_SelectedBird != null)
			{
				m_StateMgr.RemoveAllNewMarkersFromBird(m_SelectedBird);
			}
			m_SelectedBird = m_BirdsList[m_SelectedBirdIndex];
			if ((bool)m_StateMgr)
			{
				m_StateMgr.HideNewMarkerForBird(m_SelectedBird);
			}
			m_BirdEquipmentPreviewUI.SetCharacter(m_SelectedBird);
			DeRegisterEventHandler(false);
			StartCoroutine(RefreshItemList());
		}
	}

	private void m_NextCharacterButton_Clicked()
	{
		if (!m_SwitchBirdsBlocked)
		{
			for (int i = 0; i < m_ItemSlots.Count; i++)
			{
				m_ItemSlots[i].SetIsNew(false);
			}
			if (m_SelectedBird != null)
			{
				m_StateMgr.RemoveAllNewMarkersFromBird(m_SelectedBird);
			}
			m_SelectedBirdIndex = (m_SelectedBirdIndex + 1) % m_BirdsList.Count;
			m_SelectedBird = m_BirdsList[m_SelectedBirdIndex];
			if ((bool)m_StateMgr)
			{
				m_StateMgr.HideNewMarkerForBird(m_SelectedBird);
			}
			m_BirdEquipmentPreviewUI.SetCharacter(m_SelectedBird);
			DeRegisterEventHandler(false);
			StartCoroutine(RefreshItemList());
		}
	}

	private void OnInventoryOfTypeChanged(InventoryItemType itemType, IInventoryItemGameData item)
	{
	}

	public override IEnumerator SoftRefresh(IInventoryItemGameData removed, List<IInventoryItemGameData> scrapLoot)
	{
		m_ScrappingLocks++;
		InventoryItemSlot next = m_ItemSlots.FirstOrDefault();
		InventoryItemSlot destroyed = null;
		List<InventoryItemSlot> nextSlots = new List<InventoryItemSlot>();
		bool takeNext = false;
		bool selectedNext = false;
		int index = 0;
		float slideDuration = DIContainerLogic.GetPacingBalancing().EquipmentRepositionDuration;
		yield return new WaitForEndOfFrame();
		foreach (InventoryItemSlot slot2 in m_ItemSlots.Where((InventoryItemSlot s) => !s.IsDestroyedCurrently()))
		{
			if (takeNext && !selectedNext)
			{
				selectedNext = true;
				next = slot2;
				SelectSlot(next, true);
			}
			if (slot2.GetModel() == removed && !takeNext)
			{
				takeNext = true;
				destroyed = slot2;
			}
			else if (!takeNext)
			{
				next = slot2;
			}
			if (selectedNext)
			{
				nextSlots.Add(slot2);
			}
			index++;
		}
		if (!selectedNext)
		{
			SelectSlot(next, false);
		}
		if ((bool)destroyed)
		{
			m_ItemSlots.Remove(destroyed);
			if (m_EnchantableEquipmentInfo.gameObject.activeSelf)
			{
				yield return new WaitForSeconds(destroyed.FlyToTransform(m_ScrapItemRootEnch.transform, Vector3.zero, true));
			}
			else
			{
				yield return new WaitForSeconds(destroyed.FlyToTransform(m_ScrapItemRoot.transform, Vector3.zero, true));
			}
			yield return new WaitForEndOfFrame();
			destroyed.RemoveAssets();
			if (scrapLoot.Count > 0)
			{
				LootDisplayContoller item2 = null;
				item2 = ((!m_EnchantableEquipmentInfo.gameObject.activeSelf) ? (UnityEngine.Object.Instantiate(m_LootForExplosionPrefab, m_ScrapItemButton.transform.position, Quaternion.identity) as LootDisplayContoller) : (UnityEngine.Object.Instantiate(m_LootForExplosionPrefab, m_ScrapItemButtonEnch.transform.position, Quaternion.identity) as LootDisplayContoller));
				item2.SetModel(removed, scrapLoot, LootDisplayType.None);
				List<LootDisplayContoller> explodedItems = item2.Explode(true, false, 0f, true, 0f, 0f);
				foreach (LootDisplayContoller explodedLoot in explodedItems)
				{
					UnityEngine.Object.Destroy(explodedLoot.gameObject, explodedLoot.gameObject.GetComponent<Animation>().clip.length);
				}
			}
			else
			{
				GameObject explosionFX2 = null;
				explosionFX2 = ((!m_EnchantableEquipmentInfo.gameObject.activeSelf) ? (UnityEngine.Object.Instantiate(m_ExplodedFXPrefab, m_ScrapItemButtonEnch.transform.position, Quaternion.identity) as GameObject) : (UnityEngine.Object.Instantiate(m_ExplodedFXPrefab, m_ScrapItemButton.transform.position, Quaternion.identity) as GameObject));
				if (explosionFX2.GetComponent<Animation>().clip != null)
				{
					UnityEngine.Object.Destroy(explosionFX2, explosionFX2.GetComponent<Animation>().clip.length);
				}
			}
			if ((bool)m_SoundTriggers)
			{
				m_SoundTriggers.OnTriggerEventFired("item_scrapped");
			}
			UnityEngine.Object.Destroy(destroyed.gameObject, 0.5f);
			yield return new WaitForSeconds(0.5f);
		}
		foreach (InventoryItemSlot slot in nextSlots)
		{
			StartCoroutine(slot.MoveOffset(new Vector2(0f - m_ItemGrid.cellWidth, 0f), slideDuration));
		}
		if (!selectedNext)
		{
			yield return new WaitForEndOfFrame();
			m_ItemGrid.Reposition();
			if (m_ItemPanel.shouldMoveHorizontally)
			{
				SpringPanel spanel = null;
				if (m_ItemPanel.MoveBySpring(new Vector3(m_ItemGrid.cellWidth, 0f, 0f), false, out spanel))
				{
					m_finishedSpring = false;
					spanel.onFinished = OnFinishedPanelSpring;
					while (!m_finishedSpring)
					{
						yield return new WaitForEndOfFrame();
					}
				}
			}
			yield return new WaitForEndOfFrame();
			m_ItemPanel.RestrictWithinBounds(false);
		}
		else
		{
			yield return new WaitForSeconds(slideDuration);
			yield return new WaitForEndOfFrame();
			m_ItemGrid.Reposition();
		}
		m_ScrappingLocks = Mathf.Max(0, m_ScrappingLocks - 1);
	}

	private void OnFinishedPanelSpring()
	{
		m_finishedSpring = true;
	}

	private void OnOpenInventoryButtonClicked(InventoryItemType itemType)
	{
		if (itemType != m_CurrentItemType)
		{
			for (int i = 0; i < m_ItemSlots.Count; i++)
			{
				m_ItemSlots[i].SetIsNew(false);
			}
			DeRegisterEventHandler(false);
			m_CurrentItemType = itemType;
			SetUpperNodIndex();
			StartCoroutine(RefreshItemList());
		}
	}

	private void SetUpperNodIndex()
	{
		for (int i = 0; i < m_OpenInventoryButtons.Count; i++)
		{
			if (m_OpenInventoryButtons[i].m_ItemType == m_CurrentItemType)
			{
				m_UpperNodButtonIndex = i;
				m_OpenInventoryButtons[i].Select();
			}
		}
	}

	private void DeRegisterEventHandler(bool deregisterSlots = true)
	{
		m_ButtonClose.Clicked -= m_ButtonClose_Clicked;
		m_ScrapItemButton.Clicked -= m_ScrapItemButton_Clicked;
		m_ScrapItemButtonEnch.Clicked -= m_ScrapItemButton_Clicked;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(2);
		DeRegisterNextCharacterButtons();
		DeRegisterCategoryButtons();
		if (deregisterSlots)
		{
			foreach (InventoryItemSlot itemSlot in m_ItemSlots)
			{
				itemSlot.OnUsed -= OnSlotSelected;
			}
		}
		if (m_Inventory != null)
		{
			m_Inventory.InventoryOfTypeChanged -= OnInventoryOfTypeChanged;
		}
	}

	private void m_ScrapItemButton_Clicked()
	{
		OnSlotToScrap(m_SelectedSlot);
	}

	private void m_ButtonClose_Clicked()
	{
		if (m_ScrappingLocks <= 0)
		{
			for (int i = 0; i < m_ItemSlots.Count; i++)
			{
				m_ItemSlots[i].SetIsNew(false);
			}
			StartCoroutine(PlayLeaveAnimation());
			m_Entered = false;
		}
	}

	private void OnDestroy()
	{
		if ((bool)DIContainerInfrastructure.GetCoreStateMgr())
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave();
		}
	}

	private void HandleBackButton()
	{
		DebugLog.Log("Pressed Back Button: " + GetType());
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("back_button_pressed", string.Empty);
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		m_ButtonClose_Clicked();
	}

	private void OpenClassInfo()
	{
		DIContainerInfrastructure.GetCoreStateMgr().ShowClassInfoUi(m_SelectedItem as ClassItemGameData, m_SelectedBird.ClassSkin);
	}

	private void OpenEnchantmentUi()
	{
		if ((bool)m_SelectedSlot && m_SelectedSlot.GetModel() != null && m_CurrentItemType == m_SelectedSlot.GetModel().ItemBalancing.ItemType)
		{
			StartCoroutine(ActivateEnchanting());
		}
	}

	private IEnumerator ActivateEnchanting()
	{
		if (m_StateMgr.m_EnchantmentUi == null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Popup_Enchantment", OnEnchantmentPopupLoaded);
			yield break;
		}
		m_ItemInfoAnimation.Play("ItemInfo_Leave");
		m_HeaderAnimation.Play("Header_Leave");
		m_ItemCategoryButtonsAnimation.Play("Categories_Leave");
		yield return StartCoroutine(m_BirdEquipmentPreviewUI.Leave());
		m_StateMgr.m_EnchantmentUi.EnterBird(m_SelectedItem as EquipmentGameData, this, m_StateMgr.m_EnchantmentPopup);
		m_ScrappingLocks = 0;
		base.gameObject.SetActive(false);
	}

	public void OnEnchantmentUiLoaded()
	{
		m_StateMgr.m_EnchantmentUi = UnityEngine.Object.FindObjectOfType(typeof(EnchantmentUI)) as EnchantmentUI;
		m_StateMgr.m_EnchantmentUi.gameObject.SetActive(false);
		DebugLog.Log("Window_Enchantment loaded!");
		StartCoroutine(ActivateEnchanting());
	}

	public void OnEnchantmentPopupLoaded()
	{
		m_StateMgr.m_EnchantmentPopup = UnityEngine.Object.FindObjectOfType(typeof(EnchantingResultPopup)) as EnchantingResultPopup;
		m_StateMgr.m_EnchantmentPopup.gameObject.SetActive(false);
		DebugLog.Log("Popup_Enchantment loaded!");
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_Enchantment", OnEnchantmentUiLoaded);
	}

	public void DeactivateEnchanting()
	{
		base.gameObject.SetActive(true);
		SetUpperNodIndex();
		StartCoroutine(m_BirdEquipmentPreviewUI.Enter());
		StartCoroutine(PlayEnterAnimation());
		SetItemListContent();
		SelectDefaultSlot(false);
		StartCoroutine(RestorePosition(m_ItemPanel, m_SelectedSlot.transform, m_ItemGrid));
	}

	public bool isPvp()
	{
		return m_StateMgr is ArenaCampStateMgr;
	}
}

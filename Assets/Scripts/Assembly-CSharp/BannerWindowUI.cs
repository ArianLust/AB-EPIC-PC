using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using UnityEngine;

public class BannerWindowUI : BirdWindowUIBase
{
	[SerializeField]
	private InventoryItemSlot m_TipSlotPrefab;

	[SerializeField]
	private InventoryItemSlot m_BannerSlotPrefab;

	[SerializeField]
	private InventoryItemSlot m_EmblemSlotPrefab;

	[SerializeField]
	private UILabel m_EquipmentInfoText;

	[SerializeField]
	private BannerItemInfo m_BannerItemInfo;

	[SerializeField]
	private BannerEquipmentPreviewUI m_BannerEquipmentPreviewUI;

	[SerializeField]
	private UILabel m_HeaderLevelText;

	[SerializeField]
	private UILabel m_HeaderNameAndClassName;

	[SerializeField]
	private GameObject m_ExplodedFXPrefab;

	[SerializeField]
	private List<OpenInventoryButton> m_OpenInventoryButtons = new List<OpenInventoryButton>();

	private List<IInventoryItemGameData> m_GameDatas = new List<IInventoryItemGameData>();

	private BannerGameData m_Banner;

	private InventoryGameData m_Inventory;

	private InventoryItemType m_CurrentItemType = InventoryItemType.BannerTip;

	private IInventoryItemGameData m_SelectedItem;

	private bool m_UpdateAnimBlocked;

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
	private BannerItemInfo m_EnchantableEquipmentInfo;

	[SerializeField]
	private UILabel m_EnchantmentLabel;

	[SerializeField]
	private UISprite m_EnchantmentProgress;

	[SerializeField]
	private UISprite m_EnchantmentSprite;

	[SerializeField]
	private GameObject m_EnchantmentMaxLabel;

	[SerializeField]
	private UIInputTrigger m_EnchantItemButton;

	[SerializeField]
	private UISprite m_EnchantItemButtonSprite;

	[SerializeField]
	private SoundTriggerList m_SoundTriggers;

	private bool m_IsRefreshing;

	private int m_SelectedBirdIndex;

	private bool m_Entered;

	private int m_ScrappingLocks;

	private bool m_finishedSpring;

	private BaseCampStateMgr m_StateMgr;

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

	public void SetModel(InventoryGameData inventory, BannerGameData banner, InventoryItemType defaultItemType = InventoryItemType.Class)
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Enter();
		base.gameObject.SetActive(true);
		m_Inventory = inventory;
		m_Banner = banner;
		StartCoroutine(InitializeBannerWindowUI());
	}

	public override void UpdateSlotIndicators()
	{
		foreach (OpenInventoryButton openInventoryButton in m_OpenInventoryButtons)
		{
			if (m_Inventory.HasNewItemBanner(openInventoryButton.m_ItemType, m_Banner))
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

	private IEnumerator InitializeBannerWindowUI()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("banner_manager_animate");
		yield return new WaitForEndOfFrame();
		if ((bool)m_StateMgr)
		{
			(m_StateMgr as ArenaCampStateMgr).RefreshBannerMarkers();
		}
		SetItemListContent();
		foreach (OpenInventoryButton oib2 in m_OpenInventoryButtons)
		{
			if (m_Inventory.HasNewItemBanner(oib2.m_ItemType, m_Banner))
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
		UIPanel[] panels = base.gameObject.GetComponentsInChildren<UIPanel>();
		UIPanel[] array = panels;
		foreach (UIPanel p in array)
		{
			p.enabled = true;
		}
		SelectDefaultSlot(false);
		SetItemInfo();
		yield return StartCoroutine(EnterItemList());
		StartCoroutine(SetHeader());
		yield return StartCoroutine(RestorePosition(m_ItemPanel, m_SelectedSlot.transform, m_ItemGrid));
		m_BannerEquipmentPreviewUI.SetModel(m_Banner);
		yield return StartCoroutine(m_BannerEquipmentPreviewUI.Enter());
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("banner_manager_animate");
		foreach (OpenInventoryButton oib in m_OpenInventoryButtons)
		{
			oib.Activate(true);
		}
		m_Entered = true;
		RegisterEventHandler();
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("bannermanager_entered", string.Empty);
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
		m_BannerEquipmentPreviewUI.GetComponent<Animation>().Play("CharacterDisplay_Enter");
		m_HeaderAnimation.Play("Header_Enter");
		m_ItemCategoryButtonsAnimation.Play("Categories_Enter");
		yield break;
	}

	private IEnumerator PlayLeaveAnimation()
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("banner_manager_animate");
		m_ItemGridAnimation.Play("CategoryContent_Leave");
		m_ItemInfoAnimation.Play("ItemInfo_Leave");
		m_BannerEquipmentPreviewUI.GetComponent<Animation>().Play("CharacterDisplay_Leave");
		m_HeaderAnimation.Play("Header_Leave");
		m_ItemCategoryButtonsAnimation.Play("Categories_Leave");
		(m_StateMgr as ArenaCampStateMgr).RemoveAllNewMarkersFromBanner(m_Banner);
		yield return StartCoroutine(m_BannerEquipmentPreviewUI.Leave());
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("banner_manager_animate");
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

	private IEnumerator RefreshItemList()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("bird_manager_refresh");
		if (m_IsRefreshing)
		{
			yield break;
		}
		m_IsRefreshing = true;
		yield return StartCoroutine(PlayGridChangeAnimation(false));
		SetItemListContent();
		foreach (OpenInventoryButton oib in m_OpenInventoryButtons)
		{
			if (m_Inventory.HasNewItemBanner(oib.m_ItemType, m_Banner))
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
		m_BannerEquipmentPreviewUI.RefreshStats(false);
		SelectDefaultSlot(false);
		yield return StartCoroutine(RestorePosition(m_ItemPanel, m_SelectedSlot.transform, m_ItemGrid));
		StartCoroutine(SetHeader());
		yield return StartCoroutine(PlayGridChangeAnimation(true));
		m_ItemInfoAnimation.Play("ItemInfo_Change_In");
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
		m_BannerEquipmentPreviewUI.RefreshStats(playUpdateAnim);
	}

	private IEnumerator SetHeader()
	{
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
		BannerItemGameData bannerItemGameData = m_SelectedItem as BannerItemGameData;
		if (bannerItemGameData != null)
		{
			flag = bannerItemGameData.AllowEnchanting();
		}
		if (flag)
		{
			m_EnchantableEquipmentInfo.gameObject.SetActive(true);
			m_BannerItemInfo.gameObject.SetActive(false);
			m_EnchantableEquipmentInfo.SetModel(m_Banner, bannerItemGameData);
			m_EnchantmentLabel.enabled = true;
			m_EnchantmentLabel.text = bannerItemGameData.EnchantementLevel.ToString();
			m_EnchantmentProgress.fillAmount = bannerItemGameData.EnchantmentProgress;
			bool flag2 = bannerItemGameData.IsMaxEnchanted();
			if (flag2 && bannerItemGameData.EnchantementLevel == 0)
			{
				m_EnchantmentLabel.enabled = false;
				m_EnchantmentSprite.spriteName = "Enchantment_NA";
				m_EnchantableEquipmentInfo.gameObject.SetActive(false);
				m_BannerItemInfo.gameObject.SetActive(true);
				m_BannerItemInfo.SetModel(m_Banner, bannerItemGameData);
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
		else
		{
			m_BannerItemInfo.gameObject.SetActive(true);
			m_EnchantableEquipmentInfo.gameObject.SetActive(false);
			m_BannerItemInfo.SetModel(m_Banner, bannerItemGameData);
		}
	}

	private void SelectDefaultSlot(bool playUpdateAnim)
	{
		IInventoryItemGameData equippedItem = m_Banner.InventoryGameData.Items[m_CurrentItemType].FirstOrDefault();
		InventoryItemSlot inventoryItemSlot = m_ItemSlots.FirstOrDefault((InventoryItemSlot s) => s.GetModel().ItemBalancing.NameId.Equals(equippedItem.ItemBalancing.NameId) && s.GetModel().ItemData.Level.Equals(equippedItem.ItemData.Level) && s.GetModel().ItemData.Quality.Equals(equippedItem.ItemData.Quality));
		if (!inventoryItemSlot)
		{
			inventoryItemSlot = m_ItemSlots.FirstOrDefault();
		}
		if ((bool)m_SelectedSlot)
		{
			m_SelectedSlot.Deselect();
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
			m_SelectedSlot.Deselect();
		}
		m_UpdateAnimBlocked = true;
		inventoryItemSlot.SelectItemData();
		if ((bool)m_SelectedSlot)
		{
			m_SelectedSlot.Select();
			m_SelectedSlot.SetUsed(true);
			foreach (InventoryItemSlot itemSlot in m_ItemSlots)
			{
				itemSlot.RefreshStat();
			}
		}
		OpenInventoryButton openInventoryButton = m_OpenInventoryButtons.FirstOrDefault((OpenInventoryButton o) => o.m_ItemType == m_SelectedSlot.GetModel().ItemBalancing.ItemType);
		if (m_Inventory.HasNewItemBanner(openInventoryButton.m_ItemType, m_Banner))
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

	private void OnSlotToScrap(InventoryItemSlot slot)
	{
		slot.SetUsed(true);
		StartCoroutine(OnSlotScrappedCoroutine(slot));
	}

	private IEnumerator OnSlotScrappedCoroutine(InventoryItemSlot slot)
	{
		if (!(slot.GetModel() is BannerItemGameData) || !slot || slot.GetModel() == null || m_CurrentItemType != slot.GetModel().ItemBalancing.ItemType)
		{
			yield break;
		}
		List<IInventoryItemGameData> scrapLoot = null;
		if (!DIContainerLogic.CraftingService.IsScrapPossible(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, slot.GetModel() as BannerItemGameData))
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
				ScrapItem(slot);
			}, delegate
			{
			});
		}
		else if (IsBestItemSlot(slot))
		{
			DIContainerInfrastructure.GetCoreStateMgr().ShowConfirmationPopup(DIContainerInfrastructure.GetLocaService().Tr("confirmation_scrap_bestitem", "Do you really want to scrap your best item?"), delegate
			{
				ScrapItem(slot);
			}, delegate
			{
			});
		}
		else
		{
			ScrapItem(slot);
		}
	}

	private void ScrapItem(InventoryItemSlot slot)
	{
		StartCoroutine(RefreshItemInfo(true));
		List<IInventoryItemGameData> scrapLoot = DIContainerLogic.CraftingService.ScrapBannerItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, slot.GetModel() as BannerItemGameData);
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_scrap_success", "?Item successfully scrapped?"), "scrapSuccess", DispatchMessage.Status.Info);
		OnSlotScrapped(slot, scrapLoot);
	}

	private bool IsSetItemSlot(InventoryItemSlot slot)
	{
		return slot.GetModel() is BannerItemGameData && ((BannerItemGameData)slot.GetModel()).IsSetItem;
	}

	private bool IsBestItemSlot(InventoryItemSlot slot)
	{
		InventoryItemSlot inventoryItemSlot = m_ItemSlots.FirstOrDefault();
		if (inventoryItemSlot != null)
		{
			BannerItemGameData bannerItemGameData = inventoryItemSlot.GetModel() as BannerItemGameData;
			if (bannerItemGameData == null)
			{
				return false;
			}
			if (Math.Abs(slot.GetModel().ItemMainStat - bannerItemGameData.ItemMainStat) < 0.1f)
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
		m_GameDatas = m_Inventory.Items[m_CurrentItemType].OrderByDescending((IInventoryItemGameData d) => d.ItemMainStat).ToList();
		for (int num = m_ItemSlots.Count - 1; num >= 0; num--)
		{
			InventoryItemSlot inventoryItemSlot = m_ItemSlots[num];
			DeRegisterEventHandlerFromSlot(inventoryItemSlot);
			m_ItemSlots.Remove(inventoryItemSlot);
			UnityEngine.Object.Destroy(inventoryItemSlot.gameObject);
		}
		int num2 = 0;
		for (int i = 0; i < m_GameDatas.Count; i++)
		{
			IInventoryItemGameData inventoryItemGameData = m_GameDatas[i];
			InventoryItemSlot inventoryItemSlot2 = InstantiateItemSlot(inventoryItemGameData);
			inventoryItemSlot2.name = (i + 1).ToString("000") + inventoryItemGameData.ItemBalancing.SortPriority.ToString("00") + "_" + inventoryItemSlot2.name;
			m_ItemSlots.Add(inventoryItemSlot2);
			inventoryItemSlot2.transform.parent = m_ItemGrid.transform;
			inventoryItemSlot2.transform.localPosition = Vector3.zero;
			inventoryItemSlot2.SetModel(inventoryItemGameData, true);
			UnityHelper.SetLayerRecusively(inventoryItemSlot2.gameObject, LayerMask.NameToLayer("Interface"));
			DeRegisterEventHandlerFromSlot(inventoryItemSlot2);
			RegisterEventHandlerFromSlot(inventoryItemSlot2);
			num2++;
		}
	}

	private InventoryItemSlot InstantiateItemSlot(IInventoryItemGameData item)
	{
		switch (item.ItemBalancing.ItemType)
		{
		case InventoryItemType.Banner:
			return UnityEngine.Object.Instantiate(m_BannerSlotPrefab);
		case InventoryItemType.BannerEmblem:
			return UnityEngine.Object.Instantiate(m_EmblemSlotPrefab);
		case InventoryItemType.BannerTip:
			return UnityEngine.Object.Instantiate(m_TipSlotPrefab);
		default:
			return UnityEngine.Object.Instantiate(m_TipSlotPrefab);
		}
	}

	private void DeRegisterEventHandlerFromSlot(InventoryItemSlot slot)
	{
		slot.OnScrap -= OnSlotToScrap;
		slot.OnSelected -= OnSlotSelected;
		slot.OnUsed -= OnSlotUsed;
	}

	private void RegisterEventHandlerFromSlot(InventoryItemSlot slot)
	{
		DeRegisterEventHandlerFromSlot(slot);
		slot.OnScrap += OnSlotToScrap;
		slot.OnSelected += OnSlotSelected;
		slot.OnUsed += OnSlotUsed;
	}

	private void OnSlotUsed(InventoryItemSlot slot)
	{
		slot.SetUsed(true);
		StartCoroutine(SelectSlot(slot));
	}

	private IEnumerator SelectSlot(InventoryItemSlot slot)
	{
		Vector3 offset = Vector3.zero;
		Transform root = null;
		BannerAssetController bannerAsset = m_BannerEquipmentPreviewUI.m_CharacterController.m_AssetController as BannerAssetController;
		if ((bool)bannerAsset)
		{
			switch (m_CurrentItemType)
			{
			case InventoryItemType.Banner:
				root = bannerAsset.m_BannerFlagRoot;
				break;
			case InventoryItemType.BannerEmblem:
				root = bannerAsset.m_BannerFlagRoot;
				break;
			case InventoryItemType.BannerTip:
				root = bannerAsset.m_BannerTipRoot;
				break;
			}
		}
		Transform parentBefore = slot.transform.parent;
		yield return new WaitForSeconds(slot.FlyToTransform(root, offset));
		yield return new WaitForEndOfFrame();
		if (!slot || slot.GetModel() == null || m_CurrentItemType != slot.GetModel().ItemBalancing.ItemType)
		{
			yield break;
		}
		slot.ResetFromFly();
		OnSlotSelected(slot);
		StartCoroutine(RefreshItemInfo(true));
		if ((bool)m_SoundTriggers)
		{
			m_SoundTriggers.OnTriggerEventFired("item_equipped");
		}
		yield return new WaitForEndOfFrame();
		if ((bool)bannerAsset)
		{
			switch (m_CurrentItemType)
			{
			case InventoryItemType.BannerTip:
				bannerAsset.PlayFocusTipAnim();
				break;
			case InventoryItemType.Banner:
				bannerAsset.PlayFocusBannerAnim();
				break;
			case InventoryItemType.BannerEmblem:
				bannerAsset.PlayFocusEmblemAnim();
				break;
			}
		}
		TrackBannerSetAchievement(slot.GetModel() as BannerItemGameData);
	}

	private void TrackBannerSetAchievement(BannerItemGameData bannerItem)
	{
		AchievementData achievementTracking = DIContainerInfrastructure.GetCurrentPlayer().Data.AchievementTracking;
		if (m_CurrentItemType != InventoryItemType.BannerEmblem && !achievementTracking.BannerSetCompleted && bannerItem.IsSetCompleted(DIContainerInfrastructure.GetCurrentPlayer().BannerGameData))
		{
			string achievementIdForStoryItemIfExists = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("completeBannerSet");
			if (!string.IsNullOrEmpty(achievementIdForStoryItemIfExists))
			{
				DIContainerInfrastructure.GetAchievementService().ReportUnlocked(achievementIdForStoryItemIfExists);
				achievementTracking.BannerSetCompleted = true;
			}
		}
	}

	private void OnSlotSelected(InventoryItemSlot slot)
	{
		m_SelectedItem = slot.GetModel();
		DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { m_SelectedItem }, m_CurrentItemType, m_Banner.InventoryGameData);
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		if ((bool)m_SelectedSlot)
		{
			m_SelectedSlot.Deselect();
		}
		m_SelectedSlot = slot;
		m_SelectedSlot.Select();
		foreach (InventoryItemSlot itemSlot in m_ItemSlots)
		{
			itemSlot.RefreshStat();
		}
		OpenInventoryButton openInventoryButton = m_OpenInventoryButtons.FirstOrDefault((OpenInventoryButton o) => o.m_ItemType == m_SelectedSlot.GetModel().ItemBalancing.ItemType);
		if (m_Inventory.HasNewItemBanner(openInventoryButton.m_ItemType, m_Banner))
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
		SetItemInfo();
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, HandleBackButton);
		m_ButtonClose.Clicked += m_ButtonClose_Clicked;
		m_EnchantItemButton.Clicked += OpenEnchantmentUi;
		m_ScrapItemButton.Clicked += m_ScrapItemButton_Clicked;
		m_ScrapItemButtonEnch.Clicked += m_ScrapItemButton_Clicked;
		RegisterCategoryButtons();
		m_Inventory.InventoryOfTypeChanged -= OnInventoryOfTypeChanged;
		m_Inventory.InventoryOfTypeChanged += OnInventoryOfTypeChanged;
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
			if (m_ScrapItemRootEnch.gameObject.activeSelf)
			{
				yield return new WaitForSeconds(destroyed.FlyToTransform(m_ScrapItemRootEnch.transform, Vector3.zero));
			}
			else
			{
				yield return new WaitForSeconds(destroyed.FlyToTransform(m_ScrapItemRoot.transform, Vector3.zero));
			}
			yield return new WaitForEndOfFrame();
			destroyed.RemoveAssets();
			if (scrapLoot.Count > 0)
			{
				LootDisplayContoller item2 = null;
				item2 = ((!m_ScrapItemButtonEnch.gameObject.activeSelf) ? (UnityEngine.Object.Instantiate(m_LootForExplosionPrefab, m_ScrapItemButton.transform.position, Quaternion.identity) as LootDisplayContoller) : (UnityEngine.Object.Instantiate(m_LootForExplosionPrefab, m_ScrapItemButtonEnch.transform.position, Quaternion.identity) as LootDisplayContoller));
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
				explosionFX2 = ((!m_ScrapItemButtonEnch.gameObject.activeSelf) ? (UnityEngine.Object.Instantiate(m_ExplodedFXPrefab, m_ScrapItemButton.transform.position, Quaternion.identity) as GameObject) : (UnityEngine.Object.Instantiate(m_ExplodedFXPrefab, m_ScrapItemButtonEnch.transform.position, Quaternion.identity) as GameObject));
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
		m_EnchantItemButton.Clicked -= OpenEnchantmentUi;
		m_ScrapItemButton.Clicked -= m_ScrapItemButton_Clicked;
		m_ScrapItemButtonEnch.Clicked -= m_ScrapItemButton_Clicked;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(2);
		m_EnchantItemButton.Clicked -= OpenEnchantmentUi;
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
		m_HeaderAnimation.Play("Header_Leave");
		m_ItemCategoryButtonsAnimation.Play("Categories_Leave");
		yield return StartCoroutine(m_BannerEquipmentPreviewUI.Leave());
		m_StateMgr.m_EnchantmentUi.EnterBanner(m_SelectedItem as BannerItemGameData, this, m_StateMgr.m_EnchantmentPopup);
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
		StartCoroutine(ReEnterAfterEnchanting());
	}

	private IEnumerator ReEnterAfterEnchanting()
	{
		StartCoroutine(m_BannerEquipmentPreviewUI.Enter());
		yield return StartCoroutine(PlayEnterAnimation());
		SetItemListContent();
		SelectDefaultSlot(false);
		yield return StartCoroutine(RestorePosition(m_ItemPanel, m_SelectedSlot.transform, m_ItemGrid));
	}
}

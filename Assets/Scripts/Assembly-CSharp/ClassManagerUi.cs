using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using UnityEngine;

public class ClassManagerUi : MonoBehaviour
{
	[Header("Generic Stuff")]
	[SerializeField]
	private BirdEquipmentPreviewUI m_BirdEquipmentPreviewUI;

	[SerializeField]
	public UIInputTrigger m_ButtonClose;

	[Header("Footer")]
	[SerializeField]
	public ClassItemInfo m_ClassInfo;

	[SerializeField]
	private UIInputTrigger m_ClassInfoButton;

	[SerializeField]
	private BuyClassItemInfo m_PremiumClassBuyInfo;

	[Header("Button List")]
	[SerializeField]
	private UIGrid m_ItemGrid;

	[SerializeField]
	private UIScrollView m_ItemPanel;

	[SerializeField]
	private InventoryItemSlot ClassSlotPrefab;

	[Header("Bird Button Tabs")]
	[SerializeField]
	private List<BirdTabButton> m_selectBirdButtons;

	[Header("Animations")]
	[SerializeField]
	private Animation m_HeaderAnimation;

	[SerializeField]
	private Animation m_ItemCategoryButtonsAnimation;

	[SerializeField]
	private Animation m_ItemGridAnimation;

	[SerializeField]
	private Animation m_ItemInfoAnimation;

	private bool m_IsRefreshing;

	private bool m_Entered;

	private bool m_finishedSpring;

	private Color m_colorWhite = new Color(1f, 1f, 1f);

	private Color m_colorDarkGreen = new Color(0.5f, 1f, 0f);

	private BirdGameData m_SelectedBird;

	private bool m_newPremium;

	private bool m_alreadyEquipped;

	private bool m_isPvp;

	private BattlePreperationUI m_bps;

	private bool m_updateAnimBlocked;

	private bool m_switchBirdsBlocked;

	private InventoryItemSlot m_selectedSlot;

	private ClassItemGameData m_selectedClass;

	private List<InventoryItemSlot> m_ItemSlots = new List<InventoryItemSlot>();

	[HideInInspector]
	public InventoryItemSlot SelectedSlot
	{
		get
		{
			return m_selectedSlot;
		}
	}

	public void UpdateSlotIndicators()
	{
		List<BirdGameData> birds = DIContainerInfrastructure.GetCurrentPlayer().Birds;
		BirdTabButton oib;
		foreach (BirdTabButton selectBirdButton in m_selectBirdButtons)
		{
			oib = selectBirdButton;
			if (DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.HasNewItemBird(InventoryItemType.Class, birds.FirstOrDefault((BirdGameData b) => b.Name == oib.m_BirdName)))
			{
				if ((bool)oib && oib.gameObject.activeSelf)
				{
					oib.m_NewMarker.SetActive(true);
				}
			}
			else if ((bool)oib)
			{
				oib.m_NewMarker.SetActive(false);
			}
		}
	}

	public void RefreshAll()
	{
		m_newPremium = false;
		StartCoroutine(InitializeBirdWindowUi());
	}

	public void ReEnterFromShop()
	{
		base.gameObject.SetActive(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 8u,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false,
			showEnergy = false
		}, true);
		StartCoroutine(InitializeBirdWindowUi());
	}

	public void EnterClassManager(bool arena, BattlePreperationUI bps)
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 8u,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false,
			showEnergy = false
		}, true);
		m_isPvp = arena;
		m_bps = bps;
		base.gameObject.SetActive(true);
		StartCoroutine(InitializeBirdWindowUi());
	}

	private IEnumerator InitializeBirdWindowUi()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("class_manager_enter");
		List<BirdGameData> birds = DIContainerInfrastructure.GetCurrentPlayer().Birds;
		ActivateBirdTabs();
		if (m_SelectedBird == null)
		{
			m_SelectedBird = birds.FirstOrDefault();
		}
		m_selectedClass = m_SelectedBird.ClassItem;
		m_newPremium = false;
		base.gameObject.SetActive(true);
		m_BirdEquipmentPreviewUI.SetModels(birds);
		SetItemListContent();
		UpdateSlotIndicators();
		SelectDefaultSlot(false);
		SetItemInfo();
		yield return StartCoroutine(EnterItemList());
		m_BirdEquipmentPreviewUI.SetCharacter(m_SelectedBird);
		yield return StartCoroutine(RestorePosition());
		yield return StartCoroutine(m_BirdEquipmentPreviewUI.Enter());
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("class_manager_enter");
		m_Entered = true;
		RegisterEventHandler();
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("bps_classmanager_entered", string.Empty);
	}

	private void ActivateBirdTabs()
	{
		List<BirdGameData> birds = DIContainerInfrastructure.GetCurrentPlayer().Birds;
		BirdTabButton btb;
		foreach (BirdTabButton selectBirdButton in m_selectBirdButtons)
		{
			btb = selectBirdButton;
			if (!birds.Any((BirdGameData b) => b.BalancingData.NameId == btb.m_BirdName))
			{
				btb.m_BirdShadowObject.SetActive(true);
				btb.SetInactive();
			}
		}
	}

	private IEnumerator RestorePosition()
	{
		m_ItemPanel.DisableSpring();
		m_ItemPanel.ResetPosition();
		yield return new WaitForEndOfFrame();
		m_ItemGrid.Reposition();
		yield return new WaitForEndOfFrame();
		if (m_ItemPanel.shouldMoveHorizontally)
		{
			m_ItemPanel.MoveAbsolute(-Vector3.Scale(m_selectedSlot.transform.localPosition + m_ItemPanel.transform.localPosition - new Vector3(m_ItemPanel.panel.clipRange.z / 2f - m_ItemGrid.cellWidth / 2f, 0f, 0f), new Vector3(1f, 0f, 0f)));
		}
		else
		{
			m_ItemPanel.ResetPosition();
		}
		yield return new WaitForEndOfFrame();
		m_ItemPanel.RestrictWithinBounds(true);
	}

	private IEnumerator EnterItemList()
	{
		yield return new WaitForEndOfFrame();
		SelectBirdButton();
		PlayEnterAnimation();
	}

	private void PlayEnterAnimation()
	{
		m_BirdEquipmentPreviewUI.GetComponent<Animation>().Play("CharacterDisplay_Enter");
		m_HeaderAnimation.Play("Header_Enter");
		m_ItemCategoryButtonsAnimation.Play("Categories_Enter");
	}

	private IEnumerator PlayLeaveAnimation(bool reEnterBattlePreparations = true)
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("class_manager_leave");
		m_HeaderAnimation.Play("Header_Leave");
		m_ItemCategoryButtonsAnimation.Play("Categories_Leave");
		yield return StartCoroutine(m_BirdEquipmentPreviewUI.Leave());
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(8u);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("class_manager_leave");
		if (reEnterBattlePreparations)
		{
			m_bps.CreateBirds();
			m_bps.Enter(true);
		}
		base.gameObject.SetActive(false);
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
		if (!m_IsRefreshing)
		{
			m_IsRefreshing = true;
			if (showUpdateAnim)
			{
				yield return StartCoroutine(PlayGridChangeAnimation(false));
			}
			SetItemListContent();
			SetItemInfo();
			if (showUpdateAnim)
			{
				m_BirdEquipmentPreviewUI.RefreshStats(true);
			}
			SelectDefaultSlot(false);
			yield return StartCoroutine(RestorePosition());
			SelectDefaultSlot(false);
			if (showUpdateAnim)
			{
				yield return StartCoroutine(PlayGridChangeAnimation(true));
			}
			m_IsRefreshing = false;
			RegisterEventHandler();
			DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("bird_manager_refresh");
		}
	}

	private void RefreshItemInfo()
	{
		m_updateAnimBlocked = false;
		SetItemInfo();
		if (!m_alreadyEquipped && !m_newPremium)
		{
			m_BirdEquipmentPreviewUI.RefreshStats(true);
		}
	}

	private void SetItemInfo()
	{
		if (m_newPremium)
		{
			m_ClassInfo.gameObject.SetActive(false);
			m_PremiumClassBuyInfo.gameObject.SetActive(true);
			m_PremiumClassBuyInfo.m_ClassMgr = this;
			m_PremiumClassBuyInfo.SetModel(m_selectedClass, m_SelectedBird);
		}
		else
		{
			m_ClassInfo.gameObject.SetActive(true);
			m_PremiumClassBuyInfo.gameObject.SetActive(false);
			m_ClassInfo.m_ClassMgr = this;
			m_ClassInfo.SetModel(m_selectedClass, m_SelectedBird, m_BirdEquipmentPreviewUI, m_selectedSlot);
		}
	}

	private void SelectDefaultSlot(bool playUpdateAnim)
	{
		m_newPremium = false;
		m_selectedClass = m_SelectedBird.ClassItem;
		InventoryItemSlot inventoryItemSlot = m_ItemSlots.FirstOrDefault((InventoryItemSlot s) => s.GetModel().ItemBalancing.NameId.Equals(m_selectedClass.ItemBalancing.NameId));
		if (!inventoryItemSlot)
		{
			inventoryItemSlot = m_ItemSlots.FirstOrDefault((InventoryItemSlot c) => c.GetModel().ItemBalancing is ClassItemBalancingData && !(c.GetModel().ItemBalancing as ClassItemBalancingData).IsPremium);
		}
		if ((bool)m_selectedSlot)
		{
			m_selectedSlot.Deselect(m_newPremium);
		}
		SelectSlot(inventoryItemSlot);
	}

	private void SelectSlot(InventoryItemSlot inventoryItemSlot)
	{
		if (!inventoryItemSlot)
		{
			return;
		}
		if ((bool)m_selectedSlot)
		{
			m_selectedSlot.Deselect(m_newPremium);
		}
		foreach (InventoryItemSlot itemSlot in m_ItemSlots)
		{
			if (itemSlot != inventoryItemSlot)
			{
				itemSlot.RemoveLeftOverSelection();
			}
		}
		m_updateAnimBlocked = true;
		inventoryItemSlot.SelectItemData();
		if (!m_selectedSlot)
		{
			return;
		}
		m_selectedSlot.Select(m_newPremium);
		m_selectedSlot.SetUsed(true);
		foreach (InventoryItemSlot itemSlot2 in m_ItemSlots)
		{
			itemSlot2.RefreshStat();
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
		Transform root = m_BirdEquipmentPreviewUI.m_CurrentCharacterController.m_AssetController.HeadGearBone;
		if (slot != null && slot.GetModel() != null)
		{
			m_selectedClass = slot.GetModel() as ClassItemGameData;
		}
		ClassItemBalancingData classData = m_selectedClass.BalancingData;
		m_newPremium = classData.IsPremium && !DIContainerLogic.InventoryService.CheckForItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, classData.NameId);
		foreach (BirdGameData bird in DIContainerInfrastructure.GetCurrentPlayer().Birds)
		{
			if (bird.BalancingData.NameId == classData.RestrictedBirdId)
			{
				m_alreadyEquipped = bird.ClassItem.BalancingData.NameId == classData.NameId;
				break;
			}
		}
		if (!m_newPremium && !m_alreadyEquipped)
		{
			m_switchBirdsBlocked = true;
			yield return new WaitForSeconds(slot.FlyToTransform(root, offset));
		}
		yield return new WaitForEndOfFrame();
		m_switchBirdsBlocked = false;
		if ((bool)slot && slot.GetModel() != null)
		{
			slot.ResetFromFly();
			OnSlotSelected(slot);
			yield return new WaitForEndOfFrame();
			if (!m_newPremium && !m_alreadyEquipped)
			{
				m_BirdEquipmentPreviewUI.m_CurrentCharacterController.m_AssetController.PlayCheerAnim();
				m_BirdEquipmentPreviewUI.PlayCharacterChanged();
			}
		}
	}

	private void SetItemListContent()
	{
		List<ClassItemBalancingData> list = (from c in DIContainerBalancing.Service.GetBalancingDataList<ClassItemBalancingData>()
			where c.RestrictedBirdId == m_SelectedBird.BalancingData.NameId
			select c).ToList();
		List<IInventoryItemGameData> list2 = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.Class].Where((IInventoryItemGameData i) => i.IsValidForBird(m_SelectedBird)).ToList();
		for (int j = 0; j < list2.Count; j++)
		{
			for (int num = list.Count - 1; num >= 0; num--)
			{
				ClassItemBalancingData classItemBalancingData = list[num];
				ClassItemGameData classItemGameData = list2[j] as ClassItemGameData;
				if (classItemBalancingData == null || !string.IsNullOrEmpty(classItemBalancingData.ReplacementClassNameId) || list2[j].ItemBalancing.NameId == list[num].NameId || (classItemGameData != null && classItemGameData.BalancingData.ReplacementClassNameId == list[num].NameId))
				{
					list.RemoveAt(num);
				}
				IInventoryItemGameData inventoryItemGameData = new ClassItemGameData(classItemBalancingData.NameId);
				foreach (IInventoryItemGameData item in list2)
				{
					if (item.Name == inventoryItemGameData.Name)
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
		list2 = list2.OrderBy((IInventoryItemGameData d) => d.ItemBalancing.SortPriority).ToList();
		for (int num2 = m_ItemSlots.Count - 1; num2 >= 0; num2--)
		{
			InventoryItemSlot inventoryItemSlot = m_ItemSlots[num2];
			DeRegisterEventHandlerFromSlot(inventoryItemSlot);
			m_ItemSlots.Remove(inventoryItemSlot);
			Object.Destroy(inventoryItemSlot.gameObject);
		}
		SetupButtons(list, list2);
	}

	private void SetupButtons(List<ClassItemBalancingData> allClasses, List<IInventoryItemGameData> playerClasses)
	{
		int num = 0;
		for (int i = 0; i < playerClasses.Count; i++)
		{
			IInventoryItemGameData inventoryItemGameData = playerClasses[i];
			InventoryItemSlot inventoryItemSlot = Object.Instantiate(ClassSlotPrefab);
			inventoryItemSlot.name = (i + 1).ToString("000") + inventoryItemGameData.ItemBalancing.SortPriority.ToString("00") + "_" + inventoryItemSlot.name;
			m_ItemSlots.Add(inventoryItemSlot);
			inventoryItemSlot.transform.parent = m_ItemGrid.transform;
			inventoryItemSlot.transform.localPosition = Vector3.zero;
			inventoryItemSlot.SetModel(inventoryItemGameData, m_isPvp);
			DeRegisterEventHandlerFromSlot(inventoryItemSlot);
			RegisterEventHandlerFromSlot(inventoryItemSlot);
			num++;
		}
		for (int j = 0; j < allClasses.Count; j++)
		{
			ClassItemGameData classItemGameData = new ClassItemGameData(allClasses[j].NameId);
			InventoryItemSlot inventoryItemSlot2 = Object.Instantiate(ClassSlotPrefab);
			if (classItemGameData.BalancingData.IsPremium && classItemGameData.ItemData.Level == 0 && DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "unlock_mastery_badge") > 0)
			{
				DIContainerInfrastructure.GetCurrentPlayer().AdvanceBirdMasteryToHalfOfHighest(classItemGameData);
			}
			inventoryItemSlot2.name = (num + 1).ToString("000") + classItemGameData.ItemBalancing.SortPriority.ToString("00") + "_" + inventoryItemSlot2.name;
			m_ItemSlots.Add(inventoryItemSlot2);
			inventoryItemSlot2.transform.parent = m_ItemGrid.transform;
			inventoryItemSlot2.transform.localPosition = Vector3.zero;
			inventoryItemSlot2.SetModel(classItemGameData, m_isPvp);
			if (classItemGameData.BalancingData.IsPremium)
			{
				inventoryItemSlot2.m_purchaseIndicator.SetActive(true);
				bool flag = DIContainerLogic.GetSalesManagerService().IsItemOnSale(classItemGameData.BalancingData.NameId);
				inventoryItemSlot2.m_purchaseIndicatorBody.color = ((!flag) ? m_colorWhite : m_colorDarkGreen);
			}
			else
			{
				inventoryItemSlot2.SetSlotUnavailable();
			}
			DeRegisterEventHandlerFromSlot(inventoryItemSlot2);
			RegisterEventHandlerFromSlot(inventoryItemSlot2);
			num++;
		}
	}

	private void DeRegisterEventHandlerFromSlot(InventoryItemSlot slot)
	{
		slot.OnUsed -= OnSlotUsed;
		slot.OnSelected -= OnSlotSelected;
		if ((bool)m_ClassInfoButton)
		{
			m_ClassInfoButton.Clicked -= OpenClassInfo;
		}
	}

	private void RegisterEventHandlerFromSlot(InventoryItemSlot slot)
	{
		DeRegisterEventHandlerFromSlot(slot);
		slot.OnUsed += OnSlotUsed;
		slot.OnSelected += OnSlotSelected;
		if ((bool)m_ClassInfoButton)
		{
			m_ClassInfoButton.Clicked += OpenClassInfo;
		}
	}

	private void OnSlotSelected(InventoryItemSlot slot)
	{
		m_selectedClass = slot.GetModel() as ClassItemGameData;
		if (!m_newPremium)
		{
			DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { m_selectedClass }, InventoryItemType.Class, m_SelectedBird.InventoryGameData);
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			foreach (InventoryItemSlot itemSlot in m_ItemSlots)
			{
				if (itemSlot != slot)
				{
					itemSlot.RemoveLeftOverSelection();
				}
			}
		}
		if ((bool)m_selectedSlot)
		{
			m_selectedSlot.Deselect(m_newPremium);
		}
		m_selectedSlot = slot;
		m_selectedSlot.Select(m_newPremium);
		foreach (InventoryItemSlot itemSlot2 in m_ItemSlots)
		{
			itemSlot2.RefreshStat();
		}
		RefreshItemInfo();
		UpdateSlotIndicators();
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, HandleBackButton);
		m_ButtonClose.Clicked += m_ButtonClose_Clicked;
		RegisterCategoryButtons();
	}

	private void DeRegisterCategoryButtons()
	{
		foreach (BirdTabButton selectBirdButton in m_selectBirdButtons)
		{
			selectBirdButton.OnButtonClicked -= OnOpenInventoryButtonClicked;
		}
	}

	private void RegisterCategoryButtons()
	{
		DeRegisterCategoryButtons();
		foreach (BirdTabButton selectBirdButton in m_selectBirdButtons)
		{
			selectBirdButton.OnButtonClicked += OnOpenInventoryButtonClicked;
		}
	}

	private void OnOpenInventoryButtonClicked(string birdName)
	{
		if (!(m_SelectedBird.Name == birdName))
		{
			m_SelectedBird = DIContainerInfrastructure.GetCurrentPlayer().Birds.FirstOrDefault((BirdGameData b) => b.BalancingData.NameId == birdName);
			m_BirdEquipmentPreviewUI.SetCharacter(m_SelectedBird);
			for (int i = 0; i < m_ItemSlots.Count; i++)
			{
				m_ItemSlots[i].SetIsNew(false);
			}
			DeRegisterEventHandler(false);
			SelectBirdButton();
			StartCoroutine(RefreshItemList());
		}
	}

	private void SelectBirdButton()
	{
		foreach (BirdTabButton selectBirdButton in m_selectBirdButtons)
		{
			selectBirdButton.Activate(selectBirdButton.m_BirdName == m_SelectedBird.BalancingData.NameId);
		}
	}

	private void DeRegisterEventHandler(bool deregisterSlots = true)
	{
		m_ButtonClose.Clicked -= m_ButtonClose_Clicked;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(2);
		DeRegisterCategoryButtons();
		if (!deregisterSlots)
		{
			return;
		}
		foreach (InventoryItemSlot itemSlot in m_ItemSlots)
		{
			itemSlot.OnUsed -= OnSlotSelected;
		}
	}

	private void m_ButtonClose_Clicked()
	{
		Leave();
	}

	public void Leave(bool reEnterBattlePreparations = true)
	{
		for (int i = 0; i < m_ItemSlots.Count; i++)
		{
			m_ItemSlots[i].SetIsNew(false);
		}
		StartCoroutine(PlayLeaveAnimation(reEnterBattlePreparations));
		m_Entered = false;
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
		DIContainerInfrastructure.GetCoreStateMgr().ShowClassInfoUi(m_selectedClass, m_SelectedBird.ClassSkin);
	}
}

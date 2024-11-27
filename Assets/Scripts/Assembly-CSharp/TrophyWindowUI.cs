using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Character;
using UnityEngine;

public class TrophyWindowUI : BirdWindowUIBase
{
	[SerializeField]
	private InventoryItemSlot m_TrophylotPrefab;

	[SerializeField]
	private TrophyItemInfo m_TrophyItemInfo;

	[SerializeField]
	private TrophyPreviewUI m_TrophyPreviewUI;

	[SerializeField]
	private UILabel m_HeaderNameAndClassName;

	private List<IInventoryItemGameData> m_GameDatas = new List<IInventoryItemGameData>();

	private TrophyData m_Trophy;

	private InventoryGameData m_Inventory;

	private InventoryItemType m_CurrentItemType = InventoryItemType.Trophy;

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
	private UILabel m_borderLabelHeader;

	[SerializeField]
	private UILabel m_borderLabelDescription;

	[SerializeField]
	private UISprite m_avatarBorder;

	[SerializeField]
	private UITexture m_playerAvatar;

	private WWW m_OpponentTextureDownload;

	private Texture2D m_OpponentTexture;

	[SerializeField]
	private SoundTriggerList m_SoundTriggers;

	private bool m_IsRefreshing;

	private int m_SelectedBirdIndex;

	private bool m_Entered;

	private int m_ScrappingLocks;

	private bool m_finishedSpring;

	private bool m_EnteredOnce;

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

	public override void UpdateSlotIndicators()
	{
	}

	public override UIGrid getItemGrid()
	{
		return m_ItemGrid;
	}

	public void SetModel(InventoryGameData inventory, TrophyData currentEquippedTrophy, InventoryItemType defaultItemType = InventoryItemType.Trophy)
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Enter();
		base.gameObject.SetActive(true);
		m_Inventory = inventory;
		m_Trophy = currentEquippedTrophy;
		if (!m_EnteredOnce)
		{
			m_EnteredOnce = true;
		}
		StartCoroutine(InitializeTrophyWindowUI());
	}

	private IEnumerator InitializeTrophyWindowUI()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("trophy_manager_animate");
		yield return new WaitForEndOfFrame();
		SetItemListContent();
		UIPanel[] panels = base.gameObject.GetComponentsInChildren<UIPanel>();
		UIPanel[] array = panels;
		foreach (UIPanel p in array)
		{
			if (p != null)
			{
				p.enabled = true;
			}
		}
		SelectDefaultSlot(false);
		SetItemInfo();
		yield return StartCoroutine(EnterItemList());
		StartCoroutine(SetHeader());
		if (m_SelectedSlot != null)
		{
			yield return StartCoroutine(RestorePosition(m_ItemPanel, m_SelectedSlot.transform, m_ItemGrid));
		}
		if (m_TrophyPreviewUI != null)
		{
			m_TrophyPreviewUI.SetModel(this.m_Trophy);
			UpdateBorder();
			yield return StartCoroutine(m_TrophyPreviewUI.Enter());
		}
		if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData != null && DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data != null && !string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.SocialPictureUrl))
		{
			StartCoroutine(this.LoadTexture());
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("trophy_manager_animate");
        m_Entered = true;
        RegisterEventHandler();
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
			panel.MoveAbsolute(-Vector3.Scale(targetTransform.transform.localPosition + panel.transform.localPosition - new Vector3(panel.panel.clipRange.z / 2f - containingGrid.cellWidth / 2f, 0f, 0f), new Vector3(1f, 0f, 0f)));
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
		StartCoroutine(PlayEnterAnimation());
	}

	private IEnumerator PlayEnterAnimation()
	{
		m_ItemInfoAnimation.Play("ItemInfo_Enter");
		m_HeaderAnimation.Play("Header_Enter");
		m_ItemCategoryButtonsAnimation.Play("Categories_Enter");
		yield break;
	}

	private IEnumerator PlayLeaveAnimation()
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("trophy_manager_animate");
		yield return new WaitForEndOfFrame();
		(m_StateMgr as ArenaCampStateMgr).RefreshTrophy();
		m_ItemInfoAnimation.Play("ItemInfo_Leave");
		m_HeaderAnimation.Play("Header_Leave");
		m_ItemCategoryButtonsAnimation.Play("Categories_Leave");
		yield return StartCoroutine(m_TrophyPreviewUI.Leave());
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("trophy_manager_animate");
		base.gameObject.SetActive(false);
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave();
	}

	private IEnumerator RefreshItemList()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("trophy_manager_refresh");
		if (!m_IsRefreshing)
		{
			m_IsRefreshing = true;
			SetItemListContent();
			SetItemInfo();
			SelectDefaultSlot(false);
			yield return StartCoroutine(RestorePosition(m_ItemPanel, m_SelectedSlot.transform, m_ItemGrid));
			StartCoroutine(SetHeader());
			SelectDefaultSlot(false);
			m_IsRefreshing = false;
			RegisterEventHandler();
			DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("trophy_manager_refresh");
		}
	}

	private IEnumerator SetHeader()
	{
		m_HeaderAnimation.Stop();
		m_HeaderAnimation.Play("Header_Change_Out");
		yield return new WaitForSeconds(m_HeaderAnimation["Header_Change_Out"].clip.length);
		m_HeaderNameAndClassName.text = DIContainerInfrastructure.GetLocaService().GetInventoryItemTypeName(m_CurrentItemType);
		m_HeaderAnimation.Play("Header_Change_In");
		yield return new WaitForSeconds(m_HeaderAnimation["Header_Change_In"].clip.length);
	}

	private void SetItemInfo()
	{
		m_TrophyItemInfo.gameObject.SetActive(true);
		m_TrophyItemInfo.SetModel(m_Trophy);
	}

	public void ShowTooltip()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowTrophyOverlay(base.transform, DIContainerInfrastructure.GetCurrentPlayer().Data.PvPTrophy, true);
	}

	public void HideAllTooltips()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
	}

	private void SelectDefaultSlot(bool playUpdateAnim)
	{
		InventoryItemSlot inventoryItemSlot = m_ItemSlots.FirstOrDefault((InventoryItemSlot s) => s.name.Equals(m_Trophy.NameId));
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
		if ((bool)inventoryItemSlot)
		{
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
			}
		}
	}

	private void SetItemListContent()
	{
		m_GameDatas = m_Inventory.Items[m_CurrentItemType].OrderByDescending((IInventoryItemGameData d) => d.ItemMainStat).ToList();
		for (int num = m_ItemSlots.Count - 1; num >= 0; num--)
		{
			InventoryItemSlot inventoryItemSlot = m_ItemSlots[num];
			DeRegisterEventHandlerFromSlot(inventoryItemSlot);
			m_ItemSlots.Remove(inventoryItemSlot);
			Object.Destroy(inventoryItemSlot.gameObject);
		}
		int num2 = 0;
		for (int i = 0; i < m_GameDatas.Count; i++)
		{
			IInventoryItemGameData inventoryItemGameData = m_GameDatas[i];
			if (inventoryItemGameData.ItemData.Level < DIContainerBalancing.EventBalancingService.GetBalancingDataList<PvPSeasonManagerBalancingData>().Count)
			{
				InventoryItemSlot inventoryItemSlot2 = Object.Instantiate(m_TrophylotPrefab);
				TrophyData trophyFromItem = GetTrophyFromItem(inventoryItemGameData);
				inventoryItemSlot2.name = trophyFromItem.NameId;
				inventoryItemSlot2.Trophy = trophyFromItem;
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
	}

	private TrophyData GetTrophyFromItem(IInventoryItemGameData item)
	{
		TrophyData trophyData = new TrophyData();
		string nameId = string.Empty;
		int level = item.ItemData.Level;
		int quality = item.ItemData.Quality;
		switch (quality)
		{
		case 1:
			nameId = "Season" + ((level - 1) % 5 + 1) + "Wood";
			break;
		case 2:
			nameId = "Season" + ((level - 1) % 5 + 1) + "Stone";
			break;
		case 3:
			nameId = "Season" + ((level - 1) % 5 + 1) + "Silver";
			break;
		case 4:
			nameId = "Season" + ((level - 1) % 5 + 1) + "Gold";
			break;
		case 5:
			nameId = "Season" + ((level - 1) % 5 + 1) + "Platinum";
			break;
		case 6:
			nameId = "Season" + ((level - 1) % 5 + 1) + "Diamond";
			break;
		}
		trophyData.NameId = nameId;
		trophyData.Seasonid = level;
		trophyData.FinishedLeagueId = quality;
		return trophyData;
	}

	private void DeRegisterEventHandlerFromSlot(InventoryItemSlot slot)
	{
		slot.OnSelected -= OnSlotSelected;
		slot.OnUsed -= OnSlotUsed;
	}

	private void RegisterEventHandlerFromSlot(InventoryItemSlot slot)
	{
		DeRegisterEventHandlerFromSlot(slot);
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
		Vector3 offset = new Vector3(160f, 0f, 0f);
		Transform root = m_TrophyPreviewUI.transform.GetChild(0);
		Transform parentBefore = slot.transform.parent;
		yield return new WaitForSeconds(slot.FlyToTransform(root, offset));
		yield return new WaitForEndOfFrame();
		if ((bool)slot && slot.GetModel() != null && m_CurrentItemType == slot.GetModel().ItemBalancing.ItemType)
		{
			slot.ResetFromFly();
			OnSlotSelected(slot);
			if ((bool)m_SoundTriggers)
			{
				m_SoundTriggers.OnTriggerEventFired("item_equipped");
			}
		}
	}

	private void OnSlotSelected(InventoryItemSlot slot)
	{
		m_SelectedItem = slot.GetModel();
		if (slot.Trophy != null)
		{
			m_Trophy = slot.Trophy;
			m_TrophyPreviewUI.SetModel(m_Trophy);
			UpdateBorder();
			DIContainerInfrastructure.GetCurrentPlayer().Data.PvPTrophy = slot.Trophy;
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		}
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
		m_UpdateAnimBlocked = false;
		SetItemInfo();
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, HandleBackButton);
		m_ButtonClose.Clicked += m_ButtonClose_Clicked;
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
			StartCoroutine(RefreshItemList());
		}
	}

	private void DeRegisterEventHandler(bool deregisterSlots = true)
	{
		m_ButtonClose.Clicked -= m_ButtonClose_Clicked;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(2);
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

	private IEnumerator LoadTexture()
	{
		if (m_OpponentTextureDownload == null)
		{
			m_OpponentTextureDownload = new WWW(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.SocialPictureUrl);
		}
		while (m_OpponentTextureDownload != null && !m_OpponentTextureDownload.isDone)
		{
			yield return new WaitForSeconds(0.5f);
		}
		if (string.IsNullOrEmpty(m_OpponentTextureDownload.error))
		{
			m_playerAvatar.mainTexture = m_OpponentTextureDownload.texture;
			DIContainerInfrastructure.GetCoreStateMgr().SaveSocialAvatar(m_OpponentTextureDownload, string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.SocialId) ? DIContainerInfrastructure.IdentityService.SharedId : DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.SocialId);
		}
		else
		{
			m_playerAvatar.mainTexture = DIContainerInfrastructure.GetCoreStateMgr().LoadSocialAvatar(string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.SocialId) ? DIContainerInfrastructure.IdentityService.SharedId : DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.SocialId);
		}
	}

	private void UpdateBorder()
	{
		switch (m_Trophy.FinishedLeagueId)
		{
		case 1:
			m_avatarBorder.spriteName = "WoodLeague";
			break;
		case 2:
			m_avatarBorder.spriteName = "StoneLeague";
			break;
		case 3:
			m_avatarBorder.spriteName = "SilverLeague";
			break;
		case 4:
			m_avatarBorder.spriteName = "GoldLeague";
			break;
		case 5:
			m_avatarBorder.spriteName = "PlatinumLeague";
			break;
		case 6:
			m_avatarBorder.spriteName = "DiamondLeague";
			break;
		}
		m_avatarBorder.MakePixelPerfect();
		string text = "s" + m_Trophy.Seasonid.ToString("00") + "_l" + m_Trophy.FinishedLeagueId.ToString("00");
		m_borderLabelDescription.text = DIContainerInfrastructure.GetLocaService().Tr("avatar_border_" + text + "_desc");
		m_borderLabelHeader.text = DIContainerInfrastructure.GetLocaService().Tr("avatar_border_" + text + "_name");
	}
}

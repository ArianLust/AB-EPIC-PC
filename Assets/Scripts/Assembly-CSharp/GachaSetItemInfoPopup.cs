using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using UnityEngine;

public class GachaSetItemInfoPopup : MonoBehaviour
{
	[SerializeField]
	private UIInputTrigger m_CloseButton;

	[SerializeField]
	private UIInputTrigger m_InfoButton;

	[SerializeField]
	private UIInputTrigger m_CloseInfoScreenButton;

	[SerializeField]
	private Animation m_InfoScreenAnimation;

	[SerializeField]
	private UILabel m_Header;

	[SerializeField]
	private UIGrid m_ItemGrid;

	[SerializeField]
	private BirdEquipmentPreviewUI m_BirdEquipmentPreviewUI;

	[SerializeField]
	private BannerEquipmentPreviewUI m_BannerEquipmentPreviewUI;

	[SerializeField]
	private UILabel m_InfoPopupRainbowRiotText;

	[SerializeField]
	private Animation m_BackgroundAnimation;

	[SerializeField]
	private Animation m_HeaderAnimation;

	[SerializeField]
	private Animation m_CharacterDisplayAnimation;

	[SerializeField]
	private Animation m_CategoriesAnimation;

	[SerializeField]
	private BirdTabButton m_RedTab;

	[SerializeField]
	private BirdTabButton m_YellowTab;

	[SerializeField]
	private BirdTabButton m_WhiteTab;

	[SerializeField]
	private BirdTabButton m_BlackTab;

	[SerializeField]
	private BirdTabButton m_BlueTab;

	[SerializeField]
	private GameObject m_TabList;

	[SerializeField]
	private GameObject m_DeactivatedBlack;

	[SerializeField]
	private GameObject m_ActivatedBlack;

	[SerializeField]
	private GameObject m_DeactivatedBlue;

	[SerializeField]
	private GameObject m_ActivatedBlue;

	private string m_currentSelectedTab;

	[SerializeField]
	private EquipmentSetInfoElement m_EquipmentAPrefab;

	[SerializeField]
	private EquipmentSetInfoElement m_EquipmentBPrefab;

	[SerializeField]
	private EquipmentSetInfoElement m_BannerAPrefab;

	[SerializeField]
	private EquipmentSetInfoElement m_BannerBPrefab;

	[SerializeField]
	private GameObject m_SelectionFrameA;

	[SerializeField]
	private GameObject m_SelectionFrameB;

	private GameObject m_instantiatedFrameA;

	private GameObject m_instantiatedFrameB;

	[SerializeField]
	private Animation m_ItemInfoAnimation;

	[SerializeField]
	private UILabel m_ItemName;

	[SerializeField]
	private UILabel m_SetDescription;

	[SerializeField]
	private UISprite m_SetSkillIcon;

	private BirdGameData m_redBirdCopy;

	private BirdGameData m_yellowBirdCopy;

	private BirdGameData m_whiteBirdCopy;

	private BirdGameData m_blackBirdCopy;

	private BirdGameData m_blueBirdsCopy;

	private BirdGameData m_currentSelectedBird;

	private BannerGameData m_currentSelectedBanner;

	private EquipmentSetInfoElement m_currentSelectedButton;

	private BirdGameData m_currentSelectedOriginalBird;

	private Dictionary<int, BirdGameData> m_allOriginalBirds;

	private Dictionary<string, Dictionary<string, IInventoryItemGameData>> m_setItemsList;

	private bool m_isNormalGacha;

	private List<string> m_alreadyAddedItems;

	private Animation m_gachaPopupUiAnimation;

	private void Awake()
	{
		base.gameObject.SetActive(false);
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_SetItemInfoUi = this;
	}

	private void OnDestroy()
	{
		DeregisterEventHandler();
	}

	private void RegisterEventHandler()
	{
		DeregisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(3, ClosePopup);
		m_CloseButton.Clicked += ClosePopup;
		m_InfoButton.Clicked += OpenInfoScreen;
		m_CloseInfoScreenButton.Clicked += CloseInfoScreen;
		m_RedTab.m_ButtonTrigger.Clicked += SwitchToRed;
		m_YellowTab.m_ButtonTrigger.Clicked += SwitchToYellow;
		m_WhiteTab.m_ButtonTrigger.Clicked += SwitchToWhite;
		m_BlackTab.m_ButtonTrigger.Clicked += SwitchToBlack;
		m_BlueTab.m_ButtonTrigger.Clicked += SwitchToBlues;
	}

	private void DeregisterEventHandler()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(3);
		m_CloseButton.Clicked -= ClosePopup;
		m_InfoButton.Clicked -= OpenInfoScreen;
		m_CloseInfoScreenButton.Clicked -= CloseInfoScreen;
		m_RedTab.m_ButtonTrigger.Clicked -= SwitchToRed;
		m_YellowTab.m_ButtonTrigger.Clicked -= SwitchToYellow;
		m_WhiteTab.m_ButtonTrigger.Clicked -= SwitchToWhite;
		m_BlackTab.m_ButtonTrigger.Clicked -= SwitchToBlack;
		m_BlueTab.m_ButtonTrigger.Clicked -= SwitchToBlues;
	}

	public void ClosePopup()
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(LeaveCoroutine());
		}
	}

	private void OpenInfoScreen()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(4, CloseInfoScreen);
		int rainbowRiot1Multi = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").RainbowRiot1Multi;
		int rainbowRiot2Multi = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").RainbowRiot2Multi;
		m_InfoPopupRainbowRiotText.text = DIContainerInfrastructure.GetLocaService().Tr("set_info_desc_03").Replace("{value_1}", rainbowRiot1Multi.ToString())
			.Replace("{value_2}", rainbowRiot2Multi.ToString());
		m_InfoScreenAnimation.gameObject.SetActive(true);
		m_InfoScreenAnimation.Play("Popup_Enter");
	}

	private void CloseInfoScreen()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(4);
		m_InfoScreenAnimation.Play("Popup_Leave");
		Invoke("DeactiveInfoScreen", m_InfoScreenAnimation["Popup_Leave"].length);
	}

	private void DeactiveInfoScreen()
	{
		m_InfoScreenAnimation.gameObject.SetActive(false);
	}

	private IEnumerator LeaveCoroutine()
	{
		DeregisterEventHandler();
		StartCoroutine(m_BannerEquipmentPreviewUI.Leave());
		StartCoroutine(m_BirdEquipmentPreviewUI.Leave());
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(5u);
		m_BackgroundAnimation.Play("RootWindow_Leave");
		m_CharacterDisplayAnimation.Play("CharacterDisplay_Leave");
		m_HeaderAnimation.Play("Header_Leave");
		m_CategoriesAnimation.Play("Categories_Leave");
		DIContainerInfrastructure.GetCoreStateMgr().m_PopupRoot.Enter();
		if (m_gachaPopupUiAnimation != null)
		{
			m_gachaPopupUiAnimation.Play("GachaStep_1_Enter");
		}
		yield return new WaitForSeconds(0.75f);
		if (m_gachaPopupUiAnimation != null)
		{
			m_gachaPopupUiAnimation.Play("GachaStep_1_Enter_Buttons");
		}
		base.gameObject.SetActive(false);
	}

	public void Show(bool normalGacha, Animation gachaPopupUiAnimation)
	{
		m_gachaPopupUiAnimation = gachaPopupUiAnimation;
		DIContainerInfrastructure.GetCoreStateMgr().m_PopupRoot.Leave();
		if (m_gachaPopupUiAnimation != null)
		{
			m_gachaPopupUiAnimation.Play("GachaStep_1_Exit");
		}
		m_isNormalGacha = normalGacha;
		base.gameObject.SetActive(true);
		StartCoroutine(EnterCoroutine());
	}

	private IEnumerator EnterCoroutine()
	{
		yield return new WaitForSeconds(0.1f);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showEnergy = false,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false
		}, false);
		m_alreadyAddedItems = new List<string>();
		m_RedTab.m_NewMarker.SetActive(false);
		m_YellowTab.m_NewMarker.SetActive(false);
		m_WhiteTab.m_NewMarker.SetActive(false);
		m_BlackTab.m_NewMarker.SetActive(false);
		m_BlueTab.m_NewMarker.SetActive(false);
		FillItemList();
		if (m_isNormalGacha)
		{
			CreateBirdPreview();
		}
		else
		{
			InitBanners();
			StartCoroutine(m_BannerEquipmentPreviewUI.Enter());
			m_BannerEquipmentPreviewUI.RefreshStats(false);
		}
		UIPanel[] componentsInChildren = GetComponentsInChildren<UIPanel>();
		foreach (UIPanel panel in componentsInChildren)
		{
			panel.enabled = true;
		}
		m_CategoriesAnimation.Play("Categories_Enter");
		m_BackgroundAnimation.Play("RootWindow_Enter");
		m_CharacterDisplayAnimation.Play("CharacterDisplay_Enter");
		m_HeaderAnimation.Play("Header_Enter");
		yield return new WaitForSeconds(0.5f);
		RegisterEventHandler();
	}

	private void CreateBirdPreview()
	{
		bool flag = false;
		bool flag2 = false;
		List<BirdGameData> allBirds = DIContainerInfrastructure.GetCurrentPlayer().AllBirds;
		List<BirdGameData> list = new List<BirdGameData>();
		m_allOriginalBirds = new Dictionary<int, BirdGameData>();
		for (int i = 0; i < allBirds.Count; i++)
		{
			BirdGameData birdGameData = allBirds[i];
			if (birdGameData.Name == "bird_red")
			{
				m_redBirdCopy = new BirdGameData(birdGameData);
				list.Add(m_redBirdCopy);
				m_allOriginalBirds.Add(0, birdGameData);
			}
			else if (birdGameData.Name == "bird_yellow")
			{
				m_yellowBirdCopy = new BirdGameData(birdGameData);
				list.Add(m_yellowBirdCopy);
				m_allOriginalBirds.Add(1, birdGameData);
			}
			else if (birdGameData.Name == "bird_white")
			{
				m_whiteBirdCopy = new BirdGameData(birdGameData);
				list.Add(m_whiteBirdCopy);
				m_allOriginalBirds.Add(2, birdGameData);
			}
			else if (birdGameData.Name == "bird_black")
			{
				m_blackBirdCopy = new BirdGameData(birdGameData);
				list.Add(m_blackBirdCopy);
				m_allOriginalBirds.Add(3, birdGameData);
				flag = true;
			}
			else if (birdGameData.Name == "bird_blue")
			{
				m_blueBirdsCopy = new BirdGameData(birdGameData);
				list.Add(m_blueBirdsCopy);
				m_allOriginalBirds.Add(4, birdGameData);
				flag2 = true;
			}
		}
		m_DeactivatedBlack.SetActive(!flag);
		m_ActivatedBlack.SetActive(flag);
		m_DeactivatedBlue.SetActive(!flag2);
		m_ActivatedBlue.SetActive(flag2);
		m_BirdEquipmentPreviewUI.SetModels(list, true);
		SwitchToRed();
		StartCoroutine(m_BirdEquipmentPreviewUI.Enter());
		m_BirdEquipmentPreviewUI.RefreshStats(false);
	}

	public void OnItemSelected(EquipmentSetInfoElement button)
	{
		if (m_currentSelectedButton != button && m_currentSelectedButton != button.m_Partner)
		{
			m_currentSelectedButton = button;
			if (m_isNormalGacha)
			{
				StartCoroutine(FlyToBird(button));
			}
			else
			{
				StartCoroutine(FlyToBanner(button));
			}
		}
	}

	private IEnumerator FlyToBanner(EquipmentSetInfoElement button)
	{
		BannerAssetController bannerAsset = m_BannerEquipmentPreviewUI.m_CharacterController.m_AssetController as BannerAssetController;
		Transform root1 = bannerAsset.m_BannerFlagRoot;
		Transform root2 = bannerAsset.m_BannerTipRoot;
		yield return StartCoroutine(SelectNewButton(button, root1, root2));
		m_BannerEquipmentPreviewUI.m_CharacterController.m_AssetController.PlayCheerAnim();
		DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { button.m_Item }, button.m_Item.ItemBalancing.ItemType, m_currentSelectedBanner.InventoryGameData);
		DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { button.m_Partner.m_Item }, button.m_Partner.m_Item.ItemBalancing.ItemType, m_currentSelectedBanner.InventoryGameData);
		SetFooterBanner();
		m_BannerEquipmentPreviewUI.RefreshStats(false);
	}

	private IEnumerator FlyToBird(EquipmentSetInfoElement button)
	{
		Transform root1 = m_BirdEquipmentPreviewUI.m_CurrentCharacterController.m_AssetController.MainHandBone;
		Transform root2 = m_BirdEquipmentPreviewUI.m_CurrentCharacterController.m_AssetController.OffHandBone;
		yield return StartCoroutine(SelectNewButton(button, root1, root2));
		m_BirdEquipmentPreviewUI.m_CurrentCharacterController.m_AssetController.PlayCheerAnim();
		DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { button.m_Item }, button.m_Item.ItemBalancing.ItemType, m_currentSelectedBird.InventoryGameData);
		DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { button.m_Partner.m_Item }, button.m_Partner.m_Item.ItemBalancing.ItemType, m_currentSelectedBird.InventoryGameData);
		SetFooterItem();
		m_BirdEquipmentPreviewUI.RefreshStats(false);
	}

	private IEnumerator SelectNewButton(EquipmentSetInfoElement oldButton, Transform root1, Transform root2)
	{
		m_instantiatedFrameA.GetComponent<Animation>().Play("Hide");
		m_instantiatedFrameB.GetComponent<Animation>().Play("Hide");
		yield return new WaitForSeconds(m_instantiatedFrameB.GetComponent<Animation>()["Hide"].length);
		if (oldButton.m_IsSecondary)
		{
			m_instantiatedFrameA.transform.parent = oldButton.m_Partner.transform;
			m_instantiatedFrameB.transform.parent = oldButton.transform;
			ShowFrameAnimations();
			oldButton.m_Partner.FlyToTransform(root1);
			yield return new WaitForSeconds(oldButton.FlyToTransform(root2));
		}
		else
		{
			m_instantiatedFrameB.transform.parent = oldButton.m_Partner.transform;
			m_instantiatedFrameA.transform.parent = oldButton.transform;
			ShowFrameAnimations();
			oldButton.m_Partner.FlyToTransform(root2);
			yield return new WaitForSeconds(oldButton.FlyToTransform(root1));
		}
		oldButton.ResetFromFly();
		oldButton.m_Partner.ResetFromFly();
	}

	private void ShowFrameAnimations()
	{
		m_instantiatedFrameA.transform.localPosition = Vector3.zero;
		m_instantiatedFrameB.transform.localPosition = Vector3.zero;
		m_instantiatedFrameA.GetComponent<Animation>().Play("Show");
		m_instantiatedFrameA.GetComponent<Animation>().PlayQueued("Loop");
		m_instantiatedFrameB.GetComponent<Animation>().Play("Show");
		m_instantiatedFrameB.GetComponent<Animation>().PlayQueued("Loop");
		m_instantiatedFrameA.transform.localScale = Vector3.one;
		m_instantiatedFrameB.transform.localScale = Vector3.one;
	}

	public IEnumerator SetupItems(string charName)
	{
		DeregisterEventHandler();
		m_BirdEquipmentPreviewUI.PlayCharacterChanged();
		foreach (Transform child2 in m_ItemGrid.transform)
		{
			UnityEngine.Object.Destroy(child2.gameObject);
		}
		yield return new WaitForEndOfFrame();
		m_instantiatedFrameA = UnityEngine.Object.Instantiate(m_SelectionFrameA);
		m_instantiatedFrameB = UnityEngine.Object.Instantiate(m_SelectionFrameB);
		m_instantiatedFrameA.transform.localScale = Vector3.one;
		m_instantiatedFrameB.transform.localScale = Vector3.one;
		bool anyNewFound = false;
		for (int i = 0; i < m_setItemsList[charName].Count; i++)
		{
			IInventoryItemGameData setItem = m_setItemsList[charName].Values.ToList()[i];
			if (m_alreadyAddedItems.Contains(setItem.ItemBalancing.NameId) || setItem.ItemBalancing.ItemType == InventoryItemType.BannerEmblem)
			{
				continue;
			}
			bool equipThisItem = false;
			if (setItem is EquipmentGameData)
			{
				equipThisItem = ((EquipmentGameData)setItem).BalancingData.ShowAsNew;
			}
			else if (setItem is BannerItemGameData)
			{
				equipThisItem = ((BannerItemGameData)setItem).BalancingData.FlagAsNew;
			}
			if (!anyNewFound && m_ItemGrid.transform.childCount == m_setItemsList[charName].Count - 2)
			{
				equipThisItem = true;
			}
			if (equipThisItem)
			{
				anyNewFound = true;
				if (m_isNormalGacha)
				{
					DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { setItem }, setItem.ItemBalancing.ItemType, m_currentSelectedBird.InventoryGameData);
					DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { m_setItemsList[charName][setItem.ItemBalancing.NameId] }, m_setItemsList[charName][setItem.ItemBalancing.NameId].ItemBalancing.ItemType, m_currentSelectedBird.InventoryGameData);
				}
				else
				{
					DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { setItem }, setItem.ItemBalancing.ItemType, m_currentSelectedBanner.InventoryGameData);
					DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { m_setItemsList[charName][setItem.ItemBalancing.NameId] }, m_setItemsList[charName][setItem.ItemBalancing.NameId].ItemBalancing.ItemType, m_currentSelectedBanner.InventoryGameData);
					m_BannerEquipmentPreviewUI.RefreshStats(false);
				}
			}
			int setItemId = Convert.ToInt32(Regex.Match(setItem.ItemBalancing.NameId, "\\d+").Value);
			EquipmentSetInfoElement infoItem2 = null;
			EquipmentSetInfoElement correspondingItem2 = null;
			if (m_isNormalGacha)
			{
				m_TabList.SetActive(true);
				infoItem2 = UnityEngine.Object.Instantiate(m_EquipmentAPrefab);
				correspondingItem2 = UnityEngine.Object.Instantiate(m_EquipmentBPrefab);
			}
			else
			{
				m_TabList.SetActive(false);
				infoItem2 = UnityEngine.Object.Instantiate(m_BannerAPrefab);
				correspondingItem2 = UnityEngine.Object.Instantiate(m_BannerBPrefab);
			}
			infoItem2.Init(setItem, correspondingItem2, this);
			SetItemPrefab(infoItem2.transform);
			infoItem2.transform.name = i + 1 + "_A_" + infoItem2.transform.name;
			if (equipThisItem)
			{
				infoItem2.transform.name = "0_" + infoItem2.transform.name;
			}
			correspondingItem2.Init(m_setItemsList[charName][setItem.ItemBalancing.NameId], infoItem2, this);
			SetItemPrefab(correspondingItem2.transform);
			correspondingItem2.transform.name = i + 1 + "_B_" + correspondingItem2.transform.name;
			if (equipThisItem)
			{
				correspondingItem2.transform.name = "0_" + correspondingItem2.transform.name;
			}
			UIPanel panel1 = infoItem2.GetComponent<UIPanel>();
			UIPanel panel2 = correspondingItem2.GetComponent<UIPanel>();
			if (panel1 != null)
			{
				UnityEngine.Object.Destroy(panel1);
			}
			if (panel2 != null)
			{
				UnityEngine.Object.Destroy(panel2);
			}
			if (equipThisItem)
			{
				m_instantiatedFrameA.transform.parent = infoItem2.transform;
				m_instantiatedFrameB.transform.parent = correspondingItem2.transform;
				m_instantiatedFrameA.transform.localPosition = Vector3.zero;
				m_instantiatedFrameB.transform.localPosition = Vector3.zero;
				m_instantiatedFrameA.GetComponent<Animation>().Play("Loop");
				m_instantiatedFrameB.GetComponent<Animation>().Play("Loop");
				m_instantiatedFrameA.transform.localScale = Vector3.one;
				m_instantiatedFrameB.transform.localScale = Vector3.one;
				m_currentSelectedButton = infoItem2;
				if (m_isNormalGacha)
				{
					SetFooterItem();
				}
				else
				{
					SetFooterBanner();
				}
			}
			m_alreadyAddedItems.Add(m_setItemsList[charName][setItem.ItemBalancing.NameId].ItemBalancing.NameId);
			m_ItemGrid.Reposition();
			EquipmentSetInfoElement[] componentsInChildren = m_ItemGrid.GetComponentsInChildren<EquipmentSetInfoElement>();
			foreach (EquipmentSetInfoElement child in componentsInChildren)
			{
				child.SetPosition();
			}
		}
		RegisterEventHandler();
	}

	private void SetItemPrefab(Transform itemObject)
	{
		itemObject.parent = m_ItemGrid.transform;
		itemObject.localPosition = Vector3.zero;
		UnityHelper.SetLayerRecusively(itemObject.gameObject, LayerMask.NameToLayer("IgnoreTutorialInterface"));
	}

	private void InitBanners()
	{
		m_currentSelectedBanner = new BannerGameData(DIContainerInfrastructure.GetCurrentPlayer().BannerGameData);
		if (m_BirdEquipmentPreviewUI.m_CurrentCharacterController != null)
		{
			m_BirdEquipmentPreviewUI.m_CurrentCharacterController.gameObject.SetActive(false);
		}
		StartCoroutine(SetupItems("banner"));
		m_BannerEquipmentPreviewUI.SetModel(m_currentSelectedBanner);
		m_BannerEquipmentPreviewUI.m_CharacterController.transform.localPosition = Vector3.zero;
	}

	private void SwitchToRed()
	{
		if (!(m_currentSelectedTab == "red"))
		{
			m_currentSelectedTab = "red";
			StartCoroutine(SetupItems("bird_red"));
			m_RedTab.Activate(true);
			m_YellowTab.Activate(false);
			m_WhiteTab.Activate(false);
			m_BlackTab.Activate(false);
			m_BlueTab.Activate(false);
			m_BirdEquipmentPreviewUI.SetCharacter(m_redBirdCopy);
			m_currentSelectedBird = m_redBirdCopy;
			m_currentSelectedOriginalBird = m_allOriginalBirds[0];
		}
	}

	private void SwitchToYellow()
	{
		if (!(m_currentSelectedTab == "yellow"))
		{
			m_currentSelectedTab = "yellow";
			StartCoroutine(SetupItems("bird_yellow"));
			m_RedTab.Activate(false);
			m_YellowTab.Activate(true);
			m_WhiteTab.Activate(false);
			m_BlackTab.Activate(false);
			m_BlueTab.Activate(false);
			m_BirdEquipmentPreviewUI.SetCharacter(m_yellowBirdCopy);
			m_currentSelectedBird = m_yellowBirdCopy;
			m_currentSelectedOriginalBird = m_allOriginalBirds[1];
		}
	}

	private void SwitchToWhite()
	{
		if (!(m_currentSelectedTab == "white"))
		{
			m_currentSelectedTab = "white";
			StartCoroutine(SetupItems("bird_white"));
			m_RedTab.Activate(false);
			m_YellowTab.Activate(false);
			m_WhiteTab.Activate(true);
			m_BlackTab.Activate(false);
			m_BlueTab.Activate(false);
			m_BirdEquipmentPreviewUI.SetCharacter(m_whiteBirdCopy);
			m_currentSelectedBird = m_whiteBirdCopy;
			m_currentSelectedOriginalBird = m_allOriginalBirds[2];
		}
	}

	private void SwitchToBlack()
	{
		if (!(m_currentSelectedTab == "black"))
		{
			m_currentSelectedTab = "black";
			StartCoroutine(SetupItems("bird_black"));
			m_RedTab.Activate(false);
			m_YellowTab.Activate(false);
			m_WhiteTab.Activate(false);
			m_BlackTab.Activate(true);
			m_BlueTab.Activate(false);
			m_BirdEquipmentPreviewUI.SetCharacter(m_blackBirdCopy);
			m_currentSelectedBird = m_blackBirdCopy;
			m_currentSelectedOriginalBird = m_allOriginalBirds[3];
		}
	}

	private void SwitchToBlues()
	{
		if (!(m_currentSelectedTab == "blues"))
		{
			m_currentSelectedTab = "blues";
			StartCoroutine(SetupItems("bird_blue"));
			m_RedTab.Activate(false);
			m_YellowTab.Activate(false);
			m_WhiteTab.Activate(false);
			m_BlackTab.Activate(false);
			m_BlueTab.Activate(true);
			m_BirdEquipmentPreviewUI.SetCharacter(m_blueBirdsCopy);
			m_currentSelectedBird = m_blueBirdsCopy;
			m_currentSelectedOriginalBird = m_allOriginalBirds[4];
		}
	}

	private void FillItemList()
	{
		m_setItemsList = new Dictionary<string, Dictionary<string, IInventoryItemGameData>>();
		m_setItemsList.Add("bird_red", new Dictionary<string, IInventoryItemGameData>());
		m_setItemsList.Add("bird_yellow", new Dictionary<string, IInventoryItemGameData>());
		m_setItemsList.Add("bird_white", new Dictionary<string, IInventoryItemGameData>());
		m_setItemsList.Add("bird_black", new Dictionary<string, IInventoryItemGameData>());
		m_setItemsList.Add("bird_blue", new Dictionary<string, IInventoryItemGameData>());
		m_setItemsList.Add("banner", new Dictionary<string, IInventoryItemGameData>());
		if (m_isNormalGacha)
		{
			List<EquipmentBalancingData> list = (from e in DIContainerBalancing.Service.GetBalancingDataList<EquipmentBalancingData>()
				where !string.IsNullOrEmpty(e.CorrespondingSetItemId)
				select e).ToList();
			foreach (EquipmentBalancingData item in list)
			{
				if (!item.HideInPreview)
				{
					m_setItemsList[item.RestrictedBirdId].Add(item.CorrespondingSetItemId, CreateItemFromLoot(item.NameId));
					if (item.ShowAsNew)
					{
						SetNewMarker(item);
					}
				}
			}
		}
		else
		{
			List<BannerItemBalancingData> list2 = (from e in DIContainerBalancing.Service.GetBalancingDataList<BannerItemBalancingData>()
				where !string.IsNullOrEmpty(e.CorrespondingSetItem)
				select e).ToList();
			foreach (BannerItemBalancingData item2 in list2)
			{
				if (!item2.HideInPreview)
				{
					m_setItemsList["banner"].Add(item2.CorrespondingSetItem, CreateItemFromLoot(item2.NameId));
				}
			}
		}
		if (!m_isNormalGacha)
		{
		}
	}

	private void SetNewMarker(EquipmentBalancingData balancing)
	{
		if (balancing.RestrictedBirdId == "bird_red")
		{
			m_RedTab.m_NewMarker.SetActive(true);
		}
		else if (balancing.RestrictedBirdId == "bird_yellow")
		{
			m_YellowTab.m_NewMarker.SetActive(true);
		}
		else if (balancing.RestrictedBirdId == "bird_white")
		{
			m_WhiteTab.m_NewMarker.SetActive(true);
		}
		else if (balancing.RestrictedBirdId == "bird_black")
		{
			m_BlackTab.m_NewMarker.SetActive(true);
		}
		else if (balancing.RestrictedBirdId == "bird_blue")
		{
			m_BlueTab.m_NewMarker.SetActive(true);
		}
	}

	private Dictionary<string, IInventoryItemGameData> SortEquipment(Dictionary<string, IInventoryItemGameData> m_setItemsList)
	{
		SortedDictionary<int, string> sortedDictionary = new SortedDictionary<int, string>();
		Dictionary<string, IInventoryItemGameData> dictionary = new Dictionary<string, IInventoryItemGameData>();
		foreach (string key in m_setItemsList.Keys)
		{
			IInventoryItemGameData inventoryItemGameData = m_setItemsList[key];
			bool showAsNew = ((EquipmentBalancingData)inventoryItemGameData.ItemBalancing).ShowAsNew;
			EquipmentBalancingData equipmentBalancingData = inventoryItemGameData.ItemBalancing as EquipmentBalancingData;
			int i = Convert.ToInt32((float)DIContainerBalancing.Service.GetBalancingData<BirdBalancingData>(equipmentBalancingData.RestrictedBirdId).SortPriority * ((float)m_setItemsList.Count / 5f));
			if (showAsNew)
			{
				i--;
			}
			for (; sortedDictionary.ContainsKey(i); i++)
			{
			}
			sortedDictionary.Add(i, key);
		}
		foreach (string value in sortedDictionary.Values)
		{
			dictionary.Add(value, m_setItemsList[value]);
		}
		return dictionary;
	}

	private IInventoryItemGameData CreateItemFromLoot(string nameId)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		dictionary.Add(nameId, 1);
		int level = DIContainerInfrastructure.GetCurrentPlayer().Data.Level + 2;
		IInventoryItemGameData inventoryItemGameData = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(dictionary, level), EquipmentSource.Gatcha).FirstOrDefault();
		inventoryItemGameData.ItemData.Quality = 4;
		return inventoryItemGameData;
	}

	private void SetFooterItem()
	{
		if (m_currentSelectedButton == null)
		{
			DebugLog.Error("[GachaSetItemInfoPopup] No item selected!");
			return;
		}
		m_ItemInfoAnimation.Play("ItemInfo_Change_Out");
		m_ItemName.text = DIContainerInfrastructure.GetLocaService().Tr((m_currentSelectedButton.m_Item as EquipmentGameData).SetItemSkill.SkillNameId + "_name");
		SkillBattleDataBase skillBattleDataBase = (m_currentSelectedButton.m_Item as EquipmentGameData).SetItemSkill.GenerateSkillBattleData();
		m_SetDescription.text = skillBattleDataBase.GetLocalizedDescription(new BirdCombatant(m_currentSelectedOriginalBird).SetPvPBird(!m_isNormalGacha));
		m_ItemInfoAnimation.PlayQueued("ItemInfo_Change_In");
	}

	private void SetFooterBanner()
	{
		if (m_currentSelectedButton == null)
		{
			DebugLog.Error("[GachaSetItemInfoPopup] No item selected!");
			return;
		}
		m_ItemInfoAnimation.Play("ItemInfo_Change_Out");
		SkillBattleDataBase skillBattleDataBase = (m_currentSelectedButton.m_Item as BannerItemGameData).SetItemSkill.GenerateSkillBattleData();
		m_ItemName.text = (m_currentSelectedButton.m_Item as BannerItemGameData).SetItemSkill.SkillLocalizedName;
		m_SetDescription.text = skillBattleDataBase.GetLocalizedDescription(new BannerCombatant(m_currentSelectedBanner));
		m_SetSkillIcon.spriteName = BannerItemGameData.GetPerkIconNameByPerk((m_currentSelectedButton.m_Item as BannerItemGameData).SetItemSkill.GetPerkType());
		m_ItemInfoAnimation.PlayQueued("ItemInfo_Change_In");
	}
}

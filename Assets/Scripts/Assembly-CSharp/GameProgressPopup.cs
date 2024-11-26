using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class GameProgressPopup : MonoBehaviour
{
	private enum GameProgressTab : byte
	{
		Progress = 0,
		StarCollection = 1
	}

	[SerializeField]
	private UIInputTrigger m_CloseButton;

	[SerializeField]
	private CampProp m_MasteryBadge;

	[SerializeField]
	private List<CampProp> m_CampProps = new List<CampProp>();

	[SerializeField]
	private List<CampProp> m_LeveledCampProps = new List<CampProp>();

	[SerializeField]
	private List<CampProp> m_StackingCampProps = new List<CampProp>();

	[SerializeField]
	private Animation m_HeaderAnimation;

	[SerializeField]
	private Animation m_AreaSelectionAnimation;

	[SerializeField]
	private Animation m_ContentBoardAnimation;

	[SerializeField]
	private Animation m_FooterAnimation;

	[SerializeField]
	private GameObject m_StarRewardPrefab;

	[SerializeField]
	private UILabel m_StarCollectionCounter;

	[SerializeField]
	private GameObject m_StarCollectionGrid;

	[SerializeField]
	private UIInputTrigger m_StarTab;

	[SerializeField]
	private UIInputTrigger m_ProgressTab;

	[SerializeField]
	private GameObject m_StarCollectionParent;

	[SerializeField]
	private GameObject m_StarCollectionFooter;

	[SerializeField]
	private GameObject m_ProgressParent;

	[SerializeField]
	private GameObject m_ProgressParentFooter;

	[SerializeField]
	private Transform m_Marker;

	[SerializeField]
	private Transform m_MarkerPosition_StarCollection;

	[SerializeField]
	private Transform m_MarkerPosition_Progress;

	[SerializeField]
	private GameObject m_FirstKey;

	[SerializeField]
	private BoxCollider m_KeyCollider;

	private bool m_isInProgress;

	private InventoryGameData m_inventory;

	private bool m_isFriend;

	private void OnDestroy()
	{
		DeregisterEventHandler();
	}

	private void RegisterEventHandler()
	{
		DeregisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(3, ClosePopup);
		m_CloseButton.Clicked += ClosePopup;
		m_StarTab.Clicked += EnterStars;
		m_ProgressTab.Clicked += EnterProgress;
	}

	private void DeregisterEventHandler()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(3);
		m_CloseButton.Clicked -= ClosePopup;
		m_StarTab.Clicked -= EnterStars;
		m_ProgressTab.Clicked -= EnterProgress;
	}

	public void ClosePopup()
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine("LeaveCoroutine");
		}
	}

	private IEnumerator LeaveCoroutine()
	{
		DeregisterEventHandler();
		m_HeaderAnimation.Play("Header_Leave");
		m_AreaSelectionAnimation.Play("AreaSelection_Leave");
		m_ContentBoardAnimation.Play("List_Leave");
		m_FooterAnimation.Play("Footer_Leave");
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(5u);
		yield return new WaitForSeconds(1f);
		base.gameObject.SetActive(false);
	}

	public void Show(bool isFriend, bool StartWithStarCollection = false)
	{
		StopCoroutine("LeaveCoroutine");
		base.gameObject.SetActive(true);
		m_ProgressParent.SetActive(true);
		m_StarCollectionParent.SetActive(false);
		m_ProgressParentFooter.SetActive(true);
		m_StarCollectionFooter.SetActive(false);
		if (isFriend)
		{
			m_inventory = ClientInfo.CurrentCampInventory;
		}
		else
		{
			m_inventory = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData;
		}
		m_isFriend = isFriend;
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Enter();
		StartCoroutine(EnterCoroutine(StartWithStarCollection));
	}

	private IEnumerator EnterCoroutine(bool startWithStarCollection)
	{
		if (startWithStarCollection)
		{
			yield return StartCoroutine(SetupStarCollection());
		}
		else
		{
			yield return StartCoroutine(SetupProgress());
		}
		m_HeaderAnimation.Play("Header_Enter");
		m_AreaSelectionAnimation.Play("AreaSelection_Enter");
		m_ContentBoardAnimation.Play("List_Enter");
		m_FooterAnimation.Play("Footer_Enter");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showEnergy = false,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false
		}, false);
		yield return new WaitForSeconds(0.5f);
	}

	private void EnterProgress()
	{
		if (!m_isInProgress)
		{
			DeregisterEventHandler();
			StartCoroutine(SwitchTab(GameProgressTab.Progress));
		}
	}

	private void EnterStars()
	{
		if (m_isInProgress)
		{
			DeregisterEventHandler();
			StartCoroutine(SwitchTab(GameProgressTab.StarCollection));
		}
	}

	private float GetMarkerYPosition_Progress()
	{
		return (!(m_MarkerPosition_Progress != null)) ? m_Marker.localPosition.y : m_MarkerPosition_Progress.localPosition.y;
	}

	private IEnumerator SwitchTab(GameProgressTab newTab)
	{
		if (!m_AreaSelectionAnimation || !m_ContentBoardAnimation || !m_FooterAnimation)
		{
			Debug.LogError("Could not play Switch Animation on GameProgress. One or more Animations not linked");
			GameProgressPopup gameProgressPopup = this;
			IEnumerator routine;
			if (newTab == GameProgressTab.Progress)
			{
				IEnumerator enumerator = SetupProgress();
				routine = enumerator;
			}
			else
			{
				routine = SetupStarCollection();
			}
			yield return gameProgressPopup.StartCoroutine(routine);
			yield break;
		}
		m_ContentBoardAnimation.Play("List_Leave");
		yield return new WaitForSeconds(m_AreaSelectionAnimation["AreaSelection_Leave"].length);
		GameProgressPopup gameProgressPopup2 = this;
		IEnumerator routine2;
		if (newTab == GameProgressTab.Progress)
		{
			IEnumerator enumerator = SetupProgress();
			routine2 = enumerator;
		}
		else
		{
			routine2 = SetupStarCollection();
		}
		yield return gameProgressPopup2.StartCoroutine(routine2);
		m_ContentBoardAnimation.Play("List_Enter");
	}

	private IEnumerator SetupProgress()
	{
		m_isInProgress = true;
		m_Marker.localPosition = new Vector3(m_Marker.localPosition.x, GetMarkerYPosition_Progress(), m_Marker.localPosition.z);
		m_ProgressParent.SetActive(true);
		m_StarCollectionParent.SetActive(false);
		m_ProgressParentFooter.SetActive(true);
		m_StarCollectionFooter.SetActive(false);
		foreach (CampProp leveledProp in m_LeveledCampProps)
		{
			if ((bool)leveledProp && leveledProp.GetModel() != null)
			{
				leveledProp.SetCounter(leveledProp.GetModel().Data.Level);
			}
		}
		foreach (CampProp stackingCampProp in m_StackingCampProps)
		{
			if ((bool)stackingCampProp && stackingCampProp.GetModel() != null)
			{
				stackingCampProp.SetCounter(stackingCampProp.GetModel().ItemValue);
			}
		}
		if (m_MasteryBadge != null)
		{
			IInventoryItemGameData masteryBadge = null;
			if (DIContainerLogic.InventoryService.TryGetItemGameData(ClientInfo.CurrentCampInventory, "unlock_mastery_badge", out masteryBadge))
			{
				m_MasteryBadge.SetCounter(masteryBadge.ItemData.Level);
			}
		}
		foreach (CampProp prop in m_CampProps)
		{
			if (!prop.m_IsInitialized)
			{
				prop.Awake();
			}
		}
		while (m_CampProps.Count((CampProp p) => !p.m_IsInitialized) > 0)
		{
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(0.1f);
		m_KeyCollider.enabled = m_FirstKey.activeSelf;
		RegisterEventHandler();
	}

	private float GetMarkerYPosition_StarCollection()
	{
		return (!(m_MarkerPosition_StarCollection != null)) ? m_Marker.localPosition.y : m_MarkerPosition_StarCollection.localPosition.y;
	}

	private IEnumerator SetupStarCollection()
	{
		m_isInProgress = false;
		m_Marker.localPosition = new Vector3(m_Marker.localPosition.x, GetMarkerYPosition_StarCollection(), m_Marker.localPosition.z);
		m_ProgressParent.SetActive(false);
		m_StarCollectionParent.SetActive(true);
		m_ProgressParentFooter.SetActive(false);
		m_StarCollectionFooter.SetActive(true);
		yield return StartCoroutine(CreateStarCollection());
		m_StarCollectionGrid.GetComponent<UIGrid>().Reposition();
		RegisterEventHandler();
	}

	private IEnumerator CreateStarCollection()
	{
		foreach (Transform child in m_StarCollectionGrid.transform)
		{
			Object.Destroy(child.gameObject);
		}
		yield return new WaitForEndOfFrame();
		if (m_StarCollectionGrid.transform.childCount > 0)
		{
			yield return new WaitForEndOfFrame();
			m_StarCollectionGrid.GetComponent<UIGrid>().Reposition();
			yield break;
		}
		int currentStars = DIContainerLogic.InventoryService.GetItemValue(m_inventory, "star_collection");
		int maxStars = 0;
		maxStars += (from c in DIContainerBalancing.Service.GetBalancingDataList<ChronicleCaveHotspotBalancingData>()
			where c.Type == HotspotType.Battle
			select c).Count() * 3;
		maxStars += (from c in DIContainerBalancing.Service.GetBalancingDataList<HotspotBalancingData>()
			where c.CountForStars
			select c).Count() * 3;
		m_StarCollectionCounter.text = currentStars + " / " + maxStars;
		PlayerGameData player = DIContainerInfrastructure.GetCurrentPlayer();
		foreach (BasicShopOfferBalancingData offer in DIContainerLogic.GetShopService().GetShopOffers(player, "shop_star_rewards"))
		{
			BasicShopOfferBalancingData offerBalancing = offer;
			if (offerBalancing == null)
			{
				continue;
			}
			if (offer.NameId == "offer_star_reward_04_snoutling" && player.Data.WonAvengerByStars)
			{
				offerBalancing = DIContainerLogic.GetShopService().GetShopOffer("offer_star_reward_04");
			}
			float neededStars = 0f;
			IInventoryItemGameData reward = null;
			foreach (Requirement req in offerBalancing.BuyRequirements)
			{
				if (req.NameId == "star_collection")
				{
					neededStars = req.Value;
					break;
				}
			}
			List<IInventoryItemGameData> rewards = DIContainerLogic.GetShopService().GetShopOfferContent(player, offerBalancing, DIContainerLogic.GetSalesManagerService().GetOfferSaleDetails(offer.NameId));
			foreach (IInventoryItemGameData tempReward in rewards)
			{
				if (!tempReward.ItemBalancing.NameId.StartsWith("star_aquiredreward"))
				{
					reward = tempReward;
					break;
				}
			}
			GameObject OfferListElement = Object.Instantiate(m_StarRewardPrefab);
			OfferListElement.name = neededStars + "_ListElement_BattleStarsReward";
			ResourceCostBlind cost = OfferListElement.GetComponentInChildren<ResourceCostBlind>();
			if (cost != null)
			{
				cost.SetModel(string.Empty, null, DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat((int)neededStars), string.Empty);
			}
			OfferListElement.transform.parent = m_StarCollectionGrid.transform;
			OfferListElement.transform.localPosition = Vector3.zero;
			OfferListElement.transform.localScale = Vector3.one;
			if (reward is ConsumableItemGameData)
			{
				OfferListElement.GetComponent<GenericOverlayInvoker>().m_LocaIdent = reward.ItemBalancing.LocaBaseId + "_desc";
			}
			else
			{
				OfferListElement.GetComponent<GenericOverlayInvoker>().m_LocaIdent = reward.ItemBalancing.LocaBaseId + "_tt";
			}
			LootDisplayContoller lootDisplay = OfferListElement.GetComponent<LootDisplayContoller>();
			lootDisplay.SetModel(reward, new List<IInventoryItemGameData>(), LootDisplayType.None, "_Large", false, false, true);
			if ((float)currentStars >= neededStars)
			{
				IInventoryItemGameData popupItem = null;
				string popupname = offerBalancing.NameId.Replace("offer_star_reward", "star_popup");
				bool isNew = false;
				if (DIContainerLogic.InventoryService.TryGetItemGameData(player.InventoryGameData, DIContainerBalancing.Service.GetBalancingData<BasicItemBalancingData>(popupname), out popupItem))
				{
					isNew = popupItem.ItemData.IsNew;
				}
				if (isNew && !m_isFriend)
				{
					StartCoroutine(PlayDelayedAnimation(OfferListElement.GetComponent<Animator>(), "BattleStarsReward_Achieved", 1.5f));
					popupItem.ItemData.IsNew = false;
				}
				else
				{
					OfferListElement.GetComponent<Animator>().Play("BattleStarsReward_SetAchieved");
				}
			}
		}
	}

	private IEnumerator PlayDelayedAnimation(Animator Anim, string AnimName, float Delay)
	{
		yield return new WaitForSeconds(Delay);
		Anim.Play(AnimName);
	}
}

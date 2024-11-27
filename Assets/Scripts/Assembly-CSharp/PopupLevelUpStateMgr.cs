using System.Collections;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class PopupLevelUpStateMgr : MonoBehaviour
{
	[SerializeField]
	private UIInputTrigger m_AbortButton;

	[SerializeField]
	private Transform m_ProfLRoot;

	[SerializeField]
	private Transform m_CenteredItemRoot;

	[SerializeField]
	private UILabel m_NewLevel;

	[SerializeField]
	private UILabel m_NewRank;

	[SerializeField]
	private Transform m_BirdRoot;

	[SerializeField]
	private Transform m_BirdClassRoot;

	[SerializeField]
	private float m_MaximumShowTime = 4.5f;

	private WaitTimeOrAbort m_AsyncOperation;

	private GameObject m_IconInstanciated;

	private string m_IconClassNameID;

	public bool m_IsShowing;

	[SerializeField]
	private CharacterAssetController m_ProfCharacterAssetController;

	[SerializeField]
	private UILabel m_powerLevelOldLabel;

	[SerializeField]
	private UILabel m_powerLevelNewLabel;

	private void Awake()
	{
		base.gameObject.SetActive(false);
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_LevelUpPopup = this;
	}

	private void SetDragControllerActive(bool flag)
	{
		if (DIContainerInfrastructure.CurrentDragController != null)
		{
			DIContainerInfrastructure.CurrentDragController.SetActiveDepth(flag, 1);
		}
	}

	public WaitTimeOrAbort ShowLeveUpPopup(int level, int oldPowerLevelTotal)
	{
		m_IsShowing = true;
		base.gameObject.SetActive(true);
		StartCoroutine("EnterCoroutine");
		m_NewLevel.text = level.ToString("0");
		m_powerLevelOldLabel.text = oldPowerLevelTotal.ToString();
		m_powerLevelNewLabel.text = DIContainerInfrastructure.GetCurrentPlayer().Data.HighestPowerLevelEver.ToString();
		DIContainerInfrastructure.GetCurrentPlayer().RewardLevelUpLoot(level);
		bool flag = false;
		if (DIContainerLogic.RequirementService.CheckRequirement(DIContainerInfrastructure.GetCurrentPlayer(), new Requirement
		{
			NameId = "hotspot_023_battleground",
			RequirementType = RequirementType.HaveUnlockedHotpsot,
			Value = 1f
		}))
		{
			foreach (string key in DIContainerInfrastructure.GetCurrentPlayer().Data.ShopOffersNew.Keys)
			{
				BasicShopOfferBalancingData balancing;
				if (DIContainerBalancing.Service.TryGetBalancingData<BasicShopOfferBalancingData>(key, out balancing) && balancing.Category == "shop_01_potions" && DIContainerInfrastructure.GetCurrentPlayer().Data.ShopOffersNew[key])
				{
					flag = true;
				}
			}
		}
		m_ProfLRoot.gameObject.SetActive(flag);
		if (flag)
		{
			m_ProfCharacterAssetController.PlayIdleAnimation();
			InvokeRepeating("ProfTalk", 1.5f, 6f);
		}
		m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
		return m_AsyncOperation;
	}

	public WaitTimeOrAbort ShowRankUpPopUp(ClassItemGameData classItem)
	{
		m_IsShowing = true;
		base.gameObject.SetActive(true);
		m_ProfLRoot.gameObject.SetActive(false);
		m_BirdRoot.gameObject.SetActive(false);
		m_NewLevel.transform.parent.gameObject.SetActive(false);
		m_NewRank.transform.parent.gameObject.SetActive(true);
		m_NewRank.text = classItem.Data.Level.ToString();
		StartCoroutine("EnterCoroutine");
		m_IconInstanciated = DIContainerInfrastructure.GetClassAssetProvider().InstantiateObject(classItem.ItemBalancing.AssetBaseId, m_BirdClassRoot, Vector3.zero, Quaternion.identity, false);
		m_IconClassNameID = classItem.Data.NameId;
		m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
		return m_AsyncOperation;
	}

	private void ProfTalk()
	{
		m_ProfCharacterAssetController.PlayAnimation("Talk");
		m_ProfCharacterAssetController.PlayAnimationQueued("Idle");
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().RegisterPopupEntered(true);
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_level_enter");
		SetDragControllerActive(false);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showFriendshipEssence = false,
			showLuckyCoins = true,
			showSnoutlings = false
		}, false);
		GetComponent<Animation>().Play("Popup_LevelUp_Enter");
		yield return new WaitForSeconds(GetComponent<Animation>()["Popup_LevelUp_Enter"].length);
		yield return new WaitForSeconds(1f);
		RegisterEventHandlers();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_level_enter");
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(4, m_AbortButton_Clicked);
		m_AbortButton.Clicked += m_AbortButton_Clicked;
	}

	private void DeRegisterEventHandlers()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(4);
		m_AbortButton.Clicked -= m_AbortButton_Clicked;
	}

	private IEnumerator LeaveCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().RegisterPopupEntered(false);
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_level_leave");
		DeRegisterEventHandlers();
		SetDragControllerActive(true);
		GetComponent<Animation>().Play("Popup_LevelUp_Leave");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		yield return new WaitForSeconds(GetComponent<Animation>()["Popup_LevelUp_Leave"].length);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(5u);
		m_AsyncOperation.Abort();
		m_AsyncOperation = null;
		m_IsShowing = false;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_level_leave");
		DIContainerInfrastructure.GetCoreStateMgr().LeaveShop();
		if (m_IconInstanciated != null)
		{
			DIContainerInfrastructure.GetClassAssetProvider().DestroyObject(m_IconClassNameID, m_IconInstanciated);
		}
		base.gameObject.SetActive(false);
	}

	private void m_AbortButton_Clicked()
	{
		m_AbortButton.Clicked -= m_AbortButton_Clicked;
		StartCoroutine("LeaveCoroutine");
	}
}

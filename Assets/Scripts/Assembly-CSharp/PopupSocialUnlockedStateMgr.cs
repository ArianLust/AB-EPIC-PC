using System.Collections;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using UnityEngine;

public class PopupSocialUnlockedStateMgr : MonoBehaviour
{
	[SerializeField]
	private UILabel m_FeatureNameLabel;

	[SerializeField]
	private UILabel m_FeatureDescLabel;

	[SerializeField]
	private UILabel m_ButtonLabel;

	[SerializeField]
	private UIInputTrigger m_AbortButton;

	[SerializeField]
	private UIInputTrigger m_SignInButton;

	[SerializeField]
	private GameObject m_SignInButtonRoot;

	[SerializeField]
	private GameObject m_AbortButtonRoot;

	[SerializeField]
	private Transform m_ItemRoot;

	[SerializeField]
	private Transform m_CenteredItemRoot;

	private GameObject m_FeatureObject;

	private string m_FeatureObjectNameId;

	private BasicItemGameData m_BasicItemGameData;

	[SerializeField]
	private float m_MaximumShowTime = 4.5f;

	private FeatureObjectType m_ObjectType;

	private WaitTimeOrAbort m_AsyncOperation;

	public bool m_IsShowing;

	[SerializeField]
	private CharacterControllerCamp m_CampCharacterControllerPrefab;

	private void Awake()
	{
		base.gameObject.SetActive(false);
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_SocialUnlockedPopup = this;
	}

	public WaitTimeOrAbort ShowUnlockFeaturePopup(BasicItemGameData basicItem)
	{
		if (basicItem == null)
		{
			m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
			m_AsyncOperation.Abort();
			return m_AsyncOperation;
		}
		m_IsShowing = true;
		m_BasicItemGameData = basicItem;
		base.gameObject.SetActive(true);
		StartCoroutine("EnterCoroutine");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false
		}, true);
		m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
		return m_AsyncOperation;
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().RegisterPopupEntered(true);
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_unlock_enter");
		SetDragControllerActive(false);
		m_FeatureObject = InstantiateFeatureObject(m_BasicItemGameData);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showSnoutlings = false
		}, true);
		GetComponent<Animation>().Play("Popup_Invitation_Enter");
		yield return new WaitForSeconds(GetComponent<Animation>()["Popup_Invitation_Enter"].length);
		RegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_unlock_enter");
	}

	private GameObject InstantiateFeatureObject(BasicItemGameData basicItemGameData)
	{
		m_ObjectType = FeatureObjectType.CampPropUnlock;
		m_AbortButtonRoot.gameObject.SetActive(true);
		if (basicItemGameData.ItemBalancing.ItemType != InventoryItemType.Story)
		{
			return null;
		}
		if (m_BasicItemGameData.BalancingData.NameId == "unlock_rovio_account")
		{
			m_ButtonLabel.text = DIContainerInfrastructure.GetLocaService().Tr("invitation_popup_signin", "Sign in");
		}
		else if (m_BasicItemGameData.BalancingData.NameId == "unlock_facebook")
		{
			m_ButtonLabel.text = DIContainerInfrastructure.GetLocaService().Tr("invitation_popup_facebook_signin", "Sign in");
		}
		if (basicItemGameData.BalancingData.NameId.StartsWith("hint_"))
		{
			m_FeatureNameLabel.text = m_BasicItemGameData.ItemLocalizedName;
			m_FeatureDescLabel.text = m_BasicItemGameData.ItemLocalizedDesc;
		}
		else if (basicItemGameData.BalancingData.NameId.StartsWith("skill_"))
		{
			m_FeatureNameLabel.text = m_BasicItemGameData.ItemLocalizedName;
			m_FeatureDescLabel.text = m_BasicItemGameData.ItemLocalizedDesc;
		}
		else
		{
			m_FeatureNameLabel.text = m_BasicItemGameData.ItemLocalizedName;
			m_FeatureDescLabel.text = m_BasicItemGameData.ItemLocalizedDesc;
		}
		if (basicItemGameData.ItemAssetName.StartsWith("bird_"))
		{
			m_FeatureObjectNameId = basicItemGameData.ItemAssetName;
			CharacterControllerCamp characterControllerCamp = Object.Instantiate(m_CampCharacterControllerPrefab);
			m_ObjectType = FeatureObjectType.BirdUnlock;
			characterControllerCamp.transform.parent = m_ItemRoot;
			characterControllerCamp.transform.localPosition = Vector3.zero;
			characterControllerCamp.transform.localScale = Vector3.one;
			characterControllerCamp.SetModel(new BirdGameData(m_FeatureObjectNameId, DIContainerInfrastructure.GetCurrentPlayer().Data.Level), false);
			UnityHelper.SetLayerRecusively(characterControllerCamp.gameObject, base.gameObject.layer);
			return characterControllerCamp.gameObject;
		}
		if (basicItemGameData.ItemAssetName.StartsWith("pig_"))
		{
			m_FeatureObjectNameId = basicItemGameData.ItemAssetName;
			CharacterControllerCamp characterControllerCamp2 = Object.Instantiate(m_CampCharacterControllerPrefab);
			m_ObjectType = FeatureObjectType.BirdUnlock;
			characterControllerCamp2.transform.parent = m_ItemRoot;
			characterControllerCamp2.transform.localPosition = Vector3.zero;
			characterControllerCamp2.transform.localScale = Vector3.one;
			PigBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<PigBalancingData>(m_FeatureObjectNameId);
			characterControllerCamp2.SetModel(balancingData.NameId, false);
			StartCoroutine(CheerCharacterRepeating(characterControllerCamp2));
			UnityHelper.SetLayerRecusively(characterControllerCamp2.gameObject, base.gameObject.layer);
			return characterControllerCamp2.gameObject;
		}
		if (DIContainerInfrastructure.PropLiteAssetProvider().ContainsAsset(basicItemGameData.ItemAssetName))
		{
			m_FeatureObjectNameId = basicItemGameData.ItemAssetName;
			GameObject gameObject = DIContainerInfrastructure.PropLiteAssetProvider().InstantiateObject(m_FeatureObjectNameId, m_CenteredItemRoot, Vector3.zero, Quaternion.identity);
			gameObject.SetActive(true);
			VectorContainer component = gameObject.GetComponent<VectorContainer>();
			if ((bool)component)
			{
				gameObject.transform.parent = m_CenteredItemRoot;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.localPosition += component.m_Vector;
			}
			else
			{
				gameObject.transform.parent = m_ItemRoot;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = Vector3.one;
			}
			UnityHelper.SetLayerRecusively(gameObject, base.gameObject.layer);
			return gameObject;
		}
		return null;
	}

	private IEnumerator CheerCharacterRepeating(CharacterControllerCamp character)
	{
		if ((bool)character)
		{
			yield return new WaitForSeconds(character.PlayCheerCharacter());
			yield return new WaitForSeconds(Random.Range(2f, 6f));
			StartCoroutine(CheerCharacterRepeating(character));
		}
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(4, m_AbortButton_Clicked);
		m_AbortButton.Clicked += m_AbortButton_Clicked;
		m_SignInButton.Clicked += SignInButtonClicked;
	}

	private void DeRegisterEventHandlers()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(4);
		m_AbortButton.Clicked -= m_AbortButton_Clicked;
		m_SignInButton.Clicked -= SignInButtonClicked;
		DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent -= FacebookLoginSucceededEvent;
		DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent -= FacebookLoginFailedEvent;
	}

	private IEnumerator LeaveCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().RegisterPopupEntered(false);
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_unlock_leave");
		SetDragControllerActive(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(5u);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		GetComponent<Animation>().Play("Popup_Invitation_Leave");
		yield return new WaitForSeconds(GetComponent<Animation>()["Popup_Invitation_Leave"].length);
		if (m_ObjectType == FeatureObjectType.BirdUnlock)
		{
			DebugLog.Log("Destroy Character");
			m_FeatureObject.GetComponent<CharacterControllerCamp>().DestroyCharacter();
		}
		else
		{
			DIContainerInfrastructure.PropLiteAssetProvider().DestroyObject(m_FeatureObjectNameId, m_FeatureObject);
		}
		m_IsShowing = false;
		m_AsyncOperation.Abort();
		m_AsyncOperation = null;
		if (m_BasicItemGameData != null)
		{
			DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("feature_unlocked", m_BasicItemGameData.BalancingData.NameId);
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_unlock_leave");
		DIContainerInfrastructure.GetCoreStateMgr().LeaveShop();
		base.gameObject.SetActive(false);
	}

	private void SetDragControllerActive(bool flag)
	{
		if (DIContainerInfrastructure.CurrentDragController != null)
		{
			DIContainerInfrastructure.CurrentDragController.SetActiveDepth(flag, 1);
		}
	}

	private void m_AbortButton_Clicked()
	{
		DeRegisterEventHandlers();
		StartCoroutine("LeaveCoroutine");
	}

	private void SignInButtonClicked()
	{
		DeRegisterEventHandlers();
		if (m_BasicItemGameData.BalancingData.NameId == "unlock_rovio_account")
		{
			DIContainerInfrastructure.GetCoreStateMgr().GotoCampScreenViaHotlink("RovioId");
			DIContainerInfrastructure.GetCoreStateMgr().StopAutoDailyLoginPopup();
		}
		else if (m_BasicItemGameData.BalancingData.NameId == "unlock_facebook")
		{
			DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent -= FacebookLoginSucceededEvent;
			DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent -= FacebookLoginFailedEvent;
			DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent += FacebookLoginSucceededEvent;
			DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent += FacebookLoginFailedEvent;
			DIContainerInfrastructure.GetCoreStateMgr().TryLoginOnFacebook();
		}
		StartCoroutine("LeaveCoroutine");
	}

	private void FacebookLoginFailedEvent(string obj)
	{
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("social_facebook_login_failed", "Facebook login failed!"), "facebook", DispatchMessage.Status.Error);
	}

	private void FacebookLoginSucceededEvent()
	{
		DIContainerInfrastructure.GetCoreStateMgr().StartRefreshFriends();
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("social_facebook_login_succes", "Facebook login succesfully!"), "facebook", DispatchMessage.Status.Info);
	}
}

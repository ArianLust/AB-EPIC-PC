using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using SmoothMoves;
using UnityEngine;

public class PopupFeatureUnlockedStateMgr : MonoBehaviour
{
	[SerializeField]
	private UILabel m_FeatureNameLabel;

	[SerializeField]
	private UILabel m_FeatureDescLabel;

	[SerializeField]
	private UILabel m_NewLabel;

	[SerializeField]
	private UIInputTrigger m_AbortButton;

	[SerializeField]
	private GameObject m_AbortButtonRoot;

	[SerializeField]
	private List<Transform> m_DualItemRoot;

	[SerializeField]
	private List<Transform> m_DualCenteredItemRoot;

	[SerializeField]
	private Transform m_ItemRoot;

	[SerializeField]
	private Transform m_CenteredItemRoot;

	[SerializeField]
	private UILabel m_TimerLabel;

	[SerializeField]
	private GameObject m_TimerObject;

	[SerializeField]
	private GameObject m_gachaRainbowRiotHand;

	private List<GameObject> m_FeatureObject = new List<GameObject>();

	private List<string> m_FeatureObjectNameId = new List<string>();

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
		DIContainerInfrastructure.GetCoreStateMgr().m_FeatureUnlockedPopup = this;
	}

	public WaitTimeOrAbort ShowUnlockFeaturePopup(BasicItemGameData basicItem)
	{
		m_IsShowing = true;
		if (basicItem == null)
		{
			if (m_AsyncOperation == null)
			{
				m_AsyncOperation = new WaitTimeOrAbort(0f);
			}
			m_AsyncOperation.Abort();
			return m_AsyncOperation;
		}
		m_BasicItemGameData = basicItem;
		m_FeatureObject.Clear();
		m_FeatureObjectNameId.Clear();
		base.gameObject.SetActive(true);
		if (m_BasicItemGameData.ItemBalancing.NameId.StartsWith("special_offer_rainbow_riot"))
		{
			m_TimerObject.gameObject.SetActive(true);
			StartCoroutine(CountDownTime(DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.BalancingData.RainbowRiotTime));
		}
		else
		{
			m_TimerObject.gameObject.SetActive(false);
		}
		StartCoroutine("EnterCoroutine");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false
		}, false);
		m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
		return m_AsyncOperation;
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().RegisterPopupEntered(true);
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_unlock_enter");
		SetDragControllerActive(false);
		m_FeatureObject = InstantiateFeatureObject(m_BasicItemGameData, m_CenteredItemRoot, m_ItemRoot);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showSnoutlings = false
		}, true);
		GetComponent<Animation>().Play("Popup_FeatureUnlocked_Enter");
		yield return new WaitForSeconds(GetComponent<Animation>()["Popup_FeatureUnlocked_Enter"].length);
		if (m_BasicItemGameData.ItemBalancing.NameId.StartsWith("special_offer_rainbow_riot"))
		{
			DebugLog.Log("Let Golden Pig Rioting!");
			if (m_FeatureObject.Count > 0)
			{
				GameObject pigHand = UnityEngine.Object.Instantiate(m_gachaRainbowRiotHand);
				pigHand.transform.parent = m_FeatureObject.FirstOrDefault().transform.Find("Root/Body");
				pigHand.transform.localScale = Vector3.one;
				UISprite sprite = pigHand.transform.Find("Animation/Hand").GetComponent<UISprite>();
				pigHand.SetActive(false);
				yield return new WaitForSeconds(0.3f);
				pigHand.SetActive(true);
				pigHand.GetComponent<Animation>().Play("RainbowRiotMarker_Enter");
				pigHand.transform.localPosition = new Vector3(-10f, 30f, 3f);
				if (DIContainerInfrastructure.GetCurrentPlayer().Data.IsExtraRainbowRiot)
				{
					sprite.spriteName = "Hand_RainbowRiotB";
					pigHand.GetComponentInChildren<UILabel>().text = DIContainerInfrastructure.GetLocaService().Tr("rainbowriot_hand_desc").Replace("{value_1}", DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").RainbowRiot2Multi.ToString());
				}
				else
				{
					sprite.spriteName = "Hand_RainbowRiotA";
					pigHand.GetComponentInChildren<UILabel>().text = DIContainerInfrastructure.GetLocaService().Tr("rainbowriot_hand_desc").Replace("{value_1}", DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").RainbowRiot1Multi.ToString());
				}
				GameObject firstOrDefault = m_FeatureObject.FirstOrDefault();
				if (firstOrDefault != null)
				{
					BoneAnimation boneAnim = firstOrDefault.GetComponent<BoneAnimation>();
					if ((bool)boneAnim)
					{
						DebugLog.Log("Let Golden start Pig Rioting!");
						boneAnim.Play("RainbowRiot");
					}
				}
			}
		}
		RegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_unlock_enter");
	}

	private List<GameObject> InstantiateFeatureObject(BasicItemGameData basicItemGameData, Transform centerTransform, Transform rooTransform)
	{
		m_ObjectType = FeatureObjectType.CampPropUnlock;
		m_AbortButtonRoot.gameObject.SetActive(true);
		if (basicItemGameData.ItemBalancing.ItemType != InventoryItemType.Story)
		{
			return null;
		}
		if (basicItemGameData.BalancingData.NameId.StartsWith("hint_"))
		{
			m_NewLabel.text = DIContainerInfrastructure.GetLocaService().Tr("popup_unlock_hint", "Hint!");
			m_FeatureNameLabel.text = m_BasicItemGameData.ItemLocalizedName;
			m_FeatureDescLabel.text = m_BasicItemGameData.ItemLocalizedDesc;
		}
		else if (basicItemGameData.BalancingData.NameId.StartsWith("skill_"))
		{
			m_NewLabel.text = string.Empty;
			m_FeatureNameLabel.text = m_BasicItemGameData.ItemLocalizedName;
			m_FeatureDescLabel.text = m_BasicItemGameData.ItemLocalizedDesc;
		}
		else
		{
			m_NewLabel.text = DIContainerInfrastructure.GetLocaService().Tr("popup_unlock_new", "New!");
			m_FeatureNameLabel.text = m_BasicItemGameData.ItemLocalizedName;
			m_FeatureDescLabel.text = m_BasicItemGameData.ItemLocalizedDesc;
		}
		string[] array = basicItemGameData.ItemAssetName.Split(';');
		List<GameObject> list = new List<GameObject>();
		if (array.Length == 2)
		{
			for (int i = 0; i < array.Length && i < m_DualItemRoot.Count && i < m_DualCenteredItemRoot.Count; i++)
			{
				DebugLog.Log("Asset Name: " + array[i]);
				list.Add(CreateFeatureObjectAsset(array[i], basicItemGameData, m_DualCenteredItemRoot[i], m_DualItemRoot[i]));
			}
		}
		else if (array.Length == 1)
		{
			list.Add(CreateFeatureObjectAsset(array[0], basicItemGameData, centerTransform, rooTransform));
		}
		return list;
	}

	private IEnumerator CountDownTime(float seconds)
	{
		for (float timeLeft = seconds; timeLeft >= 0f; timeLeft -= 1f)
		{
			m_TimerLabel.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(TimeSpan.FromSeconds(timeLeft));
			yield return new WaitForSeconds(1f);
		}
		StartCoroutine(LeaveCoroutine());
	}

	private GameObject CreateFeatureObjectAsset(string assetName, BasicItemGameData basicItemGameData, Transform centerTransform, Transform rooTransform)
	{
		if (assetName.StartsWith("bird_"))
		{
			m_FeatureObjectNameId.Add(assetName);
			CharacterControllerCamp characterControllerCamp = UnityEngine.Object.Instantiate(m_CampCharacterControllerPrefab);
			m_ObjectType = FeatureObjectType.BirdUnlock;
			characterControllerCamp.SetModel(new BirdGameData(assetName, DIContainerInfrastructure.GetCurrentPlayer().Data.Level), false);
			characterControllerCamp.transform.parent = rooTransform;
			characterControllerCamp.transform.localPosition = Vector3.zero;
			characterControllerCamp.transform.localScale = Vector3.one;
			if (assetName == "bird_sonic")
			{
				characterControllerCamp.transform.localScale = new Vector3(0.85f, 0.85f, 1f);
			}
			UnityHelper.SetLayerRecusively(characterControllerCamp.gameObject, base.gameObject.layer);
			return characterControllerCamp.gameObject;
		}
		if (assetName.StartsWith("pig_"))
		{
			m_FeatureObjectNameId.Add(assetName);
			CharacterControllerCamp characterControllerCamp2 = UnityEngine.Object.Instantiate(m_CampCharacterControllerPrefab);
			m_ObjectType = FeatureObjectType.BirdUnlock;
			characterControllerCamp2.transform.parent = rooTransform;
			characterControllerCamp2.transform.localPosition = Vector3.zero;
			characterControllerCamp2.transform.localScale = Vector3.one;
			PigBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<PigBalancingData>(assetName);
			characterControllerCamp2.SetModel(balancingData.NameId, false);
			characterControllerCamp2.transform.localScale = Vector3.one;
			StartCoroutine(CheerCharacterRepeating(characterControllerCamp2));
			UnityHelper.SetLayerRecusively(characterControllerCamp2.gameObject, base.gameObject.layer);
			return characterControllerCamp2.gameObject;
		}
		if (assetName.StartsWith("DailyPost"))
		{
			m_FeatureDescLabel.text += "_2";
		}
		if (DIContainerInfrastructure.PropLiteAssetProvider().ContainsAsset(basicItemGameData.ItemAssetName))
		{
			m_FeatureObjectNameId.Add(assetName);
			GameObject gameObject = DIContainerInfrastructure.PropLiteAssetProvider().InstantiateObject(assetName, centerTransform, Vector3.zero, Quaternion.identity);
			gameObject.SetActive(true);
			VectorContainer component = gameObject.GetComponent<VectorContainer>();
			if ((bool)component)
			{
				gameObject.transform.parent = centerTransform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.localPosition += component.m_Vector;
			}
			else
			{
				gameObject.transform.parent = rooTransform;
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
			yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 6f));
			StartCoroutine(CheerCharacterRepeating(character));
		}
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

	public void LeavePopup()
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(LeaveCoroutine());
		}
	}

	private IEnumerator LeaveCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().RegisterPopupEntered(false);
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_unlock_leave");
		SetDragControllerActive(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(5u);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		GetComponent<Animation>().Play("Popup_FeatureUnlocked_Leave");
		yield return new WaitForSeconds(GetComponent<Animation>()["Popup_FeatureUnlocked_Leave"].length);
		if (m_ObjectType == FeatureObjectType.BirdUnlock)
		{
			DebugLog.Log("Destroy Character");
			foreach (GameObject o in m_FeatureObject)
			{
				o.GetComponent<CharacterControllerCamp>().DestroyCharacter();
			}
		}
		else
		{
			for (int index = 0; index < m_FeatureObjectNameId.Count; index++)
			{
				string feature = m_FeatureObjectNameId[index];
				DIContainerInfrastructure.PropLiteAssetProvider().DestroyObject(feature, m_FeatureObject[index]);
			}
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

	private void m_BuyButton_Clicked()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.GetCoreStateMgr().ShowShop("shop_premium", delegate
		{
		});
		StartCoroutine("LeaveCoroutine");
	}
}

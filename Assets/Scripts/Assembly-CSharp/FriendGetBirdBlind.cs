using System;
using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Interfaces;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class FriendGetBirdBlind : MonoBehaviour
{
	[SerializeField]
	private FriendInfoElement m_FriendInfoElement;

	[SerializeField]
	private GameObject m_TimerRoot;

	[SerializeField]
	private UILabel m_TimerLabel;

	[SerializeField]
	private GameObject m_SelectedRoot;

	[SerializeField]
	private GameObject m_SelectableRoot;

	[SerializeField]
	private UIInputTrigger m_SelectButton;

	[SerializeField]
	private GameObject m_BuyableRoot;

	[SerializeField]
	private UIInputTrigger m_BuyButton;

	[SerializeField]
	private ResourceCostBlind m_CostBlind;

	[SerializeField]
	private UIInputTrigger m_SkipCooldownButton;

	[SerializeField]
	private ResourceCostBlind m_SkipCostBlind;

	[SerializeField]
	private Transform m_BirdRoot;

	[SerializeField]
	private Transform m_AlternatingRoot;

	[SerializeField]
	private Vector3 m_AlternatingCharacterOffset = new Vector3(-150f, 0f, 0f);

	private FriendBirdWindowStateMgr m_StateMgr;

	private FriendGameData m_Model;

	private Vector3 m_CharacterOffset;

	[SerializeField]
	private CharacterControllerWorldMap m_WorldMapCharacterControllerPrefab;

	private CharacterControllerWorldMap m_Character;

	[SerializeField]
	private bool m_useCharacterInterfaceLayer = true;

	[SerializeField]
	private UILabel m_PowerLevelLabelLeftLabel;

	[SerializeField]
	private UILabel m_PowerLevelLabelRightLabel;

	[SerializeField]
	private GameObject m_PowerLevelLeftObject;

	[SerializeField]
	private GameObject m_PowerLevelRighObject;

	private bool m_isLeftAligned;

	private bool m_alignmentSet;

	public void Initialize(FriendBirdWindowStateMgr stateMgr, int index)
	{
		m_alignmentSet = false;
		if ((bool)m_Character)
		{
			UnityEngine.Object.Destroy(m_Character.gameObject);
		}
		CancelInvoke("CheckIfBirdLoaded");
		m_StateMgr = stateMgr;
		m_FriendInfoElement.SetDefault();
		base.gameObject.name = index.ToString("000") + "_FriendBlind";
	}

	public void SetAlternatingOffset(int index)
	{
		if ((bool)m_AlternatingRoot && index % 2 == 1)
		{
			m_isLeftAligned = true;
			m_AlternatingRoot.Translate(m_AlternatingCharacterOffset);
			if ((bool)m_Character)
			{
				m_Character.transform.localPosition = Vector3.zero;
			}
		}
		else
		{
			m_isLeftAligned = false;
		}
		m_alignmentSet = true;
	}

	private void CheckIfBirdLoaded()
	{
		if (m_Model.IsFriendBirdLoaded)
		{
			CancelInvoke("CheckIfBirdLoaded");
			m_Character = UnityEngine.Object.Instantiate(m_WorldMapCharacterControllerPrefab);
			UnityHelper.SetLayerRecusively(m_Character.gameObject, (!m_useCharacterInterfaceLayer) ? LayerMask.NameToLayer("Interface") : LayerMask.NameToLayer("InterfaceCharacter"));
			m_Character.transform.parent = m_BirdRoot;
			m_Character.transform.localPosition = Vector3.zero;
			m_Character.transform.localScale = Vector3.one;
			m_Model.FriendBird.IsNPC = true;
			m_Character.SetModel(m_Model.FriendBird, false, true);
			m_Character.transform.localScale = Vector3.Scale(Vector3.one, (!m_Character.AssetIsOnWrongSide(m_Character.m_AssetController.m_AssetFaction, Faction.Birds)) ? Vector3.one : new Vector3(-1f, 1f, 1f));
			if ((bool)m_SelectButton)
			{
				m_SelectButton.Clicked -= OnBirdClicked;
				m_SelectButton.Clicked += OnBirdClicked;
			}
			if ((bool)m_BuyButton)
			{
				m_BuyButton.Clicked -= OnBirdClicked;
				m_BuyButton.Clicked += OnBirdClicked;
			}
			StartCoroutine(WaitForAlignment());
		}
	}

	private IEnumerator WaitForAlignment()
	{
		while (!m_alignmentSet)
		{
			yield return new WaitForEndOfFrame();
		}
		if (m_isLeftAligned)
		{
			m_PowerLevelLeftObject.SetActive(true);
			m_PowerLevelRighObject.SetActive(false);
			m_PowerLevelLabelLeftLabel.text = DIContainerInfrastructure.GetPowerLevelCalculator().GetBirdPowerLevel(m_Model.FriendBird).ToString();
		}
		else
		{
			m_PowerLevelLeftObject.SetActive(false);
			m_PowerLevelRighObject.SetActive(true);
			m_PowerLevelLabelRightLabel.text = DIContainerInfrastructure.GetPowerLevelCalculator().GetBirdPowerLevel(m_Model.FriendBird).ToString();
		}
	}

	private void OnBirdClicked()
	{
		if (m_Character == null)
		{
			DebugLog.Error("No character to select!");
			return;
		}
		if (!DIContainerLogic.SocialService.IsGetFriendBirdPossible(DIContainerInfrastructure.GetCurrentPlayer(), m_Model))
		{
			if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.GetBirdCooldowns[m_Model.FriendId] != 0)
			{
				return;
			}
			Requirement mightyEagleBirdReqirement = DIContainerBalancing.Service.GetBalancingData<SocialEnvironmentBalancingData>("default").MightyEagleBirdReqirement;
			if (!DIContainerLogic.RequirementService.ExecuteRequirements(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, new List<Requirement> { mightyEagleBirdReqirement }, "buy_mighty_eagle_bird"))
			{
				IInventoryItemGameData data = null;
				if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "lucky_coin", out data) && data.ItemBalancing.NameId == "lucky_coin")
				{
					m_StateMgr.m_LuckyCoinController.SwitchToShop();
				}
				return;
			}
			m_Model.HasPaid = true;
		}
		StartCoroutine(SelectAndLeave(m_Character.GetModel()));
	}

	private void OnSkipCooldownClicked()
	{
		if (m_Character == null)
		{
			DebugLog.Error("No character to select!");
			return;
		}
		if (!DIContainerLogic.SocialService.IsGetFriendBirdPossible(DIContainerInfrastructure.GetCurrentPlayer(), m_Model))
		{
			if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.GetBirdCooldowns[m_Model.FriendId] == 0)
			{
				return;
			}
			Requirement skipCooldownRequirement = DIContainerBalancing.Service.GetBalancingData<SocialEnvironmentBalancingData>("default").SkipCooldownRequirement;
			if (!DIContainerLogic.RequirementService.ExecuteRequirements(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, new List<Requirement> { skipCooldownRequirement }, "skip_friend_bird_cooldown"))
			{
				IInventoryItemGameData data = null;
				if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "lucky_coin", out data) && data.ItemBalancing.NameId == "lucky_coin")
				{
					m_StateMgr.m_LuckyCoinController.SwitchToShop();
				}
				return;
			}
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.GetBirdCooldowns[m_Model.FriendId] = 0u;
		}
		StartCoroutine(SelectAndLeave(m_Character.GetModel()));
	}

	private IEnumerator SelectAndLeave(ICharacter character)
	{
		foreach (Transform blind in base.transform.parent)
		{
			FriendGetBirdBlind fgb = blind.GetComponent<FriendGetBirdBlind>();
			if ((bool)fgb)
			{
				fgb.DeRegisterEventHandler();
			}
		}
		if ((bool)m_StateMgr)
		{
			m_StateMgr.SelectedFriend(m_Model);
		}
		if ((bool)m_StateMgr)
		{
			m_StateMgr.Leave();
		}
		yield break;
	}

	public FriendGetBirdBlind SetModel(FriendGameData friend, FriendGameData selectedFriend)
	{
		RegisterEventHandler();
		m_FriendInfoElement.SetModel(friend);
		m_Model = friend;
		if (m_Model.FriendData == null)
		{
			return this;
		}
		if (m_Model.FriendData.NeedsPayment)
		{
			base.gameObject.name = "001_FriendBlind";
		}
		else if (m_Model.FriendData.IsNPC)
		{
			base.gameObject.name = (700 - m_Model.FriendData.Level).ToString("000") + "_FriendBlind";
		}
		else
		{
			base.gameObject.name = (500 - m_Model.FriendData.Level).ToString("000") + "_FriendBlind";
		}
		DebugLog.Log("Friend Info set!");
		if ((bool)m_BuyableRoot)
		{
			m_BuyableRoot.SetActive(false);
		}
		if ((bool)m_SelectableRoot)
		{
			m_SelectableRoot.SetActive(false);
		}
		if ((bool)m_SelectedRoot)
		{
			m_SelectedRoot.SetActive(false);
		}
		if ((bool)m_TimerRoot)
		{
			m_TimerRoot.SetActive(false);
		}
		if ((bool)m_BirdRoot)
		{
			DebugLog.Log("Check friend bird availiable!");
			if (!DIContainerLogic.SocialService.IsGetFriendBirdPossible(DIContainerInfrastructure.GetCurrentPlayer(), m_Model))
			{
				DebugLog.Log("Check friend bird availiable failed!");
				if (m_Model.Data != null && m_Model.Data.NeedsPayment)
				{
					DebugLog.Log("Mighty eagle!");
					if ((bool)m_CostBlind)
					{
						m_BuyableRoot.SetActive(true);
						Requirement mightyEagleBirdReqirement = DIContainerBalancing.Service.GetBalancingData<SocialEnvironmentBalancingData>("default").MightyEagleBirdReqirement;
						IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(mightyEagleBirdReqirement.NameId);
						m_CostBlind.SetModel(balancingData.AssetBaseId, null, mightyEagleBirdReqirement.Value, DIContainerInfrastructure.GetLocaService().Tr("gen_lbl_youhaveitem", new Dictionary<string, string> { 
						{
							"{value}",
							DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, mightyEagleBirdReqirement.NameId))
						} }));
						RegisterEventHandler();
					}
				}
				else
				{
					DebugLog.Log("Cooldown!");
					if ((bool)m_TimerRoot)
					{
						m_TimerRoot.SetActive(true);
					}
					Requirement skipCooldownRequirement = DIContainerBalancing.Service.GetBalancingData<SocialEnvironmentBalancingData>("default").SkipCooldownRequirement;
					IInventoryItemBalancingData balancingData2 = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(skipCooldownRequirement.NameId);
					if ((bool)m_SkipCostBlind)
					{
						m_SkipCostBlind.SetModel(balancingData2.AssetBaseId, null, skipCooldownRequirement.Value, string.Empty);
					}
					if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.GetBirdCooldowns == null)
					{
						DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.GetBirdCooldowns = new Dictionary<string, uint>();
					}
					if (!DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.GetBirdCooldowns.ContainsKey(m_Model.FriendId))
					{
						DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.GetBirdCooldowns.Add(m_Model.FriendId, 0u);
					}
					if (m_TimerLabel != null)
					{
						StartCoroutine(CountDownTimer(m_TimerLabel, DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.GetBirdCooldowns[m_Model.FriendId]).AddSeconds(DIContainerBalancing.Service.GetBalancingData<SocialEnvironmentBalancingData>("default").TimeForGetFriendBird)));
					}
					RegisterEventHandler();
				}
			}
			else if (selectedFriend != null && friend.FriendId == selectedFriend.FriendId)
			{
				if ((bool)m_SelectedRoot)
				{
					m_SelectedRoot.SetActive(true);
				}
			}
			else if ((bool)m_SelectableRoot)
			{
				m_SelectableRoot.SetActive(true);
			}
			InvokeRepeating("CheckIfBirdLoaded", 0f, 0.1f);
		}
		return this;
	}

	private IEnumerator CountDownTimer(UILabel timerLabel, DateTime targetTime)
	{
		DateTime trustedTime;
		while (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		while (targetTime > trustedTime)
		{
			if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				TimeSpan timeLeft = targetTime - trustedTime;
				timerLabel.text = DIContainerInfrastructure.GetLocaService().Tr("social_borrowfriendbird_timer", new Dictionary<string, string> { 
				{
					"{value_1}",
					DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(timeLeft)
				} });
			}
			yield return new WaitForSeconds(1f);
		}
		UnityEngine.Object.Destroy(timerLabel.gameObject);
		SetModel(m_Model, null);
	}

	public FriendGameData GetModel()
	{
		return m_Model;
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		if ((bool)m_BuyButton)
		{
			m_BuyButton.Clicked += OnBirdClicked;
		}
		if ((bool)m_SelectButton)
		{
			m_SelectButton.Clicked += OnBirdClicked;
		}
		if ((bool)m_SkipCooldownButton)
		{
			m_SkipCooldownButton.Clicked += OnSkipCooldownClicked;
		}
	}

	private void DeRegisterEventHandler()
	{
		if ((bool)m_BuyButton)
		{
			m_BuyButton.Clicked -= OnBirdClicked;
		}
		if ((bool)m_SelectButton)
		{
			m_SelectButton.Clicked -= OnBirdClicked;
		}
		if ((bool)m_SkipCooldownButton)
		{
			m_SkipCooldownButton.Clicked -= OnSkipCooldownClicked;
		}
	}

	private void m_ShopButton_Clicked()
	{
		StartCoroutine(SwitchToShop());
	}

	private IEnumerator SwitchToShop()
	{
		m_StateMgr.HideCharacters(true);
		DIContainerInfrastructure.GetCoreStateMgr().ShowShop("shop_premium", delegate
		{
			m_StateMgr.HideCharacters(false);
		});
		yield break;
	}

	private void OnReceivedPlayer(PublicPlayerData player)
	{
		m_StateMgr.Leave();
		DIContainerInfrastructure.GetCoreStateMgr().GotoFirendCampScreen(player, m_Model);
	}

	private void OnDestroy()
	{
		if ((bool)m_Character)
		{
			UnityHelper.SetLayerRecusively(m_Character.gameObject, LayerMask.NameToLayer("Scenery"));
		}
		DeRegisterEventHandler();
	}
}

using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Interfaces;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class PopupSingleBirdRevive : MonoBehaviour
{
	[SerializeField]
	private Transform m_CharacterSpawnRoot;

	[SerializeField]
	private UIInputTrigger m_CancelButtonTrigger;

	[SerializeField]
	private UIInputTrigger m_ReviveButtonTrigger;

	[SerializeField]
	private UILabel m_DescriptionText;

	[SerializeField]
	private ResourceCostBlind m_ReviveBirdsCost;

	[SerializeField]
	public CharacterControllerCamp m_CharacterCampPrefab;

	private float m_buttonClickedTime;

	private BattleUIStateMgr m_battleUiStateMgr;

	private BattleMgrBase m_battleMgr;

	private ICombatant m_knockedOutBird;

	private bool m_isActive;

	public void Enter()
	{
		StartCoroutine(EnterCoroutine());
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().RegisterPopupEntered(true);
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_revive_enter");
		m_isActive = true;
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 6u,
			showFriendshipEssence = false,
			showLuckyCoins = true,
			showSnoutlings = false
		}, true);
		base.gameObject.PlayAnimationOrAnimatorState("Popup_SingleBirdRevive_Enter");
		yield return new WaitForSeconds(base.gameObject.GetAnimationOrAnimatorStateLength("Popup_SingleBirdRevive_Enter"));
		RegisterEventHandlers();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_level_enter");
	}

	public void SetModel(BattleUIStateMgr battleUIStateMgr, ICombatant knockedOutBird)
	{
		m_battleUiStateMgr = battleUIStateMgr;
		m_battleMgr = battleUIStateMgr.m_BattleMgr;
		m_knockedOutBird = knockedOutBird;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("{value_1}", DIContainerInfrastructure.GetLocaService().GetCharacterName(knockedOutBird.CombatantNameId));
		m_DescriptionText.text = DIContainerInfrastructure.GetLocaService().Tr("birdlost_desc", dictionary);
		CharacterControllerCamp characterControllerCamp = Object.Instantiate(m_CharacterCampPrefab);
		characterControllerCamp.SetModel(knockedOutBird.CharacterModel);
		characterControllerCamp.transform.parent = m_CharacterSpawnRoot;
		characterControllerCamp.transform.localPosition = Vector3.zero;
		characterControllerCamp.transform.localScale = Vector3.one;
		characterControllerCamp.transform.GetComponentInChildren<CharacterAssetController>().PlayKnockoutAnim();
		UnityHelper.SetLayerRecusively(characterControllerCamp.gameObject, LayerMask.NameToLayer("Interface"));
		Requirement reviveSingleBirdsRequirement = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").ReviveSingleBirdsRequirement;
		IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(reviveSingleBirdsRequirement.NameId);
		m_ReviveBirdsCost.SetModel(balancingData.AssetBaseId, null, reviveSingleBirdsRequirement.Value, string.Empty);
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(4, Leave);
		m_ReviveButtonTrigger.Clicked += ReviveButtonClicked;
		m_CancelButtonTrigger.Clicked += Leave;
	}

	private void DeRegisterEventHandlers()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(4);
		m_ReviveButtonTrigger.Clicked -= ReviveButtonClicked;
		m_CancelButtonTrigger.Clicked -= Leave;
	}

	private void ReviveButtonClicked()
	{
		if (IsButtonAlreadyClickedThisFrame())
		{
			return;
		}
		Requirement reviveSingleBirdsRequirement = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").ReviveSingleBirdsRequirement;
		if (!DIContainerLogic.RequirementService.ExecuteRequirements(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, new List<Requirement> { reviveSingleBirdsRequirement }, "revive_birds"))
		{
			Requirement requirement = reviveSingleBirdsRequirement;
			if (requirement != null && requirement.RequirementType == RequirementType.PayItem)
			{
				IInventoryItemGameData data = null;
				if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, requirement.NameId, out data))
				{
					if (data.ItemBalancing.NameId == "lucky_coin")
					{
						DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.m_PlayerStatsController[1].m_StatBar.SwitchToShop();
					}
					else if (data.ItemBalancing.NameId == "gold")
					{
						DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.m_PlayerStatsController[0].m_StatBar.SwitchToShop();
					}
					else if (data.ItemBalancing.NameId == "friendship_essence")
					{
						DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.m_PlayerStatsController[2].m_StatBar.SwitchToShop();
					}
				}
			}
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateLuckyCoinsBar();
		}
		else
		{
			m_ReviveButtonTrigger.Clicked -= ReviveButtonClicked;
			m_battleMgr.ReviveBird(m_knockedOutBird);
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateLuckyCoinsBar();
			StartCoroutine(LeaveCoroutine());
		}
	}

	private bool IsButtonAlreadyClickedThisFrame()
	{
		if (m_buttonClickedTime == Time.time)
		{
			return true;
		}
		m_buttonClickedTime = Time.time;
		return false;
	}

	public void Leave()
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(LeaveCoroutine());
		}
	}

	private IEnumerator LeaveCoroutine()
	{
		if (m_isActive)
		{
			DIContainerInfrastructure.GetCoreStateMgr().RegisterPopupEntered(false);
			DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_revive_leave");
			m_isActive = false;
			DeRegisterEventHandlers();
			m_battleMgr.IsPausePossible = true;
			m_battleMgr.IsConsumableUsePossible = true;
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(5u);
			base.gameObject.PlayAnimationOrAnimatorState("Popup_SingleBirdRevive_Leave");
			yield return new WaitForSeconds(base.gameObject.GetAnimationOrAnimatorStateLength("Popup_SingleBirdRevive_Leave"));
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.IsInBattle = true;
			DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_level_leave");
			if ((bool)m_CharacterSpawnRoot.GetChild(0))
			{
				Object.Destroy(m_CharacterSpawnRoot.GetChild(0).gameObject);
			}
			m_knockedOutBird.CombatantView.m_reviveClicked = false;
			base.gameObject.SetActive(false);
		}
	}
}

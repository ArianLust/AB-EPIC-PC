using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Interfaces;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class BattleUIStateMgr : MonoBehaviour
{
	public RageMeterController m_RageMeter;

	public BattleProgressBar m_ProgressIndicator;

	public ConsumableBarController m_ConsumableBar;

	public BattleMgrBase m_BattleMgr;

	public ResourceCostBlind m_ReviveBirdsCost;

	public UIInputTrigger m_ReviveBirdsButton;

	public GameObject m_ReviveBirdsButtonRoot;

	public UISprite m_ReviveProgressBar;

	public UILabel m_ReviveProgressTimer;

	public Animation m_ReviveCounterAnim;

	public GameObject m_ConsumableBlocker;

	public GameObject m_EnvEffectRoot;

	public UISprite m_EnvEffectIcon;

	[SerializeField]
	private GameObject m_TurnIndicator;

	public Transform m_TurnIndicatorRoot;

	public GameObject m_TurnIndicatorPrefab;

	private bool m_ConsumableButtonEntered;

	private bool m_ConsumableBarEntered;

	private bool m_EnteredAnimation;

	private bool m_HasConsumables;

	[SerializeField]
	private GameObject m_PauseButtonRoot;

	[SerializeField]
	private GameObject m_AutoBattleButtonRoot;

	[SerializeField]
	public GameObject m_ConsumableButtonRoot;

	[SerializeField]
	public UIInputTrigger m_ConsumableButton;

	[SerializeField]
	public UIInputTrigger m_PauseButton;

	[SerializeField]
	public UIInputTrigger m_AutoBattleButton;

	[SerializeField]
	public Animation m_AutoBattleArrows;

	[SerializeField]
	public UISprite m_AutoBattlePlay;

	[SerializeField]
	public UISprite m_AutoBattleStop;

	[SerializeField]
	private Animation m_ConsumableButtonWarningAnim;

	[SerializeField]
	public float m_WarningHealthPercent = 0.2f;

	[SerializeField]
	private PopupSingleBirdRevive m_SingleBirdRevivePopup;

	private bool m_isInitialized;

	private bool m_hasLeft;

	private Camera m_InterfaceCamera;

	private bool m_lockedConsumables;

	private bool m_PauseButtonEntered;

	private bool m_AutoBattleButtonEntered;

	private bool m_ConsumableBarAnimating;

	private bool m_ConsumableButtonAnimating;

	private bool m_PauseAnimating;

	private bool m_AutoBattleAnimating;

	private bool m_AutoBattleState;

	private SkillGameData m_EnvironmentalEffect;

	private float m_buttonClickedTime;

	[SerializeField]
	private Animation m_PvPTutorialDisplay;

	[SerializeField]
	private AutoScalingTextBox m_PvPTutorialText;

	private bool m_canOpenConsumable;

	public bool CanOpenConsumable
	{
		get
		{
			return m_canOpenConsumable && m_BattleMgr.IsConsumableUsePossible;
		}
	}

	public bool CanCloseConsumable { get; private set; }

	public bool m_ConsumableButtonEnterable { get; set; }

	[method: MethodImpl(32)]
	public event Action ConsumableButtonClicked;

	public void ShowEnvEffectTooltip()
	{
		if (m_EnvironmentalEffect != null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowSkillOverlay(m_EnvEffectRoot.transform, null, m_EnvironmentalEffect, true);
		}
	}

	public void SetBattleMgr(BattleMgrBase battleMgr)
	{
		m_BattleMgr = battleMgr;
		if (m_BattleMgr.Model == null)
		{
			return;
		}
		m_RageMeter.SetBattleMgr(m_BattleMgr);
		m_ProgressIndicator.SetBattleMgr(battleMgr);
		m_ConsumableBar.SetBattleMgr(m_BattleMgr);
		m_InterfaceCamera = Camera.allCameras.FirstOrDefault((Camera c) => c.CompareTag("UICamera"));
		RegisterEventHandlers();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(0u);
		m_RageMeter.gameObject.SetActive(false);
		m_TurnIndicator.SetActive(false);
		bool flag = m_BattleMgr.Model.m_ChronicleCaveBattle || m_BattleMgr.Model.Balancing.NameId.StartsWith("battle_event");
		if (m_BattleMgr.Model.m_EnvironmentalEffects != null && m_BattleMgr.Model.m_EnvironmentalEffects.Count > 0 && flag)
		{
			m_EnvironmentalEffect = m_BattleMgr.Model.m_EnvironmentalEffects.Values.FirstOrDefault();
			if (m_EnvironmentalEffect != null)
			{
				m_EnvEffectIcon.spriteName = m_EnvironmentalEffect.m_SkillIconName;
			}
			battleMgr.Model.EnvironmentalEffectTriggered -= EnvironmentalEffectTriggered;
			battleMgr.Model.EnvironmentalEffectTriggered += EnvironmentalEffectTriggered;
		}
		if (m_BattleMgr.AutoBattle)
		{
			m_AutoBattleArrows.Play("AutoBattle_Active");
			m_AutoBattlePlay.gameObject.SetActive(false);
			m_AutoBattleStop.gameObject.SetActive(true);
			m_AutoBattleState = true;
		}
		else
		{
			m_AutoBattleArrows.Play("AutoBattle_Inactive");
			m_AutoBattlePlay.gameObject.SetActive(true);
			m_AutoBattleStop.gameObject.SetActive(false);
			m_AutoBattleState = false;
		}
		m_isInitialized = true;
	}

	private void EnvironmentalEffectTriggered(string skillName)
	{
		if (skillName == m_EnvironmentalEffect.SkillNameId && !m_EnvEffectRoot.GetComponent<Animation>().isPlaying)
		{
			m_EnvEffectRoot.GetComponent<Animation>().Play("EnvironmentalEffect_Focus");
		}
	}

	private void DeRegisterEventHandlers()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(0);
		m_ReviveBirdsButton.Clicked -= m_ReviveBirdsButton_Clicked;
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(0, HandleBackButton);
		m_ReviveBirdsButton.Clicked += m_ReviveBirdsButton_Clicked;
	}

	private void Update()
	{
		if (!m_BattleMgr)
		{
			return;
		}
		if (m_BattleMgr.IsConsumableUsePossible && !m_ConsumableButtonEntered && !m_ConsumableBarEntered && !m_ConsumableButtonAnimating && !m_ConsumableBarAnimating)
		{
			EnterConsumableButton();
		}
		if (!m_BattleMgr.IsConsumableUsePossible && m_ConsumableButtonEntered && !m_ConsumableButtonAnimating)
		{
			LeaveConsumableButton();
		}
		if (!m_BattleMgr.IsConsumableUsePossible && m_ConsumableBarEntered && !m_ConsumableBarAnimating)
		{
			LeaveConsumableBar();
		}
		if (!m_BattleMgr.IsPausePossible && m_PauseButtonEntered && !m_PauseAnimating)
		{
			LeavePauseButton();
		}
		if (m_BattleMgr.IsPausePossible && !m_PauseAnimating && !m_PauseButtonEntered)
		{
			EnterPauseButton();
		}
		if (!m_BattleMgr.IsAutoBattlePossible && m_AutoBattleButtonEntered && !m_AutoBattleAnimating)
		{
			LeaveAutoBattleButton();
		}
		if (m_BattleMgr.IsAutoBattlePossible && !m_AutoBattleAnimating && !m_AutoBattleButtonEntered)
		{
			EnterAutoBattleButton();
		}
		if (m_AutoBattleState != m_BattleMgr.AutoBattle)
		{
			if (m_BattleMgr.AutoBattle)
			{
				m_AutoBattleArrows.Play("AutoBattle_Active");
				m_AutoBattlePlay.gameObject.SetActive(false);
				m_AutoBattleStop.gameObject.SetActive(true);
				m_AutoBattleState = true;
			}
			else
			{
				m_AutoBattleArrows.Play("AutoBattle_Inactive");
				m_AutoBattlePlay.gameObject.SetActive(true);
				m_AutoBattleStop.gameObject.SetActive(false);
				m_AutoBattleState = false;
			}
		}
	}

	private void HandleBackButton()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		m_BattleMgr.m_BattlePaused.Enter();
		if (m_ConsumableBarEntered)
		{
			DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("back_button_pressed", string.Empty);
			LeaveConsumableBar();
		}
	}

	private void m_ReviveBirdsButton_Clicked()
	{
		if (IsButtonAlreadyClickedThisFrame())
		{
			return;
		}
		Requirement requirement = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").ReviveBirdsRequirement;
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (!currentPlayer.Data.FirstReviveUsed)
		{
			Requirement requirement2 = new Requirement();
			requirement2.Value = 0f;
			requirement2.NameId = requirement.NameId;
			requirement2.RequirementType = requirement.RequirementType;
			requirement = requirement2;
		}
		if (!DIContainerLogic.RequirementService.ExecuteRequirements(currentPlayer.InventoryGameData, new List<Requirement> { requirement }, "revive_birds"))
		{
			Requirement requirement3 = requirement;
			if (requirement3 != null && requirement3.RequirementType == RequirementType.PayItem)
			{
				IInventoryItemGameData data = null;
				if (DIContainerLogic.InventoryService.TryGetItemGameData(currentPlayer.InventoryGameData, requirement3.NameId, out data))
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
			if (!IsInvoking("UnpauseReviveTimer"))
			{
				m_BattleMgr.m_ShowRevivePopupTimeLeft = DIContainerLogic.GetPacingBalancing().RevivePopupShowTime;
				PauseReviveTimer(true);
				Invoke("UnpauseReviveTimer", DIContainerLogic.GetPacingBalancing().MissingCurrencyOverlayTime);
			}
		}
		else
		{
			if (m_BattleMgr.m_ShowRevivePopup)
			{
				currentPlayer.Data.FirstReviveUsed = true;
				m_ReviveBirdsButton.Clicked -= m_ReviveBirdsButton_Clicked;
				m_BattleMgr.ReviveAllBirds();
				DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateLuckyCoinsBar();
				LeaveReviveButton();
			}
			currentPlayer.SavePlayerData();
		}
	}

	private void ResetRevivePopup()
	{
		m_BattleMgr.m_ShowRevivePopupTimeLeft = DIContainerLogic.GetPacingBalancing().RevivePopupShowTime;
		m_BattleMgr.m_IsInShopForRevive = false;
		EnterReviveButton();
	}

	public void HideTurnIndicator()
	{
		DebugLog.Log("[BattleUIStateMgr] HideTurnIndicator");
		if (m_TurnIndicator != null)
		{
			m_TurnIndicator.GetComponent<Animation>().Play("Guide_BattleTurnHelper_Hide");
		}
	}

	public void ShowTurnIndicator(Faction lastTurnFaction, Faction nextTurnFaction)
	{
		DebugLog.Log(string.Concat("[BattleUIStateMgr] ShowTurnIndicator: ", lastTurnFaction, ", ", nextTurnFaction));
		if (DIContainerLogic.InventoryService.GetItemValue(m_BattleMgr.Model.m_ControllerInventory, "unlock_hide_turns") <= 0)
		{
			if (!m_TurnIndicator.activeInHierarchy)
			{
				DebugLog.Log("[BattleUIStateMgr] ShowTurnIndicator: activate in hierarchy");
				m_TurnIndicator.SetActive(true);
			}
			DebugLog.Log("[BattleUIStateMgr] ShowTurnIndicator: Changing animation state");
			if (lastTurnFaction == Faction.None && nextTurnFaction == Faction.Birds)
			{
				m_TurnIndicator.GetComponent<Animation>().Play("Guide_BattleTurnHelper_BirdTurn");
			}
			else if (lastTurnFaction == Faction.Birds && nextTurnFaction == Faction.Pigs)
			{
				m_TurnIndicator.GetComponent<Animation>().Play("Guide_BattleTurnHelper_ChangeToPigTurn");
			}
			else if (lastTurnFaction == Faction.Pigs && nextTurnFaction == Faction.Birds)
			{
				m_TurnIndicator.GetComponent<Animation>().Play("Guide_BattleTurnHelper_ChangeToBirdTurn");
			}
		}
	}

	public IEnumerator Enter()
	{
		if (m_BattleMgr.Model.Balancing.BattleParticipantsIds.Count > 1)
		{
			m_ProgressIndicator.Enter();
		}
		if (m_EnvironmentalEffect != null)
		{
			m_EnvEffectRoot.GetComponent<Animation>().Play("EnvironmentalEffect_Enter");
		}
		if (m_BattleMgr.Model.GetRageAvailable(Faction.Birds))
		{
			m_RageMeter.gameObject.SetActive(true);
			m_RageMeter.Enter();
		}
		if (m_BattleMgr.Model.IsPvP && DIContainerInfrastructure.GetCurrentPlayer().Data.PvPTutorialDisplayState == 1)
		{
			ShowPvPTutorial(1);
		}
		m_hasLeft = false;
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(2u);
		yield break;
	}

	public IEnumerator Leave()
	{
		if (m_BattleMgr.Model.Balancing.BattleParticipantsIds.Count > 1)
		{
			m_ProgressIndicator.Leave();
		}
		if (m_EnvironmentalEffect != null)
		{
			m_EnvEffectRoot.GetComponent<Animation>().Play("EnvironmentalEffect_Leave");
		}
		m_hasLeft = true;
		if (m_BattleMgr.Model.GetRageAvailable(Faction.Birds))
		{
			StartCoroutine(m_RageMeter.Leave());
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("battle_end");
		yield return new WaitForSeconds(0.25f);
	}

	public float EnterConsumableButton()
	{
		m_ConsumableButtonEntered = true;
		if ((bool)m_ConsumableBlocker)
		{
			if (m_BattleMgr.Model.m_IsConsumableBlocked)
			{
				m_ConsumableBlocker.SetActive(true);
			}
			else
			{
				m_ConsumableBlocker.SetActive(false);
			}
		}
		if (m_BattleMgr.Model.m_CombatantsByInitiative.Any((ICombatant c) => c.CanUseConsumable && c.CurrentHealth / c.ModifiedHealth <= m_WarningHealthPercent))
		{
			ShowConsumableHighlight(true);
		}
		else
		{
			ShowConsumableHighlight(false);
		}
		StartCoroutine(EnterConsumableButtonCoroutine());
		return 0f;
	}

	private void ShowConsumableHighlight(bool show)
	{
		if (show)
		{
			m_ConsumableButtonWarningAnim.Stop();
			m_ConsumableButtonWarningAnim.Play("ConsumableButton_Warning_Start");
			m_ConsumableButtonWarningAnim.PlayQueued("ConsumableButton_Warning_Loop");
		}
		else
		{
			m_ConsumableButtonWarningAnim.Stop();
			m_ConsumableButtonWarningAnim.Play("ConsumableButton_Warning_End");
		}
	}

	public float LeaveConsumableButton()
	{
		m_ConsumableButtonAnimating = true;
		m_ConsumableButtonEntered = false;
		DeRegisterConsumableEventHandlers();
		StartCoroutine(LeaveConsumableButtonCoroutine());
		return m_ConsumableButtonRoot.GetComponent<Animation>()["ConsumableButton_Leave"].length;
	}

	private IEnumerator LeaveConsumableButtonCoroutine()
	{
		m_ConsumableButtonRoot.GetComponent<Animation>().Play("ConsumableButton_Leave");
		yield return new WaitForSeconds(m_ConsumableButtonRoot.GetComponent<Animation>()["ConsumableButton_Leave"].length);
		m_ConsumableButtonAnimating = false;
		m_ConsumableButtonEntered = false;
	}

	private IEnumerator EnterConsumableButtonCoroutine()
	{
		m_ConsumableButtonRoot.GetComponent<Animation>().Play("ConsumableButton_Enter");
		yield return new WaitForSeconds(m_ConsumableButtonRoot.GetComponent<Animation>()["ConsumableButton_Enter"].length);
		RegisterConsumableEventHandlers();
		m_ConsumableButtonAnimating = false;
		m_ConsumableButtonEntered = true;
	}

	private void RegisterConsumableEventHandlers()
	{
		DeRegisterConsumableEventHandlers();
		m_ConsumableButton.Clicked += m_ConsumableButtonRoot_Clicked;
		m_canOpenConsumable = true;
	}

	private void DeRegisterConsumableEventHandlers()
	{
		m_ConsumableButton.Clicked -= m_ConsumableButtonRoot_Clicked;
		m_canOpenConsumable = false;
	}

	private void RegisterPauseEventHandlers()
	{
		DeRegisterPauseEventHandlers();
		m_PauseButton.Clicked += PauseButtonPressed;
	}

	private void DeRegisterPauseEventHandlers()
	{
		m_PauseButton.Clicked -= PauseButtonPressed;
	}

	private void RegisterAutoBattleEventHandlers()
	{
		DeRegisterAutoBattleEventHandlers();
		m_AutoBattleButton.Clicked += AutoBattleButtonPressed;
	}

	private void DeRegisterAutoBattleEventHandlers()
	{
		m_AutoBattleButton.Clicked -= AutoBattleButtonPressed;
	}

	private void m_ConsumableButtonRoot_Clicked()
	{
		if (!IsButtonAlreadyClickedThisFrame() && !m_BattleMgr.Model.m_IsConsumableBlocked)
		{
			if (this.ConsumableButtonClicked != null)
			{
				this.ConsumableButtonClicked();
			}
			LeaveConsumableButton();
			EnterConsumableBar();
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

	public float EnterConsumableBar()
	{
		m_ConsumableBarAnimating = true;
		m_BattleMgr.IsPausePossible = false;
		StartCoroutine(EnterConsumableBarCoroutine());
		return 0f;
	}

	private IEnumerator EnterConsumableBarCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("battle_consumable_enter");
		yield return StartCoroutine(Leave());
		m_ConsumableBarAnimating = false;
		m_ConsumableBarEntered = true;
		m_ConsumableBar.Enter();
		while (!DIContainerInfrastructure.BackButtonMgr.IsBackButtonAvailiable)
		{
			yield return new WaitForEndOfFrame();
		}
		CanCloseConsumable = true;
	}

	public float LeaveConsumableBar()
	{
		if (!m_ConsumableBarEntered && !m_ConsumableBarAnimating)
		{
			return 0f;
		}
		m_ConsumableBarAnimating = true;
		StartCoroutine(LeaveConsumableBarCoroutine());
		return 0f;
	}

	private IEnumerator LeaveConsumableBarCoroutine()
	{
		yield return StartCoroutine(m_ConsumableBar.LeaveCoroutine());
		m_ConsumableBarAnimating = false;
		m_ConsumableBarEntered = false;
		CanCloseConsumable = false;
		m_BattleMgr.IsPausePossible = true;
		StartCoroutine(Enter());
	}

	public void EnterPauseButton()
	{
		if (!IsButtonAlreadyClickedThisFrame() && !m_PauseButtonEntered)
		{
			m_PauseAnimating = true;
			DeRegisterPauseEventHandlers();
			StartCoroutine(EnterPauseButtonCoroutine());
		}
	}

	public void EnterAutoBattleButton()
	{
		if (!m_AutoBattleButtonEntered)
		{
			m_AutoBattleAnimating = true;
			DeRegisterAutoBattleEventHandlers();
			StartCoroutine(EnterAutoBattleButtonCoroutine());
		}
	}

	public IEnumerator EnterPauseButtonCoroutine()
	{
		m_PauseButtonRoot.GetComponent<Animation>().Play("PauseButton_Enter");
		yield return new WaitForSeconds(m_PauseButtonRoot.GetComponent<Animation>()["PauseButton_Enter"].length);
		m_PauseAnimating = false;
		RegisterPauseEventHandlers();
		m_PauseButtonEntered = true;
	}

	public IEnumerator EnterAutoBattleButtonCoroutine()
	{
		m_AutoBattleButtonRoot.GetComponent<Animation>().Play("AutoBattle_Enter");
		yield return new WaitForSeconds(m_AutoBattleButtonRoot.GetComponent<Animation>()["AutoBattle_Enter"].length);
		m_AutoBattleAnimating = false;
		RegisterAutoBattleEventHandlers();
		m_AutoBattleButtonEntered = true;
	}

	public void LeavePauseButton()
	{
		if (m_PauseButtonEntered)
		{
			m_PauseAnimating = true;
			DeRegisterPauseEventHandlers();
			StartCoroutine(LeavePauseButtonCoroutine());
		}
	}

	public void LeaveAutoBattleButton()
	{
		if (m_AutoBattleButtonEntered)
		{
			m_AutoBattleAnimating = true;
			DeRegisterAutoBattleEventHandlers();
			StartCoroutine(LeaveAutoBattleButtonCoroutine());
		}
	}

	public IEnumerator LeavePauseButtonCoroutine()
	{
		m_PauseButtonRoot.GetComponent<Animation>().Play("PauseButton_Leave");
		yield return new WaitForSeconds(m_PauseButtonRoot.GetComponent<Animation>()["PauseButton_Leave"].length);
		m_PauseAnimating = false;
		m_PauseButtonEntered = false;
	}

	public IEnumerator LeaveAutoBattleButtonCoroutine()
	{
		m_AutoBattleButtonRoot.GetComponent<Animation>().Play("AutoBattle_Leave");
		yield return new WaitForSeconds(m_AutoBattleButtonRoot.GetComponent<Animation>()["AutoBattle_Leave"].length);
		m_AutoBattleAnimating = false;
		m_AutoBattleButtonEntered = false;
	}

	public IEnumerator LeavePauseOnly()
	{
		if ((bool)m_PauseButtonRoot && m_PauseButtonEntered && !m_BattleMgr.m_BattleMainLoopDone)
		{
			m_PauseButtonRoot.GetComponent<Animation>().Play("PauseButton_Leave");
			yield return new WaitForSeconds(m_PauseButtonRoot.GetComponent<Animation>()["PauseButton_Leave"].length);
			m_PauseButtonEntered = false;
		}
	}

	public IEnumerator LeaveAutoBattleOnly()
	{
		if ((bool)m_AutoBattleButtonRoot && m_AutoBattleButtonEntered && !m_BattleMgr.m_BattleMainLoopDone)
		{
			m_AutoBattleButtonRoot.GetComponent<Animation>().Play("AutoBattle_Leave");
			yield return new WaitForSeconds(m_AutoBattleButtonRoot.GetComponent<Animation>()["AutoBattle_Leave"].length);
			m_AutoBattleButtonEntered = false;
		}
	}

	public IEnumerator EnterPauseOnly()
	{
		if ((bool)m_PauseButtonRoot && !m_PauseButtonEntered && !m_BattleMgr.m_BattleMainLoopDone)
		{
			m_PauseButtonEntered = true;
			m_PauseButtonRoot.GetComponent<Animation>().Play("PauseButton_Enter");
			yield return new WaitForSeconds(m_PauseButtonRoot.GetComponent<Animation>()["PauseButton_Enter"].length);
		}
	}

	public IEnumerator EnterAutoBattleOnly()
	{
		if ((bool)m_AutoBattleButtonRoot && !m_AutoBattleButtonEntered && !m_BattleMgr.m_BattleMainLoopDone)
		{
			m_AutoBattleButtonEntered = true;
			m_AutoBattleButtonRoot.GetComponent<Animation>().Play("AutoBattle_Enter");
			yield return new WaitForSeconds(m_AutoBattleButtonRoot.GetComponent<Animation>()["AutoBattle_Enter"].length);
		}
	}

	public void UpdateProgressBar()
	{
		StartCoroutine(m_ProgressIndicator.UpdateProgress());
	}

	public void PauseButtonPressed()
	{
		m_BattleMgr.m_BattlePaused.Enter();
	}

	public void AutoBattleButtonPressed()
	{
		CancelInvoke("AutoBattlePressed");
		Invoke("AutoBattlePressed", 0.1f);
	}

	public void EnterPause()
	{
		m_BattleMgr.m_BattlePaused.Enter();
	}

	public void AutoBattlePressed()
	{
		m_BattleMgr.AutoBattle = !m_BattleMgr.AutoBattle;
		if (m_BattleMgr.AutoBattle)
		{
			m_AutoBattleArrows.Play("AutoBattle_Active");
			m_AutoBattlePlay.gameObject.SetActive(false);
			m_AutoBattleStop.gameObject.SetActive(true);
			m_AutoBattleState = true;
		}
		else
		{
			m_AutoBattleArrows.Play("AutoBattle_Inactive");
			m_AutoBattlePlay.gameObject.SetActive(true);
			m_AutoBattleStop.gameObject.SetActive(false);
			m_AutoBattleState = false;
		}
	}

	public void OnDisable()
	{
		if (m_BattleMgr != null && m_BattleMgr.Model != null)
		{
			m_BattleMgr.Model.EnvironmentalEffectTriggered -= EnvironmentalEffectTriggered;
		}
		DeRegisterEventHandlers();
	}

	public void EnterReviveButton()
	{
		LeavePauseOnly();
		LeaveAutoBattleOnly();
		m_SingleBirdRevivePopup.Leave();
		m_ReviveBirdsButtonRoot.SetActive(true);
		m_ReviveBirdsButtonRoot.GetComponent<Animation>().Play("ReviveBirdsButton_Enter");
		Requirement requirement = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").ReviveBirdsRequirement;
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (!currentPlayer.Data.FirstReviveUsed)
		{
			Requirement requirement2 = new Requirement();
			requirement2.Value = 0f;
			requirement2.NameId = requirement.NameId;
			requirement2.RequirementType = requirement.RequirementType;
			requirement = requirement2;
		}
		IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(requirement.NameId);
		m_ReviveBirdsCost.SetModel(balancingData.AssetBaseId, null, requirement.Value, string.Empty);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.IsInBattle = false;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(0);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 1u,
			showLuckyCoins = true
		}, true);
		m_BattleMgr.m_ShowRevivePopupTimeLeft = DIContainerLogic.GetPacingBalancing().RevivePopupShowTime;
		m_ReviveBirdsButton.Clicked -= m_ReviveBirdsButton_Clicked;
		m_ReviveBirdsButton.Clicked += m_ReviveBirdsButton_Clicked;
		PauseReviveTimer(false);
	}

	private void UnpauseReviveTimer()
	{
		PauseReviveTimer(false);
	}

	public void PauseReviveTimer(bool pause)
	{
		if (pause)
		{
			StopCoroutine("ProgressReviveTimer");
			CancelInvoke("CountReviveTimer");
			return;
		}
		CancelInvoke("CountReviveTimer");
		StopCoroutine("ProgressReviveTimer");
		StartCoroutine("ProgressReviveTimer");
		InvokeRepeating("CountReviveTimer", 0f, 1f);
	}

	private void CountReviveTimer()
	{
		m_ReviveCounterAnim.Stop();
		m_ReviveCounterAnim.Play("ReviveBirdsCounter_Count");
		m_ReviveProgressTimer.text = m_BattleMgr.m_ShowRevivePopupTimeLeft.ToString("0");
	}

	public void InstantResetRevivePopup()
	{
		m_ReviveProgressBar.fillAmount = 1f;
		m_ReviveProgressTimer.text = m_BattleMgr.m_ShowRevivePopupTimeLeft.ToString("0");
		StopCoroutine("ProgressReviveTimer");
		StartCoroutine("ProgressReviveTimer");
	}

	private IEnumerator ProgressReviveTimer()
	{
		while (m_BattleMgr.m_ShowRevivePopupTimeLeft > 0f)
		{
			m_ReviveProgressBar.fillAmount = m_BattleMgr.m_ShowRevivePopupTimeLeft / DIContainerLogic.GetPacingBalancing().RevivePopupShowTime;
			yield return new WaitForEndOfFrame();
		}
	}

	public void LeaveReviveButton()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(0, HandleBackButton);
		m_ReviveBirdsButtonRoot.GetComponent<Animation>().Play("ReviveBirdsButton_Leave");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(1u);
	}

	internal void LockConsumableBar(bool locked)
	{
		m_lockedConsumables = locked;
		if (!locked)
		{
		}
	}

	public void ShowSingleRevivePopup(ICombatant sender)
	{
		m_BattleMgr.IsPausePossible = false;
		m_BattleMgr.IsConsumableUsePossible = false;
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.IsInBattle = false;
		m_SingleBirdRevivePopup.gameObject.SetActive(true);
		m_SingleBirdRevivePopup.SetModel(this, sender);
		m_SingleBirdRevivePopup.Enter();
	}

	public void LeaveSingleRevivePopup()
	{
		m_SingleBirdRevivePopup.Leave();
	}

	public float ShowPvPTutorial(int step)
	{
		m_PvPTutorialText.SetText(DIContainerInfrastructure.GetLocaService().Tr("pvp_battle_tutorial" + step.ToString("D2")));
		m_PvPTutorialDisplay.Play("Guide_PvPTutorial_Open");
		return m_PvPTutorialDisplay.GetClip("Guide_PvPTutorial_Open").length;
	}

	public float HidePvpTutorial()
	{
		m_PvPTutorialDisplay.Play("Guide_PvPTutorial_Close");
		return m_PvPTutorialDisplay.GetClip("Guide_PvPTutorial_Close").length;
	}

	public IEnumerator HideAndShowPvPTip(int stepIndex)
	{
		yield return new WaitForSeconds(HidePvpTutorial() + 0.35f);
		ShowPvPTutorial(stepIndex);
	}
}

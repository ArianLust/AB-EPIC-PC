using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.Shared.BalancingData;
using UnityEngine;

public class GenericUIStateMgr : MonoBehaviour
{
	public enum PlayerStatsType
	{
		Snoutlings = 0,
		LuckyCoins = 1,
		FriendshipEssence = 2,
		Energy = 3
	}

	public XPBarController m_XPBarController;

	public GameObject m_LevelDisplay;

	public UILabel m_LevelLabel;

	[SerializeField]
	private List<GameObject> m_MaxLevelDisplays = new List<GameObject>();

	public List<BarRegistry> m_RegisteredBars = new List<BarRegistry>();

	[SerializeField]
	private UISprite m_LevelProgres;

	[SerializeField]
	public List<PlayerStatControllerEnterInfo> m_PlayerStatsController;

	[SerializeField]
	private UIGrid m_playerStatsGrid;

	private int neededXPForLevelUp;

	private bool m_Initialized;

	private bool m_LevelDisplayShow;

	private bool m_LevelDisplayEntered;

	[SerializeField]
	private NonInteractableTooltipController m_TTController;

	private bool m_isTooltipShowing;

	private bool m_breakTooltip;

	private bool m_IsInBattle;

	public bool IsInBattle
	{
		get
		{
			return m_IsInBattle && !DIContainerInfrastructure.GetCoreStateMgr().IsShopOpen();
		}
		set
		{
			m_IsInBattle = value;
		}
	}

	private void Awake()
	{
		if (DIContainerInfrastructure.GetCoreStateMgr() != null)
		{
			base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI = this;
		}
		ResetInventory();
	}

	private void Start()
	{
		m_Initialized = true;
	}

	public void Update()
	{
		if (!m_Initialized || IsInBattle)
		{
			return;
		}
		if (m_LevelDisplayShow != m_LevelDisplayEntered)
		{
			if (m_LevelDisplayShow)
			{
				EnterLevelDisplayNow();
			}
			else
			{
				LeaveLevelDisplayNow();
			}
		}
		foreach (PlayerStatControllerEnterInfo item in m_PlayerStatsController)
		{
			if (item.m_HasToBeShown)
			{
				if (!item.m_Entered && !item.m_Animating)
				{
					StartCoroutine(EnterAndStayStatsBar(item));
				}
				else if (item.m_Entered && !item.m_Animating && item.m_PositionIndexChanged)
				{
					StartCoroutine(LeaveAndEnterStatsBar(item));
				}
				item.m_PositionIndexChanged = false;
				item.m_StatBar.SetShopLink(item.m_ShowShopLink);
			}
			else if (!item.m_HasToBeShown && item.m_Entered && !item.m_Animating)
			{
				StartCoroutine(LeavePlayerStatsBar(item));
			}
		}
		m_playerStatsGrid.Reposition();
	}

	public void ReInitialize()
	{
		ResetInventory();
		LeaveLevelDisplay();
		m_XPBarController.ResetXP();
	}

	private void ResetInventory()
	{
		foreach (PlayerStatControllerEnterInfo item in m_PlayerStatsController)
		{
			item.m_StatBar.SetInventory(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData);
		}
	}

	public void RemoveHandlers()
	{
		m_XPBarController.RemoveEventHandlers();
	}

	public bool IsPlayingLevelUp()
	{
		return m_XPBarController.IsPlayingLevelUp();
	}

	public void BlockShopLinks(bool block)
	{
		foreach (PlayerStatControllerEnterInfo item in m_PlayerStatsController)
		{
			if (block)
			{
				item.m_StatBar.DeRegisterEventHandlers();
			}
			else if (item.m_ShowShopLink)
			{
				item.m_StatBar.RegisterEventHandlers();
			}
		}
	}

	public void EnterPlayerStatControllerInBattle(PlayerStatsType statsType, BattleUIStateMgr battleMgr = null)
	{
		m_PlayerStatsController[(int)statsType].m_StatBar.Enter(battleMgr);
		m_playerStatsGrid.enabled = true;
	}

	private void EnterPlayerStatController(PlayerStatsType index, bool showShopLink = false)
	{
		m_PlayerStatsController[(int)index].m_StatBar.UpdateValueOnly();
		m_PlayerStatsController[(int)index].m_ShowShopLink = showShopLink;
		m_PlayerStatsController[(int)index].m_HasToBeShown = true;
		List<PlayerStatControllerEnterInfo> list = (from s in m_PlayerStatsController
			where s.m_HasToBeShown
			orderby s.m_Priority
			select s).ToList();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].m_PositionIndexChanged = list[i].m_PositionIndex != i;
			list[i].m_PositionIndex = i;
		}
	}

	public void RegisterBar(BarRegistry barRegistry, bool shopLink)
	{
		bool flag = false;
		for (int i = 0; i < m_RegisteredBars.Count; i++)
		{
			if (m_RegisteredBars[i].Depth == barRegistry.Depth)
			{
				m_RegisteredBars[i] = barRegistry;
				flag = true;
			}
		}
		if (!flag)
		{
			m_RegisteredBars.Add(barRegistry);
		}
		m_RegisteredBars = m_RegisteredBars.OrderBy((BarRegistry b) => b.Depth).ToList();
		EnterCurrentBar(shopLink);
	}

	public void DeRegisterBar(uint depth)
	{
		bool flag = false;
		BarRegistry barRegistry = null;
		for (int num = m_RegisteredBars.Count - 1; num >= 0; num--)
		{
			if (m_RegisteredBars[num].Depth == depth)
			{
				if (num == m_RegisteredBars.Count - 1)
				{
					flag = true;
				}
				barRegistry = m_RegisteredBars[num];
				m_RegisteredBars.Remove(m_RegisteredBars[num]);
			}
		}
		if (flag && barRegistry != null)
		{
			if (barRegistry.showFriendshipEssence)
			{
				LeavePlayerStatController(PlayerStatsType.FriendshipEssence);
			}
			if (barRegistry.showLuckyCoins)
			{
				LeavePlayerStatController(PlayerStatsType.LuckyCoins);
			}
			if (barRegistry.showSnoutlings)
			{
				LeavePlayerStatController(PlayerStatsType.Snoutlings);
			}
			if (barRegistry.showEnergy)
			{
				LeavePlayerStatController(PlayerStatsType.Energy);
			}
		}
		EnterCurrentBar(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideMissingCurrencyOverlay();
	}
	
	public void ShowLevelTooltip()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowXPOverlayOverlay(m_LevelDisplay.transform, DIContainerInfrastructure.GetCurrentPlayer(), interfaceCam: true);
	}

	private void EnterCurrentBar(bool shopLink)
	{
		BarRegistry barRegistry = m_RegisteredBars.LastOrDefault();
		if (barRegistry != null)
		{
			if (barRegistry.showFriendshipEssence)
			{
				EnterPlayerStatController(PlayerStatsType.FriendshipEssence, shopLink);
			}
			else
			{
				LeavePlayerStatController(PlayerStatsType.FriendshipEssence);
			}
			if (barRegistry.showSnoutlings)
			{
				EnterPlayerStatController(PlayerStatsType.Snoutlings, shopLink);
			}
			else
			{
				LeavePlayerStatController(PlayerStatsType.Snoutlings);
			}
			if (barRegistry.showLuckyCoins)
			{
				EnterPlayerStatController(PlayerStatsType.LuckyCoins, shopLink);
			}
			else
			{
				LeavePlayerStatController(PlayerStatsType.LuckyCoins);
			}
			if (barRegistry.showEnergy)
			{
				EnterPlayerStatController(PlayerStatsType.Energy, shopLink);
			}
			else
			{
				LeavePlayerStatController(PlayerStatsType.Energy);
			}
		}
	}

	public void ResetRegistration()
	{
		m_RegisteredBars.Clear();
	}

	public void StopCoinUpdates()
	{
		m_PlayerStatsController[0].m_StatBar.StopBattleEntering();
	}

	private IEnumerator LeaveAndEnterStatsBar(PlayerStatControllerEnterInfo playerStatControllerEnterInfo)
	{
		playerStatControllerEnterInfo.m_Entered = false;
		playerStatControllerEnterInfo.m_Animating = true;
		playerStatControllerEnterInfo.m_StatBar.DeRegisterEventHandlers();
		yield return new WaitForSeconds(playerStatControllerEnterInfo.m_StatBar.Leave());
		m_playerStatsGrid.enabled = true;
		yield return new WaitForSeconds(playerStatControllerEnterInfo.m_StatBar.EnterAndStay(playerStatControllerEnterInfo.m_ShowShopLink));
		playerStatControllerEnterInfo.m_StatBar.RegisterEventHandlers();
		playerStatControllerEnterInfo.m_Animating = false;
		playerStatControllerEnterInfo.m_Entered = true;
	}

	private IEnumerator LeavePlayerStatsBar(PlayerStatControllerEnterInfo playerStatControllerEnterInfo)
	{
		playerStatControllerEnterInfo.m_StatBar.DeRegisterEventHandlers();
		playerStatControllerEnterInfo.m_Entered = false;
		playerStatControllerEnterInfo.m_Animating = true;
		yield return new WaitForSeconds(playerStatControllerEnterInfo.m_StatBar.Leave());
		playerStatControllerEnterInfo.m_Animating = false;
		playerStatControllerEnterInfo.m_StatBar.UpdateValueOnly();
	}

	private IEnumerator EnterAndStayStatsBar(PlayerStatControllerEnterInfo playerStatControllerEnterInfo)
	{
		playerStatControllerEnterInfo.m_StatBar.UpdateValueOnly();
		playerStatControllerEnterInfo.m_Animating = true;
		m_playerStatsGrid.enabled = true;
		yield return new WaitForSeconds(playerStatControllerEnterInfo.m_StatBar.EnterAndStay(playerStatControllerEnterInfo.m_ShowShopLink));
		playerStatControllerEnterInfo.m_StatBar.RegisterEventHandlers();
		playerStatControllerEnterInfo.m_Entered = true;
		playerStatControllerEnterInfo.m_Animating = false;
	}

	private void LeavePlayerStatController(PlayerStatsType statsType)
	{
		m_PlayerStatsController[(int)statsType].m_StatBar.UpdateValueOnly();
		m_PlayerStatsController[(int)statsType].m_HasToBeShown = false;
	}

	public float LeaveAllBars(bool forced = false)
	{
		if (forced)
		{
			foreach (int value in Enum.GetValues(typeof(PlayerStatsType)))
			{
				LeavePlayerStatController((PlayerStatsType)value);
			}
			m_RegisteredBars.Clear();
		}
		float result = 0f;
		foreach (PlayerStatControllerEnterInfo item in m_PlayerStatsController)
		{
			result = item.m_StatBar.GetEnterDuration();
			item.m_HasToBeShown = false;
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideMissingCurrencyOverlay();
		}
		return result;
	}

	public bool UpdateAllBars()
	{
		UpdateCoinsBar();
		UpdateFriendshipEssenceBar();
		UpdateLuckyCoinsBar();
		UpdateEventEnergyBar();
		return true;
	}

	public void EnableEnergyTimer(bool enable)
	{
		GameObject timerRoot = m_PlayerStatsController[3].m_StatBar.m_TimerRoot;
		if (timerRoot != null)
		{
			timerRoot.SetActive(enable);
		}
	}

	public void SetEnergyTimer(string time)
	{
		UILabel timerLabel = m_PlayerStatsController[3].m_StatBar.m_TimerLabel;
		if (timerLabel != null)
		{
			timerLabel.text = time;
		}
	}

	public float UpdateCoinsBar(bool singleStep = false)
	{
		return m_PlayerStatsController[0].m_StatBar.UpdateAnim(singleStep);
	}

	public float UpdateLuckyCoinsBar()
	{
		return m_PlayerStatsController[1].m_StatBar.UpdateAnim();
	}

	public float UpdateEventEnergyBar()
	{
		return m_PlayerStatsController[3].m_StatBar.UpdateAnim();
	}

	public float UpdateFriendshipEssenceBar()
	{
		return m_PlayerStatsController[2].m_StatBar.UpdateAnim();
	}

	public void AddLuckyCoinsOnlyToUi(int amount)
	{
		m_PlayerStatsController[1].m_StatBar.AddOnlyUI(amount);
	}

	public CoinBarController GetLuckyCoinController()
	{
		return GetControllerForResourceBar("lucky_coin");
	}

	public CoinBarController GetControllerForResourceBar(string resourceNameId)
	{
		switch (resourceNameId)
		{
		case "lucky_coin":
			return m_PlayerStatsController[1].m_StatBar;
		case "gold":
			return m_PlayerStatsController[0].m_StatBar;
		case "friendship_essence":
			return m_PlayerStatsController[2].m_StatBar;
		default:
			return null;
		}
	}

	public void EnterLevelDisplay()
	{
		ExperienceLevelBalancingData balancing;
		if (DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + DIContainerInfrastructure.GetCurrentPlayer().Data.Level.ToString("00"), out balancing))
		{
			neededXPForLevelUp = balancing.Experience;
			if ((bool)m_LevelLabel)
			{
				m_LevelLabel.text = DIContainerInfrastructure.GetCurrentPlayer().Data.Level.ToString("0");
			}
			foreach (GameObject maxLevelDisplay in m_MaxLevelDisplays)
			{
				maxLevelDisplay.SetActive(false);
			}
		}
		else
		{
			neededXPForLevelUp = (int)DIContainerInfrastructure.GetCurrentPlayer().Data.Experience;
			if ((bool)m_LevelLabel)
			{
				m_LevelLabel.text = DIContainerInfrastructure.GetCurrentPlayer().Data.Level.ToString("0");
			}
			foreach (GameObject maxLevelDisplay2 in m_MaxLevelDisplays)
			{
				maxLevelDisplay2.SetActive(true);
			}
		}
		m_LevelProgres.fillAmount = DIContainerInfrastructure.GetCurrentPlayer().Data.Experience / (float)neededXPForLevelUp;
		m_LevelDisplayShow = true;
	}

	public void LeaveLevelDisplay()
	{
		m_LevelDisplayShow = false;
	}

	private void LeaveLevelDisplayNow()
	{
		m_LevelDisplayEntered = false;
		m_LevelDisplay.GetComponent<Animation>().Play("Display_Top_Leave");
	}

	private void EnterLevelDisplayNow()
	{
		m_LevelDisplayEntered = true;
		m_LevelDisplay.GetComponent<Animation>().Play("Display_Top_Enter");
	}

	public void ShowNonInteractableTooltip(string iconAssetID, string headerLocaIdent, string descLocaIdent, float duration = 3f, float delay = 0f, Dictionary<string, string> replacementDictionary = null)
	{
		if (m_isTooltipShowing)
		{
			DebugLog.Log("Tooltip already active!");
			return;
		}
		m_isTooltipShowing = true;
		m_TTController.SetTooltip(iconAssetID, headerLocaIdent, descLocaIdent, replacementDictionary);
		StartCoroutine(EnterNonInteractableTooltip(duration, delay));
	}

	private IEnumerator EnterNonInteractableTooltip(float duration, float waitForIt)
	{
		m_breakTooltip = false;
		if (waitForIt > 0f)
		{
			yield return new WaitForSeconds(waitForIt);
		}
		while (DIContainerInfrastructure.GetCoreStateMgr().IsAnyPopupActive)
		{
			yield return new WaitForSeconds(1f);
		}
		if (m_breakTooltip)
		{
			m_breakTooltip = false;
			yield break;
		}
		float timeToLeave = m_TTController.Enter();
		yield return new WaitForSeconds(timeToLeave + duration);
		LeaveNonInteractableTooltip();
	}

	public void LeaveNonInteractableTooltip()
	{
		m_breakTooltip = true;
		if (m_isTooltipShowing)
		{
			m_TTController.Leave();
			m_isTooltipShowing = false;
		}
	}
}

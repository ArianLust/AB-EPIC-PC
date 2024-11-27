using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using UnityEngine;

public class PopupDemotionStateMgr : MonoBehaviour
{
	[SerializeField]
	private UIInputTrigger m_AbortButton;

	[SerializeField]
	private UILabel m_LeagueLabel;

	[SerializeField]
	private UILabel m_DemotionDescLabel;

	[SerializeField]
	private UISprite m_LeagueCrown;

	private IInventoryItemGameData m_LeagueItemGameData;

	public bool m_IsShowing;

	private void Awake()
	{
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
	}

	private void Start()
	{
		Enter();
	}

	public void Enter()
	{
		m_IsShowing = true;
		if (!DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "pvp_league_crown", out m_LeagueItemGameData))
		{
			Object.Destroy(base.gameObject);
			return;
		}
		int level = m_LeagueItemGameData.ItemData.Level;
		m_LeagueCrown.spriteName = PvPSeasonManagerGameData.GetLeagueAssetName(level);
		m_LeagueLabel.text = DIContainerInfrastructure.GetLocaService().GetLeagueName(level);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("{value_1}", DIContainerInfrastructure.GetLocaService().GetLeagueName(level));
		m_DemotionDescLabel.text = DIContainerInfrastructure.GetLocaService().Tr("leaguefinished_noparticipation_desc", dictionary);
		base.gameObject.SetActive(true);
		StartCoroutine("EnterCoroutine");
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_noleagueparticipation_anim");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u
		}, true);
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_NoLeagueParticipation_Enter"));
		RegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_noleagueparticipation_anim");
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(4, OnAbortButtonClicked);
		m_AbortButton.Clicked += OnAbortButtonClicked;
	}

	private void DeRegisterEventHandlers()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(4);
		m_AbortButton.Clicked -= OnAbortButtonClicked;
	}

	private IEnumerator LeaveCoroutine()
	{
		DeRegisterEventHandlers();
		PvPSeasonManagerGameData CurrentPvPSeasonGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData;
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_noleagueparticipation_anim");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(5u);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_NoLeagueParticipation_Leave"));
		m_IsShowing = false;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_noleagueparticipation_anim");
		Object.Destroy(base.gameObject);
		base.gameObject.SetActive(false);
	}

	private void OnAbortButtonClicked()
	{
		StartCoroutine("LeaveCoroutine");
	}
}

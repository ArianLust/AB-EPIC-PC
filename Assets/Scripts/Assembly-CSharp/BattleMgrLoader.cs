using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

public class BattleMgrLoader : BattleMgr
{
	[SerializeField]
	private Transform m_SceneryRoot;

	private List<GenericAssetProvider> m_BattlegroundAssetProviders;

	private BattlegroundScenery m_BattlegroundScenery;

	private new void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			OnApplicationResumed();
		}
	}

	protected override void OnApplicationResumed()
	{
		if (m_isInitialized && !m_BattleMainLoopDone && m_InterfaceCamera.gameObject.activeInHierarchy && (base.IsPausePossible || m_BattleUI.CanCloseConsumable) && DIContainerInfrastructure.GetCurrentPlayer().GetCurrentWorldProgress() > 1)
		{
			m_BattlePaused.Enter();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.ForceLoading = true;
		DebugLog.Log(GetType(), "Awake: Forcing loadingscreen to stay until battleground is loaded");
		StartCoroutine(InitPrefabs());
	}

	private IEnumerator InitPrefabs()
	{
		if (m_BattlegroundAssetProviders == null || m_BattlegroundAssetProviders.Count == 0)
		{
			m_BattlegroundAssetProviders = new List<GenericAssetProvider>();
		}
		DebugLog.Log(GetType(), "InitPrefabs: searching assetprovider for: " + m_Model.m_BattleGroundName + ". Number of providers: " + m_BattlegroundAssetProviders.Count);
		DebugLog.Log(GetType(), "InitPrefabs: AssetProvider initializing...");
		foreach (GenericAssetProvider ap in m_BattlegroundAssetProviders)
		{
			if (ap.ContainsAsset(m_Model.m_BattleGroundName))
			{
				DebugLog.Log(GetType(), "InitPrefabs: AssetProvider for Scenery found");
				GameObject scenery = ap.InstantiateObject(m_Model.m_BattleGroundName, m_SceneryRoot, Vector3.zero, Quaternion.identity);
				DebugLog.Log(GetType(), "InitPrefabs() Is Battleground Scenery instantiated? " + (scenery != null));
				m_BattlegroundScenery = scenery.GetComponent<BattlegroundScenery>();
				break;
			}
		}
		if (m_BattlegroundScenery == null)
		{
			DebugLog.Error("Assetprovider not found for battleground: " + m_Model.m_BattleGroundName);
			yield break;
		}
		m_ScenePerWaveSettings = m_BattlegroundScenery.m_ScenePerWaveSettings;
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.ForceLoading = false;
	}

	protected override IEnumerator Start()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("battle_init");
		m_LoadedLevels.Add("Menu_Battleground", false);
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Menu_Battleground", base.OnWindowMenuBattlegroundLoaded);
		yield return new WaitForEndOfFrame();
		m_LoadedLevels.Add("Popup_BattlePaused", false);
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Popup_BattlePaused", base.OnPopupBattlePausedLoaded);
		yield return new WaitForEndOfFrame();
		DIContainerInfrastructure.AdService.AddPlacement("LevelStartInterstitial");
		while (m_LoadedLevels.Values.Count((bool e) => !e) > 0)
		{
			yield return new WaitForEndOfFrame();
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(0u);
		yield return new WaitForEndOfFrame();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.IsInBattle = true;
		if (DIContainerInfrastructure.AdService.ShowAd("LevelStartInterstitial"))
		{
			DIContainerInfrastructure.AdService.MutedGameSoundForPlacement("LevelStartInterstitial");
			while (DIContainerInfrastructure.GetCoreStateMgr().m_AllInputBlocked)
			{
				yield return new WaitForEndOfFrame();
			}
		}
		m_BattleUI.SetBattleMgr(this);
		m_BattlePaused.SetBattleMgr(this);
		DeRegisterEventHandlers();
		RegisterEventHandlers();
		if ((bool)m_StorySequencesRoot)
		{
			m_ActionTrees = m_StorySequencesRoot.GetComponentsInChildren<ExecuteBattleActionTree>();
		}
		else
		{
			m_ActionTrees = new ExecuteBattleActionTree[0];
		}
		DebugLog.Log("Battle started!");
		IInventoryItemGameData data;
		if (DIContainerLogic.InventoryService.TryGetItemGameData(m_Model.m_ControllerInventory, "gold", out data))
		{
			m_Model.m_CoinsAtBattleStart = data.ItemValue;
		}
		ExecuteBattleActionTree tree = GetActionTreeForBattle(m_Model.CurrentWaveIndex, false, true);
		if (tree != null)
		{
			if (tree.m_ExecuteBeforeWave)
			{
				if (tree.m_BlockBattleExecution)
				{
					yield return StartCoroutine(ExecuteActionTree(tree));
				}
				else
				{
					StartCoroutine(ExecuteActionTree(tree));
				}
			}
		}
		else if ((bool)m_GenericStartActionTree)
		{
			if (!base.Model.IsPvP)
			{
				yield return StartCoroutine(ExecuteActionTree(m_GenericStartActionTree));
			}
			else
			{
				StartCoroutine(ExecuteActionTree(m_GenericStartActionTree));
			}
		}
		yield return StartCoroutine(SetupInitialPositions());
		for (int i = 0; i < 11; i++)
		{
			m_CharacterInteractionBlockedItems.Add(Object.Instantiate(m_CharacterInteractionBlockedPrefab));
		}
		foreach (GameObject item in m_CharacterInteractionBlockedItems)
		{
			item.SetActive(false);
			item.transform.parent = m_BattleArea;
		}
		if (base.Model.IsPvP)
		{
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddLevel("Popup_CoinFlip", true, false, delegate
			{
				m_CoinFlipLoaded = true;
			});
			yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().DelayBeforePvPCoinflip);
			while (!m_CoinFlipLoaded)
			{
				yield return new WaitForEndOfFrame();
			}
			GameObject coinflipGameObject = GameObject.Find("1_CoinFlip");
			yield return new WaitForSeconds(coinflipGameObject.PlayAnimationOrAnimatorState(m_Model.m_PigsStartTurn ? "CoinFlip_OpponentWins" : "CoinFlip_PlayerWins"));
			Object.Destroy(coinflipGameObject.transform.parent.gameObject);
		}
		StartCoroutine(m_BattleUI.Enter());
		DIContainerLogic.GetBattleService().AddPassiveEffects(m_Model);
		if (DIContainerInfrastructure.GetCurrentPlayer().GetCurrentWorldProgress() == 1)
		{
			m_BattleUI.m_PauseButton.gameObject.SetActive(false);
			DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("first_battle");
		}
		base.IsPausePossible = true;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("battle_init");
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave();
		DIContainerInfrastructure.GetCoreStateMgr().m_PopupRoot.Leave();
		yield return new WaitForSeconds(0.5f);
		m_isInitialized = true;
		foreach (ICombatant combatant in m_Model.m_CombatantsByInitiative)
		{
			DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(0f, EffectTriggerType.Instant, combatant, null);
		}
		StartCoroutine("DoNextStep");
	}
}

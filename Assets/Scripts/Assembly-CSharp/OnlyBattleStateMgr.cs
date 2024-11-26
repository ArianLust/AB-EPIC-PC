using System;
using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.Shared.BalancingData;
using SmoothMoves;
using UnityEngine;

public class OnlyBattleStateMgr : CoreStateMgr
{
	public int BirdsLevel = 1;

	public string m_BattleBalancingNameId;

	public List<string> m_BirdBalancingNameIds;

	public string m_BattlegroundNameId = "DefaultBattleground";

	public string m_DefaultInventoryNameId = "player_inventory";

	public InventoryView AdditionalPlayerItems;

	private bool m_balancingDataInitialized;

	public List<string> m_PvPBirdBalancingNameIds = new List<string>();

	protected override void Awake()
	{
		CoreStateMgr.Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		DIContainerInfrastructure.GetLocaService().InitDefaultLoca(this);
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(AnimationManager));
		for (int num = array.Length - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(array[num]);
		}
	}

	protected override IEnumerator Start()
	{
		DIContainerInfrastructure.GetVersionService().Init();
		DIContainerBalancing.OnBalancingDataInitialized += delegate
		{
			m_balancingDataInitialized = true;
		};
		DIContainerBalancing.Init();
		while (!m_balancingDataInitialized)
		{
			yield return new WaitForEndOfFrame();
		}
		DIContainerInfrastructure.GetLocaService().InitDefaultLoca(this);
		while (!DIContainerInfrastructure.GetLocaService().Initialized)
		{
			yield return new WaitForSeconds(0.1f);
		}
		DIContainerInfrastructure.InitCurrentPlayerIfNecessary();
		base.SceneLoadingMgr.AddUILevel("LoadingScreen");
		base.SceneLoadingMgr.AddUILevel("DisplayElements");
		base.SceneLoadingMgr.AddUILevel("StorySequence");
		base.SceneLoadingMgr.AddUILevel("InfoOverlays");
		base.SceneLoadingMgr.AddUILevel("Window_Root");
		base.SceneLoadingMgr.AddUILevel("Popup_Root");
		yield return new WaitForSeconds(1f);
		InitAndEnterBattle();
	}

	private void InitAndEnterBattle()
	{
		BattleStartGameData battleStartGameData = new BattleStartGameData();
		battleStartGameData.callback = OnBattleDone;
		battleStartGameData.m_BackgroundAssetId = m_BattlegroundNameId;
		battleStartGameData.m_BattleBalancingNameId = m_BattleBalancingNameId;
		battleStartGameData.m_InvokerLevel = BirdsLevel;
		battleStartGameData.m_BattleRandomSeed = UnityEngine.Random.Range(1, int.MaxValue);
		battleStartGameData.m_Inventory = new InventoryGameData(m_DefaultInventoryNameId);
		DIContainerLogic.GetLootOperationService().RewardLoot(battleStartGameData.m_Inventory, 0, DIContainerLogic.GetLootOperationService().GenerateLoot(battleStartGameData.m_Inventory.BalancingData.DefaultInventoryContent, 1), "DebugLoot");
		AdditionalPlayerItems.AddInventoryItemsToInventory(battleStartGameData.m_Inventory);
		DIContainerInfrastructure.GetCurrentPlayer().Data.Level = BirdsLevel;
		List<BirdGameData> list = new List<BirdGameData>();
		foreach (string birdBalancingNameId in m_BirdBalancingNameIds)
		{
			BirdBalancingData balancing = null;
			if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>(birdBalancingNameId, out balancing))
			{
				DebugLog.Log("Success: " + birdBalancingNameId);
				BirdGameData birdGameData = new BirdGameData(balancing.NameId);
				birdGameData.Data.Level = BirdsLevel;
				list.Add(birdGameData);
			}
		}
		List<BirdGameData> list2 = new List<BirdGameData>();
		foreach (string pvPBirdBalancingNameId in m_PvPBirdBalancingNameIds)
		{
			BirdBalancingData balancing2 = null;
			if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>(pvPBirdBalancingNameId, out balancing2))
			{
				DebugLog.Log("Success: " + pvPBirdBalancingNameId);
				BirdGameData birdGameData2 = new BirdGameData(balancing2.NameId);
				birdGameData2.Data.Level = BirdsLevel;
				list2.Add(birdGameData2);
			}
		}
		battleStartGameData.m_Birds = list;
		battleStartGameData.m_PvPBirds = list2;
		battleStartGameData.m_BirdBanner = new BannerGameData("bird_banner");
		battleStartGameData.m_PigBanner = new BannerGameData("bird_banner");
		ClientInfo.CurrentBattleStartGameData = battleStartGameData;
		GotoBattle(m_BattlegroundNameId);
	}

	public override void GotoWorldMap()
	{
	}

	public void OnBattleDone(IAsyncResult result)
	{
		DebugLog.Log("Battle callback received!");
		if (result != null)
		{
			DebugLog.Log("Battle Async Result != null!");
			BattleEndGameData battleEndGameData = DIContainerLogic.GetBattleService().EndBattle(result);
			DebugLog.Log("Battle EndData is null: " + (battleEndGameData == null));
			InitAndEnterBattle();
		}
	}

	public override void GotoCampScreen()
	{
		base.SceneLoadingMgr.LoadGameScene("Arena", new List<string> { "Menu_Arena" });
	}

	public override void GotoDefaultBattle()
	{
		base.SceneLoadingMgr.LoadGameScene("DefaultBattleground", new List<string> { "Menu_Battleground", "Popup_BattlePaused" });
	}

	public void EndBattle()
	{
		if (ClientInfo.CurrentBattleGameData != null)
		{
			DIContainerLogic.GetBattleService().RegisterBattleEnded(ClientInfo.CurrentBattleGameData);
			ClientInfo.CurrentBattleGameData = null;
		}
	}

	public override void ReturnFromBattle()
	{
		EndBattle();
	}

	public override ITutorialMgr InstantiateNullTutorialMgr()
	{
		TutorialMgrNullImpl tutorialMgrNullImpl = UnityEngine.Object.Instantiate(m_TutorialNullMgr);
		UnityEngine.Object.DontDestroyOnLoad(tutorialMgrNullImpl);
		return tutorialMgrNullImpl;
	}

	public override ITutorialMgr InstantiateTutorialMgr()
	{
		TutorialMgrNullImpl tutorialMgrNullImpl = UnityEngine.Object.Instantiate(m_TutorialNullMgr);
		UnityEngine.Object.DontDestroyOnLoad(tutorialMgrNullImpl);
		return tutorialMgrNullImpl;
	}
}

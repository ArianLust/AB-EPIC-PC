using System.Linq;
using ABH.Services.Logic;
using ABH.Services.Logic.Interfaces;
using ABH.Shared.BalancingData;
using Assets.Scripts.Services.Infrastructure.Time;
using Assets.Scripts.Services.Logic;
using Chimera.Library.Components.Interfaces;
using Chimera.Library.Components.Services;

public class DIContainerLogic
{
	private static ITimingService m_fallbackTimingService;

	private static ITimingService m_timingService;

	private static ITimingService m_serverOnlyTimingService;

	private static ITimingService m_deviceTimingService;

	private static PlayerService m_playerService;

	private static IRequirementOperationService m_requirementService;

	private static InventoryOperationServiceRealImpl m_inventoryService;

	private static LootOperations m_lootOperationService;

	private static CraftingService m_craftingService;

	private static DailyLoginLogic m_dailyLoginLogic;

	private static EnchantmentLogic m_enchantmentLogic;

	private static EventSystemService m_EventSystemService;

	private static PvPSeasonService m_PvPSeasonService;

	private static WorldMapService m_worldMapService;

	private static ChronicleCaveService m_chronicleCaveService;

	private static IResourceNodeManager m_resourceNodeManager;

	private static BattleService m_battleService;

	private static ShopService m_shopService;

	private static SalesManagerService m_salesManagerService;

	private static SocialService m_socialService;

	private static PacingBalancing m_PacingBalancing;

	private static VisualEffectsBalancing m_VisualEffectsBalancing;

	public static IProfileMerger m_profileMerger;

	private static PvPObjectivesService m_pvpObjectivesService;

	private static ICustomerSupportService m_customerSupportService;

	private static ChimeraBackendService m_BackendService;

	private static BonusEventService m_BonusEventService;

	private static RateAppController m_rateAppControler;

	private static NotificationPopupController m_notificationPopupController;

	public static PlayerService PlayerOperationsService
	{
		get
		{
			if (m_playerService == null)
			{
				m_playerService = new PlayerService();
			}
			return m_playerService;
		}
		set
		{
			m_playerService = value;
		}
	}

	public static IRequirementOperationService RequirementService
	{
		get
		{
			if (m_requirementService == null)
			{
				m_requirementService = new RequirementOperationServiceRealImpl();
				m_requirementService.InitializeRequirementOperations();
			}
			return m_requirementService;
		}
		set
		{
			m_requirementService = value;
		}
	}

	public static InventoryOperationServiceRealImpl InventoryService
	{
		get
		{
			if (m_inventoryService != null)
			{
				return m_inventoryService;
			}
			m_inventoryService = new InventoryOperationServiceRealImpl();
			m_inventoryService.SetCraftingService(CraftingService);
			CraftingService.SetInventoryService(m_inventoryService);
			m_inventoryService.InitializeInventoryOperations();
			return m_inventoryService;
		}
		set
		{
			m_inventoryService = value;
		}
	}

	public static CraftingService CraftingService
	{
		get
		{
			if (m_craftingService != null)
			{
				return m_craftingService;
			}
			m_craftingService = new CraftingService();
			m_craftingService.SetLootService(GetLootOperationService()).SetWorldBalancing(DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island")).SetDebugLog(DebugLog.Log)
				.SetErrorLog(DebugLog.Error);
			return m_craftingService;
		}
		set
		{
			m_craftingService = value;
		}
	}

	public static DailyLoginLogic DailyLoginLogic
	{
		get
		{
			if (m_dailyLoginLogic == null)
			{
				m_dailyLoginLogic = new DailyLoginLogic();
			}
			return m_dailyLoginLogic;
		}
		set
		{
			m_dailyLoginLogic = value;
		}
	}

	public static EnchantmentLogic EnchantmentLogic
	{
		get
		{
			if (m_enchantmentLogic == null)
			{
				m_enchantmentLogic = new EnchantmentLogic();
			}
			return m_enchantmentLogic;
		}
	}

	public static EventSystemService EventSystemService
	{
		get
		{
			if (m_EventSystemService != null)
			{
				return m_EventSystemService;
			}
			m_EventSystemService = new EventSystemService();
			m_EventSystemService.SetDebugLog(delegate(string m)
			{
				DebugLog.Log(typeof(EventSystemService), m);
			}).SetErrorLog(delegate(string m)
			{
				DebugLog.Error(typeof(EventSystemService), m);
			}).SetTimingService(GetServerOnlyTimingService());
			return m_EventSystemService;
		}
		set
		{
			m_EventSystemService = value;
		}
	}

	public static PvPSeasonService PvPSeasonService
	{
		get
		{
			if (m_PvPSeasonService != null)
			{
				return m_PvPSeasonService;
			}
			m_PvPSeasonService = new PvPSeasonService();
			m_PvPSeasonService.SetDebugLog(delegate(string m)
			{
				DebugLog.Log(typeof(PvPSeasonService), m);
			}).SetErrorLog(delegate(string m)
			{
				DebugLog.Error(typeof(PvPSeasonService), m);
			}).SetTimingService(GetServerOnlyTimingService());
			return m_PvPSeasonService;
		}
		set
		{
			m_PvPSeasonService = value;
		}
	}

	public static WorldMapService WorldMapService
	{
		get
		{
			if (m_worldMapService != null)
			{
				return m_worldMapService;
			}
			m_worldMapService = new WorldMapService(RequirementService);
			m_worldMapService.SetDebugLog(delegate(string m)
			{
				DebugLog.Log(m);
			}).SetErrorLog(delegate(string m)
			{
				DebugLog.Error(m);
			});
			return m_worldMapService;
		}
		set
		{
			m_worldMapService = value;
		}
	}

	public static ChronicleCaveService ChronicleCaveService
	{
		get
		{
			if (m_chronicleCaveService != null)
			{
				return m_chronicleCaveService;
			}
			m_chronicleCaveService = new ChronicleCaveService(RequirementService);
			m_chronicleCaveService.SetDebugLog(delegate(string m)
			{
				DebugLog.Log(m);
			}).SetErrorLog(delegate(string m)
			{
				DebugLog.Error(m);
			});
			return m_chronicleCaveService;
		}
		set
		{
			m_chronicleCaveService = value;
		}
	}

	public static SocialService SocialService
	{
		get
		{
			if (m_socialService != null)
			{
				return m_socialService;
			}
			m_socialService = new SocialService();
			m_socialService.SetDebugLog(delegate(string s)
			{
				DebugLog.Log(s);
			}).SetErrorLog(delegate(string s)
			{
				DebugLog.Error(s);
			});
			return m_socialService;
		}
		set
		{
			m_socialService = value;
		}
	}

	public static IProfileMerger ProfileMerger
	{
		get
		{
			if (m_profileMerger == null)
			{
				bool enableProfileMerging = DIContainerBalancing.Service.GetBalancingDataList<ClientConfigBalancingData>().FirstOrDefault().EnableProfileMerging;
				object profileMerger2;
				if (enableProfileMerging)
				{
					IProfileMerger profileMerger = new ProfileMerger();
					profileMerger2 = profileMerger;
				}
				else
				{
					profileMerger2 = new ProfileFakeMerger();
				}
				m_profileMerger = (IProfileMerger)profileMerger2;
				DebugLog.Log("[DIContainerLogic] Initialized the ProfileMerger. Using the real impl? " + enableProfileMerging);
			}
			return m_profileMerger;
		}
	}

	internal static ICustomerSupportService CustomerSupportService
	{
		get
		{
			if (m_customerSupportService != null)
			{
				return m_customerSupportService;
			}
			m_customerSupportService = new CustomerSupportNullImpl();
			return m_customerSupportService;
		}
	}

	internal static ChimeraBackendService BackendService
	{
		get
		{
			if (m_BackendService == null)
			{
				m_BackendService = new ChimeraBackendService();
				m_BackendService.Init();
			}
			return m_BackendService;
		}
	}

	public static BonusEventService GetBonusEventService
	{
		get
		{
			if (m_BonusEventService == null)
			{
				m_BonusEventService = new BonusEventService();
			}
			return m_BonusEventService.UpdateEvents();
		}
	}

	public static RateAppController RateAppController
	{
		get
		{
			if (m_rateAppControler == null)
			{
				m_rateAppControler = new RateAppController();
			}
			return m_rateAppControler;
		}
	}

	public static NotificationPopupController NotificationPopupController
	{
		get
		{
			if (m_notificationPopupController == null)
			{
				m_notificationPopupController = new NotificationPopupController();
			}
			return m_notificationPopupController;
		}
	}

	internal static ITimingService GetTimingService()
	{
		if (DIContainerInfrastructure.GetCoreStateMgr() == null || !DIContainerBalancing.IsInitialized)
		{
			DebugLog.Warn("[DIContainerLogic] GetTimingService(): returning fallback timing service");
			return m_fallbackTimingService ?? (m_fallbackTimingService = new TimingService());
		}
		bool useSkynestTimingService = false;
		if (m_fallbackTimingService != null)
		{
			m_fallbackTimingService = null;
			DebugLog.Warn("[DIContainerLogic] GetTimingService(): fallback timing service was returned before, now returning the real one: " + ((!useSkynestTimingService) ? "TimingService" : "TimingServiceSkynestStrictImpl"));
		}
		if (m_timingService == null)
		{
			if (!useSkynestTimingService)
			{
				DebugLog.Log("[DIContainerLogic] GetTimingService(): initialized the TimingService()");
				m_timingService = new TimingService();
			}
			else
			{
				DebugLog.Log("[DIContainerLogic] GetTimingService(): initialized the TimingServiceSkynestStrictImpl()");
				m_timingService = new TimingServiceSkynestStrictImpl();
			}
		}
		return m_timingService;
	}

	internal static ITimingService GetServerOnlyTimingService()
	{
		return m_serverOnlyTimingService ?? (m_serverOnlyTimingService = new TimingService());
	}

	internal static ITimingService GetDeviceTimingService()
	{
		m_deviceTimingService = new TimingService();
		return m_deviceTimingService;
	}

	internal static LootOperations GetLootOperationService()
	{
		return m_lootOperationService ?? (m_lootOperationService = new LootOperations());
	}

	internal static IResourceNodeManager GetResourceNodeManager()
	{
		return m_resourceNodeManager ?? (m_resourceNodeManager = DIContainerInfrastructure.GetCoreStateMgr().GetComponent<ResourceNodeManager>());
	}

	internal static BattleService GetBattleService()
	{
		if (m_battleService != null)
		{
			return m_battleService;
		}
		m_battleService = new BattleService();
		m_battleService.RegisterDebugLog(delegate(string m, BattleLogTypes t)
		{
			DebugLog.Log("BattleLog: " + m);
		});
		m_battleService.RegisterErrorLog(delegate(string m, BattleLogTypes t)
		{
			DebugLog.Error("BattleLog: " + m);
		});
		m_battleService.SetRequirementService(RequirementService).SetLogEnabled(false);
		return m_battleService;
	}

	internal static ShopService GetShopService()
	{
		return m_shopService ?? (m_shopService = new ShopService());
	}

	internal static SalesManagerService GetSalesManagerService()
	{
		return m_salesManagerService ?? (m_salesManagerService = new SalesManagerService());
	}

	internal static PacingBalancing GetPacingBalancing()
	{
		return DIContainerInfrastructure.GetCoreStateMgr().m_PacingBalancing;
	}

	internal static VisualEffectsBalancing GetVisualEffectsBalancing()
	{
		return DIContainerInfrastructure.GetCoreStateMgr().m_VisualEffectsBalancing;
	}

	internal static PvPObjectivesService GetPvpObjectivesService()
	{
		if (m_pvpObjectivesService != null)
		{
			return m_pvpObjectivesService;
		}
		m_pvpObjectivesService = new PvPObjectivesService(DIContainerInfrastructure.GetCurrentPlayer());
		return m_pvpObjectivesService;
	}
}

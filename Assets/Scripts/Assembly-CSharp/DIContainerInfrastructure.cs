using System;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Services.Logic;
using ABH.Services.Logic.Interfaces;
using ABH.Shared.Models;
using Assets.Scripts.Services.Infrastructure;
using Assets.Scripts.Services.Logic;
using ChBgLib.Text;
using Chimera.Library.Components.Interfaces;
using Chimera.Library.Components.Services;
using Interfaces.Identity;
using Interfaces.Notification;
using Interfaces.Purchasing;
using Rcs;
using UnityEngine;

public class DIContainerInfrastructure
{
	private static IChannelService m_channel;

	private static IAssetsService m_assetsService;

	private static IMessagingService m_messagingService;

	private static IPurchasingService m_purchasingService;

	private static IScoringService m_ScoringService;

	private static IMatchmakingService m_MatchmakingService;

	private static IRemoteStorageService m_remoteStorageService;

	private static IIdentityService m_identityService;

	private static IAdService m_adService;

	private static IWebRequestParameterSignatureService m_webRequestParameterSignatureService;

	private static INetworkStatusService m_networkStatusService;

	private static ISerializer m_webRequestSerializer;

	private static string m_targetBuildGroup;

	private static RovioAccSyncState m_rovioAccSyncState;

	private static BeaconPurchaseProcessor m_purchaseProcessor;

	private static ISystemInfo m_systemInfo;

	private static bool m_isRotationAllowed = true;

	private static IFacebookWrapper m_facebookComponent;

	private static IHashService m_hashService;

	private static ISerializer m_locaSerializer;

	private static ISerializer m_balancingDataSerializer;

	private static ISerializer m_binarySerializer;

	private static ISerializer m_stringSerializer;

	private static AndroidBackButtonMgr m_BackButtonMgr;

	private static INotificationService m_NotificationService;

	private static IProfileMgr<ABH.Shared.Models.PlayerData> m_profileMgr;

	private static AssetData m_assetData;

	private static IStorageService m_storageService;

	private static ABHLocaService m_locaService;

	private static ABHLocaService m_startupLocaService;

	private static PlayerGameData m_currentPlayer;

	private static bool m_initializingCurrentPlayer;

	private static CoreStateMgr m_coreStateMgr;

	private static EventSystemStateMgr m_EventSystemStateMgr;

	private static PvPSeasonStateMgr m_PvPSeasonStateMgr;

	private static BaseCampStateMgr m_baseStateMgr;

	private static BaseLocationStateManager m_locationStateMgr;

	private static GenericAssetProvider m_bannerAssetProvider;

	private static GenericAssetProvider m_comboAssetProvider;

	private static GenericAssetProvider m_characterLowResAssetProvider;

	private static GenericAssetProvider m_comicCutsceneProvider;

	private static GenericAssetProvider m_battlegroundAssetProvider;

	private static GenericAssetProvider m_worldMapAssetProvider;

	private static GenericAssetProvider m_shopIconAssetProvider;

	private static GenericAssetProvider m_propLiteAssetProvider;

	private static GenericAssetProvider m_genericIconAtlasAssetProvider;

	private static GenericAssetProvider m_battleEffectAssetProvider;

	private static GenericAssetProvider m_AllEventsAssetProvider;

	private static GenericAssetProvider m_CurrentEventAssetProvider;

	private static GenericAssetProvider m_bundlePrefabAssetProvider;

	private static Dictionary<AssetbundleIndex, GenericAssetProvider> m_audioAssetProvider = new Dictionary<AssetbundleIndex, GenericAssetProvider>();

	private static ICryptographyService m_cryptographyService;

	private static SoundManager m_audioManager;

	private static AudioSource m_primaryMusicSource;

	private static AudioSource m_secondaryMusicSource;

	private static ITutorialMgr m_tutorialMgr;

	private static NumberFormatProviderImpl m_formatProvider;

	private static Version m_version;

	private static AssetBundleCrcChecker m_assetBundleCrcChecker;

	private static IAchievementService m_achievementService;

	private static ClientUpdateService m_clientUpdateService;

	private static AppResumeService m_appResumeService;

	private static AndroidExpansionFileMgr m_androidExpansionFileMgr;

	private static IAppAttributionService m_attributionService;

	private static IAppTrackService m_comScoreService;

	private static IAnalyticsSystem m_analyticsSystemCombo;

	private static IAnalyticsSystem m_analyticsSystemBeaconOnly;

	private static ICompressionService m_compressionService;

	private static IEncryptionService m_encryptionService;

	private static ICompatibilityService m_compatibilityService;

	private static TimeScaleMgr m_TimeScaleMgr;

	private static NotificationPermissionMgr m_notificationPermissionMgr;

    private static SkynestMetaDataCache m_skynestMetadataCache;

    private static CustomMessageService m_customMessageService;

	private static PlaymobService m_playmobService;

	private static PowerLevelCalculator m_powerLevelCalculator;

	public static IMessagingService MessagingService
	{
		get
		{
			if (m_messagingService == null)
			{
				MessagingService = new MessagingServiceOfflineImpl();
			}
			return m_messagingService;
		}
		set
		{
			m_messagingService = value;
		}
	}

	public static IPurchasingService PurchasingService
	{
		get
		{
			if (m_purchasingService == null)
			{
				m_purchasingService = new PurchasingServiceNullImpl().SetPaymentProvider("heroic");
				m_purchasingService.AutoRestorePurchasesAfterInit = true;
			}
			return m_purchasingService;
		}
		set
		{
			m_purchasingService = value;
		}
	}

	public static IScoringService ScoringService
	{
		get
		{
			if (m_ScoringService != null)
			{
				return m_ScoringService;
			}
			m_ScoringService = new ScoringServiceNullImpl();
			m_ScoringService.SetDebugLog(delegate(string m)
			{
				DebugLog.Log(typeof(ScoringServiceNullImpl), m);
			}).SetErrorLog(delegate(string m)
			{
				DebugLog.Error(typeof(ScoringServiceNullImpl), m);
			});
			return m_ScoringService;
		}
		set
		{
			m_ScoringService = value;
		}
	}

	public static IMatchmakingService MatchmakingService
	{
		get
		{
			if (m_MatchmakingService != null)
			{
				return m_MatchmakingService;
			}
			m_MatchmakingService = new MatchmakingServiceNullImpl();
			m_MatchmakingService.SetDebugLog(delegate(string m)
			{
				DebugLog.Log(typeof(MatchmakingServiceNullImpl), m);
			}).SetErrorLog(delegate(string m)
			{
				DebugLog.Error(typeof(MatchmakingServiceNullImpl), m);
			});
			return m_MatchmakingService;
		}
		set
		{
			m_MatchmakingService = value;
		}
	}

	public static IRemoteStorageService RemoteStorageService
	{
		get
		{
			if (m_remoteStorageService == null)
			{
				m_remoteStorageService = new StorageServiceOfflineImpl();
			}
			return m_remoteStorageService;
		}
	}

	public static IIdentityService IdentityService
	{
		get
		{
			if (m_identityService == null)
			{
				m_identityService = new IdentityServiceNullImpl();
			}
			return m_identityService;
		}
		set
		{
			m_identityService = value;
		}
	}

	public static IAdService AdService
	{
		get
		{
			if (m_adService == null)
			{
				m_adService = new AdServiceNullImpl();
			}
			return m_adService;
		}
		set
		{
			m_adService = value;
		}
	}

	public static AndroidBackButtonMgr BackButtonMgr
	{
		get
		{
			return m_BackButtonMgr ?? (m_BackButtonMgr = (GetCoreStateMgr() ? GetCoreStateMgr().gameObject.GetComponent<AndroidBackButtonMgr>() : null));
		}
	}

	public static bool IsCurrentPlayerInitialized { get; private set; }

	public static EventSystemStateMgr EventSystemStateManager
	{
		get
		{
			return m_EventSystemStateMgr ?? (m_EventSystemStateMgr = UnityEngine.Object.FindObjectOfType(typeof(EventSystemStateMgr)) as EventSystemStateMgr);
		}
	}

	public static PvPSeasonStateMgr PvPSeasonStateMgr
	{
		get
		{
			return m_PvPSeasonStateMgr ?? (m_PvPSeasonStateMgr = UnityEngine.Object.FindObjectOfType(typeof(PvPSeasonStateMgr)) as PvPSeasonStateMgr);
		}
	}

	public static BaseCampStateMgr BaseStateMgr
	{
		get
		{
			if (!m_baseStateMgr)
			{
				m_baseStateMgr = UnityEngine.Object.FindObjectOfType(typeof(BaseCampStateMgr)) as BaseCampStateMgr;
			}
			return m_baseStateMgr;
		}
	}

	public static BaseLocationStateManager LocationStateMgr
	{
		get
		{
			if (!m_locationStateMgr)
			{
				m_locationStateMgr = UnityEngine.Object.FindObjectOfType(typeof(BaseLocationStateManager)) as BaseLocationStateManager;
			}
			return m_locationStateMgr;
		}
	}

	public static GenericAssetProvider ProjectileAssetProvider
	{
		get
		{
			return m_comboAssetProvider ?? (m_comboAssetProvider = (GetCoreStateMgr() ? GetCoreStateMgr().gameObject.GetComponentsInChildren<GenericAssetProvider>().FirstOrDefault((GenericAssetProvider c) => c.name.StartsWith("CharacterHighResAndEquipmentAssetProvider")) : null));
		}
	}

	private static GenericAssetProvider AllEventsAssetProvider
	{
		get
		{
			return GetCoreStateMgr().gameObject.GetComponentsInChildren<GenericAssetProvider>(true).FirstOrDefault((GenericAssetProvider prov) => prov.name == "AllEventsAssetProvider");
		}
		set
		{
			m_AllEventsAssetProvider = value;
		}
	}

	private static GenericAssetProvider CurrentEventAssetProvider
	{
		get
		{
			return EventSystemStateManager.GetCurrentEventAssetProvider();
		}
	}

	public static ICryptographyService CryptographyService
	{
		get
		{
			if (m_cryptographyService != null)
			{
				return m_cryptographyService;
			}
			m_cryptographyService = new CryptographyServiceSimpleImpl();
			return m_cryptographyService;
		}
	}

	public static SoundManager AudioManager
	{
		get
		{
			if (m_audioManager == null)
			{
				m_audioManager = (GetCoreStateMgr() ? GetCoreStateMgr().gameObject.GetComponentsInChildren<SoundManager>().FirstOrDefault() : null);
				if (m_audioManager != null)
				{
					m_audioManager.Initialize();
				}
			}
			return m_audioManager;
		}
	}

	public static AudioSource PrimaryMusicSource
	{
		get
		{
			if (m_primaryMusicSource != null)
			{
				return m_primaryMusicSource;
			}
			m_primaryMusicSource = (GetCoreStateMgr() ? GetCoreStateMgr().gameObject.GetComponentsInChildren<AudioSource>().FirstOrDefault((AudioSource c) => c.name == "1_Audio_Music_Primary") : null);
			return m_primaryMusicSource;
		}
	}

	public static AudioSource SecondaryMusicSource
	{
		get
		{
			if (m_secondaryMusicSource != null)
			{
				return m_secondaryMusicSource;
			}
			m_secondaryMusicSource = (GetCoreStateMgr() ? GetCoreStateMgr().gameObject.GetComponentsInChildren<AudioSource>().FirstOrDefault((AudioSource c) => c.name == "2_Audio_Music_Secondary") : null);
			return m_secondaryMusicSource;
		}
	}

	public static ITutorialMgr TutorialMgr
	{
		get
		{
			if (m_tutorialMgr != null)
			{
				return m_tutorialMgr;
			}
			m_tutorialMgr = GetCoreStateMgr().InstantiateTutorialMgr(); // GetCoreStateMgr().InstantiateNullTutorialMgr();
			return m_tutorialMgr;
		}
		set
		{
			m_tutorialMgr = value;
		}
	}

	public static DragController CurrentDragController { get; set; }

	public static TimeScaleMgr TimeScaleMgr
	{
		get
		{
			bool num = m_TimeScaleMgr;
			if (!num)
			{
				num = !GetCoreStateMgr();
			}
			return (!num) ? GetCoreStateMgr().GetComponent<TimeScaleMgr>() : null;
		}
	}

	public static IChannelService GetChannelService()
	{
		return m_channel ?? (m_channel = new ChannelServiceBeaconImpl());
	}

	public static IAssetsService GetAssetsService()
	{
		return m_assetsService ?? (m_assetsService = new AssetsServiceLocalImpl());
	}

	public static IWebRequestParameterSignatureService GetWebRequestParameterSignatureService()
	{
		if (m_webRequestParameterSignatureService == null)
		{
			WebRequestParameterSignatureImpl webRequestParameterSignatureImpl = new WebRequestParameterSignatureImpl(GetHashService());
			webRequestParameterSignatureImpl.Log = DebugLog.Log;
			webRequestParameterSignatureImpl.LogError = DebugLog.Error;
			webRequestParameterSignatureImpl.SignatureSalt = () => string.Empty;
			m_webRequestParameterSignatureService = webRequestParameterSignatureImpl;
		}
		return m_webRequestParameterSignatureService;
	}

	public static INetworkStatusService GetNetworkStatusService()
	{
		if (m_networkStatusService == null)
		{
			m_networkStatusService = new NetworkStatusServiceNull();
			m_networkStatusService.Log = DebugLog.Log;
			m_networkStatusService.LogError = DebugLog.Error;
		}
		return m_networkStatusService;
	}

	public static ISerializer GetWebRequestSerializer()
	{
		ISerializer serializer = m_webRequestSerializer;
		if (serializer == null)
		{
			BinarySerializerProtoBufPrecompiledImpl binarySerializerProtoBufPrecompiledImpl = new BinarySerializerProtoBufPrecompiledImpl();
			binarySerializerProtoBufPrecompiledImpl.Log = DebugLog.Log;
			binarySerializerProtoBufPrecompiledImpl.LogError = DebugLog.Error;
			serializer = (m_webRequestSerializer = binarySerializerProtoBufPrecompiledImpl);
		}
		return serializer;
	}

	public static string GetTargetBuildGroup()
	{
		if (string.IsNullOrEmpty(m_targetBuildGroup))
		{
			TextAsset textAsset = Resources.Load("targetbuildgroup") as TextAsset;
			if (textAsset != null)
			{
				string text = textAsset.text.ToLower().Trim();
				m_targetBuildGroup = ReplaceUnmappableCharacters(text);
				DebugLog.Log("targetbuildgroup is now set to " + m_targetBuildGroup);
			}
			else
			{
				DebugLog.Error("targetbuildgroup asset not found!");
			}
		}
		return m_targetBuildGroup;
	}
	
	private static string ReplaceUnmappableCharacters(string localizedString)
	{
		if (string.IsNullOrEmpty(localizedString))
		{
			return localizedString;
		}
		DebugLog.Log("[DIContainerInfrastructure]", "[ReplaceUnmappableCharacters] Old string: " + localizedString);
		localizedString = localizedString
			.Replace("æ", "e")
			.Replace("ø", "oe")
			.Replace("å", "aa")
			.Replace("æ", "ae")
			.Replace("Æ", "Ae")
			.Replace("Ø", "Oe")
			.Replace("Ǿ", "Oe")
			.Replace("ǿ", "oe")
			.Replace("ǽ", "ae")
			.Replace("Ǽ", "Ae")
			.Replace("Å", "Aa")
			.Replace("ı", "i");
		DebugLog.Log("[DIContainerInfrastructure]", "[ReplaceUnmappableCharacters] New string: " + localizedString);
		return localizedString;
	}

	public static RovioAccSyncState GetRovioAccSyncState()
	{
		return m_rovioAccSyncState ?? (m_rovioAccSyncState = new RovioAccSyncState());
	}

	public static BeaconPurchaseProcessor GetPurchaseProcessor()
	{
		return m_purchaseProcessor ?? (m_purchaseProcessor = new BeaconPurchaseProcessor());
	}

	public static void SetPurchaseProcessor(BeaconPurchaseProcessor purchaseProcessor)
	{
		m_purchaseProcessor = purchaseProcessor;
	}

	public static ISystemInfo GetSystemInfoService()
	{
		return m_systemInfo ?? (m_systemInfo = new SystemInfoNullImpl());
	}

	public static bool ResetWithNewProfile(ABH.Shared.Models.PlayerData newplayerData, bool enterWorldMap = true)
	{
		DebugLog.Log("ResetWithNewProfile started.");
		if ((bool)GetCoreStateMgr() && (bool)GetCoreStateMgr().m_GenericUI)
		{
			GetCoreStateMgr().m_GenericUI.RemoveHandlers();
		}
		GetProfileMgr().RemoveProfile();
		GetProfileMgr().CurrentProfile = newplayerData;
		if (newplayerData != null)
		{
			GetProfileMgr().SaveCurrentProfile();
		}
		m_stringSerializer = null;
		m_profileMgr = null;
		m_currentPlayer = null;
		m_storageService = null;
		if (m_tutorialMgr != null)
		{
			m_tutorialMgr.Remove();
		}
		m_tutorialMgr = null;
		m_initializingCurrentPlayer = false;
		m_currentPlayer = null;
		InitCurrentPlayerIfNecessary(delegate
		{
			ResetWithNewProfileWhenPlayerAvailable(enterWorldMap);
		});
		return true;
	}

	private static void ResetWithNewProfileWhenPlayerAvailable(bool enterWorldMap)
	{
		if (ContentLoader.Instance.LastAccountWasNew)
		{
			GetCurrentPlayer().Data.NotificationUsageState = 0;
		}
		if ((bool)GetCoreStateMgr() && (bool)GetCoreStateMgr().m_GenericUI)
		{
			GetCoreStateMgr().m_GenericUI.ReInitialize();
			GetCoreStateMgr().StartRefreshFriends();
			if ((bool)AudioManager)
			{
				if (GetCurrentPlayer().Data.IsMusicMuted)
				{
					AudioManager.AddMuteReason(0, "Data.IsMusicMuted");
				}
				else
				{
					AudioManager.RemoveMuteReason(0, "Data.IsMusicMuted");
				}
				if (GetCurrentPlayer().Data.IsSoundMuted)
				{
					AudioManager.AddMuteReason(1, "Data.IsSoundMuted");
				}
				else
				{
					AudioManager.RemoveMuteReason(1, "Data.IsSoundMuted");
				}
			}
		}
		ITutorialMgr tutorialMgr = TutorialMgr;
		if ((bool)EventSystemStateManager)
		{
			EventSystemStateManager.ResetEventManager();
		}
		if ((bool)PvPSeasonStateMgr)
		{
			PvPSeasonStateMgr.ResetPvPSystem();
		}
		if (enterWorldMap && (bool)GetCoreStateMgr())
		{
			GetCoreStateMgr().DelayedGotoWorldMapOrIntro();
		}
		DebugLog.Log("ResetWithNewProfile finished.");
	}

	public static bool IsRotationAllowed()
	{
		//AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		//AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		//AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getContentResolver", new object[0]);
		//AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.provider.Settings$System");
		//int num = androidJavaClass2.CallStatic<int>("getInt", new object[3] { androidJavaObject, "accelerometer_rotation", 0 });
		//DebugLog.Log("Android Rotation: " + num);
		//androidJavaClass.Dispose();
		//@static.Dispose();
		//androidJavaObject.Dispose();
		//androidJavaClass2.Dispose();
		//m_isRotationAllowed = num > 0;
		//return m_isRotationAllowed;

		return false;
	}

	public static IFacebookWrapper GetFacebookWrapper()
	{
        //return m_facebookComponent ?? (m_facebookComponent = new FacebookWrapperHatchImpl(ContentLoader.Instance.m_BeaconConnectionMgr.FriendsService, delegate(User.SocialNetworkProfile profile)
        //{
        //	SocialEnvironmentData data = GetCurrentPlayer().SocialEnvironmentGameData.Data;
        //	data.SocialPictureUrl = profile.AvatarUrl;
        //	data.SocialPlayerName = profile.Name;
        //	data.SocialId = profile.Uid;
        //}));

        return m_facebookComponent ?? (m_facebookComponent = new FacebookWrapperNullImpl());

    }

	public static IHashService GetHashService()
	{
		return m_hashService ?? (m_hashService = new HashServiceNullImpl());
	}

	public static ISerializer GetLocaSerializer()
	{
		ISerializer serializer = m_locaSerializer;
		if (serializer == null)
		{
			BinarySerializerProtoBufPrecompiledImpl binarySerializerProtoBufPrecompiledImpl = new BinarySerializerProtoBufPrecompiledImpl();
			binarySerializerProtoBufPrecompiledImpl.Log = DebugLog.Log;
			binarySerializerProtoBufPrecompiledImpl.LogError = DebugLog.Error;
			serializer = (m_locaSerializer = binarySerializerProtoBufPrecompiledImpl);
		}
		return serializer;
	}

	public static ISerializer GetBalancingDataSerializer()
	{
		ISerializer serializer = m_balancingDataSerializer;
		if (serializer == null)
		{
			BinarySerializerProtoBufPrecompiledImpl binarySerializerProtoBufPrecompiledImpl = new BinarySerializerProtoBufPrecompiledImpl();
			binarySerializerProtoBufPrecompiledImpl.Log = DebugLog.Log;
			binarySerializerProtoBufPrecompiledImpl.LogError = DebugLog.Error;
			serializer = (m_balancingDataSerializer = binarySerializerProtoBufPrecompiledImpl);
		}
		return serializer;
	}

	public static ISerializer GetBinarySerializer()
	{
		ISerializer serializer = m_binarySerializer;
		if (serializer == null)
		{
			BinarySerializerProtoBufPrecompiledImpl binarySerializerProtoBufPrecompiledImpl = new BinarySerializerProtoBufPrecompiledImpl();
			binarySerializerProtoBufPrecompiledImpl.Log = DebugLog.Log;
			binarySerializerProtoBufPrecompiledImpl.LogError = DebugLog.Error;
			serializer = (m_binarySerializer = binarySerializerProtoBufPrecompiledImpl);
		}
		return serializer;
	}

	public static ISerializer GetStringSerializer()
	{
		ISerializer serializer = m_stringSerializer;
		if (serializer == null)
		{
			StringSerializerProtoBufPrecompiledImpl stringSerializerProtoBufPrecompiledImpl = new StringSerializerProtoBufPrecompiledImpl();
			stringSerializerProtoBufPrecompiledImpl.Log = DebugLog.Log;
			stringSerializerProtoBufPrecompiledImpl.LogError = DebugLog.Error;
			serializer = (m_stringSerializer = stringSerializerProtoBufPrecompiledImpl);
		}
		return serializer;
	}

	public static INotificationService NotificationService()
	{
		return m_NotificationService ?? (m_NotificationService = new NotificationServiceNullImpl());
	}

	public static IProfileMgr<ABH.Shared.Models.PlayerData> GetProfileMgr()
	{
		return m_profileMgr ?? (m_profileMgr = new ProfileStringSerializedImpl<ABH.Shared.Models.PlayerData>());
	}

	public static AssetData GetAssetData()
	{
		if (m_assetData == null)
		{
			string @string = GetPlayerPrefsService().GetString("assetData");
			if (!string.IsNullOrEmpty(@string))
			{
				m_assetData = GetStringSerializer().Deserialize<AssetData>(@string);
			}
			else
			{
				m_assetData = new AssetData();
			}
		}
		return m_assetData;
	}

	public static IStorageService GetPlayerPrefsService()
	{
		return m_storageService ?? (m_storageService = new StorageUnityImpl());
	}

	public static ABHLocaService GetLocaService()
	{
		return m_locaService ?? (m_locaService = new ABHLocaService(new ProtobufSerializedSkynestLocaLoaderImpl().Init(GetLocaSerializer()), new LocaHelper()));
	}

	public static ABHLocaService GetStartupLocaService()
	{
		return m_startupLocaService ?? (m_startupLocaService = new ABHLocaService(new ProtobufSerializedLocaLoaderImpl().Init(GetLocaSerializer()), new LocaHelper()));
	}

	public static PlayerGameData GetCurrentPlayer()
	{
		return m_currentPlayer;
	}

	public static void InitCurrentPlayerIfNecessary(Action onCompleted = null, bool restart = false)
	{
		onCompleted = onCompleted ?? ((Action)delegate
		{
		});
		DebugLog.Log("[DIContainerInfrastructure] InitCurrentPlayerIfNecessary");
		CraftingService craftingService = DIContainerLogic.CraftingService;
		InventoryOperationServiceRealImpl inventoryService = DIContainerLogic.InventoryService;
		if (m_initializingCurrentPlayer)
		{
			DebugLog.Error("[DIContainerInfrastructure] InitCurrentPlayer is already initializing, don't call twice!");
			return;
		}
		if (restart)
		{
			m_currentPlayer = null;
		}
		if (m_currentPlayer != null)
		{
			return;
		}
		m_initializingCurrentPlayer = true;
		if (GetProfileMgr().LoadCurrentProfile())
		{
			if (DIContainerLogic.InventoryService.m_previousPlayerSaved)
			{
				DIContainerLogic.InventoryService.m_previousPlayerSaved = false;
			}
			if (DIContainerLogic.InventoryService.m_previousPlayerReason != null)
			{
				DIContainerLogic.InventoryService.m_previousPlayerReason = null;
			}
			try
			{
				DIContainerLogic.InventoryService.RemoveClassUpgradesOutsideBalancingData(GetProfileMgr().CurrentProfile);
				DIContainerLogic.InventoryService.RemoveEquipmentOutsideBalancingData(GetProfileMgr().CurrentProfile);
				DIContainerLogic.InventoryService.RemoveCraftingItemsOutsideBalancingData(GetProfileMgr().CurrentProfile);
			}
			catch
			{
			}
			m_currentPlayer = new PlayerGameData(GetProfileMgr().CurrentProfile);
			m_currentPlayer.FixRareCauldronCase();
			IsCurrentPlayerInitialized = true;
			m_initializingCurrentPlayer = false;
			onCompleted();
			return;
		}
		//RemoteStorageService.GetPrivateProfile(delegate(ABH.Shared.Models.PlayerData onlineProfile)
		//{
		//	if (onlineProfile != null && (onlineProfile.Experience > 0f || onlineProfile.Level > 1 || onlineProfile.IsUserConverted))
		//	{
		//		m_currentPlayer = new PlayerGameData(onlineProfile);
		//		DebugLog.Log(typeof(DIContainerInfrastructure), "ProfileProtection: Got a non-empty profile from the server, using it!");
		//	}
		//	else
		//	{
		//		DebugLog.Log(typeof(DIContainerInfrastructure), "ProfileProtection: Got an empty profile from the server, not using it!, acc = " + IdentityService.UserCredentials.UserName);
		//		CreateNewPlayerProfile();
		//	}
		//	IsCurrentPlayerInitialized = true;
		//	m_initializingCurrentPlayer = false;
		//	onCompleted();
		//}, delegate(string error)
		//{
		//	DebugLog.Log(typeof(DIContainerInfrastructure), "ProfileProtection: Got an error \"" + error + "\" from the server!");
		CreateNewPlayerProfile();
		IsCurrentPlayerInitialized = true;
		m_initializingCurrentPlayer = false;
		onCompleted();
		//});
	}

	private static void CreateNewPlayerProfile()
	{
		DebugLog.Log(typeof(DIContainerInfrastructure), "CreateNewPlayerProfile");
		m_currentPlayer = new PlayerGameData().Init("player");
		m_currentPlayer.Data.CreationDate = DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp();
		m_currentPlayer.Data.IsNewCreatedAccount = true;
		m_currentPlayer.SavePlayerDataManual();
		m_initializingCurrentPlayer = false;
	}

	public static CoreStateMgr GetCoreStateMgr()
	{
		if (CoreStateMgr.Instance != null)
		{
			m_coreStateMgr = CoreStateMgr.Instance;
		}
		else if (CoreStateMgr.Instance != null)
		{
			m_coreStateMgr = CoreStateMgr.Instance;
		}
		else if (CoreStateMgr.Instance != null)
		{
			m_coreStateMgr = CoreStateMgr.Instance;
		}
		else if (CoreStateMgr.Instance != null)
		{
			m_coreStateMgr = CoreStateMgr.Instance;
		}
		return m_coreStateMgr;
	}

	public static AsynchStatusService GetAsynchStatusService()
	{
		return ContentLoader.Instance.m_AsynchStatusService;
	}

	public static GenericAssetProvider GetClassAssetProvider()
	{
		return m_comboAssetProvider ?? (m_comboAssetProvider = (GetCoreStateMgr() ? GetCoreStateMgr().gameObject.GetComponentsInChildren<GenericAssetProvider>().FirstOrDefault((GenericAssetProvider c) => c.name.StartsWith("CharacterHighResAndEquipmentAssetProvider")) : null));
	}

	public static GenericAssetProvider GetEquipmentAssetProvider()
	{
		return m_comboAssetProvider ?? (m_comboAssetProvider = (GetCoreStateMgr() ? GetCoreStateMgr().gameObject.GetComponentsInChildren<GenericAssetProvider>().FirstOrDefault((GenericAssetProvider c) => c.name.StartsWith("CharacterHighResAndEquipmentAssetProvider")) : null));
	}

	public static GenericAssetProvider GetBannerAssetProvider()
	{
		return m_bannerAssetProvider ?? (m_bannerAssetProvider = (GetCoreStateMgr() ? GetCoreStateMgr().gameObject.GetComponentsInChildren<GenericAssetProvider>().FirstOrDefault((GenericAssetProvider c) => c.name.StartsWith("BannerAssetProvider")) : null));
	}

	public static GenericAssetProvider GetCharacterAssetProvider(bool isWorldmap)
	{
		if (isWorldmap || GetCompatibilityService().isLowEnd())
		{
			return m_characterLowResAssetProvider ?? (m_characterLowResAssetProvider = (GetCoreStateMgr() ? GetCoreStateMgr().gameObject.GetComponentsInChildren<GenericAssetProvider>().FirstOrDefault((GenericAssetProvider c) => c.name.StartsWith("CharacterLowResAssetProvider")) : null));
		}
		return m_comboAssetProvider ?? (m_comboAssetProvider = (GetCoreStateMgr() ? GetCoreStateMgr().gameObject.GetComponentsInChildren<GenericAssetProvider>().FirstOrDefault((GenericAssetProvider c) => c.name.StartsWith("CharacterHighResAndEquipmentAssetProvider")) : null));
	}

	public static GenericAssetProvider GetComicCutsceneAssetProvider()
	{
		return m_comicCutsceneProvider ?? (m_comicCutsceneProvider = (GetCoreStateMgr() ? GetCoreStateMgr().gameObject.GetComponentsInChildren<GenericAssetProvider>().FirstOrDefault((GenericAssetProvider c) => c.name.StartsWith("ComicCutsceneAssetProvider")) : null));
	}

	public static GenericAssetProvider GetBattlegroundAssetProvider()
	{
		return m_battlegroundAssetProvider ?? (GetCoreStateMgr() ? GetCoreStateMgr().gameObject.GetComponentsInChildren<GenericAssetProvider>().FirstOrDefault((GenericAssetProvider c) => c.name == "StreamingBattlegroundsAssetProvider") : null);
	}

	public static GenericAssetProvider GetWorldMapAssetProvider()
	{
		return m_worldMapAssetProvider ?? (GetCoreStateMgr() ? GetCoreStateMgr().gameObject.GetComponentsInChildren<GenericAssetProvider>().FirstOrDefault((GenericAssetProvider c) => c.name == "StreamingWorldMapAssetProvider") : null);
	}

	public static GenericAssetProvider GetShopIconAtlasAssetProvider()
	{
		return m_shopIconAssetProvider ?? (GetCoreStateMgr() ? GetCoreStateMgr().gameObject.GetComponentsInChildren<GenericAssetProvider>().FirstOrDefault((GenericAssetProvider c) => c.name == "ShopIconAtlasAssetProvider") : null);
	}

	public static GenericAssetProvider PropLiteAssetProvider()
	{
		return m_propLiteAssetProvider ?? (m_propLiteAssetProvider = (GetCoreStateMgr() ? GetCoreStateMgr().gameObject.GetComponentsInChildren<GenericAssetProvider>().FirstOrDefault((GenericAssetProvider c) => c.name == "PropLiteAssetProvider") : null));
	}

	public static GenericAssetProvider GetGenericIconAtlasAssetProvider()
	{
		return m_genericIconAtlasAssetProvider ?? (m_genericIconAtlasAssetProvider = (GetCoreStateMgr() ? GetCoreStateMgr().gameObject.GetComponentsInChildren<GenericAssetProvider>().FirstOrDefault((GenericAssetProvider c) => c.name == "GenericIconAtlasAssetProvider") : null));
	}

	public static GenericAssetProvider GetBattleEffectAssetProvider()
	{
		return m_battleEffectAssetProvider ?? (m_battleEffectAssetProvider = (GetCoreStateMgr() ? GetCoreStateMgr().gameObject.GetComponentsInChildren<GenericAssetProvider>().FirstOrDefault((GenericAssetProvider c) => c.name == "BattleEffectAssetProvider") : null));
	}

	public static GenericAssetProvider GetBundlePrefabAssetProvider()
	{
		return m_bundlePrefabAssetProvider ?? (m_bundlePrefabAssetProvider = (GetCoreStateMgr() ? GetCoreStateMgr().gameObject.GetComponentsInChildren<GenericAssetProvider>().FirstOrDefault((GenericAssetProvider c) => c.name.StartsWith("BundlePrefabsAssetProvider")) : null));
	}

	public static GenericAssetProvider GetAudioAssetProvider(AssetbundleIndex priority)
	{
		if (!m_audioAssetProvider.ContainsKey(priority))
		{
			m_audioAssetProvider.Add(priority, GetCoreStateMgr() ? GetCoreStateMgr().gameObject.GetComponentsInChildren<GenericAssetProvider>().FirstOrDefault((GenericAssetProvider c) => c.name.StartsWith("AudioAssetProviderPriority" + priority)) : null);
		}
		return m_audioAssetProvider[priority];
	}

	public static void UnloadAudioAssetProviders()
	{
		foreach (GenericAssetProvider value in m_audioAssetProvider.Values)
		{
			value.RemoveAssetLinks();
		}
	}

	public static NumberFormatProviderImpl GetFormatProvider()
	{
		return m_formatProvider ?? (m_formatProvider = new NumberFormatProviderImpl());
	}

	public static Version GetVersionService()
	{
		return m_version ?? (m_version = new Version());
	}

	public static AssetBundleCrcChecker GetAssetBundleCrcChecker()
	{
		return m_assetBundleCrcChecker ?? (m_assetBundleCrcChecker = new AssetBundleCrcChecker().Init());
	}

	public static IAchievementService GetAchievementService()
	{
		return m_achievementService ?? (m_achievementService = new AchievementServiceGooglePlayServicesImpl());
	}

	public static void InjectAchievementServiceFromWP8(IAchievementService xboxLiveAchievementService)
	{
		m_achievementService = xboxLiveAchievementService;
		DebugLog.Log("[DIContainerInfrastructure]", "InjectAchievementServiceFromWP8 was called with " + xboxLiveAchievementService.GetType());
	}

	public static ClientUpdateService GetClientUpdateService()
	{
		return m_clientUpdateService ?? (m_clientUpdateService = new ClientUpdateService());
	}

	public static AppResumeService GetAppResumeService()
	{
		return m_appResumeService ?? (m_appResumeService = new AppResumeService());
	}

	public static AndroidExpansionFileMgr GetAndroidExpansionFileMgr()
	{
		if (m_androidExpansionFileMgr == null)
		{
			string googlePlayKey = DIContainerConfig.GetGooglePlayKey();
			AndroidExpansionFileMgr androidExpansionFileMgr = new AndroidExpansionFileMgr();
			androidExpansionFileMgr.BASE64_PUBLIC_KEY = googlePlayKey;
			m_androidExpansionFileMgr = androidExpansionFileMgr;
			if (googlePlayKey.Length >= 30)
			{
				DebugLog.Log("[DIContainerInfrastructure] Using public key which ends with " + googlePlayKey.Substring(googlePlayKey.Length - 30));
			}
			else
			{
				DebugLog.Error("[DIContainerInfrastructure] public key is invalid (too short)");
			}
		}
		return m_androidExpansionFileMgr;
	}

	public static IAppAttributionService GetAttributionService()
	{
		return m_attributionService ?? (m_attributionService = new AdjustServiceNullImpl());
	}

	public static IAppTrackService GetComScoreService()
	{
		return m_comScoreService ?? (m_comScoreService = new ComScoreServiceNullImpl());
	}

	public static IAnalyticsSystem GetAnalyticsSystem(bool includeFlurry = false)
	{
		if (m_analyticsSystemBeaconOnly == null)
		{
			m_analyticsSystemBeaconOnly = new AnalyticsSystemNullImpl();
			m_analyticsSystemBeaconOnly.StartSession();
		}
		IAnalyticsSystem result;
		if (includeFlurry)
		{
			IAnalyticsSystem multipleAnalyticsSystem = GetMultipleAnalyticsSystem();
			result = multipleAnalyticsSystem;
		}
		else
		{
			result = m_analyticsSystemBeaconOnly;
		}
		return result;
	}

	private static IAnalyticsSystem GetMultipleAnalyticsSystem()
	{
		if (m_analyticsSystemCombo == null)
		{
			m_analyticsSystemCombo = new AnalyticsSystemMultipleImpl();
			AnalyticsSystemMultipleImpl analyticsSystemMultipleImpl = m_analyticsSystemCombo as AnalyticsSystemMultipleImpl;
			string appKey = "PJF8VW2Y44656TFZF5Z4";
			analyticsSystemMultipleImpl.Add(new AnalyticsSystemNullImpl().Init(appKey));
			analyticsSystemMultipleImpl.Add(m_analyticsSystemBeaconOnly);
			analyticsSystemMultipleImpl.StartSession();
		}
		return m_analyticsSystemCombo;
	}

	public static ICompressionService GetCompressionService()
	{
		return m_compressionService ?? (m_compressionService = new CompressionDotNetZipImpl());
	}

	public static IEncryptionService GetEncryptionService()
	{
		IEncryptionService encryptionService = m_encryptionService;
		if (encryptionService == null)
		{
			EncryptionServiceBouncyCastleImpl encryptionServiceBouncyCastleImpl = new EncryptionServiceBouncyCastleImpl();
			encryptionServiceBouncyCastleImpl.KeyLength = 24;
			encryptionServiceBouncyCastleImpl.InitializationVector = new byte[8] { 5, 24, 123, 22, 77, 234, 45, 66 };
			encryptionService = (m_encryptionService = encryptionServiceBouncyCastleImpl);
		}
		return encryptionService;
	}

	public static ICompatibilityService GetCompatibilityService()
	{
		return m_compatibilityService ?? (m_compatibilityService = new CompatibilityAndroidImpl());
	}

	public static NotificationPermissionMgr GetNotificationPermissionMgr()
	{
		return m_notificationPermissionMgr ?? (m_notificationPermissionMgr = new NotificationPermissionMgr());
	}

    public static SkynestMetaDataCache GetSkynestMetaDataCache()
    {
        return m_skynestMetadataCache ?? (m_skynestMetadataCache = new SkynestMetaDataCache());
    }

    public static CustomMessageService GetCustomMessageService()
	{
		return m_customMessageService ?? (m_customMessageService = new CustomMessageService());
	}

	public static PlaymobService GetPlaymobService()
	{
		return m_playmobService ?? (m_playmobService = new PlaymobService());
	}

	public static PowerLevelCalculator GetPowerLevelCalculator()
	{
		return m_powerLevelCalculator ?? (m_powerLevelCalculator = new PowerLevelCalculator());
	}
}

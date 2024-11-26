using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using UnityEngine;

public class ClientInfo
{
	public const string AppleId = "";

	public static string ClientIp;

	public static BattleStartGameData CurrentBattleStartGameData;

	public static BattleGameData CurrentBattleGameData;

	public static CoreStateMgr CoreStateMgr;

	public static InventoryGameData CurrentCampInventory;

	public static bool IsFriend = false;

	public static bool HasFreeRoll = false;

	public static FriendGameData InspectedFriend;

	public static List<BirdGameData> CurrentCampBirds;

	public static Dictionary<string, float> LoadingTracking = new Dictionary<string, float>();

	public static float lastTimeInSeconds;

	public static bool IsDebugGuiEnabled = true;

	public static bool IsAdCooldownEnabled = true;

	public static bool ShowMasteryConversionPopup;

	public static BannerGameData CurrentBanner;

	public static void AddLoadingTracking(string stateToTrack)
	{
		if (!LoadingTracking.ContainsKey(stateToTrack))
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			LoadingTracking.Add(stateToTrack, realtimeSinceStartup - lastTimeInSeconds);
			lastTimeInSeconds = realtimeSinceStartup;
		}
	}
}

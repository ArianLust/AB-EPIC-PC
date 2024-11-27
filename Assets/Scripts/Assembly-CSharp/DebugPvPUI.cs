using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using UnityEngine;

public class DebugPvPUI : GenericDebugUI
{
	private bool m_Opened;

	private string m_CurrentName = "unkonwn";

	private List<string> m_PlayerBirdNames = new List<string> { "bird_red", "bird_yellow", "bird_white" };

	private int enemyLevel = 10;

	private List<string> m_EnemyClassNames = new List<string> { "class_knight", "class_mage", "class_cleric" };

	private string m_PlayerBannerTip = "banner_tip_00";

	private string m_EnemyBannerTip = "banner_tip_00";

	private string m_PlayerBannerEmblem = "banner_emblem_00";

	private string m_EnemyBannerEmblem = "banner_emblem_00";

	private string m_PlayerBanner = "banner_banner_00";

	private string m_EnemyBanner = "banner_banner_00";

	private Vector2 scrollPosition;

	private List<ClassItemBalancingData> classBalancing;

	private bool m_EnemyBirdsOpened;

	private bool m_OwnBirdsOpened;

	private List<BirdGameData> playerBirds;

	private bool m_OwnTipOpened;

	private List<BannerItemBalancingData> tipBalancing;

	private bool m_EnemyTipOpened;

	private bool m_OwnBaseOpened;

	private List<BannerItemBalancingData> bannerBalancing;

	private bool m_EnemyBannerOpened;

	private bool m_OwnEmblemOpened;

	private List<BannerItemBalancingData> emblemBalancing;

	private bool m_EnemyEmblemOpened;

	private int enemyMasteryLevel = 1;

	private BoxCollider m_boxcollider;

	private string enemyLevelString;

	private string enemyMasterString;

	public static int coinStart;
}

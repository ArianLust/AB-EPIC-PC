using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ABH.GameDatas;
using ABH.Shared.Models.Character;
using UnityEngine;

public class PvpSeasonResultUI : MonoBehaviour
{
	[SerializeField]
	private UILabel m_seasonFinishedDescription;

	[SerializeField]
	private UIInputTrigger m_closeButtonTrigger;

	[SerializeField]
	private UIInputTrigger m_checkOutButtonTrigger;

	[SerializeField]
	private UISprite m_avatarBorder;

	[SerializeField]
	private CHMeshSprite m_trophySprite;

	[SerializeField]
	private UITexture m_playerAvatar;

	public bool m_SeasonendPopupshowing;

	private int m_seasonInteger;

	private WWW m_OpponentTextureDownload;

	private Texture2D m_OpponentTexture;

	private void Awake()
	{
		base.gameObject.SetActive(false);
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_SeasonEndPopup = this;
	}

	public void Init()
	{
		base.gameObject.SetActive(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showEnergy = false,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false
		}, false);
		m_SeasonendPopupshowing = true;
		string cachedSeasonName = DIContainerInfrastructure.GetCurrentPlayer().Data.m_CachedSeasonName;
		if (!string.IsNullOrEmpty(cachedSeasonName))
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			m_seasonInteger = int.Parse(Regex.Match(cachedSeasonName, "\\d+").Value);
			string value = cachedSeasonName.Replace(m_seasonInteger.ToString(), (m_seasonInteger + 1).ToString());
			dictionary.Add("{value_1}", cachedSeasonName);
			dictionary.Add("{value_2}", value);
			m_seasonFinishedDescription.text = DIContainerInfrastructure.GetLocaService().Tr("seasonfinished_desc", dictionary);
		}
		if (!string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.SocialPictureUrl))
		{
			StartCoroutine(LoadTexture());
		}
		RegisterEventHandler();
		CheckSeasonEndRewards();
		base.gameObject.PlayAnimationOrAnimatorState("Popup_SeasonFinished_Enter");
	}

	private void RegisterEventHandler()
	{
		DeregisterEventHandler();
		m_closeButtonTrigger.Clicked += ClosePopup;
		m_checkOutButtonTrigger.Clicked += CheckOut;
	}

	private void DeregisterEventHandler()
	{
		m_closeButtonTrigger.Clicked -= ClosePopup;
		m_checkOutButtonTrigger.Clicked -= CheckOut;
	}

	private IEnumerator LoadTexture()
	{
		if (m_OpponentTextureDownload == null)
		{
			m_OpponentTextureDownload = new WWW(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.SocialPictureUrl);
		}
		while (m_OpponentTextureDownload != null && !m_OpponentTextureDownload.isDone)
		{
			yield return new WaitForSeconds(0.5f);
		}
		if (string.IsNullOrEmpty(m_OpponentTextureDownload.error))
		{
			m_playerAvatar.mainTexture = m_OpponentTextureDownload.texture;
			DIContainerInfrastructure.GetCoreStateMgr().SaveSocialAvatar(m_OpponentTextureDownload, string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.SocialId) ? DIContainerInfrastructure.IdentityService.SharedId : DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.SocialId);
		}
		else
		{
			m_playerAvatar.mainTexture = DIContainerInfrastructure.GetCoreStateMgr().LoadSocialAvatar(string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.SocialId) ? DIContainerInfrastructure.IdentityService.SharedId : DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.SocialId);
		}
	}

	private void CheckOut()
	{
		ArenaCampStateMgr arenaCampStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_ArenaCampStateMgr;
		if (arenaCampStateMgr != null)
		{
			arenaCampStateMgr.ShowTrophyManager();
		}
		ClosePopup();
	}

	private void ClosePopup()
	{
		StartCoroutine(LeaveCoroutine());
	}

	private IEnumerator LeaveCoroutine()
	{
		DeregisterEventHandler();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(5u);
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_SeasonFinished_Leave"));
		m_SeasonendPopupshowing = false;
		base.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		DeregisterEventHandler();
	}

	private void CheckSeasonEndRewards()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		int highestFinishedLeague = currentPlayer.Data.HighestFinishedLeague;
		string text = string.Empty;
		switch (highestFinishedLeague)
		{
		case 1:
			m_avatarBorder.spriteName = "WoodLeague";
			text = "Season" + ((m_seasonInteger - 1) % 5 + 1) + "Wood";
			break;
		case 2:
			m_avatarBorder.spriteName = "StoneLeague";
			text = "Season" + ((m_seasonInteger - 1) % 5 + 1) + "Stone";
			break;
		case 3:
			m_avatarBorder.spriteName = "SilverLeague";
			text = "Season" + ((m_seasonInteger - 1) % 5 + 1) + "Silver";
			break;
		case 4:
			m_avatarBorder.spriteName = "GoldLeague";
			text = "Season" + ((m_seasonInteger - 1) % 5 + 1) + "Gold";
			break;
		case 5:
			m_avatarBorder.spriteName = "PlatinumLeague";
			text = "Season" + ((m_seasonInteger - 1) % 5 + 1) + "Platinum";
			break;
		case 6:
			m_avatarBorder.spriteName = "DiamondLeague";
			text = "Season" + ((m_seasonInteger - 1) % 5 + 1) + "Diamond";
			break;
		}
		m_avatarBorder.MakePixelPerfect();
		m_trophySprite.m_SpriteName = text;
		m_trophySprite.UpdateSprite(false, true);
		BasicItemGameData basicItemGameData = new BasicItemGameData("pvp_trophy");
		basicItemGameData.ItemData.Quality = highestFinishedLeague;
		basicItemGameData.ItemData.Level = m_seasonInteger;
		currentPlayer.RemoveInvalidTrophiesForSeason(m_seasonInteger);
		currentPlayer.InventoryGameData.AddNewItemToInventory(basicItemGameData);
		TrophyData trophyData = new TrophyData();
		trophyData.NameId = text;
		trophyData.Seasonid = m_seasonInteger;
		trophyData.FinishedLeagueId = highestFinishedLeague;
		currentPlayer.Data.PvPTrophy = trophyData;
		currentPlayer.Data.HighestFinishedLeague = 0;
		currentPlayer.SavePlayerData();
	}

	public void ShowTrophyPopup()
	{
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.PvPTrophy != null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowTrophyOverlay(base.transform, DIContainerInfrastructure.GetCurrentPlayer().Data.PvPTrophy, true);
		}
	}

	public void HideAllTooltips()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
	}
}

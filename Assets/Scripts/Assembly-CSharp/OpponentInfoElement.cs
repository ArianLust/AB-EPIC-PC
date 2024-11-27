using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using UnityEngine;

public class OpponentInfoElement : MonoBehaviour
{
	public enum NicknameAllowed
	{
		Allowed = 0,
		NotAllowed = 1,
		NicknameCheckFailure = 2
	}

	[SerializeField]
	private UITexture m_OpponentAvatar;

	[SerializeField]
	private UISprite m_NPCAvatar;

	[SerializeField]
	private UILabel m_OpponentNameLabel;

	[SerializeField]
	private ResourceCostBlind m_RankBonus;

	[SerializeField]
	private ResourceCostBlind m_RankBonusWithLeagueChange;

	[SerializeField]
	private UISprite m_PromotionIndicatorWithRankBonus;

	[SerializeField]
	private UISprite m_CrownWithRankBonus;

	[SerializeField]
	private GameObject m_PromotionIndicatorRootWithoutRankBonus;

	[SerializeField]
	private UISprite m_PromotionIndicatorWithoutRankBonus;

	[SerializeField]
	private UISprite m_CrownWithoutRankBonus;

	[SerializeField]
	private UILabel m_LevelLabel;

	[SerializeField]
	private UILabel m_RankLabel;

	[SerializeField]
	private UILabel m_ScoreLabel;

	[SerializeField]
	private GameObject m_ScoreLabelRoot;

	[SerializeField]
	private GameObject m_EditButton;

	[SerializeField]
	private GameObject m_PlayerIndicator;

	[SerializeField]
	private List<GameObject> m_StarIndicators = new List<GameObject>();

	private OpponentGameData m_Model;

	[SerializeField]
	private GameObject m_UpdateIndicator;

	[SerializeField]
	public UIInputTrigger m_ElementPressedTrigger;

	[SerializeField]
	public UISprite m_AvatarBorder;

	[SerializeField]
	public GameObject m_CheaterIconStars;

	[SerializeField]
	public GameObject m_CheaterIconScore;

	[SerializeField]
	private UIInput m_input;

	private bool m_destroyed;

	private bool m_isFriend;

	private string m_enteredNickname;

	private string m_TranslatedNickname;

	private string m_TranslatingNicknameErrorMessage;

	public void SetModel(OpponentGameData opponentData, bool isPlayer, bool isFriend = false)
	{
		m_Model = opponentData;
		RegisterEventHandlers();
		InvokeRepeating("CheckIfLoaded", 0.1f, 0.1f);
		CheckIfLoaded();
		if ((bool)m_EditButton)
		{
			m_EditButton.SetActive(opponentData.IsSelf);
		}
		if (isPlayer && (bool)m_PlayerIndicator)
		{
			m_PlayerIndicator.SetActive(true);
		}
		if ((bool)m_RankLabel)
		{
			m_RankLabel.gameObject.SetActive(!isFriend);
		}
		m_isFriend = isFriend;
	}

	public void SetNew(bool isNew)
	{
		if ((bool)m_UpdateIndicator)
		{
			m_UpdateIndicator.SetActive(isNew);
		}
	}

	public void SetDefault(int score, int rank, int starRating, bool pvp, bool isFriend = false, bool isSelf = false)
	{
		if ((bool)m_OpponentAvatar)
		{
			m_OpponentAvatar.gameObject.SetActive(false);
			m_OpponentAvatar.material = new Material(m_OpponentAvatar.material);
		}
		if ((bool)m_OpponentNameLabel)
		{
			m_OpponentNameLabel.text = DIContainerInfrastructure.GetLocaService().Tr("gen_opponent_unkown", "Unnamed Player");
		}
		if ((bool)m_EditButton)
		{
			m_EditButton.SetActive(false);
		}
		if ((bool)m_LevelLabel)
		{
			m_LevelLabel.text = string.Empty;
		}
		if ((bool)m_RankLabel)
		{
			m_RankLabel.text = ((score > 0) ? rank.ToString("0") : "-");
		}
		if ((bool)m_ScoreLabel)
		{
			if (score < 0)
			{
				score = 0;
			}
			m_ScoreLabel.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(score);
		}
		if (m_StarIndicators.Count > 0)
		{
			for (int i = 0; i < m_StarIndicators.Count; i++)
			{
				GameObject gameObject = m_StarIndicators[i];
				if (isFriend)
				{
					gameObject.SetActive(false);
					if ((bool)m_RankBonus)
					{
						m_RankBonus.gameObject.SetActive(false);
					}
					if ((bool)m_RankBonusWithLeagueChange)
					{
						m_RankBonusWithLeagueChange.gameObject.SetActive(false);
					}
				}
				else if (pvp)
				{
					gameObject.SetActive(i == starRating);
				}
				else
				{
					gameObject.SetActive(i == starRating - 1);
				}
			}
		}
		bool hasRankBonusItem = false;
		bool hasProOrDemotion = false;
		if (pvp)
		{
			if (score > 0 && DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(DIContainerInfrastructure.GetCurrentPlayer()) && DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData.Balancing.PvPBonusLootTablesPerRank.Count > rank - 1)
			{
				hasRankBonusItem = true;
			}
			if (DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(DIContainerInfrastructure.GetCurrentPlayer()))
			{
				PvPSeasonManagerGameData currentPvPSeasonGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData;
				if ((starRating <= 0 && currentPvPSeasonGameData.Data.CurrentLeague > 1) || (starRating > 2 && currentPvPSeasonGameData.Data.CurrentLeague < currentPvPSeasonGameData.Balancing.MaxLeague) || (score <= 0 && currentPvPSeasonGameData.Data.CurrentLeague > 1))
				{
					hasProOrDemotion = true;
				}
			}
		}
		if ((bool)m_RankBonus && !isFriend)
		{
			if (pvp)
			{
				HandleRankBonusPvP(score, rank, hasRankBonusItem, hasProOrDemotion, starRating);
			}
			else
			{
				HandleRankBonusEvents(score, rank);
			}
		}
	}

	private void HandleRankBonusPvP(int score, int rank, bool hasRankBonusItem, bool hasProOrDemotion, int starRating)
	{
		if (score > 0 && DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(DIContainerInfrastructure.GetCurrentPlayer()) && DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData.Balancing.PvPBonusLootTablesPerRank.Count > rank - 1)
		{
			PvPSeasonManagerGameData currentPvPSeasonGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData;
			int currentLeague = currentPvPSeasonGameData.Data.CurrentLeague;
			currentLeague = ((starRating > 0) ? Mathf.Min(currentLeague + 1, currentPvPSeasonGameData.Balancing.MaxLeague) : Mathf.Max(1, currentLeague - 1));
			List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(new Dictionary<string, int> { 
			{
				DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData.CurrentSeasonTurn.GetScalingRankRewardLootTable(rank),
				1
			} }, DIContainerInfrastructure.GetCurrentPlayer().Data.Level));
			IInventoryItemGameData inventoryItemGameData = itemsFromLoot.FirstOrDefault();
			if (inventoryItemGameData != null)
			{
				if (hasProOrDemotion)
				{
					m_RankBonus.gameObject.SetActive(false);
					if ((bool)m_RankBonusWithLeagueChange)
					{
						m_RankBonus.gameObject.SetActive(false);
						m_RankBonusWithLeagueChange.gameObject.SetActive(true);
						m_RankBonusWithLeagueChange.SetModel(inventoryItemGameData.ItemAssetName, null, inventoryItemGameData.ItemValue, string.Empty);
						if (m_CrownWithRankBonus != null)
						{
							m_CrownWithRankBonus.spriteName = PvPSeasonManagerGameData.GetLeagueAssetName(currentLeague);
						}
						if (m_PromotionIndicatorWithRankBonus != null)
						{
							m_PromotionIndicatorWithRankBonus.spriteName = ((starRating > 0) ? "Arrow_Up" : "Arrow_Down");
						}
					}
				}
				else
				{
					m_RankBonus.gameObject.SetActive(true);
					m_RankBonus.SetModel(inventoryItemGameData.ItemAssetName, null, inventoryItemGameData.ItemValue, string.Empty);
				}
				return;
			}
			m_RankBonus.gameObject.SetActive(false);
			if (hasProOrDemotion)
			{
				m_RankBonusWithLeagueChange.gameObject.SetActive(true);
				m_RankBonusWithLeagueChange.SetModel(null, null, string.Empty, string.Empty);
				m_RankBonusWithLeagueChange.transform.Find("Label").gameObject.SetActive(false);
				if (m_CrownWithRankBonus != null)
				{
					m_CrownWithRankBonus.spriteName = PvPSeasonManagerGameData.GetLeagueAssetName(currentLeague);
				}
				if (m_PromotionIndicatorWithRankBonus != null)
				{
					m_PromotionIndicatorWithRankBonus.spriteName = ((starRating > 0) ? "Arrow_Up" : "Arrow_Down");
				}
			}
		}
		else if (hasProOrDemotion && !m_isFriend)
		{
			PvPSeasonManagerGameData currentPvPSeasonGameData2 = DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData;
			if (currentPvPSeasonGameData2 != null)
			{
				m_RankBonus.gameObject.SetActive(false);
				int currentLeague2 = currentPvPSeasonGameData2.Data.CurrentLeague;
				currentLeague2 = ((starRating > 0) ? Mathf.Min(currentLeague2 + 1, currentPvPSeasonGameData2.Balancing.MaxLeague) : Mathf.Max(1, currentLeague2 - 1));
				m_RankBonusWithLeagueChange.gameObject.SetActive(true);
				m_RankBonusWithLeagueChange.SetModel(null, null, string.Empty, string.Empty);
				m_RankBonusWithLeagueChange.transform.Find("Label").gameObject.SetActive(false);
				if (m_CrownWithRankBonus != null)
				{
					m_CrownWithRankBonus.spriteName = PvPSeasonManagerGameData.GetLeagueAssetName(currentLeague2);
				}
				if (m_PromotionIndicatorWithRankBonus != null)
				{
					m_PromotionIndicatorWithRankBonus.spriteName = ((starRating > 0) ? "Arrow_Up" : "Arrow_Down");
				}
			}
		}
		else
		{
			m_RankBonus.gameObject.SetActive(false);
		}
	}

	private void HandleRankBonusEvents(int score, int rank)
	{
		if (score > 0 && DIContainerLogic.EventSystemService.IsCurrentEventAvailable(DIContainerInfrastructure.GetCurrentPlayer()) && DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.EventBalancing.EventBonusLootTablesPerRank.Count > rank - 1)
		{
			List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(new Dictionary<string, int> { 
			{
				DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.GetScalingRankRewardLootTable(rank),
				1
			} }, DIContainerInfrastructure.GetCurrentPlayer().Data.Level));
			IInventoryItemGameData inventoryItemGameData = itemsFromLoot.FirstOrDefault();
			if (inventoryItemGameData != null)
			{
				m_RankBonus.gameObject.SetActive(true);
				m_RankBonus.SetModel(inventoryItemGameData.ItemAssetName, null, inventoryItemGameData.ItemValue, string.Empty);
			}
			else
			{
				m_RankBonus.gameObject.SetActive(false);
			}
		}
		else
		{
			m_RankBonus.gameObject.SetActive(false);
		}
	}

	private void RefreshInfos()
	{
		if (m_AvatarBorder != null && m_Model.PublicPlayerData.Trophy != null && m_Model.PublicPlayerData.Trophy.FinishedLeagueId > 0)
		{
			m_AvatarBorder.gameObject.SetActive(true);
			switch (m_Model.PublicPlayerData.Trophy.FinishedLeagueId)
			{
			case 1:
				m_AvatarBorder.spriteName = "WoodLeague";
				break;
			case 2:
				m_AvatarBorder.spriteName = "StoneLeague";
				break;
			case 3:
				m_AvatarBorder.spriteName = "SilverLeague";
				break;
			case 4:
				m_AvatarBorder.spriteName = "GoldLeague";
				break;
			case 5:
				m_AvatarBorder.spriteName = "PlatinumLeague";
				break;
			case 6:
				m_AvatarBorder.spriteName = "DiamondLeague";
				break;
			}
			m_AvatarBorder.MakePixelPerfect();
		}
		else if ((bool)m_AvatarBorder)
		{
			m_AvatarBorder.gameObject.SetActive(false);
		}
		if (string.IsNullOrEmpty(m_Model.PublicPlayerData.SocialAvatarUrl))
		{
			if ((bool)m_OpponentAvatar)
			{
				m_OpponentAvatar.gameObject.SetActive(false);
			}
			if ((bool)m_NPCAvatar)
			{
				m_NPCAvatar.gameObject.SetActive(true);
				m_NPCAvatar.spriteName = GetNPCSprite(string.Empty);
			}
		}
		else if ((bool)m_OpponentAvatar)
		{
			if (m_Model.OpponentTexture != null && m_Model.OpponentTexture.height != 8 && m_Model.OpponentTexture.width != 8)
			{
				if ((bool)m_OpponentAvatar)
				{
					m_OpponentAvatar.gameObject.SetActive(true);
					m_OpponentAvatar.mainTexture = m_Model.OpponentTexture;
				}
				if ((bool)m_NPCAvatar)
				{
					m_NPCAvatar.gameObject.SetActive(false);
				}
			}
			else
			{
				if ((bool)m_OpponentAvatar)
				{
					m_OpponentAvatar.gameObject.SetActive(false);
				}
				if ((bool)m_NPCAvatar)
				{
					m_NPCAvatar.gameObject.SetActive(true);
					m_NPCAvatar.spriteName = GetNPCSprite(string.Empty);
				}
			}
		}
		if (m_Model != null)
		{
			if ((bool)m_OpponentNameLabel)
			{
				m_OpponentNameLabel.text = DIContainerInfrastructure.GetLocaService().ReplaceUnmappableCharacters(m_Model.OpponentName);
			}
			if ((bool)m_LevelLabel)
			{
				m_LevelLabel.text = m_Model.OpponentLevel.ToString("0");
			}
		}
		else
		{
			if ((bool)m_OpponentNameLabel)
			{
				m_OpponentNameLabel.text = DIContainerInfrastructure.GetLocaService().Tr("friends_loading", "Loading...");
			}
			if ((bool)m_LevelLabel)
			{
				m_LevelLabel.text = string.Empty;
			}
		}
	}

	private string GetNPCSprite(string id)
	{
		switch (id)
		{
		case "NPC_Porky":
			return "Avatar_PrincePorky";
		case "NPC_Adventurer":
			return "Avatar_Adventurer";
		case "NPC_Low":
			return "Avatar_MerchantPig";
		case "NPC_High":
			return "Avatar_MightyEagle";
		default:
			return "Avatar_" + id;
		}
	}

	private void CheckIfLoaded()
	{
		RefreshInfos();
		if (m_Model.OpponentTextureIsLoaded)
		{
			CancelInvoke("CheckIfLoaded");
		}
		else if (m_Model != null && !m_Model.OpponentTextureIsLoaded && !m_Model.OpponentTextureIsLoading)
		{
			if ((bool)m_OpponentAvatar)
			{
				m_OpponentAvatar.mainTexture = m_Model.OpponentTexture;
			}
			if (string.IsNullOrEmpty(m_Model.PublicPlayerData.SocialAvatarUrl) && (bool)m_NPCAvatar)
			{
				if ((bool)m_OpponentAvatar)
				{
					m_OpponentAvatar.gameObject.SetActive(false);
				}
				m_NPCAvatar.gameObject.SetActive(true);
				m_NPCAvatar.spriteName = GetNPCSprite(string.Empty);
				return;
			}
			if ((bool)m_OpponentAvatar)
			{
				m_OpponentAvatar.gameObject.SetActive(true);
			}
			if ((bool)m_NPCAvatar)
			{
				m_NPCAvatar.gameObject.SetActive(false);
			}
		}
		else
		{
			if ((bool)m_OpponentAvatar)
			{
				m_OpponentAvatar.gameObject.SetActive(false);
			}
			if ((bool)m_NPCAvatar)
			{
				m_NPCAvatar.gameObject.SetActive(true);
			}
		}
	}

	private void OnDestroy()
	{
		m_destroyed = true;
		DeRegisterEventHandlers();
		if (m_Model != null)
		{
			m_Model.UnloadFriendTexture();
		}
		CancelInvoke();
	}

	private void DeRegisterEventHandlers()
	{
		if (m_Model != null)
		{
			m_Model.OnTextureUnloaded -= OnTextureUnloaded;
		}
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		if (m_Model != null)
		{
			m_Model.OnTextureUnloaded += OnTextureUnloaded;
		}
	}

	private void OnTextureUnloaded()
	{
		if (!m_destroyed && (bool)base.gameObject)
		{
			CancelInvoke();
			InvokeRepeating("CheckIfLoaded", 0.1f, 0.1f);
		}
	}

	public void SetNPCIcon(bool set)
	{
		if ((bool)m_OpponentAvatar)
		{
			m_OpponentAvatar.gameObject.SetActive(false);
		}
		if ((bool)m_NPCAvatar)
		{
			m_NPCAvatar.gameObject.SetActive(set);
			m_NPCAvatar.spriteName = GetNPCSprite("Avatar_Generic");
		}
	}

	public void OnSubmit()
	{
		string value = m_input.value;
		if (string.IsNullOrEmpty(value))
		{
			StartCoroutine(ResetLabel());
			return;
		}
		m_enteredNickname = value;
		DIContainerInfrastructure.IdentityService.ValidateNickname(value, CheckBlackListSuccess, CheckBlackListFailed);
	}

	private IEnumerator ResetLabel()
	{
		yield return new WaitForEndOfFrame();
		m_OpponentNameLabel.text = m_enteredNickname;
	}

	private void CheckBlackListSuccess(bool success, string message)
	{
		if (ContainsSymbol(m_enteredNickname))
		{
			CheckBlackListFailed((message != "NICKNAME_OK") ? message : "NICKNAME_BLACKLISTED");
			return;
		}
		StartCoroutine(TranslateNickname(m_enteredNickname, delegate
		{
			NicknameAllowed nicknameAllowed = IsNicknameAllowed(m_enteredNickname);
			if (success && nicknameAllowed == NicknameAllowed.Allowed)
			{
				DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.EventPlayerName = m_enteredNickname;
				m_OpponentNameLabel.text = DIContainerInfrastructure.GetLocaService().ReplaceUnmappableCharacters(m_enteredNickname);
				DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			}
			else if (success && nicknameAllowed == NicknameAllowed.NicknameCheckFailure)
			{
				StartCoroutine(DelayedResetName());
			}
			else
			{
				CheckBlackListFailed((message != "NICKNAME_OK") ? message : "NICKNAME_BLACKLISTED");
			}
		}));
	}

	private void CheckBlackListFailed(string message)
	{
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_playername_blacklist"), "blacklistfail", DispatchMessage.Status.Error);
		DebugLog.Warn("Invalid nickname entered: " + message);
		StartCoroutine(DelayedResetName());
	}

	private IEnumerator DelayedResetName()
	{
		yield return new WaitForEndOfFrame();
		string realName = DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.EventPlayerName;
		Debug.LogError("realname: " + realName);
		m_OpponentNameLabel.text = DIContainerInfrastructure.GetLocaService().ReplaceUnmappableCharacters(realName);
		Debug.LogError("m_OpponentNameLabel.text: " + m_OpponentNameLabel.text);
	}

	public void SetCheater(bool isCheating)
	{
		if (isCheating)
		{
			if (m_ScoreLabelRoot != null)
			{
				m_ScoreLabelRoot.SetActive(false);
			}
			if (m_RankBonus != null)
			{
				m_RankBonus.gameObject.SetActive(false);
			}
			foreach (GameObject starIndicator in m_StarIndicators)
			{
				starIndicator.SetActive(false);
			}
		}
		if (m_CheaterIconStars != null)
		{
			m_CheaterIconStars.SetActive(isCheating);
		}
		if (m_CheaterIconScore != null)
		{
			m_CheaterIconScore.SetActive(isCheating);
		}
	}

	private IEnumerator TranslateNickname(string nickname, Action onFinishCallback)
	{
		if (string.IsNullOrEmpty(nickname))
		{
			Debug.LogError("[OpponentInfoElement] Nickname is not yet set");
			yield break;
		}
		WWW www = new WWW("https://translate.google.com/translate_a/single?dj=1&q=" + nickname + "&sl=auto&tl=en&hl=id-ID&ie=UTF-8&oe=UTF-8&client=at&dt=t&dt=ld&dt=qca&dt=rm&dt=bd&dt=md&dt=ss&dt=ex&source=t2t_rd&otf=3", null, new Dictionary<string, string> { { "User-Agent", "GoogleTranslate/6.3.0.RC06.277163268" } });
		yield return www;
		if (www.isDone && string.IsNullOrEmpty(www.error))
		{
			if (string.IsNullOrEmpty(www.text))
			{
				Debug.LogError("[OpponentInfoElement] Text is empty, quitting translate nickname");
				onFinishCallback();
				yield break;
			}
			if (!www.text.Contains("sentences") && !www.text.Contains("src"))
			{
				Debug.LogError("[OpponentInfoElement] Unsupported response code: " + www.text);
				StartCoroutine(ContentLoader.Instance.SubmitErrorReport("Unsupported response code: " + www.text));
				onFinishCallback();
				yield break;
			}
			m_TranslatedNickname = ProcessTranslatedNickname(www.text, nickname);
		}
		else if (www.isDone && !string.IsNullOrEmpty(www.error))
		{
			string text = (string.IsNullOrEmpty(www.text) ? null : ("\r\nError source: " + www.text));
			StartCoroutine(ContentLoader.Instance.SubmitErrorReport("Error msg: " + www.error + text));
			m_TranslatingNicknameErrorMessage = www.error;
			Debug.LogError("[OpponentInfoElement] Translating nickname failed: " + www.error);
		}
		onFinishCallback();
	}

	private NicknameAllowed IsNicknameAllowed(string nickname)
	{
		if (string.IsNullOrEmpty(m_TranslatedNickname) && m_TranslatingNicknameErrorMessage != "429: Too Many Requests")
		{
			Debug.LogError("[OpponentInfoElement] Translated nickname is null");
			return NicknameAllowed.NicknameCheckFailure;
		}
		string text = ((m_TranslatingNicknameErrorMessage != "429: Too Many Requests") ? m_TranslatedNickname.ToLower() : nickname.ToLower());
		if (text.Contains("boob") || text.Contains("pussy") || text.Contains("vagina") || text.Contains("dick") || text.Contains("arse") || text.Contains("sexy") || CheckWithSymbolOrSpaceFilter(text, "poo") || text.Contains("fard") || text.Contains("fart") || CheckWithSymbolOrSpaceFilter(text, "pee") || text.Contains("porn") || ContainsSymbol(text) || CheckWithSymbolOrSpaceFilter(text, "butt") || text.Contains("xxx") || text.Contains("penis") || text.Contains("poop") || text.Contains("shart") || CheckWithSymbolOrSpaceFilter(text, "bra") || text.Contains("bralette") || text.Contains("bikini"))
		{
			return NicknameAllowed.NotAllowed;
		}
		return NicknameAllowed.Allowed;
	}

	private bool CheckWithSymbolOrSpaceFilter(string str, string value)
	{
		if (!(str == value) && !str.Contains(" " + value) && !str.Contains(value + " ") && !str.Contains("-" + value) && !str.Contains(value + "-") && !str.Contains("_" + value) && !str.Contains(value + "_") && !str.Contains("+" + value) && !str.Contains(value + "+") && !str.Contains("," + value) && !str.Contains(value + ",") && !str.Contains("." + value) && !str.Contains(value + ".") && !str.Contains(";" + value) && !str.Contains(value + ";") && !str.Contains("'" + value) && !str.Contains(value + "'") && !str.Contains("\"" + value) && !str.Contains(value + "\"") && !str.Contains("/" + value) && !str.Contains(value + "/") && !str.Contains("(" + value + ")") && !str.Contains("[" + value + "]"))
		{
			return str.Contains("{" + value + "}");
		}
		return true;
	}

	private string ProcessTranslatedNickname(string googleTranslateJsonResponse, string nicknameFallback)
	{
		string result = null;
		foreach (KeyValuePair<string, object> item in SimpleJsonConverter.DecodeJsonDict(googleTranslateJsonResponse))
		{
			if (item.Key.ToLower().Equals("sentences"))
			{
				foreach (object item2 in item.Value as IEnumerable)
				{
					Dictionary<string, object> dictionary = item2 as Dictionary<string, object>;
					if (dictionary.ContainsKey("trans"))
					{
						result = dictionary["trans"].ToString();
					}
				}
			}
			if (item.Key.ToLower().Equals("src"))
			{
				if (item.Value.ToString() == "en")
				{
					result = nicknameFallback;
				}
				break;
			}
		}
		return result;
	}

	private bool ContainsSymbol(string nickname)
	{
		if (!nickname.Contains("@") && !nickname.Contains("#") && !nickname.Contains("%") && !nickname.Contains("^") && !nickname.Contains("&"))
		{
			return nickname.Contains("*");
		}
		return true;
	}
}

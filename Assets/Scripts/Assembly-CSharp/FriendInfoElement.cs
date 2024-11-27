using ABH.GameDatas;
using UnityEngine;

public class FriendInfoElement : MonoBehaviour
{
	[SerializeField]
	private UITexture m_FriendAvatar;

	[SerializeField]
	public UISprite m_NPCAvatar;

	[SerializeField]
	private UILabel m_FriendNameLabel;

	[SerializeField]
	private UILabel m_LastPlayLabel;

	[SerializeField]
	private UILabel m_LevelLabel;

	private FriendGameData m_Model;

	[SerializeField]
	private GameObject m_UpdateIndicator;

	[SerializeField]
	public UISprite m_ArenaIcon;

	[SerializeField]
	public UILabel m_ArenaRank;

	[SerializeField]
	public UISprite m_AvatarBorder;

	private bool m_destroyed;

	private bool m_pictureAlreadySet;

	public void SetModel(FriendGameData friend, bool PictureAlreadySet = false)
	{
		m_Model = friend;
		RegisterEventHandlers();
		m_pictureAlreadySet = PictureAlreadySet;
		InvokeRepeating("CheckIfLoaded", 0.1f, 0.1f);
		CheckIfLoaded();
		RefreshInfos();
	}

	public void SetNew(bool isNew)
	{
		if ((bool)m_UpdateIndicator)
		{
			m_UpdateIndicator.SetActive(isNew);
		}
	}

	public void SetDefault()
	{
		if ((bool)m_FriendAvatar && !m_pictureAlreadySet)
		{
			m_FriendAvatar.material = new Material(m_FriendAvatar.material);
		}
		if ((bool)m_FriendNameLabel)
		{
			m_FriendNameLabel.text = DIContainerInfrastructure.GetLocaService().Tr("friends_loading", "Loading...");
		}
		if ((bool)m_LevelLabel)
		{
			m_LevelLabel.text = string.Empty;
		}
		if ((bool)m_ArenaIcon)
		{
			m_ArenaIcon.gameObject.SetActive(false);
		}
		if ((bool)m_ArenaRank)
		{
			m_ArenaRank.text = "-";
		}
		if ((bool)m_LastPlayLabel)
		{
			m_LastPlayLabel.text = string.Empty;
		}
	}

	private void RefreshInfos()
	{
		if (m_AvatarBorder != null && m_Model.PublicPlayerData != null && m_Model.PublicPlayerData.Trophy != null && m_Model.PublicPlayerData.Trophy.FinishedLeagueId > 0)
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
		if (!m_pictureAlreadySet)
		{
			if (m_Model.isNpcFriend && (bool)m_NPCAvatar)
			{
				if ((bool)m_FriendAvatar)
				{
					m_FriendAvatar.gameObject.SetActive(false);
				}
				m_NPCAvatar.gameObject.SetActive(true);
				m_NPCAvatar.spriteName = GetNPCSprite(m_Model.FriendId);
			}
			else if ((bool)m_FriendAvatar)
			{
				if (m_Model.FriendTexture != null && m_Model.FriendTexture.height != 8 && m_Model.FriendTexture.width != 8)
				{
					if ((bool)m_FriendAvatar)
					{
						m_FriendAvatar.gameObject.SetActive(true);
						m_FriendAvatar.mainTexture = m_Model.FriendTexture;
					}
					if ((bool)m_NPCAvatar)
					{
						m_NPCAvatar.gameObject.SetActive(false);
					}
				}
				else
				{
					if ((bool)m_FriendAvatar)
					{
						m_FriendAvatar.gameObject.SetActive(false);
					}
					if ((bool)m_NPCAvatar)
					{
						m_NPCAvatar.gameObject.SetActive(true);
						m_NPCAvatar.spriteName = GetNPCSprite(m_Model.FriendId);
					}
				}
			}
		}
		if (m_Model.IsFriendDataLoaded)
		{
			if ((bool)m_FriendNameLabel)
			{
				m_FriendNameLabel.text = DIContainerInfrastructure.GetLocaService().ReplaceUnmappableCharacters(m_Model.FriendName);
			}
			if ((bool)m_LevelLabel)
			{
				m_LevelLabel.text = m_Model.FriendLevel.ToString("0");
			}
			if ((bool)m_ArenaIcon)
			{
				string arenaIconName = GetArenaIconName();
				if (string.IsNullOrEmpty(arenaIconName))
				{
					m_ArenaIcon.gameObject.SetActive(false);
				}
				else
				{
					m_ArenaIcon.gameObject.SetActive(true);
					m_ArenaIcon.spriteName = arenaIconName;
				}
			}
			if ((bool)m_ArenaRank)
			{
				m_ArenaRank.text = "#" + m_Model.FriendPvpRank.ToString("0");
			}
			if ((bool)m_LastPlayLabel)
			{
				m_LastPlayLabel.text = string.Empty;
			}
		}
		else
		{
			if ((bool)m_FriendNameLabel)
			{
				m_FriendNameLabel.text = DIContainerInfrastructure.GetLocaService().Tr("friends_loading", "Loading...");
			}
			if ((bool)m_LevelLabel)
			{
				m_LevelLabel.text = string.Empty;
			}
			if ((bool)m_ArenaIcon)
			{
				m_ArenaIcon.gameObject.SetActive(false);
			}
			if ((bool)m_ArenaRank)
			{
				m_ArenaRank.text = "-";
			}
			if ((bool)m_LastPlayLabel)
			{
				m_LastPlayLabel.text = string.Empty;
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
		if (m_Model.FriendTextureIsLoaded)
		{
			CancelInvoke("CheckIfLoaded");
		}
		else
		{
			if (m_pictureAlreadySet)
			{
				return;
			}
			if (m_Model.IsFriendDataLoaded && !m_Model.FriendTextureIsLoaded && !m_Model.FriendTextureIsLoading)
			{
				if ((bool)m_FriendAvatar)
				{
					m_FriendAvatar.mainTexture = m_Model.FriendTexture;
				}
				if (m_Model.isNpcFriend && (bool)m_NPCAvatar)
				{
					if ((bool)m_FriendAvatar)
					{
						m_FriendAvatar.gameObject.SetActive(false);
					}
					m_NPCAvatar.gameObject.SetActive(true);
					m_NPCAvatar.spriteName = string.Empty;
					return;
				}
				if ((bool)m_FriendAvatar)
				{
					m_FriendAvatar.gameObject.SetActive(true);
				}
				if ((bool)m_NPCAvatar)
				{
					m_NPCAvatar.gameObject.SetActive(false);
				}
			}
			else
			{
				if ((bool)m_FriendAvatar)
				{
					m_FriendAvatar.gameObject.SetActive(false);
				}
				if ((bool)m_NPCAvatar)
				{
					m_NPCAvatar.gameObject.SetActive(true);
				}
			}
		}
	}

	private void OnDestroy()
	{
		m_destroyed = true;
		DeRegisterEventHandlers();
		if (m_Model != null)
		{
			if (m_FriendAvatar != null && m_FriendAvatar.mainTexture != null)
			{
				Object.DestroyImmediate(m_FriendAvatar.mainTexture, true);
			}
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

	internal void SetNPCIcon(bool set)
	{
		if ((bool)m_FriendAvatar)
		{
			m_FriendAvatar.gameObject.SetActive(false);
		}
		if ((bool)m_NPCAvatar)
		{
			m_NPCAvatar.gameObject.SetActive(set);
			m_NPCAvatar.spriteName = GetNPCSprite("Avatar_Generic");
		}
	}

	private string GetArenaIconName()
	{
		switch (m_Model.FriendPvpLeague)
		{
		case 6:
			return "LeagueCrown_Stone";
		case 5:
			return "LeagueCrown_Bronze";
		case 4:
			return "LeagueCrown_Silver";
		case 3:
			return "LeagueCrown_Gold";
		case 2:
			return "LeagueCrown_Platinum";
		case 1:
			return "LeagueCrown_Diamond";
		default:
			return string.Empty;
		}
	}
}

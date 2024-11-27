using System;
using System.Runtime.CompilerServices;
using ABH.Shared.Models;
using UnityEngine;

namespace ABH.GameDatas
{
	public class FriendGameData
	{
		private FriendData m_FriendData;

		private string m_FriendId;

		private WWW m_FriendTextureDownload;

		private Texture2D m_FriendTexture;

		private uint m_LastBirdRequest;

		private BirdGameData m_FriendBird;

		public bool isNpcFriend;

		public bool canSpawnNewGachaRoll = true;

		public bool HasPaid;

		private bool m_gachaFreeRoll;

		private bool m_gachaFreePvpRoll;

		private PublicPlayerData m_publicPlayerData;

		public PublicPlayerData PublicPlayerData
		{
			get
			{
				PublicPlayerData value = null;
				if (DIContainerInfrastructure.GetCurrentPlayer() != null)
				{
					DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.PublicPlayerDatas.TryGetValue(FriendId, out value);
				}
				return value;
			}
		}

		public FriendData Data
		{
			get
			{
				return FriendData;
			}
		}

		public bool IsFriendDataLoaded
		{
			get
			{
				return FriendData != null;
			}
		}

		public bool IsFriendBirdLoaded
		{
			get
			{
				return m_FriendBird != null;
			}
		}

		public int FriendBirdLevel
		{
			get
			{
				return IsFriendBirdLoaded ? m_FriendBird.Level : 0;
			}
		}

		public string FriendName
		{
			get
			{
				if (FriendData != null)
				{
					if (isNpcFriend)
					{
						return GetNPCFriendName(FriendId);
					}
					return FriendData.FirstName;
				}
				return FriendId;
			}
		}

		public string FriendId
		{
			get
			{
				return m_FriendId;
			}
		}

		public BirdGameData FriendBird
		{
			get
			{
				return m_FriendBird;
			}
		}

		public Texture2D FriendTexture
		{
			get
			{
				if (m_FriendTextureDownload == null && FriendData != null)
				{
					m_FriendTextureDownload = new WWW(FriendData.PictureUrl);
				}
				if (FriendTextureIsLoaded && string.IsNullOrEmpty(m_FriendTextureDownload.error))
				{
					m_FriendTexture = m_FriendTextureDownload.texture;
					if (PublicPlayerData != null)
					{
						DIContainerInfrastructure.GetCoreStateMgr().SaveSocialAvatar(m_FriendTextureDownload, (DIContainerInfrastructure.GetCoreStateMgr().GetSharedId(PublicPlayerData) == null) ? PublicPlayerData.SocialId : DIContainerInfrastructure.GetCoreStateMgr().GetSharedId(PublicPlayerData));
					}
				}
				else if (PublicPlayerData != null)
				{
					m_FriendTexture = DIContainerInfrastructure.GetCoreStateMgr().LoadSocialAvatar((DIContainerInfrastructure.GetCoreStateMgr().GetSharedId(PublicPlayerData) == null) ? PublicPlayerData.SocialId : DIContainerInfrastructure.GetCoreStateMgr().GetSharedId(PublicPlayerData));
				}
				return m_FriendTexture;
				return null;
			}
		}

		public DateTime LastBirdRequest
		{
			get
			{
				return DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(m_LastBirdRequest);
			}
		}

		public bool FriendTextureIsLoading
		{
			get
			{
				return FriendData != null && m_FriendTextureDownload != null && !m_FriendTextureDownload.isDone;
			}
		}

		public bool FriendTextureIsLoaded
		{
			get
			{
				return FriendData != null && m_FriendTextureDownload != null && m_FriendTextureDownload.isDone;
			}
		}

		public int FriendLevel
		{
			get
			{
				return (FriendData == null) ? 1 : FriendData.Level;
			}
		}

		public int FriendPvpRank
		{
			get
			{
				return (FriendData == null) ? 15 : FriendData.PvpRank;
			}
		}

		public int FriendPvpLeague
		{
			get
			{
				return (FriendData == null) ? 6 : FriendData.PvpLeague;
			}
		}

		public FriendData FriendData
		{
			get
			{
				if (PublicPlayerData != null)
				{
					PublicPlayerData publicPlayerData = PublicPlayerData;
					if (m_FriendData == null)
					{
						m_FriendData = new FriendData
						{
							Id = publicPlayerData.SocialId
						};
					}
					m_FriendData.FirstName = publicPlayerData.SocialPlayerName;
					m_FriendData.PictureUrl = publicPlayerData.SocialAvatarUrl;
					if (!m_FriendData.IsNPC || !(publicPlayerData.SocialId == "NPC_Low"))
					{
						m_FriendData.Level = publicPlayerData.Level;
					}
				}
				return m_FriendData;
			}
		}

		[method: MethodImpl(32)]
		public event Action OnTextureUnloaded;

		public FriendGameData(string friendId)
		{
			m_FriendId = friendId;
			switch (friendId)
			{
			case "NPC_High":
			case "NPC_Porky":
			case "NPC_Adventurer":
			case "NPC_Low":
				isNpcFriend = true;
				break;
			}
			switch (friendId)
			{
			case "NPC_High":
			case "NPC_Porky":
			case "NPC_Adventurer":
				canSpawnNewGachaRoll = false;
				break;
			}
		}

		public FriendGameData SetLastBirdRequest(uint lastBirdRequest)
		{
			m_LastBirdRequest = lastBirdRequest;
			return this;
		}

		public FriendGameData SetFreeGachaRoll(bool freeRoll)
		{
			m_gachaFreeRoll = freeRoll;
			return this;
		}

		public FriendGameData SetFreePvpGachaRoll(bool freeRoll)
		{
			m_gachaFreePvpRoll = freeRoll;
			return this;
		}

		public FriendGameData SetFriendData(FriendData friendData)
		{
			if (friendData.IsNPC)
			{
				isNpcFriend = true;
			}
			m_FriendData = friendData;
			return this;
		}

		public void UseFriendBird()
		{
			m_LastBirdRequest = 0u;
		}

		public FriendGameData SetFriendBird(BirdGameData bird)
		{
			DateTime trustedTime;
			if (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				return this;
			}
			if (m_FriendBird == null || (isNpcFriend && FriendData != null && !FriendData.NeedsPayment) || trustedTime > LastBirdRequest.AddSeconds(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.BalancingData.CacheFriendBirdTime))
			{
				m_FriendBird = bird;
				DebugLog.Log("Friend Bird Level: " + m_FriendBird.Level);
				m_LastBirdRequest = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime);
			}
			else
			{
				DebugLog.Log("Already got Friend Bird: " + m_FriendBird.Level);
			}
			return this;
		}

		private string GetNPCFriendName(string friendId)
		{
			switch (friendId)
			{
			case "NPC_High":
				return DIContainerInfrastructure.GetLocaService().Tr("npc_friend_eagle");
			case "NPC_Porky":
				return DIContainerInfrastructure.GetLocaService().Tr("npc_friend_prince");
			case "NPC_Adventurer":
				return DIContainerInfrastructure.GetLocaService().Tr("npc_friend_adventurer");
			case "NPC_Low":
				return DIContainerInfrastructure.GetLocaService().Tr("npc_friend_merchant");
			default:
				return DIContainerInfrastructure.GetLocaService().Tr("npc_" + friendId);
			}
		}

		public void UnloadFriendTexture()
		{
			m_FriendTexture = null;
			if (this.OnTextureUnloaded != null)
			{
				this.OnTextureUnloaded();
			}
		}
	}
}

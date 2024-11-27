using System;
using System.Runtime.CompilerServices;
using ABH.Shared.Models;
using UnityEngine;

namespace ABH.GameDatas
{
	public class OpponentGameData
	{
		private WWW m_OpponentTextureDownload;

		private Texture2D m_OpponentTexture;

		private PublicPlayerData m_publicPlayerData;

		public bool IsSelf { get; private set; }

		public PublicPlayerData PublicPlayerData
		{
			get
			{
				return m_publicPlayerData;
			}
		}

		public int OpponentLevel
		{
			get
			{
				return m_publicPlayerData.Level;
			}
		}

		public string OpponentName
		{
			get
			{
				string empty = string.Empty;
				if (PublicPlayerData != null && !string.IsNullOrEmpty(PublicPlayerData.EventPlayerName))
				{
					return PublicPlayerData.EventPlayerName;
				}
				if (IsSelf)
				{
					return DIContainerInfrastructure.GetLocaService().Tr("gen_opponent_self", "You");
				}
				if (PublicPlayerData != null && !string.IsNullOrEmpty(PublicPlayerData.SocialPlayerName))
				{
					string[] array = PublicPlayerData.SocialPlayerName.Split(' ');
					return array[0];
				}
				return DIContainerInfrastructure.GetLocaService().Tr("gen_opponent_unkown", "Unnamed Player");
			}
		}

		public Texture2D OpponentTexture
		{
			get
			{
				// if (m_OpponentTextureDownload == null)
				// {
				// 	m_OpponentTextureDownload = new WWW(PublicPlayerData.SocialAvatarUrl);
				// }
				// if (OpponentTextureIsLoaded && string.IsNullOrEmpty(m_OpponentTextureDownload.error))
				// {
				// 	m_OpponentTexture = m_OpponentTextureDownload.texture;
				// 	DIContainerInfrastructure.GetCoreStateMgr().SaveSocialAvatar(m_OpponentTextureDownload, (DIContainerInfrastructure.GetCoreStateMgr().GetSharedId(PublicPlayerData) == null) ? PublicPlayerData.SocialId : DIContainerInfrastructure.GetCoreStateMgr().GetSharedId(PublicPlayerData));
				// }
				// else
				// {
				// 	m_OpponentTexture = DIContainerInfrastructure.GetCoreStateMgr().LoadSocialAvatar((DIContainerInfrastructure.GetCoreStateMgr().GetSharedId(PublicPlayerData) == null) ? PublicPlayerData.SocialId : DIContainerInfrastructure.GetCoreStateMgr().GetSharedId(PublicPlayerData));
				// }
				// return m_OpponentTexture;
				return null;
			}
		}

		public bool OpponentTextureIsLoading
		{
			get
			{
				return m_OpponentTextureDownload != null && !m_OpponentTextureDownload.isDone;
			}
		}

		public bool OpponentTextureIsLoaded
		{
			get
			{
				return m_OpponentTextureDownload != null && m_OpponentTextureDownload.isDone;
			}
		}

		[method: MethodImpl(32)]
		public event Action OnTextureUnloaded;

		public OpponentGameData(PublicPlayerData opponentPlayerData, bool self = false)
		{
			m_publicPlayerData = opponentPlayerData;
			IsSelf = self;
		}

		public void UnloadFriendTexture()
		{
			m_OpponentTexture = null;
			if (this.OnTextureUnloaded != null)
			{
				this.OnTextureUnloaded();
			}
		}
	}
}

using System;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.MailboxMessages
{
	public abstract class BaseMailboxMessage : IMailboxMessageGameData
	{
		protected MessageDataIncoming m_Message;

		private FriendGameData m_sender;

		public FriendGameData Sender
		{
			get
			{
				if (m_sender != null)
				{
					return m_sender;
				}
				if (!DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends.TryGetValue(m_Message.Sender.Id, out m_sender))
				{
					m_sender = new FriendGameData(m_Message.Sender.Id);
				}
				m_sender.SetFriendData(m_Message.Sender);
				return m_sender;
			}
		}

		public abstract string ContentDescription { get; }

		public abstract string IconAssetId { get; }

		public abstract string IconAtlasName { get; }

		public abstract Dictionary<string, LootInfoData> Loot { get; }

		public abstract MessageInfo ResponseMessage { get; }

		public abstract bool HasReward { get; }

		public abstract bool HasResponseMessage { get; }

		public long SendTime
		{
			get
			{
				return m_Message.ReceivedAt;
			}
		}

		public virtual bool IsViewed
		{
			get
			{
				return m_Message.ViewedAt != 0 && DIContainerLogic.GetTimingService().IsAfter(DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(m_Message.ViewedAt));
			}
		}

		public virtual bool IsUsed
		{
			get
			{
				return m_Message.UsedAt != 0 && DIContainerLogic.GetTimingService().IsAfter(DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(m_Message.UsedAt));
			}
		}

		public string Id
		{
			get
			{
				return m_Message.Id;
			}
		}

		public virtual string URL
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual bool HasURL
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsNotSimpleCustomMessage
		{
			get
			{
				return false;
			}
		}

		public virtual void OnAddMessage()
		{
		}

		public virtual bool IsEqual(MessageDataIncoming message)
		{
			return message.MessageType == m_Message.MessageType && message.Parameter1 == m_Message.Parameter1 && message.Parameter2 == m_Message.Parameter2 && message.Sender.Id == m_Message.Sender.Id;
		}

		public virtual IMailboxMessageGameData SetMessageData(MessageDataIncoming message)
		{
			m_Message = message;
			return this;
		}

		public virtual bool ViewMessageContent(PlayerGameData player, Action<bool> callbackWhenDone)
		{
			callbackWhenDone(true);
			return true;
		}

		public virtual bool UseMessageContent(PlayerGameData player, Action<bool, IMailboxMessageGameData> callbackWhenDone)
		{
			callbackWhenDone(true, this);
			return true;
		}

		public virtual bool IsAddMessageAllowed(SocialEnvironmentGameData socialEnvironmentGameData)
		{
			return true;
		}

		public MessageDataIncoming GetMessageData()
		{
			return m_Message;
		}

		public virtual bool CheckAddRequirements(PlayerGameData playerGameData)
		{
			return true;
		}
	}
}

using System;
using System.Collections.Generic;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using Rcs;

namespace ABH.GameDatas.MailboxMessages
{
	public class RequestInvitationMessage : BaseMailboxMessage
	{
		private Dictionary<string, LootInfoData> m_Loot;

		private Action<bool> m_callbackWhenDone;

		public override string ContentDescription
		{
			get
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("{sender_name}", Sender.FriendName);
				return DIContainerInfrastructure.GetLocaService().Tr("social_ask_invitation", dictionary);
			}
		}

		public override string IconAssetId
		{
			get
			{
				return "ButtonLabel_New";
			}
		}

		public override string IconAtlasName
		{
			get
			{
				return "GenericElements";
			}
		}

		public override Dictionary<string, LootInfoData> Loot
		{
			get
			{
				return m_Loot;
			}
		}

		public override MessageInfo ResponseMessage
		{
			get
			{
				MessageInfo messageInfo = new MessageInfo();
				messageInfo.MessageType = MessageType.ResponseInvitationMessage;
				return messageInfo;
			}
		}

		public override bool HasReward
		{
			get
			{
				return false;
			}
		}

		public override bool HasResponseMessage
		{
			get
			{
				return true;
			}
		}

		public override bool IsUsed
		{
			get
			{
				return IsViewed;
			}
		}

		public override bool ViewMessageContent(PlayerGameData player, Action<bool> callbackWhenDone)
		{
			m_callbackWhenDone = callbackWhenDone;
			if (HasReward)
			{
				DIContainerLogic.GetLootOperationService().RewardLoot(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, Loot, "message_claim_RequestInvitationMessage");
			}
			if (HasResponseMessage)
			{
				MessageDataIncoming messageDataIncoming = new MessageDataIncoming();
				messageDataIncoming.MessageType = ResponseMessage.MessageType;
				messageDataIncoming.Parameter1 = ResponseMessage.Parameter1;
				messageDataIncoming.Parameter2 = ResponseMessage.Paramter2;
				messageDataIncoming.SentAt = DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp();
				messageDataIncoming.Sender = DIContainerInfrastructure.GetCurrentPlayer().GetFriendData();
				MessageDataIncoming message = messageDataIncoming;
				ABHAnalyticsHelper.SendSocialEvent(message, m_Message.Sender);
				DIContainerInfrastructure.MessagingService.RespondMessage(message, m_Message.Sender.Id, RequestFinished, RequestFinishedWithError);
			}
			else
			{
				callbackWhenDone(true);
			}
			return true;
		}

		private void RequestFinishedWithError(Mailbox.ErrorCode theError)
		{
			if (m_callbackWhenDone != null)
			{
				m_callbackWhenDone(false);
			}
			if (theError != Mailbox.ErrorCode.ErrorInvalidParameters && DIContainerLogic.SocialService.AddResendMessage(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData, this))
			{
				DIContainerInfrastructure.GetAsynchStatusService().ShowError(DIContainerInfrastructure.GetLocaService().Tr("toast_message_responseerror", "The Message Response failed it will be respawned soon in your Mailbox"), "response_error");
			}
		}

		private void RequestFinished()
		{
			if (m_callbackWhenDone != null)
			{
				m_callbackWhenDone(true);
			}
		}
	}
}

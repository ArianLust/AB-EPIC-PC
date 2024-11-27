using System;
using System.Collections.Generic;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using Rcs;

namespace ABH.GameDatas.MailboxMessages
{
	public class RequestFriendshipGateMessage : BaseMailboxMessage
	{
		private Dictionary<string, LootInfoData> m_Loot;

		private Action<bool> m_callbackWhenDone;

		public override string ContentDescription
		{
			get
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("{sender_name}", Sender.FriendName);
				HotspotBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<HotspotBalancingData>(m_Message.Parameter1);
				if (balancingData != null)
				{
					if (balancingData.ZoneStageIndex == 0)
					{
						dictionary.Add("{hotspot_name}", DIContainerInfrastructure.GetLocaService().GetZoneName(balancingData.ZoneLocaIdent));
					}
					else
					{
						dictionary.Add("{hotspot_name}", DIContainerInfrastructure.GetLocaService().GetZoneName(balancingData.ZoneLocaIdent) + " - " + balancingData.ZoneStageIndex);
					}
				}
				return DIContainerInfrastructure.GetLocaService().Tr("social_ask_friendship_gate", dictionary);
			}
		}

		public override string IconAssetId
		{
			get
			{
				return "Camp";
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
				messageInfo.MessageType = MessageType.ResponseFriendshipGateMessage;
				messageInfo.Parameter1 = m_Message.Parameter1;
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
				DIContainerLogic.GetLootOperationService().RewardLoot(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, Loot, "message_claim_RequestFriendshipGateMessage");
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
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.RemoveMessage(this);
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

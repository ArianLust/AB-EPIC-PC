using System;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.MailboxMessages
{
	public class ResponsePvpGachaUseMessage : BaseMailboxMessage
	{
		private Dictionary<string, LootInfoData> m_Loot;

		public override string ContentDescription
		{
			get
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("{sender_name}", Sender.FriendName);
				if (Sender.isNpcFriend)
				{
					return DIContainerInfrastructure.GetLocaService().Tr("social_notify_npc_pvpgacha");
				}
				return DIContainerInfrastructure.GetLocaService().Tr("social_notify_pvpgacha_use", dictionary);
			}
		}

		public override string IconAssetId
		{
			get
			{
				return "Resource_LuckyCoin";
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
				return null;
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
				return false;
			}
		}

		public override bool IsEqual(MessageDataIncoming message)
		{
			return false;
		}

		public override void OnAddMessage()
		{
			base.OnAddMessage();
			DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_friendusedpvpgacha", "A Friend used your Gacha machine and Filled your Gacha bar a bit."), "toast_friendusedgacha", DispatchMessage.Status.Info);
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.SetNewPiggieMcCoolDate(MessageType.ResponsePvpGachaUseMessage);
		}

		public override bool UseMessageContent(PlayerGameData player, Action<bool, IMailboxMessageGameData> callbackWhenDone)
		{
			if (IsUsed)
			{
				return false;
			}
			m_Message.UsedAt = DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp();
			DIContainerLogic.SocialService.RemoveMessage(player, this);
			DIContainerLogic.InventoryService.AddItem(player.InventoryGameData, 0, 0, "pvpgacha_standard_uses", 1, "message_friend_pvpgacha_use");
			return base.UseMessageContent(player, callbackWhenDone);
		}

		public override bool ViewMessageContent(PlayerGameData player, Action<bool> callbackWhenDone)
		{
			if (HasReward)
			{
				DIContainerLogic.GetLootOperationService().RewardLoot(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, Loot, "message_claim_ResponsePvpGachaUseMessage");
			}
			if (HasResponseMessage)
			{
				DIContainerLogic.SocialService.RespondMessage(DIContainerInfrastructure.GetCurrentPlayer(), DIContainerInfrastructure.GetCurrentPlayer().GetFriendData(), m_Message.Sender.Id, ResponseMessage.MessageType, ResponseMessage.Parameter1, ResponseMessage.Paramter2, callbackWhenDone);
			}
			else
			{
				callbackWhenDone(true);
			}
			m_Message.ViewedAt = DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp();
			DIContainerLogic.SocialService.RemoveMessage(player, this);
			return true;
		}
	}
}

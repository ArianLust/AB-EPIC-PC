using System;
using System.Collections.Generic;
using System.Linq;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.MailboxMessages
{
	public class ResponseFriendshipEssenceMessage : BaseMailboxMessage
	{
		private Dictionary<string, LootInfoData> m_Loot;

		public override string ContentDescription
		{
			get
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("{sender_name}", Sender.FriendName);
				IInventoryItemGameData inventoryItemGameData = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(Loot).FirstOrDefault();
				if (m_Message != null)
				{
					dictionary.Add("{count}", m_Message.Parameter2.ToString("0"));
				}
				dictionary.Add("{max_count}", DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.BalancingData.FriendShipEssenceMessageCap.ToString("0"));
				if (inventoryItemGameData != null)
				{
					dictionary.Add("{reward}", inventoryItemGameData.ItemValue.ToString("0") + "x " + inventoryItemGameData.ItemLocalizedName);
				}
				if (Sender.isNpcFriend)
				{
					return DIContainerInfrastructure.GetLocaService().Tr("social_notify_npc_friendship_essence", dictionary);
				}
				return DIContainerInfrastructure.GetLocaService().Tr("social_notify_friendship_essence", dictionary);
			}
		}

		public override string IconAssetId
		{
			get
			{
				return "Resource_FriendshipEssence";
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
				if (m_Loot != null)
				{
					return m_Loot;
				}
				m_Loot = DIContainerLogic.GetLootOperationService().GenerateLoot(new Dictionary<string, int> { { "loot_social_request_friendship_essence", 1 } }, 1);
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
				return true;
			}
		}

		public override bool HasResponseMessage
		{
			get
			{
				return false;
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
			if (HasReward)
			{
				DIContainerLogic.GetLootOperationService().RewardLoot(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, Loot, "message_claim_ResponseFriendshipEssenceMessage");
			}
			if (HasResponseMessage)
			{
				DIContainerLogic.SocialService.RespondMessage(DIContainerInfrastructure.GetCurrentPlayer(), DIContainerInfrastructure.GetCurrentPlayer().GetFriendData(), m_Message.Sender.Id, ResponseMessage.MessageType, ResponseMessage.Parameter1, ResponseMessage.Paramter2, callbackWhenDone);
			}
			else
			{
				callbackWhenDone(true);
			}
			player.SocialEnvironmentGameData.RemoveMessage(this);
			return true;
		}

		public override bool IsAddMessageAllowed(SocialEnvironmentGameData socialEnvironmentGameData)
		{
			if (socialEnvironmentGameData.Data.FriendShipEssenceMessageCapCount >= socialEnvironmentGameData.BalancingData.FriendShipEssenceMessageCap)
			{
				DebugLog.Log("Message Limit for Friendship messages reached!");
				return false;
			}
			return true;
		}

		public override void OnAddMessage()
		{
			base.OnAddMessage();
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipEssenceMessageCapCount = DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipEssenceMessageCapCount + 1;
			m_Message.Parameter2 = DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipEssenceMessageCapCount;
			DebugLog.Log("Friendship Essence Messages left to get: " + (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.BalancingData.FriendShipEssenceMessageCap - DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipEssenceMessageCapCount));
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.SetNewPiggieMcCoolDate(MessageType.ResponseFriendshipEssenceMessage);
		}
	}
}

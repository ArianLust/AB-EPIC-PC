using System;
using System.Collections.Generic;
using System.Linq;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.MailboxMessages
{
	public class ResponseBirdBorrowMessage : BaseMailboxMessage
	{
		private Dictionary<string, LootInfoData> m_Loot;

		public override string ContentDescription
		{
			get
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("{sender_name}", Sender.FriendName);
				if (!string.IsNullOrEmpty(m_Message.Parameter1))
				{
					BirdGameData bird = DIContainerInfrastructure.GetCurrentPlayer().GetBird(m_Message.Parameter1, true);
					if (bird != null)
					{
						dictionary.Add("{bird_name}", DIContainerInfrastructure.GetLocaService().GetCharacterName(bird.BalancingData.LocaId));
					}
				}
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
					return DIContainerInfrastructure.GetLocaService().Tr("social_notify_npc_borrow", dictionary);
				}
				return DIContainerInfrastructure.GetLocaService().Tr("social_notify_borrow_bird", dictionary);
			}
		}

		public override string IconAssetId
		{
			get
			{
				BirdGameData bird = DIContainerInfrastructure.GetCurrentPlayer().GetBird(m_Message.Parameter1);
				if (bird != null)
				{
					return bird.AssetName;
				}
				return string.Empty;
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

		public override bool IsEqual(MessageDataIncoming message)
		{
			return false;
		}

		public override bool ViewMessageContent(PlayerGameData player, Action<bool> callbackWhenDone)
		{
			if (HasReward)
			{
				DIContainerLogic.GetLootOperationService().RewardLoot(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, Loot, "message_claim_ResponseBirdBorrowMessage");
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
			if (socialEnvironmentGameData.Data.FriendShipEssenceMessageByBirdCapCount >= socialEnvironmentGameData.BalancingData.FriendShipEssenceMessageCap)
			{
				DebugLog.Log("Message Limit for Friendship messages reached!");
				return false;
			}
			return true;
		}

		public override void OnAddMessage()
		{
			base.OnAddMessage();
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipEssenceMessageByBirdCapCount++;
			m_Message.Parameter2 = DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipEssenceMessageByBirdCapCount;
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.SetNewPiggieMcCoolDate(MessageType.ResponseBirdBorrowMessage);
		}
	}
}

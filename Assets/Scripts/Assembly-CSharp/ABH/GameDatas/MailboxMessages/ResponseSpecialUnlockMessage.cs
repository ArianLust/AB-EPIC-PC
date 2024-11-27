using System;
using System.Collections.Generic;
using System.Linq;
using ABH.Shared.BalancingData;
using ABH.Shared.Interfaces;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.MailboxMessages
{
	public class ResponseSpecialUnlockMessage : BaseMailboxMessage
	{
		private Dictionary<string, LootInfoData> m_Loot;

		public override string ContentDescription
		{
			get
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("{sender_name}", Sender.FriendName);
				IInventoryItemBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<BasicItemBalancingData>(m_Message.Parameter1);
				if (balancingData != null)
				{
					dictionary.Add("{unlock_desc}", DIContainerInfrastructure.GetLocaService().GetItemDesc(balancingData.LocaBaseId));
				}
				IInventoryItemGameData inventoryItemGameData = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(Loot).FirstOrDefault();
				if (inventoryItemGameData != null)
				{
					dictionary.Add("{reward}", inventoryItemGameData.ItemValue.ToString("0") + "x " + inventoryItemGameData.ItemLocalizedName);
				}
				return DIContainerInfrastructure.GetLocaService().Tr("social_notify_special_unlock", dictionary);
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
				DIContainerLogic.GetLootOperationService().RewardLoot(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, Loot, "message_claim_ResponseSpecialUnlockMessage");
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
	}
}

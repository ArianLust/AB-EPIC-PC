using System;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.MailboxMessages
{
	internal class CustomMessageGameData : BaseMailboxMessage
	{
		private CustomMessageBalancingData m_Balancing;

		private bool isSimpleMessage
		{
			get
			{
				return m_Balancing == null;
			}
		}

		public override string ContentDescription
		{
			get
			{
				if (isSimpleMessage)
				{
					return DIContainerInfrastructure.GetLocaService().Tr(m_Message.Parameter1);
				}
				return DIContainerInfrastructure.GetLocaService().Tr(m_Balancing.LocaId);
			}
		}

		public override string IconAssetId
		{
			get
			{
				if (isSimpleMessage)
				{
					return "Resource_FriendshipEssence";
				}
				return m_Balancing.ButtonSpriteNameId;
			}
		}

		public override string IconAtlasName
		{
			get
			{
				if (isSimpleMessage)
				{
					return "GenericElements";
				}
				return m_Balancing.ButtonAtlasId;
			}
		}

		public override Dictionary<string, LootInfoData> Loot
		{
			get
			{
				if (isSimpleMessage || m_Balancing.LootTableReward == null)
				{
					return null;
				}
				return DIContainerLogic.GetLootOperationService().GenerateLoot(m_Balancing.LootTableReward, 1);
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
				return !isSimpleMessage && m_Balancing.LootTableReward != null;
			}
		}

		public override bool HasResponseMessage
		{
			get
			{
				return false;
			}
		}

		public override bool HasURL
		{
			get
			{
				return !isSimpleMessage && !string.IsNullOrEmpty(m_Balancing.URLToOpen);
			}
		}

		public override string URL
		{
			get
			{
				return (!HasURL) ? base.URL : m_Balancing.URLToOpen;
			}
		}

		public override bool IsNotSimpleCustomMessage
		{
			get
			{
				return !isSimpleMessage;
			}
		}

		public override IMailboxMessageGameData SetMessageData(MessageDataIncoming message)
		{
			m_Message = message;
			DIContainerBalancing.Service.TryGetBalancingData<CustomMessageBalancingData>(m_Message.Parameter1, out m_Balancing);
			return this;
		}

		public override bool ViewMessageContent(PlayerGameData player, Action<bool> callbackWhenDone)
		{
			if (HasReward)
			{
				DIContainerLogic.GetLootOperationService().RewardLoot(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, Loot, "message_claim_ResponseFriendshipEssenceMessage");
			}
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.RemoveMessage(this);
			DIContainerInfrastructure.GetCustomMessageService().AcknowledgedCustomMessage(this);
			return base.ViewMessageContent(player, callbackWhenDone);
		}

		public override bool IsEqual(MessageDataIncoming message)
		{
			return m_Message.Id == message.Id;
		}

		public override bool Equals(object obj)
		{
			CustomMessageGameData customMessageGameData = obj as CustomMessageGameData;
			return customMessageGameData != null && customMessageGameData.m_Message.Id == m_Message.Id;
		}

		public override int GetHashCode()
		{
			return m_Message.Id.GetHashCode();
		}

		public override string ToString()
		{
			return "CustomMessage[" + Id + " -> " + ContentDescription + "]";
		}

		public override bool CheckAddRequirements(PlayerGameData playerGameData)
		{
			if (isSimpleMessage)
			{
				return true;
			}
			return DIContainerLogic.RequirementService.CheckGenericRequirements(playerGameData, m_Balancing.AddMessageRequirements);
		}
	}
}

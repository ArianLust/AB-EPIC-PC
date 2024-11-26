using System;
using System.Collections.Generic;
using ABH.Shared.BalancingData;
using ABH.Shared.Interfaces;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.MailboxMessages
{
	public class WonInPvpChallengeMessage : BaseMailboxMessage
	{
		private Dictionary<string, LootInfoData> m_Loot;

		public override string ContentDescription
		{
			get
			{
				string text = string.Empty;
				IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(Loot.FirstOrDefault().Key);
				if (balancingData != null)
				{
					text = DIContainerInfrastructure.GetLocaService().GetItemName(balancingData.LocaBaseId);
				}
				if (m_Message.Parameter2 <= 1)
				{
					string value = string.Empty;
					if (Sender.PublicPlayerData != null)
					{
						value = Sender.PublicPlayerData.EventPlayerName;
						if (string.IsNullOrEmpty(value))
						{
							string[] array = Sender.PublicPlayerData.SocialPlayerName.Split(' ');
							value = array[0];
						}
					}
					if (string.IsNullOrEmpty(value))
					{
						value = DIContainerInfrastructure.GetLocaService().Tr("gen_opponent_unkown", "Unnamed Player");
					}
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("{value_1}", value);
					dictionary.Add("{reward}", Loot.FirstOrDefault().Value.Value + " " + text);
					return DIContainerInfrastructure.GetLocaService().Tr("social_notify_pvpdefend_01", dictionary);
				}
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
				dictionary2.Add("{value_1}", m_Message.Parameter2.ToString());
				dictionary2.Add("{reward}", Loot.FirstOrDefault().Value.Value + " " + text);
				return DIContainerInfrastructure.GetLocaService().Tr("social_notify_pvpdefend_02", dictionary2);
			}
		}

		public override string IconAssetId
		{
			get
			{
				return "PvPRankingPoints";
			}
		}

		public override string IconAtlasName
		{
			get
			{
				return "ArenaElements";
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
				m_Loot = DIContainerLogic.GetLootOperationService().GenerateLoot(new Dictionary<string, int> { { "loot_social_request_pvp_points", 1 } }, 1);
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

		public void CombineMessage(WonInPvpChallengeMessage message)
		{
			int parameter = message.m_Message.Parameter2;
			m_Message.Parameter2 += parameter;
			if (m_Message.Parameter2 <= DIContainerBalancing.Service.GetBalancingData<SocialEnvironmentBalancingData>("default").MaxRewardsForPvp)
			{
				Loot.FirstOrDefault().Value.Value = message.Loot.FirstOrDefault().Value.Value * m_Message.Parameter2;
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
				DIContainerLogic.GetLootOperationService().RewardLoot(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, Loot, "message_claim_WonInPvpChallengeMessage");
			}
			if (callbackWhenDone != null)
			{
				callbackWhenDone(true);
			}
			player.SocialEnvironmentGameData.RemoveMessage(this);
			return true;
		}
	}
}

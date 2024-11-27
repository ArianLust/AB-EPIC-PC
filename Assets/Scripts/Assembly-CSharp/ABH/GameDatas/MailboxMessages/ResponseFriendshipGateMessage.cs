using System;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.MailboxMessages
{
	public class ResponseFriendshipGateMessage : BaseMailboxMessage
	{
		private Dictionary<string, LootInfoData> m_Loot;

		public override string ContentDescription
		{
			get
			{
				DebugLog.Log("[ResponseFriendshipGateMessage] Start Get Content Desc!");
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if (Sender == null)
				{
					DebugLog.Error("[ResponseFriendshipGateMessage] Sender is null!");
				}
				else
				{
					dictionary.Add("{sender_name}", Sender.FriendName);
					DebugLog.Log("[ResponseFriendshipGateMessage] Sender Name: " + Sender.FriendName);
				}
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
				else
				{
					DebugLog.Log("[ResponseFriendshipGateMessage] Hotspot Name not found: " + m_Message.Parameter1);
				}
				return DIContainerInfrastructure.GetLocaService().Tr("social_notify_friendship_gate", dictionary);
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

		public override void OnAddMessage()
		{
			base.OnAddMessage();
			UseMessageContent(DIContainerInfrastructure.GetCurrentPlayer(), delegate
			{
			});
		}

		public override bool ViewMessageContent(PlayerGameData player, Action<bool> callbackWhenDone)
		{
			if (HasReward)
			{
				DIContainerLogic.GetLootOperationService().RewardLoot(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, Loot, "message_claim_ResponseFriendshipGateMessage");
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
			DIContainerLogic.SocialService.RemoveMessage(DIContainerInfrastructure.GetCurrentPlayer(), this);
			return true;
		}

		public override bool UseMessageContent(PlayerGameData player, Action<bool, IMailboxMessageGameData> callbackWhenDone)
		{
			if (IsUsed)
			{
				return false;
			}
			m_Message.UsedAt = DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp();
			DIContainerLogic.SocialService.RemoveMessage(DIContainerInfrastructure.GetCurrentPlayer(), this);
			if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks == null)
			{
				DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks = new Dictionary<string, List<string>>();
			}
			if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.NewFriendShipGateUnlocks == null)
			{
				DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.NewFriendShipGateUnlocks = new Dictionary<string, List<string>>();
			}
			Dictionary<string, List<string>> friendShipGateUnlocks = DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks;
			Dictionary<string, List<string>> newFriendShipGateUnlocks = DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.NewFriendShipGateUnlocks;
			if (friendShipGateUnlocks != null && !friendShipGateUnlocks.ContainsKey(m_Message.Parameter1))
			{
				friendShipGateUnlocks.Add(m_Message.Parameter1, new List<string>());
			}
			if (newFriendShipGateUnlocks != null && !newFriendShipGateUnlocks.ContainsKey(m_Message.Parameter1))
			{
				newFriendShipGateUnlocks.Add(m_Message.Parameter1, new List<string>());
			}
			if (m_Message.Sender.IsNPC || !friendShipGateUnlocks[m_Message.Parameter1].Contains(m_Message.Sender.Id))
			{
				if (friendShipGateUnlocks[m_Message.Parameter1] == null)
				{
					friendShipGateUnlocks[m_Message.Parameter1] = new List<string>();
				}
				if (!newFriendShipGateUnlocks.ContainsKey(m_Message.Parameter1))
				{
					if (newFriendShipGateUnlocks[m_Message.Parameter1] == null)
					{
						friendShipGateUnlocks[m_Message.Parameter1] = new List<string>();
					}
					friendShipGateUnlocks[m_Message.Parameter1].Add(m_Message.Sender.Id);
				}
				friendShipGateUnlocks[m_Message.Parameter1].Add(m_Message.Sender.Id);
				DebugLog.Log("Friendship Gate Unlock Friend Added!");
			}
			HotspotGameData value = null;
			if (DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.HotspotGameDatas.TryGetValue(m_Message.Parameter1, out value) && !DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.GetAndSetFriendshipGateLockState(value))
			{
				value.RaiseHotspotChanged();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("{value_1}", value.StageName);
				Dictionary<string, string> replacementStrings = dictionary;
				DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_friendshipgateopened", replacementStrings), "fg_" + value.BalancingData.NameId, DispatchMessage.Status.Info);
			}
			return base.UseMessageContent(player, callbackWhenDone);
		}
	}
}

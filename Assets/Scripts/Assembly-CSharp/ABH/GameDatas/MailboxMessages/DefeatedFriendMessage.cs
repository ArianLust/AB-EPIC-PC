using System;
using System.Collections.Generic;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.MailboxMessages
{
	public class DefeatedFriendMessage : BaseMailboxMessage
	{
		private Dictionary<string, LootInfoData> m_Loot;

		public override string ContentDescription
		{
			get
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("{value_1}", Sender.FriendName);
				return DIContainerInfrastructure.GetLocaService().Tr("social_notify_friendWon", dictionary);
			}
		}

		public override string IconAssetId
		{
			get
			{
				return "Check_Small";
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

		public override bool IsUsed
		{
			get
			{
				return IsViewed;
			}
		}

		public override bool ViewMessageContent(PlayerGameData player, Action<bool> callbackWhenDone)
		{
			if (callbackWhenDone != null)
			{
				callbackWhenDone(true);
			}
			player.SocialEnvironmentGameData.RemoveMessage(this);
			return true;
		}
	}
}

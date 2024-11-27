using System;
using System.Collections.Generic;
using ABH.GameDatas.MailboxMessages;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;

namespace ABH.GameDatas.Interfaces
{
	public interface IMailboxMessageGameData
	{
		string Id { get; }

		FriendGameData Sender { get; }

		string ContentDescription { get; }

		string IconAssetId { get; }

		string IconAtlasName { get; }

		string URL { get; }

		Dictionary<string, LootInfoData> Loot { get; }

		MessageInfo ResponseMessage { get; }

		bool IsNotSimpleCustomMessage { get; }

		bool HasURL { get; }

		bool HasReward { get; }

		bool HasResponseMessage { get; }

		bool IsViewed { get; }

		bool IsUsed { get; }

		long SendTime { get; }

		IMailboxMessageGameData SetMessageData(MessageDataIncoming message);

		MessageDataIncoming GetMessageData();

		bool UseMessageContent(PlayerGameData player, Action<bool, IMailboxMessageGameData> callbackWhenDone);

		bool ViewMessageContent(PlayerGameData player, Action<bool> callbackWhenDone);

		bool IsEqual(MessageDataIncoming message);

		void OnAddMessage();

		bool IsAddMessageAllowed(SocialEnvironmentGameData socialEnvironmentGameData);

		bool CheckAddRequirements(PlayerGameData playerGameData);
	}
}

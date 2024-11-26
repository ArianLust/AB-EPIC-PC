using System.Collections.Generic;
using System.Linq;
using ABH.Shared.Models;
using Rcs;

public class MessagingServiceBeaconImpl : IMessagingService
{
	public Mailbox m_Mailbox;

	public void Initialize()
	{
		DebugLog.Log(GetType(), "Initialize");
		m_Mailbox = new Mailbox(ContentLoader.Instance.m_BeaconConnectionMgr.Identiy);
		m_Mailbox.SetMessagesReceivedCallback(onMessagesReceived);
		m_Mailbox.SetStateChangedCallback(OnMailboxStateChanged);
		m_Mailbox.StartMonitoring();
	}

	private void OnMailboxStateChanged(Mailbox.StateType state)
	{
		DebugLog.Log(GetType(), "OnMailboxStateChanged: " + m_Mailbox.GetState());
	}

	private void onMessagesReceived(List<Message> messages)
	{
		DebugLog.Log(GetType(), "onMessagesReceived: Number of messages received: " + messages.Count);
		if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FirstMessageFetchTime == 0)
		{
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FirstMessageFetchTime = DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp();
		}
		List<MessageDataIncoming> list = ParseMessages(messages);
		List<MessageDataIncoming> list2 = new List<MessageDataIncoming>();
		foreach (MessageDataIncoming item in list)
		{
			if (item.SentAt >= DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FirstMessageFetchTime)
			{
				list2.Add(item);
				continue;
			}
			DebugLog.Log(string.Concat("Skipped Message: ", item.MessageType, "Id: ", item.Id, " because its from an older Account!"));
		}
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.AddIncomingMessages(list2);
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
	}

	public bool IsAvailable()
	{
		return m_Mailbox != null;
	}

	public Message RespondMessage(MessageDataIncoming message, string receiverId, Mailbox.SendSuccessCallback onResponseTold, Mailbox.SendErrorCallback onResponseError)
	{
		if (!IsAvailable())
		{
			DebugLog.Error(GetType(), "Not available!");
			return null;
		}
		DebugLog.Log(GetType(), "Start send Message");
		Message message2 = new Message(DIContainerInfrastructure.GetStringSerializer().Serialize(message));
		m_Mailbox.Send(receiverId, message2.GetContent(), onResponseTold, onResponseError);
		return message2;
	}

	public void SendMessages(MessageDataIncoming message, IEnumerable<string> receiverIds)
	{
		if (!IsAvailable())
		{
			DebugLog.Error(GetType(), "Not available!");
			return;
		}
		DebugLog.Log(GetType(), "Start send Message: " + DIContainerInfrastructure.GetStringSerializer().Serialize(message));
		Message message2 = new Message(DIContainerInfrastructure.GetStringSerializer().Serialize(message));
		foreach (string receiverId in receiverIds)
		{
			DebugLog.Log(GetType(), "SendMessage: Sending message to id: " + receiverId + "   content: " + message2.GetContent());
			m_Mailbox.Send(receiverId, message2.GetContent(), OnMessageSendSuccess, OnMessageSendError);
		}
		DoTheTracking(message, receiverIds);
	}

	private void DoTheTracking(MessageDataIncoming message, IEnumerable<string> receiverIds)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string text = ((DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData == null) ? "0" : ((DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data != null) ? DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.SocialId : "-1"));
		string text2 = ((receiverIds.Count() != 1) ? "broadcast" : receiverIds.FirstOrDefault());
		FacebookWrapperHatchImpl facebookWrapperHatchImpl = DIContainerInfrastructure.GetFacebookWrapper() as FacebookWrapperHatchImpl;
		if (facebookWrapperHatchImpl != null)
		{
			string facebookIdForFriendRovioAccId = facebookWrapperHatchImpl.GetFacebookIdForFriendRovioAccId(text2);
			if (!string.IsNullOrEmpty(facebookIdForFriendRovioAccId))
			{
				text2 = facebookIdForFriendRovioAccId;
				DebugLog.Log(GetType(), "SendMessages: found facebook id for friend: " + text2);
			}
			else
			{
				DebugLog.Log(GetType(), "SendMessages: found no facebook id for friend: " + text2);
			}
		}
		else
		{
			DebugLog.Error(GetType(), "SendMessages: facebook wrapper is no FacebookWrapperHatchImpl");
		}
		DebugLog.Log(GetType(), string.Concat("SendMessages: ", message.MessageType, ", ownSocialId = ", text, ", friendId = ", text2));
		dictionary.Add("social_network", DIContainerInfrastructure.GetFacebookWrapper().GetNetwork());
		dictionary.Add("social_network_id", text);
		dictionary.Add("social_network_friend_id", text2);
		dictionary.Add("social_network_request_id", message.MessageType.ToString());
		dictionary.Add("social_network_request_info", message.toShortParameterString());
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("SocialEvent", dictionary);
	}

	private void OnMessageSendError(Mailbox.ErrorCode error)
	{
		DebugLog.Log(GetType(), "OnMessageSendError: " + error);
	}

	private void OnMessageSendSuccess()
	{
		DebugLog.Log(GetType(), "OnMessageSendSuccess: SUCCESS!");
	}

	public void GetMessages(uint count)
	{
		if (!IsAvailable())
		{
			DebugLog.Error(GetType(), "Not available!");
			return;
		}
		DebugLog.Log(GetType(), "sync mailbox");
		m_Mailbox.Sync();
	}

	private List<MessageDataIncoming> ParseMessages(List<Message> messages)
	{
		List<MessageDataIncoming> list = new List<MessageDataIncoming>();
		foreach (Message message in messages)
		{
			list.Add(ParseMessage(message));
		}
		return list;
	}

	private MessageDataIncoming ParseMessage(Message message)
	{
		DebugLog.Log("Got Message: " + message.GetId() + "   " + message.GetContent() + "   " + message.GetSenderId());
		MessageDataIncoming messageDataIncoming = DIContainerInfrastructure.GetStringSerializer().Deserialize<MessageDataIncoming>(message.GetContent());
		messageDataIncoming.Id = message.GetId();
		messageDataIncoming.Sender.Id = message.GetSenderId();
		DebugLog.Log(string.Concat("Message ID: ", messageDataIncoming.Id, "Message Type: ", messageDataIncoming.MessageType, " SenderId: ", messageDataIncoming.Sender.Id));
		return messageDataIncoming;
	}
}

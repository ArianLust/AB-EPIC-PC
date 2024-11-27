using System.Collections.Generic;
using ABH.Shared.Models;
using Rcs;

public class MessagingServiceOfflineImpl : IMessagingService
{
	public void Initialize()
	{
	}

	public bool IsAvailable()
	{
		return false;
	}

	public Message RespondMessage(MessageDataIncoming message, string receiverId, Messaging.MessageSentCallback OnResponseTold, Messaging.ErrorCallback OnResponseError)
	{
		if (OnResponseTold != null)
		{
			OnResponseTold(null);
		}
		return null;
	}

	public void SendMessages(MessageDataIncoming message, IEnumerable<string> receiverIds)
	{
	}

	public void GetMessages(uint count)
	{
	}

	public Message RespondMessage(MessageDataIncoming message, string receiverId, Mailbox.SendSuccessCallback onResponseTold, Mailbox.SendErrorCallback onResponseError)
	{
		if (onResponseTold != null)
		{
			onResponseTold();
		}
		return null;
	}
}

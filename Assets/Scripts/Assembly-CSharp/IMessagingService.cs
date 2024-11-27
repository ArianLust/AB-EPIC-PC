using System.Collections.Generic;
using ABH.Shared.Models;
using Rcs;

public interface IMessagingService
{
	void Initialize();

	bool IsAvailable();

	Message RespondMessage(MessageDataIncoming message, string receiverId, Mailbox.SendSuccessCallback onResponseTold, Mailbox.SendErrorCallback onResponseError);

	void SendMessages(MessageDataIncoming message, IEnumerable<string> receiverIds);

	void GetMessages(uint count);
}

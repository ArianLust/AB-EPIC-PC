using System;

public interface IChannelService
{
	event Action ChannelRedirected;

	event Action ChannelClosed;

	void DisplayToonsTv(string groupId, string channeldId, string videoId);
}

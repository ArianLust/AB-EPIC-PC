using System;
using System.Runtime.CompilerServices;

public class ChannelServiceNullImpl : IChannelService
{
	private string m_lastMessageId;

	[method: MethodImpl(32)]
	public event Action ChannelRedirected;

	[method: MethodImpl(32)]
	public event Action ChannelClosed;

	public void DisplayToonsTv()
	{
	}

	public void DisplayToonsTv(string groupId, string channeldId, string videoId)
	{
	}
}

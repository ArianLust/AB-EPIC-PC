using System;
using System.Runtime.CompilerServices;
using Rcs;
using UnityEngine;

public class ChannelServiceBeaconImpl : IChannelService
{
	private Channel m_channel;

	public Channel Channel
	{
		get
		{
			if (m_channel == null)
			{
				m_channel = new Channel(ContentLoader.Instance.m_BeaconConnectionMgr.Identiy);
			}
			return m_channel;
		}
	}

	[method: MethodImpl(32)]
	public event Action ChannelRedirected;

	[method: MethodImpl(32)]
	public event Action ChannelClosed;

	public void DisplayToonsTv(string groupId, string channelId, string videoId)
	{
		if (!Channel.IsChannelViewOpened())
		{
			DIContainerInfrastructure.GetAsynchStatusService().ShowLocalLoading(DIContainerInfrastructure.GetLocaService().Tr("skynest_loadingtoonstv", "Loading Toons.tv..."), true);
			DIContainerInfrastructure.AudioManager.AddMuteReason(1, GetType().ToString());
			DIContainerInfrastructure.AudioManager.AddMuteReason(0, GetType().ToString());
			DebugLog.Log("Try OpenChannelView");
			Channel.Params @params = new Channel.Params();
			if (!string.IsNullOrEmpty(channelId))
			{
				DebugLog.Log(GetType(), "setting Channel Id = " + channelId);
				@params.ChannelId = channelId;
			}
			if (!string.IsNullOrEmpty(groupId))
			{
				DebugLog.Log(GetType(), "setting Group Id = " + groupId);
				@params.GroupId = groupId;
			}
			if (!string.IsNullOrEmpty(videoId))
			{
				DebugLog.Log(GetType(), "setting Video Id = " + videoId);
				@params.VideoId = videoId;
			}
			@params.Width = Screen.width;
			@params.Height = Screen.height;
			Channel.OpenChannelView(@params, SkynestChannelActions);
		}
	}

	private void SkynestChannelActions(Channel.LoadResult loadResult)
	{
		switch (loadResult)
		{
		case Channel.LoadResult.Cancelled:
			DebugLog.Log(GetType(), "SkynestChannelActions: CANCELLED");
			break;
		case Channel.LoadResult.Closed:
			DebugLog.Log(GetType(), "SkynestChannelActions: CLOSED");
			SkynestChannelOnClosed();
			break;
		case Channel.LoadResult.Failed:
			DebugLog.Log(GetType(), "SkynestChannelActions: FAILED");
			SkynestChannelLoadingError();
			break;
		case Channel.LoadResult.Redirected:
			DebugLog.Log(GetType(), "SkynestChannelActions: REDIRECTED");
			OnChannelRedirected();
			break;
		case Channel.LoadResult.Success:
			DebugLog.Log(GetType(), "SkynestChannelActions: SUCCESS");
			ToonsChannelShown();
			break;
		}
	}

	private void ToonsChannelShown()
	{
		DebugLog.Log("[Skynest Channel] Shown!");
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("channel_show");
	}

	private void SkynestChannelLoadingError()
	{
		DebugLog.Error("Channel loading failed!");
		DIContainerInfrastructure.GetAsynchStatusService().ShowError(DIContainerInfrastructure.GetLocaService().Tr("skynest_failedtoonstv", "Failed Loading Toons.tv"), "cinema");
		DIContainerInfrastructure.GetAsynchStatusService().HideLocalLoading();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("channel_show");
		Channel.CancelChannelViewLoading();
		DIContainerInfrastructure.AudioManager.RemoveMuteReason(1, GetType().ToString());
		DIContainerInfrastructure.AudioManager.RemoveMuteReason(0, GetType().ToString());
	}

	private void SkynestChannelOnClosed()
	{
		DebugLog.Error("Channel closed!");
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("skynest_closedtoonstv", "Thanks for watching Toons.tv!"), null, DispatchMessage.Status.Info);
		DIContainerInfrastructure.GetAsynchStatusService().HideLocalLoading();
		DIContainerInfrastructure.AudioManager.RemoveMuteReason(1, GetType().ToString());
		DIContainerInfrastructure.AudioManager.RemoveMuteReason(0, GetType().ToString());
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("channel_show");
		if (this.ChannelClosed != null)
		{
			this.ChannelClosed();
		}
	}

	private void OnChannelRedirected()
	{
		DebugLog.Log(GetType(), "Channel Redirected Callback!");
		DIContainerInfrastructure.GetAsynchStatusService().HideLocalLoading();
		DIContainerInfrastructure.AudioManager.RemoveMuteReason(1, GetType().ToString());
		DIContainerInfrastructure.AudioManager.RemoveMuteReason(0, GetType().ToString());
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("channel_show");
		if (this.ChannelRedirected != null)
		{
			this.ChannelRedirected();
		}
	}
}

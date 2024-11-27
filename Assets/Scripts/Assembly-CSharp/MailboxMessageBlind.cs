using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ABH.GameDatas.Interfaces;
using ABH.GameDatas.MailboxMessages;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using UnityEngine;

public class MailboxMessageBlind : MonoBehaviour
{
	[SerializeField]
	private GameObject m_BuyIndicatorPrefab;

	[SerializeField]
	private UISprite m_ButtonSprite;

	[SerializeField]
	private UIAtlas m_GenericFriendIconsAtlas;

	[SerializeField]
	public UIInputTrigger m_ConfirmButtonTrigger;

	[SerializeField]
	public UIInputTrigger m_TextButtonTrigger;

	[SerializeField]
	public UILabel m_Description;

	[SerializeField]
	public FriendInfoElement m_FriendInfo;

	[SerializeField]
	public CHMotionTween m_Tween;

	[SerializeField]
	public SoundTriggerList m_SoundTriggers;

	private IMailboxMessageGameData m_Model;

	private BaseCampStateMgr m_StateMgr;

	private ArenaCampStateMgr m_ArenaStateMgr;

	private bool m_Destroyed;

	[method: MethodImpl(32)]
	public event Action<IMailboxMessageGameData> MessageConfirmed;

	public IMailboxMessageGameData GetModel()
	{
		return m_Model;
	}

	public void ShowTooltip()
	{
	}

	public void SetModel(IMailboxMessageGameData model, BaseCampStateMgr stateMgr, int index, bool PictureAlreadySet = false)
	{
		RegisterEventHandlers();
		m_StateMgr = stateMgr;
		m_Model = model;
		base.gameObject.name = index.ToString("000") + "_MailboxMessage";
		m_Description.text = DIContainerInfrastructure.GetLocaService().ReplaceUnmappableCharacters(m_Model.ContentDescription);
		if (m_Model.IsNotSimpleCustomMessage)
		{
			UIAtlas uIAtlas = null;
			if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset(m_Model.IconAtlasName))
			{
				GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject(m_Model.IconAtlasName) as GameObject;
				if (gameObject != null)
				{
					uIAtlas = gameObject.GetComponent<UIAtlas>();
				}
			}
			if ((bool)uIAtlas)
			{
				m_ButtonSprite.atlas = uIAtlas;
				m_ButtonSprite.spriteName = m_Model.IconAssetId;
			}
			else
			{
				m_ButtonSprite.spriteName = "Check_Small";
			}
		}
		else if (m_Model.HasReward)
		{
			m_ButtonSprite.atlas = m_GenericFriendIconsAtlas;
			m_ButtonSprite.spriteName = "SocialReward";
		}
		else
		{
			m_ButtonSprite.spriteName = "Check_Small";
		}
		if (m_Model.Sender.isNpcFriend)
		{
			GetComponent<GenericOverlayInvoker>().m_LocaIdent = "social_mail_npc_tt";
		}
		m_FriendInfo.SetDefault();
		m_FriendInfo.SetModel(m_Model.Sender, PictureAlreadySet);
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		if (m_ConfirmButtonTrigger != null)
		{
			m_ConfirmButtonTrigger.Clicked += ConfirmButtonTriggerClicked;
		}
		if (m_TextButtonTrigger != null)
		{
			m_TextButtonTrigger.Clicked += TextButtonClicked;
		}
	}

	private void TextButtonClicked()
	{
		if (m_Model.HasURL)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Button", "MessageLinkButton");
			dictionary.Add("Destination", "CustomMessage");
			dictionary.Add("URL", m_Model.URL);
			DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("LinkClicked", dictionary);
			Application.OpenURL(m_Model.URL);
		}
	}

	private void ConfirmButtonTriggerClicked()
	{
		if ((bool)m_SoundTriggers)
		{
			m_SoundTriggers.OnTriggerEventFired("message_reward_accepted");
		}
		if (m_Model.ViewMessageContent(DIContainerInfrastructure.GetCurrentPlayer(), OnMessageViewed))
		{
			if (m_Model is ResponseFriendshipEssenceMessage)
			{
				MessageDataIncoming messageDataIncoming = new MessageDataIncoming();
				messageDataIncoming.MessageType = MessageType.ResponseFriendshipEssenceMessage;
				messageDataIncoming.Sender = DIContainerInfrastructure.GetCurrentPlayer().GetFriendData();
				messageDataIncoming.SentAt = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
				ABHAnalyticsHelper.SendSocialEvent(messageDataIncoming, m_Model.Sender.FriendData);
			}
			else if (m_Model is ResponseFriendshipGateMessage)
			{
				MessageDataIncoming messageDataIncoming = new MessageDataIncoming();
				messageDataIncoming.MessageType = MessageType.ResponseFriendshipGateMessage;
				messageDataIncoming.Sender = DIContainerInfrastructure.GetCurrentPlayer().GetFriendData();
				messageDataIncoming.SentAt = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
				ABHAnalyticsHelper.SendSocialEvent(messageDataIncoming, m_Model.Sender.FriendData);
			}
			DeRegisterEventHandlers();
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateFriendshipEssenceBar();
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateLuckyCoinsBar();
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateCoinsBar();
		}
	}

	private IEnumerator ShowConfirmationAndRefresh()
	{
		yield return new WaitForSeconds(ShowBuyedIndicator());
		if (m_Model.HasReward)
		{
			m_ButtonSprite.gameObject.SetActive(false);
		}
		if (m_StateMgr != null)
		{
			m_StateMgr.m_SocialWindow.SoftMailboxRefresh(this);
		}
		else if (m_ArenaStateMgr != null)
		{
			m_ArenaStateMgr.m_SocialWindow.SoftMailboxRefresh(this);
		}
	}

	private void OnMessageFailed()
	{
		DebugLog.Warn("Failed to View Message");
		if (m_Destroyed)
		{
			DIContainerLogic.SocialService.RemoveMessage(DIContainerInfrastructure.GetCurrentPlayer(), m_Model);
			return;
		}
		DIContainerLogic.SocialService.RemoveMessage(DIContainerInfrastructure.GetCurrentPlayer(), m_Model);
		StartCoroutine(ShowFailAndRefresh());
	}

	private IEnumerator ShowFailAndRefresh()
	{
		yield return new WaitForSeconds(ShowBuyedIndicator());
		if (m_Model.HasReward)
		{
			m_ButtonSprite.gameObject.SetActive(false);
		}
		if (m_StateMgr != null)
		{
			m_StateMgr.m_SocialWindow.SoftMailboxRefresh(this);
		}
		else if (m_ArenaStateMgr != null)
		{
			m_ArenaStateMgr.m_SocialWindow.SoftMailboxRefresh(this);
		}
	}

	public void OnMessageViewed(bool succes)
	{
		if (!succes && !m_Destroyed)
		{
			OnMessageFailed();
		}
		else if (m_Destroyed)
		{
			if (succes)
			{
				DIContainerLogic.SocialService.RemoveMessage(DIContainerInfrastructure.GetCurrentPlayer(), m_Model);
			}
		}
		else
		{
			DIContainerLogic.SocialService.RemoveMessage(DIContainerInfrastructure.GetCurrentPlayer(), m_Model);
			StartCoroutine(ShowConfirmationAndRefresh());
		}
	}

	private void DeRegisterEventHandlers()
	{
		if ((bool)m_ConfirmButtonTrigger)
		{
			m_ConfirmButtonTrigger.Clicked -= ConfirmButtonTriggerClicked;
		}
		if ((bool)m_TextButtonTrigger)
		{
			m_TextButtonTrigger.Clicked -= TextButtonClicked;
		}
	}

	private void OnDestroy()
	{
		m_Destroyed = true;
		DeRegisterEventHandlers();
	}

	public float ShowBuyedIndicator()
	{
		return 0f;
	}

	private void SetLayerRecusively(GameObject go, int layer)
	{
		go.layer = layer;
		foreach (Transform item in go.transform)
		{
			SetLayerRecusively(item.gameObject, layer);
		}
	}

	public IEnumerator MoveOffset(Vector2 offset, float duration)
	{
		Vector3 move = new Vector3(offset.x, offset.y, 0f);
		if ((bool)m_Tween)
		{
			m_Tween.m_EndOffset = offset;
			m_Tween.m_DurationInSeconds = duration;
			m_Tween.Play();
			yield return new WaitForSeconds(duration);
		}
	}
}

using System;
using System.Collections;
using ABH.GameDatas;
using ABH.Shared.Models;
using UnityEngine;

public class FriendVisitingBlind : MonoBehaviour
{
	[SerializeField]
	private UILabel m_IndexLabel;

	[SerializeField]
	private UIInputTrigger m_DeleteFriend;

	[SerializeField]
	public UIInputTrigger m_VisitFriend;

	[SerializeField]
	public UIInputTrigger m_FightFriend;

	[SerializeField]
	private UISprite m_LeagueIcon;

	[SerializeField]
	private UILabel m_CurrentRank;

	[SerializeField]
	private Animation m_SelectAnimation;

	[SerializeField]
	private FriendInfoElement m_FriendInfoElement;

	[SerializeField]
	private GameObject m_BonusRoot;

	[SerializeField]
	private GameObject m_FreeGachaRoot;

	[SerializeField]
	private GameObject m_TimerPrefab;

	[SerializeField]
	private ResourceCostBlind m_CostPrefab;

	[SerializeField]
	private Transform m_AlternatingRoot;

	[SerializeField]
	private CHMotionTween m_Tween;

	private SocialWindowUI m_StateMgr;

	private FriendGameData m_Model;

	private ResourceCostBlind m_InstancedCostBlind;

	[SerializeField]
	private Vector3 m_AlternatingCharacterOffset;

	private bool m_pvp;

	public void Initialize(SocialWindowUI stateMgr, int index, bool pvp)
	{
		m_StateMgr = stateMgr;
		m_pvp = pvp;
		m_FriendInfoElement.SetDefault();
		base.gameObject.name = index.ToString("000") + "_FriendBlind";
		if ((bool)m_IndexLabel)
		{
			m_IndexLabel.text = (index + 1).ToString("0");
			if (index < 0)
			{
				m_IndexLabel.gameObject.SetActive(false);
				base.gameObject.name = 999 + "_FriendBlind";
			}
		}
	}

	public void SetAddFriendBonus(int index, GameObject bonus)
	{
		if ((bool)bonus)
		{
			bonus.transform.parent = m_BonusRoot.transform;
			bonus.transform.localPosition = Vector3.zero + m_AlternatingCharacterOffset * GetOffset(index);
		}
	}

	private int GetOffset(int index)
	{
		if (index % 2 == 1)
		{
			return 1;
		}
		return -1;
	}

	public FriendVisitingBlind SetModel(FriendGameData friend)
	{
		if (friend == null)
		{
			return this;
		}
		RegisterEventHandler();
		if ((bool)m_FriendInfoElement)
		{
			m_FriendInfoElement.SetModel(friend);
		}
		else
		{
			DebugLog.Warn("Friend info element is null!");
		}
		m_Model = friend;
		if ((bool)m_FreeGachaRoot)
		{
			if (m_pvp)
			{
				m_FreeGachaRoot.SetActive(DIContainerLogic.SocialService.HasFreePvpGachaRoll(friend, DIContainerInfrastructure.GetCurrentPlayer()));
			}
			else
			{
				m_FreeGachaRoot.SetActive(DIContainerLogic.SocialService.HasFreeGachaRoll(friend, DIContainerInfrastructure.GetCurrentPlayer()));
			}
			if (friend.PublicPlayerData != null && m_FreeGachaRoot.GetComponentInChildren<UISprite>() != null)
			{
				InventoryGameData inventory = new InventoryGameData(friend.PublicPlayerData.Inventory);
				if (DIContainerLogic.InventoryService.GetItemValue(inventory, "story_goldenpig_advanced") >= 1)
				{
					m_FreeGachaRoot.GetComponentInChildren<UISprite>().spriteName = "AdvFreeGachaRoll";
				}
				else
				{
					m_FreeGachaRoot.GetComponentInChildren<UISprite>().spriteName = "FreeGachaRoll";
				}
			}
		}
		if ((bool)m_DeleteFriend)
		{
			m_DeleteFriend.gameObject.SetActive(!friend.isNpcFriend);
		}
		if (!m_pvp)
		{
			return this;
		}
		if (friend.PublicPlayerData != null && friend.PublicPlayerData.Banner != null)
		{
			if ((bool)m_BonusRoot)
			{
				m_BonusRoot.SetActive(true);
			}
			if ((bool)m_FightFriend)
			{
				m_FightFriend.gameObject.SetActive(true);
			}
			if ((bool)m_LeagueIcon)
			{
				m_LeagueIcon.spriteName = PvPSeasonManagerGameData.GetLeagueAssetName(friend.PublicPlayerData.League);
			}
			if ((bool)m_CurrentRank)
			{
				m_CurrentRank.text = "#" + friend.PublicPlayerData.PvPRank.ToString("0");
			}
		}
		else
		{
			if ((bool)m_BonusRoot)
			{
				m_BonusRoot.SetActive(false);
			}
			if ((bool)m_FightFriend)
			{
				m_FightFriend.gameObject.SetActive(false);
			}
			if ((bool)m_LeagueIcon)
			{
				m_LeagueIcon.gameObject.SetActive(false);
			}
			if ((bool)m_CurrentRank)
			{
				m_LeagueIcon.gameObject.SetActive(false);
			}
		}
		return this;
	}

	private IEnumerator CountDownTimer(UILabel timerLabel, DateTime targetTime)
	{
		DateTime trustedTime;
		while (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		while (targetTime > trustedTime)
		{
			if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				TimeSpan timeLeft = targetTime - trustedTime;
				timerLabel.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(timeLeft);
			}
			yield return new WaitForSeconds(1f);
		}
		UnityEngine.Object.Destroy(timerLabel.gameObject);
	}

	public FriendGameData GetModel()
	{
		return m_Model;
	}

	public void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		if ((bool)m_DeleteFriend)
		{
			m_DeleteFriend.Clicked += m_DeleteFriend_Clicked;
		}
		if ((bool)m_VisitFriend)
		{
			m_VisitFriend.Clicked += m_VisitFriend_Clicked;
		}
		if ((bool)m_FightFriend)
		{
			m_FightFriend.Clicked += OnFightFriendClicked;
		}
	}

	public void DeRegisterEventHandler()
	{
		if ((bool)m_DeleteFriend)
		{
			m_DeleteFriend.Clicked -= m_DeleteFriend_Clicked;
		}
		if ((bool)m_VisitFriend)
		{
			m_VisitFriend.Clicked -= m_VisitFriend_Clicked;
		}
		if ((bool)m_FightFriend)
		{
			m_FightFriend.Clicked -= OnFightFriendClicked;
		}
	}

	private void m_DeleteFriend_Clicked()
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.RemoveFriend(m_Model);
		m_StateMgr.ResetWindow();
	}

	private void m_VisitFriend_Clicked()
	{
		DeRegisterEventHandler();
		if (m_Model.isNpcFriend)
		{
			OnReceivedPlayer(DIContainerLogic.SocialService.GetNPCPlayer(m_Model, Mathf.Max(1, DIContainerInfrastructure.GetCurrentPlayer().Data.Level - 2)));
			return;
		}
		if (m_Model.PublicPlayerData != null)
		{
			OnReceivedPlayer(m_Model.PublicPlayerData);
		}
		else
		{
			DIContainerInfrastructure.GetAsynchStatusService().ShowError(DIContainerInfrastructure.GetLocaService().Tr("toast_error_loadingplayer", "The inspect of the player is not possible at the moment. Please try later!"), "inspect_error");
			if (m_pvp)
			{
				if (DIContainerLogic.SocialService.HasFreePvpGachaRoll(m_Model, DIContainerInfrastructure.GetCurrentPlayer()))
				{
					DIContainerLogic.SocialService.UsedFreePvpGachaRoll(m_Model, DIContainerInfrastructure.GetCurrentPlayer());
				}
			}
			else if (DIContainerLogic.SocialService.HasFreeGachaRoll(m_Model, DIContainerInfrastructure.GetCurrentPlayer()))
			{
				DIContainerLogic.SocialService.UsedFreeGachaRoll(m_Model, DIContainerInfrastructure.GetCurrentPlayer());
			}
		}
		SetModel(m_Model);
	}

	private void OnFightFriendClicked()
	{
		ArenaCampStateMgr arenaCampStateMgr = DIContainerInfrastructure.BaseStateMgr as ArenaCampStateMgr;
		if ((bool)arenaCampStateMgr)
		{
			if (m_Model.isNpcFriend)
			{
				arenaCampStateMgr.StartUnrankedMatch(DIContainerLogic.SocialService.GetNPCPlayer(m_Model, Mathf.Max(1, DIContainerInfrastructure.GetCurrentPlayer().Data.Level - 2)), m_Model);
			}
			else
			{
				arenaCampStateMgr.StartUnrankedMatch(m_Model.PublicPlayerData, m_Model);
			}
			m_StateMgr.Leave(false);
		}
	}

	private void OnReceivedPlayer(PublicPlayerData player)
	{
		DIContainerInfrastructure.GetAsynchStatusService().HideLocalLoading();
		m_StateMgr.Leave();
		m_StateMgr.m_StateMgr.DeRegisterEventHandler();
		if (m_pvp)
		{
			DIContainerInfrastructure.GetCoreStateMgr().GotoFirendArenaScreen(player, m_Model);
		}
		else
		{
			DIContainerInfrastructure.GetCoreStateMgr().GotoFirendCampScreen(player, m_Model);
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

	private void OnDestroy()
	{
		DeRegisterEventHandler();
	}
}

using System.Collections.Generic;
using ABH.Shared.Models;
using UnityEngine;

[RequireComponent(typeof(BaseCampStateMgr))]
public class DebugMessageUI : GenericDebugUI
{
	private bool m_Opened;

	private BaseCampStateMgr m_CampStateMgr;

	private ulong id;

	private bool m_ShowMessages;

	public List<FriendData> m_AllFriends = new List<FriendData>();

	public List<bool> m_ShouldInvite = new List<bool>();

	private Vector2 scrollPosition;
}

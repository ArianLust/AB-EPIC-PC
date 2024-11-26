using System.Collections.Generic;
using ABH.Services.Logic;
using UnityEngine;

[RequireComponent(typeof(DebugBattleUI))]
[RequireComponent(typeof(BattleMgr))]
public class DebugBattleLogUI : GenericDebugUI
{
	public class BattleDebugMessage
	{
		public string message;

		public BattleLogTypes type;
	}

	private bool m_Opened;

	private BattleMgrBase m_BattleMgr;

	private DebugBattleUI m_DebugBattleUI;

	private Vector3 m_BasePos = default(Vector3);

	private Queue<BattleDebugMessage> messages = new Queue<BattleDebugMessage>();

	public int MessageQueueLength = 15;
}

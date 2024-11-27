using System.Collections.Generic;
using ABH.Services.Logic;
using UnityEngine;

[RequireComponent(typeof(BattleMgr))]
public class DebugBattleUI : GenericDebugUI
{
	private bool m_Opened;

	private BattleMgrBase m_BattleMgr;

	public bool m_ShowLog;

	public bool m_ClearLog;

	public List<BattleLogTypes> LogFilter = new List<BattleLogTypes>
	{
		BattleLogTypes.BattleEffect,
		BattleLogTypes.Damage,
		BattleLogTypes.Heal,
		BattleLogTypes.Bird,
		BattleLogTypes.General,
		BattleLogTypes.Perk,
		BattleLogTypes.Pig,
		BattleLogTypes.Rage
	};
}

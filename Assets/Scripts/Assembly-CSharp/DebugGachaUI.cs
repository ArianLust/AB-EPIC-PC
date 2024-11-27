using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Services.Logic;
using UnityEngine;

[RequireComponent(typeof(CampStateMgr))]
public class DebugGachaUI : GenericDebugUI
{
	private bool m_Opened;

	private CampStateMgr m_CampStateMgr;

	private RequirementOperationServiceInjectableImpl injectableImpl;

	private bool m_UseFreePruchase;

	private PlayerGameData player;

	private List<KeyValuePair<string, int>> haveItemReqs = new List<KeyValuePair<string, int>>();

	private List<KeyValuePair<string, int>> notHaveItemReqs = new List<KeyValuePair<string, int>>();
}

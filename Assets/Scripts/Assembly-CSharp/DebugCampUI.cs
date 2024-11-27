using UnityEngine;

[RequireComponent(typeof(BaseCampStateMgr))]
public class DebugCampUI : GenericDebugUI
{
	private bool m_Opened;

	private BaseCampStateMgr m_CampStateMgr;

	private int hammerLevel = 1;

	private int swordLevel = 1;

	private bool showPiggieVisits;

	private string _mailboxActionStatus = string.Empty;

	private bool m_UseFreePruchase;
}

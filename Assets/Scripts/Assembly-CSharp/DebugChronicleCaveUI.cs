using UnityEngine;

[RequireComponent(typeof(ChronicleCaveStateMgr))]
public class DebugChronicleCaveUI : GenericDebugUI
{
	private bool m_Opened;

	private ChronicleCaveStateMgr m_ChronicleCaveStateMgr;
}

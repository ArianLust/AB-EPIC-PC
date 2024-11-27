using UnityEngine;

[RequireComponent(typeof(ActionTree))]
public class DebugActionTreeUI : GenericDebugUI
{
	private bool m_Opened;

	private ActionTree m_ActionTree;

	public bool m_AlwaysShow;
}

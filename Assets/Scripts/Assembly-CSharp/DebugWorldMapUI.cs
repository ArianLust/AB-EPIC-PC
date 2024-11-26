using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WorldMapStateMgr))]
public class DebugWorldMapUI : GenericDebugUI
{
	private bool m_Opened;

	private WorldMapStateMgr m_WorldMapStateMgr;

	private List<ActionTree> m_StorySequences;

	public Vector2 scrollPosition = Vector2.zero;

	public PinchZoom m_PinchZoom;

	private string m_HotspotID = "hotspot_091_chroniclecave";

	private string m_ProgressId = "1";

	private int m_restedBonusAmount;
}

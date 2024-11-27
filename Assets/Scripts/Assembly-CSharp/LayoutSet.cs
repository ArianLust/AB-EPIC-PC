using System;
using UnityEngine;

[Serializable]
public class LayoutSet
{
	public GameObject m_goSource;

	public bool m_bCam = true;

	public GameObject m_goRelativeTo;

	public int m_iRelativeToScreen;

	public static string[] m_AnchorNames = new string[9] { "UL", "UM", "UR", "ML", "MM", "MR", "BL", "BM", "BR" };

	public bool m_IsPositionPercentX;

	public bool m_IsPositionPercentY;

	public bool m_IsPositionPercentZ;

	public Vector3 m_v3Position = Vector3.zero;

	public LayoutSet()
	{
	}

	public LayoutSet(LayoutSet copy)
	{
		m_goSource = copy.m_goSource;
		m_bCam = copy.m_bCam;
		m_goRelativeTo = copy.m_goRelativeTo;
		m_iRelativeToScreen = copy.m_iRelativeToScreen;
		m_IsPositionPercentX = copy.m_IsPositionPercentX;
		m_IsPositionPercentY = copy.m_IsPositionPercentY;
		m_IsPositionPercentZ = copy.m_IsPositionPercentZ;
		m_v3Position = copy.m_v3Position;
	}

	public bool AreValuesEqual(LayoutSet test)
	{
		return m_goSource == test.m_goSource && m_bCam == test.m_bCam && m_goRelativeTo == test.m_goRelativeTo && m_iRelativeToScreen == test.m_iRelativeToScreen && m_IsPositionPercentX == test.m_IsPositionPercentX && m_IsPositionPercentY == test.m_IsPositionPercentY && m_IsPositionPercentZ == test.m_IsPositionPercentZ && m_v3Position == test.m_v3Position;
	}
}

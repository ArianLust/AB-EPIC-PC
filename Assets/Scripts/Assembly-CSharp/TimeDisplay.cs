using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
	public const float UpdateInterval = 0.2f;

	[SerializeField]
	private UILabel m_label;

	private float m_lastUpdate;

	public void Awake()
	{
		base.useGUILayout = false;
		base.enabled = false;
	}
}

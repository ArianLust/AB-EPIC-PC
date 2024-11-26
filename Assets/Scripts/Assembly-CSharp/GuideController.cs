using UnityEngine;

public class GuideController : MonoBehaviour
{
	public Animation m_Animation;

	private string m_Trigger;

	private bool m_Left;

	private bool m_HaveFinishedSpawnPos;

	private Vector3 m_FinishedSpawnPos = default(Vector3);

	public GuideController SetThumbUpPosition(Vector3 pos)
	{
		m_HaveFinishedSpawnPos = true;
		m_FinishedSpawnPos = pos;
		return this;
	}

	public bool TryGetFinishSpawnPos(out Vector3 spawnPos)
	{
		spawnPos = m_FinishedSpawnPos;
		return m_HaveFinishedSpawnPos;
	}

	public void Enter(string trigger)
	{
		m_Trigger = trigger;
	}

	public void Leave()
	{
		m_Left = true;
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		if (!m_Left && DIContainerInfrastructure.TutorialMgr != null)
		{
			DIContainerInfrastructure.TutorialMgr.HideHelp(m_Trigger, false);
		}
	}
}

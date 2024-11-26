using UnityEngine;

public class BattleLootVisualization : MonoBehaviour
{
	private Vector2 m_MoveDir;

	private bool m_DoMove;

	public Vector2 MoveDirMin = new Vector2(-160f, -80f);

	public Vector2 MoveDirMax = new Vector2(160f, 0f);

	public CHMotionTween m_MotionTween;

	public float MoveTime = 0.6f;

	public float SpawnDelay;

	private void Start()
	{
		m_MoveDir.x = Random.Range(MoveDirMin.x, MoveDirMax.x);
		m_MoveDir.y = Random.Range(MoveDirMin.y, MoveDirMax.y);
		if ((bool)m_MotionTween)
		{
			m_MotionTween.m_EndOffset = new Vector3(m_MoveDir.x, m_MoveDir.y);
			m_MotionTween.m_DurationInSeconds = MoveTime;
		}
		m_DoMove = true;
		if (SpawnDelay > 0f)
		{
			base.gameObject.SetActive(false);
			Invoke("Activate", SpawnDelay);
		}
		else
		{
			Activate();
		}
	}

	public void ReStart()
	{
		CancelInvoke();
		Start();
	}

	private void StopMove()
	{
		m_DoMove = false;
	}

	private void Activate()
	{
		base.gameObject.SetActive(true);
		if ((bool)m_MotionTween)
		{
			m_MotionTween.Play();
		}
		Invoke("StopMove", MoveTime + SpawnDelay);
	}

	private void Update()
	{
		if (m_DoMove && !m_MotionTween)
		{
			base.transform.Translate(m_MoveDir.x * Time.deltaTime, m_MoveDir.y * Time.deltaTime, 0f);
		}
	}
}

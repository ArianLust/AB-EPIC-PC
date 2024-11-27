using UnityEngine;

public class BattleLootController : MonoBehaviour
{
	private Vector2 m_MoveDir;

	private bool m_DoMove;

	private int m_Force;

	private void Start()
	{
		m_Force = Random.Range(100, 150);
		m_MoveDir.x = Random.Range(-160, 160);
		m_MoveDir.y = Random.Range(-80, 0);
		m_MoveDir.Normalize();
		m_DoMove = true;
		Invoke("StopMove", 0.6f);
	}

	private void StopMove()
	{
		m_DoMove = false;
	}

	private void Update()
	{
		if (m_DoMove)
		{
			base.transform.Translate(m_MoveDir.x * (float)m_Force * Time.deltaTime, m_MoveDir.y * (float)m_Force * Time.deltaTime, 0f);
		}
	}
}

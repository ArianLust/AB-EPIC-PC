using UnityEngine;

public class ScaleController : MonoBehaviour
{
	public Transform m_ScaleTransform;

	public Vector3 m_BaseScale = new Vector3(1f, 1f, 1f);

	private UnityEngine.AI.NavMeshAgent m_agent;

	private float m_baseSpeed;

	[HideInInspector]
	public uint m_Index;

	[HideInInspector]
	public float m_Last_z_Position;

	private void Start()
	{
		if ((bool)ScaleMgr.Instance)
		{
			ScaleMgr.Instance.RemoveScaleController(this);
			ScaleMgr.Instance.RegisterScaleController(this);
		}
		m_agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		if (m_agent != null)
		{
			m_baseSpeed = m_agent.speed;
		}
	}

	private void OnEnable()
	{
		if ((bool)ScaleMgr.Instance)
		{
			ScaleMgr.Instance.RemoveScaleController(this);
			ScaleMgr.Instance.RegisterScaleController(this);
		}
	}

	private void OnDisable()
	{
		if (ScaleMgr.Instance != null)
		{
			ScaleMgr.Instance.RemoveScaleController(this);
		}
	}

	private void OnDestroy()
	{
		if ((bool)ScaleMgr.Instance)
		{
			ScaleMgr.Instance.RemoveScaleController(this);
		}
	}

	public UnityEngine.AI.NavMeshAgent GetNavMeshAgent()
	{
		return m_agent;
	}

	public float GetBaseSpeed()
	{
		return m_baseSpeed;
	}
}

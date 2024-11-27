using System.Collections;
using ABH.GameDatas.Interfaces;
using UnityEngine;

public class RageMeterFXMovement : MonoBehaviour
{
	private RageMeterController m_RageMeter;

	private CHMotionTween m_RageFxTween;

	private Transform m_Source;

	private Vector3 m_Offset;

	public void Init(RageMeterController meter, ICombatant source, Vector3 offset)
	{
		m_RageMeter = meter;
		m_Source = source.CombatantView.transform;
		m_Offset = offset;
	}

	private void Awake()
	{
		m_RageFxTween = base.transform.GetComponent<CHMotionTween>();
	}

	private void Start()
	{
		m_RageFxTween.m_StartTransform = m_Source;
		m_RageFxTween.m_StartOffset = Vector3.zero;
		m_RageFxTween.m_EndTransform = m_RageMeter.transform;
		m_RageFxTween.m_EndOffset = m_Offset;
		m_RageFxTween.Play();
		StartCoroutine(RageOnItsWay());
	}

	private IEnumerator RageOnItsWay()
	{
		yield return new WaitForSeconds(m_RageFxTween.MovementDuration);
		Object.Destroy(base.gameObject);
	}

	public float GetFlyTime()
	{
		return m_RageFxTween.MovementDuration;
	}
}

using UnityEngine;

public class FollowTransform : MonoBehaviour
{
	[SerializeField]
	private Transform m_TransformToFollow;

	[SerializeField]
	private Vector3 m_Scale;

	[SerializeField]
	private Vector3 m_Offset;

	[SerializeField]
	private float m_Speed;

	[SerializeField]
	private Camera m_ScreenPosCamera;

	private Transform cachedTransform;

	private void Start()
	{
		cachedTransform = base.transform;
		cachedTransform.position = Vector3.Scale(m_TransformToFollow.position, m_Scale) + Vector3.Scale(cachedTransform.position, Vector3.one - m_Scale);
		cachedTransform.position += m_Offset;
	}

	private void OnEnable()
	{
		cachedTransform = base.transform;
		cachedTransform.position = Vector3.Scale(m_TransformToFollow.position, m_Scale) + Vector3.Scale(cachedTransform.position, Vector3.one - m_Scale);
		cachedTransform.position += m_Offset;
	}

	private void LateUpdate()
	{
		cachedTransform.position = Vector3.Scale(cachedTransform.position, Vector3.one - m_Scale) + Vector3.Scale(Vector3.Lerp(cachedTransform.position, m_TransformToFollow.position, m_Speed), m_Scale);
		cachedTransform.position += m_Offset;
	}
}

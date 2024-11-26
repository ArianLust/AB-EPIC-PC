using UnityEngine;

[AddComponentMenu("Chimera/DestroyGameObjectAfterTime")]
public class DestroyGameObjectAfterTime : MonoBehaviour
{
	[SerializeField]
	private float m_TimeTillDestroy = 1f;

	private void Start()
	{
		Object.Destroy(base.gameObject, m_TimeTillDestroy);
	}
}

using UnityEngine;

public class DestroyCamera : MonoBehaviour
{
	private void Awake()
	{
		if (ContentLoader.Instance != null)
		{
			Object.Destroy(base.gameObject);
		}
	}
}

using UnityEngine;

[AddComponentMenu("Chimera/DestroyGameObjectOnDisable")]
public class DestroyGameObjectOnDisable : MonoBehaviour
{
	private void OnDisable()
	{
		Object.Destroy(base.gameObject);
	}
}

using UnityEngine;

public class LoadCutscenePrefab : MonoBehaviour
{
	private void Start()
	{
		DIContainerInfrastructure.GetComicCutsceneAssetProvider().InstantiateObject(base.transform.name, base.transform, Vector3.zero, Quaternion.identity, false);
	}
}

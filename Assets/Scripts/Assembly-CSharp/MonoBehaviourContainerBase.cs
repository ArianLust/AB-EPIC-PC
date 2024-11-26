using UnityEngine;

public class MonoBehaviourContainerBase : MonoBehaviour, IMonoBehaviourContainer
{
	public void AddComponentSafely<T>(ref T member) where T : MonoBehaviour
	{
		if (member == null)
		{
			member = base.gameObject.GetComponent<T>();
		}
		if (member == null)
		{
			member = base.gameObject.AddComponent<T>();
		}
	}
}

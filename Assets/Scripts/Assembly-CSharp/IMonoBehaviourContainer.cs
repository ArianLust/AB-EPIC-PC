using UnityEngine;

public interface IMonoBehaviourContainer
{
	void AddComponentSafely<T>(ref T member) where T : MonoBehaviour;
}

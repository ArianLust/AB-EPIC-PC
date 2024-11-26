using UnityEngine;

public class TriggerActionTreeTest : MonoBehaviour
{
	[SerializeField]
	private ActionTree m_ActionTree;

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Q))
		{
			m_ActionTree.Load(m_ActionTree.startNode);
		}
	}
}

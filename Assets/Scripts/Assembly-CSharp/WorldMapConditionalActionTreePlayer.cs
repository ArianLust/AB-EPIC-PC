using System.Collections.Generic;
using UnityEngine;

public class WorldMapConditionalActionTreePlayer : MonoBehaviour
{
	public List<ConditionalActionTree> m_ConditionalActionTrees;

	public void Awake()
	{
		foreach (ConditionalActionTree conditionalActionTree in m_ConditionalActionTrees)
		{
			if (conditionalActionTree.IsActive())
			{
				conditionalActionTree.ActionTree.Load();
			}
		}
	}
}

using System.Collections.Generic;
using UnityEngine;

public class WorldMapConditionalActionTreePlayerOffProgres : MonoBehaviour
{
	public List<ConditionalActionTreeOffProgres> m_ConditionalActionTrees;

	public bool PlayCutscene()
	{
		bool result = false;
		foreach (ConditionalActionTreeOffProgres conditionalActionTree in m_ConditionalActionTrees)
		{
			if (conditionalActionTree.IsActive())
			{
				conditionalActionTree.ActionTree.Load();
				result = true;
			}
		}
		return result;
	}
}

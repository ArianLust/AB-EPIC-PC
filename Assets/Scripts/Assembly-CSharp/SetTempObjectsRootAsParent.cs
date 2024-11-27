using UnityEngine;

public class SetTempObjectsRootAsParent : MonoBehaviour
{
	private void Awake()
	{
		if (base.transform.parent == null)
		{
			base.transform.parent = CoreStateMgr.Instance.GetTempObjectRoot();
		}
	}
}

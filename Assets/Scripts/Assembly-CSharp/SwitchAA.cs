using System.Collections.Generic;
using UnityEngine;

public class SwitchAA : MonoBehaviour
{
	[SerializeField]
	private List<int> m_supportedAATypes = new List<int>();

	private int counter;

	private void Start()
	{
		Object.Destroy(this);
	}
}

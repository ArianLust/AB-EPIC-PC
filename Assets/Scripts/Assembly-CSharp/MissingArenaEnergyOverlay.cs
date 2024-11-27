using UnityEngine;

public class MissingArenaEnergyOverlay : MonoBehaviour
{
	public void ShowGenericOverlay()
	{
		GetComponent<Animation>().Play("InfoOverlay_Enter");
	}

	public void Hide()
	{
		if (base.gameObject.activeInHierarchy)
		{
			GetComponent<Animation>().Play("InfoOverlay_Leave");
			Invoke("Disable", GetComponent<Animation>()["InfoOverlay_Leave"].length);
		}
	}

	private void Disable()
	{
		base.gameObject.SetActive(false);
	}
}

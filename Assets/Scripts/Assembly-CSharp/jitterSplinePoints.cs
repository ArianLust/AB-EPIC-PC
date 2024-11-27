using UnityEngine;

public class jitterSplinePoints : MonoBehaviour
{
	public RageSpline rageSpline;

	private void Start()
	{
		rageSpline = GetComponent<RageSpline>();
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.J))
		{
			for (int i = 0; i < rageSpline.spline.points.Length - 1; i++)
			{
				float num = Random.Range(-0.25f, 0.25f);
				Vector3 position = rageSpline.GetPosition(i);
				rageSpline.SetPoint(i, new Vector3(position.x + num, position.y + num));
			}
		}
		rageSpline.RefreshMesh();
	}
}

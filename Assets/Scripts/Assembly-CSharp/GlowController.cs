using UnityEngine;

public class GlowController : MonoBehaviour
{
	public Mesh SupportMesh;

	public Mesh NeutralMesh;

	public Mesh AttackMesh;

	public void SetStateColor(GlowState state)
	{
		switch (state)
		{
		case GlowState.Support:
		{
			MeshFilter[] componentsInChildren2 = GetComponentsInChildren<MeshFilter>(true);
			foreach (MeshFilter meshFilter2 in componentsInChildren2)
			{
				meshFilter2.mesh = SupportMesh;
			}
			break;
		}
		case GlowState.Attack:
		{
			MeshFilter[] componentsInChildren3 = GetComponentsInChildren<MeshFilter>(true);
			foreach (MeshFilter meshFilter3 in componentsInChildren3)
			{
				meshFilter3.mesh = AttackMesh;
			}
			break;
		}
		case GlowState.Neutral:
		{
			MeshFilter[] componentsInChildren = GetComponentsInChildren<MeshFilter>(true);
			foreach (MeshFilter meshFilter in componentsInChildren)
			{
				meshFilter.mesh = NeutralMesh;
			}
			break;
		}
		}
	}
}

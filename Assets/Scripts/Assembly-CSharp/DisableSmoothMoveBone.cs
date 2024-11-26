using System.Collections;
using SmoothMoves;
using UnityEngine;

public class DisableSmoothMoveBone : MonoBehaviour
{
	[SerializeField]
	private BoneAnimation m_BoneAnimation;

	[SerializeField]
	private string BoneNameToDisable = "MainHand";

	private void OnEnable()
	{
		StartCoroutine(CheckIfBoneAnimationInitializedThenHideBone());
	}

	private IEnumerator CheckIfBoneAnimationInitializedThenHideBone()
	{
		yield return new WaitForEndOfFrame();
		if ((bool)m_BoneAnimation && m_BoneAnimation.mMesh != null)
		{
			m_BoneAnimation.HideBone(BoneNameToDisable, true);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenStateMgr : MonoBehaviour
{
	[SerializeField]
	private List<Animation> m_Animations = new List<Animation>();

	[SerializeField]
	private float m_SplashScreenShowTime = 2f;

	private IEnumerator Start()
	{
		foreach (Animation anim in m_Animations)
		{
			foreach (AnimationState clip2 in anim)
			{
				if (clip2.name.Contains("_Enter") && anim.Play(clip2.name))
				{
					yield return new WaitForSeconds(anim[clip2.name].length);
				}
			}
			yield return new WaitForSeconds(m_SplashScreenShowTime);
			foreach (AnimationState clip in anim)
			{
				if (clip.name.Contains("_Leave") && anim.Play(clip.name))
				{
					yield return new WaitForSeconds(anim[clip.name].length);
				}
			}
		}
	}
}

using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Chimera/TriggerAnimatorByAnimation")]
public class TriggerAnimatorByAnimation : MonoBehaviour
{
	public List<Animator> m_AnimatorsToPlay;

	public void PlayAnimation(string nameOfAnimation)
	{
		string[] array = nameOfAnimation.Split(".".ToCharArray());
		if (array.Length != 2)
		{
			return;
		}
		string text = array[0];
		string stateName = array[1];
		foreach (Animator item in m_AnimatorsToPlay)
		{
			if (string.IsNullOrEmpty(text))
			{
				item.Play(stateName);
			}
			else if (item.name == text)
			{
				item.Play(stateName);
			}
		}
	}
}

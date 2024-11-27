using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Chimera/TriggerAnimationByAnimation")]
public class TriggerAnimationByAnimation : MonoBehaviour
{
	public List<Animation> m_AnimationsToPlay;

	public void PlayAnimation(string nameOfAnimation)
	{
		string[] array = nameOfAnimation.Split(".".ToCharArray());
		if (array.Length != 2)
		{
			return;
		}
		string text = array[0];
		string animation = array[1];
		foreach (Animation item in m_AnimationsToPlay)
		{
			if (!(item == null))
			{
				if (string.IsNullOrEmpty(text))
				{
					item.Play(animation);
				}
				else if (item.name == text)
				{
					item.Play(animation);
				}
			}
		}
	}

	public void StopAnimation(string nameOfAnimation)
	{
		foreach (Animation item in m_AnimationsToPlay)
		{
			if (string.IsNullOrEmpty(nameOfAnimation))
			{
				item.Stop();
			}
			else if (item.name == nameOfAnimation)
			{
				item.Stop();
			}
		}
	}
}

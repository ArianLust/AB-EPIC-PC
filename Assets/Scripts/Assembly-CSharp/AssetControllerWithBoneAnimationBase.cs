using System;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;
using UnityEngine;

public class AssetControllerWithBoneAnimationBase : MonoBehaviour
{
	public BoneAnimation m_BoneAnimation;

	public bool HasNoBoneAnimation { get; private set; }

	private void Awake()
	{
		if (!m_BoneAnimation)
		{
			HasNoBoneAnimation = true;
			m_BoneAnimation = base.gameObject.AddComponent<ProxyBoneAnimation>();
		}
	}

	public IEnumerator WaitThenPlayAnimation(string animationNameToStop, string animationName, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		if (HasNoBoneAnimation)
		{
			base.gameObject.PlayAnimationOrAnimatorState(animationName);
			yield break;
		}
		m_BoneAnimation.Stop(animationNameToStop);
		PlayAnimation(animationName);
	}

	public IEnumerator PlayAnimationAndWaitForFinish(string animationName)
	{
		if (HasNoBoneAnimation)
		{
			yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState(animationName));
			yield break;
		}
		PlayAnimation(animationName);
		yield return new WaitForSeconds(GetAnimationLength(animationName));
	}

	public void PlayAnimationForTime(string animationName, string animationToContinue, float time)
	{
		if (HasNoBoneAnimation)
		{
			base.gameObject.PlayAnimationOrAnimatorState(animationName);
			StartCoroutine(WaitThenPlayAnimation(animationName, animationToContinue, time));
		}
		else
		{
			PlayAnimation(animationName);
			StartCoroutine(WaitThenPlayAnimation(animationName, animationToContinue, time));
		}
	}

	public void PlayAnimationQueued(string animationName)
	{
		try
		{
			if (HasNoBoneAnimation)
			{
				base.gameObject.PlayAnimationOrAnimatorStateQueued(new List<string> { animationName }, this);
			}
			else if (m_BoneAnimation != null && m_BoneAnimation[animationName] != null)
			{
				m_BoneAnimation.PlayQueued(animationName);
			}
			else
			{
				DebugLog.Error("Animation " + animationName + " not found!");
			}
		}
		catch (ArgumentOutOfRangeException)
		{
			DebugLog.Error("Animation " + animationName + " not found!");
		}
	}

	protected void StopAnimation(string animationName)
	{
		try
		{
			if (HasNoBoneAnimation)
			{
				base.gameObject.StopAnimationOrAnimatorState(animationName);
			}
			else if (m_BoneAnimation != null && m_BoneAnimation[animationName] != null)
			{
				m_BoneAnimation.Stop(animationName);
			}
			else
			{
				DebugLog.Error("Animation " + animationName + " not found!");
			}
		}
		catch (ArgumentOutOfRangeException)
		{
			DebugLog.Error("Animation " + animationName + " not found!");
		}
	}

	public bool IsPlayingAnimation(string animationName)
	{
		if (HasNoBoneAnimation)
		{
			return base.gameObject.IsAnimationOrAnimatorPlaying(animationName);
		}
		if (m_BoneAnimation != null)
		{
			return m_BoneAnimation.IsPlaying(animationName);
		}
		return false;
	}

	public bool HasAnimation(string animationName)
	{
		if (HasNoBoneAnimation)
		{
			return base.gameObject.GetAnimationOrAnimatorStateLength(animationName) > 0f;
		}
		if (m_BoneAnimation != null)
		{
			return m_BoneAnimation[animationName] != null;
		}
		return false;
	}

	public void PlayAnimation(string animationName)
	{
		try
		{
			if (HasNoBoneAnimation)
			{
				base.gameObject.PlayAnimationOrAnimatorState(animationName);
			}
			else if (m_BoneAnimation != null && m_BoneAnimation[animationName] != null)
			{
				m_BoneAnimation.Play(animationName);
			}
			else
			{
				DebugLog.Error("Animation " + animationName + " not found!");
			}
		}
		catch (ArgumentOutOfRangeException)
		{
			DebugLog.Error("Animation " + animationName + " not found!");
		}
	}

	public float GetAnimationLength(string animationName)
	{
		try
		{
			if (HasNoBoneAnimation)
			{
				return base.gameObject.GetAnimationOrAnimatorStateLength(animationName);
			}
			float result = 0f;
			if (m_BoneAnimation != null && m_BoneAnimation[animationName] != null)
			{
				result = m_BoneAnimation[animationName].length;
			}
			return result;
		}
		catch (ArgumentOutOfRangeException)
		{
			DebugLog.Error("Animation " + animationName + " not found!");
			return 0f;
		}
	}
}

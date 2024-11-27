using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationExtensions
{
	public static float PlayAnimationOrAnimatorState(this GameObject gameObject, string clipName)
	{
		Animation component = gameObject.GetComponent<Animation>();
		if ((bool)component)
		{
			if (component[clipName] == null)
			{
				DebugLog.Error("[AnimationExtensions] Animation clip not found " + clipName);
				return 0f;
			}
			component.Play(clipName);
			return component[clipName].length;
		}
		Animator component2 = gameObject.GetComponent<Animator>();
		if ((bool)component2)
		{
			AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController();
			animatorOverrideController.runtimeAnimatorController = component2.runtimeAnimatorController;
			AnimatorOverrideController animatorOverrideController2 = animatorOverrideController;
			AnimationClipPair[] clips = animatorOverrideController2.clips;
			foreach (AnimationClipPair animationClipPair in clips)
			{
				if (animationClipPair.originalClip.name == clipName)
				{
					component2.Play(clipName, 0, 0f);
					component2.Update(0f);
					return component2.GetCurrentAnimatorStateInfo(0).length;
				}
			}
		}
		return 0f;
	}

	public static bool IsAnimationOrAnimatorPlaying(this GameObject gameObject, string clipName)
	{
		Animation component = gameObject.GetComponent<Animation>();
		if ((bool)component)
		{
			if (component[clipName] == null)
			{
				return false;
			}
			return component.IsPlaying(clipName);
		}
		Animator component2 = gameObject.GetComponent<Animator>();
		if ((bool)component2)
		{
			component2.Update(0f);
			if (component2.GetCurrentAnimatorStateInfo(0).IsName(clipName) && component2.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f != 0f)
			{
				return true;
			}
		}
		return false;
	}

	public static float GetAnimationOrAnimatorStateLength(this GameObject gameObject, string clipName)
	{
		Animation component = gameObject.GetComponent<Animation>();
		if ((bool)component)
		{
			if (component[clipName] == null)
			{
				DebugLog.Error("[AnimationExtensions] Animation clip not found " + clipName);
				return 0f;
			}
			return component[clipName].length;
		}
		Animator component2 = gameObject.GetComponent<Animator>();
		if ((bool)component2)
		{
			AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController();
			animatorOverrideController.runtimeAnimatorController = component2.runtimeAnimatorController;
			AnimatorOverrideController animatorOverrideController2 = animatorOverrideController;
			AnimationClipPair[] clips = animatorOverrideController2.clips;
			foreach (AnimationClipPair animationClipPair in clips)
			{
				if (animationClipPair.originalClip.name == clipName)
				{
					return animationClipPair.originalClip.length;
				}
			}
			return 0f;
		}
		return 0f;
	}

	public static void StopAnimationOrAnimatorState(this GameObject gameObject, string clipName)
	{
		Animation component = gameObject.GetComponent<Animation>();
		if ((bool)component)
		{
			if (component[clipName] == null)
			{
				DebugLog.Error("[AnimationExtensions] Animation clip not found " + clipName);
			}
			component.Stop(clipName);
			return;
		}
		Animator component2 = gameObject.GetComponent<Animator>();
		if ((bool)component2)
		{
			component2.Play("Idle");
			component2.Update(0f);
		}
	}

	public static void StopAnimationOrAnimatorState(this GameObject gameObject, List<string> clipNameList, MonoBehaviour syncMonoBehaviour)
	{
		Animation component = gameObject.GetComponent<Animation>();
		if ((bool)component)
		{
			foreach (string clipName in clipNameList)
			{
				if (component[clipName] == null)
				{
					DebugLog.Error("[AnimationExtensions] Animation clip not found " + clipName);
				}
				component.Stop(clipName);
			}
			return;
		}
		Animator component2 = gameObject.GetComponent<Animator>();
		if ((bool)component2)
		{
			component2.Update(0f);
			syncMonoBehaviour.StopCoroutine(component2.PlayAnimatorStatesQueued(clipNameList));
		}
	}

	public static void PlayAnimationOrAnimatorStateQueued(this GameObject gameObject, List<string> clipNameList, MonoBehaviour syncMonoBehaviour)
	{
		if ((bool)gameObject.GetComponent<Animation>())
		{
			for (int i = 0; i < clipNameList.Count; i++)
			{
				if (i == 0)
				{
					gameObject.GetComponent<Animation>().Play(clipNameList[i]);
				}
				else
				{
					gameObject.GetComponent<Animation>().PlayQueued(clipNameList[i]);
				}
			}
		}
		else
		{
			Animator component = gameObject.GetComponent<Animator>();
			if ((bool)component)
			{
				syncMonoBehaviour.StartCoroutine(component.PlayAnimatorStatesQueued(clipNameList));
			}
		}
	}

	public static bool HasAnimation(this GameObject gameObject, string clipName)
	{
		Animation component = gameObject.GetComponent<Animation>();
		if ((bool)component)
		{
			if (component[clipName] == null)
			{
				return false;
			}
			return true;
		}
		Animator component2 = gameObject.GetComponent<Animator>();
		if ((bool)component2)
		{
			AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController();
			animatorOverrideController.runtimeAnimatorController = component2.runtimeAnimatorController;
			AnimatorOverrideController animatorOverrideController2 = animatorOverrideController;
			AnimationClipPair[] clips = animatorOverrideController2.clips;
			foreach (AnimationClipPair animationClipPair in clips)
			{
				if (animationClipPair.originalClip.name == clipName)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static IEnumerator PlayAnimatorStatesQueued(this Animator animator, List<string> clipNameList, Action onComplete = null)
	{
		foreach (string clip in clipNameList)
		{
			if (animator == null)
			{
				break;
			}
			if (animator.GetCurrentAnimatorStateInfo(0).IsName("Defeated"))
			{
				yield break;
			}
			animator.Play(clip, 0, 0f);
			animator.Update(0f);
			float length = animator.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(length);
		}
		if (onComplete != null)
		{
			onComplete();
		}
	}

	public static IEnumerator PlayTimeScaleIndependent(this Animation animation, string clipName, bool useTimeScale, Action onComplete)
	{
		if (!useTimeScale)
		{
			AnimationState _currState = animation[clipName];
			bool isPlaying = true;
			float _startTime = 0f;
			float _progressTime = 0f;
			float _timeAtLastFrame2 = 0f;
			float _timeAtCurrentFrame2 = 0f;
			float deltaTime2 = 0f;
			animation.Play(clipName);
			_timeAtLastFrame2 = Time.realtimeSinceStartup;
			while (isPlaying)
			{
				_timeAtCurrentFrame2 = Time.realtimeSinceStartup;
				deltaTime2 = _timeAtCurrentFrame2 - _timeAtLastFrame2;
				_timeAtLastFrame2 = _timeAtCurrentFrame2;
				_progressTime += deltaTime2;
				_currState.normalizedTime = _progressTime / _currState.length;
				animation.Sample();
				if (_progressTime >= _currState.length)
				{
					if (_currState.wrapMode != WrapMode.Loop)
					{
						isPlaying = false;
					}
					else
					{
						_progressTime = 0f;
					}
				}
				yield return new WaitForEndOfFrame();
			}
			yield return null;
			if (onComplete != null)
			{
				onComplete();
			}
		}
		else
		{
			animation.Play(clipName);
		}
	}
}

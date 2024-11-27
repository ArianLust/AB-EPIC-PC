using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class IgnoreTimeScaleAnimation : MonoBehaviour
{
	[SerializeField]
	private string m_clipName;

	private Animation m_animation;

	private float _timeAtLastFrame;

	private float _timeAtCurrentFrame;

	private float deltaTime;

	private float _progressTime;

	private void Awake()
	{
		m_animation = GetComponent<Animation>();
	}

	private void OnDisable()
	{
		StopCoroutine("Play");
	}

	private void OnEnable()
	{
		StartCoroutine("Play");
	}

	private IEnumerator Play()
	{
		while (!m_animation.IsPlaying(m_clipName))
		{
			yield return new WaitForEndOfFrame();
		}
		yield return StartCoroutine(m_animation.PlayTimeScaleIndependent(m_clipName, false, delegate
		{
		}));
		StartCoroutine("Play");
	}
}

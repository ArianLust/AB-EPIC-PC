using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using UnityEngine;

public class CharacterSpeechBubble : MonoBehaviour
{
	public UILabel m_BubbleValue;

	public UISprite m_BubbleIcon;

	public UISprite m_BubbleTargetIcon;

	public GameObject m_BubbleTargetRoot;

	public Animation m_TimeAnimation;

	public Animation m_IdleAnimation;

	public UISprite m_ProgressBar;

	[SerializeField]
	private List<GameObject> m_ChargeProgressDivider = new List<GameObject>();

	[HideInInspector]
	public bool m_IsTargetedBubble;

	public bool m_ReverseFill;

	private int m_Duration;

	private int m_CurrentTurn;

	private Animator m_Animator;

	private Animation m_Animation;

	private void Awake()
	{
		m_Animator = GetComponent<Animator>();
		m_Animation = GetComponent<Animation>();
	}

	public void SetModel(ICharacter iCharacter)
	{
	}

	public void SetModel(ICombatant m_Model, string iconAssetId, int duration)
	{
		if ((bool)m_BubbleIcon && !string.IsNullOrEmpty(iconAssetId))
		{
			m_BubbleIcon.spriteName = iconAssetId;
		}
		m_Duration = duration;
		if ((bool)m_ProgressBar)
		{
			m_ProgressBar.fillAmount = ((!m_ReverseFill) ? 0f : 1f);
		}
		else if ((bool)m_BubbleValue)
		{
			m_BubbleValue.text = duration.ToString("0");
		}
		for (int i = 0; i < m_ChargeProgressDivider.Count; i++)
		{
			if (i == duration - 2)
			{
				m_ChargeProgressDivider[i].SetActive(true);
			}
			else
			{
				m_ChargeProgressDivider[i].SetActive(false);
			}
		}
	}

	public void SetTargetIcon(string targetIconAssetId)
	{
		if ((bool)m_BubbleTargetIcon)
		{
			m_BubbleTargetIcon.spriteName = targetIconAssetId;
		}
	}

	public void UpdateBubbleTurnValue(int turnsLeft)
	{
		if (turnsLeft == 0)
		{
			return;
		}
		if ((bool)m_ProgressBar)
		{
			float fillAmount = m_ProgressBar.fillAmount;
			if (m_ReverseFill)
			{
				StartCoroutine(UpdateTimerAndWarnIfAlmostOver(turnsLeft <= 1, fillAmount, 0f + (float)turnsLeft / (float)m_Duration));
			}
			else
			{
				StartCoroutine(UpdateTimerAndWarnIfAlmostOver(turnsLeft <= 1, fillAmount, 1f - (float)turnsLeft / (float)m_Duration));
			}
		}
		else if ((bool)m_BubbleValue)
		{
			m_BubbleValue.text = turnsLeft.ToString();
			if (m_ReverseFill)
			{
				StartCoroutine(UpdateTimerAndWarnIfAlmostOver(turnsLeft <= 1, 0f, 0f + (float)turnsLeft / (float)m_Duration));
			}
			else
			{
				StartCoroutine(UpdateTimerAndWarnIfAlmostOver(turnsLeft <= 1, 0f, 1f - (float)turnsLeft / (float)m_Duration));
			}
		}
	}

	private IEnumerator UpdateTimerAndWarnIfAlmostOver(bool warn, float oldValue, float newValue)
	{
		yield return StartCoroutine(UpdateProgressBar(oldValue, newValue, UpdateTimer()));
		if (warn)
		{
			PlayTimerWarning();
		}
		else
		{
			m_TimeAnimation.Play("Bubble_Idle");
		}
	}

	private IEnumerator UpdateProgressBar(float oldValue, float newValue, float duration)
	{
		if (!m_ProgressBar)
		{
			yield return new WaitForSeconds(duration);
			yield break;
		}
		for (float timeLeft = duration; timeLeft > 0f; timeLeft -= Time.deltaTime)
		{
			yield return new WaitForEndOfFrame();
			m_ProgressBar.fillAmount = timeLeft / duration * oldValue + (1f - timeLeft / duration) * newValue;
		}
		m_ProgressBar.fillAmount = newValue;
	}

	public float PlayTimerWarning()
	{
		if (m_TimeAnimation != null && (bool)m_TimeAnimation["Bubble_TimerWarning"])
		{
			m_TimeAnimation.Play("Bubble_TimerWarning");
			return m_TimeAnimation["Bubble_TimerWarning"].length;
		}
		return 0f;
	}

	public float PlayEmotion()
	{
		return base.gameObject.PlayAnimationOrAnimatorState("Bubble_PlayEmotion");
	}

	public float ShowAndMinimize()
	{
		return base.gameObject.PlayAnimationOrAnimatorState("Bubble_ShowAndMinimize");
	}

	public float UpdateTimer()
	{
		return base.gameObject.PlayAnimationOrAnimatorState("Bubble_UpdateTimer");
	}

	public float UpdateSkill()
	{
		return base.gameObject.PlayAnimationOrAnimatorState("Bubble_UpdateSkill");
	}

	public float UseSkill()
	{
		if ((bool)m_ProgressBar)
		{
			float fillAmount = m_ProgressBar.fillAmount;
			StartCoroutine(UpdateTimerAndWarnIfAlmostOver(true, fillAmount, 1f));
		}
		float num = base.gameObject.PlayAnimationOrAnimatorState("Bubble_UseSkill");
		if (num == 0f)
		{
			num = base.gameObject.PlayAnimationOrAnimatorState("Bubble_Hide");
		}
		if (num == 0f)
		{
			num = m_Duration;
		}
		return num;
	}

	public void RemoveBubble()
	{
		if (m_TimeAnimation != null)
		{
			m_TimeAnimation.Stop();
		}
		StartCoroutine(RemoveAfterDelay(UseSkill()));
	}

	private IEnumerator RemoveAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		Object.Destroy(base.gameObject);
	}

	private IEnumerator RemoveAfterAnimation()
	{
		float currLength = base.gameObject.GetAnimationOrAnimatorStateLength("Bubble_UseSkill");
		yield return new WaitForSeconds(currLength);
		Object.Destroy(base.gameObject);
	}

	public float Show()
	{
		return base.gameObject.PlayAnimationOrAnimatorState("Bubble_Show");
	}

	public float Hide()
	{
		return base.gameObject.PlayAnimationOrAnimatorState("Bubble_Hide");
	}

	public void DisableTargetIcon(bool disable)
	{
		m_BubbleTargetRoot.gameObject.SetActive(!disable);
	}
}

using UnityEngine;

public class ScrollingCombatText : MonoBehaviour
{
	public UILabel m_LabelHealthChange;

	public UILabel m_LabelText;

	public Animation m_Animation;

	public Color m_PositiveColor;

	public Color m_NegativeColor;

	public Color m_NeutralColor = new Color(0.5f, 0.5f, 0.5f);

	private float m_LiveTime;

	private float m_LiveTimeDone;

	private bool m_InitReady;

	public void ShowHealthChangedWithText(int value, string text, string animationName)
	{
		AnimationClip clip = m_Animation.GetClip(animationName);
		if ((bool)m_LabelText)
		{
			m_LabelText.text = text;
		}
		if ((bool)clip)
		{
			m_Animation.Play(clip.name);
		}
		m_LabelHealthChange.text = value.ToString();
		if (value < 0)
		{
			m_LabelHealthChange.color = m_NegativeColor;
		}
		else if (value > 0)
		{
			m_LabelHealthChange.color = m_PositiveColor;
		}
		else
		{
			m_LabelHealthChange.color = m_NeutralColor;
		}
		m_LabelText.color = m_LabelHealthChange.color;
		m_LiveTime = clip.length;
		m_InitReady = true;
	}

	public void ShowHealthChange(int value)
	{
		m_LabelHealthChange.text = value.ToString();
		if (value < 0)
		{
			m_LabelHealthChange.color = m_NegativeColor;
		}
		else if (value > 0)
		{
			m_LabelHealthChange.color = m_PositiveColor;
		}
		else
		{
			m_LabelHealthChange.color = m_NeutralColor;
		}
		AnimationClip clip = m_Animation.GetClip("CombatText_Damage_Basic");
		m_LiveTime = clip.length;
		m_Animation.Play("CombatText_Damage_Basic");
		m_InitReady = true;
	}

	public void ShowText(string text)
	{
		m_LabelText.text = text;
		AnimationClip clip = m_Animation.GetClip("CombatText_Popup");
		m_LiveTime = clip.length;
		m_Animation.Play("CombatText_Popup");
		m_InitReady = true;
	}

	private void Update()
	{
		if (m_InitReady)
		{
			m_LiveTimeDone += Time.deltaTime;
			if (m_LiveTimeDone > m_LiveTime)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	private void OnDisable()
	{
		Object.Destroy(base.gameObject);
	}
}

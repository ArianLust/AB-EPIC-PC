using UnityEngine;

public class StatisticsElement : MonoBehaviour
{
	[SerializeField]
	private UILabel m_StatValueLabel;

	[SerializeField]
	private UILabel m_StatComparisonValueLabel;

	[SerializeField]
	private GameObject m_StatComparisonRoot;

	[SerializeField]
	private GameObject m_StatComparisonHigher;

	[SerializeField]
	private GameObject m_StatComparisonEqual;

	[SerializeField]
	private GameObject m_StatComparisonLower;

	[SerializeField]
	private UILabel m_StatValueChangeLabel;

	[SerializeField]
	private Animation m_StatValueChangedAnimation;

	[SerializeField]
	private UISprite m_StatSprite;

	[SerializeField]
	private Color m_PositiveColor = new Color(0f, 1f, 0f);

	[SerializeField]
	private Color m_NegativeColor = new Color(1f, 0f, 0f);

	[SerializeField]
	private Color m_NeutralColor = new Color(1f, 1f, 1f);

	public void SetIconSprite(string ident)
	{
		if ((bool)m_StatSprite)
		{
			m_StatSprite.spriteName = ident;
		}
	}

	public void SetValueLabel(float stat)
	{
		if ((bool)m_StatValueLabel)
		{
			m_StatValueLabel.text = stat.ToString("0");
			DebugLog.Log("Stat changed: " + stat);
		}
	}

	public void RefreshStat(bool showChange, bool showComparison, float newValue, float oldValue)
	{
		if ((bool)m_StatValueLabel)
		{
			m_StatValueLabel.text = DIContainerInfrastructure.GetFormatProvider().GetBattleStatsFormat(newValue);
		}
		if ((bool)m_StatValueChangeLabel)
		{
			m_StatValueChangeLabel.gameObject.SetActive(false);
		}
		if ((bool)m_StatComparisonRoot)
		{
			m_StatComparisonRoot.gameObject.SetActive(false);
		}
		if (showChange)
		{
			float num = newValue - oldValue;
			if (num != 0f)
			{
				if ((bool)m_StatValueChangeLabel)
				{
					m_StatValueChangeLabel.gameObject.SetActive(true);
					string text = DIContainerInfrastructure.GetFormatProvider().GetBattleStatsFormat(num);
					if (num > 0f)
					{
						text = "+" + DIContainerInfrastructure.GetFormatProvider().GetBattleStatsFormat(num);
						m_StatValueChangeLabel.color = m_PositiveColor;
					}
					else if (num < 0f)
					{
						m_StatValueChangeLabel.color = m_NegativeColor;
					}
					m_StatValueChangeLabel.text = text;
					if ((bool)m_StatValueChangedAnimation)
					{
						m_StatValueChangedAnimation.Stop();
						m_StatValueChangedAnimation.Play("Display_Stat_ValueChanged");
					}
				}
			}
			else if ((bool)m_StatValueChangeLabel)
			{
				m_StatValueChangeLabel.gameObject.SetActive(false);
			}
		}
		if (!showComparison)
		{
			return;
		}
		float num2 = newValue - oldValue;
		if ((bool)m_StatComparisonRoot)
		{
			m_StatComparisonRoot.gameObject.SetActive(true);
		}
		if ((bool)m_StatComparisonValueLabel)
		{
			string battleStatsFormat = DIContainerInfrastructure.GetFormatProvider().GetBattleStatsFormat(num2);
			if (num2 > 0f)
			{
				m_StatComparisonValueLabel.color = m_PositiveColor;
				m_StatComparisonValueLabel.text = battleStatsFormat;
			}
			else if (num2 < 0f)
			{
				m_StatComparisonValueLabel.color = m_NegativeColor;
				m_StatComparisonValueLabel.text = battleStatsFormat.Replace("-", string.Empty);
			}
			else
			{
				m_StatComparisonValueLabel.text = string.Empty;
			}
		}
		if (num2 > 0f)
		{
			if ((bool)m_StatComparisonHigher)
			{
				m_StatComparisonHigher.SetActive(true);
			}
			if ((bool)m_StatComparisonEqual)
			{
				m_StatComparisonEqual.SetActive(false);
			}
			if ((bool)m_StatComparisonLower)
			{
				m_StatComparisonLower.SetActive(false);
			}
		}
		else if (num2 < 0f)
		{
			if ((bool)m_StatComparisonHigher)
			{
				m_StatComparisonHigher.SetActive(false);
			}
			if ((bool)m_StatComparisonEqual)
			{
				m_StatComparisonEqual.SetActive(false);
			}
			if ((bool)m_StatComparisonLower)
			{
				m_StatComparisonLower.SetActive(true);
			}
		}
		else
		{
			if ((bool)m_StatComparisonHigher)
			{
				m_StatComparisonHigher.SetActive(false);
			}
			if ((bool)m_StatComparisonEqual)
			{
				m_StatComparisonEqual.SetActive(true);
			}
			if ((bool)m_StatComparisonLower)
			{
				m_StatComparisonLower.SetActive(false);
			}
		}
	}
}

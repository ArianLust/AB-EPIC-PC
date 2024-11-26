using UnityEngine;

public class EquipmentComparisonSpeechBubble : MonoBehaviour
{
	public StatisticsElement m_StatisticsComparison;

	public Animation m_ComparisionAnimation;

	public void SetComparisionValues(string statsIconId, float oldValue, float newValue)
	{
		m_StatisticsComparison.SetIconSprite(statsIconId);
		m_StatisticsComparison.RefreshStat(false, true, newValue, oldValue);
		float num = newValue - oldValue;
		if (num > 0f)
		{
			m_ComparisionAnimation.Play("EquipmentComparisonBubble_Higher");
		}
		else if (num < 0f)
		{
			m_ComparisionAnimation.Play("EquipmentComparisonBubble_Lower");
		}
		else
		{
			m_ComparisionAnimation.Play("EquipmentComparisonBubble_Equal");
		}
	}

	public float Show()
	{
		if ((bool)GetComponent<Animation>()["Bubble_Show"])
		{
			GetComponent<Animation>().Play("Bubble_Show");
			return GetComponent<Animation>()["Bubble_Show"].length;
		}
		return 0f;
	}

	public float Hide()
	{
		if ((bool)GetComponent<Animation>()["Bubble_Hide"])
		{
			GetComponent<Animation>().Play("Bubble_Hide");
			return GetComponent<Animation>()["Bubble_Hide"].length;
		}
		return 0f;
	}
}

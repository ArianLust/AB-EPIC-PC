using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldMapMenuButtonStates : MonoBehaviour
{
	[Serializable]
	public struct ButtonPair
	{
		public GameObject m_Marker;

		public int m_Priority;
	}

	[SerializeField]
	private ButtonPair m_seasonEnd;

	[SerializeField]
	private ButtonPair m_turnEnd;

	[SerializeField]
	private ButtonPair m_videoGacha;

	[SerializeField]
	private ButtonPair m_newObjectives;

	[SerializeField]
	private ButtonPair m_friendGacha;

	[SerializeField]
	private ButtonPair m_rainbowRiot;

	[SerializeField]
	private ButtonPair m_updateIndicator;

	private List<ButtonPair> m_allButtons;

	private int m_currentActivePrio;

	private void Awake()
	{
		m_currentActivePrio = int.MaxValue;
		m_allButtons = new List<ButtonPair> { m_seasonEnd, m_turnEnd, m_videoGacha, m_newObjectives, m_friendGacha, m_updateIndicator, m_rainbowRiot };
		foreach (ButtonPair allButton in m_allButtons)
		{
			allButton.m_Marker.SetActive(false);
		}
	}

	public void SetRainbowRiotMarker(bool show)
	{
		ActivateMarker(m_rainbowRiot, show);
	}

	public void SetUpdateMarker(bool show)
	{
		ActivateMarker(m_updateIndicator, show);
	}

	public void SetVideoGachaMarker(bool show)
	{
		// ActivateMarker(m_videoGacha, show); no ad marker because there are literally no ads to be shown
	}

	public void SetFriendGachaMarker(bool show)
	{
		ActivateMarker(m_friendGacha, show);
	}

	public void SetSeasonEndMarker(bool show)
	{
		ActivateMarker(m_seasonEnd, show);
	}

	public void SetTurnEndMarker(bool show)
	{
		ActivateMarker(m_turnEnd, show);
	}

	public void SetObjectiveMarker(bool show)
	{
		ActivateMarker(m_newObjectives, show);
	}

	private void ActivateMarker(ButtonPair buttonPair, bool show)
	{
		if (buttonPair.m_Marker != null && m_currentActivePrio >= buttonPair.m_Priority)
		{
			buttonPair.m_Marker.SetActive(show);
			m_currentActivePrio = ((!show) ? int.MaxValue : buttonPair.m_Priority);
		}
		if (m_allButtons == null)
		{
			return;
		}
		foreach (ButtonPair item in m_allButtons.Where((ButtonPair b) => b.m_Priority > m_currentActivePrio))
		{
			item.m_Marker.SetActive(false);
		}
	}
}

using System.Collections;
using ABH.GameDatas.Battle;
using ABH.Shared.Generic;
using UnityEngine;

public class ScorePopup : MonoBehaviour
{
	[SerializeField]
	private UILabel m_ScoreLabel;

	public void ShowScore(int score, BattleGameData battle, CharacterHealthBar healthBar, Faction faction)
	{
		if (faction == Faction.Birds)
		{
			StartCoroutine(ShowBirdSurvivedScore(score, battle, healthBar));
		}
		else
		{
			StartCoroutine(ShowPigDefeatedScore(score));
		}
	}

	private IEnumerator ShowPigDefeatedScore(int score)
	{
		m_ScoreLabel.text = score.ToString("0");
		GetComponent<Animation>().Play("ScorePopup_Pig");
		yield return new WaitForSeconds(GetComponent<Animation>()["ScorePopup_Pig"].length);
		Object.Destroy(base.gameObject);
	}

	private IEnumerator ShowBirdSurvivedScore(int score, BattleGameData battle, CharacterHealthBar healthBar)
	{
		m_ScoreLabel.text = 0.ToString("0");
		GetComponent<Animation>().Play("ScorePopup_Bird");
		float time = GetComponent<Animation>()["ScorePopup_Pig"].length;
		float timeLeft = time;
		float step = (float)score / time;
		healthBar.EmptyBarOverTime(time);
		while (timeLeft > 0f)
		{
			m_ScoreLabel.text = ((time - timeLeft) * step).ToString("0");
			yield return new WaitForEndOfFrame();
			timeLeft -= Time.deltaTime;
		}
		m_ScoreLabel.text = score.ToString("0");
		yield return new WaitForSeconds(0.5f);
		Object.Destroy(base.gameObject);
	}
}

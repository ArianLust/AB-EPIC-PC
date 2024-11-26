using ABH.GameDatas;
using UnityEngine;

public class DailyObjectiveDetailElement : DailyObjectiveBaseElement
{
	[SerializeField]
	private UILabel m_mainText;

	public override void Init(PvPObjectivesGameData gameData)
	{
		base.Init(gameData);
		if (gameData.Data.Solved)
		{
			GetComponent<Animation>().Play("DailyObjective_Complete");
		}
		else
		{
			GetComponent<Animation>().Play("DailyObjective_Open");
		}
		m_mainText.text = gameData.GetTooltipText();
		if ((bool)m_rewardLabel)
		{
			m_rewardLabel.text = gameData.Reward.ToString();
		}
		m_progressLabel.text = gameData.Data.Progress + "/" + gameData.Amount;
	}
}

using System.Collections;
using ABH.GameDatas;
using UnityEngine;

public class DailyObjectiveBoardElement : DailyObjectiveBaseElement
{
	[SerializeField]
	private UILabel m_objectiveLabel;

	[SerializeField]
	private Animator m_completedAnimation;

	public float Init(PvPObjectivesGameData gameData, float delay)
	{
		base.Init(gameData);
		m_completedAnimation.Play("DailyObjective_SetUnfinished");
		if (gameData.m_SuccessDuringBattle <= 0)
		{
			if (gameData.Data.Solved)
			{
				m_completedAnimation.Play("DailyObjective_SetFinished");
			}
			m_objectiveLabel.text = gameData.Data.Progress + "/" + gameData.Amount;
		}
		else
		{
			m_objectiveLabel.text = gameData.Data.Progress - gameData.m_SuccessDuringBattle + "/" + gameData.Amount;
			m_progressLabel.text = "+" + gameData.m_SuccessDuringBattle.ToString("0");
			m_rewardLabel.text = "+" + gameData.Reward.ToString("0");
			StartCoroutine(UpdateAnimation(gameData, delay));
			delay += 1f;
		}
		return delay;
	}

	private IEnumerator UpdateAnimation(PvPObjectivesGameData gameData, float delay)
	{
		yield return new WaitForSeconds(1f + delay);
		m_objectiveLabel.text = gameData.Data.Progress + "/" + gameData.Amount;
		m_completedAnimation.Play("Update");
		yield return new WaitForSeconds(0.75f);
		if (gameData.Data.Solved)
		{
			m_completedAnimation.Play("Finished");
			yield return new WaitForSeconds(1.75f);
			m_completedAnimation.Play("DailyObjective_SetFinished");
			int ObjectivesSetFinished = 0;
		}
		else
		{
			m_completedAnimation.Play("DailyObjective_SetUnfinished");
		}
		gameData.m_SuccessDuringBattle = 0;
	}

	public void ShowTooltip()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowDailyObjectiveOverlay(base.transform, false);
	}

	public void HideAllTooltips()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideDailyObjectiveOverlay();
	}
}

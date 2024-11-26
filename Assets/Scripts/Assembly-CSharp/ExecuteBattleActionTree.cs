using UnityEngine;

public class ExecuteBattleActionTree : MonoBehaviour
{
	[SerializeField]
	private ActionTree m_ActionTree;

	[SerializeField]
	private string m_BattleID;

	[SerializeField]
	private int m_WaveIndex;

	public bool m_ExecuteBeforeWave;

	public bool m_ExecuteWhenWon = true;

	public bool m_BlockBattleExecution = true;

	public bool m_LetPigsWaitForEnter;

	public bool ShouldExecute(string id, int wave, bool won, bool before)
	{
		if (won)
		{
			return id == m_BattleID && wave == m_WaveIndex && m_ExecuteWhenWon && before == m_ExecuteBeforeWave;
		}
		return id == m_BattleID && !m_ExecuteWhenWon;
	}

	public void StartActionTree()
	{
		if (DIContainerInfrastructure.GetCoreStateMgr().m_DisableStorySequences)
		{
			m_ActionTree.isFinished = true;
			return;
		}
		DebugLog.Error("START ACTION TREE " + base.gameObject.name);
		m_ActionTree.Load(m_ActionTree.startNode);
	}

	public bool IsDone()
	{
		return m_ActionTree.node == null;
	}
}

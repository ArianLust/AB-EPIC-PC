using System;
using UnityEngine;

[Serializable]
public class SceneryPerWaveSetting
{
	public Transform SceneryRoot;

	public SceneSwitchMode SwitchModeToNextWaveBattleground;

	public int RepeatCount;

	public ActionTree OutBattleActionTree;

	public ActionTree InBattleActionTree;
}

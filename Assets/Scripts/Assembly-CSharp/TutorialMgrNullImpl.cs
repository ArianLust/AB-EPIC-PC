using UnityEngine;

public class TutorialMgrNullImpl : MonoBehaviour, ITutorialMgr
{
	public bool IsCurrentlyLocked
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public void ShowHelp(Transform pos, string trigger, float zOffset, float yOffset)
	{
	}

	public void HideHelp()
	{
	}

	public void HideHelp(string trigger, bool finished)
	{
	}

	public void ShowTutorialGuideIfNecessary(string trigger, string param)
	{
	}

	public void StartTutorialStep(string ident)
	{
	}

	public void FinishTutorialStep(string ident)
	{
	}

	public void FinishTutorial(string ident)
	{
	}

	public void FinishWholeTutorial()
	{
	}

	public void SetTutorialCameras(bool activate)
	{
	}

	public void SkipToTutorialStep(string ident, int step, bool forceTrigger)
	{
	}

	public void InitPlayerTutorialTracks()
	{
	}

	public void SkipToTutorialStep(string ident, int step)
	{
	}

	public void ShowFromToHelp(BattleMgrBase battleMgr, Transform sourceRoot, Transform source, Transform target, string trigger, float zOffset)
	{
	}

	public void StepBackOneTutorialStep(string ident)
	{
	}

	public void ResetTutorial(string ident)
	{
	}

	public void StartTutorial(string ident, int startStep = 0)
	{
	}

	public void Remove()
	{
		Object.Destroy(base.gameObject);
	}

	public void FinishActiveTutorials(string ident)
	{
	}
}

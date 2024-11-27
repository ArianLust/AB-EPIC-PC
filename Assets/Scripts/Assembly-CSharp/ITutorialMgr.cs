using UnityEngine;

public interface ITutorialMgr
{
	bool IsCurrentlyLocked { get; set; }

	void HideHelp();

	void HideHelp(string trigger, bool finished);

	void ShowHelp(Transform pos, string trigger, float zOffset, float yOffset = 0f);

	void ShowFromToHelp(BattleMgrBase battleMgr, Transform sourceRoot, Transform source, Transform target, string trigger, float zOffset);

	void ShowTutorialGuideIfNecessary(string trigger, string additionalParameter);

	void StartTutorialStep(string ident);

	void StartTutorial(string ident, int startStep = 0);

	void StepBackOneTutorialStep(string ident);

	void ResetTutorial(string ident);

	void FinishTutorialStep(string ident);

	void FinishTutorial(string ident);

	void FinishWholeTutorial();

	void FinishActiveTutorials(string ident);

	void SetTutorialCameras(bool activate);

	void SkipToTutorialStep(string ident, int step, bool forceTrigger = false);

	void InitPlayerTutorialTracks();

	void Remove();
}

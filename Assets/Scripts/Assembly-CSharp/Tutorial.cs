using System.Collections.Generic;

public class Tutorial
{
	public int TutorialProgress;

	public List<TutorialStepState> TutorialSteps = new List<TutorialStepState>();

	public bool IsFinished
	{
		get
		{
			return TutorialProgress >= TutorialSteps.Count;
		}
	}
}

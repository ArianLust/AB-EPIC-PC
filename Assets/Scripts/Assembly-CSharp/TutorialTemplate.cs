using System.Collections.Generic;

public class TutorialTemplate
{
	public List<TutorialStepTemplate> StepTemplates = new List<TutorialStepTemplate>();

	public bool NonPersistant;

	public bool ForceStartNextTutorials;

	public int NextTutorialStartStep;

	public List<string> FollowUpTutorials = new List<string>();
}

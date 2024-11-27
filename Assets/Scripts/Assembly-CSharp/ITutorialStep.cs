using System.Collections.Generic;

public interface ITutorialStep
{
	bool IsAutoStarted { get; }

	ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart);

	void EvaluateStep(string trigger, List<string> parameters);
}

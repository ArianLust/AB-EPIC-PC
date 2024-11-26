using System.Collections.Generic;

namespace ABH.Tutorial.Steps
{
	public class FinishedBattleTutorialStep : BaseTutorialStep
	{
		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "finished_battle"))
			{
				FinishStep("finished_battle", parameters);
			}
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "finished_battle"))
			{
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
			}
		}
	}
}

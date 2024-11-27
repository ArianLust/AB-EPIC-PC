using System.Collections.Generic;

namespace ABH.Tutorial.Steps
{
	public class EnteredBattleTutorialStep : BaseTutorialStep
	{
		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "entered_battle") && ContainsParameter(parameters))
			{
				FinishStep("entered_battle", parameters);
			}
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "entered_battle") && ContainsParameter(parameters))
			{
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
			}
		}
	}
}

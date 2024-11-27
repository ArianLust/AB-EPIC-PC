using System.Collections.Generic;

namespace ABH.Tutorial.Steps
{
	public class EnteredBattleWaveTutorialStep : BaseTutorialStep
	{
		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_ResetTrigger = "battle_to_worldmap";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void ResetStep()
		{
			base.ResetStep();
			m_TutorialMgr.ResetTutorial(m_TutorialIdent);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "entered_battle_wave") && ContainsParameter(parameters))
			{
				FinishStep("entered_battle_wave", parameters);
			}
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "entered_battle_wave") && ContainsParameter(parameters))
			{
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
			}
		}
	}
}

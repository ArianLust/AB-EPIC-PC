using System.Collections.Generic;

namespace ABH.Tutorial.Steps
{
	public class BaseTutorialStep : ITutorialStep
	{
		protected ITutorialMgr m_TutorialMgr;

		protected string m_AllowedTrigger;

		protected string m_TutorialIdent;

		protected bool m_Paused;

		protected bool m_Started;

		protected string m_StepBackTrigger;

		protected string m_PauseTrigger;

		protected string m_ResumeTrigger;

		protected string m_ResetTrigger;

		protected List<string> m_possibleParams = new List<string>();

		private bool m_autoStarted;

		public bool IsAutoStarted
		{
			get
			{
				return m_autoStarted;
			}
		}

		public virtual ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_TutorialMgr = DIContainerInfrastructure.TutorialMgr;
			m_autoStarted = autoStart;
			m_AllowedTrigger = allowedTrigger;
			m_TutorialIdent = tutorialIdent;
			m_possibleParams = possibleParams;
			return this;
		}

		public void EvaluateStep(string trigger, List<string> parameters)
		{
			if (TryReset(trigger))
			{
				ResetStep();
			}
			else if (TryStepBackStep(trigger))
			{
				StepBackStep();
			}
			else if (TryPause(trigger))
			{
				PauseStep();
			}
			else if (TryResume(trigger))
			{
				ResumeStep();
			}
			else if (!m_Started)
			{
				StartStep(trigger, parameters);
			}
		}

		protected virtual void StartStep(string trigger, List<string> parameters)
		{
		}

		protected virtual void FinishStep(string trigger, List<string> parameters)
		{
		}

		protected bool ContainsParameter(List<string> paramsToCheck)
		{
			foreach (string item in paramsToCheck)
			{
				foreach (string possibleParam in m_possibleParams)
				{
					if (item == possibleParam)
					{
						return true;
					}
				}
			}
			return false;
		}

		protected virtual void PauseStep()
		{
		}

		protected bool TryPause(string trigger)
		{
			if (!m_Started || m_PauseTrigger != trigger || m_Paused)
			{
				return false;
			}
			m_Paused = true;
			return m_Paused;
		}

		protected virtual void ResumeStep()
		{
		}

		protected bool TryResume(string trigger)
		{
			if (!m_Started || m_ResumeTrigger != trigger || !m_Paused)
			{
				return false;
			}
			m_Paused = false;
			return !m_Paused;
		}

		protected virtual void ResetStep()
		{
		}

		protected bool TryReset(string trigger)
		{
			return m_ResetTrigger == trigger;
		}

		protected virtual void StepBackStep()
		{
		}

		protected bool TryStepBackStep(string trigger)
		{
			return m_Started && m_StepBackTrigger == trigger;
		}
	}
}

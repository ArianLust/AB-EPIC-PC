using System.Collections.Generic;

public class DebugEventUI : GenericDebugUI
{
	private bool m_Opened;

	private string m_CurrentName = "unkonwn";

	private List<KeyValuePair<string, uint>> m_FakeBossDefeatsThisSession;

	private string m_progressId = "2";
}

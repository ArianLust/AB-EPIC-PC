public class DebugServerUI : GenericDebugUI
{
	private bool m_Opened;

	private bool m_ShowMemory;

	private float m_MemoryInMB;

	private float m_LastLoadTime;

	private bool m_isLoading;

	private string m_cachedLeaderboardId = string.Empty;

	private string m_cachedPvpLBId = string.Empty;

	public FPSDisplay m_FpsDisplay;
}

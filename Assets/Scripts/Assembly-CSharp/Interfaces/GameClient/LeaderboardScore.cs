namespace Interfaces.GameClient
{
	public class LeaderboardScore
	{
		private uint m_rank;

		private GameScore m_score;

		public uint Rank
		{
			get
			{
				return m_rank;
			}
			internal set
			{
				m_rank = value;
			}
		}

		public GameScore Score
		{
			get
			{
				return m_score;
			}
		}

		internal LeaderboardScore()
		{
			m_score = new GameScore();
		}
	}
}

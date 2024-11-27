using System.Collections.Generic;

namespace Interfaces.GameClient
{
	public class GameScore
	{
		private string m_level;

		private string m_accountId;

		private Dictionary<string, long> m_parameters;

		private Dictionary<string, string> m_metadata;

		public string Level
		{
			get
			{
				return m_level;
			}
			set
			{
				if (m_level != value)
				{
					m_level = value;
				}
			}
		}

		public string AccountId
		{
			get
			{
				return m_accountId;
			}
			internal set
			{
				m_accountId = value;
			}
		}

		public Dictionary<string, long> Parameters
		{
			get
			{
				return m_parameters;
			}
			set
			{
				if (m_parameters != value)
				{
					m_parameters = value;
				}
			}
		}

		public Dictionary<string, string> Metadata
		{
			get
			{
				return m_metadata;
			}
			set
			{
				if (m_metadata != value)
				{
					m_metadata = value;
				}
			}
		}

		public long Points
		{
			get
			{
				long value;
				if (m_parameters.TryGetValue("points", out value))
				{
					return value;
				}
				return 0L;
			}
			set
			{
				m_parameters["points"] = value;
			}
		}

		public GameScore(string level)
		{
			m_parameters = new Dictionary<string, long>();
			m_metadata = new Dictionary<string, string>();
			m_level = level;
		}

		internal GameScore()
		{
			m_parameters = new Dictionary<string, long>();
			m_metadata = new Dictionary<string, string>();
		}
	}
}

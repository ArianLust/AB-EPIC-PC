using System.Collections.Generic;

namespace Interfaces.GameClient
{
	public delegate void GameClientMatchmakingHandler(long transactionId, List<LeaderboardScore> scores);
}

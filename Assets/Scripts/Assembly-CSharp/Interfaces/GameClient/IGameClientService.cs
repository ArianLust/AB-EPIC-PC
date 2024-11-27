namespace Interfaces.GameClient
{
	public interface IGameClientService
	{
		event GameClientSuccessHandler ScoreSent;

		event GameClientErrorHandler ScoreSendError;

		event GameClientMatchmakingHandler MatchMakingFetched;

		event GameClientErrorHandler MatchMakingFetchError;

		event GameClientScoreFetchHandler ScoreFetched;

		event GameClientErrorHandler ScoreFetchError;
	}
}

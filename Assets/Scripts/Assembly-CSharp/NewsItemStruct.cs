using ABH.Shared.BalancingData;
using ABH.Shared.Events.BalancingData;

public struct NewsItemStruct
{
	public string nameId;

	public NewsEventType type;

	public bool isRunning;

	public uint targetTimestamp;

	public EventManagerBalancingData gameplayEventBalancing;

	public BasicShopOfferBalancingData shopOfferBalancing;

	public BonusEventBalancingData bonusEventBalancing;
}

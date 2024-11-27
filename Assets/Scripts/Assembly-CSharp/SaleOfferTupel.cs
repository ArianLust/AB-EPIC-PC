using ABH.Shared.BalancingData;
using ABH.Shared.Models.Generic;

public struct SaleOfferTupel
{
	public SaleItemDetails OfferDetails;

	public SalesManagerBalancingData SaleBalancing;

	public bool IsEmpty()
	{
		return OfferDetails == null || SaleBalancing == null;
	}
}

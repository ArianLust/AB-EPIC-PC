namespace Interfaces.Purchasing
{
	public enum RedeemResponse
	{
		CODE_OK = 0,
		CODE_NOT_FOUND = -1,
		CODE_EXPIRED = -2,
		CODE_MAX_REDEEMS_EXCEEDED = -3,
		CODE_MAX_ACCOUNTS_EXCEEDED = -4,
		CODE_NOT_YET_VALID = -5,
		CODE_RATE_LIMIT_EXCEEDED = -6,
		CODE_PRODUCT_NOT_FOUND = -7,
		CODE_OTHER_ERROR = -10
	}
}

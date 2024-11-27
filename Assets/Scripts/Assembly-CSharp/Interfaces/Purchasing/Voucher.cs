using System.Collections.Generic;

namespace Interfaces.Purchasing
{
	public struct Voucher
	{
		public string voucherId;

		public string productId;

		public bool isConsumable;

		public SourceType sourceType;

		public string sourceId;

		public Dictionary<string, string> clientData;
	}
}

using System.Collections.Generic;

namespace Interfaces.Purchasing
{
	public struct Product
	{
		public string productId;

		public string providerId;

		public string type;

		public string token;

		public string name;

		public string price;

		public float referencePrice;

		public string description;

		public Dictionary<string, string> providerData;

		public Dictionary<string, string> clientData;
	}
}

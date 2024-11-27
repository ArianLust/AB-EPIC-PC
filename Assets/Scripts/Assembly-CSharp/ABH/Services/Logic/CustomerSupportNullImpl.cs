using ABH.Services.Logic.Interfaces;

namespace ABH.Services.Logic
{
	internal class CustomerSupportNullImpl : ICustomerSupportService
	{
		public bool Initialize()
		{
			return true;
		}

		public void ShowRateApp()
		{
		}

		public void ShowRateAppAlways()
		{
		}

		public void ShowHelpCenter()
		{
		}
	}
}

using System.Collections;

namespace Interfaces.Notification
{
	public class RemoteNotificationPlatformIndependent
	{
		public int Id { get; set; }

		public string alertBody { get; set; }

		public int applicationIconBadgeNumber { get; set; }

		public bool hasAction { get; set; }

		public string soundName { get; set; }

		public IDictionary userInfo { get; set; }
	}
}

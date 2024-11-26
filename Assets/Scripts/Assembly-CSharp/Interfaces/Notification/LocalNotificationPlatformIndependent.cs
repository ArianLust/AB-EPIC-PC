using System;
using System.Collections;

namespace Interfaces.Notification
{
	public class LocalNotificationPlatformIndependent
	{
		public int Id { get; set; }

		public string alertAction { get; set; }

		public string alertBody { get; set; }

		public string alertSubheader { get; set; }

		public string alertLaunchImage { get; set; }

		public int applicationIconBadgeNumber { get; set; }

		public DateTime fireDate { get; set; }

		public bool hasAction { get; set; }

		public CalendarIdentifierPlatformIndependent repeatCalendar { get; set; }

		public CalendarUnitPlatformIndependent repeatInterval { get; set; }

		public string soundName { get; set; }

		public string timeZone { get; set; }

		public IDictionary userInfo { get; set; }
	}
}

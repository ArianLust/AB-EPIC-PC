namespace Interfaces.Notification
{
	public enum CalendarUnitPlatformIndependent
	{
		Era = 2,
		Year = 4,
		Month = 8,
		Day = 0x10,
		Hour = 0x20,
		Minute = 0x40,
		Second = 0x80,
		Week = 0x100,
		Weekday = 0x200,
		WeekdayOrdinal = 0x400,
		Quarter = 0x800
	}
}

public class CompatibilityAndroidImpl : ICompatibilityService
{
	public bool isCompatible()
	{
		return true;
	}

	public bool isLowEnd()
	{
		return false;
	}

	public bool isHighEnd()
	{
		return false;
	}
}

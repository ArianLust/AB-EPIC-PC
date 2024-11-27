using UnityEngine;

public class DeviceIdentifierUnityImpl : IDeviceIdentifierService
{
	public string GetAnonymizedDeviceIdentifier()
	{
		return SystemInfo.deviceUniqueIdentifier;
	}
}

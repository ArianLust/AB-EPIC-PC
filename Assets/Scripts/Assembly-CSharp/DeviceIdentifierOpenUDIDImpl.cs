using System;

public class DeviceIdentifierOpenUDIDImpl : IDeviceIdentifierService
{
	public string GetAnonymizedDeviceIdentifier()
	{
		throw new NotSupportedException("No OpenUDID for this platform available!");
	}
}

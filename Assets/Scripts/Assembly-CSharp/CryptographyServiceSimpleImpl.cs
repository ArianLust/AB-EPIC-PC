public class CryptographyServiceSimpleImpl : ICryptographyService
{
	public string EncryptString(string clearString)
	{
		return clearString + "1";
	}

	public string DecryptString(string encryptedString)
	{
		return encryptedString.TrimEnd('1');
	}

	public float EncryptFloat(float clearFloat)
	{
		return clearFloat;
	}

	public float DecryptFloat(float encryptedFloat)
	{
		return encryptedFloat;
	}
}

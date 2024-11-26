public interface ICryptographyService
{
	string EncryptString(string clearString);

	string DecryptString(string encryptedString);

	float EncryptFloat(float clearFloat);

	float DecryptFloat(float encryptedFloat);
}

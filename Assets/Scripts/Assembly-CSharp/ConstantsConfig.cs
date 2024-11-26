public class ConstantsConfig
{
	public string AssetBundleFileExtension
	{
		get
		{
			return ".assetbundle";
		}
	}

	public string AssetBundleCrcMapResourceFileName
	{
		get
		{
			return "assetbundlecrcmap.txt";
		}
	}

	public char AssetBundleCrcMapSeparator
	{
		get
		{
			return '\t';
		}
	}

	public string StoragePublicProfileKey
	{
		get
		{
			return "publicprofile";
		}
	}

	public string StoragePrivateProfileKey
	{
		get
		{
			return "privateprofile";
		}
	}

	public string EncryptionAlgo
	{
		get
		{
			return "DESede/CBC/PKCS7PADDING";
		}
	}

	public string ProfileKey
	{
		get
		{
			return "player";
		}
	}
}

using Chimera.Library.Components.Interfaces;
using UnityEngine;

public class StorageAccessAndroidImpl : IStorageAccessService
{
	private AndroidJavaClass m_storageAccessClass;

	public StorageAccessAndroidImpl()
	{
		m_storageAccessClass = new AndroidJavaClass("de.chimeraentertainment.android.systemtools.StorageAccess");
	}

	public string GetTextFileContentFromSdCard(string fileNamePath)
	{
		return m_storageAccessClass.CallStatic<string>("getTextFileContentFromSdCard", new object[1] { fileNamePath });
	}
}

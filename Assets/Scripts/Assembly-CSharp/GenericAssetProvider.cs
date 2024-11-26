using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.Shared.Models;
using UnityEngine;

public class GenericAssetProvider : MonoBehaviour
{
	private Type BaseType;

	public string PathAddition = string.Empty;

	public Transform CachedInstancesRoot;

	public string BundleURLEditor;

	public bool SelfInitializing = true;

	public AssetBundleMode BundleMode = AssetBundleMode.Prefabs;

	private AssetBundle Bundle;

	public bool InstantiateCopies = true;

	public int EditorBundleVersion;

	public int MaxAmountOfCachedObjects = 10;

	public int MinTimeForAssetUnload = 60;

	public List<EditorAssetInfo> AssetInfos = new List<EditorAssetInfo>();

	public Action<string> ReportError = Debug.Log;

	public readonly Dictionary<string, AssetLoadingInfo> UsedAssets = new Dictionary<string, AssetLoadingInfo>();

	private readonly Dictionary<string, Stack<GameObject>> CachedObjects = new Dictionary<string, Stack<GameObject>>();

	public bool m_Initialized;

	public float m_LoadingProgress = 1f;

	private bool m_Destroyed;

	private bool m_initializing;

	private string DefaultAssetbundlePath
	{
		get
		{
			return "file:///" + Application.dataPath + "/Resources/AssetBundles/" + m_AssetbundleFileName;
		}
	}

	private string DefaultStreamingAssetbundlePath
	{
		get
		{
			return "file:///" + Application.dataPath + "/Resources/AssetBundles/" + m_AssetbundleFileName;
		}
	}

	private string m_AssetbundleFileName
	{
		get
		{
			return DIContainerInfrastructure.GetTargetBuildGroup() + "_" + base.gameObject.name.ToLower() + ((BundleMode != AssetBundleMode.Prefabs) ? ".unity3d" : DIContainerConfig.GetConstants().AssetBundleFileExtension);
		}
	}

	private IEnumerator Start()
	{
		if (SelfInitializing)
		{
			yield return StartCoroutine(InitializeCoroutine());
		}
	}

	public IEnumerator InitializeCoroutine()
	{
		if (m_initializing)
		{
			yield break;
		}
		m_initializing = true;
		AssetInfo assetInfo = GetAssetInfoFor(m_AssetbundleFileName);
		// if (assetInfo != null)
		// {
		// 	if (assetInfo.FileExistsCheck())
		// 	{
		// 		using (WWW www = WWW.LoadFromCacheOrDownload(assetInfo.GetFilePathWithPixedFileTripleSlashes(), assetInfo.AssetVersion))
		// 		{
		// 			while (!www.isDone)
		// 			{
		// 				m_LoadingProgress = www.progress;
		// 				yield return new WaitForSeconds(0.1f);
		// 			}
		// 			Bundle = ((www == null) ? null : www.assetBundle);
		// 			if (Bundle == null)
		// 			{
		// 				DebugLog.Error(assetInfo.FilePath + "could not be loaded " + ((www == null) ? string.Empty : www.error));
		// 			}
		// 			else
		// 			{
		// 				DebugLog.Log("[GenericAssetProvider] loaded bundle: " + m_AssetbundleFileName);
		// 			}
		// 		}
		// 	}
		// }
		// else
		// {
		// 	DebugLog.Log(m_AssetbundleFileName + " does not exist require an AssetBundle so we skipped the AssetBundle loading.");
		// }
		Initialize();
	}

	private AssetInfo GetAssetInfoFor(string assetNamePlusExtension)
	{
		// if (Application.isEditor)
		// {
		// 	string filePath = ((!assetNamePlusExtension.Contains(".unity3d")) ? DefaultAssetbundlePath : DefaultStreamingAssetbundlePath);
		// 	if (assetNamePlusExtension.Contains("assetprovider"))
		// 	{
		// 		AssetInfo assetInfo = new AssetInfo();
		// 		assetInfo.FilePath = filePath;
		// 		assetInfo.Name = assetNamePlusExtension;
		// 		assetInfo.ClientVersion = "1.0.2";
		// 		assetInfo.AssetVersion = EditorBundleVersion;
		// 		return assetInfo;
		// 	}
		// 	return null;
		// }
		return DIContainerInfrastructure.GetAssetData().GetAssetInfoFor(assetNamePlusExtension);
	}

	public List<string> GetAssetsToDeletePaths()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < AssetInfos.Count; i++)
		{
			EditorAssetInfo editorAssetInfo = AssetInfos[i];
			if (editorAssetInfo.AssetLoadingType == LoadingType.FromResources && editorAssetInfo.DeleteOnBuild)
			{
				list.Add(Application.dataPath.ToString() + "/Resources/" + editorAssetInfo.Path);
			}
		}
		return list;
	}

	public void Initialize(bool clear = true)
	{
		if (m_Initialized)
		{
			return;
		}
		UsedAssets.Clear();
		for (int i = 0; i < AssetInfos.Count; i++)
		{
			EditorAssetInfo editorAssetInfo = AssetInfos[i];
			if (editorAssetInfo.DeleteOnBuild)
			{
				continue;
			}
			string key = editorAssetInfo.NameId.ToLower();
			switch (editorAssetInfo.AssetLoadingType)
			{
			case LoadingType.FromResources:
			{
				int startIndex = editorAssetInfo.Path.LastIndexOf("/", StringComparison.Ordinal);
				string loadPath = editorAssetInfo.Path.Insert(startIndex, PathAddition);
				UsedAssets.Add(key, new AssetLoadingInfo
				{
					AssetLoadingType = editorAssetInfo.AssetLoadingType,
					LastRequestTime = DateTime.MinValue,
					loaded = false,
					LoadPath = loadPath
				});
				break;
			}
			case LoadingType.FromMemory:
				if (editorAssetInfo.AssetLink == null)
				{
					DebugLog.Error("Failed no Asset Link set!");
				}
				UsedAssets.Add(key, new AssetLoadingInfo
				{
					AssetLoadingType = editorAssetInfo.AssetLoadingType,
					LastRequestTime = DateTime.MinValue,
					loaded = true,
					Asset = editorAssetInfo.AssetLink
				});
				break;
			case LoadingType.FromBundle:
				UsedAssets.Add(key, new AssetLoadingInfo
				{
					AssetLoadingType = editorAssetInfo.AssetLoadingType,
					LastRequestTime = DateTime.MinValue,
					loaded = false
				});
				break;
			}
		}
		if (clear)
		{
			AssetInfos.Clear();
		}
		m_Initialized = true;
		m_LoadingProgress = 1f;
	}

	public Texture GetTexture(string nameId)
	{
		return GetObject(nameId) as Texture;
	}

	public AudioClip GetAudioClip(string nameId)
	{
		return GetObject(nameId) as AudioClip;
	}

	public GameObject GetGameObject(string nameId)
	{
		return GetObject(nameId) as GameObject;
	}

	public UnityEngine.Object GetObject(string nameId)
	{
		string key = nameId.ToLower();
		AssetLoadingInfo value;
		if (!UsedAssets.TryGetValue(key, out value))
		{
			return null;
		}
		if (value.loaded)
		{
			value.LastRequestTime = DIContainerLogic.GetDeviceTimingService().GetPresentTime();
			if (value.Asset == null)
			{
				value.loaded = false;
			}
			return value.Asset;
		}
		switch (value.AssetLoadingType)
		{
		case LoadingType.FromResources:
			value.Asset = Resources.Load(value.LoadPath);
			break;
		case LoadingType.FromBundle:
			value.Asset = Resources.Load($"IBundles/{m_AssetbundleFileName}/Prefab/{key}");
			if (value.Asset == null)
			{
				value.Asset = Resources.Load($"IBundles/{m_AssetbundleFileName}/{key}");
			}
			break;
		case LoadingType.FromStreamedBundle:
			DebugLog.Error(GetType(), "GetObject: Object " + nameId + " is a streamed scene, cannot be fetched with GetObject!");
			break;
		}
		value.LastRequestTime = DIContainerLogic.GetDeviceTimingService().GetPresentTime();
		value.loaded = true;
		if (value.Asset == null)
		{
			value.loaded = false;
		}
		return value.Asset;
	}

	public void PreCacheObject(string nameId)
	{
		if (!InstantiateCopies)
		{
			return;
		}
		string key = nameId.ToLower();
		if (!CachedObjects.ContainsKey(key))
		{
			CachedObjects.Add(key, new Stack<GameObject>());
		}
		if (SummedCount(CachedObjects[key]) < MaxAmountOfCachedObjects)
		{
			GameObject gameObject = InstantiateObject(nameId, CachedInstancesRoot, Vector3.zero, Quaternion.identity, false);
			if (!CachedObjects[key].Contains(gameObject))
			{
				CachedObjects[key].Push(gameObject);
				gameObject.SetActive(false);
			}
		}
	}

	public void DestroyCachedObjects(string nameId)
	{
		if (!InstantiateCopies)
		{
			return;
		}
		string key = nameId.ToLower();
		if (CachedObjects.ContainsKey(key))
		{
			while (CachedObjects[key].Count > 0)
			{
				UnityEngine.Object.Destroy(CachedObjects[key].Pop());
			}
		}
	}

	public GameObject InstantiateObject(string nameId, Transform parent, Vector3 position, Quaternion rotation, bool fromCache = true)
	{
		if (!InstantiateCopies)
		{
			return null;
		}
		GameObject gameObject = null;
		string text = nameId.ToLower();
		if (fromCache && CachedObjects.Count > 0 && CachedObjects.ContainsKey(text) && CachedObjects[text].Count > 0)
		{
			UnityEngine.Object @object = GetObject(text);
			gameObject = CachedObjects[text].Pop();
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = position;
			gameObject.gameObject.SetActive(true);
		}
		else
		{
			UnityEngine.Object object2 = GetObject(text);
			if (!object2)
			{
				return null;
			}
			gameObject = ((!parent) ? (UnityEngine.Object.Instantiate(object2, position, rotation) as GameObject) : (UnityEngine.Object.Instantiate(object2, parent.position, rotation) as GameObject));
			gameObject.transform.parent = parent;
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
		return gameObject;
	}

	public void DestroyObject(string nameId, GameObject targetObject)
	{
		if (!targetObject || !InstantiateCopies)
		{
			return;
		}
		string key = nameId.ToLower();
		if (!CachedObjects.ContainsKey(key))
		{
			CachedObjects.Add(key, new Stack<GameObject>());
		}
		if (!CachedObjects[key].Contains(targetObject))
		{
			if (SummedCount(CachedObjects[key]) < MaxAmountOfCachedObjects)
			{
				CachedObjects[key].Push(targetObject);
				targetObject.transform.parent = CachedInstancesRoot;
				targetObject.transform.localPosition = Vector3.zero;
				targetObject.transform.localScale = Vector3.one;
				targetObject.SetActive(false);
			}
			else
			{
				UnityEngine.Object.Destroy(targetObject);
			}
		}
	}

	private int SummedCount(Stack<GameObject> CachedObjects)
	{
		return CachedObjects.Count;
	}

	public int GetAssetsToUnloadCount()
	{
		return 0;
	}

	public void RemoveAssetLinks()
	{
		foreach (KeyValuePair<string, AssetLoadingInfo> item in UsedAssets.Where((KeyValuePair<string, AssetLoadingInfo> i) => i.Value.AssetLoadingType != LoadingType.FromMemory && i.Value.loaded && DIContainerLogic.GetTimingService().IsAfter(i.Value.LastRequestTime.AddSeconds(MinTimeForAssetUnload))))
		{
			item.Value.loaded = false;
			item.Value.Asset = null;
			if (CachedObjects.ContainsKey(item.Key.ToLower()))
			{
				int num = 0;
				while (CachedObjects[item.Key.ToLower()].Count > 0)
				{
					UnityEngine.Object.Destroy(CachedObjects[item.Key.ToLower()].Pop());
					num++;
				}
			}
		}
	}

	public bool ContainsAsset(string assetName)
	{
		if (string.IsNullOrEmpty(assetName))
		{
			return false;
		}
		if (AssetInfos == null)
		{
			DebugLog.Log(GetType(), "ContainsAsset: AssetInfos == null");
		}
		return UsedAssets.ContainsKey(assetName.ToLower());
	}

	private void OnDestroy()
	{
		m_Destroyed = true;
		if (CachedObjects == null)
		{
			return;
		}
		foreach (Stack<GameObject> value in CachedObjects.Values)
		{
			foreach (GameObject item in value)
			{
				UnityEngine.Object.Destroy(item);
			}
		}
	}

	public bool UnloadBundle(bool unloadAllLoadedObjects = false)
	{
		if (Bundle == null || !m_Initialized)
		{
			DebugLog.Log(GetType(), "UnloadBundle: Bundle is null or not initialized => nothing to unload!");
			return false;
		}
		DebugLog.Log(GetType(), "UnloadBundle: Bundle undloaded! set initialized to false.");
		m_Initialized = false;
		m_initializing = false;
		Bundle.Unload(unloadAllLoadedObjects);
		return true;
	}
}

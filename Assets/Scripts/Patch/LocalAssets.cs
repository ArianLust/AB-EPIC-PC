
//#define FORCE_WEBREQUEST

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ABH.Shared.Models;
using UnityEngine;
using UnityEngine.Networking;

public static class LocalAssets
{
    private static int GetErrorCode(string reason)
    {
        return reason == "File not found" ? 2 : 1;
    }

    public static void LoadMetadata(SkynestAssets.LoadMetadataSuccessHandler onSuccess, SkynestAssets.LoadMetadataErrorHandler onError)
    {
        OpenStreamingAsset("metadata.txt", (Stream stream) =>
        {
            using (stream)
            {
                onSuccess(DeserializeMetadata(new StreamReader(stream)));
            }
        }, (string reason) =>
        {
            DebugLog.Error("Failed to load metadata: " + reason);
            onError(new string[] { "metadata.txt" }, GetErrorCode(reason));
        }, _ => { });
    }

    public static void Load(string[] files, SkynestAssets.LoadSuccessHandler onSuccess, SkynestAssets.LoadErrorHandler onError, SkynestAssets.LoadProgressHandler onProgress)
    {
        MonoBehaviour mb = DIContainerInfrastructure.GetCoreStateMgr();
		if (mb == null) mb = ContentLoader.Instance;

        if (files.Length <= 0)
        {
            onError(files, 3);
            return;
        }

        mb.StartCoroutine(Load_Coroutine(files, onSuccess, onError, onProgress));
    }

    private static IEnumerator Load_Coroutine(string[] files, SkynestAssets.LoadSuccessHandler onSuccess, SkynestAssets.LoadErrorHandler onError, SkynestAssets.LoadProgressHandler onProgress)
    {
        var loaded = new Dictionary<string, string>();
        var loading = new List<string>(files);

        foreach (var file in files)
        {
            string filePath = GetFilePath(file);
            {
                #if UNITY_ANDROID || UNITY_WEBGL || FORCE_WEBREQUEST
                if (!Directory.Exists(GetFilePath(""))) Directory.CreateDirectory(GetFilePath(""));

                int stat = 0;
                OpenStreamingAsset(file, (Stream stream) =>
                {
                    using (stream)
                    {
                        using (var fs = File.Create(filePath))
                        {
                            stream.CopyTo(fs);
                        }
                    }
                    stat = 1;
                }, (string reason) =>
                {
                    DebugLog.Error($"Failed to download asset: {reason}");
                    onError(loading.ToArray(), GetErrorCode(reason));
                    stat = 2;
                }, (float progress) =>
                {
                    onProgress(loaded, loading.ToArray(), files.Length, loaded.Count + progress); // GC Allocation..?
                });
                while (stat != 1)
                {
                    if (stat == 2)
                    {
                        yield break;
                    }
                    yield return null;
                }
                #else
                if (!File.Exists(filePath))
                {
                    DebugLog.Error("Failed to download asset at path: " + filePath + ". File not found");
                    onError(loading.ToArray(), GetErrorCode("File not found"));
                    yield break;
                }
                #endif
            }

            loaded.Add(file, filePath);
            loading.Remove(file);
            onProgress(loaded, loading.ToArray(), files.Length, loaded.Count);

            yield return null;
        }
        
        onSuccess(loaded);
    }


    private static string GetStreamingAssetPath(string path)
    {
        return Application.streamingAssetsPath + "/local/" + path;
    }

    private static string GetFilePath(string path)
    {
        //return Path.Combine(Application.temporaryCachePath, "downloaded", path);
        #if UNITY_ANDROID || UNITY_WEBGL || FORCE_WEBREQUEST
        return Path.Combine(Application.persistentDataPath, "downloaded", path);
        #else
        return GetStreamingAssetPath(path);
        #endif
    }

    private static void OpenStreamingAsset(string path, Action<Stream> onSuccess, Action<string> onFailed, Action<float> onProgress)
    {
        #if UNITY_ANDROID || UNITY_WEBGL || FORCE_WEBREQUEST
        MonoBehaviour mb = DIContainerInfrastructure.GetCoreStateMgr();
	if (mb == null) mb = ContentLoader.Instance;
        
        // I don't understand why does coroutine cannot be lambda
        IEnumerator Download()
        {
            using (var request = UnityWebRequest.Get(GetStreamingAssetPath(path)))
            {
                request.SendWebRequest();
                
                float oldProgress = 0;
                while (!request.isDone)
                {
                    float newProgress = request.downloadProgress;
                    if (newProgress != oldProgress)
                    {
                        onProgress(newProgress);
                        oldProgress = newProgress;
                    }

                    yield return null;

                    if (!string.IsNullOrEmpty(request.error))
                        break;
                }
                if (!string.IsNullOrEmpty(request.error))
                {
                    if (request.error.Contains("404"))
                    {
                        onFailed("File not found");
                    }
                    else
                    {
                        onFailed(request.error);
                    }
                    yield break;
                }
                yield return null;
                onSuccess(new MemoryStream(request.downloadHandler.data ?? new byte[0]));
            }
        }
        mb.StartCoroutine(Download());
        #else
        FileStream fs;
        try
        {
            fs = File.OpenRead(GetStreamingAssetPath(path));
        }
        catch (FileNotFoundException)
        {
            onFailed("File not found");
            return;
        }
        catch (DirectoryNotFoundException)
        {
            onFailed("File not found");
            return;
        }
        catch (IOException ex)
        {
            onFailed(ex.Message);
            return;
        }
        onProgress(1f);
        onSuccess(fs);
        #endif
    }

    private static Dictionary<string, SkynestAssets.AssetInfo> DeserializeMetadata(TextReader reader)
    {
        var metadata = new Dictionary<string, SkynestAssets.AssetInfo>();

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (line.StartsWith("#") || string.IsNullOrEmpty(line)) continue;

            var split = line.Split(':');

            var info = new SkynestAssets.AssetInfo();
            info.Name = split[0];
            if (split.Length > 1)
            {
                info.Hash = split[1];
            }
            metadata.Add(split[0], info);

            //DebugLog.Log("Loaded file metadata: " + split[0] + (split.Length > 1 ? (" hash:" + split[1]) : ""));
        }

        return metadata;
    }



}

using System;
using System.Collections.Generic;

public class SkynestMetaDataCache
{
    private enum StateEnum
    {
        Ready = 0,
        Initializing = 1,
        Initialized = 2,
        Error = 3
    }

    private Dictionary<string, SkynestAssets.AssetInfo> m_cache = new Dictionary<string, SkynestAssets.AssetInfo>();

    private StateEnum m_state;

    public bool IsInitialized
    {
        get
        {
            return m_state == StateEnum.Initialized;
        }
    }

    public Dictionary<string, SkynestAssets.AssetInfo> AllMetaData
    {
        get
        {
            if (m_state == StateEnum.Initialized)
            {
                return m_cache;
            }
            DebugLog.Warn("[SkynestMetaDataCache]: AllMetaData queried while state == " + m_state);
            return null;
        }
    }

    public void Init(Action<bool> callback)
    {
        if (m_state == StateEnum.Ready)
        {
            DebugLog.Log("[SkynestMetaDataCache]: Initializing");
            m_state = StateEnum.Initializing;
            LocalAssets.LoadMetadata(delegate (Dictionary<string, SkynestAssets.AssetInfo> infos)
            {
                OnInitSuccess(infos, callback);
            }, delegate (string[] files, int errorCode)
            {
                OnInitError(files, errorCode, callback);
            });
        }
    }

    private void OnInitSuccess(Dictionary<string, SkynestAssets.AssetInfo> metaData, Action<bool> callback)
    {
        DebugLog.Log("[SkynestMetaDataCache]: Initialized");
        m_state = StateEnum.Initialized;
        m_cache = metaData;
        callback(true);
    }

    private void OnInitError(string[] files, int errorCode, Action<bool> callback)
    {
        DebugLog.Error("[SkynestMetaDataCache]: Initialization error: " + errorCode);
        m_state = StateEnum.Error;
        callback(false);
    }

    public SkynestAssets.AssetInfo GetSkynestAssetInfoFor(string filename)
    {
        if (m_state == StateEnum.Initialized)
        {
            SkynestAssets.AssetInfo value;
            if (!m_cache.TryGetValue(filename, out value))
            {
                DebugLog.Warn("[SkynestMetaDataCache]: GetSkynestAssetInfoFor(" + filename + ") no asset info available.");
            }
            return value;
        }
        DebugLog.Warn("[SkynestMetaDataCache]: GetSkynestAssetInfoFor(" + filename + ") called while state == " + m_state);
        return default(SkynestAssets.AssetInfo);
    }

    public bool IsAssetInfoAvailable(string filename)
    {
        SkynestAssets.AssetInfo value;
        if (m_state == StateEnum.Initialized)
        {
            return m_cache.TryGetValue(filename, out value);
        }
        return false;
    }
}

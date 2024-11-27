using System;
using UnityEngine;

internal static class DebugLog
{
    private static string m_timestamp
    {
        get
        {
            return DateTime.Now.ToString("HH:mm:ss.ffffff");
        }
    }

    public static void Log(Type tag, string msg)
    {
        Debug.Log(m_timestamp + ": [" + tag.Name + "] " + msg);
    }

    public static void Log(object msg)
    {
        Debug.Log(m_timestamp + ": " + msg);
    }

    public static void ForceLog(Type tag, object msg)
    {
        Debug.Log(m_timestamp + ": [" + tag + "] " + msg);
    }

    public static void ForceWarn(Type tag, object msg)
    {
        Debug.LogWarning(m_timestamp + ": [" + tag + "] " + msg);
    }

    public static void Log(string tag, string msg)
    {
        Debug.Log(m_timestamp + ": [" + tag + "] " + msg);
    }


    public static void Log(Type tag, string msg, LogPlatform platform)
    {
        Debug.Log(m_timestamp + ": [" + tag.Name + "] " + msg);
    }

    public static void Log(string tag, string msg, LogPlatform platform)
    {
        Debug.Log(m_timestamp + ": [" + tag + "] " + msg);
    }

    public static void Error(object msg)
    {
        Debug.LogError(m_timestamp + ": " + msg);
    }

    public static void Error(Type tag, string msg)
    {
        Debug.LogError(m_timestamp + ": [" + tag.Name + "] " + msg);
    }

    public static void Error(string tag, string msg)
    {
        Debug.LogError(m_timestamp + ": [" + tag + "] " + msg);
    }

    public static void Error(Type tag, string msg, LogPlatform platform)
    {
        Debug.LogError(m_timestamp + ": [" + tag.Name + "] " + msg);
    }

    public static void Error(string tag, string msg, LogPlatform platform)
    {
        Debug.LogError(m_timestamp + ": [" + tag + "] " + msg);
    }

    public static void Warn(Type tag, string msg, LogPlatform platform)
    {
        Debug.LogWarning(m_timestamp + ": [" + tag.Name + "] " + msg);
    }

    public static void Warn(string tag, string msg, LogPlatform platform)
    {
        Debug.LogWarning(m_timestamp + ": [" + tag + "] " + msg);
    }

    public static void Warn(Type tag, string msg)
    {
        Debug.LogWarning(m_timestamp + ": [" + tag.Name + "] " + msg);
    }

    public static void Warn(string tag, string msg)
    {
        Debug.LogWarning(m_timestamp + ": [" + tag + "] " + msg);
    }

    public static void Warn(object msg)
    {
        Debug.LogWarning(m_timestamp + ": " + msg);
    }
}

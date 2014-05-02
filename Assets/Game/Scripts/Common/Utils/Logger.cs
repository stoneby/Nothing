using System.Threading;
using UnityEngine;

/// <summary>
/// Logger wrapper of unity log, which will take care of removing log from release build.
/// </summary>
public static class Logger
{
    /// <summary>
    /// Wrapper Debug.Log
    /// </summary>
    /// <param name="message">Message to log</param>
    public static void Log(object message)
    {
        if (Debug.isDebugBuild)
        {
            Debug.Log(message);
        }
    }

    public static void ThreadLog(object message)
    {
        Loom.QueueOnMainThread(() => Log(message));
    }

    /// <summary>
    /// Wrapper Debug.LogWarning.
    /// </summary>
    /// <param name="message">Message to log</param>
    public static void LogWarning(object message)
    {
        if (Debug.isDebugBuild)
        {
            Debug.LogWarning(message);
        }
    }

    public static void ThreadLogWarning(object message)
    {
        Loom.QueueOnMainThread(() => LogWarning(message));
    }

    /// <summary>
    /// Wrapper Debug.LogError
    /// </summary>
    /// <param name="message">Message to log</param>
    public static void LogError(object message)
    {
        if (Debug.isDebugBuild)
        {
            Debug.LogError(message);
        }
    }

    public static void ThreadLogError(object message)
    {
        Loom.QueueOnMainThread(() => LogError(message));
    }
}

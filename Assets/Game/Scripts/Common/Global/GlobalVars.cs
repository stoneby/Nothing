using System.Threading;
using UnityEngine;

public class GlobalVars : Singleton<GlobalVars>
{
    private static int mainThreadId;

    public static bool IsMainThread()
    {
        return (Thread.CurrentThread.ManagedThreadId == mainThreadId);
    }

    private void Start()
    {
        mainThreadId = Thread.CurrentThread.ManagedThreadId;

        Logger.Log("Main thread id: " + mainThreadId);
    }
}

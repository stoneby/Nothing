using System.Threading;
using UnityEngine;

public class GlobalVars : Singleton<GlobalVars>
{
    private static int mainThreadId;

    public static bool IsMainThread()
    {
        return (Thread.CurrentThread.ManagedThreadId == mainThreadId);
    }

    private void Awake()
    {
        mainThreadId = Thread.CurrentThread.ManagedThreadId;

        Debug.Log("Main thread id: " + mainThreadId);
    }
}

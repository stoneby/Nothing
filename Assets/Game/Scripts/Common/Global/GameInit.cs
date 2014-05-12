using System.Collections;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    #region Coroutine

    private static void GameStart(WindowManagerReady e)
    {
        EventManager.Instance.RemoveListener<WindowManagerReady>(GameStart);

        Logger.Log("GameInit started after everything ready, all start later call.");
        WindowManager.Instance.Show(typeof(LoadingWaitWindow), true);
        WindowManager.Instance.Show(typeof(BackgroundFillsWindow), true);
    }

    #endregion

    #region Mono

    void Awake()
    {
        // This is importance in AddListener in Awake, PostEvent() is in Start, keep the sequence.
        EventManager.Instance.AddListener<WindowManagerReady>(GameStart);
    }

    #endregion
}

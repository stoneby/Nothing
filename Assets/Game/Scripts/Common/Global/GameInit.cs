using System.Collections;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    #region Coroutine

    private static void GameStart(WindowManagerReady e)
    {
        EventManager.Instance.RemoveListener<WindowManagerReady>(GameStart);
        
        Debug.Log("GameInit started after everything ready, all start later call.");
        WindowManager.Instance.Show(typeof (LoadingWindow), true);
        WindowManager.Instance.Show(typeof(BackgroundFillsWindow), true);
    }

    #endregion

    #region Mono

    void Awake()
    {
        EventManager.Instance.AddListener<WindowManagerReady>(GameStart);
    }

    #endregion
}

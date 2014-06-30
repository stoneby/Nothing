using UnityEngine;

public class GameInit : MonoBehaviour
{
    public GameObject Parent;
    public GameObject GameInputBox;

    #region Coroutine

    private static void GameStart(WindowManagerReady e)
    {
        EventManager.Instance.RemoveListener<WindowManagerReady>(GameStart);

        Logger.Log("GameInit started after everything ready, all start later call.");
        WindowManager.Instance.Show<LoadingWaitWindow>(true);
    }

    #endregion

    #region Mono

    void Awake()
    {
        // This is importance in AddListener in Awake, PostEvent() is in Start, keep the sequence.
        EventManager.Instance.AddListener<WindowManagerReady>(GameStart);

        var gameInputBox = NGUITools.AddChild(Parent, GameInputBox);
        gameInputBox.transform.localPosition += new Vector3(0, Utils.Root.activeHeight / 2f, 0);
    }

    #endregion
}

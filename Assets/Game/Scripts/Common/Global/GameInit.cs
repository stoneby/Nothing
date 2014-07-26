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
        NetManager.OnMessageSended = OnMessageSended;
        NetworkControl.OnMessageReceived = OnMessageReceived;
    }

    #endregion

    #region Private Methods

    void Awake()
    {
        // This is importance in AddListener in Awake, PostEvent() is in Start, keep the sequence.
        EventManager.Instance.AddListener<WindowManagerReady>(GameStart);

        var gameInputBox = NGUITools.AddChild(Parent, GameInputBox);
        gameInputBox.transform.localPosition += new Vector3(0, Utils.Root.activeHeight / 2f, 0);
    }


    private static void OnMessageReceived()
    {
        WindowManager.Instance.Show<CommunitingWindow>(false);
    }

    private static void OnMessageSended()
    {
        WindowManager.Instance.Show<CommunitingWindow>(true);
    }

    #endregion
}

using UnityEngine;

public class GameInit : MonoBehaviour
{
    public GameObject Parent;
    public GameObject GameInputBox;

    private const float DelayTime = 0.1f;
    private const string ShowDelay = "ShowDelay";

    #region Coroutine

    private static void GameStart(WindowManagerReady e)
    {
        EventManager.Instance.RemoveListener<WindowManagerReady>(GameStart);

        Logger.Log("GameInit started after everything ready, all start later call.");
        WindowManager.Instance.Show<LoginWindow>(true);
        WindowManager.Instance.Show<LoadingWaitWindow>(true);
        NetManager.OnMessageSended = OnMessageSended;
        NetManager.OnMessageReceived = OnMessageReceived;
        NetManager.OnMessageThrowException = OnMessageExeption;
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
        Loom.QueueOnMainThread(() => WindowManager.Instance.Show<LoadingWaitWindow>(false));
        Loom.Instance.RemoveDelay(ShowDelay);
    }

    private static void OnMessageSended(bool playLoadingWait)
    {
        if (playLoadingWait)
        {
            Loom.QueueOnMainThread(() => WindowManager.Instance.Show<LoadingWaitWindow>(true), DelayTime, ShowDelay);
        }
    }

    private static void OnMessageExeption()
    {
        Loom.QueueOnMainThread(() => WindowManager.Instance.Show<LoadingWaitWindow>(false));
        Loom.QueueOnMainThread(() => PingTest.Instance.CheckConnection());
    }

    #endregion
}

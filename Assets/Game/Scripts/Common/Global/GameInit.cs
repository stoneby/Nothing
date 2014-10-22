using UnityEngine;

public class GameInit : MonoBehaviour
{
    public GameObject Parent;
    public GameObject GameInputBox;
    public int TargetFrameRate = 24;

    private const float DelayTime = 0.1f;
    private const string ShowDelay = "ShowDelay";

    private bool isWindowManagerReady;
    private bool isResourcesDownLoaded;
    private bool isRemoteXmlFinish;
    private const float NearlyFullProgress = 0.95f;

    #region Coroutine

    private static void GameStart()
    {
        Logger.Log("GameInit started after everything ready, all start later call.");
        WindowManager.Instance.Show<LoginMainWindow>(true);
        WindowManager.Instance.Show<LoadingWaitWindow>(false);
        NetManager.OnMessageSended = OnMessageSended;
        NetManager.OnMessageReceived = OnMessageReceived;
        NetManager.OnMessageThrowException = OnMessageExeption;
    }

    #endregion

    #region Private Methods

    void Awake()
    {
        Application.targetFrameRate = TargetFrameRate;
        // This is importance in AddListener in Awake, PostEvent() is in Start, keep the sequence.
        EventManager.Instance.AddListener<WindowManagerReady>(OnManagerReady);
        ResoucesManager.Instance.OnResourcesDownLoadStarted += OnResourcesDownLoadStarted;
        ResoucesManager.Instance.OnDownLoadProgessChanged += OnDownLoadProgessChanged;
        ResoucesManager.Instance.OnResourcesDownLoaded += OnResourcesDownLoaded;
        GameConfiguration.Instance.OnParseRomoteXmlFinish += OnRomoteXmlFinish;

        var gameInputBox = NGUITools.AddChild(Parent, GameInputBox);
        gameInputBox.transform.localPosition += new Vector3(0, Utils.Root.activeHeight / 2f, 0);
    }

    private void OnDownLoadProgessChanged(float progess)
    {
        var loadingWait = WindowManager.Instance.GetWindow<LoadingWaitWindow>();
        if (progess >= NearlyFullProgress)
        {
            progess = 1;
        }
        loadingWait.SetProgress("" + (int)(progess * 100) + "%");
    }

    private void OnResourcesDownLoadStarted()
    {
        Debug.Log("=========================================>  OnResourcesDownLoaded ");
        var loadingWait = WindowManager.Instance.Show<LoadingWaitWindow>(true);
        loadingWait.CancelTimeOut();
        loadingWait.ShowProgress();
    }

    private void OnRomoteXmlFinish()
    {
        Debug.Log("=========================================>  OnRomoteXmlFinish ");
        isRemoteXmlFinish = true;
        ExcuteGameStart();
    }

    private void OnResourcesDownLoaded()
    {
        Debug.Log("=========================================>  OnResourcesDownLoaded ");
        WindowManager.Instance.Show<LoadingWaitWindow>(false);
        isResourcesDownLoaded = true;
        ResoucesManager.Instance.OnResourcesDownLoaded -= OnResourcesDownLoaded;
        ExcuteGameStart();
    }

    private void OnManagerReady(WindowManagerReady e)
    {
        isWindowManagerReady = true;
        EventManager.Instance.RemoveListener<WindowManagerReady>(OnManagerReady);
        WindowManager.Instance.Show<LoginWindow>(true);
        WindowManager.Instance.Show<LoadingWaitWindow>(true);
        ExcuteGameStart();
    }

    private void ExcuteGameStart()
    {
        if (isWindowManagerReady && isResourcesDownLoaded && isRemoteXmlFinish)
        {
            GameStart();
        }
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

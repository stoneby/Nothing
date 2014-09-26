using System.Collections;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class LoadingWaitWindow : Window
{
    public float TimeOut = 5f;

    #region Window

    public override void OnEnter()
    {
        Invoke("ExcuteTimeOut", TimeOut);
    }

    public override void OnExit()
    {
        CancelInvoke("ExcuteTimeOut");
    }

    #endregion

    private void ExcuteTimeOut()
    {
        WindowManager.Instance.Show<LoadingWaitWindow>(false);
        PingTest.Instance.CheckConnection();
    }
}

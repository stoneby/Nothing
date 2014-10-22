using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class LoadingWaitWindow : Window
{
    public float TimeOut = 5f;
    public Transform Progress;
    public UILabel ProgressValue;

    #region Window

    public override void OnEnter()
    {
        Invoke("ExcuteTimeOut", TimeOut);
        NGUITools.SetActive(Progress.gameObject, false);
    }

    public override void OnExit()
    {
        CancelTimeOut();
    }

    #endregion

    private void ExcuteTimeOut()
    {
        WindowManager.Instance.Show<LoadingWaitWindow>(false);
        PingTest.Instance.CheckConnection();
    }

    public void CancelTimeOut()
    {
        CancelInvoke("ExcuteTimeOut");
    }

    public void ShowProgress()
    {
        NGUITools.SetActive(Progress.gameObject, true);
    }

    public void SetProgress(string value)
    {
        ProgressValue.text = value;
    }
}

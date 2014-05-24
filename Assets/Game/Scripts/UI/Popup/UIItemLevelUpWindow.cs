using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIItemLevelUpWindow : Window
{
    private UIEventListener backBtnLis;

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Awake()
    {
        backBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Back").gameObject);
    }

    private void InstallHandlers()
    {
        backBtnLis.onClick += OnBackBtnClicked;
    }

    private void UnInstallHandlers()
    {
        backBtnLis.onClick -= OnBackBtnClicked;
    }

    private void OnBackBtnClicked(GameObject go)
    {
        WindowManager.Instance.Show<UIItemInfoWindow>(true);
    }

    #endregion
}

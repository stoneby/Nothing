using System.Linq;
using UnityEngine;

/// <summary>
/// Hero team build window controller.
/// </summary>
public class UITeamBuildWindow : Window
{
    #region private Fields

    private UIEventListener backBtnLis;
    private UIEventListener editBtnLis;

    #endregion

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

    #region Private Methods

    private void Awake()
    {
        backBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Back").gameObject);
        editBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Edit").gameObject);
    }

    private void InstallHandlers()
    {
        backBtnLis.onClick += OnBackBtnClicked;
        editBtnLis.onClick += OnEditBtnClicked;
    }

    private void UnInstallHandlers()
    {
        backBtnLis.onClick -= OnBackBtnClicked;
        editBtnLis.onClick -= OnEditBtnClicked;
    }

    private void OnEditBtnClicked(GameObject go)
    {
        
    }

    private void OnBackBtnClicked(GameObject go)
    {
        WindowManager.Instance.Show(typeof (UITeamBuildWindow), false);
    }

    #endregion
}

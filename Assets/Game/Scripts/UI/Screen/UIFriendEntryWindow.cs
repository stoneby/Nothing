using System.Collections.Generic;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIFriendEntryWindow : Window
{
    #region Public Fields

    public List<UIToggle> Toggles;
    public List<FriendHandlerBase> FriendHandlers;
    public CloseButtonControl CloseButtonControl;

    #endregion

    #region Private Fields

    private UILabel friendCount;

    #endregion

    #region Public Methods

    public void RefreshFriendCount()
    {
        var scFriendLoadingAll = FriendModelLocator.Instance.ScFriendLoadingAll;
        friendCount.text = string.Format("{0}/{1}", scFriendLoadingAll.FriendList.Count, scFriendLoadingAll.FriendLimit);
    }

    #endregion

    #region Private Methods

    private void InstallHandlers()
    {
        CloseButtonControl.OnCloseWindow = OnClose;
    }

    private void UnInstallHandlers()
    {
        CloseButtonControl.OnCloseWindow = null;
    }

    private void OnClose()
    {
        WindowManager.Instance.Show<UIMainScreenWindow>(true);
    }

    #endregion

    #region Window

    public override void OnEnter()
    {
        GlobalWindowSoundController.Instance.PlayOpenSound();
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        WindowManager.Instance.Show<UIMainScreenWindow>(true);
    }

    private void Awake()
    {
        friendCount = transform.Find("TopTitle/FriendCount/FriendCountValue").GetComponent<UILabel>();
    }

    #endregion
}

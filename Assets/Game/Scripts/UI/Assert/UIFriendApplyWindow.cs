using Property;
using UnityEngine;
using KXSGCodec;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIFriendApplyWindow : Window
{
    private UIEventListener closeLis;
    private UIEventListener applyLis;
    private UIEventListener viewDetailLis;
    private FriendItem friendItem;

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
    private void Awake()
    {
        closeLis = UIEventListener.Get(transform.Find("Buttons/Button-Close").gameObject);
        applyLis = UIEventListener.Get(transform.Find("Buttons/Button-Apply").gameObject);
        viewDetailLis = UIEventListener.Get(transform.Find("Buttons/Button-ViewDetail").gameObject);
        friendItem = GetComponent<FriendItem>();
    }

    private void InstallHandlers()
    {
        closeLis.onClick = OnClose;
        applyLis.onClick = OnApply;
        viewDetailLis.onClick = OnViewDetail;

    }

    private void UnInstallHandlers()
    {
        closeLis.onClick = null;
        applyLis.onClick = null;
        viewDetailLis.onClick = null;
    }


    private void OnClose(GameObject go)
    {
        WindowManager.Instance.Show<UIFriendApplyWindow>(false);
    }

    private void OnApply(GameObject go)
    {
        var msg = new CSFriendApply { FriendUuid = friendItem.FriendId};
        NetManager.SendMessage(msg);
        WindowManager.Instance.Show<UIFriendApplyWindow>(false);
    }

    private void OnViewDetail(GameObject go)
    {
        WindowManager.Instance.Show<UIFriendApplyWindow>(false);
    }

    public void Refresh(FriendInfo info)
    {
        friendItem.Init(info);
    }

    #endregion
}

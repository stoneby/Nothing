using System.Linq;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIFriendLockWindow : Window
{
    private UIEventListener closeLis;
    private UIEventListener lockLis;
    private UIEventListener delLis;
    private UIEventListener viewDetailLis;
    private FriendItem friendItem;

    //The reference of the friend item which trigger the current window.
    private FriendItem sourceItem;

    private UILabel lockLabel;
    private bool isLocked;

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
        lockLis = UIEventListener.Get(transform.Find("Buttons/Button-Lock").gameObject);
        delLis = UIEventListener.Get(transform.Find("Buttons/Button-Del").gameObject);
        viewDetailLis = UIEventListener.Get(transform.Find("Buttons/Button-ViewDetail").gameObject);
        friendItem = GetComponent<FriendItem>();
        lockLabel = lockLis.GetComponentInChildren<UILabel>();
    }

    private void InstallHandlers()
    {
        closeLis.onClick = OnClose;
        lockLis.onClick = OnLock;
        delLis.onClick = OnDel;
        viewDetailLis.onClick = OnViewDetail;
    }

    private void UnInstallHandlers()
    {
        closeLis.onClick = null;
        lockLis.onClick = null;
        delLis.onClick = null;
        viewDetailLis.onClick = null;
    }

    private void OnViewDetail(GameObject go)
    {
        WindowManager.Instance.Show<UIFriendApplyWindow>(false);
        var detail = WindowManager.Instance.Show<UIFriendDetailWindow>(true);
        detail.Init(friendItem.FriendInfo);
    }

    private void OnDel(GameObject go)
    {
        ShowDelAssert();
    }

    private void OnLock(GameObject go)
    {
        var bindType = isLocked ? FriendConstant.FriendUnBind : FriendConstant.FriendBind;
        var msg = new CSFriendBind { FriendUuid = friendItem.FriendInfo.FriendUuid, BindType = bindType };
        NetManager.SendMessage(msg);
    }

    private void OnClose(GameObject go)
    {
        WindowManager.Instance.Show<UIFriendLockWindow>(false);
    }

    #endregion

    public void Refresh(FriendInfo info, FriendItem source)
    {
        sourceItem = source;
        friendItem.Init(info);
        isLocked = FriendUtils.IsFriendBind(info);
        lockLabel.text = isLocked
                             ? LanguageManager.Instance.GetTextValue("UIFriendLock.UnLock")
                             : LanguageManager.Instance.GetTextValue("UIFriendLock.Lock");
        delLis.GetComponent<UIButton>().isEnabled = !isLocked;
    }

    public void ShowLockSucc(FriendInfo info)
    {
        sourceItem.Init(info);
        WindowManager.Instance.Show<UIFriendLockWindow>(false);
    }

    public void ShowDelSucc()
    {
        if (PoolManager.Pools.ContainsKey("FriendRelated"))
        {
            var grid = sourceItem.transform.parent.GetComponent<UIGrid>();
            sourceItem.transform.parent = PoolManager.Pools["FriendRelated"].transform;
            PoolManager.Pools["FriendRelated"].Despawn(sourceItem.transform);
            grid.repositionNow = true;
        }
        WindowManager.Instance.Show<UIFriendLockWindow>(false);
    }

    private void ShowDelAssert()
    {
        var assertWindow = WindowManager.Instance.GetWindow<AssertionWindow>();
        assertWindow.Title = LanguageManager.Instance.GetTextValue("UIFriendLock.DelFriendAssert") + friendItem.FriendInfo.FriendName + "?";
        assertWindow.Message = "";
        assertWindow.AssertType = AssertionWindow.Type.OkCancel;
        assertWindow.OkButtonClicked += OnDelOkPressed;
        WindowManager.Instance.Show<AssertionWindow>(true);
    }

    private void OnDelOkPressed(GameObject sender)
    {
        var assertWindow = WindowManager.Instance.GetWindow<AssertionWindow>();
        assertWindow.OkButtonClicked -= OnDelOkPressed;
        var msg = new CSFriendDelete { FriendUuid = friendItem.FriendInfo.FriendUuid };
        NetManager.SendMessage(msg);
    }
}

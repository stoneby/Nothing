using KXSGCodec;

public class ApplyItem : FriendItem
{
    #region Private Fields

    private UILabel loginLbl;
    private UIButton rejectBtn;
    private UIButton agreeBtn;

    private UIEventListener rejectLis;
    private UIEventListener agreeLis;

    #endregion

    public void Init(FriendInfo info, UIEventListener.VoidDelegate rejectDel, UIEventListener.VoidDelegate agreeDel)
    {
        base.Init(info);
        var loginDateTime = Utils.ConvertFromJavaTimestamp(info.LastLoginTime);
        loginLbl.text = Utils.GetTimeUntilNow(loginDateTime);
        rejectLis.onClick = rejectDel;
        agreeLis.onClick = agreeDel;
    }

    protected override void Awake()
    {
        base.Awake();
        loginLbl = transform.Find("LoginTime/LoginTimeValue").GetComponent<UILabel>();
        rejectBtn = transform.Find("Reject").GetComponent<UIButton>();
        agreeBtn = transform.Find("Agree").GetComponent<UIButton>();
        rejectLis = UIEventListener.Get(rejectBtn.gameObject);
        agreeLis = UIEventListener.Get(agreeBtn.gameObject);
    }
}

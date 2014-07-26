using System;
using KXSGCodec;

public class GivenItem : FriendItem
{
    private UILabel loginLbl;
    private UIButton givenBtn;

    private UIEventListener givenLis;

    protected override void Awake()
    {
        base.Awake();
        loginLbl = transform.Find("LoginTime/LoginTimeValue").GetComponent<UILabel>();
        givenBtn = transform.Find("GivenBtn").GetComponent<UIButton>();
        givenLis = UIEventListener.Get(givenBtn.gameObject);
    }

    public void Init(FriendInfo info, UIEventListener.VoidDelegate dDelegate)
    {
        base.Init(info);
        var loginDateTime = Utils.ConvertFromJavaTimestamp(info.LastLoginTime);
        loginLbl.text = Utils.GetTimeUntilNow(loginDateTime);
        var givenTime = Utils.ConvertFromJavaTimestamp(info.GiveEnergyTime);
        givenBtn.isEnabled = !Utils.IsSameDay(givenTime, DateTime.Today);
        givenLis.onClick = dDelegate;
    }
}

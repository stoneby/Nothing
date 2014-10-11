using System;
using KXSGCodec;
using UnityEngine;

public class GivenItem : FriendItem
{
    #region Private Fields

    private UILabel loginLbl;
    private UIButton givenBtn;
    private UIButton givenBtnSprite;
    private UIEventListener givenLis;
    private UIEventListener delLis;
    private UIEventListener.VoidDelegate delegateCached;

    #endregion

    public TweenRotation TweenRotation;

    public void Init(FriendInfo info, UIEventListener.VoidDelegate dDelegate)
    {
        base.Init(info);
        var loginDateTime = Utils.ConvertFromJavaTimestamp(info.LastLoginTime);
        loginLbl.text = Utils.GetTimeUntilNow(loginDateTime);
        var givenTime = Utils.ConvertFromJavaTimestamp(info.GiveEnergyTime);
        givenBtnSprite.isEnabled = givenBtn.isEnabled = !Utils.IsSameDay(givenTime, DateTime.Today);
        givenLis.onClick = dDelegate;
        delegateCached = dDelegate;
    }

    protected override void Awake()
    {
        base.Awake();
        loginLbl = transform.Find("LoginTime/LoginTimeValue").GetComponent<UILabel>();
        givenBtn = transform.Find("GivenBtn").GetComponent<UIButton>();
        givenBtnSprite = Utils.FindChild(transform, "GivenBtnSprite").GetComponent<UIButton>();
        givenLis = UIEventListener.Get(givenBtn.gameObject);
        delLis = UIEventListener.Get(transform.Find("Button-Del").gameObject);
        ShowDel(false);
    }

    private void OnDel(GameObject go)
    {
        var csmsg = new CSFriendDelete {FriendUuid = FriendInfo.FriendUuid};
        NetManager.SendMessage(csmsg);
    }

    public void Play(bool play)
    {
        if (play)
        {
            TweenRotation.PlayForward();
        }
        else
        {
            TweenRotation.enabled = false;
            TweenRotation.transform.localRotation = Quaternion.identity;
        }
    }

    public void ShowDel(bool show)
    {
        NGUITools.SetActive(delLis.gameObject, show);
        if (show)
        {
            givenLis.onClick = null;
            delLis.onClick = OnDel;
        }
        else
        {
            givenLis.onClick = delegateCached;
            delLis.onClick = null;
        }
    }

    private void OnDisable()
    {
        ShowDel(false);
        Play(false);
    }
}

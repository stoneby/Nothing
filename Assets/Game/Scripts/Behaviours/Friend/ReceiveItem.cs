﻿using KXSGCodec;

public class ReceiveItem : FriendItem
{
    #region Private Fields

    private UILabel givenTime;
    private UIButton receiveBtn;
    private UIButton receiveBtnSprite;
    private UIEventListener receiveLis;

    #endregion

    public void Init(RecieveEnergyInfo energyInfo, UIEventListener.VoidDelegate dDelegate)
    {
        givenTime.text = Utils.GetTimeUntilNow(energyInfo.GiveEnergyTime);
        receiveBtnSprite.isEnabled = receiveBtn.isEnabled = !energyInfo.RecieveStatus;
        receiveLis.onClick = dDelegate;
        var friendInfo = FriendModelLocator.Instance.FindInfo(energyInfo.FriendUuid);
        Init(friendInfo);
    }

    protected override void Awake()
    {
        base.Awake();
        givenTime = transform.FindChild("GiveTime/GiveTimeValue").GetComponent<UILabel>();
        receiveBtn = transform.Find("ReceiveBtn").GetComponent<UIButton>();
        receiveBtnSprite = Utils.FindChild(transform, "ReceiveBtnSprite").GetComponent<UIButton>();
        receiveLis = UIEventListener.Get(receiveBtn.gameObject);
    }
}

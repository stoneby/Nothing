using KXSGCodec;

public class ReceiveItem : FriendItem
{
    private UILabel givenTime;
    private UIButton receiveBtn;
    private UIEventListener receiveLis;

    protected override void Awake()
    {
        base.Awake();
        givenTime = transform.FindChild("GiveTime/GiveTimeValue").GetComponent<UILabel>();
        receiveBtn = transform.Find("ReceiveBtn").GetComponent<UIButton>();
        receiveLis = UIEventListener.Get(receiveBtn.gameObject);
    }

    public void Init(RecieveEnergyInfo energyInfo, UIEventListener.VoidDelegate dDelegate)
    {
        givenTime.text = Utils.GetTimeUntilNow(energyInfo.GiveEnergyTime);
        receiveBtn.isEnabled = !energyInfo.RecieveStatus;
        receiveLis.onClick = dDelegate;
        var friendInfo = FriendModelLocator.Instance.FindInfo(energyInfo.FriendUuid);
        Init(friendInfo);
    }
}

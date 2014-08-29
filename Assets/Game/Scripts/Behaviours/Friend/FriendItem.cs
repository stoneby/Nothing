using Property;
using UnityEngine;
using KXSGCodec;

public class FriendItem : MonoBehaviour
{
    public FriendInfo FriendInfo { get; private set; }

    private UILabel nameLbl;
    private UILabel atkLbl;
    private UILabel hpLbl;
    private UILabel lvlLbl;
    private UISprite bindIcon;

    protected NGUILongPress longPress;

    protected virtual void Awake()
    {
        nameLbl = transform.Find("Name").GetComponent<UILabel>();
        atkLbl = transform.Find("Attack/AtkValue").GetComponent<UILabel>();
        hpLbl = transform.Find("Hp/HpValue").GetComponent<UILabel>();
        lvlLbl = transform.Find("Level/LevelValue").GetComponent<UILabel>();
        longPress = transform.Find("BG").GetComponent<NGUILongPress>();
        var bindIconTran = Utils.FindChild(transform,"BindIcon");
        if (bindIconTran)
        {
            bindIcon = bindIconTran.GetComponent<UISprite>();
        }
    }

    public virtual void Init(FriendInfo info)
    {
        FriendInfo = info;
        nameLbl.text = info.FriendName;
        atkLbl.text = FriendUtils.GetProp(info, RoleProperties.ROLE_ATK).ToString();
        hpLbl.text = FriendUtils.GetProp(info, RoleProperties.ROLE_HP).ToString();
        lvlLbl.text = info.FriendLvl.ToString();
        if (bindIcon)
        {
            bindIcon.enabled = FriendUtils.IsFriendBind(info);
        }
        if(longPress)
        {
            longPress.OnLongPress = OnLongPress;
            longPress.OnNormalPress = OnNormalPress;
        }
    }

    private void OnNormalPress(GameObject go)
    {
        var friendLock = WindowManager.Instance.Show<UIFriendLockWindow>(true);
        if (friendLock != null)
        {
            friendLock.Refresh(FriendInfo, this);
        }
    }

    private void OnLongPress(GameObject go)
    {
        var friendDetail = WindowManager.Instance.Show<UIFriendDetailWindow>(true);
        if(friendDetail != null)
        {
            friendDetail.Init(FriendInfo);
        }
    }
}

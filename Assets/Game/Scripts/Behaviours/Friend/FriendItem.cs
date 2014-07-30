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

    protected NGUILongPress longPress;

    protected virtual void Awake()
    {
        nameLbl = transform.Find("Name").GetComponent<UILabel>();
        atkLbl = transform.Find("Attack/AtkValue").GetComponent<UILabel>();
        hpLbl = transform.Find("Hp/HpValue").GetComponent<UILabel>();
        lvlLbl = transform.Find("Level/LevelValue").GetComponent<UILabel>();
        longPress = transform.Find("BG").GetComponent<NGUILongPress>();
    }

    public virtual void Init(FriendInfo info)
    {
        FriendInfo = info;
        nameLbl.text = info.FriendName;
        atkLbl.text = FriendUtils.GetProp(info, RoleProperties.ROLE_ATK).ToString();
        hpLbl.text = FriendUtils.GetProp(info, RoleProperties.ROLE_HP).ToString();
        lvlLbl.text = info.FriendLvl.ToString();
        if(longPress)
        {
            longPress.OnLongPress = OnLongPress;
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

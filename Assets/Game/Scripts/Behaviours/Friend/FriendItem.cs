using Property;
using UnityEngine;
using KXSGCodec;

public class FriendItem : MonoBehaviour
{
    public long FriendId { get; private set; }

    private UILabel nameLbl;
    private UILabel atkLbl;
    private UILabel hpLbl;
    private UILabel lvlLbl;

    protected virtual void Awake()
    {
        nameLbl = transform.Find("Name").GetComponent<UILabel>();
        atkLbl = transform.Find("Attack/AtkValue").GetComponent<UILabel>();
        hpLbl = transform.Find("Hp/HpValue").GetComponent<UILabel>();
        lvlLbl = transform.Find("Level/LevelValue").GetComponent<UILabel>();
    }

    public virtual void Init(FriendInfo info)
    {
        FriendId = info.FriendUuid;
        nameLbl.text = info.FriendName;
        atkLbl.text = FriendUtils.GetProp(info, RoleProperties.ROLE_ATK).ToString();
        hpLbl.text = FriendUtils.GetProp(info, RoleProperties.ROLE_HP).ToString();
        lvlLbl.text = info.FriendLvl.ToString();
    }
}

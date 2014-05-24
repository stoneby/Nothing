using KXSGCodec;
using Property;
using UnityEngine;
using System.Collections;

public class FriendItemControl : MonoBehaviour
{
    private FriendInfo FriendData;
    private bool IsFriend;

    private GameObject AttrackLabel;
    private GameObject NameLabel;
    private GameObject MqLabel;
    private GameObject FriendSprite;

    private int AtkValue;
    private int HpValue;

    private UIEventListener BtnClickUIEventListener;
    

	// Use this for initialization
	void Start () 
    {
        AttrackLabel = transform.FindChild("Attrack Label").gameObject;
        NameLabel = transform.FindChild("Name Label").gameObject;
        MqLabel = transform.FindChild("Mq Label").gameObject;
        FriendSprite = transform.FindChild("Friend Sprite").gameObject;
        BtnClickUIEventListener = UIEventListener.Get(gameObject);
        BtnClickUIEventListener.onClick += OnItemClick;

	    ShowData();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Init(FriendInfo friend, bool isfriend)
    {
        FriendData = friend;
        IsFriend = isfriend;
        AtkValue = 0;
        HpValue = 0;

        for (int i = 0; i < FriendData.HeroProp.Count; i++)
        {
            AtkValue += FriendData.HeroProp[i].Prop[RoleProperties.HERO_ATK];
            HpValue += FriendData.HeroProp[i].Prop[RoleProperties.HERO_HP];
        }
        ShowData();
        
    }

    private void ShowData()
    {
        if (AttrackLabel == null) return;
        var lb = AttrackLabel.GetComponent<UILabel>();
        lb.text = "攻 " + AtkValue + "    " + "Hp " + HpValue;

        lb = NameLabel.GetComponent<UILabel>();
        lb.text = FriendData.FriendName + "    " + "Lv " + FriendData.FriendLvl;

        lb = MqLabel.GetComponent<UILabel>();
        lb.text = "可获得名气值 " + FriendData.FriendFamous + "点";

        var sp = FriendSprite.GetComponent<UISprite>();
        if (IsFriend)
        {
            sp.spriteName = "friend";
        }
        else
        {
            sp.spriteName = "guest";
        }
    }

    private void OnItemClick(GameObject game)
    {
        var e = new FriendClickEvent();
        e.FriendData = FriendData;
        e.IsFriend = IsFriend;
        EventManager.Instance.Post(e);


    }
}

using KXSGCodec;
using Property;
using UnityEngine;
using System.Collections;

public class FriendItemControl : KxItemRender
{
    public FriendVO FriendData;

    private GameObject AttrackLabel;
    private GameObject NameLabel;
    private GameObject MqLabel;
    private GameObject FriendSprite;

    private int AtkValue;
    private int HpValue;

    //private UIEventListener BtnClickUIEventListener;
    

	// Use this for initialization
	void Start () 
    {
        
        AttrackLabel = transform.FindChild("Attrack Label").gameObject;
        NameLabel = transform.FindChild("Name Label").gameObject;
        MqLabel = transform.FindChild("Mq Label").gameObject;
        FriendSprite = transform.FindChild("Friend Sprite").gameObject;
        

	    ShowData();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void SetData<T>(T data)
    {
        //throw new System.NotImplementedException();
        FriendData = data as FriendVO;
        Init(FriendData);
    }

    public void Init(FriendVO friend)
    {
        FriendData = friend;
        AtkValue = 0;
        HpValue = 0;

        for (int i = 0; i < FriendData.Data.HeroProp.Count; i++)
        {
            AtkValue += FriendData.Data.HeroProp[i].Prop[RoleProperties.HERO_ATK];
            HpValue += FriendData.Data.HeroProp[i].Prop[RoleProperties.HERO_HP];
        }
        ShowData();
        
    }

    private void ShowData()
    {
        if (AttrackLabel == null) return;
        var lb = AttrackLabel.GetComponent<UILabel>();
        lb.text = "攻 " + AtkValue + "    " + "Hp " + HpValue;

        lb = NameLabel.GetComponent<UILabel>();
        lb.text = FriendData.Data.FriendName + "    " + "Lv " + FriendData.Data.FriendLvl;

        lb = MqLabel.GetComponent<UILabel>();
        lb.text = "可获得名气值 " + FriendData.Data.FriendFamous + "点";

        var sp = FriendSprite.GetComponent<UISprite>();
        if (FriendData.IsFriend)
        {
            sp.spriteName = "friend";
        }
        else
        {
            sp.spriteName = "guest";
        }
    }

    
}

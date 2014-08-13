using Property;
using UnityEngine;
using System.Collections;

public class FriendHeadControl: KxItemRender {
    public FriendVO FriendData;

    private GameObject AttrackLabel;
    private GameObject NameLabel;
    private GameObject MqLabel;
    private GameObject FriendSprite;

    private int AtkValue;
    private int HpValue;
    private int HeroTempalteId = 0;
	// Use this for initialization
	void Start () {
        AttrackLabel = transform.FindChild("Container info/Label atk").gameObject;
        NameLabel = transform.FindChild("Container info/Label name").gameObject;
        MqLabel = transform.FindChild("Container info/Label mingqi").gameObject;
        FriendSprite = transform.FindChild("Container info/Sprite head").gameObject;

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
            AtkValue += FriendData.Data.HeroProp[i].Prop[RoleProperties.ROLE_ATK];
            HpValue += FriendData.Data.HeroProp[i].Prop[RoleProperties.ROLE_HP];
            if (i == 0)
            {
                HeroTempalteId = FriendData.Data.HeroProp[i].HeroTemplateId;
            }
        }
        ShowData();

    }

    private void ShowData()
    {
        if (AttrackLabel == null) return;
        var lb = AttrackLabel.GetComponent<UILabel>();
        lb.text = AtkValue.ToString();

        lb = NameLabel.GetComponent<UILabel>();
        lb.text = FriendData.Data.FriendName;

        lb = MqLabel.GetComponent<UILabel>();
        lb.text = FriendData.Data.FriendFamous.ToString();

        var sp = FriendSprite.GetComponent<UISprite>();
        int k = HeroTempalteId % 14;
        sp.spriteName = "head_" + k;
    }
}

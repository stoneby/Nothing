using System.Collections.Generic;
using KXSGCodec;
using Property;
using UnityEngine;
using System.Collections;

public class AtkHeroItemControl : MonoBehaviour
{
    public FriendInfo FriendData;
    public List<HeroInfo> HeroList; 
//    public HeroInfo HeroData0;
//    public HeroInfo HeroData1;
//    public HeroInfo HeroData2;

    public int Attrack;
    public int Hp;
    public int Recover;
    public int Mp;

    private GameObject AtkLabel;
    private GameObject LevelLabel;
    private GameObject HpLabel;

    private GameObject LeaderLabel;
    private GameObject SecondLabel;

    private int LeaderAtk;
    private int LeaderHp;
    private int TheLevel;

    private bool IsFriendItem;

    private bool IsFriend;
    private int LeaderType;

	// Use this for initialization
	void Start () 
    {
        AtkLabel = transform.FindChild("Label atk").gameObject;
        LevelLabel = transform.FindChild("Label level").gameObject;
        HpLabel = transform.FindChild("Label hp").gameObject;
        LeaderLabel = transform.FindChild("Label leader").gameObject;
        SecondLabel = transform.FindChild("Label 2nd").gameObject;
        ShowData();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    
	}

    public void SetData(FriendInfo frienddata, bool isfriend)
    {
        IsFriendItem = true;
        IsFriend = isfriend;
        
        FriendData = frienddata;
        Attrack = 0;
        Hp = 0;
        Recover = 0;
        Mp = 0;
        TheLevel = FriendData.FriendLvl;
        for (int i = 0; i < FriendData.HeroProp.Count; i++)
        {
            if (FriendData.HeroProp[i].Prop.ContainsKey(RoleProperties.HERO_ATK))
            {
                Attrack += FriendData.HeroProp[i].Prop[RoleProperties.HERO_ATK];
                if (i == 0)
                {
                    LeaderAtk = FriendData.HeroProp[i].Prop[RoleProperties.HERO_ATK];
                }
            }
            if (FriendData.HeroProp[i].Prop.ContainsKey(RoleProperties.HERO_HP))
            {
                Hp += FriendData.HeroProp[i].Prop[RoleProperties.HERO_HP];
                if (i == 0)
                {
                    LeaderHp = FriendData.HeroProp[i].Prop[RoleProperties.HERO_HP];
                }
            }
            if (FriendData.HeroProp[i].Prop.ContainsKey(RoleProperties.HERO_MP))
            {
                Mp += FriendData.HeroProp[i].Prop[RoleProperties.HERO_MP];
            }
            if (FriendData.HeroProp[i].Prop.ContainsKey(RoleProperties.HERO_RECOVER))
            {
                Recover += FriendData.HeroProp[i].Prop[RoleProperties.HERO_RECOVER];
            }
        }

        ShowData();
    }

    public void SetData(HeroInfo herodata0, HeroInfo herodata1, HeroInfo herodata2, int leadertype)
    {
        IsFriendItem = false;
        LeaderType = leadertype;
        HeroList = new List<HeroInfo>();
        HeroList.Add(herodata0);
        HeroList.Add(herodata1);
        HeroList.Add(herodata2);
        Attrack = 0;
        Hp = 0;
        Recover = 0;
        Mp = 0;
        TheLevel = (herodata0 != null) ? (int)herodata0.Lvl : 0;
        for (int i = 0; i < HeroList.Count; i++)
        {
            if (HeroList[i] == null) continue;
            if (HeroList[i].Prop.ContainsKey(RoleProperties.HERO_ATK))
            {
                Attrack += HeroList[i].Prop[RoleProperties.HERO_ATK];
                if (i == 0)
                {
                    LeaderAtk = HeroList[i].Prop[RoleProperties.HERO_ATK];
                }
            }
            if (HeroList[i].Prop.ContainsKey(RoleProperties.HERO_HP))
            {
                Hp += HeroList[i].Prop[RoleProperties.HERO_HP];
                if (i == 0)
                {
                    LeaderHp = HeroList[i].Prop[RoleProperties.HERO_HP];
                }
            }
            if (HeroList[i].Prop.ContainsKey(RoleProperties.HERO_RECOVER))
            {
                Recover += HeroList[i].Prop[RoleProperties.HERO_RECOVER];
            }
            if (HeroList[i].Prop.ContainsKey(RoleProperties.HERO_MP))
            {
                Mp += HeroList[i].Prop[RoleProperties.HERO_MP];
            }
        }

        ShowData();
    }

    private void ShowData()
    {
        if (AtkLabel == null) return;
        var lb = AtkLabel.GetComponent<UILabel>();
        lb.text = LeaderAtk.ToString();

        lb = LevelLabel.GetComponent<UILabel>();
        lb.text = TheLevel.ToString();

        lb = HpLabel.GetComponent<UILabel>();
        lb.text = LeaderHp.ToString();

        if (IsFriendItem)
        {
            LeaderLabel.SetActive(true);
            lb = LeaderLabel.GetComponent<UILabel>();
            lb.text = IsFriend ? "Friend" : "Guest";
        }
        else
        {
            switch (LeaderType)
            {
                case 1:
                    LeaderLabel.SetActive(true);
                    lb = LeaderLabel.GetComponent<UILabel>();
                    lb.text = "Leader";
                    break;
                case 2:
                    SecondLabel.SetActive(true);
                    lb = SecondLabel.GetComponent<UILabel>();
                    lb.text = "2nd";
                    break;
                case 3:
                    SecondLabel.SetActive(true);
                    lb = SecondLabel.GetComponent<UILabel>();
                    lb.text = "3rd";
                    break;
                default:
                    break;
            }
        }
    }
}

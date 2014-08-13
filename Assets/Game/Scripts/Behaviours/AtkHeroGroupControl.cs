using KXSGCodec;
using UnityEngine;
using System.Collections;

public class AtkHeroGroupControl : KxItemRender
{
    private TeamInfo TeamData;
    private GameObject TestLabel;

    private GameObject Item1;
    private GameObject Item2;
    private GameObject Item3;
    private GameObject Item4;
    private GameObject Item5;
    private GameObject Item6;
    private GameObject Item7;
    private GameObject Item8;
    private GameObject Item9;
	// Use this for initialization
	void Start () 
    {
        TestLabel = transform.FindChild("Test Label").gameObject;
        Item1 = transform.FindChild("RaidHeadItem1").gameObject;
        Item2 = transform.FindChild("RaidHeadItem2").gameObject;
        Item3 = transform.FindChild("RaidHeadItem3").gameObject;
        Item4 = transform.FindChild("RaidHeadItem4").gameObject;
        Item5 = transform.FindChild("RaidHeadItem5").gameObject;
        Item6 = transform.FindChild("RaidHeadItem6").gameObject;
        Item7 = transform.FindChild("RaidHeadItem7").gameObject;
        Item8 = transform.FindChild("RaidHeadItem8").gameObject;
        Item9 = transform.FindChild("RaidHeadItem9").gameObject;
        SetShow();
	}
	

    public override void SetData<T>(T data)
    {
        TeamData = data as TeamInfo;
        SetShow();
        //throw new System.NotImplementedException();
    }

    private void SetShow()
    {
        if (TestLabel == null) return;
        var lb = TestLabel.GetComponent<UILabel>();
        lb.text = (ItemIndex + 1).ToString();

        CallSetData(Item1, 0, 1);
        CallSetData(Item2, 1, 2);
        CallSetData(Item3, 2, 2);
        CallSetData(Item4, 3, 3);
        CallSetData(Item5, 4, 3);
        CallSetData(Item6, 5, 3);
        CallSetData(Item7, 6, 3);
        CallSetData(Item8, 7, 3);
        CallSetData(Item9, 8, 3);
    }

    private void CallSetData(GameObject obj, int theindex, int type)
    {
        var item = obj.GetComponent<RaidHeadControl>();
        HeroInfo h0 = (TeamData.ListHeroUuid.Count > theindex)
            ? HeroModelLocator.Instance.FindHero(TeamData.ListHeroUuid[theindex])
            : null;
        
        item.SetData(h0, type);
    }

    public int GetAtkValue()
    {
        int v = 0;
        var item = Item1.GetComponent<RaidHeadControl>();
        v += item.Attrack;
        item = Item2.GetComponent<RaidHeadControl>();
        v += item.Attrack;
        item = Item3.GetComponent<RaidHeadControl>();
        v += item.Attrack;
        item = Item4.GetComponent<RaidHeadControl>();
        v += item.Attrack;
        item = Item5.GetComponent<RaidHeadControl>();
        v += item.Attrack;
        item = Item6.GetComponent<RaidHeadControl>();
        v += item.Attrack;
        item = Item7.GetComponent<RaidHeadControl>();
        v += item.Attrack;
        item = Item8.GetComponent<RaidHeadControl>();
        v += item.Attrack;
        item = Item9.GetComponent<RaidHeadControl>();
        v += item.Attrack;
        return v;
    }

    public int GetHpValue()
    {
        int v = 0;
        var item = Item1.GetComponent<RaidHeadControl>();
        v += item.Hp;
        item = Item2.GetComponent<RaidHeadControl>();
        v += item.Hp;
        item = Item3.GetComponent<RaidHeadControl>();
        v += item.Hp;
        item = Item4.GetComponent<RaidHeadControl>();
        v += item.Hp;
        item = Item5.GetComponent<RaidHeadControl>();
        v += item.Hp;
        item = Item6.GetComponent<RaidHeadControl>();
        v += item.Hp;
        item = Item7.GetComponent<RaidHeadControl>();
        v += item.Hp;
        item = Item8.GetComponent<RaidHeadControl>();
        v += item.Hp;
        item = Item9.GetComponent<RaidHeadControl>();
        v += item.Hp;
        return v;
    }

    public int GetRecoverValue()
    {
        int v = 0;
        var item = Item1.GetComponent<RaidHeadControl>();
        v += item.Recover;
        item = Item2.GetComponent<RaidHeadControl>();
        v += item.Recover;
        item = Item3.GetComponent<RaidHeadControl>();
        v += item.Recover;
        item = Item4.GetComponent<RaidHeadControl>();
        v += item.Recover;
        item = Item5.GetComponent<RaidHeadControl>();
        v += item.Recover;
        item = Item6.GetComponent<RaidHeadControl>();
        v += item.Recover;
        item = Item7.GetComponent<RaidHeadControl>();
        v += item.Recover;
        item = Item8.GetComponent<RaidHeadControl>();
        v += item.Recover;
        item = Item9.GetComponent<RaidHeadControl>();
        v += item.Recover;
        return v;
    }

    public int GetMpValue()
    {
        int v = 0;
        var item = Item1.GetComponent<RaidHeadControl>();
        v += item.Mp;
        item = Item2.GetComponent<RaidHeadControl>();
        v += item.Mp;
        item = Item3.GetComponent<RaidHeadControl>();
        v += item.Mp;
        item = Item4.GetComponent<RaidHeadControl>();
        v += item.Mp;
        item = Item5.GetComponent<RaidHeadControl>();
        v += item.Mp;
        item = Item6.GetComponent<RaidHeadControl>();
        v += item.Mp;
        item = Item7.GetComponent<RaidHeadControl>();
        v += item.Mp;
        item = Item8.GetComponent<RaidHeadControl>();
        v += item.Mp;
        item = Item9.GetComponent<RaidHeadControl>();
        v += item.Mp;
        return v;
    }
}

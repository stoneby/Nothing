using KXSGCodec;
using UnityEngine;
using System.Collections;

public class AtkHeroGroupControl : KxItemRender
{
    private TeamInfo TeamData;
    private GameObject TestLabel;

    private GameObject Item0;
    private GameObject Item1;
    private GameObject Item2;
	// Use this for initialization
	void Start () 
    {
        TestLabel = transform.FindChild("Test Label").gameObject;
        Item0 = transform.FindChild("AtkHeroItem0").gameObject;
        Item1 = transform.FindChild("AtkHeroItem1").gameObject;
        Item2 = transform.FindChild("AtkHeroItem2").gameObject;
        SetShow();
	}
	
	// Update is called once per frame
	void Update () {
	
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
        lb.text = "[" + ItemIndex + "]";

        CallSetData(Item0, 0, 3, 4, 1);
        CallSetData(Item1, 1, 5, 6, 2);
        CallSetData(Item2, 2, 7, 8, 3);
    }

    private void CallSetData(GameObject obj, int xx, int yy, int zz, int type)
    {
        var item = obj.GetComponent<AtkHeroItemControl>();
        HeroInfo h0 = (TeamData.ListHeroUuid.Count > xx)
            ? HeroModelLocator.Instance.FindHero(TeamData.ListHeroUuid[xx])
            : null;
        HeroInfo h1 = (TeamData.ListHeroUuid.Count > yy)
            ? HeroModelLocator.Instance.FindHero(TeamData.ListHeroUuid[yy])
            : null;
        HeroInfo h2 = (TeamData.ListHeroUuid.Count > zz)
            ? HeroModelLocator.Instance.FindHero(TeamData.ListHeroUuid[zz])
            : null;
        item.SetData(h0, h1, h2, type);
    }

    public int GetAtkValue()
    {
        int v = 0;
        var item = Item0.GetComponent<AtkHeroItemControl>();
        v += item.Attrack;
        item = Item1.GetComponent<AtkHeroItemControl>();
        v += item.Attrack;
        item = Item2.GetComponent<AtkHeroItemControl>();
        v += item.Attrack;
        return v;
    }

    public int GetHpValue()
    {
        int v = 0;
        var item = Item0.GetComponent<AtkHeroItemControl>();
        v += item.Hp;
        item = Item1.GetComponent<AtkHeroItemControl>();
        v += item.Hp;
        item = Item2.GetComponent<AtkHeroItemControl>();
        v += item.Hp;
        return v;
    }

    public int GetRecoverValue()
    {
        int v = 0;
        var item = Item0.GetComponent<AtkHeroItemControl>();
        v += item.Recover;
        item = Item1.GetComponent<AtkHeroItemControl>();
        v += item.Recover;
        item = Item2.GetComponent<AtkHeroItemControl>();
        v += item.Recover;
        return v;
    }

    public int GetMpValue()
    {
        int v = 0;
        var item = Item0.GetComponent<AtkHeroItemControl>();
        v += item.Mp;
        item = Item1.GetComponent<AtkHeroItemControl>();
        v += item.Mp;
        item = Item2.GetComponent<AtkHeroItemControl>();
        v += item.Mp;
        return v;
    }
}

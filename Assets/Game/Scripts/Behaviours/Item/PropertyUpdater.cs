using UnityEngine;

public class PropertyUpdater : MonoBehaviour
{
    private UILabel atkLabel;
    private UILabel hpLabel;
    private UILabel lvlLabel;
    private UILabel mpLabel;
    private UILabel recoverLabel;

    private int lvlCached;
    private int atkCached;
    private int hpCached;
    private int recoverCached;
    private int mpCached;
    private int maxLvlCached;

    private const string DefaultValue = "-";

    private const string ChangeColor = "[00ff00]";
    private const string ColorSuffix = "[-]";

    private void Awake()
    {
        atkLabel = transform.Find("Atk/AtkValue").GetComponent<UILabel>();
        var lvlTran = transform.Find("Lvl/LvlValue");
        if(lvlTran)
        {
            lvlLabel = lvlTran.GetComponent<UILabel>();
        }
        hpLabel = transform.FindChild("Hp/HpValue").GetComponent<UILabel>();
        recoverLabel = transform.Find("Recover/RecoverValue").GetComponent<UILabel>();
        mpLabel = transform.Find("Mp/MpValue").GetComponent<UILabel>();
    }

    public void UpdateProperty(int lvl, int maxLvl, int atk, int hp, int recover, int mp)
    {
        lvlCached = lvl;
        maxLvlCached = maxLvl;
        atkCached = atk;
        hpCached = hp;
        recoverCached = recover;
        mpCached = mp;
        if(lvlLabel != null)
        {
            lvlLabel.text = string.Format("{0}/{1}", lvl, maxLvl);
        }
        atkLabel.text = atk > 0 ? atk.ToString() : DefaultValue;
        hpLabel.text = hp > 0 ? hp.ToString() : DefaultValue;
        recoverLabel.text = recover > 0 ? recover.ToString() : DefaultValue;
        mpLabel.text = recover > 0 ? mp.ToString() : DefaultValue;
    }

    public void PreShowChangedProperty(int lvlChanged, int atkChanged, int hpChanged, int recoverChanged, int mpChanged)
    {
        const string colorPrefix = ChangeColor + "+";
        if (lvlLabel != null)
        {
            lvlLabel.text = string.Format("{0}/{1}", lvlCached, maxLvlCached) + colorPrefix + lvlChanged + ColorSuffix; 
        }
        atkLabel.text = atkCached + colorPrefix + atkChanged + ColorSuffix;
        hpLabel.text = hpCached + colorPrefix + hpChanged + ColorSuffix;
        recoverLabel.text = recoverCached + colorPrefix + recoverChanged + ColorSuffix;
        mpLabel.text = mpCached + colorPrefix + mpChanged + ColorSuffix;
    }

    public void Reset()
    {
        if (lvlLabel != null)
        {
            lvlLabel.text = "-";
        }
        atkLabel.text = "-";
        hpLabel.text = "-";
        recoverLabel.text = "-";
        mpLabel.text = "-";
    }
}

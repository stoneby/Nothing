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
    private const string ColorSuffix = "[-]";

    private const string AddColorPrefix = "[00ff00]+";
    private const string SubColorPrefix = "[ff0000]";

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
        if (lvlLabel != null )
        {
            var changed = "";
            if (lvlChanged > 0)
            {
                changed = AddColorPrefix + lvlChanged + ColorSuffix;
            }
            else if (lvlChanged < 0)
            {
                changed = SubColorPrefix + lvlChanged + ColorSuffix;
            }
            lvlLabel.text = string.Format("{0}{1}/{2}", lvlCached, changed, maxLvlCached);
        }
        Preshow(atkLabel, atkCached, atkChanged);
        Preshow(hpLabel, hpCached, hpChanged);
        Preshow(recoverLabel, recoverCached, recoverChanged);
        Preshow(mpLabel, mpCached, mpChanged);
    }

    private void Preshow(UILabel label, int cachedValue, int changed)
    {
        if(changed > 0)
        {
            label.text = cachedValue + AddColorPrefix + changed + ColorSuffix;
        }
        else if(changed < 0)
        {
            label.text = cachedValue + SubColorPrefix + changed + ColorSuffix;
        }
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

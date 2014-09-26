using UnityEngine;

public class PropertyUpdater : MonoBehaviour
{
    public UILabel AtkLabel;
    public UILabel HpLabel;
    public UILabel LvlLabel;
    public UILabel MpLabel;
    public UILabel RecoverLabel;

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

    public void UpdateProperty(int lvl, int maxLvl, int atk, int hp, int recover, int mp)
    {
        lvlCached = lvl;
        maxLvlCached = maxLvl;
        atkCached = atk;
        hpCached = hp;
        recoverCached = recover;
        mpCached = mp;
        if(LvlLabel != null)
        {
            LvlLabel.text = string.Format("{0}/{1}", lvl, maxLvl);
        }
        AtkLabel.text = atk >= 0 ? atk.ToString() : DefaultValue;
        HpLabel.text = hp >= 0 ? hp.ToString() : DefaultValue;
        RecoverLabel.text = recover >=0 ? recover.ToString() : DefaultValue;
        MpLabel.text = mp >= 0 ? mp.ToString() : DefaultValue;
    }

    public void PreShowChangedProperty(int lvlChanged, int atkChanged, int hpChanged, int recoverChanged, int mpChanged)
    {
        if (LvlLabel != null )
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
            else
            {
                LvlLabel.text = string.Format("{0}/{1}", lvlCached, maxLvlCached);
            }
            LvlLabel.text = string.Format("{0}{1}/{2}", lvlCached, changed, maxLvlCached);
        }
        Preshow(AtkLabel, atkCached, atkChanged);
        Preshow(HpLabel, hpCached, hpChanged);
        Preshow(RecoverLabel, recoverCached, recoverChanged);
        Preshow(MpLabel, mpCached, mpChanged);
        if(lvlChanged == 0 && atkChanged == 0 && hpChanged == 0 && recoverChanged == 0 && mpChanged == 0)
        {
            UpdateProperty(lvlCached, maxLvlCached, atkCached, hpCached, recoverCached, mpCached);
        }
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
        else
        {
            label.text = cachedValue >= 0 ? cachedValue.ToString() : DefaultValue;
        }
    }

    public void Reset()
    {
        if (LvlLabel != null)
        {
            LvlLabel.text = "-";
        }
        AtkLabel.text = "-";
        HpLabel.text = "-";
        RecoverLabel.text = "-";
        MpLabel.text = "-";
    }
}

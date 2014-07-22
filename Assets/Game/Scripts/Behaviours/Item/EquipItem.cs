using KXSGCodec;
using System.Globalization;
using UnityEngine;

public class EquipItem : ItemBase
{
    private Transform sortRelatedTran;
    private Transform lockMaskTran;
    private Transform equipMaskTran;
    private Transform lockedIcon;
    private Transform curTeamEquiped;
    private Transform otherTeamEquiped;

    private sbyte bindState;
    public sbyte BindState
    {
       get { return bindState; }
       set
        {
            bindState = value;
            lockedIcon.gameObject.SetActive(bindState == 1);
        }
    }
    private sbyte equipStatus;
    public sbyte EquipStatus
    {
        get { return equipStatus; }
        set
        {
            equipStatus = value;
            curTeamEquiped.gameObject.SetActive(equipStatus == 1);
            otherTeamEquiped.gameObject.SetActive(false);
        }
    }

    protected override void Awake()
    {   
        base.Awake();
        sortRelatedTran = cachedTran.FindChild("SortRelated");
        lockMaskTran = cachedTran.FindChild("BindMask");
        equipMaskTran = cachedTran.FindChild("EquipMask");
        ShowLockMask(false);
        ShowEquipMask(false);
        lockedIcon = cachedTran.FindChild("BindIcon");
        curTeamEquiped = cachedTran.FindChild("CurEquiped");
        otherTeamEquiped = cachedTran.FindChild("OtherEquiped");
    }

    public override void InitItem(ItemInfo itemInfo)
    {
        base.InitItem(itemInfo);
        BindState = itemInfo.BindStatus;
        EquipStatus = itemInfo.EquipStatus;
    }

    /// <summary>
    /// Show each hero items with the job info.
    /// </summary>
    public virtual void ShowByJob(sbyte job, int atk)
    {
        var attackTitle = Utils.FindChild(sortRelatedTran, "Attack-Title");
        var jobSymobl = Utils.FindChild(sortRelatedTran, "JobSymbol").GetComponent<UISprite>();
        var attackValue = Utils.FindChild(sortRelatedTran, "Attack-Value").GetComponent<UILabel>();
        NGUITools.SetActiveChildren(sortRelatedTran.gameObject, false);
        NGUITools.SetActive(attackValue.gameObject, true);
        if (job != -1)
        {
            NGUITools.SetActive(jobSymobl.gameObject, true);
            jobSymobl.spriteName = HeroConstant.HeroJobPrefix + job;
            attackValue.text = atk != -1 ? atk.ToString(CultureInfo.InvariantCulture) : "-";
        }
        else
        {
            NGUITools.SetActive(attackTitle.gameObject, true);
            attackValue.text = atk != -1 ? atk.ToString(CultureInfo.InvariantCulture) : "-";
        }
    }

    /// <summary>
    /// Show each hero items with the hp info.
    /// </summary>
    public virtual void ShowByHp(int hp)
    {
        var hpTitle = Utils.FindChild(sortRelatedTran, "HP-Title");
        var hpValue = Utils.FindChild(sortRelatedTran, "HP-Value").GetComponent<UILabel>();
        NGUITools.SetActiveChildren(sortRelatedTran.gameObject, false);
        NGUITools.SetActive(hpTitle.gameObject, true);
        NGUITools.SetActive(hpValue.gameObject, true);
        hpValue.text = hp != -1 ? hp.ToString(CultureInfo.InvariantCulture) : "-";
    }

    /// <summary>
    /// Show each hero items with the recover info.
    /// </summary>
    public virtual void ShowByRecover(int recover)
    {
        var recoverTitle = Utils.FindChild(sortRelatedTran, "Recover-Title");
        var recoverValue = Utils.FindChild(sortRelatedTran, "Recover-Value").GetComponent<UILabel>();
        NGUITools.SetActiveChildren(sortRelatedTran.gameObject, false);
        NGUITools.SetActive(recoverTitle.gameObject, true);
        NGUITools.SetActive(recoverValue.gameObject, true);
        recoverValue.text = recover != -1 ? recover.ToString(CultureInfo.InvariantCulture) : "-";
    }

    /// <summary>
    /// Show each hero items with the level info.
    /// </summary>
    public virtual void ShowByLvl(short level)
    {
        var lvTitle = Utils.FindChild(sortRelatedTran, "LV-Title");
        var lvValue = Utils.FindChild(sortRelatedTran, "LV-Value").GetComponent<UILabel>();
        lvValue.text = level.ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelatedTran.gameObject, false);
        NGUITools.SetActive(lvTitle.gameObject, true);
        NGUITools.SetActive(lvValue.gameObject, true);
    }

    /// <summary>
    /// Show each hero items with the quality of the hero.
    /// </summary>
    public virtual void ShowByQuality(int star)
    {
    }

    public void ShowLockMask(bool show)
    {
        lockMaskTran.gameObject.SetActive(show);
    }

    public void ShowEquipMask(bool show)
    {
        equipMaskTran.gameObject.SetActive(show);
    }

}

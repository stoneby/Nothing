﻿using System.Globalization;
using UnityEngine;

public class HeroItem : HeroItemBase
{
    private Transform sortRelatedTran;
    protected override void Awake()
    {
        base.Awake();
        sortRelatedTran = cachedTran.FindChild("SortRelated");
    }

    /// <summary>
    /// Show each hero items with the job info.
    /// </summary>
    public void ShowByJob(sbyte job, int atk)
    {
        var jobSymobl = Utils.FindChild(sortRelatedTran, "JobSymbol").GetComponent<UISprite>();
        var attack = Utils.FindChild(sortRelatedTran, "Attack").GetComponent<UILabel>();
        jobSymobl.spriteName = HeroConstant.HeroJobPrefix + job;
        attack.text = atk.ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelatedTran.gameObject, false);
        NGUITools.SetActive(jobSymobl.gameObject, true);
        NGUITools.SetActive(attack.gameObject, true);
    }

    /// <summary>
    /// Show each hero items with the hp info.
    /// </summary>
    public void ShowByHp(int hp)
    {
        var hpTitle = Utils.FindChild(sortRelatedTran, "HP-Title");
        var hpValue = Utils.FindChild(sortRelatedTran, "HP-Value").GetComponent<UILabel>();
        hpValue.text = hp.ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelatedTran.gameObject, false);
        NGUITools.SetActive(hpTitle.gameObject, true);
        NGUITools.SetActive(hpValue.gameObject, true);
    }

    /// <summary>
    /// Show each hero items with the recover info.
    /// </summary>
    public void ShowByRecover(int recover)
    {
        var recoverTitle = Utils.FindChild(sortRelatedTran, "Recover-Title");
        var recoverValue = Utils.FindChild(sortRelatedTran, "Recover-Value").GetComponent<UILabel>();
        recoverValue.text = recover.ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelatedTran.gameObject, false);
        NGUITools.SetActive(recoverTitle.gameObject, true);
        NGUITools.SetActive(recoverValue.gameObject, true);
    }

    /// <summary>
    /// Show each hero items with the level info.
    /// </summary>
    public void ShowByLvl(short level)
    {
        var lvTitle = Utils.FindChild(sortRelatedTran, "LV-Title");
        var lvValue = Utils.FindChild(sortRelatedTran, "LV-Value").GetComponent<UILabel>();
        lvValue.text = level.ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelatedTran.gameObject, false);
        NGUITools.SetActive(lvTitle.gameObject, true);
        NGUITools.SetActive(lvValue.gameObject, true);
    }
}

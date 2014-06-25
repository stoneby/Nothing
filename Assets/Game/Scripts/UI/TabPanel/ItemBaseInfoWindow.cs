using System.Collections;
using System.Globalization;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class ItemBaseInfoWindow : Window
{
    #region Public Fileds

    public static SCItemDetail ItemDetail;

    #endregion 

    #region Private Fileds

    private ItemInfo itemInfo;
    private UILabel bindlabel;
    private UILabel nBindLabel;
    private UILabel attack;
    private UILabel hp;
    private UILabel recover;
    private UILabel mp;

    private const string ColorString = "[56ff0b](";
    private const string ColorEndString = ")[-]";

    #endregion

    #region Window

    public override void OnEnter()
    {
        itemInfo = ItemModeLocator.Instance.FindItem(ItemDetail.BagIndex);
        Refresh();
    }

    public override void OnExit()
    {
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    void Awake()
    {
        bindlabel = Utils.FindChild(transform, "BindLabel").GetComponent<UILabel>();
        nBindLabel = Utils.FindChild(transform, "NBindLabel").GetComponent<UILabel>();
        var property = Utils.FindChild(transform, "Property");
        attack = Utils.FindChild(property, "AttackValue").GetComponent<UILabel>();
        hp = Utils.FindChild(property, "HPValue").GetComponent<UILabel>();
        recover = Utils.FindChild(property, "RecoverValue").GetComponent<UILabel>();
        mp = Utils.FindChild(property, "MPValue").GetComponent<UILabel>();
    }

    /// <summary>
    /// Update all ui related data.
    /// </summary>
    private void Refresh()
    {
        if (itemInfo.BindStatus == 0)
        {
            nBindLabel.gameObject.SetActive(true);
            bindlabel.gameObject.SetActive(false);
        }
        if (itemInfo.BindStatus == 1)
        {
            bindlabel.gameObject.SetActive(true);
            nBindLabel.gameObject.SetActive(false);
        }
        Utils.FindChild(transform, "Name").GetComponent<UILabel>().text = ItemModeLocator.Instance.GetName(itemInfo.TmplId);
        var stars = Utils.FindChild(transform, "Stars");
        var quality = ItemModeLocator.Instance.GetQuality(itemInfo.TmplId);
        var starCount = Mathf.CeilToInt((float)quality / ItemType.QualitiesPerStar);
        var childCount = stars.transform.childCount;
        for (int index = 0; index < starCount; index++)
        {
            NGUITools.SetActive(stars.FindChild("Star" + (childCount - index - 1)).gameObject, true);
        }
        for (int index = childCount - starCount - 1; index >= 0; index--)
        {
            NGUITools.SetActive(stars.FindChild("Star" + index).gameObject, false);
        }
        Utils.FindChild(transform, "LV-Value").GetComponent<UILabel>().text = string.Format("{0}/{1}", itemInfo.Level, itemInfo.MaxLvl);
        Utils.FindChild(transform, "Limit-Value").GetComponent<UILabel>().text = string.Format("{0}/{1}", itemInfo.UpVal, ItemModeLocator.Instance.GetUpLimit(itemInfo.TmplId));
        Utils.FindChild(transform, "Luck-Value").GetComponent<UILabel>().text = "20";
        Utils.FindChild(transform, "Job-Value").GetComponent<UISprite>().spriteName = HeroConstant.HeroJobPrefix + ItemModeLocator.Instance.GetJob(itemInfo.TmplId);

        attack.text = ItemModeLocator.Instance.GetAttack(itemInfo.TmplId, itemInfo.Level).ToString(CultureInfo.InvariantCulture);
        hp.text = ItemModeLocator.Instance.GetHp(itemInfo.TmplId, itemInfo.Level).ToString(CultureInfo.InvariantCulture);
        recover.text = ItemModeLocator.Instance.GetRecover(itemInfo.TmplId, itemInfo.Level).ToString(CultureInfo.InvariantCulture);
        mp.text = ItemModeLocator.Instance.GetMp(itemInfo.TmplId).ToString(CultureInfo.InvariantCulture);
    }

    public void RefreshChangedData()
    {
        Utils.FindChild(transform, "LV-Value").GetComponent<UILabel>().text = string.Format("{0}/{1}", itemInfo.Level, itemInfo.MaxLvl);
        attack.text = ItemModeLocator.Instance.GetAttack(itemInfo.TmplId, itemInfo.Level).ToString(CultureInfo.InvariantCulture);
        hp.text = ItemModeLocator.Instance.GetHp(itemInfo.TmplId, itemInfo.Level).ToString(CultureInfo.InvariantCulture);
        recover.text = ItemModeLocator.Instance.GetRecover(itemInfo.TmplId, itemInfo.Level).ToString(CultureInfo.InvariantCulture);
    }

    public IEnumerator PreShowLevelUp(short level)
    {
        var lvlLabel = Utils.FindChild(transform, "LV-Value").GetComponent<UILabel>();
        var attackTxt = attack.text;
        var hpTxt = hp.text;
        var recoverTxt = recover.text;

        var incAtk = ItemModeLocator.Instance.GetAttack(itemInfo.TmplId, level) -
                     ItemModeLocator.Instance.GetAttack(itemInfo.TmplId, itemInfo.Level);
        var incHp = ItemModeLocator.Instance.GetHp(itemInfo.TmplId, level) -
                         ItemModeLocator.Instance.GetHp(itemInfo.TmplId, itemInfo.Level);
        var incRecover = ItemModeLocator.Instance.GetRecover(itemInfo.TmplId, level) -
                         ItemModeLocator.Instance.GetRecover(itemInfo.TmplId, itemInfo.Level);

        while (true)
        {
            lvlLabel.text = string.Format("{0}/{1}", itemInfo.Level, itemInfo.MaxLvl);
            attack.text = attackTxt;
            hp.text = hpTxt;
            recover.text = recoverTxt;
            yield return new WaitForSeconds(1f);

            lvlLabel.text = string.Format("{0}{1}{2}{3}/{4}", itemInfo.Level, ColorString, level,
                                          ColorEndString, itemInfo.MaxLvl);
            if(incAtk > 0)
            {
                attack.text = string.Format("{0}{1}{2}{3}", attackTxt, ColorString, incAtk, ColorEndString);
            }

            if (incHp > 0)
            {
                hp.text = string.Format("{0}{1}{2}{3}", hpTxt, ColorString, incHp, ColorEndString);
            }

            if (incRecover > 0)
            {
                recover.text = string.Format("{0}{1}{2}{3}", recoverTxt, ColorString, incRecover, ColorEndString);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    #endregion
}

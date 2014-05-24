using System.Globalization;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class ItemBaseInfoWindow : Window
{
    public static SCItemDetail ItemDetail;
    private ItemInfo itemInfo;
    private UILabel bindlabel;
    private UILabel nBindLabel;
    private UILabel attack;
    private UILabel hp;
    private UILabel recover;
    private UILabel mp;

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

    #region Mono

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
        Utils.FindChild(transform, "Name").GetComponent<UILabel>().text = ItemModeLocator.Instance.GetName(itemInfo);
        var stars = Utils.FindChild(transform, "Stars");
        var quality = ItemModeLocator.Instance.GetQuality(itemInfo);
        for (int index = 0; index < quality; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, true);
        }
        for (int index = quality; index < stars.childCount; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, false);
        }
        Utils.FindChild(transform, "LV-Value").GetComponent<UILabel>().text = string.Format("{0}/{1}", itemInfo.Level, itemInfo.MaxLvl);
        Utils.FindChild(transform, "Limit-Value").GetComponent<UILabel>().text = string.Format("{0}/{1}", itemInfo.UpVal, ItemModeLocator.Instance.GetUpLimit(itemInfo));
        Utils.FindChild(transform, "Luck-Value").GetComponent<UILabel>().text = "20";
        Utils.FindChild(transform, "Job-Value").GetComponent<UISprite>().spriteName = UIHerosDisplayWindow.JobPrefix + ItemModeLocator.Instance.GetJob(itemInfo);

        attack.text = ItemModeLocator.Instance.GetAttack(itemInfo).ToString(CultureInfo.InvariantCulture);
        hp.text = ItemModeLocator.Instance.GetHp(itemInfo).ToString(CultureInfo.InvariantCulture);
        recover.text = ItemModeLocator.Instance.GetRecover(itemInfo).ToString(CultureInfo.InvariantCulture);
        mp.text = ItemModeLocator.Instance.GetMp(itemInfo).ToString(CultureInfo.InvariantCulture);
    }

    #endregion
}

using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIMaterialInfoWindow : Window
{
    private UIEventListener backBtnLis;
    private ItemInfo itemInfo;
    private UILabel bindlabel;
    private UILabel nBindLabel;
    private UILabel explainName;
    private UILabel explainDesc;

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
        itemInfo = ItemModeLocator.Instance.FindItem(ItemBaseInfoWindow.ItemDetail.BagIndex);
        Refresh();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Awake()
    {
        backBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Back").gameObject);
        bindlabel = Utils.FindChild(transform, "BindLabel").GetComponent<UILabel>();
        nBindLabel = Utils.FindChild(transform, "NBindLabel").GetComponent<UILabel>();
        var explainContent = Utils.FindChild(transform, "ExplainContent");
        explainName = Utils.FindChild(explainContent, "Name").GetComponent<UILabel>();
        explainDesc = Utils.FindChild(explainContent, "Desc").GetComponent<UILabel>();
    }

    private void InstallHandlers()
    {
        backBtnLis.onClick += OnBackBtnClicked;
    }

    private void UnInstallHandlers()
    {
        backBtnLis.onClick -= OnBackBtnClicked;
    }

    private void OnBackBtnClicked(GameObject go)
    {
        WindowManager.Instance.Show(typeof(UIEquipsDisplayWindow), true);
        WindowManager.Instance.Show(typeof(UIMaterialInfoWindow), false);
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
        Utils.FindChild(transform, "Job-Value").GetComponent<UISprite>().spriteName = UIHerosDisplayWindow.JobPrefix +
                                                                                      ItemModeLocator.Instance.GetJob(
                                                                                          itemInfo);
        explainName.text = ItemModeLocator.Instance.GetName(itemInfo);
        explainDesc.text = ItemModeLocator.Instance.GetDesc(itemInfo);
    }

    #endregion
}

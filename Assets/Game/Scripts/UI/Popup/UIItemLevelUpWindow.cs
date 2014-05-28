using System.Collections.Generic;
using System.Globalization;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIItemLevelUpWindow : Window
{
    private UIEventListener backBtnLis;
    private readonly List<Transform> lvlUpSelBtns = new List<Transform>();
    private ItemInfo itemInfo;
    private UILabel costExpValue;

    #region Window

    public override void OnEnter()
    {
        itemInfo = ItemModeLocator.Instance.FindItem(ItemBaseInfoWindow.ItemDetail.BagIndex);
        InstallHandlers();
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
        costExpValue = Utils.FindChild(transform, "CostExpValue").GetComponent<UILabel>();
        var levelUpSelBtns = Utils.FindChild(transform, "LevelUpSelBtns");
        for (int i = 0; i < levelUpSelBtns.childCount; i++)
        {
            lvlUpSelBtns.Add(levelUpSelBtns.GetChild(i));  
        }
    }

    private void InstallHandlers()
    {
        backBtnLis.onClick += OnBackBtnClicked;
        for (int i = 0; i < lvlUpSelBtns.Count; i++)
        {
            UIEventListener.Get(lvlUpSelBtns[i].gameObject).onClick += OnLevelUpSel;
        }
    }

    private void UnInstallHandlers()
    {
        backBtnLis.onClick -= OnBackBtnClicked;
        for (int i = 0; i < lvlUpSelBtns.Count; i++)
        {
            UIEventListener.Get(lvlUpSelBtns[i].gameObject).onClick -= OnLevelUpSel;
        }
    }

    
    private int GetExpToFullLvl()
    {
        var curLvl = itemInfo.Level;
        var maxLvl = itemInfo.MaxLvl;
        var itemLvlTmpls = ItemModeLocator.Instance.ItemConfig.ItemLvlTmpl;
        var result = itemLvlTmpls[curLvl].MaxExp - itemInfo.CurExp;
        for (int i = curLvl + 1; i < maxLvl; i++)
        {
            result += itemLvlTmpls[i].MaxExp;
        }
        return result;
    }

    private void Refresh()
    {
        costExpValue.text = GetExpToFullLvl().ToString(CultureInfo.InvariantCulture);
    }

    private void OnLevelUpSel(GameObject go)
    {
        WindowManager.Instance.Show<UIItemLevelUpSelWindow>(true);
        WindowManager.Instance.Show<ItemBaseInfoWindow>(false);
    }

    private void OnBackBtnClicked(GameObject go)
    {
        WindowManager.Instance.Show<UIItemInfoWindow>(true);
    }

    #endregion
}

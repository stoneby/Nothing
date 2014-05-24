using System.Collections.Generic;
using System.Globalization;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIEquipSellWindow : Window
{
    #region Private Fields

    private UIEventListener sellLis;
    private UIEventListener cancelSelLis;
    private UIEventListener oneKeyAddLis;
    private UIEventListener tradeInLis;
    private UIEventListener sellOkLis;
    private UIEventListener sellCancelLis;
    private UIEventListener sortBtnLis;

    private UILabel sortLabel;
    private List<ItemInfo> infos;
    private int capacity;
    private UILabel itemNums;
    private UIGrid grid;

    #endregion

    #region Public Fields

    /// <summary>
    /// The prefab of the equip item.
    /// </summary>
    public GameObject ItemPrefab;

    #endregion

    #region Window

    public override void OnEnter()
    {
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
        sellLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Sell").gameObject);
        cancelSelLis = UIEventListener.Get(Utils.FindChild(transform, "Button-CancelSel").gameObject);
        oneKeyAddLis = UIEventListener.Get(Utils.FindChild(transform, "Button-OneKeyAdd").gameObject);
        sortBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Sort").gameObject);
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();
        var sortType = ItemModeLocator.Instance.OrderType;
        sortLabel.text = StringTable.SortStrings[sortType];
        tradeInLis = UIEventListener.Get(Utils.FindChild(transform, "Button-TradeIn").gameObject);
        infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos ?? new List<ItemInfo>();
        capacity = ItemModeLocator.Instance.ScAllItemInfos.Capacity;
        itemNums = Utils.FindChild(transform, "EquipNums").GetComponent<UILabel>();
        grid = GetComponentInChildren<UIGrid>();
    }

    /// <summary>
    /// Install all handlers.
    /// </summary>
    private void InstallHandlers()
    {
        sellLis.onClick += OnSell;
        cancelSelLis.onClick += OnCancelSel;
        oneKeyAddLis.onClick += OnOneKeyAdd;
        tradeInLis.onClick += OnTradeIn;
    }

    /// <summary>
    /// Uninstall all handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        sellLis.onClick -= OnSell;
        cancelSelLis.onClick -= OnCancelSel;
        oneKeyAddLis.onClick -= OnOneKeyAdd;
        tradeInLis.onClick -= OnTradeIn;
    }

    /// <summary>
    /// Init the ui data when we enter the window.
    /// </summary>
    private void UpdateItemList()
    {
        itemNums.text = string.Format("{0}/{1}", infos.Count, capacity);
        var itemCount = infos.Count;
        var childCount = grid.transform.childCount;
        if (childCount != itemCount)
        {
            var isAdd = childCount < itemCount;
            Utils.AddOrDelItems(grid.transform, ItemPrefab.transform, isAdd, Mathf.Abs(itemCount - childCount), "Heros",
                    OnItemClicked);
            grid.repositionNow = true;
        }
    }

    /// <summary>
    /// The callback of clicking sort button.
    /// </summary>
    private void OnSortClicked(GameObject go)
    {
        var orderType = ItemModeLocator.Instance.OrderType;
        orderType = (sbyte)((orderType + 1) % StringTable.SortStrings.Count);
        sortLabel.text = StringTable.SortStrings[orderType];
        ItemModeLocator.Instance.OrderType = orderType;
        Refresh();
    }

    private void Refresh()
    {
        UpdateItemList();
        var orderType = ItemModeLocator.Instance.OrderType;
        ItemModeLocator.Instance.SortItemList(orderType, infos);
        for (int i = 0; i < infos.Count; i++)
        {
            var item = grid.transform.GetChild(i);
            var bagIndex = infos[i].BagIndex;
            item.GetComponent<EquipItemInfoPack>().BagIndex = bagIndex;
            ShowItem(orderType, item, bagIndex);
        }
        itemNums.text = string.Format("{0}/{1}", infos.Count, capacity);
    }

    private void OnItemClicked(GameObject go)
    {
        
    }

    private void OnSell(GameObject go)
    {
        
    }

    private void OnCancelSel(GameObject go)
    {
       
    }

    private void OnOneKeyAdd(GameObject go)
    {
       
    }

    private void OnTradeIn(GameObject go)
    {
        
    }

    /// <summary>
    /// Show the info of the item.
    /// </summary>
    /// <param name="orderType">The order type of </param>
    /// <param name="itemTran">The transform of item.</param>
    /// <param name="bagIndex">The bag index of item.</param>
    private void ShowItem(short orderType, Transform itemTran, short bagIndex)
    {
        var itemInfo = ItemModeLocator.Instance.FindItem(bagIndex);

        var sortRelated = Utils.FindChild(itemTran, "SortRelated");
        var stars = Utils.FindChild(itemTran, "Rarity");
        var quality = ItemModeLocator.Instance.GetQuality(itemInfo);
        for (int index = 0; index < quality; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, true);
        }
        for (int index = quality; index < stars.transform.childCount; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, false);
        }
        switch (orderType)
        {
            //����˳������
            case 0:
                ShowByLvl(sortRelated, itemInfo);
                break;

            //�佫ְҵ����
            case 1:
                ShowByJob(sortRelated, itemInfo);
                break;

            //�佫ϡ�ж�����
            case 2:
                ShowByLvl(sortRelated, itemInfo);
                break;

            //�ն���˳������
            case 3:
                ShowByLvl(sortRelated, itemInfo);
                break;

            //����������
            case 4:
                ShowByJob(sortRelated, itemInfo);
                break;

            //HP����
            case 5:
                ShowByHp(sortRelated, itemInfo);
                break;

            //�ظ�������
            case 6:
                ShowByRecover(sortRelated, itemInfo);
                break;

            //�ȼ�����
            case 7:
                ShowByLvl(sortRelated, itemInfo);
                break;
        }
    }

    /// <summary>
    /// Show each hero items with the job info.
    /// </summary>
    private void ShowByJob(Transform sortRelated, ItemInfo itemInfo)
    {
        var attackTitle = Utils.FindChild(sortRelated, "Attack-Title");
        var jobSymobl = Utils.FindChild(sortRelated, "JobSymbol").GetComponent<UISprite>();
        var attackValue = Utils.FindChild(sortRelated, "Attack-Value").GetComponent<UILabel>();
        NGUITools.SetActiveChildren(sortRelated.gameObject, false);
        NGUITools.SetActive(attackValue.gameObject, true);
        var jobValue = ItemModeLocator.Instance.GetJob(itemInfo);
        var attack = ItemModeLocator.Instance.GetAttack(itemInfo);
        if (jobValue != -1)
        {
            NGUITools.SetActive(jobSymobl.gameObject, true);
            jobSymobl.spriteName = UIHerosDisplayWindow.JobPrefix + jobValue;
            attackValue.text = attack != -1 ? attack.ToString(CultureInfo.InvariantCulture) : "-";
        }
        else
        {
            NGUITools.SetActive(attackTitle.gameObject, true);
            attackValue.text = attack != -1 ? attack.ToString(CultureInfo.InvariantCulture) : "-";
        }
    }

    /// <summary>
    /// Show each hero items with the hp info.
    /// </summary>
    private void ShowByHp(Transform sortRelated, ItemInfo itemInfo)
    {
        var hpTitle = Utils.FindChild(sortRelated, "HP-Title");
        var hpValue = Utils.FindChild(sortRelated, "HP-Value").GetComponent<UILabel>();
        NGUITools.SetActiveChildren(sortRelated.gameObject, false);
        NGUITools.SetActive(hpTitle.gameObject, true);
        NGUITools.SetActive(hpValue.gameObject, true);
        var hp = ItemModeLocator.Instance.GetHp(itemInfo);
        hpValue.text = hp != -1 ? hp.ToString(CultureInfo.InvariantCulture) : "-";
    }

    /// <summary>
    /// Show each hero items with the recover info.
    /// </summary>
    private void ShowByRecover(Transform sortRelated, ItemInfo itemInfo)
    {
        var recoverTitle = Utils.FindChild(sortRelated, "Recover-Title");
        var recoverValue = Utils.FindChild(sortRelated, "Recover-Value").GetComponent<UILabel>();
        NGUITools.SetActiveChildren(sortRelated.gameObject, false);
        NGUITools.SetActive(recoverTitle.gameObject, true);
        NGUITools.SetActive(recoverValue.gameObject, true);
        var recover = ItemModeLocator.Instance.GetHp(itemInfo);
        recoverValue.text = recover != -1 ? recover.ToString(CultureInfo.InvariantCulture) : "-";
    }

    /// <summary>
    /// Show each hero items with the level info.
    /// </summary>
    private void ShowByLvl(Transform sortRelated, ItemInfo itemInfo)
    {
        var lvTitle = Utils.FindChild(sortRelated, "LV-Title");
        var lvValue = Utils.FindChild(sortRelated, "LV-Value").GetComponent<UILabel>();
        lvValue.text = itemInfo.Level.ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelated.gameObject, false);
        NGUITools.SetActive(lvTitle.gameObject, true);
        NGUITools.SetActive(lvValue.gameObject, true);
    }

    #endregion
}
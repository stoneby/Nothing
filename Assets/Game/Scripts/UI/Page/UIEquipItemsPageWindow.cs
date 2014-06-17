using System.Collections.Generic;
using System.Globalization;
using KXSGCodec;
using System.Collections;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIEquipItemsPageWindow : Window
{
    #region Private Fields

    private UIEventListener sortBtnLis;
    private UIEventListener addTestBtnLis;
    private UILabel itemNums;
    private UILabel sortLabel;
    private List<ItemInfo> infos;
    private int capacity;
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
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    void Awake()
    {
        sortBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Sort").gameObject);
        addTestBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Add").gameObject);
        grid = GetComponentInChildren<UIGrid>();
        capacity = ItemModeLocator.Instance.ScAllItemInfos.Capacity;
        infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos ?? new List<ItemInfo>();
        itemNums = Utils.FindChild(transform, "EquipNums").GetComponent<UILabel>();
        var sortType = ItemModeLocator.Instance.OrderType;
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();
        sortLabel.text = StringTable.ItemSortStrings[(int)sortType];
    }

    private void InstallHandlers()
    {
        sortBtnLis.onClick += OnSortClicked;
        addTestBtnLis.onClick += OnAddClicked;
    }

    private void UnInstallHandlers()
    {
        sortBtnLis.onClick -= OnSortClicked;
        addTestBtnLis.onClick -= OnAddClicked;
    }

    /// <summary>
    /// The callback of clicking sort button.
    /// </summary>
    private void OnSortClicked(GameObject go)
    {
        var orderType = ItemModeLocator.Instance.OrderType;
        orderType = (ItemHelper.OrderType)(((int)orderType + 1) % StringTable.ItemSortStrings.Count);
        sortLabel.text = StringTable.ItemSortStrings[(int)orderType];
        ItemModeLocator.Instance.OrderType = orderType;
        Refresh();
    }

    /// <summary>
    /// The callback of clicking add button.
    /// </summary>
    private void OnAddClicked(GameObject go)
    {
        var csmsg = new CSAddItemTest();
        NetManager.SendMessage(csmsg);
    }

    /// <summary>
    /// The callback of clicking each item.
    /// </summary>
    private void OnItemInfoClicked(GameObject go)
    {
        var bagIndex = go.GetComponent<EquipItem>().BagIndex;
        var csmsg = new CSQueryItemDetail {BagIndex = bagIndex};
        NetManager.SendMessage(csmsg);
    }

    /// <summary>
    /// Fill in the equip item game objects.
    /// </summary> 
    private void UpdateItemList()
    {
        infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos ?? new List<ItemInfo>();
        var itemCount = infos.Count;
        var childCount = grid.transform.childCount;
        if (childCount != itemCount)
        {
            var isAdd = childCount < itemCount;
            Utils.AddOrDelItems(grid.transform, ItemPrefab.transform, isAdd, Mathf.Abs(itemCount - childCount), "Heros",
                    OnItemInfoClicked);
            grid.repositionNow = true;
        }
    }

    public void Refresh()
    {
        UpdateItemList();
        var orderType = ItemModeLocator.Instance.OrderType;
        ItemModeLocator.Instance.SortItemList(orderType, infos);
        for (int i = 0; i < infos.Count; i++)
        {
            var item = grid.transform.GetChild(i);
            var bagIndex = infos[i].BagIndex;
            item.GetComponent<EquipItem>().BagIndex = bagIndex;
            ShowItem(orderType, item, bagIndex);
        }
        itemNums.text = string.Format("{0}/{1}", infos.Count, capacity);
    }

    /// <summary>
    /// Show the info of the item.
    /// </summary>
    /// <param name="orderType">The order type of </param>
    /// <param name="itemTran">The transform of item.</param>
    /// <param name="bagIndex">The bag index of item.</param>
    private void ShowItem(ItemHelper.OrderType orderType, Transform itemTran, short bagIndex)
    {
        var itemInfo = ItemModeLocator.Instance.FindItem(bagIndex);

        var sortRelated = Utils.FindChild(itemTran, "SortRelated");
        var stars = Utils.FindChild(itemTran, "Rarity");
        var quality = ItemModeLocator.Instance.GetQuality(itemInfo.TmplId);
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
            case ItemHelper.OrderType.Time:
                ShowByLvl(sortRelated, itemInfo);
                break;

            case ItemHelper.OrderType.Job:
                ShowByJob(sortRelated, itemInfo);
                break;

            case ItemHelper.OrderType.Rarity:
                ShowByLvl(sortRelated, itemInfo);
                break;

            case ItemHelper.OrderType.Team:
                ShowByLvl(sortRelated, itemInfo);
                break;

            case ItemHelper.OrderType.Attack:
                ShowByJob(sortRelated, itemInfo);
                break;

            case ItemHelper.OrderType.Health:
                ShowByHp(sortRelated, itemInfo);
                break;

            case ItemHelper.OrderType.Recover:
                ShowByRecover(sortRelated, itemInfo);
                break;

            case ItemHelper.OrderType.Level:
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
        var jobValue = ItemModeLocator.Instance.GetJob(itemInfo.TmplId);
        var attack = ItemModeLocator.Instance.GetAttack(itemInfo.TmplId, itemInfo.Level);
        if(jobValue != -1)
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
        var hp = ItemModeLocator.Instance.GetHp(itemInfo.TmplId, itemInfo.Level);
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
        var recover = ItemModeLocator.Instance.GetRecover(itemInfo.TmplId, itemInfo.Level);
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

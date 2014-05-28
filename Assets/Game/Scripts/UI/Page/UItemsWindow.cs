using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UItemsWindow : Window
{
    private UIEventListener sortBtnLis;
    private UIGrid grid;
    private UIPanel scrollPanel;
    private UIPanel panel;
    private Transform offset;
    private UILabel sortLabel;
    private int rowToShow;
    private List<ItemInfo> infos;

    private UIEventListener.VoidDelegate itemClicked;
    private int depth;

    #region Public Fields

    /// <summary>
    /// The prefab of the equip item.
    /// </summary>
    public GameObject ItemPrefab;

    public UIEventListener.VoidDelegate ItemClicked
    {
        get { return itemClicked; }
        set
        {
            if (value != itemClicked)
            {
                var parent = grid.transform;
                for(int i = 0; i < parent.childCount; i++)
                {
                    var item = parent.GetChild(i);
                    UIEventListener.Get(item.gameObject).onClick = value;
                }
            }
        }
    }

    public int Depth
    {
        get { return depth; }
        set
        {
            if(value != depth)
            panel.depth = depth;
            scrollPanel.depth = depth + 1;
        }
    }

    /// <summary>
    /// The rows the hero visible area on the current window.
    /// </summary>
    public int RowToShow
    {
        get
        {
            return rowToShow;
        }
        set
        {
            if (rowToShow != value)
            {
                rowToShow = value;
                var clipHeight = scrollPanel.baseClipRegion.w;
                scrollPanel.baseClipRegion = new Vector4(scrollPanel.baseClipRegion.x, scrollPanel.baseClipRegion.y, scrollPanel.baseClipRegion.z, rowToShow * grid.cellHeight);
                var pos = offset.transform.localPosition;
                offset.transform.localPosition = new Vector3(pos.x, pos.y - 0.5f * (scrollPanel.baseClipRegion.w - clipHeight), pos.z);
                scrollPanel.GetComponent<UIScrollView>().ResetPosition();
            }
        }
    }

    #endregion

    #region Window

    public override void OnEnter()
    {
        UpdateItemList();
        Refresh();
    }

    public override void OnExit()
    {
        DespawnItems();
    }

    #endregion

    #region Private Methods


    /// <summary>
    /// Despawn hero instance to the hero pool.
    /// </summary>
    private void DespawnItems()
    {
        if (PoolManager.Pools.ContainsKey("Heros"))
        {
            var list = grid.transform.Cast<Transform>().ToList();
            for (int index = 0; index < list.Count; index++)
            {
                var item = list[index];
                UIEventListener.Get(item.gameObject).onClick = null;
                item.parent = PoolManager.Pools["Heros"].transform;
                PoolManager.Pools["Heros"].Despawn(item);
            }
        }
    }

    // Use this for initialization
    void Awake()
    {
        sortBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Sort").gameObject);
        sortBtnLis.onClick += OnSortClicked;
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();
        infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos ?? new List<ItemInfo>();
        scrollPanel = GetComponentInChildren<UIScrollView>().GetComponent<UIPanel>();
        grid = GetComponentInChildren<UIGrid>();
        offset = Utils.FindChild(transform, "Offset");
        panel = GetComponent<UIPanel>();
    }

    private void OnSortClicked(GameObject go)
    {
        var orderType = ItemModeLocator.Instance.OrderType;
        orderType = (sbyte)((orderType + 1) % StringTable.SortStrings.Count);
        sortLabel.text = StringTable.SortStrings[orderType];
        ItemModeLocator.Instance.OrderType = orderType;
        Refresh();
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
                    null);
            grid.repositionNow = true;
        }
    }

    public void Refresh()
    {
        var orderType = ItemModeLocator.Instance.OrderType;
        sortLabel.text = StringTable.SortStrings[orderType];
        ItemModeLocator.Instance.SortItemList(orderType, infos);
        for (int i = 0; i < infos.Count; i++)
        {
            var item = grid.transform.GetChild(i);
            var bagIndex = infos[i].BagIndex;
            item.GetComponent<EquipItemInfoPack>().BagIndex = bagIndex;
            ShowItem(orderType, item, bagIndex);
        }
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
            //ÈëÊÖË³ÐòÅÅÐò
            case 0:
                ShowByLvl(sortRelated, itemInfo);
                break;

            //Îä½«Ö°ÒµÅÅÐò
            case 1:
                ShowByJob(sortRelated, itemInfo);
                break;

            //Îä½«Ï¡ÓÐ¶ÈÅÅÐò
            case 2:
                ShowByLvl(sortRelated, itemInfo);
                break;

            //ÕÕ¶ÓÎéË³ÐòÅÅÐò
            case 3:
                ShowByLvl(sortRelated, itemInfo);
                break;

            //¹¥»÷Á¦ÅÅÐò
            case 4:
                ShowByJob(sortRelated, itemInfo);
                break;

            //HPÅÅÐò
            case 5:
                ShowByHp(sortRelated, itemInfo);
                break;

            //»Ø¸´Á¦ÅÅÐò
            case 6:
                ShowByRecover(sortRelated, itemInfo);
                break;

            //µÈ¼¶ÅÅÐò
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

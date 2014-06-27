using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UItemsWindow : Window
{
    #region Private Fields

    private UIEventListener sortBtnLis;
    private UIPanel scrollPanel;
    private UIPanel panel;
    private Transform offset;
    private UILabel sortLabel;
    private int rowToShow;
    private List<ItemInfo> infos;
    private int depth;
    private int cachedDepth;
    private UIEventListener.VoidDelegate itemClicked;

    #endregion

    #region Public Fields

    [HideInInspector] 
    public UIGrid Items;

    /// <summary>
    /// The prefab of the equip item.
    /// </summary>
    public GameObject ItemPrefab;

    public UIEventListener.VoidDelegate ItemClicked
    {
        get { return itemClicked; }
        set
        {
            itemClicked = value;
            var parent = Items.transform;
            for(int i = 0; i < parent.childCount; i++)
            {
                var item = parent.GetChild(i);
                UIEventListener.Get(item.gameObject).onClick = value;
            }
        }
    }

    public int Depth
    {
        get { return depth; }
        set
        {
            if (value != depth)
            {
                depth = value;
                panel.depth = depth;
                scrollPanel.depth = depth + 1;
            }
        }
    }

    /// <summary>
    /// The rows the hero visible area on the current window.
    /// </summary>
    public int RowToShow
    {
        get { return rowToShow; }
        set
        {
            if(rowToShow != value)
            {
                rowToShow = value;
                var clipHeight = scrollPanel.baseClipRegion.w;
                scrollPanel.baseClipRegion = new Vector4(scrollPanel.baseClipRegion.x, scrollPanel.baseClipRegion.y,
                                                         scrollPanel.baseClipRegion.z, rowToShow * Items.cellHeight);
                var pos = offset.transform.localPosition;
                offset.transform.localPosition = new Vector3(pos.x,
                                                             pos.y - 0.5f * (scrollPanel.baseClipRegion.w - clipHeight),
                                                             pos.z);
                scrollPanel.GetComponent<UIScrollView>().ResetPosition();
            }
        }
    }

    #endregion

    #region Window

    public override void OnEnter()
    {
        scrollPanel.GetComponent<UIScrollView>().ResetPosition();
        infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos ?? new List<ItemInfo>();
        Refresh(infos);
    }

    public override void OnExit()
    {
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        sortBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Sort").gameObject);
        sortBtnLis.onClick += OnSortClicked;
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();
        scrollPanel = GetComponentInChildren<UIScrollView>().GetComponent<UIPanel>();
        Items = GetComponentInChildren<UIGrid>();
        offset = Utils.FindChild(transform, "Offset");
        panel = GetComponent<UIPanel>();
        cachedDepth = panel.depth;
    }

    private void OnSortClicked(GameObject go)
    {
        var orderType = ItemModeLocator.Instance.OrderType;
        orderType = (ItemHelper.OrderType)(((int)orderType + 1) % (StringTable.SortStrings.Count - 1));
        sortLabel.text = StringTable.SortStrings[(int)orderType];
        ItemModeLocator.Instance.OrderType = orderType;
        Refresh(infos);
    }

    /// <summary>
    /// Fill in the equip item game objects.
    /// </summary> 
    private void UpdateItemList(int itemCounts)
    {
        var childCount = Items.transform.childCount;
        if(childCount != itemCounts)
        {
            var isAdd = childCount < itemCounts;
            Utils.AddOrDelItems(Items.transform, ItemPrefab.transform, isAdd, Mathf.Abs(itemCounts - childCount),
                                "Heros",
                                null);
        }
        Items.repositionNow = true;
    }

    public void Refresh(List<ItemInfo> newInfos)
    {
        UpdateItemList(newInfos.Count);
        var orderType = ItemModeLocator.Instance.OrderType;
        sortLabel.text = StringTable.SortStrings[(int)orderType];
        ItemModeLocator.Instance.SortItemList(orderType, newInfos);
        for (var i = 0; i < newInfos.Count; i++)
        {
            var equipItem = Items.transform.GetChild(i).GetComponent<EquipItem>();
            equipItem.InitItem(newInfos[i]);
            ItemHelper.ShowItem(orderType, equipItem, newInfos[i]);
        }
        scrollPanel.GetComponent<UIScrollView>().ResetPosition();
    }

    public void DespawnItems(List<short> bagIndexs)
    {
        if(bagIndexs == null)
        {
            return;
        }
        var list = new List<Transform>();
        var childCount = Items.transform.childCount;
        for(var i = 0; i < bagIndexs.Count; i++)
        {
            var bagIndex = bagIndexs[i];
            for(int j = 0; j < childCount; j++)
            {
                var child = Items.transform.GetChild(j);
                if(child.GetComponent<EquipItem>().BagIndex == bagIndex)
                {
                    list.Add(child);
                }
            }
        }
        if(PoolManager.Pools.ContainsKey("Heros"))
        {
            for(int index = 0; index < list.Count; index++)
            {
                var item = list[index];
                UIEventListener.Get(item.gameObject).onClick = null;
                item.parent = PoolManager.Pools["Heros"].transform;
                PoolManager.Pools["Heros"].Despawn(item);
            }
        }
        Items.repositionNow = true;
    }

    public void ResetDepth()
    {
        Depth = cachedDepth;
    }

    #endregion
}

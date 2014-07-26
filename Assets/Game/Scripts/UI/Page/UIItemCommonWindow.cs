using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIItemCommonWindow : Window
{
    private UIEventListener sortBtnLis;
    private UILabel sortLabel;
    private UILabel itemNum;
    private List<ItemInfo> infos;
    private List<ItemInfo> cachedInfos;
    private UIToggle[] toggles;
    private UIEventListener.VoidDelegate normalClicked;

    [HideInInspector]
    public UIGrid Items;

    /// <summary>
    /// The prefab of the hero.
    /// </summary>
    public GameObject ItemPrefab;

    public UIEventListener.VoidDelegate NormalClicked
    {
        get { return normalClicked; }
        set
        {
            normalClicked = value;
            var parent = Items.transform;
            for (var i = 0; i < parent.childCount; i++)
            {
                var item = parent.GetChild(i);
                var longPressDetecter = item.GetComponent<NGUILongPress>();
                longPressDetecter.OnNormalPress = value;
            }
        }
    }

    #region Window

    public override void OnEnter()
    {
        infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos ?? new List<ItemInfo>();
        Refresh(infos);
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
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();
        itemNum = Utils.FindChild(transform, "ItemNumValue").GetComponent<UILabel>();
        Items = GetComponentInChildren<UIGrid>();
        toggles = transform.Find("ToggleButtons").GetComponentsInChildren<UIToggle>();
        var itemInfos = ItemModeLocator.Instance.ScAllItemInfos;
        cachedInfos = (infos != null && itemInfos.ItemInfos != null) ? itemInfos.ItemInfos : new List<ItemInfo>();
    }

    private void InstallHandlers()
    {
        for (var i = 0; i < toggles.Length; i++)
        {
            EventDelegate.Add(toggles[i].onChange, ExcuteFilter);
        }
        sortBtnLis.onClick = OnSortClicked;
    }

    private void UnInstallHandlers()
    {
        for (var i = 0; i < toggles.Length; i++)
        {
            EventDelegate.Remove(toggles[i].onChange, ExcuteFilter);
        }
        sortBtnLis.onClick = null;
    }

    private void OnSortClicked(GameObject go)
    {
        var orderType = ItemModeLocator.Instance.OrderType;
        orderType = (ItemHelper.OrderType)(((int)orderType + 1) % (StringTable.SortStrings.Count -1));
        sortLabel.text = StringTable.SortStrings[(int)orderType];
        ItemModeLocator.Instance.OrderType = orderType;
        Refresh(infos);
    }

    /// <summary>
    /// Fill in the item game objects.
    /// </summary> 
    private void UpdateItemList(int itemCount)
    {
        var childCount = Utils.GetActiveChildCount(Items.transform);
        if (childCount != itemCount)
        {
            var isAdd = childCount < itemCount;
            HeroUtils.AddOrDelItems(Items.transform, ItemPrefab.transform, isAdd, Mathf.Abs(itemCount - childCount),
                                HeroConstant.HeroPoolName,
                                null,
                                OnLongPress);
        }
        Items.repositionNow = true;
    }

    private void OnLongPress(GameObject go)
    {
        WindowManager.Instance.Show<UIItemDetailWindow>(true);
    }

    /// <summary>
    /// Filtering items with special job. 
    /// </summary>
    /// <remarks>
    /// This function can only be called in the callback of uitoggle's onChange, 
    /// as the UIToggle.current will be null in other places.
    /// </remarks>
    private void ExcuteFilter()
    {
        bool val = UIToggle.current.value;
        if (val)
        {
            var filter = UIToggle.current.GetComponent<ItemTypeFilterInfo>().Filter;
            cachedInfos = ItemHelper.FilterItems(infos, filter) ?? new List<ItemInfo>();
            var filterObjects = new List<Transform>();
            for (var i = 0; i < infos.Count; i++)
            {
                if (i < cachedInfos.Count)
                {
                    var item = Items.transform.GetChild(i).GetComponent<NewEquipItem>();
                    filterObjects.Add(item.transform);
                    NGUITools.SetActive(item.gameObject, true);
                }
                else
                {
                    var item = Items.transform.GetChild(i);
                    NGUITools.SetActive(item.gameObject, false);
                }
            }
            Items.repositionNow = true;
            for (var i = 0; i < filterObjects.Count; i++)
            {
                var item = filterObjects[i].GetComponent<NewEquipItem>();
                item.InitItem(infos[i]);
            }
        }
    }

    /// <summary>
    /// Refresh the heros page window with the special hero info list.
    /// </summary>
    /// <param name="newInfos">The hero info list to refresh data.</param>
    public void Refresh(List<ItemInfo> newInfos)
    {
        var capacity = ItemModeLocator.Instance.ScAllItemInfos.Capacity;
        itemNum.text = string.Format("{0}/{1}", newInfos.Count, capacity);
        UpdateItemList(newInfos.Count);
        var orderType = ItemModeLocator.Instance.OrderType;
        sortLabel.text = StringTable.SortStrings[(int)orderType];
        ItemModeLocator.Instance.SortItemList(orderType, newInfos);
        for (var i = 0; i < newInfos.Count; i++)
        {
            var equipItem = Items.transform.GetChild(i).GetComponent<NewEquipItem>();
            equipItem.InitItem(newInfos[i]);
            ItemHelper.ShowItem(orderType, equipItem, newInfos[i]);
        }
    }

    #endregion
}

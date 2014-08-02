using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using UnityEngine;
using OrderType = ItemHelper.OrderType;

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

    private UIScrollView cachedScrollView;
    private ItemHelper.EquipType curFilter = ItemHelper.EquipType.All;

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
        infos = ItemHelper.FilterItems(cachedInfos, curFilter);
        var orderType = ItemModeLocator.Instance.OrderType;
        ExcuteSort(orderType);
        ShowItems();
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
        cachedScrollView = NGUITools.FindInParents<UIScrollView>(Items.gameObject);
        toggles = transform.Find("ToggleButtons").GetComponentsInChildren<UIToggle>();
        cachedInfos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos;
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
        orderType = (OrderType)(((int)orderType + 1) % (ItemHelper.SortKeys.Count - 1));
        sortLabel.text = LanguageManager.Instance.GetTextValue(ItemHelper.SortKeys[(int)orderType]);
        ItemModeLocator.Instance.OrderType = orderType;
        ExcuteSort(orderType);
        var index = 0;
        for(int i = 0; i < Items.transform.childCount; i++)
        {
            var child = Items.transform.GetChild(i);
            if(NGUITools.GetActive(child.gameObject))
            {
                RefreshItem(child.GetComponent<NewEquipItem>(), infos[index]);
                index++;
            }
        }
    }

    /// <summary>
    /// Fill in the item game objects.
    /// </summary> 
    private void UpdateItemList(int itemCount)
    {
        var childCount = Items.transform.childCount;
        if (Items.transform.childCount != itemCount)
        {
            var isAdd = childCount < itemCount;
            HeroUtils.AddOrDelItems(Items.transform, ItemPrefab.transform, isAdd, Mathf.Abs(itemCount - childCount),
                                HeroConstant.HeroPoolName,
                                null,
                                OnLongPress);
        }
        NGUITools.SetActiveChildren(Items.gameObject, true);
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
        var val = UIToggle.current.value;
        if (val)
        {
            curFilter = UIToggle.current.GetComponent<ItemTypeFilterInfo>().Filter;
            if (curFilter != ItemHelper.EquipType.All)
            {
                cachedScrollView.ResetPosition();
                infos = ItemHelper.FilterItems(cachedInfos, curFilter);
                if(infos == null || infos.Count == 0)
                {
                    NGUITools.SetActiveChildren(Items.gameObject, false);
                    return;
                }
                var bags = infos.Select(t => t.BagIndex).ToList();
                var filterObjects = new List<Transform>();
                for (int i = 0; i < Items.transform.childCount; i++)
                {
                    var child = Items.transform.GetChild(i);
                    if (bags.Contains(child.GetComponent<NewEquipItem>().BagIndex))
                    {
                        filterObjects.Add(child);
                        NGUITools.SetActive(child.gameObject, true);
                    }
                    else
                    {
                        NGUITools.SetActive(child.gameObject, false);
                    }
                }
                Items.Reposition();
                ExcuteSort(ItemModeLocator.Instance.OrderType);
                for (int i = 0; i < filterObjects.Count; i++)
                {
                    var filterObject = filterObjects[i];
                    var item = filterObject.GetComponent<NewEquipItem>();
                    RefreshItem(item, infos[i]);
                }
                RefreshItemCount(filterObjects.Count);
            }
            else
            {
                infos = ItemHelper.FilterItems(cachedInfos, curFilter);
                var orderType = ItemModeLocator.Instance.OrderType;
                ExcuteSort(orderType);
                ShowItems();
            }
        }
    }

    private void ShowItems()
    {
        var count = infos == null ? 0 : infos.Count;
        UpdateItemList(count);
        Refresh(infos);
        StartCoroutine(Reposition());
    }

    private IEnumerator Reposition()
    {
        yield return null;
        Items.repositionNow = true;
    }

    private void ExcuteSort(OrderType orderType)
    {
        sortLabel.text = LanguageManager.Instance.GetTextValue(ItemHelper.SortKeys[(int)orderType]);
        ItemModeLocator.Instance.SortItemList(orderType, infos);
    }

    /// <summary>
    /// Refresh the heros page window with the special hero info list.
    /// </summary>
    /// <param name="newInfos">The hero info list to refresh data.</param>
    public void Refresh(List<ItemInfo> newInfos)
    {
        if(newInfos == null)
        {
            RefreshItemCount(0);
            return;
        }
        RefreshItemCount(newInfos.Count);
        for (var i = 0; i < newInfos.Count; i++)
        {
            var equipItem = Items.transform.GetChild(i).GetComponent<NewEquipItem>();
            RefreshItem(equipItem, newInfos[i]);
        }
    }

    private static void RefreshItem(NewEquipItem equipItem, ItemInfo info)
    {
        equipItem.InitItem(info);
        ItemHelper.ShowItem(ItemModeLocator.Instance.OrderType, equipItem, info);
    }

    /// <summary>
    /// Refresh the label of item count.
    /// </summary>
    /// <param name="count">The current count of item.</param>
    public void RefreshItemCount(int count)
    {
        var capacity = ItemModeLocator.Instance.ScAllItemInfos.Capacity;
        itemNum.text = string.Format("{0}/{1}", count, capacity);
    }

    #endregion
}

using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using UnityEngine;

public class HeroSelItem : MonoBehaviour 
{
    private List<ItemInfo> canEquipItems;
    private UIEventListener confirmLis;
    private UIEventListener cancelLis;
    private const int CountOfOneGroup = 4;
    private bool descendSort;
    public UIToggle DescendToggle;
    private UIEventListener sortBtnLis;
    private UILabel sortLabel;
    private short maskBagIndexCached;
    private GameObject lastMask;
    public CustomGrid Items;
    public UIEventListener.VoidDelegate ConfirmClicked;
    public UIEventListener.VoidDelegate CancelClicked;
    public UIEventListener.VoidDelegate EquipItemClicked;
    private Position curPos = HeroConstant.InvalidPos;
    private readonly List<short> otherBagIndexsCached = new List<short>(); 
    private readonly List<Position> otherPosList = new List<Position>(); 

    public void Refresh(string uuid, List<string> uuids)
    {
        canEquipItems = FilterItems();
        InitWrapContents(canEquipItems);
        var uuidList = canEquipItems.Select(item => item.Id).ToList();
        if(uuid !=  "")
        {
            curPos = Utils.OneDimToTwo(uuidList.IndexOf(uuid), CountOfOneGroup);
        }
        foreach (var uid in uuids)
        {
            if(uid != "")
            {
                otherPosList.Add(Utils.OneDimToTwo(uuidList.IndexOf(uid), CountOfOneGroup));
            }
        }
        var parent = Items.transform;
        for (var i = 0; i < parent.childCount; i++)
        {
            var item = parent.GetChild(i);
            for (var j = 0; j < item.childCount; j++)
            {
                var child = item.GetChild(j).gameObject;
                var activeCache = child.activeSelf;
                NGUITools.SetActive(child, true);
                var lis = UIEventListener.Get(child);
                lis.onClick = OnEquipItemClick;
                NGUITools.SetActive(child, activeCache);
            }
        }
    }

    private void Awake()
    {
        confirmLis = UIEventListener.Get(transform.Find("Buttons/Button-Confirm").gameObject);
        cancelLis = UIEventListener.Get(transform.Find("Buttons/Button-Cancel").gameObject);
        sortBtnLis = UIEventListener.Get(transform.Find("Buttons/Button-Sort").gameObject);
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();
    }

    private void OnEnable()
    {
        InstallHandlers();
    }

    private void OnDisable()
    {
        UnInstallHandlers();
    }

    private void InstallHandlers()
    {
        EventDelegate.Add(DescendToggle.onChange, SortTypeChanged);
        confirmLis.onClick += OnConfirm;
        cancelLis.onClick += OnCancel;
        sortBtnLis.onClick += OnSort;
        Items.OnUpdate += OnUpdate;
    }

    private void UnInstallHandlers()
    {
        EventDelegate.Remove(DescendToggle.onChange, SortTypeChanged);
        confirmLis.onClick -= OnConfirm;
        cancelLis.onClick -= OnCancel;
        sortBtnLis.onClick -= OnSort;
        Items.OnUpdate -= OnUpdate;
    }

    private void OnSort(GameObject go)
    {
        var orderType = ItemModeLocator.Instance.OrderType;
        orderType = (ItemHelper.OrderType)(((int)orderType + 1) % (ItemHelper.SortKeys.Count - 1));
        sortLabel.text = LanguageManager.Instance.GetTextValue(ItemHelper.SortKeys[(int)orderType]);
        ItemModeLocator.Instance.OrderType = orderType;
        InitWrapAndRecalculate();
    }

    private void OnCancel(GameObject go)
    {
        NGUITools.Destroy(gameObject);
        if (CancelClicked != null)
        {
            CancelClicked(go);
        }
    }

    private void OnConfirm(GameObject go)
    {
        NGUITools.Destroy(gameObject);
        if (ConfirmClicked != null)
        {
            ConfirmClicked(go);
        }
    }

    private void OnUpdate(GameObject sender, int index)
    {
        var wrapItem = sender.GetComponent<WrapItemContent>();
        var col = wrapItem.Children.Count;
        for (var i = 0; i < col; i++)
        {
            var pos = new Position { X = index, Y = i };
            var showEquip = curPos == pos;
            if(showEquip)
            {
                lastMask = wrapItem.ShowEquipMask(i, true);
            }
            else
            {
                wrapItem.ShowEquipMask(i, false);
            }
            var showMask = otherPosList.Contains(pos);
            wrapItem.ShowMask(i, showMask);
        }
    }

    private void SortTypeChanged()
    {
        descendSort = DescendToggle.value;
        InitWrapAndRecalculate();
    }

    private void InitWrapAndRecalculate()
    {
        var needCaculate = curPos != HeroConstant.InvalidPos;
        if (needCaculate)
        {
            maskBagIndexCached = GetInfo(curPos).BagIndex;
        }
        otherBagIndexsCached.Clear();
        foreach(var pos in otherPosList)
        {
            otherBagIndexsCached.Add(GetInfo(pos).BagIndex);
        }
        InitWrapContents(canEquipItems);
        if (needCaculate)
        {
            var index = canEquipItems.FindIndex(info => info.BagIndex == maskBagIndexCached);
            curPos = Utils.OneDimToTwo(index, CountOfOneGroup);
        }
        otherPosList.Clear();
        foreach (var bag in otherBagIndexsCached)
        {
            var index = canEquipItems.FindIndex(info => info.BagIndex == bag);
            var pos = Utils.OneDimToTwo(index, CountOfOneGroup);
            otherPosList.Add(pos);
        }
        RefreshMask();
    }

    private void RefreshMask()
    {
        var childCount = Items.transform.childCount;
        for (var i = 0; i < childCount; i++)
        {
            var wrapItem = Items.transform.GetChild(i).GetComponent<WrapItemContent>();
            for (var j = 0; j < wrapItem.Children.Count; j++)
            {
                var pos = new Position { X = wrapItem.Row, Y = j };
                var showMask = curPos == pos;
                wrapItem.ShowEquipMask(j, showMask);
                showMask = otherPosList.Contains(pos);
                wrapItem.ShowMask(j, showMask);
            }
        }
    }

    public ItemInfo GetInfo(Position pos)
    {
        var oneDimsionIndex = pos.X * CountOfOneGroup + pos.Y;
        return canEquipItems[oneDimsionIndex];
    }

    private void OnEquipItemClick(GameObject go)
    {
        NGUITools.SetActive(lastMask, false);
        var itemContent = go.transform.parent.GetComponent<WrapItemContent>();
        var pos = new Position { X = itemContent.Row, Y = itemContent.Children.IndexOf(go.transform) };
        var isToCancelEquip = curPos == pos;
        if (!isToCancelEquip)
        {
            lastMask = itemContent.ShowEquipMask(pos.Y, true);
            curPos = pos;
        }
        else
        {
            curPos = HeroConstant.InvalidPos;
        }
        if(EquipItemClicked != null)
        {
            EquipItemClicked(isToCancelEquip ? null : go);
        }
    }

    /// <summary>
    /// Filter all items to exclude material items.
    /// </summary>
    /// <returns>The item info list of all items after filtering.</returns>
    private List<ItemInfo> FilterItems()
    {
        if (ItemModeLocator.Instance.ScAllItemInfos.ItemInfos != null)
        {
            return
                ItemModeLocator.Instance.ScAllItemInfos.ItemInfos.FindAll(
                    item => ItemModeLocator.Instance.GetItemType(item.TmplId) != ItemHelper.EquipType.Material);
        }
        return new List<ItemInfo>();
    }

    private void InitWrapContents(List<ItemInfo> itemInfos)
    {
        if (itemInfos == null) return;
        var orderType = ItemModeLocator.Instance.OrderType;
        ItemModeLocator.Instance.SortItemList(orderType, itemInfos, descendSort);
        var list = new List<List<ItemInfo>>();
        var totalCount = itemInfos.Count;
        var rows = Mathf.CeilToInt((float)totalCount / CountOfOneGroup);
        for (var i = 0; i < rows; i++)
        {
            var infosContainer = new List<ItemInfo>();
            for (var j = 0; j < CountOfOneGroup; j++)
            {
                var oneDim = i * CountOfOneGroup + j;
                if (oneDim < totalCount)
                {
                    infosContainer.Add(itemInfos[oneDim]);
                }
            }
            list.Add(infosContainer);
        }
        Items.Init(list);
    }
}

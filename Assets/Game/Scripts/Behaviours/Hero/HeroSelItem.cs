using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using UnityEngine;

public class HeroSelItem : MonoBehaviour 
{
    public CustomGrid Items;
    public HeroSortControl SortControl;
    public UIEventListener.VoidDelegate ConfirmClicked;
    public UIEventListener.VoidDelegate EquipItemClicked;

    private List<ItemInfo> canEquipItems;
    private UIEventListener confirmLis;
    private const int CountOfOneGroup = 4;
    private short maskBagIndexCached;
    private GameObject lastMask;
    private Position curPos = HeroConstant.InvalidPos;
    private readonly List<short> otherBagIndexsCached = new List<short>(); 
    private readonly List<Position> otherPosList = new List<Position>();
    private bool isFirst = true;

    public void Refresh(string uuid, List<string> uuids)
    {
        if (isFirst)
        {
            FilterItems();
            SortControl.Init(canEquipItems);
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
            isFirst = false;
        }
        if(canEquipItems == null)
        {
            return;
        }
        var uuidList = canEquipItems.Where(item => item != null).Select(item => item.Id).ToList();
        if(uuid !=  "")
        {
            curPos = Utils.OneDimToTwo(uuidList.IndexOf(uuid), CountOfOneGroup);
        }
        otherPosList.Clear();
        foreach (var uid in uuids)
        {
            if(uid != "")
            {
                otherPosList.Add(Utils.OneDimToTwo(uuidList.IndexOf(uid), CountOfOneGroup));
            }
        }
        RefreshMask();
    }

    private void Awake()
    {
        confirmLis = UIEventListener.Get(transform.Find("Buttons/Button-Confirm").gameObject);
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
        confirmLis.onClick += OnConfirm;
        Items.OnUpdate += OnUpdate;
        SortControl.InstallHandlers();
        SortControl.OnSortOrderChangedBefore += SortBefore;
        SortControl.OnSortOrderChangedAfter += SortAfter;
        SortControl.OnExcuteAfterSort += OnExcuteAfterSort;
    }

    private void UnInstallHandlers()
    {
        confirmLis.onClick -= OnConfirm;
        Items.OnUpdate -= OnUpdate;
        SortControl.UnInstallHandlers();
        SortControl.OnSortOrderChangedBefore -= SortBefore;
        SortControl.OnSortOrderChangedAfter -= SortAfter;
        SortControl.OnExcuteAfterSort -= OnExcuteAfterSort;
    }

    private void SortBefore()
    {
        var needCaculate = curPos != HeroConstant.InvalidPos;
        if (needCaculate)
        {
            maskBagIndexCached = GetInfo(curPos).BagIndex;
        }
        otherBagIndexsCached.Clear();
        foreach (var pos in otherPosList)
        {
            otherBagIndexsCached.Add(GetInfo(pos).BagIndex);
        }
    }

    private void SortAfter()
    {
        var needCaculate = curPos != HeroConstant.InvalidPos;
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

    private void OnExcuteAfterSort()
    {
        ItemHelper.InitWrapContents(Items, canEquipItems, CountOfOneGroup, ItemModeLocator.Instance.ScAllItemInfos.Capacity);
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
    private void FilterItems()
    {
        var mainInfo = WindowManager.Instance.GetWindow<UIHeroCommonWindow>().HeroInfo;
        if(ItemModeLocator.Instance.ScAllItemInfos.ItemInfos != null && mainInfo != null)
        {
            var mainJob = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[mainInfo.TemplateId].Job;
            canEquipItems =
                ItemModeLocator.Instance.ScAllItemInfos.ItemInfos.FindAll(
                    item =>
                    ItemModeLocator.Instance.GetItemType(item.TmplId) == ItemHelper.EquipType.Armor ||
                    (ItemModeLocator.Instance.GetItemType(item.TmplId) == ItemHelper.EquipType.Equip &&
                     ItemModeLocator.Instance.GetJob(item.TmplId) == mainJob));
        }
    }
}

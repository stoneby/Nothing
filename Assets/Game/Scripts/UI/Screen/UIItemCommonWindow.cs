using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIItemCommonWindow : Window
{
    public GameObject BaseItemPrefab;
    public CustomGrid Items;
    public HeroSortControl SortControl;
    public CloseButtonControl CloseButtonControl;
    public GameObject ExtendBagConfirm;
    public UIItemDetailHandler ItemDetailHandler;
    public UISellItemHandler ItemSellHandler;
    public UILevelUpItemHandler LevelUpItemHandler;
    public UIEvolveItemHandler EvolveItemHandler;
    public List<ItemInfo> Infos { get; private set; }
    public ItemInfo MainInfo { get; private set; }
    public int CountOfOneGroup = 4;
    /// <summary>
    /// If the item level is greater than this star count, we will show confirm dialog.
    /// </summary>
    public int ConfirmStar = 3;
    public string EvolveAndLevelColor = "[ffff00]";
    public const string ColorEnd = "[-]";
    public delegate void SortOrderChanged(List<ItemInfo> hInfos);

    private ExtendBag itemExtendConfirm;
    private UIEventListener extendLis;
    private UILabel itemNum;
    private UIEventListener.VoidDelegate normalClicked;
    private int itemCellLength = 120;
    private bool isTriggerByStart = true;
    private Position curSelPos = HeroConstant.InvalidPos;
    public Position CurSelPos
    {
        get { return curSelPos; }
        set
        {
            var oneDim = value.X * CountOfOneGroup + value.Y;
            if(Infos != null && Infos.Count > oneDim && oneDim >= 0)
            {
                MainInfo = Infos[oneDim];
                curSelPos = value;
                var localPos = new Vector3(CurSelPos.Y * itemCellLength, -CurSelPos.X * itemCellLength, 0);
                selMask.transform.position = Items.transform.TransformPoint(localPos);
            }
            else
            {
                MainInfo = null;
                curSelPos = HeroConstant.InvalidPos;
                ShowSelMask(false);
            }
        }
    }

    private short bagIndexCached;
    private Transform selMask;
    private readonly Position defaultPos = HeroConstant.FirstPos;

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
                if (item.name != "ExtendButton")
                {
                    for(var j = 0; j < item.childCount; j++)
                    {
                        var hero = item.GetChild(j).gameObject;
                        var activeCache = hero.activeSelf;
                        NGUITools.SetActive(hero, true);
                        var longPressHandler = hero.GetComponent<ItemLongPressHandler>();
                        if (longPressHandler)
                        {
                            var longPress = longPressHandler.InstallLongPress();
                            longPress.OnNormalPress = value;
                        }
                        NGUITools.SetActive(hero, activeCache);
                    }
                }
            }
        }
    }

    #region Window

    private void InitWrapContents(List<ItemInfo> itemInfos)
    {
        RefreshItemMaxNum();
        RepositionMaxExtendBtn();
        if(itemInfos == null || itemInfos.Count == 0)
        {
            ItemHelper.HideItems(Items.transform);
            return;
        }
        ItemHelper.InitWrapContents(Items, itemInfos, CountOfOneGroup, ItemModeLocator.Instance.ScAllItemInfos.Capacity);
    }

    public override void OnEnter()
    {
        GlobalWindowSoundController.Instance.PlayOpenSound();
        Infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos;
        SortControl.Init(Infos);
        InitWrapContents(Infos);
        InstallHandlers();
        if (Infos != null && Infos.Count > 0)
        {
            ShowSelMask(true);
            if (curSelPos == HeroConstant.InvalidPos)
            {
                CurSelPos = defaultPos;
            }
        }
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        WindowManager.Instance.Show<UIMainScreenWindow>(true);
        CurSelPos = HeroConstant.InvalidPos;
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    void Awake()
    {
        itemNum = Utils.FindChild(transform, "ItemNumValue").GetComponent<UILabel>();
        NGUITools.SetActive(Items.transform.parent.gameObject, true);
        selMask = transform.Find("SelMask");
        selMask.transform.parent = Items.transform.parent;
        NGUITools.SetActive(selMask.gameObject, false);
        extendLis = UIEventListener.Get(Items.transform.Find("ExtendButton/Icon").gameObject);
    }

    private void InstallHandlers()
    {
        CloseButtonControl.OnCloseWindow = OnClose;
        extendLis.onClick = OnExtend;
        SortControl.InstallHandlers();
        SortControl.OnSortOrderChangedBefore += SortBefore;
        SortControl.OnSortOrderChangedAfter += SortAfter;
        SortControl.OnExcuteAfterSort += OnExcuteAfterSort;
    }

    private void UnInstallHandlers()
    {
        CloseButtonControl.OnCloseWindow = null;
        extendLis.onClick = null;
        SortControl.UnInstallHandlers();
        SortControl.OnSortOrderChangedBefore -= SortBefore;
        SortControl.OnSortOrderChangedAfter -= SortAfter;
        SortControl.OnExcuteAfterSort -= OnExcuteAfterSort;
    }

    private void SortBefore()
    {
        if (CurSelPos.X >= 0 && CurSelPos.Y >= 0)
        {
            var cachedInfo = GetInfo(CurSelPos);
            if (cachedInfo != null)
            {
                bagIndexCached = cachedInfo.BagIndex;
            }
        }
    }

    private void OnExcuteAfterSort()
    {
        InitWrapContents(Infos);
    }

    private void SortAfter()
    {
        if (CurSelPos.X >= 0 && CurSelPos.Y >= 0 && Infos != null)
        {
            var newIndex = Infos.FindIndex(info => info.BagIndex == bagIndexCached);
            CurSelPos = Utils.OneDimToTwo(newIndex, CountOfOneGroup);
        }
        if (isTriggerByStart)
        {
            CurSelPos = defaultPos;
            isTriggerByStart = false;
        }
    }

    private void OnExtend(GameObject go)
    {
        if (ItemModeLocator.Instance.Bag.ItemExtTmpls.Count - PlayerModelLocator.Instance.ExtendItemTimes != 0)
        {
            itemExtendConfirm = NGUITools.AddChild(transform.gameObject, ExtendBagConfirm).GetComponent<ExtendBag>();
            itemExtendConfirm.ExtendContentKey = ItemType.ExtendContentKey;
            itemExtendConfirm.ExtendLimitKey = ItemType.ExtendLimitKey;
            var bases = ItemModeLocator.Instance.Bag;
            var costDict = bases.ItemExtTmpls.ToDictionary(item => item.Value.Id, item => item.Value.ExtendCost);
            itemExtendConfirm.Init(PlayerModelLocator.Instance.ExtendItemTimes, bases.BagBaseTmpls[1].ExtendItemCount,
                                   costDict);
            itemExtendConfirm.OkClicked += OnExtendBagOk;
        }
        else
        {
            PopTextManager.PopTip("可拥有道具数已达上限", false);
        }
    }

    private void OnExtendBagOk(GameObject go)
    {
        var msg = new CSExtendItemBag() { ExtendSize = itemExtendConfirm.ExtendSize };
        NetManager.SendMessage(msg);
    }

    private void RepositionMaxExtendBtn()
    {
        var count = Infos == null ? 0 : Infos.Count;
        var pos = Utils.OneDimToTwo(count, CountOfOneGroup);
        extendLis.transform.parent.localPosition = new Vector3(pos.Y * itemCellLength, -pos.X * itemCellLength, 0);
    }

    public void RefreshItemMaxNum()
    {
        var count = (Infos == null ? 0 : Infos.Count);
        itemNum.text = string.Format("{0}/{1}", count, ItemModeLocator.Instance.ScAllItemInfos.Capacity);
        Logger.Log("Extend item size to:" + count + "/" + ItemModeLocator.Instance.ScAllItemInfos.Capacity);
    }

    private void OnClose()
    {
        WindowManager.Instance.Show<UIMainScreenWindow>(true);
    }


    /// <summary>
    /// Refresh the heros page window with the special hero info list.
    /// </summary>
    /// <param name="newInfos">The hero info list to refresh data.</param>
    public void Refresh(List<ItemInfo> newInfos)
    {
        Infos = newInfos;
        RefreshItemCount(newInfos != null ? newInfos.Count : 0);
        InitWrapContents(newInfos);
    }

    /// <summary>
    /// Refresh the heros page window with the special hero info list.
    /// </summary>
    public void Refresh()
    {
        Refresh(Infos);
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

    public void ShowSelMask(bool show)
    {
        NGUITools.SetActive(selMask.gameObject, show);
    }

    public ItemInfo GetInfo(Position pos)
    {
        var oneDimsionIndex = Utils.TwoDimToOne(pos, CountOfOneGroup);
        if (Infos == null || Infos.Count <= oneDimsionIndex)
        {
            return null;
        }
        return Infos[oneDimsionIndex];
    }

    #endregion
}

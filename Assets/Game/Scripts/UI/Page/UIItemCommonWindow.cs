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
    public GameObject ExtendBagConfirm;
    private ExtendBag itemExtendConfirm;
    private UIEventListener extendLis;

    private UIEventListener closeBtnLis;
    private StretchItem closeBtnLine;
    private UIEventListener sortBtnLis;
    private UILabel sortLabel;
    private UILabel itemNum;
    public List<ItemInfo> Infos { get; private set; }
    private UIEventListener.VoidDelegate normalClicked;
    private int itemCellLength = 120;
    private bool triggeredByStart = true;

    public UIItemDetailHandler ItemDetailHandler;
    public UISellItemHandler ItemSellHandler;
    public UILevelUpItemHandler LevelUpItemHandler;

    public ItemInfo MainInfo { get; private set; }

    private Position curSelPos = new Position { X = -1, Y = -1 };
    public Position CurSelPos
    {
        get { return curSelPos; }
        set
        {
            if (curSelPos != value)
            {
                curSelPos = value;
                var oneDim = value.X * CountOfOneGroup + value.Y;
                if(Infos != null && Infos.Count > oneDim)
                {
                    MainInfo = Infos[oneDim];
                }
            }
        }
    }

    private bool descendSort;
    private readonly Position defaultPos = new Position { X = 0, Y = 0 };

    public delegate void SortOrderChanged(List<ItemInfo> hInfos);
    private short bagIndexCached;

    /// <summary>
    /// The call back of the sort order changed.
    /// </summary>
    public SortOrderChanged OnSortOrderChangedBefore;
    public SortOrderChanged OnSortOrderChangedAfter;

    public CustomGrid Items;
    public UIToggle DescendToggle;
    private Transform selMask;
    private bool isEntered;

    /// <summary>
    /// The prefab of the hero.
    /// </summary>
    public GameObject ItemPrefab;
    public int CountOfOneGroup = 4;

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
                if (item.name != "ZZZForSort_AddItem")
                {
                    for (var j = 0; j < item.childCount; j++)
                    {
                        var hero = item.GetChild(j).gameObject;
                        var activeCache = hero.activeSelf;
                        NGUITools.SetActive(hero, true);
                        var lis = UIEventListener.Get(hero);
                        lis.onClick = value;
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
        if (itemInfos == null || itemInfos.Count == 0)
        {
            return;
        }
        var orderType = HeroModelLocator.Instance.OrderType;
        ItemModeLocator.Instance.SortItemList(orderType, Infos, descendSort);
        var list = new List<List<ItemInfo>>();
        var rows = Mathf.CeilToInt((float)itemInfos.Count / CountOfOneGroup);
        for (var i = 0; i < rows; i++)
        {
            var infosContainer = new List<ItemInfo>();
            for (var j = 0; j < CountOfOneGroup; j++)
            {
                if (i * CountOfOneGroup + j < Infos.Count)
                {
                    infosContainer.Add(itemInfos[i * CountOfOneGroup + j]);
                }
            }
            list.Add(infosContainer);
        }
        Items.Init(list);
        RepositionMaxExtendBtn();
    }

    public override void OnEnter()
    {
        Infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos;
        isEntered = true;
        InitWrapContents(Infos);
        InstallHandlers();
        if (Infos != null && Infos.Count > 0)
        {
            ShowSelMask(Infos != null && Infos.Count > 0);
            RefreshSelMask(defaultPos);
        }
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
        var buttons = transform.Find("Buttons");
        extendLis = UIEventListener.Get(Utils.FindChild(transform, "AddItem").gameObject);
        sortBtnLis = UIEventListener.Get(buttons.Find("Button-Sort").gameObject);
        closeBtnLis = UIEventListener.Get(buttons.Find("Button-Close").gameObject);
        closeBtnLine = buttons.Find("Button-CloseLine").GetComponent<StretchItem>();
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();
        itemNum = Utils.FindChild(transform, "ItemNumValue").GetComponent<UILabel>();
        NGUITools.SetActive(Items.transform.parent.gameObject, true);
        selMask = transform.Find("SelMask");
        selMask.transform.parent = Items.transform.parent;
        NGUITools.SetActive(selMask.gameObject, false);
    }

    private void InstallHandlers()
    {
        EventDelegate.Add(DescendToggle.onChange, SortTypeChanged);
        extendLis.onClick = OnExtend;
        sortBtnLis.onClick = OnSortClicked;
        closeBtnLis.onClick = OnClose;
        closeBtnLine.DragFinish = OnClose;
    }

    private void UnInstallHandlers()
    {
        EventDelegate.Add(DescendToggle.onChange, SortTypeChanged);
        extendLis.onClick = null;
        sortBtnLis.onClick = null;
        closeBtnLis.onClick = null;
        closeBtnLine.DragFinish = null;
    }

    private void SortTypeChanged()
    {
        descendSort = DescendToggle.value;
        if (isEntered)
        {
            if (triggeredByStart)
            {
                InitWrapContents(Infos);
                return;
            }
            SortBefore();
            InitWrapContents(Infos);
            SortAfter();
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
        Utils.FindChild(transform, "Grid").GetComponent<UIGrid>().Reposition();
        var addItemBtn = Utils.FindChild(transform, "ZZZForSort_AddItem");
        switch (Infos.Count % CountOfOneGroup)
        {
            case 0:
                break;
            case 1:
                addItemBtn.localPosition += new Vector3(itemCellLength * 1, itemCellLength, 0);
                break;
            case 2:
                addItemBtn.localPosition += new Vector3(itemCellLength * 2, itemCellLength, 0);
                break;
            case 3:
                addItemBtn.localPosition += new Vector3(itemCellLength * 3, itemCellLength, 0);
                break;
        }
    }

    public void RefreshItemMaxNum()
    {
        var count = (Infos == null ? 0 : Infos.Count);
        itemNum.text = string.Format("{0}/{1}", count, ItemModeLocator.Instance.ScAllItemInfos.Capacity);
        Logger.Log("Extend item size to:" + count + "/" + ItemModeLocator.Instance.ScAllItemInfos.Capacity);
    }

    private void OnClose(GameObject go)
    {
        WindowManager.Instance.Show<UIItemCommonWindow>(false);
    }

    private void OnSortClicked(GameObject go)
    {
        SortBefore();
        var orderType = ItemModeLocator.Instance.OrderType;
        orderType = (OrderType)(((int)orderType + 1) % (ItemHelper.SortKeys.Count - 1));
        sortLabel.text = LanguageManager.Instance.GetTextValue(ItemHelper.SortKeys[(int)orderType]);
        ItemModeLocator.Instance.OrderType = orderType;
        InitWrapContents(Infos);
        SortAfter();
    }

    private void SortBefore()
    {
        if (OnSortOrderChangedBefore != null)
        {
            OnSortOrderChangedBefore(Infos);
        }
        if(CurSelPos.X >= 0 && CurSelPos.Y >= 0)
        {
            var cachedInfo = GetInfo(CurSelPos);
            if(cachedInfo != null)
            {
                bagIndexCached = cachedInfo.BagIndex;
            }
        }
    }

    private void SortAfter()
    {
        if(OnSortOrderChangedAfter != null)
        {
            OnSortOrderChangedAfter(Infos);
        }
        if(CurSelPos.X >= 0 && CurSelPos.Y >= 0 && Infos != null)
        {
            var newIndex = Infos.FindIndex(info => info.BagIndex == bagIndexCached);
            CurSelPos = new Position {X = newIndex / CountOfOneGroup, Y = newIndex % CountOfOneGroup};
            RefreshSelMask(CurSelPos);
        }
    }

    public void RefreshSelMask(Position position)
    {
        CurSelPos = defaultPos;
        var heros = Items.transform;
        var childCount = heros.childCount;
        for (var i = 0; i < childCount; i++)
        {
            var wrapItemContent = heros.GetChild(i).GetComponent<WrapItemContent>();
            var found = false;
            if (wrapItemContent != null && wrapItemContent.gameObject.activeSelf)
            {
                for (var j = 0; j < wrapItemContent.Children.Count; j++)
                {
                    var pos = new Position { X = wrapItemContent.Row, Y = j };
                    found = pos == position;
                    if (found)
                    {
                        var selObj = wrapItemContent.Children[pos.Y].gameObject;
                        ShowSelMask(selObj.transform.position);
                        break;
                    }
                }
            }
            if (found)
            {
                break;
            }
        }
    }

    /// <summary>
    /// Refresh the heros page window with the special hero info list.
    /// </summary>
    /// <param name="newInfos">The hero info list to refresh data.</param>
    public void Refresh(List<ItemInfo> newInfos)
    {
        RefreshItemCount(newInfos != null ? newInfos.Count : 0);
        InitWrapContents(newInfos);
        RefreshSelMask(CurSelPos);
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

    public void ShowSelMask(Vector3 pos)
    {
        selMask.position = pos;
        ShowSelMask(true);
    }

    public void ShowSelMask(bool show)
    {
        NGUITools.SetActive(selMask.gameObject, show);
    }

    public ItemInfo GetInfo(Position pos)
    { 
        var oneDimsionIndex = pos.X * CountOfOneGroup + pos.Y;
        if (Infos == null || Infos.Count <= oneDimsionIndex)
        {
            return null;
        }
        return Infos[oneDimsionIndex];
    }

    #endregion
}

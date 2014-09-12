using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UISellItemHandler : MonoBehaviour
{
    private UIItemCommonWindow commonWindow;
    private UILabel selCount;
    private UILabel moneyCount;
    private const int MaxSellCount = 12;
    private const int HighQuality = 3;
    private long totalMoney;
    private UIEventListener sellLis;
    private UIGrid grid;
    private readonly List<short> cachedSelBagIndexs = new List<short>();
    private readonly Dictionary<Position, GameObject> sellObjects = new Dictionary<Position, GameObject>();

    private long TotalMoney
    {
        get { return totalMoney; }
        set
        {
            totalMoney = value;
            moneyCount.text = totalMoney.ToString();
        }
    }

    #region Window

    private void OnEnable()
    {
        MtaManager.TrackBeginPage(MtaType.SellItemWindow);
        commonWindow = WindowManager.Instance.GetWindow<UIItemCommonWindow>();
        commonWindow.ShowSelMask(false);
        commonWindow.NormalClicked = OnNormalClick;
        RefreshSelAndCoins();
        InstallHandlers();
        sellLis.GetComponent<UIButton>().isEnabled = false;
    }

    private void OnDisable()
    {
        MtaManager.TrackEndPage(MtaType.SellItemWindow);
        UnInstallHandlers();
        CleanUp();
    }

    private void OnNormalClick(GameObject go)
    {
        var pos = GetPosition(go);
        var isToSell = !sellObjects.ContainsKey(pos);
        if ((isToSell && sellObjects.Count >= MaxSellCount) || !CanSell(commonWindow.GetInfo(pos)))
        {
            return;
        }
        var bagIndex = go.GetComponent<ItemBase>().BagIndex;
        var itemInfo = ItemModeLocator.Instance.FindItem(bagIndex);
        if (isToSell)
        {
            var child = NGUITools.AddChild(grid.gameObject, commonWindow.BaseItemPrefab);
            var heroBase = child.GetComponent<ItemBase>();
            heroBase.InitItem(itemInfo);
            UIEventListener.Get(child.gameObject).onClick = CancelItemSell;
            sellObjects.Add(pos, child);
        }
        else
        {
            RemoveSellObject(pos);
        }
        var flag = isToSell ? 1 : -1;
        var tempId = itemInfo.TmplId;
        var sellCost = ItemModeLocator.Instance.GetSellCost(tempId, itemInfo.CurExp, itemInfo.Level,
                                                            ItemModeLocator.Instance.GetQuality(tempId));
        TotalMoney += flag * sellCost;
        RefreshSelAndCoins();
        var item = NGUITools.FindInParents<WrapItemContent>(go);
        item.ShowSellMask(pos.Y, isToSell);
        var exitsToSell = sellObjects.Count != 0;
        sellLis.GetComponent<UIButton>().isEnabled = exitsToSell;
        grid.repositionNow = true;
    }

    public static Position GetPosition(GameObject go)
    {
        var item = NGUITools.FindInParents<WrapItemContent>(go);
        var row = item.Row;
        var col = item.Children.IndexOf(go.transform);
        return new Position { X = row, Y = col };
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        grid = transform.Find("Grid").GetComponent<UIGrid>();
        selCount = transform.Find("SelCount/SelValue").GetComponent<UILabel>();
        moneyCount = transform.Find("CoinsCount/CoinsValue").GetComponent<UILabel>();
        var buttons = transform.Find("Buttons");
        sellLis = UIEventListener.Get(buttons.Find("Button-Sell").gameObject);
    }

    private void InstallHandlers()
    {
        sellLis.onClick = OnSell;
        commonWindow.Items.OnUpdate += OnUpdate;
        commonWindow.SortControl.OnSortOrderChangedBefore += OnSortOrderChangedBefore;
        commonWindow.SortControl.OnSortOrderChangedAfter += OnSortOrderChangedAfter;
    }

    private void UnInstallHandlers()
    {
        sellLis.onClick = null;
        commonWindow.Items.OnUpdate -= OnUpdate;
        commonWindow.SortControl.OnSortOrderChangedBefore -= OnSortOrderChangedBefore;
        commonWindow.SortControl.OnSortOrderChangedAfter -= OnSortOrderChangedAfter;
    }

    private void OnUpdate(GameObject sender, int index)
    {
        var wrapItem = sender.GetComponent<WrapItemContent>();
        var col = wrapItem.Children.Count;
        for (var i = 0; i < col; i++)
        {
            var pos = new Position { X = index, Y = i };
            var show = sellObjects.Keys.Contains(pos);
            wrapItem.ShowSellMask(i, show);
        }
    }

    private void OnSortOrderChangedAfter()
    {
        var hInfos = commonWindow.Infos;
        var countPerGroup = commonWindow.CountOfOneGroup;
        var posList = GetPositionsViaBagIndex(cachedSelBagIndexs, hInfos, countPerGroup);
        var values = sellObjects.Values.ToList();
        sellObjects.Clear();
        for (var i = 0; i < posList.Count; i++)
        {
            sellObjects.Add(posList[i], values[i]);
        }
        UpdateSellMasks();
    }

    private void OnSortOrderChangedBefore()
    {
        CacheBagIndexsFromPos(sellObjects.Keys, cachedSelBagIndexs);
    }

    private void UpdateSellMasks()
    {
        var items = commonWindow.Items.transform;
        var childCount = items.childCount;
        for (var i = 0; i < childCount; i++)
        {
            var wrapitem = items.GetChild(i).GetComponent<WrapItemContent>();
            if(wrapitem)
            {
                for (var j = 0; j < wrapitem.Children.Count; j++)
                {
                    var pos = new Position { X = wrapitem.Row, Y = j };
                    var showSellMask = sellObjects.ContainsKey(pos);
                    wrapitem.ShowSellMask(j, showSellMask);
                }
            }
        }
    }

    private void CacheBagIndexsFromPos(IEnumerable<Position> list, List<short> bagIndexs)
    {
        bagIndexs.Clear();
        bagIndexs.AddRange(list.Select(pos => commonWindow.GetInfo(pos).BagIndex));
    }

    public static List<Position> GetPositionsViaBagIndex(IEnumerable<short> bags, List<ItemInfo> hInfos, int countPerGroup)
    {
        return bags.Select(bagIndex => hInfos.FindIndex(info => info.BagIndex == bagIndex)).Select(
            index => new Position { X = index / countPerGroup, Y = index % countPerGroup }).ToList();
    }

    private void OnSell(GameObject go)
    {
        var list = GetSellBagIndexs();
        if (ContainsRare(list))
        {
            var assertWindow = WindowManager.Instance.GetWindow<AssertionWindow>();
            assertWindow.AssertType = AssertionWindow.Type.OkCancel;
            assertWindow.Message = "";
            assertWindow.Title = LanguageManager.Instance.GetTextValue(ItemType.SellConfirmKey);
            assertWindow.OkButtonClicked = OnSellConfirmOk;
            assertWindow.CancelButtonClicked = OnSellConfirmCancel;
            WindowManager.Instance.Show(typeof(AssertionWindow), true);
        }
        else
        {
            var msg = new CSSellItems { SellItemIndexes = list };
            NetManager.SendMessage(msg);
        }
    }

    private void OnSellConfirmCancel(GameObject sender)
    {
        CleanSellConfirm();
    }

    private void OnSellConfirmOk(GameObject sender)
    {
        CleanSellConfirm();
        var msg = new CSSellItems { SellItemIndexes = GetSellBagIndexs() };
        NetManager.SendMessage(msg);
    }

    private List<short> GetSellBagIndexs()
    {
        return sellObjects.Select(sellObject => commonWindow.GetInfo(sellObject.Key).BagIndex).ToList();
    }

    private bool ContainsRare(List<short> bagIndexs)
    {
        for (int i = 0; i < bagIndexs.Count; i++)
        {
            var itemInfo = ItemModeLocator.Instance.FindItem(bagIndexs[i]);
            if (itemInfo != null)
            {
                if (ItemModeLocator.Instance.GetQuality(itemInfo.TmplId) >= HighQuality)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void CleanSellConfirm()
    {
        WindowManager.Instance.Show(typeof(AssertionWindow), false);
    }

    private bool CanSell(ItemInfo info)
    {
        return (info.BindStatus == 0 && info.EquipStatus == 0);
    }

    private void CancelItemSell(GameObject go)
    {
        var pos = new Position();
        foreach (var teamObj in sellObjects)
        {
            if (teamObj.Value == go)
            {
                pos = teamObj.Key;
            }
        }
        RemoveSellObject(pos);
        RefreshCurScreen();
        grid.repositionNow = true;
    }

    private void RemoveSellObject(Position pos)
    {
        var clone = sellObjects[pos];
        NGUITools.Destroy(clone);
        sellObjects.Remove(pos);
    }

    private void RefreshCurScreen()
    {
        var items = commonWindow.Items.transform;
        var childCount = items.childCount;
        for (var i = 0; i < childCount; i++)
        {
            var wrapItem = items.GetChild(i).GetComponent<WrapItemContent>();
            if (wrapItem)
            {
                for (var j = 0; j < wrapItem.Children.Count; j++)
                {
                    var pos = new Position { X = wrapItem.Row, Y = j };
                    var showSellMask = sellObjects.ContainsKey(pos);
                    wrapItem.ShowSellMask(j, showSellMask);
                }
            }
        }
    }

    /// <summary>
    /// Refresh some ui items when we needed.
    /// </summary>
    private void RefreshSelAndCoins()
    {
        selCount.text = sellObjects.Count.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Set color back to normal and destory mask game objects.
    /// </summary>
    private void CleanMasks()
    {
        var items = commonWindow.Items.transform;
        for (var i = 0; i < items.childCount; i++)
        {
            var wrapItem = items.GetChild(i).GetComponent<WrapItemContent>();
            if (wrapItem)
            {
                wrapItem.ShowSellMasks(false);
            }
        }
        var clones = sellObjects.Select(sellObj => sellObj.Value).ToList();
        for (var i = clones.Count - 1; i >= 0; i--)
        {
            NGUITools.Destroy(clones[i]);
        }
        sellObjects.Clear();
        cachedSelBagIndexs.Clear();
    }

    public void CleanUp()
    {
        CleanMasks();
        TotalMoney = 0;
        RefreshSelAndCoins();
    }

    public void SellOverUpdate()
    {
        if (sellObjects.ContainsKey(commonWindow.CurSelPos))
        {
            commonWindow.CurSelPos = HeroConstant.FirstPos;
        }
        CleanUp();
    }

    #endregion
}

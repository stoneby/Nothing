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
    private UIEventListener buyBackLis;
    private UIEventListener cancelLis;
    private UIGrid grid;

    public GameObject BaseItemPrefab;

    private readonly List<short> cachedSelBagIndexs = new List<short>();

    private readonly Dictionary<Position, GameObject> sellObjects = new Dictionary<Position, GameObject>();

    #region Window

    private void OnEnable()
    {
        MtaManager.TrackBeginPage(MtaType.SellItemWindow);
        commonWindow = WindowManager.Instance.GetWindow<UIItemCommonWindow>();
        commonWindow.HideSelMask();
        commonWindow.NormalClicked = OnNormalClick;
        selCount.text = sellObjects.Count.ToString(CultureInfo.InvariantCulture);
        InstallHandlers();
        sellLis.GetComponent<UIButton>().isEnabled = false;
        cancelLis.GetComponent<UIButton>().isEnabled = false;
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
 
        if (isToSell)
        {
            var child = NGUITools.AddChild(grid.gameObject, BaseItemPrefab);
            var heroBase = child.GetComponent<ItemBase>();
            var bagIndex = go.GetComponent<ItemBase>().BagIndex;
            heroBase.BagIndex = bagIndex;
            var itemInfo = ItemModeLocator.Instance.FindItem(bagIndex);
            heroBase.InitItem(itemInfo);
            UIEventListener.Get(child.gameObject).onClick = CancelItemSell;
            sellObjects.Add(pos, child);
        }
        else
        {
            RemoveSellObject(pos);
        }
        var item = NGUITools.FindInParents<WrapItemContent>(go);
        item.ShowSellMask(pos.Y, isToSell);
        var exitsToSell = sellObjects.Count != 0;
        sellLis.GetComponent<UIButton>().isEnabled = exitsToSell;
        cancelLis.GetComponent<UIButton>().isEnabled = exitsToSell;
        grid.repositionNow = true;
    }

    private Position GetPosition(GameObject go)
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
        cancelLis = UIEventListener.Get(buttons.Find("Button-Cancel").gameObject);
        sellLis = UIEventListener.Get(buttons.Find("Button-Sell").gameObject);
        buyBackLis = UIEventListener.Get(buttons.Find("Button-BuyBack").gameObject);
    }

    private void InstallHandlers()
    {
        sellLis.onClick = OnSell;
        buyBackLis.onClick = OnBuyBack;
        cancelLis.onClick = OnCancel;
        commonWindow.Items.OnUpdate += OnUpdate;
        commonWindow.OnSortOrderChangedBefore += OnSortOrderChangedBefore;
        commonWindow.OnSortOrderChangedAfter += OnSortOrderChangedAfter;
    }

    private void UnInstallHandlers()
    {
        sellLis.onClick = null;
        buyBackLis.onClick = null;
        cancelLis.onClick = null;
        commonWindow.Items.OnUpdate += OnUpdate;
        commonWindow.OnSortOrderChangedBefore += OnSortOrderChangedBefore;
        commonWindow.OnSortOrderChangedAfter += OnSortOrderChangedAfter;
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

    private void OnSortOrderChangedAfter(List<ItemInfo> hInfos)
    {
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

    private void OnSortOrderChangedBefore(List<ItemInfo> hInfos)
    {
        CacheBagIndexsFromPos(sellObjects.Keys, cachedSelBagIndexs);
    }

    private void UpdateSellMasks()
    {
        var items = commonWindow.Items.transform;
        var childCount = items.childCount;
        for (var i = 0; i < childCount; i++)
        {
            var wrapHero = items.GetChild(i).GetComponent<WrapHerosItem>();
            for (var j = 0; j < wrapHero.Children.Count; j++)
            {
                var pos = new Position { X = wrapHero.Row, Y = j };
                var showSellMask = sellObjects.ContainsKey(pos);
                wrapHero.ShowSellMask(j, showSellMask);
            }
        }
    }

    private void CacheBagIndexsFromPos(IEnumerable<Position> list, List<short> bagIndexs)
    {
        bagIndexs.Clear();
        bagIndexs.AddRange(list.Select(pos => commonWindow.GetInfo(pos).BagIndex));
    }

    private List<Position> GetPositionsViaBagIndex(IEnumerable<short> bags, List<ItemInfo> hInfos, int countPerGroup)
    {
        return bags.Select(bagIndex => hInfos.FindIndex(info => info.BagIndex == bagIndex)).Select(
            index => new Position { X = index / countPerGroup, Y = index % countPerGroup }).ToList();
    }

    private void OnBuyBack(GameObject go)
    {
        
    }

    private void OnCancel(GameObject go)
    {
        CleanUp();
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
        var pos = GetPosition(go);
        RemoveSellObject(pos);
        grid.repositionNow = true;
    }

    private void RemoveSellObject(Position pos)
    {
        var clone = sellObjects[pos];
        NGUITools.Destroy(clone);
        sellObjects.Remove(pos);
    }

    /// <summary>
    /// Refresh some ui items when we needed.
    /// </summary>
    private void RefreshSelAndSoul()
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
            var item = items.GetChild(i).GetComponent<WrapItemContent>();
            item.ShowSellMasks(false);
        }
        var clones = sellObjects.Select(sellObj => sellObj.Value).ToList();
        for (var i = clones.Count - 1; i >= 0; i--)
        {
            NGUITools.Destroy(clones[i]);
        }
        sellObjects.Clear();
    }

    public void CleanUp()
    {
        CleanMasks();
        totalMoney = 0;
        RefreshSelAndSoul();
    }

    #endregion
}

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UISellItemWindow : Window
{
    private UIItemCommonWindow itemsWindow;
    private UIDragDropContainer dragDropContainer;
    private GameObject itemToSell;
    private GameObject itemToCancel;
    private readonly List<GameObject> sellMasks = new List<GameObject>();
    private UILabel selCount;
    private UILabel moneyCount;
    private GameObject sellMask;
    private readonly List<short> sellList = new List<short>();
    private const int MaxSellCount = 12;
    private const int HighQuality = 3;
    private long totalMoney;
    private UIEventListener backLis;
    private UIEventListener sellLis;
    private UIEventListener buyBackLis;
    private UIEventListener cancelLis;

    public GameObject BaseItemPrefab;

    //The key is the game object copied on the right, the value is the original hero game object.
    private readonly Dictionary<GameObject, GameObject> sellDictionary = new Dictionary<GameObject, GameObject>();

    #region Window

    public override void OnEnter()
    {
        MtaManager.TrackBeginPage(MtaType.SellItemWindow);
        itemsWindow = WindowManager.Instance.GetWindow<UIItemCommonWindow>();
        itemsWindow.NormalClicked = OnNormalClickForSell;
        selCount.text = sellList.Count.ToString(CultureInfo.InvariantCulture);
        InstallHandlers();
        sellLis.GetComponent<UIButton>().isEnabled = false;
        cancelLis.GetComponent<UIButton>().isEnabled = false;
    }

    public override void OnExit()
    {
        MtaManager.TrackEndPage(MtaType.SellItemWindow);
        UnInstallHandlers();
        CleanUp();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        dragDropContainer = transform.Find("DragDropContainer").GetComponent<UIDragDropContainer>();
        selCount = Utils.FindChild(transform, "SelValue").GetComponent<UILabel>();
        moneyCount = Utils.FindChild(transform, "CoinsValue").GetComponent<UILabel>();
        backLis = UIEventListener.Get(transform.Find("Buttons/Button-Back").gameObject);
        cancelLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Cancel").gameObject);
        sellLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Sell").gameObject);
        buyBackLis = UIEventListener.Get(Utils.FindChild(transform, "Button-BuyBack").gameObject);
        sellMask = Utils.FindChild(transform, "SellMask").gameObject;
        sellMask.SetActive(false);
    }

    private void InstallHandlers()
    {
        sellLis.onClick = OnSell;
        buyBackLis.onClick = OnBuyBack;
        cancelLis.onClick = OnCancel;
        backLis.onClick = OnBack;
    }

    private void UnInstallHandlers()
    {
        sellLis.onClick = null;
        buyBackLis.onClick = null;
        cancelLis.onClick = null;
        backLis.onClick = null;
    }

    private void OnBuyBack(GameObject go)
    {
        
    }

    private void OnCancel(GameObject go)
    {
        CleanUp();
        itemsWindow.NormalClicked = OnNormalClickForSell;
    }

    private void OnSell(GameObject go)
    {
        if (ContainsRare(sellList))
        {
            var assertWindow = WindowManager.Instance.GetWindow<AssertionWindow>();
            assertWindow.AssertType = AssertionWindow.Type.OkCancel;
            assertWindow.Message = "";
            assertWindow.Title = StringTable.SellConfirm;
            assertWindow.OkButtonClicked += OnSellConfirmOk;
            assertWindow.CancelButtonClicked += OnSellConfirmCancel;
            WindowManager.Instance.Show(typeof(AssertionWindow), true);
        }
        else
        {
            var msg = new CSSellItems { SellItemIndexes = sellList };
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
        var msg = new CSSellItems { SellItemIndexes = sellList };
        NetManager.SendMessage(msg);
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
        var assertWindow = WindowManager.Instance.GetWindow<AssertionWindow>();
        assertWindow.OkButtonClicked -= OnSellConfirmOk;
        assertWindow.CancelButtonClicked -= OnSellConfirmCancel;
        WindowManager.Instance.Show(typeof(AssertionWindow), false);
    }

    /// <summary>
    ///  The callback of clicking back button.
    /// </summary>
    private void OnBack(GameObject go)
    {
        WindowManager.Instance.Show<UISellItemWindow>(false);
        WindowManager.Instance.Show<UIItemCommonWindow>(false);
    }

    private void OnNormalClickForSell(GameObject go)
    {
        var bagIndex = go.GetComponent<ItemBase>().BagIndex;
        var info = ItemModeLocator.Instance.FindItem(bagIndex);
        UIItemSnapShotWindow.ItemInfo = info;
        //已装备或已绑定的话，按钮不可点
        bool isEnabled = (info.BindStatus == 0 && info.EquipStatus == 0);
        itemToSell = go;
        itemToSell.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClickForCancel;
        var snapShot = WindowManager.Instance.Show<UIItemSnapShotWindow>(true);
        snapShot.InitTemplate("HeroOrItemSnapShot.Sell", ItemSell, isEnabled);
    }

    private void OnNormalClickForCancel(GameObject go)
    {
        itemToCancel = go;
        var bagIndex = go.GetComponent<ItemBase>().BagIndex;
        UIItemSnapShotWindow.ItemInfo = ItemModeLocator.Instance.FindItem(bagIndex);
        var snapShot = WindowManager.Instance.Show<UIItemSnapShotWindow>(true);
        snapShot.InitTemplate("HeroOrItemSnapShot.CancelSell", CancelHeroSell);
    }

    private void ItemSell(GameObject go)
    {
        if (sellList.Count == 0)
        {
            sellLis.GetComponent<UIButton>().isEnabled = true;
            cancelLis.GetComponent<UIButton>().isEnabled = true;
        }
        if (sellList.Count >= MaxSellCount)
        {
            return;
        }
        var reparentTarget = dragDropContainer.reparentTarget;
        var child = NGUITools.AddChild(reparentTarget.gameObject, BaseItemPrefab);
        var bagIndex = itemToSell.GetComponent<ItemBase>().BagIndex;
        child.GetComponent<ItemBase>().InitItem(ItemModeLocator.Instance.FindItem(bagIndex));
        child.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClickForCancel;
        var grid = reparentTarget.GetComponent<UIGrid>();
        grid.repositionNow = true;
        WindowManager.Instance.Show<UIItemSnapShotWindow>(false);
        RefreshSellList(itemToSell, child);
    }

    private void CancelHeroSell(GameObject go)
    {
        var bagIndex = itemToCancel.GetComponent<ItemBase>().BagIndex;
        var heroClone = sellDictionary.First(item => item.Key.GetComponent<ItemBase>().BagIndex == bagIndex).Key;
        var heros = itemsWindow.Items.transform;
        for (var i = 0; i < heros.childCount; i++)
        {
            var child = heros.GetChild(i);
            if (child.GetComponent<ItemBase>().BagIndex == bagIndex)
            {
                child.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClickForSell;
                RefreshSellList(child.gameObject, heroClone);
            }
        }
        WindowManager.Instance.Show<UIItemSnapShotWindow>(false);
        if (sellList.Count == 0)
        {
            sellLis.GetComponent<UIButton>().isEnabled = false;
            cancelLis.GetComponent<UIButton>().isEnabled = false;
        }
    }

    private void RefreshSellList(GameObject go, GameObject copied)
    {
        var bagIndex = go.GetComponent<ItemBase>().BagIndex;
        if (!sellList.Contains(bagIndex))
        {
            sellList.Add(bagIndex);
            var maskToAdd = NGUITools.AddChild(go, sellMask);
            sellMasks.Add(maskToAdd);
            maskToAdd.SetActive(true);
            sellDictionary.Add(copied, go);
        }
        else
        {
            sellList.Remove(bagIndex);
            var maskToAdd = go.transform.FindChild("SellMask(Clone)").gameObject;
            sellMasks.Remove(maskToAdd);
            Destroy(maskToAdd);
            sellDictionary.Remove(copied);
            NGUITools.Destroy(copied);
        }
        RefreshSelAndSoul();
    }

    /// <summary>
    /// Refresh some ui items when we needed.
    /// </summary>
    private void RefreshSelAndSoul()
    {
        selCount.text = sellList.Count.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Set color back to normal and destory mask game objects.
    /// </summary>
    private void CleanMasks()
    {
        for (int i = 0; i < sellMasks.Count; i++)
        {
            sellMasks[i].transform.parent = null;
            Destroy(sellMasks[i]);
        }
        sellMasks.Clear();
    }

    public void CleanUp()
    {
        CleanMasks();
        sellList.Clear();
        foreach (var pair in sellDictionary)
        {
            NGUITools.Destroy(pair.Key);
        }
        sellDictionary.Clear();
        totalMoney = 0;
        RefreshSelAndSoul();
    }

    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIBuyBackItemWindow : Window
{
    private UIItemCommonWindow itemsWindow;
    private UIDragDropContainer dragDropContainer;
    private GameObject itemToBuyBack;
    private GameObject itemToCancel;
    private readonly List<GameObject> masks = new List<GameObject>();
    private UILabel selCount;
    private UILabel moneyCount;
    private GameObject mask;
    private readonly List<short> buyList = new List<short>();
    private const int MaxSellCount = 12;
    private const int HighQuality = 3;
    private long totalMoney;
    private UIEventListener backLis;
    private UIEventListener buyBackLis;
    private List<ItemInfo> infos;


    //The key is the game object copied on the right, the value is the original hero game object.
    private readonly Dictionary<GameObject, GameObject> sellDictionary = new Dictionary<GameObject, GameObject>();

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
        buyBackLis.GetComponent<UIButton>().isEnabled = false;
    }

    public override void OnExit()
    {
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
        buyBackLis = UIEventListener.Get(Utils.FindChild(transform, "Button-BuyBack").gameObject);
        mask = Utils.FindChild(transform, "Mask").gameObject;
        mask.SetActive(false);
    }

    private void InstallHandlers()
    {
        buyBackLis.onClick = OnBuyBack;
        backLis.onClick = OnBack;
    }

    private void UnInstallHandlers()
    {
        buyBackLis.onClick = null;
        backLis.onClick = null;
    }

    private void Init()
    {
        infos = ItemModeLocator.Instance.BuyBackItems.ItemInfos ?? new List<ItemInfo>();
        ClearExpireItems(infos);
        infos.Sort(CompareItemByExpireTime);

        itemsWindow = WindowManager.Instance.GetWindow<UIItemCommonWindow>();
        itemsWindow.NormalClicked = OnNormalClickForSell;
        selCount.text = buyList.Count.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Clean up the items which are already expired.
    /// </summary>
    /// <param name="infoList">The item infos to be checked.</param>
    private void ClearExpireItems(List<ItemInfo> infoList)
    {
        long now = Utils.ConvertToJavaTimestamp(DateTime.Now);
        for (int i = 0; i < infoList.Count; i++)
        {
            var info = infoList[i];
            if (info.ExpireTime < now)
            {
                infoList.Remove(info);
            }
        }
    }

    /// <summary>
    /// The comparation of item info by expire time.
    /// </summary>
    /// <param name="p1">The left hero info.</param>
    /// <param name="p2">The right hero info.</param>
    /// <returns>The result of the comparation</returns>
    private int CompareItemByExpireTime(ItemInfo p1, ItemInfo p2)
    {
        int compareResult = p2.ExpireTime.CompareTo(p1.ExpireTime);
        if (compareResult == 0)
        {
            return p2.TmplId.CompareTo(p1.TmplId);
        }
        return compareResult;
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
        if (ContainsRare(buyList))
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
            var msg = new CSSellItems { SellItemIndexes = buyList };
            NetManager.SendMessage(msg);
        }
    }

    private void OnSellConfirmCancel(GameObject sender)
    {
        CleanConfirmWindow();
    }

    private void OnSellConfirmOk(GameObject sender)
    {
        CleanConfirmWindow();
        var msg = new CSSellItems { SellItemIndexes = buyList };
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

    private void CleanConfirmWindow()
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
        WindowManager.Instance.Show<UISellHeroWindow>(false);
        WindowManager.Instance.Show<UIHeroCommonWindow>(false);
    }

    private void OnNormalClickForSell(GameObject go)
    {
        var bagIndex = go.GetComponent<ItemBase>().BagIndex;
        var info = ItemModeLocator.Instance.FindItem(bagIndex);
        UIItemSnapShotWindow.ItemInfo = info;
        itemToBuyBack = go;
        itemToBuyBack.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClickForCancel;
        var snapShot = WindowManager.Instance.Show<UIItemSnapShotWindow>(true);
        snapShot.InitTemplate("HeroOrItemSnapShot.Sell", ItemBuyBack);
    }

    private void OnNormalClickForCancel(GameObject go)
    {
        itemToCancel = go;
        var bagIndex = go.GetComponent<ItemBase>().BagIndex;
        UIItemSnapShotWindow.ItemInfo = ItemModeLocator.Instance.FindItem(bagIndex);
        var snapShot = WindowManager.Instance.Show<UIItemSnapShotWindow>(true);
        snapShot.InitTemplate("HeroOrItemSnapShot.CancelSell", CancelHeroSell);
    }

    private void ItemBuyBack(GameObject go)
    {
        if (buyList.Count == 0)
        {
            buyBackLis.GetComponent<UIButton>().isEnabled = true;
        }
        if (buyList.Count >= MaxSellCount)
        {
            return;
        }
        var reparentTarget = dragDropContainer.reparentTarget;
        var child = NGUITools.AddChild(reparentTarget.gameObject, itemToBuyBack);
        child.GetComponent<ItemBase>().BagIndex = itemToBuyBack.GetComponent<ItemBase>().BagIndex;
        child.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClickForCancel;
        var grid = reparentTarget.GetComponent<UIGrid>();
        grid.repositionNow = true;
        WindowManager.Instance.Show<UIItemSnapShotWindow>(false);
        RefreshSellList(itemToBuyBack, child);
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
        if (buyList.Count == 0)
        {
            buyBackLis.GetComponent<UIButton>().isEnabled = false;
        }
    }

    private void RefreshSellList(GameObject go, GameObject copied)
    {
        var bagIndex = go.GetComponent<ItemBase>().BagIndex;
        if (!buyList.Contains(bagIndex))
        {
            buyList.Add(bagIndex);
            var maskToAdd = NGUITools.AddChild(go, mask);
            masks.Add(maskToAdd);
            maskToAdd.SetActive(true);
            sellDictionary.Add(copied, go);
        }
        else
        {
            buyList.Remove(bagIndex);
            var maskToAdd = go.transform.FindChild("Mask(Clone)").gameObject;
            masks.Remove(maskToAdd);
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
        selCount.text = buyList.Count.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Set color back to normal and destory mask game objects.
    /// </summary>
    private void CleanMasks()
    {
        for (int i = 0; i < masks.Count; i++)
        {
            masks[i].transform.parent = null;
            Destroy(masks[i]);
        }
        masks.Clear();
    }

    public void CleanUp()
    {
        CleanMasks();
        buyList.Clear();
        foreach (var pair in sellDictionary)
        {
            NGUITools.Destroy(pair.Key);
        }
        sellDictionary.Clear();
        totalMoney = 0;
        RefreshSelAndSoul();
    }

    /// <summary>
    /// Update the expire time of each buy back items per second.
    /// </summary>
    private void UpdateExpireTime()
    {
        var oneSecond = new TimeSpan(0, 0, 1);
        //while (buyBackItems.Count > 0)
        {
            //yield return new WaitForSeconds(1);
            //for (int i = 0; i < buyBackItems.Count; i++)
            //{
            //    if (buyBackItems[i].TimeRemain.TotalSeconds > 1)
            //    {
            //        buyBackItems[i].TimeRemain = buyBackItems[i].TimeRemain.Subtract(oneSecond);
            //    }
            //    else
            //    {
            //        for (int j = 0; j < infos.Count; j++)
            //        {
            //            if (infos[j].BagIndex == buyBackItems[i].BagIndex)
            //            {
            //                infos.Remove(infos[j]);
            //                if (PoolManager.Pools.ContainsKey(HeroConstant.HeroPoolName))
            //                {
            //                    var item = buyBackItems[i].transform;
            //                    UIEventListener.Get(buyBackItems[i].transform.gameObject).onClick = null;
            //                    item.parent = PoolManager.Pools[HeroConstant.HeroPoolName].transform;
            //                    PoolManager.Pools[HeroConstant.HeroPoolName].Despawn(item);
            //                }
            //                buyBackItems.RemoveAt(i);
            //                grid.repositionNow = true;
            //                break;
            //            }
        //            }
        //        }
        //    }
        }
    }

    #endregion

}

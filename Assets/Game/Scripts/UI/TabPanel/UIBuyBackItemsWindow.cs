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
public class UIBuyBackItemsWindow : Window
{
    private UIEventListener buyBackLis;
    private UIEventListener backLis;
    private GameObject curClicked;

    private UIEventListener cancelLis;
    private UILabel costCoins;
    private UILabel selCount;
    private UILabel canBuyBackCount;
    private UIGrid grid;
    private List<ItemInfo> infos;
    private int costCoinValue;
    private GameObject mask;
    private readonly List<NewBuyBackItem> buyBackItems = new List<NewBuyBackItem>();

    private UIDragDropContainer dragDropContainer;

    //The key is the item cloned, the value is the item on the left.
    private readonly Dictionary<GameObject, GameObject> selItems = new Dictionary<GameObject, GameObject>();

    public GameObject BaseItemPrefab;

    public int CostCoinValue
    {
        get { return costCoinValue; }
        private set
        {
            costCoinValue = value;
            costCoins.text = costCoinValue.ToString(CultureInfo.InvariantCulture);
        }
    }

    public Transform ItemPrefab;

    #region Window

    public override void OnEnter()
    {
        Refresh();
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        dragDropContainer = transform.Find("DragDropContainer").GetComponent<UIDragDropContainer>();
        buyBackLis = UIEventListener.Get(transform.Find("Buttons/Button-BuyBack").gameObject);
        backLis = UIEventListener.Get(transform.Find("Buttons/Button-Back").gameObject);
        mask = transform.Find("Mask").gameObject;
        NGUITools.SetActive(mask, false);
        grid = transform.Find("Container/Scroll View/Grid").GetComponent<UIGrid>();
        selCount = transform.Find("SelCount/SelValue").GetComponent<UILabel>();
        canBuyBackCount = transform.Find("ItemNum/ItemNumValue").GetComponent<UILabel>();
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

    private void OnBack(GameObject go)
    {
        WindowManager.Instance.Show<UIBuyBackItemsWindow>(false);
    }

    private void OnBuyBack(GameObject go)
    {
        if (selItems.Count > 0)
        {
            var buyBacks = selItems.Select(item => item.Key.GetComponent<ItemBase>().BagIndex).ToList();
            var msg = new CSBuyBackItems { BuybackItemIndexes = buyBacks };
            NetManager.SendMessage(msg);
        }
    }

    /// <summary>
    /// Fill in the items if the count changes.
    /// </summary>
    /// <param name="itemCount">The count of item to be shown.</param>
    private void FillItems(int itemCount)
    {
        var childCount = grid.transform.childCount;
        if (childCount != itemCount)
        {
            var isAdd = childCount < itemCount;
            HeroUtils.AddOrDelItems(grid.transform, ItemPrefab.transform, isAdd, Mathf.Abs(itemCount - childCount), HeroConstant.HeroPoolName,
                    null);
            grid.repositionNow = true;
        }
    }

    /// <summary>
    /// Refresh the data of the shown items.
    /// </summary>
    public void Refresh()
    {
        infos = ItemModeLocator.Instance.BuyBackItems.ItemInfos ?? new List<ItemInfo>();
        ClearExpireItems(infos);
        infos.Sort(CompareItemByExpireTime);
        infos = infos.GetRange(0, Math.Min(infos.Count, ItemType.BuyBackLimit));
        var count = infos.Count;
        FillItems(count);
        buyBackItems.Clear();
        for (var i = 0; i < count; i++)
        {
            var buyBackItem = grid.transform.GetChild(i).GetComponent<NewBuyBackItem>();
            buyBackItem.InitItem(infos[i]);
            buyBackItems.Add(buyBackItem);
            var longPress = buyBackItem.GetComponent<NGUILongPress>();
            longPress.OnLongPress = OnLongPress;
            longPress.OnNormalPress = OnNormal;
        }
        StartCoroutine(UpdateExpireTime());
        RefreshUi();
    }

    public void BuyBackItemSucc()
    {
        var keys = selItems.Keys.ToList();
        var count = keys.Count;
        for (var i = count -1; i >= 0; i--)
        {
            var item = selItems[keys[i]];
            if(PoolManager.Pools.ContainsKey(HeroConstant.HeroPoolName))
            {
                var baseItem = keys[i];
                DespawnItem(item);
                var buyBackItem = item.GetComponent<NewBuyBackItem>();
                buyBackItems.Remove(buyBackItem);
                var info = ItemModeLocator.Instance.FindBuyBackItem(buyBackItem.BagIndex);
                infos.Remove(info);
                baseItem.transform.parent = null;
                NGUITools.Destroy(baseItem);
            }
        }
        grid.repositionNow = true;
        selItems.Clear();
        RefreshUi();
    }

    private static void DespawnItem(GameObject item)
    {
        var longPressDetecter = item.GetComponent<NGUILongPress>();
        longPressDetecter.OnNormalPress = null;
        longPressDetecter.OnLongPress = null;
        item.transform.parent = PoolManager.Pools[HeroConstant.HeroPoolName].transform;
        PoolManager.Pools[HeroConstant.HeroPoolName].Despawn(item.transform);
    }

    private void OnNormal(GameObject go)
    {
        curClicked = go;
        var bagIndex = go.GetComponent<NewBuyBackItem>().BagIndex;
        var info = ItemModeLocator.Instance.FindBuyBackItem(bagIndex);
        UIItemSnapShotWindow.ItemInfo = info;
        var snapShot = WindowManager.Instance.Show<UIItemSnapShotWindow>(true, true);
        snapShot.InitTemplate("ItemSnapShot.SelBuyBack", SelBuyBack);
    }

    private void SelBuyBack(GameObject go)
    {
        var reparentTarget = dragDropContainer.reparentTarget;
        var child = NGUITools.AddChild(reparentTarget.gameObject, BaseItemPrefab);
        var maskClone = NGUITools.AddChild(curClicked, mask);
        NGUITools.SetActive(maskClone, true);
        selItems.Add(child, curClicked);
        var bagIndex = curClicked.GetComponent<NewBuyBackItem>().BagIndex;
        child.GetComponent<ItemBase>().InitItem(ItemModeLocator.Instance.FindBuyBackItem(bagIndex));
        child.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClickForCancel;
        curClicked.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClickForCancel;
        var selGrid = reparentTarget.gameObject.GetComponent<UIGrid>();
        selGrid.repositionNow = true;
        WindowManager.Instance.Show<UIItemSnapShotWindow>(false);
        RefreshUi();
    }

    private void OnNormalClickForCancel(GameObject go)
    {
        curClicked = go;
        var bagIndex = go.GetComponent<ItemBase>().BagIndex;
        var info = ItemModeLocator.Instance.FindBuyBackItem(bagIndex);
        UIItemSnapShotWindow.ItemInfo = info;
        var snapShot = WindowManager.Instance.Show<UIItemSnapShotWindow>(true, true);
        snapShot.InitTemplate("ItemSnapShot.CancelBuyBack", CancelSelBuyBack);
    }

    private void CancelSelBuyBack(GameObject go)
    {
        var keys = selItems.Keys;
        GameObject clone = (from key in keys
                            let bagIndex = key.GetComponent<ItemBase>().BagIndex
                            where bagIndex == curClicked.GetComponent<ItemBase>().BagIndex
                            select key).FirstOrDefault();
        if (clone != null && selItems.ContainsKey(clone))
        {
            var source = selItems[clone];
            var maskClone = source.transform.Find("Mask(Clone)");
            NGUITools.Destroy(maskClone.gameObject);
            source.GetComponent<NGUILongPress>().OnNormalPress = OnNormal;
            selItems.Remove(clone);
            NGUITools.Destroy(clone);
        }
        RefreshUi();
        WindowManager.Instance.Show<UIItemSnapShotWindow>(false);
    }

    private void OnLongPress(GameObject go)
    {
        WindowManager.Instance.Show<UIItemDetailWindow>(true);
    }

    private void RefreshUi()
    {
        selCount.text = selItems.Count.ToString(CultureInfo.InvariantCulture);
        canBuyBackCount.text = string.Format("{0}/{1}", buyBackItems.Count, 100);
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

    /// <summary>
    /// Update the expire time of each buy back items per second.
    /// </summary>
    private IEnumerator UpdateExpireTime()
    {
        var oneSecond = new TimeSpan(0, 0, 1);
        while (buyBackItems.Count > 0)
        {
            yield return new WaitForSeconds(1);
            for (var i = 0; i < buyBackItems.Count; i++)
            {
                if (buyBackItems[i].TimeRemain.TotalSeconds > 1)
                {
                    buyBackItems[i].TimeRemain = buyBackItems[i].TimeRemain.Subtract(oneSecond);
                }
                else
                {
                    for (var j = 0; j < infos.Count; j++)
                    {
                        if (infos[j].BagIndex == buyBackItems[i].BagIndex)
                        {
                            infos.Remove(infos[j]);
                            if (PoolManager.Pools.ContainsKey(HeroConstant.HeroPoolName))
                            {
                                var item = buyBackItems[i].transform;
                                UIEventListener.Get(buyBackItems[i].transform.gameObject).onClick = null;
                                item.parent = PoolManager.Pools[HeroConstant.HeroPoolName].transform;
                                PoolManager.Pools[HeroConstant.HeroPoolName].Despawn(item);
                            }
                            buyBackItems.RemoveAt(i);
                            grid.repositionNow = true;
                            break;
                        }
                    }
                }
            }
        }
    }

    #endregion
}

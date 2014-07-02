using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class BuyBackDialogWindow : Window
{
    #region Private Fields

    private UIEventListener okLis;
    private UIEventListener cancelLis;
    private UILabel costCoins;
    private UIGrid grid;
    private List<ItemInfo> infos;
    private readonly List<BuyBackItem> buyBackItems = new List<BuyBackItem>();
    private readonly List<short> buyBacks = new List<short>();
    private int costCoinValue;

    #endregion Private Fields

    #region Public Fields

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

    #endregion

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
        Reset();
        Refresh();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            var buyBackItem = grid.transform.GetChild(i).GetComponent<BuyBackItem>();
            buyBackItem.ItemClicked -= OnItemClicked;
        }
        StopCoroutine("UpdateExpireTime");
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Used to initial some varibles.
    /// </summary>
    private void Awake()
    {
        okLis = UIEventListener.Get(transform.Find("OK").gameObject);
        cancelLis = UIEventListener.Get(transform.Find("Cancel").gameObject);
        costCoins = Utils.FindChild(transform, "CostValue").GetComponent<UILabel>();
        grid = GetComponentInChildren<UIGrid>();
    }

    /// <summary>
    /// Install handlers.
    /// </summary>
    private void InstallHandlers()
    {
        okLis.onClick = OnOk;
        cancelLis.onClick = OnCancel;
    }

    /// <summary>
    /// Uninstall handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        okLis.onClick = null;
        cancelLis.onClick = null;
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
        for (int i = 0; i < count; i++)
        {
            var buyBackItem = grid.transform.GetChild(i).GetComponent<BuyBackItem>();
            buyBackItem.Init(infos[i]);
            buyBackItem.ItemClicked += OnItemClicked;
            buyBackItems.Add(buyBackItem);
        }
        StartCoroutine(UpdateExpireTime());
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
            for (int i = 0; i < buyBackItems.Count; i++)
            {
                if (buyBackItems[i].TimeRemain.TotalSeconds > 1)
                {
                    buyBackItems[i].TimeRemain = buyBackItems[i].TimeRemain.Subtract(oneSecond);
                }
                else
                {
                    for(int j = 0; j < infos.Count; j++)
                    {
                        if(infos[j].BagIndex == buyBackItems[i].BagIndex)
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

    /// <summary>
    /// The call back of the buy back item clicked.
    /// </summary>
    /// <param name="go">The event sender.</param>
    private void OnItemClicked(GameObject go)
    {
        var buyBackItem = go.GetComponent<BuyBackItem>();
        buyBacks.Add(buyBackItem.BagIndex);
        var itemInfo = ItemModeLocator.Instance.FindBuyBackItem(buyBackItem.BagIndex);
        CostCoinValue += ItemModeLocator.Instance.GetSalePrice(itemInfo.TmplId);
    }

    /// <summary>
    /// The call back of the ok button clicked.
    /// </summary>
    /// <param name="go">The event sender.</param>
    private void OnOk(GameObject go)
    {
        if (buyBacks.Count > 0)
        {
            var msg = new CSBuyBackItems { BuybackItemIndexes = buyBacks };
            NetManager.SendMessage(msg);
        }
        else
        {
            WindowManager.Instance.Show<BuyBackDialogWindow>(false);
        }
    }

    /// <summary>
    /// The call back of the cancel button clicked.
    /// </summary>
    /// <param name="go">The event sender.</param>
    private void OnCancel(GameObject go)
    {
        CleanUp();
    }

    /// <summary>
    /// Reset some varibles.
    /// </summary>
    private void Reset()
    {
        CostCoinValue = 0;
        buyBacks.Clear();
        buyBackItems.Clear();
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
            if(info.ExpireTime < now)
            {
                infoList.Remove(info);
            }
        }
    }

    /// <summary>
    /// Do some clean up job.
    /// </summary>
    public void CleanUp()
    {
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            var child = grid.transform.GetChild(i);
            var buyBackItem = child.GetComponent<BuyBackItem>();
            if (buyBacks.Contains(buyBackItem.BagIndex))
            {
                buyBackItem.Selected = false;
            }
        }
        buyBacks.Clear();
        WindowManager.Instance.Show<BuyBackDialogWindow>(false);
    }

    #endregion
}

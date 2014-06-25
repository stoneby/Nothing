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

    private void Awake()
    {
        okLis = UIEventListener.Get(transform.Find("OK").gameObject);
        cancelLis = UIEventListener.Get(transform.Find("Cancel").gameObject);
        costCoins = Utils.FindChild(transform, "CostValue").GetComponent<UILabel>();
        grid = GetComponentInChildren<UIGrid>();
    }

    private void FillItems(int itemCount)
    {
        var childCount = grid.transform.childCount;
        if (childCount != itemCount)
        {
            var isAdd = childCount < itemCount;
            Utils.AddOrDelItems(grid.transform, ItemPrefab.transform, isAdd, Mathf.Abs(itemCount - childCount), "Heros",
                    null);
            grid.repositionNow = true;
        }
    }

    public void Refresh()
    {
        infos = ItemModeLocator.Instance.BuyBackItems.ItemInfos ?? new List<ItemInfo>();
        ClearExpireItems(infos);
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
                            if (PoolManager.Pools.ContainsKey("Items"))
                            {
                                var item = buyBackItems[i].transform;
                                UIEventListener.Get(buyBackItems[i].transform.gameObject).onClick = null;
                                item.parent = PoolManager.Pools["Items"].transform;
                                PoolManager.Pools["Items"].Despawn(item);
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

    private void OnItemClicked(GameObject go)
    {
        var buyBackItem = go.GetComponent<BuyBackItem>();
        buyBacks.Add(buyBackItem.BagIndex);
        var itemInfo = ItemModeLocator.Instance.FindBuyBackItem(buyBackItem.BagIndex);
        CostCoinValue += ItemModeLocator.Instance.GetSalePrice(itemInfo.TmplId);
    }

    private void InstallHandlers()
    {
        okLis.onClick = OnOK;
        cancelLis.onClick = OnCancel;
    }

    private void UnInstallHandlers()
    {
        okLis.onClick = null;
        cancelLis.onClick = null;
    }

    private void OnOK(GameObject go)
    {
       if(buyBacks.Count > 0)
       {
           var msg = new CSBuyBackItems {BuybackItemIndexes = buyBacks};
           NetManager.SendMessage(msg);
       }
       else
       {
           WindowManager.Instance.Show<BuyBackDialogWindow>(false);
       }
    } 
    
    private void OnCancel(GameObject go)
    {
        CleanUp();
    }

    private void Reset()
    {
        CostCoinValue = 0;
        buyBacks.Clear();
        buyBackItems.Clear();
    }

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

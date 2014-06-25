using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// The handler to handle the item sell operation.
/// </summary>
public class ItemSellHandler : MonoBehaviour
{
    #region Private Fields

    private readonly List<short> sellList = new List<short>();
    private readonly List<GameObject> sellMasks = new List<GameObject>();
    private UILabel sellCount;
    private const int MaxSellCount = 10;
    private const int HighQuality = 3;
    private GameObject sellMask;
    public Vector3 MaskOffset = new Vector3(0, 13, 0);
    private UIEventListener sellCancelLis;
    private UIEventListener sellOkLis;
    private UIEventListener buyBackLis;

    #endregion

    #region Private Methods

    private void OnDisable()
    {
        sellCancelLis.onClick = null;
        sellOkLis.onClick = null;
        buyBackLis.onClick = null;
        CleanMasks();
    }

    private void OnEnable()
    {
        sellList.Clear();
        sellCount.text = string.Format("{0}/{1}", 0, MaxSellCount);
        sellCancelLis.onClick = OnSellCancel;
        sellOkLis.onClick = OnSellOk;
        buyBackLis.onClick = OnBuyBack;
    }

    private void Awake()
    {
        sellCount = Utils.FindChild(transform, "SellNum").GetComponent<UILabel>();
        sellMask = Utils.FindChild(transform, "SellMask").gameObject;
        sellCancelLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Cancel").gameObject);
        sellOkLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Sell").gameObject);
        buyBackLis = UIEventListener.Get(Utils.FindChild(transform, "Button-TradeIn").gameObject);
        sellMask.SetActive(false);
    }

    private void OnSellOk(GameObject go)
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

    private void OnSellCancel(GameObject go)
    {
        CleanUp();
    }

    private void OnBuyBack(GameObject go)
    {
        if (ItemModeLocator.Instance.BuyBackItems == null || ItemModeLocator.Instance.BuyBackItems.ItemInfos == null)
        {
            var msg = new CSQueryAllItems { BagType = 1 };
            NetManager.SendMessage(msg);
        }
        else
        {
            WindowManager.Instance.Show<BuyBackDialogWindow>(true);
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

    private void CleanSellConfirm()
    {
        var assertWindow = WindowManager.Instance.GetWindow<AssertionWindow>();
        assertWindow.OkButtonClicked -= OnSellConfirmOk;
        assertWindow.CancelButtonClicked -= OnSellConfirmCancel;
        WindowManager.Instance.Show(typeof(AssertionWindow), false);
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

    private void CleanMasks()
    {
        for (int i = 0; i < sellMasks.Count; i++)
        {
            sellMasks[i].transform.parent = null;
            Destroy(sellMasks[i]);
        }
        sellMasks.Clear(); 
    }

    #endregion

    #region Public Methods

    public void CleanUp()
    {
        CleanMasks();
        sellList.Clear();
        sellCount.text = string.Format("{0}/{1}", sellList.Count, MaxSellCount);
    }

    public void Refresh()
    {
        var itemsWindow = WindowManager.Instance.GetWindow<UItemsWindow>();
        itemsWindow.ItemClicked = ItemSellClicked;
    }

    public void ItemSellClicked(GameObject go)
    {
        var itemPack = go.GetComponent<EquipItem>();
        var bagIndex = itemPack.BagIndex;
        var info = ItemModeLocator.Instance.FindItem(bagIndex);
        //已装备或已绑定的话，不响应
        if (info.BindStatus == 1 || info.EquipStatus == 1)
        {
            return;
        }
        if (!sellList.Contains(bagIndex))
        {
            if (sellList.Count >= MaxSellCount)
            {
                return;
            }
            sellList.Add(bagIndex);
            var child = NGUITools.AddChild(go, sellMask);
            child.transform.localPosition = MaskOffset;
            child.SetActive(true);
            sellMasks.Add(child);
        }
        else
        {
            sellList.Remove(bagIndex);
            var child = go.transform.FindChild("SellMask(Clone)");
            sellMasks.Remove(child.gameObject);
            child.parent = null;
            Destroy(child.gameObject);
        }
        sellCount.text = string.Format("{0}/{1}", sellList.Count, MaxSellCount);
    }


    #endregion
}

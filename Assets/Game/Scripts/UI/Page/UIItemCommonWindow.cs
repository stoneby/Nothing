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
    private UIEventListener closeBtnLis;
    private StretchItem closeBtnLine;
    private UIEventListener sortBtnLis;
    private UILabel sortLabel;
    private UILabel itemNum;
    public List<ItemInfo> Infos { get; private set; }
    private UIEventListener.VoidDelegate normalClicked;

    public UIItemDetailHandler ItemDetailHandler;
    public UISellItemHandler ItemSellHandler;

    private bool descendSort;

    public delegate void SortOrderChanged(List<ItemInfo> hInfos);

    /// <summary>
    /// The call back of the sort order changed.
    /// </summary>
    public SortOrderChanged OnSortOrderChangedBefore;
    public SortOrderChanged OnSortOrderChangedAfter;

    public CustomGrid Items;


    private Transform selMask;
    [HideInInspector]
    public GameObject CurSel;

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

    #region Window

    private void InitWrapContents(List<ItemInfo> itemInfos)
    {
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
    }

    public override void OnEnter()
    {
        InitWrapContents(Infos);
        InstallHandlers();
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
        sortBtnLis = UIEventListener.Get(buttons.Find("Button-Sort").gameObject);
        closeBtnLis = UIEventListener.Get(buttons.Find("Button-Close").gameObject);
        closeBtnLine = buttons.Find("Button-CloseLine").GetComponent<StretchItem>();
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();
        itemNum = Utils.FindChild(transform, "ItemNumValue").GetComponent<UILabel>();
        Infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos;
        NGUITools.SetActive(Items.transform.parent.gameObject, true);
        selMask = transform.Find("SelMask");
        selMask.transform.parent = Items.transform.parent;
        NGUITools.SetActive(selMask.gameObject, false);
    }

    private void InstallHandlers()
    {
        sortBtnLis.onClick = OnSortClicked;
        closeBtnLis.onClick = OnClose;
        closeBtnLine.DragFinish = OnClose;
    }

    private void UnInstallHandlers()
    {
        sortBtnLis.onClick = null;
        closeBtnLis.onClick = null;
        closeBtnLine.DragFinish = null;
    }

    private void OnClose(GameObject go)
    {
        WindowManager.Instance.Show<UIItemCommonWindow>(false);
    }


    private void OnSortClicked(GameObject go)
    {
        if (OnSortOrderChangedBefore != null)
        {
            OnSortOrderChangedBefore(Infos);
        }
        var orderType = ItemModeLocator.Instance.OrderType;
        orderType = (OrderType)(((int)orderType + 1) % (ItemHelper.SortKeys.Count - 1));
        sortLabel.text = LanguageManager.Instance.GetTextValue(ItemHelper.SortKeys[(int)orderType]);
        ItemModeLocator.Instance.OrderType = orderType;
        InitWrapContents(Infos);
        if (OnSortOrderChangedAfter != null)
        {
            OnSortOrderChangedAfter(Infos);
        }
    }

    private void ExcuteSort(OrderType orderType)
    {
        sortLabel.text = LanguageManager.Instance.GetTextValue(ItemHelper.SortKeys[(int)orderType]);
        ItemModeLocator.Instance.SortItemList(orderType, Infos);
    }

    /// <summary>
    /// Refresh the heros page window with the special hero info list.
    /// </summary>
    /// <param name="newInfos">The hero info list to refresh data.</param>
    public void Refresh(List<ItemInfo> newInfos)
    {
        RefreshItemCount(newInfos.Count);
        InitWrapContents(newInfos);
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
        ShowSelMask();
    }

    public void ShowSelMask()
    {
        if (selMask.gameObject.activeSelf == false)
        {
            NGUITools.SetActive(selMask.gameObject, true);
        }
    }

    public void HideSelMask()
    {
        if (selMask.gameObject.activeSelf)
        {
            NGUITools.SetActive(selMask.gameObject, false);
        }
    }

    public ItemInfo GetInfo(Position pos)
    {
        var oneDimsionIndex = pos.X * CountOfOneGroup + pos.Y;
        return Infos[oneDimsionIndex];
    }

    #endregion
}

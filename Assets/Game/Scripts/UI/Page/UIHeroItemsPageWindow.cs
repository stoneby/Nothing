using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using UnityEngine;
using OrderType = ItemHelper.OrderType;

/// <summary>
/// The hero list window to show all heros.
/// </summary>
public class UIHeroItemsPageWindow : Window
{
    #region Private Fields

    private UIEventListener extendBagLis;
    private ExtendBag extendConfirm;
    private UIPanel panel;
    private UIGrid grid;
    private GameObject offset;
    private UIEventListener sortBtnLis;
    private UILabel heroNums;
    private UILabel sortLabel;
    private SCHeroList scHeroList;
    private UIToggle[] toggles; 
    private int rowToShow;

    #endregion

    #region Public Fields

    public GameObject ExtendConfirm;

    /// <summary>
    /// The rows the hero visible area on the current window.
    /// </summary>
    public int RowToShow
    {
        get
        {
            return rowToShow;
        }
        set
        {
            if (rowToShow != value)
            {
                rowToShow = value;
                var clipHeight = panel.baseClipRegion.w;
                panel.baseClipRegion = new Vector4(panel.baseClipRegion.x, panel.baseClipRegion.y, panel.baseClipRegion.z, rowToShow * grid.cellHeight);
                var pos = offset.transform.localPosition;
                offset.transform.localPosition = new Vector3(pos.x, pos.y - 0.5f * (panel.baseClipRegion.w - clipHeight), pos.z);
                pos = grid.transform.localPosition;
                grid.transform.localPosition = new Vector3(pos.x, pos.y + 0.5f * (panel.baseClipRegion.w - clipHeight), pos.z);
                panel.GetComponent<UIScrollView>().ResetPosition();
            }
        }
    }

    /// <summary>
    /// The prefab of the hero item.
    /// </summary>
    public GameObject HeroPrefab;

    #endregion

    #region Window

    public override void OnEnter()
    {
        Refresh();
        panel.GetComponent<UIScrollView>().ResetPosition();
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
        panel = GetComponentInChildren<UIPanel>();
        grid = GetComponentInChildren<UIGrid>();
        grid.onReposition += OnReposition;
        offset = Utils.FindChild(transform, "Offset").gameObject;
        heroNums = Utils.FindChild(transform, "HeroNums").GetComponent<UILabel>();
        sortBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Sort").gameObject);
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();
        toggles = GetComponentsInChildren<UIToggle>();
        scHeroList = HeroModelLocator.Instance.SCHeroList;
        extendBagLis = UIEventListener.Get(Utils.FindChild(transform, "ExtendBag").gameObject);
    }

    /// <summary>
    /// Fill in the hero game objects.
    /// </summary> 
    private void UpdateHeroList()
    {
        var heroCount = HeroModelLocator.Instance.SCHeroList.HeroList.Count;
        var childCount = grid.transform.childCount;
        if (childCount != heroCount)
        {
            var isAdd = childCount < heroCount;
            Utils.AddOrDelItems(grid.transform, HeroPrefab.transform, isAdd, Mathf.Abs(heroCount - childCount), "Heros",
                                OnHeroInfoClicked);
            grid.repositionNow = true;
        }
    }

    private void OnReposition()
    {
        var items = grid.transform;
        var childCount = Utils.GetActiveChildCount(items);
        Utils.MoveToParent(items, extendBagLis.transform);
        if (childCount != 0)
        {
            var maxPerLine = grid.maxPerLine;
            if (childCount % maxPerLine != 0)
            {
                extendBagLis.transform.localPosition = new Vector3((childCount % maxPerLine) * grid.cellWidth,
                                                                   -grid.cellHeight * (childCount / maxPerLine),
                                                                   0);
            }
            else
            {
                extendBagLis.transform.localPosition = new Vector3(0,
                                                                   -grid.cellHeight * (childCount / maxPerLine),
                                                                   0);
            }
        }
        extendBagLis.transform.parent = items.parent;
        extendBagLis.gameObject.SetActive(true);
    }

    /// <summary>
    /// Install all handlers.
    /// </summary>
    private void InstallHandlers()
    {
        extendBagLis.onClick = OnExtenBag;
        sortBtnLis.onClick = OnSortClicked;
        for (int i = 0; i < toggles.Length; i++)
        {
            EventDelegate.Add(toggles[i].onChange, ExcuteFilter);
        }
    }

    /// <summary>
    /// Uninstall all handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        sortBtnLis.onClick = null;
        extendBagLis.onClick = null;
        for (int i = 0; i < toggles.Length; i++)
        {
            EventDelegate.Remove(toggles[i].onChange, ExcuteFilter);
        }
    }

    private void OnExtenBag(GameObject go)
    {
        extendConfirm = NGUITools.AddChild(transform.gameObject, ExtendConfirm).GetComponent<ExtendBag>();
        var bases = ItemModeLocator.Instance.Bag;
        var costDict = bases.HeroExtTmpl.ToDictionary(item => item.Value.Id, item => item.Value.Cost);
        extendConfirm.Init(PlayerModelLocator.Instance.ExtendItemTimes + 1, bases.BagBaseTmpl[1].EachExtItemNum,
                           costDict);
        extendConfirm.OkClicked += OnExendBagOk;
    }

    private void OnExendBagOk(GameObject go)
    {
        var msg = new CSHeroMaxExtend{ ExtendTimes = extendConfirm.ExtendSize };
        NetManager.SendMessage(msg);
    }

    /// <summary>
    /// The callback of clicking sort button.
    /// </summary>
    private void OnSortClicked(GameObject go)
    {
        scHeroList.OrderType = (sbyte)((scHeroList.OrderType + 1) % StringTable.SortStrings.Count);
        sortLabel.text = StringTable.SortStrings[scHeroList.OrderType];
        Refresh();
    }

    /// <summary>
    /// The callback of clicking each hero item.
    /// </summary>
    private void OnHeroInfoClicked(GameObject go)
    {
        HeroBaseInfoWindow.CurUuid = go.GetComponent<HeroItem>().Uuid;
        WindowManager.Instance.Show(typeof(HeroBaseInfoWindow), true);
        WindowManager.Instance.Show(typeof(UIHeroInfoWindow), true);
    }

    /// <summary>
    /// Refresh the hero list.
    /// </summary>
    public void Refresh()
    {
        UpdateHeroList();
        var orderType = scHeroList.OrderType;
        sortLabel.text = StringTable.SortStrings[orderType];
        heroNums.text = string.Format("{0}/{1}", scHeroList.HeroList.Count, PlayerModelLocator.Instance.HeroMax);
        HeroModelLocator.Instance.SortHeroList((OrderType)orderType, scHeroList.HeroList);
        for (int i = 0; i < scHeroList.HeroList.Count; i++)
        {
            var heroInfo = scHeroList.HeroList[i];
            var item = grid.transform.GetChild(i).GetComponent<HeroItem>();
            item.InitItem(heroInfo);
            HeroUtils.ShowHero((OrderType)orderType, item, heroInfo);
        }
    }

    /// <summary>
    /// Filtering hero items with special job. 
    /// </summary>
    /// <remarks>
    /// This function can only be called in the callback of uitoggle's onChange, 
    /// as the UIToggle.current will be null in other places.
    /// </remarks>
    private void ExcuteFilter()
    {
        bool val = UIToggle.current.value;
        if (val)
        {
            var job = (sbyte)UIToggle.current.GetComponent<JobFilterInfo>().Job;
            var heros = HeroModelLocator.Instance.FilterByJob(job, scHeroList.HeroList);
            var filterObjects = new List<Transform>();
            for (int i = 0; i < scHeroList.HeroList.Count; i++)
            {
                if (i < heros.Count)
                {
                    var item = grid.transform.GetChild(i).GetComponent<HeroItem>();
                    filterObjects.Add(item.transform);
                    NGUITools.SetActive(item.gameObject, true);
                    item.GetComponent<BoxCollider>().enabled = true;
                }
                else
                {
                    var item = grid.transform.GetChild(i);
                    NGUITools.SetActive(item.gameObject, false);
                    item.GetComponent<BoxCollider>().enabled = false;
                }
            }
            grid.repositionNow = true;
            for (int i = 0; i < filterObjects.Count; i++)
            {
                var item = filterObjects[i].GetComponent<HeroItem>();
                HeroUtils.ShowHero((OrderType)scHeroList.OrderType, item, heros[i]);
            }
        }
    }

    #endregion

    #region Public Methods

    public void RefreshHeroCount()
    {
        sortLabel.text = StringTable.SortStrings[scHeroList.OrderType];
        heroNums.text = string.Format("{0}/{1}", scHeroList.HeroList.Count, PlayerModelLocator.Instance.HeroMax);
    }

    #endregion 
}

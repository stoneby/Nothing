using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;
using OrderType = ItemHelper.OrderType;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIHerosPageWindow : Window
{
    #region Private Fields

    private UIEventListener sortBtnLis;
    private UIPanel scrollPanel;
    private UIPanel panel;
    private Transform offset;
    private UILabel sortLabel;
    private int rowToShow;
    private List<HeroInfo> infos;
    private int depth;
    private int cachedDepth;
    private UIEventListener.VoidDelegate itemClicked;

    #endregion

    #region Public Fields

    [HideInInspector]
    public UIGrid Heros;

    /// <summary>
    /// The prefab of the hero.
    /// </summary>
    public GameObject HeroPrefab;

    public UIEventListener.VoidDelegate ItemClicked
    {
        get { return itemClicked; }
        set
        {
            itemClicked = value;
            var parent = Heros.transform;
            for (int i = 0; i < parent.childCount; i++)
            {
                var item = parent.GetChild(i);
                UIEventListener.Get(item.gameObject).onClick = value;
            }
        }
    }

    public int Depth
    {
        get { return depth; }
        set
        {
            if (value != depth)
            {
                depth = value;
                panel.depth = depth;
                scrollPanel.depth = depth + 1;
            }
        }
    }

    /// <summary>
    /// The rows the hero visible area on the current window.
    /// </summary>
    public int RowToShow
    {
        get { return rowToShow; }
        set
        {
            if (rowToShow != value)
            {
                rowToShow = value;
                var clipHeight = scrollPanel.baseClipRegion.w;
                scrollPanel.baseClipRegion = new Vector4(scrollPanel.baseClipRegion.x, scrollPanel.baseClipRegion.y,
                                                         scrollPanel.baseClipRegion.z, rowToShow * Heros.cellHeight);
                var pos = offset.transform.localPosition;
                offset.transform.localPosition = new Vector3(pos.x,
                                                             pos.y - 0.5f * (scrollPanel.baseClipRegion.w - clipHeight),
                                                             pos.z);
                scrollPanel.GetComponent<UIScrollView>().ResetPosition();
            }
        }
    }
    
    /// <summary>
    /// The call back of the sort order changed.
    /// </summary>
    public UIEventListener.VoidDelegate OnSortOrderChanged;

    #endregion

    #region Window

    public override void OnEnter()
    {
        scrollPanel.GetComponent<UIScrollView>().ResetPosition();
        infos = HeroModelLocator.Instance.SCHeroList.HeroList ?? new List<HeroInfo>();
        Refresh(infos);
    }

    public override void OnExit()
    {
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    void Awake()
    {
        sortBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Sort").gameObject);
        sortBtnLis.onClick += OnSortClicked;
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();
        scrollPanel = GetComponentInChildren<UIScrollView>().GetComponent<UIPanel>();
        Heros = GetComponentInChildren<UIGrid>();
        offset = Utils.FindChild(transform, "Offset");
        panel = GetComponent<UIPanel>();
        cachedDepth = panel.depth;
    }

    private void OnSortClicked(GameObject go)
    {
        var orderType = HeroModelLocator.Instance.OrderType;
        orderType = (OrderType)(((int)orderType + 1) % (ItemHelper.SortKeys.Count));
        sortLabel.text = LanguageManager.Instance.GetTextValue(ItemHelper.SortKeys[(int)orderType]);
        HeroModelLocator.Instance.OrderType = orderType;
        Refresh(infos);
        if(OnSortOrderChanged != null)
        {
            OnSortOrderChanged(go);
        }
    }

    /// <summary>
    /// Fill in the hero item game objects.
    /// </summary> 
    private void UpdateItemList(int heroCount)
    {
        var childCount = Heros.transform.childCount;
        if (childCount != heroCount)
        {
            var isAdd = childCount < heroCount;
            HeroUtils.AddOrDelItems(Heros.transform, HeroPrefab.transform, isAdd, Mathf.Abs(heroCount - childCount),
                                HeroConstant.HeroPoolName,
                                null);
        }
        Heros.repositionNow = true;
    }

    /// <summary>
    /// Refresh the heros page window with the special hero info list.
    /// </summary>
    /// <param name="newInfos">The hero info list to refresh data.</param>
    public void Refresh(List<HeroInfo> newInfos)
    {
        UpdateItemList(newInfos.Count);
        var orderType = HeroModelLocator.Instance.OrderType;
        sortLabel.text = LanguageManager.Instance.GetTextValue(ItemHelper.SortKeys[(int)orderType]);
        HeroModelLocator.Instance.SortHeroList(orderType, newInfos);
        for (int i = 0; i < newInfos.Count; i++)
        {
            var heroItem = Heros.transform.GetChild(i).GetComponent<HeroItem>();
            var info = newInfos[i];
            List<long> curTeamUuids;
            List<long> allTeamUuids;
            HeroModelLocator.Instance.GetTeamUuids(out curTeamUuids, out allTeamUuids, HeroModelLocator.Instance.SCHeroList.CurrentTeamIndex);
            heroItem.InitItem(info, curTeamUuids, allTeamUuids);
            HeroUtils.ShowHero(orderType, heroItem, info);
        }
        scrollPanel.GetComponent<UIScrollView>().ResetPosition();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Set back the panel depth.
    /// </summary>
    public void ResetDepth()
    {
        Depth = cachedDepth;
    }

    #endregion
}

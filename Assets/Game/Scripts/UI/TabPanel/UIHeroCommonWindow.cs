using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

public class UIHeroCommonWindow : Window 
{
    private UIEventListener sortBtnLis;

    private UILabel sortLabel;
    private UILabel herosNum;
    private List<HeroInfo> infos;

    private UIToggle[] toggles;
    private SCHeroList scHeroList;
    private UIEventListener.VoidDelegate normalClicked;
    private UIScrollView scrollView;

    [HideInInspector]
    public UIGrid Heros;

    /// <summary>
    /// The prefab of the hero.
    /// </summary>
    public GameObject HeroPrefab;

    /// <summary>
    /// The call back of the sort order changed.
    /// </summary>
    public UIEventListener.VoidDelegate OnSortOrderChanged;

    public UIEventListener.VoidDelegate NormalClicked
    {
        get { return normalClicked; }
        set
        {
            normalClicked = value;
            var parent = Heros.transform;
            for (int i = 0; i < parent.childCount; i++)
            {
                var item = parent.GetChild(i);
                var longPressDetecter = item.GetComponent<NGUILongPress>();
                longPressDetecter.OnNormalPress = value;
            }
        }
    }
 
    #region Window

    public override void OnEnter()
    {
        Refresh(infos);
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
        sortBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Sort").gameObject);
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();
        herosNum = Utils.FindChild(transform, "HeroNumValue").GetComponent<UILabel>();
        Heros = GetComponentInChildren<UIGrid>();
        toggles = transform.Find("ToggleButtons").GetComponentsInChildren<UIToggle>();
        scHeroList = HeroModelLocator.Instance.SCHeroList;
        infos = scHeroList.HeroList ?? new List<HeroInfo>();
        scrollView = transform.Find("Container/Scroll View").GetComponent<UIScrollView>();
    }

    private void InstallHandlers()
    {
        for (var i = 0; i < toggles.Length; i++)
        {
            EventDelegate.Add(toggles[i].onChange, ExcuteFilter);
        }
        sortBtnLis.onClick = OnSortClicked;
    }

    private void UnInstallHandlers()
    {
        for (var i = 0; i < toggles.Length; i++)
        {
            EventDelegate.Remove(toggles[i].onChange, ExcuteFilter);
        }
        sortBtnLis.onClick = null;
    }

    private void OnSortClicked(GameObject go)
    {
        var orderType = HeroModelLocator.Instance.OrderType;
        orderType = (ItemHelper.OrderType)(((int)orderType + 1) % StringTable.SortStrings.Count);
        sortLabel.text = StringTable.SortStrings[(int)orderType];
        HeroModelLocator.Instance.OrderType = orderType;
        Refresh(infos);
        if (OnSortOrderChanged != null)
        {
            OnSortOrderChanged(go);
        }
    }

    /// <summary>
    /// Fill in the hero item game objects.
    /// </summary> 
    private void UpdateItemList(int heroCount)
    {
        var childCount = Utils.GetActiveChildCount(Heros.transform);
        if (childCount != heroCount)
        {
            var isAdd = childCount < heroCount;
            HeroUtils.AddOrDelItems(Heros.transform, HeroPrefab.transform, isAdd, Mathf.Abs(heroCount - childCount),
                                HeroConstant.HeroPoolName,
                                null,
                                OnLongPress);
        }
        Heros.repositionNow = true;
    }

    private void OnLongPress(GameObject go)
    {
        UIHeroDetailWindow.IsLongPressEnter = true;
        var heroDetail = WindowManager.Instance.Show<UIHeroDetailWindow>(true);
        var uuid = go.GetComponent<NewHeroItem>().Uuid;
        var info = HeroModelLocator.Instance.FindHero(uuid);
        heroDetail.RefreshData(info);
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
            infos = HeroModelLocator.Instance.FilterByJob(job, scHeroList.HeroList);
            var filterObjects = new List<Transform>();
            for (int i = 0; i < scHeroList.HeroList.Count; i++)
            {
                if (i < infos.Count)
                {
                    var item = Heros.transform.GetChild(i).GetComponent<NewHeroItem>();
                    filterObjects.Add(item.transform);
                    NGUITools.SetActive(item.gameObject, true);
                }
                else
                {
                    var item = Heros.transform.GetChild(i);
                    NGUITools.SetActive(item.gameObject, false);
                }
            }
            Heros.repositionNow = true;
            for (var i = 0; i < filterObjects.Count; i++)
            {
                var item = filterObjects[i].GetComponent<NewHeroItem>();
                item.InitItem(infos[i]);
            }
            scrollView.ResetPosition();
        }
    }

    /// <summary>
    /// Refresh the heros page window with the special hero info list.
    /// </summary>
    /// <param name="newInfos">The hero info list to refresh data.</param>
    /// <param name="teamIndex"> </param>
    public void Refresh(List<HeroInfo> newInfos, int teamIndex)
    {
        herosNum.text = string.Format("{0}/{1}", scHeroList.HeroList.Count, PlayerModelLocator.Instance.HeroMax);
        UpdateItemList(newInfos.Count);
        var orderType = HeroModelLocator.Instance.OrderType;
        sortLabel.text = StringTable.SortStrings[(int)orderType];
        HeroModelLocator.Instance.SortHeroList(orderType, newInfos);
        List<long> curTeamUuids;
        List<long> allTeamUuids;
        HeroModelLocator.Instance.GetTeamUuids(out curTeamUuids, out allTeamUuids, teamIndex);
        for (int i = 0; i < newInfos.Count; i++)
        {
            var heroItem = Heros.transform.GetChild(i).GetComponent<NewHeroItem>();
            var info = newInfos[i];
            heroItem.InitItem(info, curTeamUuids, allTeamUuids);
            HeroUtils.ShowHero(orderType, heroItem, info);
        }
    }

    /// <summary>
    /// Refresh the heros page window with the special hero info list.
    /// </summary>
    /// <param name="newInfos">The hero info list to refresh data.</param>
    public void Refresh(List<HeroInfo> newInfos)
    {
        Refresh(newInfos, HeroModelLocator.Instance.SCHeroList.CurrentTeamIndex);
    }

    public void Refresh(int teamIndex)
    {
        Refresh(infos, teamIndex);
    }

    #endregion
}

using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

public class UIHeroCommonWindow : Window 
{
    private UIEventListener sortBtnLis;
    private UILabel sortLabel;
    private UILabel herosNum;
    private List<HeroInfo> infos;
    private UIGrid heros;
    private UIToggle[] toggles;
    private SCHeroList scHeroList;

    /// <summary>
    /// The prefab of the hero.
    /// </summary>
    public GameObject HeroPrefab;
 
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
        sortBtnLis.onClick += OnSortClicked;
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();
        herosNum = Utils.FindChild(transform, "HeroNumValue").GetComponent<UILabel>();
        heros = GetComponentInChildren<UIGrid>();
        toggles = transform.Find("ToggleButtons").GetComponentsInChildren<UIToggle>();
        scHeroList = HeroModelLocator.Instance.SCHeroList;
        infos = scHeroList.HeroList ?? new List<HeroInfo>();
    }

    private void InstallHandlers()
    {
        for (var i = 0; i < toggles.Length; i++)
        {
            EventDelegate.Add(toggles[i].onChange, ExcuteFilter);
        }
    }

    private void UnInstallHandlers()
    {
        for (var i = 0; i < toggles.Length; i++)
        {
            EventDelegate.Remove(toggles[i].onChange, ExcuteFilter);
        }
    }

    private void OnSortClicked(GameObject go)
    {
        var orderType = HeroModelLocator.Instance.OrderType;
        orderType = (ItemHelper.OrderType)(((int)orderType + 1) % StringTable.SortStrings.Count);
        sortLabel.text = StringTable.SortStrings[(int)orderType];
        HeroModelLocator.Instance.OrderType = orderType;
        Refresh(infos);
    }

    /// <summary>
    /// Fill in the hero item game objects.
    /// </summary> 
    private void UpdateItemList(int heroCount)
    {
        var childCount = Utils.GetActiveChildCount(heros.transform);
        if (childCount != heroCount)
        {
            var isAdd = childCount < heroCount;
            HeroUtils.AddOrDelItems(heros.transform, HeroPrefab.transform, isAdd, Mathf.Abs(heroCount - childCount),
                                HeroConstant.HeroPoolName,
                                OnNormalPress,
                                OnLongPress);
        }
        heros.repositionNow = true;
    }

    private void OnNormalPress(GameObject go)
    {
        UIHeroSnapShotWindow.CurUuid = go.GetComponent<HeroItemBase>().Uuid;
        WindowManager.Instance.Show<UIHeroSnapShotWindow>(true);
    }

    private void OnLongPress(GameObject go)
    {
        WindowManager.Instance.Show<UIHeroDetailWindow>(true);
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
                    var item = heros.transform.GetChild(i).GetComponent<HeroItemBase>();
                    filterObjects.Add(item.transform);
                    NGUITools.SetActive(item.gameObject, true);
                    item.GetComponent<BoxCollider>().enabled = true;
                }
                else
                {
                    var item = heros.transform.GetChild(i);
                    NGUITools.SetActive(item.gameObject, false);
                    item.GetComponent<BoxCollider>().enabled = false;
                }
            }
            heros.repositionNow = true;
            for (var i = 0; i < filterObjects.Count; i++)
            {
                var item = filterObjects[i].GetComponent<HeroItemBase>();
                item.InitItem(infos[i]);
            }
        }
    }

    /// <summary>
    /// Refresh the heros page window with the special hero info list.
    /// </summary>
    /// <param name="newInfos">The hero info list to refresh data.</param>
    public void Refresh(List<HeroInfo> newInfos)
    {
        herosNum.text = string.Format("{0}/{1}", scHeroList.HeroList.Count, PlayerModelLocator.Instance.HeroMax);
        UpdateItemList(newInfos.Count);
        var orderType = HeroModelLocator.Instance.OrderType;
        sortLabel.text = StringTable.SortStrings[(int)orderType];
        HeroModelLocator.Instance.SortHeroList(orderType, newInfos);
        for (int i = 0; i < newInfos.Count; i++)
        {
            var heroItem = heros.transform.GetChild(i).GetComponent<HeroItemBase>();
            var info = newInfos[i];
            heroItem.InitItem(info);
        }
    }

    #endregion
}

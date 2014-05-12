using System.Globalization;
using System.Linq;
using KXSGCodec;
using Property;
using UnityEngine;
using System.Collections;

/// <summary>
/// The hero list window to show all heros.
/// </summary>
public class UIHeroItemsPageWindow : Window
{
    #region Private Fields

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
        scHeroList = HeroModelLocator.Instance.SCHeroList;
        sortLabel.text = StringTable.SortStrings[scHeroList.OrderType];
		heroNums.text = string.Format("{0}/{1}", scHeroList.HeroList.Count, PlayerModelLocator.Instance.HeroMax);
        StartCoroutine(FillHeroList());
        Refresh();
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        if (PoolManager.Pools.ContainsKey("Heros"))
        {
            var list = grid.transform.Cast<Transform>().ToList();
            for(int index = 0; index < list.Count; index++)
            {
                var item = list[index];
                UIEventListener.Get(item.gameObject).onClick -= OnHeroInfoClicked;
                item.parent = PoolManager.Pools["Heros"].transform;
                PoolManager.Pools["Heros"].Despawn(item);
            }
        }
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    void Awake()
    {
        panel = GetComponentInChildren<UIPanel>();
        grid = GetComponentInChildren<UIGrid>();
        offset = Utils.FindChild(transform, "Offset").gameObject;
        heroNums = Utils.FindChild(transform, "HeroNums").GetComponent<UILabel>();
        sortBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Sort").gameObject);
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();
        toggles = GetComponentsInChildren<UIToggle>();
    }

    /// <summary>
    /// Fill in the hero game objects.
    /// </summary> 
    private IEnumerator FillHeroList()
    {
        var heroCount = HeroModelLocator.Instance.SCHeroList.HeroList.Count;
        for (int i = 0; i < heroCount; i++)
        {
            var item = PoolManager.Pools["Heros"].Spawn(HeroPrefab.transform);
            Utils.MoveToParent(grid.transform, item);
            NGUITools.SetActive(item.gameObject, true);
            UIEventListener.Get(item.gameObject).onClick += OnHeroInfoClicked;
        }
        grid.repositionNow = true;
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// Install all handlers.
    /// </summary>
    private void InstallHandlers()
    {
        sortBtnLis.onClick += OnSortClicked;
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
        sortBtnLis.onClick -= OnSortClicked;
        for (int i = 0; i < toggles.Length; i++)
        {
            EventDelegate.Remove(toggles[i].onChange, ExcuteFilter);
        }
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
        HeroBaseInfoWindow.CurUuid = go.GetComponent<HeroInfoPack>().Uuid;
        WindowManager.Instance.Show(typeof(HeroBaseInfoWindow), true);
        WindowManager.Instance.Show(typeof(UIHeroInfoWindow), true);
    }

    /// <summary>
    /// Refresh the hero list.
    /// </summary>
    private void Refresh()
    {
        var orderType = scHeroList.OrderType;
        HeroModelLocator.Instance.SortHeroList(orderType, scHeroList.HeroList);
        for (int i = 0; i < scHeroList.HeroList.Count; i++)
        {
            var item = grid.transform.GetChild(i);
            var uUid = scHeroList.HeroList[i].Uuid;
            item.GetComponent<HeroInfoPack>().Uuid = uUid;
            ShowHero(orderType, item, uUid);
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
            for (int i = 0; i < scHeroList.HeroList.Count; i++)
            {
                if (i < heros.Count)
                {
                    var item = grid.transform.GetChild(i);
                    NGUITools.SetActiveChildren(item.gameObject, true);
                    item.GetComponent<BoxCollider>().enabled = true;
                    var uUid = heros[i].Uuid;
                    item.GetComponent<HeroInfoPack>().Uuid = uUid;
                    ShowHero(scHeroList.OrderType, item, uUid);
                }
                else
                {
                    var item = grid.transform.GetChild(i);
                    NGUITools.SetActiveChildren(item.gameObject, false);
                    item.GetComponent<BoxCollider>().enabled = false;
                }
            }
        }
    }

    /// <summary>
    /// Show the info of the hero.
    /// </summary>
    /// <param name="orderType">The order type of </param>
    /// <param name="heroTran">The transform of hero.</param>
    /// <param name="uUid">The template id of hero.</param>
    private void ShowHero(short orderType, Transform heroTran, long uUid)
    {
        var heroInfo = HeroModelLocator.Instance.FindHero(uUid);
        var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
        var sortRelated = Utils.FindChild(heroTran, "SortRelated");
        var stars = Utils.FindChild(heroTran, "Rarity");
        for (int index = 0; index < heroTemplate.Star; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, true);
        }
        for (int index = heroTemplate.Star; index < stars.transform.childCount; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, false);
        }
        switch (orderType)
        {
            //ÈëÊÖË³ÐòÅÅÐò
            case 0:
                ShowByLvl(sortRelated, heroInfo);
                break;

            //Îä½«Ö°ÒµÅÅÐò
            case 1:
                ShowByJob(sortRelated, heroInfo, heroTemplate);
                break;

            //Îä½«Ï¡ÓÐ¶ÈÅÅÐò
            case 2:
                ShowByLvl(sortRelated, heroInfo);
                break;

            //ÕÕ¶ÓÎéË³ÐòÅÅÐò
            case 3:
                ShowByLvl(sortRelated, heroInfo);
                break;

            //¹¥»÷Á¦ÅÅÐò
            case 4:
                ShowByJob(sortRelated, heroInfo, heroTemplate);
                break;

            //HPÅÅÐò
            case 5:
                ShowByHp(sortRelated, heroInfo);
                break;

            //»Ø¸´Á¦ÅÅÐò
            case 6:
                ShowByRecover(sortRelated, heroInfo);
                break;

            //µÈ¼¶ÅÅÐò
            case 7:
                ShowByLvl(sortRelated, heroInfo);
                break;
        }
    }

    /// <summary>
    /// Show each hero items with the job info.
    /// </summary>
    private void ShowByJob(Transform sortRelated, HeroInfo heroInfo, HeroTemplate heroTemplate)
    {
        var jobSymobl = Utils.FindChild(sortRelated, "JobSymbol").GetComponent<UISprite>();
        var attack = Utils.FindChild(sortRelated, "Attack").GetComponent<UILabel>();
        jobSymobl.spriteName = UIHerosDisplayWindow.JobPrefix + heroTemplate.Job;
        attack.text = heroInfo.Prop[RoleProperties.HERO_ATK].ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelated.gameObject, false);
        NGUITools.SetActive(jobSymobl.gameObject, true);
        NGUITools.SetActive(attack.gameObject, true);
    }

    /// <summary>
    /// Show each hero items with the hp info.
    /// </summary>
    private void ShowByHp(Transform sortRelated, HeroInfo heroInfo)
    {
        var hp = Utils.FindChild(sortRelated, "HP-Title");
        var hpValue = Utils.FindChild(sortRelated, "HP-Value").GetComponent<UILabel>();
        hpValue.text = heroInfo.Prop[RoleProperties.HERO_HP].ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelated.gameObject, false);
        NGUITools.SetActive(hp.gameObject, true);
        NGUITools.SetActive(hpValue.gameObject, true);
    }

    /// <summary>
    /// Show each hero items with the recover info.
    /// </summary>
    private void ShowByRecover(Transform sortRelated, HeroInfo heroInfo)
    {
        var recover = Utils.FindChild(sortRelated, "Recover-Title");
        var recoverValue = Utils.FindChild(sortRelated, "Recover-Value").GetComponent<UILabel>();
        recoverValue.text = heroInfo.Prop[RoleProperties.HERO_RECOVER].ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelated.gameObject, false);
        NGUITools.SetActive(recover.gameObject, true);
        NGUITools.SetActive(recoverValue.gameObject, true);
    }

    /// <summary>
    /// Show each hero items with the level info.
    /// </summary>
    private void ShowByLvl(Transform sortRelated, HeroInfo heroInfo)
    {
        var lvTitle = Utils.FindChild(sortRelated, "LV-Title");
        var lvValue = Utils.FindChild(sortRelated, "LV-Value").GetComponent<UILabel>();
        lvValue.text = heroInfo.Lvl.ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelated.gameObject, false);
        NGUITools.SetActive(lvTitle.gameObject, true);
        NGUITools.SetActive(lvValue.gameObject, true);
    }

    #endregion
}

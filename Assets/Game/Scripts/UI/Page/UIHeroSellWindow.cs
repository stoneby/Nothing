using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KXSGCodec;
using Property;
using UnityEngine;

/// <summary>
/// The sell heros window.
/// </summary>
public class UIHeroSellWindow : Window
{
    #region Public Fields

    public GameObject HeroPrefab;
    public GameObject SellHeroPic;

    #endregion

    #region Private Fields

    private UIEventListener okLis;
    private UIEventListener cancelLis;
    private UIEventListener backLis;  
    private UIEventListener sellOkLis;
    private UIEventListener sellCancelLis;
    private UIEventListener sortBtnLis;
    private UILabel heroNums;
    private SCHeroList scHeroList;
    private UIGrid herosGrid;
    private UIGrid sellGrid;
    private Transform sellDialog;
    private GameObject mask;
    private UILabel selectCount;
    private UILabel soulCount;
    private long totalSoul;
    private Hero hero;
    private UILabel sortLabel;
    private List<long> teamMembers = new List<long>(); 
    private readonly CSHeroSell csHeroSell = new CSHeroSell { SellList = new List<long>() };
    private readonly List<GameObject> sellHeros = new List<GameObject>();
    private readonly Color disableColor = Color.gray;
    private const int MaxHeroCountCanSell = 10;

    #endregion

    #region Window

    public override void OnEnter()
    {
        sortLabel.text = StringTable.SortStrings[scHeroList.OrderType];
        InstallHandlers();
        FillHeroList();
        InitData();
        Refresh();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        DespawnHeros();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Awake()
    {
        okLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Ok").gameObject);
        cancelLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Cancel").gameObject);
        backLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Back").gameObject);
        sortBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Sort").gameObject);
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();

        herosGrid = Utils.FindChild(transform, "HerosGrid").GetComponent<UIGrid>();
        scHeroList = HeroModelLocator.Instance.SCHeroList;
        heroNums = Utils.FindChild(transform, "HeroNums").GetComponent<UILabel>();
        sellDialog = Utils.FindChild(transform, "SellDialog");
        sellOkLis = UIEventListener.Get(Utils.FindChild(sellDialog, "Button-SellOk").gameObject);
        sellCancelLis = UIEventListener.Get(Utils.FindChild(sellDialog, "Button-SellCancel").gameObject);
        sellGrid = sellDialog.GetComponentInChildren<UIGrid>();
        sellDialog.gameObject.SetActive(false);
        mask = Utils.FindChild(transform, "Mask").gameObject;
        mask.SetActive(false);
        selectCount = Utils.FindChild(transform, "Selected-Value").GetComponent<UILabel>();
        soulCount = Utils.FindChild(transform, "Soul-Value").GetComponent<UILabel>();
        hero = HeroModelLocator.Instance.HeroTemplates;
    }

    /// <summary>
    /// Install all handlers.
    /// </summary>
    private void InstallHandlers()
    {
        okLis.onClick += OnOkClicked;
        cancelLis.onClick += OnCancelClicked;
        backLis.onClick += OnBackClicked;
        sellOkLis.onClick += OnSellOkClicked;
        sellCancelLis.onClick += OnSellCancelClicked;
        sortBtnLis.onClick += OnSortClicked;
    }
    
    /// <summary>
    /// Uninstall all handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        okLis.onClick -= OnOkClicked;
        cancelLis.onClick -= OnCancelClicked;
        backLis.onClick -= OnBackClicked;
        sellOkLis.onClick -= OnSellOkClicked;
        sellCancelLis.onClick -= OnSellCancelClicked;
        sortBtnLis.onClick -= OnSortClicked;
    }

    /// <summary>
    /// Add the heros.
    /// </summary>
    private void FillHeroList()
    {
        var heroCount = scHeroList.HeroList.Count;
        for (int index = 0; index < heroCount; index++)
        {
            var item = PoolManager.Pools["Heros"].Spawn(HeroPrefab.transform);
            Utils.MoveToParent(herosGrid.transform, item);
            NGUITools.SetActive(item.gameObject, true);
            item.name = item.name + index;
            UIEventListener.Get(item.gameObject).onClick += OnHeroItemClicked;
        }
        herosGrid.Reposition();
    }

    /// <summary>
    /// Refresh some ui items when we needed.
    /// </summary>
    private void Refresh()
    {
        selectCount.text = sellHeros.Count.ToString(CultureInfo.InvariantCulture);
        soulCount.text = totalSoul.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Init the ui data when we enter the window.
    /// </summary>
    private void InitData()
    {
        heroNums.text = string.Format("{0}/{1}", scHeroList.HeroList.Count, PlayerModelLocator.Instance.HeroMax);
        var count = scHeroList.TeamList.Count;
        for (int teamIndex = 0; teamIndex < count; teamIndex++)
        {
            for (int uUidIndex = 0; uUidIndex < scHeroList.TeamList[teamIndex].ListHeroUuid.Count; uUidIndex++)
            {
                if (scHeroList.TeamList[teamIndex].ListHeroUuid[uUidIndex] != UITeamEditWindow.DefaultNonHero)
                {
                    teamMembers.Add(scHeroList.TeamList[teamIndex].ListHeroUuid[uUidIndex]);
                }
            }
        }
        teamMembers = teamMembers.Distinct().ToList();
        var orderType = HeroModelLocator.Instance.SCHeroList.OrderType;
        HeroModelLocator.Instance.SortHeroList(orderType,scHeroList.HeroList);
        for (int index = 0; index < scHeroList.HeroList.Count; index++)
        {
            var item = herosGrid.transform.GetChild(index);
            var uUid = scHeroList.HeroList[index].Uuid;
            item.GetComponent<HeroInfoPack>().Uuid = uUid;
            CheckState(item, uUid);
            ShowHero(orderType, item, uUid);
        }
    }

    /// <summary>
    /// Reset back of some variables.
    /// </summary>
    private void Reset()
    {
        var list = sellGrid.transform.Cast<Transform>().ToList();
        for (int index = list.Count - 1; index >= 0; index--)
        {
            Destroy(list[index].gameObject);
        }
        sellDialog.gameObject.SetActive(false);
        sellHeros.Clear();
        totalSoul = 0;
    }

    /// <summary>
    /// Check the hero with special uuid can be sold.
    /// </summary>
    /// <param name="uUid">The uuid of the hero to be checked.</param>
    /// <returns>If true, the hero with the special uuid can be sold.</returns>
    private bool CanSell(long uUid)
    {
        return !teamMembers.Contains(uUid);
    }
    
    /// <summary>
    /// Check if the hero with special uuid can be clicked or not.
    /// </summary>
    /// <param name="heroObj">The transfrom of the hero to be checked.</param>
    /// <param name="uUid">The uuid of the hero.</param>
    private void CheckState(Transform heroObj, long uUid)
    {
        if (!CanSell(uUid))
        {
            heroObj.GetComponent<BoxCollider>().enabled = false;
            heroObj.FindChild("BG").GetComponent<UISprite>().color = disableColor;
            heroObj.FindChild("Hero").GetComponent<UISprite>().color = disableColor;
        }
        else
        {
            heroObj.GetComponent<BoxCollider>().enabled = true;
            heroObj.FindChild("BG").GetComponent<UISprite>().color = Color.white;
            heroObj.FindChild("Hero").GetComponent<UISprite>().color = Color.white;
        }
    }

    /// <summary>
    /// Despawn hero instance to the hero pool.
    /// </summary>
    private void DespawnHeros()
    {
        if (PoolManager.Pools.ContainsKey("Heros"))
        {
            var list = herosGrid.transform.Cast<Transform>().ToList();
            for (int index = 0; index < list.Count; index++)
            {
                var item = list[index];
                var maskToDel = item.FindChild("Mask(Clone)");
                if (maskToDel != null)
                {
                    Destroy(maskToDel.gameObject);
                }
                UIEventListener.Get(item.gameObject).onClick -= OnHeroItemClicked;
                item.parent = PoolManager.Pools["Heros"].transform;
                PoolManager.Pools["Heros"].Despawn(item);
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
    /// The callback of clicking sort button.
    /// </summary>
    private void OnSortClicked(GameObject go)
    {
        var orderType = HeroModelLocator.Instance.SCHeroList.OrderType;
        orderType = (sbyte)((orderType + 1) % StringTable.SortStrings.Count);
        scHeroList.OrderType = orderType;
        sortLabel.text = StringTable.SortStrings[scHeroList.OrderType];
        HeroModelLocator.Instance.SortHeroList(orderType, scHeroList.HeroList);
        for (int i = 0; i < scHeroList.HeroList.Count; i++)
        {
            var item = herosGrid.transform.GetChild(i);
            var uUid = scHeroList.HeroList[i].Uuid;
            item.GetComponent<HeroInfoPack>().Uuid = uUid;
            ShowHero(orderType, item, uUid);
        }
    }

    /// <summary>
    /// The callback of clicking sell ok button.
    /// </summary>
    private void OnSellOkClicked(GameObject go)
    {
        NetManager.SendMessage(csHeroSell);
    }

    /// <summary>
    /// The callback of clicking sell cancel button.
    /// </summary>
    private void OnSellCancelClicked(GameObject go)
    {
        var list = sellGrid.transform.Cast<Transform>().ToList();
        for (int index = list.Count - 1; index >= 0; index--)
        {
            Destroy(list[index].gameObject);
        }
        sellDialog.gameObject.SetActive(false);
    }

    /// <summary>
    /// The callback of clicking each hero item.
    /// </summary>
    private void OnHeroItemClicked(GameObject go)
    {
        var uUid = go.GetComponent<HeroInfoPack>().Uuid;
        if (csHeroSell.SellList.Count >= MaxHeroCountCanSell && !csHeroSell.SellList.Contains(uUid))
        {
            return;
        }
        var heroInfo = HeroModelLocator.Instance.FindHero(uUid);
        var baseSoul = hero.HeroTmpl[heroInfo.TemplateId].Price;
        long costSoul = 0;
        var level = heroInfo.Lvl;
        var stars = hero.HeroTmpl[heroInfo.TemplateId].Star;
        switch (stars)
        {
            case 1:
                for (int index = 1; index < level; index++)
                {
                    costSoul += hero.LvlUpTmpl[index].CostSoulStar1;
                }
                break;
            case 2:
                for (int index = 1; index < level; index++)
                {
                    costSoul += hero.LvlUpTmpl[index].CostSoulStar2;
                }
                break;
            case 3:
                for (int index = 1; index < level; index++)
                {
                    costSoul += hero.LvlUpTmpl[index].CostSoulStar3;
                }
                break;
            case 4:
                for (int index = 1; index < level; index++)
                {
                    costSoul += hero.LvlUpTmpl[index].CostSoulStar4;
                }
                break;
            case 5:
                for (int index = 1; index < level; index++)
                {
                    costSoul += hero.LvlUpTmpl[index].CostSoulStar5;
                }
                break;
        }
        if(!csHeroSell.SellList.Contains(uUid))
        {
            csHeroSell.SellList.Add(uUid);
            var maskToAdd = NGUITools.AddChild(go, mask);
            maskToAdd.SetActive(true);
            sellHeros.Add(go);
            totalSoul += (long)(baseSoul + costSoul * hero.BaseTmpl[1].SellCoeff);
        }
        else
        {
            csHeroSell.SellList.Remove(uUid);
            Destroy(go.transform.FindChild("Mask(Clone)").gameObject);
            sellHeros.Remove(go);
            totalSoul -= (long)(baseSoul + costSoul * hero.BaseTmpl[1].SellCoeff);
        }
        Refresh();
    }

    /// <summary>
    /// The callback of clicking ok button.
    /// </summary>
    private void OnOkClicked(GameObject go)
    {
        sellDialog.gameObject.SetActive(true);
        for (int index = 0; index < csHeroSell.SellList.Count; index++)
        {
            NGUITools.AddChild(sellGrid.gameObject, SellHeroPic);
        }
        sellGrid.Reposition();
    }

    /// <summary>
    /// The callback of clicking cancel button.
    /// </summary>
    private void OnCancelClicked(GameObject go)
    {
        csHeroSell.SellList.Clear();
        for (int index = 0; index < sellHeros.Count; index++)
        {
            Destroy(sellHeros[index].transform.FindChild("Mask(Clone)").gameObject);
        }
    }

    /// <summary>
    /// The callback of clicking back button.
    /// </summary>
    private void OnBackClicked(GameObject go)
    {
        WindowManager.Instance.Show(typeof(UIHeroItemsPageWindow), true);
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

    #region Public Methods

    /// <summary>
    /// Update data when sell finished.
    /// </summary>
    public void SellOverUpdate()
    {
        heroNums.text = string.Format("{0}/{1}", scHeroList.HeroList.Count, PlayerModelLocator.Instance.HeroMax);
        var count = herosGrid.transform.childCount;
        var uuids = scHeroList.HeroList.Select(item => item.Uuid).ToList();
        for (int index = 0; index < count; index++)
        {
            var item = herosGrid.transform.GetChild(index).gameObject;
            if (!uuids.Contains(item.GetComponent<HeroInfoPack>().Uuid))
            {
                Destroy(item);
            }
        }
        herosGrid.repositionNow = true;
        OnCancelClicked(null);
        Reset();
        Refresh();
    }

    #endregion
}

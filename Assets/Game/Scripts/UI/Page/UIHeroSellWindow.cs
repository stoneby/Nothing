using KXSGCodec;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Template;
using UnityEngine;
using OrderType = ItemHelper.OrderType;

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
    private readonly List<Transform> canNotSells = new List<Transform>(); 
    private readonly Color disableColor = Color.gray;
    private const int MaxHeroCountCanSell = 10;

    #endregion

    #region Window

    public override void OnEnter()
    {
        sortLabel.text = StringTable.SortStrings[scHeroList.OrderType];
        InstallHandlers();
        InitData();
        Refresh();
        RefreshSelAndSoul();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        CleanUp();
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
        okLis.onClick = OnOkClicked;
        cancelLis.onClick = OnCancelClicked;
        sellOkLis.onClick = OnSellOkClicked;
        sellCancelLis.onClick = OnSellCancelClicked;
        sortBtnLis.onClick = OnSortClicked;
    }
    
    /// <summary>
    /// Uninstall all handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        okLis.onClick = null;
        cancelLis.onClick = null;
        sellOkLis.onClick = null;
        sellCancelLis.onClick = null;
        sortBtnLis.onClick = null;
    }

    private void UpdateHeroList()
    {
        var heroCount = HeroModelLocator.Instance.SCHeroList.HeroList.Count;
        var childCount = herosGrid.transform.childCount;
        if (childCount != heroCount)
        {
            var isAdd = childCount < heroCount;
            Utils.AddOrDelItems(herosGrid.transform, HeroPrefab.transform, isAdd, Mathf.Abs(heroCount - childCount), "Heros",
                                OnHeroItemClicked);
            herosGrid.repositionNow = true;
        }
    }

    /// <summary>
    /// Refresh some ui items when we needed.
    /// </summary>
    private void RefreshSelAndSoul()
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
                if (scHeroList.TeamList[teamIndex].ListHeroUuid[uUidIndex] != HeroConstant.NoneInitHeroUuid)
                {
                    teamMembers.Add(scHeroList.TeamList[teamIndex].ListHeroUuid[uUidIndex]);
                }
            }
        }
        teamMembers = teamMembers.Distinct().ToList();
    }

    public void Refresh()
    {
        UpdateHeroList();
        var orderType = HeroModelLocator.Instance.SCHeroList.OrderType;
        HeroModelLocator.Instance.SortHeroList((OrderType)orderType, scHeroList.HeroList);
        for (int index = 0; index < scHeroList.HeroList.Count; index++)
        {
            var item = herosGrid.transform.GetChild(index).GetComponent<HeroItem>();
            var uUid = scHeroList.HeroList[index].Uuid;
            var info = scHeroList.HeroList[index];
            CheckState(item.transform, uUid);
            item.InitItem(info);
            HeroUtils.ShowHero((OrderType)orderType, item, info);
        }
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
            canNotSells.Add(heroObj);
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
    /// The callback of clicking sort button.
    /// </summary>
    private void OnSortClicked(GameObject go)
    {
        var orderType = HeroModelLocator.Instance.SCHeroList.OrderType;
        orderType = (sbyte)((orderType + 1) % StringTable.SortStrings.Count);
        scHeroList.OrderType = orderType;
        sortLabel.text = StringTable.SortStrings[scHeroList.OrderType];
        HeroModelLocator.Instance.SortHeroList((OrderType)orderType, scHeroList.HeroList);
        for (int i = 0; i < scHeroList.HeroList.Count; i++)
        {
            var item = herosGrid.transform.GetChild(i).GetComponent<HeroItem>();
            var info = scHeroList.HeroList[i];
            item.InitItem(info);
            HeroUtils.ShowHero((OrderType)orderType, item, info);
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
        CleanSellDialog();
    }

    /// <summary>
    /// The callback of clicking each hero item.
    /// </summary>
    private void OnHeroItemClicked(GameObject go)
    {
        var uUid = go.GetComponent<HeroItem>().Uuid;
        if (csHeroSell.SellList.Count >= MaxHeroCountCanSell && !csHeroSell.SellList.Contains(uUid))
        {
            return;
        }
        var heroInfo = HeroModelLocator.Instance.FindHero(uUid);
        var baseSoul = hero.HeroTmpl[heroInfo.TemplateId].Price;
        var level = heroInfo.Lvl;
        var stars = hero.HeroTmpl[heroInfo.TemplateId].Star;
        var costSoul = GetCostSoul(stars, level);
        if(!csHeroSell.SellList.Contains(uUid))
        {
            csHeroSell.SellList.Add(uUid);
            var maskToAdd = NGUITools.AddChild(go, mask);
            go.transform.FindChild("BG").GetComponent<UISprite>().color = Color.gray;
            maskToAdd.SetActive(true);
            sellHeros.Add(go);
            totalSoul += (long)(baseSoul + costSoul * hero.BaseTmpl[1].SellCoeff);
        }
        else
        {
            csHeroSell.SellList.Remove(uUid);
            Destroy(go.transform.FindChild("Mask(Clone)").gameObject);
            sellHeros.Remove(go);
            go.transform.FindChild("BG").GetComponent<UISprite>().color = Color.white;
            totalSoul -= (long)(baseSoul + costSoul * hero.BaseTmpl[1].SellCoeff);
        }
        RefreshSelAndSoul();
    }

    /// <summary>
    /// Get the cost soul of special star and level.
    /// </summary>
    /// <param name="stars">The star of the hero.</param>
    /// <param name="level">The level of the hero.</param>
    /// <returns></returns>
    private long GetCostSoul(sbyte stars, short level)
    {
        long costSoul = 0;
        switch(stars)
        {
            case 1:
                for(int index = 1; index < level; index++)
                {
                    costSoul += hero.LvlUpTmpl[index].CostSoulStar1;
                }
                break;
            case 2:
                for(int index = 1; index < level; index++)
                {
                    costSoul += hero.LvlUpTmpl[index].CostSoulStar2;
                }
                break;
            case 3:
                for(int index = 1; index < level; index++)
                {
                    costSoul += hero.LvlUpTmpl[index].CostSoulStar3;
                }
                break;
            case 4:
                for(int index = 1; index < level; index++)
                {
                    costSoul += hero.LvlUpTmpl[index].CostSoulStar4;
                }
                break;
            case 5:
                for(int index = 1; index < level; index++)
                {
                    costSoul += hero.LvlUpTmpl[index].CostSoulStar5;
                }
                break;
        }
        return costSoul;
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
        for (int index = 0; index < sellHeros.Count; index++)
        {
            Destroy(sellHeros[index].transform.FindChild("Mask(Clone)").gameObject);
            sellHeros[index].transform.FindChild("BG").GetComponent<UISprite>().color = Color.white;
        }
        csHeroSell.SellList.Clear();
        sellHeros.Clear();
        totalSoul = 0;
        RefreshSelAndSoul();
    }

    private void CleanUp()
    {
        for (var index = 0; index < sellHeros.Count; index++)
        {
            Destroy(sellHeros[index].transform.FindChild("Mask(Clone)").gameObject);
            sellHeros[index].transform.FindChild("BG").GetComponent<UISprite>().color = Color.white;
        }
        csHeroSell.SellList.Clear();
        sellHeros.Clear();
        totalSoul = 0;
        RefreshSelAndSoul();

        for (var i = 0; i < canNotSells.Count; i++)
        {
            var heroObj = canNotSells[i].transform;
            heroObj.GetComponent<BoxCollider>().enabled = true;
            heroObj.FindChild("BG").GetComponent<UISprite>().color = Color.white;
            heroObj.FindChild("Hero").GetComponent<UISprite>().color = Color.white;
        }
        canNotSells.Clear();
    }

    private void CleanSellDialog()
    {
        var list = sellGrid.transform.Cast<Transform>().ToList();
        for (int index = list.Count - 1; index >= 0; index--)
        {
            Destroy(list[index].gameObject);
        }
        sellDialog.gameObject.SetActive(false);
    }


    #endregion

    #region Public Methods

    /// <summary>
    /// Update data when sell finished.
    /// </summary>
    public void SellOverUpdate()
    {
        CleanSellDialog();
        heroNums.text = string.Format("{0}/{1}", scHeroList.HeroList.Count, PlayerModelLocator.Instance.HeroMax);
        for (var index = 0; index < sellHeros.Count; index++)
        {
            var item = sellHeros[index];
            Destroy(sellHeros[index].transform.FindChild("Mask(Clone)").gameObject);
            sellHeros[index].transform.FindChild("BG").GetComponent<UISprite>().color = Color.white;
            item.transform.parent = PoolManager.Pools["Heros"].transform;
            PoolManager.Pools["Heros"].Despawn(item.transform);

        }
        csHeroSell.SellList.Clear();
        sellHeros.Clear();
        totalSoul = 0;
        RefreshSelAndSoul();
        herosGrid.repositionNow = true;
    }

    #endregion
}

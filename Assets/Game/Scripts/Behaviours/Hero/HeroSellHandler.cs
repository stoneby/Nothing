using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KXSGCodec;
using Template;
using Template.Auto.Hero;
using UnityEngine;

public class HeroSellHandler : MonoBehaviour 
{
    #region Private Fields

    private UIEventListener okLis;
    private UIEventListener cancelLis;
    private readonly List<GameObject> sellMasks = new List<GameObject>();
    private UILabel selCount;
    private UILabel sellCount;
    private UILabel soulCount;
    private const int MaxSellCount = 10;
    private GameObject sellMask;
    public Vector3 MaskOffset = new Vector3(0, 13, 0);
    private SCHeroList scHeroList;
    private readonly List<GameObject> sellHeros = new List<GameObject>();
    private readonly List<Transform> canNotSells = new List<Transform>();
    private readonly CSHeroSell csHeroSell = new CSHeroSell { SellList = new List<long>() };
    private List<long> teamMembers = new List<long>(); 
    private readonly Color disableColor = Color.gray;
    private const int MaxHeroCountCanSell = 10;
    private Hero hero;
    private long totalSoul;
    private UIHerosPageWindow cachedHerosWindow;

    #endregion

    #region Private Methods

    private void OnDisable()
    {
        UnInstallHandlers();
        CleanUp();
    }

    private void OnEnable()
    {
        sellCount.text = string.Format("{0}/{1}", csHeroSell.SellList.Count, MaxSellCount);
        InstallHandlers();
        GetTeamMembers();
        FreshSellStates();
    }

    private void Awake()
    {
        okLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Ok").gameObject);
        cancelLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Cancel").gameObject);
        sellCount = Utils.FindChild(transform, "SellNum").GetComponent<UILabel>();
        soulCount = Utils.FindChild(transform, "Soul-Value").GetComponent<UILabel>();
        selCount = Utils.FindChild(transform, "Selected-Value").GetComponent<UILabel>();
        sellMask = Utils.FindChild(transform, "SellMask").gameObject;
        sellMask.SetActive(false);
        hero = HeroModelLocator.Instance.HeroTemplates;
        scHeroList = HeroModelLocator.Instance.SCHeroList;
        cachedHerosWindow = WindowManager.Instance.GetWindow<UIHerosPageWindow>();
    }

    /// <summary>
    /// Install all handlers.
    /// </summary>
    private void InstallHandlers()
    {
        okLis.onClick = OnOkClicked;
        cancelLis.onClick = OnCancelClicked;
        cachedHerosWindow.OnSortOrderChanged += SortOrderChanged;
    }

    /// <summary>
    /// Uninstall all handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        okLis.onClick = null;
        cancelLis.onClick = null;
        cachedHerosWindow.OnSortOrderChanged -= SortOrderChanged;
    }

    /// <summary>
    /// The call back of the sort order type changed.
    /// </summary>
    /// <param name="go">The event sender.</param>
    private void SortOrderChanged(GameObject go)
    {
        CleanMasks();
        for (var i = 0; i < canNotSells.Count; i++)
        {
            var heroObj = canNotSells[i].transform;
            heroObj.GetComponent<BoxCollider>().enabled = true;
            heroObj.FindChild("BG").GetComponent<UISprite>().color = Color.white;
            heroObj.FindChild("Hero").GetComponent<UISprite>().color = Color.white;
        }
        FreshSellStates();
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
    /// The call back of the ok button clicked.
    /// </summary>
    /// <param name="go">The sender of the event.</param>
    private void OnOkClicked(GameObject go)
    {
        var sellDialog = WindowManager.Instance.Show<UISellDialogWindow>(true);
        sellDialog.InitDialog(csHeroSell.SellList);
    }

    /// <summary>
    /// The call back of the cancel button clicked.
    /// </summary>
    /// <param name="go">The sender of the event.</param>
    private void OnCancelClicked(GameObject go)
    {
        CleanMasks();
        csHeroSell.SellList.Clear();
        sellHeros.Clear();
        totalSoul = 0;
        RefreshSelAndSoul();
    }

    /// <summary>
    /// Set color back to normal and destory mask game objects.
    /// </summary>
    private void CleanMasks()
    {
        for (int i = 0; i < sellMasks.Count; i++)
        {
            sellMasks[i].transform.parent = null;
            sellHeros[i].transform.FindChild("BG").GetComponent<UISprite>().color = Color.white;
            Destroy(sellMasks[i]);
        }
        sellMasks.Clear();
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
        for (var index = 1; index < level; index++)
        {
            costSoul += hero.LvlUpTmpls[index].CostSoul[index - 1];
        }
        return costSoul;
    }

    /// <summary>
    /// Refresh some ui items when we needed.
    /// </summary>
    private void RefreshSelAndSoul()
    {
        selCount.text = csHeroSell.SellList.Count.ToString(CultureInfo.InvariantCulture);
        sellCount.text = string.Format("{0}/{1}", csHeroSell.SellList.Count, MaxSellCount);
        soulCount.text = totalSoul.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Get the the team members.
    /// </summary>
    private void GetTeamMembers()
    {
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

    #endregion

    #region Public Methods

    /// <summary>
    /// Init the ui data when we enter the window.
    /// </summary>
    public void FreshSellStates()
    {
        var heros = cachedHerosWindow.Heros.transform;
        for (int i = 0; i < heros.childCount; i++)
        {
            var heroTran = heros.GetChild(i);
            var uuid = heroTran.GetComponent<HeroItem>().Uuid;
            CheckState(heroTran, uuid);
        }
    }

    /// <summary>
    /// Do the clean up job.
    /// </summary>
    public void CleanUp()
    {
        CleanMasks();
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

    /// <summary>
    /// Refresh the ui.
    /// </summary>
    public void Refresh()
    {
        var herosWindow = WindowManager.Instance.GetWindow<UItemsWindow>();
        herosWindow.ItemClicked = HeroSellClicked;
    }

    /// <summary>
    /// The call back of the hero sell item clicked.
    /// </summary>
    /// <param name="go">The event sender.</param>
    public void HeroSellClicked(GameObject go)
    {
        var uUid = go.GetComponent<HeroItem>().Uuid;
        if (csHeroSell.SellList.Count >= MaxHeroCountCanSell && !csHeroSell.SellList.Contains(uUid))
        {
            return;
        }
        var heroInfo = HeroModelLocator.Instance.FindHero(uUid);
        var baseSoul = hero.HeroTmpls[heroInfo.TemplateId].Price;
        var level = heroInfo.Lvl;
        var stars = hero.HeroTmpls[heroInfo.TemplateId].Star;
        var costSoul = GetCostSoul(stars, level);
        if (!csHeroSell.SellList.Contains(uUid))
        {
            csHeroSell.SellList.Add(uUid);
            var maskToAdd = NGUITools.AddChild(go, sellMask);
            sellMasks.Add(maskToAdd);
            go.transform.FindChild("BG").GetComponent<UISprite>().color = Color.gray;
            maskToAdd.SetActive(true);
            sellHeros.Add(go);
            totalSoul += (long)(baseSoul + costSoul * hero.BaseTmpls[1].SellCoeff);
        }
        else
        {
            csHeroSell.SellList.Remove(uUid);
            var maskToAdd = go.transform.FindChild("SellMask(Clone)").gameObject;
            sellMasks.Remove(maskToAdd);
            Destroy(maskToAdd);
            sellHeros.Remove(go);
            go.transform.FindChild("BG").GetComponent<UISprite>().color = Color.white;
            totalSoul -= (long)(baseSoul + costSoul * hero.BaseTmpls[1].SellCoeff);
        }
        RefreshSelAndSoul();
    }

    #endregion
}

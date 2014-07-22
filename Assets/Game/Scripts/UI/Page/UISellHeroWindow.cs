using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KXSGCodec;
using Template;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UISellHeroWindow : Window
{
    private UIHeroCommonWindow herosWindow;
    private UIDragDropContainer dragDropContainer;
    private GameObject heroToSell;
    private GameObject heroToCancel;
    private readonly List<GameObject> sellMasks = new List<GameObject>();
    private UILabel selCount;
    private UILabel soulCount;
    private GameObject sellMask;
    public Vector3 MaskOffset = new Vector3(0, 13, 0);
    private SCHeroList scHeroList;
    private readonly List<Transform> canNotSells = new List<Transform>();
    private readonly CSHeroSell csHeroSell = new CSHeroSell {SellList = new List<long>()};
    private List<long> teamMembers = new List<long>();
    private const int MaxSellCount = 12;
    private UIEventListener sellLis;
    private Hero hero;
    private long totalSoul;
    private UIEventListener backLis;

    //The key is the game object copied on the right, the value is the original hero game object.
    private readonly Dictionary<GameObject, GameObject> sellDictionary = new Dictionary<GameObject, GameObject>(); 

    #region Window

    public override void OnEnter()
    {
        herosWindow = WindowManager.Instance.GetWindow<UIHeroCommonWindow>();
        herosWindow.NormalClicked = OnNormalClickForSell;
        selCount.text = string.Format("{0}/{1}", csHeroSell.SellList.Count, MaxSellCount);
        GetTeamMembers();
        FreshSellStates();
        InstallHandlers();
        HeroConstant.EnterType = HeroConstant.HeroDetailEnterType.SellHero;
        sellLis.GetComponent<UIButton>().isEnabled = false;
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        CleanUp();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        dragDropContainer = transform.Find("DragDropContainer").GetComponent<UIDragDropContainer>();
        selCount = Utils.FindChild(transform, "SelValue").GetComponent<UILabel>();
        soulCount = Utils.FindChild(transform, "SoulValue").GetComponent<UILabel>();
        scHeroList = HeroModelLocator.Instance.SCHeroList;
        hero = HeroModelLocator.Instance.HeroTemplates;
        backLis = UIEventListener.Get(transform.Find("Buttons/Button-Back").gameObject);
        sellLis = UIEventListener.Get(transform.Find("Buttons/Button-Sell").gameObject);
        sellMask = Utils.FindChild(transform, "SellMask").gameObject;
        sellMask.SetActive(false);
    }

    private void InstallHandlers()
    {
        sellLis.onClick = OnSell;
        backLis.onClick = OnBack;
    }

    private void UnInstallHandlers()
    {
        sellLis.onClick = null;
        backLis.onClick = null;
    }

    private void OnSell(GameObject go)
    {
        var sellDialog = WindowManager.Instance.Show<UISellDialogWindow>(true);
        sellDialog.InitDialog(csHeroSell.SellList);
    }

    /// <summary>
    ///  The callback of clicking back button.
    /// </summary>
    private void OnBack(GameObject go)
    {
        WindowManager.Instance.Show<UISellHeroWindow>(false);
        WindowManager.Instance.Show<UIHeroCommonWindow>(false);
    }

    private void OnNormalClickForSell(GameObject go)
    {
        heroToSell = go;
        heroToSell.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClickForCancel;
        UIHeroSnapShotWindow.CurUuid = go.GetComponent<HeroItemBase>().Uuid;
        var snapShot = WindowManager.Instance.Show<UIHeroSnapShotWindow>(true);
        snapShot.InitTemplate("HeroOrItemSnapShot.Sell", HeroSell);
    } 
    
    private void OnNormalClickForCancel(GameObject go)
    {
        heroToCancel = go;
        UIHeroSnapShotWindow.CurUuid = go.GetComponent<HeroItemBase>().Uuid;
        var snapShot = WindowManager.Instance.Show<UIHeroSnapShotWindow>(true);
        snapShot.InitTemplate("HeroOrItemSnapShot.CancelSell", CancelHeroSell);
    }

    private void HeroSell(GameObject go)
    {
        if (csHeroSell.SellList.Count == 0)
        {
            sellLis.GetComponent<UIButton>().isEnabled = true;
        }
        if(csHeroSell.SellList.Count >= MaxSellCount)
        {
            return;
        }
        var reparentTarget = dragDropContainer.reparentTarget;
        var child = NGUITools.AddChild(reparentTarget.gameObject, heroToSell);
        child.GetComponent<HeroItemBase>().Uuid = heroToSell.GetComponent<HeroItemBase>().Uuid;
        child.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClickForCancel;
        var grid = reparentTarget.GetComponent<UIGrid>();
        grid.repositionNow = true;
        WindowManager.Instance.Show<UIHeroSnapShotWindow>(false);
        RefreshSellList(heroToSell, child);
    }

    private void CancelHeroSell(GameObject go)
    {
        var uuidToCancel = heroToCancel.GetComponent<HeroItemBase>().Uuid;
        var heroClone = sellDictionary.First(item => item.Key.GetComponent<HeroItemBase>().Uuid == uuidToCancel).Key;
        var heros = herosWindow.Heros.transform;
        for (var i = 0; i < heros.childCount; i++)
        {
            var child = heros.GetChild(i);
            if (child.GetComponent<HeroItemBase>().Uuid == uuidToCancel)
            {
                child.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClickForSell;
                RefreshSellList(child.gameObject, heroClone);
            }
        }
        WindowManager.Instance.Show<UIHeroSnapShotWindow>(false);
        if (csHeroSell.SellList.Count == 0)
        {
            sellLis.GetComponent<UIButton>().isEnabled = false;
        }
    }

    private void RefreshSellList(GameObject go, GameObject copied)
    {
        var uuid = go.GetComponent<HeroItemBase>().Uuid;
        var heroInfo = HeroModelLocator.Instance.FindHero(uuid);
        var baseSoul = hero.HeroTmpl[heroInfo.TemplateId].Price;
        var level = heroInfo.Lvl;
        var stars = hero.HeroTmpl[heroInfo.TemplateId].Star;
        var costSoul = GetCostSoul(stars, level);
        if(!csHeroSell.SellList.Contains(uuid))
        {
            csHeroSell.SellList.Add(uuid);
            var maskToAdd = NGUITools.AddChild(go, sellMask);
            sellMasks.Add(maskToAdd);
            go.transform.FindChild("BG").GetComponent<UISprite>().color = Color.gray;
            maskToAdd.SetActive(true);
            totalSoul += (long)(baseSoul + costSoul * hero.BaseTmpl[1].SellCoeff);
            sellDictionary.Add(copied, go);
        }
        else
        {
            csHeroSell.SellList.Remove(uuid);
            var maskToAdd = go.transform.FindChild("SellMask(Clone)").gameObject;
            sellMasks.Remove(maskToAdd);
            Destroy(maskToAdd);
            go.transform.FindChild("BG").GetComponent<UISprite>().color = Color.white;
            totalSoul -= (long)(baseSoul + costSoul * hero.BaseTmpl[1].SellCoeff);
            sellDictionary.Remove(copied);
            NGUITools.Destroy(copied);
        }
        RefreshSelAndSoul();
    }

    /// <summary>
    /// Refresh some ui items when we needed.
    /// </summary>
    private void RefreshSelAndSoul()
    {
        selCount.text = csHeroSell.SellList.Count.ToString(CultureInfo.InvariantCulture);
        soulCount.text = totalSoul.ToString(CultureInfo.InvariantCulture);
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
        return costSoul;
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
            heroObj.FindChild("BG").GetComponent<UISprite>().color = Color.gray;
            heroObj.FindChild("Hero").GetComponent<UISprite>().color = Color.gray;
        }
        else
        {
            heroObj.GetComponent<BoxCollider>().enabled = true;
            heroObj.FindChild("BG").GetComponent<UISprite>().color = Color.white;
            heroObj.FindChild("Hero").GetComponent<UISprite>().color = Color.white;
        }
    }

    /// <summary>
    /// Set color back to normal and destory mask game objects.
    /// </summary>
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

    /// <summary>
    /// Init the ui data when we enter the window.
    /// </summary>
    public void FreshSellStates()
    {
        var heros = herosWindow.Heros.transform;
        for (int i = 0; i < heros.childCount; i++)
        {
            var heroTran = heros.GetChild(i);
            var uuid = heroTran.GetComponent<HeroItemBase>().Uuid;
            CheckState(heroTran, uuid);
        }
    }

    public void CleanUp()
    {
        CleanMasks();
        csHeroSell.SellList.Clear();
        foreach (var pair in sellDictionary)
        {
           NGUITools.Destroy(pair.Key);
        }
        sellDictionary.Clear();
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
}

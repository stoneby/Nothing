using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KXSGCodec;
using Property;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIHeroSellWindow : Window
{
    public GameObject HeroPrefab;
    public GameObject SellHeroPic;

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
    private List<long> teamMembers = new List<long>(); 
    private readonly CSHeroSell csHeroSell = new CSHeroSell{SellList = new List<long>()};
    private readonly List<GameObject> sellHeros = new List<GameObject>();
    private readonly Color disableColor = Color.gray;
    private long totalSoul;
    private Hero hero;

    private readonly List<string> sortContents = new List<string>
                                                     {
                                                         "��������",
                                                         "ְҵ����",
                                                         "ϡ�ж�����",
                                                         "��������",
                                                         "����������",
                                                         "HP����",
                                                         "�ظ�������",
                                                         "�ȼ�����",
                                                     };
    private UILabel sortLabel;

    #region Window

    public override void OnEnter()
    {
        sortLabel.text = sortContents[scHeroList.OrderType];
        InstallHandlers();
        FillHeroList();
        InitData();
        Refresh();
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

    private void Refresh()
    {
        selectCount.text = sellHeros.Count.ToString(CultureInfo.InvariantCulture);
        soulCount.text = totalSoul.ToString(CultureInfo.InvariantCulture);
    }

    private void InitData()
    {
        heroNums.text = string.Format("{0}/{1}", scHeroList.HeroList.Count, 100);
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
            CheckSellable(item, uUid);
            ShowHero(orderType, item, uUid);
        }
    }

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

    public void SellOverUpdate()
    {
        heroNums.text = string.Format("{0}/{1}", scHeroList.HeroList.Count, 100);
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

    private bool CanSell(long uUid)
    {
        return !teamMembers.Contains(uUid);
    }
    
    private void CheckSellable(Transform heroObj, long uUid)
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
            //����˳������
            case 0:
                ShowByLvl(sortRelated, heroInfo);
                break;

            //�佫ְҵ����
            case 1:
                ShowByJob(sortRelated, heroInfo, heroTemplate);
                break;

            //�佫ϡ�ж�����
            case 2:
                ShowByLvl(sortRelated, heroInfo);
                break;

            //�ն���˳������
            case 3:
                ShowByLvl(sortRelated, heroInfo);
                break;

            //����������
            case 4:
                ShowByJob(sortRelated, heroInfo, heroTemplate);
                break;

            //HP����
            case 5:
                ShowByHp(sortRelated, heroInfo);
                break;

            //�ظ�������
            case 6:
                ShowByRecover(sortRelated, heroInfo);
                break;

            //�ȼ�����
            case 7:
                ShowByLvl(sortRelated, heroInfo);
                break;
        }
    }

    private void FillHeroList()
    {
        var heroCount = scHeroList.HeroList.Count;
        for (int index = 0; index < heroCount; index++)
        {
            var item = NGUITools.AddChild(herosGrid.gameObject, HeroPrefab);
            item.name = item.name + index;
            UIEventListener.Get(item).onClick += OnHeroItemClicked;
        }
        herosGrid.Reposition();
    }

    private void InstallHandlers()
    {
        okLis.onClick += OnOkClicked;
        cancelLis.onClick += OnCancelClicked;
        backLis.onClick += OnBackClicked;
        sellOkLis.onClick += OnSellOkClicked;
        sellCancelLis.onClick += OnSellCancelClicked;
        sortBtnLis.onClick += OnSortClicked;
    }

    private void UnInstallHandlers()
    {
        okLis.onClick -= OnOkClicked;
        cancelLis.onClick -= OnCancelClicked;
        backLis.onClick -= OnBackClicked;
        sellOkLis.onClick -= OnSellOkClicked;
        sellCancelLis.onClick -= OnSellCancelClicked;
        sortBtnLis.onClick -= OnSortClicked;
    }

    private void OnSortClicked(GameObject go)
    {
        var orderType = HeroModelLocator.Instance.SCHeroList.OrderType;
        orderType = (sbyte)((orderType + 1) % sortContents.Count);
        scHeroList.OrderType = orderType;
        sortLabel.text = sortContents[scHeroList.OrderType];
        HeroModelLocator.Instance.SortHeroList(orderType, scHeroList.HeroList);
        for (int i = 0; i < scHeroList.HeroList.Count; i++)
        {
            var item = herosGrid.transform.GetChild(i);
            var uUid = scHeroList.HeroList[i].Uuid;
            item.GetComponent<HeroInfoPack>().Uuid = uUid;
            ShowHero(orderType, item, uUid);
        }
    }

    private void OnSellOkClicked(GameObject go)
    {
        NetManager.SendMessage(csHeroSell);
    }

    private void OnSellCancelClicked(GameObject go)
    {
        var list = sellGrid.transform.Cast<Transform>().ToList();
        for (int index = list.Count - 1; index >= 0; index--)
        {
            Destroy(list[index].gameObject);
        }
        sellDialog.gameObject.SetActive(false);
    }

    private void OnHeroItemClicked(GameObject go)
    {
        var uUid = go.GetComponent<HeroInfoPack>().Uuid;
        var heroInfo = HeroModelLocator.Instance.FindHero(uUid);
        var baseSoul = hero.HeroTmpl[heroInfo.TemplateId].Price;
        long costSoul = 0;
        var level = heroInfo.Lvl;
        for (int index = 1; index < level; index++)
        {
            costSoul += hero.LvlUpTmpl[index].CostSoul;
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

    private void OnOkClicked(GameObject go)
    {
        sellDialog.gameObject.SetActive(true);
        for (int index = 0; index < csHeroSell.SellList.Count; index++)
        {
            NGUITools.AddChild(sellGrid.gameObject, SellHeroPic);
        }
        sellGrid.Reposition();
    }

    private void OnCancelClicked(GameObject go)
    {
        csHeroSell.SellList.Clear();
        for (int index = 0; index < sellHeros.Count; index++)
        {
            Destroy(sellHeros[index].transform.FindChild("Mask(Clone)").gameObject);
        }
    }

    private void OnBackClicked(GameObject go)
    {
        WindowManager.Instance.Show(typeof(UIHeroItemsPageWindow), true);
    }

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
    private void ShowByHp(Transform sortRelated, HeroInfo heroInfo)
    {
        var hp = Utils.FindChild(sortRelated, "HP-Title");
        var hpValue = Utils.FindChild(sortRelated, "HP-Value").GetComponent<UILabel>();
        hpValue.text = heroInfo.Prop[RoleProperties.HERO_HP].ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelated.gameObject, false);
        NGUITools.SetActive(hp.gameObject, true);
        NGUITools.SetActive(hpValue.gameObject, true);
    }
    private void ShowByRecover(Transform sortRelated, HeroInfo heroInfo)
    {
        var recover = Utils.FindChild(sortRelated, "Recover-Title");
        var recoverValue = Utils.FindChild(sortRelated, "Recover-Value").GetComponent<UILabel>();
        recoverValue.text = heroInfo.Prop[RoleProperties.HERO_RECOVER].ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelated.gameObject, false);
        NGUITools.SetActive(recover.gameObject, true);
        NGUITools.SetActive(recoverValue.gameObject, true);
    }
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

using System.Collections.Generic;
using System.Globalization;
using KXSGCodec;
using Property;
using Template;
using Template.Auto.Hero;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIHeroDetailWindow : Window
{
    private HeroInfo heroInfo;
    private HeroTemplate heroTemplate;
    private UILabel attack;
    private UILabel hp;
    private UILabel recover;
    private UILabel mp;
    private UILabel activeSkillName;
    private UILabel activeSkillDesc; 
    private UILabel leaderSkillName;
    private UILabel leaderSkillDesc;
    private readonly List<UIEventListener> heroSelItemLis = new List<UIEventListener>();
    private sbyte curEquipIndex = -1;
    private Transform baseInfos;
    private UIEventListener backLis;
    private GameObject heroSelItemIns;

    public static bool IsLongPressEnter;

    public sbyte CurEquipIndex
    {
        get { return curEquipIndex; }
        private set { curEquipIndex = value; }
    }

    /// <summary>
    /// The uuid of current hero info.
    /// </summary>
    public static long CurUuid;

    /// <summary>
    /// The current hero info.
    /// </summary>
    public HeroInfo HeroInfo
    {
        get { return heroInfo; }
        private set
        {
            if (heroInfo != value)
            {
                heroInfo = value;
                heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[heroInfo.TemplateId];
                CurUuid = heroInfo.Uuid;
            }
        }
    }

    public GameObject HeroSelItemPrefab;

    #region Window

    public override void OnEnter()
    {
        MtaManager.TrackBeginPage(MtaType.HeroDetailWindow);
        InstallHandlers();
        if(heroSelItemIns != null)
        {
            NGUITools.Destroy(heroSelItemIns);
            heroSelItemIns = null;
        }
    }

    public override void OnExit()
    {
        MtaManager.TrackEndPage(MtaType.HeroDetailWindow);
        UnInstallHandlers();
        NGUITools.SetActiveChildren(baseInfos.gameObject, true);
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    void Awake()
    {
        backLis = UIEventListener.Get(transform.Find("Button-Back").gameObject);
        var property = Utils.FindChild(transform, "Property");
        attack = Utils.FindChild(property, "AttackValue").GetComponent<UILabel>();
        hp = Utils.FindChild(property, "HPValue").GetComponent<UILabel>();
        recover = Utils.FindChild(property, "RecoverValue").GetComponent<UILabel>();
        mp = Utils.FindChild(property, "MPValue").GetComponent<UILabel>();
        var activeSkill = Utils.FindChild(transform, "ActiveSkill");
        activeSkillName = activeSkill.Find("Name").GetComponent<UILabel>();
        activeSkillDesc = activeSkill.Find("Desc").GetComponent<UILabel>();
        var leaderSkill = Utils.FindChild(transform, "LeaderSkill");
        leaderSkillName = leaderSkill.Find("Name").GetComponent<UILabel>();
        leaderSkillDesc = leaderSkill.Find("Desc").GetComponent<UILabel>();
        baseInfos = transform.Find("BaseInfo");
        var equips = Utils.FindChild(transform, "Equips");
        for (var i = 0; i < equips.childCount; i++)
        {
            var child = equips.GetChild(i);
            heroSelItemLis.Add(UIEventListener.Get(child.gameObject));
        }
    }

    private void InstallHandlers()
    {
        for (var i = 0; i < heroSelItemLis.Count; i++)
        {
            var selItemLis = heroSelItemLis[i];
            selItemLis.onClick = OnHeroSelItem;
        }
        backLis.onClick = OnBack;
    }

    private void UnInstallHandlers()
    {
        for (var i = 0; i < heroSelItemLis.Count; i++)
        {
            var selItemLis = heroSelItemLis[i];
            selItemLis.onClick = null;
        }
        backLis.onClick = null;
    }

    private void OnBack(GameObject go)
    {
        if (!IsLongPressEnter)
        {
            WindowManager.Instance.Show<UIHeroSnapShotWindow>(true);
        }
        if(HeroConstant.EnterType == HeroConstant.HeroDetailEnterType.BuildingTeam)
        {
            WindowManager.Instance.Show<UIBuildingTeamWindow>(true);
        }
        if (HeroConstant.EnterType == HeroConstant.HeroDetailEnterType.LvlUp)
        {
            WindowManager.Instance.Show<UILevelUpHeroWindow>(true);
        }
        if (HeroConstant.EnterType == HeroConstant.HeroDetailEnterType.SellHero)
        {
            WindowManager.Instance.Show<UISellHeroWindow>(true);
        }
        WindowManager.Instance.Show<UIHeroDetailWindow>(false);
        WindowManager.Instance.Show<UIHeroCommonWindow>(true);
        IsLongPressEnter = false;

    }

    private void OnHeroSelItem(GameObject go)
    {
        var eventLis = go.GetComponent<UIEventListener>();
        CurEquipIndex = (sbyte) heroSelItemLis.IndexOf(eventLis);
        if (ItemModeLocator.Instance.ScAllItemInfos == null)
        {
            ItemModeLocator.Instance.GetItemPos = ItemType.GetItemInHeroInfo;
            var csmsg = new CSQueryAllItems { BagType = ItemType.MainItemBagType };
            NetManager.SendMessage(csmsg);
        }
        else
        {
            RefreshCanEquipItems();
        }
    }

    #endregion

    public void RefreshData(HeroInfo info)
    {
        heroInfo = info;
        HeroBaseInfoWindow.CurUuid = info.Uuid;
        heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[heroInfo.TemplateId];
        RefreshData();
    }

    public void EquipOver(HeroInfo info)
    {
        RefreshData(info);
        if (heroSelItemIns != null)
        {
            NGUITools.Destroy(heroSelItemIns);
            heroSelItemIns = null;
            NGUITools.SetActiveChildren(baseInfos.gameObject, true);
        }
    }

    /// <summary>
    /// Update ui data.
    /// </summary>
    private void RefreshData()
    {
        Utils.FindChild(baseInfos, "LV-Value").GetComponent<UILabel>().text = string.Format("{0}/{1}", heroInfo.Lvl, heroTemplate.LvlLimit);
        Utils.FindChild(baseInfos, "Limit-Value").GetComponent<UILabel>().text = string.Format("{0}/{1}", heroInfo.BreakTimes, heroTemplate.BreakLimit);
        Utils.FindChild(baseInfos, "Luck-Value").GetComponent<UILabel>().text = heroTemplate.Lucky.ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(baseInfos, "Name").GetComponent<UILabel>().text = heroTemplate.Name;
        var stars = Utils.FindChild(baseInfos, "Stars");
        for (int index = stars.childCount - 1; index >= stars.childCount - heroTemplate.Star; index--)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, true);
        }
        for (int index = 0; index < stars.childCount - heroTemplate.Star; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, false);
        }
        attack.text = heroInfo.Prop[RoleProperties.ROLE_ATK].ToString(CultureInfo.InvariantCulture);
        hp.text = heroInfo.Prop[RoleProperties.ROLE_HP].ToString(CultureInfo.InvariantCulture);
        recover.text = heroInfo.Prop[RoleProperties.ROLE_RECOVER].ToString(CultureInfo.InvariantCulture);
        mp.text = heroInfo.Prop[RoleProperties.ROLE_MP].ToString(CultureInfo.InvariantCulture);
        var skillTmp = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls;
        if (skillTmp.ContainsKey(heroTemplate.SpSkill))
        {
            var activeSkillTemp = skillTmp[heroTemplate.ActiveSkill];
            activeSkillName.text = activeSkillTemp.BaseTmpl.Name;
            activeSkillDesc.text = activeSkillTemp.BaseTmpl.Desc;
        }

        if (skillTmp.ContainsKey(heroTemplate.LeaderSkill))
        {
            var leaderSkillTemp = skillTmp[heroTemplate.LeaderSkill];
            leaderSkillName.text = leaderSkillTemp.BaseTmpl.Name;
            leaderSkillDesc.text = leaderSkillTemp.BaseTmpl.Desc;
        }
        InitEquipedItems();
    }

    private void InitEquipedItems()
    {
        var equips = heroInfo.EquipUuid;
        for (var i = 0; i < equips.Count; i++)
        {
            var itemIcon = heroSelItemLis[i].transform.GetChild(0).GetComponent<UISprite>();
            itemIcon.enabled = equips[i] != "";
        }
        for (var i = equips.Count; i < heroSelItemLis.Count; i++)
        {
            var itemIcon = heroSelItemLis[i].transform.GetChild(0).GetComponent<UISprite>();
            itemIcon.enabled = false;
        }
    }

    public void RefreshCanEquipItems()
    {
        NGUITools.SetActiveChildren(baseInfos.gameObject, false);
        heroSelItemIns = NGUITools.AddChild(gameObject, HeroSelItemPrefab);
        heroSelItemIns.GetComponent<HeroSelItem>().Refresh();
    } 
}

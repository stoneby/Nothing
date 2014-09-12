using System.Collections.Generic;
using System.Linq;
using Assets.Game.Scripts.Net.handler;
using KXSGCodec;
using Property;
using Template.Auto.Skill;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIBuildingTeamWindow : Window
{
    #region Private Fields

    private static sbyte curTeamIndex = -1;
    private List<long> curTeamCached; 
    private List<long> curTeam; 
    private SCHeroList scHeroList;
    private const int Column = 3;
    private GameObject toggleObjects;
    private GameObject tabTemplate;
    private bool isToClose;
    private int activeTab;
    private List<HeroInfo> infos;
    private UIGrid teamGrid;
    private UILabel leaderSkillName;
    private UILabel leaderSkillDesc;
    private const string TeamSpriteN = "Team{0}N";
    private const string TeamSpriteD = "Team{0}D";
    private readonly List<long> cachedSelUuids = new List<long>();
    private readonly List<GameObject> toggles = new List<GameObject>();
    private readonly List<List<long>> teams = new List<List<long>>();
    private readonly Dictionary<Position, GameObject> teamObjects = new Dictionary<Position, GameObject>();

    #endregion

    #region Public Fields

    public CloseButtonControl CloseButtonControl;
    public HeroSortControl HeroSortControl;
    public PropertyUpdater PropertyUpdater;
    public GameObject BaseHeroPrefab;
    public float IntervalOfTabs = 80f;
    public int CountOfOneGroup = 4;
    public CustomGrid Heros;
    public static sbyte CurTeamIndex
    {
        get { return curTeamIndex; }
    }

    #endregion

    #region Window

    public override void OnEnter()
    {
        MtaManager.TrackBeginPage(MtaType.HeroBuildingTeamWindow);
        isToClose = false;
        NGUITools.SetActive(Heros.gameObject, true);
        infos = scHeroList.HeroList;
        HeroSortControl.Init(infos);
        HeroUtils.InitWrapContents(Heros, infos, CountOfOneGroup, PlayerModelLocator.Instance.HeroMax);
        Init();
        InstallHandlers();
    }

    public override void OnExit()
    {
        MtaManager.TrackEndPage(MtaType.HeroBuildingTeamWindow);
        UnInstallHandlers();
        Clean();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        NGUITools.SetActive(PropertyUpdater.gameObject, true);
        scHeroList = HeroModelLocator.Instance.SCHeroList;
        toggleObjects = transform.Find("ToggleButtons").gameObject;
        tabTemplate = transform.Find("TabTemplate").gameObject;
        teamGrid = transform.Find("Item/Grid").GetComponent<UIGrid>();
        NGUITools.SetActive(tabTemplate, false);
        NGUITools.SetActive(Heros.transform.parent.gameObject, true);
        InstallHeroClicks();
        leaderSkillName = transform.Find("LeaderSkill/Name").GetComponent<UILabel>();
        leaderSkillDesc = transform.Find("LeaderSkill/Desc").GetComponent<UILabel>();
    }

    private void InstallHeroClicks()
    {
        var parent = Heros.transform;
        for (var i = 0; i < parent.childCount; i++)
        {
            var item = parent.GetChild(i);
            for (var j = 0; j < item.childCount; j++)
            {
                var hero = item.GetChild(j).gameObject;
                var activeCache = hero.activeSelf;
                NGUITools.SetActive(hero, true);
                var lis = UIEventListener.Get(hero);
                lis.onClick = OnHeroClicked;
                NGUITools.SetActive(hero, activeCache);
            }
        }
    }

    private void OnHeroClicked(GameObject go)
    {
        var pos = UISellHeroHandler.GetPosition(go);
        var isIntoTeam = !teamObjects.ContainsKey(pos);
        if ((isIntoTeam && teamObjects.Count >= HeroConstant.MaxHerosPerTeam))
        {
            return;
        }
        var index = HeroUtils.GetPosInTeam(curTeam);
        if (isIntoTeam)
        {
            var child = NGUITools.AddChild(teamGrid.gameObject, BaseHeroPrefab);
            SetPositionViaIndex(child, index);
            var heroBase = child.GetComponent<HeroItemBase>();
            var uuid = go.GetComponent<HeroItemBase>().Uuid;
            heroBase.Uuid = uuid;
            var heroInfo = HeroModelLocator.Instance.FindHero(heroBase.Uuid);
            heroBase.InitItem(heroInfo);
            UIEventListener.Get(child.gameObject).onClick = CancelHeroToTeam;
            teamObjects.Add(pos, child);
            curTeam[index] = uuid;
            //If it is main leader.
            if(index == 0)
            {
                UpdateSkills(heroInfo);
            }
        }
        else
        {
            CancelHeroToTeam(teamObjects[pos]);
        }
        UpdateProperty();
        var item = NGUITools.FindInParents<WrapHerosItem>(go);
        item.ShowSellMask(pos.Y, isIntoTeam);
    }

    private void SetPositionViaIndex(GameObject child, int index)
    {
        child.transform.localPosition = new Vector3(index % Column * teamGrid.cellWidth,
                                                    -index / Column * teamGrid.cellHeight, 0);
    }

    private void CancelHeroToTeam(GameObject go)
    {
        var pos = new Position();
        foreach(var teamObj in teamObjects)
        {
            if(teamObj.Value == go)
            {
                pos = teamObj.Key;
            }
        }
        var index = (int)(-Column * go.transform.localPosition.y) / (int)teamGrid.cellHeight +
                    (int)go.transform.localPosition.x / (int)teamGrid.cellWidth;
        RemoveSellObject(pos, index);
        RefreshCurScreen();
        //If it is main leader.
        if (index == 0)
        {
            ResetSkills();
        }
    }

    private void RemoveSellObject(Position pos, int index)
    {
        var clone = teamObjects[pos];
        NGUITools.Destroy(clone);
        teamObjects.Remove(pos);
        curTeam[index] = HeroConstant.NoneInitHeroUuid;
    }

    /// <summary>
    /// Install all handers for button click.
    /// </summary>
    private void InstallHandlers()
    {
        CloseButtonControl.OnCloseWindow = OnClose;
        HeroHandler.OnHeroModifyTeam += OnHeroModifyTeam;
        HeroSortControl.InstallHandlers();
        HeroSortControl.OnSortOrderChangedBefore += SortBefore;
        HeroSortControl.OnSortOrderChangedAfter += SortAfter;
        HeroSortControl.OnExcuteAfterSort += OnExcuteAfterSort;
        Heros.OnUpdate = OnUpdate;
    }

    /// <summary>
    /// UnInstall all handers for button click.
    /// </summary>
    private void UnInstallHandlers()
    {
        CloseButtonControl.OnCloseWindow = null;
        HeroSortControl.UnInstallHandlers();
        HeroSortControl.OnSortOrderChangedBefore -= SortBefore;
        HeroSortControl.OnSortOrderChangedAfter -= SortAfter;
        HeroSortControl.OnExcuteAfterSort -= OnExcuteAfterSort;
        HeroHandler.OnHeroModifyTeam -= OnHeroModifyTeam;
        Heros.OnUpdate = null;
    }

    private void SortBefore()
    {
        CacheUuidsFromPos(teamObjects.Keys, cachedSelUuids);
    }

    private void SortAfter()
    {
        RefreshAfterReCalculatePos(infos);
    }

    private void OnExcuteAfterSort()
    {
        HeroUtils.InitWrapContents(Heros, infos, CountOfOneGroup, PlayerModelLocator.Instance.HeroMax);
    }

    private void OnHeroModifyTeam(sbyte teamIndex, List<long> uuids)
    {
        if(isToClose)
        {
            WindowManager.Instance.Show<UIMainScreenWindow>(true);
        }
        else
        {
            teams[curTeamIndex] = new List<long>(curTeam);
            ChangeToTab(activeTab);
        }
    }

    private void OnClose()
    {
        if (!HeroUtils.IsValidTeam(curTeam))
        {
            ShowEditAssert();
            return;
        }
        isToClose = true;
        var isDirty = SendModifyTeamMsg();
        if (curTeamIndex != scHeroList.CurrentTeamIndex)
        {
            scHeroList.CurrentTeamIndex = curTeamIndex;
            var msg = new CSHeroChangeTeam { TeamIndex = curTeamIndex };
            NetManager.SendMessage(msg, false);
            TeamMemberManager.Instance.SetValue(curTeamIndex);
            PlayerModelLocator.UpdateTeamInfos(teams[curTeamIndex]);
        }
        if(!isDirty)
        {
            WindowManager.Instance.Show<UIMainScreenWindow>(true);
        }
    }

    private void OnUpdate(GameObject sender, int index)
    {
        var wrapItem = sender.GetComponent<WrapHerosItem>();
        var col = wrapItem.Children.Count;
        for (var i = 0; i < col; i++)
        {
            var pos = new Position { X = index, Y = i };
            var show = teamObjects.ContainsKey(pos);
            wrapItem.ShowSellMask(i, show);
        }
    }

    private bool SendModifyTeamMsg()
    {
        var isDirty = curTeam.Where((t, i) => t != curTeamCached[i]).Any();
        if(isDirty)
        {
            var msg = new CSHeroModifyTeam {TeamIndex = curTeamIndex, NewTeamList = curTeam};
            NetManager.SendMessage(msg);
        }
        return isDirty;
    }

    private void ShowEditAssert()
    {
        var assertWindow = WindowManager.Instance.GetWindow<AssertionWindow>();
        assertWindow.Title = LanguageManager.Instance.GetTextValue("BuildingTeam.AssertTitle");
        assertWindow.AssertType = AssertionWindow.Type.Ok;
        WindowManager.Instance.Show<AssertionWindow>(true);
    }


    /// <summary>
    /// Refresh the hero list.
    /// </summary>
    private void Init()
    {
        InitTeams();
        InitToggles();
        RefreshTeam();
    }

    private void InitToggles()
    {
        toggles.Clear();
        var toggleCount = scHeroList.TeamList.Count;
        for (var i = 0; i < toggleCount; i++)
        {
            var child = NGUITools.AddChild(toggleObjects, tabTemplate);
            NGUITools.SetActive(child, true);
            child.transform.GetChild(0).GetComponent<UISprite>().spriteName = string.Format(TeamSpriteN, i + 1);
            child.transform.localPosition = Vector3.down * i * IntervalOfTabs;
            var childLis = UIEventListener.Get(child);
            childLis.onClick = OnTabClicked;
            toggles.Add(child);
        }
        var toggle = toggles[curTeamIndex];
        HeroUtils.SetToggleSprite(toggle, "TableD", string.Format(TeamSpriteD, curTeamIndex + 1));
    }

    private void OnTabClicked(GameObject go)
    {
        var index = toggles.IndexOf(go);
        if (index == curTeamIndex)
        {
            return;
        }
        if (!HeroUtils.IsValidTeam(curTeam))
        {
            ShowEditAssert();
            return;
        }
        activeTab = index;
        var isDirty = SendModifyTeamMsg();
        if(!isDirty)
        {
            ChangeToTab(index);
        }
    }

    private void Clean()
    {
        CleanToggles();
        CleanObjectsInTeam();
    }

    private void CleanObjectsInTeam()
    {
        var clones = teamObjects.Values.ToList();
        for(var i = clones.Count - 1; i >= 0; i--)
        {
            NGUITools.Destroy(clones[i]);
        }
    }

    private void CleanToggles()
    {
        var count = toggleObjects.transform.childCount;
        for(var i = count - 1; i >= 0; i--)
        {
            var child = toggleObjects.transform.GetChild(i).gameObject;
            NGUITools.Destroy(child);
        }
    }

    private void CacheUuidsFromPos(IEnumerable<Position> list, List<long> uuids)
    {
        uuids.Clear();
        foreach (var pos in list)
        {
            var oneDimsionIndex = pos.X * CountOfOneGroup + pos.Y;
            cachedSelUuids.Add(infos[oneDimsionIndex].Uuid);
        }
    }

    private void RefreshAfterReCalculatePos(List<HeroInfo> hInfos)
    {
        var posList = UISellHeroHandler.GetPositionsViaUuids(cachedSelUuids, hInfos, CountOfOneGroup);
        var values = teamObjects.Values.ToList();
        teamObjects.Clear();
        for (var i = 0; i < posList.Count; i++)
        {
            teamObjects.Add(posList[i], values[i]);
        }
        RefreshCurScreen();
    }

    private void InitTeams()
    {
        curTeamIndex = scHeroList.CurrentTeamIndex;
        teams.Clear();
        var teamList = scHeroList.TeamList;
        var count = teamList.Count;
        for (var i = 0; i < count; i++)
        {
            teams.Add(new List<long>(teamList[i].ListHeroUuid));
            if (teamList[i].ListHeroUuid.Count < HeroConstant.MaxHerosPerTeam)
            {
                for (var j = teamList[i].ListHeroUuid.Count; j < HeroConstant.MaxHerosPerTeam; j++)
                {
                    teams[i].Add(HeroConstant.NoneInitHeroUuid);
                }
            }
        }
        curTeam = teams[curTeamIndex];
        curTeamCached = new List<long>(curTeam);
    }

    private void RefreshTeam()
    {
        teamObjects.Clear();
        HeroInfo leaderInfo;
        List<HeroInfo> heroInfos;
        CloneBaseHeros(out heroInfos, out leaderInfo);
        UpdateProperty(heroInfos);
        UpdateSkills(leaderInfo);
        HeroUtils.InitWrapContents(Heros, infos, CountOfOneGroup, PlayerModelLocator.Instance.HeroMax);
        RefreshCurScreen();
    }

    private void CloneBaseHeros(out List<HeroInfo> heroInfos, out HeroInfo leaderInfo)
    {
        leaderInfo = null;
        heroInfos = new List<HeroInfo>();
        var curIndex = 0;
        for(var i = 0; i < curTeam.Count; i++)
        {
            var uuid = curTeam[i];
            if(uuid != HeroConstant.NoneInitHeroUuid)
            {
                var child = NGUITools.AddChild(teamGrid.gameObject, BaseHeroPrefab);
                UIEventListener.Get(child.gameObject).onClick = CancelHeroToTeam;
                SetPositionViaIndex(child, curIndex);
                var baseHero = child.GetComponent<HeroItemBase>();
                var heroInfo = HeroModelLocator.Instance.FindHero(uuid);
                baseHero.InitItem(heroInfo);
                heroInfos.Add(heroInfo);
                var index = infos.IndexOf(heroInfo);
                teamObjects.Add(Utils.OneDimToTwo(index, CountOfOneGroup), child);
                if(i == 0)
                {
                    leaderInfo = heroInfo;
                }
            }
            curIndex++;
        }
    }

    private void UpdateProperty(IEnumerable<HeroInfo> heroInfos)
    {
        int atk, hp, recover, mp;
        HeroUtils.GetProperties(heroInfos, out atk, out hp, out recover, out mp);
        PropertyUpdater.UpdateProperty(-1, -1, atk, hp, recover, mp);
    }

    private void UpdateProperty()
    {
        var heroInfos = curTeam.Select(uid => HeroModelLocator.Instance.FindHero(uid)).Where(info => info != null).ToList();
        UpdateProperty(heroInfos);
    }

    private void UpdateSkills(HeroInfo leaderInfo)
    {
        var leaderTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[leaderInfo.TemplateId];
        var skillTempls = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls;
        HeroBattleSkillTemplate leaderSkill;
        skillTempls.TryGetValue(leaderTemplate.LeaderSkill, out leaderSkill);
        leaderSkillName.text = leaderSkill != null ? leaderSkill.Name : "-";
        leaderSkillDesc.text = leaderSkill != null ? leaderSkill.Desc : "-";
    }

    private void ResetSkills()
    {
        leaderSkillName.text =  "-";
        leaderSkillDesc.text =  "-";
    }

    private void RefreshCurScreen()
    {
        var childCount = Heros.transform.childCount;
        for(var i = 0; i < childCount; i++)
        {
            var child = Heros.transform.GetChild(i);
            var wrapHero = child.GetComponent<WrapHerosItem>();
            if(wrapHero != null && NGUITools.GetActive(child.gameObject))
            {
                for(var j = 0; j < wrapHero.Children.Count; j++)
                {
                    var pos = new Position {X = wrapHero.Row, Y = j};
                    var showSellMask = teamObjects.ContainsKey(pos);
                    wrapHero.ShowSellMask(j, showSellMask);
                }
            }
        }
    }

    #endregion

    #region Public Methods

    public void ChangeToTab(int tabIndex)
    {
        TeamMemberManager.Instance.SetValue((sbyte)tabIndex);
        var curToggle = toggles[curTeamIndex];
        var aimToggle = toggles[tabIndex];
        HeroUtils.SetToggleSprite(curToggle, "TabN", string.Format(TeamSpriteN, curTeamIndex + 1));
        HeroUtils.SetToggleSprite(aimToggle, "TableD", string.Format(TeamSpriteD, (tabIndex + 1)));
        curTeamIndex = (sbyte)tabIndex;
        curTeam = new List<long>(teams[curTeamIndex]);
        curTeamCached = new List<long>(curTeam);
        CleanObjectsInTeam();
        RefreshTeam();
    }

    #endregion
}

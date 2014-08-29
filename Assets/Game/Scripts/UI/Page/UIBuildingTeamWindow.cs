using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using Property;
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
    private const int NormalTeamMembers = 9;
    private const int Column = 3;
    private GameObject toggleObjects;
    private GameObject tabTemplate;
    private UIEventListener closeBtnLis;
    private UIEventListener sortBtnLis;
    private StretchItem closeBtnLine;
    private bool descendSort;
    private bool isEntered;
    private List<HeroInfo> infos;
    private UIGrid teamGrid;
    private UILabel sortLabel;
    private GameObject cachedCurToggle;
    private sbyte cachedCurTabIndex;
    private UILabel leaderSkillName;
    private UILabel leaderSkillDesc;
    private const string TeamSpriteN = "Team{0}N";
    private const string TeamSpriteD = "Team{0}D";
    private readonly List<long> cachedSelUuids = new List<long>();
    private readonly List<GameObject> toggles = new List<GameObject>();
    private readonly List<int> minHeroIndex = new List<int> { 1, 2, 3 };
    private readonly List<List<long>> teams = new List<List<long>>();
    private readonly Dictionary<Position, GameObject> teamObjects = new Dictionary<Position, GameObject>();

    #endregion

    #region Public Fields

    public PropertyUpdater PropertyUpdater;
    public UIToggle DescendToggle;
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
        isEntered = true;
        var orderType = HeroModelLocator.Instance.OrderType;
        sortLabel.text = LanguageManager.Instance.GetTextValue(ItemHelper.SortKeys[(int)orderType]);
        NGUITools.SetActive(Heros.gameObject, true);
        InitWrapContents(infos);
        Init();
        InstallHandlers();
    }

    public override void OnExit()
    {
        MtaManager.TrackEndPage(MtaType.HeroBuildingTeamWindow);
        UnInstallHandlers();
        if(curTeamIndex != scHeroList.CurrentTeamIndex)
        {
            scHeroList.CurrentTeamIndex = curTeamIndex;
            var msg = new CSHeroChangeTeam { TeamIndex = curTeamIndex };
            NetManager.SendMessage(msg, false);
            TeamMemberManager.Instance.SetValue(curTeamIndex);
        }
        Clean();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        NGUITools.SetActive(PropertyUpdater.gameObject, true);
        closeBtnLis = UIEventListener.Get(transform.Find("Buttons/Button-Close").gameObject);
        closeBtnLine = transform.Find("Buttons/Button-CloseLine").GetComponent<StretchItem>();
        sortBtnLis = UIEventListener.Get(transform.Find("Buttons/Button-Sort").gameObject);
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();
        scHeroList = HeroModelLocator.Instance.SCHeroList;
        infos = scHeroList.HeroList ?? new List<HeroInfo>();
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
        if ((isIntoTeam && teamObjects.Count >= NormalTeamMembers))
        {
            return;
        }
        var index = GetPosInTeam();
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
        }
        else
        {
            CancelHeroToTeam(teamObjects[pos]);
        }
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
        EventDelegate.Add(DescendToggle.onChange, SortTypeChanged);
        closeBtnLis.onClick = OnClose;
        closeBtnLine.DragFinish = OnClose;
        sortBtnLis.onClick = OnSort;
        Heros.OnUpdate = OnUpdate;
    }

    /// <summary>
    /// UnInstall all handers for button click.
    /// </summary>
    private void UnInstallHandlers()
    {
        EventDelegate.Remove(DescendToggle.onChange, SortTypeChanged);
        closeBtnLis.onClick = null;
        closeBtnLine.DragFinish = null;
        sortBtnLis.onClick = null;
        Heros.OnUpdate = null;
    }

    private void OnSort(GameObject go)
    {
        CacheUuidsFromPos(teamObjects.Keys, cachedSelUuids);
        var orderType = HeroModelLocator.Instance.OrderType;
        orderType = (ItemHelper.OrderType)(((int)orderType + 1) % ItemHelper.SortKeys.Count);
        sortLabel.text = LanguageManager.Instance.GetTextValue(ItemHelper.SortKeys[(int)orderType]);
        HeroModelLocator.Instance.OrderType = orderType;
        InitWrapContents(infos);
        RefreshAfterReCalculatePos(infos);
    }

    private void OnClose(GameObject sender)
    {
        if (!IsValidTeam())
        {
            ShowEditAssert();
            return;
        }
        SendModifyTeamMsg();
        WindowManager.Instance.Show<UIBuildingTeamWindow>(false);
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

    private void SortTypeChanged()
    {
        descendSort = DescendToggle.value;
        if (isEntered)
        {
            CacheUuidsFromPos(teamObjects.Keys, cachedSelUuids);
            InitWrapContents(infos);
            RefreshAfterReCalculatePos(infos);
        }
    }

    private bool SendModifyTeamMsg()
    {
        var isDirty = false;
        for(var i = 0; i < curTeam.Count; i++)
        {
            if(curTeam[i] != curTeamCached[i])
            {
                isDirty = true;
                break;
            }
        }
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

    private int GetPosInTeam()
    {
        for(var i = 0; i < NormalTeamMembers; i++)
        {
            if(curTeam[i] == HeroConstant.NoneInitHeroUuid)
            {
                return i;
            }
        }
        return -1;
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
        toggle.GetComponent<UISprite>().spriteName = "TableD";
        toggle.transform.GetChild(0).GetComponent<UISprite>().spriteName = string.Format(TeamSpriteD, curTeamIndex + 1);
    }

    private void OnTabClicked(GameObject go)
    {
        var index = toggles.IndexOf(go);
        if (index == curTeamIndex)
        {
            return;
        }
        if (!IsValidTeam())
        {
            ShowEditAssert();
            return;
        }
        cachedCurToggle = go;
        cachedCurTabIndex = (sbyte) index;
        var isDirty = SendModifyTeamMsg();
        if(!isDirty)
        {
            ChangeToTab(false);
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
            var clone = clones[i];
            NGUITools.Destroy(clone);
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

    /// <summary>
    /// Check if the edited team is valid(if it contains at least one leader and two assistents).
    /// </summary>
    /// <returns></returns>
    private bool IsValidTeam()
    {
        var team = teams[curTeamIndex];
        for(int i = 0; i < minHeroIndex.Count; i++)
        {
            if (team[i] == HeroConstant.NoneInitHeroUuid)
            {
                return false;
            }
        }
        return true;
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
            if (teamList[i].ListHeroUuid.Count < NormalTeamMembers)
            {
                for (var j = teamList[i].ListHeroUuid.Count; j < NormalTeamMembers; j++)
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
        var curIndex = 0;
        HeroInfo leaderInfo = null;
        for (var i = 0; i < curTeam.Count; i++)
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
                var index = infos.IndexOf(heroInfo);
                teamObjects.Add(new Position{X = index / CountOfOneGroup, Y = index % CountOfOneGroup}, child);
                if(i == 0)
                {
                    leaderInfo = heroInfo;
                }
            }
            curIndex++;
        }
        InitWrapContents(infos);
        PropertyUpdater.UpdateProperty(-1, -1, leaderInfo.Prop[RoleProperties.ROLE_ATK],
                                       leaderInfo.Prop[RoleProperties.ROLE_HP],
                                       leaderInfo.Prop[RoleProperties.ROLE_RECOVER],
                                       leaderInfo.Prop[RoleProperties.ROLE_MP]);
        var leaderTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[leaderInfo.TemplateId];
        var leaderSkill = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls[leaderTemplate.LeaderSkill];
        leaderSkillName.text = leaderSkill.Name;
        leaderSkillDesc.text = leaderSkill.Desc;
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

    private void InitWrapContents(List<HeroInfo> heroInfos)
    {
        var orderType = HeroModelLocator.Instance.OrderType;
        HeroModelLocator.Instance.SortHeroList(orderType, infos, descendSort);
        var data = new List<List<long>>();
        var rows = Mathf.CeilToInt((float)heroInfos.Count / CountOfOneGroup);
        for (var i = 0; i < rows; i++)
        {
            var list = new List<long>();
            for (var j = 0; j < CountOfOneGroup; j++)
            {
                if (i * CountOfOneGroup + j < infos.Count)
                {
                    list.Add(infos[i * CountOfOneGroup + j].Uuid);
                }
            }
            data.Add(list);
        }
        Heros.Init(data);
    }

    #endregion

    #region Public Methods

    public void ChangeToTab(bool isFromServer = true)
    {
        if (isFromServer)
        {
            teams[curTeamIndex] = new List<long>(curTeam);
        }
        var curToggle = toggles[curTeamIndex];
        curToggle.GetComponent<UISprite>().spriteName = "TabN";
        curToggle.transform.GetChild(0).GetComponent<UISprite>().spriteName = string.Format(TeamSpriteN, curTeamIndex + 1);
        cachedCurToggle.GetComponent<UISprite>().spriteName = "TableD";
        cachedCurToggle.transform.GetChild(0).GetComponent<UISprite>().spriteName = string.Format(TeamSpriteD, cachedCurTabIndex + 1);
        curTeamIndex = cachedCurTabIndex;
        curTeam = new List<long>(teams[curTeamIndex]);
        curTeamCached = new List<long>(curTeam);
        CleanObjectsInTeam();
        RefreshTeam();
    }

    #endregion
}

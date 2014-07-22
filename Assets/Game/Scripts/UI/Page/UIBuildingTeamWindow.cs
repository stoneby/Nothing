using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KXSGCodec;
using Property;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIBuildingTeamWindow : Window
{
    private EndlessSwipeEffect endlessSwipeEffect;
    private UIHeroCommonWindow herosWindow;
    private sbyte curTeamIndex = -1;
    private int teamCount;

    private UILabel curTeamValue;
    private UIEventListener leftArraw;
    private UIEventListener rightArraw; 
    private readonly List<Vector3> endlessItemPostions = new List<Vector3>();

    private GameObject heroToUp;
    private GameObject heroToDown;
    private MyDragDropSurface dragDropSurface;

    private readonly List<int> minHeroIndex = new List<int> { 1, 2, 3 };
    private readonly List<List<long>> teams = new List<List<long>>();
    private List<long> curTeamCached; 
    private List<long> curTeam; 
    private SCHeroList scHeroList;
    private GameObject mask;
    private readonly List<GameObject> masks = new List<GameObject>();
    private UIEventListener backLis;
    private const int NormalTeamMembers = 9;
    private readonly List<GameObject> herosInTeam = new List<GameObject>();
    private const int InvalidPos = -1;

    /// <summary>
    /// The index of current team. Whenever the value changed we will update the state of flip buttons and team icon.
    /// </summary>
    public sbyte CurTeamIndex
    {
        get { return curTeamIndex; }
        private set
        {
            if (curTeamIndex != value)
            {
                if (value < 0 || value > teamCount - 1)
                {
                    return;
                }
                if (curTeamIndex == 0)
                {
                    rightArraw.GetComponent<UIButton>().isEnabled = true;
                }
                if (curTeamIndex == teamCount - 1)
                {
                    leftArraw.GetComponent<UIButton>().isEnabled = true;
                }
                if (value == 0)
                {
                    rightArraw.GetComponent<UIButton>().isEnabled = false;
                }
                if (value == teamCount - 1)
                {
                    leftArraw.GetComponent<UIButton>().isEnabled = false;
                }
                curTeamIndex = value;
                curTeamValue.text = value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }

    #region Window

    public override void OnEnter()
    {
        Init();
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        heroToUp = null;
        CleanMask();
        CleanHeros();
        if(CurTeamIndex != scHeroList.CurrentTeamIndex)
        {
            scHeroList.CurrentTeamIndex = CurTeamIndex;
            var msg = new CSHeroChangeTeam {TeamIndex = CurTeamIndex};
            NetManager.SendMessage(msg);
        }
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        leftArraw = UIEventListener.Get(transform.Find("Buttons/Button-ArrowL").gameObject);
        rightArraw = UIEventListener.Get(transform.Find("Buttons/Button-ArrowR").gameObject);
        backLis = UIEventListener.Get(transform.Find("Buttons/Button-Back").gameObject);
        teamCount = HeroModelLocator.Instance.SCHeroList.TeamList.Count;
        curTeamValue = transform.Find("CurTeam/TeamValue").GetComponent<UILabel>();
        dragDropSurface = transform.Find("DragDropSurface").GetComponent<MyDragDropSurface>();
        endlessSwipeEffect = GetComponentInChildren<EndlessSwipeEffect>();
        endlessSwipeEffect.UpdateData += UpdateData;
        CacheEndlessItemsPos();
        scHeroList = HeroModelLocator.Instance.SCHeroList;
        mask = Utils.FindChild(transform, "Mask").gameObject;
        mask.SetActive(false);
    }

    /// <summary>
    /// Cache the local positions of endless swipe and its children.
    /// </summary>
    private void CacheEndlessItemsPos()
    {
        var endlessTran = endlessSwipeEffect.transform;
        endlessItemPostions.Add(endlessTran.localPosition);
        for (int i = 0; i < endlessTran.childCount; i++)
        {
            endlessItemPostions.Add(endlessSwipeEffect.transform.GetChild(i).localPosition);
        }
    }

    /// <summary>
    /// Reset the local positions of endless swipe and its children.
    /// </summary>
    private void ResetEndlessItemsPos()
    {
        //The first cached for endless swipe effect's transform local position, and others for its children.
        endlessSwipeEffect.transform.localPosition = endlessItemPostions[0];
        for (int i = 0; i < endlessSwipeEffect.transform.childCount; i++)
        {
            endlessSwipeEffect.transform.GetChild(i).localPosition = endlessItemPostions[i + 1];
        }
    }

    private void UpdateData()
    {
        CleanMask();
        CleanHeros();
        dragDropSurface.ReparentTarget = endlessSwipeEffect.CurrentItem.Find("Grid");
        CurTeamIndex = (sbyte)endlessSwipeEffect.CurCustomIndex;
        curTeam = teams[CurTeamIndex];
        curTeamCached = new List<long>(curTeam);
        herosWindow.NormalClicked = OnNormalClickedForUp;
        herosWindow.Refresh(CurTeamIndex);
        RefreshTeam(curTeam);
    }

    /// <summary>
    /// Install all handers for button click.
    /// </summary>
    private void InstallHandlers()
    {
        leftArraw.onClick = OnLeftArrowClicked;
        rightArraw.onClick = OnRightArrowClicked;
        backLis.onClick = OnBack;
        herosWindow.OnSortOrderChanged += OnSortOrderChanged;
    }

    /// <summary>
    /// UnInstall all handers for button click.
    /// </summary>
    private void UnInstallHandlers()
    {
        leftArraw.onClick = null;
        rightArraw.onClick = null;
        backLis.onClick = null;
        herosWindow.OnSortOrderChanged -= OnSortOrderChanged;
    }

    private void OnSortOrderChanged(GameObject go)
    {
        
    }

    private void OnBack(GameObject go)
    {
        if (!IsValidTeam())
        {
            ShowEditAssert();
            return;
        }
        SendModifyTeamMsg();
        WindowManager.Instance.Show<UIBuildingTeamWindow>(false);
        WindowManager.Instance.Show<UIHeroCommonWindow>(false);
    }

    private void OnLeftArrowClicked(GameObject go)
    {
        if (!IsValidTeam())
        {
            ShowEditAssert();
            return;
        }
        SendModifyTeamMsg();
        endlessSwipeEffect.ExcueSwipe(true);
    }

    private void OnRightArrowClicked(GameObject go)
    {
        if (!IsValidTeam())
        {
            ShowEditAssert();
            return;
        }
        SendModifyTeamMsg();
        endlessSwipeEffect.ExcueSwipe(false);
    }

    private void SendModifyTeamMsg()
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
            var msg = new CSHeroModifyTeam {TeamIndex = CurTeamIndex, NewTeamList = curTeam};
            NetManager.SendMessage(msg);
        }
    }

    private void ShowEditAssert()
    {
        var assertWindow = WindowManager.Instance.GetWindow<AssertionWindow>();
        assertWindow.Title = LanguageManager.Instance.GetTextValue("BuildingTeam.AssertTitle");
        assertWindow.AssertType = AssertionWindow.Type.Ok;
        WindowManager.Instance.Show<AssertionWindow>(true);
    }

    private void OnNormalClickedForUp(GameObject go)
    {
        heroToUp = go;
        UIHeroSnapShotWindow.CurUuid = go.GetComponent<HeroItemBase>().Uuid;
        var snapShot = WindowManager.Instance.Show<UIHeroSnapShotWindow>(true);
        snapShot.InitTemplate("HeroSnapShot.Go", GetUp);
    }

    private void OnNormalClickedForDown(GameObject go)
    {
        heroToDown = go;
        UIHeroSnapShotWindow.CurUuid = go.GetComponent<HeroItemBase>().Uuid;
        var snapShot = WindowManager.Instance.Show<UIHeroSnapShotWindow>(true);
        snapShot.InitTemplate("HeroSnapShot.Down", GetDown);
    }

    private void GetDown(GameObject go)
    {
        var downUuid = heroToDown.GetComponent<HeroItemBase>().Uuid;
        var child = herosInTeam.Find(item => item.GetComponent<HeroItemBase>().Uuid == downUuid);
        herosInTeam.Remove(child);
        NGUITools.Destroy(child);
        var index = curTeam.IndexOf(downUuid);
        curTeam[index] = HeroConstant.NoneInitHeroUuid;
        var heros = herosWindow.Heros.transform;
        for (var i = 0; i < heros.childCount; i++)
        {
            var hero = heros.GetChild(i);
            if(hero.GetComponent<HeroItemBase>().Uuid == downUuid)
            {
                hero.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClickedForUp;
                var maskClone = hero.Find("Mask(Clone)").gameObject;
                masks.Remove(maskClone);
                NGUITools.Destroy(maskClone);
            }
        }
        WindowManager.Instance.Show<UIHeroSnapShotWindow>(false);
    }

    private void GetUp(GameObject go)
    {
        if(herosInTeam.Count >= NormalTeamMembers)
        {
            return;
        }
        heroToUp.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClickedForDown;
        var pos = GetPosInTeam();
        GetIntoTeam(heroToUp, pos);
        WindowManager.Instance.Show<UIHeroSnapShotWindow>(false);
    }

    private void GetIntoTeam(GameObject hero, int pos)
    {
        if (pos == InvalidPos)
        {
            Logger.LogError("The position to get into team is not valid!");
            return;
        }
        var reparentTarget = dragDropSurface.ReparentTarget;
        var child = NGUITools.AddChild(reparentTarget.gameObject, hero);
        child.transform.localPosition = new Vector3(dragDropSurface.CellWidth * (pos % dragDropSurface.Column),
                                                    -dragDropSurface.CellHeight * (pos / dragDropSurface.Column), 0f);
        child.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClickedForDown;
        AddMask(hero.transform, pos);
        herosInTeam.Add(child);
        curTeam[pos] = hero.GetComponent<HeroItemBase>().Uuid;
        child.GetComponent<HeroItemBase>().Uuid = curTeam[pos];
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
        herosWindow = WindowManager.Instance.GetWindow<UIHeroCommonWindow>();
        herosWindow.NormalClicked = OnNormalClickedForUp;
        InitTeams();
        ResetEndlessItemsPos();
        endlessSwipeEffect.InitCustomData(CurTeamIndex, scHeroList.TeamList.Count);
        dragDropSurface.ReparentTarget = endlessSwipeEffect.CurrentItem.Find("Grid");
        HeroConstant.EnterType = HeroConstant.HeroDetailEnterType.BuildingTeam;
        RefreshTeam(curTeam);
    }

    private void RefreshTeam(List<long> curTeamUuids)
    {
        var herosTran = herosWindow.Heros.transform;
        for(var heroIndex = 0; heroIndex < herosTran.childCount; heroIndex++)
        {
            var item = herosTran.GetChild(heroIndex);
            var uUid = item.GetComponent<HeroItemBase>().Uuid;
            if(curTeamUuids.Contains(uUid))
            {
                item.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClickedForDown;
                var pos = curTeamUuids.IndexOf(uUid);
                GetIntoTeam(item.gameObject, pos);
            }
        }
    }

    /// <summary>
    /// Add the mask to the hero item game object.
    /// </summary>
    /// <param name="item">The hero game object.</param>
    /// <param name="numIndex">The number sprite on the mask, the number is base on zero.</param>
    private void AddMask(Transform item, int numIndex)
    {
        var maskToShow = NGUITools.AddChild(item.gameObject, mask);
        masks.Add(maskToShow);
        maskToShow.SetActive(true);
        maskToShow.transform.GetChild(0).GetComponent<UISprite>().spriteName =
            (numIndex + 1).ToString(CultureInfo.InvariantCulture);
    }

    private void CleanMask()
    {
        for (var i = masks.Count - 1; i >= 0; i--)
        {
            NGUITools.Destroy(masks[i]);
        }
        masks.Clear();
    }

    private void CleanHeros()
    {
        for (var i = 0; i < herosInTeam.Count; i++)
        {
            NGUITools.Destroy(herosInTeam[i]); 
        }
        herosInTeam.Clear();
    }

    /// <summary>
    /// Check if the edited team is valid(if it contains at least one leader and two assistents).
    /// </summary>
    /// <returns></returns>
    private bool IsValidTeam()
    {
        var team = teams[CurTeamIndex];
        for(int i = 0; i < minHeroIndex.Count; i++)
        {
            if (team[i] == HeroConstant.NoneInitHeroUuid)
            {
                return false;
            }
        }
        return true;
    }

    private void InitTeams()
    {
        CurTeamIndex = scHeroList.CurrentTeamIndex;
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
        curTeam = teams[CurTeamIndex];
        curTeamCached = new List<long>(curTeam);
    }

    #endregion
}

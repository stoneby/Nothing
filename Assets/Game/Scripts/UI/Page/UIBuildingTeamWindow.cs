using System.Collections.Generic;
using System.Globalization;
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
    private int curTeamIndex = -1;
    private int teamCount;

    private UILabel curTeamValue;
    private UIEventListener leftArraw;
    private UIEventListener rightArraw; 
    private readonly List<Vector3> endlessItemPostions = new List<Vector3>();

    private GameObject heroToGetBattle;
    private UIDragDropContainer dragDropContainer;

    private readonly List<int> minHeroIndex = new List<int> { 1, 2, 3 };
    /// <summary>
    /// The key is the uuid of the hero, the value is the position in the team.
    /// </summary>
    private readonly Dictionary<long, int> selectedItems = new Dictionary<long, int>();
    private SCHeroList scHeroList;
    private GameObject mask;
    private const int LeaderCount = 3;
    private List<GameObject> masks = new List<GameObject>(); 


    /// <summary>
    /// The index of current team. Whenever the value changed we will update the state of flip buttons and team icon.
    /// </summary>
    public int CurTeamIndex
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
                HeroModelLocator.Instance.SCHeroList.CurrentTeamIndex = (sbyte)value;
                curTeamValue.text = value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }

    #region Window

    public override void OnEnter()
    {
        herosWindow = WindowManager.Instance.GetWindow<UIHeroCommonWindow>();
        herosWindow.NormalClicked = OnNormalClicked;
        ResetEndlessItemsPos();
        CurTeamIndex = HeroModelLocator.Instance.SCHeroList.CurrentTeamIndex;
        endlessSwipeEffect.InitCustomData(CurTeamIndex, HeroModelLocator.Instance.SCHeroList.TeamList.Count);
        InstallHandlers();
        Init();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        heroToGetBattle = null;
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        leftArraw = UIEventListener.Get(transform.Find("Buttons/ArrowL").gameObject);
        rightArraw = UIEventListener.Get(transform.Find("Buttons/ArrowR").gameObject);
        teamCount = HeroModelLocator.Instance.SCHeroList.TeamList.Count;
        curTeamValue = transform.Find("CurTeam/TeamValue").GetComponent<UILabel>();
        dragDropContainer = transform.Find("DragDropContainer").GetComponent<UIDragDropContainer>();
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
        dragDropContainer.reparentTarget = endlessSwipeEffect.CurrentItem.Find("Grid");
        CurTeamIndex = endlessSwipeEffect.CurCustomIndex;
        var heroUuids = HeroModelLocator.Instance.SCHeroList.TeamList[CurTeamIndex].ListHeroUuid;
        RefreshTeam(heroUuids);
    }


    /// <summary>
    /// Install all handers for button click.
    /// </summary>
    private void InstallHandlers()
    {
        leftArraw.onClick = OnLeftArrowClicked;
        rightArraw.onClick = OnRightArrowClicked;
    }

    /// <summary>
    /// UnInstall all handers for button click.
    /// </summary>
    private void UnInstallHandlers()
    {
        leftArraw.onClick = null;
        rightArraw.onClick = null;
    }

    private void OnLeftArrowClicked(GameObject go)
    {
        endlessSwipeEffect.ExcueSwipe(true);
    }

    private void OnRightArrowClicked(GameObject go)
    {
        endlessSwipeEffect.ExcueSwipe(false);
    }

    private void OnNormalClicked(GameObject go)
    {
        heroToGetBattle = go;
        UIHeroSnapShotWindow.CurUuid = go.GetComponent<HeroItemBase>().Uuid;
        var snapShot = WindowManager.Instance.Show<UIHeroSnapShotWindow>(true);
        snapShot.InitTemplate("HeroSnapShot.Go", GetIntoBattle);
    }

    private void GetIntoBattle(GameObject go)
    {
        GetIntoTeam(heroToGetBattle);
        WindowManager.Instance.Show<UIHeroSnapShotWindow>(false);
    }

    private void GetIntoTeam(GameObject hero)
    {
        var reparentTarget = dragDropContainer.reparentTarget;
        NGUITools.AddChild(reparentTarget.gameObject, hero);
        var grid = reparentTarget.GetComponent<UIGrid>();
        grid.repositionNow = true;
    }

    /// <summary>
    /// Refresh the hero list.
    /// </summary>
    private void Init()
    {
        selectedItems.Clear();
        var index = scHeroList.CurrentTeamIndex;
        var curTeamUuids = scHeroList.TeamList[index].ListHeroUuid;
        RefreshTeam(curTeamUuids);
    }

    private void RefreshTeam(List<long> curTeamUuids)
    {
        CleanMask();
        var herosTran = herosWindow.Heros.transform;
        for(var heroIndex = 0; heroIndex < herosTran.childCount; heroIndex++)
        {
            var item = herosTran.GetChild(heroIndex);
            var uUid = item.GetComponent<HeroItemBase>().Uuid;
            if(curTeamUuids.Contains(uUid))
            {
                GetIntoTeam(item.gameObject);
                var numIndex = curTeamUuids.IndexOf(uUid) + 1;
                var maskToShow = NGUITools.AddChild(item.gameObject, mask);
                masks.Add(maskToShow);
                maskToShow.SetActive(true);
                maskToShow.transform.GetChild(0).GetComponent<UISprite>().spriteName =
                    numIndex.ToString(CultureInfo.InvariantCulture);
            }
        }
    }

    private void CleanMask()
    {
        for (var i = masks.Count - 1; i >= 0; i--)
        {
            NGUITools.Destroy(masks[i]);
        }
        masks.Clear();
    }

    #endregion
}

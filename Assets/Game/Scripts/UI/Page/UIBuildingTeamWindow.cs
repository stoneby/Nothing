using System.Collections.Generic;
using System.Globalization;
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
        CurTeamIndex = endlessSwipeEffect.CurCustomIndex;
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
        var reparentTarget = dragDropContainer.reparentTarget;
        NGUITools.AddChild(reparentTarget.gameObject, heroToGetBattle);
        var grid = reparentTarget.GetComponent<UIGrid>();
        grid.repositionNow = true;
        WindowManager.Instance.Show<UIHeroSnapShotWindow>(false);
    }

    #endregion
}

using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using UnityEngine;
using OrderType = ItemHelper.OrderType;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIHeroBindWindow : Window
{
    #region Public Fields

    public GameObject HeroPrefab;
    public static List<long> ChangedLockList { get; private set; }

    #endregion

    #region Private Fields

    private readonly List<long> lockList = new List<long>();
    private readonly List<long> lastLockList = new List<long>();
    private UILabel bindCount;
    private int cachedLockCount;
    private SCHeroList scHeroList;
    private UILabel sortLabel;
    private UIEventListener sortBtnLis;
    private UIGrid herosGrid;

    #endregion

    #region Window

    public override void OnEnter()
    {
        Refresh();
        InitLockData();
    }

    public override void OnExit()
    {
        var changed = LockStateChanged();
        if (changed)
        {
            ChangedLockList = lockList.Union(lastLockList).Except(lockList.Intersect(lastLockList)).ToList();
            var msg = new CSHeroBind { HeroUuid = ChangedLockList };
            NetManager.SendMessage(msg);
        }
        for (var i = 0; i < lockList.Count; i++)
        {
            ShowLockMask(lockList[i], false);
        }
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    void Awake()
    {
        bindCount = Utils.FindChild(transform, "BindNums").GetComponent<UILabel>();
        sortBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Sort").gameObject);
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();
        herosGrid = Utils.FindChild(transform, "Items").GetComponent<UIGrid>();
        scHeroList = HeroModelLocator.Instance.SCHeroList;
    }

    /// <summary>
    /// Fill in the hero game objects.
    /// </summary> 
    private void UpdateHeroList()
    {
        var heroCount = HeroModelLocator.Instance.SCHeroList.HeroList.Count;
        var childCount = herosGrid.transform.childCount;
        if (childCount != heroCount)
        {
            var isAdd = childCount < heroCount;
            HeroUtils.AddOrDelItems(herosGrid.transform, HeroPrefab.transform, isAdd, Mathf.Abs(heroCount - childCount), HeroConstant.HeroPoolName,
                                OnHeroItemClicked);
            herosGrid.repositionNow = true;
        }
    }

    /// <summary>
    /// Refresh the hero list.
    /// </summary>
    public void Refresh()
    {
        UpdateHeroList();
        var orderType = scHeroList.OrderType;
        sortLabel.text = StringTable.SortStrings[orderType];
        var capacity = PlayerModelLocator.Instance.HeroMax;
        bindCount.text = string.Format("{0}/{1}", cachedLockCount, capacity);
        HeroModelLocator.Instance.SortHeroList((OrderType)orderType, scHeroList.HeroList);
        for (var i = 0; i < scHeroList.HeroList.Count; i++)
        {
            var heroInfo = scHeroList.HeroList[i];
            var item = herosGrid.transform.GetChild(i).GetComponent<HeroItem>();
            item.InitItem(heroInfo);
            HeroUtils.ShowHero((OrderType)orderType, item, heroInfo);
        }
    }

    private void OnHeroItemClicked(GameObject go)
    {
        var heroItem = go.GetComponent<HeroItem>();
        var uid = heroItem.Uuid;
        if (!lockList.Contains(uid))
        {
            lockList.Add(uid);
            heroItem.ShowLockMask(true);
            cachedLockCount++;
        }
        else
        {
            lockList.Remove(uid);
            heroItem.ShowLockMask(false);
            cachedLockCount--;
        }
        var capacity = ItemModeLocator.Instance.ServerConfigMsg.BindItemLimit;
        bindCount.text = string.Format("{0}/{1}", cachedLockCount, capacity);
    }

    private void InitLockData()
    {
        lockList.Clear();
        lastLockList.Clear();
        var infos = scHeroList.HeroList;
        cachedLockCount = 0;
        if (infos == null)
        {
            return;
        }
        for (var i = 0; i < infos.Count; i++)
        {
            var info = infos[i];
            if (info.Bind)
            {
                cachedLockCount++;
                var uid = info.Uuid;
                lastLockList.Add(uid);
                lockList.Add(uid);
                ShowLockMask(uid, true);
            }
        }
    }

    private void ShowLockMask(long uid, bool show)
    {
        var itemsTran = herosGrid.transform;
        for (var i = 0; i < itemsTran.childCount; i++)
        {
            var item = itemsTran.GetChild(i).GetComponent<HeroItem>();
            if (item.Uuid == uid)
            {
                item.ShowLockMask(show);
                break;
            }
        }
    }

    private bool LockStateChanged()
    {
        if (lockList.Count != lastLockList.Count)
        {
            return true;
        }
        if (lockList.Count == 0)
        {
            return false;
        }
        var lockListTemp = new List<long>(lockList);
        lockListTemp.Sort();
        lastLockList.Sort();
        for (var i = 0; i < lockList.Count; i++)
        {
            if (lockListTemp[i] != lastLockList[i])
            {
                return true;
            }
        }
        return false;
    }

    #endregion
}

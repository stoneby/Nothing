using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using UnityEngine;

public class HeroLockHandler : MonoBehaviour 
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
    private UILabel sortLabel;
    private UIEventListener sortBtnLis;
    private UIHerosPageWindow herosWindow;

    #endregion

    #region Private Methods

    private void OnEnable()
    {
        herosWindow = WindowManager.Instance.GetWindow<UIHerosPageWindow>();
        InitLockData();
        var capacity = PlayerModelLocator.Instance.HeroMax;
        bindCount.text = string.Format("{0}/{1}", cachedLockCount, capacity);
    }

    private void OnDisable()
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

    // Use this for initialization
    void Awake()
    {
        bindCount = Utils.FindChild(transform, "BindNum").GetComponent<UILabel>();
    }

    public void OnHeroItemClicked(GameObject go)
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
        var infos = HeroModelLocator.Instance.SCHeroList.HeroList;
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
        var itemsTran = herosWindow.Heros.transform;
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

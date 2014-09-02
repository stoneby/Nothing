using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using Template.Auto.Hero;
using UnityEngine;

public class UIHeroCommonWindow : Window
{
    private ExtendBag itemExtendConfirm;
    private UIEventListener extendLis;
    private UIEventListener sortBtnLis;
    private UIEventListener closeBtnLis;
    private StretchItem closeBtnLine;
    private UILabel sortLabel;
    private UILabel herosNum;
    private SCHeroList scHeroList;
    private UIEventListener.VoidDelegate normalClicked;
    private bool descendSort;
    private Transform selMask;
    private Position defaultPos = HeroConstant.FirstPos;
    private HeroInfo heroInfo;
    private HeroTemplate heroTemplate;
    private const int HeroCellLength = 120;
    private long uuidCached;
    public UIHeroDetailHandler HeroDetailHandler;
    public UILevelUpHeroHandler LevelUpHeroHandler;
    public UISellHeroHandler SellHeroHandler;
    public HeroTemplate HeroTemplate
    {
        get { return heroTemplate; }
    }

    private Position curSelPos = HeroConstant.InvalidPos;
    public Position CurSelPos
    {
        get { return curSelPos; }
        set
        {
            if (curSelPos != value)
            {
                curSelPos = value;
                var oneDim = value.X * CountOfOneGroup + value.Y;
                if (Infos != null && Infos.Count > oneDim)
                {
                    HeroInfo = Infos[oneDim];
                }
            }
        }
    }

    public List<long> LockedMemberUuid = new List<long>();

    public List<HeroInfo> Infos { get; private set; }

    /// <summary>
    /// The current hero info.
    /// </summary>
    public HeroInfo HeroInfo
    {
        get { return heroInfo; }
        set
        {
            if (heroInfo != value)
            {
                heroInfo = value;
                heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[heroInfo.TemplateId];
            }
        }
    }

    public CustomGrid Heros;
    public UIToggle DescendToggle;
    public GameObject ExtendBagConfirm;
    /// <summary>
    /// The prefab of the hero.
    /// </summary>
    public GameObject HeroPrefab;
    public int CountOfOneGroup = 4;
    public delegate void SortOrderChanged(List<HeroInfo> hInfos);

    /// <summary>
    /// The call back of the sort order changed.
    /// </summary>
    public SortOrderChanged OnSortOrderChangedBefore;
    public SortOrderChanged OnSortOrderChangedAfter;

    public UIEventListener.VoidDelegate NormalClicked
    {
        get { return normalClicked; }
        set
        {
            normalClicked = value;
            var parent = Heros.transform;
            for (var i = 0; i < parent.childCount; i++)
            {
                var item = parent.GetChild(i);
                if (item.name != "ExtendButton")
                {
                    for (var j = 0; j < item.childCount; j++)
                    {
                        var hero = item.GetChild(j).gameObject;
                        var activeCache = hero.activeSelf;
                        NGUITools.SetActive(hero, true);
                        var lis = UIEventListener.Get(hero);
                        lis.onClick = value;
                        NGUITools.SetActive(hero, activeCache);
                    }
                }
            }
        }
    }

    #region Window

    public override void OnEnter()
    {
        Infos = scHeroList.HeroList;
        var orderType = HeroModelLocator.Instance.OrderType;
        sortLabel.text = LanguageManager.Instance.GetTextValue(ItemHelper.SortKeys[(int)orderType]);
        NGUITools.SetActive(Heros.gameObject, true);
        InitWrapContents(Infos);
        if (Infos != null && Infos.Count > 0)
        {
            ShowSelMask(Infos != null && Infos.Count > 0);
       
            RefreshLockMemberList();
        }
        InstallHandlers();
    }

    public void RefreshSelMask(Position position)
    {
        CurSelPos = position;
        var heros = Heros.transform;
        var childCount = heros.childCount;
        for (var i = 0; i < childCount; i++)
        {
            var wrapHerosItem = heros.GetChild(i).GetComponent<WrapHerosItem>();
            var found = false;
            if (wrapHerosItem != null && wrapHerosItem.gameObject.activeSelf)
            {
                for (var j = 0; j < wrapHerosItem.Children.Count; j++)
                {
                    var pos = new Position { X = wrapHerosItem.Row, Y = j };
                    found = pos == position;
                    if (found)
                    {
                        var selObj = wrapHerosItem.Children[pos.Y].gameObject;
                        ShowSelMask(selObj.transform.position);
                        break;
                    }
                }
            }
            if(found)
            {
                break;
            }
        }
    }

    private void InitWrapContents(List<HeroInfo> heroInfos)
    {
        RefreshHeroMaxNum();
        RepositionMaxExtendBtn();
        if (heroInfos == null || heroInfos.Count == 0)
        {
            ItemHelper.HideItems(Heros.transform);
            return;
        }
        var orderType = HeroModelLocator.Instance.OrderType;
        HeroModelLocator.Instance.SortHeroList(orderType, heroInfos, descendSort);
        var data = new List<List<long>>();
        var rows = Mathf.CeilToInt((float)heroInfos.Count / CountOfOneGroup);
        for (var i = 0; i < rows; i++)
        {
            var list = new List<long>();
            for (var j = 0; j < CountOfOneGroup; j++)
            {
                if (i * CountOfOneGroup + j < heroInfos.Count)
                {
                    list.Add(heroInfos[i * CountOfOneGroup + j].Uuid);
                }
            }
            data.Add(list);
        }
        Heros.Init(data);
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        WindowManager.Instance.Show<UIMainScreenWindow>(true);
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    void Awake()
    {
        var buttons = transform.Find("Buttons");
        sortBtnLis = UIEventListener.Get(buttons.Find("Button-Sort").gameObject);
        closeBtnLis = UIEventListener.Get(buttons.Find("Button-Close").gameObject);
        closeBtnLine = buttons.Find("Button-CloseLine").GetComponent<StretchItem>();
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();
        herosNum = Utils.FindChild(transform, "HeroNumValue").GetComponent<UILabel>();
        scHeroList = HeroModelLocator.Instance.SCHeroList;
        NGUITools.SetActive(Heros.transform.parent.gameObject, true);
        selMask = transform.Find("SelMask");
        selMask.transform.parent = Heros.transform.parent;
        NGUITools.SetActive(selMask.gameObject, false);
        extendLis = UIEventListener.Get(Heros.transform.Find("ExtendButton/Icon").gameObject);
    }

    /// <summary>
    /// Refresh LockMemberUuid in OnEnter.
    /// </summary>
    private void RefreshLockMemberList()
    {
        LockedMemberUuid.Clear();
        if(Infos == null)
        {
            return;
        }
        foreach (var item in Infos)
        {
            if (item.Bind)
            {
                LockedMemberUuid.Add(item.Uuid);
            }
        }
    }

    private void OnExtend(GameObject go)
    {
        if (ItemModeLocator.Instance.Bag.HeroExtTmpls.Count - PlayerModelLocator.Instance.ExtendHeroTimes != 0)
        {
            itemExtendConfirm = NGUITools.AddChild(transform.gameObject, ExtendBagConfirm).GetComponent<ExtendBag>();
            itemExtendConfirm.ExtendContentKey = HeroConstant.ExtendContentKey;
            itemExtendConfirm.ExtendLimitKey = HeroConstant.ExtendLimitKey;
            var bases = ItemModeLocator.Instance.Bag;
            var costDict = bases.HeroExtTmpls.ToDictionary(item => item.Value.Id, item => item.Value.ExtendCost);
            itemExtendConfirm.Init(PlayerModelLocator.Instance.ExtendHeroTimes, bases.BagBaseTmpls[1].ExtendHeroCount,
                                   costDict);
            itemExtendConfirm.OkClicked += OnExtendBagOk;
        }
        else
        {
            PopTextManager.PopTip("可拥有武将数已达上限", false);
        }
    }

    private void OnExtendBagOk(GameObject go)
    {
        var msg = new CSHeroMaxExtend() { ExtendTimes = (sbyte)itemExtendConfirm.ExtendSize };
        NetManager.SendMessage(msg);
    }

    private void RepositionMaxExtendBtn()
    {
        var count = Infos == null ? 0 : Infos.Count;
        var pos = Utils.OneDimToTwo(count, CountOfOneGroup);
        extendLis.transform.parent.localPosition = new Vector3(pos.Y * HeroCellLength, -pos.X * HeroCellLength, 0);
    }

    public void RefreshHeroMaxNum()
    {
        var count = Infos == null ? 0 : Infos.Count;
        herosNum.text = string.Format("{0}/{1}", count, PlayerModelLocator.Instance.HeroMax);
    }

    private void InstallHandlers()
    {
        EventDelegate.Add(DescendToggle.onChange, SortTypeChanged);
        extendLis.onClick = OnExtend;
        sortBtnLis.onClick = OnSort;
        closeBtnLis.onClick = OnClose;
        closeBtnLine.DragFinish = OnClose;
        Heros.OnUpdate += OnUpdate;
    }

    private void UnInstallHandlers()
    {
        extendLis.onClick = null;
        sortBtnLis.onClick = null;
        closeBtnLis.onClick = null;
        closeBtnLine.DragFinish = null;
        EventDelegate.Remove(DescendToggle.onChange, SortTypeChanged);
        Heros.OnUpdate -= OnUpdate;
    }

    private void OnUpdate(GameObject sender, int index)
    {
        var wrapHero = sender.GetComponent<WrapHerosItem>();
        var col = wrapHero.Children.Count;
        for (var i = 0; i < col; i++)
        {
            var pos = new Position { X = index, Y = i };
            var show = pos == CurSelPos;
            if(show)
            {
                ShowSelMask(wrapHero.Children[pos.Y].position);
            }
        }
    }

    private void SortTypeChanged()
    {
        descendSort = DescendToggle.value;
        defaultPos = descendSort ? Utils.OneDimToTwo(Infos.Count - 1, CountOfOneGroup) : HeroConstant.FirstPos;
        if (curSelPos == HeroConstant.InvalidPos)
        {
            RefreshSelMask(defaultPos);
        }
        SortBefore();
        InitWrapContents(Infos);
        SortAfter();
    }

    private void SortBefore()
    {
        if (OnSortOrderChangedBefore != null)
        {
            OnSortOrderChangedBefore(Infos);
        }
        uuidCached = GetInfo(curSelPos).Uuid;
    }

    private void SortAfter()
    {
        if (OnSortOrderChangedAfter != null)
        {
            OnSortOrderChangedAfter(Infos);
        }
        if (CurSelPos.X >= 0 && CurSelPos.Y >= 0 && Infos != null)
        {
            var newIndex = Infos.FindIndex(info => info.Uuid == uuidCached);
            CurSelPos = Utils.OneDimToTwo(newIndex, CountOfOneGroup);
            RefreshSelMask(CurSelPos);
        }
    }

    private void OnSort(GameObject go)
    {
        SortBefore();
        var orderType = HeroModelLocator.Instance.OrderType;
        orderType = (ItemHelper.OrderType)(((int)orderType + 1) % ItemHelper.SortKeys.Count);
        sortLabel.text = LanguageManager.Instance.GetTextValue(ItemHelper.SortKeys[(int)orderType]);
        HeroModelLocator.Instance.OrderType = orderType;
        InitWrapContents(Infos);
        SortAfter();
    }

    private void OnClose(GameObject go)
    {
        WindowManager.Instance.Show<UIHeroCommonWindow>(false);
    }

    /// <summary>
    /// Refresh the heros page window with the special hero info list.
    /// </summary>
    /// <param name="newInfos">The hero info list to refresh data.</param>
    /// <param name="teamIndex"> </param>
    public void Refresh(List<HeroInfo> newInfos, int teamIndex)
    {

    }

    /// <summary>
    /// Refresh the heros page window with the special hero info list.
    /// </summary>
    /// <param name="newInfos">The hero info list to refresh data.</param>
    public void Refresh(List<HeroInfo> newInfos)
    {
        var count = newInfos == null ? 0 : newInfos.Count;
        herosNum.text = string.Format("{0}/{1}", count, PlayerModelLocator.Instance.HeroMax);
        InitWrapContents(newInfos);
    }

    /// <summary>
    /// Refresh the heros page window with the special hero info list.
    /// </summary>
    public void Refresh()
    {
        Refresh(Infos);
    }

    public void Refresh(int teamIndex)
    {
        Refresh(Infos, teamIndex);
    }

    public void ShowSelMask(Vector3 pos)
    {
        selMask.position = pos;
        ShowSelMask(true);
    }

    public void ShowSelMask(bool show)
    {
        NGUITools.SetActive(selMask.gameObject, show);
    }

    public HeroInfo GetInfo(Position pos)
    {
        var oneDimsionIndex = pos.X * CountOfOneGroup + pos.Y;
        return Infos[oneDimsionIndex];
    }

    public void ShowLockState(UISprite lockSprite)
    {
        if (LockedMemberUuid.Contains(heroInfo.Uuid))
        {
            lockSprite.spriteName = "HeroLock_close";
        }
        else
        {
            lockSprite.spriteName = "HeroLock_open";
        }
    }

    public void ReverseLockState()
    {
        if (LockedMemberUuid.Contains(heroInfo.Uuid))
        {
            LockedMemberUuid.Remove(heroInfo.Uuid);
            Logger.Log("!!!!!!!!!!!UnLocked hero.");
        }
        else
        {
            LockedMemberUuid.Add(heroInfo.Uuid);
            Logger.Log("!!!!!!!!!!!Locked hero.");
        }
        var csHeroBindMsg = new CSHeroBind { HeroUuid = new List<long> { heroInfo.Uuid } };
        NetManager.SendMessage(csHeroBindMsg);
    }

    #endregion
}

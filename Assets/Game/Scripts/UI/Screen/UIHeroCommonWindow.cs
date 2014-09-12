using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using Template.Auto.Hero;
using UnityEngine;

public class UIHeroCommonWindow : Window
{
    private ExtendBag itemExtendConfirm;
    private UIEventListener extendLis;
    private UILabel herosNum;
    private SCHeroList scHeroList;
    private bool isTriggerByStart = true;
    private UIEventListener.VoidDelegate normalClicked;
    private Transform selMask;
    private readonly Position defaultPos = HeroConstant.FirstPos;
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
            var oneDim = value.X * CountOfOneGroup + value.Y;
            if(Infos != null && Infos.Count > oneDim && oneDim >= 0)
            {
                HeroInfo = Infos[oneDim];
                curSelPos = value;
                var localPos = new Vector3(CurSelPos.Y * HeroCellLength, -CurSelPos.X * HeroCellLength, 0);
                selMask.transform.position = Heros.transform.TransformPoint(localPos);
            }
            else
            {
                HeroInfo = null;
                curSelPos = HeroConstant.InvalidPos;
                ShowSelMask(false);
            }
        }
    }

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
    public GameObject ExtendBagConfirm;
    public HeroSortControl HeroSortControl;
    public CloseButtonControl CloseControl;
    /// <summary>
    /// The prefab of the hero.
    /// </summary>
    public GameObject BaseHeroPrefab;
    public int CountOfOneGroup = 4;

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
        NGUITools.SetActive(Heros.gameObject, true);
        HeroSortControl.Init(Infos);
        InitWrapContents(Infos);
        if (Infos != null && Infos.Count > 0)
        {
            ShowSelMask(true);
        }
        if(curSelPos == HeroConstant.InvalidPos)
        {
            CurSelPos = defaultPos;
        }
        InstallHandlers();
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
        HeroUtils.InitWrapContents(Heros, heroInfos, CountOfOneGroup, PlayerModelLocator.Instance.HeroMax);
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
        herosNum = Utils.FindChild(transform, "HeroNumValue").GetComponent<UILabel>();
        scHeroList = HeroModelLocator.Instance.SCHeroList;
        NGUITools.SetActive(Heros.transform.parent.gameObject, true);
        selMask = transform.Find("SelMask");
        selMask.transform.parent = Heros.transform.parent;
        NGUITools.SetActive(selMask.gameObject, false);
        extendLis = UIEventListener.Get(Heros.transform.Find("ExtendButton/Icon").gameObject);
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
        CloseControl.OnCloseWindow = OnClose;
        HeroSortControl.InstallHandlers();
        HeroSortControl.OnSortOrderChangedBefore += SortBefore;
        HeroSortControl.OnSortOrderChangedAfter += SortAfter;
        HeroSortControl.OnExcuteAfterSort += OnExcuteAfterSort;
        extendLis.onClick = OnExtend;
    }

    private void UnInstallHandlers()
    {
        CloseControl.OnCloseWindow = null;
        HeroSortControl.UnInstallHandlers();
        HeroSortControl.OnSortOrderChangedBefore -= SortBefore;
        HeroSortControl.OnSortOrderChangedAfter -= SortAfter;
        HeroSortControl.OnExcuteAfterSort -= OnExcuteAfterSort;
        extendLis.onClick = null;
    }

    private void SortBefore()
    {
        if(CurSelPos != HeroConstant.InvalidPos)
        {
            uuidCached = GetInfo(curSelPos).Uuid;
        }
    }

    private void SortAfter()
    {
        InitWrapContents(Infos);
        if (CurSelPos != HeroConstant.InvalidPos && Infos != null)
        {
            var newIndex = Infos.FindIndex(info => info.Uuid == uuidCached);
            CurSelPos = Utils.OneDimToTwo(newIndex, CountOfOneGroup);
        }
        if (isTriggerByStart)
        {
            CurSelPos = defaultPos;
            isTriggerByStart = false;
        }
    }

    private void OnExcuteAfterSort()
    {
        InitWrapContents(Infos);
    }

    private void OnClose()
    {
        WindowManager.Instance.Show<UIMainScreenWindow>(true);
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

    public void ShowSelMask(bool show)
    {
        NGUITools.SetActive(selMask.gameObject, show);
    }

    public HeroInfo GetInfo(Position pos)
    {
        var oneDimsionIndex = pos.X * CountOfOneGroup + pos.Y;
        return Infos[oneDimsionIndex];
    }

    #endregion
}

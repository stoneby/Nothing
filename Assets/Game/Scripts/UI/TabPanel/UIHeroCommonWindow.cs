using System.Collections.Generic;
using KXSGCodec;
using Template.Auto.Hero;
using UnityEngine;

public class UIHeroCommonWindow : Window 
{
    private UIEventListener sortBtnLis;
    private UIEventListener closeBtnLis;
    private StretchItem closeBtnLine;

    private UILabel sortLabel;
    private UILabel herosNum;

    public List<HeroInfo> Infos { get; private set; }

    private SCHeroList scHeroList;
    private UIEventListener.VoidDelegate normalClicked;

    private GameObject ScrollViewObj;
    public CustomGrid Heros;
    public UIToggle DescendToggle;

    private bool descendSort;

    private bool isEntered;

    private Transform selMask;
    public GameObject CurSel;

    public UIHeroDetailHandler HeroDetailHandler;
    public UILevelUpHeroHandler LevelUpHeroHandler;
    public UISellHeroHandler SellHeroHandler;

    private HeroInfo heroInfo;
    private HeroTemplate heroTemplate;

    public HeroTemplate HeroTemplate
    {
        get { return heroTemplate; }
    }

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
 
    #region Window

    public override void OnEnter()
    {
        isEntered = true;
        var orderType = HeroModelLocator.Instance.OrderType;
        sortLabel.text = LanguageManager.Instance.GetTextValue(ItemHelper.SortKeys[(int)orderType]);
        NGUITools.SetActive(Heros.gameObject, true);
        herosNum.text = string.Format("{0}/{1}", Infos.Count, PlayerModelLocator.Instance.HeroMax);
        InitWrapContents(Infos);
        InstallHandlers();
    }

    private void InitWrapContents(List<HeroInfo> heroInfos)
    {
        var orderType = HeroModelLocator.Instance.OrderType;
        HeroModelLocator.Instance.SortHeroList(orderType, Infos, descendSort);
        var data = new List<List<long>>();
        var rows = Mathf.CeilToInt((float)heroInfos.Count / CountOfOneGroup);
        for (var i = 0; i < rows; i++)
        {
            var list = new List<long>();
            for (var j = 0; j < CountOfOneGroup; j++)
            {
                if (i * CountOfOneGroup + j < Infos.Count)
                {
                     list.Add(Infos[i * CountOfOneGroup + j].Uuid);
                }
            }
            data.Add(list);
        }
        Heros.Init(data);
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        isEntered = false;
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    void Awake()
    {
        ScrollViewObj = transform.FindChild("Scroll View").gameObject;
        var buttons = transform.Find("Buttons");
        sortBtnLis = UIEventListener.Get(buttons.Find("Button-Sort").gameObject);
        closeBtnLis = UIEventListener.Get(buttons.Find("Button-Close").gameObject);
        closeBtnLine = buttons.Find("Button-CloseLine").GetComponent<StretchItem>();
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();
        herosNum = Utils.FindChild(transform, "HeroNumValue").GetComponent<UILabel>();
        scHeroList = HeroModelLocator.Instance.SCHeroList;
        Infos = scHeroList.HeroList ?? new List<HeroInfo>();
        NGUITools.SetActive(Heros.transform.parent.gameObject, true);
        selMask = transform.Find("SelMask");
        selMask.transform.parent = Heros.transform.parent;
        NGUITools.SetActive(selMask.gameObject, false);
        HeroDetailHandler.SetParentBoolDelegate = DetailSetHandler;

        if (ItemModeLocator.Instance.ScAllItemInfos == null)
        {
            ItemModeLocator.Instance.GetItemPos = ItemType.GetItemInHeroInfo;
            var csmsg = new CSQueryAllItems { BagType = ItemType.MainItemBagType };
            NetManager.SendMessage(csmsg);
        }
    }

    private void DetailSetHandler(GameObject obj, bool state)
    {
        if (ScrollViewObj != null) ScrollViewObj.SetActive(state);
    }

    private void InstallHandlers()
    {
        EventDelegate.Add(DescendToggle.onChange, SortTypeChanged);
        sortBtnLis.onClick = OnSort;
        closeBtnLis.onClick = OnClose;
        closeBtnLine.DragFinish = OnClose;
    }

    private void UnInstallHandlers()
    {
        sortBtnLis.onClick = null;
        closeBtnLis.onClick = null;
        closeBtnLine.DragFinish = null;
        EventDelegate.Remove(DescendToggle.onChange, SortTypeChanged);
    }

    private void SortTypeChanged()
    {
        descendSort = DescendToggle.value;
        if (isEntered)
        {
            InitWrapContents(Infos);
        }
    }

    private void OnSort(GameObject go)
    {
        if (OnSortOrderChangedBefore != null)
        {
            OnSortOrderChangedBefore(Infos);
        }
        var orderType = HeroModelLocator.Instance.OrderType;
        orderType = (ItemHelper.OrderType)(((int)orderType + 1) % ItemHelper.SortKeys.Count);
        sortLabel.text = LanguageManager.Instance.GetTextValue(ItemHelper.SortKeys[(int)orderType]);
        HeroModelLocator.Instance.OrderType = orderType;
        InitWrapContents(Infos);
        if (OnSortOrderChangedAfter != null)
        {
            OnSortOrderChangedAfter(Infos);
        }
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
        herosNum.text = string.Format("{0}/{1}", newInfos.Count, PlayerModelLocator.Instance.HeroMax);
        InitWrapContents(newInfos);
    }

    public void Refresh(int teamIndex)
    {
        Refresh(Infos, teamIndex);
    }

    public void ShowSelMask(Vector3 pos)
    {
        selMask.position = pos;
        ShowSelMask();
    }

    public void ShowSelMask()
    {
        if (selMask.gameObject.activeSelf == false)
        {
            NGUITools.SetActive(selMask.gameObject, true);
        }
    }

    public void HideSelMask()
    {
        if(selMask.gameObject.activeSelf)
        {
            NGUITools.SetActive(selMask.gameObject, false);
        }
    }

    public HeroInfo GetInfo(Position pos)
    {
        var oneDimsionIndex = pos.X * CountOfOneGroup + pos.Y;
        return Infos[oneDimsionIndex];
    }

    #endregion
}

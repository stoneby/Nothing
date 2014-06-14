using System.Globalization;
using KXSGCodec;
using Template;
using UnityEngine;

/// <summary>
/// To show common ui of hero skill view, hero level up and hero limit break. 
/// </summary>
public class HeroBaseInfoWindow : Window
{
    #region Private Fields

    private EndlessSwipeEffect endlessSwipeEffect;
    private Vector3 endlessPosCached;
    private int curHeroIndex;
    private HeroInfo heroInfo;
    public HeroInfo HeroInfo
    {
        get { return heroInfo; }
        private set
        {
            if(heroInfo != value)
            {
                heroInfo = value;
                heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
                CurUuid = heroInfo.Uuid;
            }
        }
    }
    private UIEventListener skillBtnLis;
    private UIEventListener lvBtnLis;
    private UIEventListener limitBtnLis;
    private HeroTemplate heroTemplate;
    private UIEventListener item1Lis;
    private UIEventListener item2Lis;
    private sbyte curEquipIndex = -1;

    #endregion

    #region Public Fields

    /// <summary>
    /// The uuid of current hero info.
    /// </summary>
    public static long CurUuid;

    public sbyte CurEquipIndex
    {
        get { return curEquipIndex; }
        private set { curEquipIndex = value; }
    }

    /// <summary>
    /// The sprite name when the button is clicked.
    /// </summary>
    public static string DownBtnSpriteName = "BtnD";

    /// <summary>
    /// The sprite name when the button is normal.
    /// </summary>
    public static string NormalBtnSpriteName = "BtnN";

    #endregion

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
        Toggle(1);
        HeroInfo = HeroModelLocator.Instance.FindHero(CurUuid);
        curHeroIndex = HeroModelLocator.Instance.SCHeroList.HeroList.IndexOf(HeroInfo);
        endlessSwipeEffect.InitCustomData(curHeroIndex, HeroModelLocator.Instance.SCHeroList.HeroList.Count);
        Refresh();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        endlessSwipeEffect.transform.position = endlessPosCached;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Awake()
    {
        skillBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Skill").gameObject);
        lvBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-LV").gameObject);
        limitBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Limit").gameObject);
        endlessSwipeEffect = GetComponentInChildren<EndlessSwipeEffect>();
        endlessPosCached = endlessSwipeEffect.transform.position;
        endlessSwipeEffect.UpdateData += UpdateData;
        item1Lis = UIEventListener.Get(Utils.FindChild(transform, "Item1").gameObject);
        item2Lis = UIEventListener.Get(Utils.FindChild(transform, "Item2").gameObject);
    }

    /// <summary>
    /// Update the current data.
    /// </summary>
    private void UpdateData()
    {
        curHeroIndex = endlessSwipeEffect.CurCustomIndex;
        HeroInfo = HeroModelLocator.Instance.SCHeroList.HeroList[curHeroIndex];
        Refresh();
        WindowManager.Instance.GetWindow<UIHeroInfoWindow>().RefreshData(HeroInfo);
    }

    /// <summary>
    /// Initialize the endless swipe effect in case it has dependence.
    /// </summary>
    private void Start()
    {
        //Toggle(1);
        //HeroInfo = HeroModelLocator.Instance.FindHero(CurUuid);
        //curHeroIndex = HeroModelLocator.Instance.SCHeroList.HeroList.IndexOf(HeroInfo);
        //endlessSwipeEffect.InitCustomData(curHeroIndex, HeroModelLocator.Instance.SCHeroList.HeroList.Count);
        //Refresh();
    }

    /// <summary>
    /// Install all handlers.
    /// </summary>
    private void InstallHandlers()
    {
        skillBtnLis.onClick += OnSkillBtnClicked;
        lvBtnLis.onClick += OnLvBtnClicked;
        limitBtnLis.onClick += OnLimitBtnClicked;
        item1Lis.onClick += HeroSelItemHandler;
        item2Lis.onClick += HeroSelItemHandler;
    }

    /// <summary>
    /// UnInstall all handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        skillBtnLis.onClick -= OnSkillBtnClicked;
        lvBtnLis.onClick -= OnLvBtnClicked;
        limitBtnLis.onClick -= OnLimitBtnClicked;
        item1Lis.onClick -= HeroSelItemHandler;
        item2Lis.onClick -= HeroSelItemHandler;
    }

    private void HeroSelItemHandler(GameObject go)
    {
        CurEquipIndex = (sbyte)(go == item1Lis.gameObject ? 0 : 1);
        if(ItemModeLocator.Instance.ScAllItemInfos == null)
        {
            ItemModeLocator.Instance.GetItemPos = ItemType.GetItemInHeroInfo;
            var csmsg = new CSQueryAllItems { BagType = 0 };
            NetManager.SendMessage(csmsg);       
        }
        else
        {
            ShowHeroSelItems();
        }
    }

    /// <summary>
    /// Update all ui related data.
    /// </summary>
    private void Refresh()
    {
        var infoTran = Utils.FindChild(transform, "Info");
        Utils.FindChild(infoTran, "Name").GetComponent<UILabel>().text = heroTemplate.Name;
        var stars = Utils.FindChild(infoTran, "Stars");
        for (int index = 0; index < heroTemplate.Star; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, true);
        }
        for (int index = heroTemplate.Star; index < stars.childCount; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, false);
        }
        Utils.FindChild(infoTran, "LV-Value").GetComponent<UILabel>().text = string.Format("{0}/{1}", HeroInfo.Lvl, heroTemplate.LvlLimit);
        Utils.FindChild(infoTran, "Limit-Value").GetComponent<UILabel>().text = string.Format("{0}/{1}", HeroInfo.BreakTimes, heroTemplate.BreakLimit);
        Utils.FindChild(infoTran, "Luck-Value").GetComponent<UILabel>().text = heroTemplate.Lucky.ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(infoTran, "Job-Value").GetComponent<UISprite>().spriteName = UIHerosDisplayWindow.JobPrefix + heroTemplate.Job;
        var equips = HeroInfo.EquipUuid;
        for (int i = 0; i < equips.Count; i++)
        {
            var itemIcon = Utils.FindChild(infoTran, string.Format("Item{0}Icon", i+1));
            var itemLabel = Utils.FindChild(infoTran, string.Format("Item{0}Label", i + 1));
            if(equips[i] == "")
            {
                itemIcon.gameObject.SetActive(false);
                itemLabel.gameObject.SetActive(true);
            }
            else
            {
                itemIcon.gameObject.SetActive(true);
                itemLabel.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// The callback of clicking hero limit break button.
    /// </summary>
    private void OnLimitBtnClicked(GameObject go)
    {
        
    }

    /// <summary>
    /// The callback of clicking hero level up button.
    /// </summary>
    private void OnLvBtnClicked(GameObject go)
    {
        Toggle(2);
        WindowManager.Instance.Show(typeof(UILevelUpWindow), true);
    }

    /// <summary>
    /// The callback of clicking skill view button.
    /// </summary>
    private void OnSkillBtnClicked(GameObject go)
    {
        Toggle(1);
        WindowManager.Instance.Show(typeof(UIHeroInfoWindow), true);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Show or hide all buttons.
    /// </summary>
    /// <param name="show">If true, show all buttons.</param>
    public void ShowButtons(bool show)
    {
        NGUITools.SetActive(skillBtnLis.gameObject, show);
        NGUITools.SetActive(lvBtnLis.gameObject, show);
        NGUITools.SetActive(limitBtnLis.gameObject, show);
    }

    /// <summary>
    /// Toggle the button's color.
    /// </summary>
    /// <param name="index">Indicates which button will have the highlight color.</param>
    public void Toggle(int index)
    {
        switch(index)
        {
            case 1:
                skillBtnLis.GetComponent<UISprite>().spriteName = DownBtnSpriteName;
                lvBtnLis.GetComponent<UISprite>().spriteName = NormalBtnSpriteName;
                limitBtnLis.GetComponent<UISprite>().spriteName = NormalBtnSpriteName;
                break;
            case 2:
                lvBtnLis.GetComponent<UISprite>().spriteName = DownBtnSpriteName;
                skillBtnLis.GetComponent<UISprite>().spriteName = NormalBtnSpriteName;
                limitBtnLis.GetComponent<UISprite>().spriteName = NormalBtnSpriteName;
                break;
            case 3:
                limitBtnLis.GetComponent<UISprite>().spriteName = DownBtnSpriteName;
                skillBtnLis.GetComponent<UISprite>().spriteName = NormalBtnSpriteName;
                lvBtnLis.GetComponent<UISprite>().spriteName = NormalBtnSpriteName;
                break;
        }
    }

    public void EnableSwipeEffect(bool enable)
    {
        endlessSwipeEffect.enabled = enable;
    }

    public void ShowHeroSelItems()
    {
        WindowManager.Instance.Show<UIHeroSelItemWindow>(true);
        WindowManager.Instance.Show<HeroBaseInfoWindow>(false);
    }

    #endregion
}

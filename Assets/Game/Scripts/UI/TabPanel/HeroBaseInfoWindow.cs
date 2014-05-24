using KXSGCodec;
using UnityEngine;

/// <summary>
/// To show common ui of hero skill view, hero level up and hero limit break. 
/// </summary>
public class HeroBaseInfoWindow : Window
{
    #region Private Fields

    private EndlessSwipeEffect endlessSwipeEffect;
    private int curHeroIndex;
    private HeroInfo heroInfo;
    private HeroInfo HeroInfo
    {
        get { return heroInfo; }
        set
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

    #endregion

    #region Public Fields

    /// <summary>
    /// The uuid of current hero info.
    /// </summary>
    public static long CurUuid;

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
    }

    public override void OnExit()
    {
        UnInstallHandlers();
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
        endlessSwipeEffect.UpdateData += UpdateData;
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
        Toggle(1);
        HeroInfo = HeroModelLocator.Instance.FindHero(CurUuid);
        curHeroIndex = HeroModelLocator.Instance.SCHeroList.HeroList.IndexOf(HeroInfo);
        endlessSwipeEffect.InitCustomData(curHeroIndex, HeroModelLocator.Instance.SCHeroList.HeroList.Count);
        Refresh();
    }

    /// <summary>
    /// Install all handlers.
    /// </summary>
    private void InstallHandlers()
    {
        skillBtnLis.onClick += OnSkillBtnClicked;
        lvBtnLis.onClick += OnLvBtnClicked;
        limitBtnLis.onClick += OnLimitBtnClicked;
    }

    /// <summary>
    /// UnInstall all handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        skillBtnLis.onClick -= OnSkillBtnClicked;
        lvBtnLis.onClick -= OnLvBtnClicked;
        limitBtnLis.onClick -= OnLimitBtnClicked;
    }

    /// <summary>
    /// Update all ui related data.
    /// </summary>
    private void Refresh()
    {
        Utils.FindChild(transform, "Name").GetComponent<UILabel>().text = heroTemplate.Name;
        var stars = Utils.FindChild(transform, "Stars");
        for (int index = 0; index < heroTemplate.Star; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, true);
        }
        for (int index = heroTemplate.Star; index < stars.childCount; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, false);
        }
        Utils.FindChild(transform, "LV-Value").GetComponent<UILabel>().text = string.Format("{0}/{1}", HeroInfo.Lvl, heroTemplate.LvlLimit);
        Utils.FindChild(transform, "Limit-Value").GetComponent<UILabel>().text = string.Format("{0}/{1}", HeroInfo.BreakTimes, heroTemplate.BreakLimit);
        Utils.FindChild(transform, "Luck-Value").GetComponent<UILabel>().text = heroTemplate.Lucky.ToString();
        Utils.FindChild(transform, "Job-Value").GetComponent<UISprite>().spriteName = UIHerosDisplayWindow.JobPrefix + heroTemplate.Job;
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

    #endregion
}

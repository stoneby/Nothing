using System.Globalization;
using KXSGCodec;
using Property;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIHeroSnapShotWindow : Window
{
    private UIEventListener cancelLis;
    private UIEventListener viewDetailLis;
    private UIEventListener templateLis;
    private UIEventListener backLis;
    private UILabel templateLabel;
    private UILabel atkLabel;
    private UILabel hpLabel;
    private UILabel recoverLabel;
    private UILabel mpLabel;
    private UILabel level;
    private UILabel nameLabel;
    private UISprite jobIcon;

    public UIEventListener.VoidDelegate TemplateBtnPressed;

    private HeroInfo heroInfo;

    /// <summary>
    /// The uuid of current hero info.
    /// </summary>
    public static long CurUuid;

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
        heroInfo = HeroModelLocator.Instance.FindHero(CurUuid);
        Refresh();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Public Methods

    public void InitTemplate(string key, UIEventListener.VoidDelegate templatePressed)
    {
        templateLabel.text = LanguageManager.Instance.GetTextValue(key);
        TemplateBtnPressed = templatePressed;
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        cancelLis = UIEventListener.Get(transform.Find("Buttons/CancelBtn").gameObject);
        viewDetailLis = UIEventListener.Get(transform.Find("Buttons/ViewDetailBtn").gameObject);
        templateLis = UIEventListener.Get(transform.Find("Buttons/TemplateBtn").gameObject);
        backLis = UIEventListener.Get(transform.Find("Buttons/Button-Back").gameObject);
        templateLabel = templateLis.GetComponentInChildren<UILabel>();
        var property = transform.Find("Property");
        atkLabel = property.Find("Attack/AttackValue").GetComponent<UILabel>();
        hpLabel = property.Find("HP/HPValue").GetComponent<UILabel>();
        recoverLabel = property.Find("Recover/RecoverValue").GetComponent<UILabel>();
        mpLabel = property.Find("MP/MPValue").GetComponent<UILabel>();
        level = transform.Find("Hero/Level/LevelLabel").GetComponent<UILabel>();
        jobIcon = transform.Find("Hero/Job/JobIcon").GetComponent<UISprite>();
        nameLabel = transform.Find("Name").GetComponent<UILabel>();
    }

    private void InstallHandlers()
    {
        cancelLis.onClick = OnCancel;
        viewDetailLis.onClick = OnViewDetail;
        templateLis.onClick = OnTemplate;
        backLis.onClick = OnBack;
    }

    private void UnInstallHandlers()
    {
        cancelLis.onClick = null;
        viewDetailLis.onClick = null;
        templateLis.onClick = null;
        backLis.onClick = null;
    }

    private void OnBack(GameObject go)
    {
        WindowManager.Instance.Show<UIHeroSnapShotWindow>(false);
    }

    private void OnCancel(GameObject go)
    {
        WindowManager.Instance.Show<UIHeroSnapShotWindow>(false);
    }

    private void OnViewDetail(GameObject go)
    {
        UIHeroDetailHandler.IsLongPressEnter = false;
        //var heroDetail = WindowManager.Instance.Show<UIHeroDetailHandler>(true, true);
        //heroDetail.RefreshData(heroInfo);
    }

    private void OnTemplate(GameObject go)
    {
        if(TemplateBtnPressed != null)
        {
            TemplateBtnPressed(go);
        }
    }

    private void Refresh()
    {
        RefreshProperty();
        RefreshHero();
    }

    private void RefreshHero()
    {
        level.text = heroInfo.Lvl.ToString(CultureInfo.InvariantCulture);
        var herotemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[heroInfo.TemplateId];
        jobIcon.spriteName = HeroConstant.HeroJobPrefix + herotemplate.Job;
        nameLabel.text = herotemplate.Name;
    }

    private void RefreshProperty()
    {
        atkLabel.text = heroInfo.Prop[RoleProperties.ROLE_ATK].ToString(CultureInfo.InvariantCulture);
        hpLabel.text = heroInfo.Prop[RoleProperties.ROLE_HP].ToString(CultureInfo.InvariantCulture);
        recoverLabel.text = heroInfo.Prop[RoleProperties.ROLE_RECOVER].ToString(CultureInfo.InvariantCulture);
        mpLabel.text = heroInfo.Prop[RoleProperties.ROLE_MP].ToString(CultureInfo.InvariantCulture);
    }

    #endregion
}

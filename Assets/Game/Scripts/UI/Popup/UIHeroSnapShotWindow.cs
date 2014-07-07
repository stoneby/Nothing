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
    private UILabel atkLabel;
    private UILabel hpLabel;
    private UILabel recoverLabel;
    private UILabel mpLabel;
    private UILabel level;
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

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        cancelLis = UIEventListener.Get(transform.Find("Buttons/CancelBtn").gameObject);
        viewDetailLis = UIEventListener.Get(transform.Find("Buttons/ViewDetailBtn").gameObject);
        templateLis = UIEventListener.Get(transform.Find("Buttons/TemplateBtn").gameObject);
        var property = transform.Find("Property");
        atkLabel = property.Find("Attack/AttackValue").GetComponent<UILabel>();
        hpLabel = property.Find("HP/HPValue").GetComponent<UILabel>();
        recoverLabel = property.Find("Recover/RecoverValue").GetComponent<UILabel>();
        mpLabel = property.Find("MP/MPValue").GetComponent<UILabel>();
        level = transform.Find("Hero/Level/LevelLabel").GetComponent<UILabel>();
        jobIcon = transform.Find("Hero/Job/JobIcon").GetComponent<UISprite>();
    }

    private void InstallHandlers()
    {
        cancelLis.onClick = OnCancel;
        viewDetailLis.onClick = OnViewDetail;
        templateLis.onClick = OnTemplate;
    } 
    
    private void UnInstallHandlers()
    {
        cancelLis.onClick = null;
        viewDetailLis.onClick = null;
        templateLis.onClick = null;
    }

    private void OnCancel(GameObject go)
    {
        WindowManager.Instance.Show<UIHeroSnapShotWindow>(false);
    }

    private void OnViewDetail(GameObject go)
    {
        WindowManager.Instance.Show<UIHeroDetailWindow>(true);
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
        var herotemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
        jobIcon.spriteName = HeroConstant.HeroJobPrefix + herotemplate.Job;
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

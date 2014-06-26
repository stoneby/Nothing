using System.Collections;
using System.Globalization;
using KXSGCodec;
using Property;
using Template;
using UnityEngine;

/// <summary>
/// The window to show the detail info of single hero.
/// </summary>
public class UIHeroInfoWindow : Window
{
    #region Private Fields

    private UIEventListener backBtnLis;
    private UILabel attack;
    private UILabel hp;
    private UILabel recover;
    private UILabel mp;
    private HeroInfo heroInfo;
    private bool needShowLevelUp;
    private SCPropertyChangedNumber propertychanged;
    private HeroTemplate heroTemplate;

    #endregion

    #region Public Fields

    /// <summary>
    /// The game object which needs to be shown when click skill tab.
    /// </summary>
    public GameObject SkillContent;

    /// <summary>
    /// The game object which needs to be shown when click talent tab.
    /// </summary>
    public GameObject TalentContent;

    #endregion

    #region Window

    public override void OnEnter()
    {
        // enable finger guester.
        FingerGestures.Instance.enabled = true;

        heroInfo = HeroModelLocator.Instance.FindHero(HeroBaseInfoWindow.CurUuid);
        heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
        RefreshData();
        InstallHandlers();
    }

    public override void OnExit()
    {
        StopAllCoroutines();
        attack.color = UILevelUpWindow.NonChangedColor;
        hp.color = UILevelUpWindow.NonChangedColor;
        recover.color = UILevelUpWindow.NonChangedColor;
        mp.color = UILevelUpWindow.NonChangedColor;
        UnInstallHandlers();

        // disable finger guester.
        FingerGestures.Instance.enabled = false;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Install all handlers.
    /// </summary>
    private void InstallHandlers()
    {
        backBtnLis.onClick = OnBackBtnClicked;
    }

    /// <summary>
    /// Uninstall all handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        backBtnLis.onClick = null;
    }

    private void OnHeroPeopertyChanged(SCPropertyChangedNumber scpropertychanged)
    {
        needShowLevelUp = true;
        propertychanged = scpropertychanged;

    }

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Awake()
    {
        backBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Back").gameObject);
        var property = Utils.FindChild(transform, "Property");
        attack = Utils.FindChild(property, "AttackValue").GetComponent<UILabel>();
        hp = Utils.FindChild(property, "HPValue").GetComponent<UILabel>();
        recover = Utils.FindChild(property, "RecoverValue").GetComponent<UILabel>();
        mp = Utils.FindChild(property, "MPValue").GetComponent<UILabel>();
        CommonHandler.HeroPropertyChanged += OnHeroPeopertyChanged;
    }

    /// <summary>
    /// Update ui data.
    /// </summary>
    private void RefreshData()
    {
        attack.text = heroInfo.Prop[RoleProperties.ROLE_ATK].ToString(CultureInfo.InvariantCulture);
        hp.text = heroInfo.Prop[RoleProperties.ROLE_HP].ToString(CultureInfo.InvariantCulture);
        recover.text = heroInfo.Prop[RoleProperties.ROLE_RECOVER].ToString(CultureInfo.InvariantCulture);
        mp.text = heroInfo.Prop[RoleProperties.ROLE_MP].ToString(CultureInfo.InvariantCulture);

        var skillTmp = HeroModelLocator.Instance.SkillTemplates.SkillTmpl;
        var leaderSkillTemp = skillTmp[heroTemplate.LeaderSkill];
        var activeSkillTemp = skillTmp[heroTemplate.ActiveSkill];
        var spSkillTemp = skillTmp[heroTemplate.SpSkill];
        var activeSkill = Utils.FindChild(SkillContent.transform, "Skill-Active");
        var leaderSkill = Utils.FindChild(transform, "Skill-Leader");
        var spSkill = Utils.FindChild(SkillContent.transform, "Skill-SP");
        Utils.FindChild(activeSkill, "Name").GetComponent<UILabel>().text = activeSkillTemp.Name;
        Utils.FindChild(activeSkill, "Desc").GetComponent<UILabel>().text = activeSkillTemp.Desc;
        Utils.FindChild(activeSkill, "Cost").GetComponent<UILabel>().text = activeSkillTemp.CostMp.ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(leaderSkill, "Name").GetComponent<UILabel>().text = leaderSkillTemp.Name;
        Utils.FindChild(leaderSkill, "Desc").GetComponent<UILabel>().text = leaderSkillTemp.Desc;
        Utils.FindChild(spSkill, "Name").GetComponent<UILabel>().text = spSkillTemp.Name;
        Utils.FindChild(spSkill, "Desc").GetComponent<UILabel>().text = spSkillTemp.Desc;
        Utils.FindChild(spSkill, "LV-Value").GetComponent<UILabel>().text = (spSkillTemp.Id % 10).ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(spSkill, "Probability-Value").GetComponent<UILabel>().text = spSkillTemp.OccorRate + "%";

        if(skillTmp.ContainsKey(heroTemplate.PassiveSkill1))
        {
            NGUITools.SetActive(TalentContent, true);
            var passiveSkill = skillTmp[heroTemplate.PassiveSkill1];
            var skillOne = Utils.FindChild(TalentContent.transform, "SP-SkillOne");
            Utils.FindChild(skillOne, "Name").GetComponent<UILabel>().text = passiveSkill.Name;
            Utils.FindChild(skillOne, "Desc").GetComponent<UILabel>().text = passiveSkill.Desc;
            NGUITools.SetActive(TalentContent, false);
        }
        if(needShowLevelUp)
        {
            StartCoroutine("ShowLevelUpEffect", propertychanged);
            needShowLevelUp = false;
        }
    }

    public void RefreshData(HeroInfo info)
    {
        heroInfo = info;
        HeroBaseInfoWindow.CurUuid = info.Uuid;
        heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
        RefreshData();
    }

    /// <summary>
    /// The callback of clicking back button.
    /// </summary>
    private void OnBackBtnClicked(GameObject go)
    {
        WindowManager.Instance.Show(typeof(UIHerosDisplayWindow), true);
        WindowManager.Instance.Show(typeof(UIHeroInfoWindow), false);
    }

    /// <summary>
    /// The coroutine to show the effect of the result of level up.
    /// </summary>
    private IEnumerator ShowLevelUpEffect(object obj)
    {
        var changedNumber = obj as SCPropertyChangedNumber;
        if(changedNumber != null)
        {
            var lvlBefore = heroInfo.Lvl;
            var atkBefore = heroInfo.Prop[RoleProperties.ROLE_ATK];
            var hpBefore = heroInfo.Prop[RoleProperties.ROLE_HP];
            var recoverBefore = heroInfo.Prop[RoleProperties.ROLE_RECOVER];
            var mpBefore = heroInfo.Prop[RoleProperties.ROLE_MP];

            SetHeroInfo(changedNumber);
            var changedLvl = heroInfo.Lvl - lvlBefore;
            var changedAtk = changedNumber.PropertyChanged[RoleProperties.ROLE_ATK] - atkBefore;
            var changedHp = changedNumber.PropertyChanged[RoleProperties.ROLE_HP] - hpBefore;
            var changedRecover = changedNumber.PropertyChanged[RoleProperties.ROLE_RECOVER] - recoverBefore;
            var changedMp = changedNumber.PropertyChanged[RoleProperties.ROLE_MP] - mpBefore;

            var heroBase = WindowManager.Instance.GetWindow<HeroBaseInfoWindow>();
            var lvLabel = Utils.FindChild(heroBase.transform, "LV-Value").GetComponent<UILabel>();

            while (true)
            {
                ShowNormalLabels(lvLabel);
                yield return new WaitForSeconds(1f);
                SetLevelUpLevels(lvLabel, changedLvl, changedAtk, changedHp, changedRecover, changedMp);
                yield return new WaitForSeconds(1f);
            }
        }   
    }

    private void SetLevelUpLevels(UILabel lvLabel, int changedLvl, int changedAtk, int changedHp, int changedRecover,
                                  int changedMp)
    {
        lvLabel.color = UILevelUpWindow.ChangedColor;
        lvLabel.text = string.Format("(+{0})", changedLvl);
        attack.color = UILevelUpWindow.ChangedColor;
        attack.text = string.Format("(+{0})", changedAtk);
        hp.color = UILevelUpWindow.ChangedColor;
        hp.text = string.Format("(+{0})", changedHp);
        recover.color = UILevelUpWindow.ChangedColor;
        recover.text = string.Format("(+{0})", changedRecover);
        mp.color = UILevelUpWindow.ChangedColor;
        mp.text = string.Format("(+{0})", changedMp);
    }

    private void ShowNormalLabels(UILabel lvLabel)
    {
        lvLabel.color = UILevelUpWindow.NonChangedColor;
        lvLabel.text = string.Format("{0}/{1}", heroInfo.Lvl, heroTemplate.LvlLimit);
        attack.color = UILevelUpWindow.NonChangedColor;
        attack.text = heroInfo.Prop[RoleProperties.ROLE_ATK].ToString(CultureInfo.InvariantCulture);
        hp.color = UILevelUpWindow.NonChangedColor;
        hp.text = heroInfo.Prop[RoleProperties.ROLE_HP].ToString(CultureInfo.InvariantCulture);
        recover.color = UILevelUpWindow.NonChangedColor;
        recover.text = heroInfo.Prop[RoleProperties.ROLE_RECOVER].ToString(CultureInfo.InvariantCulture);
        mp.color = UILevelUpWindow.NonChangedColor;
        mp.text = heroInfo.Prop[RoleProperties.ROLE_MP].ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Set the hero info with special key.
    /// </summary>
    /// <param name="changedNumber">The property changed number.</param>
    private void SetHeroInfo(SCPropertyChangedNumber changedNumber)
    {
        if (changedNumber.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_LEVEL))
        {
            heroInfo.Lvl = (short)changedNumber.PropertyChanged[RoleProperties.ROLEBASE_LEVEL];
        }
        if (changedNumber.PropertyChanged.ContainsKey(RoleProperties.ROLE_ATK))
        {
            heroInfo.Prop[RoleProperties.ROLE_ATK] = changedNumber.PropertyChanged[RoleProperties.ROLE_ATK];
        }
        if (changedNumber.PropertyChanged.ContainsKey(RoleProperties.ROLE_HP))
        {
            heroInfo.Prop[RoleProperties.ROLE_ATK] = changedNumber.PropertyChanged[RoleProperties.ROLE_HP];
        }
        if (changedNumber.PropertyChanged.ContainsKey(RoleProperties.ROLE_RECOVER))
        {
            heroInfo.Prop[RoleProperties.ROLE_ATK] = changedNumber.PropertyChanged[RoleProperties.ROLE_RECOVER];
        }
        if (changedNumber.PropertyChanged.ContainsKey(RoleProperties.ROLE_MP))
        {
            heroInfo.Prop[RoleProperties.ROLE_ATK] = changedNumber.PropertyChanged[RoleProperties.ROLE_MP];
        }
    }

    #endregion
}

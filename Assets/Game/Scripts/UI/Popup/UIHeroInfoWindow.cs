using System.Collections;
using System.Globalization;
using System.Linq;
using KXSGCodec;
using Property;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIHeroInfoWindow : Window
{
    #region Private Fields

    private UIEventListener skillBtnLis;
    private UIEventListener lvBtnLis;
    private UIEventListener limitBtnLis;
    private UIEventListener backBtnLis;

    private UIEventListener skillTabLis;
    private UIEventListener talentTabLis;

    public GameObject StarPrefab;
    public GameObject LimitFillPrefab;
    public GameObject LimitEmptyPrefab;
    private GameObject skillContent;
    private GameObject talentContent;
    private GameObject stars;
    private GameObject limitSymbols;

    private UILabel attack;
    private UILabel hp;
    private UILabel recover;
    private UILabel mp;

    private const string NormalTabSprite = "TabN";
    private const string BtnDownTabSprite = "TabD";

    private HeroInfo heroInfo;
    private HeroTemplate heroTemplate;
    private UISprite skillTab;
    private UISprite talentTab;

    #endregion

    #region Public Fields

    public static long Uuid;

    #endregion

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
        heroInfo = HeroModelLocator.Instance.FindHero(Uuid);
        heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
        RefreshData();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        StopAllCoroutines();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    void Awake()
    {
        skillBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Skill").gameObject);
        lvBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-LV").gameObject);
        limitBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Limit").gameObject);
        backBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Back").gameObject);
        skillTab = Utils.FindChild(transform, "SkillTab").GetComponent<UISprite>();
        talentTab = Utils.FindChild(transform, "TalentTab").GetComponent<UISprite>();
        skillTabLis = UIEventListener.Get(skillTab.gameObject);
        talentTabLis = UIEventListener.Get(talentTab.gameObject);
        skillContent = skillTabLis.transform.FindChild("SkillContent").gameObject;
        talentContent = talentTabLis.transform.FindChild("TalentContent").gameObject;
        stars = Utils.FindChild(transform, "Stars").gameObject;
        limitSymbols = Utils.FindChild(transform, "LimitSymbols").gameObject;
        var property = Utils.FindChild(transform, "Property");
        attack = Utils.FindChild(property, "AttackValue").GetComponent<UILabel>();
        hp = Utils.FindChild(property, "HPValue").GetComponent<UILabel>();
        recover = Utils.FindChild(property, "RecoverValue").GetComponent<UILabel>();
        mp = Utils.FindChild(property, "MPValue").GetComponent<UILabel>();
    }


    private void RefreshData()
    {
        RefreshBaseInfoData();
        RefreshPropertyData();
        RefreshSkillData();
        //This is for demo.
        NGUITools.SetActiveChildren(talentContent, true);
        RefreshTalentData();
        NGUITools.SetActiveChildren(talentContent, false);
    }

    private void RefreshBaseInfoData()
     {
         var baseInfo = Utils.FindChild(transform, "BaseInfo");
         Utils.FindChild(baseInfo, "Name").GetComponent<UILabel>().text = heroTemplate.Name;
         var spriteWidth = StarPrefab.GetComponent<UISprite>().width;
         for (int index = 0; index < heroTemplate.Star; index++)
         {
             var starObj = NGUITools.AddChild(stars, StarPrefab);
             starObj.transform.localPosition = new Vector3((index + 1) * spriteWidth, 0, 0);
         }
         Utils.FindChild(baseInfo, "LV-Value").GetComponent<UILabel>().text = string.Format("{0}/{1}", heroInfo.Lvl, heroTemplate.LvlLimit);
         spriteWidth = LimitFillPrefab.GetComponent<UISprite>().width;
         for (int index = 0; index < heroTemplate.BreakLimit; index++)
         {
             var fill = NGUITools.AddChild(limitSymbols, index < heroInfo.BreakTimes ? LimitFillPrefab : LimitEmptyPrefab);
             fill.transform.localPosition = new Vector3(index * spriteWidth, 0, 0);
         }
         Utils.FindChild(baseInfo, "Luck-Value").GetComponent<UILabel>().text = "20";
         Utils.FindChild(baseInfo, "Job-Value").GetComponent<UISprite>().spriteName = UIHerosDisplayWindow.JobPrefix + heroTemplate.Job;
     }

    private void RefreshPropertyData()
    {
        attack.text = heroInfo.Prop[RoleProperties.HERO_ATK].ToString(CultureInfo.InvariantCulture);
        hp.text = heroInfo.Prop[RoleProperties.HERO_HP].ToString(CultureInfo.InvariantCulture);
        recover.text = heroInfo.Prop[RoleProperties.HERO_RECOVER].ToString(CultureInfo.InvariantCulture);
        mp.text = heroInfo.Prop[RoleProperties.HERO_MP].ToString(CultureInfo.InvariantCulture);
    }

    private void RefreshSkillData()
    {
        var skillTmp = HeroModelLocator.Instance.SkillTemplates.SkillTmpl;
        var leaderSkillTemp = skillTmp[heroTemplate.LeaderSkill];
        var activeSkillTemp = skillTmp[heroTemplate.ActiveSkill];
        var spSkillTemp = skillTmp[heroTemplate.SpSkill];

        var activeSkill = Utils.FindChild(transform, "Skill-Active");
        var leaderSkill = Utils.FindChild(transform, "Skill-Leader");
        var spSkill = Utils.FindChild(transform, "Skill-SP");
        Utils.FindChild(activeSkill, "Name").GetComponent<UILabel>().text = activeSkillTemp.Name;
        Utils.FindChild(activeSkill, "Desc").GetComponent<UILabel>().text = activeSkillTemp.Desc;
        Utils.FindChild(activeSkill, "Cost").GetComponent<UILabel>().text = activeSkillTemp.CostMp.ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(leaderSkill, "Name").GetComponent<UILabel>().text = leaderSkillTemp.Name;
        Utils.FindChild(leaderSkill, "Desc").GetComponent<UILabel>().text = leaderSkillTemp.Desc;
        Utils.FindChild(spSkill, "Name").GetComponent<UILabel>().text = spSkillTemp.Name;
        Utils.FindChild(spSkill, "Desc").GetComponent<UILabel>().text = spSkillTemp.Desc;
        Utils.FindChild(spSkill, "LV-Value").GetComponent<UILabel>().text = (spSkillTemp.Id % 10).ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(spSkill, "Probability-Value").GetComponent<UILabel>().text = spSkillTemp.OccorRate + "%";
    }

    private void RefreshTalentData()
    {
        var skillTmp = HeroModelLocator.Instance.SkillTemplates.SkillTmpl;
        var passiveSkill = skillTmp[heroTemplate.PassiveSkill1];

        var skillOne = Utils.FindChild(talentContent.transform, "SP-SkillOne");
        Utils.FindChild(skillOne, "Name").GetComponent<UILabel>().text = passiveSkill.Name;
        Utils.FindChild(skillOne, "Desc").GetComponent<UILabel>().text = passiveSkill.Desc;   
    }

    private void InstallHandlers()
    {
        skillBtnLis.onClick += OnSkillBtnClicked;
        lvBtnLis.onClick += OnLvBtnClicked;
        limitBtnLis.onClick += OnLimitBtnClicked;
        backBtnLis.onClick += OnBackBtnClicked;
        skillTabLis.onClick += OnTabClicked;
        talentTabLis.onClick += OnTabClicked;
    }

    private void UnInstallHandlers()
    {
        skillBtnLis.onClick -= OnSkillBtnClicked;
        lvBtnLis.onClick -= OnLvBtnClicked;
        limitBtnLis.onClick -= OnLimitBtnClicked;
        backBtnLis.onClick -= OnBackBtnClicked;
        skillTabLis.onClick -= OnTabClicked;
        talentTabLis.onClick -= OnTabClicked;
    }

    private void OnBackBtnClicked(GameObject go)
    {
        CleanUp();
        gameObject.SetActive(false);
    }

    private void OnLimitBtnClicked(GameObject go)
    {

    }

    private void OnLvBtnClicked(GameObject go)
    {
        WindowManager.Instance.Show(typeof(UILevelUpWindow), true);
    }

    private void OnSkillBtnClicked(GameObject go)
    {

    }

    private void OnTabClicked(GameObject go)
    {
        var isSkillTab = (go == skillTab.gameObject);
        skillTab.spriteName = isSkillTab ? BtnDownTabSprite : NormalTabSprite;
        talentTab.spriteName = isSkillTab ? NormalTabSprite : BtnDownTabSprite; ;
        NGUITools.SetActiveChildren(skillContent, isSkillTab);
        NGUITools.SetActiveChildren(talentContent, !isSkillTab);
    }

    /// <summary>
    /// Do some clean up work before leaving this window.
    /// </summary>
    private void CleanUp()
    {
        var list = stars.transform.Cast<Transform>().ToList();
        for (int index = list.Count - 1; index >= 0; index--)
        {
            Destroy(list[index].gameObject);
        }
        list = limitSymbols.transform.Cast<Transform>().ToList();
        for (int index = list.Count - 1; index >= 0; index--)
        {
            Destroy(list[index].gameObject);
        }
    }

    public void ShowLevelUp(SCPropertyChangedNumber changedNumber)
    {
        StartCoroutine("ShowLevelUpEffect", changedNumber);
    }

    private IEnumerator ShowLevelUpEffect(object obj)
    {
        var changedNumber = obj as SCPropertyChangedNumber;

        var lvlBefore = heroInfo.Lvl;
        var atkBefore = heroInfo.Prop[RoleProperties.HERO_ATK];
        var hpBefore = heroInfo.Prop[RoleProperties.HERO_HP];
        var recoverBefore = heroInfo.Prop[RoleProperties.HERO_RECOVER];
        var mpBefore = heroInfo.Prop[RoleProperties.HERO_MP];

        heroInfo.Lvl = (short)changedNumber.PropertyChanged[RoleProperties.ROLEBASE_LEVEL];
        heroInfo.Prop[RoleProperties.HERO_ATK] = changedNumber.PropertyChanged[RoleProperties.HERO_ATK];
        heroInfo.Prop[RoleProperties.HERO_HP] = changedNumber.PropertyChanged[RoleProperties.HERO_HP];
        heroInfo.Prop[RoleProperties.HERO_RECOVER] = changedNumber.PropertyChanged[RoleProperties.HERO_RECOVER];
        heroInfo.Prop[RoleProperties.HERO_MP] = changedNumber.PropertyChanged[RoleProperties.HERO_MP];

        var changedLvl = heroInfo.Lvl - lvlBefore;
        var changedATK = changedNumber.PropertyChanged[RoleProperties.HERO_ATK] - atkBefore;
        var changedHp = changedNumber.PropertyChanged[RoleProperties.HERO_HP] - hpBefore;
        var changedRecover = changedNumber.PropertyChanged[RoleProperties.HERO_RECOVER] - recoverBefore;
        var changedMp = changedNumber.PropertyChanged[RoleProperties.HERO_MP] - mpBefore;

        var baseInfo = Utils.FindChild(transform, "BaseInfo");
        var lvLabel = Utils.FindChild(baseInfo, "LV-Value").GetComponent<UILabel>();

        while (true)
        {
            lvLabel.color = UILevelUpWindow.NonChangedColor;
            lvLabel.text = string.Format("{0}/{1}", heroInfo.Lvl, heroTemplate.LvlLimit);
            attack.color = UILevelUpWindow.NonChangedColor;
            attack.text = heroInfo.Prop[RoleProperties.HERO_ATK].ToString();
            hp.color = UILevelUpWindow.NonChangedColor;
            hp.text = heroInfo.Prop[RoleProperties.HERO_HP].ToString();
            recover.color = UILevelUpWindow.NonChangedColor;
            recover.text = heroInfo.Prop[RoleProperties.HERO_RECOVER].ToString();
            mp.color = UILevelUpWindow.NonChangedColor;
            mp.text = heroInfo.Prop[RoleProperties.HERO_MP].ToString();
            yield return new WaitForSeconds(1f);
            lvLabel.color = UILevelUpWindow.ChangedColor;
            lvLabel.text = string.Format("(+{0})", changedLvl);
            attack.color = UILevelUpWindow.ChangedColor;
            attack.text = string.Format("(+{0})", changedATK);
            hp.color = UILevelUpWindow.ChangedColor;
            hp.text = string.Format("(+{0})", changedHp);
            recover.color = UILevelUpWindow.ChangedColor;
            recover.text = string.Format("(+{0})", changedRecover);
            mp.color = UILevelUpWindow.ChangedColor;
            mp.text = string.Format("(+{0})", changedMp);
            yield return new WaitForSeconds(1f);
        }
    }

    #endregion
}

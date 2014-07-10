using System.Globalization;
using KXSGCodec;
using Property;
using Template;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIHeroDetailWindow : Window
{
    private HeroInfo heroInfo;
    private HeroTemplate heroTemplate;
    private UILabel attack;
    private UILabel hp;
    private UILabel recover;
    private UILabel mp;
    private UILabel activeSkillName;
    private UILabel activeSkillDesc; 
    private UILabel leaderSkillName;
    private UILabel leaderSkillDesc;

    #region Window

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Awake()
    {
        var property = Utils.FindChild(transform, "Property");
        attack = Utils.FindChild(property, "AttackValue").GetComponent<UILabel>();
        hp = Utils.FindChild(property, "HPValue").GetComponent<UILabel>();
        recover = Utils.FindChild(property, "RecoverValue").GetComponent<UILabel>();
        mp = Utils.FindChild(property, "MPValue").GetComponent<UILabel>();
        var activeSkill = Utils.FindChild(transform, "ActiveSkill");
        activeSkillName = activeSkill.Find("Name").GetComponent<UILabel>();
        activeSkillDesc = activeSkill.Find("Desc").GetComponent<UILabel>();
        var leaderSkill = Utils.FindChild(transform, "LeaderSkill");
        leaderSkillName = leaderSkill.Find("Name").GetComponent<UILabel>();
        leaderSkillDesc = leaderSkill.Find("Desc").GetComponent<UILabel>();
    }

    #endregion

    public void RefreshData(HeroInfo info)
    {
        heroInfo = info;
        HeroBaseInfoWindow.CurUuid = info.Uuid;
        heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
        RefreshData();
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
        if (skillTmp.ContainsKey(heroTemplate.SpSkill))
        {
            var activeSkillTemp = skillTmp[heroTemplate.ActiveSkill];
            activeSkillName.text = activeSkillTemp.Name;
            activeSkillDesc.text = activeSkillTemp.Desc;
        }

        if (skillTmp.ContainsKey(heroTemplate.LeaderSkill))
        {
            var leaderSkillTemp = skillTmp[heroTemplate.LeaderSkill];
            leaderSkillName.text = leaderSkillTemp.Name;
            leaderSkillDesc.text = leaderSkillTemp.Desc;
        }
    }
}

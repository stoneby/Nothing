using Template.Auto.Hero;
using Template.Auto.Skill;
using UnityEngine;

public class HeroSkillUpdater : MonoBehaviour
{
    public UILabel ActiveName;
    public UILabel ActiveDesc;

    public UILabel LeaderName;
    public UILabel LeaderDesc;

    public void RefreshSkills(HeroTemplate heroTemplate)
    {
        var skillTmp = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls;
        HeroBattleSkillTemplate skillTemp;
        skillTmp.TryGetValue(heroTemplate.ActiveSkill, out skillTemp);
        var contains = skillTemp != null;
        ActiveName.text = contains ? skillTemp.Name : "-";
        ActiveDesc.text = contains ? skillTemp.Desc : "-";
        skillTmp.TryGetValue(heroTemplate.LeaderSkill, out skillTemp);
        contains = skillTemp != null;
        LeaderName.text = contains ? skillTemp.Name : "-";
        LeaderDesc.text = contains ? skillTemp.Desc : "-";
    }
}

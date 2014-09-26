using KXSGCodec;
using Property;
using UnityEngine;

public class HeroDetailInfoControl : MonoBehaviour
{
    public HeroBaseInfoRefresher BaseInfoRefresher;
    public PropertyUpdater PropertyUpdater;
    public HeroSkillUpdater HeroSkillUpdater;
    public HeroItemBase HeroItemBase;

    public void Refresh(HeroInfo heroInfo)
    {
        var template = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[heroInfo.TemplateId];
        if (BaseInfoRefresher)
        {
            BaseInfoRefresher.Refresh(heroInfo);
        }
        if (HeroItemBase)
        {
            HeroItemBase.InitItem(heroInfo);
        }
        if(PropertyUpdater)
        {
            PropertyUpdater.UpdateProperty(heroInfo.Lvl,
                                           template.LvlLimit,
                                           heroInfo.Prop[RoleProperties.ROLE_ATK],
                                           heroInfo.Prop[RoleProperties.ROLE_HP],
                                           heroInfo.Prop[RoleProperties.ROLE_RECOVER],
                                           heroInfo.Prop[RoleProperties.ROLE_MP]);
        }
        HeroSkillUpdater.RefreshSkills(template);
    }
}

using System.Globalization;
using KXSGCodec;
using Property;
using UnityEngine;
using System.Collections;

public class TeamHeroItem : HeroItemBase
{
    private UILabel lvLabel;
    private UILabel attackLabel;
    private UILabel hpLabel;

    protected override void Awake()
    {
        base.Awake();
        lvLabel = Utils.FindChild(transform, "LV-Value").GetComponent<UILabel>();
        attackLabel = Utils.FindChild(transform, "Attack-Value").GetComponent<UILabel>();
        hpLabel = Utils.FindChild(transform, "HP-Value").GetComponent<UILabel>();
    }

    public override void InitItem(HeroInfo heroInfo)
    {
        base.InitItem(heroInfo);
        lvLabel.text = heroInfo.Lvl.ToString(CultureInfo.InvariantCulture);
        attackLabel.text = heroInfo.Prop[RoleProperties.ROLE_ATK].ToString(CultureInfo.InvariantCulture);
        hpLabel.text = heroInfo.Prop[RoleProperties.ROLE_ATK].ToString(CultureInfo.InvariantCulture);
    }
}
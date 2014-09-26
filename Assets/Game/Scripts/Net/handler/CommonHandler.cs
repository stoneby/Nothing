using KXSGCodec;
using Property;

public class CommonHandler
{
    public delegate void PropertyChangedNumber(SCPropertyChangedNumber scPropertyChanged);
    public static PropertyChangedNumber PlayerPropertyChanged;
    public static PropertyChangedNumber HeroPropertyChanged;

    public static void OnPropertyChangedNumber(ThriftSCMessage msg)
    {
        var propertyChangedMsg = msg.GetContent() as SCPropertyChangedNumber;
        if (propertyChangedMsg != null)
        {
            if (propertyChangedMsg.RoleType == 2)
            {
                var uuid = propertyChangedMsg.Uuid;
                var hero = HeroModelLocator.Instance.FindHero(uuid);
                if(propertyChangedMsg.PropertyChanged.ContainsKey(RoleProperties.ROLE_ATK))
                {
                    hero.Prop[RoleProperties.ROLE_ATK] = propertyChangedMsg.PropertyChanged[RoleProperties.ROLE_ATK];
                }
                if (propertyChangedMsg.PropertyChanged.ContainsKey(RoleProperties.ROLE_HP))
                {
                    hero.Prop[RoleProperties.ROLE_HP] = propertyChangedMsg.PropertyChanged[RoleProperties.ROLE_HP];
                }
                if (propertyChangedMsg.PropertyChanged.ContainsKey(RoleProperties.ROLE_RECOVER))
                {
                    hero.Prop[RoleProperties.ROLE_RECOVER] = propertyChangedMsg.PropertyChanged[RoleProperties.ROLE_RECOVER];
                } 
                if(propertyChangedMsg.PropertyChanged.ContainsKey(RoleProperties.ROLE_MP))
                {
                    hero.Prop[RoleProperties.ROLE_MP] = propertyChangedMsg.PropertyChanged[RoleProperties.ROLE_MP];
                }
                if (propertyChangedMsg.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_LEVEL))
                {
                    hero.Lvl = (short)propertyChangedMsg.PropertyChanged[RoleProperties.ROLEBASE_LEVEL];
                }
                var playerModel = PlayerModelLocator.Instance;
                var isInTeam = TeamMemberManager.Instance.CurTeam.Contains(uuid);
                if(isInTeam)
                {
                    foreach (var key in HeroConstant.PropKeys)
                    {
                        playerModel.TeamProp[key] += (propertyChangedMsg.PropertyChanged[key] - hero.Prop[key]);
                    }
                }
                if(HeroPropertyChanged != null)
                {
                    HeroPropertyChanged(propertyChangedMsg);
                }
            }
            if (propertyChangedMsg.RoleType == 1)
            {
                if (propertyChangedMsg.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_DIAMOND))
                {
                    PlayerModelLocator.Instance.Diamond =
                        propertyChangedMsg.PropertyChanged[RoleProperties.ROLEBASE_DIAMOND];
                }
                if (propertyChangedMsg.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_HERO_SPIRIT))
                {
                    PlayerModelLocator.Instance.Sprit =
                        propertyChangedMsg.PropertyChanged[RoleProperties.ROLEBASE_HERO_SPIRIT];
                }
                if (propertyChangedMsg.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_GOLD))
                {
                    PlayerModelLocator.Instance.Gold = propertyChangedMsg.PropertyChanged[RoleProperties.ROLEBASE_GOLD];
                }

                if (propertyChangedMsg.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_CURRENT_EXP))
                {
                    PlayerModelLocator.Instance.Exp =
                        propertyChangedMsg.PropertyChanged[RoleProperties.ROLEBASE_CURRENT_EXP];
                }
                if (propertyChangedMsg.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_ENERGY))
                {
                    EnergyIncreaseControl.Instance.Energy = propertyChangedMsg.PropertyChanged[RoleProperties.ROLEBASE_ENERGY];
                }
                if (propertyChangedMsg.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_EXTEND_HREO_BAG_TIMES))
                {
                    PlayerModelLocator.Instance.ExtendHeroTimes =
                        propertyChangedMsg.PropertyChanged[RoleProperties.ROLEBASE_EXTEND_HREO_BAG_TIMES];
                }
                if (propertyChangedMsg.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_EXTEND_ITEM_BAG_TIMES))
                {
                    PlayerModelLocator.Instance.ExtendItemTimes =
                        propertyChangedMsg.PropertyChanged[RoleProperties.ROLEBASE_EXTEND_ITEM_BAG_TIMES];
                }

                if (propertyChangedMsg.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_LEVEL))
                {
                    PlayerModelLocator.Instance.Level =
                        (short)propertyChangedMsg.PropertyChanged[RoleProperties.ROLEBASE_LEVEL];
                }

                if (propertyChangedMsg.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_FAMOUS))
                {
                    PlayerModelLocator.Instance.Famous =
                        propertyChangedMsg.PropertyChanged[RoleProperties.ROLEBASE_FAMOUS];
                }

                if (propertyChangedMsg.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_SUPER_CHIP))
                {
                    PlayerModelLocator.Instance.SuperChip =
                        propertyChangedMsg.PropertyChanged[RoleProperties.ROLEBASE_SUPER_CHIP];
                }
                if (PlayerPropertyChanged != null)
                {
                    PlayerPropertyChanged(propertyChangedMsg);
                }
            }
        }    
    }
}

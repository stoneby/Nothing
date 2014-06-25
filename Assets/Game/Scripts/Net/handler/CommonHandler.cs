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
                    PlayerModelLocator.Instance.Energy =
                        propertyChangedMsg.PropertyChanged[RoleProperties.ROLEBASE_ENERGY];
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

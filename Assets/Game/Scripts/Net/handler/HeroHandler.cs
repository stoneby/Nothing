using KXSGCodec;
using Property;
using UnityEngine;

namespace Assets.Game.Scripts.Net.handler
{
    public class HeroHandler
    {
        public static void OnHeroMessage(ThriftSCMessage msg)
        {
            switch (msg.GetMsgType())
            {
                case (short)MessageType.SC_HERO_LIST:
                    HeroModelLocator.Instance.SCHeroList = msg.GetContent() as SCHeroList;
                    HeroModelLocator.AlreadyRequest = true;
                    if (HeroModelLocator.Instance.GetHeroPos == RaidType.GetHeroInBattle)
                    {
                        WindowManager.Instance.Show(typeof(BattleConfirmTabWindow), true);
                    }
                    else if (HeroModelLocator.Instance.GetHeroPos == RaidType.GetHeroInHeroPanel)
                    {
                        Utils.ShowWithoutDestory(typeof(UIHerosDisplayWindow));
                    }
                    else if (HeroModelLocator.Instance.GetHeroPos == RaidType.GetHeroInHeroCreateTeam)
                    {
                        Utils.ShowWithoutDestory(typeof(UITeamShowingWindow));
                    }
                    break;

                case (short)MessageType.SC_HERO_CREATE_ONE:
                    var createOneMsg = msg.GetContent() as SCHeroCreateOne;
                    HeroModelLocator.Instance.SCHeroList.HeroList.Add(createOneMsg.NewHero);
                    WindowManager.Instance.GetWindow<UIHeroItemsPageWindow>().Refresh();
                    break;
                case (short)MessageType.SC_HERO_MODIFY_TEAM:
                    var md5Msg = msg.GetContent() as SCHeroModifyTeam;
                    HeroModelLocator.Instance.SCHeroList.TeamList[md5Msg.TeamIndex].ListHeroUuid = md5Msg.NewTeamInfo;
                    WindowManager.Instance.GetWindow<UITeamBuildWindow>().Refresh();
                    WindowManager.Instance.Show(typeof(UITeamEditWindow), false);
                    break;

                case (short)MessageType.SC_HERO_SELL:
                    var sellList = (msg.GetContent() as SCHeroSell).SellList;
                    var heroList = HeroModelLocator.Instance.SCHeroList.HeroList;
                    for (var index = 0; index < heroList.Count; index++)
                    {
                        if (sellList.Contains(heroList[index].Uuid))
                        {
                            heroList.Remove(heroList[index]);
                            index--;
                        }
                    }
                    WindowManager.Instance.GetWindow<UIHeroSellWindow>().SellOverUpdate();
                    break;

                case (short)MessageType.SC_HERO_LVL_UP:
                    WindowManager.Instance.GetWindow<UILevelUpWindow>().ShowLevelOver();
                    break;

                case (short)MessageType.SC_PROPERTY_CHANGED_NUMBER:
                    var propertyChangedMsg = msg.GetContent() as SCPropertyChangedNumber;
                    if (propertyChangedMsg.RoleType == 2)
                    {
                        WindowManager.Instance.GetWindow<UILevelUpWindow>().PropertyChangedNumber
                            = msg.GetContent() as SCPropertyChangedNumber;
                    }
                    if (propertyChangedMsg.RoleType == 1)
                    {
                        var propertyChangedNumber = msg.GetContent() as SCPropertyChangedNumber;

                        if (propertyChangedNumber.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_DIAMOND))
                        {
                            PlayerModelLocator.Instance.Diamond = propertyChangedNumber.PropertyChanged[RoleProperties.ROLEBASE_DIAMOND];
                        }
                        if (propertyChangedNumber.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_HERO_SPIRIT))
                        {
                            PlayerModelLocator.Instance.Sprit = propertyChangedNumber.PropertyChanged[RoleProperties.ROLEBASE_HERO_SPIRIT];
                        }
                        if (propertyChangedNumber.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_GOLD))
                        {
                            PlayerModelLocator.Instance.Gold = propertyChangedNumber.PropertyChanged[RoleProperties.ROLEBASE_GOLD];
                        }

                        if (propertyChangedNumber.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_CURRENT_EXP))
                        {
                            PlayerModelLocator.Instance.Exp = propertyChangedNumber.PropertyChanged[RoleProperties.ROLEBASE_CURRENT_EXP];
                        }
                        if (propertyChangedNumber.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_ENERGY))
                        {
                            PlayerModelLocator.Instance.Energy = propertyChangedNumber.PropertyChanged[RoleProperties.ROLEBASE_ENERGY];
                        }
                        if (propertyChangedNumber.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_EXTEND_HREO_BAG_TIMES))
                        {
                            PlayerModelLocator.Instance.ExtendHeroTimes = propertyChangedNumber.PropertyChanged[RoleProperties.ROLEBASE_EXTEND_HREO_BAG_TIMES];
                        }
                        if (propertyChangedNumber.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_EXTEND_ITEM_BAG_TIMES))
                        {
                            PlayerModelLocator.Instance.ExtendItemTimes = propertyChangedNumber.PropertyChanged[RoleProperties.ROLEBASE_EXTEND_ITEM_BAG_TIMES];
                        }

                        if (propertyChangedNumber.PropertyChanged.ContainsKey(RoleProperties.ROLEBASE_LEVEL))
                        {
                            PlayerModelLocator.Instance.Level = (short) propertyChangedNumber.PropertyChanged[RoleProperties.ROLEBASE_LEVEL];
                        }

                        var mainWindow = WindowManager.Instance.GetWindow<UIMainScreenWindow>();//.Show(typeof(UIMainScreenWindow), true);
                        mainWindow.GetComponent<UIMainScreenWindow>().refreshData();
                    }
                    break;
                case (short)MessageType.SC_HERO_MAX_EXTEND:
                    var themsg = msg.GetContent() as SCHeroMaxExtend;
                    if (themsg != null)
                    {
                        PlayerModelLocator.Instance.HeroMax = themsg.RefreshHeroCountLimit;
                        WindowManager.Instance.GetWindow<UIHeroItemsPageWindow>().RefreshHeroCount();
                    }
                    break;
                case (short)MessageType.SC_HERO_CHANGE_EQUIP:
                    var hChangeEquipMsg = msg.GetContent() as SCHeroChangeEquip;
                    if (hChangeEquipMsg != null)
                    {
                        var heroBaseWindow = WindowManager.Instance.GetWindow<HeroBaseInfoWindow>();
                        heroBaseWindow.HeroInfo.EquipUuid[heroBaseWindow.CurEquipIndex] = hChangeEquipMsg.EquipUuid;
                        WindowManager.Instance.Show<UIHeroSelItemWindow>(false);
                        WindowManager.Instance.Show<HeroBaseInfoWindow>(true);
                        WindowManager.Instance.Show<UIHeroInfoWindow>(true);
                        WindowManager.Instance.Show<UItemsWindow>(false);
                    }
                    break;
            }
        }
    }
}



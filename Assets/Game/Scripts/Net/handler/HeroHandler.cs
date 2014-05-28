using KXSGCodec;
using Property;

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
                            PlayerModelLocator.Instance.Sprit = propertyChangedNumber.PropertyChanged[RoleProperties.ROLEBASE_GOLD];
                        }
                        var mainWindow = WindowManager.Instance.Show(typeof(UIMainScreenWindow), true);
                        mainWindow.GetComponent<UIMainScreenWindow>().refreshData();
                    }
                    break;
            }
        }
    }
}



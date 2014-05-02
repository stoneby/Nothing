using KXSGCodec;

namespace Assets.Game.Scripts.Net.handler
{
    public class HeroHandler
    {
        public static void OnHeroMessage(ThriftSCMessage msg)
        {
            switch (msg.GetMsgType())
            {
                case (short)MessageType.SC_HERO_LIST:
                    HeroModelLocator.Instance.SCHeroList = msg.getContent() as SCHeroList;
                    WindowManager.Instance.Show(typeof(UIHerosDisplayWindow), true);
                    break;
                case (short)MessageType.SC_HERO_MODIFY_TEAM:
                    var md5msg = msg.getContent() as SCHeroModifyTeam;
                    WindowManager.Instance.GetWindow<UITeamBuildWindow>(typeof(UITeamBuildWindow)).Refresh(
                        md5msg.TeamIndex, md5msg.NewTeamInfo);
                    HeroModelLocator.Instance.SCHeroList.TeamList[md5msg.TeamIndex].ListHeroUuid = md5msg.NewTeamInfo;
                    WindowManager.Instance.Show(typeof(UITeamEditWindow), false);
                    break;
                case (short)MessageType.SC_HERO_SELL:
                    var sellList = (msg.getContent() as SCHeroSell).SellList;
                    var heroList = HeroModelLocator.Instance.SCHeroList.HeroList;
                    for (int index = 0; index < heroList.Count; index++)
                    {
                        if(sellList.Contains(heroList[index].Uuid))
                        {
                            heroList.Remove(heroList[index]);
                            index--;
                        }
                    }
                    WindowManager.Instance.GetWindow<UIHeroSellWindow>(typeof(UIHeroSellWindow)).SellOverUpdate();
                    break;
                case (short)MessageType.SC_HERO_LVL_UP:
                    WindowManager.Instance.GetWindow<UILevelUpWindow>(typeof(UILevelUpWindow)).ShowLevelOver();
                    break; 
                case (short)MessageType.SC_PROPERTY_CHANGED_NUMBER:
                    var propertyChangedMsg = msg.getContent() as SCPropertyChangedNumber;
                    if (propertyChangedMsg.RoleType == 2)
                    {
                        WindowManager.Instance.GetWindow<UILevelUpWindow>(typeof(UILevelUpWindow)).PropertyChangedNumber
                            =msg.getContent() as SCPropertyChangedNumber;
                    }
                    break;
            }
        }
    }
}



using KXSGCodec;
using UnityEngine;

namespace Assets.Game.Scripts.Net.handler
{
    class PlayerHandler 
    {
        public static void OnCreatePlayer(ThriftSCMessage msg)
        {
            if (ServiceManager.IsDebugAccount == 1)
            {
                ServiceManager.SetDebugAccount(ServiceManager.DebugUserName, ServiceManager.DebugPassword);
            }
            WindowManager.Instance.Show<LoginCreateRoleWindow>(true);
        }

        public static void OnPlayerInfo(ThriftSCMessage msg)
        {
            PopTextManager.PopTip("登录成功，返回玩家角色信息");
            var themsg = msg.GetContent() as SCPlayerInfoMsg;


            if (themsg != null)
            {
                PlayerModelLocator.Instance.HeroId = themsg.HeroId;
                PlayerModelLocator.Instance.RoleId = themsg.CId;
                PlayerModelLocator.Instance.Name = themsg.Name;
                PlayerModelLocator.Instance.HeadIconId = themsg.HeadIconId;
                PlayerModelLocator.Instance.Level = themsg.Lvl;
                PlayerModelLocator.Instance.Exp = themsg.Exp;
                PlayerModelLocator.Instance.Diamond = themsg.Diamond;
                PlayerModelLocator.Instance.Gold = themsg.Gold;
                PlayerModelLocator.Instance.Sprit = themsg.Spirit;
                PlayerModelLocator.Instance.ExtendHeroTimes = themsg.HeroExtendTimes;
                PlayerModelLocator.Instance.ExtendItemTimes = themsg.ItemExtendTimes;
                PlayerModelLocator.Instance.HeroMax = themsg.HeroMax;
                PlayerModelLocator.Instance.Energy = themsg.Energy;
                ServiceManager.UserID = themsg.UId;
                if (ServiceManager.IsDebugAccount == 1)
                {
                    MtaManager.ReportGameUser(ServiceManager.DebugUserName, ServiceManager.ServerData.ID, PlayerModelLocator.Instance.Level.ToString());
                }
                else
                {
                    //PopTextManager.PopTip("玩家账号：" + themsg.UName);
                    ServiceManager.AddServer(ServiceManager.ServerData.Url);
                    ServiceManager.SetAccount(themsg.UName);
                    MtaManager.ReportGameUser(ServiceManager.UserName, ServiceManager.ServerData.ID, PlayerModelLocator.Instance.Level.ToString());
                }
            }
            EventManager.Instance.Post(new LoginEvent { Message = "This is login event." });

            WindowManager.Instance.Show(WindowGroupType.Popup, false);

#if !UNITY_IPHONE

            GameObject.Find("Global").gameObject.GetComponent<PersistenceHandler>().GoToPersistenceWay();

#endif

            WindowManager.Instance.Show<UIMainScreenWindow>(true);
            WindowManager.Instance.Show<MainMenuBarWindow>(true);

            HttpResourceManager.LoadAll();
        }
    }
}

using System.Collections.Generic;
using KXSGCodec;
using System.IO;

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

            //Load perisitence file
            if (new FileInfo(BattleWindow.LoginInfoPath).Exists == true)
            {
                if(BattleWindow.LoadLoginInfo()["AccountID"] == themsg.UName&&
                BattleWindow.LoadLoginInfo()["ServerID"] == ServiceManager.ServerData.ID)
                {
                    if (new FileInfo(BattleWindow.StartBattleMessagePath).Exists == true &&
                        new FileInfo(BattleWindow.BattleEndMessagePath).Exists == false)
                    {
                        Logger.Log("Persistence mode: ReStartBattle.");

                        if (new FileInfo(BattleWindow.PersistencePath).Exists == false)
                        {
                            PersistenceConfirmWindow.Mode = PersistenceConfirmWindow.PersistenceMode.ReStartBattle;
                        }
                        else
                        {
                            PersistenceConfirmWindow.Mode =
                                PersistenceConfirmWindow.PersistenceMode.ReStartBattleWithPersistence;
                        }
                        var window = WindowManager.Instance.Show<PersistenceConfirmWindow>(true);
                        window.gameObject.SetActive(true);
                        window.SetLabel();
                    }
                    else if (new FileInfo(BattleWindow.StartBattleMessagePath).Exists == true &&
                        new FileInfo(BattleWindow.BattleEndMessagePath).Exists == true)
                    {
                        Logger.Log("Persistence mode: ReSendMessage.");

                        PersistenceConfirmWindow.Mode = PersistenceConfirmWindow.PersistenceMode.ReSendMessageNext;
                        var window = WindowManager.Instance.Show<PersistenceConfirmWindow>(true);
                        window.gameObject.SetActive(true);
                        window.SetLabel();
                    }
                }
                else
                {
                    //Delete file if switch account or server.
                    new FileInfo(BattleWindow.MissionModelLocatorPath).Delete();
                    new FileInfo(BattleWindow.StartBattleMessagePath).Delete();
                    new FileInfo(BattleWindow.PersistencePath).Delete();
                    new FileInfo(BattleWindow.BattleEndMessagePath).Delete();
                }
            }
            
            //Store loginaccount file 
            var tempDictionary = new Dictionary<string, string>();
            tempDictionary.Add("AccountID", themsg.UName);
            tempDictionary.Add("ServerID", ServiceManager.ServerData.ID);

            BattleWindow.StoreLoginInfo(tempDictionary);

#endif

            WindowManager.Instance.Show<UIMainScreenWindow>(true);
            WindowManager.Instance.Show<MainMenuBarWindow>(true);

            HttpResourceManager.LoadAll();
        }
    }
}

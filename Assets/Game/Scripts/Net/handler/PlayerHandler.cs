using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

namespace Assets.Game.Scripts.Net.handler
{
    class PlayerHandler
    {
        private static bool isNewPlayer=false;

        public static void OnCreatePlayer(ThriftSCMessage msg)
        {
            isNewPlayer = true;

            if (ServiceManager.IsDebugAccount == 1)
            {
                ServiceManager.SetDebugAccount(ServiceManager.DebugUserName, ServiceManager.DebugPassword);
            }
            WindowManager.Instance.Show<LoginCreateRoleWindow>(true);

            var scCreatePlayerMsg = msg.GetContent() as SCCreatePlayerMsg;
            //Set user's name if it's a new account
            if (scCreatePlayerMsg.CharName != null)
            {
                var windowObject = WindowManager.Instance.GetWindow<LoginCreateRoleWindow>().gameObject;
                var inputObject = Utils.FindChild(windowObject.transform, "Input - account");
                var labelObject = Utils.FindChild(inputObject.transform, "Label");
                inputObject.GetComponent<UIInput>().value =
                    labelObject.GetComponent<UILabel>().text = scCreatePlayerMsg.CharName;
            }
        }

        /// <summary>
        /// Set RandomCharName from server.
        /// </summary>
        /// <param name="msg"></param>
        public static void OnRandomCharName(ThriftSCMessage msg)
        {
            var scRandomNameMsg = msg.GetContent() as SCRandomCharNameMsg;

            var windowObject = WindowManager.Instance.GetWindow<LoginCreateRoleWindow>().gameObject;
            var inputObject = Utils.FindChild(windowObject.transform, "Input - account");
            var labelObject = Utils.FindChild(inputObject.transform, "Label");
            inputObject.GetComponent<UIInput>().value =
                labelObject.GetComponent<UILabel>().text = scRandomNameMsg.CharName;
        }

        public static void OnPlayerInfo(ThriftSCMessage msg)
        {
            UnityEngine.Debug.Log("Receive scplayerinfo msg from server.");
            //PopTextManager.PopTip("登录成功，返回玩家角色信息");
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
                EnergyIncreaseControl.Instance.Energy = themsg.Energy;
                PlayerModelLocator.Instance.TeamProp = new Dictionary<int, int>(themsg.TeamProp);
                PlayerModelLocator.Instance.TeamList = new List<int>(themsg.TeamList);
                UnityEngine.Debug.Log("Set PlayerModelLocator ends.");
                ServiceManager.UserID = themsg.UId;
                if (ServiceManager.IsDebugAccount == 1)
                {
                    MtaManager.ReportGameUser(ServiceManager.DebugUserName, ServiceManager.ServerData.ID, PlayerModelLocator.Instance.Level.ToString());
                }
                else
                {
                    ServiceManager.AddServer(ServiceManager.ServerData.Url);
                    ServiceManager.SetAccount(themsg.UName);
                    MtaManager.ReportGameUser(ServiceManager.UserName, ServiceManager.ServerData.ID, PlayerModelLocator.Instance.Level.ToString());
                }
            }
            EventManager.Instance.Post(new LoginEvent { Message = "This is login event." });
            HttpResourceManager.Instance.OnLoadFinish += OnFinish;
            //WindowManager.Instance.Show<MainMenuBarWindow>(true);
            UnityEngine.Debug.Log("Start loading template.");
            HttpResourceManager.Instance.LoadTemplate();
            WindowManager.Instance.Show<LoadingWaitWindow>(true);
        }

        private static void OnFinish()
        {
            Debug.Log("Go to finish in playerhandler.");
            WindowManager.Instance.Show<LoginAccountWindow>(false);
            WindowManager.Instance.Show<LoginCreateRoleWindow>(false);
            WindowManager.Instance.Show<LoginMainWindow>(false);
            WindowManager.Instance.Show<LoginRegisterWindow>(false);
            WindowManager.Instance.Show<LoginServersWindow>(false);
            WindowManager.Instance.Show<LoadingWaitWindow>(false);
            WindowManager.Instance.Show<UIMainScreenWindow>(true);

            HttpResourceManager.Instance.OnLoadFinish -= OnFinish;

            Debug.Log("go to green hand way.");
            //GreenHand battle
            if (isNewPlayer)
            {
                isNewPlayer = false;
                GreenhandController.Instance.SendStartMessage();
            }

            Debug.Log("Go to persistence way.");
            //BattlePersistence
            PersistenceHandler.Instance.GoToPersistenceWay();
        }
    }
}

using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

namespace Assets.Game.Scripts.Net.handler
{
    class PlayerHandler
    {
        public static bool IsSetCreatePlayerMsg = false;
        public static ThriftSCMessage SCCreatePlayerMsg;

        public static void OnCreatePlayer()
        {
            if (!IsSetCreatePlayerMsg)
            {
                Logger.LogError("SCCreatePlayerMsg not setted!");
                return;
            }

            if (ServiceManager.IsDebugAccount == 1)
            {
                ServiceManager.SetDebugAccount(ServiceManager.DebugUserName, ServiceManager.DebugPassword);
            }
            WindowManager.Instance.Show<LoginCreateRoleWindow>(true);

            var scCreatePlayerMsg = SCCreatePlayerMsg.GetContent() as SCCreatePlayerMsg;
            //Set user's name if it's a new account
            if (scCreatePlayerMsg.CharName != null)
            {
                var windowObject = WindowManager.Instance.GetWindow<LoginCreateRoleWindow>().gameObject;
                var inputObject = Utils.FindChild(windowObject.transform, "Input - account");
                var labelObject = Utils.FindChild(inputObject.transform, "Label");
                inputObject.GetComponent<UIInput>().value =
                    labelObject.GetComponent<UILabel>().text = scCreatePlayerMsg.CharName;
            }

            IsSetCreatePlayerMsg = false;
            //SystemModelLocator.Instance.NoNeedNotice = true;
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
            Debug.Log("Receive scplayerinfo msg from server.");
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
                PlayerModelLocator.Instance.CanSign = themsg.CanSign;
                PlayerModelLocator.Instance.HasFinishedQuest = themsg.HasFinishedQuest;
                EnergyIncreaseControl.Instance.Energy = themsg.Energy;
                PlayerModelLocator.Instance.TeamProp = new Dictionary<int, int>(themsg.TeamProp);
                PlayerModelLocator.Instance.TeamList = new List<int>(themsg.TeamList);
                Debug.Log("Set PlayerModelLocator ends.");
                ServiceManager.UserID = themsg.UId;

                var csMsg = new CSGameNoticeList();
                NetManager.SendMessage(csMsg);
                Debug.Log("Sended  CSGameNoticeList");

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
        }
    }
}

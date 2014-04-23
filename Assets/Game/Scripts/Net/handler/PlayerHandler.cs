using Assets.Game.Scripts.Net.network;
using KXSGCodec;
using UnityEngine;

namespace Assets.Game.Scripts.Net.handler
{
    class PlayerHandler
    {
        public static void OnCreatePlayer(ThriftSCMessage msg)
        {
            Debug.Log("MessageType.SC_CREATE_PLAYER");
            var csMsg = new CSCreatePlayerMsg();
            var act = ServiceManager.AccountData ?? ServiceManager.GetDefaultAccount();
            csMsg.Name = act.Account + act.Account;
            PopTextManager.PopTip("登录成功，正在创建角色(" + csMsg.Name + ")");
            NetManager.SendMessage(csMsg);
        }

        public static void OnPlayerInfo(ThriftSCMessage msg)
        {
            PopTextManager.PopTip("登录成功，返回玩家角色信息");
            var themsg = msg.getContent() as SCPlayerInfoMsg;
            if (themsg != null)
            {
                PlayerModelLocator.Instance.HeroId = themsg.HeroId;
                PlayerModelLocator.Instance.Name = themsg.Name;
                PlayerModelLocator.Instance.HeadIconId = themsg.HeadIconId;
                PlayerModelLocator.Instance.Level = themsg.Lvl;
                PlayerModelLocator.Instance.Exp = themsg.Exp;
                PlayerModelLocator.Instance.Diamond = themsg.Diamond;
                PlayerModelLocator.Instance.Gold = themsg.Gold;
            }
            EventManager.Instance.Post(new LoginEvent() { Message = "This is login event." });
            WindowManager.Instance.Show(typeof(MainMenuWindow), true);
            WindowManager.Instance.Show(typeof(MainMenuBarWindow), true);
            WindowManager.Instance.Show(WindowGroupType.Popup, false);
        }
    }
}

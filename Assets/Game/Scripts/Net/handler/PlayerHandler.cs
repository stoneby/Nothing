using KXSGCodec;

namespace Assets.Game.Scripts.Net.handler
{
    class PlayerHandler
    {
        public static void OnCreatePlayer(ThriftSCMessage msg)
        {
            if (ServiceManager.AccountData != null)
            {
                ServiceManager.AddAccount(ServiceManager.AccountData);
                ServiceManager.SaveAccount();
            }
           
            WindowManager.Instance.Show(typeof(LoginCreateRoleWindow), true);
            
        }

        public static void OnPlayerInfo(ThriftSCMessage msg)
        {
            PopTextManager.PopTip("登录成功，返回玩家角色信息");
            var themsg = msg.GetContent() as SCPlayerInfoMsg;
            if (themsg != null)
            {
                PlayerModelLocator.Instance.HeroId = themsg.HeroId;
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
                //PlayerModelLocator.Instance.ItemMax = themsg.i;
                PlayerModelLocator.Instance.Energy = themsg.Energy;
            }
            EventManager.Instance.Post(new LoginEvent() { Message = "This is login event." });
            WindowManager.Instance.Show(typeof(UIMainScreenWindow), true);
            WindowManager.Instance.Show(typeof(MainMenuBarWindow), true);
            WindowManager.Instance.Show(WindowGroupType.Popup, false);

            HttpResourceManager.LoadAll();
        }
    }
}

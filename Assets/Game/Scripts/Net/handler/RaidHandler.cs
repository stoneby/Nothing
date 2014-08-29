using KXSGCodec;

namespace Assets.Game.Scripts.Net.handler
{
    class RaidHandler
    {
        public static void OnRaidAddition(ThriftSCMessage msg)
        {
            var addmsg = msg.GetContent() as SCRaidAddtion;
            if (addmsg != null)
            {
                MissionModelLocator.Instance.RaidAddition = addmsg;
            }
            else
            {
                PopTextManager.PopTip("返回战斗的数据错误");
            }
        }

        public static void OnRaidLoadingAll(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCRaidLoadingAll;
            if (themsg != null)
            {
                MissionModelLocator.Instance.RaidLoadingAll = themsg;
                WindowManager.Instance.Show(typeof(RaidsWindow), true);
                WindowManager.Instance.Show<MainMenuBarWindow>(false);
            }
            else
            {
                //PopTextManager.PopTip("返回的副本数据错误");
            }
        }

        public static void OnRaidQueryFriend(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCRaidQueryFriend;
            if (themsg != null)
            {
                //var e = new FriendEvent();
                //e.RaidFriend = themsg;
                //EventManager.Instance.Post(e);
                MissionModelLocator.Instance.FriendsMsg = themsg;

                if (HeroModelLocator.AlreadyRequest == false)
                {
                    HeroModelLocator.Instance.GetHeroPos = RaidType.GetHeroInBattle;
                    var csmsg = new CSHeroList();
                    NetManager.SendMessage(csmsg);
                }
                else
                {
                    WindowManager.Instance.Show(typeof(SetBattleWindow), true);
                    WindowManager.Instance.Show<RaidsWindow>(false);
                }
            }
            else
            {
                PopTextManager.PopTip("返回战斗的数据错误");
            }
        }

        public static void OnRaidReward(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCRaidReward;
            if (themsg != null)
            {
                MissionModelLocator.Instance.BattleReward = themsg;
                PersistenceHandler.IsRaidFinish = true;

                WindowManager.Instance.Show(typeof(BattleWinWindow), true);
            }
            else
            {
                //PopTextManager.PopTip("返回战斗的数据错误");
            }
        }

        public static void OnRaidClearDailyTimes(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCRaidClearDailyTimes;
            if (themsg != null)
            {

            }
            else
            {
                //PopTextManager.PopTip("返回战斗的数据错误");
            }
        }

        public static void OnRaidEnterFail(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCRaidEnterFail;
            if (themsg != null)
            {
                PopTextManager.PopTip(themsg.Reason, false);
            }
            else
            {
                //PopTextManager.PopTip("返回战斗的数据错误");
            }
        }

        public static void OnRaidNewStage(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCRaidNewStage;
            if (themsg != null)
            {
                //PopTextManager.PopTip("返回下一关卡数据");
                MissionModelLocator.Instance.AddNewStage(themsg);
            }
            else
            {
                //PopTextManager.PopTip("返回战斗的数据错误");
            }
        }

        public static void OnRaidReceiveReward(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCRaidReceiveAwards;
            if (themsg != null)
            {
                if (!MissionModelLocator.Instance.RaidLoadingAll.HasAwardInfo.Contains(MissionModelLocator.Instance.Raid.TemplateId))
                {
                    MissionModelLocator.Instance.RaidLoadingAll.HasAwardInfo.Add(MissionModelLocator.Instance.Raid.TemplateId);
                }
                PopTextManager.PopTip("奖励领取成功");
            }
            else
            {
                //PopTextManager.PopTip("返回战斗的数据错误");
            }
        }

        public static void OnRaidFinishAddFriend(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCRaidFinishAddFriend;
            if (themsg != null)
            {
                MissionModelLocator.Instance.ShowAddFriendAlert = true;
            }
        }

        
    }
}

using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.battle.share.enums;
using com.kx.sglm.gs.battle.share.factory.creater;
using KXSGCodec;
using System.Collections.Generic;

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
                //PopTextManager.PopTip("返回战斗的数据错误");
            }
        }

        public static void OnRaidLoadingAll(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCRaidLoadingAll;
            if (themsg != null)
            {
                MissionModelLocator.Instance.RaidLoadingAll = themsg;
                WindowManager.Instance.Show(typeof(MissionTabWindow), true);
            }
            else
            {
                PopTextManager.PopTip("返回的副本数据错误");
            }
        }

        public static void OnRaidQueryFriend(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCRaidQueryFriend;
            if (themsg != null)
            {
                var e = new FriendEvent();
                e.RaidFriend = themsg;
                EventManager.Instance.Post(e);
            }
            else
            {
                //PopTextManager.PopTip("返回战斗的数据错误");
            }
        }

        public static void OnRaidReward(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCRaidReward;
            if (themsg != null)
            {

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

            }
            else
            {
                //PopTextManager.PopTip("返回战斗的数据错误");
            }
        }
    }
}

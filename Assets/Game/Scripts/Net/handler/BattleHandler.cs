using Assets.Game.Scripts.Common.Model;
using Assets.Game.Scripts.Net.network;
using KXSGCodec;
using UnityEngine;

namespace Assets.Game.Scripts.Net.handler
{
    class BattleHandler
    {
        public static void OnBattlePveStart(ThriftSCMessage msg)
        {
            var battlestartmsg = msg.getContent() as SCBattlePveStartMsg;
            if (battlestartmsg != null)
            {
                PopTextManager.PopTip("返回战斗数据");
                BattleModelLocator.Instance.BattleType = battlestartmsg.BattleType;
                BattleModelLocator.Instance.FighterList = battlestartmsg.FighterList;
                BattleModelLocator.Instance.MonsterGroup = battlestartmsg.MonsterGroup;
                BattleModelLocator.Instance.MonsterList = battlestartmsg.MonsterList;
                BattleModelLocator.Instance.RaidID = battlestartmsg.RaidID;
                BattleModelLocator.Instance.Uuid = battlestartmsg.Uuid;

                var window = WindowManager.Instance.Show(typeof(BattleWindow), true).gameObject;
                WindowManager.Instance.Show(typeof(MainMenuBarWindow), false);

                //GameObject obj = GameObject.Find("Battle(Clone)");
                Logger.Log(window);
                var battlemanager = window.GetComponent<InitBattleField>();
//                var attracks = new int[12];
//                for (var i = 0; i < attracks.Length; i++)
//                {
//                    attracks[i] = (i % 2 == 0) ? 1 : 5;
//                }
//                var enemys = new int[2];
//                enemys[0] = 1;
//                enemys[1] = 2;
                battlemanager.StartBattle();
            }
            else
            {
                PopTextManager.PopTip("返回战斗的数据错误");
            }
        }
    }
}

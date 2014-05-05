using System.Collections.Generic;
using Assets.Game.Scripts.Common.Model;
using Assets.Game.Scripts.Net.network;
using com.kx.sglm.gs.battle.data;
using com.kx.sglm.gs.battle.enums;
using com.kx.sglm.gs.battle.factory.creater;
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
                BattleModelLocator.Instance.MonsterGroup = (List<int>) battlestartmsg.MonsterGroup;
                BattleModelLocator.Instance.RaidID = battlestartmsg.RaidID;
                BattleModelLocator.Instance.Uuid = battlestartmsg.Uuid;
                
                //构建服务器开始战斗逻辑
                var type = BattleType.getValue(battlestartmsg.BattleType);
                BattleModelLocator.Instance.Source = new BattleSource(type);
                BattleModelLocator.Instance.Source.Uuid = battlestartmsg.Uuid;

                BattleModelLocator.Instance.HeroList = FighterInfoCreater.createListFromMsgHero(BattleSideEnum.SIDEA, battlestartmsg.FighterList);
                BattleModelLocator.Instance.MonsterList = FighterInfoCreater.createListFormMsgMonster(BattleSideEnum.SIDEB, battlestartmsg.MonsterGroup, battlestartmsg.MonsterList);
		        var _allFighterList = new List<FighterInfo>();
                _allFighterList.AddRange(BattleModelLocator.Instance.HeroList);
                _allFighterList.AddRange(BattleModelLocator.Instance.MonsterList);
                BattleModelLocator.Instance.Source.FighterProp = _allFighterList;

                var _factory = type.Factory;
                BattleModelLocator.Instance.MainBattle = _factory.createBattle(BattleModelLocator.Instance.Source);
                BattleModelLocator.Instance.MainBattle.start();
                BattleModelLocator.Instance.MonsterIndex = 0;
                //客户端显示战斗
                var window = WindowManager.Instance.Show(typeof(BattleWindow), true).gameObject;
                WindowManager.Instance.Show(typeof(MainMenuBarWindow), false);

                var battlemanager = window.GetComponent<InitBattleField>();
                battlemanager.StartBattle();
            }
            else
            {
                PopTextManager.PopTip("返回战斗的数据错误");
            }
        }
    }
}

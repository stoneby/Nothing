using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.battle.share.enums;
using com.kx.sglm.gs.battle.share.factory.creater;
using KXSGCodec;
using System.Collections.Generic;

namespace Assets.Game.Scripts.Net.handler
{
    class BattleHandler
    {
        public static void OnBattlePveStart(ThriftSCMessage msg)
        {
            var battlestartmsg = msg.GetContent() as SCBattlePveStartMsg;
            if (battlestartmsg != null)
            {
                PopTextManager.PopTip("返回战斗数据");
                BattleModelLocator.Instance.BattleType = battlestartmsg.BattleType;
                BattleModelLocator.Instance.EnemyGroup = (List<int>)battlestartmsg.MonsterGroup;
                BattleModelLocator.Instance.RaidID = battlestartmsg.RaidID;
                BattleModelLocator.Instance.Uuid = battlestartmsg.Uuid;

                //构建服务器开始战斗逻辑
                var type = BattleType.getValue(battlestartmsg.BattleType);
                BattleModelLocator.Instance.Source = new BattleSource(type);
                BattleModelLocator.Instance.Source.Uuid = battlestartmsg.Uuid;

                BattleModelLocator.Instance.HeroList = FighterInfoCreater.createListFromMsgHero(BattleSideEnum.SIDEA, battlestartmsg.FighterList);
                BattleModelLocator.Instance.EnemyList = FighterInfoCreater.createListFormMsgMonster(BattleSideEnum.SIDEB, battlestartmsg.MonsterGroup, battlestartmsg.MonsterList);
                var _allFighterList = new List<FighterInfo>();
                _allFighterList.AddRange(BattleModelLocator.Instance.HeroList);
                _allFighterList.AddRange(BattleModelLocator.Instance.EnemyList);
                BattleSource _source = BattleModelLocator.Instance.Source;
                BattleModelLocator.Instance.Source.FighterProp = _allFighterList;

                
                _source.heroSkillList.AddRange(battlestartmsg.HeroSkillList);
                _source.MonsterAList.AddRange(battlestartmsg.MonsterAIList);
                _source.monsterSkillList.AddRange(battlestartmsg.MonsterSkillList);

                FighterInfoCreater.initBattleSkillService(_source);
                

                var _factory = type.Factory;
                BattleModelLocator.Instance.MainBattle = _factory.createBattle(BattleModelLocator.Instance.Source);
                BattleModelLocator.Instance.MainBattle.start();
                BattleModelLocator.Instance.MonsterIndex = 0;
                //客户端显示战斗
                var window = WindowManager.Instance.Show(typeof(BattleWindow), true).gameObject;
                WindowManager.Instance.Show(typeof(MissionTabWindow), false);
                WindowManager.Instance.Show(typeof(MainMenuBarWindow), false);
                WindowManager.Instance.Show(typeof (BattleConfirmTabWindow), false);
            }
            else
            {
                PopTextManager.PopTip("返回战斗的数据错误");
            }
        }
    }
}

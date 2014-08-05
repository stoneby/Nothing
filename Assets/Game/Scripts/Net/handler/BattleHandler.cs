using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share;
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
                BattleModelLocator.Instance.EnemyGroup = battlestartmsg.MonsterGroup;
                BattleModelLocator.Instance.RaidID = battlestartmsg.RaidID;
                BattleModelLocator.Instance.Uuid = battlestartmsg.Uuid;

                // server logic data.
                var type = BattleType.getValue(battlestartmsg.BattleType);
                BattleModelLocator.Instance.Source = new BattleSource(type)
                {
                    Uuid = battlestartmsg.Uuid
                };

                BattleModelLocator.Instance.HeroList = FighterInfoCreater.createListFromMsgHero(BattleSideEnum.SIDEA, battlestartmsg.FighterList);
                BattleModelLocator.Instance.EnemyList = FighterInfoCreater.createListFormMsgMonster(BattleSideEnum.SIDEB, battlestartmsg.MonsterGroup, battlestartmsg.MonsterList);

                var allFighterList = new List<FighterInfo>();
                allFighterList.AddRange(BattleModelLocator.Instance.HeroList);
                allFighterList.AddRange(BattleModelLocator.Instance.EnemyList);
                var source = BattleModelLocator.Instance.Source;
                BattleModelLocator.Instance.Source.FighterProp = allFighterList;
                source.spMaxBuffId = battlestartmsg.SpMaxBuffId;
                source.heroSkillList.AddRange(battlestartmsg.HeroSkillList);
                source.MonsterAIList.AddRange(battlestartmsg.MonsterAIList);
                source.monsterSkillList.AddRange(battlestartmsg.MonsterSkillList);
                if (battlestartmsg.BuffList != null)
                {
                    source.buffList.AddRange(battlestartmsg.BuffList);
                }
                FighterInfoCreater.initBattleSkillService(source);

                var factory = type.Factory;
                BattleModelLocator.Instance.MainBattle = factory.createBattle(BattleModelLocator.Instance.Source);
                BattleModelLocator.Instance.MainBattle.start();
                BattleModelLocator.Instance.MonsterIndex = 0;

                // client side show.
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

using System.Collections.Generic;
using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.battle.share.enums;
using com.kx.sglm.gs.battle.share.factory.creater;
using KXSGCodec;

using UnityEngine;



namespace Assets.Game.Scripts.Net.handler
{
    class BattleHandler
    {
        public static void OnBattlePveStart(ThriftSCMessage msg)
        {
            var battlestartmsg = msg.GetContent() as SCBattlePveStartMsg;

#if !UNITY_IPHONE

            //Store missionmodellocator and battlestartmsg for battle persistence.
            var persistenceHandler = GameObject.Find("Global").GetComponent<PersistenceHandler>();
            persistenceHandler.StoreMissionModelLocator();
            persistenceHandler.StoreStartBattleMessage(battlestartmsg);

#endif

            if (battlestartmsg != null)
            {
                PopTextManager.PopTip("返回战斗数据");
//                BattleModelLocator.Instance.BattleType = battlestartmsg.BattleType;
//                BattleModelLocator.Instance.RaidID = battlestartmsg.RaidID;
//                BattleModelLocator.Instance.Uuid = battlestartmsg.Uuid;
//
//                // server logic data.
//                var type = BattleType.getValue(battlestartmsg.BattleType);
//                BattleModelLocator.Instance.Source = new BattleSource(type)
//                {
//                    Uuid = battlestartmsg.Uuid
//                };
//                IBattleTemplateService _service = BattleTemplateModelLocator.Instance;
//                var _creater = new BattleSourceTemplateCreater(_service);
//                var _source = _creater.createPVESource(battlestartmsg);
//
//                //赋值给BattleModeLocator
//                BattleModelLocator.Instance.HeroList = _source.getSideFighters(BattleSideEnum.SIDE_LEFT);
//                BattleModelLocator.Instance.EnemyList = _source.getSideFighters(BattleSideEnum.SIDEB_RIGHT);
//
//                BattleModelLocator.Instance.Source = _source;
//                BattleModelLocator.Instance.EnemyGroup = _source.MonsterGroup;
                BattleCreateUtils.initBattleModeLocator(BattleModelLocator.Instance, battlestartmsg);

                var factory = BattleModelLocator.Instance.Source.BattleType.Factory;
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

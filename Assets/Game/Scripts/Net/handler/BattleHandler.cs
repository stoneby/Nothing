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
        public const string EnergyNotEnoughKey = "Battle.EnergyNotEnough";
        public static void OnBattlePveStart(ThriftSCMessage msg)
        {
            var battlestartmsg = msg.GetContent() as SCBattlePveStartMsg;

            //Store missionmodellocator and battlestartmsg for battle persistence.
            PersistenceHandler.Instance.StoreStartBattle(battlestartmsg);

            if (battlestartmsg != null)
            {
                PopTextManager.PopTip("返回战斗数据");
                BattleCreateUtils.initBattleModeLocator(BattleModelLocator.Instance, battlestartmsg);

                var factory = BattleModelLocator.Instance.Source.BattleType.Factory;
                BattleModelLocator.Instance.MainBattle = factory.createBattle(BattleModelLocator.Instance.Source);
                BattleModelLocator.Instance.MainBattle.start();
                BattleModelLocator.Instance.MonsterIndex = 0;

                // client side show.
                var window = WindowManager.Instance.Show(typeof(BattleWindow), true).gameObject;
                WindowManager.Instance.Show(typeof(RaidsWindow), false);
                //WindowManager.Instance.Show(typeof(MainMenuBarWindow), false);
                WindowManager.Instance.Show(typeof (SetBattleWindow), false);
            }
            else
            {
                PopTextManager.PopTip("返回战斗的数据错误");
            }
        }

        public static void OnEnergyNotEnough(ThriftSCMessage msg)
        {
            var energyNotEnough = msg.GetContent() as SCEnergyNotEnough;
            if(energyNotEnough != null)
            {
                var assertWindow = WindowManager.Instance.GetWindow<AssertionWindow>();
                assertWindow.AssertType = AssertionWindow.Type.OkCancel;
                assertWindow.Title = string.Format(LanguageManager.Instance.GetTextValue(EnergyNotEnoughKey), energyNotEnough.BuyEnergyCost);
                assertWindow.Message = "";
                assertWindow.OkButtonClicked += OkButtonClicked;
                WindowManager.Instance.Show(typeof(AssertionWindow), true);
            }
        }

        private static void OkButtonClicked(GameObject sender)
        {
            var assertWindow = WindowManager.Instance.GetWindow<AssertionWindow>();
            assertWindow.OkButtonClicked -= OkButtonClicked;
            var msg = new CSBuyEnergy();
            NetManager.SendMessage(msg);
        }
    }
}

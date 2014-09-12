using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share;
using com.kx.sglm.gs.battle.share.enums;
using KXSGCodec;

using UnityEngine;



namespace Assets.Game.Scripts.Net.handler
{
    class BattleHandler
    {
        public const string EnergyNotEnoughKey = "Battle.EnergyNotEnough";
        public static void OnBattlePveStart(ThriftSCMessage msg)
        {
            var battleStartMsg = msg.GetContent() as SCBattlePveStartMsg;

            if (battleStartMsg == null)
            {
                Logger.LogError("Battle start message should not be null.");
                return;
            }

            PersistenceHandler.Instance.Enabled = (battleStartMsg.BattleType != BattleType.GREENHANDPVE.Index);

            //Store missionmodellocator and battlestartmsg for battle persistence.
            PersistenceHandler.Instance.StoreStartBattle(battleStartMsg);

            // initialize battle model locator from battle start message.
            BattleModelLocator.Instance.Init(battleStartMsg);

            // client side show.
            WindowManager.Instance.Show(typeof(BattleWindow), true);
            WindowManager.Instance.Show(typeof(RaidsWindow), false);
            WindowManager.Instance.Show(typeof(SetBattleWindow), false);
        }

        public static void OnEnergyNotEnough(ThriftSCMessage msg)
        {
            var energyNotEnough = msg.GetContent() as SCEnergyNotEnough;
            if (energyNotEnough != null)
            {
                var assertWindow = WindowManager.Instance.GetWindow<AssertionWindow>();
                assertWindow.AssertType = AssertionWindow.Type.OkCancel;
                assertWindow.Title = string.Format(LanguageManager.Instance.GetTextValue(EnergyNotEnoughKey), energyNotEnough.BuyEnergyCost);
                assertWindow.Message = "";
                assertWindow.OkButtonClicked = OkClicked;
                assertWindow.CancelButtonClicked = CancelClicked;
                WindowManager.Instance.Show(typeof(AssertionWindow), true);
            }
        }

        private static void OkClicked(GameObject sender)
        {
            UnRegist();
            var msg = new CSBuyEnergy();
            NetManager.SendMessage(msg);
        }

        private static void CancelClicked(GameObject sender)
        {
            UnRegist();
        }

        private static void UnRegist()
        {
        }
    }
}

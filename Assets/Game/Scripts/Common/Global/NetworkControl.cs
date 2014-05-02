using Assets.Game.Scripts.Net.handler;
using Assets.Game.Scripts.Net.network;
using KXSGCodec;
using UnityEngine;
using System.Collections;

public class NetworkControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(MessageHandler());

	}

    private IEnumerator MessageHandler()
    {
        while (true)
        {
            var msg = NetManager.GetMessage();
            while (msg != null)
            {
                Logger.Log(msg.GetMsgType());
                switch (msg.GetMsgType())
                {
                    case (short)MessageType.SC_SYSTEM_INFO_MSG:
                        SystemHandler.OnSystemInfo(msg);
                        break;
                    case (short)MessageType.SC_CREATE_PLAYER_MSG:
                        PlayerHandler.OnCreatePlayer(msg);
                        break;
                    case (short)MessageType.SC_PLAYER_INFO_MSG:
                        PlayerHandler.OnPlayerInfo(msg);
                        break;
                    case (short)MessageType.SC_BATTLE_PVE_START_MSG:
                        BattleHandler.OnBattlePveStart(msg);
                        break;
                    case (short)MessageType.SC_HERO_LIST:
                    case (short)MessageType.SC_HERO_MODIFY_TEAM:
                    case (short)MessageType.SC_HERO_SELL:
                    case (short)MessageType.SC_HERO_LVL_UP:
                    case (short)MessageType.SC_PROPERTY_CHANGED_NUMBER:
                    case (short)MessageType.SC_HERO_CREATE_ONE:
                        HeroHandler.OnHeroMessage(msg);
                        break;

                }
                msg = NetManager.GetMessage();
            }

            yield return new WaitForSeconds(0.5f);
        }
        

    }
}

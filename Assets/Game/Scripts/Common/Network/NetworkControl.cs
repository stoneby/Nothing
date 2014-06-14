using Assets.Game.Scripts.Net.handler;
using KXSGCodec;
using System.Collections;
using UnityEngine;

public class NetworkControl : MonoBehaviour
{
    private float realTime = -10;

    private static IEnumerator MessageHandler()
    {
        while (true)
        {
            var msg = NetManager.GetMessage();
            while (msg != null)
            {
                Logger.Log(msg.GetMsgType());
                switch (msg.GetMsgType())
                {
                    case (short) MessageType.SC_SYSTEM_INFO_MSG:
                        SystemHandler.OnSystemInfo(msg);
                        break;
                    case (short) MessageType.SC_ERROR_INFO_MSG:
                        SystemHandler.OnErrorInfo(msg);
                        break;
                    case (short) MessageType.SC_CREATE_PLAYER_MSG:
                        PlayerHandler.OnCreatePlayer(msg);
                        break;
                    case (short) MessageType.SC_PLAYER_INFO_MSG:
                        PlayerHandler.OnPlayerInfo(msg);
                        break;
                    case (short) MessageType.SC_BATTLE_PVE_START_MSG:
                        BattleHandler.OnBattlePveStart(msg);
                        break;
                    case (short) MessageType.SC_HERO_LIST:
                    case (short) MessageType.SC_HERO_MODIFY_TEAM:
                    case (short) MessageType.SC_HERO_SELL:
                    case (short) MessageType.SC_HERO_LVL_UP:
                    case (short) MessageType.SC_PROPERTY_CHANGED_NUMBER:
                    case (short) MessageType.SC_HERO_CREATE_ONE:
                    case (short) MessageType.SC_HERO_MAX_EXTEND:
                    case (short) MessageType.SC_HERO_CHANGE_EQUIP:
                        HeroHandler.OnHeroMessage(msg);
                        break;
                    case (short) MessageType.SC_RAID_ADDTION:
                        RaidHandler.OnRaidAddition(msg);
                        break;
                    case (short) MessageType.SC_RAID_LOADING_ALL:
                        RaidHandler.OnRaidLoadingAll(msg);
                        break;
                    case (short) MessageType.SC_RAID_QUERY_FRIEND:
                        RaidHandler.OnRaidQueryFriend(msg);
                        break;
                    case (short) MessageType.SC_RAID_REWARD:
                        RaidHandler.OnRaidReward(msg);
                        break;
                    case (short) MessageType.SC_RAID_CLEAR_DAILY_TIMES:
                        RaidHandler.OnRaidClearDailyTimes(msg);
                        break;
                    case (short) MessageType.SC_RAID_ENTER_FAIL:
                        RaidHandler.OnRaidEnterFail(msg);
                        break;
                    case (short)MessageType.SC_RAID_NEW_STAGE:
                        RaidHandler.OnRaidNewStage(msg);
                        break;
                    case (short)MessageType.SC_RAID_RECEIVE_AWARDS:
                        RaidHandler.OnRaidReceiveReward(msg);
                        break;
                    case (short) MessageType.SC_ALL_ITEM_INFOS:
                        ItemHandler.OnAllItemInfos(msg);
                        break;     
                    case (short) MessageType.SC_SERVER_CONFIG_MSG:
                        ItemHandler.OnServerConfigMsg(msg);
                        break; 
                    case (short) MessageType.SC_ADD_ITEM:
                        ItemHandler.OnAddItem(msg);
                        break;
                    case (short)MessageType.SC_ITEM_DETAIL:
                        ItemHandler.OnItemDetail(msg);
                        break;
                    case (short)MessageType.SC_STRENGTH_ITEM_SUCC:
                        ItemHandler.OnStrengthItemSucc(msg);
                        break;
                    case (short)MessageType.SC_ITEM_SELL_SUCC:
                        ItemHandler.OnItemSellSucc(msg);
                        break;     
                    case (short)MessageType.SC_EVOLUTE_ITEM_SUCC:
                        ItemHandler.OnEvoluteItemSucc(msg);
                        break;
                    case (short)MessageType.SC_EXTEND_ITEM_BAG_SUCC:
                        ItemHandler.OnExtendItemBagSucc(msg);
                        break;   
                    case (short)MessageType.SC_ITEM_LOCK_OPER_SUCC:
                        ItemHandler.OnItemLockOperSucc(msg);
                        break;
                    case (short)MessageType.SC_BUY_BACK_ITEM_SUCC:
                        ItemHandler.OnBuyBackItemSucc(msg);
                        break;
                }
                msg = NetManager.GetMessage();
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Start()
    {
        StartCoroutine(MessageHandler());
    }

    private void Update()
    {
        //返回键
        if (Application.platform == RuntimePlatform.Android && (Input.GetKeyDown(KeyCode.Escape)))
        {
            if (Time.realtimeSinceStartup - realTime < 3)
            {
                Application.Quit();
            }
            else
            {
                realTime = Time.realtimeSinceStartup;
                PopTextManager.PopTip("再按一次退出游戏");
            }
        }

        // Home键
        if (Application.platform == RuntimePlatform.Android && (Input.GetKeyDown(KeyCode.Home)))
        {
            Application.Quit();
        }
    }
}

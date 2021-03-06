﻿using System;
using Assets.Game.Scripts.Net.handler;
using KXSGCodec;
using System.Collections;
using UnityEngine;

public class NetworkControl : MonoBehaviour
{
    private const float ApplicationQuitTime = 3f;
    private float lastTimeSinceStart;

    private IEnumerator MessageHandler()
    {
        while (true)
        {
            var msg = NetManager.GetMessage();
            try
            {
                while (msg != null)
                {
                    Debug.Log(msg.GetMsgType());
                    switch (msg.GetMsgType())
                    {
                        case (short)MessageType.SC_SYSTEM_INFO_MSG:
                            SystemHandler.OnSystemInfo(msg);
                            break;
                        case (short)MessageType.SC_ERROR_INFO_MSG:
                            SystemHandler.OnErrorInfo(msg);
                            break;
                        case (short)MessageType.SC_GREENHAND_FLAG_MSG:
                            GreenHandGuideHandler.Instance.SetGreenHandGuideFlag(msg);
                            break;
                        case (short)MessageType.SC_CREATE_PLAYER_MSG:
                            PlayerHandler.IsSetCreatePlayerMsg = true;
                            PlayerHandler.SCCreatePlayerMsg = msg;
                            GreenHandGuideHandler.Instance.GoToCreatePlayer();
                            break;
                        case (short)MessageType.SC_PLAYER_INFO_MSG:
                            PlayerHandler.OnPlayerInfo(msg);
                            break;
                        case (short)MessageType.SC_BATTLE_PVE_START_MSG:
                            BattleHandler.OnBattlePveStart(msg);
                            break;
                        case (short)MessageType.SC_ENERGY_NOT_ENOUGH:
                            BattleHandler.OnEnergyNotEnough(msg);
                            break;
                        case (short)MessageType.SC_HERO_LIST:
                        case (short)MessageType.SC_HERO_MODIFY_TEAM:
                        case (short)MessageType.SC_HERO_SELL:
                        case (short)MessageType.SC_HERO_LVL_UP:
                        case (short)MessageType.SC_HERO_CREATE_ONE:
                        case (short)MessageType.SC_HERO_MAX_EXTEND:
                        case (short)MessageType.SC_HERO_CHANGE_EQUIP:
                        case (short)MessageType.SC_HERO_BIND_SUCC:
                            HeroHandler.OnHeroMessage(msg);
                            break;
                        case (short)MessageType.SC_PROPERTY_CHANGED_NUMBER:
                            CommonHandler.OnPropertyChangedNumber(msg);
                            break;
                        case (short)MessageType.SC_RAID_ADDTION:
                            RaidHandler.OnRaidAddition(msg);
                            break;
                        case (short)MessageType.SC_RAID_LOADING_ALL:
                            RaidHandler.OnRaidLoadingAll(msg);
                            break;
                        case (short)MessageType.SC_RAID_QUERY_FRIEND:
                            RaidHandler.OnRaidQueryFriend(msg);
                            break;
                        case (short)MessageType.SC_RAID_REWARD:
                            RaidHandler.OnRaidReward(msg);
                            break;
                        case (short)MessageType.SC_RAID_CLEAR_DAILY_TIMES:
                            RaidHandler.OnRaidClearDailyTimes(msg);
                            break;
                        case (short)MessageType.SC_RAID_ENTER_FAIL:
                            RaidHandler.OnRaidEnterFail(msg);
                            break;
                        case (short)MessageType.SC_RAID_NEW_STAGE:
                            RaidHandler.OnRaidNewStage(msg);
                            break;
                        case (short)MessageType.SC_RAID_RECEIVE_AWARDS:
                            RaidHandler.OnRaidReceiveReward(msg);
                            break;
                        case (short)MessageType.SC_RAID_FINISH_ADD_FRIEND:
                            RaidHandler.OnRaidFinishAddFriend(msg);
                            break;
                        case (short)MessageType.SC_ALL_ITEM_INFOS:
                            ItemHandler.OnAllItemInfos(msg);
                            break;
                        case (short)MessageType.SC_SERVER_CONFIG_MSG:
                            ItemHandler.OnServerConfigMsg(msg);
                            break;
                        case (short)MessageType.SC_ADD_ITEM:
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
                        case (short)MessageType.SC_LOTTERY_LIST:
                            ChooseCardHandler.OnLotteryList(msg);
                            break;
                        case (short)MessageType.SC_LOTTERY:
                            ChooseCardHandler.OnLottery(msg);
                            break;
                        case (short)MessageType.SC_LOTTERY_REFRESH_TIMES:
                            ChooseCardHandler.OnLotteryRefreshTimes(msg);
                            break;
                        case (short)MessageType.SC_ADD_ITEMS_AND_HEROS:
                            ChooseCardHandler.OnAddItemsAndHeros(msg);
                            break;
                        case (short)MessageType.SC_LOTTERY_CANNOT_FREE:
                            ChooseCardHandler.OnLotteryNotFree(msg);
                            break;
                        case (short)MessageType.SC_LOTTERY_COMPOSE_LIST:
                            ChooseCardHandler.OnLotteryComposeList(msg);
                            break;
                        case (short)MessageType.SC_LOTTERY_COMPOSE_SUCC:
                            ChooseCardHandler.OnLotteryComposeSucc(msg);
                            break;
                        case (short)MessageType.SC_RECHARGE_ID_MSG:
                            SDKPayManager.DoPay(msg);
                            break;
                        case (short)MessageType.SC_FRIEND_LOADING_ALL:
                            FriendHandler.OnFriendLoadingAll(msg);
                            break;
                        case (short)MessageType.SC_FRIEND_APPLY_LIST:
                            FriendHandler.OnFriendApplyList(msg);
                            break;
                        case (short)MessageType.SC_FRIEND_QUERY_BY_NAME:
                            FriendHandler.OnFriendQueryByName(msg);
                            break;
                        case (short)MessageType.SC_FRIEND_GIVE_ENERGY_SUCC:
                            FriendHandler.OnFriendGiveEnergySucc(msg);
                            break;
                        case (short)MessageType.SC_FRIEND_RECIEVE_ENERGY_LIST:
                            FriendHandler.OnFriendRecieveEnergyList(msg);
                            break;
                        case (short)MessageType.SC_FRIEND_RECIEVE_ENERGY_SUCC:
                            FriendHandler.OnFriendReciveEnergySucc(msg);
                            break;
                        case (short)MessageType.SC_FRIEND_APPLY_SUCC:
                            FriendHandler.OnFriendApplySucc(msg);
                            break;
                        case (short)MessageType.SC_FRIEND_APPLY_OPER_SUCC:
                            FriendHandler.OnFriendApplyOperSucc(msg);
                            break;
                        case (short)MessageType.SC_FRIEND_EXTEND_SUCC:
                            FriendHandler.OnFriendExtendSucc(msg);
                            break;
                        case (short)MessageType.SC_FRIEND_BIND_SUCC:
                            FriendHandler.OnFriendBindSucc(msg);
                            break;
                        case (short)MessageType.SC_FRIEND_DELETE_SUCC:
                            FriendHandler.OnFriendDelSucc(msg);
                            break;
                        case (short)MessageType.SC_RANDOM_CHAR_NAME_MSG:
                            PlayerHandler.OnRandomCharName(msg);
                            break;
                        case (short)MessageType.SC_HERO_FRIST_LOGIN_GIVE:
                            ChooseCardHandler.OnHeroFirstLoginGive(msg);
                            break;
                        case (short)MessageType.SC_MAIL_LIST_MSG:
                            MailHandler.OnMailList(msg);
                            break;
                        case (short)MessageType.SC_MAIL_UNREAD_STATE_MSG:
                            MailHandler.OnMailUnreadStateMsg(msg);
                            break;
                        case (short)MessageType.SC_MAIL_OPTION_RESULT_MSG:
                            MailHandler.OnMailOptionResultMsg(msg);
                            break;
                        case (short)MessageType.SC_MAIL_UPDATE_MSG:
                            MailHandler.OnMailUpdateMsg(msg);
                            break;
                        case (short)MessageType.SC_MAIL_DETAIL_MSG:
                            MailHandler.OnMailDetailMsg(msg);
                            break;
                        case (short)MessageType.SC_MAIL_DELETE_MSG:
                            MailHandler.OnMailDeleteMsg(msg);
                            break;
                        case (short)MessageType.SC_GAME_NOTICE_LIST_MSG:
                            SystemHandler.OnNoticeList(msg);
                            break;
                        case (short)MessageType.SC_GAME_NOTICE_DETAIL_MSG:
                            SystemHandler.OnNoticeDetail(msg);
                            break;
                        case (short)MessageType.SC_SIGN_LOAD:
                            SystemHandler.OnSignLoad(msg);
                            break;
                        case (short)MessageType.SC_SIGN_SUCC:
                            SystemHandler.OnSignSuccess(msg);
                            break;
                        case (short)MessageType.SC_QUEST:
                            SystemHandler.OnQuestLoad(msg);
                            break;
                        case (short)MessageType.SC_QUEST_RECIEVE_REWARD_SUCC:
                            SystemHandler.OnQuestReward(msg);
                            break;
                        case (short)MessageType.SC_QUEST_FINISH:
                            SystemHandler.OnQuestFinish(msg);
                            break;
                    }
                    msg = NetManager.GetMessage();
                }
            }
            catch (Exception e)
            {
                var errorLog = string.Format("Message Parse Error.\nMessage:\n{1}\nStackTrace:\n{0}", e.StackTrace,
                    e.Message);
                //PopTextManager.PopTip(errorLog);
                Debug.LogError(errorLog);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator AppliationChecker()
    {
        // performance tune, only check every frame for andriod.
        if (Application.platform == RuntimePlatform.Android)
        {
            while (true)
            {
                // return key.
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (Time.realtimeSinceStartup - lastTimeSinceStart < ApplicationQuitTime)
                    {
                        Application.Quit();
                    }
                    else
                    {
                        lastTimeSinceStart = Time.realtimeSinceStartup;
                        PopTextManager.PopTip("再按一次退出游戏", false);
                    }
                }

                // Home key.
                if (Input.GetKeyDown(KeyCode.Home))
                {
                    Application.Quit();
                }

                // break one frame.
                yield return null;
            }
        }
    }

    private void Start()
    {
        StartCoroutine(MessageHandler());
        StartCoroutine(AppliationChecker());
    }
}

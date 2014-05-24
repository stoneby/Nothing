/**
 * Autogenerated by CodeGenerator
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
 
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift.Protocol;

namespace KXSGCodec
{
    /// <summary>
    /// sc message type map util
    /// </summary>
    public class SCMessageHelper
    {
        public static TBase createMessage(short type)
        {
        	            if (type == MessageType.SC_BATTLE_PVE_START_MSG.GetHashCode())
            {
                return new SCBattlePveStartMsg();
            }
			            if (type == MessageType.SC_HERO_CREATE_ONE.GetHashCode())
            {
                return new SCHeroCreateOne();
            }
			            if (type == MessageType.SC_HERO_LIST.GetHashCode())
            {
                return new SCHeroList();
            }
			            if (type == MessageType.SC_HERO_LVL_UP.GetHashCode())
            {
                return new SCHeroLvlUp();
            }
			            if (type == MessageType.SC_HERO_MAX_EXTEND.GetHashCode())
            {
                return new SCHeroMaxExtend();
            }
			            if (type == MessageType.SC_HERO_MODIFY_TEAM.GetHashCode())
            {
                return new SCHeroModifyTeam();
            }
			            if (type == MessageType.SC_HERO_SELL.GetHashCode())
            {
                return new SCHeroSell();
            }
			            if (type == MessageType.SC_ADD_ITEM.GetHashCode())
            {
                return new SCAddItem();
            }
			            if (type == MessageType.SC_ALL_ITEM_INFOS.GetHashCode())
            {
                return new SCAllItemInfos();
            }
			            if (type == MessageType.SC_DELETE_ITEMS.GetHashCode())
            {
                return new SCDeleteItems();
            }
			            if (type == MessageType.SC_EVOLUTE_ITEM_SUCC.GetHashCode())
            {
                return new SCEvoluteItemSucc();
            }
			            if (type == MessageType.SC_EXTEND_ITEM_BAG_SUCC.GetHashCode())
            {
                return new SCExtendItemBagSucc();
            }
			            if (type == MessageType.SC_ITEM_DETAIL.GetHashCode())
            {
                return new SCItemDetail();
            }
			            if (type == MessageType.SC_STRENGTH_ITEM_SUCC.GetHashCode())
            {
                return new SCStrengthItemSucc();
            }
			            if (type == MessageType.SC_CREATE_PLAYER_MSG.GetHashCode())
            {
                return new SCCreatePlayerMsg();
            }
			            if (type == MessageType.SC_PLAYER_INFO_MSG.GetHashCode())
            {
                return new SCPlayerInfoMsg();
            }
			            if (type == MessageType.SC_SERVER_LIST_MSG.GetHashCode())
            {
                return new SCServerListMsg();
            }
			            if (type == MessageType.SC_PROPERTY_CHANGED_NUMBER.GetHashCode())
            {
                return new SCPropertyChangedNumber();
            }
			            if (type == MessageType.SC_PROPERTY_CHANGED_STRING.GetHashCode())
            {
                return new SCPropertyChangedString();
            }
			            if (type == MessageType.SC_RAID_ADDTION.GetHashCode())
            {
                return new SCRaidAddtion();
            }
			            if (type == MessageType.SC_RAID_CLEAR_DAILY_TIMES.GetHashCode())
            {
                return new SCRaidClearDailyTimes();
            }
			            if (type == MessageType.SC_RAID_ENTER_FAIL.GetHashCode())
            {
                return new SCRaidEnterFail();
            }
			            if (type == MessageType.SC_RAID_LOADING_ALL.GetHashCode())
            {
                return new SCRaidLoadingAll();
            }
			            if (type == MessageType.SC_RAID_NEW_STAGE.GetHashCode())
            {
                return new SCRaidNewStage();
            }
			            if (type == MessageType.SC_RAID_QUERY_FRIEND.GetHashCode())
            {
                return new SCRaidQueryFriend();
            }
			            if (type == MessageType.SC_RAID_RECEIVE_AWARDS.GetHashCode())
            {
                return new SCRaidReceiveAwards();
            }
			            if (type == MessageType.SC_RAID_REWARD.GetHashCode())
            {
                return new SCRaidReward();
            }
			            if (type == MessageType.SC_ERROR_INFO_MSG.GetHashCode())
            {
                return new SCErrorInfoMsg();
            }
			            if (type == MessageType.SC_SYSTEM_INFO_MSG.GetHashCode())
            {
                return new SCSystemInfoMsg();
            }
			 			Logger.LogError("Unknown sc msg type: " + type.ToString());
	        return null;

        }
    }
}
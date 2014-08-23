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

namespace KXSGCodec
{
    /// <summary>
    /// cs message type map util
    /// </summary>
    public class CSMessageHelper
    {
        private static Dictionary<Type, short> MSG_TYPE_DICT = new Dictionary<Type,short>();
        private static CSMessageHelper INSTANCE = new CSMessageHelper();

        private  CSMessageHelper() 
        {
        	            MSG_TYPE_DICT[typeof(CSBattlePveFinishMsg)] = (short)MessageType.CS_BATTLE_PVE_FINISH_MSG.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSRaidBattleStartMsg)] = (short)MessageType.CS_RAID_BATTLE_START_MSG.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSFriendApply)] = (short)MessageType.CS_FRIEND_APPLY.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSFriendApplyList)] = (short)MessageType.CS_FRIEND_APPLY_LIST.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSFriendApplyOper)] = (short)MessageType.CS_FRIEND_APPLY_OPER.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSFriendBind)] = (short)MessageType.CS_FRIEND_BIND.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSFriendDelete)] = (short)MessageType.CS_FRIEND_DELETE.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSFriendExtend)] = (short)MessageType.CS_FRIEND_EXTEND.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSFriendGiveEnergy)] = (short)MessageType.CS_FRIEND_GIVE_ENERGY.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSFriendLoadingAll)] = (short)MessageType.CS_FRIEND_LOADING_ALL.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSFriendQueryByName)] = (short)MessageType.CS_FRIEND_QUERY_BY_NAME.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSFriendRecieveEnergy)] = (short)MessageType.CS_FRIEND_RECIEVE_ENERGY.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSFriendRecieveEnergyList)] = (short)MessageType.CS_FRIEND_RECIEVE_ENERGY_LIST.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSHeroBind)] = (short)MessageType.CS_HERO_BIND.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSHeroChangeEquip)] = (short)MessageType.CS_HERO_CHANGE_EQUIP.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSHeroChangeOrder)] = (short)MessageType.CS_HERO_CHANGE_ORDER.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSHeroChangeTeam)] = (short)MessageType.CS_HERO_CHANGE_TEAM.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSHeroCreateOne)] = (short)MessageType.CS_HERO_CREATE_ONE.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSHeroList)] = (short)MessageType.CS_HERO_LIST.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSHeroLvlUp)] = (short)MessageType.CS_HERO_LVL_UP.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSHeroMaxExtend)] = (short)MessageType.CS_HERO_MAX_EXTEND.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSHeroModifyTeam)] = (short)MessageType.CS_HERO_MODIFY_TEAM.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSHeroSell)] = (short)MessageType.CS_HERO_SELL.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSBuyBackItems)] = (short)MessageType.CS_BUY_BACK_ITEMS.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSEvoluteItem)] = (short)MessageType.CS_EVOLUTE_ITEM.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSExtendItemBag)] = (short)MessageType.CS_EXTEND_ITEM_BAG.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSItemLockOper)] = (short)MessageType.CS_ITEM_LOCK_OPER.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSQueryAllItems)] = (short)MessageType.CS_QUERY_ALL_ITEMS.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSQueryItemDetail)] = (short)MessageType.CS_QUERY_ITEM_DETAIL.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSSellItems)] = (short)MessageType.CS_SELL_ITEMS.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSStrengthItem)] = (short)MessageType.CS_STRENGTH_ITEM.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSCreatePlayerMsg)] = (short)MessageType.CS_CREATE_PLAYER_MSG.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSPasswdLoginMsg)] = (short)MessageType.CS_PASSWD_LOGIN_MSG.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSQuickLoginMsg)] = (short)MessageType.CS_QUICK_LOGIN_MSG.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSRandomCharNameMsg)] = (short)MessageType.CS_RANDOM_CHAR_NAME_MSG.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSTokenLoginMsg)] = (short)MessageType.CS_TOKEN_LOGIN_MSG.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSLottery)] = (short)MessageType.CS_LOTTERY.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSLotteryCompose)] = (short)MessageType.CS_LOTTERY_COMPOSE.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSLotteryComposeList)] = (short)MessageType.CS_LOTTERY_COMPOSE_LIST.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSLotteryList)] = (short)MessageType.CS_LOTTERY_LIST.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSBuyEnergy)] = (short)MessageType.CS_BUY_ENERGY.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSRaidLoadingAll)] = (short)MessageType.CS_RAID_LOADING_ALL.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSRaidQueryFriend)] = (short)MessageType.CS_RAID_QUERY_FRIEND.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSRaidReceiveAwards)] = (short)MessageType.CS_RAID_RECEIVE_AWARDS.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSGetRechargeIdMsg)] = (short)MessageType.CS_GET_RECHARGE_ID_MSG.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSRefreshRechargeMsg)] = (short)MessageType.CS_REFRESH_RECHARGE_MSG.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSDebugCmdMsg)] = (short)MessageType.CS_DEBUG_CMD_MSG.GetHashCode();
			        }

        public static short getMessageType(Type msg)
        {
            return MSG_TYPE_DICT[msg];
        }
    }
}

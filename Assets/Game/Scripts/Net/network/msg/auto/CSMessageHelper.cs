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
        	            MSG_TYPE_DICT[typeof(CSBattleFinishMsg)] = (short)MessageType.CS_BATTLE_FINISH_MSG.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSRaidBattleStartMsg)] = (short)MessageType.CS_RAID_BATTLE_START.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSBagExtend)] = (short)MessageType.CS_BAG_EXTEND.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSHeroChangeOrder)] = (short)MessageType.CS_HERO_CHANGE_ORDER.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSHeroChangeTeam)] = (short)MessageType.CS_HERO_CHANGE_TEAM.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSHeroList)] = (short)MessageType.CS_HERO_LIST.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSHeroLvlUp)] = (short)MessageType.CS_HERO_LVL_UP.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSHeroModifyTeam)] = (short)MessageType.CS_HERO_MODIFY_TEAM.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSHeroSell)] = (short)MessageType.CS_HERO_SELL.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSCreatePlayerMsg)] = (short)MessageType.CS_CREATE_PLAYER_MSG.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSPasswdLoginMsg)] = (short)MessageType.CS_PASSWD_LOGIN_MSG.GetHashCode();
			            MSG_TYPE_DICT[typeof(CSQuickLoginMsg)] = (short)MessageType.CS_QUICK_LOGIN_MSG.GetHashCode();
			        }

        public static short getMessageType(Type msg)
        {
            return MSG_TYPE_DICT[msg];
        }
    }
}
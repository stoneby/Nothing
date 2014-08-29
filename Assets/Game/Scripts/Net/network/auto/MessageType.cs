/**
 * Autogenerated by Thrift Compiler (0.9.1)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */

namespace KXSGCodec
{
  public enum MessageType
  {
    CS_BEGIN = 100,
    CS_PASSWD_LOGIN_MSG = 101,
    CS_QUICK_LOGIN_MSG = 102,
    CS_TOKEN_LOGIN_MSG = 103,
    CS_CREATE_PLAYER_MSG = 104,
    CS_HERO_LIST = 105,
    CS_HERO_MAX_EXTEND = 106,
    CS_HERO_LVL_UP = 107,
    CS_HERO_MODIFY_TEAM = 108,
    CS_HERO_CHANGE_TEAM = 109,
    CS_HERO_SELL = 110,
    CS_HERO_CHANGE_ORDER = 111,
    CS_HERO_CREATE_ONE = 112,
    CS_RAID_BATTLE_START_MSG = 113,
    CS_BATTLE_PVE_FINISH_MSG = 114,
    CS_RAID_LOADING_ALL = 115,
    CS_RAID_QUERY_FRIEND = 116,
    CS_RAID_RECEIVE_AWARDS = 117,
    CS_QUERY_ALL_ITEMS = 118,
    CS_EXTEND_ITEM_BAG = 119,
    CS_QUERY_ITEM_DETAIL = 120,
    CS_STRENGTH_ITEM = 121,
    CS_EVOLUTE_ITEM = 122,
    CS_SELL_ITEMS = 123,
    CS_BUY_BACK_ITEMS = 124,
    CS_ITEM_LOCK_OPER = 125,
    CS_HERO_CHANGE_EQUIP = 126,
    CS_LOTTERY = 127,
    CS_LOTTERY_LIST = 128,
    CS_LOTTERY_COMPOSE = 129,
    CS_LOTTERY_COMPOSE_LIST = 130,
    CS_HERO_BIND = 131,
    CS_DEBUG_CMD_MSG = 132,
    CS_GET_RECHARGE_ID_MSG = 133,
    CS_REFRESH_RECHARGE_MSG = 134,
    CS_FRIEND_APPLY = 135,
    CS_FRIEND_APPLY_LIST = 136,
    CS_FRIEND_DELETE = 137,
    CS_FRIEND_LOADING_ALL = 138,
    CS_FRIEND_RECIEVE_ENERGY_LIST = 139,
    CS_FRIEND_APPLY_OPER = 140,
    CS_FRIEND_QUERY_BY_NAME = 141,
    CS_FRIEND_EXTEND = 142,
    CS_FRIEND_GIVE_ENERGY = 143,
    CS_FRIEND_RECIEVE_ENERGY = 144,
    CS_FRIEND_BIND = 145,
    CS_BUY_ENERGY = 146,
    CS_RANDOM_CHAR_NAME_MSG = 147,
    CS_END = 148,
    SC_BEGIN = 2000,
    SC_CREATE_PLAYER_MSG = 2001,
    SC_PLAYER_INFO_MSG = 2002,
    SC_SYSTEM_INFO_MSG = 2003,
    SC_ERROR_INFO_MSG = 2004,
    SC_SERVER_LIST_MSG = 2005,
    SC_HERO_LIST = 2006,
    SC_HERO_MAX_EXTEND = 2007,
    SC_HERO_LVL_UP = 2008,
    SC_HERO_MODIFY_TEAM = 2009,
    SC_HERO_SELL = 2010,
    SC_HERO_CREATE_ONE = 2011,
    SC_BATTLE_PVE_START_MSG = 2012,
    SC_PROPERTY_CHANGED_NUMBER = 2013,
    SC_PROPERTY_CHANGED_STRING = 2014,
    SC_RAID_ADDTION = 2015,
    SC_RAID_CLEAR_DAILY_TIMES = 2016,
    SC_RAID_LOADING_ALL = 2017,
    SC_RAID_NEW_STAGE = 2018,
    SC_RAID_QUERY_FRIEND = 2019,
    SC_RAID_REWARD = 2020,
    SC_RAID_ENTER_FAIL = 2021,
    SC_RAID_RECEIVE_AWARDS = 2022,
    SC_ALL_ITEM_INFOS = 2023,
    SC_EXTEND_ITEM_BAG_SUCC = 2024,
    SC_ITEM_DETAIL = 2025,
    SC_STRENGTH_ITEM_SUCC = 2026,
    SC_EVOLUTE_ITEM_SUCC = 2027,
    SC_DELETE_ITEMS = 2028,
    SC_ADD_ITEM = 2029,
    SC_ADD_ITEMS_AND_HEROS = 2030,
    SC_ITEM_LOCK_OPER_SUCC = 2031,
    SC_ITEM_SELL_SUCC = 2032,
    SC_BUY_BACK_ITEM_SUCC = 2033,
    SC_SERVER_CONFIG_MSG = 2034,
    SC_HERO_CHANGE_EQUIP = 2035,
    SC_LOTTERY = 2036,
    SC_LOTTERY_LIST = 2037,
    SC_LOTTERY_CANNOT_FREE = 2038,
    SC_LOTTERY_REFRESH_TIMES = 2039,
    SC_LOTTERY_COMPOSE_LIST = 2040,
    SC_LOTTERY_COMPOSE_SUCC = 2041,
    SC_HERO_BIND_SUCC = 2042,
    SC_RECHARGE_ID_MSG = 2043,
    SC_FRIEND_APPLY_LIST = 2044,
    SC_FRIEND_APPLY_SUCC = 2045,
    SC_FRIEND_DELETE_SUCC = 2046,
    SC_FRIEND_LOADING_ALL = 2047,
    SC_FRIEND_RECIEVE_ENERGY_LIST = 2048,
    SC_FRIEND_APPLY_OPER_SUCC = 2049,
    SC_FRIEND_QUERY_BY_NAME = 2050,
    SC_FRIEND_EXTEND_SUCC = 2051,
    SC_FRIEND_GIVE_ENERGY_SUCC = 2052,
    SC_FRIEND_RECIEVE_ENERGY_SUCC = 2053,
    SC_FRIEND_BIND_SUCC = 2054,
    SC_RAID_FINISH_ADD_FRIEND = 2055,
    SC_HERO_CHANGE_TEAM_SUCC = 2056,
    SC_ENERGY_NOT_ENOUGH = 2057,
    SC_RANDOM_CHAR_NAME_MSG = 2058,
    SC_HERO_FRIST_LOGIN_GIVE = 2059,
    SC_END = 2060,
  }
}

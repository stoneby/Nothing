namespace com.kx.sglm.gs.battle.share
{

	using PointDirection = com.kx.sglm.gs.battle.share.enums.PointDirection;
	using HeroArrLogicHelper = com.kx.sglm.gs.battle.share.helper.HeroArrLogicHelper;
	using RoleAProperty = com.kx.sglm.gs.hero.properties.RoleAProperty;

	/// <summary>
	/// 战斗逻辑相关常量。都是一些数值上的常量，没有Map中的Key<br>
	/// 查找相关key请参照<seealso cref="BattleKeyConstants"/>
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleConstants
	{

		/// <summary>
		/// 英雄队伍正方形宽度
		/// </summary>
		public const int HERO_FIGHT_SIZE = 3;
		/// <summary>
		/// 英雄队伍数组总长度
		/// </summary>
		public static readonly int HERO_BATTLE_ARR_LENGTH = HERO_FIGHT_SIZE * HERO_FIGHT_SIZE;

		/// <summary>
		/// 战斗内所有概率都是万分比
		/// </summary>
		public const float BATTLE_RATIO_BASE = 10000.0f;

		/// <summary>
		/// 战斗开启的最少阵营
		/// </summary>
		public const int BATTLE_START_LEAST_SIDE = 2;

		public const int MIDDLE_POINT_INDEX = 4;

		/// <summary>
		/// -1也要写常量，我真是个强迫症……
		/// </summary>
		public const int BATTLE_FIGHTER_NON_INDEX = -1;

		public static readonly float[] BATTLE_HERO_INDEX_RATIO_FLOTA = new float[] {1.0f, 1.2f, 1.5f, 1.9f, 2.4f, 3.0f, 3.7f, 4.5f, 9.0f};

		public static readonly PointDirection[] MID_POINT_DIRECTION = new PointDirection[] {PointDirection.UP, PointDirection.DOWN, PointDirection.LEFT, PointDirection.RIGHT};
		/// <summary>
		/// 攻击倍数加成数组
		/// </summary>
		public static int[] BATTLE_HERO_INDEX_RATIO = HeroArrLogicHelper.initHeroIndexIntRatio(BATTLE_HERO_INDEX_RATIO_FLOTA, BATTLE_RATIO_BASE);

		/// <summary>
		/// 特殊属性数组 </summary>
		public static readonly int[] BATTLE_MUTI_PROP_ARR = new int[] {RoleAProperty.DECRDAMAGE, RoleAProperty.INCRDAMAGE};

		/// <summary>
		/// 生成的可连接点数组
		/// </summary>
		public static readonly int[] POINT_CONNECT_ARR = HeroArrLogicHelper.creatHeroPointConnecter(HERO_BATTLE_ARR_LENGTH);

		/// <summary>
		/// 链接的最少点 </summary>
		public const int HERO_LEAST_POINT = 1;

		/// <summary>
		/// 玩家队伍的最少参与武将 </summary>
		public const int HERO_TEAM_LEAST_FIGHTER = 12;

		/// <summary>
		/// 怪物弱职业弱化倍数 </summary>
		public const int MONSTER_WEAK_RATIO = 2;

		/// <summary>
		/// 默认怪物目标index </summary>
		public const int MONSTER_DEFAULT_TARGET_INDEX = 0;

		/// <summary>
		/// 自身队伍武将 </summary>
		public const int FIGHTER_TYPE_HERO = 1;
		/// <summary>
		/// 填空型扎古 </summary>
		public const int FIGHTER_TYPE_GUEST = 2;
		/// <summary>
		/// 好友武将 </summary>
		public const int FIGHTER_TYPE_FREIND = 3;
		/// <summary>
		/// NPC武将 </summary>
		public const int FIGHTER_TYPE_NPC = 4;

		public const int TEST_TOTAL_SP = 50;
		/// <summary>
		/// 可以放队长技能的Index，第0,1,2,9位置的武将可以放技能 </summary>
		public const int FIGHTER_ACTIVE_SKILL_FLAG = 0x207;

		public const int FIGHTER_LEADER_SKILL_FLAG = 0x201;

		public const int FIGHTER_FIGHT_DEFAULT_COUNT = 1;

		public const float FIGHTER_FIGHT_DEFAULT_RATIO = 10000;

		public const int TARGET_GETTER_FOR_ENEMY_INDEX = 0;
		public const int TARGET_GETTER_FOR_FRIEND_INDEX = 1;
		/// <summary>
		/// buff的最大上线 </summary>
		public const int BUFF_MAX_SIZE = 8;
		/// <summary>
		/// debuff的最大上线 </summary>
		public const int DEBUFF_MAX_SIZE = 8;
		/// <summary>
		/// 所有BUFF的最大数值上线 </summary>
		public static readonly int BUFF_ALL_MAX_SIZE = BUFF_MAX_SIZE + DEBUFF_MAX_SIZE;
		/// <summary>
		/// 只有DEBUFF触发 </summary>
		public const int BUFF_FLAG = 0x01;
		/// <summary>
		/// 只有BUFF触发 </summary>
		public const int DEBUFF_FALG = 0x02;
		/// <summary>
		/// 全部BUFF都触发 </summary>
		public static readonly int BUFF_ALL_FALG = BUFF_FLAG | DEBUFF_FALG;
		/// <summary>
		/// 武将生存的最低血量，1 </summary>
		public const int FIGHTER_ALIVE_MIN_HP = 1;
		/// <summary>
		/// 可攻击的标识 </summary>
		public const int ATTACK_DIS_FLAG = 0X01;
		/// <summary>
		/// 可使用技能的标识 </summary>
		public const int SKILL_DIS_FLAG = 0X02;
		/// <summary>
		/// 攻击不为0 </summary>
		public const int ATTACK_ZERO_FLAG = 0X04;
		/// <summary>
		/// 攻击不为0 </summary>
		public const int SP_MAX_FALG = 0X08;
		/// <summary>
		/// 护盾 </summary>
		public const int MONSTER_SHIELD_FLAG = 0X10;
		/// <summary>
		/// 全部不可以动作的flag </summary>
		public const int DISABLE_FLAG = 0xffff;
		/// <summary>
		/// 最小攻击值为1，不破防时使用 </summary>
		public const float MIN_ATTACK = 1.0F;
		/// <summary>
		/// 等没有怪物护盾时的flag </summary>
		public const int MONSTER_SHIELD_NIL_VALUE = 0;
		/// <summary>
		/// 护盾提供的减伤比例 </summary>
		public const float MONSTER_SHIELD_DAMAGE_REDUCE = 2000;

	}

}
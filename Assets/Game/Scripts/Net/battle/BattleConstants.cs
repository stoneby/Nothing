namespace com.kx.sglm.gs.battle
{

	using HeroArrLogicHelper = com.kx.sglm.gs.battle.helper.HeroArrLogicHelper;

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

		/// <summary>
		/// -1也要写常量，我真是个强迫症……
		/// </summary>
		public const int BATTLE_FIGHTER_NON_INDEX = -1;

		/// <summary>
		/// 攻击倍数加成数组
		/// </summary>
		public static readonly int[] BATTLE_HERO_INDEX_RATIO = new int[] {10000, 12000, 15000, 19000, 24000, 30000, 37000, 45000, 90000};

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

	}

}
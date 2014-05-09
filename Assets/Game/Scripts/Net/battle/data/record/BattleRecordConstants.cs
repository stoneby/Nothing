namespace com.kx.sglm.gs.battle.share.data.record
{

	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;

	/// <summary>
	/// 战报常量
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleRecordConstants
	{

		/// <summary>
		/// 攻击 </summary>
		public const int SINGLE_ACTION_TYPE_ATTACK = 0;

		/// <summary>
		/// 回血 </summary>
		public const int SINGLE_ACTION_TYPE_RECOVER = 1;

		/// <summary>
		/// 防御 </summary>
		public const int SINGLE_ACTION_TYPE_DEFENCE = 2;

		/// <summary>
		/// 被回血【无动作 </summary>
		public const int SINGLE_ACTION_TYPE_RECOVERED = 3;

		/// <summary>
		/// SP攻击 </summary>
		public const int SINGLE_ACTION_TYPE_SP_ATTACK = 4;

		/// <summary>
		/// 目标SideAID </summary>
		public static readonly int TARGET_SIDE_A = BattleSideEnum.SIDEA.Index;

		/// <summary>
		/// 目标SideBID </summary>
		public static readonly int TARGET_SIDE_B = BattleSideEnum.SIDEB.Index;

		/// <summary>
		/// 场景结束 </summary>
		public const int BATTLE_SCENE_END = 0;

		/// <summary>
		/// 战斗结束 </summary>
		public const int BATTLE_ALL_END = 1;

		//////////////////////////////////////////////////////////////////////////
		//////////////////////////////key////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// 战报用基础ID </summary>
		private static int RECORD_PROP_KEY_BASE = 0;

		/// <summary>
		/// 用于<seealso cref="SingleActionRecord"/>当前血�? </summary>
		public static readonly int SINGLE_ACTION_PROP_HP = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// 用于<seealso cref="SingleActionRecord"/>当前攻击目标 </summary>
		public static readonly int SINGLE_ACTION_PROP_ATTACK_TARGET = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// 总血量值key </summary>
		public static readonly int BATTLE_HERO_TOTAL_HP = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// 总气力值key </summary>
		public static readonly int BATTLE_HERO_TOTAL_MP = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// 战斗结束胜利方ID </summary>
		public static readonly int BATTLE_END_WIN_SIDE = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// 英雄Mp </summary>
		public static readonly int BATTLE_HERO_PROP_MP = RECORD_PROP_KEY_BASE++;
	}

}
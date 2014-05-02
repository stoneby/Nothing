namespace com.kx.sglm.gs.battle.data.record
{

	using BattleSideEnum = com.kx.sglm.gs.battle.enums.BattleSideEnum;

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

		public static readonly int TARGET_SIDE_A = BattleSideEnum.SIDEA.Index;

		public static readonly int TARGET_SIDE_B = BattleSideEnum.SIDEB.Index;

		public const int BATTLE_SCENE_END = 0;

		public const int BATTLE_ALL_END = 1;

		/// <summary>
		/// 战报用基础ID
		/// </summary>
		private static int RECORD_PROP_KEY_BASE = 0;

		/// <summary>
		/// 用于<seealso cref="SingleActionRecord"/>当前血量
		/// </summary>
		public static readonly int SINGLE_ACTION_PROP_HP = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// 用于<seealso cref="SingleActionRecord"/>当前攻击目标
		/// </summary>
		public static readonly int SINGLE_ACTION_PROP_ATTACK_TARGET = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// 总血量值key
		/// </summary>
		public static readonly int BATTLE_HERO_TOTAL_HP = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// 总气力值key
		/// </summary>
		public static readonly int BATTLE_HERO_TOTAL_SP = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// 战斗结束胜利方ID </summary>
		public static readonly int BATTLE_END_WIN_SIDE = RECORD_PROP_KEY_BASE++;

	}

}
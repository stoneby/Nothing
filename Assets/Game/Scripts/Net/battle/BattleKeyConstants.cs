namespace com.kx.sglm.gs.battle.share
{

	/// <summary>
	/// 战斗逻辑内key常量
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleKeyConstants
	{

		private static int BATTLE_KEY_BASE = 0;

		/// <summary>
		/// 玩家队伍选择目标Index </summary>
		public static readonly int BATTLE_KEY_HERO_TEAM_TARGET = BATTLE_KEY_BASE++;

		/// <summary>
		/// 怪物弱化职业 </summary>
		public static readonly int BATTLE_KEY_MONSTER_WEEK_JOB = BATTLE_KEY_BASE++;

		/// <summary>
		/// 副本ID </summary>
		public static readonly int BATTLE_PROP_RAID_ID = BATTLE_KEY_BASE++;

		/// <summary>
		/// 武将模板Id </summary>
		public static readonly int BATTLE_KEY_HERO_TEMPLATE = BATTLE_KEY_BASE++;

		/// <summary>
		/// 怪物所在场�? </summary>
		public static readonly int BATTLE_KEY_MONSTER_SCENE = BATTLE_KEY_BASE++;

		public static readonly int BATTLE_KEY_HERO_TYPE = BATTLE_KEY_BASE++;

		public static readonly int BATTLE_PROP_MONSTER_GROUP = BATTLE_KEY_BASE++;

		public static readonly int BATTLE_PROP_MONSTER_DROP_ITEM = BATTLE_KEY_BASE++;

		public static readonly int BATTLE_PROP_MONSTER_DROP_HERO = BATTLE_KEY_BASE++;

		public static readonly int BATTLE_PROP_MONSTER_DROP_COIN = BATTLE_KEY_BASE++;

		public static readonly int BATTLE_PROP_MONSTER_DROP_SPRIT = BATTLE_KEY_BASE++;

		public static readonly int BATTLE_PROP_MONSTER_DEFAULT_CD = BATTLE_KEY_BASE++;

		public static readonly int BATTLE_PROP_MONSTER_AI_ID = BATTLE_KEY_BASE++;

		////////////////////////////////战斗状态key//////////////////////////////////////
		private static sbyte BATTLE_STATE_KEY_BASE = 0;

		public static readonly sbyte BATTLE_STATE_MONSTER_SKILL_ROUND = BATTLE_STATE_KEY_BASE++;

	}

}
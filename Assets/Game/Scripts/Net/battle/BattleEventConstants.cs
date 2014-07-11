namespace com.kx.sglm.gs.battle.share
{

	public class BattleEventConstants
	{

		private static int BATTLE_EVENT_BASE = 0;

		/// <summary>
		/// 战斗场景开始 </summary>
		public static readonly int BATTLE_SCENE_START = BATTLE_EVENT_BASE++;
		/// <summary>
		/// 队伍出手开始 </summary>
		public static readonly int BATTLE_TEAM_SHOT_START = BATTLE_EVENT_BASE++;
		/// <summary>
		/// 队伍挂起时【即主动技能】 </summary>
		public static readonly int BATTLE_HUNG_UP = BATTLE_EVENT_BASE++;
		/// <summary>
		/// 攻击前，收到攻击信息但计算攻击数值前 </summary>
		public static readonly int BATTLE_BEFORE_FIGHTER_ATTACK = BATTLE_EVENT_BASE++;
		/// <summary>
		/// 攻击技能 </summary>
		public static readonly int BATTLE_FIGHTER_ATTACK = BATTLE_EVENT_BASE++;

		/// <summary>
		/// 回合结束 </summary>
		public static readonly int BATTLE_ROUND_FINISH = BATTLE_EVENT_BASE++;

		/// <summary>
		/// 场景结束 </summary>
		public static readonly int BATTLE_SCENE_FINISH = BATTLE_EVENT_BASE++;

		public static readonly int SIZE = BATTLE_EVENT_BASE;

	}

}
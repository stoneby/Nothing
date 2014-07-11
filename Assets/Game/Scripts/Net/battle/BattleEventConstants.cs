namespace com.kx.sglm.gs.battle.share
{

	public class BattleEventConstants
	{

		private static int BATTLE_EVENT_BASE = 0;

		/// <summary>
		/// ս��������ʼ </summary>
		public static readonly int BATTLE_SCENE_START = BATTLE_EVENT_BASE++;
		/// <summary>
		/// ������ֿ�ʼ </summary>
		public static readonly int BATTLE_TEAM_SHOT_START = BATTLE_EVENT_BASE++;
		/// <summary>
		/// �������ʱ�����������ܡ� </summary>
		public static readonly int BATTLE_HUNG_UP = BATTLE_EVENT_BASE++;
		/// <summary>
		/// ����ǰ���յ�������Ϣ�����㹥����ֵǰ </summary>
		public static readonly int BATTLE_BEFORE_FIGHTER_ATTACK = BATTLE_EVENT_BASE++;
		/// <summary>
		/// �������� </summary>
		public static readonly int BATTLE_FIGHTER_ATTACK = BATTLE_EVENT_BASE++;

		/// <summary>
		/// �غϽ��� </summary>
		public static readonly int BATTLE_ROUND_FINISH = BATTLE_EVENT_BASE++;

		/// <summary>
		/// �������� </summary>
		public static readonly int BATTLE_SCENE_FINISH = BATTLE_EVENT_BASE++;

		public static readonly int SIZE = BATTLE_EVENT_BASE;

	}

}
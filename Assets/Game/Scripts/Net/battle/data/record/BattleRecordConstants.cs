namespace com.kx.sglm.gs.battle.share.data.record
{

	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;

	/// <summary>
	/// ս������
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleRecordConstants
	{

		/// <summary>
		/// ���� </summary>
		public const int SINGLE_ACTION_TYPE_ATTACK = 0;

		/// <summary>
		/// ��Ѫ </summary>
		public const int SINGLE_ACTION_TYPE_RECOVER = 1;

		/// <summary>
		/// ���� </summary>
		public const int SINGLE_ACTION_TYPE_DEFENCE = 2;

		/// <summary>
		/// ����Ѫ���޶��� </summary>
		public const int SINGLE_ACTION_TYPE_RECOVERED = 3;

		/// <summary>
		/// SP���� </summary>
		public const int SINGLE_ACTION_TYPE_SP_ATTACK = 4;

		/// <summary>
		/// תɫ </summary>
		public const int SINGLE_ACTION_TYPE_CHANGE_COLOR = 5;

		/// <summary>
		/// ��תɫ </summary>
		public const int SINGLE_ACTION_TYPE_CHANGED_COLOR = 6;

		/// <summary>
		/// ��תɫ </summary>
		public const int SINGLE_ACTION_TYPE_BUFF_STATE = 7;

		/// <summary>
		/// Ŀ��SideAID </summary>
		public static readonly int TARGET_SIDE_LEFT = BattleSideEnum.SIDE_LEFT.Index;

		/// <summary>
		/// Ŀ��SideBID </summary>
		public static readonly int TARGET_SIDE_RIGHT = BattleSideEnum.SIDEB_RIGHT.Index;

		/// <summary>
		/// �������� </summary>
		public const int BATTLE_SCENE_END = 0;

		/// <summary>
		/// ս������ </summary>
		public const int BATTLE_ALL_END = 1;

		// ////////////////////////////////////////////////////////////////////////
		// ////////////////////////////key////////////////////////////////////////
		// //////////////////////////////////////////////////////////////////////

		/// <summary>
		/// ս���û���ID </summary>
		private static int RECORD_PROP_KEY_BASE = 0;

		/// <summary>
		/// ����<seealso cref="SingleActionRecord"/>��ǰѪ�� </summary>
		public static readonly int SINGLE_ACTION_PROP_HP = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// ����<seealso cref="SingleActionRecord"/>��ǰ����Ŀ�� </summary>
		public static readonly int SINGLE_ACTION_PROP_ATTACK_TARGET = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// ��Ѫ��ֵkey </summary>
		public static readonly int BATTLE_HERO_TOTAL_HP = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// ������ֵkey </summary>
		public static readonly int BATTLE_HERO_TOTAL_MP = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// ս������ʤ����ID </summary>
		public static readonly int BATTLE_END_WIN_SIDE = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// Ӣ��Mp </summary>
		public static readonly int BATTLE_HERO_PROP_MP = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// Sp���ܹ������� </summary>
		public static readonly int BATTLE_HERO_PROP_HIT_COUNT = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// Sp���ܹ��������˺� </summary>
		public static readonly int BATTLE_HERO_PROP_HIT_SINGLE_DAMAGE = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// Sp���ܹ��������˺� </summary>
		public static readonly int BATTLE_HERO_PROP_COLOR_CHANGE = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// ���＼��CD </summary>
		public static readonly int BATTLE_MONSTER_SKILL_ROUND = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// Fighter״̬λ </summary>
		public static readonly int BATTLE_FIGHTER_STATE_FLAG = RECORD_PROP_KEY_BASE++;

		/// <summary>
		/// ս������ </summary>
		private static int BATTLE_ERROR_RECORD_BASE = 0;
		/// <summary>
		/// Ӣ��������ɫУ�鲻ͨ�� </summary>
		public static readonly int BATTLE_ERROR_HERO_COLER_INDEX = BATTLE_ERROR_RECORD_BASE++;
		/// <summary>
		/// Ŀ����� </summary>
		public static readonly int BATTLE_ERROR_TARGET = BATTLE_ERROR_RECORD_BASE++;
	}






























}
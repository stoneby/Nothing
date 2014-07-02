namespace com.kx.sglm.gs.battle.share
{

	using HeroArrLogicHelper = com.kx.sglm.gs.battle.share.helper.HeroArrLogicHelper;

	/// <summary>
	/// ս���߼���س���������һЩ��ֵ�ϵĳ�����û��Map�е�Key<br>
	/// �������key�����<seealso cref="BattleKeyConstants"/>
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleConstants
	{

		/// <summary>
		/// Ӣ�۶��������ο��
		/// </summary>
		public const int HERO_FIGHT_SIZE = 3;
		/// <summary>
		/// Ӣ�۶��������ܳ���
		/// </summary>
		public static readonly int HERO_BATTLE_ARR_LENGTH = HERO_FIGHT_SIZE * HERO_FIGHT_SIZE;

		/// <summary>
		/// ս�������и��ʶ�����ֱ�
		/// </summary>
		public const float BATTLE_RATIO_BASE = 10000.0f;

		/// <summary>
		/// ս��������������Ӫ
		/// </summary>
		public const int BATTLE_START_LEAST_SIDE = 2;

		/// <summary>
		/// -1ҲҪд�����������Ǹ�ǿ��֢����
		/// </summary>
		public const int BATTLE_FIGHTER_NON_INDEX = -1;

		/// <summary>
		/// ���������ӳ�����
		/// </summary>
		public static readonly int[] BATTLE_HERO_INDEX_RATIO = new int[] {10000, 12000, 15000, 19000, 24000, 30000, 37000, 45000, 90000};

		/// <summary>
		/// ���ɵĿ����ӵ�����
		/// </summary>
		public static readonly int[] POINT_CONNECT_ARR = HeroArrLogicHelper.creatHeroPointConnecter(HERO_BATTLE_ARR_LENGTH);

		/// <summary>
		/// ���ӵ����ٵ� </summary>
		public const int HERO_LEAST_POINT = 1;

		/// <summary>
		/// ��Ҷ�������ٲ����佫 </summary>
		public const int HERO_TEAM_LEAST_FIGHTER = 12;

		/// <summary>
		/// ������ְҵ�������� </summary>
		public const int MONSTER_WEAK_RATIO = 2;

		/// <summary>
		/// Ĭ�Ϲ���Ŀ��index </summary>
		public const int MONSTER_DEFAULT_TARGET_INDEX = 0;

		/// <summary>
		/// ��������佫 </summary>
		public const int FIGHTER_TYPE_HERO = 1;
		/// <summary>
		/// ��������� </summary>
		public const int FIGHTER_TYPE_GUEST = 2;
		/// <summary>
		/// �����佫 </summary>
		public const int FIGHTER_TYPE_FREIND = 3;
		/// <summary>
		/// NPC�佫 </summary>
		public const int FIGHTER_TYPE_NPC = 4;

		public const int TEST_TOTAL_SP = 50;
		/// <summary>
		/// ���ԷŶӳ����ܵ�Index����0,1,2,9λ�õ��佫���Էż��� </summary>
		public const int FIGHTER_ACTIVE_SKILL_FLAG = 0x207;

		public const int FIGHTER_FIGHT_DEFAULT_COUNT = 1;

		public const float FIGHTER_FIGHT_DEFAULT_RATIO = 1;

		public const int TARGET_GETTER_FOR_ENEMY_INDEX = 0;

		public const int TARGET_GETTER_FOR_FRIEND_INDEX = 1;

		public const int BUFF_MAX_SIZE = 8;

		public const int DEBUFF_MAX_SIZE = 8;

		public static readonly int BUFF_ALL_MAX_SIZE = BUFF_MAX_SIZE + DEBUFF_MAX_SIZE;

		public const int BUFF_FLAG = 0x01;

		public const int DEBUFF_FALG = 0x02;

		public static readonly int BUFF_ALL_FALG = BUFF_FLAG | DEBUFF_FALG;

		public const int FIGHTER_ALIVE_MIN_HP = 1;
		/// <summary>
		/// �ɹ����ı�ʶ </summary>
		public const int ATTACK_DIS_FLAG = 0X01;
		/// <summary>
		/// ��ʹ�ü��ܵı�ʶ </summary>
		public const int SKILL_DIS_FLAG = 0X02;
		/// <summary>
		/// ������Ϊ0 </summary>
		public const int ATTACK_ZERO_FLAG = 0X04;

		public const int DISABLE_FLAG = 0xffff;
	}


}